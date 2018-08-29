using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ASA.Web.Services.ASAMemberService;
using ASA.Web.Services.Proxies;
using ASA.Web.Services.Proxies.SALTService;
using ASA.Web.Services.SurveyService.DataContracts;
using ASA.Web.Services.SurveyService.Exceptions;
using ASA.Web.Services.SurveyService.ServiceContracts;
using log4net;

using ASA.Web.WTF;
using ASA.Web.WTF.Integration;

namespace ASA.Web.Services.SurveyService
{
    public class SurveyAdapter : ISurveyAdapter
    {
        private const string Classname = "Survey";
        private static readonly ILog Log = LogManager.GetLogger(Classname);
        private static readonly Dictionary<string, int> SurveyIdLookup = new Dictionary<string, int>();
 
        public SurveyAdapter()
        {
            Log.Debug("ASA.Web.Services.SurveyService.SurveyAdapter() object being created ...");
        }

        #region Main Functions

        /// <summary>
        /// Inserts the survey.
        /// </summary>
        /// <param name="survey">The survey.</param>
        /// <returns></returns>
        /// <exception cref="SurveyOperationException">Web Survey Service - ASA.Web.Services.SurveyService.SurveyAdapter.InsertSurvey()</exception>
        public Common.ResultCodeModel InsertSurvey(SurveyModel survey)
        {
            const string logMethodName = ".InsertSurvey(SurveyModel survey) - ";
            Log.Debug(logMethodName + "Begin Method");

            var result = new Common.ResultCodeModel(0);

            try
            {
                IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent").PostSurvey(survey.ToDataContract());
                result.ResultCode = 1;
            }

            catch (Exception ex)
            {
                Log.Error("ASA.Web.Services.SurveyService.SurveyAdapter.InsertSurvey(): Exception =>" + ex.ToString());
                throw new SurveyOperationException("Web Survey Service - ASA.Web.Services.SurveyService.SurveyAdapter.InsertSurvey()", ex);
            }

            Log.Debug(logMethodName + "End Method");
            return result;
        }

        /// <summary>
        /// Gets the question and response.
        /// </summary>
        /// <param name="surveyId">The survey id.</param>
        /// <returns></returns>
        /// <exception cref="SurveyOperationException">Web Survey Service - ASA.Web.Services.SurveyService.SurveyAdapter.GetQuestionAndResponse()</exception>
        public SurveyListModel GetQuestionAndResponse(int surveyId)
        {
            const string logMethodName = ".GetQuestionAndResponse(string surveyId) - ";
            Log.Debug(logMethodName + "Begin Method");
            
            var sList = new SurveyListModel();
            var memberAdapter = new AsaMemberAdapter();

            try
            {
                var survey = IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent").GetSurveyById(surveyId).ToDomainModel();
                if (survey != null)
                {
                    var response = IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent").GetUserSurveyResults(surveyId, memberAdapter.GetMemberIdFromContext()).ToDomainModel();
                    
                    sList.Surveys = new List<SurveyModel>();
                    if (response.Surveys.Any())
                    {
                        survey.Response = response.Surveys.First().Response;
                        survey.ResponseCount = 1;
                    }

                    sList.Surveys = new List<SurveyModel>() {survey};
                }
            }

            catch (Exception ex)
            {
                Log.Error("ASA.Web.Services.SurveyService.SurveyAdapter.GetQuestionAndResponse(): Exception =>" + ex.ToString());
                throw new SurveyOperationException("Web Survey Service - ASA.Web.Services.SurveyService.SurveyAdapter.GetQuestionAndResponse()", ex);
            }

            Log.Debug(logMethodName + "End Method");
            return sList;
        }

        /// <summary>
        /// Gets the individuals response.
        /// </summary>
        /// <param name="surveyId">The survey id.</param>
        /// <param name="memberId">The member id.</param>
        /// <returns></returns>
        /// <exception cref="SurveyOperationException">Web Survey Service - ASA.Web.Services.SurveyService.SurveyAdapter.GetQuestionAndResponse()</exception>
        public SurveyListModel GetIndividualsResponse(int surveyId, int memberId)
        {
            const string logMethodName = ".GetIndividualsResponse(string surveyId, string surveyQuestionId, string individualId) - ";
            Log.Debug(logMethodName + "Begin Method"); 
            
            SurveyListModel sList;

            try
            {
                var memberAdapter = new AsaMemberAdapter();
                sList = IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent").GetUserSurveyResults(surveyId, memberAdapter.GetMemberIdFromContext()).ToDomainModel();
            }

            catch (Exception ex)
            {
                Log.Error("ASA.Web.Services.SurveyService.SurveyAdapter.GetQuestionAndResponse(): Exception =>" + ex.ToString());
                throw new SurveyOperationException("Web Survey Service - ASA.Web.Services.SurveyService.SurveyAdapter.GetQuestionAndResponse()", ex);
            }

            Log.Debug(logMethodName + "End Method");
            return sList;
        }

        /// <summary>
        /// Gets the response and totals.
        /// </summary>memberAdapter
        /// <param name="surveyId">The survey id.</param>
        /// <returns></returns>
        /// <exception cref="SurveyOperationException">Web Survey Service - ASA.Web.Services.SurveyService.SurveyAdapter.GetResponseAndTotals()</exception>
        public SurveyListModel GetResponseAndTotals(int surveyId)
        {
            const string logMethodName = ".GetResponseAndTotals(string surveyId) - ";
            Log.Debug(logMethodName + "Begin Method");
            var sList = new SurveyListModel();

            try
            {
                var memberAdapter = new AsaMemberAdapter();
                var survey = IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent").GetSurveyById(surveyId);

                if (survey != null)
                {
                    foreach (var option in survey.SurveyOptions)
                    {
                        sList.Surveys.Add(new SurveyModel()
                            {
                                IndividualId = memberAdapter.GetActiveDirectoryKeyFromContext(),
                                MemberId = memberAdapter.GetMemberIdFromContext(),
                                SurveyId = survey.SurveyId.ToString(CultureInfo.InvariantCulture),
                                SurveyQuestionId = survey.SurveyId.ToString(CultureInfo.InvariantCulture),
                                Response = option.OptionValue,
                                ResponseCount = option.TotalResponseCount,
                                QuestionText = survey.SurveyQuestion,

                            });

                    }
                }

            }

            catch (Exception ex)
            {
                Log.Error("ASA.Web.Services.SurveyService.SurveyAdapter.GetSurveyQuestion(): Exception =>" + ex.ToString());
                throw new SurveyOperationException("Web Survey Service - ASA.Web.Services.SurveyService.SurveyAdapter.GetResponseAndTotals()", ex);
            }

            Log.Debug(logMethodName + "End Method");
            return sList;
        }

        /// <summary>
        /// Add the response.
        /// </summary>
        /// <param name="response">The Vlc response model.</param>
        /// <returns></returns>
        /// <exception cref="SurveyOperationException">Web Survey Service - ASA.Web.Services.SurveyService.SurveyAdapter.AddVlcResponse()</exception>
        public bool AddVlcResponse(VLCQuestionResponseModel response)
        {
            var result = SaltServiceAgent.AddVlcResponse(response.ToDataContract());
            return result;
        }


        /// <summary>
        /// Add the response.
        /// </summary>
        /// <param name="response">The JellyVision Quiz response model.</param>
        /// <returns>true/false</returns>
        public bool AddJellyVisionQuizResponse(JellyVisionQuizResponseModel response)
        {
            var result = IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent").AddJellyVisionQuizResponse(response.ToDataContract());
            return result;
        }

        /// <summary>
        /// Get Cost of Living state list.
        /// </summary>
        /// <returns>COLResponseModel containing the COLStateModel list of States.</returns>
        /// <exception cref="SurveyOperationException">Web Survey Service - ASA.Web.Services.SurveyService.SurveyAdapter.GetCOLStateList()</exception>
        public COLResponseModel GetCOLStateList()
        {
            const string logMethodName = ".GetCOLStateList() - ";
            Log.Info(logMethodName + "Begin Method");

            var colResponseModel = new COLResponseModel();

            try
            {
                colResponseModel.State = IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent").GetCOLStates().ToDomainObject();
            }

            catch (Exception ex)
            {
                Log.Error(logMethodName + "Exception =>" + ex.ToString());
                throw new SurveyOperationException("Web Survey Service - ASA.Web.Services.SurveyService.SurveyAdapter.GetCOLStateList()", ex);
            }

            Log.Info(logMethodName + "End Method");
            return colResponseModel;
        }

        /// <summary>
        /// Get the list of Cost of Living urban areas.
        /// </summary>
        /// <param name="request">The COLRequestModel with the state id provided.</param>
        /// <returns>COLResponseModel containing the COLUrbanAreaModel list of urban areas.</returns>
        /// <exception cref="SurveyOperationException">Web Survey Service - ASA.Web.Services.SurveyService.SurveyAdapter.GetCOLStateList()</exception>
        public COLResponseModel GetCOLUrbanAreaList(COLRequestModel request)
        {
            const string logMethodName = ".GetCOLUrbanAreaList(COLRequestModel request) - ";
            Log.Info(logMethodName + "Begin Method");

            var colResponseModel = new COLResponseModel();

            try
            {
                colResponseModel.UrbanArea = IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent").GetCOLUrbanAreas((int)request.StateId).ToDomainObject();
            }

            catch (Exception ex)
            {
                Log.Error(logMethodName + "Exception =>" + ex.ToString());
                throw new SurveyOperationException("Web Survey Service - ASA.Web.Services.SurveyService.SurveyAdapter.GetCOLUrbanAreaList()", ex);
            }

            Log.Info(logMethodName + "End Method");
            return colResponseModel;
        }

        /// <summary>
        /// Gets the CostOfLiving results.
        /// </summary>
        /// <param name="request">The COLRequestModel with the CityA, "CityB and Salary provided.</param>
        /// <returns>COLResponseModel with COLCostBreakDownModel and NeededSalary & percentageSalaryChange</returns>
        /// <exception cref="SurveyOperationException">Web Survey Service - ASA.Web.Services.SurveyService.SurveyAdapter.GetCOLStateList()</exception>
        public COLResponseModel GetCOLResults(COLRequestModel request)
        {
            const string logMethodName = ".GetCOLResults(COLRequestModel request) - ";
            Log.Info(logMethodName + "Begin Method");

            var colResponseModel = new COLResponseModel();

            try
            {
                colResponseModel = IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent").GetCOLResults((int)request.CityA, (int)request.CityB, (decimal)request.Salary).ToDomainObject();
            }

            catch (Exception ex)
            {
                Log.Error(logMethodName + "Exception =>" + ex.ToString());
                throw new SurveyOperationException("Web Survey Service - ASA.Web.Services.SurveyService.SurveyAdapter.GetCOLResults()", ex);
            }

            Log.Info(logMethodName + "End Method");
            return colResponseModel;
        }

        /// <summary>
        /// Get JSI major list.
        /// </summary>
        /// <returns>JSIQuizListModel</returns>
        /// <exception cref="SurveyOperationException">Web Survey Service - ASA.Web.Services.SurveyService.SurveyAdapter.GetJSIMajorList()</exception>
        public JSIQuizListModel GetJSIMajorList()
        {
            const string logMethodName = ".GetJSIMajorList() - ";
            Log.Info(logMethodName + "Begin Method");

            var jList = new JSIQuizListModel();

            try
            {
                var jsiMajor = IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent").GetJSIMajor().ToDomainObject();
                jList = new JSIQuizListModel(jsiMajor);
            }

            catch (Exception ex)
            {
                Log.Error(logMethodName + "Exception =>" + ex.ToString());
                throw new SurveyOperationException("Web Survey Service - ASA.Web.Services.SurveyService.SurveyAdapter.GetJSIMajorList()", ex);
            }

            Log.Info(logMethodName + "End Method");
            return jList;
        }

        /// <summary>
        /// Get JSI state list.
        /// </summary>
        /// <returns>JSIQuizListModel</returns>
        /// <exception cref="SurveyOperationException">Web Survey Service - ASA.Web.Services.SurveyService.SurveyAdapter.GetJSIStateList()</exception>
        public JSIQuizListModel GetJSIStateList()
        {
            const string logMethodName = ".GetJSIStateList() - ";
            Log.Info(logMethodName + "Begin Method");

            var jList = new JSIQuizListModel();

            try
            {
                var jsiState = IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent").GetJSIState().ToDomainObject();
                jList = new JSIQuizListModel(jsiState);
            }

            catch (Exception ex)
            {
                Log.Error(logMethodName + "Exception =>" + ex.ToString());
                throw new SurveyOperationException("Web Survey Service - ASA.Web.Services.SurveyService.SurveyAdapter.GetJSIStateList()", ex);
            }

            Log.Info(logMethodName + "End Method");
            return jList;
        }

        /// <summary>
        /// Get JSI school list.
        /// </summary>
        /// <param name="majorId">JSI RefMajorId</param>
        /// <param name="stateId">JSI RefStateId</param>
        /// <returns>JSIQuizListModel</returns>
        /// <exception cref="SurveyOperationException">Web Survey Service - ASA.Web.Services.SurveyService.SurveyAdapter.GetJSISchoolList()</exception>
        public JSIQuizListModel GetJSISchoolList(int majorId, int stateId)
        {
            const string logMethodName = ".GetJSISchoolList(int majorId, int stateId) - ";
            Log.Info(logMethodName + "Begin Method");
            Log.Debug(logMethodName + string.Format("input majorId={0}, stateId={1}", majorId, stateId));

            var jList = new JSIQuizListModel();

            try
            {
                var jsiSchool = IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent").GetJSISchoolMajor(majorId, stateId).ToDomainObject();
                jList = new JSIQuizListModel(jsiSchool);
            }

            catch (Exception ex)
            {
                Log.Error(logMethodName + "Exception =>" + ex.ToString());
                throw new SurveyOperationException("Web Survey Service - ASA.Web.Services.SurveyService.SurveyAdapter.GetJSISchoolList()", ex);
            }

            Log.Info(logMethodName + "End Method");
            return jList;
        }

        /// <summary>
        /// Post the results of the JSI quiz.
        /// </summary>
        /// <param name="jsiQuizModel">single JSIQuizModel object</param>
        /// <returns>?</returns>
        public JSIQuizListModel PostJSIResponse(JSIQuizModel jsiQuizModel)
        {
            const string logMethodName = ".PostJSIResponse(JSIQuizModel jsiQuizModel) - ";
            Log.Info(logMethodName + "Begin Method");

            var jList = new JSIQuizListModel();

            try
            {
                var jsiQuizResult = IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent").PostJSIResponse(jsiQuizModel.ToDataContract()).ToDomainObject();
                jList = new JSIQuizListModel(jsiQuizResult);
            }
            catch (Exception ex)
            {
                Log.Error(logMethodName + "Exception =>" + ex.ToString());
                throw new SurveyOperationException("Web Survey Service - ASA.Web.Services.SurveyService.SurveyAdapter.GetJSISchoolList()", ex);
            }

            Log.Info(logMethodName + "End Method");
            return jList;
        }
        #endregion
    }
}
