using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using SALTShaker.DAL;
using SALTShaker.DAL.DataContracts;
using SALTShaker.DAL.Utilities;
using SALTShaker.Proxies;
using SALTShaker.Proxies.SALTService;
using log4net;

using ASA.Web.WTF.Integration;

namespace SALTShaker.BLL
{
    public class MemberRepository : IDisposable, IMemberRepository
    {
        #region SALTService calls

        private static IEnumerable<vMemberAcademicInfoModel> MemberSearch = new List<vMemberAcademicInfoModel>();
        private IEnumerable<vMemberAcademicInfoModel> FilteredMembers = new List<vMemberAcademicInfoModel>();
        private static readonly ILog logger = LogManager.GetLogger(typeof(MemberRepository));

        public MemberRepository()
        {

            //ServiceController sc = new SALTShaker.Proxies.PROSPECTService.ProspectServiceClient().;

            //switch (sc.Status)
            //{
            //    case ServiceControllerStatus.Running:
            //        return "Running";
            //    case ServiceControllerStatus.Stopped:
            //        return "Stopped";
            //    case ServiceControllerStatus.Paused:
            //        return "Paused";
            //    case ServiceControllerStatus.StopPending:
            //        return "Stopping";
            //    case ServiceControllerStatus.StartPending:
            //        return "Starting";
            //    default:
            //        return "Status Changing";
            //}
     
        }

        public SaltMemberModel GetMember(int memberId)
        {
            var member = new SaltMemberModel();
               
            IEnumerable<vMemberAcademicInfoModel> SaltSearchResult = IntegrationLoader.LoadDependency<ISaltServiceAgentAsync>("saltServiceAgentAsync").GetMembersBySearchParms(memberId, "", "", "", ar => { }).ToVMemberAcademicInfoDomainObject();
                
            foreach (var Item in SaltSearchResult)
            {
                member.MemberID = Item.MemberID;
                member.FirstName = Item.FirstName;
                member.LastName = Item.LastName;
                member.EmailAddress = Item.EmailAddress;
                member.OrganizationName = Item.OrganizationName;
            }

            return member; 
        }

        public Tuple<IEnumerable<SaltMemberModel>, int> GetMembers(int startRowIndex, int maximumRows)
        {
            var result = SaltServiceAgentAsync.GetMembersAcademicInfoView(startRowIndex, maximumRows, ar => { });

            var memToReturn = result.Item1.ToList().ToDomainObject();
            int totalRows = result.Item2;

            return Tuple.Create(memToReturn, totalRows);

        }

        public IEnumerable<MemberOrganizationModel> GetMemberOrganizations(int memberId)
        {
            return SaltServiceAgent.GetUserOrganizations(memberId).ToDomainObject().OrderBy(o => o.OrganizationName).ToList();
        }

        public SaltMemberModel GetMemberByEmail(string email)
        {
            var member = new SaltMemberModel();            

            //this will return a single record and thus firstorDefault.
            member = IntegrationLoader.LoadDependency<ISaltServiceAgentAsync>("saltServiceAgentAsync").GetMembersBySearchParms(null, "", "", email, ar => { }).ToDomainObject().FirstOrDefault();
            
            return member;
        }

        public IEnumerable<MemberActivityHistoryModel> GetMemberActivityHistory(int MemberID)
        {
            var memberActivityHistory = SaltServiceAgent.GetUserActivityHistory(MemberID).ToMemberActivityHistoryModelList();

            return memberActivityHistory;
        }

        public IEnumerable<vMemberAcademicInfoModel> GetMembersBySearchParms(Nullable<Int32> MemberID, String FirstName, String LastName, String EmailAddress, String CommunityDisplayName)
        {
            FirstName = (String.IsNullOrEmpty(FirstName)) ? "" : FirstName;
            LastName = (String.IsNullOrEmpty(LastName)) ? "" : LastName;
            EmailAddress = (String.IsNullOrEmpty(EmailAddress)) ? "" : EmailAddress;
            CommunityDisplayName = (String.IsNullOrEmpty(CommunityDisplayName)) ? "" : CommunityDisplayName;

            if (MemberID != null)
            {
                FilteredMembers = MemberSearch.Where(entity => entity.MemberID == MemberID).ToList();
            }

            if (FilteredMembers.Count() > 0)
            {
                //members found in memory
                return FilteredMembers;
            }
            else
            {
                // retrieve Members with specified search params from db
                IEnumerable<vMemberAcademicInfoModel> SaltSearchResult = IntegrationLoader.LoadDependency<ISaltServiceAgentAsync>("saltServiceAgentAsync").GetMembersBySearchParms(MemberID, FirstName, LastName, EmailAddress, ar => { }).ToVMemberAcademicInfoDomainObject();

                return SaltSearchResult;
            }
        }

        public IEnumerable<SaltMemberModel> GetMembersBySearchParms(String FirstName, String LastName, String CommunityDisplayName)
        {
            FirstName = (String.IsNullOrEmpty(FirstName)) ? "" : FirstName;
            LastName = (String.IsNullOrEmpty(LastName)) ? "" : LastName;
            CommunityDisplayName = (String.IsNullOrEmpty(CommunityDisplayName)) ? "" : CommunityDisplayName;
            
            List<SaltMemberModel> SaltSearchResult = new List<SaltMemberModel>(); 
            
            SaltSearchResult = IntegrationLoader.LoadDependency<ISaltServiceAgentAsync>("saltServiceAgentAsync").GetMembersBySearchParms(null, FirstName, LastName, "", ar => { }).ToDomainObject().ToList();

            //common members
            //Integrate results, filtering out common entries (duplicates from Prospect get removed)
            //Sorting is done to put the most recently added Organization to the top that is active:EffectiveStartDate desc then if EffectiveEndDate is NULL sort those item by OrganizationEffectiveEndDate desc
            var result = SaltSearchResult.OrderBy(i => i.EmailAddress).ThenByDescending(i => i.OrganizationEffectiveStartDate).OrderByDescending(i => !(i.OrganizationEffectiveEndDate.HasValue)).ThenByDescending(i => i.OrganizationEffectiveEndDate).ToList();

            return result;
        }

        /// <summary>
        /// Deactivates the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public bool DeactivateUser(string email, string modifierUserName = null)
        {
            int userId = SaltServiceAgent.GetUserByUsername(email).MemberId;
            SaltServiceAgent.DeactivateUser(userId, modifierUserName);
            return true;
        }

        public void SetMemberTableActiveFlag(bool isActive, string email, string modifierUserName = null)
        {
            try {
                if (isActive)
                {
                    MemberContract member = SaltServiceAgent.GetUserByUsername(email);
                    member.IsMemberActive = isActive;
                    SaltServiceAgent.UpdateUser(member);
                }
                else
                {
                    DeactivateUser(email, modifierUserName);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<vMemberRoleModel> GetMemberRoles(int MemberID)
        {
            try
            {
                return SaltServiceAgent.GetMemberRoles(MemberID).ToDomainObject();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpsertMemberRoles(int memberId, List<vMemberRoleModel> memberRoles, string ModifiedBy)
        {
            try
            {
                return SaltServiceAgent.UpsertMemberRoles(memberId, memberRoles.ToMemberRolesDataContractList().ToArray(), ModifiedBy);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
        //For IDisposable interface
        #region IDisposable interface
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    //context.Dispose();
                }
            }
            this.disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
 
