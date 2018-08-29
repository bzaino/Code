using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using Asa.Salt.Web.Common.Types.Constants;
using ASA.Web.Services.LessonsService.DataContracts;
using ASA.Web.Services.LessonsService.DataContracts.Lesson2;
using ASA.Web.Services.Proxies;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ASA.Web.Services.LessonsService.ServiceContracts
{
    public partial class Lessons
    {

        /// <summary>
        /// Gets the user by the active directory id.
        /// </summary>
        /// <param name="activeDirectoryKey">The active directory key.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson2/User/IndividualId/{activeDirectoryKey}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public User GetLesson2UserByActiveDirectoryKey(string activeDirectoryKey)
        {
            return GetUserByActiveDirectoryKey(activeDirectoryKey);
        }

        /// <summary>
        /// Creates the lesson2 user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson2/User/", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public User CreateLesson2User(User user)
        {
            //next sanitize the data before further steps

            foreach (PropertyInfo propertyInfo in user.GetType().GetProperties())
            {
                //get the value of the property 

                object value = propertyInfo.GetValue(user, null);
                //do sanitization only for string 
                if (value.GetType() == typeof(string))
                {
                    value = Regex.Replace(value.ToString(), @"[^A-Za-z0-9 _\.-]", "");
                    propertyInfo.SetValue(user, value, null);
                }
            }

            if (!user.IsValid())
            {
                User lesson2User = new User();
                lesson2User.AddError("Model is invalid!");
                return lesson2User;
            }
                return CreateUser(user, LessonTypes.MasterYourPlastic.ToString(CultureInfo.InvariantCulture));

        }

        /// <summary>
        /// Gets the lesson2 user.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson2/User/", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public User GetLesson2User()
        {
            return GetUserByLessonId(LessonTypes.MasterYourPlastic.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Creates the or update lesson2 user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson2/User/{userId}", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public User CreateOrUpdateLesson2User(User user,string userId)
        {
            if (user.IsValid())
            {
                var userGuid = GetUserLessonId();
                return !userGuid.HasValue ? CreateUser(user, LessonTypes.MasterYourPlastic.ToString(CultureInfo.InvariantCulture)) : UpdateLesson2User(user, userGuid.Value.ToString());
            }
            else {
                user.AddError("User object is not valid!");
            }

            return user; 
        }

        /// <summary>
        /// Updates the user lesson.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson2/User/{userId}", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public User UpdateLesson2User(User user, string userId)
        {

            if (userId == null || string.Empty == userId)
            {
                user.AddError("User ID is undefined!");
                return user;
            }
             SaltServiceAgent.UpdateUserLesson(user.ToDataContract(LessonTypes.MasterYourPlastic));
           
            return user;
        }

        /// <summary>
        /// Gets the card information.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson2/CardInformation/", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public CardInformation GetCardInformation()
        {
            var lessonUserId = GetUserLessonId();
            return !lessonUserId.HasValue ? null : SaltServiceAgent.GetUserLesson2Results(lessonUserId.Value).ToDomainObject().CurrentBalance;
        }

        /// <summary>
        /// Gets the debt reduction options.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson2/DebtReductionOptions/", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public DebtReductionOptions GetDebtReductionOptions()
        {
            var lessonUserId = GetUserLessonId();
            return !lessonUserId.HasValue ? null : SaltServiceAgent.GetUserLesson2Results(lessonUserId.Value).ToDomainObject().DebtReductionOptions;
        }

        /// <summary>
        /// Posts the card information.
        /// </summary>
        /// <param name="cardinformation">The cardinformation.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson2/CardInformation/", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public CardInformation PostCardInformation(CardInformation cardinformation)
        {
            var lessonUserId = GetUserLessonId();

            if (!lessonUserId.HasValue)
                return null;

           var result= SaltServiceAgent.PostLesson2(new Lesson2() {CurrentBalance = cardinformation, User = new User(){UserId = lessonUserId.Value}}.ToDataContract()).ToDomainObject();

            return result.CurrentBalance;
        }

        /// <summary>
        /// Posts the card information.
        /// </summary>
        /// <param name="debtReductionOptions">The debt reduction options.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson2/DebtReductionOptions/", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public DebtReductionOptions PostDebtReductionOptions(DebtReductionOptions debtReductionOptions)
        {
            var lessonUserId = GetUserLessonId();

            if (!lessonUserId.HasValue)
                return null;

            var result =SaltServiceAgent.PostLesson2(new Lesson2() { DebtReductionOptions = debtReductionOptions, User = new User() { UserId = lessonUserId.Value } }.ToDataContract()).ToDomainObject();

            return result.DebtReductionOptions;
        }

        /// <summary>
        /// Gets the recurring expenses.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson2/RecurringExpense/", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public List<RecurringExpense> GetRecurringExpenses()
        {
            var lessonUserId = GetUserLessonId();

            return !lessonUserId.HasValue
                       ? null
                       : SaltServiceAgent.GetUserLesson2Results(lessonUserId.Value)
                                         .RecurringExpenses.ToRecurringExpenseDomainObject();
        }

        /// <summary>
        /// Posts the recurring expense.
        /// </summary>
        /// <param name="recurringExpense">The recurring expense.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson2/RecurringExpense/", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public RecurringExpense PostRecurringExpense(RecurringExpense recurringExpense)
        {
            var lessonUserId = GetUserLessonId();

            if (!lessonUserId.HasValue || recurringExpense.Value <= 0)
                return null;

            lock (_lockObject)
            {
                var result = SaltServiceAgent.PostLesson2(
                    new Lesson2()
                    {
                        RecurringExpenses = new List<RecurringExpense>(SaltServiceAgent.GetUserLesson2Results(lessonUserId.Value, LessonSteps.Lesson2.RecurringExpenses)
                                    .ToDomainObject()
                                    .RecurringExpenses) { recurringExpense },
                        User = new User() { UserId = lessonUserId.Value }
                    }.ToDataContract()).ToDomainObject();

                return result.RecurringExpenses.First(re => re.Equals(recurringExpense));
            }
        }

        /// <summary>
        /// Updates the recurring expense.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="recurringExpense">The recurring expense.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson2/RecurringExpense/{id}", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public bool UpdateRecurringExpense(string id,RecurringExpense recurringExpense)
        {
            return PostRecurringExpense(recurringExpense) != null;
        }

        /// <summary>
        /// Updates the one time expense.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="oneTimeExpense">The one time expense.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson2/OneTimeExpense/{id}", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public bool UpdateOneTimeExpense(string id, OneTimeExpense oneTimeExpense)
        {
            return PostOneTimeExpense(oneTimeExpense) != null;
        }

        /// <summary>
        /// Gets the one time expenses.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson2/OneTimeExpense/", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public List<OneTimeExpense> GetOneTimeExpenses()
        {
            var lessonUserId = GetUserLessonId();

            return !lessonUserId.HasValue ? null : SaltServiceAgent.GetUserLesson2Results(lessonUserId.Value).ToDomainObject().OneTimeExpenses.ToList();
        }

        /// <summary>
        /// Gets the imported expenses.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson2/ImportedExpenses/", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public List<Expense> GetImportedExpenses()
        {
            var lessonUserId = GetUserLessonId();

            return !lessonUserId.HasValue ? null : SaltServiceAgent.GetUserLesson2Results(lessonUserId.Value).ToDomainObject().ImportedExpenses.ToList();
        }

        /// <summary>
        /// Posts the one time expense.
        /// </summary>
        /// <param name="oneTimeExpense">The one time expense.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson2/OneTimeExpense/", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public OneTimeExpense PostOneTimeExpense(OneTimeExpense oneTimeExpense)
        {
            var lessonUserId = GetUserLessonId();

            if (!lessonUserId.HasValue || oneTimeExpense.Value <= 0)
                return null;

            lock (_lockObject)
            {
                var result = SaltServiceAgent.PostLesson2(
                    new Lesson2()
                    {
                        OneTimeExpenses = new List<OneTimeExpense>(SaltServiceAgent.GetUserLesson2Results(lessonUserId.Value, LessonSteps.Lesson2.OneTimePurchases)
                                    .ToDomainObject()
                                    .OneTimeExpenses) { oneTimeExpense },
                        User = new User() { UserId = lessonUserId.Value }
                    }.ToDataContract()).ToDomainObject();

                return result.OneTimeExpenses.First(oe => oe.Equals(oneTimeExpense));
            }
        }

        /// <summary>
        /// Deletes the one time expense.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson2/OneTimeExpense/{expenseId}", Method = "DELETE", ResponseFormat = WebMessageFormat.Json)]
        public bool DeleteOneTimeExpense(string expenseId, OneTimeExpense oneTimeExpense)
        {
            var lessonUserId = GetUserLessonId();

            if (!lessonUserId.HasValue)
                return false;

            lock (_lockObject)
            {
                var oneTimeExpenses = SaltServiceAgent.GetUserLesson2Results(lessonUserId.Value, LessonSteps.Lesson2.OneTimePurchases).ToDomainObject().OneTimeExpenses.ToList();
                oneTimeExpenses.Remove(oneTimeExpenses.First(e => e.Equals(oneTimeExpense)));

                SaltServiceAgent.PostLesson2(
                    new Lesson2()
                    {
                        OneTimeExpenses = new List<OneTimeExpense>(oneTimeExpenses),
                        User = new User() { UserId = lessonUserId.Value }
                    }.ToDataContract()).ToDomainObject();
            }
            return true;
        }

        /// <summary>
        /// Deletes the one time expenses.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson2/OneTimeExpense/", Method = "DELETE", ResponseFormat = WebMessageFormat.Json)]
        public bool DeleteOneTimeExpenses()
        {
            var lessonUserId = GetUserLessonId();

            return lessonUserId.HasValue && SaltServiceAgent.DeleteUserLessonResponses(lessonUserId.Value, LessonTypes.MasterYourPlastic ,LessonQuestions.Lesson2.AreYouMakingAnyOneTimePurchases, null, null);
        }

        /// <summary>
        /// Deletes the recurring expense.
        /// </summary>
        /// <param name="expenseId">The expense id.</param>
        /// <param name="recurringExpense">The recurring expense.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson2/RecurringExpense/{expenseId}", Method = "DELETE", ResponseFormat = WebMessageFormat.Json)]
        public bool DeleteRecurringExpense(string expenseId, RecurringExpense recurringExpense)
        {
            var lessonUserId = GetUserLessonId();

            if (!lessonUserId.HasValue)
                return false;

            lock (_lockObject)
            {
                var recurringExpenses = SaltServiceAgent.GetUserLesson2Results(lessonUserId.Value, LessonSteps.Lesson2.RecurringExpenses).ToDomainObject().RecurringExpenses.ToList();
                recurringExpenses.Remove(recurringExpenses.First(e => e.Equals(recurringExpense)));

                SaltServiceAgent.PostLesson2(
                    new Lesson2()
                    {
                        RecurringExpenses = new List<RecurringExpense>(recurringExpenses),
                        User = new User() { UserId = lessonUserId.Value }
                    }.ToDataContract()).ToDomainObject();
            }
            return true;
        }

        /// <summary>
        /// Deletes the recurring expenses.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson2/RecurringExpense/", Method = "DELETE", ResponseFormat = WebMessageFormat.Json)]
        public bool DeleteRecurringExpenses()
        {
            var lessonUserId = GetUserLessonId();

            return lessonUserId.HasValue && SaltServiceAgent.DeleteUserLessonResponses(lessonUserId.Value, LessonTypes.MasterYourPlastic,LessonQuestions.Lesson2.WhatAreYourRecurringExpenses, null, null);
        }
    }
}
