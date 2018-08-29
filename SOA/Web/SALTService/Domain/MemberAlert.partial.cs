using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Asa.Salt.Web.Services.Domain
{
    public partial class MemberAlert
    {
        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public override int Id
        {
            get { return this.MemberAlertId; }
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns></returns>
        public override IList<ValidationResult> Validate()
        {
            var validationDescriptors = new AssociatedMetadataTypeTypeDescriptionProvider(typeof(MemberAlert), typeof(MemberAlertValidation));

            TypeDescriptor.AddProviderTransparent(validationDescriptors, typeof(MemberAlert));

            var results = new List<ValidationResult>();
            var validationContext = new ValidationContext(this, null, null);

            Validator.TryValidateObject(this, validationContext, results, true);

            TypeDescriptor.RemoveProviderTransparent(validationDescriptors, typeof(MemberAlert));

            return results.ToList();
        }
    }

    public class MemberAlertValidation
    {
        [Required]
        public string MemberAlertId { get; set; }

        [Required]
        public string AlertMessage { get; set; }

        [Required]
        public string AlertTypeId { get; set; }

        [Required]
        public bool IsAlertViewed { get; set; }
    }
}
