using System;
using System.Data;
using System.Collections.Generic;
using SALTShaker.DAL.DataContracts;

namespace SALTShaker.DAL
{
    public interface IMemberRepository : IDisposable
    {
        SaltMemberModel GetMember(int memberId);
        Tuple<IEnumerable<SaltMemberModel>, int> GetMembers(int startRowIndex, int maximumRows);
        SaltMemberModel GetMemberByEmail(string email);
        IEnumerable<MemberOrganizationModel> GetMemberOrganizations(int memberId);
        IEnumerable<MemberActivityHistoryModel> GetMemberActivityHistory(int MemberID);
        IEnumerable<vMemberAcademicInfoModel> GetMembersBySearchParms(Nullable<Int32> MemberID, String FirstName, String LastName, String EmailAddress, String CommunityDisplayName);
        IEnumerable<SaltMemberModel> GetMembersBySearchParms(String FirstName, String LastName, String CommunityDisplayName);
        void SetMemberTableActiveFlag(bool isActive, string email, string modifierUserName);
        IEnumerable<vMemberRoleModel> GetMemberRoles(int MemberID);
        bool UpsertMemberRoles(int memberId, List<vMemberRoleModel> memberRoles, string ModifiedBy);
    }
}
