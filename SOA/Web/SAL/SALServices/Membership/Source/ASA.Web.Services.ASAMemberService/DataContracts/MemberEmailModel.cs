using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ASA.Web.Common.Validation;
using ASA.Web.Services.Common;
using ASA.Web.Collections;


namespace ASA.Web.Services.ASAMemberService.DataContracts
{
    public class MemberEmailModel : BaseWebModel, IPrimary
    {
        public MemberEmailModel() : base() { }
        public MemberEmailModel(Boolean newRecord = false) : base(newRecord) { }

        public string EmailKey { get; set; }
        /// <summary>
        /// Used for testing
        /// </summary>
        public string IndividualId { get; set; }

        [DisplayName("Email")]
        [Required]
        [StringLength(64, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 8)]
        [EmailValidator(ErrorMessage = "Email address is invalid.")]
        public string EmailAddress { get; set; }

        [DisplayName("Type")]
        public string Type { get; set; }

        [DisplayName("Type ID")]
        public string TypeID { get; set; }

        public bool IsPrimary { get; set; }
    }
}
