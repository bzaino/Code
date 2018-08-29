using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using ASA.Web.Services.Common;
using System.Linq;
using Common.Logging;

namespace ASA.Web.Services.LoanService.Proxy
{
    public static class ReferenceDataTableNames
    {
        public static string LOAN_STATUS = "RefLoanStatus";
        public static string LOAN_TYPE = "RefLoanType";
    }

    public static class ReferenceDataRetriever
    {
        private const string CLASSNAME = "ASA.Web.Services.LoanService.Proxy.ReferenceDataRetriever";
        static readonly ILog _log = LogManager.GetLogger(CLASSNAME);

        private static List<ReferenceDataTableMapping> _refDataMappings = new List<ReferenceDataTableMapping>();
        private static Object _objectForLock = new Object();
        private static Boolean _initialized = false;

        private static ReferenceDataTableMapping _LoanStatus = new ReferenceDataTableMapping(ReferenceDataTableNames.LOAN_STATUS, "LoanStatusid", "LoanStatusName");
        private static ReferenceDataTableMapping _LoanType = new ReferenceDataTableMapping(ReferenceDataTableNames.LOAN_TYPE, "LoanTypeid", "LoanTypeName");

        public static List<ReferenceDataTableMapping> RefDataMappings
        {
            get
            {
                if (!IsInitialized())
                    InitializeReferenceData();

                return _refDataMappings;
            }
        }

        private static bool IsInitialized()
        {
            return _initialized && _refDataMappings.Count >= 2; // where 2 is the number of ref tables that should get loaded.
        }

        private static void InitializeReferenceData()
        {
            _log.Debug("START InitializeReferenceData");
            _refDataMappings.Add(_LoanStatus);
            _refDataMappings.Add(_LoanType);
           
            if (Config.Testing)
                _initialized = MockReferenceDataRetriever.InitializeReferenceData(_refDataMappings, _initialized, _objectForLock);
            else
                _initialized = RefDataHelper.InitializeReferenceData(_refDataMappings, _initialized, _objectForLock);
            _log.Debug("END InitializeReferenceData");
        }

        public static string GetLoanType(string loanTypeId)
        {
            _log.Debug("START GetLoanType");
            return RefDataHelper.GetNameForId(ReferenceDataTableNames.LOAN_TYPE, loanTypeId, RefDataMappings);
        }

        public static string GetLoanStatusId(string loanStatus)
        {
            _log.Debug("START GetLoanStatusId");
            return RefDataHelper.GetIdForName(ReferenceDataTableNames.LOAN_STATUS, loanStatus, RefDataMappings);
        }

        public static string GetLoanTypeId(string loanType)
        {
            _log.Debug("START GetLoanTypeId");
            return RefDataHelper.GetIdForName(ReferenceDataTableNames.LOAN_TYPE, loanType, RefDataMappings);
        }

        public static string GetLoanStatus(string loanStatusId)
        {
            _log.Debug("START GetLoanStatus");
            return RefDataHelper.GetNameForId(ReferenceDataTableNames.LOAN_STATUS, loanStatusId, RefDataMappings);
        }

        public static Dictionary<string, string> GetLoanStatusMapping()
        {
            _log.Debug("START GetLoanStatusMapping");
            return RefDataHelper.GetReferenceDataTableMapping(ReferenceDataTableNames.LOAN_STATUS, RefDataMappings);
        }

        public static Dictionary<string, string> GetLoanTypeMapping()
        {
            _log.Debug("START GetLoanTypeMapping");
            return RefDataHelper.GetReferenceDataTableMapping(ReferenceDataTableNames.LOAN_TYPE, RefDataMappings);
        }
    }

}
