using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.SurveyService.ServiceContracts;
using ASA.Web.Services.SurveyService.DataContracts;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.SurveyService
{
    public class MockSurveyAdapter : ISurveyAdapter
    {
        public ResultCodeModel InsertSurvey(SurveyModel survey)
        {
            return MockJsonLoader.GetJsonObjectFromFile<ResultCodeModel>("SurveyService", @"SetSurvey");
        }

        public SurveyListModel GetResponseAndTotals(string surveyId)  
        {
            return MockJsonLoader.GetJsonObjectFromFile<SurveyListModel>("SurveyService", @"GetResponseAndTotals");
        }

        public SurveyListModel GetQuestionAndResponse(string surveyId)
        {
            return MockJsonLoader.GetJsonObjectFromFile<SurveyListModel>("SurveyService", @"GetQuestionAndResponse");
        }

        public SurveyListModel GetIndividualsResponse(string surveyId, string surveyDetailId, string individualId)
        {
            return MockJsonLoader.GetJsonObjectFromFile<SurveyListModel>("SurveyService", @"GetIndividualsResponse");
        }
    }
}
