using System;
using System.Text.RegularExpressions;

using ASA.Web.Services.Common;

namespace ASA.Web.Services.SearchService.DataContracts
{
    public class SearchQueryModel : BaseWebModel
    {
        // Exmaple request: "?Ntk=ContentSearch&Ntx=mode+matchany&Ntt=test&No=undefined"
        //
        // The following are removed: !@#^*()-[]\{}|/<>
        // The following is removed when encoded (entered by user, not from uri): %
        //
        // Length is 116 because: 62 for actual query and then 54 for request formatting.
        public string query { get; set; }

        public SearchQueryModel(string input, string requestUrl)
        {
            query = input;

            // Shorten if needed.
            if (query.Length > 2000)
            {                  
                // Acceptable chars to end on, prevents cutting off symbol encoding,
                // which would output a broken symbol. (Ex. %20 (or " ") to %2 ("%2")
                char[] accept = {'a', 'A', 'b', 'B', 'c', 'C', 'd', 'D', 'e', 'E', 'f', 'F', 
                                 'g', 'G', 'h', 'H', 'i', 'I', 'j', 'J', 'k', 'K', 'l', 'L', 
                                 'm', 'M', 'n', 'N', 'o', 'O', 'p', 'P', 'q', 'Q', 'r', 'R', 
                                 's', 'S', 't', 'T', 'u', 'U', 'v', 'V', 'w', 'W', 'x', 'X', 
                                 'y', 'Y', 'z', 'Z'};

                // Shorten the query, with room for "..."
                query = query.Substring(0, 301);
                // Make sure ending on acceptable char
                query = query.Substring(0, query.LastIndexOfAny(accept) + 1);

                // Determine if query has quotation marks, 
                // add second one to end if needed.
                if (query.IndexOf("%22") != -1)
                {
                    query += "...%22";
                }
                else
                {
                    query += "...";
                }
            }

            // Remove encoded chars. (<, >, ^, \)
            query = query.Replace("%3C", "").Replace("%3E", "").Replace("%5E", "").Replace("%5C", "");
            //Remove other chars that are NOT in below regex string.
            query = Regex.Replace(query, @"[^\w\s \%\&\?\+\=\-\'\,\.\;\:\(\)]", "");

            // If now a blank query and not predictiveSearch, add default search string to give 0 results.
            if (String.IsNullOrEmpty(query) && !requestUrl.EndsWith("predictiveSearch"))
            {
                query = "?Ntk=ContentSearch&Ntx=mode+matchany&Ntt=%20&No=undefined";
            }
        }
    }
}
