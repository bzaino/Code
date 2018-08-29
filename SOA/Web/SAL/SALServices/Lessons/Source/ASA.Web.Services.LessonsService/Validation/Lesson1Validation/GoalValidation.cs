using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.LessonsService.DataContracts.Lesson1;

namespace ASA.Web.Services.LessonsService.Validation.Lesson1Validation
{
    public class GoalValidation
    {
        /// <summary>
        /// The class name
        /// </summary>
        private const string Classname = "ASA.Web.Services.LessonsService.Validation.Lesson1Validation.GoalValidation";

        /// <summary>
        /// The logger
        /// </summary>
        static readonly Log.ServiceLogger.IASALog Log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(Classname);
        
        /// <summary>
        /// Initializes a new instance of the <see cref="GoalValidation"/> class.
        /// </summary>
        public GoalValidation()
        {
            Log.Info("ASA.Web.Services.ASAMemberService.GoalValidation() object being created ...");

            const string logMethodName = ".ctor() - ";
            Log.Debug(logMethodName + "Begin Method");

            Log.Debug(logMethodName + "End Method");
        }

        public static bool ValidateName(string name)
        {
            bool bValid = false;
            //expenses validation goes here

            return bValid;
        }

        public static bool ValidateMonths(decimal months)
        {
            bool bValid = false;
            //expenses validation goes here

            return bValid;
        }

        public static bool ValidateValue(decimal value)
        {
            bool bValid = false;
            //expenses validation goes here

            return bValid;
        }

        public static bool ValidateUserId(int userId)
        {
            bool bValid = false;
            //expenses validation goes here

            return bValid;
        }

        public static bool ValidateGoal(Goal goal)
        {
            bool bValid = false;
            //expenses validation goes here
            bValid = GoalValidation.ValidateName(goal.Name) && GoalValidation.ValidateMonths(goal.Months) && GoalValidation.ValidateUserId(goal.UserId) && GoalValidation.ValidateValue(goal.Value);

            return bValid;
        }
    }
}
