using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using ASA.Web.Services.ASAMemberService;
using ASA.Web.Services.ASAMemberService.ServiceContracts;
using ASA.Web.Services.SearchService.ServiceContracts;

using MemberCourseModel = ASA.Web.Services.ASAMemberService.DataContracts.MemberCourseModel;
using MemberOrganizationModel = ASA.Web.Services.ASAMemberService.DataContracts.MemberOrganizationModel;
using OrganizationProductModel = ASA.Web.Services.ASAMemberService.DataContracts.OrganizationProductModel;

namespace ASA.Web.Services.SearchService
{
    public class EndecaUtility
    {

        public ISearchAdapter _searchadapter;
        public IAsaMemberAdapter _memberAdapter;

        public EndecaUtility()
        {
            _memberAdapter = new AsaMemberAdapter();
            _searchadapter = new SearchAdapter();
        }
        public EndecaUtility(ISearchAdapter searchAdapter, IAsaMemberAdapter memberAdapter)
        {
            _memberAdapter = memberAdapter;
            _searchadapter = searchAdapter;
        }

        public string CombineEndecaFilters(string target, string toAdd) 
        {
            /*  The params will look something like the following:
             
                    target:  AND(NOT(P_Primary_Key:101-18261),NOT(P_Primary_Key:101-7416),NOT(P_Primary_Key:101-23826))
                    addOn:   AND(NOT(Tags:payment%20plans),NOT(Tags:postponements),NOT(Tags:cancellations),NOT(Tags:late%20payments))

                We will cut the last character ")" off the target string, and cut the first 4 characters "AND(" from the addOn string
                Then we combine them, making sure they are separated by a comma, and add the trailing parenthasees at the end
            */
            if (string.IsNullOrEmpty(target))
            {
                target = toAdd;
            }
            else
            {
                target = target.Substring(0, target.Length - 1);

                //The addOn param might have a single filter
                //In this case there will be no "AND()" wrapping the filters, and we can skip the replacing and add the addOn to the target variable
                if (toAdd.IndexOf("AND(") > -1)
                {
                    toAdd = toAdd.Replace("AND(", "");
                    toAdd = toAdd.Substring(0, toAdd.Length - 1);
                }
                target += "," + toAdd + ")";
            }

            return target;
        }

        public string updateFilterParameter(string search, int memberId, bool isSearchPage)
        {
            bool includeCourses = false;
            if (memberId > 0)
            {
                IList<MemberOrganizationModel> organizations = _memberAdapter.GetMemberOrganizations(memberId);
                includeCourses = _searchadapter.IsProductActive(organizations, 2);
            }

            string hideRecordString = FormatHideRecordStringWithCourses(memberId, search, includeCourses);

            string filterParam = GetParameterByNameFromString("Nr", search);
            search = UpdateQueryString(search, "Nrs", BuildContentDimensionSearchString(GetParameterByNameFromString("Dims", search), isSearchPage, includeCourses));
            if (!String.IsNullOrEmpty(hideRecordString))
            {
                if (!String.IsNullOrEmpty(filterParam))
                {
                    //If we have both a HideRecord and Nr param, we need to combine the strings, as they will both be sent with the Nr param
                    filterParam = CombineEndecaFilters(filterParam, hideRecordString);
                }
                else
                {
                    //We need to make sure the values in HideRecord are sent with the "Nr" param when the filterParam variable is empty (No "Nr" was passed in the url)
                    filterParam = hideRecordString;
                }
                search = UpdateQueryString(search, "Nr", filterParam);
            }

            if (memberId != 0)
            {
                /*Check if it needs to overwrite the Ns param which was populated by default*/
                if (search.Contains("Ns=BR"))
                {
                    search = UpdateQueryString(search, "Ns", _memberAdapter.BuildOutEndecaBoost(memberId));
                }
            }
            /*clean up fake params that no longer needed for api url*/
            search = RemoveQueryString(search, new string[] { "Dims", "Type", "HideRecord" });
            return search;
        }

        public string BuildContentDimensionSearchString(string commaSepratedDimension, bool isSearchPage, bool includeCourses=false)
        {
            if (string.IsNullOrEmpty(commaSepratedDimension))
            {
                string dimString = "collection()/record[ContentTypes=endeca:dval-by-id(41)//id or ContentTypes=endeca:dval-by-id(42)//id or ContentTypes=endeca:dval-by-id(43)//id or ContentTypes=endeca:dval-by-id(44)//id or ContentTypes=endeca:dval-by-id(45)//id or ContentTypes=endeca:dval-by-id(46)//id or ContentTypes=endeca:dval-by-id(157)//id or ContentTypes=endeca:dval-by-id(303)//id";
                //Definintions should be included in search results per SWD-6296
                if (isSearchPage)
                {
                    dimString += " or ContentTypes=endeca:dval-by-id(159)//id";
                }

                if (includeCourses)
                {
                    dimString += " or ContentTypes=endeca:dval-by-id(304)//id";
                }
                
                dimString += "]";

                return dimString;
            }
            string[] dimensionArray = commaSepratedDimension.Split(new string[] { "," }, StringSplitOptions.None);
            string dimensionsStartString = "collection()/record[";
            string stringEnding = "]";
            // if spanish is one of the dimensions, add it to the beginning of the dimension string
            if (dimensionArray.Contains("104"))
            {
                dimensionsStartString += "Language=endeca:dval-by-id(104)//id and (";
                stringEnding = ")]";
            }
            List<String> contentTypeSnippets = new List<String>();
            foreach (string dim in dimensionArray)
            {
                if (dim != "104")
                {
                    contentTypeSnippets.Add("ContentTypes=endeca:dval-by-id(" + dim + ")//id");
                }
            }
            string dimensionsMiddleString = string.Join(" or ", contentTypeSnippets);
            return dimensionsStartString + dimensionsMiddleString + stringEnding;
        }
        public string UpdateQueryString(string queryString, string paramName, string paramVal)
        {
            var updatedQueryString = HttpUtility.ParseQueryString(queryString);
            updatedQueryString.Set(paramName, paramVal);
            return "?" + updatedQueryString.ToString();
        }
        public string RemoveQueryString(string queryString, string[] paramNameArray)
        {
            var updatedQueryString = HttpUtility.ParseQueryString(queryString);
            foreach (string paramName in paramNameArray)
            {
                updatedQueryString.Remove(paramName);
            }
            return "?" + updatedQueryString.ToString();
        }
        public string GetParameterByNameFromString(string paramName, string queryString)
        {
            string toReturn = "";
            var httpQueryString = HttpUtility.ParseQueryString(queryString);
            if (!string.IsNullOrEmpty(httpQueryString.Get(paramName)))
            {
                toReturn = httpQueryString.Get(paramName);
            }
            return toReturn;
        }

        /// <summary>
        /// Turns something like: ["101-12345", "101-56802", "101-58742", "101-98752"] into:
        /// AND(NOT(P_Primary_Key:101-12345),NOT(P_Primary_Key:101-56802),NOT(P_Primary_Key:101-58742),NOT(P_Primary_Key:101-98752))
        /// which is the format endeca expects for the "Nr" parameter
        /// </summary>
        /// <param name="list">list of contentId to exclude</param>
        /// <returns></returns>
        public string BuildPrimaryKeyExcludeFilter(List<string> list)
        {
            //Add "NOT(P_Primary_Key:" ")" around each tag value
            IEnumerable<string> decoratedTags = list.Select(m => m.Replace(m, "NOT(P_Primary_Key:" + m + ")"));
            //Join the decorated tags with a comma and wrap them in the "AND()" combiner
            return "AND(" + String.Join(",", decoratedTags) + ")";
        }

        /// <summary>
        /// Adds courses to be hidden/excluded to the HideRecord parameter based on the members orgainzations participation the Course.
        /// </summary>
        /// <param name="organizations">a list interface of the Member's organizations</param>
        /// <param name="search">the search url containing the HideRecord parameter</param>
        /// <param name="includeCourses">signals that courses are to be included, so need to build out hide/exclusion string</param>
        /// <param name="testMode">signals that call is from unitTest</param>
        /// <returns>hideRecords parameter with courses to be hidden added onto it</returns>
        public string FormatHideRecordStringWithCourses(int memberId, string search, bool includeCourses=false, bool testMode=false )
        {
            string hideRecords = GetParameterByNameFromString("HideRecord", search);

            if (includeCourses == true && memberId > 0)
            {
                //list of the members organizations that participate in courses.
                IList<OrganizationProductModel> organizationProductsList = _memberAdapter.GetMemberOrganizationProducts(memberId).Where(p => p.ProductTypeID == 1).ToList();

                List<MemberCourseModel> moodleCourseList = null;
                if (testMode)
                {
                    //stub out the return model from SaltCouse/IDP for testing
                    moodleCourseList = new List<MemberCourseModel> { 
                        new MemberCourseModel { IdNumber = "Budgeting", ContentID = "101-25847" },
                        new MemberCourseModel { IdNumber = "Money", ContentID = "101-44444" }, 
                        new MemberCourseModel { IdNumber = "Credit and Debt Management", ContentID = "101-26024" }, 
                        new MemberCourseModel { IdNumber = "Taxes", ContentID = "101-55555" } 
                    };
                }
                else
                {
                    //need to get the list of courses. This will contain the ContentId.
                    moodleCourseList = _memberAdapter.GetSaltCoursesFromWebConfig();
                }

                //This list will contain just the ContentId's that need to excluded from the endeca return results
                List<MemberCourseModel> contentIdsToExclude = moodleCourseList.Where(c => !organizationProductsList.Any(op => op.ProductName == c.IdNumber)).ToList();

                //Only need to be done if there are any course to exclude
                if (contentIdsToExclude.Count > 0)
                {                    
                    string formattedString = BuildPrimaryKeyExcludeFilter(contentIdsToExclude.Select(c => c.ContentID).ToList());

                    if (hideRecords.Contains("NOT(ContentTypes:Definition)"))
                    {
                        //add "AND(" and ")" to string so combination routine works correctly
                        hideRecords = "AND(" + hideRecords + ")";
                    }
                    hideRecords = CombineEndecaFilters(hideRecords, formattedString);
                }
            }
            return hideRecords;
        }
    }
}
