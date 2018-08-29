using System.Collections.Generic;
using System.Globalization;
using ASA.Web.Services.ASAMemberService;
using ASA.Web.Services.Proxies.SALTService;
using ASA.Web.Services.SurveyService.DataContracts;
using System;

namespace ASA.Web.Services.SurveyService
{
    public static class TranslateSurveyModel
    {

        /// <summary>
        /// To the domain model.
        /// </summary>
        /// <param name="surveyDataContract">The survey data contract.</param>
        /// <returns></returns>
        public static SurveyModel ToDomainModel(this SurveyContract surveyDataContract)
        {
            var memberAdapter = new AsaMemberAdapter();
            return surveyDataContract == null
                       ? null
                       : new SurveyModel()
                           {
                               ListOfAnswerOptions = surveyDataContract.ListOfValues,
                               QuestionText = surveyDataContract.SurveyQuestion,
                               SurveyId = surveyDataContract.SurveyId.ToString(CultureInfo.InvariantCulture),
                               IndividualId = memberAdapter.GetActiveDirectoryKeyFromContext(),
                               MemberId = memberAdapter.GetMemberIdFromContext(),
                               SurveyQuestionId=surveyDataContract.SurveyId.ToString(CultureInfo.InvariantCulture)
                           };
        }

        /// <summary>
        /// To the domain model.
        /// </summary>
        /// <param name="surveyResponseDataContract">The survey response data contract.</param>
        /// <returns></returns>
        public static SurveyListModel ToDomainModel(this SurveyResponseContract surveyResponseDataContract)
        {
            if (null == surveyResponseDataContract)
            {
                return new SurveyListModel();
            }

            var toReturn = new SurveyListModel
                {
                    Surveys = new List<SurveyModel>
                        {
                            new SurveyModel()
                                {
                                    IndividualId = new AsaMemberAdapter().GetActiveDirectoryKeyFromContext(),
                                    MemberId = surveyResponseDataContract.MemberId,
                                    ResponseCount = surveyResponseDataContract.TotalResponseCount,
                                    Response = surveyResponseDataContract.SurveyResponseId.ToString(CultureInfo.InvariantCulture),
                                }
                        }
                };
            return toReturn;

        }

        /// <summary>
        /// To the data contract.
        /// </summary>
        /// <param name="surveyResponse">The survey response.</param>
        /// <returns></returns>
        public static SurveyResponseContract ToDataContract(this SurveyModel surveyResponse)
        {
            return new SurveyResponseContract()
            {

                MemberId = surveyResponse.MemberId,
                SurveyOptionText = System.Web.HttpUtility.HtmlDecode(surveyResponse.Response)
                
            };

        }

        /// <summary>
        /// VLCQuestionResponseModel to its data contract.
        /// </summary>
        /// <param name="response">The VLC Question Response model </param>
        /// <returns></returns>
        public static VLCUserResponseContract ToDataContract(this VLCQuestionResponseModel response)
        {
            return new VLCUserResponseContract()
            {
                MemberID = response.MemberID,
                Response = response.ResponseText,
                ResponseDate = response.ResponseDate,
                VLCQuestionID = response.VLCQuestionID,
                VLCUserResponseID = response.VLCUserResponseID,
                VLCQuestion = new VLCQuestionContract() { 
                    ExternalItemID = Convert.ToInt16(response.Question.QuestionID), 
                    ExternalItemVersionNumber = Convert.ToByte(response.Question.QuestionVersion), 
                    Question = response.Question.QuestionText  }

            };
        }

        /// <summary>
        /// JellyVisionQuizResponseModel to its data contract.
        /// </summary>
        /// <param name="response">The JellyVisionQuiz Response model </param>
        /// <returns></returns>
        public static JellyVisionQuizResponseContract ToDataContract(this JellyVisionQuizResponseModel response)
        {
            return new JellyVisionQuizResponseContract()
            {
                MemberId = response.Id,
                QuizName = response.quizName,
                QuizResponse = response.responses,
                QuizResult = response.personalityType,
                QuizTakenSite = response.referrerID
            };
        }

        /// <summary>
        /// Converts vCostOfLivingStateList contracts to the domain COLResponseModel COLStateModel object.
        /// </summary>
        /// <param name="vCostOfLivingStateListContracts">The vCostOfLivingStateList contracts.</param>
        /// <returns>List of COLStateModel</returns>
        public static List<COLStateModel> ToDomainObject(this List<vCostOfLivingStateListContract> vCostOfLivingStateListContracts)
        {
            List<COLStateModel> toReturn = new List<COLStateModel>();
            foreach (var item in vCostOfLivingStateListContracts)
            {
                toReturn.Add(new COLStateModel()
                {
                    State = item.StateName,
                    StateCode = item.StateCode,
                    StateId = item.RefStateID
                });
            }

            return toReturn;
        }

        /// <summary>
        /// Converts RefGeographicIndex contracts to the domain CostofLiving UrbanArea object.
        /// </summary>
        /// <param name="refGeographicIndexContracts">The RefGeographicIndex contracts.</param>
        /// <returns>List of COLUrbanAreaModel</returns>
        public static List<COLUrbanAreaModel> ToDomainObject(this List<RefGeographicIndexContract> refGeographicIndexContracts)
        {
            var toReturn = new List<COLUrbanAreaModel>() { };
            foreach (var item in refGeographicIndexContracts)
            {
                toReturn.Add(new COLUrbanAreaModel()
                {
                    RefGeographicalIndexID = item.RefGeographicIndexID,
                    UrbanAreaName = item.UrbanAreaName
                });
            }

            return toReturn;
        }

        /// <summary>
        /// Converts COLResults contract to the domain CostofLiving COLCostBreakDownModel object and couple of fields.
        /// </summary>
        /// <param name="refGeographicIndexContracts">The COLResults contracts.</param>
        /// <returns>COLResponseModel with COLCostBreakDownModel object and couple of fields populated.</returns>
        public static COLResponseModel ToDomainObject(this COLResultsContract colResultsContract)
        {
            var toReturn = new COLResponseModel { };
            toReturn.CostBreakDown = new List<COLCostBreakDownModel>();

            toReturn.ComparableSalary = colResultsContract.ComparableSalary;
            toReturn.PercentageIncomeToMaintain = colResultsContract.IncomeToMaintain;
            toReturn.PercentageIncomeToSustain = colResultsContract.IncomeToSustain;

            //Housing
            toReturn.CostBreakDown.Add(SetCostBreakDownModel("Housing", colResultsContract.Housing));

            //Food
            toReturn.CostBreakDown.Add(SetCostBreakDownModel("Food", colResultsContract.Groceries));

            //Utilities
            toReturn.CostBreakDown.Add(SetCostBreakDownModel("Utilities", colResultsContract.Utilities));

            //Transportation
            toReturn.CostBreakDown.Add(SetCostBreakDownModel("Transportation", colResultsContract.Transportation));

            //Healthcare
            toReturn.CostBreakDown.Add(SetCostBreakDownModel("Healthcare", colResultsContract.Health));

            //Miscellaneous - not needed/used ny the fromt-end, but it is returned by the SaltService
            //toReturn.CostBreakDown.Add(SetCostBreakDownModel("Miscellaneous", colResultsContract.Miscellaneous));

            return toReturn;
        }

        public static COLCostBreakDownModel SetCostBreakDownModel(string category, decimal percentageChange)
        {
            COLCostBreakDownModel colCostBreakDownModel = new COLCostBreakDownModel();

            colCostBreakDownModel.Category = category;
            colCostBreakDownModel.PercentageChange = percentageChange;

            //value of zero we leave empty for PercentageMoreOrLess
            if (percentageChange > 0)
            {
                colCostBreakDownModel.PercentageChangeIndicator = "higher";
            }

            if (percentageChange < 0)
            {
                colCostBreakDownModel.PercentageChangeIndicator = "lower";
            }

            return colCostBreakDownModel;
        }

        /// <summary>
        /// Converts RefMajor contracts to the domain JSI ? object.
        /// </summary>
        /// <param name="jsiQuestionsContracts">The jsiQuestions contracts.</param>
        /// <returns>List of JSIQuizModel</returns>
        public static List<JSIQuizModel> ToDomainObject(this List<RefMajorContract> refMajorContracts)
        {
            var toReturn = new List<JSIQuizModel>() { };
            var model = new JSIQuizModel() { };
            foreach (var item in refMajorContracts)
            {
                toReturn.Add(new JSIQuizModel() { 
                                    Major = item.MajorName,
                                    MajorId = item.RefMajorID.ToString()
                });
            }

            return toReturn;
        }

        /// <summary>
        /// Converts RefState contracts to the domain JSI ? object.
        /// </summary>
        /// <param name="jsiQuestionsContracts">The jsiQuestions contracts.</param>
        /// <returns>List of JSIQuizModel</returns>
        public static List<JSIQuizModel> ToDomainObject(this List<RefStateContract> refStateContracts)
        {
            var toReturn = new List<JSIQuizModel>() { };
            var model = new JSIQuizModel() { };
            foreach (var item in refStateContracts)
            {
                toReturn.Add(new JSIQuizModel()
                {
                    State = item.StateName,
                    StateCd = item.StateCode,
                    StateId = item.RefStateID.ToString()
                });
            }

            return toReturn;
        }

        /// <summary>
        /// Converts JSISchoolMajor contracts to the domain JSI ? object.
        /// </summary>
        /// <param name="jsiQuestionsContracts">The jsiQuestions contracts.</param>
        /// <returns>List of JSIQuizModel</returns>
        public static List<JSIQuizModel> ToDomainObject(this List<JSISchoolMajorContract> jsiSchoolMajorContracts)
        {
            var toReturn = new List<JSIQuizModel>() { };
            var model = new JSIQuizModel() { };
            foreach (var item in jsiSchoolMajorContracts)
            {
                toReturn.Add(new JSIQuizModel()
                {
                    School = item.SchoolName,
                    SchoolId = item.SchoolID.ToString()
                });
            }

            return toReturn;
        }

        /// <summary>
        /// jsiQuizModel to its data contract.
        /// </summary>
        /// <param name="response">The JSI Quiz Response model </param>
        /// <returns></returns>
        public static JSIQuizAnswerContract ToDataContract(this JSIQuizModel jsiQuizModel)
        {
            return new JSIQuizAnswerContract()
            {
                MemberID = jsiQuizModel.MemberId > 0 ? jsiQuizModel.MemberId : 0,
                RefMajorID = Convert.ToInt32(jsiQuizModel.MajorId),
                RefSalaryEstimatorSchoolID = Convert.ToInt32(jsiQuizModel.SchoolId)
            };
        }

        /// <summary>
        /// Converts JSIResult contracts to the domain JSI ? object.
        /// </summary>
        /// <param name="jsiQuestionsContracts">The jsiQuestions contracts.</param>
        /// <returns>List of JSIQuizModel</returns>
        public static List<JSIQuizModel> ToDomainObject(this List<JSIQuizResultContract> jsiQuizResultContracts)
        {
            var toReturn = new List<JSIQuizModel>() { };
            var model = new JSIQuizModel() { };
            foreach (var item in jsiQuizResultContracts)
            {
                toReturn.Add(new JSIQuizModel()
                {
                    OccupationName = item.OccupationName,
                    EstimatedSalaryAmount = item.EstimatedSalaryAmount
                });
            }

            return toReturn;
        }
    }
}
