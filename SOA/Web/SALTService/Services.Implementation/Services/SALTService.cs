using System;
using System.Collections.Generic;
using Asa.Salt.Web.Common.Types.Enums;
using Asa.Salt.Web.Common.Types.Unity;
using Asa.Salt.Web.Services.Application.Implementation.Mapping.Extensions;
using Asa.Salt.Web.Services.BusinessServices.Interfaces;
using Asa.Salt.Web.Services.Contracts.Data;
using Asa.Salt.Web.Services.Contracts.Data.Lessons;
using Asa.Salt.Web.Services.Contracts.Data.Lessons.Lesson1;
using Asa.Salt.Web.Services.Contracts.Data.Lessons.Lesson2;
using Asa.Salt.Web.Services.Contracts.Data.Lessons.Lesson3;
using Asa.Salt.Web.Services.Contracts.Operations;
using Asa.Salt.Web.Services.Domain;
using Asa.Salt.Web.Services.Domain.Lessons;
using Asa.Salt.Web.Services.Domain.Lessons.Lesson1;
using Asa.Salt.Web.Services.Domain.Lessons.Lesson2;
using Asa.Salt.Web.Services.Domain.Lessons.Lesson3;

namespace Asa.Salt.Web.Services.Application.Implementation.Services
{

    public class SaltService : ISaltService
    {
        /// <summary>
        /// The lessons service
        /// </summary>
        private readonly ILazyResolver<ILessonsService> _lessonsService;
        /// <summary>
        /// The user service
        /// </summary>
        private readonly ILazyResolver<IMemberService> _userService;
        /// <summary>
        /// The lookup service
        /// </summary>
        private readonly ILazyResolver<ILookupService> _lookupService;
        /// <summary>
        /// The alert service
        /// </summary>
        private readonly ILazyResolver<IAlertService> _alertService;

        /// <summary>
        /// The reminder service
        /// </summary>
        private readonly ILazyResolver<IReminderService> _reminderService;

        /// <summary>
        /// The loan service
        /// </summary>
        private readonly ILazyResolver<ILoanService> _loanService;

        /// <summary>
        /// The survey service
        /// </summary>
        private readonly ILazyResolver<ISurveyService> _surveyService;

        /// <summary>
        /// The content service
        /// </summary>
        private readonly ILazyResolver<IContentService> _contentService;
        

        /// <summary>
        /// Initializes a new instance of the <see cref="SaltService"/> class.
        /// </summary>
        public SaltService(ILazyResolver<IAlertService> alertService, ILazyResolver<IMemberService> memberService, ILazyResolver<ILookupService> lookupService, ILazyResolver<IReminderService> reminderService, ILazyResolver<ILoanService> loanService, ILazyResolver<ISurveyService> surveyService, ILazyResolver<ILessonsService> lessonsService, ILazyResolver<IContentService> contentService)
        {
            _alertService = alertService;
            _userService = memberService;
            _lookupService = lookupService;
            _reminderService = reminderService;
            _loanService = loanService;
            _surveyService = surveyService;
            _lessonsService = lessonsService;
            _contentService = contentService;
        }

        /// <summary>
        /// Inserts a new content interaction
        /// </summary>
        /// <param name="interaction">The interaction to insert.</param>
        /// <returns></returns>
        public MemberContentInteractionContract AddInteraction(MemberContentInteractionContract interaction)
        {
            return _contentService.Resolve().AddInteraction(interaction.ToDomainObject<MemberContentInteractionContract, MemberContentInteraction>()).ToDataContract<MemberContentInteraction, MemberContentInteractionContract>(); 
        }

        /// <summary>
        /// Updates a single content interaction and returns the updated model.
        /// </summary>
        /// <param name="userId">The member id.</param>
        /// <param name="interactionToUpdate">The interaction to update</param>
        /// <returns>MemberContentInteraction</returns>
        public MemberContentInteractionContract UpdateInteraction(int userId, MemberContentInteractionContract interactionToUpdate)
        {
            return _contentService.Resolve().UpdateInteraction(userId, interactionToUpdate.ToDomainObject<MemberContentInteractionContract, MemberContentInteraction>()).ToDataContract<MemberContentInteraction, MemberContentInteractionContract>();
        }

        /// <summary>
        /// Deletes a content interaction
        /// </summary>
        /// <param name="userId">The userId of the member</param>
        /// <param name="interactionId">The MemberContentInteractionID of the interaction to delete</param>
        /// <returns></returns>
        public bool DeleteInteraction(int userId, int interactionId)
        {
            return _contentService.Resolve().DeleteInteraction(userId, interactionId);
        }

        /// <summary>
        /// Gets users content interaction by contentId and contentType
        /// </summary>
        /// <param name="userId">The userId of the member</param>
        /// <param name="contentId">The id of the contentId to get</param>
        /// <param name="contentType">0=All;1=Rate;2=Save</param>
        /// <returns>List{MemberContentInteraction}</returns>
        public List<MemberContentInteractionContract> GetUserInteractions(int userId, string contentId, Int32 contentType)
        {
            return _contentService.Resolve().GetUserInteractions(userId, contentId, contentType).ToDataContract<MemberContentInteraction, MemberContentInteractionContract>();
        }

        /// <summary>
        /// Gets users content interactions by content type.
        /// </summary>
        /// <param name="userId">The userId of the member</param>
        /// <param name="contentType">0=All;1=Rate;2=Save</param>
        /// <returns>List{MemberContentInteraction}</returns>
        public List<MemberContentInteractionContract> GetUserInteractions(int userId, Int32 contentType)
        {
            return _contentService.Resolve().GetUserInteractions(userId, contentType).ToDataContract<MemberContentInteraction, MemberContentInteractionContract>();
        }

        /// <summary>
        /// Gets the user by member id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public MemberContract GetUser(int userId)
        {
            return _userService.Resolve().GetUser(userId).ToDataContract<Member, MemberContract>();
        }

        /// <summary>
        /// Gets the user by username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        public MemberContract GetUser(string username)
        {
            return _userService.Resolve().GetUser(username).ToDataContract<Member, MemberContract>();
        }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="activeDirectoryKey">The active directory key.</param>
        /// <returns></returns>
        public MemberContract GetUser(Guid activeDirectoryKey)
        {
            return _userService.Resolve().GetUser(activeDirectoryKey).ToDataContract<Member, MemberContract>();
        }

        /// <summary>
        /// Gets the user organizations
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<MemberOrganizationContract> GetUserOrganizations(int userId)
        {
            return _userService.Resolve().GetMemberOrganizations(userId).ToDataContract<MemberOrganization, MemberOrganizationContract>();
        }

        /// <summary>
        /// Registers the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public RegisterMemberResultContract RegisterUser(UserRegistrationContract user)
        {
            var result = _userService.Resolve().RegisterUser(user.ToDomainObject<UserRegistrationContract, UserAccount>());
            return new RegisterMemberResultContract()
                        {
                            Member = result.Member.ToDataContract<Member, MemberContract>(),
                            CreateStatus = result.CreateStatus
                        };
        }

        /// <summary>
        /// Deactivates the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>bool</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool DeactivateUser(int userId, string modifierUserName = null)
        {
            return _userService.Resolve().DeactivateUser(userId, modifierUserName);
        }

        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>bool</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool DeleteUser(int userId)
        {
            return _userService.Resolve().DeleteUser(userId);
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public MemberUpdateStatus UpdateUser(MemberContract user)
        {
            return _userService.Resolve().UpdateUser(user.ToDomainObject<MemberContract, Member>());
        }

        /// <summary>
        /// Updates the user's affiliation with organization/s
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="memberOrgAffiliations"></param>
        /// <returns></returns>
        public bool UpdateUserOrgAffiliation(int userId, IList<MemberOrganizationContract> userOrgAffiliations)
        {
            return _userService.Resolve().UpdateUserOrgAffiliation(userId, userOrgAffiliations.ToDomainObject<MemberOrganizationContract, MemberOrganization>());
        }

        /// <summary>
        /// Gets the member's alerts.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public List<MemberAlertContract> GetUserAlerts(int userId)
        {
            return _alertService.Resolve().GetUserAlerts(userId).ToDataContract<IList<MemberAlert>, List<MemberAlertContract>>();
        }

        /// <summary>
        /// Inserts or Updates user profile question/s responses by deleting previous responses, 
        /// if any, and inserting the new responses.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="responses"></param>
        /// <returns></returns>
        public bool UpdateUserProfileResponses(int userId, IList<MemberProfileQAContract> responses)
        {
            return _userService.Resolve().UpdateUserProfileResponses(userId, responses.ToDomainObject<IList<MemberProfileQAContract>, List<MemberProfileQA>>());
        }

        public List<MemberActivityHistoryContract> GetUserActivityHistory(int userId)
        {
            return _userService.Resolve().GetUserActivityHistory(userId).ToDataContract<List<MemberActivityHistory>, List<MemberActivityHistoryContract>>();
        }

        /// <summary>
        /// Gets all organizations.
        /// </summary>
        public IEnumerable<RefOrganizationContract> GetAllOrgs()
        {
            return _lookupService.Resolve().GetAllOrgs().ToDataContract<RefOrganization, RefOrganizationContract>();
        }

        /// <summary>
        /// Gets a paginated list of organizations.
        /// </summary>
        /// <param name="filter">The partial name to filter the organization by</param>
        /// <param name="orgTypeNames">The organization type names list to filter on</param>
        /// <param name="rowsPerPage">The number of rows to return</param>
        /// <param name="rowOffset">The number of rows from the beginng of the list to skip to implement paging</param>
        /// <returns></returns>
        public OrgPagedListContract GetOrgs(string filter, string[] orgTypeNames, int rowsPerPage, int rowOffset)
        {
            return _lookupService.Resolve().GetOrgs(filter, orgTypeNames, rowsPerPage, rowOffset).ToDataContract<OrgPagedList, OrgPagedListContract>();
        }

        /// <summary>
        /// Gets the organization.
        /// </summary>
        /// <param name="opeCode">The ope code.</param>
        /// <param name="branchCode">The branch code.</param>
        /// <returns></returns>
        public RefOrganizationContract GetOrg(string opeCode, string branchCode)
        {
            return _lookupService.Resolve().GetOrg(opeCode, branchCode).ToDataContract<RefOrganization, RefOrganizationContract>();
        }

        /// <summary>
        /// Gets the organization by external org id.
        /// </summary>
        /// <param name="externalOrgId">The external org id.</param>
        /// <param name="branchCode">The branch code.</param>
        /// <returns></returns>
        public RefOrganizationContract GetOrgByExternalOrgID(string externalOrgId)
        {
            return _lookupService.Resolve().GetOrgByExternalID(externalOrgId).ToDataContract<RefOrganization, RefOrganizationContract>();
        }

        /// <summary>
        /// Gets the organization by RefOrganizationID.
        /// </summary>
        /// <param name="organizationId">The RefOrganizationID.</param>
        /// <returns>RefOrganizationContract</returns>
        public RefOrganizationContract GetOrgByOrganizationId(int organizationId)
        {
            return _lookupService.Resolve().GetOrgByOrganizationID(organizationId).ToDataContract<RefOrganization, RefOrganizationContract>();
        }
        /// <summary>
        /// Updates Organization bool attributes ReadyToLaunch, LookupAllowed, etc... based on Organization ID
        /// </summary>
        /// <param name="iOrgID">Organization ID </param>
        /// <param name="bIsLookupAllowed">Allowed look up bool flag </param>
        /// <param name="sModifiedBy">user id</param>
        /// <returns>bool</returns>
        //TODO:   bool UpdateRefOrg(Nullable<int> iOrgID, Nullable<bool> bIsLookupAllowed, string sModifiedBy);

        /// <summary>
        /// update Organization Product Subscription table
        /// </summary>
        /// <param name="iOrgID">Organization ID</param>
        /// <param name="dtProductList">System.Data.DataTable</param>
        /// <param name="sModifiedBy">User ID</param>
        /// <returns></returns>
        //TODO:   bool UpdateOrgProductSubscription(Nullable<int> iOrgID, System.Data.DataTable dtProductList, string sModifiedBy);

        /// <summary>
        /// Gets Organization products
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public List<RefOrganizationProductContract> GetOrgProducts(int orgId)
        {
            return _lookupService.Resolve().GetOrgProducts(orgId).ToDataContract<IList<RefOrganizationProduct>, List<RefOrganizationProductContract>>();
        }

        /// <summary>
        /// Gets Organization products for a given Member ID
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List<RefOrganizationProductContract></returns>
        public List<RefOrganizationProductContract> GetOrgProductsByMemberId(int userId)
        {
            return _userService.Resolve().GetOrgProductsByMemberID(userId).ToDataContract<IList<RefOrganizationProduct>, List<RefOrganizationProductContract>>();
        }

        /// <summary>
        /// Gets profile questions
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RefProfileQuestionContract> GetAllProfileQuestions()
        {
            return _lookupService.Resolve().GetAllProfileQuestions().ToDataContract<RefProfileQuestion, RefProfileQuestionContract>();
        }

        /// <summary>
        /// Gets profile answers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RefProfileAnswerContract> GetAllProfileAnswers()
        {
            return _lookupService.Resolve().GetAllProfileAnswers().ToDataContract<RefProfileAnswer, RefProfileAnswerContract>();
        }

        /// <summary>
        /// Gets profile Responses
        /// </summary>
        /// <returns></returns>
        public List<MemberProfileQAContract> GetProfileResponses(int userId)
        {
            return _userService.Resolve().GetUserProfileQuestionsAndAnswers(userId).ToDataContract<MemberProfileQA, MemberProfileQAContract>();
        }


        /// <summary>
        /// Deletes the alert.
        /// </summary>
        /// <param name="alertId">The alert id.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool DeleteAlert(int alertId)
        {
            return _alertService.Resolve().DeleteAlert(alertId);
        }

        /// <summary>
        /// Deletes the loan.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="loanId">The loan id.</param>
        /// <returns>bool</returns>
        public bool DeleteLoan(int userId, int loanId)
        {
            return _loanService.Resolve().RemoveLoan(userId, loanId);
        }

        /// <summary>
        /// Updates the loan.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="loan">The loan.</param>
        /// <returns>MemberReportedLoanContract</returns>
        public MemberReportedLoanContract UpdateLoan(int userId, MemberReportedLoanContract loanContract)
        {
            return _loanService.Resolve().UpdateLoan(userId, loanContract.ToDomainObject<MemberReportedLoanContract, MemberReportedLoan>()).ToDataContract<MemberReportedLoan, MemberReportedLoanContract>(); ;
        }

        /// <summary>
        /// Deletes the user's response to the specified question.
        /// </summary>
        /// <param name="lessonUserId">The lesson user id.</param>
        /// <param name="lessonId">The lesson id.</param>
        /// <param name="questionId">The question id.</param>
        /// <param name="questionResponseId">The question response id.</param>
        /// <param name="groupNumber">The group number.</param>
        /// <returns></returns>
        public bool DeleteUserLessonQuestionResponses(int lessonUserId, int lessonId,int? questionId, int? questionResponseId, int? groupNumber)
        {
            return _lessonsService.Resolve().DeleteUserLessonQuestionResponses(lessonUserId, lessonId, questionId, questionResponseId, groupNumber);
        }

        /// <summary>
        /// Gets the payment reminders.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public IList<PaymentReminderContract> GetUserPaymentReminders(int userId)
        {
            return _reminderService.Resolve().GetUserPaymentReminders(userId).ToDataContract<IList<PaymentReminder>, List<PaymentReminderContract>>();
        }

        /// <summary>
        /// Saves the payment reminders.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="reminders">The reminders.</param>
        /// <returns></returns>
        public RemindersUpdateStatus SaveUserPaymentReminders(int userId, IList<PaymentReminderContract> reminders)
        {
            return _reminderService.Resolve().SavePaymentReminders(userId, reminders.ToDomainObject<IList<PaymentReminderContract>, List<PaymentReminder>>());
        }

        /// <summary>
        /// Imports the loan file.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="file">The file.</param>
        /// <param name="sourceName">The source name e.g. 'KWYO'</param>
        /// <returns>IList{MemberReportedLoanContract}.</returns>
        public IList<MemberReportedLoanContract> ImportUserLoans(int userId, byte[] file, string sourceName)
        {
            var memberReportedLoans = _loanService.Resolve().ImportUserLoans(userId, file, sourceName);
            return memberReportedLoans.ToDataContract<MemberReportedLoan, MemberReportedLoanContract>();
        }

        /// <summary>
        /// Gets the member reported loans.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>IList{MemberReportedLoanContract}.</returns>
        public IList<MemberReportedLoanContract> GetUserLoans(int userId)
        {
            return _loanService.Resolve().GetUserLoans(userId).ToDataContract<MemberReportedLoan, MemberReportedLoanContract>();
        }

        /// <summary>
        ///  Gets the user loans by user ID and array of record source names.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="recordSourceNames">array of record source names.</param>
        /// <returns></returns>
        public IList<MemberReportedLoanContract> GetUserLoansRecordSourceList(int userId, string[] recordSourceNames)
        {
            return _loanService.Resolve().GetUserLoansRecordSourceList(userId, recordSourceNames).ToDataContract<MemberReportedLoan, MemberReportedLoanContract>();
        }

        /// <summary>
        /// Gets the member reported loans.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>IList{MemberReportedLoanContract}.</returns>
        public IList<vMemberReportedLoansContract> GetReportedLoans(int userId)
        {
            return _loanService.Resolve().GetReportedLoans(userId).ToDataContract<vMemberReportedLoans, vMemberReportedLoansContract>();
        }

        /// <summary>
        /// Gets the survey.
        /// </summary>
        /// <param name="surveyId">The survey id.</param>
        /// <returns></returns>
        public SurveyContract GetSurvey(int surveyId)
        {
            return _surveyService.Resolve().GetSurvey(surveyId).ToDataContract<Survey, SurveyContract>();
        }

        /// <summary>
        /// Gets the survey results.
        /// </summary>
        /// <param name="surveyId">The survey id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public SurveyResponseContract GetUserSurveyResults(int surveyId, int userId)
        {
            return _surveyService.Resolve().GetUserSurveyResults(surveyId, userId).ToDataContract<SurveyResponse, SurveyResponseContract>();
        }

        /// <summary>
        /// Posts the survey.
        /// </summary>
        /// <param name="surveyResponse">The survey response.</param>
        /// <returns></returns>
        public bool PostSurvey(SurveyResponseContract surveyResponse)
        {
            return _surveyService.Resolve().PostSurvey(surveyResponse.ToDomainObject<SurveyResponseContract, SurveyResponse>());
        }

        /// <summary>
        /// Gets lessons reference data.
        /// </summary>
        /// <param name="refDataGroup">The ref data group id.</param>
        /// <returns></returns>
        public List<RefLessonLookupDataContract> GetLessonsReferenceData(RefLessonLookupDataTypes refDataGroup)
        {
            var toReturn = _lessonsService.Resolve().GetReferenceData(refDataGroup);
            return toReturn.ToDataContract<IList<RefLessonLookupData>, List<RefLessonLookupDataContract>>();
        }

        /// <summary>
        /// Gets the member lessons.
        /// </summary>
        /// <param name="lessonUserId">The lesson user id.</param>
        /// <returns></returns>
        public IList<MemberLessonContract> GetUserLessons(int lessonUserId)
        {
            return _lessonsService.Resolve().GetUserLessons(lessonUserId).ToDataContract<IList<MemberLesson>, List<MemberLessonContract>>();
        }

        /// <summary>
        /// Creates the user lesson.
        /// </summary>
        /// <param name="userLesson">The user lesson.</param>
        /// <returns></returns>
        public MemberLessonContract StartUserLesson(MemberLessonContract userLesson)
        {
            var memberLesson = _lessonsService.Resolve().StartUserLesson(userLesson.ToDomainObject<MemberLessonContract, MemberLesson>());
            return memberLesson.ToDataContract<MemberLesson, MemberLessonContract>();
        }

        /// <summary>
        /// Posts the users lesson 1 data.
        /// </summary>
        /// <param name="lesson">The lesson.</param>
        /// <returns></returns>
        public PostLessonResultContract<Lesson1Contract> PostLesson1(Lesson1Contract lesson)
        {
            return _lessonsService.Resolve().PostLesson1(lesson.ToDomainObject<Lesson1Contract, Lesson1>()).ToDataContract<PostLessonResult<Lesson1>, PostLessonResultContract<Lesson1Contract>>();
        }

        /// <summary>
        /// Posts the users lesson 2 data.
        /// </summary>
        /// <param name="lesson">The lesson.</param>
        /// <returns></returns>
        public PostLessonResultContract<Lesson2Contract> PostLesson2(Lesson2Contract lesson)
        {
            return _lessonsService.Resolve().PostLesson2(lesson.ToDomainObject<Lesson2Contract, Lesson2>()).ToDataContract<PostLessonResult<Lesson2>, PostLessonResultContract<Lesson2Contract>>();
        }

        /// <summary>
        /// Posts the users lesson 3 data.
        /// </summary>
        /// <param name="lesson">The lesson.</param>
        /// <returns></returns>
        public PostLessonResultContract<Lesson3Contract> PostLesson3(Lesson3Contract lesson)
        {
            return _lessonsService.Resolve().PostLesson3(lesson.ToDomainObject<Lesson3Contract, Lesson3>()).ToDataContract<PostLessonResult<Lesson3>, PostLessonResultContract<Lesson3Contract>>();
        }

        /// <summary>
        /// Gets the user's lesson1 results.
        /// </summary>
        /// <param name="lessonUserId">The lesson user id.</param>
        /// <param name="lessonStepId">The lesson step id.</param>
        /// <returns></returns>
        public Lesson1Contract GetUserLesson1Results(int lessonUserId, int? lessonStepId = null)
        {
            return _lessonsService.Resolve().GetUserLesson1Results(lessonUserId, lessonStepId).ToDataContract<Lesson1, Lesson1Contract>();
        }

        /// <summary>
        /// Gets the user's lesson 2 results.
        /// </summary>
        /// <param name="lessonUserId">The lesson user id.</param>
        /// <param name="lessonStepId">The lesson step id.</param>
        /// <returns></returns>
        public Lesson2Contract GetUserLesson2Results(int lessonUserId, int? lessonStepId = null)
        {
            return _lessonsService.Resolve().GetUserLesson2Results(lessonUserId, lessonStepId).ToDataContract<Lesson2, Lesson2Contract>();
        }

        /// <summary>
        /// Gets the user's lesson3 results.
        /// </summary>
        /// <param name="lessonUserId">The lesson user id.</param>
        /// <param name="lessonStepId">The lesson step id.</param>
        /// <returns></returns>
        public Lesson3Contract GetUserLesson3Results(int lessonUserId, int? lessonStepId = null)
        {
            return _lessonsService.Resolve().GetUserLesson3Results(lessonUserId, lessonStepId).ToDataContract<Lesson3, Lesson3Contract>();
        }

		/// <summary>
		/// Gets RefRegistration sources
		/// </summary>
		/// <returns>Ilist of all rows in the RegRegistrationSource table</returns>
		public IList<RefRegistrationSourceContract> GetAllRefRegistrationSources()
		{
			return _lookupService.Resolve().GetAllRefRegistrationSources().ToDataContract<RefRegistrationSource, RefRegistrationSourceContract>();
		}

        /// <summary>
        /// Gets all the RefRegistrationSourceTypes
        /// </summary>
        /// <returns></returns>
        public IList<RefRegistrationSourceTypeContract> GetAllRefRegistrationSourceTypes()
		{
			return _lookupService.Resolve().GetAllRefRegistrationSourceTypes().ToDataContract<RefRegistrationSourceType, RefRegistrationSourceTypeContract>();
		}

        /// <summary>
        /// Gets all RefUserRoles
        /// </summary>
        /// <returns></returns>
        public IList<RefMemberRoleContract> GetAllRefUserRoles()
        {
            return _lookupService.Resolve().GetAllRefUserRoles().ToDataContract<RefMemberRole, RefMemberRoleContract>();
        }

        /// <summary>
        /// Adds a row to the RefRegistrationSource table
        /// </summary>
        /// <param name="refRegistrationSourceContract">new RefRegistrationSource field values</param>
        /// <returns>bool</returns>
        public bool AddRefRegistrationSource(RefRegistrationSourceContract refRegistrationSourceContract)
        {
            return _lookupService.Resolve().AddRefRegistrationSource(refRegistrationSourceContract.ToDomainObject<RefRegistrationSourceContract, RefRegistrationSource>()); 
        }

        /// <summary>
        /// Updates a row to the RefRegistrationSource table
        /// </summary>
        /// <param name="refRegistrationSourceContract">updated RefRegistrationSource field values</param>
        /// <returns>bool</returns>
        public bool UpdateRefRegistrationSource(RefRegistrationSourceContract refRegistrationSourceContract)
        {
            return _lookupService.Resolve().UpdateRefRegistrationSource(refRegistrationSourceContract.ToDomainObject<RefRegistrationSourceContract, RefRegistrationSource>());
        }

        /// <summary>
        /// Gets all the RefCampaigns
        /// </summary>
        /// <returns>an IList of RefCampaigns</returns>
        public IList<RefCampaignContract> GetAllRefCampaigns()
        {
            return _lookupService.Resolve().GetAllRefCampaigns().ToDataContract<RefCampaign, RefCampaignContract>();
        }

        /// <summary>
        /// Adds a row to the RefCampaign table
        /// </summary>
        /// <param name="refCampaignContract">new RefCampaign field values</param>
        /// <returns>bool</returns>
        public bool AddRefCampaign(RefCampaignContract refCampaignContract)
        {
            return _lookupService.Resolve().AddRefCampaign(refCampaignContract.ToDomainObject<RefCampaignContract, RefCampaign>());
        }

        /// <summary>
        /// Updates a row to the RefCampaign table
        /// </summary>
        /// <param name="refCampaignContract">updated RefCampaign field values</param>
        /// <returns>bool</returns>
        public bool UpdateRefCampaign(RefCampaignContract refCampaignContract)
        {
            return _lookupService.Resolve().UpdateRefCampaign(refCampaignContract.ToDomainObject<RefCampaignContract, RefCampaign>());
        }

        /// <summary>
        /// Gets all the RefChannels
        /// </summary>
        /// <returns>an IList of RefChannels</returns>
        public IList<RefChannelContract> GetAllRefChannels()
        {
            return _lookupService.Resolve().GetAllRefChannels().ToDataContract<RefChannel, RefChannelContract>();
        }

        /// <summary>
        /// Associates the lessons with user.
        /// </summary>
        /// <param name="lessonUserId">The lesson user id.</param>
        /// <param name="userId">The user id.</param>
        public bool AssociateLessonsWithUser(int lessonUserId, int userId)
        {
            return _lessonsService.Resolve().AssociateLessonsWithUser(lessonUserId, userId);
        }

        /// <summary>
        /// Create A Loan.
        /// </summary>
        /// <param name="userid">The user id</param>
        /// <param name="loan">The loan to add</param>
        /// <returns>MemberReportedLoan</returns>
        public MemberReportedLoanContract CreateLoan(int userId, MemberReportedLoanContract loan)
        {
            return _loanService.Resolve().CreateLoan(userId, loan.ToDomainObject<MemberReportedLoanContract, MemberReportedLoan>()).ToDataContract<MemberReportedLoan, MemberReportedLoanContract>();
        }

        /// <summary>
        /// Updates the user lesson.
        /// </summary>
        /// <param name="userLesson">The user lesson.</param>
        /// <returns></returns>
        public bool UpdateUserLesson(MemberLessonContract userLesson)
        {
            return _lessonsService.Resolve().UpdateUserLesson(userLesson.ToDomainObject<MemberLessonContract, MemberLesson>());
        }


        /// <summary>
        /// Gets the member VLC profile.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public VLCUserProfileContract GetVlcMemberProfile(int userId)
        {
            return _userService.Resolve().GetVlcMemberProfile(userId).ToDataContract<VLCUserProfile, VLCUserProfileContract>();
        }

        /// <summary>
        /// Updates user's VLC profile.
        /// </summary>
        /// <param name="profile">The VLC Member Profile to update.</param>
        /// <returns></returns>
        public bool UpdateVlcMemberProfile(VLCUserProfileContract profile)
        {
            return _userService.Resolve().UpdateVlcMemberProfile(profile.ToDomainObject<VLCUserProfileContract, VLCUserProfile>());
        }

        /// <summary>
        /// Adds the VLC questions response.
        /// </summary>
        /// <param name="response">The VLC response to save.</param>
        /// <returns></returns>
        public bool AddVlcResponse(VLCUserResponseContract response)
        {
            return _surveyService.Resolve().AddVlcResponse(response.ToDomainObject<VLCUserResponseContract, VLCUserResponse>());
        }

        /// <summary>
        /// Adds the JellyVision Quiz response.
        /// </summary>
        /// <param name="response">The JellyVision Quiz response to save.</param>
        /// <returns></returns>
        public bool AddJellyVisionQuizResponse(JellyVisionQuizResponseContract response)
        {
            return _surveyService.Resolve().AddJellyVisionQuizResponse(response.ToDomainObject<JellyVisionQuizResponseContract, JellyVisionQuizResponse>());
        }

        /// <summary>
        /// Gets COL States reference data.
        /// </summary>
        /// <param name=""></param>
        /// <returns>List of RefStateContract</returns>
        public List<vCostOfLivingStateListContract> GetCOLStates()
        {
            return _surveyService.Resolve().GetCOLStates().ToDataContract<IList<vCostOfLivingStateList>, List<vCostOfLivingStateListContract>>();
        }

        /// <summary>
        /// Gets COL Urban Area reference data.
        /// </summary>
        /// <param name="stateId">The RefStateID</param>
        /// <returns>List of RefGeographicIndexContract</returns>
        public List<RefGeographicIndexContract> GetCOLUrbanAreas(int stateId)
        {
            return _surveyService.Resolve().GetCOLUrbanAreas(stateId).ToDataContract<IList<RefGeographicIndex>, List<RefGeographicIndexContract>>();
        }

        /// <summary>
        /// Gets the CostOfLiving results.
        /// </summary>
        /// <param name="cityA">The RefGeographicIndexID for CityA.</param>
        /// <param name="cityB">The RefGeographicIndexID for CityB.</param>
        /// <param name="salary">salary</param>
        /// <returns>COLResultsContract</returns>
        public COLResultsContract GetCOLResults(int cityA, int cityB, decimal salary)
        {
            return _surveyService.Resolve().GetCOLResults(cityA, cityB, salary).ToDataContract<COLResults, COLResultsContract>();
        }

        /// <summary>
        /// Gets JSI Majors reference data.
        /// </summary>
        /// <param name=""></param>
        /// <returns>RefMajorContract</returns>
        public JSIQuestionsContract GetRefMajors()
        {
            return new JSIQuestionsContract()
            {
                majors = _surveyService.Resolve().GetRefMajors().ToDataContract<IList<RefMajor>, List<RefMajorContract>>()
            };
        }
        
        /// <summary>
        /// Gets JSI States reference data.
        /// </summary>
        /// <param name=""></param>
        /// <returns>RefStateContract</returns>
        public JSIQuestionsContract GetRefStates()
        {
            return new JSIQuestionsContract()
            {
                states = _surveyService.Resolve().GetRefStates().ToDataContract<IList<RefState>, List<RefStateContract>>()
            };
        }

        /// <summary>
        /// Gets JSI School by MajorId and StateId data.
        /// </summary>
        /// <param name="majorId">JSI RefMajorId</param>
        /// <param name="stateId">JSI RefStateId</param>
        /// <returns>JSISchoolMajorContract</returns>
        public JSIQuestionsContract GetJSISchools(int majorId, int stateId)
        {
            return new JSIQuestionsContract()
            {
                schools = _surveyService.Resolve().GetJSISchools(majorId, stateId).ToDataContract<IList<JSISchoolMajor>, List<JSISchoolMajorContract>>()
            };
        }

        /// <summary>
        /// Adds the JSI Quiz response.
        /// </summary>
        /// <param name="response">The JellyVision Quiz response to save.</param>
        /// <returns>JSIQuestionsContract</returns>
        public JSIQuestionsContract PostJSIResponse(JSIQuizAnswerContract response)
        {
            return new JSIQuestionsContract()
            {
                results = _surveyService.Resolve().PostJSIResponse(response.ToDomainObject<JSIQuizAnswerContract, JSIQuizAnswer>()).ToDataContract<IList<JSIQuizResult>, List<JSIQuizResultContract>>()
            };
        }

        public Tuple<IEnumerable<vMemberAcademicInfoContract>, int> GetMembersAcademicInfoView(int startRowIndex, int maximumRows)
        {
            var result = _userService.Resolve().GetMembersAcademicInfoView(startRowIndex, maximumRows);
            IEnumerable<vMemberAcademicInfoContract> _vMemInfo = result.Item1.ToDataContract<vMemberAcademicInfo, vMemberAcademicInfoContract>();
            int _vMemCount = result.Item2;
            return Tuple.Create(_vMemInfo, _vMemCount);

        }

        /// <summary>
        /// Gets Organizations the Member is affiliated with
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<MemberOrganizationContract> GetMemberOrganizations(int userId)
        {
            return _userService.Resolve().GetMemberOrganizations(userId).ToDataContract<IList<MemberOrganization>, List<MemberOrganizationContract>>();
        }
	
        /// <summary>
        /// Gets Memebr products
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<MemberProductContract> GetMemberProducts(int userId)
        {
            return _userService.Resolve().GetMemberProducts(userId).ToDataContract<IList<MemberProduct>, List<MemberProductContract>>();
        }

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RefProductContract> GetAllProducts()
        {
            return _lookupService.Resolve().GetAllProducts().ToDataContract<RefProduct, RefProductContract>();
        }

        /// <summary>
        /// Add Member product
        /// </summary>
        /// <param name="memberProduct"></param>
        /// <returns></returns>
        public bool AddMemberProduct(MemberProductContract memberProduct)
        {
            return _userService.Resolve().AddMemberProduct(memberProduct.ToDomainObject<MemberProductContract, MemberProduct>());
        }

        /// <summary>
        /// Update Member product
        /// </summary>
        /// <param name="memberProduct"></param>
        /// <returns></returns>
        public bool UpdateMemberProduct(MemberProductContract memberProduct)
        {
            return _userService.Resolve().UpdateMemberProduct(memberProduct.ToDomainObject<MemberProductContract, MemberProduct>());
        }

        /// <summary>
        /// Gets Members for SALTShaker search and navigation
        /// </summary>
        /// <param name="MemberID"></param>
        /// <param name="FirstName"></param>
        /// <param name="LastName"></param>
        /// <param name="EmailAddress"></param>
        /// <param name="OrganizationName"></param>
        /// <returns>IEnumerable<vMemberAcademicInfoContract></returns>
        public IEnumerable<vMemberAcademicInfoContract> GetMembersBySearchParms(Nullable<Int32> MemberID, String FirstName, String LastName, String EmailAddress)
        {
            return _userService.Resolve().GetMembersBySearchParms(MemberID, FirstName, LastName, EmailAddress).ToDataContract<vMemberAcademicInfo, vMemberAcademicInfoContract>();
        }

        /// <summary>
        /// Gets MemberActivationHistory 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<MemberActivationHistoryContract> GetUserActivationHistory(int userId)
        {
            return _userService.Resolve().GetUserActivationHistory(userId).ToDataContract<IList<MemberActivationHistory>, List<MemberActivationHistoryContract>>();
        }

        /// <summary>
        /// Updates Organization bool attributes ReadyToLaunch, LookupAllowed, etc... based on Organization ID
        /// </summary>
        /// <param name="iOrgID">Organization ID </param>
        /// <param name="bIsLookupAllowed">Allowed look up bool flag </param>
        /// <param name="sModifiedBy">user id</param>
        /// <returns>bool</returns>
        public bool UpdateRefOrg(int iOrgID, Nullable<bool> bIsLookupAllowed, string sModifiedBy)
        {
            return _lookupService.Resolve().UpdateRefOrg(iOrgID, bIsLookupAllowed, sModifiedBy);
        }

        /// <summary>
        /// insert/update an OrganizationToDoList item
        /// </summary>
        /// <param name="toDoListItem"></param>
        /// <returns>bool</returns>
        public bool UpsertOrgToDoListItem(OrganizationToDoListContract toDoListItem)
        {
            return _lookupService.Resolve().UpsertOrgToDoList(toDoListItem.ToDomainObject<OrganizationToDoListContract, OrganizationToDoList>());
        }

        /// <summary>
        /// update Organization Product Subscription table
        /// </summary>
        /// <param name="iOrgID">Organization ID</param>
        /// <param name="dtProductList">System.Data.DataTable</param>
        /// <param name="sModifiedBy">User ID</param>
        /// <returns></returns>
        public bool UpdateOrgProductsSubscription(Nullable<int> iOrgID, System.Data.DataTable dtProductList, string sModifiedBy)
        {
            return _lookupService.Resolve().UpdateOrgProductsSubscription(iOrgID, dtProductList, sModifiedBy);
        }

        /// <summary>
        /// Inserts or Updates user Scholarship question/s responses 
        /// </summary>
        /// <param name="MemberID">ID</param>
        /// <param name="responses">IList<MemberQA></param>
        /// <returns>bool</returns>
        public bool UpsertQuestionAnswer(int MemberID, IList<MemberQAContract> Responses)
        {
            return _userService.Resolve().UpsertQuestionAnswer(MemberID, Responses.ToDomainObject<IList<MemberQAContract>,List<MemberQA>>());
        }

        /// <summary>
        /// Gets Member Scholarship Question Answer
        /// </summary>
        /// <param name="MemberID">Nullable<Int32></param>
        /// <param name="EmailAddress">string</param>
        /// <param name="SourceID">int</param>
        /// <returns>IEnumerable<vMemberQuestionAnswer></returns>
        public IEnumerable<vMemberQuestionAnswerContract> GetMemberQuestionAnswer(Nullable<Int32> MemberID, String EmailAddress, int SourceID)
        {
            return _userService.Resolve().GetMemberQuestionAnswer(MemberID, EmailAddress, SourceID).ToDataContract<vMemberQuestionAnswer, vMemberQuestionAnswerContract>();
        }

        /// <summary>
        /// Retrieves all todos for a given member
        /// </summary>
        /// <param name="memberID">ID</param>
        /// <returns>IEnumerable<MemberToDoListContract></returns>
        public IEnumerable<MemberToDoListContract> GetMemberToDos(int memberID)
        {
            return _userService.Resolve().GetMemberToDos(memberID).ToDataContract<MemberToDoList, MemberToDoListContract>();
        }

        /// <summary>
        /// Insert/update a todo
        /// </summary>
        /// <param name="todoContract">Todo</param>
        /// <returns>bool</returns>
        public bool UpsertMemberToDo(MemberToDoListContract todoContract)
        {
            return _userService.Resolve().UpsertMemberToDo(todoContract.ToDomainObject<MemberToDoListContract, MemberToDoList>());
        }

        /// <summary>
        /// Retrieves all Member Roles for a given member
        /// </summary>
        /// <param name="memberID">MemberID</param>
        /// <returns>List of MemberRole</returns>
        public IEnumerable<MemberRoleContract> GetMemberRoles(int memberID)
        {
            return _userService.Resolve().GetMemberRoles(memberID).ToDataContract<MemberRole, MemberRoleContract>();
        }

        /// <summary>
        /// Inserts/Updates MemberRoles
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="memberRoles"></param>
        /// <param name="modifiedBy"></param>
        /// <returns></returns>
        public bool UpsertMemberRoles(int memberId, IList<MemberRoleContract> memberRoles, string modifiedBy)
        {
            return _userService.Resolve().UpsertMemberRoles(memberId, memberRoles.ToDomainObject<IList<MemberRoleContract>, List<MemberRole>>(), modifiedBy);
        }
    }
}
