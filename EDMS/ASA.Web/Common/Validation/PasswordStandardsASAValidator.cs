using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ASA.Web.Common.Validation
{
    public class PasswordStandardsASAValidator : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            try
            {

                if (value != null)
                {
                    Regex passwordRegex = new Regex(RegexStrings.REGISTRATION_FORM_PASSWORD);
                    //NOTE: we DO want a null value to pass validation here because we don't want to 
                    //force all passwords using PasswordStandardsASAValidator to be required.
                    //There is a separate [Required] data annotation that can be used for that.
                    if (value == null || passwordRegex.IsMatch((string)value))
                        return true;
                    else
                        return false;
                }
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
