using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.SurveyService.DataContracts;


namespace ASA.Web.Services.SurveyService.Validation
{
    public class SurveyValidation
    {
        public static bool ValidateSurveyId(string surveyId)
        {
	            bool bValid = false;
	            SurveyModel survey = new SurveyModel();
                survey.SurveyId = surveyId;
                if (surveyId != null && survey.IsValid("Survey Id"))
	            {
	                bValid = true;
	            }
	
	            return bValid;
        }

        public static bool ValidateInputSurveyList(SurveyListModel sList)
        {
            bool bValid = false;
            if (sList != null && sList.Surveys != null)
            {
                bValid = true;
                foreach (SurveyModel survey in sList.Surveys)
                {
                    bValid &= survey.IsValid();
                    if (!bValid)
                        break;
                }
            }

            return bValid;
        }

        public static bool ValidateSurvey(SurveyModel survey)
        {
            bool bValid = false;
            if (survey != null)
                bValid = survey.IsValid();
            return bValid;
        }
    }
}
