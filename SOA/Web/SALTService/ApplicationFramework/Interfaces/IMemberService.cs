using System;
using System.Collections.Generic;

using Asa.Salt.Web.Common.Types.Enums;
using Asa.Salt.Web.Services.Domain;

namespace Asa.Salt.Web.Services.BusinessServices.Interfaces
{
    public interface IMemberService
    {
        /// <summary> 
        /// Gets the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        Member GetUser(int userId);

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="activeDirectoryKey">The active directory key.</param>
        /// <returns></returns>
        Member GetUser(Guid activeDirectoryKey);

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        Member GetUser(string username);
        
        /// <summary>
        /// Registers the user.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <returns></returns>
        RegisterMemberResult RegisterUser(UserAccount account);

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        MemberUpdateStatus UpdateUser(Member user);
        
        /// <summary>
        /// Updates the user's affiliation with organization/s
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="memberOrgAffiliations"></param>
        /// <returns></returns>
        bool UpdateUserOrgAffiliation(int userId, IList<MemberOrganization> userOrgAffiliations);

        /// <summary>
        /// Will extract the goals from a members profile answers. Goals are QuestionId's "17", "18", "19", "20"
        /// </summary>
        /// <param name="memberProfileAnswers">List of member ProfileAnswers</param>
        /// <returns>List of strings containing the members goals</returns>
        List<string> GetMemberGoals(List<MemberProfileAnswer> memberProfileAnswers);
        
        /// <summary>
        /// checks if dashboard product is enable on at least one organization
        /// </summary>
        /// <param name="organizations">List of member organizations</param>
        /// <returns>bool</returns>
        bool isDashBoardEnabled(List<MemberOrganization> organizations);

        /// <summary>
        /// Determines which org info will be sent to ExactTarget.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Member Organization</returns>
        MemberOrganization DetermineMemberOrgInfo(Member user);

        /// <summary>
        /// Deactivates the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="modifierUserName">The modifier UserName.</param>
        /// <returns>bool</returns>
        bool DeactivateUser(int userId, string modifierUserName = null);

        /// <summary>
        /// Deletes the user if age is less than the minimum required.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>bool</returns>
        bool DeleteUser(int userId);

        /// <summary>
        /// Gets user profile Questions And Answers.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>MemberProfileQA</returns>
        List<MemberProfileQA> GetUserProfileQuestionsAndAnswers(int userId);


        List<MemberOrganization> GetMemberOrganizations(int userId);

        /// <summary>
        /// Inserts or Updates user profile question/s responses by deleting previous responses, 
        /// if any, and inserting the new responses.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="responses"></param>
        /// <returns>bool</returns>
        bool UpdateUserProfileResponses(int userId, IList<MemberProfileQA> responses);

        /// <summary>
        /// Gets the user activity history.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List<MemberActivityHistory></returns>
        List<MemberActivityHistory> GetUserActivityHistory(int userId);

        /// <summary>
        /// Gets the user VLC Member Profile.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        VLCUserProfile GetVlcMemberProfile(int userId);

        /// <summary>
        /// Updates the VLC member profile.
        /// </summary>
        /// <param name="profile">The user's VLC member profile to update.</param>
        /// <returns></returns>
        bool UpdateVlcMemberProfile(VLCUserProfile profile);

        /// <summary>
        /// Gets the user products.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<MemberProduct> GetMemberProducts(int userId);

        /// <summary>
        /// Adds member product.
        /// </summary>
        /// <param name="memberProduct"></param>
        /// <returns></returns>
        bool AddMemberProduct(MemberProduct memberProduct);

        /// <summary>
        /// Updates the memeber's product participation.
        /// </summary>
        /// <param name="memberProduct"></param>
        /// <returns></returns>
        bool UpdateMemberProduct(MemberProduct memberProduct);

        /// <summary>
        /// Updates the member's affiliation/s to Organization/s.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="memberOrgAffiliations"></param>
        /// <returns></returns>
        //bool UpdateMemberOrg(int userId, List<> memberOrgAffiliations);

        /// <summary>
        /// Gets Members for SALTShaker search and navigation.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        IEnumerable<vMemberAcademicInfo> GetMembersBySearchParms(Nullable<Int32> MemberID, String FirstName, String LastName, String EmailAddress);

        /// <summary>
        /// Gets MembersAcademicInfo View along with records count for SALTShaker.
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        Tuple<IEnumerable<vMemberAcademicInfo>, int> GetMembersAcademicInfoView(int startRowIndex, int maximumRows);

        /// <summary>
        /// Gets Member Activation History.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<MemberActivationHistory> GetUserActivationHistory(int userId);


        /// <summary>
        /// Gets user Scholarship Questions And Answers.
        /// </summary>
        /// <param name="MemberID">Nullable<Int32></param>
        /// <param name="EmailAddress">string</param>
        /// <param name="SourceID">int</param>
        /// <returns>vMemberQuestionAnswer</returns>
        IEnumerable<vMemberQuestionAnswer> GetMemberQuestionAnswer(Nullable<Int32> MemberID, String EmailAddress, int SourceID);

        /// <summary>
        /// Inserts or Updates user Scholarship question/s responses
        /// </summary>
        /// <param name="MemberID">int</param>
        /// <param name="responses">IList<MemberQA></param>
        /// <returns>bool</returns>
        bool UpsertQuestionAnswer(int MemberID, IList<MemberQA> Responses);

        /// <summary>
        /// Retrieves all todos for a given member
        /// </summary>
        /// <param name="memberID">ID</param>
        /// <returns>bool</returns>
        IEnumerable<MemberToDoList> GetMemberToDos(int memberID);

        /// <summary>
        /// Insert/update a todo
        /// </summary>
        /// <param name="todoContract">Todo</param>
        /// <returns>bool</returns>
        bool UpsertMemberToDo(MemberToDoList todo);

        /// <summary>
        /// Retrieves all Member Roles for a given member
        /// </summary>
        /// <param name="memberID">MemberID</param>
        /// <returns>List of MemberRole</returns>
        IEnumerable<MemberRole> GetMemberRoles(int memberID);

        /// <summary>
        /// Inserts/Updates MemberRoles
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="memberRoles"></param>
        /// <param name="modifiedBy"></param>
        /// <returns></returns>
        bool UpsertMemberRoles(int memberId, IList<MemberRole> memberRoles, string modifiedBy = "SVC_ACCT");

        /// <summary>
        /// Gets a list of Org Products given a userId
        /// </summary>
        /// <param name="userId">The user's Membership ID.</param>
        /// <returns>List<RefOrganizationProduct></returns>
        List<RefOrganizationProduct> GetOrgProductsByMemberID(int userId);
    }
}
