using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

using log4net;
using Newtonsoft.Json.Linq;

using SALTShaker.DAL;
using SALTShaker.DAL.DataContracts;

namespace SALTShaker.BLL
{
    public class ContentRepository : IDisposable, IContentRepository
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(MappingExtensions));

        #region SAL api calls
        public ContentRepository()
        {
        }
        public List<ContentModel> GetContentItems()
        {
            var host = ConfigurationManager.AppSettings["endecaHost"].ToString();
            var port = ConfigurationManager.AppSettings["endecaPort"].ToString();
            var dimensionValues = ConfigurationManager.AppSettings["endecaDimensionValue"].ToString();
            var query = ConfigurationManager.AppSettings["endecaContentQuery"].ToString();

            string url = "http://" + host + ":" + port + query + BuildQueryString(dimensionValues);

            List<EndecaContentModel> endecaContentModelList = new List<EndecaContentModel>();
            try
            {
                //RESOURCE_LEAK
                WebRequest reqEndeca = WebRequest.Create(url);
                using (HttpWebResponse resEndeca = (HttpWebResponse)reqEndeca.GetResponse())
                {
                    {
                        // Get the stream containing content returned by the server.
                        JObject endecaAsJSON;
                        using (resEndeca)
                        {
                            Stream receiveStream = resEndeca.GetResponseStream();
                            if (receiveStream != null)
                            {
                                using (StreamReader streamReader = new StreamReader(resEndeca.GetResponseStream()))
                                {
                                    endecaAsJSON = JObject.Parse(streamReader.ReadToEnd());

                                    try
                                    {
                                        //if we have results fill in our contentObject
                                        if (endecaAsJSON["mainContent"][0]["records"].Count() > 0)
                                        {
                                            //Go over each result returned from Endeca
                                            foreach (JToken jsonRecord in endecaAsJSON["mainContent"][0]["records"])
                                            {
                                                var endecaContentModel = new EndecaContentModel();
                                                endecaContentModel.ContentType = jsonRecord["attributes"]["ContentTypes"][0].ToString();
                                                endecaContentModel.PageTitle = jsonRecord["attributes"]["page_title"][0].ToString();
                                                endecaContentModel.PrimaryKey = jsonRecord["attributes"]["P_Primary_Key"][0].ToString();
                                                endecaContentModelList.Add(endecaContentModel);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.Error("GetContentItems failed Exception:", ex);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (WebException we)
            {
                //just logging error, allowing for empty object to be returned. No need to blow up process
                logger.Error(we.Message);
                logger.Error("Failed trying to hit: " + url);
            }
            catch (Exception ex)
            {
                //just logging error, allowing for empty object to be returned. No need to blow up process
                logger.Error("GetContentItems Exception trying to hit: " + url + ", error: " + ex.Message);
            }

            return new ContentModel().ToContentModel(endecaContentModelList);
        }
        public string BuildQueryString(string commaSepratedDimension)
        {
            string queryString = "Nrs=" + BuildContentDimensionSearchString(commaSepratedDimension);
            //encode string
            var updatedQueryString = HttpUtility.ParseQueryString(queryString);
            return "?" + updatedQueryString.ToString();
        }
        /// <summary>
        /// Build dimension string for Endeca query cloned from SearchServce Endeca Utility
        /// </summary>
        /// <param name="commaSepratedDimension"></param>
        /// <returns></returns>
        public string BuildContentDimensionSearchString(string commaSepratedDimension)
        {
            if (string.IsNullOrEmpty(commaSepratedDimension))
            {
                //These are the only content types from the filter page
                //41: 'Article',
                //42: 'Video',
                //43: 'Form',
                //44: 'Lesson',
                //45: 'Infographic',
                //46: 'Tool',
                //157: 'Comic',
                //303: 'Ebook',
                //304: 'Course'
                string dimString = "collection()/record[ContentTypes=endeca:dval-by-id(41)//id or ContentTypes=endeca:dval-by-id(42)//id or ContentTypes=endeca:dval-by-id(43)//id or ContentTypes=endeca:dval-by-id(44)//id or ContentTypes=endeca:dval-by-id(45)//id or ContentTypes=endeca:dval-by-id(46)//id or ContentTypes=endeca:dval-by-id(157)//id or ContentTypes=endeca:dval-by-id(303)//id or ContentTypes=endeca:dval-by-id(304)//id]";
                
                return dimString;
            }
            
            string[] dimensionArray = commaSepratedDimension.Split(new string[] { "," }, StringSplitOptions.None);
            string dimensionsStartString = "collection()/record[";
            string stringEnding = "]";
            List<String> contentTypeSnippets = new List<String>();
            foreach (string dim in dimensionArray)
            {
                contentTypeSnippets.Add("ContentTypes=endeca:dval-by-id(" + dim + ")//id");
            }
            string dimensionsMiddleString = string.Join(" or ", contentTypeSnippets);

            return dimensionsStartString + dimensionsMiddleString + stringEnding;
        }
        #endregion
        //For IDisposable interface
        #region IDisposable interface
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    //context.Dispose();
                }
            }
            this.disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
