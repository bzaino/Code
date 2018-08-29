using System.IO;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Collections.Generic;
using ASA.Log.ServiceLogger;
using ASA.Web.Services.ASAMemberService;
using ASA.Web.Services.ASAMemberService.ServiceContracts;
using ASA.Web.Services.Common;
using ASA.Web.Services.Proxies;
using ASA.Web.Services.Proxies.SALTService;
using ASA.Web.Services.SelfReportedService.Proxy.DataContracts;
using ASA.Web.WTF.Integration;
using System;

namespace ASA.Web.Services.SelfReportedService
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SelfReported
    {
        /// <summary>
        /// The logger
        /// </summary>
        static readonly IASALog Log = ASALogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The self reported loan adapter
        /// </summary>
        //private readonly ISelfReportedAdapter _selfReportedAdapter = null;

        /// <summary>
        /// The member adapter
        /// </summary>
        private readonly IAsaMemberAdapter _memberAdapter = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelfReported"/> class.
        /// </summary>
        public SelfReported()
        {
            Log.Info("ASA.Web.Services.SelfReported.ServiceImplementation.SelfReported() object being created ...");
            
            //_selfReportedAdapter = new SelfReportedAdapter();
            _memberAdapter = new AsaMemberAdapter();
        }

        /// <summary>
        /// Gets the self reported loans.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "restLoans", ResponseFormat = WebMessageFormat.Json)]
        [AspNetCacheProfile("DoNotCache")]
        public SelfReportedLoanListModel GetSelfReportedLoans()
        {
            Log.Info("ASA.Web.Services.SelfReported.GetSelfReportedLoans() starting ...");
           
            var userId = _memberAdapter.GetMemberIdFromContext();
            var loanList = IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent").GetUserReportedLoans(userId);
           
            Log.Info("ASA.Web.Services.SelfReported.GetSelfReportedLoans() ending ...");
           
            return loanList.FromMemberReportedLoanContractList();
        }

        /// <summary>
        /// Gets the KWYO self reported loans.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "kwyoLoans", ResponseFormat = WebMessageFormat.Json)]
        [AspNetCacheProfile("DoNotCache")]
        public int GetKWYOSelfReportedLoans()
        {
            Log.Info("ASA.Web.Services.SelfReported.GetKWYOSelfReportedLoans() starting ...");

            var userId = _memberAdapter.GetMemberIdFromContext();

            var recordSourceList = new string[] { "Imported-KWYO", "Member-KWYO" };

            var loanList = IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent").GetUserReportedLoansRecordSourceList(userId, recordSourceList);

            Log.Info("ASA.Web.Services.SelfReported.GetKWYOSelfReportedLoans() ending ...");

            return loanList.Count;
        }

        /// <summary>
        /// Saves the name of the self reported loans imported from a NSLDS file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "SelfReportedUpload/{sourceName}", Method = "POST")]
        public SelfReportedLoanListModel SaveSelfReportedLoansWithFileName(string sourceName, Stream file)
        {
            var reader = new StreamReader(file);
            var fileBytes = Encoding.ASCII.GetBytes(reader.ReadToEnd());
            var userId = _memberAdapter.GetMemberIdFromContext();
            
            List<MemberReportedLoanContract> loanList;
            SelfReportedLoanListModel errorObj = new SelfReportedLoanListModel();

            try
            {
                loanList = SaltServiceAgent.ImportLoanFile(userId, fileBytes, sourceName);

                if (loanList.Count == 0)
                {
                    //return an error message so user knows that loans were not imported
                    errorObj.ErrorList.Add(new ErrorModel("No loans returned"));
                    return errorObj;
                }
                else
                {
                    return loanList.FromMemberReportedLoanContractList();
                }
            }
            catch (Exception ex)
            {
                //return error with message from SALT Service
                errorObj.Loans = new List<SelfReportedLoanModel>();
                errorObj.ErrorList.Add(new ErrorModel(ex.Message));
                return errorObj;
            }
        }

        /// <summary>
        /// Deletes a single loan.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="loanId">The loan id.</param>
        /// <returns>bool</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "restLoans/{id}", Method = "DELETE", ResponseFormat = WebMessageFormat.Json)]
        public bool DeleteLoan(string id)
        {
            int loanId = Convert.ToInt32(id);
            var userId = _memberAdapter.GetMemberIdFromContext();
            var serviceResponse = SaltServiceAgent.DeleteLoan(userId, loanId);
            if (!serviceResponse)
            {
                Log.Debug("End Method");
                throw new WebFaultException<string>("{Failed to delete" + "}", System.Net.HttpStatusCode.BadRequest);
            }
            return serviceResponse;
        }

        /// <summary>
        /// Creates a single loan.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="loanId">The loan id.</param>
        /// <returns>bool</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "restLoans", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public SelfReportedLoanModel CreateLoan(SelfReportedLoanModel loan)
        {
            if (!loan.IsValid())
            {
                throw new System.ComponentModel.DataAnnotations.ValidationException();
            }
            var loanContract = loan.ToMemberReportedLoanContract();
            var userId = _memberAdapter.GetMemberIdFromContext();
            var serviceResponse = SaltServiceAgent.CreateLoan(userId, loanContract).FromMemberReportedLoanContract();
            return serviceResponse;
        }

        /// <summary>
        /// Updates a single loan.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="loan">The loan.</param>
        /// <returns>Loan</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "restLoans/{id}", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public SelfReportedLoanModel UpdateLoan(string id, SelfReportedLoanModel loan)
        {
            if (!loan.IsValid())
            {
                throw new System.ComponentModel.DataAnnotations.ValidationException();
            }
            var loanContract = loan.ToMemberReportedLoanContract();
            int loanId = Convert.ToInt32(id);
            var userId = _memberAdapter.GetMemberIdFromContext();
            var serviceResponse = SaltServiceAgent.UpdateLoan(userId, loanContract).FromMemberReportedLoanContract();
            return serviceResponse;
        }

    }
}
