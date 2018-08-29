using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Asa.Salt.Web.Services.Domain
{
    public partial class MemberQA
    {
        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public override int Id
        {
           get { return this.ExternalSourceAnswerID; }
        }

        /// <summary>
        /// Validates the specified validation context.
        /// </summary>
        /// <returns></returns>
        public override IList<ValidationResult> Validate()
        {
            var validationDescriptors = new AssociatedMetadataTypeTypeDescriptionProvider(typeof(MemberQA), typeof(MemberQAValidation));

            TypeDescriptor.AddProviderTransparent(validationDescriptors, typeof(MemberQA));

            var results = new List<ValidationResult>();
            var validationContext = new ValidationContext(this, null, null);

            Validator.TryValidateObject(this, validationContext, results, true);

            TypeDescriptor.RemoveProviderTransparent(validationDescriptors, typeof(MemberQA));

            return results.ToList();
        }

        /// <summary>
        /// Validation rules for MemberQAs.
        /// </summary>
        public class MemberQAValidation
        {
            [StringLength(500)]
            public string AnswerText { get; set; }

            [StringLength(1000)]
            public string FreeformAnswerText { get; set; }
        }

    }
}

