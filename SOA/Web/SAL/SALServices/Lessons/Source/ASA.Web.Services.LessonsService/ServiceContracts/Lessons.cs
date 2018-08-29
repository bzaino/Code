using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ASA.Web.Services.ASAMemberService;
using ASA.Web.Services.LessonsService.DataContracts;
using ASA.Web.Services.LessonsService.DataContracts.Lesson1;
using ASA.Web.Services.LessonsService.DataContracts.Lesson2;
using ASA.Web.Services.Proxies;
using ASA.Web.Services.Proxies.SALTService;
using Asa.Salt.Web.Common.Types.Constants;
using Asa.Salt.Web.Common.Types.Enums;

using ASA.Web.WTF;
using ASA.Web.WTF.Integration;

namespace ASA.Web.Services.LessonsService.ServiceContracts
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class Lessons
    {

        /// <summary>
        /// A synch lock object
        /// </summary>
        private static object _lockObject = new object();

        /// <summary>
        /// Gets the frequencies.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Frequency/", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public IList<Frequency> GetFrequencies()
        {
            return
                SaltServiceAgent.GetLessonsReferenceData(RefLessonLookupDataTypes.Frequencies)
                                .ToFrequency();
        }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="lessonId">The lesson id.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "User/{userId}/{lessonId}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public User GetUser(string userId, string lessonId)
        {
            var toReturn = SaltServiceAgent.GetUserLessons(Convert.ToInt32(userId));
            return toReturn == null || !toReturn.Any(ml=>ml.LessonId==Convert.ToInt16(lessonId))? null : toReturn.ToArray().ToDomainObject();
        }

        /// <summary>
        /// Gets the user by lesson id.
        /// </summary>
        /// <param name="lessonId">The lesson id.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "User/{lessonId}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public User GetUserByLessonId(string lessonId)
        {
            var userId = GetUserLessonId();

            //var user = (!userId.HasValue ? null : GetUser(userId.Value.ToString(), lessonId)) ??
            //           CreateUser(new User() {UserId = userId.Value},lessonId);

            //cov-10329 - rework/expanded above statement to fix null value of userId being passed
            if (userId.HasValue)
            {
                var user = GetUser(userId.Value.ToString(), lessonId);
                if (user == null)
                {
                    return CreateUser(new User() { UserId = userId.Value }, lessonId);
                }
                else
                {
                    return user;
                }
            }
            else
            {
                return new User();
            }
        }

        /// <summary>
        /// Retaking a lesson.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="lessonId">The lesson id.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "User/{lessonId}", Method = "DELETE", ResponseFormat = WebMessageFormat.Json)]
        public bool RetakeLesson(string lessonId)
        {
            var userId = GetUserLessonId();
            var lesson = Convert.ToInt32(lessonId);

            return userId.HasValue 
                && SaltServiceAgent.DeleteUserLessonResponses(userId.Value, lesson, null, null, null) 
                && SaltServiceAgent.UpdateUserLesson(new MemberLessonContract()
                {
                    LessonId = lesson,
                    CurrentStep = 0,
                    LessonUserId = userId.Value
                }) ;
        }

        /// <summary>
        /// Creates the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="lessonId">The lesson id.</param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "User/{lessonId}", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public User CreateUser(User user, string lessonId )
        {
            User lessonUser = new User();
            int validatedLessonId;
            bool lessonIdIsValid = Int32.TryParse(lessonId, out validatedLessonId);
            if (user.IsValid())
            {
                if (lessonIdIsValid)
                {
                    lock (_lockObject)
                    {
                        lessonUser = SaltServiceAgent.StartUserLesson(user.ToDataContract(validatedLessonId)).ToDomainObject();
                        return lessonUser;
                    }
                }
                else
                {
                    user.AddError("Invalid lessonId");
                }
            }

            //return invalidated incoming user agumented with errormodel
            return user; 
        }

        /// <summary>
        /// Gets the user by the active directory id.
        /// </summary>
        /// <param name="activeDirectoryKey">The active directory key.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "User/IndividualId/{activeDirectoryKey}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public User GetUserByActiveDirectoryKey(string activeDirectoryKey)
        {
            var user = IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent").GetUserByActiveDirectoryKey(activeDirectoryKey);
            if (user != null)
            {
                if (user.MemberLessons.Any())
                {
                    var toReturn = user.MemberLessons.ToDomainObject();
                    return toReturn;
                }
            }

            return null;
        }

        /// <summary>
        /// Associates the lesson with the registered user.
        /// </summary>
        /// <param name="userLessonId">The user lesson id.</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "User/AssociateRegisteredUser/{userLessonId}", Method = "POST")]
        public void AssociateLessonWithRegisteredUser(string userLessonId)
        {
            int validatedUserLessonId;
            bool userLessonIdIsValid = Int32.TryParse(userLessonId, out validatedUserLessonId);

            if (userLessonIdIsValid)
            {
                var memberAdapter = new AsaMemberAdapter();
                var userId = memberAdapter.GetMemberIdFromContext();

                SaltServiceAgent.AssociateLessonsWithUser(Convert.ToInt32(validatedUserLessonId), userId);
            }
            
        }

        /// <summary>
        /// Gets the user lesson id.
        /// </summary>
        /// <returns></returns>
        private int? GetUserLessonId()
        {
            if (HttpContext.Current.Request.Cookies["UserGuid"] != null)
            {
                return Convert.ToInt32(HttpContext.Current.Request.Cookies["UserGuid"].Value);
            }

            return null;
        }
    }
}
