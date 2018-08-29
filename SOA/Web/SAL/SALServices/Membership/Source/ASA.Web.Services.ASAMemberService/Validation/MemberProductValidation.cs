using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using ASA.Web.Services.ASAMemberService.DataContracts;

namespace ASA.Web.Services.ASAMemberService.Validation
{
    public class MemberProductValidation
    {
        public static bool validateMemberProductModel(MemberProductModel MemberProductModel)
        {
            var validationResults = new List<ValidationResult>();
            //validate MemberProductModel
            var validationContext = new ValidationContext(MemberProductModel, serviceProvider: null, items: null);
            bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(MemberProductModel, validationContext, validationResults, true);

            return isValid;
        }
    }
}
