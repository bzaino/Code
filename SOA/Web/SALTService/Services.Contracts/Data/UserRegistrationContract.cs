using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Asa.Salt.Web.Services.Contracts.Data
{
    [KnownType(typeof(MemberOrganizationContract))]
    public class UserRegistrationContract
    {
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string EmailAddress { get; set; }
        [DataMember]
        public bool IsContactAllowed { get; set; }
        [DataMember]
        public virtual ICollection<MemberOrganizationContract> MemberOrganizations { get; set; }
        [DataMember]
        public Nullable<int> RegistrationSourceId { get; set; }
        [DataMember]
        public string RegistrationSourceName { get; set; }
        [DataMember]
        public string InvitationToken { get; set; }
        [DataMember]
        public string ActiveDirectoryKey { get; set; }
        [DataMember]
        public Nullable<int> GradeLevelId { get; set; }
        [DataMember]
        public string GradeLevel { get; set; }
        [DataMember]
        public Nullable<int> EnrollmentStatusId { get; set; }
        [DataMember]
        public string EnrollmentStatus { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public System.DateTime CreatedDate { get; set; }
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string PasswordReminderQuestion { get; set; }
        [DataMember]
        public string PasswordReminderQuestionAnswer { get; set; }
        [DataMember]
        public Nullable<int> ExpectedGraduationYear{ get; set; }
        [DataMember]
        public Nullable<short> YearOfBirth { get; set; }
    }
}
