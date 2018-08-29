using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.AlertService.DataContracts;


namespace ASA.Web.Services.AlertService.Validation
{
    public class AlertValidation
    {
        public static bool ValidateAlertId(string id)
        {
            bool bValid = false;
            AlertModel alert = new AlertModel();
            alert.ID = id;
            if (id != null && alert.IsValid("ID"))
            {
                bValid = true; 
            }

            return bValid;
        }
    }
}
