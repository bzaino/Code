using System;
using System.Collections.Generic;

using Asa.Salt.Web.Common.Types.Enums;
using ASA.Web.Services.ASAMemberService.DataContracts;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.ASAMemberService
{
    public class ASAMemberAdapterStub : ASA.Web.Services.ASAMemberService.ServiceContracts.IAsaMemberAdapter
    {
        /// <summary>
        /// Gets the member.
        /// </summary>
        /// <param name="memberId">The member id.</param>
        /// <returns></returns>
        public ASAMemberModel GetMember(int memberId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the member by email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public ASAMemberModel GetMemberByEmail(string email)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the member.
        /// </summary>
        /// <param name="systemId">The system id.</param>
        /// <returns></returns>
        public ASAMemberModel GetMember(Guid systemId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates session cookies and gets the member.
        /// </summary>
        /// <param name="logon">create session cookies</param>
        /// <param name="emailAddress">member's email address.</param>
        /// <returns>ASAMemberModel</returns>
        public ASAMemberModel Logon(bool logon, string emailAddress)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates the specified member.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public RegistrationResultModel Create(ASAMemberModel member)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates the specified member.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public MemberUpdateStatus Update(ASAMemberModel member)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete the specified member from SALT db and ActiveDirectory.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>bool</returns>
        public bool DeleteMember(ASAMemberModel member)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deactivates the account.
        /// </summary>
        /// <param name="individualId">The individual id.</param>
        /// <returns></returns>
        public ResultCodeModel DeActivateAccount(string individualId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the member id from context.
        /// </summary>
        /// <returns></returns>
        public int GetMemberIdFromContext()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets memberId cookie
        /// </summary>
        public void SetMemberIdCookie(string memberId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks if the memberId passed in matches the one in context
        /// </summary>
        /// <returns>bool</returns>
        public static bool IsCurrentUserResponse { get; set; }
        public bool IsCurrentUser(string memberId)
        {
            return IsCurrentUserResponse;
        }

        /// <summary>
        /// Gets the organization by oe branch.
        /// </summary>
        /// <param name="oeCode">The oe code.</param>
        /// <param name="branch">The branch.</param>
        /// <returns></returns>
        public BasicOrgInfoModel GetOrganizationByOeBranch(string oeCode, string branch)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the organization by external org ID.
        /// </summary>
        /// <param name="externalOrgId">The external org ID.</param>
        /// <returns></returns>
        public BasicOrgInfoModel GetOrganizationByExternalOrgID(string externalOrgId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the active directory key from context.
        /// </summary>
        /// <returns></returns>
        public string GetActiveDirectoryKeyFromContext()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates the login timestamp for the user after a successful login
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public bool UpdateLastLoginTimestamp(int userId)
        {
            throw new NotImplementedException();
        }

        //TODO change this to return a VLCMemberProfile
        public VLCMemberProfileModel GetVlcProfile(int userId)
        {
            throw new NotImplementedException();
        }

        //TODO change this to accept VLCMemberProfile model
        public bool UpdateVlcProfile(VLCMemberProfileModel profile)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates Member Organization/s affiliation
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="memberOrgAffiliations"></param>
        /// <returns></returns>
        public bool UpdateMemberOrgAffliiation(string memberId, IList<MemberOrganizationModel> memberOrgAffiliations)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates member profile responses.
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="profileResponses"></param>
        /// <returns></returns>
        public bool UpdateMemberProfileResponses(string memberId, IList<MemberProfileQAModel> profileResponses)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets Organizations the Member is affiliated with
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public IList<MemberOrganizationModel> GetMemberOrganizations(int memberId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Will get all the the products the member's organizations subscribe to
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns>a list of type OrganizationProductModel contain </returns>
        public List<OrganizationProductModel> GetMemberOrganizationProducts(int memberId)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Gets the member enrolled salt courses
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns>IList<MemberCourseModel></returns>
        public IList<MemberCourseModel> GetMemberCourses(int memberId, bool fromMoodle = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get a List of Salt Courses offered by Moodle from the IDP web.config
        /// </summary>
        /// <returns>List of MemberCourseModel</returns>
        public List<MemberCourseModel> GetSaltCoursesFromWebConfig()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Syncs the completed courses from moodle to salt
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool SyncCoursesCompletion(int userId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the member products.
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns>IList<MemberProductModel></returns>
        public IList<MemberProductModel> GetMemberProducts(int memberId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates member product
        /// </summary>
        /// <param name="memberProduct"></param>
        /// <returns>bool</returns>
        public bool UpdateMemberProduct(MemberProductModel memberProduct)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds member product
        /// </summary>
        /// <param name="memberProduct"></param>
        /// <returns>bool</returns>
        public bool AddMemberProduct(MemberProductModel memberProduct)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Build out the Endeca Boost for a member
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns>boost string</returns>
        public string BuildOutEndecaBoost(int memberId)
        {
            throw new NotImplementedException();
        }

    /// <summary>
        /// Inserts or Updates user question/s responses 
        /// </summary>
        /// <param name="MemberID">ID</param>
        /// <param name="responses">IList<MemberQA></param>
        /// <returns>bool</returns>
        public bool UpsertQuestionAnswer(string MemberID, IList<MemberQAModel> choicesList)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Gets Member Scholarship Question Answer
        /// </summary>
        /// <param name="MemberID">Nullable<Int32></param>
        /// <param name="EmailAddress">string</param>
        /// <param name="SourceID">int</param>
        /// <returns>IList<vMemberQuestionAnswer></returns>
        public IList<vMemberQuestionAnswerModel> GetMemberQuestionAnswer(Nullable<Int32> MemberID, string EmailAddress, int SourceID)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Gets Member Scholarship Question Answer overload
        /// </summary>
        /// <param name="MemberID">required<int></param>
        /// <param name="SourceID">int</param>
        /// <returns>IList<vMemberQuestionAnswer></returns>
        public IList<vMemberQuestionAnswerModel> GetMemberQuestionAnswer(int MemberID, int SourceID)
        {
            throw new NotImplementedException();
        }

        public IList<MemberToDoModel> GetMemberToDos(int memberID)
        {
            throw new NotImplementedException();
        }

        public bool UpsertMemberToDo(MemberToDoModel todo)
        {

            throw new NotImplementedException();
        }

        public Dictionary<string, MemberToDoModel> GetToDoDictionary(int memberId)
        {
            throw new NotImplementedException();
        }

        public IList<ProfileQAModel> GetAllProfileQAs(int memberId)
        {
            throw new NotImplementedException();
        }

        public List<ProfileResponseModel> GetProfileResponses(int memberId)
        {
            throw new NotImplementedException();
        }

        public bool IsProductActive(IList<MemberOrganizationModel> memberOrganizations, int productId)
        {
            throw new NotImplementedException();
        }
    }
}
