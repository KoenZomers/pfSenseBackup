﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace KoenZomers.Tools.pfSense.pfSenseBackup.Protocols
{
    /// <summary>
    /// Implementation of the OPNSense protocol for version 19.07
    /// </summary>
    public class OPNSenseVersion197 : IPfSenseProtocol
    {
        /// <summary>
        /// Connects with the specified OPNSense server using the 19.07 protocol implementation and returns the backup file contents
        /// </summary>
        /// <param name="pfSenseServer">OPNSense server details which identifies which OPNSense server to connect to</param>
        /// <param name="cookieJar">Cookie container to use through the communication with OPNSense</param>
        /// <param name="timeout">Timeout in milliseconds on how long requests to OPNSense may take. Default = 60000 = 60 seconds.</param>
        /// <returns>OPNSenseBackupFile instance containing the retrieved backup content from OPNSense</returns>
        public PfSenseBackupFile Execute(PfSenseServerDetails pfSenseServer, CookieContainer cookieJar, int timeout = 60000)
        {
            Program.WriteOutput("Connecting using protocol version {0}", new object[] { pfSenseServer.Version });

            // Create a session on the OPNSense webserver
            var loginPageContents = HttpUtility.HttpGetLoginPageContents(pfSenseServer.ServerBaseUrl, cookieJar, timeout);

            // Check if a response was returned from the login page request
            if (string.IsNullOrEmpty(loginPageContents))
            {
                throw new ApplicationException("Unable to retrieve login page contents");
            }

            Program.WriteOutput("Authenticating");

            // Use a regular expression to fetch the anti cross site scriping token from the HTML
            var xssToken = Regex.Match(loginPageContents, @"xhr.setRequestHeader\(""X-CSRFToken"", ""(.*)"" \);", RegexOptions.IgnoreCase);
                        
            // Authenticate the session
            var authenticationResult = HttpUtility.AuthenticateViaUrlEncodedFormMethod(string.Concat(pfSenseServer.ServerBaseUrl, "index.php"),
                                                                                       new Dictionary<string, string>
                                                                                       {
                                                                                            {"X-CSRFToken", xssToken.Groups[1].Value }
                                                                                       },
                                                                                       new Dictionary<string, string>
                                                                                       {
                                                                                            { "usernamefld", System.Web.HttpUtility.UrlEncode(pfSenseServer.Username) }, 
                                                                                            { "passwordfld", System.Web.HttpUtility.UrlEncode(pfSenseServer.Password) }, 
                                                                                            { "login", "1" }
                                                                                       },
                                                                                       cookieJar,
                                                                                       timeout);

            // Verify if the username/password combination was valid by examining the server response
            if (authenticationResult.Contains("Wrong username or password"))
            {
                throw new ApplicationException("ERROR: Credentials incorrect");
            }

            Program.WriteOutput("Requesting backup file");

            // Get the backup page contents for the xsrf token
            var backupPageUrl = string.Concat(pfSenseServer.ServerBaseUrl, "diag_backup.php");

            var backupPageContents = HttpUtility.HttpGetLoginPageContents(backupPageUrl, cookieJar, timeout);

            // Check if a response was returned from the login page request
            if (string.IsNullOrEmpty(backupPageContents))
            {
                throw new ApplicationException("Unable to retrieve backup page contents");
            }

            // Use a regular expression to fetch the anti cross site scriping token from the HTML
            xssToken = Regex.Match(backupPageContents, @"xhr.setRequestHeader\(""X-CSRFToken"", ""(.*)"" \);", RegexOptions.IgnoreCase);

            Program.WriteOutput("Retrieving backup file");

            var downloadArgs = new Dictionary<string, string>();

            downloadArgs.Add("download", "Download configuration");
            if (!pfSenseServer.BackupStatisticsData) { downloadArgs.Add("donotbackuprrd", "on"); }
            if(pfSenseServer.EncryptBackup)
            {
                downloadArgs.Add("encrypt", "on");
                downloadArgs.Add("encrypt_password", pfSenseServer.EncryptionPassword);
                downloadArgs.Add("encrypt_passconf", pfSenseServer.EncryptionPassword);
            }
            
            string filename;
            var pfSenseBackupFile = new PfSenseBackupFile
            {
                FileContents = HttpUtility.DownloadBackupFile(  backupPageUrl,
                                                                new Dictionary<string, string>
                                                                    {
                                                                        {"X-CSRFToken", xssToken.Groups[1].Value }
                                                                    },
                                                                downloadArgs,
                                                                cookieJar,
                                                                out filename,
                                                                timeout,
                                                                backupPageUrl),
                FileName = filename
            };
            return pfSenseBackupFile;
        }
    }
}
