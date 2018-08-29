using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using ASA.Log.ServiceLogger;
using ASA.Web.Services.Common;
using ASA.Web.Services.LoanService.Proxy.DataContracts;
using ASA.Web.Services.LoanService.ServiceContracts;
using ASA.Web.Services.LoanService.Validation;
using ASA.Web.Services.SelfReportedService.Proxy.DataContracts;
using Common.Logging;
using ASA.Web.Services.ASAMemberService.ServiceContracts;
using ASA.Web.Services.ASAMemberService;
using ASA.Web.Services.ASAMemberService.DataContracts;

namespace ASA.Web.Services.LoanService
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Loan //: ILoan  //NOTE: For some reason inheriting interface here didn't work when hosting service over JSON in web app...
    {
        private const string CLASSNAME = "ASA.Web.Services.LoanService.Loan";
        static readonly ILog _log = LogManager.GetLogger(CLASSNAME);

        private const string _loanAdapterExceptionMessage = "Unable to create a LoanAdapter object from the ASA.Web.Services.LoanService library. ";
        private ILoanAdapter _loanAdapter = null;

        public Loan()
        {
            _log.Info("ASA.Web.Services.LoanService.Loan() object being created ...");
            if (ASA.Web.Services.Common.Config.Testing)
                _loanAdapter = new MockLoanAdapter();
            else
                _loanAdapter = new LoanAdapter();
        }

        [OperationContract]
        [WebInvoke(UriTemplate = "GetLoans", Method = "POST")]
        public SelfReportedLoanListModel GetLoans(string ssn)
        {
            _log.Debug(string.Format("START ASA.Web.Services.LoanService.GetLoan(): ssn={0}", !string.IsNullOrEmpty(ssn)?ssn:"null"));
            HttpHeadersHelper.SetNoCacheResponseHeaders(WebOperationContext.Current);

            SelfReportedLoanListModel loans = null;
            IAsaMemberAdapter memberAdapter = null;
            memberAdapter = new AsaMemberAdapter();

            if (LoanValidation.ValidateInputSsn(ssn))
            {
                _log.Debug("calling GetActiveDirectoryKeyFromContext now.");
                int? id = memberAdapter.GetMemberIdFromContext();
                ASAMemberModel member = memberAdapter.GetMember(id.Value);

                if (_loanAdapter == null)
                {
                    _log.Error(_loanAdapterExceptionMessage);
                    loans = new SelfReportedLoanListModel();
                    ErrorModel error = new ErrorModel(_loanAdapterExceptionMessage, "Web Loan Service");
                    _log.Error("ASA.Web.Services.LoanService.GetLoan(): " + _loanAdapterExceptionMessage);
                    loans.ErrorList.Add(error);
                }
                else if(member!=null)// we should never try to retrieve loans for someone who isn't found as the logged-in member from context.
                    loans = _loanAdapter.GetLoans(ssn, member);

                if (loans == null)
                {
                    _log.Debug("No loans were retrieved for this SSN: " + ssn);
                    loans = new SelfReportedLoanListModel();
                    loans.ErrorList.Add(new ErrorModel("No Loans were retrieved for this SSN."));
                }
            }
            else
            {
                loans = new SelfReportedLoanListModel();
                ErrorModel error = new ErrorModel("Invalid search criteria", "Web Loan Service");
                _log.Warn("ASA.Web.Services.LoanService.GetLoan(): Invalid search criteria");
                loans.ErrorList.Add(error);
            }

            _log.Debug(string.Format("END ASA.Web.Services.LoanService.GetLoan(): ssn={0}", !string.IsNullOrEmpty(ssn) ? ssn : "null"));
            return loans;
        }

        [OperationContract]
        [WebGet(UriTemplate = "LoanStatusMapping", ResponseFormat = WebMessageFormat.Json)]
        [AspNetCacheProfile("CacheForLongTime")]
        public Dictionary<string, string> GetLoanStatusMapping()
        {
            _log.Debug("ASA.Web.Services.LoanService.GetLoanStatusMapping() ...");
            return Proxy.ReferenceDataRetriever.GetLoanStatusMapping();
        }

        [OperationContract]
        [WebGet(UriTemplate = "LoanTypeMapping", ResponseFormat = WebMessageFormat.Json)]
        [AspNetCacheProfile("CacheForLongTime")]
        public Dictionary<string, string> GetLoanTypeMapping()
        {
            _log.Debug("ASA.Web.Services.LoanService.GetLoanTypeMapping() ...");
            return Proxy.ReferenceDataRetriever.GetLoanTypeMapping();
        }  
    }
}
