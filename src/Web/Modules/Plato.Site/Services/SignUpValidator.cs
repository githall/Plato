using System;
using System.Threading.Tasks;
using Plato.Site.Models;
using Plato.Site.Stores;
using PlatoCore.Abstractions;

namespace Plato.Site.Services
{

    public class SignUpValidator : ISignUpValidator
    {

        private readonly ISignUpStore<SignUp> _signUpStore;

        public SignUpValidator(ISignUpStore<SignUp> signUpStore)
        {
            _signUpStore = signUpStore;
        }

        public Task<ICommandResultBase> ValidateEmailAsync(string email)
        {
            var result = new CommandResultBase();
            return Task.FromResult((ICommandResultBase)result.Success());
        }

        public async Task<ICommandResultBase> ValidateCompanyNameAsync(string companyName)
        {

            var result = new CommandResultBase();

            var signUps = await _signUpStore.QueryAsync()
                .Select<SignUpQueryParams>(q =>
                {
                    q.CompanyName.Equals(companyName);
                })
                .ToList();

            if (signUps?.Data != null)
            {

            }

            throw new NotImplementedException();


        }

    }

}
