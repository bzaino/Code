using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Asa.Salt.Web.Services.Domain
{
    public class UserAccount :IDomainObject<int>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAccount"/> class.
        /// </summary>
        public UserAccount()
        {
            ActiveDirectoryAccount = new ActiveDirectoryUser();
        }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        /// <value>
        /// The email address.
        /// </value>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is contact allowed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is contact allowed; otherwise, <c>false</c>.
        /// </value>
        public bool IsContactAllowed { get; set; }

        /// <summary>
        /// Gets or sets the user's organizations
        /// </summary>
        /// <value>
        /// The user's organizations
        /// </value>
        public List<MemberOrganization> MemberOrganizations { get; set; }

        /// <summary>
        /// Gets or sets the registration source name.
        /// </summary>
        /// <value>
        /// The registration source name.
        /// </value>
        public string RegistrationSourceName { get; set; }

        /// <summary>
        /// Gets or sets the registration source id.
        /// </summary>
        /// <value>
        /// The registration source id.
        /// </value>
        public Nullable<int> RegistrationSourceId { get; set; }

        /// <summary>
        /// Gets or sets the active directory key.
        /// </summary>
        /// <value>
        /// The active directory key.
        /// </value>
        public System.Guid ActiveDirectoryKey { get; set; }

        /// <summary>
        /// Gets or sets the grade level id.
        /// </summary>
        /// <value>
        /// The grade level id.
        /// </value>
        public Nullable<int> GradeLevelId { get; set; }


        /// <summary>
        /// Gets or sets the grade level.
        /// </summary>
        /// <value>
        /// The grade level.
        /// </value>
        public string GradeLevel { get; set; }

        /// <summary>
        /// Gets or sets the enrollment status id.
        /// </summary>
        /// <value>
        /// The enrollment status id.
        /// </value>
        public Nullable<int> EnrollmentStatusId { get; set; }

        /// <summary>
        /// Gets or sets the enrollment status.
        /// </summary>
        /// <value>
        /// The enrollment status.
        /// </value>
        public string EnrollmentStatus { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        /// <value>
        /// The created by.
        /// </value>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        /// <value>
        /// The created date.
        /// </value>
        public System.DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the invitation token.
        /// </summary>
        /// <value>
        /// The invitation token.
        /// </value>
        public Nullable<System.Guid> InvitationToken { get; set; }

        /// <summary>
        /// Gets or sets the active directory account.
        /// </summary>
        /// <value>
        /// The active directory account.
        /// </value>
        public ActiveDirectoryUser ActiveDirectoryAccount { get; set; }

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public int Id { get; private set; }

        /// <summary>
        /// Gets or sets the list of organizations.
        /// </summary>
        /// <value>
        /// The list of organizations.
        /// </value>
        public List<OrgsForCreateMember> Organizations { get; set; }

        /// <summary>
        /// Gets or sets the expected year of birth.
        /// </summary>
        /// <value>
        /// The year of birth.
        /// </value>
        public Nullable<short> YearOfBirth { get; set; }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns></returns>
        public IList<ValidationResult> Validate()
        {
            var validationDescriptors = new AssociatedMetadataTypeTypeDescriptionProvider(typeof(UserAccount), typeof(MemberValidation));

            TypeDescriptor.AddProviderTransparent(validationDescriptors, typeof(UserAccount));

            var results = new List<ValidationResult>();
            var validationContext = new ValidationContext(this, null, null);

            Validator.TryValidateObject(this, validationContext, results, true);

            TypeDescriptor.RemoveProviderTransparent(validationDescriptors, typeof(UserAccount));

            return results.ToList();
        }
    }

    public class OrgsForCreateMember
    {
        /// <summary>
        /// Gets or sets the RefOrganizationID.
        /// </summary>
        /// <value>
        /// The RefOrganizationID.
        /// </value>
        public int RefOrganizationID { get; set; }

        /// <summary>
        /// Gets or sets the ExpectedGraduationYear.
        /// </summary>
        /// <value>
        /// The ExpectedGraduationYear.
        /// </value>
        public Nullable<int> ExpectedGraduationYear { get; set; }

        /// <summary>
        /// Gets or sets the SchoolReportingID.
        /// </summary>
        /// <value>
        /// The SchoolReportingID.
        /// </value>
        public string SchoolReportingID { get; set; }

        /// <summary>
        /// Gets or sets the oe code.
        /// </summary>
        /// <value>
        /// The OPECode.
        /// </value>
        public string OPECode { get; set; }

        /// <summary>
        /// Gets or sets the branch code.
        /// </summary>
        /// <value>
        /// The BranchCode.
        /// </value>
        public string BranchCode { get; set; }
    }
}
