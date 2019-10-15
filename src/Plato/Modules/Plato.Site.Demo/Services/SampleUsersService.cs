using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Plato.Users.Services;
using Plato.Internal.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Data.Abstractions;

namespace Plato.Site.Demo.Services
{

    public class SampleUsersService : ISampleUsersService
    {

        public string[] Usernames => new string[]
        {
            "JohnD",
            "MarkDogs",
            "Reverbe",
            "Johan",
            "jcarreira",
            "tokyo2002",
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
            "daniel-gehr",
            "Cedrik",
            "nathanchase",
            "MattPress",
            "gert_oelof",
            "abiniyam",
            "austinh ",
            "wasimf",
            "project_ufa",
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
            "BarryV",
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
            "gogetter",
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
            "ericedgar",
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

        private readonly IPlatoUserManager<User> _platoUserManager;
        private readonly IDbHelper _dbHelper;

        public SampleUsersService(
            IPlatoUserManager<User> platoUserManager,
            IDbHelper dbHelper)
        {
            _platoUserManager = platoUserManager;
            _dbHelper = dbHelper;
        }

        public async Task<ICommandResultBase> InstallAsync()
        {
            return await InstallInternalAsync();
        }

        public async Task<ICommandResultBase> UninstallAsync()
        {
            return await UninstallInternalAsync();
        }

        // --------------

        async Task<ICommandResultBase> InstallInternalAsync()
        {

            // Our result
            var result = new CommandResultBase();

            foreach (var username in Usernames)
            {

                var userName = username.Trim();
                var email = userName + "@example.com";
                var password = "34Fdckf#343";

                var newUserResult = await _platoUserManager.CreateAsync(new User()
                {
                    UserName = userName,
                    Email = email,
                    Password = password,
                    DisplayName = userName
                });
            }

            return result.Success();

        }

        async Task<ICommandResultBase> UninstallInternalAsync()
        {

            var i = 0;
            var sb = new StringBuilder();
            foreach (var userName in Usernames)
            {
                sb.Append("'")
                    .Append(userName.Replace("'", "''"))
                    .Append("'");
                if (i < Usernames.Length - 1)
                {
                    sb.Append(", ");
                }
                i++;
            }

            // Replacements for SQL script
            var replacements = new Dictionary<string, string>()
            {
                ["{userNames}"] = sb.ToString()
            };

            var sql = @"
                DELETE FROM {prefix}_UserBadges WHERE UserId IN (
                    SELECT Id FROM {prefix}_Users WHERE UserName IN ({userNames})
                );
                DELETE FROM {prefix}_UserPhoto WHERE UserId IN (
                    SELECT Id FROM {prefix}_Users WHERE UserName IN ({userNames})
                );
                DELETE FROM {prefix}_UserData WHERE UserId IN (
                    SELECT Id FROM {prefix}_Users WHERE UserName IN ({userNames})
                );
                DELETE FROM {prefix}_Users WHERE UserName IN ({userNames});
            ";

            // Our result
            var result = new CommandResultBase();

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

    }

}
