using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using ASA.Web.Services.ASAMemberService.DataContracts;

namespace ASA.Web.Services.ASAMemberService.Validation
{
    class ModelValidation<T>
    {
        public static bool validateModel(T ModelToValidate)
        {
            var validationResults = new List<ValidationResult>();
            //validate ModelToValidate of type T
            var validationContext = new ValidationContext(ModelToValidate, serviceProvider: null, items: null);
            bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(ModelToValidate, validationContext, validationResults, true);

            return isValid;
        }
    }
}
