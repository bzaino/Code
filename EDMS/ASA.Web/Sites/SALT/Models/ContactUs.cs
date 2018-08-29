using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;


namespace ASA.Web.Sites.SALT.Models
{
    public class ContactUsModel
    {
        [Required(ErrorMessage = "Question type is required!")]
        [Display(Name = "What do you want to talk to us about?")]
        [DataType(DataType.Text)]
        public string QuestionType { get; set; }

        [Required(ErrorMessage = "Followup method is required!")]
        [Display(Name = "What's the best way to follow up with you?")]
        [DataType(DataType.Text)]
        public string FollowUpType { get; set; }

        [Required(ErrorMessage = "Followup method is required!")]
        [Display(Name = "Message subject line:")]
        [DataType(DataType.Text)]
        public string MessageSubject { get; set; }

        [Required(ErrorMessage = "Message is required!")]
        [Display(Name = "your message:")]
        [DataType(DataType.Text)]
        public string MessageBody { get; set; }

        public string SuccessMessage { get; set; }

        public bool Submitted { get; set; }
    }
}