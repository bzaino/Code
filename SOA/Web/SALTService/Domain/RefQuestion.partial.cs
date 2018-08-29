using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Asa.Salt.Web.Services.Domain
{
    public partial class RefQuestion
    {
        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public override int Id
        {
            get { return this.RefQuestionID; }
        }

        /// <summary>
        /// Validates the specified validation context.
        /// </summary>
        /// <returns></returns>
        public override IList<ValidationResult> Validate()
        {
            var validationDescriptors = new AssociatedMetadataTypeTypeDescriptionProvider(typeof(RefQuestion), typeof(RefQuestionValidation));

            TypeDescriptor.AddProviderTransparent(validationDescriptors, typeof(RefQuestion));

            var results = new List<ValidationResult>();
            var validationContext = new ValidationContext(this, null, null);

            Validator.TryValidateObject(this, validationContext, results, true);

            TypeDescriptor.RemoveProviderTransparent(validationDescriptors, typeof(RefQuestion));

            return results.ToList();
        }

        /// <summary>
        /// Validation rules for RefQuestions.
        /// </summary>
        public class RefQuestionValidation
        {

            [StringLength(500)]
            public string StandardQuestionText { get; set; }

        }

    }
}
