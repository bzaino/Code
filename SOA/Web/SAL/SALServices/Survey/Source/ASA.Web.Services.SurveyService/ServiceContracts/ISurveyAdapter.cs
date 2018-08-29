using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.Common;
using ASA.Web.Services.SurveyService.DataContracts;
using ASA.Web.Services.Proxies;
using ASA.Web.Services.Proxies.SALTService;

namespace ASA.Web.Services.SurveyService.ServiceContracts
{
    public interface ISurveyAdapter
    {
        /// <summary>
        /// Inserts the survey.
        /// </summary>
        /// <param name="survey">The survey.</param>
        /// <returns></returns>
        ResultCodeModel InsertSurvey(SurveyModel survey);

        /// <summary>
        /// Gets the question and response.
        /// </summary>
        /// <param name="surveyId">The survey id.</param>
        /// <returns></returns>
        SurveyListModel GetQuestionAndResponse(int surveyId);

        /// <summary>
        /// Gets the individuals response.
        /// </summary>
        /// <param name="surveyId">The survey id.</param>
        /// <param name="memberId">The member id.</param>
        /// <returns></returns>
        SurveyListModel GetIndividualsResponse(int surveyId, int memberId);

        /// <summary>
        /// Gets the response and totals.
        /// </summary>
        /// <param name="surveyId">The survey id.</param>
        /// <returns></returns>
        SurveyListModel GetResponseAndTotals(int surveyId);

        /// <summary>
        /// Post the question response.
        /// </summary>
        /// <param name="response">The vlc question response.</param>
        /// <returns></returns>
        bool AddVlcResponse(VLCQuestionResponseModel response);

        /// <summary>
        /// Post the results to the JellyVision quiz.
        /// </summary>
        /// <param name="response">The JellyVision quiz responses.</param>
        /// <returns></returns>
        bool AddJellyVisionQuizResponse(JellyVisionQuizResponseModel response);

        /// <summary>
        /// Get the list of Cost of Living states.
        /// </summary>
        /// <returns>COLResponseModel containing the COLStateModel list of States.</returns>
        COLResponseModel GetCOLStateList();

        /// <summary>
        /// Get the list of Cost of Living urban areas.
        /// </summary>
        /// <param name="request">The COLRequestModel with the state id provided.</param>
        /// <returns>COLResponseModel containing the COLUrbanAreaModel list of urban areas.</returns>
        COLResponseModel GetCOLUrbanAreaList(COLRequestModel request);

        /// <summary>
        /// Gets the CostOfLiving results.
        /// </summary>
        /// <param name="request">The COLRequestModel with the CityA, "CityB and Salary provided.</param>
        /// <returns>COLResponseModel with COLCostBreakDownModel and NeededSalary & percentageSalaryChange</returns>
        /// <exception cref="SurveyOperationException">Web Survey Service - ASA.Web.Services.SurveyService.SurveyAdapter.GetCOLStateList()</exception>
        COLResponseModel GetCOLResults(COLRequestModel request);

        /// <summary>
        /// Get the list of JSI majors.
        /// </summary>
        /// <param name="response">The JSI list of Majors.</param>
        /// <returns>JSIQuizListModel</returns>
        JSIQuizListModel GetJSIMajorList();
        
        /// <summary>
        /// Get the list of JSI states.
        /// </summary>
        /// <param name="response">The JSI list of States.</param>
        /// <returns>JSIQuizListModel</returns>
        JSIQuizListModel GetJSIStateList();

        /// <summary>
        /// Get the list of JSI schools.
        /// </summary>
        /// <param name="majorId">JSI RefMajorId</param>
        /// <param name="stateId">JSI RefStateId</param>
        /// <returns>JSIQuizListModel</returns>
        JSIQuizListModel GetJSISchoolList(int majorId, int stateId);

        /// <summary>
        /// Post the results of the JSI quiz.
        /// </summary>
        /// <param name="jsiQuizModel">single JSIQuizModel object</param>
        /// <returns>JSIQuizListModel</returns>
        JSIQuizListModel PostJSIResponse(JSIQuizModel jsiQuizModel);
    }
}
