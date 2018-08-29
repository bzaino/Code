using System;
using System.Data.SqlTypes;
namespace Asa.Salt.Web.Services.Domain
{
    /// <summary>
    /// Used as list/table parameter to SQLServer store procedures excepting UDT (User Defined Table) GenericTableType 
    /// [TextParam]: nvarchar(1000). Use as main data transport (Field is not nullable)
    /// [ConvertTxtToDataType]: nvarchar(8) use as optional CAST value for [TextParam] when its content should be
    ///  converted from text to one of 3 allowed types(datetime, int, bit) (Field is nullable)
    /// [IntParamOrChildKey]: (int) use as optional int param or as child relationship criteria (Field is nullable)
    /// [IntParamOrParentKey]: (int) use as optional int param or as ParentKey relationship criteria (Field is nullable)
    /// 
    /// see class collapsed region for example
    /// </summary>
    public class UDT
    {
        //baisc object list usage example 
        #region
        /* given a c# list param such as IList<MemberQA> Responses */
        /* Create DataTable and define column to match SQLServer UDT param */
        //DataTable dtAddedAnswers = new DataTable("MemberQuestionAnswerTable") or whatever;
        //DataColumn[] cols = { 
        //                    new DataColumn{ColumnName = "TextParam", DataType = typeof(String), AllowDBNull = false, MaxLength = 1000},
        //                    new DataColumn{ColumnName = "ConvertTxtToDataType", DataType = typeof(String), AllowDBNull = true, MaxLength = 8},
        //                    new DataColumn{ColumnName = "IntParamOrChildKey", DataType = typeof(int), AllowDBNull = true},
        //                    new DataColumn{ColumnName = "IntParamOrParentKey", DataType = typeof(int), AllowDBNull = true}
        //                };
        // /*Add new column definitions to DataTable */
        //dtAddedAnswers.Columns.AddRange(cols);
        // /*poplulate with Responses list content*/
        //IEnumerable<UDT> eCollectionUDT = from answer in Responses 
        //        select new UDT()
        //        {
        //            TextParam = new string(answer.AnswerText.Take(500).ToArray()).Trim(),
        //            ConvertTxtToDataType = null,
        //            IntParamOrChildKey = answer.ExternalSourceAnswerID,
        //            IntParamOrParentKey = answer.ExternalSourceQuestionID
        //        };
        //foreach (var item in eCollectionUDT)
        //{
        //    dtAddedAnswers.Rows.Add(item.TextParam, item.ConvertTxtToDataType, item.IntParamOrChildKey, item.IntParamOrParentKey);
        //}
        // /*Assign Datatable dtAddedAnswers to SqlParameter questionAnswerTable and match name and type to those of store proc
        //            where TypeName is of type matching the UTD object. */
        //SqlParameter questionAnswerTable = new SqlParameter("i_MemberQuestionAnswerTable", SqlDbType.Structured)
        //{
        //    Value = dtAddedAnswers,
        //    ParameterName = "@i_MemberQuestionAnswerTable",
        //    TypeName = "dbo.GenericTableType"
        //};
        #endregion

        public String TextParam { get; set; }
        public String ConvertTxtToDataType { get; set; }
        public int? IntParamOrChildKey { get; set; }
        public int? IntParamOrParentKey { get; set; }
        public String TextParam2 { get; set; }
    }
}
