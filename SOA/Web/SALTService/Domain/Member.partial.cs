using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Asa.Salt.Web.Services.Domain
{
    public partial class Member
    {
        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public override int Id
        {
            get { return this.MemberId; }
        }

        /// <summary>
        /// Gets or sets the Registration Source Name.
        /// </summary>
        /// <value>
        /// The type of the Registration Source Name.
        /// </value>
        public string RegistrationSourceName { get; set; }


        /// <summary>
        /// Gets or sets the profile question and answers.
        /// </summary>
        /// <value>
        /// The profile question and answers.
        /// </value>
        public List<MemberProfileQA> MemberProfileQAs { get; set; }

        /// <summary>
        /// Gets or sets the Organization products for the first/default org (member.RefOrganizations.first().RefOrganizationId.
        /// </summary>
        /// <value>
        /// The Organization products for the first/default org (member.RefOrganizations.first().RefOrganizationId.
        /// </value>
        public List<RefOrganizationProduct> OrganizationProducts { get; set; }
        
        /// <summary>
        /// The Organization ID used to pass single org info for Courses profile
        /// </summary>
        public int OrganizationIdForCourses { get; set; }

        public bool? DashboardEnabled { get; set; }

        /// <summary>
        /// Validates the specified validation context.
        /// </summary>
        /// <returns></returns>
        public override IList<ValidationResult> Validate()
        {
            var validationDescriptors = new AssociatedMetadataTypeTypeDescriptionProvider(typeof(Member), typeof(MemberValidation));

            TypeDescriptor.AddProviderTransparent(validationDescriptors, typeof(Member));

            var results = new List<ValidationResult>();
            var validationContext = new ValidationContext(this, null, null);

            Validator.TryValidateObject(this, validationContext, results, true);

            TypeDescriptor.RemoveProviderTransparent(validationDescriptors, typeof(Member));

            return results.ToList();
        }

    }

    /// <summary>
    /// Validation for members
    /// </summary>
    public class MemberValidation
    {
        [Required(ErrorMessage = "A first name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "A last name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "An email address is required.")]
        public string EmailAddress { get; set; }
    }

}
