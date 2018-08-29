using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Asa.Salt.Web.Common.Types.Constants;
using Asa.Salt.Web.Common.Types.Enums;
using Asa.Salt.Web.Services.BusinessServices.Interfaces;
using Asa.Salt.Web.Services.Data.Constants;
using Asa.Salt.Web.Services.Data.Infrastructure;
using Asa.Salt.Web.Services.Data.Model.Database;
using Asa.Salt.Web.Services.Data.Repositories;
using Asa.Salt.Web.Services.Domain;
using Asa.Salt.Web.Services.Domain.Lessons;
using Asa.Salt.Web.Services.Domain.Lessons.Lesson1;
using Asa.Salt.Web.Services.Domain.Lessons.Lesson2;
using Asa.Salt.Web.Services.Domain.Lessons.Lesson3;
using Asa.Salt.Web.Services.Logging;
using Asa.Salt.Web.Services.SaltSecurity.Utilities;
using LessonStep = Asa.Salt.Web.Services.Domain.LessonStep;
using LessonQuestion = Asa.Salt.Web.Services.Domain.LessonQuestion;
using LessonQuestionAttribute = Asa.Salt.Web.Services.Domain.LessonQuestionAttribute;
using LessonQuestionResponse = Asa.Salt.Web.Services.Domain.LessonQuestionResponse;
using LoanType = Asa.Salt.Web.Services.Domain.Lessons.Lesson3.LoanType;
using RefLessonLookupData = Asa.Salt.Web.Services.Domain.RefLessonLookupData;
using RefLessonLookupDataType = Asa.Salt.Web.Services.Domain.RefLessonLookupDataType;
using MemberLesson = Asa.Salt.Web.Services.Domain.MemberLesson;

namespace Asa.Salt.Web.Services.BusinessServices
{
    /// <summary>
    /// The lessons service.
    /// </summary>
    public class LessonsService : ILessonsService
    {

        /// <summary>
        /// The lessons reference data group repository
        /// </summary>
        private readonly IRepository<RefLessonLookupDataType, int> _refLessonLookupDataGroupRepository;

        /// <summary>
        /// The lessons reference data repository
        /// </summary>
        private readonly IRepository<RefLessonLookupData, int> _refLessonLookupDataRepository;

        /// <summary>
        /// The member lesson repository
        /// </summary>
        private readonly IRepository<MemberLesson, int> _memberLessonRepository;

        /// <summary>
        /// The lesson step repository
        /// </summary>
        private readonly IRepository<LessonStep, int> _lessonStepRepository;

        /// <summary>
        /// The lesson question repository
        /// </summary>
        private readonly IRepository<LessonQuestion, int> _lessonQuestionRepository;

        /// <summary>
        /// The lesson question attribute repository
        /// </summary>
        private readonly IRepository<LessonQuestionAttribute, int> _lessonQuestionAttributeRepository;

        /// <summary>
        /// The lesson question response repository
        /// </summary>
        private readonly IRepository<LessonQuestionResponse, int> _lessonQuestionResponseRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILog _logger;

        /// <summary>
        /// The db context
        /// </summary>
        private SALTEntities _dbContext;


        /// <summary>
        /// Initializes a new instance of the <see cref="LessonsService" /> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="refLessonLookupDataGroupRepository">The lessons ref data group repository.</param>
        /// <param name="refLessonLookupDataRepository">The lessons ref data repository.</param>
        /// <param name="memberLessonRepository">The member lesson repository.</param>
        /// <param name="lessonsQuestionRepository">The lessons question repository.</param>
        /// <param name="lessonsQuestionAttributeRepository">The lessons question attribute repository.</param>
        /// <param name="lessonsQuestionResponseRepository">The lessons question response repository.</param>
        /// <param name="logger">The logger.</param>
        public LessonsService(SALTEntities dbContext, IRepository<RefLessonLookupDataType, int> refLessonLookupDataGroupRepository, IRepository<RefLessonLookupData, int> refLessonLookupDataRepository, IRepository<MemberLesson, int> memberLessonRepository, IRepository<LessonStep, int> lessonStepRepository, IRepository<LessonQuestion, int> lessonsQuestionRepository, IRepository<LessonQuestionAttribute, int> lessonsQuestionAttributeRepository, IRepository<LessonQuestionResponse, int> lessonsQuestionResponseRepository, ILog logger)
        {
            _logger = logger;
            _dbContext = dbContext;
            _memberLessonRepository = memberLessonRepository;
            _refLessonLookupDataRepository = refLessonLookupDataRepository;
            _refLessonLookupDataGroupRepository = refLessonLookupDataGroupRepository;
            _lessonQuestionAttributeRepository = lessonsQuestionAttributeRepository;
            _lessonQuestionRepository = lessonsQuestionRepository;
            _lessonQuestionResponseRepository = lessonsQuestionResponseRepository;
            _lessonStepRepository = lessonStepRepository;
        }

        /// <summary>
        /// Gets the reference data.
        /// </summary>
        /// <param name="refDataType">Type of the ref data.</param>
        /// <returns></returns>
        public IList<RefLessonLookupData> GetReferenceData(RefLessonLookupDataTypes refDataType)
        {
            var lookupData = _refLessonLookupDataRepository.GetAll();
            return lookupData.Where(l => l.RefLessonLookupDataTypeId == Convert.ToInt32(refDataType.ToString("d"))).ToList();
        }

        /// <summary>
        /// Creates the user lesson.
        /// </summary>
        /// <param name="userLesson">The user lesson.</param>
        /// <returns></returns>
        public MemberLesson StartUserLesson(MemberLesson userLesson)
        {
            if (userLesson.MemberId.HasValue && userLesson.MemberId.Value>0)
            {
                var memberLesson = _memberLessonRepository.Get(ml => ml.MemberId == userLesson.MemberId.Value).FirstOrDefault();
                userLesson.LessonUserId = memberLesson != null ? memberLesson.LessonUserId : 0;
            }

            if (userLesson.LessonUserId <= 0)
            {
                userLesson.LessonUserId = Convert.ToInt32(_dbContext.Database.SqlQuery<Int64>(StoredProcedures.GetNextUserLessonLookupId.StoredProcedureName).Select(n=>n.ToString(CultureInfo.InvariantCulture)).First());
            }

            userLesson.CreatedBy = Principal.GetIdentity();
            userLesson.CreatedDate = DateTime.Now;
            userLesson.LessonId = userLesson.LessonId;
            if (userLesson.MemberId.HasValue && userLesson.MemberId.Value <= 0)
            {
                userLesson.MemberId = null;
            }

            var unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);
            DeleteUserLesson(userLesson.LessonUserId, userLesson.LessonId);
            _memberLessonRepository.Add(userLesson);
            unitOfWork.Commit();

            return userLesson;
        }

        /// <summary>
        /// Associates the lessons with the user.
        /// </summary>
        /// <param name="lessonUserId">The lesson user id.</param>
        /// <param name="userId">The user id.</param>
        public bool  AssociateLessonsWithUser(int lessonUserId, int userId)
        {
            try
            {
                var userLesson = _memberLessonRepository.Get(ml => ml.LessonUserId == lessonUserId).FirstOrDefault();

                if (userLesson != null && !userLesson.MemberId.HasValue)
                {
                    //if the user has already taken the lesson, delete thier old responses
                    foreach (var lesson in _memberLessonRepository.Get(ml => ml.MemberId == userId && ml.LessonId == userLesson.LessonId, null, string.Empty).ToList())
                    {
                        DeleteLessonQuestionResponses(lesson.MemberLessonId, null,null,null);
                        _memberLessonRepository.Delete(lesson);
                    }

                    userLesson.MemberId = userId;
                    userLesson.ModifiedDate = DateTime.Now;
                    userLesson.ModifiedBy = Principal.GetIdentity();

                    IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);

                    _memberLessonRepository.Update(userLesson);

                    //Associated the lesson being updated with the member
                    foreach (var lesson in _memberLessonRepository.Get(ml => ml.LessonUserId == userLesson.LessonUserId))
                    {
                        lesson.MemberId = userId;
                    }
                    
                    //give all lessons the same lesson user id.
                    foreach (var lesson in _memberLessonRepository.Get(ml => ml.MemberId == userId))
                    {
                        lesson.LessonUserId = userLesson.LessonUserId;
                    }

                    unitOfWork.Commit();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message,ex);
                throw;
            }

            return true;
        }

        /// <summary>
        /// Posts the users lesson1 data.
        /// </summary>
        /// <param name="lesson">The lesson.</param>
        /// <returns></returns>
        public PostLessonResult<Lesson1> PostLesson1(Lesson1 lesson)
        {
            try
            {
                if (lesson.User == null || lesson.User.LessonUserId <= 0)
                {
                    return new PostLessonResult<Lesson1>()
                    {
                        Lesson = lesson,
                        UpdateStatus = LessonUpdateStatus.Failure
                    };    
                }

                var unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);
                var userLesson = _memberLessonRepository.Get(ml => ml.LessonUserId == lesson.User.LessonUserId && ml.LessonId == LessonTypes.HowDoesYourCashFlow, null, string.Empty).FirstOrDefault();
                var frequencyLookupType = Convert.ToInt16(RefLessonLookupDataTypes.Frequencies.ToString("d"));
                var frequencies = _refLessonLookupDataRepository.Get(r => r.RefLessonLookupDataTypeId == frequencyLookupType).ToList();

                if (lesson.Goal != null)
                {
                    DeleteLessonQuestionResponses(userLesson.MemberLessonId, LessonQuestions.Lesson1.WhatAreYouSavingFor, null, null);
                    _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson1.NeedHelpSavingTowardGoal.WhatAreYouSavingFor.SavingGoal, lesson.Goal.Name, null));
                    _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson1.NeedHelpSavingTowardGoal.WhatAreYouSavingFor.TargetMonths, lesson.Goal.Months.ToString(CultureInfo.InvariantCulture), null));
                    _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson1.NeedHelpSavingTowardGoal.WhatAreYouSavingFor.AmountNeeded, lesson.Goal.TargetAmount.ToString(CultureInfo.InvariantCulture), null));
                }

                var groupingNumber = 1;
                if (lesson.Incomes != null && lesson.Incomes.Any())
                {
                    DeleteLessonQuestionResponses(userLesson.MemberLessonId, LessonQuestions.Lesson1.WhereDoesYourMoneyComeFromEachMonth, null, null);
                    foreach (var income in lesson.Incomes)
                    {
                        _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson1.SourceOfMoney.WhereDoesYourMoneyComeFromEachMonth.Frequency, frequencies.First(f => f.Id == income.FrequencyId).Value, groupingNumber));
                        _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson1.SourceOfMoney.WhereDoesYourMoneyComeFromEachMonth.IncomeAmount, income.IncomeAmount.ToString(CultureInfo.InvariantCulture), groupingNumber));
                        _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson1.SourceOfMoney.WhereDoesYourMoneyComeFromEachMonth.IncomeSource, income.IncomeSource, groupingNumber));
                        groupingNumber += 1;
                    }
                }

                if (lesson.Expenses != null && lesson.Expenses.Any())
                {
                    groupingNumber = 1;
                    DeleteLessonQuestionResponses(userLesson.MemberLessonId, LessonQuestions.Lesson1.WhatAreYourRegularExpenses, null, null);
                    foreach (var expense in lesson.Expenses)
                    {
                        _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson1.EnterExpenses.WhatAreYourRegularExpenses.Frequency, expense.Frequency != null && expense.Frequency.Months > 0 ? expense.Frequency.Months.ToString(CultureInfo.InvariantCulture) : (expense.FrequencyId.HasValue ? frequencies.First(f => f.Id == expense.FrequencyId.Value).Value : null), groupingNumber));
                        _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson1.EnterExpenses.WhatAreYourRegularExpenses.ExpenseType, expense.ExpenseType, groupingNumber));
                        _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson1.EnterExpenses.WhatAreYourRegularExpenses.PaidByCreditCard, expense.PaidByCreditCard.ToString(), groupingNumber));
                        _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson1.EnterExpenses.WhatAreYourRegularExpenses.ParentExpenseType, !string.IsNullOrWhiteSpace(expense.ParentExpenseType) ? expense.ParentExpenseType : (expense.ParentExpenseId > 0 ? lesson.Expenses.First(e => e.Id == expense.ParentExpenseId).DisplayName.ToLower() : string.Empty), groupingNumber));
                        _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson1.EnterExpenses.WhatAreYourRegularExpenses.ExpenseAmount, expense.ExpenseAmount.ToString(CultureInfo.InvariantCulture), groupingNumber));
                        _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson1.EnterExpenses.WhatAreYourRegularExpenses.ExpenseName, expense.Name, groupingNumber));
                        _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson1.EnterExpenses.WhatAreYourRegularExpenses.ExpenseDisplayName, !string.IsNullOrWhiteSpace(expense.DisplayName) ? expense.DisplayName : !string.IsNullOrWhiteSpace(expense.Name) ? expense.Name : expense.ExpenseType, groupingNumber));
                        _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson1.EnterExpenses.WhatAreYourRegularExpenses.HasChildrenExpenses, expense.HasTopLevelExpense.ToString(), groupingNumber));
                        groupingNumber += 1;
                    }
                }

                if (userLesson != null)
                {
                    userLesson.CurrentStep = lesson.User.CurrentStep;
                    userLesson.ModifiedDate = DateTime.Now;
                    userLesson.ModifiedBy = Principal.GetIdentity();
                    _memberLessonRepository.Update(userLesson);

                    unitOfWork.Commit();

                    return new PostLessonResult<Lesson1>()
                    {
                        Lesson = lesson,
                        UpdateStatus = LessonUpdateStatus.Success
                    };
                }
                else
                {
                    return new PostLessonResult<Lesson1>()
                    {
                        Lesson = null,
                        UpdateStatus = LessonUpdateStatus.Failure
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message,ex);
                throw;
            }
        }

        /// <summary>
        /// Posts the users lesson2 data.
        /// </summary>
        /// <param name="lesson">The lesson.</param>
        /// <returns></returns>
        public PostLessonResult<Lesson2> PostLesson2(Lesson2 lesson)
        {
            try
            {
                var unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);
                var userLesson = _memberLessonRepository.Get(ml => ml.LessonUserId == lesson.User.LessonUserId && ml.LessonId == LessonTypes.MasterYourPlastic, null, string.Empty).FirstOrDefault();
                var groupingNumber = 1;

                if (lesson.CurrentBalance != null)
                {
                    DeleteLessonQuestionResponses(userLesson.MemberLessonId, LessonQuestions.Lesson2.WhatIsYourCurrentBalance, null, null);
                    _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson2.CurrentBalance.WhatIsYourCurrentBalance.CardBalance, lesson.CurrentBalance.Balance.ToString(CultureInfo.InvariantCulture), null));
                    _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson2.CurrentBalance.WhatIsYourCurrentBalance.CardInterestRate, lesson.CurrentBalance.InterestRate.ToString(CultureInfo.InvariantCulture), null));
                }

                if (lesson.RecurringExpenses != null && lesson.RecurringExpenses.Any())
                {
                    DeleteLessonQuestionResponses(userLesson.MemberLessonId, LessonQuestions.Lesson2.WhatAreYourRecurringExpenses, null, null);
                    groupingNumber = 1;

                    foreach (var expense in lesson.RecurringExpenses)
                    {
                        groupingNumber = expense.Id > 0 ? expense.Id : groupingNumber;
                        _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson2.RecurringExpenses.WhatAreYourRecurringExpenses.ExpenseAmount, expense.ExpenseAmount.ToString(CultureInfo.InvariantCulture), groupingNumber));
                        _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson2.RecurringExpenses.WhatAreYourRecurringExpenses.ExpenseName, expense.Name.ToString(CultureInfo.InvariantCulture), groupingNumber));
                        groupingNumber += 1;
                    }
                }

                if (lesson.OneTimeExpenses != null && lesson.OneTimeExpenses.Any())
                {
                    DeleteLessonQuestionResponses(userLesson.MemberLessonId, LessonQuestions.Lesson2.AreYouMakingAnyOneTimePurchases, null, null);
                    groupingNumber = 1;

                    foreach (var expense in lesson.OneTimeExpenses)
                    {
                        groupingNumber = expense.Id > 0 ? expense.Id : groupingNumber;
                        _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson2.OneTimePurchases.AreYouMakingAnyOneTimePurchases.ExpenseAmount, expense.ExpenseAmount.ToString(CultureInfo.InvariantCulture), groupingNumber));
                        _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson2.OneTimePurchases.AreYouMakingAnyOneTimePurchases.ExpenseName, string.IsNullOrWhiteSpace(expense.Name) ? string.Empty : expense.Name.ToString(CultureInfo.InvariantCulture), groupingNumber));
                        _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson2.OneTimePurchases.AreYouMakingAnyOneTimePurchases.ExpenseDate, expense.ExpenseDate.ToShortDateString(), groupingNumber));
                        groupingNumber += 1;
                    }
                }

                if (lesson.CurrentBalance != null)
                {
                    DeleteLessonQuestionResponses(userLesson.MemberLessonId, LessonQuestions.Lesson2.HowMuchDoYouTypicallyPayDown, null, null);
                    _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson2.CreditCardPayment.HowMuchDoYouTypicallyPayDown.CardMonthlyPaymentAmount, lesson.CurrentBalance.MonthlyPaymentAmount.ToString(CultureInfo.InvariantCulture), null));
                    _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson2.CreditCardPayment.HowMuchDoYouTypicallyPayDown.MakesMinimumPaymentsOnCard, lesson.CurrentBalance.MakesMinimumPayment.ToString(CultureInfo.InvariantCulture), null));
                }

                if (lesson.DebtReductionOptions != null)
                {
                    DeleteLessonQuestionResponses(userLesson.MemberLessonId, LessonQuestions.Lesson2.OptionsToReduceYourRevolvingDebt, null, null);
                    _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson2.DebtReductionOptions.OptionsToReduceYourRevolvingDebt.IncreaseYourMonthlyPayments, lesson.DebtReductionOptions.IncreaseMonthlyPayment.ToString(CultureInfo.InvariantCulture), null));
                    _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson2.DebtReductionOptions.OptionsToReduceYourRevolvingDebt.PayCashForOneTimePurchases, lesson.DebtReductionOptions.PayCashForOneTimePurchases.ToString(CultureInfo.InvariantCulture), null));
                    _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson2.DebtReductionOptions.OptionsToReduceYourRevolvingDebt.PayCashForRecurringExpenses, lesson.DebtReductionOptions.PayCashForRecurringExpenses.ToString(CultureInfo.InvariantCulture), null));
                    _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson2.DebtReductionOptions.OptionsToReduceYourRevolvingDebt.PlanAnExtraPayment, lesson.DebtReductionOptions.PlanAnExtraPayment.ToString(CultureInfo.InvariantCulture), null));
                    _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson2.DebtReductionOptions.OptionsToReduceYourRevolvingDebt.LowerYourInterestRate, lesson.DebtReductionOptions.LowerYourInterestRate.ToString(CultureInfo.InvariantCulture), null));
                    _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson2.DebtReductionOptions.OptionsToReduceYourRevolvingDebt.LoweredInterestRate, lesson.DebtReductionOptions.LoweredInterestRate.ToString(CultureInfo.InvariantCulture), null));
                    _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson2.DebtReductionOptions.OptionsToReduceYourRevolvingDebt.ExtraPaymentAmount, lesson.DebtReductionOptions.ExtraPaymentAmount.ToString(CultureInfo.InvariantCulture), null));
                    _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson2.DebtReductionOptions.OptionsToReduceYourRevolvingDebt.ExtraPaymentMonth, lesson.DebtReductionOptions.ExtraPaymentMonth.ToString(CultureInfo.InvariantCulture), null));
                }

                if (userLesson != null)
                {
                    unitOfWork.Commit();

                    return new PostLessonResult<Lesson2>()
                    {
                        Lesson = GetUserLesson2Results(lesson.User.LessonUserId),
                        UpdateStatus = LessonUpdateStatus.Success
                    };
                }
                else
                {
                    return new PostLessonResult<Lesson2>()
                    {
                        Lesson = null,
                        UpdateStatus = LessonUpdateStatus.Failure
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Posts the users lesson3 data.
        /// </summary>
        /// <param name="lesson">The lesson.</param>
        /// <returns></returns>
        public PostLessonResult<Lesson3> PostLesson3(Lesson3 lesson)
        {
            try
            {
                var unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);
                var userLesson = _memberLessonRepository.Get(ml => ml.LessonUserId == lesson.User.LessonUserId && ml.LessonId == LessonTypes.OwnYourStudentLoans, null, string.Empty).FirstOrDefault();
                var groupingNumber = 1;

                if (lesson.LoanTypes != null && lesson.LoanTypes.Any())
                {
                    DeleteLessonQuestionResponses(userLesson.MemberLessonId,LessonQuestions.Lesson3.WhatTypeOfRepaymentOptionsWouldYouLikeToExplore, null,null);
                    groupingNumber = 1;

                    foreach (var loanType in lesson.LoanTypes)
                    {
                        groupingNumber = loanType.Id > 0 ? loanType.Id : groupingNumber;
                        _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson3.YourBalance.WhatTypeOfRepaymentOptionsWouldYouLikeToExplore.DegreeType, loanType.DegreeType.ToString(CultureInfo.InvariantCulture), groupingNumber));
                        _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson3.YourBalance.WhatTypeOfRepaymentOptionsWouldYouLikeToExplore.TotalLoanAmount, loanType.Amount.ToString(CultureInfo.InvariantCulture), groupingNumber));
                        _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson3.YourBalance.WhatTypeOfRepaymentOptionsWouldYouLikeToExplore.AnnualIncome, loanType.AnnualIncome.ToString(CultureInfo.InvariantCulture), groupingNumber));
                        _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson3.YourBalance.WhatTypeOfRepaymentOptionsWouldYouLikeToExplore.FinancialDependents, loanType.FinancialDependents.ToString(CultureInfo.InvariantCulture), groupingNumber));
                        _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson3.YourBalance.WhatTypeOfRepaymentOptionsWouldYouLikeToExplore.State, loanType.State.ToString(CultureInfo.InvariantCulture), groupingNumber));
                        _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson3.YourBalance.WhatTypeOfRepaymentOptionsWouldYouLikeToExplore.InterestRate, loanType.InterestRate.ToString(CultureInfo.InvariantCulture), groupingNumber));

                        groupingNumber += 1;
                    }
                }

                if (lesson.FasterRepaymentOptions.AdditionalMonthlyPayment != null)
                {
                    DeleteLessonQuestionResponses(userLesson.MemberLessonId, LessonQuestions.Lesson3.PayMoreEachMonth, null, null);
                    _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson3.PayItDownFaster.PayMoreEachMonth.AdditionalMonthlyPayment, lesson.FasterRepaymentOptions.AdditionalMonthlyPayment.MonthlyPayment.ToString(CultureInfo.InvariantCulture), null));
                }

                if (lesson.FasterRepaymentOptions.BetterInterestRate != null)
                {
                    DeleteLessonQuestionResponses(userLesson.MemberLessonId, LessonQuestions.Lesson3.EarnABetterInterestRate, null, null);
                    _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson3.PayItDownFaster.EarnABetterInterestRate.LoweredInterestRate, lesson.FasterRepaymentOptions.BetterInterestRate.LowerInterestRate.ToString(CultureInfo.InvariantCulture), null));
                }

                if (lesson.FasterRepaymentOptions.LoanPaymentTimeline != null)
                {
                    DeleteLessonQuestionResponses(userLesson.MemberLessonId, LessonQuestions.Lesson3.SetYourOwnTimeline, null, null);
                    _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson3.PayItDownFaster.SetYourOwnTimeline.LoanPaymentTimeline, lesson.FasterRepaymentOptions.LoanPaymentTimeline.Timeline.ToString(CultureInfo.InvariantCulture), null));
                }

                if (lesson.LowerPaymentOptions.ExtendedRepayment != null)
                {
                    DeleteLessonQuestionResponses(userLesson.MemberLessonId, LessonQuestions.Lesson3.Extended, null, null);
                    _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson3.LoweringYourPayments.Extended.RepaymentExtensionYears, lesson.LowerPaymentOptions.ExtendedRepayment.ExtensionYears.ToString(CultureInfo.InvariantCulture), null));
                }

                //if (lesson.LowerPaymentOptions.IncomeBasedRepayment != null)
                //{
                //    DeleteLessonQuestionResponses(userLesson.MemberLessonId, LessonQuestions.Lesson3.IncomeBased, null, null);
                //    //_lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson3.LoweringYourPayments.IncomeBased.AnnualIncome, lesson.LowerPaymentOptions.IncomeBasedRepayment.AnnualIncome.ToString(CultureInfo.InvariantCulture), null));
                //    //_lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson3.LoweringYourPayments.IncomeBased.FinancialDependents, lesson.LowerPaymentOptions.IncomeBasedRepayment.FinancialDependents.ToString(CultureInfo.InvariantCulture), null));
                //}

                if (lesson.LowerPaymentOptions.IncomeSensitiveRepayment != null)
                {
                    DeleteLessonQuestionResponses(userLesson.MemberLessonId, LessonQuestions.Lesson3.IncomeSensitive, null, null);
                    //_lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson3.LoweringYourPayments.IncomeSensitive.AnnualIncome, lesson.LowerPaymentOptions.IncomeSensitiveRepayment.AnnualIncome.ToString(CultureInfo.InvariantCulture), null));
                    _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson3.LoweringYourPayments.IncomeSensitive.IncomePercentage, lesson.LowerPaymentOptions.IncomeSensitiveRepayment.IncomePercentage.ToString(CultureInfo.InvariantCulture), null));
                }

                if (lesson.DefermentOptions.Forbearance != null)
                {
                    DeleteLessonQuestionResponses(userLesson.MemberLessonId, LessonQuestions.Lesson3.Forbearance, null, null);
                    _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson3.DefermentAndForebearance.Forbearance.ForbearanceMonths, lesson.DefermentOptions.Forbearance.ForbearanceMonths.ToString(CultureInfo.InvariantCulture), null));
                }

                if (lesson.DefermentOptions.HardshipDeferment != null)
                {
                    DeleteLessonQuestionResponses(userLesson.MemberLessonId, LessonQuestions.Lesson3.HardshipDeferment, null, null);
                    _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson3.DefermentAndForebearance.HardshipDeferment.DefermentMonths, lesson.DefermentOptions.HardshipDeferment.DefermentMonths.ToString(CultureInfo.InvariantCulture), null));
                    _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson3.DefermentAndForebearance.HardshipDeferment.ExtraAmount, lesson.DefermentOptions.HardshipDeferment.ExtraAmount.ToString(CultureInfo.InvariantCulture), null));
                }

                if (lesson.DefermentOptions.InSchoolDeferment != null)
                {
                    DeleteLessonQuestionResponses(userLesson.MemberLessonId, LessonQuestions.Lesson3.InSchoolDeferment, null, null);
                    _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson3.DefermentAndForebearance.InSchoolDeferment.DefermentMonths, lesson.DefermentOptions.InSchoolDeferment.DefermentMonths.ToString(CultureInfo.InvariantCulture), null));
                }

                if (lesson.FavoriteRepaymentPlans != null && lesson.FavoriteRepaymentPlans.Any())
                {
                    DeleteLessonQuestionResponses(userLesson.MemberLessonId, LessonQuestions.Lesson3.TakeAMomentToReviewYourFavoritePlans, null, null);
                    groupingNumber = 1;

                    foreach (var plan in lesson.FavoriteRepaymentPlans)
                    {
                        groupingNumber = plan.Id > 0 ? plan.Id : groupingNumber;
                        _lessonQuestionResponseRepository.Add(CreateLessonQuestionResponse(userLesson.MemberLessonId, LessonQuestionAttributes.Lesson3.ReviewYourFavorites.TakeAMomentToReviewYourFavoritePlans.FavoritePlan, plan.RepaymentPlan, groupingNumber));
                        groupingNumber += 1;
                    }
                }

                unitOfWork.Commit();

                if (lesson.User != null)
                {

                    return new PostLessonResult<Lesson3>()
                    {
                        Lesson = GetUserLesson3Results(lesson.User.LessonUserId),
                        UpdateStatus = LessonUpdateStatus.Success
                    };
                }
                else
                {
                    return new PostLessonResult<Lesson3>()
                    {
                        Lesson = null,
                        UpdateStatus = LessonUpdateStatus.Failure
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Deletes the lesson question responses.
        /// </summary>
        /// <param name="userLessonId">The user lesson id.</param>
        /// <param name="questionId">The question id.</param>
        /// <param name="questionResponseId">The question response id.</param>
        /// <param name="groupNumber">The group number.</param>
        /// <returns></returns>
        private bool DeleteLessonQuestionResponses(int userLessonId, int? questionId, int? questionResponseId, int? groupNumber)
        {
            var userResponses = _lessonQuestionResponseRepository.Get(r => r.MemberLessonId == userLessonId && (!questionId.HasValue || r.LessonQuestionAttribute.LessonQuestionId == questionId) && (!questionResponseId.HasValue || r.LessonQuestionResponseId == questionResponseId.Value) && (!groupNumber.HasValue || r.GroupingNumber == groupNumber.Value)).ToList();
            if (userResponses.Any())
            {
                foreach (var lessonQuestionResponse in userResponses)
                {
                  _lessonQuestionResponseRepository.Delete(lessonQuestionResponse);  
                }
            }

            return true;
        }

        /// <summary>
        /// Deletes the user lesson question response.
        /// </summary>
        /// <param name="lessonUserId">The lesson user id.</param>
        /// <param name="lessonId">The lesson id.</param>
        /// <param name="questionId">The question id.</param>
        /// <param name="questionResponseId">The question response id.</param>
        /// <param name="groupNumber">The group number.</param>
        /// <returns></returns>
        public bool DeleteUserLessonQuestionResponses(int lessonUserId, int lessonId, int? questionId, int? questionResponseId, int? groupNumber)
        {
            try
            {
                var userLesson = _memberLessonRepository.Get(ml => ml.LessonUserId == lessonUserId && ml.LessonId == lessonId).FirstOrDefault();

                if (userLesson != null)
                {
                    var unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);
                    DeleteLessonQuestionResponses(userLesson.MemberLessonId, questionId, questionResponseId, groupNumber);
                    unitOfWork.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                throw;
            }

            return false;
        }

        /// <summary>
        /// Deletes the user lesson.
        /// </summary>
        /// <param name="lessonUserId">The lesson user id.</param>
        /// <param name="lessonId">The lesson id.</param>
        /// <returns></returns>
        private bool DeleteUserLesson(int lessonUserId, int lessonId)
        {
            var userLessons = _memberLessonRepository.Get(r => r.LessonUserId == lessonUserId && r.LessonId == lessonId).ToList();
            if (userLessons.Any())
            {
                foreach (var lesson in userLessons)
                {
                    DeleteLessonQuestionResponses(lesson.MemberLessonId,null,null,null);
                    _memberLessonRepository.Delete(lesson);
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the user's lesson1 results.
        /// </summary>
        /// <param name="lessonUserId">The lesson user id.</param>
        /// <param name="lessonStepId">The lesson step id.</param>
        /// <returns></returns>
        public Lesson1  GetUserLesson1Results(int lessonUserId, int? lessonStepId=null)
        {
            var userLesson = _memberLessonRepository.Get(ml => ml.LessonUserId == lessonUserId && ml.LessonId==LessonTypes.HowDoesYourCashFlow).FirstOrDefault();
            var lesson1 = new Lesson1(){User = userLesson};
            
            if (userLesson != null)
            {
                var userResponses = _lessonQuestionResponseRepository.Get(r => r.MemberLessonId == userLesson.MemberLessonId && (!lessonStepId.HasValue || r.LessonQuestionAttribute.LessonQuestion.LessonStepId == lessonStepId.Value), null, "LessonQuestionAttribute").ToList();
                if (userResponses.Any())
                {
                    var frequencyLookupType = Convert.ToInt16(RefLessonLookupDataTypes.Frequencies.ToString("d"));
                    var frequencies =_refLessonLookupDataRepository.Get(r => r.RefLessonLookupDataTypeId ==frequencyLookupType ).ToList();

                    lesson1.Goal = new Goal();
                    lesson1.Expenses = new List<Expense>();
                    lesson1.Incomes = new List<Income>();

                    if (userResponses.Any(r => r.LessonQuestionAttribute.LessonQuestionId == LessonQuestions.Lesson1.WhatAreYouSavingFor))
                    {
                        lesson1.Goal.TargetAmount = Convert.ToDecimal(userResponses.First(r => r.LessonQuestionAttributeId  == LessonQuestionAttributes.Lesson1.NeedHelpSavingTowardGoal.WhatAreYouSavingFor.AmountNeeded).ResponseValue);
                        lesson1.Goal.Months = Convert.ToDecimal(userResponses.First(r =>  r.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson1.NeedHelpSavingTowardGoal.WhatAreYouSavingFor.TargetMonths).ResponseValue);
                        lesson1.Goal.Name = userResponses.First(r =>  r.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson1.NeedHelpSavingTowardGoal.WhatAreYouSavingFor.SavingGoal).ResponseValue;
                        lesson1.Goal.LessonUserId = userLesson.LessonUserId;
                    }

                    if (userResponses.Any(r => r.LessonQuestionAttribute.LessonQuestionId == LessonQuestions.Lesson1.WhereDoesYourMoneyComeFromEachMonth))
                    {
                        foreach (var incomeGroup in userResponses.Where(r => r.LessonQuestionAttribute.LessonQuestionId == LessonQuestions.Lesson1.WhereDoesYourMoneyComeFromEachMonth).GroupBy(i=>i.GroupingNumber))
                        {
                            var income = new Income()
                                {
                                    Frequency = new Frequency()
                                    {
                                        Months = Convert.ToDouble(incomeGroup.First(i => i.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson1.SourceOfMoney.WhereDoesYourMoneyComeFromEachMonth.Frequency).ResponseValue),
                                        Name = frequencies.First(f => f.Value == incomeGroup.First(i => i.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson1.SourceOfMoney.WhereDoesYourMoneyComeFromEachMonth.Frequency).ResponseValue).Code
                                    },
                                    IncomeAmount = Convert.ToDecimal(incomeGroup.First(i => i.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson1.SourceOfMoney.WhereDoesYourMoneyComeFromEachMonth.IncomeAmount).ResponseValue),
                                    IncomeSource = incomeGroup.First(i => i.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson1.SourceOfMoney.WhereDoesYourMoneyComeFromEachMonth.IncomeSource).ResponseValue,
                                    LessonUserId = userLesson.LessonUserId,
                                    FrequencyId = frequencies.First(f => f.Value == incomeGroup.First(i => i.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson1.SourceOfMoney.WhereDoesYourMoneyComeFromEachMonth.Frequency).ResponseValue).Id
                                };
                            lesson1.Incomes.Add(income);
                        }
                    }

                    if (userResponses.Any(r => r.LessonQuestionAttribute.LessonQuestionId == LessonQuestions.Lesson1.WhatAreYourRegularExpenses))
                    {
                        foreach (var expenseGroup in userResponses.Where(r => r.LessonQuestionAttribute.LessonQuestionId == LessonQuestions.Lesson1.WhatAreYourRegularExpenses).GroupBy(i => i.GroupingNumber))
                        {
                            var expense = new Expense()
                            {
                                Frequency = expenseGroup.Any(i => i.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson1.EnterExpenses.WhatAreYourRegularExpenses.Frequency) &&frequencies.Any(f => f.Value == expenseGroup.First(i => i.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson1.EnterExpenses.WhatAreYourRegularExpenses.Frequency).ResponseValue) ?new Frequency()
                                {
                                    Months = Convert.ToDouble(expenseGroup.First(i => i.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson1.EnterExpenses.WhatAreYourRegularExpenses.Frequency).ResponseValue),
                                    Name = frequencies.First(f => f.Value == expenseGroup.First(i => i.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson1.EnterExpenses.WhatAreYourRegularExpenses.Frequency).ResponseValue).Code
                                }: new Frequency(),
                                DisplayName = expenseGroup.First(i => i.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson1.EnterExpenses.WhatAreYourRegularExpenses.ExpenseDisplayName).ResponseValue,
                                Name = expenseGroup.First(i => i.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson1.EnterExpenses.WhatAreYourRegularExpenses.ExpenseName).ResponseValue,
                                ExpenseAmount = Convert.ToDecimal(expenseGroup.First(i => i.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson1.EnterExpenses.WhatAreYourRegularExpenses.ExpenseAmount).ResponseValue),
                                ExpenseType = expenseGroup.First(i => i.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson1.EnterExpenses.WhatAreYourRegularExpenses.ExpenseType).ResponseValue,
                                ParentExpenseType = expenseGroup.First(i => i.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson1.EnterExpenses.WhatAreYourRegularExpenses.ParentExpenseType).ResponseValue,
                                PaidByCreditCard = Convert.ToBoolean(expenseGroup.First(i => i.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson1.EnterExpenses.WhatAreYourRegularExpenses.PaidByCreditCard).ResponseValue),
                                FrequencyId = expenseGroup.Any(i => i.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson1.EnterExpenses.WhatAreYourRegularExpenses.Frequency) && frequencies.Any(f => f.Value == expenseGroup.First(i => i.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson1.EnterExpenses.WhatAreYourRegularExpenses.Frequency).ResponseValue) ? frequencies.First(f => f.Value == expenseGroup.First(i => i.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson1.EnterExpenses.WhatAreYourRegularExpenses.Frequency).ResponseValue).Id : 0,
                                HasTopLevelExpense = Convert.ToBoolean(expenseGroup.First(i => i.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson1.EnterExpenses.WhatAreYourRegularExpenses.HasChildrenExpenses).ResponseValue),
                                Id = expenseGroup.First(i => i.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson1.EnterExpenses.WhatAreYourRegularExpenses.ExpenseName).LessonQuestionResponseId,
                                LessonUserId = userLesson.LessonUserId
                            };

                            lesson1.Expenses.Add(expense);
                        }

                        foreach (var expense in lesson1.Expenses)
                        {
                            expense.ParentExpenseId = !string.IsNullOrWhiteSpace(expense.ParentExpenseType) && lesson1.Expenses.Any(e => expense.ParentExpenseType.ToUpper().Contains(e.DisplayName.ToUpper())) ? lesson1.Expenses.First(e => expense.ParentExpenseType.ToUpper().Contains(e.DisplayName.ToUpper())).Id : !string.IsNullOrWhiteSpace(expense.ParentExpenseType) && lesson1.Expenses.Any(e => expense.ParentExpenseType.ToUpper().Contains(e.Name.ToUpper()) && string.IsNullOrWhiteSpace(e.ParentExpenseType)) ? lesson1.Expenses.First(e => expense.ParentExpenseType.ToUpper().Contains(e.Name.ToUpper()) && string.IsNullOrWhiteSpace(e.ParentExpenseType)).Id : 0;
                        }

                        lesson1.Expenses =lesson1.Expenses.Where( e => string.IsNullOrWhiteSpace(e.ParentExpenseType) ||(!string.IsNullOrWhiteSpace(e.ParentExpenseType) && e.ParentExpenseId > 0)).ToList();
                    }
                }
            }
        
           
            return lesson1;
        }

        /// <summary>
        /// Gets the user's lesson2 results.
        /// </summary>
        /// <param name="lessonUserId">The lesson user id.</param>
        /// <param name="lessonStepId">The lesson step id.</param>
        /// <returns></returns>
        public Lesson2 GetUserLesson2Results(int lessonUserId, int? lessonStepId = null)
        {
            var userLesson = _memberLessonRepository.Get(ml => ml.LessonUserId == lessonUserId && ml.LessonId==LessonTypes.MasterYourPlastic).FirstOrDefault();
            var lesson2 = new Lesson2() { User = userLesson };

            if (userLesson != null)
            {
                var userResponses = _lessonQuestionResponseRepository.Get(r => r.MemberLessonId == userLesson.MemberLessonId && r.MemberLesson.LessonId == LessonTypes.MasterYourPlastic && (!lessonStepId.HasValue || r.LessonQuestionAttribute.LessonQuestion.LessonStepId == lessonStepId.Value), null, "LessonQuestionAttribute").ToList();
                if (userResponses.Any())
                {
                    lesson2.DebtReductionOptions = new DebtReductionOptions();
                    lesson2.OneTimeExpenses = new List<Expense>();
                    lesson2.RecurringExpenses = new List<Expense>();
                    lesson2.CurrentBalance = new CardInformation();

                    if (userResponses.Any(r => r.LessonQuestionAttribute.LessonQuestionId == LessonQuestions.Lesson2.WhatIsYourCurrentBalance))
                    {
                        lesson2.CurrentBalance.Balance = Convert.ToDecimal(userResponses.First(r => r.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson2.CurrentBalance.WhatIsYourCurrentBalance.CardBalance).ResponseValue);
                        lesson2.CurrentBalance.InterestRate = Convert.ToDecimal(userResponses.First(r => r.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson2.CurrentBalance.WhatIsYourCurrentBalance.CardInterestRate).ResponseValue);
                    }

                    if (userResponses.Any(r => r.LessonQuestionAttribute.LessonQuestionId == LessonQuestions.Lesson2.WhatAreYourRecurringExpenses))
                    {
                        foreach (var recurringExpense in userResponses.Where(r => r.LessonQuestionAttribute.LessonQuestionId == LessonQuestions.Lesson2.WhatAreYourRecurringExpenses).GroupBy(i => i.GroupingNumber))
                        {
                            var expense = new Expense()
                            {
                                Name = recurringExpense.First(i => i.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson2.RecurringExpenses.WhatAreYourRecurringExpenses.ExpenseName).ResponseValue,
                                ExpenseAmount = Convert.ToDecimal(recurringExpense.First(i => i.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson2.RecurringExpenses.WhatAreYourRecurringExpenses.ExpenseAmount).ResponseValue),
                                Id = recurringExpense.First().GroupingNumber
                            };
                            lesson2.RecurringExpenses.Add(expense);
                        }
                    }

                    if (userResponses.Any(r => r.LessonQuestionAttribute.LessonQuestionId == LessonQuestions.Lesson2.AreYouMakingAnyOneTimePurchases))
                    {
                        foreach (var oneTimeExpense in userResponses.Where(r => r.LessonQuestionAttribute.LessonQuestionId == LessonQuestions.Lesson2.AreYouMakingAnyOneTimePurchases).GroupBy(i => i.GroupingNumber))
                        {
                            var expense = new Expense()
                            {
                                Name = oneTimeExpense.First(i => i.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson2.OneTimePurchases.AreYouMakingAnyOneTimePurchases.ExpenseName).ResponseValue,
                                ExpenseAmount = Convert.ToDecimal(oneTimeExpense.First(i => i.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson2.OneTimePurchases.AreYouMakingAnyOneTimePurchases.ExpenseAmount).ResponseValue),
                                ExpenseDate = Convert.ToDateTime(oneTimeExpense.First(i => i.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson2.OneTimePurchases.AreYouMakingAnyOneTimePurchases.ExpenseDate).ResponseValue),
                                Id = oneTimeExpense.First().GroupingNumber
                            };
                            lesson2.OneTimeExpenses.Add(expense);
                        }
                    }

                    if (userResponses.Any(r => r.LessonQuestionAttribute.LessonQuestionId == LessonQuestions.Lesson2.HowMuchDoYouTypicallyPayDown))
                    {
                        lesson2.CurrentBalance.MonthlyPaymentAmount = Convert.ToDecimal(userResponses.First(r => r.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson2.CreditCardPayment.HowMuchDoYouTypicallyPayDown.CardMonthlyPaymentAmount).ResponseValue);
                        lesson2.CurrentBalance.MakesMinimumPayment = Convert.ToBoolean(userResponses.First(r => r.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson2.CreditCardPayment.HowMuchDoYouTypicallyPayDown.MakesMinimumPaymentsOnCard).ResponseValue);
                    }

                    if (userResponses.Any(r => r.LessonQuestionAttribute.LessonQuestionId == LessonQuestions.Lesson2.OptionsToReduceYourRevolvingDebt))
                    {
                        lesson2.DebtReductionOptions.ExtraPaymentAmount = Convert.ToDecimal(userResponses.First(r => r.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson2.DebtReductionOptions.OptionsToReduceYourRevolvingDebt.ExtraPaymentAmount).ResponseValue);
                        lesson2.DebtReductionOptions.ExtraPaymentMonth = Convert.ToInt16(userResponses.First(r => r.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson2.DebtReductionOptions.OptionsToReduceYourRevolvingDebt.ExtraPaymentMonth).ResponseValue);
                        lesson2.DebtReductionOptions.IncreaseMonthlyPayment = Convert.ToDecimal(userResponses.First(r => r.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson2.DebtReductionOptions.OptionsToReduceYourRevolvingDebt.IncreaseYourMonthlyPayments).ResponseValue);
                        lesson2.DebtReductionOptions.LowerYourInterestRate = Convert.ToBoolean(userResponses.First(r => r.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson2.DebtReductionOptions.OptionsToReduceYourRevolvingDebt.LowerYourInterestRate).ResponseValue);
                        lesson2.DebtReductionOptions.LoweredInterestRate = Convert.ToDecimal(userResponses.First(r => r.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson2.DebtReductionOptions.OptionsToReduceYourRevolvingDebt.LoweredInterestRate).ResponseValue);
                        lesson2.DebtReductionOptions.PlanAnExtraPayment = Convert.ToBoolean(userResponses.First(r => r.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson2.DebtReductionOptions.OptionsToReduceYourRevolvingDebt.PlanAnExtraPayment).ResponseValue);
                        lesson2.DebtReductionOptions.PayCashForOneTimePurchases = Convert.ToBoolean(userResponses.First(r => r.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson2.DebtReductionOptions.OptionsToReduceYourRevolvingDebt.PayCashForOneTimePurchases).ResponseValue);
                        lesson2.DebtReductionOptions.PayCashForRecurringExpenses = Convert.ToBoolean(userResponses.First(r => r.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson2.DebtReductionOptions.OptionsToReduceYourRevolvingDebt.PayCashForRecurringExpenses).ResponseValue);
                    }
                }

                var lesson1 = _memberLessonRepository.Get(ml => (ml.LessonUserId == userLesson.LessonUserId ) && ml.LessonId == LessonTypes.HowDoesYourCashFlow).FirstOrDefault();

                if (lesson1 != null)
                {
                    var userLesson1Responses = GetUserLesson1Results(lesson1.LessonUserId);

                    if (userLesson1Responses != null && userLesson1Responses.Expenses!=null && userLesson1Responses.Expenses.Any())
                    {
                        lesson2.ImportedExpenses = new List<Expense>();
                        foreach (var expense in userLesson1Responses.Expenses.Where(expense => expense.PaidByCreditCard))
                        {
                            lesson2.ImportedExpenses.Add(expense);
                        }
                    }
                }
            }

            return lesson2;
        }

        /// <summary>
        /// Gets the user lesson3 results.
        /// </summary>
        /// <param name="lessonUserId">The lesson user id.</param>
        /// <param name="lessonStepId">The lesson step id.</param>
        /// <returns></returns>
        public Lesson3 GetUserLesson3Results(int lessonUserId, int? lessonStepId = null)
        {
            var userLesson = _memberLessonRepository.Get(ml => ml.LessonUserId == lessonUserId && ml.LessonId==LessonTypes.OwnYourStudentLoans).FirstOrDefault();
            var lesson3 = new Lesson3() { User = userLesson };

            if (userLesson != null)
            {
                var userResponses = _lessonQuestionResponseRepository.Get(r => r.MemberLessonId == userLesson.MemberLessonId && r.MemberLesson.LessonId == LessonTypes.OwnYourStudentLoans && (!lessonStepId.HasValue || r.LessonQuestionAttribute.LessonQuestion.LessonStepId == lessonStepId.Value), null, "LessonQuestionAttribute").ToList();
                if (userResponses.Any())
                {
                    lesson3.LoanTypes = new List<LoanType>();
                    lesson3.FavoriteRepaymentPlans = new List<FavoriteRepaymentPlan>();

                    if (userResponses.Any(r => r.LessonQuestionAttribute.LessonQuestionId == LessonQuestions.Lesson3.WhatTypeOfRepaymentOptionsWouldYouLikeToExplore))
                    {
                        foreach (var loan in userResponses.Where(r => r.LessonQuestionAttribute.LessonQuestionId == LessonQuestions.Lesson3.WhatTypeOfRepaymentOptionsWouldYouLikeToExplore).GroupBy(i => i.GroupingNumber))
                        {
                            var loanType = new LoanType()
                            {
                                DegreeType = loan.First(i => i.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson3.YourBalance.WhatTypeOfRepaymentOptionsWouldYouLikeToExplore.DegreeType).ResponseValue,
                                Amount = Convert.ToDecimal(loan.First(i => i.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson3.YourBalance.WhatTypeOfRepaymentOptionsWouldYouLikeToExplore.TotalLoanAmount).ResponseValue),
                                AnnualIncome = Convert.ToDecimal(loan.First(i => i.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson3.YourBalance.WhatTypeOfRepaymentOptionsWouldYouLikeToExplore.AnnualIncome).ResponseValue),
                                FinancialDependents = Convert.ToInt32(loan.First(i => i.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson3.YourBalance.WhatTypeOfRepaymentOptionsWouldYouLikeToExplore.FinancialDependents).ResponseValue),
                                State = loan.First(i => i.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson3.YourBalance.WhatTypeOfRepaymentOptionsWouldYouLikeToExplore.State).ResponseValue,
                                InterestRate = Convert.ToDecimal(loan.First(i => i.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson3.YourBalance.WhatTypeOfRepaymentOptionsWouldYouLikeToExplore.InterestRate).ResponseValue),
                                Id = loan.First().GroupingNumber
                            };
                            lesson3.LoanTypes.Add(loanType);
                        }
                    }

                    if (userResponses.Any(r => r.LessonQuestionAttribute.LessonQuestionId == LessonQuestions.Lesson3.PayMoreEachMonth))
                    {
                        lesson3.FasterRepaymentOptions.AdditionalMonthlyPayment = new AdditionalPayment()
                        {
                            MonthlyPayment = Convert.ToDecimal(userResponses.First(r => r.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson3.PayItDownFaster.PayMoreEachMonth.AdditionalMonthlyPayment).ResponseValue)
                        };
                    }

                    if (userResponses.Any(r => r.LessonQuestionAttribute.LessonQuestionId == LessonQuestions.Lesson3.SetYourOwnTimeline))
                    {
                        lesson3.FasterRepaymentOptions.LoanPaymentTimeline = new CustomTimeline()
                        {
                           Timeline  = Convert.ToInt32(userResponses.First(r => r.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson3.PayItDownFaster.SetYourOwnTimeline.LoanPaymentTimeline).ResponseValue)
                        };
                    }

                    if (userResponses.Any(r => r.LessonQuestionAttribute.LessonQuestionId == LessonQuestions.Lesson3.EarnABetterInterestRate))
                    {
                        lesson3.FasterRepaymentOptions.BetterInterestRate = new BetterInterestRate()
                        {
                            LowerInterestRate = Convert.ToDecimal(userResponses.First(r => r.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson3.PayItDownFaster.EarnABetterInterestRate.LoweredInterestRate).ResponseValue)
                        };
                    }

                    //if (userResponses.Any(r => r.LessonQuestionAttribute.LessonQuestionId == LessonQuestions.Lesson3.IncomeBased))
                    //{
                    //    lesson3.LowerPaymentOptions.IncomeBasedRepayment = new IncomeBasedRepayment()
                    //    {
                    //        AnnualIncome = Convert.ToDecimal(userResponses.First(r => r.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson3.LoweringYourPayments.IncomeBased.AnnualIncome).ResponseValue),
                    //        FinancialDependents = Convert.ToInt32(userResponses.First(r => r.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson3.LoweringYourPayments.IncomeBased.FinancialDependents).ResponseValue)
                    //    };
                    //}

                    if (userResponses.Any(r => r.LessonQuestionAttribute.LessonQuestionId == LessonQuestions.Lesson3.Extended))
                    {
                        lesson3.LowerPaymentOptions.ExtendedRepayment = new ExtendedRepayment()
                        {
                            ExtensionYears = Convert.ToInt32(userResponses.First(r => r.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson3.LoweringYourPayments.Extended.RepaymentExtensionYears).ResponseValue)
                        };
                    }

                    if (userResponses.Any(r => r.LessonQuestionAttribute.LessonQuestionId == LessonQuestions.Lesson3.IncomeSensitive))
                    {
                        lesson3.LowerPaymentOptions.IncomeSensitiveRepayment = new IncomeSensitiveRepayment()
                        {
                            //AnnualIncome = Convert.ToDecimal(userResponses.First(r => r.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson3.LoweringYourPayments.IncomeSensitive.AnnualIncome).ResponseValue),
                            IncomePercentage = Convert.ToDouble(userResponses.First(r => r.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson3.LoweringYourPayments.IncomeSensitive.IncomePercentage).ResponseValue)
                        };
                    }

                    if (userResponses.Any(r => r.LessonQuestionAttribute.LessonQuestionId == LessonQuestions.Lesson3.InSchoolDeferment))
                    {
                        lesson3.DefermentOptions.InSchoolDeferment = new InSchoolDeferment()
                        {
                            DefermentMonths = Convert.ToInt32(userResponses.First(r => r.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson3.DefermentAndForebearance.InSchoolDeferment.DefermentMonths).ResponseValue)
                        };
                    }

                    if (userResponses.Any(r => r.LessonQuestionAttribute.LessonQuestionId == LessonQuestions.Lesson3.HardshipDeferment))
                    {
                        lesson3.DefermentOptions.HardshipDeferment = new HardshipDeferment()
                        {
                            DefermentMonths = Convert.ToInt32(userResponses.First(r => r.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson3.DefermentAndForebearance.HardshipDeferment.DefermentMonths).ResponseValue),
                            ExtraAmount = Convert.ToDecimal(userResponses.First(r => r.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson3.DefermentAndForebearance.HardshipDeferment.ExtraAmount).ResponseValue)
                        };
                    }

                    if (userResponses.Any(r => r.LessonQuestionAttribute.LessonQuestionId == LessonQuestions.Lesson3.Forbearance))
                    {
                        lesson3.DefermentOptions.Forbearance = new Forbearance()
                        {
                            ForbearanceMonths = Convert.ToInt32(userResponses.First(r => r.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson3.DefermentAndForebearance.Forbearance.ForbearanceMonths).ResponseValue)
                        };
                    }

                    if (userResponses.Any(r => r.LessonQuestionAttribute.LessonQuestionId == LessonQuestions.Lesson3.TakeAMomentToReviewYourFavoritePlans))
                    {
                        lesson3.FavoriteRepaymentPlans = new List<FavoriteRepaymentPlan>();
                        foreach (var favoritePlan in userResponses.Where(r => r.LessonQuestionAttribute.LessonQuestionId == LessonQuestions.Lesson3.TakeAMomentToReviewYourFavoritePlans).GroupBy(i => i.GroupingNumber))
                        {
                            var plan = new FavoriteRepaymentPlan()
                            {
                                RepaymentPlan = favoritePlan.First(i => i.LessonQuestionAttributeId == LessonQuestionAttributes.Lesson3.ReviewYourFavorites.TakeAMomentToReviewYourFavoritePlans.FavoritePlan).ResponseValue,
                                Id = favoritePlan.First().GroupingNumber
                            };
                            lesson3.FavoriteRepaymentPlans.Add(plan);
                        }
                    }
                }
            }

            return lesson3;
        }


        /// <summary>
        /// Creates the lesson question response.
        /// </summary>
        /// <param name="userLessonId">The user lesson id.</param>
        /// <param name="attributeId">The attribute id.</param>
        /// <param name="response">The response.</param>
        /// <param name="groupingNumber">The grouping number.</param>
        /// <returns></returns>
        private LessonQuestionResponse CreateLessonQuestionResponse(int userLessonId, int attributeId, string response, int? groupingNumber)
        {
            return new LessonQuestionResponse()
                {
                    ResponseValue = response ?? string.Empty,
                    LessonQuestionAttributeId = attributeId,
                    MemberLessonId = userLessonId,
                    CreatedBy = Principal.GetIdentity(),
                    CreatedDate = DateTime.Now,
                    GroupingNumber = groupingNumber.HasValue?groupingNumber.Value:1
                };
        }

        /// <summary>
        /// Gets the member lesson.
        /// </summary>
        /// <param name="lessonUserId">The lesson user id.</param>
        /// <returns></returns>
        public IList<MemberLesson> GetUserLessons(int lessonUserId)
        {
            return _memberLessonRepository.Get(ml => ml.LessonUserId == lessonUserId).ToList();
        }


        /// <summary>
        /// Updates the user lesson.
        /// </summary>
        /// <param name="userLesson">The user lesson.</param>
        /// <returns></returns>
        public bool UpdateUserLesson(MemberLesson userLesson)
        {
            try
            {
                var lesson =
                    _memberLessonRepository.Get(
                        ml => ml.LessonUserId == userLesson.LessonUserId && ml.LessonId == userLesson.LessonId)
                                           .OrderByDescending(ml => ml.CreatedDate)
                                           .FirstOrDefault();

                if (lesson == null) return true;

                var unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);
                lesson.CurrentStep = userLesson.CurrentStep;
                lesson.ModifiedDate = DateTime.Now;
                lesson.ModifiedBy = Principal.GetIdentity();
                _memberLessonRepository.Update(lesson);
                unitOfWork.Commit();

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                throw;
            }
        }
    }
}
