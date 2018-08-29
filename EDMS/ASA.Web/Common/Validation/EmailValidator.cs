using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ASA.Web.Common.Validation
{
    public class EmailValidator : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            Regex emailRegex = new Regex(RegexStrings.REGISTRATION_FORM_EMAIL);
            //NOTE: we DO want a null value to pass validation here because we don't want to 
            // force all emails using EmailValdidator to be required.  There is a separate [Required]
            // data annotation that can be used for that.
            if (value == null || emailRegex.IsMatch((string)value))
                return true;
            else
                return false;
        }
    }
}
