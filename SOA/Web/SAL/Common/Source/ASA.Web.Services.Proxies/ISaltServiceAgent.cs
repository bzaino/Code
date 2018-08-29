using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ASA.Web.Services.Proxies.SALTService;
using Asa.Salt.Web.Common.Types.Enums;
using MemberUpdateStatus = Asa.Salt.Web.Common.Types.Enums.MemberUpdateStatus;

namespace ASA.Web.Services.Proxies
{
    public interface ISaltServiceAgent
    {
        bool DeactivateUser(int userId, string modifierUserName);

        MemberContract GetUserByUserId(int userId);
        MemberContract GetUserByActiveDirectoryKey(string activeDirectoryKey);
        MemberContract GetUserByUsername(string username);

        MemberUpdateStatus UpdateUser(MemberContract user);

        RefOrganizationContract GetOrganization(string opeCode, string branchCode);

        RefOrganizationContract GetOrganizationByExternalOrgID(string externalOrgId);

        List<RefOrganizationProductContract> GetOrganizationProducts(int orgId);

        List<RefOrganizationProductContract> GetOrganizationProductsByMemberId(int userId);

        List<MemberContentInteractionContract> GetUserInteractions(int userId, Int32 contentType);
        List<MemberContentInteractionContract> GetUserInteractions(int userId, string contentId, Int32 contentType);
        MemberContentInteractionContract AddInteraction(MemberContentInteractionContract interaction);
        bool DeleteInteraction(int userId, int id);
        MemberContentInteractionContract UpdateInteraction(int userId, MemberContentInteractionContract interactionToUpdate);

        List<vCostOfLivingStateListContract> GetCOLStates();
        List<RefGeographicIndexContract> GetCOLUrbanAreas(int stateId);
        COLResultsContract GetCOLResults(int cityA, int cityB, decimal salary);

        List<RefMajorContract> GetJSIMajor();
        List<RefStateContract> GetJSIState();
        List<JSISchoolMajorContract> GetJSISchoolMajor(int majorId, int stateId);
        List<JSIQuizResultContract> PostJSIResponse(JSIQuizAnswerContract answers);
        bool AddJellyVisionQuizResponse(JellyVisionQuizResponseContract response);

        SurveyContract GetSurveyById(int surveyId);
        SurveyResponseContract GetUserSurveyResults(int surveyId, int userId);
        bool PostSurvey(SurveyResponseContract surveyResponse);

        List<RefProfileQuestionContract> GetAllProfileQuestions();
        List<RefProfileAnswerContract> GetAllProfileAnswers();
        List<MemberProfileQAContract> GetProfileResponses(int memberId);

        List<MemberReportedLoanContract> GetUserReportedLoansRecordSourceList(int userId, string[] recordSourceNames);
        List<MemberReportedLoanContract> GetUserReportedLoans(int userId);
    }
}
