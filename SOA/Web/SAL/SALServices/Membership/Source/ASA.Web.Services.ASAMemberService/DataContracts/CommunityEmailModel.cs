using System.Collections.Generic;
using ASA.Web.Services.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace ASA.Web.Services.ASAMemberService.DataContracts
{
    public class CommunityEmailModel : BaseWebModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [DisplayName("Email Address")]
        [StringLength(60, ErrorMessage = "{0} must be less than {1} characters.")]
        public string FromEmailAddress { get; set; }

        [Required]
        [DisplayName("First Name")]
        [StringLength(80, ErrorMessage = "{0} must be less than {1} characters.")]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Last Name")]
        [StringLength(80, ErrorMessage = "{0} must be less than {1} characters.")]
        public string LastName { get; set; }

        [Required]
        [DisplayName("Subject")]
        [StringLength(100, ErrorMessage = "{0} must be less than {1} characters.")]
        public string Subject { get; set; }

        [Required]
        [DisplayName("Your Question")]
        [StringLength(400, ErrorMessage = "{0} must be less than {1} characters.")]
        public string YourQuestion { get; set; }

        [Required]
        [DisplayName("Membership Id")]
        [StringLength(50, ErrorMessage = "{0} must be less than {1} characters.")]
        public string MembershipId { get; set; }

        [DisplayName("Community Account activated bool")]
        [StringLength(50, ErrorMessage = "{0} must be less than {1} characters.")]
        public string IsCommunityAccountActivated { get; set; }
    }
}
