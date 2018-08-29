using System;

namespace SALTShaker.DAL.DataContracts
{
    public class SaltMemberModel
    {   
        //SALT Attributes
        public Nullable<int> MemberID { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String EmailAddress { get; set; }
        public DateTime MemberFirstActivationDate { get; set; }
        public String OrganizationName { get; set; }
        public String OrganizationDescription { get; set; }
        public string OrganizationExternalID { get; set; }
        public Nullable<DateTime> OrganizationEffectiveStartDate { get; set; }
        public Nullable<DateTime> OrganizationEffectiveEndDate { get; set; }
        public Nullable<Boolean> IsContracted { get; set; }
        public String OPECode { get; set; }
        public String BranchCode { get; set; }
        public String OrganizationLogoName { get; set; }
        public Nullable<Int32> MemberActivationHistoryID { get; set; }
        public Nullable<DateTime> ActivationDate { get; set; }
        public Nullable<DateTime> DeactivationDate { get; set; }
        public Boolean IsContactAllowed { get; set; }
        public Nullable<Int32> RefOrganizationID { get; set; }
        public int RefRegistrationSourceID { get; set; }
        public Nullable<Guid> InvitationToken { get; set; }
        public Guid ActiveDirectoryKey { get; set; }
        public String DisplayName { get; set; }
        public Boolean IsMemberActive { get; set; }
        public Nullable<Guid> EnrollmentStatusExternalID { get; set; }
        public String EnrollmentStatusDescription { get; set; }
        public String EnrollmentStatusCode { get; set; }
        public Nullable<Int32> RefEnrollmentStatusID { get; set; }
        public Nullable<Int32> RefGradeLevelID { get; set; }
        public String GradeLevelCode { get; set; }
        public String GradeLevelDescription { get; set; }
        public Nullable<Guid> GradeLevelExternalID { get; set; }
        public Boolean WelcomeEmailSent { get; set; }
        //test
        public System.DateTime LastLoginDate { get; set; }
    }
}
