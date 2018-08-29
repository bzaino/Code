using System;
using System.Runtime.Serialization;


namespace Asa.Salt.Web.Services.Contracts.Data
{
    [DataContract(IsReference = true)]
    public class vMemberAcademicInfoContract : IEntity 
    {
        [DataMember]
        public int MemberID { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string EmailAddress { get; set; }
        [DataMember]
        public System.DateTime MemberFirstActivationDate { get; set; }
        [DataMember]
        public Nullable<int> MemberOrganizationID { get; set; }
        [DataMember]
        public string OrganizationName { get; set; }
        [DataMember]
        public Nullable<System.DateTime> EffectiveStartDate { get; set; }
        [DataMember]
        public Nullable<System.DateTime> EffectiveEndDate { get; set; }
        [DataMember]
        public string OrganizationDescription { get; set; }
        [DataMember]
        public string OrganizationExternalID { get; set; }
        [DataMember]
        public Nullable<bool> IsContracted { get; set; }
        [DataMember]
        public string OPECode { get; set; }
        [DataMember]
        public string BranchCode { get; set; }
        [DataMember]
        public string OrganizationLogoName { get; set; }
        [DataMember]
        public Nullable<int> MemberActivationHistoryID { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ActivationDate { get; set; }
        [DataMember]
        public Nullable<System.DateTime> DeactivationDate { get; set; }
        [DataMember]
        public bool IsContactAllowed { get; set; }
        [DataMember]
        public Nullable<int> RefOrganizationID { get; set; }
        [DataMember]
        public int RefRegistrationSourceID { get; set; }
        [DataMember]
        public Nullable<System.Guid> InvitationToken { get; set; }
        [DataMember]
        public System.Guid ActiveDirectoryKey { get; set; }
        [DataMember]
        public string DisplayName { get; set; }
        [DataMember]
        public bool IsMemberActive { get; set; }
        [DataMember]
        public string EnrollmentStatusDescription { get; set; }
        [DataMember]
        public string EnrollmentStatusCode { get; set; }
        [DataMember]
        public Nullable<int> RefEnrollmentStatusID { get; set; }
        [DataMember]
        public Nullable<int> RefGradeLevelID { get; set; }
        [DataMember]
        public string GradeLevelCode { get; set; }
        [DataMember]
        public string GradeLevelDescription { get; set; }
        [DataMember]
        public bool WelcomeEmailSent { get; set; }
    }
    
}
