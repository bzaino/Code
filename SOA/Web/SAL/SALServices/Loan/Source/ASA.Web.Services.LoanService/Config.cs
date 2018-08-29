using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace ASA.Web.Services.LoanService
{
    class Config
    {

        // This value determines the maximum number of Loans the EDMS LoanService will attempt to return
        //<add key ="LoanServiceMaxEntities" value ="10" />
        private static int _LoanServiceMaxEntities = 10;
        public static int LoanServiceMaxEntities
        {
            get
            {
                try
                {
                    _LoanServiceMaxEntities = Int32.Parse(ConfigurationManager.AppSettings["LoanServiceMaxEntities"].ToString());
                    return _LoanServiceMaxEntities;
                }
                catch
                {
                    return _LoanServiceMaxEntities;
                }
            }
        }

    }
}
