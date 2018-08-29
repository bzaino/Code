using System;
using ASA.Web.Services.AppToolService.Proxy.DataContracts;

namespace ASA.Web.Services.AppToolService.Validation
{
    public class AppToolValidation
    {
        public static bool ValidateSearchId(string id)
        {
            bool bValid = false;

            if (id != null && id.Length >0)
            {
                int personId;
                bValid = Int32.TryParse(id, out personId);
            }

            return bValid;
        }

        public static bool ValidateInputAppTool(AppToolModel atModel)
        {
            if (atModel == null)
                return false;
            return atModel.IsValid();
        }
    }
}
