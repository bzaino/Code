using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using ASA.Web.Services.SurveyService.DataContracts;
using ASA.Web.Services.Common;
using ASA.Web.Services.SurveyService.ServiceContracts;
using ASA.Web.Services.SurveyService.Validation;
using ASA.Web.Services.ASAMemberService;
using ASA.Web.Services.SurveyService.Exceptions;
using System.Web.Security;
using ASA.Web.WTF;
using ASA.Web.WTF.Integration;

namespace ASA.Web.Services.SurveyService
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Survey
    {
        private const string CLASSNAME = "Survey";
        static ASA.Log.ServiceLogger.IASALog _log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(CLASSNAME);
        private const string _surveyAdapterExceptionMessage = "Unable to create a SurveyAdapter object from the ASA.Web.Services.SurveyService library. ";
        private ISurveyAdapter _surveyAdapter = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Survey"/> class.
        /// </summary>
        public Survey()
        {
            _log.Info("ASA.Web.Services.SurveyService.Survey() object being created ...");
             _surveyAdapter = new SurveyAdapter();
        }

        /// <summary>
        /// Inserts the survey.
        /// </summary>
        /// <param name="survey">The survey.</param>
        /// <returns></returns>
        /// <exception cref="SurveyOperationException">Web Survey Service - Exception in ASA.Web.Services.SurveyService.InsertSurvey()</exception>
        [OperationContract]
        [WebInvoke(UriTemplate = "Survey", Method = "POST")]
        public ResultCodeModel InsertSurvey(SurveyModel survey)
        {
            ResultCodeModel result;

            try
            {
                var memberAdapter = new AsaMemberAdapter();
                //add system data
                //response status will be hardcoded in deployment scripts so it will be the same for all environments
                survey.ResponseDate = DateTime.Now;
                survey.ResponseStatus = "21B65800-5F9A-421F-9E82-DA13B111F790";
                survey.MemberId = memberAdapter.GetMemberIdFromContext();

                if (SurveyValidation.ValidateSurvey(survey))
                {
                    _log.Info("ASA.Web.Services.SurveyService.InsertSurvey() starting ...");
                    result = _surveyAdapter.InsertSurvey(survey);
                    _log.Info("ASA.Web.Services.SurveyService.InsertSurvey() ending ...");
                }
                else
                {
                    result = new ResultCodeModel();
                    var error = new ErrorModel("Invalid information input for this survey", "Web Survey Service");
                    _log.Error("ASA.Web.Services.SurveyService.InsertSurvey(): Invalid information input for this survey");
                    result.ErrorList.Add(error);

                    if (Config.Testing)
                    {
                        //get validation errors out of list and retrun them
                        if (survey.ErrorList.Count > 0)
                        {
                            foreach (ErrorModel em in survey.ErrorList)
                            {
                                //if not empty copy message
                                if (!string.IsNullOrEmpty(em.Code))
                                {
                                    result.ErrorList.Add(em);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("ASA.Web.Services.SurveyService.InsertSurvey(): Exception => " + ex.ToString());
                throw new SurveyOperationException("Web Survey Service - Exception in ASA.Web.Services.SurveyService.InsertSurvey()", ex);
            }

            return result;
        }

        /// <summary>
        /// Gets the survey question.
        /// </summary>
        /// <param name="surveyId">The survey id.</param>
        /// <returns></returns>
        /// <exception cref="SurveyBadDataException">Null adapter in ASA.Web.Services.SurveyService.GetSurveyQuestion()</exception>
        /// <exception cref="SurveyOperationException">Web Survey Service - Exception in ASA.Web.Services.SurveyService.GetSurveyQuestion()</exception>
        [WebGet(UriTemplate = "Survey/{surveyId}", ResponseFormat = WebMessageFormat.Json)]
        public SurveyListModel GetSurveyQuestion(string surveyId)
        {
 
            SurveyListModel sList = null;
            
            try
            {
                _log.Info("ASA.Web.Services.SurveyService.GetSurveyQuestion() starting ...");

                if (_surveyAdapter == null)
                {
                    _log.Error("ASA.Web.Services.SurveyService.GetSurveyQuestion(): " + _surveyAdapterExceptionMessage);
                    throw new SurveyBadDataException("Null adapter in ASA.Web.Services.SurveyService.GetSurveyQuestion()");
                }
                else if (string.IsNullOrEmpty(surveyId))
                {
                    _log.Info("Survey Id was not provided to access GetSurveyQuestion");
                    sList = new SurveyListModel();
                    var error = new ErrorModel("Survey Id was not provided to access GetSurveyQuestion", "Web Survey Service");
                    sList.ErrorList.Add(error);
                }
                else
                {
                    sList = _surveyAdapter.GetQuestionAndResponse(Convert.ToInt32(surveyId));
                    
                }
            }
            catch (Exception ex)
            {
                _log.Error("ASA.Web.Services.SurveyService.GetSurveyQuestions(): Exception => " + ex.ToString());
                throw new SurveyOperationException("Web Survey Service - Exception in ASA.Web.Services.SurveyService.GetSurveyQuestion()", ex);
            }

            _log.Info("ASA.Web.Services.SurveyService.GetSurveyQuestion() ending ...");
            return sList;
        }

        /// <summary>
        /// Gets the individuals response.
        /// </summary>
        /// <param name="surveyId">The survey id.</param>
        /// <param name="individualId">The individual id.</param>
        /// <param name="surveyQuestionId">The survey question id.</param>
        /// <returns></returns>
        /// <exception cref="SurveyBadDataException">Null adapter in ASA.Web.Services.SurveyService.GetIndividualsResponse()</exception>
        /// <exception cref="SurveyOperationException">Web Survey Service - Exception in ASA.Web.Services.SurveyService.GetSurveyQuestion()</exception>
        [WebGet(UriTemplate = "Survey/{surveyId}/{individualId}/{surveyQuestionId=null}", ResponseFormat = WebMessageFormat.Json)]
        public SurveyListModel GetIndividualsResponse(string surveyId, string individualId, string surveyQuestionId)
        {
            var memberAdapter = new AsaMemberAdapter();

            SurveyListModel sList = null;

            try
            {
                _log.Info("ASA.Web.Services.SurveyService.GetIndividualsResponse() starting ...");
                int memberId = memberAdapter.GetMemberIdFromContext();

                if (_surveyAdapter == null)
                {
                    _log.Error("ASA.Web.Services.SurveyService.GetIndividualsResponse(): " + _surveyAdapterExceptionMessage);
                    throw new SurveyBadDataException("Null adapter in ASA.Web.Services.SurveyService.GetIndividualsResponse()");
                }

                if (string.IsNullOrEmpty(surveyId))
                {
                    _log.Info("Survey Id was not provided to access GetIndividualsResponse");
                    sList = new SurveyListModel();
                    var error = new ErrorModel("Survey Id was not provided to access GetIndividualsResponse", "Web Survey Service");
                    sList.ErrorList.Add(error);
                }
                else if (string.IsNullOrEmpty(individualId)) {
                    _log.Info("Individual Id was not provided to access GetIndividualsResponse");
                    sList = new SurveyListModel();
                    var error = new ErrorModel("Individual Id was not provided to access GetIndividualsResponse", "Web Survey Service");
                    sList.ErrorList.Add(error);
                }
                else {
                    //no errors then continue
                    sList = _surveyAdapter.GetIndividualsResponse(Convert.ToInt32(surveyId), memberId);
                }
            }
            catch (Exception ex)
            {
                _log.Error("ASA.Web.Services.SurveyService.GetSurveyQuestions(): Exception => " + ex.ToString());
                throw new SurveyOperationException("Web Survey Service - Exception in ASA.Web.Services.SurveyService.GetSurveyQuestion()", ex);
            }

            _log.Info("ASA.Web.Services.SurveyService.GetSurveyQuestion() ending ...");
            if (sList.Surveys.Any())
            {
                sList.Surveys[0].SurveyId = surveyId;
                sList.Surveys[0].SurveyQuestionId = surveyQuestionId;
            }
            return sList;
        }

        /// <summary>
        /// Gets the surveys response and totals.
        /// </summary>
        /// <param name="surveyId">The survey id.</param>
        /// <returns></returns>
        /// <exception cref="SurveyBadDataException">Null adapter in ASA.Web.Services.SurveyService.GetResponseAndTotals()</exception>
        /// <exception cref="SurveyOperationException">Web Survey Service - Exception in ASA.Web.Services.SurveyService.GetResponseAndTotals()</exception>
        [WebGet(UriTemplate = "SurveyResults/{surveyId}", ResponseFormat = WebMessageFormat.Json)]
        public SurveyListModel GetResponseAndTotals(string surveyId)
        {
            
            SurveyListModel sList = null;

            try
            {
                _log.Info("ASA.Web.Services.SurveyService.GetResponseAndTotals() starting ...");

                if (_surveyAdapter == null)
                {
                    _log.Error("ASA.Web.Services.SurveyService.GetResponseAndTotals(): " + _surveyAdapterExceptionMessage);
                    throw new SurveyBadDataException("Null adapter in ASA.Web.Services.SurveyService.GetResponseAndTotals()");
                }
                else if (string.IsNullOrEmpty(surveyId))
                {
                    _log.Info("Survey Id was not provided to access GetResponseAndTotals");
                    sList = new SurveyListModel();
                    var error = new ErrorModel("Survey Id was not provided to access GetResponseAndTotals", "Web Survey Service");
                    sList.ErrorList.Add(error);
                }
                else
                {
                    sList = _surveyAdapter.GetResponseAndTotals(Convert.ToInt32(surveyId));
                }
            }
            catch (Exception ex)
            {
                _log.Error("ASA.Web.Services.SurveyService.GetResponseAndTotalss(): Exception => " + ex.ToString());
                throw new SurveyOperationException("Web Survey Service - Exception in ASA.Web.Services.SurveyService.GetResponseAndTotals()", ex);
            }

            _log.Info("ASA.Web.Services.SurveyService.GetResponseAndTotals() ending ...");
            return sList;
        }

        [WebInvoke(UriTemplate = "AddVlcResponse", ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        public bool PublicVlcEndpoint(VLCQuestionResponseModel response)
        {
            try
            {
                if (VLCQuestionResponseValidation.validateQuestionResponseModel(response))
                {
                    var _memberAdapter = new AsaMemberAdapter();
                    var memberId = _memberAdapter.GetMemberIdFromContext();
                    response.MemberID = memberId;
                    response.ResponseDate = System.DateTime.Now;
                    return _surveyAdapter.AddVlcResponse(response);
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                _log.Error("ASA.Web.Services.SurveyService.AddVlcResponse(): Exception => " + ex.ToString());
                throw new SurveyOperationException("Web Survey Service - Exception in ASA.Web.Services.SurveyService.AddVlcResponse()", ex);
            }
        }

        /// <summary>
        /// Post the results to the JellyVision quiz.
        /// </summary>
        /// <param name="id">this indicates the user that is taking the Quiz, can be null if not logged into SALT</param>
        /// <param name="referrerID">indicates where the user is taking the Quiz, either on the SALT site or on a partner site</param>
        /// <param name="responses">question number and response in string. Example 1c2b3d</param>
        /// <param name="ptype">The personanlity type code.</param>
        /// <returns>void</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "JVQR?id={id}&referrerID={referrerID}&responses={responses}&ptype={personalityType}&token={encryptedToken}")]
        public bool PostJellyVisionQuizResponse(string id, string referrerID, string responses, string personalityType, string encryptedToken)
        {
            const string logMethodName = ".PostJellyVisionQuizResponse(string id, string referrerID, string responses, string personalityType) - ";
            _log.Debug(logMethodName + "Begin Method");
            _log.Debug(logMethodName + string.Format("id = [{0}], referrerID = [{1}], responses = [{2}], personalityType = [{3}]", id, referrerID, responses, personalityType));

            try
            {
                bool toReturn = false;
                //allow all the origin url to avoid cross domain issue.
                if (WebOperationContext.Current != null) { 
                    WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Allow-Origin", "*");
                }

                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(encryptedToken);
                if (string.IsNullOrEmpty(ticket.ToString()))
                {
                    _log.Debug("Something wrong during the decryption process!");
                    return false;
                }

                //validate the request with the values extracted from the token
                if (!ticket.Expired && ticket.Name == id && ticket.UserData == "1")
                {
                    JellyVisionQuizResponseModel jvqResponse = new JellyVisionQuizResponseModel();
                
                    //hardcoding the quizName for now. If we get another then we'll have to change this.
                    jvqResponse.quizName = "Money Personality Quiz";

                    jvqResponse.Id = !string.IsNullOrWhiteSpace(id) ? int.Parse(id) : 0;
                    jvqResponse.referrerID = referrerID;
                    jvqResponse.responses = responses;
                    jvqResponse.personalityType = personalityType;

                    if (JellyVisionQuizResponseValidation.validateQuizResponseModel(jvqResponse))
                    {
                        toReturn = _surveyAdapter.AddJellyVisionQuizResponse(jvqResponse);
                    }
                }
                else {
                    _log.Debug(logMethodName + "the request expired.");
                }

                _log.Debug(logMethodName + "End Method");

                return toReturn;
            }
            catch (Exception ex)
            {
                _log.Error(logMethodName + ": Exception => " + ex.ToString());
                return false;
                //throw new SurveyOperationException("Web Survey Service - Exception in ASA.Web.Services.SurveyService.PostJellyVisionQuizResponse()", ex);
            }
        }

        /// <summary>
        /// Gets the JSI Major list.
        /// </summary>
        /// <returns>JSIQuizListModel</returns>
        /// <exception cref="SurveyBadDataException">Null adapter in ASA.Web.Services.SurveyService.GetJSIMajorList()</exception>
        /// <exception cref="SurveyOperationException">Web Survey Service - Exception in ASA.Web.Services.SurveyService.GetJSIMajorList()</exception>
        [WebGet(UriTemplate = "JSI/Majors", ResponseFormat = WebMessageFormat.Json)]
        public JSIQuizListModel GetJSIMajorList()
        {
            const string logMethodName = ".GetJSIMajorList() - ";
            _log.Info(logMethodName + "Begin Method");

            JSIQuizListModel jList = null;

            try
            {
                if (_surveyAdapter == null)
                {
                    _log.Error(logMethodName + _surveyAdapterExceptionMessage);
                    throw new SurveyBadDataException("Null adapter in ASA.Web.Services.SurveyService.GetJSIMajorList()");
                }
                //else if (string.IsNullOrEmpty(surveyId))
                //{
                //    _log.Info(logMethodName + "Survey Id was not provided to access GetJSIMajorList");
                //    sList = new SurveyListModel();
                //    var error = new ErrorModel("Survey Id was not provided to access GetJSIMajorList", "Web Survey Service");
                //    sList.ErrorList.Add(error);
                //}
                else
                {
                    jList = _surveyAdapter.GetJSIMajorList();
                }
            }
            catch (Exception ex)
            {
                _log.Error(logMethodName + "Exception => " + ex.ToString());
                throw new SurveyOperationException("Web Survey Service - Exception in ASA.Web.Services.SurveyService.GetJSIMajorList()", ex);
            }

            _log.Info(logMethodName + "End Method");
            return jList;
        }

        /// <summary>
        /// Gets the JSI State list.
        /// </summary>
        /// <returns>JSIQuizListModel</returns>
        /// <exception cref="SurveyBadDataException">Null adapter in ASA.Web.Services.SurveyService.GetJSIStateList()</exception>
        /// <exception cref="SurveyOperationException">Web Survey Service - Exception in ASA.Web.Services.SurveyService.GetJSIStateList()</exception>
        [WebGet(UriTemplate = "JSI/States", ResponseFormat = WebMessageFormat.Json)]
        public JSIQuizListModel GetJSIStateList()
        {
            const string logMethodName = ".GetJSIStateList() - ";
            _log.Info(logMethodName + "Begin Method");

            JSIQuizListModel jList = null;

            try
            {
                if (_surveyAdapter == null)
                {
                    _log.Error(logMethodName + _surveyAdapterExceptionMessage);
                    throw new SurveyBadDataException("Null adapter in ASA.Web.Services.SurveyService.GetJSIStateList()");
                }
                //else if (string.IsNullOrEmpty(surveyId))
                //{
                //    _log.Info(logMethodName + "Survey Id was not provided to access GetJSIStateList");
                //    sList = new SurveyListModel();
                //    var error = new ErrorModel("Survey Id was not provided to access GetJSIStateList", "Web Survey Service");
                //    sList.ErrorList.Add(error);
                //}
                else
                {
                    jList = _surveyAdapter.GetJSIStateList();
                }
            }
            catch (Exception ex)
            {
                _log.Error(logMethodName + "Exception => " + ex.ToString());
                throw new SurveyOperationException("Web Survey Service - Exception in ASA.Web.Services.SurveyService.GetJSIStateList()", ex);
            }

            _log.Info(logMethodName + "End Method");
            return jList;
        }

        /// <summary>
        /// Gets the CostOfLiving State list.
        /// </summary>
        /// <returns>COLResponseModel with COLStateModel populated</returns>
        /// <exception cref="SurveyBadDataException">Null adapter in ASA.Web.Services.SurveyService.GetCOLStateList()</exception>
        /// <exception cref="SurveyOperationException">Web Survey Service - Exception in ASA.Web.Services.SurveyService.GetCOLStateList()</exception>
        [WebInvoke(UriTemplate = "COL", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public COLResponseModel GetCOLStateList()
        {
            const string logMethodName = ".GetCOLStateList() - ";
            _log.Info(logMethodName + "Begin Method");

            COLResponseModel colResponseModel = null;
            
            try
            {
                if (_surveyAdapter == null)
                {
                    _log.Error(logMethodName + _surveyAdapterExceptionMessage);
                    throw new SurveyBadDataException("Null adapter in ASA.Web.Services.SurveyService.GetCOLStateList()");
                }
                else
                {
                    colResponseModel = _surveyAdapter.GetCOLStateList();
                }
            }
            catch (Exception ex)
            {
                _log.Error(logMethodName + "Exception => " + ex.ToString());
                throw new SurveyOperationException("Web Survey Service - Exception in ASA.Web.Services.SurveyService.GetCOLStateList()", ex);
            }
            
            _log.Info(logMethodName + "End Method");
            return colResponseModel;
        }

        /// <summary>
        /// Gets the CostOfLiving Urban area list.
        /// </summary>
        /// <param name="stateId">The RefStateId for the list of urban areas.</param>
        /// <returns>COLResponseModel with COLResponseModel populated</returns>
        /// <exception cref="SurveyBadDataException">Null adapter in ASA.Web.Services.SurveyService.GetCOLUrbanAreaList()</exception>
        /// <exception cref="SurveyOperationException">Web Survey Service - Exception in ASA.Web.Services.SurveyService.GetCOLUrbanAreaList()</exception>
        [WebInvoke(UriTemplate = "COL/{stateId}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public COLResponseModel GetCOLUrbanAreaList(string stateId)
        {
            const string logMethodName = ".GetCOLUrbanAreaList(string stateId) - ";
            _log.Info(logMethodName + "Begin Method");

            COLResponseModel colResponseModel = null;

            //validate fields required for this request.
            COLRequestModel request = new COLRequestModel();
            if (!String.IsNullOrEmpty(stateId))
                request.StateId = Convert.ToInt32(stateId);

            ASAModelValidator mv = new ASAModelValidator();
            string[]fieldNames={"StateId"};
            bool validModel = mv.Validate(request, fieldNames);

            if (validModel)
            {
                try
                {
                    if (_surveyAdapter == null)
                    {
                        _log.Error(logMethodName + _surveyAdapterExceptionMessage);
                        throw new SurveyBadDataException("Null adapter in ASA.Web.Services.SurveyService.GetCOLUrbanAreaList()");
                    }
                    else
                    {
                        COLResponseModel resultsModel = _surveyAdapter.GetCOLUrbanAreaList(request);
                        colResponseModel = resultsModel;
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(logMethodName + "Exception => " + ex.ToString());
                    throw new SurveyOperationException("Web Survey Service - Exception in ASA.Web.Services.SurveyService.GetCOLUrbanAreaList()", ex);
                }
            }
            else
            {
                colResponseModel = new COLResponseModel();
                colResponseModel.ErrorList = request.ErrorList.ToList();
            }

            _log.Info(logMethodName + "End Method");
            return colResponseModel;
        }

        /// <summary>
        /// Gets the CostOfLiving results.
        /// </summary>
        /// <param name="CityA">The RefGeographicIndexID of CityA.</param>
        /// <param name="CityB">The RefGeographicIndexID of CityB.</param>
        /// <param name="Salary">The Salary provided.</param>
        /// <returns>COLResponseModel with COLCostBreakDownModel and NeededSalary & percentageSalaryChange</returns>
        /// <exception cref="SurveyBadDataException">Null adapter in ASA.Web.Services.SurveyService.GetCOLUrbanAreaList()</exception>
        /// <exception cref="SurveyOperationException">Web Survey Service - Exception in ASA.Web.Services.SurveyService.GetCOLUrbanAreaList()</exception>
        [WebInvoke(UriTemplate = "COL/{CityA}/{CityB}/{Salary}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public COLResponseModel GetCOLResults(string cityA, string cityB, string salary)
        {
            const string logMethodName = ".GetCOLResults(string cityA, string cityB, string salary) - ";
            _log.Info(logMethodName + "Begin Method");

            COLResponseModel colResponseModel = null;

            //validate fields required for this request.
            COLRequestModel request = new COLRequestModel();
            if (!String.IsNullOrEmpty(cityA))
                request.CityA = Convert.ToInt32(cityA);
            if (!String.IsNullOrEmpty(cityB))
                request.CityB = Convert.ToInt32(cityB);
            if (!String.IsNullOrEmpty(salary))
                request.Salary = Convert.ToDecimal(salary);

            ASAModelValidator mv = new ASAModelValidator();
            string[] fieldNames = { "CityA", "CityB", "Salary" };
            bool validModel = mv.Validate(request, fieldNames);

            if (validModel)
            {
                try
                {
                    if (_surveyAdapter == null)
                    {
                        _log.Error(logMethodName + _surveyAdapterExceptionMessage);
                        throw new SurveyBadDataException("Null adapter in ASA.Web.Services.SurveyService.GetCOLUrbanAreaList()");
                    }
                    else
                    {
                        COLResponseModel resultsModel = _surveyAdapter.GetCOLResults(request);
                        colResponseModel = resultsModel;
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(logMethodName + "Exception => " + ex.ToString());
                    throw new SurveyOperationException("Web Survey Service - Exception in ASA.Web.Services.SurveyService.GetCOLUrbanAreaList()", ex);
                }
            }
            else
            {
                colResponseModel = new COLResponseModel();
                colResponseModel.ErrorList = request.ErrorList.ToList();
            }

            _log.Info(logMethodName + "End Method");
            return colResponseModel;
        }

        /// <summary>
        /// Gets the JSI School list.
        /// </summary>
        /// <param name="jsiQuizModel">single JSIQuizModel object</param>
        /// <returns>JSIQuizListModel</returns>
        /// <exception cref="SurveyBadDataException">Null adapter in ASA.Web.Services.SurveyService.GetJSISchoolList()</exception>
        /// <exception cref="SurveyOperationException">Web Survey Service - Exception in ASA.Web.Services.SurveyService.GetJSISchoolList()</exception>
        [WebGet(UriTemplate = "JSI/Schools/{majorId}/{stateId}", ResponseFormat = WebMessageFormat.Json)]
        public JSIQuizListModel GetJSISchoolList(string majorId, string stateId)
        {
            const string logMethodName = ".GetJSISchoolList(int majorId, int stateId) - ";
            _log.Info(logMethodName + "Begin Method");

            JSIQuizListModel jList = null;

            try
            {
                if (_surveyAdapter == null)
                {
                    _log.Error(logMethodName + _surveyAdapterExceptionMessage);
                    throw new SurveyBadDataException("Null adapter in ASA.Web.Services.SurveyService.GetJSISchoolList()");
                }

                if (string.IsNullOrEmpty(majorId) || string.IsNullOrEmpty(stateId) )
                {
                    _log.Info(logMethodName + "Major Id or State Id was not provided to access GetJSISchoolList");
                    jList = new JSIQuizListModel();
                    var error = new ErrorModel("Major Id or State Id was not provided to access GetJSISchoolList", "Web Survey Service");
                    jList.ErrorList.Add(error);
                }
                else
                {
                    jList = _surveyAdapter.GetJSISchoolList(Convert.ToInt32(majorId), Convert.ToInt32(stateId));
                }
            }
            catch (Exception ex)
            {
                _log.Error(logMethodName + "Exception => " + ex.ToString());
                throw new SurveyOperationException("Web Survey Service - Exception in ASA.Web.Services.SurveyService.GetJSISchoolList()", ex);
            }

            _log.Info(logMethodName + "End Method");
            return jList;
        }

        /// <summary>
        /// Post the results of the JSI quiz.
        /// </summary>
        /// <param name="jsiQuizModel">single JSIQuizModel object</param>
        /// <returns>JSIQuizListModel</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "JSI/", Method = "PUT")]
        public JSIQuizListModel PostJSIResponse(JSIQuizModel jsiQuizModel)
        {
            const string logMethodName = ".PostJSIResponse(JSIQuizModel jsiQuizModel) - ";
            _log.Debug(logMethodName + "Begin Method");

            JSIQuizListModel jList = null;

            try
            {
                if (_surveyAdapter == null)
                {
                    _log.Error(logMethodName + _surveyAdapterExceptionMessage);
                    throw new SurveyBadDataException("Null adapter in ASA.Web.Services.SurveyService.PostJSIResponse()");
                }

                if (string.IsNullOrEmpty(jsiQuizModel.MajorId) || string.IsNullOrEmpty(jsiQuizModel.SchoolId))
                {
                    _log.Info(logMethodName + "MajorId or SchoolId was not provided to access PostJSIResponse");
                    jList = new JSIQuizListModel();
                    var error = new ErrorModel("MajorId or SchoolId was not provided to access PostJSIResponse", "Web Survey Service");
                    jList.ErrorList.Add(error);
                }
                else
                {
                    jList = _surveyAdapter.PostJSIResponse(jsiQuizModel);
                }

                return jList;

            }
            catch (Exception ex)
            {
                _log.Error(logMethodName + ": Exception => " + ex.ToString());
                return new JSIQuizListModel();
                //return false;
                //throw new SurveyOperationException("Web Survey Service - Exception in ASA.Web.Services.SurveyService.PostJellyVisionQuizResponse()", ex);
            }
        }
    }
}
