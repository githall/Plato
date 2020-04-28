using PlatoCore.Abstractions.SetUp;
using PlatoCore.Emails.Abstractions;
using PlatoCore.Models.Shell;
using System;

namespace Plato.Tenants.Models
{
    public class TenantSetUpContext :SetUpContext
    {

        public string Location { get; set; }

        public TenantState State { get; set; }

        public string OwnerId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public DateTimeOffset? ModifiedDate { get; set; }

        public EmailSettings EmailSettings { get; set; }

    }

}
