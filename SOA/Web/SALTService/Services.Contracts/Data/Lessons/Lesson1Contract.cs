using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data.Lessons
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(MemberLessonContract))]
    [KnownType(typeof(ExpenseContract))]
    [KnownType(typeof(IncomeContract))]
    [KnownType(typeof(GoalContract))]
    public class Lesson1Contract
    {
        public Lesson1Contract()
        {
            Goal = new GoalContract();
            Expenses = new HashSet<ExpenseContract>();
            Incomes = new HashSet<IncomeContract>();
        }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        [DataMember]
        public MemberLessonContract User { get; set; }

        /// <summary>
        /// Gets or sets the expenses.
        /// </summary>
        /// <value>
        /// The expenses.
        /// </value>
        [DataMember]
        public ICollection<ExpenseContract> Expenses { get; set; }

        /// <summary>
        /// Gets or sets the incomes.
        /// </summary>
        /// <value>
        /// The incomes.
        /// </value>
        [DataMember]
        public ICollection<IncomeContract> Incomes { get; set; }

        /// <summary>
        /// Gets or sets the goal.
        /// </summary>
        /// <value>
        /// The goal.
        /// </value>
        [DataMember]
        public GoalContract Goal { get; set; }
    }
}
