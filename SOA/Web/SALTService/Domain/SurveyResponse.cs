using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Asa.Salt.Web.Services.Domain
{
   public partial class SurveyResponse
   {
       /// <summary>
       /// Gets the id.
       /// </summary>
       /// <value>
       /// The id.
       /// </value>
       public override int Id
       {
           get { return this.SurveyResponseId; }
       }

       /// <summary>
       /// Gets or sets the total response count.
       /// </summary>
       /// <value>
       /// The total response count.
       /// </value>
       public int TotalResponseCount { get; set; }

       /// <summary>
       /// Gets or sets the survey option text.
       /// </summary>
       /// <value>
       /// The survey option text.
       /// </value>
       public string SurveyOptionText { get; set; }

       /// <summary>
       /// Validates this instance.
       /// </summary>
       /// <returns></returns>
       public override IList<ValidationResult> Validate()
       {
           var validationDescriptors = new AssociatedMetadataTypeTypeDescriptionProvider(typeof(SurveyResponse), typeof(SurveyResponseValidation));

           TypeDescriptor.AddProviderTransparent(validationDescriptors, typeof(SurveyResponse));

           var results = new List<ValidationResult>();
           var validationContext = new ValidationContext(this, null, null);

           Validator.TryValidateObject(this, validationContext, results, true);

           TypeDescriptor.RemoveProviderTransparent(validationDescriptors, typeof(SurveyResponse));

           return results.ToList();
       }
   }

   public class SurveyResponseValidation
   {

       [Required]
       public int SurveyOptionId { get; set; }

       [Required]
       public string SurveyOptionText { get; set; }

       [Required]
       public string MemberId { get; set; }
   }
}
