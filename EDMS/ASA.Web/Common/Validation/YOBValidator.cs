using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace ASA.Web.Common.Validation
{
    public class YOBValidator : ValidationAttribute
    {
        public int MinimumAge { get; set; }

        public YOBValidator()
        {
            MinimumAge = 13;
        }


        public override bool IsValid(object value)
        {
            try
            {
                if (value != null)
                {
                    var yob = (short)value;

                    //check YOB against year
                    DateTime currentYear = DateTime.Now;

                    int age = currentYear.Year - yob;

                    //if the age is less than or equal to the minimumAge then it's an invalid YOB. Equal is not okay.
                    if (yob < 0 || age <= MinimumAge)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

