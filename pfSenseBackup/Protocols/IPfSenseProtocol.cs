using System.Net;

namespace KoenZomers.Tools.pfSense.pfSenseBackup.Protocols
{
    /// <summary>
    /// Defines the interface to which pfSense protocol handlers must comply
    /// </summary>
    public interface IPfSenseProtocol
    {
        /// <summary>
        /// Connects with the specified pfSense server using the current protocol implementation and returns the backup file contents
        /// </summary>
        /// <param name="pfSenseServer">pfSense server details which identifies which pfSense server to connect to</param>
        /// <param name="cookieJar">Cookie container to use through the communication with pfSense</param>
        /// <param name="timeout">Timeout in milliseconds on how long requests to pfSense may take. Default = 60000 = 60 seconds.</param>
        /// <returns>PfSenseBackupFile instance containing the retrieved backup content from pfSense</returns>
        PfSenseBackupFile Execute(PfSenseServerDetails pfSenseServer, CookieContainer cookieJar, int timeout = 60000);
    }
}
