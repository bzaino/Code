using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Asa.Salt.Web.Services.Domain
{
    public partial class RefAnswer
    {
        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public override int Id
        {
            get { return this.RefAnswerID; }
        }

        /// <summary>
        /// Validates the specified validation context.
        /// </summary>
        /// <returns></returns>
        public override IList<ValidationResult> Validate()
        {
            var validationDescriptors = new AssociatedMetadataTypeTypeDescriptionProvider(typeof(RefAnswer), typeof(RefAnswerValidation));

            TypeDescriptor.AddProviderTransparent(validationDescriptors, typeof(RefAnswer));

            var results = new List<ValidationResult>();
            var validationContext = new ValidationContext(this, null, null);

            Validator.TryValidateObject(this, validationContext, results, true);

            TypeDescriptor.RemoveProviderTransparent(validationDescriptors, typeof(RefAnswer));

            return results.ToList();
        }

        /// <summary>
        /// Validation rules for RefAnswers.
        /// </summary>
        public class RefAnswerValidation
        {
            //public int RefAnswerID { get; set; }
            [StringLength(500)]
            public string StandardAnswerText { get; set; }
            //public bool IsRefAnswerActive { get; set; }
            //public string CreatedBy { get; set; }
            //public System.DateTime CreatedDate { get; set; }
            //public string ModifiedBy { get; set; }
            //public Nullable<System.DateTime> ModifiedDate { get; set; }
        }
    }
}
