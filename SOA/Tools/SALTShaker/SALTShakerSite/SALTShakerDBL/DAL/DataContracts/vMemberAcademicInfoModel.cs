using System;

namespace SALTShaker.DAL.DataContracts
{
    public class vMemberAcademicInfoModel
    {
        public int MemberID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public DateTime MemberFirstActivationDate { get; set; }
        public Nullable<int> MemberOrganizationID { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationDescription { get; set; }
        public string OrganizationExternalID { get; set; }
        public Nullable<bool> IsContracted { get; set; }
        public string OPECode { get; set; }
        public string BranchCode { get; set; }
        public string OrganizationLogoName { get; set; }
        public Nullable<int> MemberActivationHistoryID { get; set; }
        public Nullable<DateTime> ActivationDate { get; set; }
        public Nullable<DateTime> DeactivationDate { get; set; }
        public bool IsContactAllowed { get; set; }
        public Nullable<int> RefOrganizationID { get; set; }
        public int RefRegistrationSourceID { get; set; }
        public Nullable<Guid> InvitationToken { get; set; }
        public Guid ActiveDirectoryKey { get; set; }
        public string DisplayName { get; set; }
        public bool IsMemberActive { get; set; }
        public string EnrollmentStatusDescription { get; set; }
        public string EnrollmentStatusCode { get; set; }
        public Nullable<int> RefEnrollmentStatusID { get; set; }
        public Nullable<int> RefGradeLevelID { get; set; }
        public string GradeLevelCode { get; set; }
        public string GradeLevelDescription { get; set; }
        public bool WelcomeEmailSent { get; set; }
    }
}
