using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PlatoCore.Tasks.Abstractions;

namespace PlatoCore.Tasks
{

    public class BackgroundTaskManager : IBackgroundTaskManager
    {

       /// <summary>
       /// Globally enable or disable background tasks. Should remain enabled or true unless debugging.
       /// </summary>    
        public bool Enabled { get; set; }= true;

        private IEnumerable<IBackgroundTaskProvider> _providers;
        private ILogger<BackgroundTaskManager> _logger;
        private ISafeTimerFactory _safeTimerFactory;

        public void StartTasks(IServiceProvider serviceProvider)
        {

            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            _providers = serviceProvider.GetService<IEnumerable<IBackgroundTaskProvider>>();
            _safeTimerFactory = serviceProvider.GetService<ISafeTimerFactory>();
            _logger = serviceProvider.GetService<ILogger<BackgroundTaskManager>>();
            StartTasks();
        }

        public void StartTasks()
        {

            if (!Enabled)
            {
                return;
            }

            foreach (var provider in _providers)
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation($"Initializing background task provider of type '{provider.GetType()}'.");
                }

                _safeTimerFactory.Start(async (sender, args) =>
                {
                    try
                    {
                        await provider.ExecuteAsync(sender, args);
                    }
                    catch (Exception e)
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogError(e,
                                $"An error occurred whilst executing the timer callback for background task provider of type '{provider.GetType()}'");
                        }
                    }
                }, new SafeTimerOptions()
                {
                    Owner = provider.GetType(),
                    IntervalInSeconds = provider.IntervalInSeconds
                });

            }

        }

        public void StopTasks()
        {
            _safeTimerFactory.Stop();
        }

    }

}
