using Asa.Salt.Web.Services.Contracts.Data;
using Asa.Salt.Web.Services.Contracts.Data.Lessons;
using Asa.Salt.Web.Services.Contracts.Data.Lessons.Lesson1;
using Asa.Salt.Web.Services.Contracts.Data.Lessons.Lesson2;
using Asa.Salt.Web.Services.Contracts.Data.Lessons.Lesson3;
using Asa.Salt.Web.Services.Domain;
using Asa.Salt.Web.Services.Domain.Lessons;
using Asa.Salt.Web.Services.Domain.Lessons.Lesson1;
using Asa.Salt.Web.Services.Domain.Lessons.Lesson2;
using Asa.Salt.Web.Services.Domain.Lessons.Lesson3;
using AutoMapper;
using LoanType = Asa.Salt.Web.Services.Domain.Lessons.Lesson3.LoanType;
using LoanTypeContract = Asa.Salt.Web.Services.Contracts.Data.Lessons.Lesson3.LoanTypeContract;

using System.Collections.Generic;

namespace Asa.Salt.Web.Services.Application.Implementation.Mapping
{
    public static class AutoMapperBootStrapper
    {
        public static void ConfigureAutoMapper()
        {
            // domain -> contract
            Mapper.CreateMap<EnrollmentStatus, EnrollmentStatusContract>();
            Mapper.CreateMap<RefOrganization, RefOrganizationContract>();
            Mapper.CreateMap<RefOrganizationType, RefOrganizationTypeContract>();
            Mapper.CreateMap<OrganizationToDoList, OrganizationToDoListContract>();
            Mapper.CreateMap<GradeLevel, GradeLevelContract>();
            Mapper.CreateMap<RecordSource, RecordSourceContract>();
            Mapper.CreateMap<Member, MemberContract>();
            Mapper.CreateMap<MemberOrganization, MemberOrganizationContract>().AfterMap((source, dest) =>
            {
                if (source.RefOrganization != null)
                {
                    dest.OrganizationId = source.RefOrganization.RefOrganizationID;
                    dest.OrganizationExternalID = source.RefOrganization.OrganizationExternalID;
                    dest.OrganizationName = source.RefOrganization.OrganizationName;
                    dest.IsContracted = source.RefOrganization.IsContracted;
                    dest.OECode = source.RefOrganization.OPECode;
                    dest.BranchCode = source.RefOrganization.BranchCode;
                    dest.OrganizationLogoName = source.RefOrganization.OrganizationLogoName;
                    dest.OrganizationAliases = source.RefOrganization.OrganizationAliases;
                    dest.RefSALTSchoolTypeID = source.RefOrganization.RefSALTSchoolTypeID;
                    dest.IsLookupAllowed = source.RefOrganization.IsLookupAllowed;
                    dest.RefOrganizationTypeID = source.RefOrganization.RefOrganizationType.Id;
                    dest.OrganizationTypeExternalID = source.RefOrganization.RefOrganizationType.OrganizationTypeExternalID;
                }
            });
            Mapper.CreateMap<MemberAlert, MemberAlertContract>();
            Mapper.CreateMap<MemberRole, MemberRoleContract>();
            Mapper.CreateMap<RefMemberRole, RefMemberRoleContract>();
            Mapper.CreateMap<MemberReportedLoan, MemberReportedLoanContract>();
            Mapper.CreateMap<ActivityType, ActivityTypeContract>();
            Mapper.CreateMap<AlertType, AlertTypeContract>();
            Mapper.CreateMap<PaymentReminder, PaymentReminderContract>();
            Mapper.CreateMap<SurveyResponse, SurveyResponseContract>();
            Mapper.CreateMap<Survey, SurveyContract>();
            Mapper.CreateMap<SurveyOption, SurveyOptionContract>();
            Mapper.CreateMap<UserAccount, Member>();
            Mapper.CreateMap<RefMajor, RefMajorContract>();
            Mapper.CreateMap<RefState, RefStateContract>();
            Mapper.CreateMap<RefLessonLookupData, RefLessonLookupDataContract>();
            Mapper.CreateMap<RefLessonLookupDataType, RefLessonLookupDataTypeContract>();
            Mapper.CreateMap<MemberLesson, MemberLessonContract>();
            Mapper.CreateMap<Lesson, LessonContract>();
            Mapper.CreateMap<LessonQuestion, LessonQuestionContract>();
            Mapper.CreateMap<LessonQuestionResponse, LessonQuestionResponseContract>();
            Mapper.CreateMap<Lesson1, Lesson1Contract>();
            Mapper.CreateMap<CardInformation, CardInformationContract>();
            Mapper.CreateMap<DebtReductionOptions, DebtReductionOptionsContract>();
            Mapper.CreateMap<Deferment, DefermentContract>();
            Mapper.CreateMap<Expense, ExpenseContract>();
            Mapper.CreateMap<FasterRepayment, FasterRepaymentContract>();
            Mapper.CreateMap<StandardRepayment, StandardRepaymentContract>();
            Mapper.CreateMap<Frequency, FrequencyContract>();
            Mapper.CreateMap<Goal, GoalContract>();
            Mapper.CreateMap<Income, IncomeContract>();
            Mapper.CreateMap<LoanType, LoanTypeContract>();
            Mapper.CreateMap<LowerPayment, LowerPaymentContract>();
            Mapper.CreateMap<FavoriteRepaymentPlan, FavoriteRepaymentPlanContract>();
            Mapper.CreateMap<Lesson2, Lesson2Contract>();
            Mapper.CreateMap<Lesson3, Lesson3Contract>();
            Mapper.CreateMap<PostLessonResult<Lesson1>, PostLessonResultContract<Lesson1Contract>>();
            Mapper.CreateMap<PostLessonResult<Lesson2>, PostLessonResultContract<Lesson2Contract>>();
            Mapper.CreateMap<PostLessonResult<Lesson3>, PostLessonResultContract<Lesson3Contract>>();
            Mapper.CreateMap<InSchoolDeferment, InSchoolDefermentContract>();
            Mapper.CreateMap<HardshipDeferment, HardshipDefermentContract>();
            Mapper.CreateMap<Forbearance, ForbearanceContract>();
            Mapper.CreateMap<AdditionalPayment, AdditionalPaymentContract>();
            Mapper.CreateMap<CustomTimeline, CustomTimelineContract>();
            Mapper.CreateMap<BetterInterestRate, BetterInterestRateContract>();
            Mapper.CreateMap<IncomeBasedRepayment, IncomeBasedRepaymentContract>();
            Mapper.CreateMap<ExtendedRepayment, ExtendedRepaymentContract>();
            Mapper.CreateMap<IncomeSensitiveRepayment, IncomeSensitiveRepaymentContract>();
            Mapper.CreateMap<VLCUserProfile, VLCUserProfileContract>();
            Mapper.CreateMap<vMemberReportedLoans, vMemberReportedLoansContract>();
            Mapper.CreateMap<JSISchoolMajor, JSISchoolMajorContract>();
            Mapper.CreateMap<JSIQuizResult, JSIQuizResultContract>();
            Mapper.CreateMap<MemberProduct, MemberProductContract>();
            Mapper.CreateMap<RefOrganizationProduct, RefOrganizationProductContract>();
            Mapper.CreateMap<RefProduct, RefProductContract>();
            Mapper.CreateMap<RefProductType, RefProductTypeContract>();
            Mapper.CreateMap<vMemberAcademicInfo, vMemberAcademicInfoContract>();
            Mapper.CreateMap<MemberContentInteraction, MemberContentInteractionContract>();
            Mapper.CreateMap<MemberActivityHistory, MemberActivityHistoryContract>();
            Mapper.CreateMap<MemberActivationHistory, MemberActivationHistoryContract>();
            Mapper.CreateMap<MemberProfileAnswer, MemberProfileAnswerContract>();
            Mapper.CreateMap<RefProfileQuestion, RefProfileQuestionContract>();
            Mapper.CreateMap<RefProfileQuestionType, RefProfileQuestionTypeContract>();
            Mapper.CreateMap<RefProfileAnswer, RefProfileAnswerContract>();
            Mapper.CreateMap<MemberProfileQA, MemberProfileQAContract>();
			Mapper.CreateMap<RefRegistrationSource, RefRegistrationSourceContract>();
			Mapper.CreateMap<RefRegistrationSourceType, RefRegistrationSourceTypeContract>();
            Mapper.CreateMap<RefCampaign, RefCampaignContract>();
            Mapper.CreateMap<RefChannel, RefChannelContract>();
            Mapper.CreateMap<RefGeographicIndex, RefGeographicIndexContract>();
            Mapper.CreateMap<vCostOfLivingStateList, vCostOfLivingStateListContract>();
            Mapper.CreateMap<COLResults, COLResultsContract>();
            Mapper.CreateMap<OrgPagedList, OrgPagedListContract>();
            Mapper.CreateMap<vSourceQuestion, vSourceQuestionContract>();
            Mapper.CreateMap<vMemberQuestionAnswer,vMemberQuestionAnswerContract>();
            Mapper.CreateMap<RefAnswer, RefAnswerContract>();
            Mapper.CreateMap<RefQuestion, RefQuestionContract>();
            Mapper.CreateMap<RefSource, RefSourceContract>();
            Mapper.CreateMap<RefSourceQuestion, RefSourceQuestionContract>();
            Mapper.CreateMap<RefSourceQuestionAnswer, RefSourceQuestionAnswerContract>();
            Mapper.CreateMap<MemberToDoList, MemberToDoListContract>();

            // contract -> domain>
			Mapper.CreateMap<RefRegistrationSourceTypeContract, RefRegistrationSourceType>();
			Mapper.CreateMap<RefRegistrationSourceContract, RefRegistrationSource>();
            Mapper.CreateMap<RefCampaignContract, RefCampaign>();
            Mapper.CreateMap<RefChannelContract, RefChannel>();
            Mapper.CreateMap<RefProfileAnswerContract, RefProfileAnswer>();
            Mapper.CreateMap<RefProfileQuestionContract, RefProfileQuestion>();
            Mapper.CreateMap<RefProfileQuestionTypeContract, RefProfileQuestionType>();
            Mapper.CreateMap<MemberProfileAnswerContract, MemberProfileAnswer>();
            Mapper.CreateMap<MemberActivationHistoryContract, MemberActivationHistory>();
            Mapper.CreateMap<MemberContentInteractionContract, MemberContentInteraction>();
            Mapper.CreateMap<JellyVisionQuizResponseContract, JellyVisionQuizResponse>();
            Mapper.CreateMap<JSIQuizAnswerContract, JSIQuizAnswer>();
            Mapper.CreateMap<VLCUserProfileContract, VLCUserProfile>();
            Mapper.CreateMap<VLCUserResponseContract, VLCUserResponse>();
            Mapper.CreateMap<VLCQuestionContract, VLCQuestion>();
            Mapper.CreateMap<LessonContract, Lesson>();
            Mapper.CreateMap<LessonQuestionContract, LessonQuestion>();
            Mapper.CreateMap<LessonQuestionResponseContract, LessonQuestionResponse>();
            Mapper.CreateMap<PostLessonResultContract<Lesson1Contract>, PostLessonResult<Lesson1>>();
            Mapper.CreateMap<PostLessonResultContract<Lesson2Contract>, PostLessonResult<Lesson2>>();
            Mapper.CreateMap<PostLessonResultContract<Lesson3Contract>, PostLessonResult<Lesson3>>();
            Mapper.CreateMap<Lesson2Contract, Lesson2>();
            Mapper.CreateMap<Lesson3Contract, Lesson3>();
            Mapper.CreateMap<Lesson1Contract, Lesson1>();
            Mapper.CreateMap<InSchoolDefermentContract, InSchoolDeferment>();
            Mapper.CreateMap<HardshipDefermentContract, HardshipDeferment>();
            Mapper.CreateMap<ForbearanceContract, Forbearance>();
            Mapper.CreateMap<AdditionalPaymentContract, AdditionalPayment>();
            Mapper.CreateMap<CustomTimelineContract, CustomTimeline>();
            Mapper.CreateMap<BetterInterestRateContract, BetterInterestRate>();
            Mapper.CreateMap<IncomeBasedRepaymentContract, IncomeBasedRepayment>();
            Mapper.CreateMap<ExtendedRepaymentContract, ExtendedRepayment>();
            Mapper.CreateMap<IncomeSensitiveRepaymentContract, IncomeSensitiveRepayment>();
            Mapper.CreateMap<FavoriteRepaymentPlanContract, FavoriteRepaymentPlan>();
            Mapper.CreateMap<CardInformationContract, CardInformation>();
            Mapper.CreateMap<DebtReductionOptionsContract, DebtReductionOptions>();
            Mapper.CreateMap<DefermentContract, Deferment>();
            Mapper.CreateMap<ExpenseContract, Expense>();
            Mapper.CreateMap<FasterRepaymentContract, FasterRepayment>();
            Mapper.CreateMap<StandardRepaymentContract, StandardRepayment>();
            Mapper.CreateMap<FrequencyContract, Frequency>();
            Mapper.CreateMap<GoalContract, Goal>();
            Mapper.CreateMap<IncomeContract, Income>();
            Mapper.CreateMap<LoanTypeContract, LoanType>();
            Mapper.CreateMap<LowerPaymentContract, LowerPayment>();
            Mapper.CreateMap<MemberLessonContract, MemberLesson>();
            Mapper.CreateMap<RefLessonLookupDataContract, RefLessonLookupData>();
            Mapper.CreateMap<RefLessonLookupDataTypeContract, RefLessonLookupDataType>();
            Mapper.CreateMap<PaymentReminderContract, PaymentReminder>();
            Mapper.CreateMap<SurveyResponseContract, SurveyResponse>();
            Mapper.CreateMap<MemberRoleContract, MemberRole>();
            Mapper.CreateMap<RefMemberRoleContract, RefMemberRole>();
            Mapper.CreateMap<RefMajorContract, RefMajor>();
            Mapper.CreateMap<RefStateContract, RefState>();
            Mapper.CreateMap<MemberProfileQAContract, MemberProfileQA>();
            Mapper.CreateMap<MemberQA, MemberQAContract>();
            Mapper.CreateMap<UserRegistrationContract, UserAccount>().AfterMap((source, dest) =>
            {
                dest.ActiveDirectoryAccount = new ActiveDirectoryUser();
                dest.ActiveDirectoryAccount.Password = source.Password;
                dest.ActiveDirectoryAccount.Username = source.EmailAddress;
                dest.ActiveDirectoryAccount.PasswordReminderQuestion = source.PasswordReminderQuestion;
                dest.ActiveDirectoryAccount.PasswordReminderQuestionAnswer = source.PasswordReminderQuestionAnswer;

                dest.Organizations = new List<OrgsForCreateMember>();
                foreach (var organization  in source.MemberOrganizations)
                {
                    OrgsForCreateMember org = new OrgsForCreateMember()
                    {
                        RefOrganizationID = organization.OrganizationId,
                        OPECode = organization.OECode,
                        BranchCode = organization.BranchCode,
                        ExpectedGraduationYear = organization.ExpectedGraduationYear,
                        SchoolReportingID = organization.SchoolReportingID
                    };
                    dest.Organizations.Add(org);
                }
            });
            Mapper.CreateMap<MemberContract, Member>().AfterMap((source, dest) =>
            {
                dest.EnrollmentStatus = new EnrollmentStatus();
                dest.GradeLevel = new GradeLevel();
               
                dest.GradeLevel.GradeLevelCode = source.GradeLevelCode;
                dest.EnrollmentStatus.EnrollmentStatusCode = source.EnrollmentStatusCode;


                if (string.IsNullOrEmpty(dest.GradeLevel.GradeLevelCode))
                {
                    dest.GradeLevel = null;
                }

                if (string.IsNullOrEmpty(dest.EnrollmentStatus.EnrollmentStatusCode))
                {
                    dest.EnrollmentStatus = null;
                }

            });
            Mapper.CreateMap<MemberOrganizationContract, MemberOrganization>();
            Mapper.CreateMap<OrgPagedListContract, OrgPagedList>();
            Mapper.CreateMap<MemberReportedLoan, MemberReportedLoanContract>();
            Mapper.CreateMap<MemberReportedLoanContract, MemberReportedLoan>().ForMember(l => l.InterestRateType, option => option.Ignore());
            Mapper.CreateMap<EnrollmentStatusContract, EnrollmentStatus>();
            Mapper.CreateMap<GradeLevelContract, GradeLevel>();
            Mapper.CreateMap<RefOrganizationContract, RefOrganization>();
            Mapper.CreateMap<RefOrganizationTypeContract, RefOrganizationType>();
            Mapper.CreateMap<MemberProductContract, MemberProduct>();
            Mapper.CreateMap<RefOrganizationProductContract, RefOrganizationProduct>();
            Mapper.CreateMap<RefProductContract, RefProduct>();
            Mapper.CreateMap<RefProductTypeContract, RefProductType>();
            Mapper.CreateMap<vMemberAcademicInfoContract, vMemberAcademicInfo>();
            Mapper.CreateMap<MemberActivityHistoryContract, MemberActivityHistory>();
            Mapper.CreateMap<ActivityTypeContract, ActivityType>();
            Mapper.CreateMap<vMemberReportedLoansContract, vMemberReportedLoans>();
            Mapper.CreateMap<vSourceQuestionContract, vSourceQuestion>();
            Mapper.CreateMap<vMemberQuestionAnswerContract, vMemberQuestionAnswer>();
            Mapper.CreateMap<RefAnswerContract, RefAnswer>();
            Mapper.CreateMap<RefQuestionContract, RefQuestion>();
            Mapper.CreateMap<RefSourceContract, RefSource>();
            Mapper.CreateMap<RefSourceQuestionContract, RefSourceQuestion>();
            Mapper.CreateMap<RefSourceQuestionAnswerContract, RefSourceQuestionAnswer>();
            Mapper.CreateMap<MemberQAContract, MemberQA>();
            Mapper.CreateMap<MemberToDoListContract, MemberToDoList>();
            Mapper.CreateMap<OrganizationToDoListContract, OrganizationToDoList>();
        }
    }
}
