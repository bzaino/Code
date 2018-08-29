using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Web;
using Asa.Salt.Web.Common.Types.Constants;
using ASA.Web.Services.LessonsService.DataContracts;
using ASA.Web.Services.LessonsService.DataContracts.Lesson1;
using ASA.Web.Services.Proxies;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ASA.Web.Services.LessonsService.ServiceContracts
{
    public partial class Lessons
    {
        /// <summary>
        /// Creates the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson1User", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public Lesson1 CreateUserAndStartLesson1(User user)
        {
            //next sanitize the data before further steps

            foreach (PropertyInfo propertyInfo in user.GetType().GetProperties())
            {
                //get the value of the property 

                object value = propertyInfo.GetValue(user, null);
                //do sanitization only for string 
                if (value.GetType() == typeof(string))
                {
                    value = Regex.Replace(value.ToString(), @"[^A-Za-z0-9 _\.-{}]", "");
                    propertyInfo.SetValue(user, value, null);
                }
            }
            
            if (!user.IsValid()) { 
                Lesson1 lesson1 = new Lesson1();
                lesson1.AddError("Model is invalid!");
                return lesson1;
            }

            var lessonUser = CreateUser(user, LessonTypes.HowDoesYourCashFlow.ToString(CultureInfo.InvariantCulture));
            var lesson1User = new Lesson1()
            {
                IndividualId = lessonUser.IndividualId,
                UserId = lessonUser.UserId,
                User = lessonUser
            };

            return lesson1User;
        }

        /// <summary>
        /// Gets the user lesson1 results.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson1User/", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public Lesson1 GetUserLesson1Results()
        {
            var lessonUserId = GetUserLessonId();
            if (lessonUserId.HasValue)
            {
                var lessonUser = SaltServiceAgent.GetUserLessons(lessonUserId.Value).ToArray().ToDomainObject();
                var lesson1Results = SaltServiceAgent.GetUserLesson1Results(lessonUserId.Value).ToDomainObject();

                if (lesson1Results.User.UserId <= 0)
                {
                    lesson1Results.User.UserId = GetUserLessonId().HasValue ? GetUserLessonId().Value : 0;
                    return CreateUserAndStartLesson1(lesson1Results.User);
                }

                lesson1Results.UserId = lessonUser.UserId;
                lesson1Results.User = lessonUser;

                return lesson1Results;
            }

            return null;
        }

        /// <summary>
        /// Posts the lesson1.
        /// </summary>
        /// <param name="lesson1">The lesson1.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson1User", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public bool PostLesson1(Lesson1 lesson1)
        {
            if (lesson1.IsValid())
            {
                lock (_lockObject)
                {
                    SaltServiceAgent.PostLesson1(lesson1.ToDataContract());

                    return true;
                }
            }
            else { 
                lesson1.AddError("Lesson object is not valid");
                return false; 
            }


        }
    }
}