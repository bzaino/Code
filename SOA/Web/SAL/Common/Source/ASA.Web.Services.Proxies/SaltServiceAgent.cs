using System;
using System.Collections.Generic;
using System.Linq;

using Asa.Salt.Web.Common.Types.Enums;
using ASA.Web.Services.Proxies.SALTService;

using MemberUpdateStatus = Asa.Salt.Web.Common.Types.Enums.MemberUpdateStatus;

namespace ASA.Web.Services.Proxies
{
    public class SaltServiceAgent : ISaltServiceAgent
    {
        /// <summary>
        /// Gets user Interactions
        /// </summary>
        /// <param name="userId">The member id.</param>
        /// <param name="contentType">0=All;1=Rate;2=Save</param>
        /// <returns>List{MemberContentInteraction}</returns>
        public List<MemberContentInteractionContract> GetUserInteractions(int userId, Int32 contentType)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetUserInteractionsByContentType(userId, contentType)).ToList();
            }
        }

        /// <summary>
        /// Gets user Interactions
        /// </summary>
        /// <param name="userId">The member id.</param>
        /// <param name="contentId">The id of the content to get.</param>
        /// <param name="contentType">0=All;1=Rate;2=Save</param>
        /// <returns>List{MemberContentInteraction}</returns>
        public List<MemberContentInteractionContract> GetUserInteractions(int userId, string contentId, Int32 contentType)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetUserInteractions(userId, contentId, contentType)).ToList();
            }
        }

        /// <summary>
        /// Adds an interaction
        /// </summary>
        /// <param name="interaction">The interaction to add.</param>
        /// <returns>MemberContentInteractionContract</returns>
        public MemberContentInteractionContract AddInteraction(MemberContentInteractionContract interaction)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.AddInteraction(interaction));
            }
        }

        /// <summary>
        /// Updates a single content interaction and returns the updated model.
        /// </summary>
        /// <param name="userId">The member id.</param>
        /// <param name="interactionToUpdate">The interaction to update</param>
        /// <returns>MemberContentInteractionContract</returns>
        public MemberContentInteractionContract UpdateInteraction(int userId, MemberContentInteractionContract interactionToUpdate)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.UpdateInteraction(userId, interactionToUpdate));
            }
        }

        /// <summary>
        /// Delete an interaction
        /// </summary>
        /// <param name="userId">The id of the user.</param>
        /// <param name="id">The MemberContentInteractionID of interaction to delete.</param>
        /// <returns>bool</returns>
        public bool DeleteInteraction(int userId, int id)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.DeleteInteraction(userId, id));
            }
        }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>MemberContract</returns>
        public MemberContract GetUserByUserId(int userId)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetUserByUserId(userId));
            }
        }


        /// <summary>
        /// Gets the user by username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>MemberContract</returns>
        public MemberContract GetUserByUsername(string username)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetUserByUsername(username));
            }
        }
        
        /// <summary>
        /// Gets the user by active directory key.
        /// </summary>
        /// <param name="activeDirectoryKey">The active directory key.</param>
        /// <returns>MemberContract</returns>
        public MemberContract GetUserByActiveDirectoryKey(string activeDirectoryKey)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetUserByActiveDirectoryKey(new Guid(activeDirectoryKey)));
            }
        }

        /// <summary>
        /// Registers the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>RegisterMemberResultContract</returns>
        //public static RegisterMemberResultContract RegisterUser(UserRegistrationContract user)
        public static RegisterMemberResultContract RegisterUser(UserRegistrationContract user)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.RegisterUser(user));
            }
        }

        /// <summary>
        /// Deactivates the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>bool</returns>
        public static bool DeleteUser(int userId)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.DeleteUser(userId));
            }
        }

        /// <summary>
        /// Deactivates the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>bool</returns>
        public bool DeactivateUser(int userId, string modifierUserName)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.DeactivateUser(userId, modifierUserName));
            }
        }

        /// <summary>
        /// Deletes the alert.
        /// </summary>
        /// <param name="alertId">The alert id.</param>
        /// <returns>bool</returns>
        public static bool DeleteAlert(int alertId)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.DeleteAlert(alertId));
            }
        }

        /// <summary>
        /// Deletes the loan.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="loanId">The loan id.</param>
        /// <returns>bool</returns>
        public static bool DeleteLoan(int userId, int loanId)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.DeleteLoan(userId, loanId));
            }
        }

        /// <summary>
        /// Updates the loan.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="loan">MemberReportedLoan.</param>
        /// <returns>MemberReportedLoanContract</returns>
        public static MemberReportedLoanContract UpdateLoan(int userId, MemberReportedLoanContract loan)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.UpdateLoan(userId, loan));
            }
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>MemberUpdateStatus</returns>
        public MemberUpdateStatus UpdateUser(MemberContract user)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.UpdateUser(user));
            }
        }

        /// <summary>
        /// Gets the member's alerts.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>List<MemberAlertContract></returns>
        public static List<MemberAlertContract> GetUserAlerts(int userId)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetUserAlerts(userId)).ToList();
            }
        }

        /// <summary>
        /// Gets a list of orgs based on the name
        /// </summary>
        /// <returns>OrgPagedListContract</returns>
        public static OrgPagedListContract GetOrganizations(string name, string[] orgTypeNames, int rowsPerPage, int rowOffset)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetOrgs(name, orgTypeNames, rowsPerPage, rowOffset));
            }
        }


        /// <summary>
        /// Gets the organization.
        /// </summary>
        /// <param name="opeCode">The ope code.</param>
        /// <param name="branchCode">The branch code.</param>
        /// <returns>RefOrganizationContract</returns>
        public RefOrganizationContract GetOrganization(string opeCode, string branchCode)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetOrg(opeCode, branchCode));
            }
        }

        /// <summary>
        /// Gets the organization by external org ID.
        /// </summary>
        /// <param name="externalOrgId">The external org ID.</param>
        /// <returns>RefOrganizationContract</returns>
        public RefOrganizationContract GetOrganizationByExternalOrgID(string externalOrgId)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetOrgByExternalOrgID(externalOrgId));
            }
        }

        /// <summary>
        /// Gets the reminders.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>List<PaymentReminderContract></returns>
        public static IList<PaymentReminderContract> GetUserPaymentReminders(int userId)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetUserPaymentReminders(userId));
            }
        }

        /// <summary>
        /// Saves the reminders.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="reminders">The reminders.</param>
        /// <returns>RemindersUpdateStatus</returns>
        public static RemindersUpdateStatus SaveUserPaymentReminders(int userId, IList<PaymentReminderContract> reminders)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.SaveUserPaymentReminders(userId,reminders.ToArray()));
            }
        }

        /// <summary>
        /// Imports the loan file.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="file">The file.</param>
        /// <param name="sourceName">The record source name e.g. 'KWYO'</param>
        /// <returns>List<MemberReportedLoanContract></returns>
        public static List<MemberReportedLoanContract> ImportLoanFile(int userId, byte[] file, string sourceName)
        {
           using (var client = new SaltServiceProxy())
           {
              return client.Execute(proxy => proxy.ImportUserLoans(userId, file, sourceName)).ToList();
           }
        }

        /// <summary>
        /// Gets the member reported loans.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>List<MemberReportedLoanContract></returns>
        public List<MemberReportedLoanContract> GetUserReportedLoans(int userId)
        {
           using (var client = new SaltServiceProxy())
           {
              return client.Execute(proxy => proxy.GetUserLoans(userId)).ToList();
           }
        }

        /// <summary>
        /// Gets the member reported loans.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="recordSourceId">The loan record source ID</param>
        /// <returns>List<MemberReportedLoanContract></returns>
        public List<MemberReportedLoanContract> GetUserReportedLoansRecordSourceList(int userId, string[] recordSourceNames)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetUserLoansRecordSourceList(userId, recordSourceNames)).ToList();
            }
        }

        /// <summary>
        /// Gets the survey.
        /// </summary>
        /// <param name="surveyId">The survey id.</param>
        /// <returns>SurveyContract</returns>
        public SurveyContract GetSurveyById(int surveyId)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetSurveyById(surveyId));
            }
        }

        /// <summary>
        /// Gets the survey results.
        /// </summary>
        /// <param name="surveyId">The survey id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>SurveyResponseContract</returns>
        public SurveyResponseContract GetUserSurveyResults(int surveyId, int userId)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetUserSurveyResults(surveyId,userId));
            }
        }

        /// <summary>
        /// Posts the survey.
        /// </summary>
        /// <param name="surveyResponse">The survey response.</param>
        /// <returns>bool</returns>
        public bool PostSurvey(SurveyResponseContract surveyResponse)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.PostSurvey(surveyResponse));
            }
        }

        /// <summary>
        /// Gets the lessons reference data.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>List<RefLessonLookupDataContract></returns>
        public static List<RefLessonLookupDataContract> GetLessonsReferenceData(RefLessonLookupDataTypes type)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetLessonsReferenceData(type)).ToList();
            }
        }

        /// <summary>
        /// Gets the user lessons.
        /// </summary>
        /// <param name="lessonUserId">The lesson user id.</param>
        /// <returns>List<MemberLessonContract></returns>
        public static List<MemberLessonContract> GetUserLessons(int lessonUserId)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetUserLessons(lessonUserId)).ToList();
            }
        }

        /// <summary>
        /// Creates the user lesson.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>MemberLessonContract</returns>
        public static MemberLessonContract StartUserLesson(MemberLessonContract user)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.StartUserLesson(user));
            }
        }

        /// <summary>
        /// Updates the user lesson.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>bool</returns>
        public static bool UpdateUserLesson(MemberLessonContract user)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.UpdateUserLesson(user));
            }
        }

        /// <summary>
        /// Posts the users lesson1 responses.
        /// </summary>
        /// <param name="lesson">The lesson.</param>
        /// <returns>Lesson1Contract</returns>
        public static Lesson1Contract PostLesson1(Lesson1Contract lesson)
        {
            using (var client = new SaltServiceProxy())
            {
                var toReturn= client.Execute(proxy => proxy.PostLesson1(lesson));
                return toReturn.UpdateStatus == LessonUpdateStatus.Success ? toReturn.Lesson : null;
            }  
        }

        /// <summary>
        /// Posts the lesson2.
        /// </summary>
        /// <param name="lesson">The lesson.</param>
        /// <returns>Lesson2Contract</returns>
        public static Lesson2Contract PostLesson2(Lesson2Contract lesson)
        {
            using (var client = new SaltServiceProxy())
            {
                var toReturn = client.Execute(proxy => proxy.PostLesson2(lesson));
                return toReturn.UpdateStatus == LessonUpdateStatus.Success ? toReturn.Lesson : null;
            }
        }

        /// <summary>
        /// Posts the lesson3.
        /// </summary>
        /// <param name="lesson">The lesson.</param>
        /// <returns>Lesson3Contract</returns>
        public static Lesson3Contract PostLesson3(Lesson3Contract lesson)
        {
            using (var client = new SaltServiceProxy())
            {
                var toReturn = client.Execute(proxy => proxy.PostLesson3(lesson));
                return toReturn.UpdateStatus == LessonUpdateStatus.Success ? toReturn.Lesson : null;
            }
        }

        /// <summary>
        /// Gets the user's lesson1 results.
        /// </summary>
        /// <param name="lessonUserId">The lesson user id.</param>
        /// <param name="lessonStepId">The lesson step id.</param>
        /// <returns>Lesson1Contract</returns>
        public static Lesson1Contract GetUserLesson1Results(int lessonUserId, int? lessonStepId = null)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetUserLesson1Results(lessonUserId,lessonStepId));
            }
        }

        /// <summary>
        /// Gets the user lesson2 results.
        /// </summary>
        /// <param name="lessonUserId">The lesson user id.</param>
        /// <param name="lessonStepId">The lesson step id.</param>
        /// <returns>Lesson2Contract</returns>
        public static Lesson2Contract GetUserLesson2Results(int lessonUserId, int? lessonStepId = null)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetUserLesson2Results(lessonUserId,lessonStepId));
            }
        }

        /// <summary>
        /// Gets the user lesson3 results.
        /// </summary>
        /// <param name="lessonUserId">The lesson user id.</param>
        /// <param name="lessonStepId">The lesson step id.</param>
        /// <returns>Lesson3Contract</returns>
        public static Lesson3Contract GetUserLesson3Results(int lessonUserId, int? lessonStepId = null)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetUserLesson3Results(lessonUserId, lessonStepId));
            }
        }

        /// <summary>
        /// Associates the lessons with the user.
        /// </summary>
        /// <param name="lessonUserId">The lesson user id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>bool</returns>
        public static bool AssociateLessonsWithUser(int lessonUserId, int userId)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.AssociateLessonsWithUser(lessonUserId,userId));
            }
        }

        /// <summary>
        /// Create A Loan.
        /// </summary>
        /// <param name="userid">The user id</param>
        /// <param name="loan">The loan to add</param>
        /// <returns>MemberReportedLoanContract</returns>
        public static MemberReportedLoanContract CreateLoan(int userId, MemberReportedLoanContract loan)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.CreateLoan(userId, loan));
            }
        }

        /// <summary>
        /// Deletes the user lesson responses.
        /// </summary>
        /// <param name="lessonUserId">The lesson user id.</param>
        /// <param name="lessonId">The lesson id.</param>
        /// <param name="questionId">The question id.</param>
        /// <param name="questionResponseId">The question response id.</param>
        /// <param name="groupNumber">The group number.</param>
        /// <returns>bool</returns>
        public static bool DeleteUserLessonResponses(int lessonUserId, int lessonId, int? questionId, int? questionResponseId,int? groupNumber)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.DeleteUserLessonQuestionResponses(lessonUserId,lessonId, questionId,questionResponseId,groupNumber));
            }
        }

        public static class Async
        {
            /// <summary>
            /// Updates the user asynchronously
            /// </summary>
            /// <param name="user">The user.</param>
            /// <param name="callback">The callback.</param>
            public static void UpdateUser(MemberContract user, AsyncCallback callback)
            {
                using (var client = new SaltServiceProxy())
                {
                    client.ExecuteAsync(proxy => proxy.UpdateUser(user), callback, null);
                }
            }

            /// <summary>
            /// Gets the user.
            /// </summary>
            /// <param name="userId">The user id.</param>
            /// <param name="callback">The callback.</param>
            /// <returns>MemberContract</returns>
            public static MemberContract GetUserByUserId(int userId, AsyncCallback callback)
            {
                using (var client = new SaltServiceProxy())
                {
                    return client.ExecuteAsync(proxy => proxy.GetUserByUserId(userId), callback, userId);
                }
            }
            
        }

        /// <summary>
        /// Gets the user's VLCMemberProfile.
        /// </summary>
        /// <param name="userId">The member id.</param>
        /// <returns>VLCUserProfileContract</returns>
        public static VLCUserProfileContract GetVlcProfile(int userId)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetVlcMemberProfile(userId));
            }
        }

        /// <summary>
        /// Updates the user's VLCMemberProfile.
        /// </summary>
        /// <param name="profile">The member profile.</param>
        /// <returns>bool</returns>
        public static bool UpdateVlcProfile(VLCUserProfileContract profile)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.UpdateVlcMemberProfile(profile));
            }
        }

        /// <summary>
        /// Add the user's VLC response.
        /// </summary>
        /// <param name="response">The vlc response model.</param>
        /// <returns>bool</returns>
        public static bool AddVlcResponse(VLCUserResponseContract response)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.AddVlcResponse(response));
            }
        }

        /// <summary>
        /// Add the user's JellyVisionQuiz response.
        /// </summary>
        /// <param name="response">The JellyVisionQuiz response model.</param>
        /// <returns>bool</returns>
        public bool AddJellyVisionQuizResponse(JellyVisionQuizResponseContract response)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.AddJellyVisionQuizResponse(response));
            }
        }

        /// <summary>
        /// Get COL state List.
        /// </summary>
        /// <param name=""></param>
        /// <returns>List of vCostOfLivingStateListContract></returns>
        public List<vCostOfLivingStateListContract> GetCOLStates()
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetCOLStates().ToList());
            }
        }

        /// <summary>
        /// Get COL UrbanArea List.
        /// </summary>
        /// <param name="stateId">RefStateId for urban areas to return</param>
        /// <returns>List<RefGeographicalIndexContract></returns>
        public List<RefGeographicIndexContract> GetCOLUrbanAreas(int stateId)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetCOLUrbanAreas(stateId).ToList());
            }
        }

        /// <summary>
        /// Get COL Results.
        /// </summary>
        /// <param name="CityA">The RefGeographicIndexID of CityA.</param>
        /// <param name="CityB">The RefGeographicIndexID of CityB.</param>
        /// <param name="Salary">The Salary provided.</param>
        /// <returns>COLResultsContract with COLCostBreakDownModel and NeededSalary & percentageSalaryChange</returns>
        public COLResultsContract GetCOLResults(int cityA, int cityB, decimal salary)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetCOLResults(cityA, cityB, salary));
            }
        }

        /// <summary>
        /// Get JSI major List.
        /// </summary>
        /// <param name=""></param>
        /// <returns>List<RefMajorContract></returns>
        public List<RefMajorContract> GetJSIMajor()
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetRefMajors().majors.ToList());
            }
        }

        /// <summary>
        /// Get JSI state List.
        /// </summary>
        /// <param name=""></param>
        /// <returns>List<RefStateContract></returns>
        public List<RefStateContract> GetJSIState()
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetRefStates().states.ToList());
            }
        }

        /// <summary>
        /// Get JSI school List.
        /// </summary>
        /// <param name="majorId">JSI RefMajorId</param>
        /// <param name="stateId">JSI RefStateId</param>
        /// <returns>List<JSISchoolMajorContract></returns>
        public List<JSISchoolMajorContract> GetJSISchoolMajor(int majorId, int stateId)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetJSISchools(majorId, stateId).schools.ToList());
            }
        }

        /// <summary>
        /// Post JSI quiz answers List.
        /// </summary>
        /// <param name="answers">a JSIQuizAnswerContract</param>
        /// <returns>List<JSIQuizResultContract></returns>
        public List<JSIQuizResultContract> PostJSIResponse(JSIQuizAnswerContract answers)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.PostJSIResponse(answers).results.ToList());
            }
        }

        /// <summary>
        /// Gets profile questions List.
        /// </summary>
        /// <returns>List<RefProfileQuestionContract></returns>
        public List<RefProfileQuestionContract> GetAllProfileQuestions()
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetAllProfileQuestions().ToList());
            }
        }

        /// <summary>
        /// Gets profile answers List.
        /// </summary>
        /// <returns>List<RefProfileAnswerContract></returns>
        public List<RefProfileAnswerContract> GetAllProfileAnswers()
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetAllProfileAnswers().ToList());
            }
        }

        /// <summary>
        /// Gets profile responses List.
        /// </summary>
        /// <returns>List<MemberProfileQAContract></returns>
        public List<MemberProfileQAContract> GetProfileResponses(int memberId)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetProfileResponses(memberId).ToList());
            }
        }

        public static bool UpdateMemberProfileResponses(int memberId, IList<MemberProfileQAContract> profileResponses)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.UpdateUserProfileResponses(memberId, profileResponses.ToArray()));
            }
        }

        /// <summary>
        /// Updates Member Organization/s affiliation
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="memberOrgAffiliations"></param>
        /// <returns></returns>
        public static bool UpdateMemberOrgAffiliation(int memberId, IList<MemberOrganizationContract> memberOrgAffiliations)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.UpdateUserOrgAffiliation(memberId, memberOrgAffiliations.ToArray()));
            }
        }

        /// <summary>
        /// Gets Organizations the Member is affilliated with
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<MemberOrganizationContract> GetMemberOrganizations(int userId)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetMemberOrganizations(userId)).ToList();
            }
        }

        /// <summary>
        /// Gets Member products.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List<MemberProductContract></returns>
        public static List<MemberProductContract> GetMemberProducts(int userId)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetMemberProducts(userId)).ToList();
            }
        }

        /// <summary>
        /// Gets Organization products.
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns>List<RefOrganizationProductContract></returns>
        public List<RefOrganizationProductContract> GetOrganizationProducts(int orgId)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetOrgProducts(orgId)).ToList();
            }
        }

        /// <summary>
        /// Gets Organization products given a member id.
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>List<RefOrganizationProductContract></returns>
        public List<RefOrganizationProductContract> GetOrganizationProductsByMemberId(int userId)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetOrgProductsByMemberId(userId)).ToList();
            }
        }

        /// <summary>
        /// Adds Member product.
        /// </summary>
        /// <param name="memberProduct"></param>
        /// <returns>bool</returns>
        public static bool AddMemberProduct(MemberProductContract memberProduct)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.AddMemberProduct(memberProduct));
            }
        }

        /// <summary>
        /// Updates Member product.
        /// </summary>
        /// <param name="memberProduct"></param>
        /// <returns>bool</returns>
        public static bool UpdateMemberProduct(MemberProductContract memberProduct)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.UpdateMemberProduct(memberProduct));
            }
        }

        /// <summary>
        /// Inserts or Updates user Scholarship question/s responses 
        /// </summary>
        /// <param name="MemberID">ID</param>
        /// <param name="responses">IList<MemberQA></param>
        /// <returns>bool</returns>
        public static bool UpsertQuestionAnswer(int MemberID, IList<MemberQAContract> choicesList)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.UpsertQuestionAnswer(MemberID, choicesList.ToArray()));
            }
        }

        /// <summary>
        /// Gets Member Scholarship Question Answers for a given SourceID
        /// </summary>
        /// <param name="MemberID">Nullable<Int32></param>
        /// <param name="EmailAddress">string</param>
        /// <param name="SourceID">int</param>
        /// <returns>List<vMemberQuestionAnswer></returns>
        public static List<vMemberQuestionAnswerContract> GetMemberQuestionAnswer(Nullable<Int32> MemberID, string EmailAddress, int SourceID)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetMemberQuestionAnswer(MemberID, EmailAddress, SourceID)).ToList();
            }
        }

        /// <summary>
        /// Gets Member Todos
        /// </summary>
        /// <param name="memberID">Nullable<Int32></param>
        /// <returns>List<MemberToDoListContract></returns>
        public static List<MemberToDoListContract> GetMemberToDos(int memberID)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetMemberToDos(memberID)).ToList();
            }
        }

        public static bool UpsertMemberToDo(MemberToDoListContract todoContract)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.UpsertMemberToDo(todoContract));
            }
        }
    }
}
