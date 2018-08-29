using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASA.Web.Services.SelfReportedService.Proxy;
using ASA.Web.Services.SelfReportedService.Proxy.DataContracts;
using ASA.Web.Services.Common;
using System.Xml;
using System.Xml.Linq;
using ASA.Web.Services.Proxies.SALTService;

namespace ASA.Web.Services.SelfReportedService.Proxy
{

    public static class TranslateSelfReportedModel
    {
        #region XML --> Model
        public static SelfReportedLoanListModel MapXmlNodeToSelfReportedLoanList(XmlNode node)
        {
            SelfReportedLoanListModel srList = new SelfReportedLoanListModel();

            if (node != null)
            {
                foreach (XmlNode n in node.ChildNodes) //foreach SRL returned
                {
                    SelfReportedLoanModel srl = new SelfReportedLoanModel();

                    foreach (XmlNode field in n.ChildNodes)//map the fields
                    {
                        if (field.InnerText != null && field.InnerText.Length > 0)
                        {
                            try
                            {
                                switch (field.Name)
                                {
                                    case "a07_key":
                                        srl.LoanSelfReportedEntryId = field.InnerText;
                                        break;
                                    case "a07_ind_cst_key":
                                        srl.IndividualId = field.InnerText;
                                        break;
                                    case "a07_loan_type":
                                        srl.LoanTypeId = field.InnerText;
                                        break;
                                    case "a07_loan_status":
                                        srl.LoanStatusId = field.InnerText;
                                        break;
                                    case "a07_account_nickname":
                                        srl.AccountNickname = field.InnerText;
                                        break;
                                    case "a07_holder_name":
                                        srl.HolderName = field.InnerText;
                                        break;
                                    case "a07_school_name":
                                        srl.SchoolName = field.InnerText;
                                        break;
                                    case "a07_servicer_name":
                                        srl.ServicerName = field.InnerText;
                                        break;
                                    case "a07_servicer_url":
                                        srl.ServicerWebAddress = field.InnerText;
                                        break;
                                    case "a07_principal_balance_outstanding_amount":
                                        srl.PrincipalBalanceOutstandingAmount = Decimal.Parse(field.InnerText);
                                        break;
                                    case "a07_payment_due_amount":
                                        srl.PaymentDueAmount = Decimal.Parse(field.InnerText);
                                        break;
                                    case "a07_next_payment_due_amount":
                                        srl.NextPaymentDueAmount = Decimal.Parse(field.InnerText);
                                        break;

                                    case "a07_next_payment_due_date":
                                        srl.NextPaymentDueDate = DateTime.Parse(field.InnerText);
                                        break;

                                    case "a07_active_flag":
                                        int active = Int32.Parse(field.InnerText);
                                        if (active == 1)
                                            srl.IsActive = true;
                                        else
                                            srl.IsActive = false;
                                        break;

                                    case "a07_interest_rate":
                                        srl.InterestRate = Double.Parse(field.InnerText);
                                        break;

                                    case "a07_received_year":
                                        srl.ReceivedYear = Int32.Parse(field.InnerText);
                                        break;

                                    case "a07_original_loan_amount":
                                        srl.OriginalLoanAmount = Decimal.Parse(field.InnerText);
                                        break;
                                    case "a07_loan_record_source":
                                        srl.LoanSource = field.InnerText;
                                        break;
                                    case "a07_loan_term":
                                        srl.LoanTerm = Int32.Parse(field.InnerText);
                                        break;
                                    case "a07_add_date":
                                        srl.DateAdded = DateTime.Parse(field.InnerText);
                                        break;
                                    default:
                                        break;

                                }//switch
                            }//try
                            catch (Exception e)
                            {
                                //something went wrong?
                            }
                        }//if
                    }//for

                    srList.Loans.Add(srl);
                }//for
            }//if
            else
                srList.ErrorList.Add(new ErrorModel("Problem querying Avectra. Make sure your search criteria was valid."));

            return srList;
        }

        public static ResultCodeModel MapXmlNodeToResultCodeModel(XmlNode node)
        {
            ResultCodeModel result = new ResultCodeModel();
            if (node != null && node.ChildNodes != null)
                result.ResultCode = 1;
            else
            {
                result.ResultCode = 0;
                ErrorModel e = new ErrorModel("There was a problem with a call to xWeb for Insert/Update of ASASelfReportedLoans.");
                result.ErrorList.Add(e);
            }
            return result;
        }

        #endregion

        #region Model --> XML
        public static XmlNode MapSelfReportedLoanListToXmlNode(SelfReportedLoanListModel srLoans)
        {
            XElement elem = new XElement("ASASelfReportedLoanObjects");
            foreach (SelfReportedLoanModel srl in srLoans.Loans)
            {
                XElement srlElem = MapSelfReportedLoanToXElement(srl);
                elem.Add(srlElem);
            }

            return xWebModelHelper.GetXmlNode(elem);
        }

        public static XmlNode MapSelfReportedLoanToXmlNode(SelfReportedLoanModel srLoan)
        {
            XElement elem = new XElement("ASASelfReportedLoanObjects");
            XElement srlElem = MapSelfReportedLoanToXElement(srLoan);
            elem.Add(srlElem);
            return xWebModelHelper.GetXmlNode(elem);
        }

        public static XElement MapSelfReportedLoanToXElement(SelfReportedLoanModel srLoan)
        {
            XElement elem = new XElement("ASASelfReportedLoanObject");
            string[] fieldNames = xWebSelfReportedLoanHelper.GetFieldNames();
            object[] objArr = new object[fieldNames.Length];

            objArr[0] = new XElement("a07_key", srLoan.LoanSelfReportedEntryId);
            objArr[1] = new XElement("a07_ind_cst_key", srLoan.IndividualId);

            objArr[2] = new XElement("a07_loan_type", srLoan.LoanTypeId);
            objArr[3] = new XElement("a07_loan_status", srLoan.LoanStatusId);
            objArr[4] = new XElement("a07_account_nickname", srLoan.AccountNickname);
            objArr[5] = new XElement("a07_holder_name", srLoan.HolderName);
            objArr[6] = new XElement("a07_school_name", srLoan.SchoolName);
            objArr[7] = new XElement("a07_servicer_name", srLoan.ServicerName);
            objArr[8] = new XElement("a07_servicer_url", srLoan.ServicerWebAddress);
            objArr[9] = new XElement("a07_principal_balance_outstanding_amount", srLoan.PrincipalBalanceOutstandingAmount);
            objArr[10] = new XElement("a07_payment_due_amount", srLoan.PaymentDueAmount);
            objArr[11] = new XElement("a07_next_payment_due_amount", srLoan.NextPaymentDueAmount);
            objArr[12] = new XElement("a07_next_payment_due_date", srLoan.NextPaymentDueDate);
            if(srLoan.IsActive)
                objArr[13] = new XElement("a07_active_flag", 1);
            else
                objArr[13] = new XElement("a07_active_flag", 0);
            objArr[14] = new XElement("a07_interest_rate", srLoan.InterestRate);
            objArr[15] = new XElement("a07_received_year", srLoan.ReceivedYear);
            objArr[16] = new XElement("a07_original_loan_amount", srLoan.OriginalLoanAmount);
            objArr[17] = new XElement("a07_loan_term", srLoan.LoanTerm);
            //objArr[18] = new XElement("a07_delete_flag", srLoan.);
            objArr[18] = new XElement("a07_loan_record_source", srLoan.LoanSource);
            elem.Add(objArr);

            return elem;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loans"></param>
        /// <returns></returns>
        public static SelfReportedLoanListModel FromMemberReportedLoanContractList(this List<MemberReportedLoanContract> loans)
        {
           SelfReportedLoanListModel list = new SelfReportedLoanListModel();
           foreach (var loan in loans)
           {
              list.Loans.Add(loan.FromMemberReportedLoanContract());
           }

           return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loan"></param>
        /// <returns></returns>
        public static SelfReportedLoanModel FromMemberReportedLoanContract(this MemberReportedLoanContract loan)
        {
           return new SelfReportedLoanModel 
           {
               LoanTypeId = loan.LoanType
              ,LoanStatusId = loan.LoanStatus
              ,PrincipalBalanceOutstandingAmount = loan.PrincipalOutstandingAmount
              ,IndividualId = loan.MemberId.ToString()
              ,InterestRate = (double)loan.InterestRate
              ,ReceivedYear = (int)loan.ReceivedYear
              ,OriginalLoanAmount = loan.OriginalLoanAmount
              ,LoanSelfReportedEntryId = loan.MemberReportedLoanId.ToString()
              ,LoanTerm = (int)loan.LoanTerm
           };

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="loans"></param>
        /// <param name="memberID"></param>
        /// <returns></returns>
 
        public static MemberReportedLoanContract[] ToMemberReportedLoanContractList(this SelfReportedLoanListModel loans, int memberID)
        {
           MemberReportedLoanContract[] list = new MemberReportedLoanContract[loans.Loans.Count];
           int i = 0;
           foreach (var loan in loans.Loans)
           {
              list[i++] = loan.ToMemberReportedLoanContract(memberID);
           }

           return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loan"></param>
        /// <returns></returns>
        public static MemberReportedLoanContract ToMemberReportedLoanContract(this SelfReportedLoanModel loan, int memberID)
        {
           return new MemberReportedLoanContract 
           {
               LoanType = loan.LoanTypeId
              ,LoanStatus = loan.LoanStatusId
              ,PrincipalOutstandingAmount = loan.PrincipalBalanceOutstandingAmount 
              ,MemberId = memberID
              ,InterestRate = (decimal)loan.InterestRate
              ,ReceivedYear = (int)loan.ReceivedYear
              ,OriginalLoanAmount = loan.OriginalLoanAmount
              ,MemberReportedLoanId = loan.LoanSelfReportedEntryId == null ? 0 : Int32.Parse(loan.LoanSelfReportedEntryId)
              ,LoanTerm = loan.LoanTerm
              ,IsActive = loan.IsActive
           };

        }

    }
}