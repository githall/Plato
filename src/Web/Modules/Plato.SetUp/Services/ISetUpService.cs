using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PlatoCore.Abstractions.SetUp;

namespace Plato.SetUp.Services
{
    public interface ISetUpService
    {
        Task<string> SetUpAsync(SetUpContext context);
    }
}
