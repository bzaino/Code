using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.Services.LoanService.Proxy
{
    public static class MockReferenceDataRetriever
    {
        public static bool InitializeReferenceData(List<Common.ReferenceDataTableMapping> _refDataMappings, bool _initialized, object _objectForLock)
        {
            lock (_objectForLock)
            {
                if (!_initialized)
                {
                    _refDataMappings[0].Mapping = MockReferenceDataObjects.LOAN_STATUSES;
                    _refDataMappings[1].Mapping = MockReferenceDataObjects.LOAN_TYPES;

                    _initialized = true;
                }
            }

            return _initialized;
        }
    }

    static class MockReferenceDataObjects
    {
        //============================================================================================
        //LOAN TYPES:
        public static Dictionary<string, string> LOAN_TYPES= new Dictionary<string, string > 
        {      
	        {"CL", "Consolidation"},
	        {"D1", "DL Subsidized Stafford Subsidized"},
	        {"D2", "DL Unsubsidized Stafford Subsidized"},
	        {"D3", "DL Grad PLUS"},
	        {"D4", "DL Parent Plus"},
	        {"D5", "DL Consolidation"},
	        {"GB", "Graduate PLUS"},
	        {"HL", "HEAL"},
	        {"IN", "Institution"},
	        {"PC", "Private Consolidation"},
	        {"PL", "PLUS"},
	        {"PR", "Private"},
	        {"PU", "Perkins"},
	        {"SF", "Stafford Subsidized"},
	        {"SL", "SLS"},
	        {"ST", "State"},
	        {"SU", "Stafford Unsubsidized"},
	        {"SX", "Stafford"}
        };
		

        //============================================================================================
        //LOAN STATUS:
        public static Dictionary<string, string> LOAN_STATUSES= new Dictionary<string, string > 
        {      
	        {"AE", "Loan Transferred"},
	        {"AL", "Abandoned Loan"},
	        {"BC", "Bankruptcy Claim, Discharged"},
	        {"BK", "Bankruptcy Claim, Active"},
	        {"CA", "Canceled"},
	        {"CS", "Closed School Discharge"},
	        {"DA", "Deferred"},
	        {"DB", "Defaulted, Then Bankrupt, Active, Ch. 13"},
	        {"DC", "Defaulted, Compromise"},
	        {"DD", "Defaulted, Then Died"},
	        {"DE", "Death"},
	        {"DF", "Defaulted, Unresolved"},
	        {"DI", "Disability"},
	        {"DK", "Defaulted, Then Bankrupt, Discharged, Ch. 13"},
	        {"DL", "Defaulted, In Litigation"},
	        {"DN", "Defaulted, Then Paid in Full by Consolidation"},
	        {"DO", "Defaulted, Then Bankrupt, Active, Other"},
	        {"DP", "Defaulted, Paid in Full"},
	        {"DR", "Defaulted, Loan Included In Roll-up"},
	        {"DS", "Defaulted, Then Disabled"},
	        {"DT", "Defaulted, Collection Terminated"},
	        {"DU", "Defaulted, Unresolved Pre-2002"},
	        {"DW", "Defaulted, Write-Off"},
	        {"DX", "Defaulted, Six Consecutive Payments"},
	        {"DZ", "Defaulted, Six Consecutive Payments, Then Missed Payments"},
	        {"FB", "Forbearance"},
	        {"FC", "False Certification Discharge"},
	        {"FR", "Fraud"},
	        {"FX", "Fraud Satisfied"},
	        {"IA", "Loan Originated"},
	        {"ID", "In School or Grace Period"},
	        {"IG", "In Grace Period"},
	        {"IM", "In Military Grace"},
	        {"OD", "Defaulted, Then Bankrupt, Discharged, Other"},
	        {"PC", "Paid in Full Through Consolidation Loan"},
	        {"PF", "Paid in Full"},
	        {"PM", "Presumed Paid in Full"},
	        {"PN", "Non-defaulted, Paid in Full Through Consolidation Loan"},
	        {"PZ", "PLUS Child Death"},
	        {"RF", "Refinanced"},
	        {"RP", "In Repayment"},
	        {"UA", "Temporarily Uninsured, No Default Claim Requested"},
	        {"UB", "Temporarily Uninsured, Default Claim Denied"},
	        {"UC", "Permanently Uninsured/Unreinsured, No Default Claim Request"},
	        {"UD", "Permanently Uninsured/Unreinsured - loan in default"},
	        {"UI", "Uninsured/Unreinsured"},
	        {"XD", "Defaulted, Six Monthly Payments"}
        };
    }

}
