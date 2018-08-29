//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Asa.Salt.Web.Services.Domain
{
    public partial class Member : DomainObject<int> 
    {
        public Member()
        {
            this.MemberAlerts = new HashSet<MemberAlert>();
            this.MemberReportedLoans = new HashSet<MemberReportedLoan>();
            this.PaymentReminders = new HashSet<PaymentReminder>();
            this.SurveyResponses = new HashSet<SurveyResponse>();
            this.MemberLessons = new HashSet<MemberLesson>();
            this.VLCUserResponses = new HashSet<VLCUserResponse>();
            this.VLCUserProfile = new HashSet<VLCUserProfile>();
            this.MemberRoles = new HashSet<MemberRole>();
            this.MemberProducts = new HashSet<MemberProduct>();
            this.MemberContentInteractions = new HashSet<MemberContentInteraction>();
            this.MemberActivityHistories = new HashSet<MemberActivityHistory>();
            this.MemberActivationHistories = new HashSet<MemberActivationHistory>();
            this.MemberProfileAnswers = new HashSet<MemberProfileAnswer>();
            this.MemberQuestionAnswers = new HashSet<MemberQuestionAnswer>();
            this.MemberOrganizations = new HashSet<MemberOrganization>();
            this.MemberToDoLists = new HashSet<MemberToDoList>();
        }
    
        public int MemberId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public bool IsContactAllowed { get; set; }
        public int RegistrationSourceId { get; set; }
        public System.Guid ActiveDirectoryKey { get; set; }
        public Nullable<int> GradeLevelId { get; set; }
        public Nullable<int> EnrollmentStatusId { get; set; }
        public bool IsMemberActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<System.Guid> InvitationToken { get; set; }
        public System.DateTime MemberStartDate { get; set; }
        public System.DateTime LastLoginDate { get; set; }
        public string DisplayName { get; set; }
        public string CommunityDisplayName { get; set; }
        public bool IsCommunityActive { get; set; }
        public string USPostalCode { get; set; }
        public Nullable<short> YearOfBirth { get; set; }
        public bool WelcomeEmailSent { get; set; }
    
        public virtual EnrollmentStatus EnrollmentStatus { get; set; }
        public virtual GradeLevel GradeLevel { get; set; }
        public virtual RefRegistrationSource RegistrationSource { get; set; }
        public virtual ICollection<MemberAlert> MemberAlerts { get; set; }
        public virtual ICollection<MemberReportedLoan> MemberReportedLoans { get; set; }
        public virtual ICollection<PaymentReminder> PaymentReminders { get; set; }
        public virtual ICollection<SurveyResponse> SurveyResponses { get; set; }
        public virtual ICollection<MemberLesson> MemberLessons { get; set; }
        public virtual ICollection<VLCUserResponse> VLCUserResponses { get; set; }
        public virtual ICollection<VLCUserProfile> VLCUserProfile { get; set; }
        public virtual ICollection<MemberRole> MemberRoles { get; set; }
        public virtual ICollection<MemberProduct> MemberProducts { get; set; }
        public virtual ICollection<MemberContentInteraction> MemberContentInteractions { get; set; }
        public virtual ICollection<MemberActivityHistory> MemberActivityHistories { get; set; }
        public virtual ICollection<MemberActivationHistory> MemberActivationHistories { get; set; }
        public virtual ICollection<MemberProfileAnswer> MemberProfileAnswers { get; set; }
        public virtual ICollection<MemberQuestionAnswer> MemberQuestionAnswers { get; set; }
        public virtual ICollection<MemberOrganization> MemberOrganizations { get; set; }
        public virtual ICollection<MemberToDoList> MemberToDoLists { get; set; }
    }
    
}