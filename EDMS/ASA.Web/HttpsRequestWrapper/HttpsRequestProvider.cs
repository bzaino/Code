using System;
using System.IO;
using System.Net;
using System.Security;
using System.Text;

namespace HttpsRequestWrapper
{
    [SecuritySafeCritical]
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")] 
    public class HttpsRequestProvider
    {
        private const string Classname = "ASA.Web.HttpsRequestWrapper.HttpsRequestProvider";
        //using log4net here as opposed to ASA.Log.ServiceLogger.IASALog because IASALog is not strongly 
        //named and can't be used in the GAC
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(Classname);

        [SecuritySafeCritical]
        public String postChoicesToUnigo(byte[] choicesAsBytes, string urlToRequest)
        {
            //Add config based url back
            HttpWebRequest outgoingRequest = (HttpWebRequest)WebRequest.Create(urlToRequest);
            // Set the necessary headers of the data being posted.
            outgoingRequest.ContentType = "application/json";
            outgoingRequest.Method = "POST";
            outgoingRequest.ContentLength = choicesAsBytes.Length;
            outgoingRequest.KeepAlive = false;
            //int to empty 
            String responseAsString = String.Empty;
            try
            {
                using (Stream newStream = outgoingRequest.GetRequestStream())
                {
                    //Write our bytes to the outgoing stream
                    newStream.Write(choicesAsBytes, 0, choicesAsBytes.Length);
                    using (HttpWebResponse unigoResponse = (HttpWebResponse)outgoingRequest.GetResponse())
                    //Get a response from unigo and stream it back to the client
                    using (var responseStream = unigoResponse.GetResponseStream())
                    {
                        if (responseStream != null)
                            using (var sr = new StreamReader(responseStream))
                            {
                                //Write the stream to a string to pass back to the calling method
                                responseAsString = sr.ReadToEnd();
                            }
                    } 
                }
            }
            //The following commented out catch block helps in debugging/getting specific messages but we don't wont it active
            //as that means this method returns gracefuly without throwing an exception (ok response on client) since it gets handled in here, 
            //which would be a false positive and would make it difficult to know issues on the api call from client side
            //catch (WebException webex)
            //{
            //    WebResponse errResp = webex.Response;
            //    using (Stream respStream = errResp.GetResponseStream())
            //    {
            //        StreamReader reader = new StreamReader(respStream);
            //        string text = reader.ReadToEnd();
            //        Log.Debug(".postChoicesToUnigo(byte[] choicesAsBytes WebException: " + text);
            //    }
            //}
            catch (Exception ex)
            {
                Log.Debug(".postChoicesToUnigo(byte[] choicesAsBytes Exception: ", ex);
                throw new Exception("Exception trying to postChoicesToUnigo from HttpsRequestWrapper: \n" + ex);
            }
            return responseAsString;
        }

        [SecuritySafeCritical]
        public string getScholarshipData(string urlToRequest)
        {
            //int to empty 
            String responseAsString = String.Empty;
            for (int i = 0; i < Config.RequestRetryCount; i++)
            {
                try
                {
                    //Add config based url back
                    HttpWebRequest outgoingRequest = (HttpWebRequest)WebRequest.Create(urlToRequest);
                    // Set the necessary headers of the data being posted.
                    outgoingRequest.ContentType = "application/json";
                    outgoingRequest.Method = "GET";
                    outgoingRequest.ContentLength = 0;
                    outgoingRequest.KeepAlive = false;
                    outgoingRequest.Timeout = Config.IndividualScholarshipTimeout;

                    using (HttpWebResponse unigoResponse = (HttpWebResponse) outgoingRequest.GetResponse())
                    {
                        using (var responseStream = unigoResponse.GetResponseStream())
                        {
                            if (responseStream != null)
                            {
                                using (StreamReader sr = new StreamReader(responseStream))
                                {
                                    //Write the stream to a string to pass back to the calling method
                                    responseAsString = sr.ReadToEnd();
                                }
                            }
                        }
                    }
                    break;
                }
                catch (Exception ex)
                {

                    if (i == (Config.RequestRetryCount - 1))
                    {
                        Log.Error(String.Format(".Tried {0} times, no response, throwing the exception: ", Config.RequestRetryCount), ex);
                        throw new Exception("Exception trying to getScholarshipData from HttpsRequestWrapper: \n" + ex);
                    }
                    else
                    {
                        Log.Debug(String.Format("Did not get a response on attempt number {0}, going to retry.  Exception Message: ", (i + 1)), ex);
                    }
                }
            }
            return responseAsString;
        }

        [SecuritySafeCritical]
        private static HttpWebRequest CreateWebRequest(string requestUrl, string requestType, string contentType, byte[] dataAsBytes, bool bKeepAlive)
        {
            HttpWebRequest outgoingRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
            // Set the necessary headers of the data being posted.
            outgoingRequest.Method = requestType;
            outgoingRequest.ContentType = contentType;
            outgoingRequest.ContentLength = dataAsBytes.Length;
            outgoingRequest.KeepAlive = bKeepAlive;
            return outgoingRequest;
        }

        [SecuritySafeCritical]
        private static string MakeRequest(HttpWebRequest outgoingRequest, byte[] DataAsBytes, string responseEncodingFormat = "utf-8")
        {
            string response = String.Empty;

            if (DataAsBytes.Length > 0)
            {
                using (Stream newStream = outgoingRequest.GetRequestStream())
                {
                    //Write our bytes to the outgoing stream
                    newStream.Write(DataAsBytes, 0, DataAsBytes.Length);
                }
            }
            using (HttpWebResponse httpResponse = (HttpWebResponse)outgoingRequest.GetResponse())
            {
                //Get a response from unigo and stream it back to the client
                using (var responseStream = httpResponse.GetResponseStream())
                {
                    Encoding encoding = System.Text.Encoding.GetEncoding(responseEncodingFormat);
                    if (responseStream != null)
                    {
                        using (StreamReader sr = new StreamReader(responseStream, encoding))
                        {
                            //Write the stream to a string to pass back to the calling method
                            response = sr.ReadToEnd();
                            sr.Close();
                        }
                        responseStream.Close();
                    }
                }
            }

            return response;
        }

        [SecuritySafeCritical]
        public string DoHttpsWebRequest(HttpWebRequest webRequest, byte[] dataAsBytes, string responseEncodingFormat = "utf-8")
        {
            String responseAsString = String.Empty;
            try
            {
                responseAsString = MakeRequest(webRequest, dataAsBytes, responseEncodingFormat);
            }
            catch (Exception ex)
            {
                Log.Debug(".DoHttpsWebRequest(HttpWebRequest webRequest, byte[] dataAsBytes, string responseEncodingFormat) Exception: ", ex);
                throw new Exception("Exception trying to DoHttpsWebRequest from HttpsRequestWrapper: \n" + ex);
            }
            return responseAsString;
        }

        [SecuritySafeCritical]
        public string DoHttpsWebRequest(string requestUrl, string requestType, string contentType, byte[] dataAsBytes, bool bKeepAlive, string responseEncodingFormat = "utf-8")
        {
            HttpWebRequest outgoingRequest = CreateWebRequest(requestUrl, requestType, contentType, dataAsBytes, bKeepAlive);
            String responseAsString = String.Empty;
            try
            {
                responseAsString = MakeRequest(outgoingRequest, dataAsBytes, responseEncodingFormat);
            }
            catch (Exception ex)
            {
                Log.Debug(".DoHttpsWebRequest(string requestUrl, string requestType, string contentType, byte[] dataAsBytes, bool bKeepAlive, Encoding responseEncodingFormat) Exception: ", ex);
                throw new Exception("Exception trying to DoHttpsWebRequest from HttpsRequestWrapper: \n" + ex);
            }
            return responseAsString;
        }

        /// <summary>
        /// sends a request to Qualtric's Target Audience API
        /// </summary>
        /// <param name="urlToRequest">foramtted url string to send to Qualtrics</param>
        /// <returns>response </returns>
        //[SecuritySafeCritical]
        public String sendRequestToQualtricsTA(string urlToRequest)
        {
            const string logMethodName = ".sendRequestToQualtricsTA(string urlToRequest) - ";
            Log.Debug(logMethodName + "Begin Method");

            HttpWebRequest outgoingRequest = (HttpWebRequest)WebRequest.Create(urlToRequest);
            // Set the necessary headers of the data being posted.
            outgoingRequest.ContentType = "application/json";
            outgoingRequest.Method = "GET";
            //outgoingRequest.ContentLength = choicesAsBytes.Length;
            outgoingRequest.KeepAlive = false;
            String responseAsString = String.Empty;
            try
            {
                using (HttpWebResponse qualtricsTAResponse = (HttpWebResponse)outgoingRequest.GetResponse())
                //Get a response from qualtricsTA and stream it back to the client
                using (var responseStream = qualtricsTAResponse.GetResponseStream())
                {
                    if (responseStream != null)
                        using (var sr = new StreamReader(responseStream))
                        {
                            //Write the stream to a string to pass back to the calling method
                            responseAsString = sr.ReadToEnd();
                        }
                }
            }
            catch (Exception ex)
            {
                Log.Error(logMethodName + " Exception: ", ex);
                throw new Exception("Exception trying to QualtricsTA from HttpsRequestWrapper: \n" + ex);
            }

            Log.Debug(logMethodName + "End Method");
            return responseAsString;
        }
    }
}
