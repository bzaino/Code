using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Asa.Salt.Web.Services.BusinessServices.Interfaces;
using Asa.Salt.Web.Services.Data.Constants;
using Asa.Salt.Web.Services.Data.Infrastructure;
using Asa.Salt.Web.Services.Data.Model.Database;
using Asa.Salt.Web.Services.Data.Repositories;
using Asa.Salt.Web.Services.Domain;
using Asa.Salt.Web.Services.Logging;
using Asa.Salt.Web.Services.SaltSecurity.Utilities;
using Survey = Asa.Salt.Web.Services.Domain.Survey;
using SurveyResponse = Asa.Salt.Web.Services.Domain.SurveyResponse;
using SurveyOption = Asa.Salt.Web.Services.Domain.SurveyOption;
using Member = Asa.Salt.Web.Services.Domain.Member;
using VLCUserResponse = Asa.Salt.Web.Services.Domain.VLCUserResponse;
using VLCQuestion = Asa.Salt.Web.Services.Domain.VLCQuestion;
using RefMajor = Asa.Salt.Web.Services.Domain.RefMajor;
using RefState = Asa.Salt.Web.Services.Domain.RefState;
using RefGeographicIndex = Asa.Salt.Web.Services.Domain.RefGeographicIndex;
using vCostOfLivingStateList = Asa.Salt.Web.Services.Domain.vCostOfLivingStateList;

namespace Asa.Salt.Web.Services.BusinessServices
{
    public class SurveyService : ISurveyService
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        /// The survey repository
        /// </summary>
        private readonly IRepository<Survey, int> _surveyRepository;

        /// <summary>
        /// The survey response repository
        /// </summary>
        private readonly IRepository<SurveyResponse, int> _surveyResponseRepository;

        /// <summary>
        /// The survey option repository
        /// </summary>
        private readonly IRepository<SurveyOption, int> _surveyOptionRepository;

        /// <summary>
        /// The member repository
        /// </summary>
        private readonly IMemberRepository<Member, int> _memberRepository;

        /// <summary>
        /// The vlc user response repository
        /// </summary>
        private readonly IRepository<VLCUserResponse, int> _vlcUserResponseRepository;

        /// <summary>
        /// The vlc question repository
        /// </summary>
        private readonly IRepository<VLCQuestion, int> _vlcQuestionRepository;

        /// <summary>
        /// The JSI RefMajor repository
        /// </summary>
        private readonly IRepository<RefMajor, int> _refMajorRepository;

        /// <summary>
        /// The JSI RefState repository
        /// </summary>
        private readonly IRepository<RefState, int> _refStateRepository;

        /// <summary>
        /// The Cost of Living Geographic Index repository
        /// </summary>
        private readonly IRepository<RefGeographicIndex, int> _refGeographicIndexRepository;

        /// <summary>
        /// The Cost of Living State list repository
        /// </summary>
        private readonly IRepository<vCostOfLivingStateList, int> _vCostOfLivingStateListRepository;

        /// <summary>
        /// The db context
        /// </summary>
        private readonly SALTEntities _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="SurveyService" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="surveyRepository">The survey repository.</param>
        /// <param name="dbContext">The db context.</param>
        /// <param name="memberRepository">The member repository.</param>
        /// <param name="surveyOptionRepository">The survey option repository.</param>
        /// <param name="surveyResponseRepository">The survey response repository.</param>
        /// <param name="vlcUserResponseRepository">The vlc user response repository.</param>
        /// <param name="vlcQuesetionRepository">The vlc question repository</param>
        /// <param name="refMajorRepository">The JSI Major repository</param>
        /// <param name="refStateRepository">The JSI State repository</param>
        /// <param name="refGeographicIndexRepository">The Cost of Living Geographic Index school repository</param>
        /// <param name="vCostOfLivingStateListRepository">The Cost of Living State list repository</param>
        public SurveyService(ILog logger, IRepository<Survey, int> surveyRepository,
                                        SALTEntities dbContext,
                                        IMemberRepository<Member, int> memberRepository,
                                        IRepository<SurveyOption, int> surveyOptionRepository,
                                        IRepository<SurveyResponse, int> surveyResponseRepository,
                                        IRepository<VLCUserResponse, int> vlcUserResponseRepository,
                                        IRepository<VLCQuestion, int> vlcQuestionResponseRepository,
                                        IRepository<RefMajor, int> refMajorRepository,
                                        IRepository<RefState, int> refStateRepository,
                                        IRepository<RefGeographicIndex, int> refGeographicIndexRepository,
                                        IRepository<vCostOfLivingStateList, int> vCostOfLivingStateListRepository)
        {
            _log = logger;

            _dbContext = dbContext;
            _surveyRepository = surveyRepository;
            _surveyResponseRepository = surveyResponseRepository;
            _surveyOptionRepository = surveyOptionRepository;
            _memberRepository = memberRepository;
            _vlcUserResponseRepository = vlcUserResponseRepository;
            _vlcQuestionRepository = vlcQuestionResponseRepository;
            _refMajorRepository = refMajorRepository;
            _refStateRepository = refStateRepository;
            _refGeographicIndexRepository = refGeographicIndexRepository;
            _vCostOfLivingStateListRepository = vCostOfLivingStateListRepository;
        }

        /// <summary>
        /// Gets the survey.
        /// </summary>
        /// <param name="surveyId">The survey id.</param>
        /// <returns></returns>
        public Survey GetSurvey(int surveyId)
        {
            var survey = _surveyRepository.Get(s => s.SurveyId == surveyId, null, "SurveyOptions").Single();
            var surveyResponses = _surveyResponseRepository.Get(so => so.SurveyOption.SurveyId == surveyId).ToList();

            foreach (var option in survey.SurveyOptions)
            {
                option.TotalResponseCount = surveyResponses.Count(sr => sr.SurveyOptionId == option.SurveyOptionId);
            }

            return survey;
        }

        /// <summary>
        /// Gets the survey results.
        /// </summary>
        /// <param name="surveyId">The survey id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public SurveyResponse GetUserSurveyResults(int surveyId, int userId)
        {
            return _surveyResponseRepository.Get(sr => sr.MemberId == userId && sr.SurveyOption.SurveyId == surveyId).FirstOrDefault();
        }

        /// <summary>
        /// Posts the survey.
        /// </summary>
        /// <param name="surveyResponse">The survey response.</param>
        /// <returns></returns>
        public bool PostSurvey(SurveyResponse surveyResponse)
        {
            try
            {
                surveyResponse.CreatedBy = Principal.GetIdentity();
                surveyResponse.CreatedDate = DateTime.Now;

                if (!string.IsNullOrEmpty(surveyResponse.SurveyOptionText))
                {
                    surveyResponse.SurveyOptionId =
                        _surveyOptionRepository.Get(o => o.OptionValue == surveyResponse.SurveyOptionText)
                                               .First()
                                               .SurveyOptionId;
                }

                if (!surveyResponse.Validate().Any())
                {
                    IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);
                    _surveyResponseRepository.Add(surveyResponse);
                    unitOfWork.Commit();
                }

                return true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Posts the VLC Question Response.
        /// </summary>
        /// <param name="response">The VLC response.</param>
        /// <returns></returns>
        public bool AddVlcResponse(VLCUserResponse response)
        {
            try
            {
                //Make sure CreatedBy is set before trying to make a query from the repository or else we might get null reference exceptions
                response.VLCQuestion.CreatedBy = "WillBeOverrittenByStoredProc";
                IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);
                //Get questionID based on externalItemId (CM1 id)
                short questionID = response.VLCQuestion.ExternalItemID;
                var questionResponse = _vlcQuestionRepository.Get(m => m.ExternalItemID == questionID).OrderByDescending(n => n.ExternalItemVersionNumber).FirstOrDefault();

                //If we got back no rows, or Version number didnt match
                if (questionResponse == null || response.VLCQuestion.ExternalItemVersionNumber > questionResponse.ExternalItemVersionNumber)
                {
                    //The question being answered does not exist, or has been changed/updated 
                    //Add it to the database.  
                    //Then make a get call to retreive the QuestionID that the DB assigned
                    // if the question was updated set question ID to 0 so it can be inserted as a new question
                    response.VLCQuestion.VLCQuestionID = 0;
                    _vlcQuestionRepository.Add(response.VLCQuestion);

                    unitOfWork.Commit();

                    //Make get query again now that Question has been added
                    questionResponse = _vlcQuestionRepository.Get(m => m.ExternalItemID == questionID).OrderByDescending(n => n.ExternalItemVersionNumber).FirstOrDefault();

                }

                //Set properties that are required by the stored procedure
                response.CreatedBy = "WillBeOverrittenByStoredProc";
                response.ResponseDate = System.DateTime.Now;
                response.VLCQuestionID = questionResponse.VLCQuestionID;

                // setting VLCQuestion to null so we can only save to the VLCQuestionRepsonse Table (If VLCQuestion is non null it will trigger its insert stored proc)
                response.VLCQuestion = null;

                _vlcUserResponseRepository.Add(response);
                unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {

                _log.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Posts the JellyVision Quiz Response.
        /// </summary>
        /// <param name="response">The JellyVision Quiz Response.</param>
        /// <returns>true/false</returns>
        public bool AddJellyVisionQuizResponse(JellyVisionQuizResponse response)
        {
            const string logMethodName = ".AddJellyVisionQuizResponse(JellyVisionQuizResponse response) - ";
            _log.Debug(logMethodName + "Begin Method");

            try
            {
                //the insert function is mapped to a JellyVisionQuiz stored proc. 
                //the function import in the edmx can't be used due to conflicting type names.
                IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);

                string sqlCommand = String.Format("exec {0} {1}={2}, {3}='{4}', {5}='{6}', {7}='{8}', {9}='{10}'",
                                                    StoredProcedures.AddMemberQuiz.StoredProcedureName
                                                  , StoredProcedures.AddMemberQuiz.Parameters.MemberId, response.MemberId > 0 ? response.MemberId.ToString() : "null"
                                                  , StoredProcedures.AddMemberQuiz.Parameters.QuizName, response.QuizName
                                                  , StoredProcedures.AddMemberQuiz.Parameters.QuizTakenSite, response.QuizTakenSite
                                                  , StoredProcedures.AddMemberQuiz.Parameters.QuizResult, response.QuizResult
                                                  , StoredProcedures.AddMemberQuiz.Parameters.QuizResponse, response.QuizResponse);
                _dbContext.Database.ExecuteSqlCommand(sqlCommand);

                unitOfWork.Commit();

                _log.Debug(logMethodName + "End Method");
                return true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the States that we have Cost of Living information.
        /// </summary>
        /// <param name=""></param>
        /// <returns>List of vCostOfLivingStates</returns>
        public List<vCostOfLivingStateList> GetCOLStates()
        {
            return _vCostOfLivingStateListRepository.GetAll().ToList();
        }

        /// <summary>
        /// Gets Cost of Living urban areas for the stateId provided.
        /// </summary>
        /// <param name=""></param>
        /// <returns>List of vCostOfLivingStates</returns>
        public List<RefGeographicIndex> GetCOLUrbanAreas(int stateId)
        {
            return _refGeographicIndexRepository.Get(g => g.RefStateID == stateId).ToList();
        }

        /// <summary>
        /// Gets Cost of Living results comparisons for the inputs provided.
        /// </summary>
        /// <param name="cityA">The RefGeographicIndexID for CityA.</param>
        /// <param name="cityB">The RefGeographicIndexID for CityB.</param>
        /// <param name="salary">salary</param>
        /// <returns>Cost Of Living results</returns>
        public COLResults GetCOLResults(int cityA, int cityB, decimal salary)
        {
            const string logMethodName = ".GetCOLResultss(int cityA, int cityB, decimal salary) - ";
            _log.Debug(logMethodName + "Begin Method");

            try
            {
                COLResults colResults = new COLResults();

                //the results function is mapped to a Cost of Living stored proc. 
                //the function import in the edmx can't be used due to conflicting type names.
                //IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);

                string sqlCommand = String.Format("exec {0} {1}={2}, {3}={4}, {5}={6}",
                                    StoredProcedures.CostofLivingResult.StoredProcedureName
                                    , StoredProcedures.CostofLivingResult.Parameters.CityA_RefGeographicIndexID, cityA
                                    , StoredProcedures.CostofLivingResult.Parameters.CityB_RefGeographicIndexID, cityB
                                    , StoredProcedures.CostofLivingResult.Parameters.Salary, salary);

                SqlDataReader dataFirstReader;

                //opening the connection
                using (SqlConnection dbConn1 = new SqlConnection(_dbContext.Database.Connection.ConnectionString))
                {
                    dbConn1.Open();

                    using (SqlCommand sqlComm1 = new SqlCommand(sqlCommand, dbConn1))
                    {
                        //executing the command and assigning it to connection 
                        dataFirstReader = sqlComm1.ExecuteReader();
                        while (dataFirstReader.Read())
                        {
                            colResults.ComparableSalary = Convert.ToDecimal(dataFirstReader[0].ToString());
                            colResults.IncomeToMaintain = Convert.ToDecimal(dataFirstReader[1].ToString());
                            colResults.IncomeToSustain = Convert.ToDecimal(dataFirstReader[2].ToString());
                            colResults.Groceries = Convert.ToDecimal(dataFirstReader[3].ToString());
                            colResults.Housing = Convert.ToDecimal(dataFirstReader[4].ToString());
                            colResults.Utilities = Convert.ToDecimal(dataFirstReader[5].ToString());
                            colResults.Transportation = Convert.ToDecimal(dataFirstReader[6].ToString());
                            colResults.Health = Convert.ToDecimal(dataFirstReader[7].ToString());
                            colResults.Miscellaneous = Convert.ToDecimal(dataFirstReader[8].ToString());
                        }
                        dataFirstReader.Close();
                    }
                }

                _log.Debug(logMethodName + "End Method");
                return colResults;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the JSI Majors.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public IList<RefMajor> GetRefMajors()
        {
            var refMajor = _refMajorRepository.GetAll().ToList();
            return refMajor;
        }

        /// <summary>
        /// Gets the JSI States.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public IList<RefState> GetRefStates()
        {
            var refState = _refStateRepository.GetAll().ToList();
            return refState;
        }

        /// <summary>
        /// Gets the JSI Schools by MajorId and StateId.
        /// </summary>
        /// <param name="majorId">JSI RefMajorId</param>
        /// <param name="stateId">JSI RefStateId</param>
        /// <returns>IList<JSISchoolMajor></returns>
        public IList<JSISchoolMajor> GetJSISchools(int majorId, int stateId)
        {
            const string logMethodName = ".GetJSISchools(int majorId, int stateId) - ";
            _log.Info(logMethodName + "Begin Method");
            _log.Debug(logMethodName + string.Format("input majorId={0}, stateId={1}", majorId, stateId));

            using (_dbContext)
            {
                var returnData = (from RefSalaryEstimatorSchoolMajor in _dbContext.RefSalaryEstimatorSchoolMajors
                                  where
                                    RefSalaryEstimatorSchoolMajor.RefMajorID == majorId &&
                                    RefSalaryEstimatorSchoolMajor.RefSalaryEstimatorSchool.RefStateID == stateId
                                  select new JSISchoolMajor
                                  {
                                      SchoolName = RefSalaryEstimatorSchoolMajor.RefSalaryEstimatorSchool.SchoolName,
                                      SchoolID = (System.Int32)RefSalaryEstimatorSchoolMajor.RefSalaryEstimatorSchool.RefSalaryEstimatorSchoolID,
                                      RefStateID = (System.Int32)RefSalaryEstimatorSchoolMajor.RefSalaryEstimatorSchool.RefStateID,
                                      RefMajorID = RefSalaryEstimatorSchoolMajor.RefMajorID
                                  });

                _log.Debug(string.Format("rows returned for school/majors = {0}", returnData.Count()));
                _log.Info(logMethodName + "End Method");

                return returnData.ToList();
            }
        }

        /// <summary>
        /// Posts the JSI Quiz Response.
        /// </summary>
        /// <param name="response">The JSI Quiz Response.</param>
        /// <returns>JSIQuizResultsContract</returns>
        public IList<JSIQuizResult> PostJSIResponse(JSIQuizAnswer response)
        {
            const string logMethodName = ".PostJSIResponse(JSIQuizAnswer response) - ";
            _log.Debug(logMethodName + "Begin Method");

            try
            {
                IList<JSIQuizResult> jsiQuizResultList = new List<JSIQuizResult>();

                //the insert function is mapped to a JSI stored proc. 
                //the function import in the edmx can't be used due to conflicting type names.
                IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);

                //does not include memeberId, that will be added seperately if present
                string sqlCommand = String.Format("exec {0} {1}={2}, {3}={4}",
                                    StoredProcedures.InsertJSIMemberSalaryEstimator.StoredProcedureName
                                    , StoredProcedures.InsertJSIMemberSalaryEstimator.Parameters.RefSalaryEstimatorSchoolID, response.RefSalaryEstimatorSchoolID
                                    , StoredProcedures.InsertJSIMemberSalaryEstimator.Parameters.RefMajorID, response.RefMajorID);

                //add memeberId to sqlCommand if present
                if (response.MemberID > 0)
                {
                    sqlCommand += String.Format(", {0}={1}",
                                    StoredProcedures.InsertJSIMemberSalaryEstimator.Parameters.MemberID, response.MemberID);
                }

                SqlDataReader dataFirstReader;

                //opening the connection
                using (SqlConnection dbConn1 = new SqlConnection(_dbContext.Database.Connection.ConnectionString))
                {
                    dbConn1.Open();

                    using (SqlCommand sqlComm1 = new SqlCommand(sqlCommand, dbConn1))
                    {

                        //executing the command and assigning it to connection 
                        dataFirstReader = sqlComm1.ExecuteReader();
                        while (dataFirstReader.Read())
                        {
                            JSIQuizResult jsiQuizResult = new JSIQuizResult();
                            jsiQuizResult.OccupationName = dataFirstReader[0].ToString();
                            jsiQuizResult.EstimatedSalaryAmount = dataFirstReader[1].ToString();
                            jsiQuizResultList.Add(jsiQuizResult);
                        }
                        dataFirstReader.Close();

                        unitOfWork.Commit();
                    }
                }

                _log.Debug(logMethodName + "End Method");
                return jsiQuizResultList;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                throw;
            }
        }
    }
}
