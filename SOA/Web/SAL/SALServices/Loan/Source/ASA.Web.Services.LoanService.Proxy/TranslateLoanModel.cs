using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASA.Web.Services.LoanService.Proxy;
using ASA.Web.Services.LoanService.Proxy.DataContracts;
using ASA.Web.Services.LoanService.Proxy.LoanManagement;
using ASA.Web.Services.Common;
using ASA.Web.Services.SelfReportedService.Proxy.DataContracts;
using Common.Logging;
using ASA.Web.Services.LoanService.Proxy.PersonManagement;
using ASA.Web.Services.ASAMemberService.DataContracts;
using ASA.Web.Services.Proxies.SALTService;

namespace ASA.Web.Services.LoanService.Proxy
{
    public static class TranslateLoanModel
    {
        private const string CLASSNAME = "ASA.Web.Services.LoanService.Proxy.TranslateLoanModel";
        static readonly ILog _log = LogManager.GetLogger(CLASSNAME);

       
        #region Mappings
        public static SelfReportedLoanListModel MapGetResponseToModel(GetLoanResponse response, string individualId)
        {
            _log.Debug("START MapGetResponseToModel");
            SelfReportedLoanListModel loanList = new SelfReportedLoanListModel();

            if (response != null)
            {
                if (response.LoanCanonical != null && response.LoanCanonical.Length > 0)
                {
                    for (int i = 0; i < response.LoanCanonical.Length; i++)
                    {
                        _log.Debug("mapping info for ODS Loan with LoanId = " + response.LoanCanonical[i].LoanTier2.LoanInfoType.LoanId);
                        LoanTier2Type tier2 = response.LoanCanonical[i].LoanTier2;
                        //QC 3926: Should only be showing the users loans where "IsArchived" = 'N', and InputSourceId like '%DER%'.
                        if (tier2.LoanInfoType.IsArchived == ASA.Web.Services.LoanService.Proxy.LoanManagement.YNFlagType.N &&
                            tier2.LoanInfoType.InputSourceId != null && 
                            tier2.LoanInfoType.InputSourceId.ToUpper().Contains("DER")
                            )
                        {
                            SelfReportedLoanModel srModel = new SelfReportedLoanModel();
                            srModel.IndividualId = individualId;
                            srModel.AccountNickname = ""; // "Imported Loan " + i;     // INFO NOT AVAILALBE ON LOAN. Can be provided by borrower in UI.
                            srModel.HolderName = GetDepartmentName(tier2.OrganizationArray, "HOLD");
                            //QC 3922:  Interest rate is a fraction of 1 in ODS, and needs to display as a whole number in SALT.
                            //  Hence, we've added the multiply by 100 here.
                            if (tier2.LoanInfoType.InterestRate != null)
                            { 
                                srModel.InterestRate = Math.Round((double)(tier2.LoanInfoType.InterestRate * 100), 3, MidpointRounding.AwayFromZero);  
                            }
                            srModel.OriginalLoanAmount = tier2.LoanInfoType.ApprovedLoanAmount;//TODO: is this correct mapping?
                            srModel.IsActive = true;
                            srModel.LoanSelfReportedEntryId = "";  //note: cant give it a PK here... must treat all of these as if they are unsaved SRLE's.
                            srModel.LoanStatusId = tier2.LoanInfoType.LoanStatusId;
                            srModel.LoanTypeId = tier2.LoanInfoType.LoanTypeId;
                            srModel.NextPaymentDueAmount = tier2.RepaymentInfoType.NextPaymentDueAmount;
                            srModel.NextPaymentDueDate = tier2.RepaymentInfoType.NextPaymentDueDate;
                            srModel.PaymentDueAmount = tier2.RepaymentInfoType.NextPaymentDueAmount;
                            srModel.PrincipalBalanceOutstandingAmount = tier2.LoanInfoType.OutstandingPrincipalBalance;
                            srModel.SchoolName = GetDepartmentName(tier2.OrganizationArray, "SCHL");
                            srModel.ServicerName = GetDepartmentName(tier2.OrganizationArray, "SERV");
                            srModel.ServicerWebAddress = "";  //is there something I don't know about that can provide this info??
                            srModel.LoanTerm = 10;  //defaulting to 10 for now.  Will likely get more requirements for future releases.

                            loanList.Loans.Add(srModel);
                        }
                    }
                }
                else if (response.ResponseMessageList != null && response.ResponseMessageList.Count > 0)
                {
                    for (int i = 0; i < response.ResponseMessageList.Count; i++)
                    {
                        _log.Info("ResponseMessageList[" + i + "].MessageDetails = " + response.ResponseMessageList[i].MessageDetails);
                        ErrorModel error = new ErrorModel(response.ResponseMessageList[i].MessageDetails, "Web Loan Service", response.ResponseMessageList[i].ResponseCode);
                        loanList.ErrorList.Add(error);
                    }
                }
                else
                {
                    _log.Warn("An error occured in MapGetResponseToModel when trying to retrieve loan information. Canonical not found.");
                    ErrorModel error = new ErrorModel("An error occured when trying to retrieve loan information", "Web Loan Service");
                    loanList.ErrorList.Add(error);
                }
            }
            else
            {
                ErrorModel error = new ErrorModel("No valid response was received from the Loan Management service", "Web Loan Service");
                loanList.ErrorList.Add(error);
            }
            _log.Debug("END MapGetResponseToModel");
            return loanList;
        }

        private static string GetDepartmentName(LoanTier2Type.OrganizationArrayType organizationArrayType, string orgRole)
        {
            _log.Debug("START GetDepartmentName");
            string dept = null;
            if (organizationArrayType != null && organizationArrayType.Count > 0)
            {
                for (int i = 0; i < organizationArrayType.Count; i++)
                {
                    if (organizationArrayType[i].OrganizationRoleId != null &&
                        organizationArrayType[i].OrganizationRoleId.CompareTo(orgRole) == 0)
                    {
                        dept = organizationArrayType[i].ReadOnlyOrganizationName;
                        break;
                    }
                }
            }
            _log.Debug("END GetDepartmentName");
            return dept;
        }

        public static GetLoanRequest MapSsnToGetRequest(string ssn, int maxEntities)
        {
            _log.Debug("START MapSsnToGetRequest");
            GetLoanRequest request = new GetLoanRequest();

            CriteriaLoan_Ssn c = new CriteriaLoan_Ssn();
            c.MaxEntities = maxEntities;

            c.CriterionSSN = new ASA.Web.Services.LoanService.Proxy.LoanManagement.CriterionSSNType();
            c.CriterionSSN.SSN = ssn;
            c.CriterionSSN.LogicalOperator = ASA.Web.Services.LoanService.Proxy.LoanManagement.LogicalOperatorType.AND;
            c.CriterionSSN.RelationalOperator = ASA.Web.Services.LoanService.Proxy.LoanManagement.RelationalOperatorType.EQUALS;

            c.CriterionRoleId = new ASA.Web.Services.LoanService.Proxy.LoanManagement.CriterionRoleIdType();
            c.CriterionRoleId.RoleId = "BORR";
            c.CriterionRoleId.LogicalOperator = ASA.Web.Services.LoanService.Proxy.LoanManagement.LogicalOperatorType.AND;
            c.CriterionRoleId.RelationalOperator = ASA.Web.Services.LoanService.Proxy.LoanManagement.RelationalOperatorType.EQUALS;

            c.CriterionIsArchived = new ASA.Web.Services.LoanService.Proxy.LoanManagement.CriterionIsArchivedType();
            c.CriterionIsArchived.IsArchived = ASA.Web.Services.LoanService.Proxy.LoanManagement.YNFlagType.N;
            c.CriterionIsArchived.LogicalOperator = ASA.Web.Services.LoanService.Proxy.LoanManagement.LogicalOperatorType.AND;
            c.CriterionIsArchived.RelationalOperator = ASA.Web.Services.LoanService.Proxy.LoanManagement.RelationalOperatorType.EQUALS;

            c.CriterionInputSourceId = new ASA.Web.Services.LoanService.Proxy.LoanManagement.CriterionInputSourceIDType();
            c.CriterionInputSourceId.InputSourceId = "DER";
            c.CriterionInputSourceId.LogicalOperator = ASA.Web.Services.LoanService.Proxy.LoanManagement.LogicalOperatorType.AND;
            c.CriterionInputSourceId.RelationalOperator = ASA.Web.Services.LoanService.Proxy.LoanManagement.RelationalOperatorType.CONTAINS;

            c.ListReturnTypes = new ASA.Web.Services.LoanService.Proxy.LoanManagement.ReturnListType();
            c.ListReturnTypes.LoanTier2Type = ASA.Web.Services.LoanService.Proxy.LoanManagement.YNFlagType.Y;
            c.ListReturnTypes.LoanTier2OrganizationArray = ASA.Web.Services.LoanService.Proxy.LoanManagement.YNFlagType.Y;
            c.ListReturnTypes.LoanTier2PersonArray = ASA.Web.Services.LoanService.Proxy.LoanManagement.YNFlagType.Y;

            //set sort values.  This will help (but not gaurantee) that the newer IsArchived=N, InputSourceId %DER%
            // are more likely to be in the list of loans returned.
            SortFields sortFields = new SortFields();
            sortFields.IsArchived = SortOperatorType.DESC;
            sortFields.InputSourceId = SortOperatorType.ASC;
            c.SortFields = sortFields;

            request.Criteria = c as LoanServiceCriteriaType;
            _log.Debug("END MapSsnToGetRequest");
            return request;
        }

        public static GetPersonRequest MapSSNToGetPersonRequest(string ssn)
        {
            GetPersonRequest getRequest = new GetPersonRequest();
            CriteriaPSC2CanonicalType CriteriaPSC2 = new CriteriaPSC2CanonicalType();
            
            ASA.Web.Services.LoanService.Proxy.PersonManagement.CriterionSSNType PersonSSNCriterion = new ASA.Web.Services.LoanService.Proxy.PersonManagement.CriterionSSNType();
            PersonSSNCriterion.RelationalOperator = ASA.Web.Services.LoanService.Proxy.PersonManagement.RelationalOperatorType.EQUALS;
            PersonSSNCriterion.SSN = ssn;                                          // SEARCH CRITERIA   
            PersonSSNCriterion.LogicalOperator = ASA.Web.Services.LoanService.Proxy.PersonManagement.LogicalOperatorType.AND;

            ASA.Web.Services.LoanService.Proxy.PersonManagement.CriterionCustomerIdType criterionCustomerIdType = new ASA.Web.Services.LoanService.Proxy.PersonManagement.CriterionCustomerIdType();
            criterionCustomerIdType.RelationalOperator = ASA.Web.Services.LoanService.Proxy.PersonManagement.RelationalOperatorType.EQUALS;
            criterionCustomerIdType.LogicalOperator = ASA.Web.Services.LoanService.Proxy.PersonManagement.LogicalOperatorType.AND;
            criterionCustomerIdType.CustomerId = 1;//customerID is master person.

            ASA.Web.Services.LoanService.Proxy.PersonManagement.ReturnListType listReturnTypesType = new ASA.Web.Services.LoanService.Proxy.PersonManagement.ReturnListType();
            listReturnTypesType.PersonTier2Type = ASA.Web.Services.LoanService.Proxy.PersonManagement.YNFlagType.Y;

            CriteriaPSC2.CriterionSSN = PersonSSNCriterion;
            CriteriaPSC2.CriterionCustomerID = criterionCustomerIdType;
            CriteriaPSC2.ListReturnTypes = listReturnTypesType;

            getRequest.Criteria = CriteriaPSC2 as PersonServiceCriteriaType;
            getRequest.Criteria.MaxEntities = 25;
            return getRequest;
        }
        #endregion
    }
}
