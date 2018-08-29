using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Asa.Salt.Web.Services.Domain
{
    public partial class PaymentReminder 
    {
        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public override int Id
        {
            get { return this.PaymentReminderId; }
        }

        /// <summary>
        /// Gets the next payment due date.
        /// </summary>
        /// <value>
        /// The next payment due date.
        /// </value>
       public DateTime NextPaymentDueDate 
       {
           get
           {
               var nextPaymentDate = this.DayOfMonth > DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) ? new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 1) : new DateTime(DateTime.Now.Year, DateTime.Now.Month, this.DayOfMonth);

               if (nextPaymentDate < DateTime.Now)
                       nextPaymentDate = nextPaymentDate.AddMonths(1);

                   return nextPaymentDate;   
           }
       }

       /// <summary>
       /// Validates this instance.
       /// </summary>
       /// <returns></returns>
       public override IList<ValidationResult> Validate()
       {
           var validationDescriptors = new AssociatedMetadataTypeTypeDescriptionProvider(typeof(PaymentReminder), typeof(PaymentReminderValidation));

           TypeDescriptor.AddProviderTransparent(validationDescriptors, typeof(PaymentReminder));

           var results = new List<ValidationResult>();
           var validationContext = new ValidationContext(this, null, null);

           Validator.TryValidateObject(this, validationContext, results, true);

           TypeDescriptor.RemoveProviderTransparent(validationDescriptors, typeof(PaymentReminder));

           return results.ToList();
       }

       /// <summary>
       /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
       /// </summary>
       /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
       /// <returns>
       ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
       /// </returns>
       public override bool Equals(object obj)
       {
           var reminder = obj as PaymentReminder;

           if (reminder == null)
           {
               return false;
           }

           if (reminder.PaymentReminderId == this.PaymentReminderId)
           {
               return true;
           }

           return reminder.ServicerName.Equals(ServicerName) && reminder.NumberOfLoans == NumberOfLoans && reminder.DayOfMonth == DayOfMonth;
       }

        //added to resolve compiler warnings
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class PaymentReminderValidation
    {
        [Required]
        public string PaymentReminderId { get; set; }

        [Required]
        public int MemberId { get; set; }

        [Required]
        [StringLength(80, ErrorMessage = "{0} must be less than {1} characters.")] 
        [RegularExpression(@"^[a-zA-Z'.\s | \d | \- | \/ | \$ | \£ | \€ | \( | \) | \ | \! | \% | \+ | \& | \, | \! $]{1,200}$", ErrorMessage = "Servicer Name is invalid!")]
        public string ServicerName { get; set; }

        [Required]
        [DefaultValue(1)]
        public int NumberOfLoans { get; set; }

        [Required]
        [Range(1, 31)]
        public int DayOfMonth { get; set; }
    }
}
