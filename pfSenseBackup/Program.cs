using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace KoenZomers.Tools.pfSense.pfSenseBackup
{
    /// <summary>
    /// Application to retrieve the backup file from a pfSense installation
    /// </summary>
    internal class Program
    {
        #region Constants

        /// <summary>
        /// Defines the pfSense version to use if not explicitly specified
        /// </summary>
        private const string DefaultPfSenseVersion = "2.3.3";

        #endregion

        #region Properties

        /// <summary>
        /// Details of the pfSense server to communicate with
        /// </summary>
        public static Protocols.PfSenseServerDetails PfSenseServerDetails = new Protocols.PfSenseServerDetails();

        /// <summary>
        /// The filename which to save the backup file to
        /// </summary>
        public static string OutputFileName { get; set; }

        /// <summary>
        /// Indicates if no output should be sent (true)
        /// </summary>
        public static bool UseSilentMode { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Application entry point
        /// </summary>
        /// <param name="args">Command line arguments</param>
        private static void Main(string[] args)
        {
            // Parse the provided arguments
            if (args.Length > 0)
            {
                ParseArguments(args);
            }

            WriteOutput();

            var appVersion = Assembly.GetExecutingAssembly().GetName().Version;

            WriteOutput("pfSense Backup Tool v{0}.{1}.{2} by Koen Zomers", new object[] { appVersion.Major, appVersion.Minor, appVersion.Build });
            WriteOutput();

            // Check if parameters have been provided
            if (args.Length == 0)
            {
                // No arguments have been provided
                WriteOutput("ERROR: No arguments provided");
                WriteOutput();

                DisplayHelp();

                Environment.Exit(1);
            }

            // Make sure the provided arguments have been provided
            if (string.IsNullOrEmpty(PfSenseServerDetails.Username) ||
                string.IsNullOrEmpty(PfSenseServerDetails.Password) ||
                string.IsNullOrEmpty(PfSenseServerDetails.ServerAddress))
            {
                WriteOutput("ERROR: Not all required options have been provided");

                DisplayHelp();

                Environment.Exit(1);
            }

            // Check if the output filename parsed resulted in an error
            if (!string.IsNullOrEmpty(OutputFileName) && OutputFileName.Equals("ERROR", StringComparison.InvariantCultureIgnoreCase))
            {
                WriteOutput("ERROR: Provided output filename contains illegal characters");

                Environment.Exit(1);
            }

            // Retrieve the backup file from pfSense
            RetrieveBackupFile();

            Environment.Exit(0);
        }

        /// <summary>
        /// Retrieves the backup file from pfSense
        /// </summary>
        private static void RetrieveBackupFile()
        {
            if (PfSenseServerDetails.UseHttps)
            {
                // Ignore all certificate related errors
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                // Also allow TLS 1.1 and TLS 1.2 to be used by the pfSense server. TLS 1.0 and SSLv3 are already allowed by default.
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            }

            // Create a cookie container to hold the session cookies
            var cookieJar = new CookieContainer();

            // Define the protocol implementation to use to communicate with pfSense
            Protocols.IPfSenseProtocol pfSenseProtocol = null;
            switch (PfSenseServerDetails.Version)
            {
                case "1.2":
                    pfSenseProtocol = new Protocols.PfSenseVersion12();
                    break;

                case "2.0":
                    pfSenseProtocol = new Protocols.PfSenseVersion20();
                    break;

                case "2.1":
                case "2.2":
                    pfSenseProtocol = new Protocols.PfSenseVersion21();
                    break;

                case "2.3":
                    pfSenseProtocol = new Protocols.PfSenseVersion23();
                    break;

                case "2.3.3":
                    pfSenseProtocol = new Protocols.PfSenseVersion233();
                    break;

                default:
                    WriteOutput("Unsupported pfSense version provided ({0})", new object[] { PfSenseServerDetails.Version });
                    Environment.Exit(1);
                    break;
            }

            // Execute the communication with pfSense through the protocol implementation
            Protocols.PfSenseBackupFile pfSenseBackupFile = null;
            try
            {
                pfSenseBackupFile = pfSenseProtocol.Execute(PfSenseServerDetails, cookieJar, PfSenseServerDetails.RequestTimeOut.GetValueOrDefault(60000));
            }
            catch (Exception e)
            {
                WriteOutput("Error: {0}", new object[] { e.Message });
                Environment.Exit(1);
            }

            // Verify that the backup file returned contains content
            if (pfSenseBackupFile == null || string.IsNullOrEmpty(pfSenseBackupFile.FileContents))
            {
                WriteOutput("No valid backup contents returned");
                Environment.Exit(1);
            }

            // Define the full path to the file to store the backup in. By default this will be in the same directory as this application runs from using the filename provided by pfSense, unless otherwise specified using the -o parameter.
            string outputDirectory;
            if (string.IsNullOrEmpty(OutputFileName))
            {
                // -o flag has not been provided, store the file in the same directory as this tool runs using the same filename as provided by pfSense
                outputDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), pfSenseBackupFile.FileName);
            }
            else
            {
                // -o flag has been provided, check if just a path was provided or a path including filename
                if (Directory.Exists(OutputFileName))
                {
                    // Path was provided, use the filename as provided by pfSense to store it in the by -o provided path
                    outputDirectory = Path.Combine(OutputFileName, pfSenseBackupFile.FileName);
                }
                else
                {
                    // Complete path including filename has been provided with the -o flag, use that to store the backup
                    outputDirectory = OutputFileName;
                }
            }

            WriteOutput(string.Concat("Saving backup file to ", outputDirectory));

            // Store the backup contents in the file
            WriteBackupToFile(outputDirectory, pfSenseBackupFile.FileContents);
            
            WriteOutput();
            WriteOutput("DONE");
        }
        
        /// <summary>
        /// Writes the backup content to a file
        /// </summary>
        /// <param name="filename">Full file path where to store the file</param>
        /// <param name="backupContents">Contents of the backup to write to the file</param>
        private static void WriteBackupToFile(string filename, string backupContents)
        {
            try
            {
                File.WriteAllText(filename, backupContents, Encoding.UTF8);
            }
            catch (UnauthorizedAccessException)
            {
                WriteOutput("!! Unable to write the backup file to {0}. Make sure the account you use to run this tool has write rights to this location.", new object[] { filename });
            }
            catch(Exception ex)
            {
                WriteOutput("!! Unable to write the backup file to {0}. The error that occurred was: '{1}'", new object[] { filename, ex.Message });
            }
        }

        /// <summary>
        /// Parses all provided arguments
        /// </summary>
        /// <param name="args">String array with arguments passed to this console application</param>
        private static void ParseArguments(IList<string> args)
        {
            UseSilentMode = args.Contains("-silent");

            if (args.Contains("-u"))
            {
                PfSenseServerDetails.Username = args[args.IndexOf("-u") + 1];
            }

            if (args.Contains("-p"))
            {
                PfSenseServerDetails.Password = args[args.IndexOf("-p") + 1];
            }

            if (args.Contains("-s"))
            {
                PfSenseServerDetails.ServerAddress = args[args.IndexOf("-s") + 1];
            }

            if (args.Contains("-o"))
            {
                var outputTo = args[args.IndexOf("-o") + 1];

                // Verify that the target filename does not contain any invalid characters
                try
                {
                    new FileInfo(outputTo);
                    OutputFileName = outputTo;
                }
                catch (ArgumentException)
                {
                    OutputFileName = "ERROR";
                } 
                catch (NotSupportedException)
                {
                    OutputFileName = "ERROR";
                }                
            }

            if (args.Contains("-e"))
            {
                PfSenseServerDetails.EncryptBackup = true;
                PfSenseServerDetails.EncryptionPassword = args[args.IndexOf("-e") + 1];
            }

            if (args.Contains("-t"))
            {
                int timeout;
                if(int.TryParse(args[args.IndexOf("-t") + 1], out timeout))
                {
                    // Input is in seconds, value is in milliseconds, so multiply with 1000
                    PfSenseServerDetails.RequestTimeOut = timeout * 1000;
                }                
            }

            PfSenseServerDetails.Version = args.Contains("-v") ? args[args.IndexOf("-v") + 1] : DefaultPfSenseVersion;

            PfSenseServerDetails.BackupStatisticsData = !args.Contains("-norrd");
            PfSenseServerDetails.BackupPackageInfo = !args.Contains("-nopackage");
            PfSenseServerDetails.UseHttps = args.Contains("-usessl");
        }

        /// <summary>
        /// Shows the syntax
        /// </summary>
        private static void DisplayHelp()
        {
            WriteOutput("Usage:");
            WriteOutput("   pfSenseBackup.exe -u <username> -p <password> -s <serverip> [-v <PFSense Version> -o <filename> -usessl -norrd -nopackage]");
            WriteOutput();
            WriteOutput("u: Username of the account to use to log on to pfSense");
            WriteOutput("p: Password of the account to use to log on to pfSense");
            WriteOutput("s: IP address or DNS name of the pfSense server");
            WriteOutput("v: PFSense version. Supported are 1.2, 2.0, 2.1, 2.2, 2.3 and 2.3.3 (2.3.3 = default, optional)");
            WriteOutput("o: Folder or complete path where to store the backup file (optional)");
            WriteOutput("e: Have pfSense encrypt the backup using this password (optional)");
            WriteOutput("t: Timeout in seconds for pfSense to retrieve the backup (60 seconds = default, optional)");
            WriteOutput("usessl: if provided https will be used to connect to pfSense instead of http");
            WriteOutput("norrd: if provided no RRD statistics data will be included");
            WriteOutput("nopackage: if provided no package info data will be included");
            WriteOutput("silent: if provided no output will be shown");
            WriteOutput();
            WriteOutput("Example:");
            WriteOutput("   pfsenseBackup.exe -u admin -p mypassword -s 192.168.0.1");
            WriteOutput("   pfsenseBackup.exe -u admin -p mypassword -s 192.168.0.1:8000");
            WriteOutput("   pfsenseBackup.exe -u admin -p mypassword -s 192.168.0.1 -usessl");
            WriteOutput("   pfsenseBackup.exe -u admin -p mypassword -s 192.168.0.1 -o c:\\backups -norrd");
            WriteOutput("   pfsenseBackup.exe -u admin -p mypassword -s 192.168.0.1 -o c:\\backups\\pfsense.xml -norrd -nopackage");
            WriteOutput("   pfsenseBackup.exe -u admin -p mypassword -s 192.168.0.1 -o \"c:\\my backups\"");
            WriteOutput("   pfsenseBackup.exe -u admin -p mypassword -s 192.168.0.1 -e \"mypassword\"");
            WriteOutput("   pfsenseBackup.exe -u admin -p mypassword -s 192.168.0.1 -t 120");
            WriteOutput();
            WriteOutput("Output:");
            WriteOutput("   A timestamped file containing the backup will be created within this directory unless -o is being specified");
            WriteOutput();
        }

        /// <summary>
        /// Writes output to the console based on the silent flag being enabled
        /// </summary>
        internal static void WriteOutput()
        {
            WriteOutput(string.Empty);
        }

        /// <summary>
        /// Writes output to the console based on the silent flag being enabled
        /// </summary>
        /// <param name="text">Text to write to the console</param>
        internal static void WriteOutput(string text)
        {
            // Check if silent mode is enabled, if so, return and do not write the output
            if (UseSilentMode) return;

            // Silent mode is not enabled, write the output
            Console.WriteLine(text);
        }

        /// <summary>
        /// Writes output to the console based on the silent flag being enabled
        /// </summary>
        /// <param name="format">Formatted string to write to the console</param>
        /// <param name="args">Arguments to inser into the formatted string</param>
        internal static void WriteOutput(string format, object[] args)
        {            
            WriteOutput(string.Format(format, args));
        }

        /// <summary>
        /// Sends a POST request using the multipart form data method to download the pfSense backup file
        /// </summary>
        /// <param name="url">Url to POST the backup file request to</param>
        /// <param name="userName">userName</param>
        /// <param name="userPassword">userPassword</param>
        /// <param name="formFields">Dictonary with key/value pairs containing the forms data to POST to the webserver</param>
        /// <param name="cookieContainer">Cookies which have been recorded for this session</param>
        /// <param name="filename">Filename of the download as provided by pfSense (out parameter)</param>
        /// <returns>The website contents returned by the webserver after posting the data</returns>
        public static string DownloadBackupFile(string url, string userName, string userPassword, Dictionary<string, string> formFields, CookieContainer cookieContainer, out string filename)
        {
            filename = null;

            // Define the form separator to use in the POST request
            const string formDataBoundary = "---------------------------7dc1873b1609fa";

            // Construct the POST request which performs the login
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Accept = "*/*";
            request.ServicePoint.Expect100Continue = false;
            request.CookieContainer = cookieContainer;

            SetBasicAuthHeader(request, userName, userPassword);

            // Construct POST data
            var postData = new StringBuilder();
            foreach (var formField in formFields)
            {
                postData.AppendLine(string.Concat("--", formDataBoundary));
                postData.AppendLine(string.Format("Content-Disposition: form-data; name=\"{0}\"", formField.Key));
                postData.AppendLine();
                postData.AppendLine(formField.Value);
            }
            postData.AppendLine(string.Concat("--", formDataBoundary, "--"));

            // Convert the POST data to a byte array
            var postDataByteArray = Encoding.UTF8.GetBytes(postData.ToString());

            // Set the ContentType property of the WebRequest
            request.ContentType = string.Concat("multipart/form-data; boundary=", formDataBoundary);

            // Set the ContentLength property of the WebRequest.
            request.ContentLength = postDataByteArray.Length;

            // Get the request stream
            var dataStream = request.GetRequestStream();

            // Write the POST data to the request stream
            dataStream.Write(postDataByteArray, 0, postDataByteArray.Length);

            // Close the Stream object
            dataStream.Close();

            // Receive the response from the webserver
            HttpWebResponse response = null;

            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException wex)
            {

                response = (HttpWebResponse)wex.Response;
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    WriteOutput("ERROR: Credentials incorrect");
                    Environment.Exit(1);
                }

                WriteOutput("ERROR: {0}", new object[] { wex.Message });
                Environment.Exit(1);

            }
            catch (Exception ex)
            {
                WriteOutput("ERROR: {0}", new object[] { ex.Message });
                Environment.Exit(1);
            }

            // Make sure the webserver has sent a response
            if (response == null) return null;

            dataStream = response.GetResponseStream();

            // Make sure the datastream with the response is available
            if (dataStream == null) return null;

            // Get the content-disposition header and use a regex on its value to find out what filename pfSense assigns to the download
            var contentDispositionHeader = response.Headers["Content-Disposition"];
            var filenameRegEx = Regex.Match(contentDispositionHeader, @"filename=(?<filename>.*)(?:\s|\z)");

            if (filenameRegEx.Success && filenameRegEx.Groups["filename"].Success)
            {
                filename = filenameRegEx.Groups["filename"].Value;
            }

            var reader = new StreamReader(dataStream);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// Forcing basic http authentication for HttpWebRequest (in .NET/C#) 
        /// http://blog.kowalczyk.info/article/Forcing-basic-http-authentication-for-HttpWebReq.html
        /// </summary>
        /// <param name="req">HttpWebRequest to add Authorization Header</param>
        /// <param name="userName">UserName of PFSense</param>
        /// <param name="userPassword">Password of PFSense</param>
        public static void SetBasicAuthHeader(HttpWebRequest req, string userName, string userPassword)
        {
            string authInfo = userName + ":" + userPassword;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            req.Headers["Authorization"] = "Basic " + authInfo;
        }

        #endregion
    }
}
