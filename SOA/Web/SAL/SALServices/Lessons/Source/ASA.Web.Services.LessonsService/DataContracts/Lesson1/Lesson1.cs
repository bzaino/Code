using System;
using System.Collections.Generic;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.LessonsService.DataContracts.Lesson1
{
    public class Lesson1 : Lessons
    {
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        //public int UserId { get; set; }

        ///// <summary>
        ///// Gets or sets the individual id.
        ///// </summary>
        ///// <value>
        ///// The individual id.
        ///// </value>
        //public Guid IndividualId { get; set; }

        ///// <summary>
        ///// Gets or sets the user.
        ///// </summary>
        ///// <value>
        ///// The user.
        ///// </value>
        //public User User { get; set; }

        /// <summary>
        /// Gets or sets the expenses.
        /// </summary>
        /// <value>
        /// The expenses.
        /// </value>
        public IList<Expense> Expenses { get; set; }

        /// <summary>
        /// Gets or sets the incomes.
        /// </summary>
        /// <value>
        /// The incomes.
        /// </value>
        public IList<Income> Incomes { get; set; }

        /// <summary>
        /// Gets or sets the goal.
        /// </summary>
        /// <value>
        /// The goal.
        /// </value>
        public Goal Goal { get; set; }

        public override bool IsValid()
        {
            bool bIsValid = false;
            
            bIsValid = base.IsValid();

            if (this.Expenses != null)
                    foreach (Expense exp in this.Expenses)
                        bIsValid &= exp.IsValid();

            if (this.Incomes != null)
                foreach (Income inc in this.Incomes)
                    bIsValid &= inc.IsValid();

            if (this.Goal != null)
                bIsValid &= Goal.IsValid();

            return bIsValid;
        }

        public IList<ErrorModel> GetAllErrors()
        {
            IList<ErrorModel> errors = new List<ErrorModel>();

            foreach (ErrorModel err in this.ErrorList)
                errors.Add(err);

            if (this.Expenses != null)
                foreach (Expense exp in this.Expenses)
                    foreach (ErrorModel err in exp.ErrorList)
                        errors.Add(err);

            if (this.Incomes != null)
                foreach (Income inc in this.Incomes)
                    foreach (ErrorModel err in inc.ErrorList)
                        errors.Add(err);

            if (this.Goal != null)
                foreach (ErrorModel err in this.Goal.ErrorList)
                    errors.Add(err);

            return errors;
        }

    }
}
