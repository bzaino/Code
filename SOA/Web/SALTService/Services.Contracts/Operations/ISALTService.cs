using System;
using System.IO;
using Asa.Salt.Web.Common.Types.Enums;
using Asa.Salt.Web.Services.Contracts.Data;
using System.Collections.Generic;
using System.ServiceModel;
using Asa.Salt.Web.Services.Contracts.Data.Lessons;
using Asa.Salt.Web.Services.Contracts.Data.Lessons.Lesson1;
using Asa.Salt.Web.Services.Contracts.Data.Lessons.Lesson2;
using Asa.Salt.Web.Services.Contracts.Data.Lessons.Lesson3;

namespace Asa.Salt.Web.Services.Contracts.Operations
{
    [ServiceContract(Namespace = "Asa.Salt.Web.Services.Contracts.Operations")]
    public interface ISaltService
    {
        /// <summary>
        /// Adds a content interaction
        /// </summary>
        /// <param name="interaction">The interaction to insert</param>
        /// <returns>MemberContentInteraction</returns>
        [OperationContract(Name = "AddInteraction")]
        MemberContentInteractionContract AddInteraction(MemberContentInteractionContract interaction);

        /// <summary>
        /// Updates a single content interaction and returns the updated model.
        /// </summary>
        /// <param name="userId">The member id.</param>
        /// <param name="interactionToUpdate">The interaction to update</param>
        /// <returns>MemberContentInteraction</returns>
        [OperationContract(Name = "UpdateInteraction")]
        MemberContentInteractionContract UpdateInteraction(int userId, MemberContentInteractionContract interactionToUpdate);

        /// <summary>
        /// Deletes a content interaction
        /// </summary>
        /// <param name="userId">The userId of the member</param>
        /// <param name="interactionId">The MemberContentInteractionID of the interaction to delete</param>
        /// <returns>bool</returns>
        [OperationContract(Name = "DeleteInteraction")]
        bool DeleteInteraction(int userId, int interactionId);

        /// <summary>
        /// Gets users content interaction by contentId and contentType
        /// </summary>
        /// <param name="userId">The userId of the member</param>
        /// <param name="contentId">The id of the content to get</param>
        /// <param name="contentType">0=All;1=Rate;2=Save</param>
        /// <returns>List{MemberContentInteraction}</returns>
        [OperationContract(Name = "GetUserInteractions")]
        List<MemberContentInteractionContract> GetUserInteractions(int userId, string contentId, Int32 contentType);

        /// <summary>
        /// Gets users content interactions by content type.
        /// </summary>
        /// <param name="userId">The userId of the member</param>
        /// <param name="contentType">0=All;1=Rate;2=Save</param>
        /// <returns>List{MemberContentInteraction}</returns>
        [OperationContract(Name = "GetUserInteractionsByContentType")]
        List<MemberContentInteractionContract> GetUserInteractions(int userId, Int32 contentType);

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetUserByUserId")]
        MemberContract GetUser(int userId);

        /// <summary>
        /// Gets the user by username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetUserByUsername")]
        MemberContract GetUser(string username);

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="activeDirectoryKey">The active directory key.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetUserByActiveDirectoryKey")]
        MemberContract GetUser(Guid activeDirectoryKey);

        /// <summary>
        /// Gets the user organizations
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [OperationContract(Name = "GetUserOrganizations")]
        List<MemberOrganizationContract> GetUserOrganizations(int userId);

        /// <summary>
        /// Registers the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        [OperationContract()]
        RegisterMemberResultContract RegisterUser(UserRegistrationContract user);

        /// <summary>
        /// Deactivates the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="modifierUserName">The modifier UserName.</param>
        /// <returns></returns>
        [OperationContract()]
        bool DeactivateUser(int userId, string modifierUserName = null);

        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>bool</returns>
        [OperationContract(Name = "DeleteUser")]
        bool DeleteUser(int userId);

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        [OperationContract()]
        MemberUpdateStatus UpdateUser(MemberContract user);

        /// <summary>
        /// Updates the user's affiliation with organization/s
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="memberOrgAffiliations"></param>
        /// <returns></returns>
        [OperationContract(Name = "UpdateUserOrgAffiliation")]
        bool UpdateUserOrgAffiliation(int userId, IList<MemberOrganizationContract> userOrgAffiliations);

        /// <summary>
        /// Gets the member's alerts.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        [OperationContract()]
        List<MemberAlertContract> GetUserAlerts(int userId);

        /// <summary>
        /// Inserts or Updates user profile question/s responses by deleting previous responses, 
        /// if any, and inserting the new responses.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="responses"></param>
        /// <returns></returns>
        [OperationContract(Name = "UpdateUserProfileResponses")]
        bool UpdateUserProfileResponses(int userId, IList<MemberProfileQAContract> responses);

        /// <summary>
        /// Gets the user's activity history.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [OperationContract(Name = "GetUserActivityHistory")]
        List<MemberActivityHistoryContract> GetUserActivityHistory(int userId);

        /// <summary>
        /// Gets all organizations.
        /// </summary>
        /// <returns></returns>
        [OperationContract(Name = "GetAllOrgs")]
        IEnumerable<RefOrganizationContract> GetAllOrgs();

        /// <summary>
        /// Gets a paginated list of organizations.
        /// </summary>
        /// <param name="filter">The partial name to filter the organization by</param>
        /// <param name="orgTypeNames">The organization type names list to filter on</param>
        /// <param name="rowsPerPage">The number of rows to return</param>
        /// <param name="rowOffset">The number of rows from the beginng of the list to skip to implement paging</param>
        /// <returns></returns>
        [OperationContract()]
        OrgPagedListContract GetOrgs(string filter, string[] orgTypeNames, int rowsPerPage, int rowOffset);

        /// <summary>
        /// Gets the organization.
        /// </summary>
        /// <param name="opeCode">The ope code.</param>
        /// <param name="branchCode">The branch code.</param>
        /// <returns></returns>
        [OperationContract()]
        RefOrganizationContract GetOrg(string opeCode, string branchCode);

        /// <summary>
        /// Gets the organization by external org ID.
        /// </summary>
        /// <param name="externalOrgId">The external org ID.</param>
        /// <returns></returns>
        [OperationContract()]
        RefOrganizationContract GetOrgByExternalOrgID(string externalOrgId);

        /// <summary>
        /// Gets the organization by RefOrganizationID.
        /// </summary>
        /// <param name="organizationId">The RefOrganizationID.</param>
        /// <returns></returns>
        [OperationContract()]
        RefOrganizationContract GetOrgByOrganizationId(int organizationId);
        /// <summary>
        /// Updates Organization bool attribute LookupAllowed based on Organization ID
        /// </summary>
        /// <param name="iOrgID">Organization ID </param>
        /// <param name="bIsLookupAllowed">Allowed look up bool flag </param>
        /// <param name="sModifiedBy">user id</param>
        /// <returns>bool</returns>
        /// [OperationContract(Name = "UpdateOrgInfoFlags")]
        //TODO:   bool UpdateRefOrg(Nullable<int> iOrgID, Nullable<bool> bIsLookupAllowed, string sModifiedBy);

        /// <summary>
        /// update Organization Product Subscription table
        /// </summary>
        /// <param name="iOrgID">Organization ID</param>
        /// <param name="dtProductList">System.Data.DataTable</param>
        /// <param name="sModifiedBy">User ID</param>
        /// <returns></returns>
        /// [OperationContract(Name = "UpdateOrgProductSubscription")]
        //TODO:   bool UpdateOrgProductSubscription(Nullable<int> iOrgID, System.Data.DataTable dtProductList, string sModifiedBy);

        /// <summary>
        /// Gets Organization products
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        [OperationContract(Name = "GetOrgProducts")]
        List<RefOrganizationProductContract> GetOrgProducts(int orgId);

        /// <summary>
        /// Gets Organization products for a given Member ID
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List<RefOrganizationProductContract></returns>
        [OperationContract(Name = "GetOrgProductsByMemberId")]
        List<RefOrganizationProductContract> GetOrgProductsByMemberId(int userId);

        /// <summary>
        /// Gets all products.
        /// </summary>
        /// <returns></returns>
        [OperationContract(Name = "GetAllProducts")]
        IEnumerable<RefProductContract> GetAllProducts();

        /// <summary>
        /// Gets profile questions
        /// </summary>
        /// <returns></returns>
        [OperationContract(Name = "GetAllProfileQuestions")]
        IEnumerable<RefProfileQuestionContract> GetAllProfileQuestions();

        /// <summary>
        /// Gets profile answers
        /// </summary>
        /// <returns></returns>
        [OperationContract(Name = "GetAllProfileAnswers")]
        IEnumerable<RefProfileAnswerContract> GetAllProfileAnswers();

        /// <summary>
        /// Gets profile Responses
        /// </summary>
        /// <returns></returns>
        [OperationContract(Name = "GetProfileResponses")]
        List<MemberProfileQAContract> GetProfileResponses(int userId);


		/// <summary>
		/// Gets all the RefRegistrationSource records in the RefRegistrationSource table
		/// </summary>
        /// <returns>List of RefRegistrationSourceContract</returns>
		[OperationContract(Name = "GetAllRefRegistrationSources")]
        IList<RefRegistrationSourceContract> GetAllRefRegistrationSources();

		/// <summary>
		/// Gets registration source types
		/// </summary>
		/// <returns></returns>
		[OperationContract(Name = "GetAllRefRegistrationSourceTypes")]
		IList<RefRegistrationSourceTypeContract> GetAllRefRegistrationSourceTypes();

        /// <summary>
        /// Gets all RefUserRoles
        /// </summary>
        /// <returns></returns>
        [OperationContract(Name = "GetAllRefUserRoles")]
        IList<RefMemberRoleContract> GetAllRefUserRoles();
	
        /// <summary>
        /// Adds a row to the RefRegistrationSource table
        /// </summary>
        /// <param name="refRegistrationSourceContract">new RefRegistrationSource field values</param>
        /// <returns>true/false</returns>
        [OperationContract(Name = "AddRefRegistrationSource")]
        bool AddRefRegistrationSource(RefRegistrationSourceContract refRegistrationSourceContract);

        /// <summary>
        /// Updates a row to the RefRegistrationSource table
        /// </summary>
        /// <param name="refRegistrationSourceContract">updated RefRegistrationSource field values</param>
        /// <returns>List of RefRegistrationSourceContract</returns>
        [OperationContract(Name = "UpdateRefRegistrationSource")]
        bool UpdateRefRegistrationSource(RefRegistrationSourceContract refRegistrationSourceContract);

        /// <summary>
        /// Gets RefCampaign rows from database
        /// </summary>
        /// <returns></returns>
        [OperationContract(Name = "GetAllRefCampaigns")]
        IList<RefCampaignContract> GetAllRefCampaigns();

        /// <summary>
        /// Adds a row to the RefCampaign table
        /// </summary>
        /// <param name="refCampaignContract">new RefCampaign field values</param>
        /// <returns>true/false</returns>
        [OperationContract(Name = "AddRefCampaign")]
        bool AddRefCampaign(RefCampaignContract refCampaignContract);

        /// <summary>
        /// Updates a row to the RefCampaign table
        /// </summary>
        /// <param name="refCampaignContract">updated RefCampaign field values</param>
        /// <returns>List of RefCampaignContract</returns>
        [OperationContract(Name = "UpdateRefCampaign")]
        bool UpdateRefCampaign(RefCampaignContract refCampaignContract);

        /// <summary>
        /// Gets RefChannel rows from database
        /// </summary>
        /// <returns></returns>
        [OperationContract(Name = "GetAllRefChannels")]
        IList<RefChannelContract> GetAllRefChannels();

        /// <summary>
        /// Deletes the alert.
        /// </summary>
        /// <param name="alertId">The alert id.</param>
        /// <returns></returns>
        [OperationContract()]
        bool DeleteAlert(int alertId);

        /// <summary>
        /// Deletes the loan.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="loanId">The loan id.</param>
        /// <returns>bool</returns>
        [OperationContract()]
        bool DeleteLoan(int userId, int loanId);

        /// <summary>
        /// Updates the loan.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="loan">The loan.</param>
        /// <returns>MemberReportedLoan</returns>
        [OperationContract()]
        MemberReportedLoanContract UpdateLoan(int userId, MemberReportedLoanContract loanContract);

        /// <summary>
        /// Deletes the user lesson question response(s).
        /// </summary>
        /// <param name="lessonUserId">The lesson user id.</param>
        /// <param name="lessonId">The lesson id.</param>
        /// <param name="questionId">The question id.</param>
        /// <param name="questionResponseId">The question response id.</param>
        /// <param name="groupNumber">The group number.</param>
        /// <returns></returns>
        [OperationContract()]
        bool DeleteUserLessonQuestionResponses(int lessonUserId, int lessonId, int? questionId, int? questionResponseId, int? groupNumber);

        /// <summary>
        /// Gets the payment reminders.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        [OperationContract()]
        IList<PaymentReminderContract> GetUserPaymentReminders(int userId);

        /// <summary>
        /// Saves the payment reminders.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="reminders">The reminders.</param>
        /// <returns></returns>
        [OperationContract()]
        RemindersUpdateStatus SaveUserPaymentReminders(int userId, IList<PaymentReminderContract> reminders);

        /// <summary>
        /// Imports the loan file.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="file">The file.</param>
        /// <param name="sourceName">The source name e.g. 'KWYO'</param>
        /// <returns></returns>
        [OperationContract()]
        IList<MemberReportedLoanContract> ImportUserLoans(int userId, byte[] file, string sourceName);
        
        /// <summary>
        /// Imports the loan file.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        [OperationContract()]
        IList<MemberReportedLoanContract> GetUserLoans(int userId);

        /// <summary>
        ///  Gets the user loans by user ID and array of record source names.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="recordSourceNames">array of record source names.</param>
        /// <returns></returns>
        [OperationContract()]
        IList<MemberReportedLoanContract> GetUserLoansRecordSourceList(int userId, string[] recordSourceNames);

        /// <summary>
        /// Gets the member reported loans.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        [OperationContract()]
        IList<vMemberReportedLoansContract> GetReportedLoans(int userId);

        /// <summary>
        /// Gets the survey.
        /// </summary>
        /// <param name="surveyId">The survey id.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetSurveyById")]
        SurveyContract GetSurvey(int surveyId);

        /// <summary>
        /// Gets the survey results.
        /// </summary>
        /// <param name="surveyId">The survey id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        [OperationContract()]
        SurveyResponseContract GetUserSurveyResults(int surveyId, int userId);

        /// <summary>
        /// Posts the survey.
        /// </summary>
        /// <param name="surveyResponse">The survey response.</param>
        /// <returns></returns>
        [OperationContract()]
        bool PostSurvey(SurveyResponseContract surveyResponse);

        /// <summary>
        /// Gets lessons reference data.
        /// </summary>
        /// <param name="refDataGroup">The ref data group id.</param>
        /// <returns></returns>
        [OperationContract()]
        List<RefLessonLookupDataContract> GetLessonsReferenceData(RefLessonLookupDataTypes refDataGroup);

        /// <summary>
        /// Gets the member lessons.
        /// </summary>
        /// <param name="lessonUserId">The lesson user id.</param>
        /// <returns></returns>
        [OperationContract()]
        IList<MemberLessonContract> GetUserLessons(int lessonUserId);

        /// <summary>
        /// Creates the user lesson.
        /// </summary>
        /// <param name="userLesson">The user lesson.</param>
        /// <returns></returns>
        [OperationContract()]
        MemberLessonContract StartUserLesson(MemberLessonContract userLesson);

        /// <summary>
        /// Posts the users lesson1 data.
        /// </summary>
        /// <param name="lesson">The lesson.</param>
        /// <returns></returns>
        [OperationContract()]
        PostLessonResultContract<Lesson1Contract> PostLesson1(Lesson1Contract lesson);

        /// <summary>
        /// Posts the users lesson2 data.
        /// </summary>
        /// <param name="lesson">The lesson.</param>
        /// <returns></returns>
        [OperationContract()]
        PostLessonResultContract<Lesson2Contract> PostLesson2(Lesson2Contract lesson);

        /// <summary>
        /// Posts the users lesson3 data.
        /// </summary>
        /// <param name="lesson">The lesson.</param>
        /// <returns></returns>
        [OperationContract()]
        PostLessonResultContract<Lesson3Contract> PostLesson3(Lesson3Contract lesson);

        /// <summary>
        /// Gets the user's lesson1 results.
        /// </summary>
        /// <param name="lessonUserId">The lesson user id.</param>
        /// <param name="lessonStepId">The lesson step id.</param>
        /// <returns></returns>
        [OperationContract()]
        Lesson1Contract GetUserLesson1Results(int lessonUserId, int? lessonStepId = null);

        /// <summary>
        /// Gets the user's lesson2 results.
        /// </summary>
        /// <param name="lessonUserId">The lesson user id.</param>
        /// <param name="lessonStepId">The lesson step id.</param>
        /// <returns></returns>
        [OperationContract()]
        Lesson2Contract GetUserLesson2Results(int lessonUserId, int? lessonStepId = null);

        /// <summary>
        /// Gets the user's lesson3 results.
        /// </summary>
        /// <param name="lessonUserId">The lesson user id.</param>
        /// <param name="lessonStepId">The lesson step id.</param>
        /// <returns></returns>
        [OperationContract()]
        Lesson3Contract GetUserLesson3Results(int lessonUserId, int? lessonStepId = null);

        /// <summary>
        /// Associates the lessons with user.
        /// </summary>
        /// <param name="lessonUserId">The lesson user id.</param>
        /// <param name="userId">The user id.</param>
        [OperationContract()]
        bool AssociateLessonsWithUser(int lessonUserId, int userId);

        /// <summary>
        /// Creates a single Loan.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="loan">The loan to insert.</param>
        /// <returns>MemberReportedLoan</returns>
        [OperationContract()]
        MemberReportedLoanContract CreateLoan(int userId, MemberReportedLoanContract loan);

        /// <summary>
        /// Updates the user lesson.
        /// </summary>
        /// <param name="userLesson">The user lesson.</param>
        /// <returns></returns>
        [OperationContract()]
        bool UpdateUserLesson(MemberLessonContract userLesson);

        /// <summary>
        /// Gets the user's VLC profile.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns></returns>
        [OperationContract()]
        VLCUserProfileContract GetVlcMemberProfile(int userId);

        /// <summary>
        /// Updates user's VLC profile.
        /// </summary>
        /// <param name="profile">The VLC Member Profile to update.</param>
        /// <returns></returns>
        [OperationContract()]
        bool UpdateVlcMemberProfile(VLCUserProfileContract profile);

        /// <summary>
        /// Add a Vlc Response.
        /// </summary>
        /// <param name="response">The VLC question response contract.</param>
        /// <returns></returns>
        [OperationContract()]
        bool AddVlcResponse(VLCUserResponseContract response);

        /// <summary>
        /// Posts the JellyVision Quiz Response.
        /// </summary>
        /// <param name="response">The JellyVision Quiz Response.</param>
        /// <returns>true/false</returns>
        [OperationContract()]
        bool AddJellyVisionQuizResponse(JellyVisionQuizResponseContract response);

        /// <summary>
        /// Gets the COL States list.
        /// </summary>
        /// <param name=""></param>
        /// <returns>List of vCostOfLivingStateListContract</returns>
        [OperationContract(Name = "GetCOLStates")]
        List<vCostOfLivingStateListContract> GetCOLStates();

        /// <summary>
        /// Gets the COL Urban Area list.
        /// </summary>
        /// <param name="stateId">The RefStateID</param>
        /// <returns>List of RefGeographicIndexContract</returns>
        [OperationContract(Name = "GetCOLUrbanAreas")]
        List<RefGeographicIndexContract> GetCOLUrbanAreas(int stateId);
        
        /// <summary>
        /// Gets the CostOfLiving results.
        /// </summary>
        /// <param name="cityA">The RefGeographicIndexID for CityA.</param>
        /// <param name="cityB">The RefGeographicIndexID for CityB.</param>
        /// <param name="salary">salary</param>
        /// <returns>COLResultsContract</returns>
        [OperationContract(Name = "GetCOLResults")]
        COLResultsContract GetCOLResults(int cityA, int cityB, decimal salary);

        /// <summary>
        /// Gets the JSI Majors list.
        /// </summary>
        /// <param name=""></param>
        /// <returns>JSIQuestionsContract</returns>
        [OperationContract(Name = "GetRefMajors")]
        JSIQuestionsContract GetRefMajors();

        /// <summary>
        /// Gets the JSI States list.
        /// </summary>
        /// <param name=""></param>
        /// <returns>JSIQuestionsContract</returns>
        [OperationContract(Name = "GetRefStates")]
        JSIQuestionsContract GetRefStates();

        /// <summary>
        /// Gets the JSI School by MajorId and StateId list.
        /// </summary>
        /// <param name="majorId">JSI RefMajorId</param>
        /// <param name="stateId">JSI RefStateId</param>
        /// <returns>JSIQuestionsContract</returns>
        [OperationContract(Name = "GetJSISchools")]
        JSIQuestionsContract GetJSISchools(int majorId, int stateId);

        /// <summary>
        /// Posts the JSI Quiz Response.
        /// </summary>
        /// <param name="response">The JSI Quiz Responses.</param>
        /// <returns>JSIQuizResultsContract</returns>
        [OperationContract(Name = "PostJSIResponse")]
        JSIQuestionsContract PostJSIResponse(JSIQuizAnswerContract response);

        /// <summary>
        /// Gets members academic info view, a total records of maximumRows, starting at startRowIndex.
        /// </summary>
        /// <param name="startRowIndex"></param>
        /// <param name="maximumRows"></param>
        /// <returns>IEnumerable</returns>
        [OperationContract(Name = "GetMembersAcademicInfoView")]
        Tuple<IEnumerable<vMemberAcademicInfoContract>, int> GetMembersAcademicInfoView(int startRowIndex, int maximumRows);

        /// <summary>
        /// Gets Organizations the Member is affiliated with
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [OperationContract(Name = "GetMemberOrganizations")]
        List<MemberOrganizationContract> GetMemberOrganizations(int userId);

        /// <summary>
        /// Gets Memebr products
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [OperationContract(Name = "GetMemberProducts")]
        List<MemberProductContract> GetMemberProducts(int userId);

        /// <summary>
        /// Add Member product
        /// </summary>
        /// <param name="memberProduct"></param>
        /// <returns></returns>
        [OperationContract(Name = "AddMemberProduct")]
        bool AddMemberProduct(MemberProductContract memberProduct);

        /// <summary>
        /// Update Member product
        /// </summary>
        /// <param name="memberProduct"></param>
        /// <returns></returns>
        [OperationContract(Name = "UpdateMemberProduct")]
        bool UpdateMemberProduct(MemberProductContract memberProduct);

        /// <summary>
        /// Gets Members for SALTShaker search and navigation
        /// </summary>
        /// <param name="MemberID"></param>
        /// <param name="FirstName"></param>
        /// <param name="LastName"></param>
        /// <param name="EmailAddress"></param>
        /// <param name="OrganizationName"></param>
        /// <returns>IEnumerable<vMemberAcademicInfoContract></returns>
        [OperationContract(Name = "GetMembersBySearchParms")]
        IEnumerable<vMemberAcademicInfoContract> GetMembersBySearchParms(Nullable<Int32> MemberID, String FirstName, String LastName, String EmailAddress);
    
            /// <summary>
        /// Gets MemberActivationHistory 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [OperationContract(Name = "GetUserActivationHistory")]
        List<MemberActivationHistoryContract> GetUserActivationHistory(int userId);

        /// <summary>
        /// Updates Organization bool attribute LookupAllowed based on Organization ID
        /// </summary>
        /// <param name="iOrgID">Organization ID </param>
        /// <param name="bIsLookupAllowed">Allowed look up bool flag </param>
        /// <param name="sModifiedBy">user id</param>
        /// <returns>bool</returns>
        [OperationContract(Name = "UpdateOrgInfoFlags")]
        bool UpdateRefOrg(int iOrgID, Nullable<bool> bIsLookupAllowed, string sModifiedBy);

        /// <summary>
        /// insert/update an OrganizationToDoList item
        /// </summary>
        /// <param name="toDoListItem"></param>
        /// <returns>bool</returns>
        [OperationContract(Name = "UpsertOrgToDoListItem")]
        bool UpsertOrgToDoListItem(OrganizationToDoListContract toDoListItem);

        /// <summary>
        /// update Organization Product Subscription table
        /// </summary>
        /// <param name="iOrgID">Organization ID</param>
        /// <param name="dtProductList">System.Data.DataTable</param>
        /// <param name="sModifiedBy">User ID</param>
        /// <returns></returns>
        [OperationContract(Name = "UpdateOrgProductsSubscription")]
        bool UpdateOrgProductsSubscription(Nullable<int> iOrgID, System.Data.DataTable dtProductList, string sModifiedBy);
        
        /// <summary>
        /// Inserts or Updates user Scholarship question/s responses 
        /// </summary>
        /// <param name="MemberID">ID</param>
        /// <param name="responses">IList<MemberQA></param>
        /// <returns>bool</returns>
        [OperationContract(Name = "UpsertQuestionAnswer")]
        bool UpsertQuestionAnswer(int MemberID, IList<MemberQAContract> Responses);

        /// <summary>
        /// Gets Member Scholarship Question Answer
        /// </summary>
        /// <param name="MemberID">Nullable<Int32></param>
        /// <param name="EmailAddress">string</param>
        /// <param name="SourceID">int</param>
        /// <returns>IEnumerable<vMemberQuestionAnswer></returns>
        [OperationContract(Name = "GetMemberQuestionAnswer")]
        IEnumerable<vMemberQuestionAnswerContract> GetMemberQuestionAnswer(Nullable<Int32> MemberID, String EmailAddress, int SourceID);

        /// <summary>
        /// Retrieves all todos for a given member
        /// </summary>
        /// <param name="memberID">ID</param>
        /// <returns>IEnumerable<MemberToDoListContract></returns>
        [OperationContract(Name = "GetMemberToDos")]
        IEnumerable<MemberToDoListContract> GetMemberToDos(int memberID);

        /// <summary>
        /// Insert/update a todo
        /// </summary>
        /// <param name="todoContract">Todo</param>
        /// <returns>bool</returns>
        [OperationContract(Name = "UpsertMemberToDo")]
        bool UpsertMemberToDo(MemberToDoListContract todoContract);

        /// <summary>
        /// Retrieves all Member Roles for a given member
        /// </summary>
        /// <param name="memberID">MemberID</param>
        /// <returns>List of MemberRole</returns>
        [OperationContract(Name = "GetMemberRoles")]
        IEnumerable<MemberRoleContract> GetMemberRoles(int memberID);

        /// <summary>
        /// Inserts/Updates MemberRoles
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="memberRoles"></param>
        /// <param name="modifiedBy"></param>
        /// <returns></returns>
        [OperationContract(Name = "UpsertMemberRoles")]
        bool UpsertMemberRoles(int memberId, IList<MemberRoleContract> memberRoles, string modifiedBy);
    }
}
