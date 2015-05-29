using System;
using System.Collections.Generic;
using System.Net;

namespace KoenZomers.Tools.pfSense.pfSenseBackup.Protocols
{
    /// <summary>
    /// Implementation of the pfSense protocol for version 1.2
    /// </summary>
    public class PfSenseVersion12 : IPfSenseProtocol
    {
        /// <summary>
        /// Connects with the specified pfSense server using the v1.2 protocol implementation and returns the backup file contents
        /// </summary>
        /// <param name="pfSenseServer">pfSense server details which identifies which pfSense server to connect to</param>
        /// <param name="cookieJar">Cookie container to use through the communication with pfSense</param>
        /// <returns>PfSenseBackupFile instance containing the retrieved backup content from pfSense</returns>
        public PfSenseBackupFile Execute(PfSenseServerDetails pfSenseServer, CookieContainer cookieJar)
        {
            Program.WriteOutput("Retrieving backup file");

            var downloadArgs = new Dictionary<string, string>
                {
                    { "nopackages", pfSenseServer.BackupPackageInfo ? "" : "on" },
                    { "Submit", "Download configuration" }
                };

            string filename;
            var pfSenseBackupFile = new PfSenseBackupFile
            {
                FileContents = HttpUtility.DownloadBackupFile(string.Concat(pfSenseServer.ServerBaseUrl, "diag_backup.php"),
                                                                downloadArgs,
                                                                cookieJar,
                                                                out filename),
                FileName = filename
            };
            return pfSenseBackupFile;
        }
    }
}
