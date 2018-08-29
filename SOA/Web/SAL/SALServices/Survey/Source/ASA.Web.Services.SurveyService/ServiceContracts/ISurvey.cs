using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using ASA.Web.Services.Common;
using ASA.Web.Services.SurveyService.DataContracts;

using System.ServiceModel.Web;

namespace ASA.Web.Services.SurveyService.ServiceContracts
{
    [ServiceContract]
    public interface ISurvey
    {
        [OperationContract]
        ResultCodeModel InsertSurveyResponse(SurveyModel survey);

        [OperationContract]
        SurveyListModel GetQuestionAndResponse(string surveyId);

        [OperationContract]
        SurveyListModel GetIndividualsResponse(string surveyId, string surveyDetailId, string individualId);

        [OperationContract]
        SurveyListModel GetResponseAndTotals(string surveyId);
    }
}
