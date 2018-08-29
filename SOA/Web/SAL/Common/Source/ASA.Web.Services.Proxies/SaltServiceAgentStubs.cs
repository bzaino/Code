using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ASA.Web.Services.Proxies.SALTService;
using Asa.Salt.Web.Common.Types.Enums;
using MemberUpdateStatus = Asa.Salt.Web.Common.Types.Enums.MemberUpdateStatus;

namespace ASA.Web.Services.Proxies
{
    public class SaltServiceAgentStub : ISaltServiceAgent
    {
        public MemberContract GetUserByActiveDirectoryKeyResponse { get; set; }
        public MemberContract GetUserByActiveDirectoryKey(string activeDirectoryKey)
        {
            return GetUserByActiveDirectoryKeyResponse;
        }

        public MemberContract GetUserByUsernameResponse { get; set; }
        public MemberContract GetUserByUsername(string username)
        {
            return GetUserByUsernameResponse;
        }

        public bool DeactivateUserResponse { get; set; }
        public bool DeactivateUser(int userId, string modifierUserName)
        {
            return DeactivateUserResponse;
        }

        public MemberUpdateStatus UpdateUserResponse { get; set; }
        public MemberUpdateStatus UpdateUser(MemberContract user)
        {
            return UpdateUserResponse;
        }

        public List<RefOrganizationProductContract> GetOrganizationProductsResponse { get; set; }
        /// <summary>
        /// Gets a stubbed out Organization products.
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns>List<RefOrganizationProductContract></returns>
        public List<RefOrganizationProductContract> GetOrganizationProducts(int orgId)
        {
            return GetOrganizationProductsResponse;
        }

        public RefOrganizationContract GetOrganizationResponse { get; set; }
        public RefOrganizationContract GetOrganization(string opeCode, string branchCode)
        {
            return GetOrganizationResponse;
        }

        public RefOrganizationContract GetOrganizationByExternalOrgID(string externalOrgID)
        {
            return GetOrganizationResponse;
        }
        
        public List<MemberContentInteractionContract> GetUserInteractionsResponse { get; set; }
        public List<MemberContentInteractionContract> GetUserInteractions(int userId, Int32 contentType)
        {
            return GetUserInteractionsResponse;
        }
        public List<MemberContentInteractionContract> GetUserInteractions(int userId, string contentId, Int32 contentType)
        {
            return GetUserInteractionsResponse;
        }

        public bool DeleteInteractionResponse { get; set; }
        public bool DeleteInteraction(int userId, int id)
        {
            return DeleteInteractionResponse;
        }

        public MemberContentInteractionContract AddInteractionResponse { get; set; }
        public MemberContentInteractionContract AddInteraction(MemberContentInteractionContract interaction)
        {
            return AddInteractionResponse;
        }

        public MemberContentInteractionContract UpdateInteractionResponse { get; set; }
        public MemberContentInteractionContract UpdateInteraction(int userId, MemberContentInteractionContract interactionToUpdate)
        {
            return UpdateInteractionResponse;
        }
                
        public RegisterMemberResultContract RegisterUser(UserRegistrationContract user)
        {
            return new RegisterMemberResultContract();
        }

        public MemberContract GetUserByUserIdResponse { get; set; }
        public MemberContract GetUserByUserId(int userId)
        {
            return GetUserByUserIdResponse;
        }

        public List<vCostOfLivingStateListContract> GetCOLStatesResponse { get; set; }
        public List<vCostOfLivingStateListContract> GetCOLStates()
        {
            return GetCOLStatesResponse;
        }

        public List<RefGeographicIndexContract> GetCOLUrbanAreasResponse { get; set; }
        public List<RefGeographicIndexContract> GetCOLUrbanAreas(int stateId)
        {
            return GetCOLUrbanAreasResponse;
        }

        public COLResultsContract GetCOLResultsResponse { get; set; }
        public COLResultsContract GetCOLResults(int cityA, int cityB, decimal salary)
        {
            return GetCOLResultsResponse;
        }

        public List<RefMajorContract> GetJSIMajorResponse { get; set; }
        public List<RefMajorContract> GetJSIMajor()
        {
            return GetJSIMajorResponse;
        }

        public List<RefStateContract> GetJSIStateResponse { get; set; }
        public List<RefStateContract> GetJSIState()
        {
            return GetJSIStateResponse;
        }

        public List<JSISchoolMajorContract> GetJSISchoolMajorResponse { get; set; }
        public List<JSISchoolMajorContract> GetJSISchoolMajor(int majorId, int stateId)
        {
            return GetJSISchoolMajorResponse;
        }

        public List<JSIQuizResultContract> PostJSIResponseResponse { get; set; }
        public List<JSIQuizResultContract> PostJSIResponse(JSIQuizAnswerContract answers)
        {
            return PostJSIResponseResponse;
        }

        public bool AddJellyVisionQuizResponseResponse { get; set; }
        public bool AddJellyVisionQuizResponse(JellyVisionQuizResponseContract response)
        {
            return AddJellyVisionQuizResponseResponse;
        }

        public SurveyContract GetSurveyByIdResponse { get; set; }
        public SurveyContract GetSurveyById(int surveyId)
        {
            return GetSurveyByIdResponse;
        }

        public SurveyResponseContract GetUserSurveyResultsResponse { get; set; }
        public SurveyResponseContract GetUserSurveyResults(int surveyId, int userId)
        {
            return GetUserSurveyResultsResponse;
        }

        public bool PostSurveyResponse { get; set; }
        public bool PostSurvey(SurveyResponseContract surveyResponse)
        {
            return PostSurveyResponse;
        }

        public List<RefProfileQuestionContract> GetAllProfileQuestionsResponse { get; set; }
        public List<RefProfileQuestionContract> GetAllProfileQuestions()
        {
            return GetAllProfileQuestionsResponse;
        }

        public List<RefProfileAnswerContract> GetAllProfileAnswersResponse { get; set; }
        public List<RefProfileAnswerContract> GetAllProfileAnswers()
        {
            return GetAllProfileAnswersResponse;
        }

        public List<MemberProfileQAContract> GetAllProfileResponses { get; set; }
        public List<MemberProfileQAContract> GetProfileResponses(int memberId)
        {
            return GetAllProfileResponses;
        }


        public List<MemberReportedLoanContract> GetUserReportedLoansRecordSourceListResponses { get; set; }
        public List<MemberReportedLoanContract> GetUserReportedLoansRecordSourceList(int userId, string[] recordSourceNames)
        {
            return GetUserReportedLoansRecordSourceListResponses;
        }

        public List<MemberReportedLoanContract> GetUserReportedLoansResponses { get; set; }
        public List<MemberReportedLoanContract> GetUserReportedLoans(int userId)
        {
            return GetUserReportedLoansResponses;
        }

        public List<RefOrganizationProductContract> GetOrganizationProductsByMemberIdResponses { get; set; }
        public List<RefOrganizationProductContract> GetOrganizationProductsByMemberId(int userId)
        {
            if (GetOrganizationProductsByMemberIdResponses == null)
            {
                return new List<RefOrganizationProductContract>();
            }
            else
            {
                return GetOrganizationProductsByMemberIdResponses;
            }
        }

        public List<MemberToDoListContract> GetMemberToDosResponses { get; set; }
        public List<MemberToDoListContract> GetMemberToDos(int userId)
        {
            if (GetMemberToDosResponses == null)
            {
                return new List<MemberToDoListContract>();
            }
            else
            {
                return GetMemberToDosResponses;
            }
        }
    }
}
