using Plato.Entities.Models;
using Plato.Entities.Services;
using Plato.Internal.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Site.Demo.Models;
using Plato.Users.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Site.Demo.Services
{
     
    public class DemoService : IDemoService
    {

        Random _random;

        private readonly IEntityReplyManager<EntityReply> _entityReplyManager;
        private readonly IPlatoUserManager<User> _platoUserManager;
        private readonly IEntityManager<Entity> _entityManager;
        private readonly IPlatoUserStore<User> _platoUserStore;    
        private readonly IFeatureFacade _featureFacade;
        private readonly IDbHelper _dbHelper;

        // 1. Install sample categories 

        // 2. Install sample labels 

        // 3. Install sample tags 

        // 4. Install sample entities 

        // 5. Install sample entity replies?

        public DemoService(
            IEntityReplyManager<EntityReply> entityReplyManager,
            IPlatoUserManager<User> platoUserManager,
            IEntityManager<Entity> entityManager,            
            IPlatoUserStore<User> platoUserStore,
            IFeatureFacade featureFacade,
            IDbHelper dbHelper)
        {

            _entityReplyManager = entityReplyManager;
            _platoUserManager = platoUserManager;
            _platoUserStore = platoUserStore;
            _entityManager = entityManager;
            _featureFacade = featureFacade;
            _dbHelper = dbHelper;

            _random = new Random();

        }

        // Entities

        public async Task<ICommandResultBase> InstallEntitiesAsync(EntityDataDescriptor descriptor)
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
                var result =  await InstallEntitiesInternalAsync(descriptor, users?.Data);
                if (!result.Succeeded)
                {
                    return output.Failed(result.Errors.ToArray());
                }
            }

            return output.Success();

        }

        public async Task<ICommandResultBase> InstallEntitiesAsync(IEnumerable<EntityDataDescriptor> descriptors)
        {

            var output = new CommandResultBase();
            foreach (var descriptor in descriptors)
            {
                var result = await InstallEntitiesAsync(descriptor);
                if (!result.Succeeded)
                {
                    return output.Failed(result.Errors.ToArray());
                }
            }

            return output.Success();

        }
        
        public async Task<ICommandResultBase> UninstallEntitiesAsync(EntityDataDescriptor descriptor)
        {
            return await UninstallEntitiesInternalAsync(descriptor);
        }

        public async Task<ICommandResultBase> UninstallEntitiesAsync(IEnumerable<EntityDataDescriptor> descriptors)
        {

            var output = new CommandResultBase();
            foreach (var descriptor in descriptors)
            {
                var result = await UninstallEntitiesAsync(descriptor);
                if (!result.Succeeded)
                {
                    return output.Failed(result.Errors.ToArray());
                }
            }

            return output.Success();

        }


        // Users

        public async Task<ICommandResultBase> InstallUsersAsync()
        {
            return await InstallUsersInternalAsync();
        }


        // ------------------

        async Task<ICommandResultBase> InstallEntitiesInternalAsync(EntityDataDescriptor descriptor, IList<User> users)
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

                for (var i = 0; i < descriptor.EntityRepliesToCreate; i++)
                {
              
                    randomUser = users[_random.Next(0, users.Count - 1)];

                    var message = GetReplyText(descriptor);
                    message = message.Replace("{entityUserName}", entityResult.Response.CreatedBy.UserName);
                    message = message.Replace("{replyUserName}", randomUser?.UserName ?? "");

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
                }
            }
            else
            {
                return result.Failed();
            }

            return result.Success();

        }

        async Task<ICommandResultBase> UninstallEntitiesInternalAsync(EntityDataDescriptor descriptor)
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

        async Task<ICommandResultBase> InstallUsersInternalAsync()
        {

            // Our result
            var result = new CommandResultBase();
            
            var usernames = new string[]
                {
                    "John D",
                    "Mark Dogs",
                    "Reverbe ",
                    "Johan",
                    "jcarreira ",
                    "tokyo2002 ",
                    "ebevernage",
                    "pwelter34",
                    "frankmonroe",
                    "tabs",
                    "johangw",
                    "raymak23",
                    "beats",
                    "Fred",
                    "shan",
                    "scottrudy",
                    "thechop",
                    "lyrog",
                    "daniel.gehr",
                    "Cedrik",
                    "nathanchase",
                    "MattPress",
                    "gert.oelof",
                    "abiniyam",
                    "austinh ",
                    "wasimf",
                    "project.ufa",
                    "einaradolfsen",
                    "bstj",
                    "samos",
                    "jintoppy",
                    "mhelin",
                    "eric-914",
                    "marcus85",
                    "leopetes",
                    "angaler1984",
                    "PeterMull",
                    "Stevie",
                    "coder90",
                    "sharah",
                    "Stephen25",
                    "P4a7ker",
                    "Tipsy",
                    "Ryan",
                    "AndyLivey",
                    "RobertW",
                    "ArronG",
                    "Aleena",
                    "Annie",
                    "Cassie",
                    "Lachlan",
                    "Summers",
                    "Isla",
                    "Greer55",
                    "Carry",
                    "Loulou",
                    "MPatterson",
                    "Padilla",
                    "dejavu1987",
                    "fjanon",
                    "project.ufa",
                    "vraptorche",
                    "appleskin",
                    "jintoppy",
                    "mhelin",
                    "NajiJzr",
                    "eric-914",
                    "cportermo",
                    "jack4it",
                    "sapocockas",
                    "srowan",
                    "atpw25",
                    "ralmlopez",
                    "PartyLineLimo",
                    "murdocj",
                    "unichan2018",
                    "eliemichael",
                    "typedef",
                    "MattEllison",
                    "JaiPundir",
                    "zyberzero",
                    "tim",
                    "zakjan",
                    "revered",
                    "Breaker222",
                    "xenod",
                    "mortenbrudv",
                    "cmd_shell",
                    "mcrose",
                    "cusdom",
                    "recruit-jp",
                    "house",
                    "TedProsoft",
                    "luison",
                    "fritz",
                    "eric",
                    "rossang",
                    "AlDennis",
                    "Oxid2178",
                    "CasiOo",
                    "JimShelly",
                    "cisco",
                    "ToadRage",
                    "ericedgar ",
                    "bryan",
                    "joshuaharderr",
                    "mvehar",
                    "arkadiusz-cholewa",
                    "necipakif",
                    "PeterEltgroth",
                    "redone218",
                    "iiiyx",
                    "seanmill",
                    "00ffx"
                };
            
            foreach (var username in usernames)
            {
                var displayName = username;
                var userNAme = username;
                var email = username + "@example.com";
                var password = "34Fdckf#343";

                var newUserResult = await _platoUserManager.CreateAsync(new User()
                {
                    UserName = userNAme,
                    Email = email,
                    Password = password,
                    DisplayName = displayName
                });
            }
            
            return result.Success();

        }

        string GetEntityText(EntityDataDescriptor descriptor)
        {

            // Get random markdown sample
            var index = _random.Next(0, _entityText.Length);
            var message = _entityText[index];

            // Perform replacements
            message = ParseDescriptorTags(descriptor, message);

            // Return sample markdown
            return message;

        }

        string GetReplyText(EntityDataDescriptor descriptor)
        {

            // Get random markdown sample
            var index = _random.Next(0, _replyText.Length);
            var message = _replyText[index];

            // Perform replacements
            message = ParseDescriptorTags(descriptor, message);

            // Return sample markdown
            return message;

        }
     
        string ParseDescriptorTags(EntityDataDescriptor descriptor, string input)
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

Thanks for the post. I'm just an example reply but if I was a real person I would definately :heart: :heart: :heart: this.

-=: {replyUserName} :=-",
              @"Thanks @{entityUserName},

I'm just an example reply but if I was a real person this would make me :grinning:

Regards,  
{replyUserName}",
        };

    }

}
