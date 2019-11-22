namespace Plato.Internal.Abstractions.Settings
{

    /// <summary>
    /// Represents core application settings.
    /// </summary>
    public class PlatoOptions
    {

        /// <summary>
        /// The version of Plato.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// The release type (Final, Pre-Release, BETA etc)
        /// </summary>
        public string ReleaseType { get; set; }

        /// <summary>
        /// A boolean indicating if Plato is running in demo mode.
        /// </summary>
        public bool DemoMode { get; set; }

        /// <summary>
        /// The physical or UNC path indicating where to persist the data protection API keyring.
        /// </summary>
        public string SecretsPath { get; set; }

    }

}
