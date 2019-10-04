using Plato.Entities.Models;
using Plato.Entities.Services;
using Plato.Internal.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Users.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plato.Site.Demo.Services
{

    public interface IDemoService
    {
        Task<ICommandResultBase> InstallEntitiesAsync(string moduleId);

        Task<ICommandResultBase> InstallUsersAsync();

    }

    public class DemoService : IDemoService
    {
        
        private readonly IEntityReplyManager<EntityReply> _entityReplyManager;
        private readonly IEntityManager<Entity> _entityManager;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IPlatoUserManager<User> _platoUserManager;
        private readonly IFeatureFacade _featureFacade;

        // 1. Install sample categories 

        // 2. Install sample labels 

        // 3. Install sample tags 

        // 4. Install sample entities 

        // 5. Install sample entity replies?


        public DemoService(
            IEntityReplyManager<EntityReply> entityReplyManager,
            IEntityManager<Entity> entityManager,            
            IPlatoUserStore<User> platoUserStore,
            IPlatoUserManager<User> platoUserManager,
            IFeatureFacade featureFacade)
        {
            _entityReplyManager = entityReplyManager;
            _entityManager = entityManager;
            _platoUserStore = platoUserStore;
            _platoUserManager = platoUserManager;
            _featureFacade = featureFacade;
        }

        // Entities

        public async Task<ICommandResultBase> InstallEntitiesAsync(string moduleId)
        {
            return await InstallEntitiesInternalAsync(moduleId);
        }

        // Users

        public async Task<ICommandResultBase> InstallUsersAsync()
        {
            return await InstallUsersInternalAsync();
        }

        // ------------------

        async Task<ICommandResultBase> InstallEntitiesInternalAsync(string moduleId)
        {

            // Our result
            var result = new CommandResultBase();

            var users = await _platoUserStore.QueryAsync()
                .OrderBy("LastLoginDate", OrderBy.Desc)
                .ToList();

            var rnd = new Random();
            var totalUsers = users?.Data.Count - 1 ?? 0;
            var randomUser = users?.Data[rnd.Next(0, totalUsers)];
            var feature = await _featureFacade.GetFeatureByIdAsync(moduleId);

            var topic = new Entity()
            {
                Title = "Test Idea " + rnd.Next(0, 2000).ToString(),
                Message = GetSampleMarkDown(rnd.Next(0, 2000)),
                FeatureId = feature?.Id ?? 0,
                CreatedUserId = randomUser?.Id ?? 0,
                CreatedDate = DateTimeOffset.UtcNow
            };

            // create topic
            var entityResult = await _entityManager.CreateAsync(topic);
            if (entityResult.Succeeded)
            {
                for (var i = 0; i < 25; i++)
                {
                    rnd = new Random();
                    randomUser = users?.Data[rnd.Next(0, totalUsers)];

                    var reply = new EntityReply()
                    {
                        EntityId = entityResult.Response.Id,
                        Message = GetSampleMarkDown(i) + " - comment : " + i.ToString(),
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

        async Task<ICommandResultBase> InstallUsersInternalAsync()
        {

            // Our result
            var result = new CommandResultBase();

            var rnd = new Random();

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

        string GetSampleMarkDown(int number)
        {
            return @"Hi There, 

This is just a sample idea to demonstrate idea within Plato.Ideas use markdown for formatting and can be organized using tags, labels or categories. 

We hope you enjoy this early version of Plato :)

        string GetSampleMarkDown(int number)
Ryan :heartpulse: :heartpulse: :heartpulse:" + number;

        }


    }

}
