namespace KoenZomers.Tools.pfSense.pfSenseBackup.Protocols
{
    /// <summary>
    /// Defines a PfSense Backup File
    /// </summary>
    public class PfSenseBackupFile
    {
        /// <summary>
        /// The filename for the backup file
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The contents of the backup file
        /// </summary>
        public string FileContents { get; set; }
    }
}
