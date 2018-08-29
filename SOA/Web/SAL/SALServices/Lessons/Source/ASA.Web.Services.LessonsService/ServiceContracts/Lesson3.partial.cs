using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using ASA.Web.Services.LessonsService.DataContracts;
using ASA.Web.Services.LessonsService.DataContracts.Lesson3;
using ASA.Web.Services.Proxies;
using ASA.Web.Services.Proxies.SALTService;
using Asa.Salt.Web.Common.Types.Constants;
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
        [WebInvoke(UriTemplate = "Lesson3/User/IndividualId/{activeDirectoryKey}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public User GetLesson3UserByActiveDirectoryKey(string activeDirectoryKey)
        {
            return GetUserByActiveDirectoryKey(activeDirectoryKey);
        }

        /// <summary>
        /// Creates the lesson3 user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson3/User/", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public User CreateLesson3User(User user)
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
                User lesson3User = new User();
                lesson3User.AddError("Model is invalid!");
                return lesson3User;
            }
            return CreateUser(user, LessonTypes.OwnYourStudentLoans.ToString());
        }

        /// <summary>
        /// Gets the lesson3 user.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson3/User/{userId}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public User GetLesson3User(string userId)
        {
            return GetUserByLessonId(LessonTypes.OwnYourStudentLoans.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Creates the or update lesson3 user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson3/User/{userId}", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public User CreateOrUpdateLesson3User(User user, string userId)
        {
            var userGuid = GetUserLessonId();
            return !userGuid.HasValue ? CreateUser(user, LessonTypes.OwnYourStudentLoans.ToString(CultureInfo.InvariantCulture)) : UpdateLesson3User(user, userGuid.Value.ToString());
        }

        /// <summary>
        /// Updates the user lesson.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson3/User/{userId}", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public User UpdateLesson3User(User user, string userId)
        {

            if (userId == null || string.Empty == userId)
            {
                user.AddError("User ID is undefined!");
                return user;
            }
            SaltServiceAgent.UpdateUserLesson(user.ToDataContract(LessonTypes.OwnYourStudentLoans));

            return user;
        }

        /// <summary>
        /// Gets the loan types.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson3/LoanType/", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public IList<LoanType> GetLoanTypes()
        {
            var lessonUserId = GetUserLessonId();
            return !lessonUserId.HasValue ? null : SaltServiceAgent.GetUserLesson3Results(lessonUserId.Value, LessonSteps.Lesson3.YourBalance).LoanTypes.ToDomainObject();
        }

        /// <summary>
        /// Posts a favorite repayment plan.
        /// </summary>
        /// <param name="favorite">The favorite.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson3/Favorite/", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public Favorite PostFavoritePaymentPlan(Favorite favorite)
        {
            var lessonUserId = GetUserLessonId();

            if (!lessonUserId.HasValue)
                return null;

            lock (_lockObject)
            {
                var result = SaltServiceAgent.PostLesson3(
                    new Lesson3()
                        {
                            FavoriteRepaymentPlans = new List<Favorite>(SaltServiceAgent.GetUserLesson3Results(lessonUserId.Value, LessonSteps.Lesson3.ReviewYourFavorites).ToDomainObject().FavoriteRepaymentPlans) {favorite},
                            User = new User() {UserId = lessonUserId.Value}
                        }.ToDataContract()).ToDomainObject();

                return favorite;
            }
        }

        /// <summary>
        /// Updates the favorite payment plan.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="favorite">The favorite.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson3/Favorite/{id}", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public bool UpdateFavoritePaymentPlan(string id, Favorite favorite)
        {
            //No need to update favorite plans. They should only insert or update.
            return true;
        }

        /// <summary>
        /// Gets the favorite payment plans.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson3/Favorite/", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public IList<Favorite> GetFavoritePaymentPlans()
        {
             var lessonUserId = GetUserLessonId();

            return !lessonUserId.HasValue ? null : SaltServiceAgent.GetUserLesson3Results(lessonUserId.Value, LessonSteps.Lesson3.ReviewYourFavorites).ToDomainObject().FavoriteRepaymentPlans;
        }

        /// <summary>
        /// Deletes a favorite payment plan.
        /// </summary>
        /// <param name="favoritePlanId">The favorite plan id.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson3/Favorite/{favoritePlanId}", Method = "DELETE", ResponseFormat = WebMessageFormat.Json)]
        public bool DeleteFavoritePaymentPlan(string favoritePlanId, Favorite favorite)
        {
            var lessonUserId = GetUserLessonId();

            if (!lessonUserId.HasValue)
                return false;

            lock (_lockObject)
            {
                var result = SaltServiceAgent.PostLesson3(
                   new Lesson3()
                   {
                       FavoriteRepaymentPlans = new List<Favorite>(SaltServiceAgent.GetUserLesson3Results(lessonUserId.Value, LessonSteps.Lesson3.ReviewYourFavorites).ToDomainObject().FavoriteRepaymentPlans.Where(f => !f.Equals(favorite)).ToList()),
                       User = new User() { UserId = lessonUserId.Value }
                   }.ToDataContract()).ToDomainObject();
            }

            return true;
        }

        /// <summary>
        /// Posts a loan type.
        /// </summary>
        /// <param name="loanType">Type of the loan.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson3/LoanType/", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public LoanType PostLoanType(LoanType loanType)
        {
            var lessonUserId = GetUserLessonId();

            if (!lessonUserId.HasValue)
                return null;

            lock (_lockObject)
            {
                var result = SaltServiceAgent.PostLesson3(
                    //add new loanType object to list of LoanType and update lesson
                      new Lesson3()
                      {
                          LoanTypes = new List<LoanType>() { loanType },
                          User = new User() { UserId = lessonUserId.Value }
                      }.ToDataContract()
                ).ToDomainObject();
            }
            return loanType;
        }

        /// <summary>
        /// Updates the type of the loan.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="loanType">Type of the loan.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson3/LoanType/{id}", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public bool UpdateLoanType(string id, LoanType loanType)
        {
            return PostLoanType(loanType) != null;
        }

        /// <summary>
        /// Deletes the type of the loan.
        /// </summary>
        /// <param name="loanTypeId">The loan type id.</param>
        /// <param name="loanType">Type of the loan.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson3/LoanType/{loanTypeId}", Method = "DELETE", ResponseFormat = WebMessageFormat.Json)]
        public bool DeleteLoanType(string loanTypeId, LoanType loanType)
        {
            var lessonUserId = GetUserLessonId();

            if (!lessonUserId.HasValue)
                return false;

            lock (_lockObject)
            {
                var loanTypes = SaltServiceAgent.GetUserLesson3Results(lessonUserId.Value, LessonSteps.Lesson3.YourBalance).ToDomainObject().LoanTypes.ToList();
                loanTypes.Remove(loanTypes.First(l => l.Equals(loanType)));

                var result = SaltServiceAgent.PostLesson3(
                   new Lesson3()
                   {
                       LoanTypes = new List<LoanType>(loanTypes),
                       User = new User() { UserId = lessonUserId.Value }
                   }.ToDataContract()).ToDomainObject();
            }

            return true;
        }

        /// <summary>
        /// Deletes the loan types.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson3/LoanType/", Method = "DELETE", ResponseFormat = WebMessageFormat.Json)]
        public bool DeleteLoanTypes()
        {
            var lessonUserId = GetUserLessonId();

            return lessonUserId.HasValue && SaltServiceAgent.DeleteUserLessonResponses(lessonUserId.Value,LessonTypes.OwnYourStudentLoans,LessonQuestions.Lesson3.WhatTypeOfRepaymentOptionsWouldYouLikeToExplore,null, null);
        }

        /// <summary>
        /// Gets the faster repayment options.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson3/FasterRepayment/", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public IList<FasterRepayment> GetFasterRepaymentOptions()
        {
            var lessonUserId = GetUserLessonId();
            return !lessonUserId.HasValue ? null : SaltServiceAgent.GetUserLesson3Results(lessonUserId.Value, LessonSteps.Lesson3.PayItDownFaster).ToDomainObject().FasterRepaymentOptions;
        }

        /// <summary>
        /// Updates the faster repayment options.
        /// </summary>
        /// <param name="fasterRepayment">The faster repayment.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson3/FasterRepayment/", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public FasterRepayment PostFasterRepaymentOptions(FasterRepayment fasterRepayment)
        {
            var lessonUserId = GetUserLessonId();
           
            if (!lessonUserId.HasValue)
                return fasterRepayment;

            lock (_lockObject)
            {
                SaltServiceAgent.PostLesson3(new Lesson3()
                    {
                        FasterRepaymentOptions = new List<FasterRepayment>() {fasterRepayment},
                        User = new User() {UserId = lessonUserId.Value}
                    }.ToDataContract());
            }

            return fasterRepayment;
        }

        /// <summary>
        /// Updates the faster repayment options.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="fasterRepayment">The faster repayment.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson3/FasterRepayment/{id}", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public FasterRepayment UpdateFasterRepaymentOptions(string id, FasterRepayment fasterRepayment)
        {
            return PostFasterRepaymentOptions(fasterRepayment);
        }

        /// <summary>
        /// Gets the lower payment repayment options.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson3/LowerPayment/", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public IList<LowerPayment> GetLowerPaymentRepaymentOptions()
        {
            var lessonUserId = GetUserLessonId();
            return !lessonUserId.HasValue ? null : SaltServiceAgent.GetUserLesson3Results(lessonUserId.Value, LessonSteps.Lesson3.LoweringYourPayments).ToDomainObject().LowerPaymentOptions;
        }

        /// <summary>
        /// Posts the lower payment options.
        /// </summary>
        /// <param name="lowerPayment">The lower payment.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson3/LowerPayment/", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public LowerPayment PostLowerPaymentOptions(LowerPayment lowerPayment)
        {
            var lessonUserId = GetUserLessonId();

            if (!lessonUserId.HasValue)
                return lowerPayment;

            lock (_lockObject)
            {

                SaltServiceAgent.PostLesson3(new Lesson3()
                    {
                        LowerPaymentOptions = new List<LowerPayment>() {lowerPayment},
                        User = new User() {UserId = lessonUserId.Value}
                    }.ToDataContract());
            }

            return lowerPayment;
        }

        /// <summary>
        /// Posts the loan deferment options.
        /// </summary>
        /// <param name="deferment">The deferment.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson3/Deferment/", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public Deferment PostLoanDefermentOptions(Deferment deferment)
        {
            var lessonUserId = GetUserLessonId();

            if (!lessonUserId.HasValue)
                return deferment;

            lock (_lockObject)
            {
                SaltServiceAgent.PostLesson3(new Lesson3()
                    {
                        DefermentOptions = new List<Deferment>() {deferment},
                        User = new User() {UserId = lessonUserId.Value}
                    }.ToDataContract());
            }

            return deferment;
        }

        /// <summary>
        /// Gets the loan deferment options.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Lesson3/Deferment/", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public IList<Deferment> GetLoanDefermentOptions()
        {
            var lessonUserId = GetUserLessonId();
            return !lessonUserId.HasValue ? null : SaltServiceAgent.GetUserLesson3Results(lessonUserId.Value, LessonSteps.Lesson3.DefermentAndForebearance).ToDomainObject().DefermentOptions;
        }
    }
}
