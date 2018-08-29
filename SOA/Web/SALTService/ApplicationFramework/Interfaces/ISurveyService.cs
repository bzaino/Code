using System.Collections.Generic;
using Asa.Salt.Web.Services.Domain;

namespace Asa.Salt.Web.Services.BusinessServices.Interfaces
{
    public interface ISurveyService
    {

        /// <summary>
        /// Gets the survey.
        /// </summary>
        /// <param name="surveyId">The survey id.</param>
        /// <returns></returns>
        Survey GetSurvey(int surveyId);

        /// <summary>
        /// Gets the survey results.
        /// </summary>
        /// <param name="surveyId">The survey id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        SurveyResponse GetUserSurveyResults(int surveyId, int userId);

        /// <summary>
        /// Posts the survey.
        /// </summary>
        /// <param name="surveyResponse">The survey response.</param>
        /// <returns></returns>
        bool PostSurvey(SurveyResponse surveyResponse);

        /// <summary>
        /// Posts the VLC question response.
        /// </summary>
        /// <param name="response">The question response.</param>
        /// <returns></returns>
        bool AddVlcResponse(VLCUserResponse response);

        /// <summary>
        /// Posts the JellyVision Quiz Response.
        /// </summary>
        /// <param name="response">The JellyVision Quiz Response.</param>
        /// <returns>true/false</returns>
        bool AddJellyVisionQuizResponse(JellyVisionQuizResponse response);

        /// <summary>
        /// Gets the States that we have Cost of Living information.
        /// </summary>
        /// <returns>IList of vCostOfLivingStateList</returns>
        List<vCostOfLivingStateList> GetCOLStates();

        /// <summary>
        /// Gets the Cost of Living Geographic data for a specified state.
        /// </summary>
        /// <returns>List of RefGeographicIndex</returns>
        List<RefGeographicIndex> GetCOLUrbanAreas(int stateCd);

        /// <summary>
        /// Gets Cost of Living results comparisons for the inputs provided.
        /// </summary>
        /// <param name="cityA">The RefGeographicIndexID for CityA.</param>
        /// <param name="cityB">The RefGeographicIndexID for CityB.</param>
        /// <param name="salary">salary</param>
        /// <returns>COLResults</returns>
        COLResults GetCOLResults(int cityA, int cityB, decimal salary);

        /// <summary>
        /// Gets the JSI Majors.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        IList<RefMajor> GetRefMajors();

        /// <summary>
        /// Gets the JSI States.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        IList<RefState> GetRefStates();
        
        /// <summary>
        /// Gets the JSI Schools by MajorId and StateId.
        /// </summary>
        /// <param name=""></param>
        /// <returns>IList<JSISchoolMajor></returns>
        IList<JSISchoolMajor> GetJSISchools(int majorId, int StateId);

        /// <summary>
        /// Posts the JSI Quiz Response.
        /// </summary>
        /// <param name="response">The JSI Quiz Response.</param>
        /// <returns>IList<JSIQuizResult></returns>
        IList<JSIQuizResult> PostJSIResponse(JSIQuizAnswer response);
    }
}
