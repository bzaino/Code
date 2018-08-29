using System.Data.Entity;
using Asa.Salt.Web.Common.Types.Unity;
using Asa.Salt.Web.Services.Application.Implementation.Services;
using Asa.Salt.Web.Services.BusinessServices;
using Asa.Salt.Web.Services.BusinessServices.Interfaces;
using Asa.Salt.Web.Services.Configuration.Mail;
using Asa.Salt.Web.Services.Configuration.MemberReportedLoan;
using Asa.Salt.Web.Services.Contracts.Operations;
using Asa.Salt.Web.Services.Data.Model.Database;
using Asa.Salt.Web.Services.Data.Repositories;
using Asa.Salt.Web.Services.Email.Processors;
using Asa.Salt.Web.Services.Email.Processors.Interfaces;
using Asa.Salt.Web.Services.Jobs.Processors;
using Asa.Salt.Web.Services.Logging;
using Microsoft.Practices.Unity;
using Asa.Salt.Web.Services.BusinessServices.Utilities;
using EnrollmentStatus = Asa.Salt.Web.Services.Domain.EnrollmentStatus;
using GradeLevel = Asa.Salt.Web.Services.Domain.GradeLevel;
using LessonStep = Asa.Salt.Web.Services.Domain.LessonStep;
using LessonQuestion = Asa.Salt.Web.Services.Domain.LessonQuestion;
using LessonQuestionAttribute = Asa.Salt.Web.Services.Domain.LessonQuestionAttribute;
using LessonQuestionResponse = Asa.Salt.Web.Services.Domain.LessonQuestionResponse;
using LoanType = Asa.Salt.Web.Services.Domain.LoanType;
using Member = Asa.Salt.Web.Services.Domain.Member;
using MemberOrganization = Asa.Salt.Web.Services.Domain.MemberOrganization;
using MemberAlert = Asa.Salt.Web.Services.Domain.MemberAlert;
using MemberLesson = Asa.Salt.Web.Services.Domain.MemberLesson;
using MemberReportedLoan = Asa.Salt.Web.Services.Domain.MemberReportedLoan;
using MemberRole = Asa.Salt.Web.Services.Domain.MemberRole;
using MemberProduct = Asa.Salt.Web.Services.Domain.MemberProduct;
using MemberContentInteraction = Asa.Salt.Web.Services.Domain.MemberContentInteraction;
using MemberActivityHistory = Asa.Salt.Web.Services.Domain.MemberActivityHistory;
using PaymentReminder = Asa.Salt.Web.Services.Domain.PaymentReminder;
using RefLessonLookupData = Asa.Salt.Web.Services.Domain.RefLessonLookupData;
using RefLessonLookupDataType = Asa.Salt.Web.Services.Domain.RefLessonLookupDataType;
using RefOrganization = Asa.Salt.Web.Services.Domain.RefOrganization;
using RefOrganizationType = Asa.Salt.Web.Services.Domain.RefOrganizationType;
using Survey = Asa.Salt.Web.Services.Domain.Survey;
using SurveyOption = Asa.Salt.Web.Services.Domain.SurveyOption;
using SurveyResponse = Asa.Salt.Web.Services.Domain.SurveyResponse;
using RecordSource = Asa.Salt.Web.Services.Domain.RecordSource;
using VLCUserProfile = Asa.Salt.Web.Services.Domain.VLCUserProfile;
using VLCUserResponse = Asa.Salt.Web.Services.Domain.VLCUserResponse;
using VLCQuestion = Asa.Salt.Web.Services.Domain.VLCQuestion;
using RefMemberRole = Asa.Salt.Web.Services.Domain.RefMemberRole;
using RefMajor = Asa.Salt.Web.Services.Domain.RefMajor;
using RefState = Asa.Salt.Web.Services.Domain.RefState;
using RefOrganizationProduct = Asa.Salt.Web.Services.Domain.RefOrganizationProduct;
using RefProduct = Asa.Salt.Web.Services.Domain.RefProduct;
using RefProductType = Asa.Salt.Web.Services.Domain.RefProductType;
using vMemberAcademicInfo = Asa.Salt.Web.Services.Domain.vMemberAcademicInfo;
using ActivityType = Asa.Salt.Web.Services.Domain.ActivityType;
using MemberActivationHistory = Asa.Salt.Web.Services.Domain.MemberActivationHistory;
using MemberProfileAnswer = Asa.Salt.Web.Services.Domain.MemberProfileAnswer;
using RefProfileQuestion = Asa.Salt.Web.Services.Domain.RefProfileQuestion;
using RefProfileQuestionType = Asa.Salt.Web.Services.Domain.RefProfileQuestionType;
using RefProfileAnswer = Asa.Salt.Web.Services.Domain.RefProfileAnswer;
using vMemberReportedLoans = Asa.Salt.Web.Services.Domain.vMemberReportedLoans;
using RefRegistrationSource = Asa.Salt.Web.Services.Domain.RefRegistrationSource;
using RefRegistrationSourceType = Asa.Salt.Web.Services.Domain.RefRegistrationSourceType;
using RefCampaign = Asa.Salt.Web.Services.Domain.RefCampaign;
using RefChannel = Asa.Salt.Web.Services.Domain.RefChannel;
using RefGeographicIndex = Asa.Salt.Web.Services.Domain.RefGeographicIndex;
using vCostOfLivingStateList = Asa.Salt.Web.Services.Domain.vCostOfLivingStateList;
using vMemberQuestionAnswer = Asa.Salt.Web.Services.Domain.vMemberQuestionAnswer;
using vSourceQuestion = Asa.Salt.Web.Services.Domain.vSourceQuestion;
using RefSource = Asa.Salt.Web.Services.Domain.RefSource;
using RefSourceQuestion = Asa.Salt.Web.Services.Domain.RefSourceQuestion;
using RefSourceQuestionAnswer = Asa.Salt.Web.Services.Domain.RefSourceQuestionAnswer;
using RefQuestion = Asa.Salt.Web.Services.Domain.RefQuestion;
using RefAnswer = Asa.Salt.Web.Services.Domain.RefAnswer;
using MemberQuestionAnswer = Asa.Salt.Web.Services.Domain.MemberQuestionAnswer;
using MemberToDoList = Asa.Salt.Web.Services.Domain.MemberToDoList;

namespace Asa.Salt.Web.Services.Application.ServiceHost
{
    public static class UnityBootStrapper
    {
        /// <summary>
        /// The lock obj
        /// </summary>
        private static readonly object _lockObj = new object();

        /// <summary>
        /// The unity container
        /// </summary>
        private static UnityContainer _unityContainer;

        /// <summary>
        /// Gets the unity configuration.
        /// </summary>
        /// <returns></returns>
        public static UnityContainer GetUnityConfiguration()
        {
            lock (_lockObj)
            {
                if (_unityContainer != null)
                {
                    return _unityContainer;
                }

                _unityContainer = new UnityContainer();

                _unityContainer.RegisterType<System.ServiceModel.Dispatcher.IErrorHandler, SaltServiceErrorHandler>(new ContainerControlledLifetimeManager());
                _unityContainer.RegisterType<ILog, Log>(new ContainerControlledLifetimeManager());
                _unityContainer.RegisterType<ILoanCalc, LoanCalculators>(new ContainerControlledLifetimeManager());

                _unityContainer.RegisterType<IApplicationMailConfiguration, ApplicationMailConfiguration>(new ContainerControlledLifetimeManager());
                _unityContainer.RegisterType<IApplicationMemberReportedLoanConfiguration, ApplicationMemberReportedLoanConfiguration>(new ContainerControlledLifetimeManager());
                _unityContainer.RegisterType<IEmailProcessor, ApplicationEmailProcessor>(new ContainerControlledLifetimeManager());
                _unityContainer.RegisterType<PaymentReminderProcessor, PaymentReminderProcessor>(new PerResolveLifetimeManager());

                _unityContainer.RegisterType<ISaltService, SaltService>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IEmailService, EmailService>(new ContainerControlledLifetimeManager());
                _unityContainer.RegisterType<IReminderService, ReminderService>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IAuditService, AuditService>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<ILoanService, LoanService>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<ISurveyService, SurveyService>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<ILookupService, LookupService>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IAlertService, AlertService>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IMemberService, MemberService>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<ILessonsService, LessonsService>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IContentService, ContentService>(new PerResolveLifetimeManager());

                _unityContainer.RegisterType(typeof(ILazyResolver<>), typeof(LazyResolver<>), new PerResolveLifetimeManager());

                _unityContainer.RegisterType<SALTEntities>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<DbContext, SALTEntities>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IMemberRepository<Member, int>, MemberRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<EnrollmentStatus, int>, EnrollmentStatusRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<GradeLevel, int>, GradeLevelRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<RefOrganization, int>, RefOrganizationRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<RefOrganizationType, int>, RefOrganizationTypeRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<MemberOrganization, int>, MemberOrganizationRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<Survey, int>, SurveyRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<SurveyOption, int>, SurveyOptionRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<SurveyResponse, int>, SurveyResponseRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<LoanType, int>, LoanTypeRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<MemberReportedLoan, int>, MemberReportedLoanRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<MemberAlert, int>, MemberAlertRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<PaymentReminder, int>, PaymentReminderRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<LoanType, int>, LoanTypeRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<RefLessonLookupData, int>, RefLessonLookupDataRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<RefLessonLookupDataType, int>, RefLessonLookupDataTypeRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<LessonStep, int>, LessonStepRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<LessonQuestion, int>, LessonQuestionRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<LessonQuestionAttribute, int>, LessonQuestionAttributeRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<LessonQuestionResponse, int>, LessonQuestionResponseRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<MemberLesson, int>, MemberLessonRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<RecordSource, int>, RecordSourceRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<VLCUserProfile, int>, VLCUserProfileRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<VLCUserResponse, int>, VLCUserResponseRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<VLCQuestion, int>, VLCQuestionRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<MemberRole, int>, MemberRoleRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<RefMemberRole, int>, RefMemberRoleRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<RefMajor, int>, RefMajorRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<RefState, int>, RefStateRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<MemberProduct, int>, MemberProductRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<RefOrganizationProduct, int>, RefOrganizationProductRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<RefProduct, int>, RefProductRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<RefProductType, int>, RefProductTypeRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<vMemberAcademicInfo, int>, vMemberAcademicInfoRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<vMemberReportedLoans, int>, vMemberReportedLoansRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<MemberContentInteraction, int>, MemberContentInteractionRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<MemberActivityHistory, int>, MemberActivityHistoryRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<ActivityType, int>, ActivityTypeRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<MemberActivationHistory, int>, MemberActivationHistoryRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<MemberProfileAnswer, int>, MemberProfileAnswerRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<RefProfileQuestion, int>, RefProfileQuestionRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<RefProfileQuestionType, int>, RefProfileQuestionTypeRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<RefProfileAnswer, int>, RefProfileAnswerRepository>(new PerResolveLifetimeManager());
				_unityContainer.RegisterType<IRepository<RefRegistrationSource, int>, RefRegistrationSourceRepository>(new PerResolveLifetimeManager());
				_unityContainer.RegisterType<IRepository<RefRegistrationSourceType, int>, RefRegistrationSourceTypeRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<RefGeographicIndex, int>, RefGeographicIndexRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<vCostOfLivingStateList, int>, vCostOfLivingStateListRepository>(new PerResolveLifetimeManager());

                _unityContainer.RegisterType<IRepository<vSourceQuestion, int>, vSourceQuestionRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<vMemberQuestionAnswer, int>, vMemberQuestionAnswerRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<RefSource, int>, RefSourceRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<RefSourceQuestion, int>, RefSourceQuestionRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<RefSourceQuestionAnswer, int>, RefSourceQuestionAnswerRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<RefQuestion, int>, RefQuestionRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<RefAnswer, int>, RefAnswerRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<MemberQuestionAnswer, int>, MemberQuestionAnswerRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<RefCampaign, int>, RefCampaignRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<RefChannel, int>, RefChannelRepository>(new PerResolveLifetimeManager());
                _unityContainer.RegisterType<IRepository<MemberToDoList, int>, MemberToDoListRepository>(new PerResolveLifetimeManager());

            
                return _unityContainer;
            }
        }
    }
}
