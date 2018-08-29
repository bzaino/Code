using System.Reflection;
using ASA.Web.Services.LoanService.Proxy;
using ASA.Web.Services.LoanService.Proxy.DataContracts;
using ASA.Web.Services.LoanService.Proxy.LoanManagement;
using ASA.Web.Services.LoanService.ServiceContracts;
using ASA.Web.Services.Common;
using ASA.Web.Services.SelfReportedService.Proxy.DataContracts;
using Common.Logging;
using AASA.Web.Services.LoanService.Proxy;
using ASA.Web.Services.LoanService.Proxy.PersonManagement;
using System;
using ASA.Web.Services.ASAMemberService.DataContracts;
using ASA.Web.Services.ASAMemberService.ServiceContracts;
using ASA.Web.Services.ASAMemberService;

namespace ASA.Web.Services.LoanService
{
    public class LoanAdapter : ILoanAdapter
    {
        private IInvokeLoanManagementService _proxyLoan;
        private IInvokePersonManagementService _proxyPerson;
        private IAsaMemberAdapter _memberAdapter;

        private const string CLASSNAME = "ASA.Web.Services.LoanService.LoanAdapter";
        static readonly ILog _log = LogManager.GetLogger(CLASSNAME);

        public LoanAdapter()
        {
            _log.Debug("ASA.Web.Services.LoanService.LoanAdapter() object being created ...");
            _proxyLoan = new InvokeLoanManagementService();
            _proxyPerson = new InvokePersonManagementService();
            _memberAdapter = new AsaMemberAdapter();

        }

        public SelfReportedLoanListModel GetLoans(string ssn, ASAMemberModel member)
        {
            _log.Debug("START GetLoans");
            SelfReportedLoanListModel srlList = null;

            if (member != null) //TODO: Mehdi && member.Source != SourceType.SELF_REGISTERED_NO_MATCH)
            {
                ////the following hardcoded values are only for when you're being lazy and dont want to find a person to test GetPerson, MatchingPersonFound, and GetLoan
                //member.FirstName = "Dwayne";
                //member.LastName = "Baker";
                //member.DOB = new DateTime(1976,1,30);
                //member.IndividualId = "967A954B-AB09-4713-9B6E-2067B8C3F992";
                //ssn = "803450294";

                // See if person is in ODS for this SSN
                GetPersonRequest getPersonRequest = TranslateLoanModel.MapSSNToGetPersonRequest(ssn);
                GetPersonResponse getPersonResponse = _proxyPerson.GetPerson(getPersonRequest);

                // if person was retrieved, do they match the FName, LName, and DOB from context?
                bool personMatchFound = MatchingPersonFound(member, getPersonResponse);

                if (personMatchFound)
                {
                    _log.Debug("personMatchFound.");
                    // save this person's SSN from ODS to Avectra
                    bool ssnSaved = SaveSSNToAvectra(member, ssn);

                    if (ssnSaved)
                    {
                        // go get this person's Loans from ODS
                        GetLoanRequest getRequest = TranslateLoanModel.MapSsnToGetRequest(ssn, Config.LoanServiceMaxEntities);
                        GetLoanResponse response = _proxyLoan.GetLoan(getRequest);
                        srlList = TranslateLoanModel.MapGetResponseToModel(response, member.IndividualId);
                    }
                }
                else
                {
                    _log.Debug("NOT personMatchFound.");
                }
            }

            _log.Debug("END GetLoans");
            return srlList;
        }

        #region private
        private bool MatchingPersonFound(ASAMemberModel member, GetPersonResponse getPersonResponse)
        {
            _log.Debug("START MatchingPersonFound");
            bool bMatched = false;

            if( getPersonResponse!= null && 
                getPersonResponse.PersonCanonicalList != null &&
                getPersonResponse.PersonCanonicalList.Length >0
            )
            {
                _log.Debug("The getPersonResponse returned is constructed correctly.");
                foreach (PersonCanonicalType p in getPersonResponse.PersonCanonicalList)
                {
                    //compare this person's LastName and DOB with those on the member object.
                    if (p.PersonTier2 != null && p.PersonTier2.PersonInfoType != null)
                    {
                        _log.Debug("PersonInfoType was found");
                        if (member.DOB != null && p.PersonTier2.PersonInfoType.DateOfBirth != null) //can only compare these if they're not null
                        {
                            _log.Debug(string.Format("ODS: First={0}, Last={1}, DOB={2}", 
                                !string.IsNullOrEmpty(p.PersonTier2.PersonInfoType.FirstName)?p.PersonTier2.PersonInfoType.FirstName.ToUpper():"",
                                !string.IsNullOrEmpty(p.PersonTier2.PersonInfoType.LastName)?p.PersonTier2.PersonInfoType.LastName.ToUpper():"",
                                p.PersonTier2.PersonInfoType.DateOfBirth.ToString())
                                );
                            _log.Debug(string.Format("MRM: First={0}, Last={1}, DOB={2}",
                                !string.IsNullOrEmpty(member.FirstName) ? member.FirstName.ToUpper() : "",
                                !string.IsNullOrEmpty(member.LastName) ? member.LastName.ToUpper() : "",
                                member.DOB.ToString())
                                );
                            if (string.Compare(p.PersonTier2.PersonInfoType.LastName.ToUpper(), member.LastName.ToUpper()) == 0 &&
                                string.Compare(p.PersonTier2.PersonInfoType.FirstName.ToUpper(), member.FirstName.ToUpper()) == 0 &&
                                DateTime.Compare((DateTime)p.PersonTier2.PersonInfoType.DateOfBirth, (DateTime)member.DOB) == 0) //found match
                            {
                                bMatched = true;
                                break;
                            }
                        }
                    }
                }
            }

            _log.Debug("END MatchingPersonFound");
            return bMatched;
        }

        private bool SaveSSNToAvectra(ASAMemberModel member, string ssn)
        {
            return true;
            //_log.Debug("START SaveSSNToAvectra");

            ////save this person's PersonId from ODS to Avectra
            //bool ssnSaved = _memberAdapter.UpdateSSN(member, ssn);

            //_log.Debug("END SaveSSNToAvectra");
            //return ssnSaved;

        }
        #endregion
    }
}
