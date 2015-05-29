using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace KoenZomers.Tools.pfSense.pfSenseBackup
{
    /// <summary>
    /// Internal utility class for Http communication with pfSense
    /// </summary>
    internal static class HttpUtility
    {
        /// <summary>
        /// Performs a HEAD request to the provided url to have the remote webserver hand out a new sessionId
        /// </summary>
        /// <param name="url">Url to query</param>
        /// <param name="cookieContainer">Cookies which have been recorded for this session</param>
        public static void HttpCreateSession(string url, CookieContainer cookieContainer)
        {
            // Construct the request
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "HEAD";
            request.CookieContainer = cookieContainer;

            // Send the request to the webserver
            request.GetResponse();
        }

        /// <summary>
        /// Performs a GET request to the provided url to have the remote webserver hand out a new sessionId and return the login page contents
        /// </summary>
        /// <param name="url">Url of the login page</param>
        /// <param name="cookieContainer">Cookies which have been recorded for this session</param>
        /// <returns>Contents of the login page which can be used to parse i.e. anti-XSS tokens</returns>
        public static string HttpGetLoginPageContents(string url, CookieContainer cookieContainer)
        {
            // Construct the request
            var request = (HttpWebRequest)WebRequest.Create(url);
            //request.Method = "HEAD";
            request.CookieContainer = cookieContainer;

            // Send the request to the webserver
            var response = request.GetResponse();

            // Get the stream containing content returned by the server.
            var dataStream = response.GetResponseStream();
            if (dataStream == null) return null;
            
            // Open the stream using a StreamReader for easy access.
            var reader = new StreamReader(dataStream);
            
            // Read the content returned
            var responseFromServer = reader.ReadToEnd();
            return responseFromServer;
        }

        /// <summary>
        /// Sends a POST request using the url encoded form method to authenticate to pfSense
        /// </summary>
        /// <param name="url">Url to POST the login information to</param>
        /// <param name="formFields">Dictonary with key/value pairs containing the forms data to POST to the webserver</param>
        /// <param name="cookieContainer">Cookies which have been recorded for this session</param>
        /// <returns>The website contents returned by the webserver after posting the data</returns>
        public static string AuthenticateViaUrlEncodedFormMethod(string url, Dictionary<string, string> formFields, CookieContainer cookieContainer)
        {
            // Construct the POST request which performs the login
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Accept = "*/*";
            request.ServicePoint.Expect100Continue = false;
            request.CookieContainer = cookieContainer;

            // Construct POST data
            var postData = new StringBuilder();
            foreach (var formField in formFields)
            {
                if (postData.Length > 0) postData.Append("&");
                postData.Append(formField.Key);
                postData.Append("=");
                postData.Append(formField.Value);
            }

            // Convert the POST data to a byte array
            var postDataByteArray = Encoding.UTF8.GetBytes(postData.ToString());

            // Set the ContentType property of the WebRequest
            request.ContentType = "application/x-www-form-urlencoded";

            // Set the ContentLength property of the WebRequest.
            request.ContentLength = postDataByteArray.Length;

            // Get the request stream
            var dataStream = request.GetRequestStream();

            // Write the POST data to the request stream
            dataStream.Write(postDataByteArray, 0, postDataByteArray.Length);

            // Close the Stream object
            dataStream.Close();

            // Receive the response from the webserver
            var response = request.GetResponse() as HttpWebResponse;

            // Make sure the webserver has sent a response
            if (response == null) return null;

            dataStream = response.GetResponseStream();

            // Make sure the datastream with the response is available
            if (dataStream == null) return null;

            var reader = new StreamReader(dataStream);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// Sends a POST request using the multipart form data method to download the pfSense backup file
        /// </summary>
        /// <param name="url">Url to POST the backup file request to</param>
        /// <param name="formFields">Dictonary with key/value pairs containing the forms data to POST to the webserver</param>
        /// <param name="cookieContainer">Cookies which have been recorded for this session</param>
        /// <param name="filename">Filename of the download as provided by pfSense (out parameter)</param>
        /// <returns>The website contents returned by the webserver after posting the data</returns>
        public static string DownloadBackupFile(string url, Dictionary<string, string> formFields, CookieContainer cookieContainer, out string filename)
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
            var response = request.GetResponse() as HttpWebResponse;
            
            // Make sure the webserver has sent a response
            if (response == null) return null;            

            dataStream = response.GetResponseStream();

            // Make sure the datastream with the response is available
            if (dataStream == null) return null;

            // Get the content-disposition header and use a regex on its value to find out what filename pfSense assigns to the download
            var contentDispositionHeader = response.Headers["Content-Disposition"];
            
            // Verify that a content disposition header was returned
            if (contentDispositionHeader == null) return null;

            var filenameRegEx = Regex.Match(contentDispositionHeader, @"filename=(?<filename>.*)(?:\s|\z)");
            
            if(filenameRegEx.Success && filenameRegEx.Groups["filename"].Success)
            {
                filename = filenameRegEx.Groups["filename"].Value;
            }

            var reader = new StreamReader(dataStream);
            return reader.ReadToEnd();
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
                    System.Console.WriteLine("ERROR: Credentials incorrect");
                    System.Environment.Exit(1);
                }

                System.Console.WriteLine("ERROR: {0}", wex.Message);
                System.Environment.Exit(1);

            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("ERROR: {0}", ex.Message);
                System.Environment.Exit(1);
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
            authInfo = System.Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            req.Headers["Authorization"] = "Basic " + authInfo;
        }
    }
}
