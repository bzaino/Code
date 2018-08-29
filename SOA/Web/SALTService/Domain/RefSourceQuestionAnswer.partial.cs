using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Asa.Salt.Web.Services.Domain
{
    public partial class RefSourceQuestionAnswer
    {
        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public override int Id
        {
            get { return this.RefSourceQuestionAnswerID; }
        }

        /// <summary>
        /// Validates the specified validation context.
        /// </summary>
        /// <returns></returns>
        public override IList<ValidationResult> Validate()
        {
            var validationDescriptors = new AssociatedMetadataTypeTypeDescriptionProvider(typeof(RefSourceQuestionAnswer), typeof(RefSourceQuestionAnswerValidation));

            TypeDescriptor.AddProviderTransparent(validationDescriptors, typeof(RefSourceQuestionAnswer));

            var results = new List<ValidationResult>();
            var validationContext = new ValidationContext(this, null, null);

            Validator.TryValidateObject(this, validationContext, results, true);

            TypeDescriptor.RemoveProviderTransparent(validationDescriptors, typeof(RefSourceQuestionAnswer));

            return results.ToList();
        }

        /// <summary>
        /// Validation rules for RefSourceQuestionAnswers.
        /// </summary>
        public class RefSourceQuestionAnswerValidation
        {

            [StringLength(500)]
            public string AnswerText { get; set; }

        }

    }
}
