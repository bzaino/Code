using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.Common;
using ASA.Web.Services.LessonsService.DataContracts;

namespace ASA.Web.Services.LessonsService.Validation
{
    public class LessonsValidation: BaseWebModel
    {
        public static bool ValidateUserId(int userId)
        {
            bool bValid = false;
            //Id validation goes here
	
	        return bValid;
        }

        public static bool ValidateIndividualId(Guid individualId)
        {
            bool bValid = false;
            //individualId validation goes here

            return bValid;

        }

        public static bool ValidateUser(User user)
        {
            bool bValid = false;
            //User validation goes here

            return bValid;
        }

    }
}
