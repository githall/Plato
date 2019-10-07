using System;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Plato.Entities.Models;
using Plato.Entities.Services;
using Plato.Internal.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Site.Demo.Models;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Hosting.Abstractions;

namespace Plato.Site.Demo.Services
{

    public class SampleEntitiesService : ISampleEntitiesService
    {

        private readonly List<SampleDataDescriptor> Descriptors = new List<SampleDataDescriptor>()
        {
            new SampleDataDescriptor()
            {
                ModuleId = "Plato.Discuss",
                EntityType = "topic"
            },
            new SampleDataDescriptor()
            {
                ModuleId = "Plato.Docs",
                EntityType = "doc",
            },
            new SampleDataDescriptor()
            {
                ModuleId = "Plato.Articles",
                EntityType = "articles",
            },
            new SampleDataDescriptor()
            {
                ModuleId = "Plato.Ideas",
                EntityType = "idea"
            },
            new SampleDataDescriptor()
            {
                ModuleId = "Plato.Issues",
                EntityType = "issue"
            },
            new SampleDataDescriptor()
            {
                ModuleId = "Plato.Questions",
                EntityType = "question"
            }
        };

        Random _random;

        private readonly IEntityReplyManager<EntityReply> _entityReplyManager;  
        private readonly ISampleUsersService _sampleUsersService;
        private readonly IEntityManager<Entity> _entityManager;        
        private readonly IPlatoUserStore<User> _platoUserStore;    
        private readonly IFeatureFacade _featureFacade;
        private readonly IContextFacade _contextFacade;
        private readonly IDbHelper _dbHelper;

        public SampleEntitiesService(
            IEntityReplyManager<EntityReply> entityReplyManager,
            ISampleUsersService sampleUsersService,
            IEntityManager<Entity> entityManager,
            IPlatoUserStore<User> platoUserStore,
            IContextFacade contextFacade,
            IFeatureFacade featureFacade,
            IDbHelper dbHelper)
        {
            _entityReplyManager = entityReplyManager;
            _sampleUsersService = sampleUsersService;               
            _platoUserStore = platoUserStore;
            _entityManager = entityManager;
            _contextFacade = contextFacade;
            _featureFacade = featureFacade;
            _dbHelper = dbHelper;
            _random = new Random();
        }

        public async Task<ICommandResultBase> InstallAsync()
        {

            var output = new CommandResultBase();
            foreach (var descriptor in Descriptors)
            {
                var result = await InstallEntitiesAsync(descriptor);
                if (!result.Succeeded)
                {
                    return output.Failed(result.Errors.ToArray());
                }
            }

            return output.Success();

        }

        public async Task<ICommandResultBase> UninstallAsync()
        {

            var output = new CommandResultBase();
            foreach (var descriptor in Descriptors)
            {
                var result = await UninstallEntitiesAsync(descriptor);
                if (!result.Succeeded)
                {
                    return output.Failed(result.Errors.ToArray());
                }
            }

            return output.Success();

        }

        // ------------------

        async Task<ICommandResultBase> InstallEntitiesAsync(SampleDataDescriptor descriptor)
        {

            var output = new CommandResultBase();

            // Get sample users
            var users = await _platoUserStore.QueryAsync()
                .OrderBy("LastLoginDate", OrderBy.Desc)
                .ToList();

            // We need sample users to create sample entities
            if (users == null || users.Data == null)
            {
                return output.Failed("You must create sample users first!");
            }

            for (var i = 0; i < descriptor.EntitiesToCreate; i++)
            {
                var result = await InstallEntitiesInternalAsync(descriptor, users?.Data);
                if (!result.Succeeded)
                {
                    return output.Failed(result.Errors.ToArray());
                }
            }

            return output.Success();

        }

        async Task<ICommandResultBase> UninstallEntitiesAsync(SampleDataDescriptor descriptor)
        {
            return await UninstallEntitiesInternalAsync(descriptor);
        }

        async Task<ICommandResultBase> InstallEntitiesInternalAsync(SampleDataDescriptor descriptor, IList<User> users)
        {

            // Validate

            if (descriptor == null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            if (string.IsNullOrEmpty(descriptor.ModuleId))
            {
                throw new ArgumentNullException(nameof(descriptor.ModuleId));
            }

            // Our result
            var result = new CommandResultBase();

            // Ensure the feature is enabled
            var feature = await _featureFacade.GetFeatureByIdAsync(descriptor.ModuleId);

            if (feature == null)
            {
                return result.Failed($"The feature {descriptor.ModuleId} is not enabled!");
            }         

            // Get a random user for the post
            var randomUser = users[_random.Next(0, users.Count)];

            // Capitalize the first character of our entity type
            var entityTypeCapitalized = char.ToUpper(descriptor.EntityType[0]).ToString() + descriptor.EntityType.Substring(1);

            // Create the post
            var entity = new Entity()
            {
                Title = $"Example {entityTypeCapitalized} {_random.Next(0, 2000).ToString()}",
                Message = GetEntityText(descriptor),
                FeatureId = feature?.Id ?? 0,
                CreatedUserId = randomUser?.Id ?? 0,
                CreatedDate = DateTimeOffset.UtcNow
            };

            // create topic
            var entityResult = await _entityManager.CreateAsync(entity);
            if (entityResult.Succeeded)
            {

                var lastReplyId = string.Empty;
                var lastReplyUserName = string.Empty;
                var lastReplyMessage = string.Empty;

                for (var i = 0; i < descriptor.EntityRepliesToCreate; i++)
                {
              
                    randomUser = users[_random.Next(0, users.Count - 1)];

                    var message = GetReplyText(descriptor);
                    
                    message = message.Replace("{replyUserName}", randomUser?.UserName ?? "");

                    message = message.Replace("{lastReplyId}", lastReplyId ?? "");
                    message = message.Replace("{lastReplyMessage}", lastReplyMessage ?? "");
                    message = message.Replace("{lastReplyQuotedMessage}", lastReplyMessage.Replace(System.Environment.NewLine, System.Environment.NewLine + "> ") ?? "");
                    message = message.Replace("{lastReplyUserName}", lastReplyUserName ?? "");                    

                    message = message.Replace("{entityId}", entityResult.Response.Id.ToString() ?? "");
                    message = message.Replace("{entityTitle}", entityResult.Response.Title ?? "");
                    message = message.Replace("{entityUserName}", entityResult.Response.CreatedBy.UserName);

                    message = message.Replace("{mentionSample}", BuildMentionSampleMarkUp());
                   
                    message = message.Replace("{lastReplyUrl}", _contextFacade.GetRouteUrl(new RouteValueDictionary()
                    {
                        ["area"] = descriptor.ModuleId,
                        ["controller"] = "Home",
                        ["action"] = "Reply",
                        ["opts.id"] = entityResult.Response.Id.ToString() ?? "",
                        ["opts.alias"] = entityResult.Response.Alias.ToString() ?? "",
                        ["opts.replyId"] = lastReplyId ?? ""
                    }));

                    var reply = new EntityReply()
                    {
                        EntityId = entityResult.Response.Id,
                        Message = message,
                        CreatedUserId = randomUser?.Id ?? 0,
                        CreatedDate = DateTimeOffset.UtcNow
                    };
                    var replyResult = await _entityReplyManager.CreateAsync(reply);
                    if (!replyResult.Succeeded)
                    {
                        return result.Failed();
                    }

                    lastReplyId = replyResult.Response.Id.ToString();
                    lastReplyMessage = replyResult.Response.Message;
                    lastReplyUserName = replyResult.Response.CreatedBy.UserName;                    

                }
            }
            else
            {
                return result.Failed(result.Errors.ToArray());
            }

            return result.Success();

        }

        string BuildMentionSampleMarkUp()
        {

            var i = 0;
            var sb = new StringBuilder();
            var userNames = new List<string>();
            
            for (i = 0; i < _random.Next(4, 8); i++)
            {                
                var index = _random.Next(0, _sampleUsersService.Usernames.Length - 1);
                var username = _sampleUsersService.Usernames[index];
                if (userNames.Contains(username))
                {
                    continue;
                }
                userNames.Add(username);
            }

            i = 0;
            foreach (var userName in userNames)
            {
                sb.Append("@").Append(userName);
                if (i < userNames.Count - 1)
                {
                    sb.Append(", ");
                } else
                {
                    sb.Append(".");
                }
                i++;
            }

            return sb.ToString();

        }

        async Task<ICommandResultBase> UninstallEntitiesInternalAsync(SampleDataDescriptor descriptor)
        {

            // Validate

            if (descriptor == null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            if (string.IsNullOrEmpty(descriptor.ModuleId))
            {
                throw new ArgumentNullException(nameof(descriptor.ModuleId));
            }

            // Our result
            var result = new CommandResultBase();

            // Ensure the feature is enabled
            var feature = await _featureFacade.GetFeatureByIdAsync(descriptor.ModuleId);

            if (feature == null)
            {
                return result.Failed($"The feature {descriptor.ModuleId} is not enabled!");
            }

            // Replacements for SQL script
            var replacements = new Dictionary<string, string>()
            {
                ["{featureId}"] = feature.Id.ToString()
            };

            // Sql to execute
            var sql = @"
                DELETE FROM {prefix}_EntityData WHERE Id IN (
                    SELECT Id FROM {prefix}_Entities WHERE FeatureId = {featureId}
                );
                DELETE FROM {prefix}_EntityReplies WHERE EntityId IN (
                    SELECT Id FROM {prefix}_Entities WHERE FeatureId = {featureId}
                );
                DELETE FROM {prefix}_Entities WHERE FeatureId = {featureId};
            ";

            // Execute and return result
            var error = string.Empty;
            try
            {
                await _dbHelper.ExecuteScalarAsync<int>(sql, replacements);
            }
            catch (Exception e)
            {
                error = e.Message;
            }

            return !string.IsNullOrEmpty(error)
                ? result.Failed(error)
                : result.Success();

        }
              
        string GetEntityText(SampleDataDescriptor descriptor)
        {

            // Get random markdown sample
            var index = _random.Next(0, _entityText.Length);
            var message = _entityText[index];

            // Perform replacements
            message = ParseDescriptorTags(descriptor, message);

            // Return sample markdown
            return message;

        }

        string GetReplyText(SampleDataDescriptor descriptor)
        {

            // Get random markdown sample
            var index = _random.Next(0, _replyText.Length);
            var message = _replyText[index];

            // Perform replacements
            message = ParseDescriptorTags(descriptor, message);

            // Return sample markdown
            return message;

        }

        string ParseDescriptorTags(SampleDataDescriptor descriptor, string input)
        {
            input = input.Replace("{moduleId}", descriptor.ModuleId);
            input = input.Replace("{entityType}", descriptor.EntityType);
            return input;
        }

        string[] _entityText = new string[]
        {
        @"Hi There, 

This is just a sample {entityType} to demonstrate how the ""{moduleId}"" module works within Plato. This is not a real post and simply exists to help you get a sense for how {moduleId} works.

Messages within Plato are typically written in Markdown. 

For example you can add images...

![Plato Dashboard](https://plato.instantasp.co.uk/plato.site/content/images/dashboard.png ""enter image title here"")

# Login

You can login using the demo administrator account via the login page. Use the button below to jump the login page, then click ""Goto Dashaboard""...

[Goto Login](/login){.btn .btn-primary}

# Explore

Please feel free to explore Plato and modify any content here within the demo. This is all just sameple data.

We hope you enjoy Plato and find it useful. Thanks for exploring the demo.

---

Regards,  
The Plato Team  
Better, Faster Support... FREE  
https://plato.instantasp.co.uk/

",
        @"Hi There, 

This is just a sample {entityType} to demonstrate how the ""{moduleId}"" module works within Plato. This is not a real post and simply exists to help you get a sense for how {moduleId} works.

Messages within Plato are typically written in Markdown. 

For example you could link to [Google](https://www.google.com/) or [Wikipedia](https://en.wikipedia.org/).

# Login

You can login using the demo administrator account via the login page. Use the button below to jump the login page, then click ""Goto Dashaboard""...

[Goto Login](/login){.btn .btn-primary}

# Explore

Please feel free to explore Plato and modify any content here within the demo. This is all just sameple data.

We hope you enjoy Plato and find it useful. Thanks for exploring the demo.

---

Regards,  
The Plato Team  
Better, Faster Support... FREE  
https://plato.instantasp.co.uk/

",
        @"Hi There, 

This is just a sample {entityType} to demonstrate how the ""{moduleId}"" module works within Plato. This is not a real post and simply exists to help you get a sense for how {moduleId} works.

Messages within Plato are typically written in Markdown. 

For example you can add video...

![Video1](https://www.youtube.com/watch?v=U9t-slLl30E)

# Login

You can login using the demo administrator account via the login page. Use the button below to jump the login page, then click ""Goto Dashaboard""...

[Goto Login](/login){.btn .btn-primary}

# Explore

Please feel free to explore Plato and modify any content here within the demo. This is all just sameple data.

We hope you enjoy Plato and find it useful. Thanks for exploring the demo.

---

Regards,  
The Plato Team  
Better, Faster Support... FREE  
https://plato.instantasp.co.uk/

",
        @"Hi There, 

This is just a sample {entityType} to demonstrate how the ""{moduleId}"" module works within Plato. This is not a real post and simply exists to help you get a sense for how {moduleId} works.

Messages within Plato are typically written in Markdown. 

For example you can add formatted text such as **bold**, _italics_, `inline code` or even code blocks...

```
public void ExampleMethod() {
// ... do something
}
```

# Login

You can login using the demo administrator account via the login page. Use the button below to jump the login page, then click ""Goto Dashaboard""...

[Goto Login](/login){.btn .btn-primary}

# Explore

Please feel free to explore Plato and modify any content here within the demo. This is all just sameple data.

We hope you enjoy Plato and find it useful. Thanks for exploring the demo.

---

Regards,  
The Plato Team  
Better, Faster Support... FREE  
https://plato.instantasp.co.uk/

",
        @"Hi There, 

This is just a sample {entityType} to demonstrate how the ""{moduleId}"" module works within Plato. This is not a real post and simply exists to help you get a sense for how {moduleId} works.

Messages within Plato are typically written in Markdown. 

For example you can add headers...

# Header 1

Some example text. Some example text. Some example text. Some example text. Some example text. Some example text. Some example text. Some example text. Some example text. Some example text. 

## Header 2

Some example text. Some example text. Some example text. Some example text. Some example text. Some example text. Some example text. Some example text. Some example text. Some example text. 

### Header 3

Some example text. Some example text. Some example text. Some example text. Some example text. Some example text. Some example text. Some example text. Some example text. Some example text. 

#### Header 4

Some example text. Some example text. Some example text. Some example text. Some example text. Some example text. Some example text. Some example text. Some example text. Some example text. 

##### Header 5

Some example text. Some example text. Some example text. Some example text. Some example text. Some example text. Some example text. Some example text. Some example text. Some example text. 

###### Header 6

Some example text. Some example text. Some example text. Some example text. Some example text. Some example text. Some example text. Some example text. Some example text. Some example text. 

# Login

You can login using the demo administrator account via the login page. Use the button below to jump the login page, then click ""Goto Dashaboard""...

[Goto Login](/login){.btn .btn-primary}

# Explore

Please feel free to explore Plato and modify any content here within the demo. This is all just sameple data.

We hope you enjoy Plato and find it useful. Thanks for exploring the demo.

---

Regards,  
The Plato Team  
Better, Faster Support... FREE  
https://plato.instantasp.co.uk/

"
        };

        string[] _replyText = new string[]
        {
        @"Hi @{entityUserName}

This is just an example reply to your {entityType}. Here we would typically provide feedback or assist with your post but as this is just a demo we'll play pretend here.

This reply is just provided for demo purposes to help you get a sense for how Plato works.

---

Thanks,    
{replyUserName}",
        @"Hi @{entityUserName}

This is just a sample to help demonstrate Plato. You can ignore this post :)

If this was a real reply it may actually contain some helpful information. 

{replyUserName}  
https://www.instantasp.co.uk/",
        @"@{entityUserName}, I'm just an example reply. I'm not real but let's pretend i like this post :+1:

{replyUserName}",
        @"@{entityUserName}, I'm an examplle reply pretending I'm not happy with your post grrrr - :smiling_imp:

---

{replyUserName}  
https://plato.instantasp.co.uk/",
        @"Hey @{entityUserName},

Thanks for the post. I'm just an example reply but if I was a real person I would definately :heart: this.

-=: {replyUserName} :=-",
        @"Thanks @{entityUserName},

I'm not actually a real person. I'm just an example reply to help demonstrate how Plato works. I'm populated automatically via a background process within Plato.

Regards,  
{replyUserName}",
        @"Hey @{entityUserName},

Did you know that Plato supports quoting replies. You can see a quoted reply below...

> {lastReplyQuotedMessage}
> ^^ In response to [{lastReplyUserName}]({lastReplyUrl})

We can of course include some text after the quote. 

-=: {replyUserName} :=-",
        @"Did you know you can link to any other entity (topic, doc, article, question, idea or issue) within Plato using a # tag. Type the # character when composing a message within Plato to quickly search for and embed links to other entities.

For example we can link to the {entityType} hosting this reply using a # character follwed by the entity ID like so #{entityId}. 

You can also include optional text to display for # links like so #`1234`(`optional link text here`).

You can see a real hash link with link text here:- #{entityId}({entityTitle}) - notice the contextual tooltip when you hover over # links. You can also just use a # followed by the entity id to link to any entity like so #{entityId}.

Regards,  
{replyUserName}",
        @"I'm just an example reply but did you know you can @ mention others within Plato. Type the @ character when composing any entity to search for and mention others. 

For example here are some mentions {mentionSample} 

Regards,  
{replyUserName}",
        };

    }

}
