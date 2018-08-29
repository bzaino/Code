using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using Common.Logging;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace ASA.Web.Services.Common
{
    public class ASAModelValidator
    {
        private const string CLASSNAME = "ASA.Web.Services.Common.ASAModelValidator";
        private static ASA.Log.ServiceLogger.IASALog _log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(CLASSNAME);

        #region Protected
        /// <summary>
        /// adds Validation results onto a List provided by caller
        /// </summary>
        /// <param name="errorList">List of type ErrorModel to add validation result</param>
        /// <param name="result">validation result</param>
        protected void AddResultMessages(List<ErrorModel> errorList, ValidationResult result)
        {
                ErrorModel error = new ErrorModel();
                if (result != null)
                {
                    error.BusinessMessage = result.ErrorMessage;
                    error.Code = string.Join(",", result.MemberNames.ToArray());
                    error.DetailMessage = result.ErrorMessage;
                    //error.ServiceName = obj.GetType().AssemblyQualifiedName;
                }

                //Add ErrorModel objs to this.ErrorList
                if (errorList == null)
                    errorList = new List<ErrorModel>();

                errorList.Add(error);  
        }

        /// <summary>
        /// adds Validation results onto a BaseWebModel object's ErrorList
        /// </summary>
        /// <param name="obj">object of type BaseWebModel to add validation result</param>
        /// <param name="result">validation result</param>
        protected void AddResultMessages(BaseWebModel obj, ValidationResult result)
        {
            ErrorModel error = new ErrorModel();
            if (result != null)
            {
                error.BusinessMessage = result.ErrorMessage;
                error.Code = string.Join(",", result.MemberNames.ToArray());
                error.DetailMessage = result.ErrorMessage;
                error.ServiceName = obj.GetType().AssemblyQualifiedName;
            }

            //Add ErrorModel objs to this.ErrorList
            if (obj.ErrorList == null)
                obj.ErrorList = new List<ErrorModel>();

            obj.ErrorList.Add(error);
        }
        #endregion

        #region Public

        /// <summary>
        /// trigger Data Annotation validators on a model
        /// </summary>
        /// <param name="model">The object to validate</param>
        /// <param name="errorList">List to return validation errors if model if not of type BaseWebModel</param>
        /// <returns>bool</returns>
        public bool Validate(object model, List<ErrorModel> errorList = null)
        {
            bool valid = false;

            var validationContext = new ValidationContext(model, null, null);// { MemberName = "FirstName" };
            var validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.Validator.TryValidateObject(model, validationContext, validationResults, true);

            if (validationResults != null)
            {
                if (validationResults.Count == 0)
                    valid = true;
                else
                {
                    //we've got validation errors!
                    foreach (ValidationResult result in validationResults)
                    {
                        if (model is BaseWebModel)
                        {
                            //put result message onto model
                            AddResultMessages((BaseWebModel)model, result);
                        }
                        else
                        {
                            //put result message onto errorList
                            AddResultMessages(errorList, result);
                        }
                    }

                    //put the errors from the BaseWebModel into the errorList if user has supplied one.
                    if (model is BaseWebModel && errorList != null)
                    {
                        BaseWebModel tempModel = (BaseWebModel)model;
                        errorList.AddRange(tempModel.ErrorList);
                    }
                }
            }

            return valid;
        }

        /// <summary>
        /// trigger Data Annotation validators on a model and return the results for the requested field
        /// </summary>
        /// <param name="model">The object validate</param>
        /// <param name="fieldNames">individual field to return validation error for</param>
        /// <param name="errorList">List to return validation errors if model if not of type BaseWebModel</param>
        /// <returns>bool</returns>
        public bool Validate(object model, string fieldName, List<ErrorModel> errorList = null)
        {
            string[] fieldNames = { fieldName };
            return Validate(model, fieldNames, errorList);
        }

        /// <summary>
        /// trigger Data Annotation validators on a model and return the results for the requested array of fields
        /// </summary>
        /// <param name="model">The object validate</param>
        /// <param name="fieldNames">array of field to return validation error for</param>
        /// <param name="errorList">List to return validation errors if model if not of type BaseWebModel</param>
        /// <returns>bool</returns>
        public bool Validate(object model, string[] fieldNames, List<ErrorModel> errorList=null)
        {
            bool valid = true;

            var validationContext = new ValidationContext(model, null, null);
            var validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.Validator.TryValidateObject(model, validationContext, validationResults, true);

            foreach (ValidationResult result in validationResults)
            {
                foreach (string fieldName in fieldNames)
                {
                    List<string> invalidFieldNames = result.MemberNames.ToList<string>();
                    if (invalidFieldNames.Contains(fieldName))
                    {
                        valid = false;
                        if (model is BaseWebModel)
                        {
                            //put result message onto model
                            AddResultMessages((BaseWebModel)model, result);
                        }
                        else
                        {
                            //put result message onto errorList
                            AddResultMessages(errorList, result);
                        }
                        break;
                    }
                }
            }

            //put all the errors on the BaseWebModel into the errorList if user has supplied one.
            if (valid == false && model is BaseWebModel && errorList != null)
            {
                BaseWebModel tempModel = (BaseWebModel)model;
                errorList.AddRange(tempModel.ErrorList);
            }

            return valid;
        }
        #endregion
    }
}
