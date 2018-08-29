using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ASA.Web.Services.SurveyService.DataContracts;
using System.ComponentModel.DataAnnotations;

namespace ASA.Web.Services.SurveyService.Validation
{
    class JellyVisionQuizResponseValidation
    {
        public static bool validateQuizResponseModel(JellyVisionQuizResponseModel quizResponseModel)
        {
            var validationResults = new List<ValidationResult>();

            //validate quizResponseModel
            var validationContext = new ValidationContext(quizResponseModel, serviceProvider: null, items: null);
            bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(quizResponseModel, validationContext, validationResults, true);
            //validate quizResponseModel.quizName
            validationContext = new ValidationContext(quizResponseModel.quizName, serviceProvider: null, items: null);
            isValid &= System.ComponentModel.DataAnnotations.Validator.TryValidateObject(quizResponseModel.quizName, validationContext, validationResults, true);
            //validate quizResponseModel.referrerID
            validationContext = new ValidationContext(quizResponseModel.referrerID, serviceProvider: null, items: null);
            isValid &= System.ComponentModel.DataAnnotations.Validator.TryValidateObject(quizResponseModel.referrerID, validationContext, validationResults, true);
            //validate quizResponseModel.responses
            validationContext = new ValidationContext(quizResponseModel.responses, serviceProvider: null, items: null);
            isValid &= System.ComponentModel.DataAnnotations.Validator.TryValidateObject(quizResponseModel.responses, validationContext, validationResults, true);
            //validate quizResponseModel.personalityType
            validationContext = new ValidationContext(quizResponseModel.personalityType, serviceProvider: null, items: null);
            isValid &= System.ComponentModel.DataAnnotations.Validator.TryValidateObject(quizResponseModel.personalityType, validationContext, validationResults, true);

            return isValid;
        }
    }
}
