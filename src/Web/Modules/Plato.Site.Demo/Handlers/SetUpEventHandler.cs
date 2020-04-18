using System;
using System.Threading.Tasks;
using PlatoCore.Abstractions.SetUp;
using PlatoCore.Data.Schemas.Abstractions;

namespace Plato.Site.Demo.Handlers
{
    public class SetUpEventHandler : BaseSetUpEventHandler
    {

        private readonly ISchemaBuilder _schemaBuilder;
    
        public SetUpEventHandler(
            ISchemaBuilder schemaBuilder)
        {
            _schemaBuilder = schemaBuilder;
        }

        public override Task SetUp(
            ISetUpContext context,
            Action<string, string> reportError)
        {
            return Task.CompletedTask;
        }

    }

}
