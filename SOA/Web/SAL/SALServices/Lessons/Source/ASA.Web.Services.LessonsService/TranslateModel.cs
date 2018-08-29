using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.ASAMemberService;
using ASA.Web.Services.LessonsService.DataContracts;
using ASA.Web.Services.LessonsService.DataContracts.Lesson1;
using ASA.Web.Services.LessonsService.DataContracts.Lesson2;
using ASA.Web.Services.LessonsService.DataContracts.Lesson3;
using ASA.Web.Services.Proxies.SALTService;
using Asa.Salt.Web.Common.Types.Constants;

namespace ASA.Web.Services.LessonsService
{
    public static class TranslateModel
    {
        /// <summary>
        /// Maps the reference data contract to a frequency.
        /// </summary>
        /// <param name="refDataContract">The ref data contract.</param>
        /// <returns></returns>
        public static Frequency ToFrequency(this RefLessonLookupDataContract refDataContract )
        {
             return new Frequency()
                {
                    Name = refDataContract.Code.ToLower(),
                    Months = Convert.ToDouble(refDataContract.Value),
                    FrequencyId = refDataContract.RefLessonLookupDataId
                };
        }

        /// <summary>
        /// Maps the reference data contract to a list of frequencies.
        /// </summary>
        /// <param name="refDataContracts">The ref data contracts.</param>
        /// <returns></returns>
        public static List<Frequency> ToFrequency(this List<RefLessonLookupDataContract> refDataContracts)
        {
            return refDataContracts.Select(lessonsRefDataContract => lessonsRefDataContract.ToFrequency()).ToList();
        }

        /// <summary>
        /// To the user.
        /// </summary>
        /// <param name="userLessonContracts">The user lesson contracts.</param>
        /// <returns></returns>
        public static User ToDomainObject(this MemberLessonContract[] userLessonContracts)
        {
            var toReturn = new User();
            var memberAdapter = new AsaMemberAdapter();
            var activeDirectoryKey = memberAdapter.GetActiveDirectoryKeyFromContext();
            var lesson1 = userLessonContracts.FirstOrDefault(l => l.LessonId == 1);
            var lesson2 = userLessonContracts.FirstOrDefault(l => l.LessonId == 2);
            var lesson3 = userLessonContracts.FirstOrDefault(l => l.LessonId == 3);

            toReturn.IndividualId = !string.IsNullOrWhiteSpace(activeDirectoryKey)
                                        ? new Guid(memberAdapter.GetActiveDirectoryKeyFromContext())
                                        : Guid.Empty;
            toReturn.Lesson1Step = lesson1 != null && lesson1.CurrentStep.HasValue ? lesson1.CurrentStep.Value : 0;
            toReturn.Lesson2Step = lesson2 != null && lesson2.CurrentStep.HasValue ? lesson2.CurrentStep.Value : 0;
            toReturn.Lesson3Step = lesson3 != null && lesson3.CurrentStep.HasValue ? lesson3.CurrentStep.Value : 0;
            toReturn.MemberId = memberAdapter.GetMemberIdFromContext();
            toReturn.UserId = userLessonContracts.Any() ? userLessonContracts.First().LessonUserId : 0;

            return toReturn;

        }

        /// <summary>
        /// To the user.
        /// </summary>
        /// <param name="userLessonContract">The user lesson contract.</param>
        /// <returns></returns>
        public static User ToDomainObject(this MemberLessonContract userLessonContract)
        {
            var toReturn = new User();
            var memberAdapter = new AsaMemberAdapter();
            var activeDirectoryKey = memberAdapter.GetActiveDirectoryKeyFromContext();

            toReturn.IndividualId = !string.IsNullOrWhiteSpace(activeDirectoryKey)
                                        ? new Guid(activeDirectoryKey)
                                        : Guid.Empty;
            toReturn.MemberId = memberAdapter.GetMemberIdFromContext();

            if (null == userLessonContract)
                return toReturn;

            toReturn.UserId = userLessonContract.LessonUserId;
            toReturn.Lesson1Step = userLessonContract.LessonId == 1 && userLessonContract .CurrentStep.HasValue? userLessonContract.CurrentStep.Value : 0;
            toReturn.Lesson2Step = userLessonContract.LessonId == 2 && userLessonContract.CurrentStep.HasValue ? userLessonContract.CurrentStep.Value : 0;
            toReturn.Lesson3Step = userLessonContract.LessonId == 3 && userLessonContract.CurrentStep.HasValue ? userLessonContract.CurrentStep.Value : 0;

            return toReturn;

        }


        /// <summary>
        /// To the member lesson contract.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="lessonId">The lesson id.</param>
        /// <returns></returns>
        public static MemberLessonContract ToDataContract(this User user, int lessonId)
        {
            var toReturn = new MemberLessonContract();
            var memberAdapter = new AsaMemberAdapter();

            toReturn.LessonUserId = user.UserId;
            toReturn.MemberId = memberAdapter.GetMemberIdFromContext();
            toReturn.LessonId = lessonId;
            toReturn.CurrentStep = lessonId == LessonTypes.HowDoesYourCashFlow
                                       ? user.Lesson1Step
                                       : lessonId == LessonTypes.MasterYourPlastic
                                             ? user.Lesson2Step
                                             : lessonId == LessonTypes.OwnYourStudentLoans ? user.Lesson3Step : 0;                     

            return toReturn;
        }

        /// <summary>
        /// To the domain object.
        /// </summary>
        /// <param name="income">The income.</param>
        /// <returns></returns>
       public static IncomeContract ToDataContract(this Income income)
        {
            return new IncomeContract()
                {
                    FrequencyId = income.FrequencyId,
                    IncomeSource = income.IncomeTypeName,
                    LessonUserId = income.UserId,
                    IncomeAmount = income.Value
                };
        }

       /// <summary>
       /// To the data contract.
       /// </summary>
       /// <param name="cardInformation">The card information.</param>
       /// <returns></returns>
       public static CardInformationContract ToDataContract(this CardInformation cardInformation)
       {
           return null == cardInformation
                      ? null
                      : new CardInformationContract()
                          {
                              Balance = cardInformation.Balance,
                              InterestRate = cardInformation.InterestRate,
                              LessonUserId = cardInformation.UserId,
                              MakesMinimumPayment = cardInformation.MakesMinimumPayment,
                              MonthlyPaymentAmount = cardInformation.MonthlyPayment
                          };
       }

        /// <summary>
       /// To the data contract.
       /// </summary>
       /// <param name="debtReductionOptions">The debt reduction options.</param>
       /// <returns></returns>
       public static DebtReductionOptionsContract ToDataContract(this DebtReductionOptions debtReductionOptions)
       {
           return null == debtReductionOptions
                      ? null
                      : new DebtReductionOptionsContract()
                          {
                              ExtraPaymentAmount = debtReductionOptions.ExtraPaymentAmount,
                              ExtraPaymentMonth = debtReductionOptions.ExtraPaymentMonth,
                              LessonUserId = debtReductionOptions.UserId,
                              IncreaseMonthlyPayment = debtReductionOptions.IncreaseMonthlyPayment,
                              LowerYourInterestRate = debtReductionOptions.LoweredInterestRate > 0,
                              LoweredInterestRate = debtReductionOptions.LoweredInterestRate,
                              PayCashForOneTimePurchases = debtReductionOptions.PayCashForOneTimePurchases,
                              PayCashForRecurringExpenses = debtReductionOptions.PayCashForRecurringExpenses,
                              PlanAnExtraPayment =
                                  debtReductionOptions.ExtraPaymentAmount > 0 ||
                                  debtReductionOptions.ExtraPaymentMonth > 0
                          };
       }

        /// <summary>
       /// To the data contract.
       /// </summary>
       /// <param name="oneTimeExpense">The one time expense.</param>
       /// <returns></returns>
       public static ExpenseContract ToDataContract(this OneTimeExpense oneTimeExpense)
       {
           return new ExpenseContract()
           {
               Name = oneTimeExpense.Name,
               ExpenseAmount = oneTimeExpense.Value,
               ExpenseDate = oneTimeExpense.Month>0? new DateTime(DateTime.Now.Year,oneTimeExpense.Month,1) : DateTime.MinValue,
               Id = oneTimeExpense.OneTimeExpenseId,
               LessonUserId = oneTimeExpense.UserId
           };
       }

       /// <summary>
       /// To the data contract.
       /// </summary>
       /// <param name="recurringExpenses">The recurring expenses.</param>
       /// <returns></returns>
       public static ExpenseContract[] ToDataContract(this IList<RecurringExpense> recurringExpenses)
       {
           return null == recurringExpenses ? null : recurringExpenses.Select(recurringExpense => recurringExpense.ToDataContract()).ToArray();
       }

        /// <summary>
       /// To the data contract.
       /// </summary>
       /// <param name="oneTimeExpenses">The one time expenses.</param>
       /// <returns></returns>
       public static ExpenseContract[] ToDataContract(this IList<OneTimeExpense> oneTimeExpenses)
       {
           return null == oneTimeExpenses ? null : oneTimeExpenses.Select(oneTimeExpense => oneTimeExpense.ToDataContract()).ToArray();
       }

        /// <summary>
       /// To the one time expense domain object.
       /// </summary>
       /// <param name="oneTimeExpenses">The one time expenses.</param>
       /// <returns></returns>
       public static List<OneTimeExpense> ToOneTimeExpenseDomainObject(this ExpenseContract[] oneTimeExpenses)
        {
            return null == oneTimeExpenses ? null : oneTimeExpenses.Select(expense => expense.ToOneTimeExpenseDomainObject()).ToList();
        }

        /// <summary>
       /// To the recurring expense domain object.
       /// </summary>
       /// <param name="recurringExpenses">The recurring expenses.</param>
       /// <returns></returns>
       public static List<RecurringExpense> ToRecurringExpenseDomainObject(this ExpenseContract[] recurringExpenses)
        {
            return null == recurringExpenses ? null : recurringExpenses.Select(expense => expense.ToRecurringExpenseDomainObject()).ToList();
        }

       /// <summary>
       /// To the domain object.
       /// </summary>
       /// <param name="expenses">The expenses.</param>
       /// <returns></returns>
       public static List<Expense> ToDomainObject(this ExpenseContract[] expenses)
       {
           return null == expenses ? null : expenses.Select(expense => expense.ToDomainObject()).ToList();
       }


        /// <summary>
       /// To the data contract.
       /// </summary>
       /// <param name="recurringExpense">The recurring expense.</param>
       /// <returns></returns>
       public static ExpenseContract ToDataContract(this RecurringExpense recurringExpense)
       {
           return new ExpenseContract()
           {
               Name = recurringExpense.Name,
               ExpenseAmount = recurringExpense.Value,
               FrequencyId = recurringExpense.FrequencyId,
               Id = recurringExpense.RecurringExpenseId,
               LessonUserId = recurringExpense.UserId
           };
       }

       /// <summary>
       /// To the data contract.
       /// </summary>
       /// <param name="deferments">The deferments.</param>
       /// <returns></returns>
       public static DefermentContract ToDataContract(this IList<Deferment> deferments)
       {
           if (null == deferments)
               return null;

           var toReturn = new DefermentContract();

           foreach(var option in deferments)
           {

               toReturn.Forbearance =
                   option.PlanType == RepaymentOptionTypes.Forebearance
                       ? new ForbearanceContract()
                           {
                               ForbearanceMonths = option.ForbearanceLength
                           }
                       : null;
               toReturn.HardshipDeferment =
                   option.PlanType == RepaymentOptionTypes.HardshipBasedDeferment
                       ? new HardshipDefermentContract()
                           {
                               DefermentMonths = option.DefermentLength,
                               ExtraAmount = 0
                           }
                       : null;
               toReturn.InSchoolDeferment =
                   option.PlanType == RepaymentOptionTypes.Deferment
                       ? new InSchoolDefermentContract() {DefermentMonths = option.DefermentLength}
                       : null;
           }

           return toReturn;
       }

       /// <summary>
       /// To the data contract.
       /// </summary>
       /// <param name="fasterRepayments">The faster repayments.</param>
       /// <returns></returns>
       public static FasterRepaymentContract ToDataContract(this IList<FasterRepayment> fasterRepayments)
       {
           if (null == fasterRepayments)
               return null;

           var toReturn = new FasterRepaymentContract();
           foreach (var option in fasterRepayments)
           {
               toReturn.AdditionalMonthlyPayment =
                   option.PlanType == RepaymentOptionTypes.PayMoreEachMonth
                       ? new AdditionalPaymentContract()
                           {
                               MonthlyPayment = option.AdditionalMonthlyPayment
                           }
                       : null;
               toReturn.BetterInterestRate =
                   option.PlanType == RepaymentOptionTypes.BetterInterestRate
                       ? new BetterInterestRateContract()
                           {
                               LowerInterestRate = option.LowerInterestRate
                           }
                       : null;
               toReturn.LoanPaymentTimeline =
                   option.PlanType == RepaymentOptionTypes.SetYourOwnTimeline
                       ? new CustomTimelineContract() {Timeline = option.ShorterTimeline}
                       : null;
           }
           return toReturn;
       }

        /// <summary>
       /// To the data contract.
       /// </summary>
       /// <param name="loanType">Type of the loan.</param>
       /// <returns></returns>
       public static LoanTypeContract ToDataContract(this LoanType loanType)
       {
           return new LoanTypeContract()
           {
               DegreeType = loanType.LoanName,
               Amount = loanType.LoanAmount,
               LessonUserId = loanType.UserId,
               //new fields
               AnnualIncome = loanType.AnnualIncome,
               FinancialDependents = loanType.FinancialDependents,
               State = loanType.State,
               InterestRate = loanType.InterestRate,
               Id = loanType.LoanTypeId
           };
       }

       /// <summary>
       /// To the data contract.
       /// </summary>
       /// <param name="loanTypes">The loan types.</param>
       /// <returns></returns>
       public static LoanTypeContract[] ToDataContract(this IList<LoanType> loanTypes)
       {
           return null == loanTypes ? null : loanTypes.Select(loanType => loanType.ToDataContract()).ToArray();
       }

        /// <summary>
       /// To the data contract.
       /// </summary>
       /// <param name="standardRepayment">The standard repayment.</param>
       /// <returns></returns>
       public static StandardRepaymentContract ToDataContract(this StandardRepayment standardRepayment)
        {
            return null == standardRepayment
                       ? null
                       : new StandardRepaymentContract()
                           {
                               LessonUserId = standardRepayment.UserId
                           };
        }

       /// <summary>
       /// To the data contract.
       /// </summary>
       /// <param name="favorite">The favorite.</param>
       /// <returns></returns>
       public static FavoriteRepaymentPlanContract ToDataContract(this Favorite favorite)
       {
           return null == favorite
                      ? null
                      : new FavoriteRepaymentPlanContract()
                      {
                          Id = favorite.FavoriteId,
                          RepaymentPlan = favorite.RepaymentName
                      };
       }

       /// <summary>
       /// To the data contract.
       /// </summary>
       /// <param name="favorites">The favorites.</param>
       /// <returns></returns>
       public static FavoriteRepaymentPlanContract[] ToDataContract(this IList<Favorite> favorites)
       {
           return null == favorites ? null : favorites.Select(favorite => favorite.ToDataContract()).ToArray();
       }

       /// <summary>
       /// To the data contract.
       /// </summary>
       /// <param name="lowerPayments">The lower payments.</param>
       /// <returns></returns>
       public static LowerPaymentContract ToDataContract(this IList<LowerPayment> lowerPayments)
        {
            if (null == lowerPayments)
                return null;

            var toReturn = new LowerPaymentContract();
            foreach (var option in lowerPayments)
            {

                toReturn.ExtendedRepayment =
                    option.PlanType == RepaymentOptionTypes.ExtendedRepayment
                        ? new ExtendedRepaymentContract()
                            {
                                ExtensionYears = option.ExtendedLength
                            }
                        : null;
                toReturn.IncomeBasedRepayment =
                    option.PlanType == RepaymentOptionTypes.IncomeBased
                        ? new IncomeBasedRepaymentContract()
                            {
                                AnnualIncome = option.IncomeBasedYearlyIncome,
                                FinancialDependents = option.FinancialDependents
                            }
                        : null;
                toReturn.IncomeSensitiveRepayment =
                    option.PlanType == RepaymentOptionTypes.IncomeSensitive
                        ? new IncomeSensitiveRepaymentContract()
                            {
                                AnnualIncome = option.IncomeSensitiveYearlyIncome,
                                IncomePercentage = option.IncomeSensitiveIncomePercentage
                            }
                        : null;

            }
            return toReturn;
        }


        /// <summary>
       /// To the domain object.
       /// </summary>
       /// <param name="income">The income.</param>
       /// <returns></returns>
       public static Income ToDomainObject(this IncomeContract income)
       {
           return new Income()
           {
               FrequencyId = income.FrequencyId,
               IncomeTypeName = income.IncomeSource,
               UserId = income.LessonUserId,
               Value = income.IncomeAmount
           };
       }

       /// <summary>
       /// To the domain object.
       /// </summary>
       /// <param name="expense">The expense.</param>
       /// <returns></returns>
        public static ExpenseContract ToDataContract(this Expense expense)
        {
            return new ExpenseContract()
            {
                FrequencyId = expense.FrequencyId,
                LessonUserId = expense.UserId,
                HasTopLevelExpense = expense.Complex,
                PaidByCreditCard = expense.CreditExpense,
                ParentExpenseId = expense.ParentExpenseId,
                ParentExpenseType = expense.ParentName,
                DisplayName = expense.DisplayName,
                Recurring = expense.Recurring,
                ExpenseAmount = expense.Value,
                Id = expense.ExpenseId,
                Name = expense.Name,
                Frequency = expense.Frequency.ToDataContract()
                

            };
        }

        /// <summary>
        /// To the data contract.
        /// </summary>
        /// <param name="frequency">The frequency.</param>
        /// <returns></returns>
        public static FrequencyContract ToDataContract(this Frequency frequency)
        {
            if (frequency == null)
                return new FrequencyContract();

            return new FrequencyContract()
            {
                FrequencyId = frequency.FrequencyId,
                Months = frequency.Months,
                Name = frequency.Name


            };
        }

        /// <summary>
        /// To the domain object.
        /// </summary>
        /// <param name="expense">The expense.</param>
        /// <returns></returns>
        public static Expense ToDomainObject(this ExpenseContract expense)
        {
            return new Expense()
            {
                FrequencyId = expense.FrequencyId,
                Frequency = expense.Frequency.ToDomainObject(),
                UserId = expense.LessonUserId,
                Complex = expense.HasTopLevelExpense,
                CreditExpense = expense.PaidByCreditCard,
                ParentExpenseId = expense.ParentExpenseId,
                ExpenseId = expense.Id,
                ParentName = expense.ParentExpenseType,
                DisplayName = expense.DisplayName,
                Recurring = expense.HasTopLevelExpense,
                Value = expense.ExpenseAmount,
                Name = expense.Name


            };
        }

        /// <summary>
        /// To the domain object.
        /// </summary>
        /// <param name="cardInformation">The card information.</param>
        /// <returns></returns>
        public static CardInformation ToDomainObject(this CardInformationContract cardInformation)
        {
            return null == cardInformation
                       ? null
                       : new CardInformation()
                           {
                               Balance = cardInformation.Balance,
                               InterestRate = cardInformation.InterestRate,
                               MakesMinimumPayment = cardInformation.MakesMinimumPayment,
                               MonthlyPayment = cardInformation.MonthlyPaymentAmount,
                               UserId = cardInformation.LessonUserId
                           };
        }

        /// <summary>
        /// To the domain object.
        /// </summary>
        /// <param name="favoriteRepaymentPlan">The favorite repayment plan.</param>
        /// <returns></returns>
        public static Favorite ToDomainObject(this FavoriteRepaymentPlanContract favoriteRepaymentPlan)
        {
            return null == favoriteRepaymentPlan
                       ? null
                       : new Favorite()
                       {
                           RepaymentName = favoriteRepaymentPlan.RepaymentPlan,
                           FavoriteId = favoriteRepaymentPlan.Id
                       };
        }

        /// <summary>
        /// To the domain object.
        /// </summary>
        /// <param name="favorites">The favorites.</param>
        /// <returns></returns>
        public static List<Favorite> ToDomainObject(this FavoriteRepaymentPlanContract[] favorites)
        {
            return null == favorites ? null : favorites.Select(favorite => favorite.ToDomainObject()).ToList();
        }

        /// <summary>
        /// To the recurring expense domain object.
        /// </summary>
        /// <param name="expense">The expense.</param>
        /// <returns></returns>
        public static RecurringExpense ToRecurringExpenseDomainObject(this ExpenseContract expense)
        {
            return null == expense
                       ? null
                       : new RecurringExpense()
                           {
                               FrequencyId = expense.FrequencyId,
                               Frequency = expense.Frequency==null?null:expense.Frequency.ToDomainObject(),
                               UserId = expense.LessonUserId,
                               Value = expense.ExpenseAmount,
                               RecurringExpenseId= expense.Id,
                               Name = expense.Name,
                           };
        }

        /// <summary>
        /// To the one time expense domain object.
        /// </summary>
        /// <param name="expense">The expense.</param>
        /// <returns></returns>
        public static OneTimeExpense ToOneTimeExpenseDomainObject(this ExpenseContract expense)
        {
            return null == expense
                       ? null
                       : new OneTimeExpense()
                           {
                               UserId = expense.LessonUserId,
                               Value = expense.ExpenseAmount,
                               Month = expense.ExpenseDate==DateTime.MinValue?0:expense.ExpenseDate.Month,
                               Name = expense.Name,
                               OneTimeExpenseId= expense.Id
                           };
        }

        /// <summary>
        /// To the domain object.
        /// </summary>
        /// <param name="deferment">The deferment.</param>
        /// <returns></returns>
        public static Deferment ToDomainObject(this DefermentContract deferment)
        {
            return null == deferment
                       ? null
                       : new Deferment()
                           {
                               UserId = deferment.LessonUserId,
                               DefermentLength = deferment.InSchoolDeferment!=null?deferment.InSchoolDeferment.DefermentMonths:0,
                               HardshipDefermentExtraAmount = deferment.HardshipDeferment != null ? deferment.HardshipDeferment.ExtraAmount : 0,
                               HardshipDefermentLength = deferment.HardshipDeferment != null ? deferment.HardshipDeferment.DefermentMonths : 0,
                               ForbearanceLength = deferment.Forbearance != null ? deferment.Forbearance.ForbearanceMonths : 0
                           };
        }

        /// <summary>
        /// To the domain list object.
        /// </summary>
        /// <param name="deferment">The deferment.</param>
        /// <returns></returns>
        public static IList<Deferment> ToDomainListObject(this DefermentContract deferment)
        {
            var toReturn = deferment == null ? null : new List<Deferment>();

            if (toReturn != null)
            {
                toReturn.Add(deferment.ToDomainObject());
                toReturn.Add(deferment.ToDomainObject());
                toReturn.Add(deferment.ToDomainObject());
            }

            return toReturn;
        }

        /// <summary>
        /// To the domain list object.
        /// </summary>
        /// <param name="lowerPayment">The lower payment.</param>
        /// <returns></returns>
        public static IList<LowerPayment> ToDomainListObject(this LowerPaymentContract lowerPayment)
        {
            var toReturn = lowerPayment == null ? null : new List<LowerPayment>();

            if (toReturn != null)
            {
                toReturn.Add(lowerPayment.ToDomainObject());
                toReturn.Add(lowerPayment.ToDomainObject());
                toReturn.Add(lowerPayment.ToDomainObject());
            }

            return toReturn;
        }

        /// <summary>
        /// To the domain list object.
        /// </summary>
        /// <param name="fasterRepayment">The faster repayment.</param>
        /// <returns></returns>
        public static IList<FasterRepayment> ToDomainListObject(this FasterRepaymentContract fasterRepayment)
        {
            var toReturn = fasterRepayment == null ? null : new List<FasterRepayment>();

            if (toReturn != null)
            {
                toReturn.Add(fasterRepayment.ToDomainObject());
                toReturn.Add(fasterRepayment.ToDomainObject());
                toReturn.Add(fasterRepayment.ToDomainObject());
            }

            return toReturn;
        }

        /// <summary>
        /// To the domain object.
        /// </summary>
        /// <param name="fasterRepayment">The faster repayment.</param>
        /// <returns></returns>
        public static FasterRepayment ToDomainObject(this FasterRepaymentContract fasterRepayment)
        {
            return null == fasterRepayment
                       ? null
                       : new FasterRepayment()
                           {
                               UserId = fasterRepayment.LessonUserId,
                               LowerInterestRate = fasterRepayment.BetterInterestRate!=null?fasterRepayment.BetterInterestRate.LowerInterestRate:0,
                               AdditionalMonthlyPayment = fasterRepayment.AdditionalMonthlyPayment!=null?fasterRepayment.AdditionalMonthlyPayment.MonthlyPayment:0,
                               ShorterTimeline = fasterRepayment.LoanPaymentTimeline!=null? fasterRepayment.LoanPaymentTimeline.Timeline:0
                           };
        }

        /// <summary>
        /// To the domain object.
        /// </summary>
        /// <param name="loanType">Type of the loan.</param>
        /// <returns></returns>
        public static LoanType ToDomainObject(this LoanTypeContract loanType)
        {
            return null == loanType
                       ? new LoanType()
                       : new LoanType()
                           {
                               UserId = loanType.LessonUserId,
                               LoanAmount = loanType.Amount,
                               LoanName = loanType.DegreeType,
                               AnnualIncome = loanType.AnnualIncome,
                               FinancialDependents = loanType.FinancialDependents,
                               State = loanType.State,
                               InterestRate = loanType.InterestRate,
                               LoanTypeId = loanType.Id
                           };
        }

        /// <summary>
        /// To the domain object.
        /// </summary>
        /// <param name="loanTypes">The loan types.</param>
        /// <returns></returns>
        public static List<LoanType> ToDomainObject(this LoanTypeContract[] loanTypes)
        {
            return null == loanTypes ? null : loanTypes.Select(loanType => loanType.ToDomainObject()).ToList();
        }

        /// <summary>
        /// To the domain object.
        /// </summary>
        /// <param name="standardRepayment">The standard repayment.</param>
        /// <returns></returns>
        public static StandardRepayment ToDomainObject(this StandardRepaymentContract standardRepayment)
        {
            return null == standardRepayment
                       ? new StandardRepayment()
                       : new StandardRepayment()
                           {
                               UserId = standardRepayment.LessonUserId
                           };
        }

        /// <summary>
        /// To the domain object.
        /// </summary>
        /// <param name="lowerPayment">The lower payment.</param>
        /// <returns></returns>
        public static LowerPayment ToDomainObject(this LowerPaymentContract lowerPayment)
        {
            return null == lowerPayment
                       ? new LowerPayment()
                       : new LowerPayment()
                           {
                               UserId = lowerPayment.LessonUserId,
                               FinancialDependents = lowerPayment.IncomeBasedRepayment!=null?lowerPayment.IncomeBasedRepayment.FinancialDependents:0,
                               IncomeBasedYearlyIncome = lowerPayment.IncomeBasedRepayment != null ? lowerPayment.IncomeBasedRepayment.AnnualIncome : 0,
                               IncomeSensitiveIncomePercentage = lowerPayment.IncomeSensitiveRepayment != null ? lowerPayment.IncomeSensitiveRepayment.IncomePercentage : 0,
                               IncomeSensitiveYearlyIncome = lowerPayment.IncomeSensitiveRepayment != null ? lowerPayment.IncomeSensitiveRepayment.AnnualIncome : 0
                           };
        }

        /// <summary>
        /// To the domain object.
        /// </summary>
        /// <param name="debtReductionOptions">The debt reduction options.</param>
        /// <returns></returns>
        public static DebtReductionOptions ToDomainObject(this DebtReductionOptionsContract debtReductionOptions)
        {
            return null == debtReductionOptions
                       ? new DebtReductionOptions()
                       : new DebtReductionOptions()
                           {
                               ExtraPaymentAmount = debtReductionOptions.ExtraPaymentAmount,
                               ExtraPaymentMonth = debtReductionOptions.ExtraPaymentMonth,
                               LoweredInterestRate = debtReductionOptions.LoweredInterestRate,
                               IncreaseMonthlyPayment = debtReductionOptions.IncreaseMonthlyPayment,
                               PayCashForOneTimePurchases = debtReductionOptions.PayCashForOneTimePurchases,
                               PayCashForRecurringExpenses = debtReductionOptions.PayCashForRecurringExpenses,
                               UserId = debtReductionOptions.LessonUserId
                           };
        }

        /// <summary>
        /// To the domain object.
        /// </summary>
        /// <param name="frequency">The frequency.</param>
        /// <returns></returns>
        public static Frequency ToDomainObject(this FrequencyContract frequency)
        {
            return new Frequency()
            {
                FrequencyId = frequency.FrequencyId,
                Months = frequency.Months,
                Name = frequency.Name
            };
        }

        /// <summary>
        /// To the data contract.
        /// </summary>
        /// <param name="goal">The goal.</param>
        /// <returns></returns>
        public static GoalContract ToDataContract(this Goal goal)
        {
            return new GoalContract()
                {
                    LessonUserId = goal.UserId,
                    Months = goal.Months,
                    Name = goal.Name,
                    TargetAmount = goal.Value
                };
        }

        /// <summary>
        /// To the domain object.
        /// </summary>
        /// <param name="goal">The goal.</param>
        /// <returns></returns>
        public static Goal ToDomainObject(this GoalContract goal)
        {
            return new Goal()
            {
                UserId = goal.LessonUserId,
                Months = goal.Months,
                Name = goal.Name,
                Value = goal.TargetAmount
            };
        }

        /// <summary>
        /// To the data contract.
        /// </summary>
        /// <param name="lesson1">The lesson1.</param>
        /// <returns></returns>
        public static Lesson1Contract ToDataContract(this Lesson1 lesson1)
        {
            var expenses = new List<ExpenseContract>();
            var incomes = new List<IncomeContract>();

            if (lesson1.Expenses != null)
            {
                expenses = lesson1.Expenses.Select(expense => expense.ToDataContract()).ToList();
            }

            if (lesson1.Incomes != null)
            {
                incomes = lesson1.Incomes.Select(income => income.ToDataContract()).ToList();
            }

            return new Lesson1Contract()
            {
               Expenses = expenses.ToArray(),
               Incomes = incomes.ToArray(),
               Goal = lesson1.Goal!=null? lesson1.Goal.ToDataContract():new GoalContract(),
               User = lesson1.User.ToDataContract(LessonTypes.HowDoesYourCashFlow)
            };
        }

        /// <summary>
        /// To the data contract.
        /// </summary>
        /// <param name="lesson2">The lesson2.</param>
        /// <returns></returns>
        public static Lesson2Contract ToDataContract(this Lesson2 lesson2)
        {
            var toReturn = new Lesson2Contract()
                {
                    CurrentBalance = lesson2.CurrentBalance.ToDataContract(),
                    DebtReductionOptions = lesson2.DebtReductionOptions.ToDataContract(),
                    OneTimeExpenses = lesson2.OneTimeExpenses.ToDataContract(),
                    RecurringExpenses = lesson2.RecurringExpenses.ToDataContract(),
                    User = lesson2.User.ToDataContract(LessonTypes.MasterYourPlastic)
                };

            return toReturn;
        }

        /// <summary>
        /// To the data contract.
        /// </summary>
        /// <param name="lesson3">The lesson3.</param>
        /// <returns></returns>
        public static Lesson3Contract ToDataContract(this Lesson3 lesson3)
        {
            var toReturn = new Lesson3Contract()
            {
                FavoriteRepaymentPlans = lesson3.FavoriteRepaymentPlans!=null?lesson3.FavoriteRepaymentPlans.ToDataContract():null,
                LoanTypes = lesson3.LoanTypes!=null?lesson3.LoanTypes.ToDataContract():null,
                User = lesson3.User.ToDataContract(LessonTypes.OwnYourStudentLoans),
                DefermentOptions = lesson3.DefermentOptions!=null?lesson3.DefermentOptions.ToDataContract():new DefermentContract(),
                FasterRepaymentOptions = lesson3.FasterRepaymentOptions!=null?lesson3.FasterRepaymentOptions.ToDataContract():new FasterRepaymentContract(),
                LowerPaymentOptions = lesson3.LowerPaymentOptions!=null?lesson3.LowerPaymentOptions.ToDataContract():new LowerPaymentContract()
            };

            return toReturn;
        }

        /// <summary>
        /// To the domain object.
        /// </summary>
        /// <param name="lesson1">The lesson1.</param>
        /// <returns></returns>
        public static Lesson1 ToDomainObject(this Lesson1Contract lesson1)
        {
            var expenses = new List<Expense>();
            var incomes = new List<Income>();

            if (lesson1.Expenses != null)
            {
                expenses = lesson1.Expenses.Select(expense => expense.ToDomainObject()).ToList();
            }

            if (lesson1.Incomes != null)
            {
                incomes = lesson1.Incomes.Select(income => income.ToDomainObject()).ToList();
            }
            
            var membershipApi = new AsaMemberAdapter();
            var activeDirectoryKey = membershipApi.GetActiveDirectoryKeyFromContext();
            return new Lesson1()
            {
                Expenses = expenses,
                Incomes = incomes,
                Goal = lesson1.Goal != null ? lesson1.Goal.ToDomainObject() : new Goal(),
                User = lesson1.User.ToDomainObject(),
                IndividualId = !string.IsNullOrWhiteSpace(activeDirectoryKey)
                                        ? new Guid(activeDirectoryKey)
                                        : Guid.Empty
            };
        }

        /// <summary>
        /// To the domain object.
        /// </summary>
        /// <param name="lesson2">The lesson2.</param>
        /// <returns></returns>
        public static Lesson2 ToDomainObject(this Lesson2Contract lesson2)
        {
            var membershipApi = new AsaMemberAdapter();
            var activeDirectoryKey = membershipApi.GetActiveDirectoryKeyFromContext();
            var lesson = new Lesson2()
                {
                    CurrentBalance = lesson2.CurrentBalance.ToDomainObject(),
                    DebtReductionOptions = lesson2.DebtReductionOptions.ToDomainObject(),
                    IndividualId  = !string.IsNullOrWhiteSpace(activeDirectoryKey)
                                        ? new Guid(activeDirectoryKey)
                                        : Guid.Empty,
                    ImportedExpenses = lesson2.ImportedExpenses.ToDomainObject(),
                    OneTimeExpenses = lesson2.OneTimeExpenses.ToOneTimeExpenseDomainObject(),
                    RecurringExpenses = lesson2.RecurringExpenses.ToRecurringExpenseDomainObject(),
                    User = lesson2.User.ToDomainObject()

                };

            return lesson;
        }

        /// <summary>
        /// To the domain object.
        /// </summary>
        /// <param name="lesson3">The lesson3.</param>
        /// <returns></returns>
        public static Lesson3 ToDomainObject(this Lesson3Contract lesson3)
        {
            var membershipApi = new AsaMemberAdapter();
            var activeDirectoryKey = membershipApi.GetActiveDirectoryKeyFromContext();
            var lesson = new Lesson3()
            {
               FavoriteRepaymentPlans = lesson3.FavoriteRepaymentPlans.ToDomainObject(),
               IndividualId = !string.IsNullOrWhiteSpace(activeDirectoryKey)
                                        ? new Guid(activeDirectoryKey)
                                        : Guid.Empty,
               LoanTypes = lesson3.LoanTypes.ToDomainObject(),
               DefermentOptions = lesson3.DefermentOptions.ToDomainListObject(),
               FasterRepaymentOptions = lesson3.FasterRepaymentOptions.ToDomainListObject(),
               LowerPaymentOptions = lesson3.LowerPaymentOptions.ToDomainListObject(),
               UserId = lesson3.User.MemberLessonId,
               User =lesson3.User.ToDomainObject()

            };

            return lesson;
        }
    }
}
