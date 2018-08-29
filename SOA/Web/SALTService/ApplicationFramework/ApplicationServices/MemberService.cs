using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Asa.Salt.Web.Common.Types.Enums;
using Asa.Salt.Web.Services.BusinessServices.Interfaces;
using Asa.Salt.Web.Services.Data.Constants;
using Asa.Salt.Web.Services.Data.Infrastructure;
using Asa.Salt.Web.Services.Data.Model.Database;
using Asa.Salt.Web.Services.Data.Repositories;
using Asa.Salt.Web.Services.Domain;
using Asa.Salt.Web.Services.Logging;
using Asa.Salt.Web.Services.SaltSecurity.Utilities;
using ActivityType = Asa.Salt.Web.Services.Domain.ActivityType;
using EnrollmentStatus = Asa.Salt.Web.Services.Domain.EnrollmentStatus;
using GradeLevel = Asa.Salt.Web.Services.Domain.GradeLevel;
using Member = Asa.Salt.Web.Services.Domain.Member;
using MemberActivationHistory = Asa.Salt.Web.Services.Domain.MemberActivationHistory;
using MemberActivityHistory = Asa.Salt.Web.Services.Domain.MemberActivityHistory;
using MemberOrganization = Asa.Salt.Web.Services.Domain.MemberOrganization;
using MemberProduct = Asa.Salt.Web.Services.Domain.MemberProduct;
using MemberProfileAnswer = Asa.Salt.Web.Services.Domain.MemberProfileAnswer;
using MemberQA = Asa.Salt.Web.Services.Domain.MemberQA;
using MemberRole = Asa.Salt.Web.Services.Domain.MemberRole;
using MemberToDoList = Asa.Salt.Web.Services.Domain.MemberToDoList;
using PredicateBuilder = Asa.Salt.Web.Services.BusinessServices.Utilities.PredicateBuilder;
using RefMemberRole = Asa.Salt.Web.Services.Domain.RefMemberRole;
using RefOrganization = Asa.Salt.Web.Services.Domain.RefOrganization;
using RefOrganizationProduct = Asa.Salt.Web.Services.Domain.RefOrganizationProduct;
using RefProfileAnswer = Asa.Salt.Web.Services.Domain.RefProfileAnswer;
using RefProfileQuestion = Asa.Salt.Web.Services.Domain.RefProfileQuestion;
using VLCUserProfile = Asa.Salt.Web.Services.Domain.VLCUserProfile;
using vMemberAcademicInfo = Asa.Salt.Web.Services.Domain.vMemberAcademicInfo;
using vMemberQuestionAnswer = Asa.Salt.Web.Services.Domain.vMemberQuestionAnswer;
using vSourceQuestion = Asa.Salt.Web.Services.Domain.vSourceQuestion;

namespace Asa.Salt.Web.Services.BusinessServices
{
    /// <summary>
    /// The member service.
    /// </summary>
    public class MemberService : IMemberService
    {
        /// <summary>
        /// The user repository
        /// </summary>
        private readonly IMemberRepository<Member, int> _memberRepository;

        /// <summary>
        /// The vMemberQuestionAnswer repository
        /// </summary>
        private readonly IRepository<vMemberQuestionAnswer, int> _vMemberQuestionAnswerRepository;

        /// <summary>
        /// The vSourceQuestion repository
        /// </summary>
        private readonly IRepository<vSourceQuestion, int> _vSourceQuestionRepository;

        /// <summary>
        /// The memberAcademicInfo repository
        /// </summary>
        private readonly IRepository<vMemberAcademicInfo, int> _vMemberAcademicInfoRepository;

        /// <summary>
        /// The member role repository
        /// </summary>
        private readonly IRepository<MemberRole, int> _memberRoleRepository;

        /// <summary>
        /// The ref member role repository
        /// </summary>
        private readonly IRepository<RefMemberRole, int> _refMemberRoleRepository;

        /// <summary>
        /// The member profile answer repository
        /// </summary>
        private readonly IRepository<MemberProfileAnswer, int> _memberProfileAnswerRepository;

        /// <summary>
        /// The reference profile answer repository
        /// </summary>
        private readonly IRepository<RefProfileAnswer, int> _refProfileAnswerRepository;

        /// <summary>
        /// The reference question answer repository
        /// </summary>
        private readonly IRepository<RefProfileQuestion, int> _refProfileQuestionRepository;

        /// <summary>
        /// The enrollment status repository
        /// </summary>
        private readonly IRepository<EnrollmentStatus, int> _enrollmentStatusRepository;

        /// <summary>
        /// The member organization repository
        /// </summary>
        private readonly IRepository<MemberOrganization, int> _memberOrganizationRepository;

        /// <summary>
        /// The reference organization repository
        /// </summary>
        private readonly IRepository<RefOrganization, int> _refOrganizationRepository;

        /// <summary>
        /// The reference organization product repository
        /// </summary>
        private readonly IRepository<RefOrganizationProduct, int> _refOrganizationProductRepository;

        /// <summary>
        /// The grade level repository
        /// </summary>
        private readonly IRepository<GradeLevel, int> _gradeLevelRepository;


        /// <summary>
        /// The VLC Member Profile repository
        /// </summary>
        private readonly IRepository<VLCUserProfile, int> _vlcUserProfileRepository;

        /// <summary>
        /// The MemberProduct repositoy
        /// </summary>
        private readonly IRepository<MemberProduct, int> _memberProductRepository;

        /// <summary>
        /// The MemberActivityHistory repository
        /// </summary>
        private readonly IRepository<MemberActivityHistory, int> _memberActivityHistoryRepository;

        /// <summary>
        /// The MemberActivationHistory repository
        /// </summary>
        private readonly IRepository<MemberActivationHistory, int> _memberActivationHistoryRepository;

        /// <summary>
        /// The ActivityType repository
        /// </summary>
        private readonly IRepository<ActivityType, int> _activityTypeRepository;

        /// <summary>
        /// The ToDoList repository
        /// </summary>
        private readonly IRepository<MemberToDoList, int> _memberToDoListRepository;

        /// <summary>
        /// The email processor
        /// </summary>
        private readonly IEmailService _mailService;

        /// <summary>
        /// The database context
        /// </summary>
        private readonly SALTEntities _dbContext;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberService" /> class.
        /// </summary>
        /// <param name="mailService">The mail service.</param>
        /// <param name="dbContext">The db context.</param>
        /// <param name="memberRepository">The member repository.</param>
        /// <param name="memberOrganizationRepository">The member organization repository.</param>
        /// <param name="enrollmentStatusRepository">The enrollment status repository.</param>
        /// <param name="gradeLevelRepository">The grade level repository.</param>
        /// <param name="vlcUserProfileRepository">The VLC Member Profile repository.</param>
        /// <param name="memberRoleRepository">The Member Role repository.</param>
        /// <param name="refMemberRoleRepository">The Ref Member Role repository.</param>
        /// <param name="memberProductRepository">The Member Product repository.</param>
        /// <param name="vMemberAcademicInfoRepository">The Members Academic Info repository.</param>
        /// <param name="memberActivityHistoryRepository">The Member ActivityHistory repository.</param>
        /// <param name="refProfileAnswerRepository">The reference profile answer repository.</param>
        /// <param name="refProfileQuestionRepository">The reference profile question repository.</param>
        /// <param name="vMemberQuestionAnswerRepository">The Members scholarshipt Question and Answer repository.</param>
        /// <param name="refOrganizationRepository">The reference organization repository.</param>
        /// <param name="refOrganizationProductRepository">The reference organization product repository.</param>
        /// <param name="memberToDoListRepository">The todo list repository.</param>
        /// <param name="logger">The logger.</param>
        public MemberService(IEmailService mailService, 
                            SALTEntities dbContext, 
                            IMemberRepository<Member, int> memberRepository,
                            IRepository<MemberOrganization, int> memberOrganizationRepository,
                            IRepository<EnrollmentStatus, int> enrollmentStatusRepository,
                            IRepository<GradeLevel, int> gradeLevelRepository, 
                            IRepository<VLCUserProfile, int> vlcUserProfileRepository, 
                            IRepository<MemberRole, int> memberRoleRepository, 
                            IRepository<RefMemberRole, int> refMemberRoleRepository, 
                            IRepository<MemberProduct, int> memberProductRepository, 
                            IRepository<vMemberAcademicInfo, int> vMemberAcademicInfoRepository,
                            IRepository<MemberActivityHistory, int> memberActivityHistoryRepository,
                            IRepository<ActivityType, int> activityTypeRepository, 
                            IRepository<MemberActivationHistory, int> memberActivationHistoryRepository,
                            IRepository<MemberProfileAnswer, int> memberProfileAnswerRepository, 
                            IRepository<RefProfileAnswer, int> refProfileAnswerRepository,
                            IRepository<RefProfileQuestion, int> refProfileQuestionRepository,
                            IRepository<vMemberQuestionAnswer, int> vMemberQuestionAnswerRepository,
                            IRepository<vSourceQuestion, int> vSourceQuestionRepository,
                            IRepository<RefOrganization, int> refOrganizationRepository,
                            IRepository<RefOrganizationProduct, int> refOrganizationProductRepository,
                            IRepository<MemberToDoList, int> memberToDoListRepository,
                            ILog logger)
        {
            _log = logger;
            _dbContext = dbContext;
            _memberRepository = memberRepository;
            _memberOrganizationRepository = memberOrganizationRepository;
            _memberRoleRepository = memberRoleRepository;
            _refMemberRoleRepository = refMemberRoleRepository;
            _enrollmentStatusRepository = enrollmentStatusRepository;
            _gradeLevelRepository = gradeLevelRepository;
            _vlcUserProfileRepository = vlcUserProfileRepository;
            _memberProductRepository = memberProductRepository;
            _vMemberAcademicInfoRepository = vMemberAcademicInfoRepository;
            _memberActivityHistoryRepository = memberActivityHistoryRepository;
            _activityTypeRepository = activityTypeRepository;
            _memberActivationHistoryRepository = memberActivationHistoryRepository;
            _memberProfileAnswerRepository = memberProfileAnswerRepository;
            _refProfileAnswerRepository = refProfileAnswerRepository;
            _refProfileQuestionRepository = refProfileQuestionRepository;
            _vMemberQuestionAnswerRepository = vMemberQuestionAnswerRepository;
            _vSourceQuestionRepository = vSourceQuestionRepository;
            _refOrganizationRepository = refOrganizationRepository;
            _refOrganizationProductRepository = refOrganizationProductRepository;
            _mailService = mailService;
            _memberToDoListRepository = memberToDoListRepository;
        }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public Member GetUser(int userId)
        {
            var toReturn = _memberRepository.Get(m => m.MemberId == userId && m.IsMemberActive, null, "MemberOrganizations,EnrollmentStatus,GradeLevel,RegistrationSource,MemberLessons, MemberProducts,MemberProfileAnswers").FirstOrDefault();
            if (toReturn != null) 
            {
                var memberRoles = _memberRoleRepository.Get(r => r.MemberID == userId).ToList();
                foreach (var role in memberRoles)
                {
                    var refRole = _refMemberRoleRepository.Get(r => r.RefMemberRoleID == role.RefMemberRoleID).FirstOrDefault();
                    role.RefMemberRole = refRole;
                }
                toReturn.MemberRoles = memberRoles;

                IEnumerable<MemberProduct> memberDashboardProductActive = toReturn.MemberProducts.Where(p => p.RefProductID == 5 && p.IsMemberProductActive);
                if (memberDashboardProductActive.Any())
                {
                    toReturn.DashboardEnabled = true;
                }

                //Populate RefOrganization and Organization Products for the member organizations
                if (toReturn.MemberOrganizations.Count > 0)
                {
                    //list to hold the organizationIds for the member
                    List<Int32> listOrganizationId = new List<Int32>();

                    //remove all organizations that have an EffectiveEndDate, only return active organizations
                    var items = toReturn.MemberOrganizations.ToList();
                    items.RemoveAll(a => a.EffectiveEndDate.HasValue == true);
                    //add remaining items back onto object
                    toReturn.MemberOrganizations.Clear();
                    foreach (var organization in items)
                    {
                        toReturn.MemberOrganizations.Add(organization);
                        listOrganizationId.Add(organization.RefOrganizationID);
                    }

                    //Populate RefOrganization data for the organization
                    foreach (var memberOrganization in toReturn.MemberOrganizations)
                    {
                        memberOrganization.RefOrganization = GetRefOrg(memberOrganization.RefOrganizationID);
                        if (memberOrganization.RefOrganization != null)
                        {
                            memberOrganization.BranchCode = memberOrganization.RefOrganization.BranchCode;
                            memberOrganization.OECode = memberOrganization.RefOrganization.OPECode;
                        }
                    }

                    toReturn.OrganizationProducts = GetOrgProducts(listOrganizationId);

                    //populate Determined Organization ID (used in SSO where a single org info is required)
                    toReturn.OrganizationIdForCourses = DetermineMemberOrgIdForCourses(toReturn);

		    IEnumerable<RefOrganizationProduct> dashboardOrganizationProductActive = toReturn.OrganizationProducts.Where(p => p.RefProductID == 5 && p.IsRefOrganizationProductActive);
                    if (dashboardOrganizationProductActive.Any())
                    {
                        toReturn.DashboardEnabled = true;
                    }
                }

                //Populate Registration Source Name
                if (toReturn.RegistrationSource != null)
                {
                    toReturn.RegistrationSourceName = toReturn.RegistrationSource.RegistrationSourceName;
                    toReturn.RegistrationSourceId = toReturn.RegistrationSource.RefRegistrationSourceId;
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Validates if the determined org is contracted and subscribed to courses product
        /// </summary>
        /// <param name="user"></param>
        /// <returns>RefOrganizationID of the determined org</returns>
        private int DetermineMemberOrgIdForCourses(Member user)
        {
            int determinedOrgId = -1;
            //if there's a single organization with reporting id value, return its id
            int reportingIdCount = user.MemberOrganizations.Count(org => !string.IsNullOrEmpty(org.SchoolReportingID));
            if (reportingIdCount == 1)
            {
                determinedOrgId = user.MemberOrganizations.First(org => !string.IsNullOrEmpty(org.SchoolReportingID)).RefOrganizationID;
                return determinedOrgId;
            }

            MemberOrganization orgInfo = DetermineMemberOrgInfo(user);

            if (orgInfo.IsContracted)
            {
                foreach (var org in user.OrganizationProducts)
                {
                    //if the current org subscribes to courses product
                    if (org.RefProductID == 2 && org.RefOrganizationID == orgInfo.RefOrganizationID)
                    {
                        determinedOrgId = orgInfo.RefOrganizationID;
                        break;
                    }
                }
            }

            return determinedOrgId;
        }

        /// <summary>
        /// Gets a list of the RefOrganizationProducts for the supplied RefOrganizationID list where IsRefOrganizationProductActive is true
        /// </summary>
        /// <param name="listRefOrganizationId">List of RefOrganizationIDs to select</param>
        /// <returns>List of RefOrganizationProduct</returns>
        private List<RefOrganizationProduct> GetOrgProducts(List<Int32> listRefOrganizationId)
        {
            var arrayRefOrganizationId = listRefOrganizationId.ToArray();
            List<RefOrganizationProduct> listOrganizationProduct = new List<RefOrganizationProduct>();

            if (arrayRefOrganizationId.Count() > 0)
            {
                //get the products for all organizationIds in list.
                List<RefOrganizationProduct> organizationsProduct = _refOrganizationProductRepository.Get(op => arrayRefOrganizationId.Contains(op.RefOrganizationID) && op.IsRefOrganizationProductActive == true, null, "RefProduct").ToList();

                if (organizationsProduct != null)
                    listOrganizationProduct = organizationsProduct;
            }

            return listOrganizationProduct;
        }

        /// <summary>
        /// Gets the RefOrganization for the supplied RefOrganizationID
        /// </summary>
        /// <param name="refOrganizationId">value to match on RefOrganizationID</param>
        /// <returns>RefOrganization</returns>
        private RefOrganization GetRefOrg(int refOrganizationId)
        {
            //get the organization
            var org = _refOrganizationRepository.Get(o => o.RefOrganizationID == refOrganizationId, null, "RefOrganizationType").FirstOrDefault();

            if (org == null)
                return null;

            return org;
        }

        /// <summary>
        /// Gets a list of Member Organizations given a userId
        /// </summary>
        /// <param name="userId">The user's Membership ID.</param>
        /// <returns>List of MemberOrganizations</returns>
        public List<MemberOrganization> GetMemberOrganizations(int userId)
        {
            //first get a handle to the member
            var user = _memberRepository.Get(m => m.MemberId == userId, null, "MemberOrganizations").FirstOrDefault();

            if (user == null)
                return null;

            //Populate RefOrganization data for the organization
            foreach (var memberOrganization in user.MemberOrganizations)
            {
                memberOrganization.RefOrganization = GetRefOrg(memberOrganization.RefOrganizationID);
            }

            return user.MemberOrganizations.ToList();
        }

        /// <summary>
        /// Gets a list of Org Products given a userId
        /// </summary>
        /// <param name="userId">The user's Membership ID.</param>
        /// <returns>List<RefOrganizationProduct></returns>
        public List<RefOrganizationProduct> GetOrgProductsByMemberID(int userId)
        {
            //Get the orgs for this member
            List<MemberOrganization> memberOrgs = GetMemberOrganizations(userId);
            List<RefOrganizationProduct> orgProductsForMember = new List<RefOrganizationProduct>();

            if (memberOrgs != null && memberOrgs.Count > 0)
            {
                //Filter out inactive orgs, we don't want to be returning products that may be active for inactive orgs.
                memberOrgs = memberOrgs.Where(m => m.EffectiveEndDate == null).ToList();

                //Retrieve the org products by passing all the organization IDs as a list
                orgProductsForMember = GetOrgProducts(memberOrgs.Select(m => m.RefOrganizationID).ToList());
            }            

            return orgProductsForMember;
        }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="activeDirectoryKey">The active directory key.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Member GetUser(Guid activeDirectoryKey)
        {
            var user = _memberRepository.Get(m => m.ActiveDirectoryKey == activeDirectoryKey).FirstOrDefault();

            if (user != null)
            {
                user = GetUser(user.MemberId);
            }

            return user;
        }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        public Member GetUser(string username)
        {
            username = username.ToLowerInvariant();

            var user = _memberRepository.Get(m => m.EmailAddress == username && m.IsMemberActive).FirstOrDefault();

            if (user != null)
            {
                user = GetUser(user.MemberId);
            }

            return user;
        }

        /// <summary>
        /// Registers the user.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <returns></returns>
        public RegisterMemberResult RegisterUser(UserAccount account)
        {
            var toReturn = new RegisterMemberResult();

            try
            {
                //check for completeness
                if (account.Validate().Any())
                {
                    toReturn.CreateStatus = MemberUpdateStatus.IncompleteProfile;
                    return toReturn;
                }

                account.EmailAddress = account.EmailAddress.ToLowerInvariant().Trim();
                account.FirstName = account.FirstName.Trim();
                account.LastName = account.LastName.Trim();

                bool validOrganizationFound = false;

                //check if user already exists and is inactive.
                var user = _memberRepository.Get(m => m.EmailAddress == account.EmailAddress, null, "MemberOrganizations").FirstOrDefault();

                if (user != null)
                {
                    toReturn.CreateStatus = !user.IsMemberActive
                            ? MemberUpdateStatus.Inactive
                            : MemberUpdateStatus.Duplicate;

                    if (!user.IsMemberActive)
                    {
                        validOrganizationFound = ValidateOrganizations(account);

                        //refOrganizationID or ope code and branch code passed in but the organization was not found.
                        if (validOrganizationFound == false)
                        {
                            toReturn.CreateStatus = MemberUpdateStatus.InvalidOrganization;
                            return toReturn;
                        }

                        SetOrganizations(user, account);

                        user.ActiveDirectoryKey = account.ActiveDirectoryKey;
                        user.FirstName = account.FirstName;
                        user.LastName = account.LastName;
                        user.YearOfBirth = account.YearOfBirth;

                        UpdateUser(user);

                        user = GetUser(user.MemberId);
                    }

                    toReturn.Member = user;

                    return toReturn;
                }

                user = AutoMapper.Mapper.Map<UserAccount, Member>(account);

                validOrganizationFound = ValidateOrganizations(account);

                //refOrganizationID or ope code and branch code passed in but the organization was not found.
                if (validOrganizationFound == false)
                {
                    toReturn.CreateStatus = MemberUpdateStatus.InvalidOrganization;
                    return toReturn;
                }

                user.MemberOrganizations.Clear();
                SetOrganizations(user, account);

                //Add ASA Employee role if email ends with @asa.org
                if (user.EmailAddress.EndsWith("@asa.org", true, System.Globalization.CultureInfo.InvariantCulture))
                {
                    AddASAEmployeeUserRole(user);
                }
                
                user.LastLoginDate = DateTime.Now;
                user.MemberStartDate = DateTime.Now;
                user.IsMemberActive = true;
                user.CreatedDate = DateTime.Now;
                user.CreatedBy = Principal.GetIdentity();

                _memberRepository.Add(user);

                user = GetUser(user.MemberId);

                toReturn.Member = user;
                toReturn.CreateStatus = MemberUpdateStatus.Success;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                throw;
            }

            return toReturn;
        }

        /// <summary>
        /// Add ASA Employee role to user
        /// </summary>
        /// <param name="user"></param>
        private static void AddASAEmployeeUserRole(Member user)
        {
            bool hasEmployeeRole = false;
            foreach (MemberRole mr in user.MemberRoles)
            {
                if (mr.RefMemberRoleID == 2)
                {
                    mr.IsMemberRoleActive = true;
                    mr.ModifiedBy = Principal.GetIdentity();
                    hasEmployeeRole = true;
                    break;
                }
            }
            if (!hasEmployeeRole)
            {
                MemberRole employeeRole = new MemberRole();
                employeeRole.RefMemberRoleID = 2; //ASA Employee
                employeeRole.IsMemberRoleActive = true;
                employeeRole.CreatedBy = Principal.GetIdentity();
                user.MemberRoles.Add(employeeRole);
            }
        }

        /// <summary>
        /// Matches existing organization entries with incoming organization entries to update,
        /// Marks unmatched existing entries to set EffectiveEndDate
        /// Adds any incoming entries not found to match to the user.
        /// </summary>
        /// <param name="user">The user that will be inserted into the database</param>
        /// <param name="account">The information used to build the user record</param>
        /// <returns>false if no valid organization found</returns>
        private void SetOrganizations(Member user, UserAccount account)
        {
            foreach (MemberOrganization mo in user.MemberOrganizations)
            {
                //set the existing organizations IsOrganizationDeleted flag to true to set the EffectiveEndDate
                mo.IsOrganizationDeleted = true;

                //if an incoming organization matches an existing organization updated the existing organization with new info
                for (int i = account.Organizations.Count -1; i >= 0; i--)
                {
                    if (mo.RefOrganizationID == account.Organizations[i].RefOrganizationID)
                    {
                        mo.ExpectedGraduationYear = account.Organizations[i].ExpectedGraduationYear;
                        mo.SchoolReportingID = account.Organizations[i].SchoolReportingID;
                        mo.IsOrganizationDeleted = false;
                        account.Organizations.RemoveAt(i);
                        break;
                    }
                }
            }

            //put any incoming organizations that did not match onto user organization list
            foreach (OrgsForCreateMember organization in account.Organizations)
            {
                MemberOrganization memberOrganization = new MemberOrganization();
                memberOrganization.RefOrganizationID = organization.RefOrganizationID;
                memberOrganization.ExpectedGraduationYear = organization.ExpectedGraduationYear;
                memberOrganization.SchoolReportingID = organization.SchoolReportingID;
                memberOrganization.BranchCode = organization.BranchCode;
                memberOrganization.OECode = organization.OPECode;
                user.MemberOrganizations.Add(memberOrganization);
            }
        }

        /// <summary>
        /// Verfies that the RefOrganization entries exist
        /// </summary>
        /// <param name="account">The user.</param>
        /// <returns>false if no valid organization found</returns>
        private bool ValidateOrganizations(UserAccount account)
        {
            bool validOrganizationFound = false;
            RefOrganization refOrganization = null;

            for (int i = account.Organizations.Count-1; i >= 0; i--)
            {
                refOrganization = null;
                //RefOrganizationID is supplied look for organization by RefOrganizationID
                if (account.Organizations[i].RefOrganizationID > 0)
                {
                    var refOrganizationID = account.Organizations[i].RefOrganizationID;

                    refOrganization =
                    _refOrganizationRepository.Get(
                        o => o.RefOrganizationID == refOrganizationID).FirstOrDefault();
                }

                //if RefOrganizationID not supplied and oe/branch is look that way.
                if (refOrganization == null &&
                    !string.IsNullOrWhiteSpace(account.Organizations[i].BranchCode) &&
                    !string.IsNullOrWhiteSpace(account.Organizations[i].OPECode))
                {
                    var oeCode = account.Organizations[i].OPECode;
                    var branchCode = account.Organizations[i].BranchCode;

                    refOrganization =
                        _refOrganizationRepository.Get(
                            o => o.OPECode == oeCode && o.BranchCode == branchCode).FirstOrDefault();

                    if (refOrganization != null)
                    {
                        account.Organizations[i].RefOrganizationID = refOrganization.RefOrganizationID;
                    }
                }

                //organization not found remove it from list.
                if (refOrganization == null)
                {
                    account.Organizations.RemoveAt(i);
                    refOrganization = null;
                }
            }

            if (account.Organizations.Count > 0)
            {
                validOrganizationFound = true;
            }

            return validOrganizationFound;
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public MemberUpdateStatus UpdateUser(Member user)
        {
            //check for completeness.
            if (user.Validate().Any())
            {
                return MemberUpdateStatus.IncompleteProfile;
            }

            //check against the member id first. if the update is occuring because we are activating
            //a deactive user then you won't have a member id.
            var originalUser = _memberRepository.Get(m => m.MemberId == user.MemberId, null, "RegistrationSource").FirstOrDefault() ??
                               _memberRepository.Get(m => m.ActiveDirectoryKey == user.ActiveDirectoryKey, null, "RegistrationSource").First() ??
                               _memberRepository.Get(m => m.EmailAddress == user.EmailAddress, null, "RegistrationSource").First();

            SetUserData(user, originalUser);

            //if all organizations come marked as deleted, add a default organization
            if(user.MemberOrganizations.Where(o => o.IsOrganizationDeleted).Count() == user.MemberOrganizations.Count())
            {
                user.MemberOrganizations.Add(new MemberOrganization() { OECode = "000000", BranchCode = "00" });
            }

            foreach (MemberOrganization mo in user.MemberOrganizations)
            {
                //if school is "No school" no orginzation Id will be supplied - need to look it up
                if (mo.OECode == "000000")
                {
                    UserAccount ua = new UserAccount()
                    {
                        Organizations = new List<OrgsForCreateMember>()
                        {
                            new OrgsForCreateMember()
                            {
                                OPECode = mo.OECode,
                                BranchCode = mo.BranchCode
                            }
                        },
                    };
                    bool validOrganization = ValidateOrganizations(ua);
                    if (validOrganization)
                    {
                        mo.RefOrganizationID = ua.Organizations[0].RefOrganizationID;
                        mo.ExpectedGraduationYear = 1900;
                    }
                }
            }

            if (user.GradeLevel != null && !string.IsNullOrWhiteSpace(user.GradeLevel.GradeLevelCode))
            {
                var gradeLevel = _gradeLevelRepository.Get(g => g.GradeLevelCode == user.GradeLevel.GradeLevelCode).FirstOrDefault();

                //grade level passed in but was not found.
                if (gradeLevel == null)
                {
                    return MemberUpdateStatus.InvalidGradeLevel;
                }

                user.GradeLevel.GradeLevelId = gradeLevel.GradeLevelId;
                user.GradeLevelId = gradeLevel.GradeLevelId;
            }

            if (user.EnrollmentStatus != null && !string.IsNullOrWhiteSpace(user.EnrollmentStatus.EnrollmentStatusCode))
            {
                var enrollmentStatus = _enrollmentStatusRepository.Get(e => e.EnrollmentStatusCode == user.EnrollmentStatus.EnrollmentStatusCode).FirstOrDefault();

                //enrollment passed in but was not found.
                if (enrollmentStatus == null)
                {
                    return MemberUpdateStatus.InvalidEnrollment;
                }

                user.EnrollmentStatus.EnrollmentStatusId = enrollmentStatus.EnrollmentStatusId;
                user.EnrollmentStatusId = enrollmentStatus.EnrollmentStatusId;
            }

            /* if changing email from an @asa.org email to other domain or vice versa
             * update employee role accordingly */
            bool orginalIsASAEmail = originalUser.EmailAddress.EndsWith("@asa.org", true, System.Globalization.CultureInfo.InvariantCulture);
            bool newIsASAEmail = user.EmailAddress.EndsWith("@asa.org", true, System.Globalization.CultureInfo.InvariantCulture);
            if (!(orginalIsASAEmail && newIsASAEmail) && originalUser.EmailAddress != user.EmailAddress)
            {
                user.MemberRoles = _memberRoleRepository.Get(m => m.MemberID == user.MemberId, null, "").ToList();
                //Add ASA Employee role if email ends with @asa.org
                if (newIsASAEmail)
                {
                    AddASAEmployeeUserRole(user);
                }
                else if (orginalIsASAEmail)
                {
                    DeactivateASAEmployeeUserRole(user);
                }
            }
            
            _memberRepository.Update(user);

            return MemberUpdateStatus.Success;
        }

        /// <summary>
        /// Deactivates the ASA Employee role of the user
        /// </summary>
        /// <param name="user"></param>
        private static void DeactivateASAEmployeeUserRole(Member user)
        {
            foreach (MemberRole mr in user.MemberRoles)
            {
                if (mr.RefMemberRoleID == 2)
                {
                    mr.IsMemberRoleActive = false;
                    mr.ModifiedBy = Principal.GetIdentity();
                    break;
                }
            }
        }

        /// <summary>
        /// Updates the user's affiliation with organization/s
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="memberOrgAffiliations"></param>
        /// <returns></returns>
        public bool UpdateUserOrgAffiliation(int userId, IList<MemberOrganization> userOrgAffiliations)
        {
            const string logMethodName = "- UpdateUserOrgAffiliation(int userId, IList<MemberOrganization> userOrgAffiliations)- ";
            _log.Debug(logMethodName + "Begin Method");

            int orgIndex = 1;

            DataTable memberOrgDT = new DataTable(); //used for batch update in sql
            memberOrgDT.Columns.Add("OrgIndex", typeof(int));
            memberOrgDT.Columns.Add("RefOrganizationID", typeof(int));
            memberOrgDT.Columns.Add("ExpectedGraduationYear", typeof(int));
            memberOrgDT.Columns.Add("SchoolReportingID", typeof(string));
            memberOrgDT.Columns.Add("IsOrganizationDeleted", typeof(bool));
            memberOrgDT.TableName = "MemberOrganization";

            //Translate domain object to custom data table
            foreach (var memberOrg in userOrgAffiliations)
            {
                DataRow dr = memberOrgDT.NewRow();
                dr["OrgIndex"] = orgIndex;
                dr["RefOrganizationID"] = memberOrg.RefOrganizationID;
                dr["ExpectedGraduationYear"] = memberOrg.ExpectedGraduationYear.HasValue && memberOrg.ExpectedGraduationYear > 1899 ? memberOrg.ExpectedGraduationYear : 1900;
                dr["SchoolReportingID"] = memberOrg.SchoolReportingID;
                dr["IsOrganizationDeleted"] = memberOrg.IsOrganizationDeleted;

                memberOrgDT.Rows.Add(dr);
                orgIndex++;
            }
            
            try
            {

                IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);
                System.Data.SqlClient.SqlParameter MemberID = new System.Data.SqlClient.SqlParameter("i_MemberID", System.Data.SqlDbType.Int);
                MemberID.Value = userId;
                MemberID.ParameterName = "@i_MemberID";

                System.Data.SqlClient.SqlParameter memberOrgAffiliationList = new System.Data.SqlClient.SqlParameter("i_MemberOrgDT", System.Data.SqlDbType.Structured);
                memberOrgAffiliationList.Value = memberOrgDT;
                memberOrgAffiliationList.ParameterName = "@i_MemberOrgDT";
                memberOrgAffiliationList.TypeName = "dbo.MemberOrganizationTableType";

                object[] paramenters = { MemberID, memberOrgAffiliationList };

                _dbContext.Database.ExecuteSqlCommand("exec usp_UpsertMemberOrganization @i_MemberID, @i_MemberOrgDT", paramenters);
                unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                throw;
            }
            _log.Debug(logMethodName + "End Method");

            return true;

        }

        /// <summary>
        /// Sets/Updates the user with data from the original/db user
        /// </summary>
        /// <param name="user">The user to be updated.</param>
        /// <param name="originalUser">The user from the db to set existing data.</param>
        /// <returns></returns>
        private void SetUserData(Member user, Member originalUser)
        {
            const string logMethodName = ".SetUserData user, Member originalUser) - ";
            _log.Debug(logMethodName + "Begin Method");

            user.ModifiedBy = Principal.GetIdentity();
            user.ModifiedDate = DateTime.Now;
            user.CreatedBy = originalUser.CreatedBy;
            user.CreatedDate = originalUser.CreatedDate;
            user.MemberId = originalUser.MemberId;
            user.RegistrationSourceId = originalUser.RegistrationSourceId;
            user.InvitationToken = originalUser.InvitationToken;
            user.MemberStartDate = originalUser.MemberStartDate;
            user.GradeLevelId = originalUser.GradeLevelId;
            user.EnrollmentStatusId = originalUser.EnrollmentStatusId;
            
            if (String.IsNullOrEmpty(user.RegistrationSourceName) == true)
            {
                user.RegistrationSourceName = originalUser.RegistrationSource.RegistrationSourceName;
            }

            if (user.LastLoginDate == DateTime.MinValue)
            {
                user.LastLoginDate = originalUser.LastLoginDate;
            }

            if (string.IsNullOrEmpty(user.FirstName))
            {
                user.FirstName = originalUser.FirstName;
            }

            if (string.IsNullOrEmpty(user.LastName))
            {
                user.LastName = originalUser.LastName;
            }

            if (user.ActiveDirectoryKey == Guid.Empty)
            {
                user.ActiveDirectoryKey = originalUser.ActiveDirectoryKey;
            }

            //updates only happen to active users
            user.IsMemberActive = true;

            // Cannot update the CommunityDisplayName if it is already set (if so reset to original value)
            if (!string.IsNullOrEmpty(originalUser.CommunityDisplayName))
            {
                user.CommunityDisplayName = originalUser.CommunityDisplayName;
            }
            
            _log.Debug(logMethodName + "End Method");
        }

        /// <summary>
        /// Deactivates the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="modifiedByUserName">The person that modifed the user, if passed to method.</param>
        /// <returns>bool</returns>
        public bool DeactivateUser(int userId, string modifiedByUserName = null)
        {
            const string logMethodName = ".DeactivateUser(int userId, string modifiedByUserName = null) - ";
            _log.Debug(logMethodName + "Begin Method");

            bool bReturn = false;

            var user = GetUser(userId);

            if (user != null)
            {
                //if modifiedByUserName is null, that means the decativation takes place in SALT, not SALT Shaker.
                if (modifiedByUserName == null)
                {
                    modifiedByUserName = user.EmailAddress;
                }

                _memberRepository.DeactivateUser(user.MemberId, modifiedByUserName);

                var sendMail = new Task(() => _mailService.SendUserEmail(MemberEmailType.AccountClosureEmail, user));
                sendMail.Start();
                bReturn = true;
            }
            else
            {
                bReturn = false;
            }

            _log.Debug(logMethodName + "End Method");
            return bReturn;
        }

        /// <summary>
        /// Deletes the user if age is less than the minimum required.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>bool</returns>
        public bool DeleteUser(int userId)
        {
            bool toReturn = false;
            var user = _memberRepository.Get(m => m.MemberId == userId && m.IsMemberActive, null, "EnrollmentStatus,GradeLevel,RegistrationSource,MemberLessons").FirstOrDefault();

            if (user != null)
            {
                try
                {
                    IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);
                    _memberRepository.Delete(user);
                    unitOfWork.Commit();
                    toReturn = true;
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                    throw;
                }
            }
            return toReturn;
        }

        /// <summary>
        /// Gets User Profile Questions And Answers
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>MemberProfileQA</returns>
        public List<MemberProfileQA> GetUserProfileQuestionsAndAnswers(int userId)
        {
            //first get a handle to the member
            var user = _memberRepository.Get(m => m.MemberId == userId, null).FirstOrDefault();
            if (user == null)
            {
                return null;
            }
            List<MemberProfileQA> responses = new List<MemberProfileQA>();

            var userResponses = _memberProfileAnswerRepository.Get(pa => pa.MemberID == user.MemberId, null, "RefProfileAnswer");
            foreach (MemberProfileAnswer profileAnswer in userResponses)
            {
                MemberProfileQA mpqa = new MemberProfileQA();

                mpqa.ProfileAnswerExternalID = profileAnswer.RefProfileAnswer.ProfileAnswerExternalID;
                mpqa.ProfileAnswerName = profileAnswer.RefProfileAnswer.ProfileAnswerName;
                mpqa.ProfileAnswerDescription = profileAnswer.RefProfileAnswer.ProfileAnswerDescription;
                mpqa.CustomValue = profileAnswer.CustomValue;

                var question = _refProfileQuestionRepository.Get(q => q.RefProfileQuestionID == profileAnswer.RefProfileAnswer.RefProfileQuestionID).FirstOrDefault();
                if (question != null)
                {
                    mpqa.ProfileQuestionName = question.ProfileQuestionName;
                    mpqa.ProfileQuestionExternalID = question.ProfileQuestionExternalID;
                }                

                responses.Add(mpqa);
            }
            return responses;
        }

        /// <summary>
        /// Inserts or Updates user profile question/s responses by deleting previous responses, 
        /// if any, and inserting the new responses.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="responses"></param>
        /// <returns>bool</returns>
        public bool UpdateUserProfileResponses(int userId, IList<MemberProfileQA> responses)
        {
            //extract distinct ProfileQuestionExternalIDs from the incoming responses
            var distinctExternalQuestionIds = new List<int>(responses.Select(qid => qid.ProfileQuestionExternalID).Distinct().ToList());
            //valid question id list whose old responses are to be deleted
            List<int> validExternalQuestionIds = new List<int>();

            //check the existence of each question in the database and add to list
            foreach (int qID in distinctExternalQuestionIds)
            {
                var question = _refProfileQuestionRepository.Get(q => q.ProfileQuestionExternalID == qID).FirstOrDefault();
                if (question != null)
                {
                    validExternalQuestionIds.Add(question.ProfileQuestionExternalID);
                }
            }


            //If our responses contain any answers to goal ranking questions we need to delete any old goal ranking repsonses for this user otherwise they might end up with multiple rankings having the same goal
            //E.g. a user starts off with an org with four goals and ranks Master Money 4th.  Their org disbles two of the goals and the user re-ranks the two remaining goals with Master Money 2nd.  We need to delete the old responses so we dont end up with MasterMoney in 2nd and 4th
            IEnumerable<MemberProfileQA> goalRankResponses = responses.Where(b => b.ProfileQuestionExternalID >= 17 && b.ProfileQuestionExternalID <= 20);

            if (goalRankResponses.Any()) 
            {
                DeleteUserProfileResponses(userId, 17);
                DeleteUserProfileResponses(userId, 18);
                DeleteUserProfileResponses(userId, 19);
                DeleteUserProfileResponses(userId, 20);
            }

            //delete all these existing responses for each question that is being updated
            if (validExternalQuestionIds.Count() > 0)
            {
                foreach (int externalQuestionId in validExternalQuestionIds)
                {
                    //delete all existing responses, for each distinct valid question 
                    DeleteUserProfileResponses(userId, externalQuestionId);
                }
            }            

            //Insert the updated new responses
            InsertUserProfileResponses(userId, responses);
            return true;
        }

        private bool DeleteUserProfileResponses(int userId, int profileQuestionExternalID)
        {
            const string logMethodName = ".DeleteUserProfileResponses(int userId, int profileQuestionExternalID) - ";
            _log.Debug(logMethodName + "Begin Method");
            var previousResponses = _memberProfileAnswerRepository.Get(r => r.MemberID == userId, null, string.Empty).ToList();
            if (previousResponses.Count > 0)
            {
                IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);
                try
                {
                    //the delete function is mapped to a remove stored procedure usp_RemoveMemberProfileAnswer
                    var userIdParam = new SqlParameter(StoredProcedures.RemoveMemberProfileAnswer.Parameters.MemberID , userId);
                    var profQExternalIDParam = new SqlParameter(StoredProcedures.RemoveMemberProfileAnswer.Parameters.ProfileQuestionExternalID, profileQuestionExternalID);
                    _dbContext.Database.ExecuteSqlCommand(string.Format("{0} {1}, {2}", StoredProcedures.RemoveMemberProfileAnswer.StoredProcedureName, StoredProcedures.RemoveMemberProfileAnswer.Parameters.MemberID, StoredProcedures.RemoveMemberProfileAnswer.Parameters.ProfileQuestionExternalID), userIdParam, profQExternalIDParam);
                    unitOfWork.Commit();
                    _log.Debug(logMethodName + "End Method");
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                    throw;
                }
            }
            _log.Debug(logMethodName + "No Record was found to be deleted");
            _log.Debug(logMethodName + "End Method");
            return false;
        }

        private bool InsertUserProfileResponses(int userId, IList<MemberProfileQA> responses)
        {
            const string logMethodName = ".InsertUserProfileResponses(int userId, IList<MemberProfileAnswer> responses) - ";
            _log.Debug(logMethodName + "Begin Method");

            if (responses != null && responses.Any())
            {
                try
                {
                    IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);

                    foreach (var response in responses)
                    {
                        if (!response.Validate().Any())
                        {
                            var userIdParam = new SqlParameter(StoredProcedures.InsertMemberProfileAnswer.Parameters.MemberID, userId);
                            var profQExternalIDParam = new SqlParameter(StoredProcedures.InsertMemberProfileAnswer.Parameters.ProfileQuestionExternalID, response.ProfileQuestionExternalID);
                            var profAExternalIDParam = new SqlParameter(StoredProcedures.InsertMemberProfileAnswer.Parameters.ProfileAnswerExternalID, response.ProfileAnswerExternalID);                            
                            var customValueParam = new SqlParameter(StoredProcedures.InsertMemberProfileAnswer.Parameters.CustomValue, string.IsNullOrWhiteSpace(response.CustomValue) ? (object)DBNull.Value : response.CustomValue);                            
                            _dbContext.Database.ExecuteSqlCommand(string.Format("{0} {1}, {2}, {3}, {4}",
                                StoredProcedures.InsertMemberProfileAnswer.StoredProcedureName,
                                StoredProcedures.InsertMemberProfileAnswer.Parameters.MemberID,
                                StoredProcedures.InsertMemberProfileAnswer.Parameters.ProfileQuestionExternalID,
                                StoredProcedures.InsertMemberProfileAnswer.Parameters.ProfileAnswerExternalID,
                                StoredProcedures.InsertMemberProfileAnswer.Parameters.CustomValue),
                                userIdParam, profQExternalIDParam, profAExternalIDParam, customValueParam);
                            unitOfWork.Commit();
                        }
                    }
                    _log.Debug(logMethodName + "End Method");
                    return true;
                }
                catch (Exception ex)
                {
                    //If an exception with the following error is raised, it is false positive, Rob and Ibrahim verified that
                    //after the changes committed to the database successfully, the object context (EF) also returns the most 
                    //recent committed values. Hence, it is being ignored for now until it can be verified is an issue and/or
                    //TODO: figure out a better mechanism to handle it.
                    if (String.Equals(ex.Message, "The changes to the database were committed successfully, but an error occurred while updating the object context. The ObjectContext might be in an inconsistent state. Inner exception message: AcceptChanges cannot continue because the object's key values conflict with another object in the ObjectStateManager. Make sure that the key values are unique before calling AcceptChanges."))
                    {
                        _log.Debug(logMethodName + "End Method");
                        return true;
                    }
                    _log.Error(ex.Message, ex);
                    throw;
                }
            }

            _log.Debug(logMethodName + "End Method");
            return false;
        }

        /// <summary>
        /// Gets the users activity history.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<MemberActivityHistory> GetUserActivityHistory(int userId)
        {
            //first get a handle to the member
            var user = _memberRepository.Get(m => m.MemberId == userId).FirstOrDefault();
            if (user == null)
            {
                return null;
            }
            
            //get user history
            var userHistory = _memberActivityHistoryRepository.Get((m => m.MemberID == user.MemberId), null, "RefActivityType");

            //var activityType = _activityTypeRepository.Get(m => m.ActivityTypeName != null, null, "ActivityTypeName");
            //userHistory.RefActivityType = activityType;
            
            return userHistory.ToList();
        }

        /// <summary>
        /// checks if dashboard product is enable on at least one organization
        /// </summary>
        /// <param name="organizations">List of member organizations</param>
        /// <returns>bool</returns>
        public bool isDashBoardEnabled(List<MemberOrganization> organizations)
        {
            bool isDashBoardEnabled = false;
            foreach (MemberOrganization organization in organizations)
            {
                //check to ensure that only active orgs get here
                var dashboardIsActive = organization.RefOrganization.RefOrganizationProducts.Where(rop => rop.IsRefOrganizationProductActive == true && rop.RefProductID == 4).ToList();
                if (dashboardIsActive != null && dashboardIsActive.Count > 0)
                {
                    isDashBoardEnabled = true;
                    break;
                }
            }

            return isDashBoardEnabled;
        }

        /// <summary>
        /// Will extract the goals from a members profile answers. Goals are QuestionId's "17", "18", "19", "20"
        /// </summary>
        /// <param name="memberProfileAnswers">List of member ProfileAnswers</param>
        /// <returns>List of strings containing the members goals</returns>
        public List<string> GetMemberGoals(List<MemberProfileAnswer> memberProfileAnswers)
        {
            //get all the AnswerId's for QustionId's "17", "18", "19", "20"
            List<string> questionIds = new List<string>() { "17", "18", "19", "20" };
            var questionId = questionIds.Select(id => int.Parse(id)).ToList();
            var selectedAnswerIdRows = _refProfileAnswerRepository.Get(pa => questionId.Contains(pa.RefProfileQuestionID));
            var answerIds = selectedAnswerIdRows.Select(pa => pa.RefProfileAnswerID).ToList();
            //now get all the Member's ProfileAnswer that are for goals
            var memberSelectedGoals = memberProfileAnswers.Where(pa => answerIds.Contains(pa.RefProfileAnswerID));

            List<string> goals = new List<string>();
            foreach (MemberProfileAnswer mpa in memberSelectedGoals)
            {
                var profileAnswer = selectedAnswerIdRows.Where(pa => pa.RefProfileAnswerID == mpa.RefProfileAnswerID).FirstOrDefault();
                goals.Add(profileAnswer.ProfileAnswerName);
            }

            return goals;
        }

        public MemberOrganization DetermineMemberOrgInfo(Member user)
        {
            MemberOrganization orgToReturn = new MemberOrganization();
            var isSALTSchool = String.Empty;
            var isSALTOrg = String.Empty;
            var isNonSALTSchool = String.Empty;
            var arraySize = user.MemberOrganizations.Count;

            if (arraySize == 1)
            {
                orgToReturn = user.MemberOrganizations.First();
                orgToReturn.IsContracted = orgToReturn.RefOrganization.IsContracted;
            }
            else if (arraySize > 1)
            {     
                
                var schoolList = user.MemberOrganizations.Where(o => o.RefOrganization.RefOrganizationTypeID == 4);
                var saltSchoolCount = schoolList.Where(o => o.RefOrganization.IsContracted).Count();
                var nonSaltSchoolCount = schoolList.Where(o => o.RefOrganization.IsContracted == false).Count();
                var orgCount = user.MemberOrganizations.Where(o => o.RefOrganization.RefOrganizationTypeID != 4).Count();

                if (saltSchoolCount == arraySize || nonSaltSchoolCount == arraySize)
                {                    
                    var firstGradYear = user.MemberOrganizations.ToList().First().ExpectedGraduationYear;
                    var secondGradYear = user.MemberOrganizations.ToList().Last().ExpectedGraduationYear;
                    var currentYear = DateTime.Now.Year;

                    if ((firstGradYear <= currentYear && firstGradYear > secondGradYear) || (firstGradYear >= currentYear && firstGradYear > secondGradYear) || firstGradYear == secondGradYear)
                    {
                        orgToReturn = user.MemberOrganizations.Where(o => o.RefOrganization.RefOrganizationTypeID == 4).ToList().First();
                    }
                    else if (firstGradYear <= currentYear  && secondGradYear > firstGradYear ||  firstGradYear >= currentYear && secondGradYear > firstGradYear)
                    {
                        orgToReturn = user.MemberOrganizations.Where(o => o.RefOrganization.RefOrganizationTypeID == 4).ToList().Last();
                    }
                }
                else if (orgCount == arraySize)
                {
                    orgToReturn = user.MemberOrganizations.Where(o => o.RefOrganization.RefOrganizationTypeID != 4).First();
                }
                else
                {
                    if (saltSchoolCount > 0)
                    {
                        orgToReturn = schoolList.Where(o => o.RefOrganization.IsContracted).ToList().First();
                    }
                    else if (orgCount > 0)
                    {
                        orgToReturn = user.MemberOrganizations.Where(o => o.RefOrganization.RefOrganizationTypeID != 4).ToList().First();
                    }
                }
                orgToReturn.IsContracted = orgToReturn.RefOrganization.IsContracted;

            }

            return orgToReturn;
        }

	    /// <summary>
        /// Gets the User VLC Member Profile Model.
        /// </summary>
        /// <param name="userId">The user Id.</param>
        /// <returns></returns>
        public VLCUserProfile GetVlcMemberProfile(int userId)
        {
            var profile = _vlcUserProfileRepository.Get(m => m.MemberID == userId).FirstOrDefault();

            if (profile == null)
            {
               IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);
               profile = new VLCUserProfile()
               {
                   MemberID = userId,
                   CreatedBy = "WillBeOverwritten",
                   CreatedDate = System.DateTime.Now
               };
               _vlcUserProfileRepository.Add(profile);
               unitOfWork.Commit();
            }
            
            return profile;
        }

        /// <summary>
        /// Updates the user's VLC Member Profile.
        /// </summary>
        /// <param name="profile">The VLC profile to update.</param>
        /// <returns></returns>
        public bool UpdateVlcMemberProfile(VLCUserProfile profile)
        {
            try
            {
                profile.CreatedBy = "WillBeOverwrittenByStoredProc";
                IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);
                _vlcUserProfileRepository.Add(profile);
                unitOfWork.Commit();

                return true;
            }
            catch (Exception ex)
            {
                
                _log.Error(ex.Message, ex);
                throw;
                //return false;
            } 
        }

        /// <summary>
        /// Gets Members for SALTShaker search and navigation.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public IEnumerable<vMemberAcademicInfo> GetMembersBySearchParms(Nullable<Int32> MemberID, String FirstName, String LastName, String EmailAddress)
        {
            if (MemberID != null)
            {
                //return member view with the specified id
                return _vMemberAcademicInfoRepository.Get(v => v.MemberID == MemberID);
            }
            if (!string.IsNullOrEmpty(EmailAddress))
            {
                //return member view with the specified email
                return _vMemberAcademicInfoRepository.Get(v => v.EmailAddress == EmailAddress);
            }
            Expression<Func<vMemberAcademicInfo, bool>> predicate = GetSearchPredicate(FirstName, LastName);
            return  _vMemberAcademicInfoRepository.Get(predicate).ToList();
        }

        private static Expression<Func<vMemberAcademicInfo, bool>> GetSearchPredicate(String FirstName, String LastName)
        {
            var fnPred = PredicateBuilder.True<vMemberAcademicInfo>();
            var lnPred = PredicateBuilder.True<vMemberAcademicInfo>();

            Expression<Func<vMemberAcademicInfo, bool>> predicate = PredicateBuilder.True<vMemberAcademicInfo>();

            if (!String.IsNullOrEmpty(FirstName))
            {
                //edit the filtering function
                fnPred =  ((vMem) => vMem.FirstName.StartsWith(FirstName));
                predicate = PredicateBuilder.And(predicate, fnPred);
                
            }
            if (!String.IsNullOrEmpty(LastName))
            {
                //edit the filtering function
                lnPred =  ((vMem) => vMem.LastName.StartsWith(LastName));
                predicate = PredicateBuilder.And(predicate, lnPred);
            }

            //return a new predicate based on a combination of those predicates

            return predicate;
        }

        /// <summary>
        /// Gets MembersAcademicInfo View along with the records count for SALTShaker.
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        public Tuple<IEnumerable<vMemberAcademicInfo>, int> GetMembersAcademicInfoView(int startRowIndex, int maximumRows)
        {
            var resultset = _vMemberAcademicInfoRepository.GetRange(startRowIndex, maximumRows, (o => o.MemberID), (m => m.MemberID != 0));
            return resultset;
        }

        /// <summary>
        /// Gets Member products.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<MemberProduct> GetMemberProducts(int userId)
        {
            //first get a handle to the member
            var user = _memberRepository.Get(m => m.MemberId == userId, null, "MemberOrganizations").FirstOrDefault();

            if (user == null || user.MemberOrganizations.FirstOrDefault() == null)
                return null;

            if (user.MemberOrganizations.Count > 0)
            {
                var defaultUserOrg = _refOrganizationRepository.Get(o => o.RefOrganizationID == user.MemberOrganizations.FirstOrDefault().RefOrganizationID).First();
                        
                if (!defaultUserOrg.IsContracted)
                    return null;
            }

            var memberProducts = _memberProductRepository.Get(mp => mp.MemberID == user.MemberId, null, "");
            return memberProducts.ToList();
        }

        /// <summary>
        /// Adds Member product. Adds an entry into [MemberProduct] table
        /// which links a value response to a row in Table RefProduct by [RefProductID]. 
        /// </summary>
        /// <param name="memberProduct">Asa.Salt.Web.Services.Domain.MemberProduct object </param>
        /// <returns>true if success</returns>
        public bool AddMemberProduct(MemberProduct memberProduct)
        {
            try
            {
                //set required values as defaults
                memberProduct.ModifiedBy = Principal.GetIdentity();
                memberProduct.ModifiedDate = DateTime.Now;
                memberProduct.CreatedBy = Principal.GetIdentity();
                memberProduct.CreatedDate = DateTime.Now;
                IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);
                _memberProductRepository.Add(memberProduct);
                unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {

                _log.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Updates Member's product participation in [MemberProduct] table
        /// which links a value response to a row in table RefProduct by [RefProductID]. 
        /// </summary>
        /// <param name="memberProduct">Asa.Salt.Web.Services.Domain.MemberProduct object</param>
        /// <returns>true if success</returns>
        public bool UpdateMemberProduct(MemberProduct memberProduct)
        {
            try
            {
                //check against the member id and MemberProductID before updating object
                //a deactive user then you won't have a member id.
                var originalMemberProduct = _memberProductRepository.Get(m => m.RefProductID == memberProduct.RefProductID 
                    && m.MemberID == memberProduct.MemberID).FirstOrDefault();
                //set required values as defaults
                memberProduct.MemberProductID = originalMemberProduct.MemberProductID;
                memberProduct.ModifiedBy = Principal.GetIdentity();
                memberProduct.ModifiedDate = DateTime.Now;
                memberProduct.CreatedBy = originalMemberProduct.CreatedBy;
                memberProduct.CreatedDate = originalMemberProduct.CreatedDate;
                memberProduct.MemberID = originalMemberProduct.MemberID;
                //get db object context
                IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);
                //commit changes
                _memberProductRepository.Update(memberProduct);
                unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Gets Member Activation History.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<MemberActivationHistory> GetUserActivationHistory(int userId)
        {
            //first get a handle to the member
            var user = _memberRepository.Get(m => m.MemberId == userId).FirstOrDefault();
            if (user == null)
                return null;
            var memActHistory = _memberActivationHistoryRepository.Get(m => m.MemberID == user.MemberId, null, "");
            return memActHistory.ToList();
        }

        /// <summary>
        /// Gets Member Scholarship Question Answer
        /// </summary>
        /// <param name="MemberID">Nullable<Int32></param>
        /// <param name="EmailAddress">string</param>
        /// <param name="SourceID">int</param>
        /// <returns></returns>
        public IEnumerable<vMemberQuestionAnswer> GetMemberQuestionAnswer(Nullable<Int32> MemberID,  String EmailAddress, int SourceID)
        {
            const string logMethodName = "- GetMemberQuestionAnswer(Nullable<Int32> MemberID,  String EmailAddress)";
            _log.Debug(logMethodName + "Begin Method");
            try
            {
                if (MemberID != null)
                {
                    //return member view with the specified id
                    return _vMemberQuestionAnswerRepository.Get(v => v.MemberID == MemberID && v.RefSourceID == SourceID);
                }
                if (!string.IsNullOrEmpty(EmailAddress))
                {
                    //return member view with the specified email
                    return _vMemberQuestionAnswerRepository.Get(v => v.EmailAddress == EmailAddress && v.RefSourceID == SourceID);
                }
                else
                {
                    //return empty set
                    return _vMemberQuestionAnswerRepository.Get(v => v.MemberID == -100);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Inserts or Updates user Scholarship question/s responses 
        /// </summary>
        /// <param name="MemberID">ID</param>
        /// <param name="responses">IList<MemberQA></param>
        /// <returns>bool</returns>
        public bool UpsertQuestionAnswer(int MemberID, IList<MemberQA> Responses)
        {
            const string logMethodName = "- UpsertQuestionAnswer(int MemberID, IList<SourceQuestionAnswer> Responses)";
            _log.Debug(logMethodName + "Begin Method");
            try
            {
                if (Responses.Any())
                {
                    IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);

                    /* Create DataTable and define column to match SQLServer UDT param */
                    DataTable dtAddedAnswers = new DataTable("MemberQuestionAnswerTable");
                    /****************************************************************************
                     [TextParam]: nvarchar(1000). Use as main data transport (Field is not nullable)
                     [ConvertTxtToDataType]: nvarchar(8) use as optional CAST value for [TextParam] when its content should be
                      converted from text to one of 3 allowed types(datetime, int, bit) (Field is nullable)
                     [IntParamOrChildKey]: (int) use as optional int param or as child relationship criteria (Field is nullable)
                     [IntParamOrParentKey]: (int) use as optional int param or as ParentKey relationship criteria (Field is nullable)
                     ****************************************************************************/
                    DataColumn[] cols = { 
                                        new DataColumn{ColumnName = "TextParam", DataType = typeof(String), AllowDBNull = false, MaxLength = 1000},
                                        new DataColumn{ColumnName = "ConvertTxtToDataType", DataType = typeof(String), AllowDBNull = true, MaxLength = 8},
                                        new DataColumn{ColumnName = "IntParamOrChildKey", DataType = typeof(int), AllowDBNull = true},
                                        new DataColumn{ColumnName = "IntParamOrParentKey", DataType = typeof(int), AllowDBNull = true},
                                        new DataColumn{ColumnName = "TextParam2", DataType = typeof(String), AllowDBNull = true}
                                    };
                    /*Add new column definitions to DataTable*/
                    dtAddedAnswers.Columns.AddRange(cols);
                    /*poplulate with Responses list content*/
                    IEnumerable<UDT> eCollectionUDT = from answer in Responses 
                            select new UDT()
                            {
                                TextParam = new string(answer.AnswerText.Take(500).ToArray()).Trim(),
                                ConvertTxtToDataType = null,
                                IntParamOrChildKey = answer.ExternalSourceAnswerID,
                                IntParamOrParentKey = answer.ExternalSourceQuestionID,
                                TextParam2 = answer.FreeformAnswerText
                            };
                    foreach (var item in eCollectionUDT)
                    {
                        dtAddedAnswers.Rows.Add(item.TextParam, item.ConvertTxtToDataType, item.IntParamOrChildKey, item.IntParamOrParentKey, item.TextParam2);
                    }
                    /*Assign Datatable dtAddedAnswers to SqlParameter questionAnswerTable and match name and type to those of store proc
                     where TypeName is of type matching the UTD object. */
                    SqlParameter questionAnswerTable = new SqlParameter("i_MemberQuestionAnswerTable", SqlDbType.Structured)
                    {
                        Value = dtAddedAnswers,
                        ParameterName = "@i_MemberQuestionAnswerTable",
                        TypeName = "dbo.GenericTableType"
                    };
                    SqlParameter iMemberId = new SqlParameter("i_MemberID", SqlDbType.Int)
                    {
                        Value = MemberID,
                        ParameterName = "@i_MemberID"
                    };
                    SqlParameter iSourceId = new SqlParameter("i_SourceID", SqlDbType.Int)
                    {
                        Value = Responses.First().RefSourceID,
                        ParameterName = "@i_SourceID"
                    };

                    object[] paramenters = { iMemberId, iSourceId, questionAnswerTable };

                    _dbContext.Database.ExecuteSqlCommand("usp_UpsertMemberQuestionAnswer @i_MemberID, @i_SourceID, @i_MemberQuestionAnswerTable", paramenters);

                    unitOfWork.Commit();
                    _log.Debug(logMethodName + "End Method");
                    return true;
                }
                else
                {
                    _log.Info(string.Format("{0}: failed validation {1}", "List<MemberQA> Responses", "Responses must have length greater than zero. call exited without change."));
                    _log.Debug(logMethodName + "End Method");
                    return false;
                }

            }
            catch (Exception ex)
            {
                //TODO: figure out a better mechanism to handle it.
                if (String.Equals(ex.Message, "The changes to the database were committed successfully, but an error occurred while updating the object context. The ObjectContext might be in an inconsistent state. Inner exception message: AcceptChanges cannot continue because the object's key values conflict with another object in the ObjectStateManager. Make sure that the key values are unique before calling AcceptChanges."))
                {
                    _log.Debug(logMethodName + "End Method");
                    return false;
                }
                _log.Error(ex.Message, ex);
                throw;
            }

        }
       
        /// <summary>
        /// Retrieves all todos for a given member
        /// </summary>
        /// <param name="memberID">ID</param>
        /// <returns>MemberToDoList</returns>
        public IEnumerable<MemberToDoList> GetMemberToDos(int memberID)
        {
            return _memberToDoListRepository.Get(v => v.MemberID == memberID && v.RefToDoStatusID != 3).OrderBy(v => v.RefToDoStatusID);
        }

        /// <summary>
        /// Insert/update a todo
        /// </summary>
        /// <param name="todoContract">Todo</param>
        /// <returns>bool</returns>
        public bool UpsertMemberToDo(MemberToDoList todo)
        {
            try
            {
                //set required values as defaults
                todo.CreatedBy = Principal.GetIdentity();
                todo.ModifiedBy = todo.CreatedBy;
                todo.ModifiedDate = DateTime.Now;

                IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);
                var foo = _memberToDoListRepository.Add(todo);
                unitOfWork.Commit();
            }
            catch (Exception ex)
            {

                _log.Error(ex.Message, ex);
                throw;
            }
            return true;
        }

        /// <summary>
        /// Retrieves all Member Roles for a given member
        /// </summary>
        /// <param name="memberID">MemberID</param>
        /// <returns>List of MemberRole</returns>
        public IEnumerable<MemberRole> GetMemberRoles(int memberID)
        {
            const string logMethodName = "- GetMemberRoles(int memberID)";
            _log.Debug(logMethodName + "Begin Method");
            var memberRoles = _memberRoleRepository.Get(r => r.MemberID == memberID, null, "RefMemberRole");
            _log.Debug(logMethodName + "End Method");

            return memberRoles;
        }

        public bool UpsertMemberRoles(int memberId, IList<MemberRole> memberRoles, string modifiedBy)
        {
            const string logMethodName = "- UpsertMemberRoles(int memberId, IList<MemberRole> memberRoles)- ";
            _log.Debug(logMethodName + "Begin Method");

            //Update modifiedBy parameter to be the service account value if not provided
            if (modifiedBy == "SVC_ACCT")
            {
                modifiedBy = Principal.GetIdentity();
            }

            //MemberRole table
            DataTable memberRoleDT = new DataTable(); //used for batch update in sql
            memberRoleDT.Columns.Add("RefMemberRoleID", typeof(int));
            memberRoleDT.Columns.Add("IsMemberRoleActive", typeof(bool));
            memberRoleDT.TableName = "MemberRole";

            //Translate domain object to custom data table
            foreach (var memberRole in memberRoles)
            {
                DataRow dr = memberRoleDT.NewRow();
                dr["RefMemberRoleID"] = memberRole.RefMemberRoleID;
                dr["IsMemberRoleActive"] = memberRole.IsMemberRoleActive;

                memberRoleDT.Rows.Add(dr);
            }

            try
            {
                SqlParameter MemberID = new SqlParameter("i_MemberID", SqlDbType.Int);
                MemberID.Value = memberId;
                MemberID.ParameterName = "@i_MemberID";

                SqlParameter memberRoleList = new SqlParameter("i_MemberRoleTable", SqlDbType.Structured);
                memberRoleList.Value = memberRoleDT;
                memberRoleList.ParameterName = "@i_MemberRoleTable";
                memberRoleList.TypeName = "dbo.MemberRoleTableType";

                SqlParameter ModifiedBy = new System.Data.SqlClient.SqlParameter("i_ModifiedBy", System.Data.DbType.String);
                ModifiedBy.Value = modifiedBy;
                ModifiedBy.ParameterName = "@i_ModifiedBy";

                IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);
                object[] paramenters = { MemberID, memberRoleList,  ModifiedBy};
                _dbContext.Database.ExecuteSqlCommand("exec usp_UpsertMemberRole @i_MemberID, @i_MemberRoleTable, @i_ModifiedBy", paramenters);

                unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                throw;
            }            
            _log.Debug(logMethodName + "End Method");

            return true;
        }
    }
}
