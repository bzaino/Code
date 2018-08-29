
namespace Asa.Salt.Web.Services.Data.Constants
{
    public class StoredProcedures
    {
        /// <summary>
        /// dbo.usp_GetNextUserLessonLookupID
        /// </summary>
        public class GetNextUserLessonLookupId
        {
            public const string StoredProcedureName = "dbo.usp_GetNextUserLessonLookupID";
        }

        /// <summary>
        /// dbo.usp_DeactivateMember
        /// </summary>
        public class DeactivateMember
        {
            public const string StoredProcedureName = "usp_DeactivateMember";

            public class Parameters
            {
                public const string MemberId = "@i_MemberID";
                public const string UserName = "@i_UserName";
            }

        }

        /// <summary>
        /// dbo.usp_ActivateMember
        /// </summary>
        public class ActivateMember
        {
            public const string StoredProcedureName = "usp_ActivateMember";

            public class Parameters
            {
                public const string FirstName = "@i_FirstName";
                public const string LastName = "@i_LastName";
                public const string EmailAddress = "@i_EmailAddress";
                public const string IsContactAllowed = "@i_IsContactAllowed";
                public const string RegistrationSourceId = "@i_RefRegistrationSourceID";
                public const string InvitationToken = "@i_InvitationToken";
                public const string ActiveDirectoryKey = "@i_ActiveDirectoryKey";
                public const string RefGradeLevelID = "@i_RefGradeLevelID";
                public const string RefEnrollmentStatusID = "@i_RefEnrollmentStatusID";
                public const string YearOfBirth = "@i_YearOfBirth";
                public const string MemberOrgUDT = "@i_MemberOrganizationTable";
                public const string MemberRoleUDT = "@i_MemberRoleTable";
            }
        }

        /// <summary>
        /// dbo.usp_UpdateMember
        /// </summary>
        public class UpdateMember
        {
            public const string StoredProcedureName = "usp_UpdateMember";

            public class Parameters
            {
                public const string MemberID = "@i_MemberID";
                public const string FirstName = "@i_FirstName";
                public const string LastName = "@i_LastName";
                public const string EmailAddress = "@i_EmailAddress";
                public const string IsContactAllowed = "@i_IsContactAllowed";
                public const string RegistrationSourceId = "@i_RefRegistrationSourceID";
                public const string RefGradeLevelID = "@i_RefGradeLevelID";
                public const string RefEnrollmentStatusID = "@i_RefEnrollmentStatusID";
                public const string LastLoginDate = "@i_LastLoginDate";
                public const string DisplayName = "@i_DisplayName";
                public const string IsMemberActive = "@i_IsMemberActive";
                public const string ActiveDirectoryKey = "@i_ActiveDirectoryKey";
                public const string CommunityDisplayName = "@i_CommunityDisplayName";
                public const string IsCommunityActive = "@i_IsCommunityActive";
                public const string USPostalCode = "@i_USPostalCode";
                public const string YearOfBirth = "@i_YearOfBirth";
                public const string MemberOrgUDT = "@i_MemberOrganizationTable";
                public const string MemberRoleUDT = "@i_MemberRoleTable";
                public const string WelcomeEmailSent = "@i_WelcomeEmailSent";
            }
        }

        /// <summary>
        /// dbo.UpsertMemberOrg
        /// </summary>
        public class UpsertMemberOrg
        {
            public const string StoredProcedureName = "usp_UpsertMemberOrganization";

            public class Parameters
            {
                public const string MemberID = "@i_MemberID";
                public const string MemberOrgUDT = "@i_MemberOrganizationTable";
            }
        }

        /// <summary>
        /// dbo.UpsertMemberRole
        /// </summary>
        public class UpsertMemberRole
        {
            public const string StoredProcedureName = "usp_UpsertMemberRole";

            public class Parameters
            {
                public const string MemberID = "@i_MemberID";
                public const string MemberRoleUDT = "@i_MemberRoleTable";
                public const string ModifiedBy = "@i_ModifiedBy";
            }
        }

        /// <summary>
        /// dbo.usp_AddMemberQuiz
        /// </summary>
        public class AddMemberQuiz
        {
            public const string StoredProcedureName = "usp_AddMemberQuiz";

            public class Parameters
            {
                public const string MemberId = "@i_MemberID";
                public const string QuizName = "@i_QuizName";
                public const string QuizTakenSite = "@i_QuizTakenSite";
                public const string QuizResult = "@i_QuizResult";
                public const string QuizResponse = "@i_QuizResponse";
            }

        }

        /// <summary>
        /// dbo.usp_InsertMemberSalaryEstimatorResultAndOccupation
        /// </summary>
        public class InsertJSIMemberSalaryEstimator
        {
            public const string StoredProcedureName = "usp_InsertMemberSalaryEstimatorResultAndOccupation";

            public class Parameters
            {
                public const string RefSalaryEstimatorSchoolID = "@RefSalaryEstimatorSchoolID";
                public const string RefMajorID = "@RefMajorID";
                public const string MemberID = "@MemberID";
            }

        }

        public class RemoveMemberProfileAnswer
        {
            public const string StoredProcedureName = "usp_RemoveMemberProfileAnswer";
            public class Parameters
            {
                public const string MemberID = "@i_MemberID";
                public const string ProfileQuestionExternalID = "@i_ProfileQuestionExternalID";
            }
        }

        public class InsertMemberProfileAnswer
        {
            public const string StoredProcedureName = "usp_InsertMemberProfileAnswer";
            public class Parameters
            {
                public const string MemberID = "@i_MemberID";
                public const string ProfileAnswerExternalID = "@i_ProfileAnswerExternalID";
                public const string ProfileQuestionExternalID = "@i_ProfileQuestionExternalID";
                public const string CustomValue = "@i_CustomValue";
            }
        }

        /// <summary>
        /// dbo.usp_CostofLivingResult
        /// </summary>
        public class CostofLivingResult
        {
            public const string StoredProcedureName = "usp_CostofLivingResult";

            public class Parameters
            {
                public const string CityA_RefGeographicIndexID = "@CityA_RefGeographicIndexID";
                public const string CityB_RefGeographicIndexID = "@CityB_RefGeographicIndexID";
                public const string Salary = "@Salary";
            }
        }
    }
}
