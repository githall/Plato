using System;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Users.Services;

namespace Plato.Site.Demo.Services
{

    public class SampleUsersService : ISampleUsersService
    {

        public string[] Usernames => new string[]
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

        Random _random;

        private readonly IPlatoUserManager<User> _platoUserManager;

        public SampleUsersService(IPlatoUserManager<User> platoUserManager)
        {     
            _platoUserManager = platoUserManager;           
            _random = new Random();
        }

        public async Task<ICommandResultBase> InstallAsync()
        {
            return await InstallUsersInternalAsync();
        }


        public Task<ICommandResultBase> UninstallAsync()
        {
            throw new NotImplementedException();
        }

        // --------------

        async Task<ICommandResultBase> InstallUsersInternalAsync()
        {

            // Our result
            var result = new CommandResultBase();

            foreach (var username in Usernames)
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


    }
}
