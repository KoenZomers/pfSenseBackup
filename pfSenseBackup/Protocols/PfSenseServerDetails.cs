using System.Globalization;

namespace KoenZomers.Tools.pfSense.pfSenseBackup.Protocols
{
    /// <summary>
    /// Defines the details of a pfSense server
    /// </summary>
    public class PfSenseServerDetails
    {
        /// <summary>
        /// Defines if HTTPS should be used (True) or HTTP (False)
        /// </summary>
        public bool UseHttps { get; set; }

        /// <summary>
        /// Defines the IP address or DNS name of the pfSense server
        /// </summary>
        public string ServerAddress { get; set; }

        /// <summary>
        /// Gets the base url of the pfSense server. Constructed automatically based on the ServerAddress and UseHttps properties.
        /// </summary>
        public string ServerBaseUrl
        {
            get { return string.Format(CultureInfo.InvariantCulture, "{0}://{1}/", UseHttps ? "https" : "http", ServerAddress); }
        }

        /// <summary>
        /// Defines the timeout in milliseconds to allow for pfSense to respond before considering the connection stale and aborting
        /// </summary>
        public int? RequestTimeOut { get; set; }

        /// <summary>
        /// Version of PfSense (1.2, 2.0, 2.1)
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// The username to use to connect to pfSense
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The password to use to connect to pfSense
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Defines if the statistics data should also be backed up (True) or not (False)
        /// </summary>
        public bool BackupStatisticsData { get; set; }

        /// <summary>
        /// Defines if the package information should also be backed up (True) or not (False)
        /// </summary>
        public bool BackupPackageInfo { get; set; }

        /// <summary>
        /// Defines if the backup should be encrypted by pfSense (True) or should be downloaded unencrypted (False)
        /// </summary>
        public bool EncryptBackup { get; set; }

        /// <summary>
        /// The password to use to encrypt the backup. Only applied if EncryptBackup is set to True.
        /// </summary>
        public string EncryptionPassword { get; set; }
    }
}
