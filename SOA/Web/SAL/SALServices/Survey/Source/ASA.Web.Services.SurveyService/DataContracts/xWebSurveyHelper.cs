using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.Services.SurveyService.DataContracts
{
    public static class xWebSurveyHelper
    {

        //fields for GetSurveyResults stored procedure
        public static string SurveyresponseTotalsKeyName { get { return "srh_key"; } }
        public static string srtAnswerResult { get { return "AnswerResult"; } }
        public static string srtQuestionId { get { return "QuestionID"; } }
        public static string srtQuestionText { get { return "Question"; } }
        public static string srtResponse { get { return "Response"; } }
        public static string srtNumResponses { get { return "NumResponses"; } }

        //db table name - co_survey_detail
        //Xweb ObjectName - Surveyquestions -abbreviation sq
        //there can be multipl sets of questions with the main surveyId
        public static string SurveyquestionsObjectName { get { return "Surveyquestions"; } }
        public static string SurveyquestionsKeyName { get { return "srd_key"; } }

        //field names in co_survey_detail
        public const string sq_surveyId = "srd_srh_key";
        public const string sq_surveyDetailId = "srd_key";
        public const string sq_question_text = "srd_question_text";
        public const string sq_list_of_values = "srd_list_of_values";
        public const string sq_delete_flag = "srd_delete_flag";

        public static string[] GetSurveyquestionsFieldNames()
        {
            string[] fieldNames = new string[]{
            sq_surveyId,
            sq_surveyDetailId,
            sq_question_text,
            sq_list_of_values,
           };

           return fieldNames;
        }

        //db table name - co_survey_response
        //Xweb ObjectName - Surveyresponse -abbreviation sr
        public static string SurveyresponseObjectName { get { return "Surveyresponse"; } }
        public static string SurveyresponseKeyName { get { return "srr_key"; } }

        //field names in co_survey_response
        public const string sr_surveyId = "srr_srh_key";
        public const string sr_surveyDetailId = "srr_srd_key";
        public const string sr_membershipId = "srr_cst_key";
        public const string sr_response ="srr_response";
        public const string sr_responseDate = "srr_response_date";
        public const string sr_responseStatus = "srr_sss_key";

        public static string[] GetSurveyresponseFieldNames()
        {
            string[] fieldNames = new string[]{
            sr_surveyId,
            sr_surveyDetailId,
            sr_membershipId,
            sr_response,
            sr_responseDate,
            sr_responseStatus
           };

           return fieldNames;
        }
    }
}
