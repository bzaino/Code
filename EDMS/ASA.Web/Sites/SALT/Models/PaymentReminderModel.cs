using System;
using System.ComponentModel.DataAnnotations;

namespace ASA.Web.Sites.SALT.Models
{
    public class PaymentReminderModel
    {
        [Required(ErrorMessage = "Servicer is required!")]
        [Display(Name = "Servicer")]
        [DataType(DataType.Text)]
        public string Servicer { get; set; }

        [Required(ErrorMessage = "Repayment due date is required!")]
        [DataType(DataType.Date)]
        [Display(Name = "Repayment Due Date")]
        public DateTime RepaymentDueDate { get; set; }

        [Required(ErrorMessage = "Please add a loan count")]
        [Range(1, 20, ErrorMessage = "Please enter a number between 1 and 20")] 
        [Display(Name = "Loan Count")]
        public int loanCount { get; set; }
    }
}