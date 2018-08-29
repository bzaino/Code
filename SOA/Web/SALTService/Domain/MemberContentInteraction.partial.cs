using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Asa.Salt.Web.Services.Domain
{
    public partial class MemberContentInteraction
    {
        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public override int Id
        {
            get { return this.MemberContentInteractionID; }
        }

        /// <summary>
        /// Validates the specified validation context.
        /// </summary>
        /// <returns></returns>
        public override IList<ValidationResult> Validate()
        {
            var validationDescriptors = new AssociatedMetadataTypeTypeDescriptionProvider(typeof(MemberContentInteraction), typeof(MemberContentInteractionValidation));

            TypeDescriptor.AddProviderTransparent(validationDescriptors, typeof(MemberContentInteraction));

            var results = new List<ValidationResult>();
            var validationContext = new ValidationContext(this, null, null);

            Validator.TryValidateObject(this, validationContext, results, true);

            TypeDescriptor.RemoveProviderTransparent(validationDescriptors, typeof(MemberContentInteraction));

            return results.ToList();
        }

        /// <summary>
        /// Validation rules for user interactions.
        /// </summary>
        public class MemberContentInteractionValidation
        {

            public int MemberContentInteractionID { get; set; }
            [Required]
            public Nullable<int> MemberID { get; set; }
            [StringLength(100)]
            public string ContentID { get; set; }
            public int RefContentInteractionTypeID { get; set; }
            [StringLength(100)]
            public string MemberContentInteractionValue { get; set; }
            [StringLength(500)]
            public string MemberContentComment { get; set; }

        }
    }
}
