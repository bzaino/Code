using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.SearchService.DataContracts;
using Endeca.Data;
using System.Collections.ObjectModel;
//using ASA.Web.Services.ASAMemberService.DataContracts;
using ASA.Web.Services.Proxies.SALTService;

namespace ASA.Web.Services.SearchService
{
    public static class TranslateSearchResultsModel
    {
        internal static DataContracts.SearchResultsModel MapNavigationResultToSearchResultsModel(Endeca.Data.NavigationResult navResult)
        {
            var srm = new SearchResultsModel();
            srm.Records = new List<SearchRecordModel>();

            var recordsRes = navResult.RecordsResult;
            long totNumRecords = recordsRes.TotalRecordCount;

            foreach (var r in recordsRes.Records)
            {
                var record = new SearchRecordModel();

                record.Fields = PopulateDictionary(r);

                srm.Records.Add(record);
            }

            return srm;
        }

        private static List<string> getMultiValue(Record r, string p)
        {
            var propValueList = new List<string>();

            if (r != null && r.Properties != null)
            {
                var attrCollection = r.Properties.ByKey(p);
                propValueList.AddRange(from a in attrCollection where a != null && a.Value != null select a.Value);
            }

            return propValueList;
        }

        private static Dictionary<string, List<string>> PopulateDictionary(Record r)
        {
            var recordInfo = new Dictionary<string,List<string>>();

            if(r != null )
            {
                if(r.Properties != null)
                {
                    foreach(string key in r.Properties.Keys)
                    {
                        var propCollection = r.Properties.ByKey(key);
                        var propValueList = (from a in propCollection where a != null && a.Value != null select a.Value).ToList();
                        recordInfo.Add(key, propValueList);
                    }

                    foreach (var d in r.DimensionValues.Dimensions)
                    {
                        var dimValueList = new List<string>();
                        //TODO: may need to update this to handle multiple dimension values per record..
                        //  right now we only have one value foreach dimension in a record.
                        if (d.Name != null && r[d.Name] != null)
                            dimValueList.Add(r[d.Name]);

                        recordInfo.Add(d.Name, dimValueList);
                        
                        // = d.DisplayName;
                        //ReadOnlyCollection<IAttributeValue> propCollection = r.Properties.ByKey(key);
                        //List<string> propValueList = new List<string>();
                        //foreach (IAttributeValue a in propCollection)
                        //{
                        //    if (a != null && a.Value != null)
                        //        propValueList.Add(a.Value);
                        //}
                        //recordInfo.Add(key, propValueList);
                    }
                }
            }

            return recordInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="organizations"></param>
        /// <returns></returns>
        public static SearchResultsModel FromOrgContractList(List<RefOrganizationContract> organizations)
        {
            var srm = new SearchResultsModel();
            srm.Records = new List<SearchRecordModel>();

            foreach (var org in organizations)
            {
                var record = new SearchRecordModel();
                record.Fields = PopulateRecord(org);
                srm.Records.Add(record);
            }

            return srm;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static Dictionary<string, List<string>> PopulateRecord(RefOrganizationContract o)
        {
            var recordInfo = new Dictionary<string, List<string>>();

            //these are the properties that the website is currently expecting based on Endeca lookup
            //is there a better way to do this?
            recordInfo.Add("P_Primary_Key", new List<string>() { "School-" + o.OPECode + o.BranchCode });
            recordInfo.Add("P_School", new List<string>() { o.OrganizationName });
            recordInfo.Add("P_School_ID", new List<string>() { o.OPECode + o.BranchCode });
            recordInfo.Add("P_School_Type", new List<string>() { o.RefSALTSchoolTypeID.ToString() });
            recordInfo.Add("RecordType", new List<string>() { "School" });
            recordInfo.Add("OrganizationID", new List<string>() { o.RefOrganizationID.ToString() });

            return recordInfo;
        }

    }
}
