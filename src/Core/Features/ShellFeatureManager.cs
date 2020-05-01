﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PlatoCore.Features.Abstractions;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Models.Features;
using PlatoCore.Models.Shell;
using PlatoCore.Shell.Abstractions;
using PlatoCore.Stores.Abstractions.Shell;

namespace PlatoCore.Features
{

    // Feature event handlers are executed in a temporary shell context 
    // This is necessary as the feature may not be enabled and as 
    // such the event handlers for the feature won't be registered with DI
    // For example we can't invoke the Installing or Installed events within
    // the main context as the feature is currently disabled within this context
    // so the IFeatureEventHandler provider for the feature has not been registered within DI.
    // ShellFeatureManager instead creates a temporary context consisting of a shell descriptor
    // with the features we want to enable or disable. IFeatureEventHandler can then be registered
    // within DI for the features we are enabling or disabling and the events can be invoked.

    public class ShellFeatureManager : IShellFeatureManager
    { 
        
        private readonly IShellDescriptorManager _shellDescriptorManager;
        private readonly IShellDescriptorStore _shellDescriptorStore;
        private readonly IShellContextFactory _shellContextFactory;      
        private readonly ILogger<ShellFeatureManager> _logger;
        private readonly IShellSettings _shellSettings;
        private readonly IPlatoHost _platoHost;
        private readonly IBroker _broker;

        public ShellFeatureManager(            
            IShellDescriptorManager shellDescriptorManager,
            IShellDescriptorStore shellDescriptorStore,
            IShellContextFactory shellContextFactory,         
            ILogger<ShellFeatureManager> logger,
            IShellSettings shellSettings,
            IPlatoHost platoHost,
            IBroker broker)
        {
            _shellDescriptorManager = shellDescriptorManager;
            _shellDescriptorStore = shellDescriptorStore;      
            _shellContextFactory = shellContextFactory;      
            _shellSettings = shellSettings;
            _platoHost = platoHost;
            _logger = logger;
            _broker = broker;
        }

        public async Task<IEnumerable<IFeatureEventContext>> EnableFeatureAsync(string featureId)
        {

            // Get features to enable
            var feature = await _shellDescriptorManager.GetFeatureAsync(featureId);

            // Ensure we also enable dependencies
            var featureIds = feature.FeatureDependencies
                .Select(d => d.ModuleId).ToArray();
            
            // Enable features
            return await EnableFeaturesAsync(featureIds);

        }

        public async Task<IEnumerable<IFeatureEventContext>> DisableFeatureAsync(string featureId)
        {

            // Get feature to disable
            var feature = await _shellDescriptorManager.GetFeatureAsync(featureId);

            // Ensure we also disable dependents
            var featureIds = feature.DependentFeatures
                .Select(d => d.ModuleId).ToArray();
            
            // Disable features
          return await DisableFeaturesAsync(featureIds);

        }
        
        public async Task<IEnumerable<IFeatureEventContext>> EnableFeaturesAsync(string[] featureIds)
        {

            // Get distinct Ids
            var ids = featureIds.Distinct().ToArray();

            // Get features to enable
            var features = await _shellDescriptorManager.GetFeaturesAsync(ids);
            var featuresToInvoke = features.Distinct().ToList();
            
            // Raise installing events for features
            var results = await InvokeFeatureEventsAsync(featuresToInvoke,
                async (context, handler) =>
                {

                    // Return if feature is already enabled, no need to enable
                    if (context.Feature.IsEnabled)
                    {
                        return null;
                    }

                    var contexts = new ConcurrentDictionary<string, IFeatureEventContext>();

                    try
                    {

                        // Invoke FeatureInstalling subscriptions
                        foreach (var brokerHandler in _broker.Pub<IShellFeature>(this, "FeatureInstalling"))
                        {
                            context.Feature = await brokerHandler.Invoke(new Message<IShellFeature>(context.Feature, this));
                        }

                        // Invoke feature event handler
                        await handler.InstallingAsync(context);

                        contexts.AddOrUpdate(context.Feature.ModuleId, context, (k, v) =>
                        {
                            foreach (var error in context.Errors)
                            {
                                v.Errors.Add(error.Key, error.Value);
                            }

                            return v;
                        });

                    }
                    catch (Exception e)
                    {
                        contexts.AddOrUpdate(context.Feature.ModuleId, context, (k, v) =>
                        {
                            v.Errors.Add(context.Feature.ModuleId, e.Message);
                            return v;
                        });
                    }

                    // Did any event encounter errors?
                    var hasErrors = contexts
                        .Where(c => c.Value.Errors.Any());

                    // No errors update descriptor, raise InstalledAsync and recycle ShellContext
                    if (!hasErrors.Any())
                    {

                        // Update descriptor within database
                        var descriptor = await AddFeaturesAndSaveAsync(featureIds);

                        try
                        {

                            // Invoke FeatureInstalled subscriptions
                            foreach (var brokerHandler in _broker.Pub<IShellFeature>(this, "FeatureInstalled"))
                            {
                                context.Feature = await brokerHandler.Invoke(new Message<IShellFeature>(context.Feature, this));
                            }

                            await handler.InstalledAsync(context);
                            contexts.AddOrUpdate(context.Feature.ModuleId, context, (k, v) =>
                            {
                                foreach (var error in context.Errors)
                                {
                                    v.Errors.Add(error.Key, error.Value);
                                }

                                return v;
                            });
                        }
                        catch (Exception e)
                        {
                            contexts.AddOrUpdate(context.Feature.ModuleId, context, (k, v) =>
                            {
                                v.Errors.Add(context.Feature.ModuleId, e.Message);
                                return v;
                            });
                        }

                    }

                    return contexts;
                    
                }, 
                async context =>
                {

                    // Return if feature is already enabled, no need to enable
                    if (context.Feature.IsEnabled)
                    {
                        return null;
                    }

                    // Invoke FeatureInstalled subscriptions
                    foreach (var brokerHandler in _broker.Pub<IShellFeature>(this, "FeatureInstalled"))
                    {
                        context.Feature = await brokerHandler.Invoke(new Message<IShellFeature>(context.Feature, this));
                    }

                    var contexts = new ConcurrentDictionary<string, IFeatureEventContext>();
                    contexts.AddOrUpdate(context.Feature.ModuleId, context, (k, v) =>
                    {
                        foreach (var error in context.Errors)
                        {
                            v.Errors.Add(error.Key, error.Value);
                        }

                        return v;
                    });
                    return contexts;
                    
                });
            
            // Did any event encounter errors?
            var errors = results
                .Where(c => c.Value.Errors.Any());

            // No errors update descriptor, raise InstalledAsync and recycle ShellContext
            if (!errors.Any())
            {
                // Update descriptor within database
                await AddFeaturesAndSaveAsync(featureIds);                
            }
            
            // dispose current shell context
            RecycleShell();

            // Return all execution contexts
            return results.Values;

        }
        
        public async Task<IEnumerable<IFeatureEventContext>> DisableFeaturesAsync(string[] featureIds)
        {

            // Get distinct Ids
            var ids = featureIds.Distinct().ToArray();

            // Get features to disable
            var features = await _shellDescriptorManager.GetFeaturesAsync(ids);
            var featuresToInvoke = features.Distinct().ToList();
            
            // Raise Uninstalling events
            var results = await InvokeFeatureEventsAsync(featuresToInvoke,
                async (context, handler) =>
                {

                    // Return if feature is already disabled - no need to disable
                    if (!context.Feature.IsEnabled)
                    {
                        return null;
                    }

                    var contexts = new ConcurrentDictionary<string, IFeatureEventContext>();

                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation($"{context.Feature.ModuleId} InstallingAsync Event Raised");
                    }

                    try
                    {

                        // Invoke FeatureUninstalling subscriptions
                        foreach (var brokerHandler in _broker.Pub<IShellFeature>(this, "FeatureUninstalling"))
                        {
                            context.Feature = await brokerHandler.Invoke(new Message<IShellFeature>(context.Feature, this));
                        }

                        await handler.UninstallingAsync(context);
                        contexts.AddOrUpdate(context.Feature.ModuleId, context, (k, v) =>
                        {
                            foreach (var error in context.Errors)
                            {
                                v.Errors.Add(error.Key, error.Value);
                            }

                            return v;
                        });
                    }
                    catch (Exception e)
                    {
                        contexts.AddOrUpdate(context.Feature.ModuleId, context, (k, v) =>
                        {
                            v.Errors.Add(context.Feature.ModuleId, e.Message);
                            return v;
                        });
                    }


                    // Did any event encounter errors?
                    var hasErrors = contexts
                        .Where(c => c.Value.Errors.Any())
                        .ToList();

                    // No errors update descriptor, raise InstalledAsync and recycle ShellContext
                    if (!hasErrors.Any())
                    {

                        // Update descriptor within database
                        await RemoveFeaturesAndSaveAsync(featureIds);

                        try
                        {

                            // Invoke FeatureUninstalled subscriptions
                            foreach (var brokerHandler in _broker.Pub<IShellFeature>(this, "FeatureUninstalled"))
                            {
                                context.Feature = await brokerHandler.Invoke(new Message<IShellFeature>(context.Feature, this));
                            }

                            await handler.UninstalledAsync(context);
                            contexts.AddOrUpdate(context.Feature.ModuleId, context, (k, v) =>
                            {
                                foreach (var error in context.Errors)
                                {
                                    v.Errors.Add(error.Key, error.Value);
                                }

                                return v;
                            });

                        }
                        catch (Exception e)
                        {
                            contexts.AddOrUpdate(context.Feature.ModuleId, context, (k, v) =>
                            {
                                v.Errors.Add(context.Feature.ModuleId, e.Message);
                                return v;
                            });
                        }
                        
                        return contexts;

                    }

                    return null;

                },
                async context =>
                {

                    // Return if feature is already disabled - no need to disable
                    if (!context.Feature.IsEnabled)
                    {
                        return null;
                    }

                    // Invoke FeatureUninstalled subscriptions
                    foreach (var brokerHandler in _broker.Pub<IShellFeature>(this, "FeatureUninstalled"))
                    {
                        context.Feature = await brokerHandler.Invoke(new Message<IShellFeature>(context.Feature, this));
                    }

                    var contexts = new ConcurrentDictionary<string, IFeatureEventContext>();
                    contexts.AddOrUpdate(context.Feature.ModuleId, context, (k, v) =>
                    {
                        foreach (var error in context.Errors)
                        {
                            v.Errors.Add(error.Key, error.Value);
                        }

                        return v;
                    });
                    return contexts;
                    
                });
            
            // Did any event encounter errors?
            var errors = results
                .Where(c => c.Value.Errors.Any())
                .ToList();

            // No errors update descriptor, raise InstalledAsync and recycle ShellContext
            if (!errors.Any())
            {
                // Update descriptor within database
                var descriptor = await RemoveFeaturesAndSaveAsync(featureIds);

            }
            
            // Dispose current shell context
            RecycleShell();

            // Return all execution contexts
            return results.Values;

        }

        // -----------

        async Task<IShellDescriptor> AddFeaturesAndSaveAsync(string[] featureIds)
        {
            var descriptor = await GetOrUpdateDescriptorAsync(featureIds);
            return await _shellDescriptorStore.SaveAsync(descriptor);
        }

        async Task<IShellDescriptor> RemoveFeaturesAndSaveAsync(string[] featureIds)
        {

            // First get all existing enabled features
            var enabledFeatures = await _shellDescriptorManager.GetEnabledFeaturesAsync();

            // Add features minus our features to disable
            var descriptor = new ShellDescriptor();
            foreach (var feature in enabledFeatures)
            {
                var disable = featureIds.Any(f => f.Equals(feature.ModuleId, StringComparison.InvariantCultureIgnoreCase));
                if (!disable)
                {
                    descriptor.Modules.Add(new ShellModule(feature));
                }
            }
            
            return await _shellDescriptorStore.SaveAsync(descriptor);
            
        }

        async Task<IShellDescriptor> GetOrUpdateDescriptorAsync(string[] featureIds)
        {

            // Get existing descriptor or create a new one
            var descriptor = await _shellDescriptorManager.GetEnabledDescriptorAsync();

            // Add features to our descriptor
            foreach (var featureId in featureIds)
            {
                var feature = await _shellDescriptorManager.GetFeatureAsync(featureId);
                descriptor.Modules.Add(new ShellModule(featureId, feature.Version));
            }

            return descriptor;
        }

        async Task<ConcurrentDictionary<string, IFeatureEventContext>> InvokeFeatureEventsAsync(
            IList<IShellFeature> features,
            Func<IFeatureEventContext, IFeatureEventHandler, Task<ConcurrentDictionary<string, IFeatureEventContext>>> handler,
            Func<IFeatureEventContext, Task<ConcurrentDictionary<string, IFeatureEventContext>>> noHandler)
        {

            // Holds the results of all our event execution contexts
            var contexts = new ConcurrentDictionary<string, IFeatureEventContext>();

            // Build a list of all unique features we are enabling / disabling
            var uniqueFeatures = new ConcurrentDictionary<string, IShellFeature>();
            foreach (var feature in features)
            {
                // The feature may also reference dependencies so ensure we also
                // add any dependencies for the features to our temporary shell descriptors
                if (feature.FeatureDependencies.Any())
                {
                    foreach (var dependency in feature.FeatureDependencies)
                    {
                        if (!uniqueFeatures.ContainsKey(dependency.ModuleId))
                        {
                            uniqueFeatures.TryAdd(dependency.ModuleId, dependency);
                        }
                    }
                }
                if (!uniqueFeatures.ContainsKey(feature.ModuleId))
                {
                    uniqueFeatures.TryAdd(feature.ModuleId, feature);
                }
            }

            // Ensure minimum features are always available within the temporary shell descriptor
            // We may depend upon services from the required features within the features we are enabling / disabling
            var minimumShellDescriptor = _shellContextFactory.MinimumShellDescriptor();

            // Add features and dependencies we are enabling / disabling to our minimum shell descriptor
            foreach (var feature in uniqueFeatures.Values)
            {
                minimumShellDescriptor.Modules.Add(new ShellModule(feature.ModuleId, feature.Version));
            }

            // Create a new shell context with features and all dependencies we need to enable / disable
            using (var shellContext = _shellContextFactory.CreateDescribedContext(_shellSettings, minimumShellDescriptor))
            {
                using (var scope = shellContext.ServiceProvider.CreateScope())
                {

                    var handlers = scope.ServiceProvider.GetServices<IFeatureEventHandler>();
                    var handlersList = handlers.ToList();

                    // Iterate through each feature we wish to invoke
                    // Use the event handlers if available else just add to contexts collection
                    foreach (var feature in features)
                    {

                        // Only invoke non required features
                        if (feature.IsRequired)
                        {
                            continue;
                        }

                        // Context that will be passed around
                        var context = new FeatureEventContext()
                        {
                            Feature = feature,
                            ServiceProvider = scope.ServiceProvider,
                            Logger = _logger
                        };

                        // Get event handler for feature we are invoking
                        var featureHandler = handlersList.FirstOrDefault(h => h.ModuleId == feature.ModuleId);

                        // Get response from responsible func
                        var handlerContexts = featureHandler != null
                            ? await handler(context, featureHandler)
                            : await noHandler(context);

                        // Compile results from delegates
                        if (handlerContexts != null)
                        {
                            foreach (var handlerContext in handlerContexts)
                            {
                                contexts.AddOrUpdate(feature.ModuleId, handlerContext.Value, (k, v) =>
                                {
                                    foreach (var error in handlerContext.Value.Errors)
                                    {
                                        v.Errors.Add(error.Key, error.Value);
                                    }

                                    return v;
                                });
                            }

                        }

                        // Log any errors
                        if (context.Errors.Count > 0)
                        {
                            foreach (var error in context.Errors)
                            {
                                _logger.LogCritical(error.Value,
                                    $"An error occurred whilst invoking within {this.GetType().FullName}");
                            }
                        }

                    }

                }

            }

            return contexts;

        }

        void RecycleShell()
        {  
            _platoHost.RecycleShell(_shellSettings);
        }

    }

}
