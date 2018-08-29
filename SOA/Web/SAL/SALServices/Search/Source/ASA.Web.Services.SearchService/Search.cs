using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using System.Xml;

using HttpsRequestWrapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Asa.Salt.Web.Common.Types.Enums;
using ASA.Web.Services.ASAMemberService;
using ASA.Web.Services.ASAMemberService.ServiceContracts;
using ASA.Web.Services.Common;
using ASA.Web.Services.Proxies;
using ASA.Web.Services.SearchService.DataContracts;
using ASA.Web.Services.SearchService.ServiceContracts;
using ASA.Web.Services.SearchService.Validation;

using Member = ASA.Web.Services.ASAMemberService.DataContracts;
using MemberOrganizationModel = ASA.Web.Services.ASAMemberService.DataContracts.MemberOrganizationModel;
using MemberToDoModel = ASA.Web.Services.ASAMemberService.DataContracts.MemberToDoModel;
using CommonConfig = ASA.Web.Services.Common.Config;


namespace ASA.Web.Services.SearchService
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Search
    {

        #region Search API calls

        private static readonly ASA.Log.ServiceLogger.IASALog _log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IAsaMemberAdapter _memberAdapter = null;
        private readonly ISearchAdapter _searchAdapter = null;
        private EndecaUtility endecaUtility = new EndecaUtility();

        public Search()
        {
            _log.Info("ASA.Web.Services.SearchService.Search() object being created ...");
            if (ASA.Web.Services.Common.Config.Testing)
            {
                _memberAdapter = null;
                _searchAdapter = new MockSearchAdapter();
            }
            else
            {
                _memberAdapter = new AsaMemberAdapter();
                _searchAdapter = new SearchAdapter();
            }
        }

        [OperationContract]
        [WebGet(UriTemplate = "GetSchools?filter={filter}&offset={offset}&rows={rows}", ResponseFormat = WebMessageFormat.Json)]
        [AspNetCacheProfile("DoNotCache")]
        public SearchResultsModel GetSchools(string filter, int offset, int rows)
        {
            SearchResultsModel srm = null;

            srm = GetOrganizations(filter, offset, rows, OrganizationTypes.SCHL);

            return srm;
        }

        [OperationContract]
        [WebGet(UriTemplate = "GetOrganizations?filter={filter}&offset={offset}&rows={rows}&type={type}", ResponseFormat = WebMessageFormat.Json)]
        [AspNetCacheProfile("DoNotCache")]
        public SearchResultsModel GetOrganizations(string filter, int offset, int rows, OrganizationTypes type = OrganizationTypes.SCHL)
        {
            SearchResultsModel srm = null;
            var orgTypeList = new string[] { OrganizationTypes.SCHL.ToString() };

            if (type != OrganizationTypes.SCHL)
            {
                orgTypeList = new string[] {
                    UtilityFunctions.GetEnumDescription(OrganizationTypes.TPDP),  OrganizationTypes.WELL.ToString(),
                    OrganizationTypes.STAG.ToString(), OrganizationTypes.GUAR.ToString(), OrganizationTypes.LEND.ToString(),
                    OrganizationTypes.SERV.ToString(), OrganizationTypes.VEND.ToString()
                };
            }

            //Changed as part of SALT decoupling project to get the list from the SALT Service
            var orgList = SaltServiceAgent.GetOrganizations(filter, orgTypeList, rows, offset);
            srm = TranslateSearchResultsModel.FromOrgContractList(orgList.Organizations.ToList());
            srm.TotalRecords = orgList.TotalOrgCount;
            srm.Offset = offset;

            return srm;
        }

        [OperationContract]
        [WebGet(UriTemplate = "Organization/{orgId}/Products", ResponseFormat = WebMessageFormat.Json)]
        public IList<OrganizationProductModel> GetOrganizationProducts(string orgId)
        {
            int organizationId = -1;
            if (!String.IsNullOrEmpty(orgId))
                organizationId = Convert.ToInt32(orgId);

            IList<OrganizationProductModel> productList = new List<OrganizationProductModel>();

            if (organizationId != -1)
                productList = _searchAdapter.GetOrganizationProducts(organizationId);

            return productList;
        }

        [OperationContract]
        [WebGet(UriTemplate = "DetermineByRecommendedVisibility/{memberId}", ResponseFormat = WebMessageFormat.Json)]
        public bool ShowRecommendedSorting(string memberId)
        {
            if (_memberAdapter.IsCurrentUser(memberId))
            {
                List<ASAMemberService.DataContracts.ProfileResponseModel> responses = _memberAdapter.GetProfileResponses(int.Parse(memberId));
                if (responses.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        private string RenderWidget(ASA.Web.Services.SearchService.DataContracts.SearchRecordModel record)
        {
            string strReturn = "";

            if (HttpContext.Current.Cache[record.Fields["P_Tile_Xml_Path"][0]] != null &&
                HttpContext.Current.Cache[record.Fields["P_Tile_Xml_Path"][0]].ToString() != "")
            {

                strReturn = HttpContext.Current.Cache[record.Fields["P_Tile_Xml_Path"][0]].ToString();
            }
            else
            {
                string name = "";
                string path = HttpContext.Current.Server.MapPath("/PublishedContent" + record.Fields["P_Tile_Xml_Path"][0]);

                if (File.Exists(path))
                {
                    using (XmlTextReader reader = new XmlTextReader(path))
                    {
                        if (reader != null)
                        {
                            try
                            {
                                _log.Debug("begin reading from the XmlReader");
                                //position cursor at first properly readable XML element
                                reader.MoveToContent();

                                bool bFoundElement = false;
                                while (reader.Read())
                                {
                                    switch (reader.NodeType)
                                    {
                                        case XmlNodeType.Element:
                                            if (reader.Name == "content")
                                            {
                                                //read again to retrieve data for "content"
                                                reader.Read();
                                                name = reader.Value;
                                                bFoundElement = true;
                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                    if (bFoundElement)
                                        break;
                                }

                                if (!bFoundElement)
                                {
                                    _log.Error("ASA.Web.Services.SearchService.Search.RenderWidget():  Search key not found.");
                                }
                                else
                                {
                                    //remove chars needed for razor templates
                                    name = name.Replace("|%~", "");
                                    name = name.Replace("%|", "");
                                    HttpContext.Current.Cache[record.Fields["P_Tile_Xml_Path"][0]] = name;
                                    strReturn = name;
                                }
                            }
                            catch (Exception ex)
                            {
                                _log.Error("ASA.Web.Services.SearchService.RenderWidget(): Exception =>" + ex.ToString());
                                //assume reader is null
                                strReturn = "";
                                throw new Exception("Web Search Service - RenderWidget()", ex);
                            }
                            finally
                            {
                                _log.Debug("closing the XmlReader");
                                reader.Close();
                            }
                        }
                    }
                }
            }
            return strReturn;
        }

        [Obsolete("Method has been deprecated")]
        [OperationContract]
        [WebGet(UriTemplate = "GetAdTiles?contentTypes={contentTypes}&size={size}&layoutType={layoutType}&numrecords={numrecords}", ResponseFormat = WebMessageFormat.Json)]
        [AspNetCacheProfile("DoNotCache")]
        public string GetAdTiles(string contentTypes, string size, string layoutType, int numrecords = 1)
        {
            const string htmlReturnString = "Method has been deprecated";
            return htmlReturnString;
        }

        [OperationContract]
        [WebGet(UriTemplate = "GetMedia/{*request}", ResponseFormat = WebMessageFormat.Json)]
        [AspNetCacheProfile("DoNotCache")]
        public Stream GetMedia(string request)
        {
            Stream dataStream = new MemoryStream();
            if (WebOperationContext.Current != null)
                WebOperationContext.Current.OutgoingResponse.ContentType = "application/json";
            // Don't know why this doesn't come with the wildcard parameter in the template, but here's another
            // way to get the query string
            if (WebOperationContext.Current != null)
            {
                SearchQueryModel searchModel = new SearchQueryModel(WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri.Query, request);
                string search = searchModel.query;

                int memberId = _memberAdapter.GetMemberIdFromContext();
                bool isAuthorized = memberId != 0;
                bool includeCourses = false;
                Dictionary<string, MemberToDoModel> todos = new Dictionary<string, MemberToDoModel>();

                if (isAuthorized)
                {
                    todos = _memberAdapter.GetToDoDictionary(memberId);
                    IList<MemberOrganizationModel> organizations = _memberAdapter.GetMemberOrganizations(memberId);
                    includeCourses = _searchAdapter.IsProductActive(organizations, 2);
                }

                /*if the request is not from sort control bar or predictive search, we don't execute updateFilterParamter function.*/
                if (search.Contains("Dims=") || search.Contains("Type=") || (search.Contains("Ns=") && !request.EndsWith("blog")) || request.EndsWith("predictiveSearch"))
                {
                    search = endecaUtility.updateFilterParameter(search, memberId, false);
                }
                else
                {
                    string hideRecordString = endecaUtility.FormatHideRecordStringWithCourses(memberId, search, includeCourses);
                    search = endecaUtility.UpdateQueryString(search, "Nr", hideRecordString);
                }
                
                if (search.Length > Config.SearchApiLimit)
                {
                    _log.Error("Search request " + search + " exceeded max allowed length of " + Config.SearchApiLimit);
                    throw new WebException("Search error: Search string exceeded maximum allowed length");
                }

                //If we are requesting the dashboard homepage, we need to add the primary keys for all of the user's todos as the Rsel parameter
                //The R parameter needs to be set to 0 to tell endeca that we are using the "Featured Records Selector"
                //https://docs.oracle.com/cd/E51273_03/ToolsAndFrameworks.110/pdf/AsmAppDevGuide.pdf
                //This check needs to come after we check the "search" length, because the todo string is unbounded (the user could make every item on the site a todo), and will likely pass the 1200 character limit
                if (request.IndexOf("DashboardPrototypeMK") > -1)
                {
                    //We want to set the Rsel parameter to 0 when there are no todos, so that endeca doesnt return 500 records in the OpenTodos cartridge, since there is no filter
                    string records;
                    if (todos.Count > 0)
                    {
                        records = String.Join(",", todos.Keys);
                    }
                    else
                    {
                        records = "0";
                    }
                    search = endecaUtility.UpdateQueryString(search, "R", "0");
                    search = endecaUtility.UpdateQueryString(search, "Rsel", records);

                    //Check for dashboard cookies, which we want to add to the Endeca_user_segments param
                    var requestCookies = HttpContext.Current.Request.Cookies;

                    // Capture all cookie names into a string array.
                    String[] cookieKeys = requestCookies.AllKeys;
                    //Create a string to be built up and added to the Endeca_User_Segments param
                    String segmentsToUse = "";

                    // Grab individual cookie objects by cookie name.
                    for (var i = 0; i < cookieKeys.Length; i++)
                    {
                        var candidateCookie = requestCookies[cookieKeys[i]];
                        //The length restriction is a way to keep potentially malicious users from trying to stuff large amounts of text into a cookie
                        //We wont pass through to endeca any cookie that is too long
                        if (candidateCookie.Value.IndexOf("Dashboard-") == 0 && candidateCookie.Value.Length < Config.CookieMaxLength)
                        {
                            segmentsToUse += candidateCookie.Value + "|";
                        }
                        
                    }

                    if (!String.IsNullOrEmpty(segmentsToUse))
                    {
                        //Remove the trailing pipe from the segment string.  Not strictly necessary, as Endeca interprets it properly with a trailing pipe
                        segmentsToUse = segmentsToUse.Substring(0, segmentsToUse.Length - 1);
                        //See if we have a user segment param coming from the front end
                        //If we have one, combine it with the segmentsToUse for dashboard
                        //If there isn't one, just use the segmentsToUse variable
                        string UserSegmentParameter = endecaUtility.GetParameterByNameFromString("Endeca_user_segments", search);
                        if (!String.IsNullOrEmpty(UserSegmentParameter))
                        {
                            UserSegmentParameter += "|" + segmentsToUse;
                            search = endecaUtility.UpdateQueryString(search, "Endeca_user_segments", UserSegmentParameter);
                        }
                        else
                        {
                            search = endecaUtility.UpdateQueryString(search, "Endeca_user_segments", segmentsToUse);
                        }
                    }
                }

                string url = "http://" + Config.ITLHost + ":" + Config.ITLPort.ToString() + "/" + request + search;
                try
                {
                    //RESOURCE_LEAK
                    WebRequest reqEndeca = WebRequest.Create(url);
                    using (HttpWebResponse resEndeca = (HttpWebResponse)reqEndeca.GetResponse())
                    {
                        if (isAuthorized && todos.Count > 0 && !request.EndsWith("glossary") && !request.EndsWith("predictiveSearch"))
                        {
                            // Call this method to add the necessary todo data to the endeca data we have (RefToDoStatus, RefToDoType, etc)
                            return decorateTodoRecordProperties(resEndeca, todos);
                        }
                        else
                        {
                            // Get the stream containing content returned by the server.
                            using (StreamReader streamReader = new StreamReader(resEndeca.GetResponseStream(), true))
                            {
                                dataStream = new MemoryStream(Encoding.UTF8.GetBytes(streamReader.ReadToEnd() ?? ""));
                            } 
                        }
                    }
                }
                catch (WebException we)
                {
                    _log.Error(we.Message);
                    _log.Error("Failed trying to hit: " + url);
                    //re-throw the original, preserving the stack trace "we" is not required
                    throw;
                }
                catch (Exception ex)
                {
                    _log.Error("GetMedia Exception trying to hit: " + url + ", error: " + ex.Message);
                    //re-throw the original, preserving the stack trace "ex" is not required
                    throw;
                }

            }
            return dataStream;
        }

        private Stream decorateTodoRecordProperties(HttpWebResponse resEndeca, Dictionary<string, MemberToDoModel> todos)
        {
            // Get the stream containing content returned by the server.
            JObject endecaAsJSON = new JObject();
            //leaked_resource: Variable resEndeca going out of scope leaks the resource.
            using (resEndeca)
            {
                Stream receiveStream = resEndeca.GetResponseStream();
                if (receiveStream != null)
                {
                    //does not free or save its parameter resEndeca.
                    using (StreamReader streamReader = new StreamReader(resEndeca.GetResponseStream()))
                    {
                        endecaAsJSON = JObject.Parse(streamReader.ReadToEnd());

                        try
                        {
                            // call searchObject to loop through the json object with this condition
                            SearchObject(endecaAsJSON, obj =>
                            {
                                if (obj["attributes"] != null)
                                {
                                    //Update the attributes object with smart content properties 
                                    obj["attributes"] = UpdateEndecaData(obj, todos);
                                }
                            });
                        }
                        catch (Exception ex)
                        {
                            _log.Error("decorateTodoRecordProperties failed Exception:", ex);
                        }
                    }

                }
            }

            var serializedData = JsonConvert.SerializeObject(endecaAsJSON);
            byte[] byteArray = Encoding.UTF8.GetBytes(serializedData);
            MemoryStream modifiedStream = new MemoryStream(byteArray);

            if (WebOperationContext.Current != null)
                WebOperationContext.Current.OutgoingResponse.ContentType = "application/json";

            return modifiedStream;
        }

        // function to loop through a json object, with a specified action
        /*http://stackoverflow.com/questions/16181298/how-to-do-recursive-descent-of-json-using-json-net
         */
        private void SearchObject(JToken container, Action<JObject> objectAction)
        {
            if (container.Type == JTokenType.Object)
            {
                if (objectAction != null)
                {
                    objectAction((JObject)container);
                }

                foreach (JProperty child in container.Children<JProperty>())
                {
                    SearchObject(child.Value, objectAction);
                }
            }
            else if (container.Type == JTokenType.Array)
            {
                foreach (JToken child in container.Children())
                {
                    SearchObject(child, objectAction);
                }
            }
        }

        JToken UpdateEndecaData(JToken record, Dictionary<string, MemberToDoModel> todos)
        {
            string pKey = record["attributes"]["P_Primary_Key"][0].ToString();

            if (todos.ContainsKey(pKey))
            {
                record["attributes"]["MemberToDoListID"] = todos[pKey].MemberToDoListID;
                record["attributes"]["RefToDoStatusID"] = todos[pKey].RefToDoStatusID;
                record["attributes"]["RefToDoTypeID"] = todos[pKey].RefToDoTypeID;
                //Add a property for the last time the todo was created or changed, so that we can sort them by date on the front end
                record["attributes"]["LastModified"] = todos[pKey].ModifiedDate.HasValue ? todos[pKey].ModifiedDate : todos[pKey].CreatedDate;
                //Add a property for front end templates to quickly check if a todo is complete, rather than checking for RefToDoStatus = 2 all over the place
                if (todos[pKey].RefToDoStatusID == 2)
                {
                    record["attributes"]["ToDoComplete"] = true; 
                }
            }

            return record["attributes"];
        }

        [OperationContract]
        [WebGet(UriTemplate = "GetSearch/{*request}", ResponseFormat = WebMessageFormat.Json)]
        [AspNetCacheProfile("DoNotCache")]
        public Stream GetSearch(string request)
        {
            // Don't know why this doesn't come with the wildcard parameter in the template, but here's another
            // way to get the query string
            SearchQueryModel search = new SearchQueryModel(WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri.Query, request);
            string searchString = "";

            int memberId = _memberAdapter.GetMemberIdFromContext();

            bool isAuthorized = memberId != 0;

            Dictionary<string, MemberToDoModel> todos = new Dictionary<string, MemberToDoModel>();            
            
            if (isAuthorized)
            {
                todos = _memberAdapter.GetToDoDictionary(memberId);
            }

            /*if the request is not from sort control bar, we don't execute updateFilterParamter function.*/
            if (search.query.Contains("Dims=") || search.query.Contains("Type=") || search.query.Contains("Ns="))
            {
                searchString = endecaUtility.updateFilterParameter(search.query, memberId, true);
            }
            
            if (searchString.Length > Config.SearchApiLimit)
            {
                _log.Error("Search request " + searchString + " exceeded max allowed length of " + Config.SearchApiLimit);
                throw new WebException("Search error: Search string exceeded maximum allowed length");
            }

            WebRequest reqEndeca = WebRequest.Create("http://" + Config.ITLHost + ":" + Config.ITLPort.ToString() + "/" + request + searchString);

            //COV-10458 - RESOURCE_LEAK
            using (HttpWebResponse resEndeca = (HttpWebResponse)reqEndeca.GetResponse())
            {

                if (isAuthorized && todos.Count > 0)
                {
                    return decorateTodoRecordProperties(resEndeca, todos);
                }
                else
                {
                    WebOperationContext.Current.OutgoingResponse.ContentType = "application/json";
                    // Get the stream containing content returned by the server.
                    using (StreamReader streamReader = new StreamReader(resEndeca.GetResponseStream(), true))
                    {
                        return new MemoryStream(Encoding.UTF8.GetBytes(streamReader.ReadToEnd() ?? ""));
                    }
                }
            }
        }

        #endregion

        #region Generic search calls

        public SearchResultsModel DoSearch(string[] contentTypes, string size, string school, string role, string grade, string enrollStatus)
        {
            SearchResultsModel SearchResultsModel = null;
            bool validContentTypes = true;
            foreach (string contentType in contentTypes)
            {
                validContentTypes &= SearchValidation.ValidateSearchFilter(contentType);
            }

            if (validContentTypes &&                                //required
                SearchValidation.ValidateSearchFilter(size) &&      //required
                SearchValidation.ValidateSearchFilter(school)       //required
                )
            {
                SearchResultsModel = _searchAdapter.DoSearch(contentTypes, size, school, role, grade, enrollStatus);
            }
            else
            {
                SearchResultsModel = new SearchResultsModel();
                ErrorModel error = new ErrorModel("Invalid search criteria", "Web Search Service");
                SearchResultsModel.ErrorList.Add(error);
            }

            return SearchResultsModel;
        }
        #endregion


        /// <summary>
        /// Post a User's answers from the scholarship search tool to unigo.
        /// </summary>
        /// <param name="choices">The user's coices, an array of unigo ids</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "submitChoicesToUnigo", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public Stream submitChoicesToUnigo(UnigoChoicesModel choices)
        {
            //The WebRequest expects the "body" of the request as a stream
            //We need to convert the choices object passed in to an array of bytes, which can be written to the outgoing stream
            MemoryStream responseStream = null;
            try
            {
                String responseString;
                using (MemoryStream choicesStream = new MemoryStream())
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(string[]));
                    serializer.WriteObject(choicesStream, choices.choices);
                    byte[] choicesAsBytes = choicesStream.ToArray();
                    // Get the stream containing content returned by the server.
                    string urlToRequest = Config.ScholarshipApiURL + "scholarships?auth=" + Config.ScholarshipApiToken;
                    HttpsRequestProvider httpsProvider = new HttpsRequestProvider();
                    responseString = httpsProvider.postChoicesToUnigo(choicesAsBytes, urlToRequest);
                }
                responseStream = new MemoryStream(Encoding.UTF8.GetBytes(responseString));
            }
            catch (Exception ex)
            {
                _log.Error("ASA.Web.Services.SearchService.submitChoicesToUnigo(): Exception =>" + ex.ToString());
                throw new Exception("Web Search Service - submitChoicesToUnigo()", ex);
            }

            if (WebOperationContext.Current != null)
                WebOperationContext.Current.OutgoingResponse.ContentType = "application/json";
            
            return responseStream;
        }

        [OperationContract]
        [WebGet(UriTemplate = "Scholarships/{id}", ResponseFormat = WebMessageFormat.Json)]
        public Stream getIndividualScholarship(string id)
        {
            MemoryStream responseStream = null;
            string urlToRequest = Config.ScholarshipApiURL + "scholarship?scholarshipID=" + id + "&auth=" + Config.ScholarshipApiToken;
            try
            {
                HttpsRequestProvider httpsProvider = new HttpsRequestProvider();
                String responseString = httpsProvider.getScholarshipData(urlToRequest);
                responseStream = new MemoryStream(Encoding.UTF8.GetBytes(responseString));
            }
            catch (Exception ex)
            {
                _log.Error("ASA.Web.Services.SearchService.getIndividualScholarship(): Exception =>" + ex.ToString());
                throw new Exception("Web Search Service - getIndividualScholarship()", ex);
            }

            if (WebOperationContext.Current != null)
                WebOperationContext.Current.OutgoingResponse.ContentType = "application/json";

            return responseStream;
        }

    }
}
