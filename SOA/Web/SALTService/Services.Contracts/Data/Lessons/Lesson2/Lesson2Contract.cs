using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data.Lessons.Lesson2
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(MemberLessonContract))]
    [KnownType(typeof(ExpenseContract))]
    [KnownType(typeof(CardInformationContract))]
    [KnownType(typeof(DebtReductionOptionsContract))]
    public class Lesson2Contract
    {
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        [DataMember]
        public MemberLessonContract User { get; set; }

        /// <summary>
        /// Gets or sets the current balance.
        /// </summary>
        /// <value>
        /// The current balance.
        /// </value>
        [DataMember]
        public CardInformationContract CurrentBalance { get; set; }

        /// <summary>
        /// Gets or sets the imported expenses.
        /// </summary>
        /// <value>
        /// The imported expenses.
        /// </value>
        [DataMember]
        public IList<ExpenseContract> ImportedExpenses { get; set; }

        /// <summary>
        /// Gets or sets the recurring expenses.
        /// </summary>
        /// <value>
        /// The recurring expenses.
        /// </value>
        [DataMember]
        public IList<ExpenseContract> RecurringExpenses { get; set; }

        /// <summary>
        /// Gets or sets the one time expenses.
        /// </summary>
        /// <value>
        /// The one time expenses.
        /// </value>
        [DataMember]
        public IList<ExpenseContract> OneTimeExpenses { get; set; }

        /// <summary>
        /// Gets or sets the debt reduction options.
        /// </summary>
        /// <value>
        /// The debt reduction options.
        /// </value>
        [DataMember]
        public DebtReductionOptionsContract DebtReductionOptions { get; set; }
    }
}
