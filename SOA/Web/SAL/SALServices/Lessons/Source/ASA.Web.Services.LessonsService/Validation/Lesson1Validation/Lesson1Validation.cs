using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.LessonsService.DataContracts;
using ASA.Web.Services.LessonsService.DataContracts.Lesson1;

namespace ASA.Web.Services.LessonsService.Validation.Lesson1Validation
{
    public class Lesson1Validation : LessonsValidation
    {
        public static bool ValidateExpenses(IList<Expense> expenses)
        {
            bool bValid = false;
            //expenses validation goes here

            return bValid;
        }

        public static bool ValidateIncome(IList<Income> incomes)
        {
            bool bValid = false;
            //incomes validation goes here

            return bValid;
        }

        public static bool ValidateGoal(Goal goal)
        {
            bool bValid = false;
            //expenses validation goes here
            bValid = GoalValidation.ValidateName(goal.Name) && GoalValidation.ValidateMonths(goal.Months) && GoalValidation.ValidateUserId(goal.UserId) && GoalValidation.ValidateValue(goal.Value);

            return bValid;
        }

        public static bool ValidateLesson()
        {
            bool bValid = false;


            return bValid;
        }
    }
}
