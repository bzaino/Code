using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using ASA.Web.Services.ASAMemberService.DataContracts;

namespace ASA.Web.Services.ASAMemberService.Validation
{
    class VLCMemberProfileValidation
    {
        public static bool validateMemberProfileModel(VLCMemberProfileModel MemberProfileModel)
        {
            var validationResults = new List<ValidationResult>();
            //validate MemberProfileModel
            var validationContext = new ValidationContext(MemberProfileModel, serviceProvider: null, items: null);
            bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(MemberProfileModel, validationContext, validationResults, true);

            return isValid;
        }
    }
}
