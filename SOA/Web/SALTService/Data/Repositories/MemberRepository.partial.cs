using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using Asa.Salt.Web.Services.Data.Constants;
using Asa.Salt.Web.Services.Data.Infrastructure;
using Asa.Salt.Web.Services.Data.Model.Database;

using Member = Asa.Salt.Web.Services.Domain.Member;

namespace Asa.Salt.Web.Services.Data.Repositories
{
    public partial class MemberRepository : Repository<Member>, IMemberRepository<Member, int>
    {
        public void DeactivateUser(int memberId, string userName)
        {
            //the delete function is mapped to a usp_DeactivateMember stored proc. 
            //the function import in the edmx can't be used due to conflicting type names.

            var memberIdParam = new SqlParameter(StoredProcedures.DeactivateMember.Parameters.MemberId, memberId);
            var userNameParam = new SqlParameter(StoredProcedures.DeactivateMember.Parameters.UserName, userName);

            try
            {
                IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_context);

                _context.Database.ExecuteSqlCommand(string.Format("{0} {1}, {2}",
                    StoredProcedures.DeactivateMember.StoredProcedureName,
                    StoredProcedures.DeactivateMember.Parameters.MemberId,
                    StoredProcedures.DeactivateMember.Parameters.UserName),
                    memberIdParam, userNameParam);

                unitOfWork.Commit();
            }
            catch (Exception)
            {
                throw;
            }

            return;
        }

        /// <summary>
        /// Inserts/activates the user.
        /// </summary>
        /// <param name="user">The user to insert into the database</param>
        /// <returns>Member</returns>
        public override Member Add(Member user)
        {
            //the Add function is mapped to usp_ActivateMember stored proc. 
            //the function import in the edmx can't be used due to conflicting type names.

            //const string logMethodName = ".Add(Member user) - ";
            //_log.Debug(logMethodName + "Begin Method");

            var firstNameParam = new SqlParameter(StoredProcedures.ActivateMember.Parameters.FirstName, string.IsNullOrWhiteSpace(user.FirstName) ? (object)DBNull.Value : user.FirstName);
            var lastNameParam = new SqlParameter(StoredProcedures.ActivateMember.Parameters.LastName, string.IsNullOrWhiteSpace(user.LastName) ? (object)DBNull.Value : user.LastName);
            var emailAddressParam = new SqlParameter(StoredProcedures.ActivateMember.Parameters.EmailAddress, string.IsNullOrWhiteSpace(user.EmailAddress) ? (object)DBNull.Value : user.EmailAddress);
            var isContactAllowedParam = new SqlParameter(StoredProcedures.ActivateMember.Parameters.IsContactAllowed, user.IsContactAllowed);
            var registrationSourceIdParam = new SqlParameter(StoredProcedures.ActivateMember.Parameters.RegistrationSourceId, user.RegistrationSourceId);
            var invitationTokenParam = new SqlParameter(StoredProcedures.ActivateMember.Parameters.InvitationToken, user.InvitationToken.HasValue ? user.InvitationToken : (object)DBNull.Value);
            var activeDirectoryKeyParam = new SqlParameter(StoredProcedures.ActivateMember.Parameters.ActiveDirectoryKey, user.ActiveDirectoryKey);
            var refGradeLevelIDParam = new SqlParameter(StoredProcedures.ActivateMember.Parameters.RefGradeLevelID, user.GradeLevelId.HasValue ? user.GradeLevelId : (object)DBNull.Value);
            var refEnrollmentStatusIDParam = new SqlParameter(StoredProcedures.ActivateMember.Parameters.RefEnrollmentStatusID, user.EnrollmentStatusId.HasValue ? user.EnrollmentStatusId : (object)DBNull.Value);
            var yearOfBirthParam = new SqlParameter(StoredProcedures.ActivateMember.Parameters.YearOfBirth, user.YearOfBirth.HasValue ? user.YearOfBirth : (object)DBNull.Value);

            //MemberOrganization table
            int orgIndex = 1;
            DataTable memberOrgDT = new DataTable(); //used for batch update in sql
            memberOrgDT.Columns.Add("OrgIndex", typeof(int));
            memberOrgDT.Columns.Add("RefOrganizationID", typeof(int));
            memberOrgDT.Columns.Add("ExpectedGraduationYear", typeof(int));
            memberOrgDT.Columns.Add("SchoolReportingID", typeof(string));
            memberOrgDT.Columns.Add("IsOrganizationDeleted", typeof(bool));
            memberOrgDT.TableName = "MemberOrganization";

            //Translate domain object to custom data table
            foreach (var memberOrg in user.MemberOrganizations)
            {
                DataRow dr = memberOrgDT.NewRow();
                dr["OrgIndex"] = orgIndex;
                dr["RefOrganizationID"] = memberOrg.RefOrganizationID;
                dr["ExpectedGraduationYear"] = memberOrg.ExpectedGraduationYear;
                dr["SchoolReportingID"] = memberOrg.SchoolReportingID;
                dr["IsOrganizationDeleted"] = memberOrg.IsOrganizationDeleted;

                memberOrgDT.Rows.Add(dr);
                orgIndex++;
            }

            var memberOrgList = new System.Data.SqlClient.SqlParameter("@i_MemberOrganizationTable", System.Data.SqlDbType.Structured);
            memberOrgList.Value = memberOrgDT;
            memberOrgList.ParameterName = "@i_MemberOrganizationTable";
            memberOrgList.TypeName = "dbo.MemberOrganizationTableType";

            //MemberRole table
            DataTable memberRoleDT = new DataTable(); //used for batch update in sql
            memberRoleDT.Columns.Add("RefMemberRoleID", typeof(int));
            memberRoleDT.Columns.Add("IsMemberRoleActive", typeof(bool));
            memberRoleDT.TableName = "MemberRole";

            //Translate domain object to custom data table
            foreach (var memberRole in user.MemberRoles)
            {
                DataRow dr = memberRoleDT.NewRow();
                dr["RefMemberRoleID"] = memberRole.RefMemberRoleID;
                dr["IsMemberRoleActive"] = memberRole.IsMemberRoleActive;

                memberRoleDT.Rows.Add(dr);
            }

            var memberRoleList = new System.Data.SqlClient.SqlParameter("@i_MemberRoleTable", System.Data.SqlDbType.Structured);
            memberRoleList.Value = memberRoleDT;
            memberRoleList.ParameterName = "@i_MemberRoleTable";
            memberRoleList.TypeName = "dbo.MemberRoleTableType";

            try
            {
                int memberId = 0;

                IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_context);

                memberId = Convert.ToInt32(
                    _context.Database.SqlQuery<Int32>(string.Format("{0} {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}",
                StoredProcedures.ActivateMember.StoredProcedureName,
                StoredProcedures.ActivateMember.Parameters.FirstName,
                StoredProcedures.ActivateMember.Parameters.LastName,
                StoredProcedures.ActivateMember.Parameters.EmailAddress,
                StoredProcedures.ActivateMember.Parameters.IsContactAllowed,
                StoredProcedures.ActivateMember.Parameters.RegistrationSourceId,
                StoredProcedures.ActivateMember.Parameters.InvitationToken,
                StoredProcedures.ActivateMember.Parameters.ActiveDirectoryKey,
                StoredProcedures.ActivateMember.Parameters.RefGradeLevelID,
                StoredProcedures.ActivateMember.Parameters.RefEnrollmentStatusID,
                StoredProcedures.ActivateMember.Parameters.YearOfBirth,
                StoredProcedures.ActivateMember.Parameters.MemberOrgUDT,
                StoredProcedures.ActivateMember.Parameters.MemberRoleUDT),

                firstNameParam, lastNameParam, emailAddressParam, isContactAllowedParam, registrationSourceIdParam, invitationTokenParam, 
                activeDirectoryKeyParam, refGradeLevelIDParam, refEnrollmentStatusIDParam, yearOfBirthParam, memberOrgList, memberRoleList).Select(n=>n.ToString()).First());

                unitOfWork.Commit();

                //set memeberId in the user object
                user.MemberId = memberId;
            }
            catch (Exception)
            {
                //_log.Error(ex.Message, ex);
                throw;
            }

            //_log.Debug(logMethodName + "End Method");
            return user;
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="user">The user to update in the database</param>
        /// <returns>void</returns>
        public override void Update(Member user)
        {
            //the Update function is mapped to usp_UpdateMember stored proc. 
            //the function import in the edmx can't be used due to conflicting type names.

            //const string logMethodName = ".UserUpdate(Member user) - ";
            //_log.Debug(logMethodName + "Begin Method");

            var memberIDParam = new SqlParameter(StoredProcedures.UpdateMember.Parameters.MemberID, user.MemberId);
            var firstNameParam = new SqlParameter(StoredProcedures.UpdateMember.Parameters.FirstName, user.FirstName);
            var lastNameParam = new SqlParameter(StoredProcedures.UpdateMember.Parameters.LastName, string.IsNullOrWhiteSpace(user.LastName) ? (object)DBNull.Value : user.LastName);
            var emailAddressParam = new SqlParameter(StoredProcedures.UpdateMember.Parameters.EmailAddress, string.IsNullOrWhiteSpace(user.EmailAddress) ? (object)DBNull.Value : user.EmailAddress);
            var isContactAllowedParam = new SqlParameter(StoredProcedures.UpdateMember.Parameters.IsContactAllowed, user.IsContactAllowed);
            var registrationSourceIdParam = new SqlParameter(StoredProcedures.UpdateMember.Parameters.RegistrationSourceId, user.RegistrationSourceId);
            var refGradeLevelIDParam = new SqlParameter(StoredProcedures.UpdateMember.Parameters.RefGradeLevelID, user.GradeLevelId.HasValue ? user.GradeLevelId : (object)DBNull.Value);
            var refEnrollmentStatusIDParam = new SqlParameter(StoredProcedures.UpdateMember.Parameters.RefEnrollmentStatusID, user.EnrollmentStatusId.HasValue ? user.EnrollmentStatusId : (object)DBNull.Value);
            var lastLoginDateParam = new SqlParameter(StoredProcedures.UpdateMember.Parameters.LastLoginDate, user.LastLoginDate);
            var displayNameParam = new SqlParameter(StoredProcedures.UpdateMember.Parameters.DisplayName, string.IsNullOrWhiteSpace(user.DisplayName) ? (object)DBNull.Value : user.DisplayName);
            var isMemberActiveParam = new SqlParameter(StoredProcedures.UpdateMember.Parameters.IsMemberActive, user.IsMemberActive);
            var activeDirectoryKeyParam = new SqlParameter(StoredProcedures.UpdateMember.Parameters.ActiveDirectoryKey, user.ActiveDirectoryKey);
            var communityDisplayNameParam = new SqlParameter(StoredProcedures.UpdateMember.Parameters.CommunityDisplayName, string.IsNullOrWhiteSpace(user.CommunityDisplayName) ? (object)DBNull.Value : user.CommunityDisplayName);
            var isCommunityActiveParam = new SqlParameter(StoredProcedures.UpdateMember.Parameters.IsCommunityActive, user.IsCommunityActive);
            var uSPostalCodeParam = new SqlParameter(StoredProcedures.UpdateMember.Parameters.USPostalCode, string.IsNullOrWhiteSpace(user.USPostalCode) ? (object)DBNull.Value : user.USPostalCode);
            var yearOfBirthParam = new SqlParameter(StoredProcedures.UpdateMember.Parameters.YearOfBirth, user.YearOfBirth.HasValue ? user.YearOfBirth : (object)DBNull.Value);
            var welcomeEmailSentParam = new SqlParameter(StoredProcedures.UpdateMember.Parameters.WelcomeEmailSent, user.WelcomeEmailSent);

            //MemberOrganization table
            int orgIndex = 1;
            DataTable memberOrgDT = new DataTable(); //used for batch update in sql
            memberOrgDT.Columns.Add("OrgIndex", typeof(int));
            memberOrgDT.Columns.Add("RefOrganizationID", typeof(int));
            memberOrgDT.Columns.Add("ExpectedGraduationYear", typeof(int));
            memberOrgDT.Columns.Add("SchoolReportingID", typeof(string));
            memberOrgDT.Columns.Add("IsOrganizationDeleted", typeof(bool));
            memberOrgDT.TableName = "MemberOrganization";

            //Translate domain object to custom data table
            foreach (var memberOrg in user.MemberOrganizations)
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
            
            var memberOrgList = new System.Data.SqlClient.SqlParameter("@i_MemberOrganizationTable", System.Data.SqlDbType.Structured);
            memberOrgList.Value = memberOrgDT;
            memberOrgList.ParameterName = "@i_MemberOrganizationTable";
            memberOrgList.TypeName = "dbo.MemberOrganizationTableType";

            //MemberRole table
            DataTable memberRoleDT = new DataTable(); //used for batch update in sql
            memberRoleDT.Columns.Add("RefMemberRoleID", typeof(int));
            memberRoleDT.Columns.Add("IsMemberRoleActive", typeof(bool));
            memberRoleDT.TableName = "MemberRole";

            //Translate domain object to custom data table
            foreach (var memberRole in user.MemberRoles)
            {
                DataRow dr = memberRoleDT.NewRow();
                dr["RefMemberRoleID"] = memberRole.RefMemberRoleID;
                dr["IsMemberRoleActive"] = memberRole.IsMemberRoleActive;

                memberRoleDT.Rows.Add(dr);
            }

            var memberRoleList = new System.Data.SqlClient.SqlParameter("@i_MemberRoleTable", System.Data.SqlDbType.Structured);
            memberRoleList.Value = memberRoleDT;
            memberRoleList.ParameterName = "@i_MemberRoleTable";
            memberRoleList.TypeName = "dbo.MemberRoleTableType";

            try
            {
                IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_context);

                _context.Database.ExecuteSqlCommand(string.Format("{0} {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}",
                    StoredProcedures.UpdateMember.StoredProcedureName,
                    StoredProcedures.UpdateMember.Parameters.MemberID,
                    StoredProcedures.UpdateMember.Parameters.FirstName,
                    StoredProcedures.UpdateMember.Parameters.LastName,
                    StoredProcedures.UpdateMember.Parameters.EmailAddress,
                    StoredProcedures.UpdateMember.Parameters.IsContactAllowed,
                    StoredProcedures.UpdateMember.Parameters.RegistrationSourceId,
                    StoredProcedures.UpdateMember.Parameters.RefGradeLevelID,
                    StoredProcedures.UpdateMember.Parameters.RefEnrollmentStatusID,
                    StoredProcedures.UpdateMember.Parameters.LastLoginDate,
                    StoredProcedures.UpdateMember.Parameters.DisplayName,
                    StoredProcedures.UpdateMember.Parameters.IsMemberActive,
                    StoredProcedures.UpdateMember.Parameters.ActiveDirectoryKey,
                    StoredProcedures.UpdateMember.Parameters.CommunityDisplayName,
                    StoredProcedures.UpdateMember.Parameters.IsCommunityActive,
                    StoredProcedures.UpdateMember.Parameters.WelcomeEmailSent,
                    StoredProcedures.UpdateMember.Parameters.USPostalCode,
                    StoredProcedures.UpdateMember.Parameters.YearOfBirth,
                    StoredProcedures.UpdateMember.Parameters.MemberOrgUDT,
                    StoredProcedures.UpdateMember.Parameters.MemberRoleUDT),

                    memberIDParam, firstNameParam, lastNameParam, emailAddressParam, isContactAllowedParam, registrationSourceIdParam, refGradeLevelIDParam,
                    refEnrollmentStatusIDParam, lastLoginDateParam, displayNameParam, isMemberActiveParam, activeDirectoryKeyParam, communityDisplayNameParam,
                    isCommunityActiveParam, welcomeEmailSentParam, uSPostalCodeParam, yearOfBirthParam, memberOrgList, memberRoleList);

                unitOfWork.Commit();
            }
            catch (Exception)
            {
                //_log.Error(ex.Message, ex);
                throw;
            }

            //_log.Debug(logMethodName + "End Method");
            return;
        }
    }
}
