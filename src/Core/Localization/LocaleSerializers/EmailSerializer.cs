using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using PlatoCore.Localization.Abstractions.Models;

namespace PlatoCore.Localization.LocaleSerializers
{

    public class EmailsSerializer
    {

        private const string SectionName = "Emails";

        public static IEnumerable<LocaleEmail> Parse(IConfigurationRoot configuration)
        {

            var templates = new List<LocaleEmail>();
            var section = configuration.GetSection(SectionName);
            if (section != null)
            {
                var children = section.GetChildren();
                foreach (var child in children)
                {
                    templates.Add(new LocaleEmail
                    {
                        Name = child["Id"],
                        To = child["To"],
                        From = child["From"],
                        Cc = child["Cc"],
                        Bcc = child["Bcc"],
                        Subject = child["Subject"],
                        Message = child["Message"]
                    });
                }

            }

            return templates;

        }

    }

}
