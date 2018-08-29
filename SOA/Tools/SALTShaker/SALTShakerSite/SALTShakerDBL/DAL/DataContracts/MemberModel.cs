using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SALTShaker.DAL.DataContracts
{
    public class MemberModel
    {
        public int MemberId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public bool IsContactAllowed { get; set; }
        public Nullable<int> OrganizationId { get; set; }
        public int RegistrationSourceId { get; set; }
        public Guid ActiveDirectoryKey { get; set; }
        public Nullable<int> GradeLevelId { get; set; }
        public Nullable<int> EnrollmentStatusId { get; set; }
        public bool IsMemberActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> InvitationToken { get; set; }
        public DateTime MemberStartDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public string DisplayName { get; set; }
        public string CommunityDisplayName { get; set; }
        public bool IsCommunityActive { get; set; }
        public string USPostalCode { get; set; }
        public Nullable<short> YearOfBirth { get; set; }
    }
}
