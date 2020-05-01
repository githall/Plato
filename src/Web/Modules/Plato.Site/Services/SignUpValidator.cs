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

            // TODO: Implement email validation

            var result = new CommandResultBase();
            return Task.FromResult((ICommandResultBase)result.Success());

        }

        public async Task<ICommandResultBase> ValidateCompanyNameAsync(string companyName)
        {

            var result = new CommandResultBase();

            // ---------------
            // Does the company name already exist?
            // We need company names to be unique as the 
            // tenant RequestedPrefixUrl uses CompanyNameAlias
            // ---------------

            var signUps = await _signUpStore.QueryAsync()
                .Select<SignUpQueryParams>(q =>
                {
                    q.CompanyName.Or().Equals(companyName);
                    q.CompanyNameAlias.Or().Equals(companyName);
                })
                .ToList();

            if (signUps?.Data != null)
            {
                return result.Failed("The company name is invalid or is already in use. Please try a different company name.");
            }

            return result.Success();

        }

    }

}
