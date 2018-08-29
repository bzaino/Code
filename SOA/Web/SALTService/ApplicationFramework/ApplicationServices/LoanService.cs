using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Asa.Salt.Web.Common.Types.Constants;
using Asa.Salt.Web.Common.Types.Enums;
using Asa.Salt.Web.Services.BusinessServices.Interfaces;
using Asa.Salt.Web.Services.Configuration.MemberReportedLoan;
using Asa.Salt.Web.Services.Data.Infrastructure;
using Asa.Salt.Web.Services.Data.Model.Database;
using Asa.Salt.Web.Services.Data.Repositories;
using Asa.Salt.Web.Services.Logging;
using Asa.Salt.Web.Services.SaltSecurity.Utilities;
using LoanType = Asa.Salt.Web.Services.Domain.LoanType;
using MemberReportedLoan = Asa.Salt.Web.Services.Domain.MemberReportedLoan;
using vMemberReportedLoans = Asa.Salt.Web.Services.Domain.vMemberReportedLoans;
using RecordSource = Asa.Salt.Web.Services.Domain.RecordSource;
using Asa.Salt.Web.Services.BusinessServices.Utilities;

namespace Asa.Salt.Web.Services.BusinessServices
{
    public class LoanService : ILoanService
    {
        /// <summary>
        /// The member reported loan repository
        /// </summary>
        private readonly IRepository<MemberReportedLoan, int> _memberLoanRepository;

        /// <summary>
        /// The member reported loan repository
        /// </summary>
        //vMemberReportedLoansRepository : Repository<vMemberReportedLoans>, IRepository<vMemberReportedLoans,int>
        private readonly  IRepository<vMemberReportedLoans,int> _vmemberReportedLoanRepository;

        /// <summary>
        /// The member reported loan repository
        /// </summary>
        private readonly IRepository<LoanType, int> _loanTypeRepository;

        /// <summary>
        /// The loan record source repository
        /// </summary>
        private readonly IRepository<RecordSource, int> _recordSourceRepository;

        /// <summary>
        /// The database context
        /// </summary>
        private readonly SALTEntities _dbContext;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        /// Loan Configuration
        /// </summary>
        private readonly MemberReportedLoanConfiguration _config;

        /// <summary>
        /// The loan calculators
        /// </summary>
        private readonly ILoanCalc _loanCalc;

        private const string INVALID_NSLDS_FILE = "Invalid NSLDS file";

        /// <summary>
        /// Initializes a new instance of the <see cref="LoanService" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="dbContext">The db context.</param>
        /// <param name="loanTypeRepository">The loan type repository.</param>
        /// <param name="memberLoanRepository">The member loan repository.</param>
        /// <param name="recordSourceRepository">The record source repository.</param>
        /// <param name="loanConfiguration">The loan configuration.</param>
        /// <param name="loanCalculators">The loan calculators.</param>
        public LoanService(ILog logger, SALTEntities dbContext, IRepository<LoanType, int> loanTypeRepository, IRepository<MemberReportedLoan, int> memberLoanRepository, 
            IRepository<RecordSource, int> recordSourceRepository, IApplicationMemberReportedLoanConfiguration loanConfiguration, ILoanCalc loanCalculators,
            IRepository<vMemberReportedLoans, int> vmemberReportedLoanRepository)
        {
            _dbContext = dbContext;
            _log = logger;

            _memberLoanRepository = memberLoanRepository;
            _loanTypeRepository = loanTypeRepository;
            _recordSourceRepository = recordSourceRepository;
            _config = loanConfiguration.GetConfiguration();
            _loanCalc = loanCalculators;
            _vmemberReportedLoanRepository = vmemberReportedLoanRepository;
        }

        /// <summary>
        /// Saves the member reported loans.
        /// </summary>
        /// <param name="userId">The member id.</param>
        /// <param name="memberReportedLoans">The member reported loans.</param>
        /// <param name="recordSourceId">The record source id.</param>
        /// <returns>
        /// IList{MemberReportedLoan}.
        /// </returns>
        public IList<MemberReportedLoan> SaveImportList(int userId, IList<MemberReportedLoan> memberReportedLoans)
        {
            var returnList = new List<MemberReportedLoan>();
            var batchTimestamp = DateTime.Now;
            try
            {
                if (!ValidateLoans(memberReportedLoans))
                {
                    return null;
                }

                IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);

                //Remove all current imported loans
                var loansToRemove = _memberLoanRepository.Get(m => m.MemberId == userId && (m.RecordSourceId == LoanRecordSources.Import || m.RecordSourceId == LoanRecordSources.ImportedKWYO), null, string.Empty);
                DeleteListOfLoans(loansToRemove);

                //only loop through loans if the object is not null
                if (memberReportedLoans != null)
                {
                    //add all the current loans given.
                    foreach (var loan in memberReportedLoans)
                    {
                        if (loan.PrincipalOutstandingAmount > 0)
                        {
                            // calculate monthly payments for loans that don't have it already
                            if (loan.MonthlyPaymentAmount == null)
                            {
                                loan.MonthlyPaymentAmount = _loanCalc.calculateMonthlyPayment(loan);
                            }
                            MemberReportedLoan addedLoan = InsertLoan(loan);
                            returnList.Add(addedLoan);
                        }

                        //if loan name or servicing organization name are null, set it to empty string.
                        if (loan.LoanName == null)
                        {
                            loan.LoanName = String.Empty;
                        }

                        if (loan.ServicingOrganizationName == null)
                        {
                            loan.ServicingOrganizationName = String.Empty;
                        }
                    }
                }
                unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                throw;
            }

            return returnList;
        }

        private void DeleteListOfLoans(IEnumerable<MemberReportedLoan> currentLoans)
        {
            foreach (var l in currentLoans)
            {
                _memberLoanRepository.Delete(l);
            }
        }

        private MemberReportedLoan InsertLoan(MemberReportedLoan loan)
        {

            if (!ValidateLoans(new List<MemberReportedLoan>() { loan }))
            {
                throw new System.ComponentModel.DataAnnotations.ValidationException();
            }

            using (System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent())
            {
                loan.CreatedBy = identity.Name;
            }
            loan.CreatedDate = DateTime.Now;
            if (loan.MonthlyPaymentAmount == null)
            {
                loan.MonthlyPaymentAmount = _loanCalc.calculateMonthlyPayment(loan);
            }
            //if loan name or servicing organization name are null, set it to empty string.
            if (loan.LoanName == null)
            {
                loan.LoanName = String.Empty;
            }

            if (loan.ServicingOrganizationName == null)
            {
                loan.ServicingOrganizationName = String.Empty;
            }

            _memberLoanRepository.Add(loan);
            loan.RecordSource = _recordSourceRepository.Get(rs => rs.RecordSourceId == loan.RecordSourceId).FirstOrDefault();

            return loan;
        }

        /// <summary>
        /// Gets the member reported loans.
        /// </summary>
        /// <param name="userId">The member id.</param>
        /// <param name="recordSourceId">The record source id.</param>
        /// <returns>
        /// IList{MemberReportedLoan}.
        /// </returns>
        public IList<MemberReportedLoan> GetUserLoans(int userId, int? recordSourceId)
        {
            var loanList = _memberLoanRepository.Get(m => m.MemberId == userId, null, "RecordSource");

            return loanList == null ? null : loanList.Where(l => recordSourceId.HasValue ? l.RecordSourceId == recordSourceId.Value : l.RecordSourceId > 0).ToList();
        }

        /// <summary>
        /// Gets the user loans by user ID and list of record source names.
        /// </summary>
        /// <param name="userId">The member id.</param>
        /// <param name="recordSourceId">The record source id.</param>
        /// <returns>
        /// IList{MemberReportedLoan}.
        /// </returns>
        public IList<MemberReportedLoan> GetUserLoansRecordSourceList(int userId, string[] recordSourceNames)
        {
            var loanList = _memberLoanRepository.Get(m => m.MemberId == userId, null, "RecordSource");
            List<MemberReportedLoan> loanListBySource = new List<MemberReportedLoan>();

            if (loanList != null && recordSourceNames.Length > 0)
            {
                foreach (string source in recordSourceNames)
                {
                    var loans = loanList.Where(l => l.RecordSource.RecordSourceName == source);

                    loanListBySource.AddRange(loans);

                }
            }

            return loanList == null || loanListBySource == null ? null : loanListBySource.ToList();
        }

        /// <summary>
        /// Gets the member reported loans.
        /// </summary>
        /// <param name="userId">The member id.</param>
        /// <returns>
        /// IEnumerable{vMemberReportedLoans}.
        /// </returns>
        public IList<vMemberReportedLoans> GetReportedLoans(int userId)
        {
           
            var loanList = _vmemberReportedLoanRepository.Get(m => m.MemberID == userId).ToList();

            return loanList;
        }
        /// <summary>
        /// delete a specific loan
        /// </summary>
        /// <param name="userId">The userId</param>
        /// <param name="loanId">The loan to delete</param>
        /// <returns>bool</returns>
        public bool RemoveLoan(int userId, int loanId)
        {
            //Get the loan we intend to delete
            var loanToRemove = _memberLoanRepository.Get(m => m.MemberReportedLoanId == loanId && m.MemberId == userId).FirstOrDefault();

            if (loanToRemove != null)
            {
                try
                {
                    IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);
                    _memberLoanRepository.Delete(loanToRemove);
                    unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                    throw;
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the loan type code.
        /// </summary>
        /// <param name="loanTypeName">Name of the loan type.</param>
        /// <returns>System.String.</returns>
        private string GetLoanTypeCode(string loanTypeName)
        {
            var loanTypeList = _loanTypeRepository.GetAll();
            var loanType = loanTypeList.FirstOrDefault(l => l.LoanTypeName == loanTypeName);

            return loanType == null ? null : loanType.LoanTypeCode;
        }

        /// <summary>
        /// Validates the import file.
        /// </summary>
        /// <param name="loans">The loans.</param>
        /// <returns></returns>
        private Boolean ValidateLoans(IList<MemberReportedLoan> loans)
        {
            foreach (var loan in loans)
            {
                var validationResults = loan.Validate();
                if (validationResults.Any())
                {
                    _log.Info(string.Format("Loan failed validation {0}", validationResults.Select(v => v.ErrorMessage).Aggregate((v, i) => v + "," + i)));
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Adds loan attributes to loans being imported.
        /// </summary>
        /// <param name="loan">The loan.</param>
        /// <param name="lineFromFile">The line from file.</param>
       public void AddLoanAttribute(MemberReportedLoan loan, String lineFromFile, List<string> nameList)
        {

            var fixedRate = _config.FixedRate;
            var variableRate = _config.VariableRate;

            if (lineFromFile.Length == 0 || lineFromFile.IndexOf(":", StringComparison.Ordinal) == -1)
                return;

            var value = lineFromFile.Substring((lineFromFile.IndexOf(":", StringComparison.Ordinal) + 1), ((lineFromFile.Length - 1) - (lineFromFile.IndexOf(":"))));
            var key = lineFromFile.Substring(0, (lineFromFile.IndexOf(":", StringComparison.Ordinal)));

            switch (key.ToUpper())
            {
                case "LOAN TYPE":
                    loan.LoanType = GetLoanTypeCode(value);
                    break;

                case "LOAN INTEREST RATE TYPE":
                    if (value.ToUpper() == "FIXED")
                    {
                        loan.InterestRate = (decimal)fixedRate;
                        loan.InterestRateType = LoanInterestRateType.Fixed;
                    }
                    else
                    {
                        loan.InterestRate = (decimal)variableRate;
                        loan.InterestRateType = LoanInterestRateType.Variable;
                    }
                    break;
                case "LOAN INTEREST RATE":
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        loan.InterestRate = Convert.ToDecimal(value.Replace("%", string.Empty));
                    }
                    break;
                case "LOAN OUTSTANDING PRINCIPAL BALANCE":
                    loan.PrincipalOutstandingAmount = string.IsNullOrEmpty(value) ? null : loan.PrincipalOutstandingAmount + Convert.ToDecimal(value.Substring(1));
                    break;

                case "LOAN OUTSTANDING INTEREST BALANCE":
                    loan.PrincipalOutstandingAmount = string.IsNullOrEmpty(value) ? null : loan.PrincipalOutstandingAmount + Convert.ToDecimal(value.Substring(1));
                    break;

                case "LOAN DATE":
                    loan.OriginalLoanDate = string.IsNullOrEmpty(value) ? null as DateTime? : Convert.ToDateTime(value);
                    loan.ReceivedYear = string.IsNullOrEmpty(value) ? null as int? : Convert.ToInt32(value.Substring(6));
                    break;

                case "LOAN DISBURSED AMOUNT":
                    loan.OriginalLoanAmount = string.IsNullOrEmpty(value) ? null as decimal? : Convert.ToDecimal(value.Substring(1));
                    break;

                case "LOAN STATUS":
                    if (string.IsNullOrWhiteSpace(loan.LoanStatus))
                    {
                        loan.LoanStatus = value;
                    }
                    break;

                case "LOAN CONTACT NAME":
                    // add loan name to loan names list
                    nameList.Add(value);
                    break;

                case "LOAN CONTACT TYPE":
                    // set a loan name of "GuarantyAgency" for loans whose conact type is "Current Guaranty Agency" so that we can exclude these names later
                    if (value == "Current Guaranty Agency")
                    {
                        nameList.Add("GuarantyAgency");
                        break;
                    }
                    else
                    {
                        break;
                    }

                case "LOAN REPAYMENT PLAN SCHEDULED AMOUNT":
                    loan.MonthlyPaymentAmount = string.IsNullOrEmpty(value) ? null as decimal? : Convert.ToDecimal(value.Substring(1));
                    break;
            }

        }


        /// <summary>
        /// Imports the loan file.
        /// </summary>
        /// <param name="userId">The member id.</param>
        /// <param name="file">The file.</param>
        /// <param name="sourceName">The source name e.g. 'KWYO'</param>
        /// <returns>
        /// IList{MemberReportedLoan}.
        /// </returns>
        public IList<MemberReportedLoan> ImportUserLoans(int userId, byte[] file, string sourceName)
        {
            var loanList = new List<MemberReportedLoan>();

            try
            {
                //StreamReader reader = new StreamReader(file);
                var fileString = System.Text.Encoding.Default.GetString(file); // reader.ReadToEnd();

                //split the file into each line
                var fileLines = Regex.Split(fileString, "\r\n");

                // a list of loan names for each loan that we will choose one from to save
                List<string> loanNameList = new List<string>();

                //the NSLDS file is broken into 2 sections...student sections which start with "Student" 
                //it looks like there is no useful information in the student section so we will by pass that
                //and loan section which starts with "Loan". Each new loan group begins with a line that starts with Loan Type:
                var i = 0;
                while (i < fileLines.Length)
                {
                    //SWD 4688 - NSLDS file validation
                    // i = 8 is actually the first line in the NSLDS file.  The previous lines are all meta data about the file
                    if (i == 8)
                    {
                        if (fileLines[i].ToString() != "File Source:U.S. DEPARTMENT OF EDUCATION, NATIONAL STUDENT LOAN DATA SYSTEM (NSLDS)")
                        {
                            throw new Exception(INVALID_NSLDS_FILE);
                        }
                    }
                    // Line 2 is File Request Date. Must be <= 1 year old
                    else if (i == 9)
                    {
                        string fileCreateInfo = fileLines[i].ToString();

                        string fileCreateDate = fileCreateInfo.Substring(18, 10);

                        DateTime fileDate = Convert.ToDateTime(fileCreateDate);
                        TimeSpan dateDiff = DateTime.Now - fileDate;
                        int differenceDays = dateDiff.Days;

                        if (differenceDays > 365)
                        {
                            throw new Exception(INVALID_NSLDS_FILE);
                        }
                    }

                    //check for the beginning of a loan group
                    if (fileLines[i].IndexOf("Loan Type:", System.StringComparison.Ordinal) == 0)
                    {
                        var mrl = new MemberReportedLoan
                        {
                            MemberId = userId,
                            PrincipalOutstandingAmount = 0,
                            LoanTerm = _config.LoanTerm,
                            CreatedBy = Principal.GetIdentity(),
                            CreatedDate = System.DateTime.Now,
                            RecordSourceId = sourceName == "KWYO" ? LoanRecordSources.ImportedKWYO : LoanRecordSources.Import
                        };

                        do
                        {
                            AddLoanAttribute(mrl, fileLines[i], loanNameList);
                            i++;
                        } while (i < fileLines.Length && fileLines[i].IndexOf("Loan Type:", System.StringComparison.Ordinal) != 0);
                        
                        //choose a name that is NOT GuarantyAgency
                        string nameToUse = loanNameList.FirstOrDefault(s => !s.Contains("GuarantyAgency"));
                        mrl.LoanName = nameToUse;
                        mrl.ServicingOrganizationName = nameToUse;

                        loanList.Add(mrl);
                        // clear the list
                        loanNameList.Clear();
                    }
                    else
                    {
                        i++;
                    }
                }

                //Only Save to the DB if there are loans in the list
                if (loanList.Count > 0)
                {
                    var savedList = SaveImportList(userId, loanList);
                    return savedList;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                if (ex.Message == INVALID_NSLDS_FILE)
                {
                    throw ex;
                }

            }

            return null;
        }

        public MemberReportedLoan CreateLoan(int userId, MemberReportedLoan loan)
        {
            if (loan.Validate().Count > 0)
            {
                throw new System.ComponentModel.DataAnnotations.ValidationException();
            }
            IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);
            MemberReportedLoan addedLoan = InsertLoan(loan);
            try
            {
                unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                throw;
            }

            return addedLoan;
        }

        public MemberReportedLoan UpdateLoan(int userId, MemberReportedLoan updatedLoan)
        {
            if (updatedLoan.Validate().Count > 0)
            {
                throw new System.ComponentModel.DataAnnotations.ValidationException();
            }

            //if the loan is type "UE" (Repayment Navigator) then delte the exisiting one and create a new loan, to make sure CreatedDate is updated.
            if (updatedLoan.LoanType == "UE")
            {
                RemoveLoan(updatedLoan.MemberId, updatedLoan.MemberReportedLoanId);
                return CreateLoan(updatedLoan.MemberId, updatedLoan);
            }

            var existingLoan = _memberLoanRepository.Get(m => m.MemberReportedLoanId == updatedLoan.MemberReportedLoanId && m.MemberId == userId).FirstOrDefault();

            //set member and member id for the updated loan
            updatedLoan.ModifiedBy = existingLoan.ModifiedBy;
            updatedLoan.CreatedBy = existingLoan.CreatedBy;

            if (existingLoan != null)
            {
                try
                {
                    if (updatedLoan.MonthlyPaymentAmount == null)
                    {
                        updatedLoan.MonthlyPaymentAmount = _loanCalc.calculateMonthlyPayment(updatedLoan);
                    }
                    updatedLoan.RecordSource = _recordSourceRepository.Get(rs => rs.RecordSourceId == updatedLoan.RecordSourceId).FirstOrDefault();
                    IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);
                    _memberLoanRepository.Update(updatedLoan);
                    unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                    throw;
                }

            }
            return updatedLoan;
        }
    }
}
