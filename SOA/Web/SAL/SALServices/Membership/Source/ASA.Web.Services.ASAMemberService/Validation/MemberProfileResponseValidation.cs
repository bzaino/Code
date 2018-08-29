using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using ASA.Web.Services.ASAMemberService.DataContracts;

namespace ASA.Web.Services.ASAMemberService.Validation
{
    public class MemberProfileResponseValidation
    {
        public static bool validateMemberProfileResponse(MemberProfileResponseModel memberProfleResponse)
        {
            var validationResults = new List<ValidationResult>();
            //validate memberProfileResponse
            var validationContext = new ValidationContext(memberProfleResponse, serviceProvider: null, items: null);
            bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(memberProfleResponse, validationContext, validationResults, true);

            return isValid;
        }

        public static bool validateMemberProfileResponse(IList<MemberProfileResponseModel> memberProfleResponses)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(memberProfleResponses, serviceProvider: null, items: null);
            bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(memberProfleResponses, validationContext, validationResults, true);

            return isValid;
        }

        public static bool validateMemberProfileResponse(MemberProfileQAModel memberProfleResponse)
        {
            var validationResults = new List<ValidationResult>();
            //validate memberProfileResponse
            var validationContext = new ValidationContext(memberProfleResponse, serviceProvider: null, items: null);
            bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(memberProfleResponse, validationContext, validationResults, true);

            return isValid;
        }

        public static bool validateMemberProfileResponse(IList<MemberProfileQAModel> memberProfleResponses)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(memberProfleResponses, serviceProvider: null, items: null);
            bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(memberProfleResponses, validationContext, validationResults, true);

            return isValid;
        }

        public static bool validateMemberQAResponse(QuestionAnswerReponseModel memberQAResponses)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(memberQAResponses, serviceProvider: null, items: null);
            bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(memberQAResponses, validationContext, validationResults, true);

            return isValid;
        }
    }
}
