using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace ASA.Web.Common.Validation
{
    public class DateStringValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            DateTime dt;
            if (value == null || DateTime.TryParse((string)value, out dt))
                return true;
            else
                return false;
        }
    } 

}
