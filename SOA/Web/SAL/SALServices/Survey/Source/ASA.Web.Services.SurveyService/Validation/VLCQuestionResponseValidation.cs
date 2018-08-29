using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ASA.Web.Services.SurveyService.DataContracts;
using System.ComponentModel.DataAnnotations;

namespace ASA.Web.Services.SurveyService.Validation
{
    class VLCQuestionResponseValidation
    {
        public static bool validateQuestionResponseModel(VLCQuestionResponseModel QuestionResponseModel)
         {
            var validationResults = new List<ValidationResult>();
            //validate QuestionResponseModel
            var validationContext = new ValidationContext(QuestionResponseModel, serviceProvider: null, items: null);
            bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(QuestionResponseModel, validationContext, validationResults, true);
            //validate QuestionModel
            validationContext = new ValidationContext(QuestionResponseModel.Question, serviceProvider: null, items: null);
            isValid &= System.ComponentModel.DataAnnotations.Validator.TryValidateObject(QuestionResponseModel.Question, validationContext, validationResults, true);

            return isValid;
         }
    }
}
