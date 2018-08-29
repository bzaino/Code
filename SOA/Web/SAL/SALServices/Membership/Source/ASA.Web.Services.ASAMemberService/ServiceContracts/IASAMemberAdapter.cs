using System;
using System.Collections.Generic;

using Asa.Salt.Web.Common.Types.Enums;
using ASA.Web.Services.ASAMemberService.DataContracts;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.ASAMemberService.ServiceContracts
{
    public interface IAsaMemberAdapter
    {
        /// <summary>
        /// Gets the member.
        /// </summary>
        /// <param name="memberId">The member id.</param>
        /// <returns></returns>
        ASAMemberModel GetMember(int memberId);

        /// <summary>
        /// Gets the member by email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        ASAMemberModel GetMemberByEmail(string email);

        /// <summary>
        /// Gets the member.
        /// </summary>
        /// <param name="systemId">The system id.</param>
        /// <returns></returns>
        ASAMemberModel GetMember(Guid systemId);

        /// <summary>
        /// Creates session cookies and gets the member.
        /// </summary>
        /// <param name="logon">create session cookies</param>
        /// <param name="emailAddress">member's email address.</param>
        /// <returns>ASAMemberModel</returns>
        ASAMemberModel Logon(bool logon, string emailAddress);

        /// <summary>
        /// Creates the specified member.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        RegistrationResultModel Create(ASAMemberModel member);

        /// <summary>
        /// Updates the specified member.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        MemberUpdateStatus Update(ASAMemberModel member);

        /// <summary>
        /// Delete the specified member from SALT db and ActiveDirectory.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>bool</returns>
        bool DeleteMember(ASAMemberModel member);

        /// <summary>
        /// Deactivates the account.
        /// </summary>
        /// <param name="individualId">The individual id.</param>
        /// <returns></returns>
        ResultCodeModel DeActivateAccount(string individualId);

        /// <summary>
        /// Gets the member id from context.
        /// </summary>
        /// <returns></returns>
        int GetMemberIdFromContext();

        /// <summary>
        /// Sets memberId cookie
        /// </summary>
        void SetMemberIdCookie(string memberId);

        /// <summary>
        /// Checks if the memberId passed in matches the one in context
        /// </summary>
        /// <returns>bool</returns>
        bool IsCurrentUser(string memberId);

        /// <summary>
        /// Gets the organization by oe branch.
        /// </summary>
        /// <param name="oeCode">The oe code.</param>
        /// <param name="branch">The branch.</param>
        /// <returns></returns>
        BasicOrgInfoModel GetOrganizationByOeBranch(string oeCode, string branch);

        /// <summary>
        /// Gets the organization by external Org ID.
        /// </summary>
        /// <param name="externalOrgId">The external Org ID.</param>
        /// <returns></returns>
        BasicOrgInfoModel GetOrganizationByExternalOrgID(string externalOrgId);

        /// <summary>
        /// Gets the active directory key from context.
        /// </summary>
        /// <returns></returns>
        string GetActiveDirectoryKeyFromContext();

        /// <summary>
        /// Updates the login timestamp for the user after a successful login
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        bool UpdateLastLoginTimestamp(int userId);

        //TODO change this to return a VLCMemberProfile
        VLCMemberProfileModel GetVlcProfile(int userId);

        //TODO change this to accept VLCMemberProfile model
        bool UpdateVlcProfile(VLCMemberProfileModel profile);

        /// <summary>
        /// Updates Member Organization/s affiliation
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="memberOrgAffiliations"></param>
        /// <returns></returns>
        bool UpdateMemberOrgAffliiation(string memberId, IList<MemberOrganizationModel> memberOrgAffiliations);

        /// <summary>
        /// Updates member profile responses.
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="profileResponses"></param>
        /// <returns></returns>
        bool UpdateMemberProfileResponses(string memberId, IList<MemberProfileQAModel> profileResponses);

        /// <summary>
        /// Gets Organizations the Member is affiliated with
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IList<MemberOrganizationModel> GetMemberOrganizations(int userId);

        /// <summary>
        /// Will get all the the products the member's organizations subscribe to
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns>a list of type OrganizationProductModel contain </returns>
        List<OrganizationProductModel> GetMemberOrganizationProducts(int memberId);

        /// <summary>
        /// Gets the member enrolled salt courses
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns>IList<MemberCourseModel></returns>
        IList<MemberCourseModel> GetMemberCourses(int memberId, bool fromMoodle = false);

        /// <summary>
        /// Get a List of Salt Courses offered by Moodle from the IDP web.config
        /// </summary>
        /// <returns>List of MemberCourseModel</returns>
        List<MemberCourseModel> GetSaltCoursesFromWebConfig();

        /// <summary>
        /// Syncs the completed courses from moodle to salt
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool SyncCoursesCompletion(int userId);

        /// <summary>
        /// Gets the member products.
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns>IList<MemberProductModel></returns>
        IList<MemberProductModel> GetMemberProducts(int memberId);

        /// <summary>
        /// Updates member product
        /// </summary>
        /// <param name="memberProduct"></param>
        /// <returns>bool</returns>
        bool UpdateMemberProduct(MemberProductModel memberProduct);

        /// <summary>
        /// Adds member product
        /// </summary>
        /// <param name="memberProduct"></param>
        /// <returns>bool</returns>
        bool AddMemberProduct(MemberProductModel memberProduct);

        /// <summary>
        /// Build out the Endeca Boost for a member
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns>boost string</returns>
        string BuildOutEndecaBoost(int memberId);

        /// <summary>
        /// Inserts or Updates user Scholarship question/s responses 
        /// </summary>
        /// <param name="MemberID">ID</param>
        /// <param name="responses">IList<MemberQA></param>
        /// <returns>bool</returns>
        bool UpsertQuestionAnswer(string MemberID, IList<MemberQAModel> choicesList);

        /// <summary>
        /// Gets Member Scholarship Question Answer
        /// </summary>
        /// <param name="MemberID">Nullable<Int32></param>
        /// <param name="EmailAddress">string</param>
        /// <param name="SourceID">int</param>
        /// <returns>IList<vMemberQuestionAnswer></returns>
        IList<vMemberQuestionAnswerModel> GetMemberQuestionAnswer(Nullable<Int32> MemberID, string EmailAddress, int SourceID);
        /// <summary>
        /// Gets Member Scholarship Question Answer overload
        /// </summary>
        /// <param name="MemberID">required<int></param>
        /// <param name="SourceID">int</param>
        /// <returns>IList<vMemberQuestionAnswer></returns>
        IList<vMemberQuestionAnswerModel> GetMemberQuestionAnswer(int MemberID, int SourceID);

        IList<MemberToDoModel> GetMemberToDos(int memberID);

        bool UpsertMemberToDo(MemberToDoModel todo);

        Dictionary<string, MemberToDoModel> GetToDoDictionary(int memberId);

        IList<ProfileQAModel> GetAllProfileQAs(int memberId);
        List<ProfileResponseModel> GetProfileResponses(int memberId);
    }
}
