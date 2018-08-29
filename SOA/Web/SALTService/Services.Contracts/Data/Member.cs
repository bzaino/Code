using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(EnrollmentStatusContract))]
    [KnownType(typeof(GradeLevelContract))]
    [KnownType(typeof(RefRegistrationSourceContract))]
    [KnownType(typeof(MemberAlertContract))]
    [KnownType(typeof(MemberReportedLoanContract))]
    [KnownType(typeof(PaymentReminderContract))]
    [KnownType(typeof(SurveyResponseContract))]
    [KnownType(typeof(MemberLessonContract))]
    [KnownType(typeof(VLCUserResponseContract))]
    [KnownType(typeof(VLCUserProfileContract))]
    [KnownType(typeof(MemberRoleContract))]
    [KnownType(typeof(MemberProductContract))]
    [KnownType(typeof(MemberContentInteractionContract))]
    [KnownType(typeof(MemberActivityHistoryContract))]
    [KnownType(typeof(MemberActivationHistoryContract))]    
    [KnownType(typeof(MemberProfileAnswerContract))]
    [KnownType(typeof(MemberOrganizationContract))]
    public class MemberContract : IEntity 
    {
        public MemberContract()
        {
            this.MemberAlerts = new HashSet<MemberAlertContract>();
            this.MemberReportedLoans = new HashSet<MemberReportedLoanContract>();
            this.PaymentReminders = new HashSet<PaymentReminderContract>();
            this.SurveyResponses = new HashSet<SurveyResponseContract>();
            this.MemberLessons = new HashSet<MemberLessonContract>();
            this.VLCUserResponses = new HashSet<VLCUserResponseContract>();
            this.VLCUserProfile = new HashSet<VLCUserProfileContract>();
            this.MemberRoles = new HashSet<MemberRoleContract>();
            this.MemberProducts = new HashSet<MemberProductContract>();
            this.MemberContentInteractions = new HashSet<MemberContentInteractionContract>();
            this.MemberActivityHistories = new HashSet<MemberActivityHistoryContract>();
            this.MemberActivationHistories = new HashSet<MemberActivationHistoryContract>();
            this.MemberProfileAnswers = new HashSet<MemberProfileAnswerContract>();
            this.MemberOrganizations = new HashSet<MemberOrganizationContract>();
        }
    
        [DataMember]
        public int MemberId { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string EmailAddress { get; set; }
        [DataMember]
        public bool IsContactAllowed { get; set; }
        [DataMember]
        public int RegistrationSourceId { get; set; }
        [DataMember]
        public System.Guid ActiveDirectoryKey { get; set; }
        [DataMember]
        public Nullable<int> GradeLevelId { get; set; }
        [DataMember]
        public Nullable<int> EnrollmentStatusId { get; set; }
        [DataMember]
        public bool IsMemberActive { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public System.DateTime CreatedDate { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        [DataMember]
        public Nullable<System.Guid> InvitationToken { get; set; }
        [DataMember]
        public System.DateTime MemberStartDate { get; set; }
        [DataMember]
        public System.DateTime LastLoginDate { get; set; }
        [DataMember]
        public string DisplayName { get; set; }
        [DataMember]
        public string CommunityDisplayName { get; set; }
        [DataMember]
        public bool IsCommunityActive { get; set; }
        [DataMember]
        public string USPostalCode { get; set; }
        [DataMember]
        public Nullable<short> YearOfBirth { get; set; }
        [DataMember]
        public string EnrollmentStatusCode { get; set; }
        [DataMember]
        public string GradeLevelCode { get; set; }
        [DataMember]
        public int OrganizationIdForCourses { get; set; }
	    [DataMember]
        public bool? DashboardEnabled { get; set; }
        [DataMember]
        public bool WelcomeEmailSent { get; set; }
    
        [DataMember]
        public virtual EnrollmentStatusContract EnrollmentStatus { get; set; }
        [DataMember]
        public virtual GradeLevelContract GradeLevel { get; set; }
        [DataMember]
        public virtual RefRegistrationSourceContract RegistrationSource { get; set; }
        [DataMember]
        public virtual ICollection<MemberAlertContract> MemberAlerts { get; set; }
        [DataMember]
        public virtual ICollection<MemberReportedLoanContract> MemberReportedLoans { get; set; }
        [DataMember]
        public virtual ICollection<PaymentReminderContract> PaymentReminders { get; set; }
        [DataMember]
        public virtual ICollection<SurveyResponseContract> SurveyResponses { get; set; }
        [DataMember]
        public virtual ICollection<MemberLessonContract> MemberLessons { get; set; }
        [DataMember]
        public virtual ICollection<VLCUserResponseContract> VLCUserResponses { get; set; }
        [DataMember]
        public virtual ICollection<VLCUserProfileContract> VLCUserProfile { get; set; }
        [DataMember]
        public virtual ICollection<MemberRoleContract> MemberRoles { get; set; }
        [DataMember]
        public virtual ICollection<MemberProductContract> MemberProducts { get; set; }
        [DataMember]
        public virtual ICollection<MemberContentInteractionContract> MemberContentInteractions { get; set; }
        [DataMember]
        public virtual ICollection<MemberActivityHistoryContract> MemberActivityHistories { get; set; }
        [DataMember]
        public virtual ICollection<MemberActivationHistoryContract> MemberActivationHistories { get; set; }        
        [DataMember]
        public virtual ICollection<MemberProfileAnswerContract> MemberProfileAnswers { get; set; }
        [DataMember]
        public virtual ICollection<MemberProfileQAContract> MemberProfileQAs { get; set; }
        [DataMember]
        public virtual ICollection<MemberOrganizationContract> MemberOrganizations { get; set; }
        [DataMember]
        public virtual ICollection<RefOrganizationProductContract> OrganizationProducts { get; set; }
    }
    
}
