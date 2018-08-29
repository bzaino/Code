using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Asa.Salt.Web.Services.Domain
{
    public partial class RefOrganization
    {
        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public override int Id
        {
            get { return this.RefOrganizationID; }
        }

        /// <summary>
        /// Validates the specified validation context.
        /// </summary>
        /// <returns></returns>
        public override IList<ValidationResult> Validate()
        {
            var validationDescriptors = new AssociatedMetadataTypeTypeDescriptionProvider(typeof(RefOrganization), typeof(OrgValidation));

            TypeDescriptor.AddProviderTransparent(validationDescriptors, typeof(RefOrganization));

            var results = new List<ValidationResult>();
            var validationContext = new ValidationContext(this, null, null);

            Validator.TryValidateObject(this, validationContext, results, true);

            TypeDescriptor.RemoveProviderTransparent(validationDescriptors, typeof(RefOrganization));

            return results.ToList();
        }
    }

    /// <summary>
    /// Validation for organizations
    /// </summary>
    public class OrgValidation
    {
        //[Required(ErrorMessage = "OE Code is required.")]
        //public string OPECode { get; set; }

        //[Required(ErrorMessage = "Branch Code is required.")]
        //public string BranchCode { get; set; }

        //[Required(ErrorMessage = "School Name is required.")]
        //public string SchoolName { get; set; }
    }
}
