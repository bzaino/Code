using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using ASA.Web.Services.ASAMemberService.DataContracts;

namespace ASA.Web.Services.ASAMemberService.Validation
{
    public class MemberOrganizationValidation
    {
        public static bool validateMemberOrganizationModel(MemberOrganizationModel MemberOrganizationModel)
        {
            var validationResults = new List<ValidationResult>();
            //validate MemberOrganizationModel
            var validationContext = new ValidationContext(MemberOrganizationModel, serviceProvider: null, items: null);
            bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(MemberOrganizationModel, validationContext, validationResults, true);

            return isValid;
        }

        public static bool validateMemberOrganizationModel(IList<MemberOrganizationModel> memberOrganizationModels)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(memberOrganizationModels, serviceProvider: null, items: null);
            bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(memberOrganizationModels, validationContext, validationResults, true);

            return isValid;
        }
    }
}
