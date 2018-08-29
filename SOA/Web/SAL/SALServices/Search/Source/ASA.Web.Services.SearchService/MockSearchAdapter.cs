using System;
using System.Collections.Generic;

using ASA.Web.Services.Common;
using ASA.Web.Services.SearchService.DataContracts;
using ASA.Web.Services.SearchService.ServiceContracts;

using Member = ASA.Web.Services.ASAMemberService.DataContracts;

namespace ASA.Web.Services.SearchService
{
    public class MockSearchAdapter : ISearchAdapter
    {
        public MockSearchAdapter()
        {
        }

        public SearchResultsModel DoSearch(string[] contentTypes, string size, string school, string role, string grade, string enrollStatus)
        {
            SearchResultsModel srm = new SearchResultsModel();
            srm.ErrorList.Add(new ErrorModel("DoSearch not implemented for mocking."));

            return srm;
        }

        public SearchResultsModel GetOrganizations(string filter)
        {
            SearchResultsModel srm = new SearchResultsModel();
            srm.ResultsMessage = "Records were found";

            try
            {
                //Dictionary<string, List<string>> r = MockJsonLoader.GetJsonDotNetObjectFromFile<Dictionary<string, List<string>>>("SearchService", "GetSchools.{FILTER}");
                SearchRecordModel sr = new SearchRecordModel();
                List<string> pk = new List<string>();
                pk.Add("School");
                sr.Fields.Add("P_Primary_Key", pk);

                sr.Fields.Add("P_School_Alias",new List<string>() { "BU", "Boston Univ." });
                sr.Fields["P_School_ID"] = new List<string>() { "50" };
                sr.Fields["P_School_Name"] = new List<string>() { "Boston University" };
                sr.Fields["RecordType"] = new List<string>() { "School" };

                for (int i = 50; i > 0; i--)
                {
                    SearchRecordModel record = new SearchRecordModel();
                    //"Fields":[
                    //    {"Key":"P_Primary_Key","Value":["School"]},
                    //    {"Key":"P_School_Alias","Value":["BU","Boston Univ."]},
                    //    {"Key":"P_School_ID","Value":["50"]},
                    //    {"Key":"P_School_Name","Value":["Boston University"]},
                    //    {"Key":"Record Type","Value":["School"]}
                    //]
                    string strId = sr.Fields["P_School_ID"][0] + i.ToString();
                    record.Fields["P_School_ID"] = new List<string>() { strId };
                    record.Fields["P_School_Alias"] = new List<string>() { "BU" + i, "Boston Univ." + i };
                    record.Fields["P_School_ID"] = new List<string>() { i.ToString() };
                    record.Fields["P_School_Name"] = new List<string>() { "Boston University" + i };
                    record.Fields["RecordType"] = new List<string>() { "School" };

                    srm.Records.Add(record);
                }
            }
            catch (Exception )
            {

            }

            return srm;
        }

        public IList<OrganizationProductModel> GetOrganizationProducts(int orgId)
        {
            throw new NotImplementedException();
        }
        public bool IsProductActive(IList<Member.MemberOrganizationModel> memberOrganizations, int productId)
        {
            throw new NotImplementedException();
        }
    }
}
