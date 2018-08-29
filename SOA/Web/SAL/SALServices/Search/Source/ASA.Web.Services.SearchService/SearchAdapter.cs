using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ASA.Web.Services.Common;
using ASA.Web.Services.Proxies;
using ASA.Web.Services.SearchService.DataContracts;
using ASA.Web.Services.SearchService.ServiceContracts;
using ASA.Web.WTF.Integration;

using Member = ASA.Web.Services.ASAMemberService.DataContracts;

namespace ASA.Web.Services.SearchService
{
    public class SearchAdapter : ISearchAdapter
    {
        private const string CLASSNAME = "ASA.Web.Services.SearchService.SearchAdapter";
        private static ASA.Log.ServiceLogger.IASALog _log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(CLASSNAME); 

        public SearchAdapter()
        {
            _log.Debug("START SearchAdapter");

            _log.Debug("END SearchAdapter");
        }

        #region GetOrganizations
        public SearchResultsModel GetOrganizations(string filter)
        {
            return new SearchResultsModel();
        }

        public IList<OrganizationProductModel> GetOrganizationProducts(int orgId)
        {
            var productList = IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent").GetOrganizationProducts(orgId);

            IList<OrganizationProductModel> returnList = productList.Select(product => product.ToOrganizationProductModel()).ToList();

            return returnList;
        }

        public bool IsProductActive(IList<Member.MemberOrganizationModel> memberOrganizations, int productId)
        {
            bool bReturn = false;

            OrganizationProductModel product = new OrganizationProductModel();
            var organizations = memberOrganizations.Where(mo => mo.EffectiveEndDate == null).ToList();

            foreach (Member.MemberOrganizationModel mom in organizations)
            {
                IList<OrganizationProductModel> productList = GetOrganizationProducts(mom.OrganizationId);

                product = productList.FirstOrDefault(p => p.ProductID == productId);

                if (product != null && product.IsOrgProductActive)
                {
                    bReturn = true;
                    break;
                }
            }

            return bReturn;
        }

        #endregion

        #region Search Widget Searches
        public void BunchballAction(string param)
        {
            var foo = param;
            return;
        }

        #endregion

        #region Input param reformatting
        private string getRecordFilter(string contentTypeFilter, string sizeFilter, string schoolFilter)
        {
            string recordFilter = "";
            //AND(OR(P_Content_Type:Financial_education_content_type,P_Content_Type:Resource_content_type),P_School_Name:Rivier_College_school_name,P_Content_Size:260_x_180_content_size,RecordType:Tile)
            if (!string.IsNullOrEmpty(contentTypeFilter))
                recordFilter = contentTypeFilter;

            if (!string.IsNullOrEmpty(schoolFilter))
            {
                if (!string.IsNullOrEmpty(contentTypeFilter)) //check if previous filter was null/empty.  if not we need a comma.
                    recordFilter += ",";
                recordFilter += schoolFilter;
            }

            if (!string.IsNullOrEmpty(sizeFilter))
            {
                if (!string.IsNullOrEmpty(contentTypeFilter) || 
                    !string.IsNullOrEmpty(schoolFilter)) //check if either previous filter was null/empty.  if not we need a comma.
                    recordFilter += ",";
                recordFilter += sizeFilter;
            }

            if (!string.IsNullOrEmpty(recordFilter))
            {
                recordFilter = "AND(" + recordFilter + ",RecordType:Tile)";
            }
            
            return recordFilter;
        }

        private string getContentTypeFilter(string[] contentTypes)
        {
            if (contentTypes == null)
                contentTypes = new string[] { "" };

            StringBuilder contentTypeFilter = new StringBuilder("");

            // handle changing passed-in values into Endeca-appropriate values. 
            //  For example: ("MemBen" --> "MemBen_content_type") and ("240 x 180" --> "240_x_180_content_size")
            if (contentTypes != null && contentTypes.Length > 0)
            {
                contentTypeFilter.Append("OR(");
                for (int i = 0; i < contentTypes.Length; i++)
                {
                    if (contentTypes[i].Contains(' '))
                        contentTypes[i] = contentTypes[i].Replace(' ', '_');
                    contentTypeFilter.Append("P_Content_Type:");
                    contentTypeFilter.Append(contentTypes[i]);
                    contentTypeFilter.Append("_content_type");
                    if (i < contentTypes.Length - 1)
                        contentTypeFilter.Append(",");
                }
                contentTypeFilter.Append(")");
            }

            return contentTypeFilter.ToString();
        }

        private string getSizeFilter(string size)
        {
            if (!string.IsNullOrEmpty(size))
            {
                size = size.Replace(' ', '_');
                size += "_content_size";
                size = "P_Content_Size:" + size;
            }
            else
                size = "";

            return size;
        }

        private string getSchoolFilter(string school)
        {
            if (!string.IsNullOrEmpty(school))
            {
                school = school.Replace(' ', '_');
                school += "_school_name";
                school = "P_School_Name:" + school;
            }
            else
                school = "";

            return school;
        }
        #endregion      


        #region private

        private SearchResultsModel randomizeResults(SearchResultsModel srm)
        {
            SearchResultsModel randomizedResults = new SearchResultsModel();
            bool stillWorking = true;
            Random r = new Random();
            while (stillWorking)
            {
                if (srm.Records.Count > 0)
                {
                    //get random index into search results
                    int iRandom = r.Next(0, srm.Records.Count);

                    //add that record to the randomized results
                    randomizedResults.Records.Add(srm.Records[iRandom]);

                    //remove that record from the non-randomized results
                    srm.Records.RemoveAt(iRandom);
                }
                else
                    stillWorking = false;
            }

            return randomizedResults;
        }

        private static SearchResultsModel handleNoResults(string input)
        {
            if (input != null)
                _log.Warn("search results were null: " + input);
            else
                _log.Warn("search results were null and input was null");

            SearchResultsModel results = new SearchResultsModel();
            results.ErrorList.Add(new ErrorModel("There was a problem performing the search"));
            return results;
        }
        
        #endregion

        public SearchResultsModel DoSearch(string[] contentTypes, string size, string school, string role, string grade, string enrollStatus)
        {
            return new SearchResultsModel();
        }
    }
}
