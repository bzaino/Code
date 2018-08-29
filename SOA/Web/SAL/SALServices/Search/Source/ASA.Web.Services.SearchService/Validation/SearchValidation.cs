using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ASA.Web.Services.SearchService.Validation
{
    public class SearchValidation
    {
        public static bool ValidateSearchFilter(string filter)
        {
            //TODO: anything we actually need to validate here???
            bool bValid = !(filter == null || filter.Length > 250);
            return bValid;
        }

        public static bool ValidateAdTilesInput(string contentTypes, string size, string layout)
        {
            bool bValid = true;
            if (contentTypes == null || size == null || layout == null)
                bValid = false;
            else if (contentTypes == string.Empty || size == string.Empty || layout == string.Empty)
                bValid = false;
            else if (contentTypes.Length > 250 || size.Length > 250 || layout.Length > 250)
                bValid = false;

            return bValid;
        }
    }

}