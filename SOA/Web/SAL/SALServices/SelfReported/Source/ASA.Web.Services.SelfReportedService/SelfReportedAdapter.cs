using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using ASA.Log.ServiceLogger;
using ASA.Web.Services.SelfReportedService.Proxy.DataContracts;
using ASA.Web.Services.SelfReportedService.Proxy;
using ASA.Web.Services.SelfReportedService.ServiceContracts;
using ASA.Web.Services.Common;
using System.Reflection;
using System.Xml;
using ASA.Web.WTF;
using ASA.Web.WTF.Integration;

namespace ASA.Web.Services.SelfReportedService
{
    public class SelfReportedAdapter : ISelfReportedAdapter
    {
        //Get instance of Service Logger
        public IASALog _log = ASALogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public SelfReportedAdapter()
        {
            _log.Debug("ASA.Web.Services.SelfReportedService.SelfReportedAdapter() object being created ...");
        }


        public SelfReportedLoanListModel GetSelfReportedLoans(string individualId)
        {
            //xWebWrapper xWeb = new xWebWrapper();
            //XmlNode node = xWeb.GetQuery(
            //    "ASASelfReportedLoan", 
            //    xWebModelHelper.ConstructColumnListForFields(xWebSelfReportedLoanHelper.GetFieldNames()), 
            //    xWebSelfReportedLoanHelper.IndividualKeyName + "='" + individualId + "' and " + xWebSelfReportedLoanHelper.ActiveFlagName + "=1", 
            //    "");

            //SelfReportedLoanListModel srList = TranslateSelfReportedModel.MapXmlNodeToSelfReportedLoanList(node);
            //return srList;
            return new SelfReportedLoanListModel();
        }

        public ResultCodeModel SaveSelfReportedLoan(SelfReportedLoanModel srl)
        {
            ResultCodeModel result = new ResultCodeModel();
            
            if (srl != null)
            {
                if (!IsNewSRL(srl)) //QC 4712: check that SRL belongs to person logged in
                {
                    if (IsSRLForPersonLoggedIn(srl.LoanSelfReportedEntryId))
                    result = this.UpdateSelfReportedLoan(srl); //UpdateFacadeObject
                else
                    {
                        result.ErrorList.Add(new ErrorModel("There was a problem saving Self Reported Loan information."));
                    }
                }
                else
                    result = this.InsertSelfReportedLoan(srl); //InsertFacadeObject
            }
            return result;
        }

        public ResultCodeModel SaveSelfReportedLoans(SelfReportedLoanListModel srList)
        {
            ResultCodeModel insertResult = new ResultCodeModel();insertResult.ResultCode = 1;
            ResultCodeModel updateResult = new ResultCodeModel();updateResult.ResultCode = 1;
            SelfReportedLoanListModel srInsertList = new SelfReportedLoanListModel();
            SelfReportedLoanListModel srUpdateList = new SelfReportedLoanListModel();

            //QC 4712: do not save any SRLs that do not belong to the person logged-in.  
            srList = RemoveInvalidLoansFromList(srList);

            LogSRLData("after RemoveInvalidLoansFromList(srLoans) call", srList);

            foreach (SelfReportedLoanModel srl in srList.Loans)
            {
                if (IsNewSRL(srl))
                    srInsertList.Loans.Add(srl);
                else
                    srUpdateList.Loans.Add(srl);
            }

            if(srInsertList.Loans.Count >0)
                insertResult = InsertSelfReportedLoans(srInsertList);
            if(srUpdateList.Loans.Count >0)
                updateResult = UpdateSelfReportedLoans(srUpdateList);

            ResultCodeModel result = getCombinedResults(insertResult, updateResult, srInsertList.Loans.Count, srUpdateList.Loans.Count);
            return result;
        }

        public void LogSRLData(string p, SelfReportedLoanListModel srList)
        {
            if (_log.IsDebugEnabled)
            {
                try
                {
                    StringBuilder sb = new StringBuilder("\n" + p + "\n");
                    if (srList != null && srList.Loans != null)
                    {
                        foreach (SelfReportedLoanModel loan in srList.Loans)
                        {
                            sb.AppendLine("loan.IndividualId = " + (!string.IsNullOrEmpty(loan.IndividualId) ? loan.IndividualId : "NULL or EMPTY"));
                            sb.AppendLine("loan.LoanSelfReportedEntryId = " + (!string.IsNullOrEmpty(loan.LoanSelfReportedEntryId) ? loan.LoanSelfReportedEntryId : "NULL or EMPTY"));
                            sb.AppendLine("loan.IsActive = " + loan.IsActive);
                            sb.AppendLine("loan.LoanStatusId = " + (!string.IsNullOrEmpty(loan.LoanStatusId) ? loan.LoanStatusId : "NULL or EMPTY"));
                            sb.AppendLine("loan.NewRecord = " + loan.NewRecord);
                            sb.AppendLine("loan.OriginalLoanAmount = " + loan.OriginalLoanAmount.ToString());
                            sb.AppendLine("loan.PrincipalBalanceOutstandingAmount = " + loan.PrincipalBalanceOutstandingAmount.ToString());
                            sb.AppendLine("loan.LoanTerm = " + loan.LoanTerm.ToString());
                        }
                    }
                    _log.Debug(sb.ToString());
                }
                catch
                {
                    _log.Debug("Problem logging SelfReportedLoan data.  msg = " + p);
                }
            }
            
        }

        private SelfReportedLoanListModel RemoveInvalidLoansFromList(SelfReportedLoanListModel srList)
        {
            //get Id of the member currently logged-in
            SiteMember sm = IntegrationLoader.LoadDependency<ISiteMembership>("siteMembership").GetMember();
            string individualId = "";
            if (sm != null && sm.Profile != null && sm.Profile.Id != null)
            {
                individualId = sm.Profile.Id.ToString();
                _log.Debug(string.Format("SiteMember.Profile.Id = {0}", individualId));
            }

            List<string> invalidLoanIdList = new List<string>();

            //get list of current SRL's for the member logged-in
            if(!string.IsNullOrEmpty(individualId))
            {
                SelfReportedLoanListModel srListFromDB = GetSelfReportedLoans(individualId);
                foreach (SelfReportedLoanModel srl in srList.Loans)
                {
                    if (!string.IsNullOrEmpty(srl.LoanSelfReportedEntryId) && srListFromDB!=null)//only care about loans being updated here.
                    {
                        bool foundValidLoan = false;
                        foreach (SelfReportedLoanModel srlFromDB in srListFromDB.Loans)
                        {
                            if (srl.LoanSelfReportedEntryId == srlFromDB.LoanSelfReportedEntryId)
                            {
                                foundValidLoan = true;
                                break;
                            }
                        }

                        // if a loan attempting to be updated by user isn't in DB, then user is tampering. Remove that loan from the update list.
                        if (!foundValidLoan)
                        {
                            invalidLoanIdList.Add(srl.LoanSelfReportedEntryId);
                            _log.Warn(string.Format("User attempted to save loan information that did not belong to them. User = {0}, LoanSelfReportedEntryId = {1}", 
                                individualId, srl.LoanSelfReportedEntryId));
                        }

                    }
                }

                foreach (string str in invalidLoanIdList)
                {
                    SelfReportedLoanModel srl = srList.Loans.Find(l => l.LoanSelfReportedEntryId == str);
                    if (srl != null)
                        srList.Loans.Remove(srl);
                }
            }

            return srList;
        }

        private bool IsSRLForPersonLoggedIn(string srlId)
        {
            bool srlBelongsToPersonLoggedIn = false;
            //get Id of the member currently logged-in
            SiteMember sm = IntegrationLoader.LoadDependency<ISiteMembership>("siteMembership").GetMember();
            string individualId = "";
            if (sm != null && sm.Profile != null && sm.Profile.Id != null)
                individualId = sm.Profile.Id.ToString();

            //get list of current SRL's for the member logged-in
            if (!string.IsNullOrEmpty(individualId))
            {
                SelfReportedLoanListModel srListFromDB = GetSelfReportedLoans(individualId);

                if (!string.IsNullOrEmpty(srlId) && srListFromDB != null)//only care about loans being updated here.
                {
                    foreach (SelfReportedLoanModel srlFromDB in srListFromDB.Loans)
                    {
                        if (srlId == srlFromDB.LoanSelfReportedEntryId)
                        {
                            srlBelongsToPersonLoggedIn = true;
                            break;
                        }
                    }

                    if(!srlBelongsToPersonLoggedIn)
                        _log.Warn(string.Format("User attempted to save loan information that did not belong to them. User = {0}, LoanSelfReportedEntryId = {1}", individualId, srlId));
                }
                else if (string.IsNullOrEmpty(srlId))
                    srlBelongsToPersonLoggedIn = true;
            }

            return srlBelongsToPersonLoggedIn;
        }

        public ResultCodeModel InsertSelfReportedLoans(SelfReportedLoanListModel srLoans)
        {
            ResultCodeModel result = new Common.ResultCodeModel();

            if (srLoans == null)
            {
                _log.Info("Null SelfReportedLoan list object in ASA.Web.Services.SelfReportedService.SelfReportedAdapter.InsertSelfReportedLoans.");
            }
            else
            {
                // INSERT
                //xWebWrapper xWeb = new xWebWrapper();
                //XmlNode node = TranslateSelfReportedModel.MapSelfReportedLoanListToXmlNode(srLoans);
                ////node = xWeb.InsertFacadeObject("ASASelfReportedLoan", node);
                //result = TranslateSelfReportedModel.MapXmlNodeToResultCodeModel(node);
            }

            return result;
        }
        
           
        public ResultCodeModel InsertSelfReportedLoan(SelfReportedLoanModel srLoan)
        {
            ResultCodeModel result = new ResultCodeModel();

            if (srLoan == null)
            {
                _log.Info("Null SelfReportedLoan object in ASA.Web.Services.SelfReportedService.SelfReportedAdapter.InsertSelfReportedLoan.");
            }
            else
            {
                // INSERT
                //xWebWrapper xWeb = new xWebWrapper();
                //XmlNode node = TranslateSelfReportedModel.MapSelfReportedLoanToXmlNode(srLoan);
                //node = xWeb.InsertFacadeObject("ASASelfReportedLoan", node);
                //result = TranslateSelfReportedModel.MapXmlNodeToResultCodeModel(node);
            }

            return result;
        }

        public ResultCodeModel UpdateSelfReportedLoans(SelfReportedLoanListModel srLoans)
        {
            ResultCodeModel resultModel = new ResultCodeModel();
            bool bHadSuccess = false;
            bool bHadPartialSuccess = false;
            bool bHadFail = false;
            if (srLoans != null && srLoans.Loans.Count > 0)
            {
                foreach (SelfReportedLoanModel srl in srLoans.Loans)
                {
                    resultModel = UpdateSelfReportedLoan(srl);
                    if (resultModel.ResultCode == 1)
                        bHadSuccess = true;
                    else if (resultModel.ResultCode == 2)
                        bHadPartialSuccess = true;
                    else
                        bHadFail = true;
                }

                if (bHadPartialSuccess || (bHadFail && bHadSuccess))
                    resultModel.ResultCode = 2;
                else if (bHadFail)
                    resultModel.ResultCode = 0;
                else
                    resultModel.ResultCode = 1;
            }

            return resultModel;
        }

        public ResultCodeModel UpdateSelfReportedLoan(SelfReportedLoanModel srLoan)
        {
            ResultCodeModel result = new ResultCodeModel();

            if (srLoan == null)
            {
                _log.Info("Null SelfReportedLoan object in ASA.Web.Services.SelfReportedService.SelfReportedAdapter.UpdateSelfReportedLoan.");
            }
            else
            {
                // UPDATE
                //xWebWrapper xWeb = new xWebWrapper();
                //XmlNode node = TranslateSelfReportedModel.MapSelfReportedLoanToXmlNode(srLoan);
                //node = xWeb.UpdateFacadeObject("ASASelfReportedLoan", srLoan.LoanSelfReportedEntryId, node);
                //result = TranslateSelfReportedModel.MapXmlNodeToResultCodeModel(node);
            }

            return result;
        }

        #region private
        private bool IsNewSRL(SelfReportedLoanModel srl)
        {
            Guid g;
            if (Guid.TryParse(srl.LoanSelfReportedEntryId, out g))
                return false;
            else
                return true;
        }        
        
        private ResultCodeModel getCombinedResults(ResultCodeModel insertResult, ResultCodeModel updateResult, int insertCount, int updateCount)
        {
            ResultCodeModel result = new ResultCodeModel();
            bool bInsertSuccess = true;
            bool bUpdateSuccess = true;
            if (updateResult.ResultCode == 2)
                result.ResultCode = 2;
            else
            {
                if (insertResult.ResultCode == 0)
                    bInsertSuccess = false;
                if (updateResult.ResultCode == 0)
                    bUpdateSuccess = false;

                if (bInsertSuccess && bUpdateSuccess)
                    result.ResultCode = 1;  //complete success
                else if (bUpdateSuccess || bInsertSuccess)
                    result.ResultCode = 2; //partial success
                else
                    result.ResultCode = 0;
            }

            foreach (ErrorModel e in insertResult.ErrorList)
                result.ErrorList.Add(e);
            foreach (ErrorModel e in updateResult.ErrorList)
                result.ErrorList.Add(e);

            return result;
        }
        #endregion
    }
}
