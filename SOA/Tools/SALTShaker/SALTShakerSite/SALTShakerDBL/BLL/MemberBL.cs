using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SALTShaker.DAL;
using SALTShaker.DAL.DataContracts;

namespace SALTShaker.BLL
{
    public class MemberBL : IDisposable

    {
        private IMemberRepository memberRepository;

        public static SaltMemberModel currentMember = null;

        public MemberBL()
        {
            this.memberRepository = new MemberRepository();
        }

        public MemberBL(IMemberRepository memberRepository)
        {
            this.memberRepository = memberRepository;
        }

        public SaltMemberModel GetMember(int memberId)
        {
            currentMember = memberRepository.GetMember(memberId);
            
            return currentMember;
        }

        public SaltMemberModel GetMember(string userName)
        {
            currentMember = memberRepository.GetMemberByEmail(userName);

            return currentMember;
        }

        public Tuple<IEnumerable<SaltMemberModel>, int> GetMembers(int startRowIndex, int maximumRows)
        {
            return memberRepository.GetMembers(startRowIndex, maximumRows);
        }

        public IEnumerable<MemberOrganizationModel> GetMemberOrganizations(int MemberID)
        {
            return memberRepository.GetMemberOrganizations(MemberID);
        }

        public void SetMemberTableActiveFlag(bool isActive, string email, string modifierUserName = null)
        {
            memberRepository.SetMemberTableActiveFlag(isActive, email, modifierUserName);
        }

        public IEnumerable<MemberActivityHistoryModel> GetMemberActivityHistory(int MemberID)
        {
            return memberRepository.GetMemberActivityHistory(MemberID);
        }

        public IEnumerable<vMemberRoleModel> GetAllRefUserRoles()
        {
            ISaltRefDataRepository saltRefDataRepository = new SaltRefDataRepository();
            return saltRefDataRepository.GetAllRefUserRoles();
        }

        public IEnumerable<vMemberRoleModel> GetMemberRoles(int MemberID)
        {
            return memberRepository.GetMemberRoles(MemberID);
        }

        public bool UpdateMemberRoles(int MemberID, List<vMemberRoleModel> memberRoles, string ModifiedBy)
        {
            return memberRepository.UpsertMemberRoles(MemberID, memberRoles, ModifiedBy);
        }

        public IEnumerable<vMemberAcademicInfoModel> GetMembersBySearchParms(Nullable<Int32> MemberID, String FirstName, String LastName, String EmailAddress, String CommunityDisplayName)
        {
            return memberRepository.GetMembersBySearchParms(MemberID, FirstName, LastName, EmailAddress, CommunityDisplayName);
        }

        public IEnumerable<SaltMemberModel> GetMembersBySearchParms(String FirstName, String LastName, String CommunityDisplayName)
        {
            return memberRepository.GetMembersBySearchParms(FirstName, LastName, CommunityDisplayName);
        }

        //For IDisposable interface
        #region IDisposable interface
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    memberRepository.Dispose();
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
