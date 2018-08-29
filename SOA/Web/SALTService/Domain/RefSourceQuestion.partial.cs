using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Asa.Salt.Web.Services.Domain
{
    public partial class RefSourceQuestion
    {
        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public override int Id
        {
            get { return this.RefSourceQuestionID; }
        }

        /// <summary>
        /// Validates the specified validation context.
        /// </summary>
        /// <returns></returns>
        public override IList<ValidationResult> Validate()
        {
            var validationDescriptors = new AssociatedMetadataTypeTypeDescriptionProvider(typeof(RefSourceQuestion), typeof(RefSourceQuestionValidation));

            TypeDescriptor.AddProviderTransparent(validationDescriptors, typeof(RefSourceQuestion));

            var results = new List<ValidationResult>();
            var validationContext = new ValidationContext(this, null, null);

            Validator.TryValidateObject(this, validationContext, results, true);

            TypeDescriptor.RemoveProviderTransparent(validationDescriptors, typeof(RefSourceQuestion));

            return results.ToList();
        }

        /// <summary>
        /// Validation rules for RefSourceQuestions.
        /// </summary>
        public class RefSourceQuestionValidation
        {

            [StringLength(500)]
            public string QuestionText { get; set; }

        }

    }
}
