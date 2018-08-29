using System;

namespace ASA.Web.Services.LessonsService.DataContracts.Lesson1
{
    public class Expense 
    {

        /// <summary>
        /// Gets or sets the expense id.
        /// </summary>
        /// <value>
        /// The expense id.
        /// </value>
        public int ExpenseId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public Decimal Value { get; set; }

        /// <summary>
        /// Gets or sets the frequency id.
        /// </summary>
        /// <value>
        /// The frequency id.
        /// </value>
        public int? FrequencyId { get; set; }

        /// <summary>
        /// Gets or sets the frequency.
        /// </summary>
        /// <value>
        /// The frequency.
        /// </value>
        public virtual Frequency Frequency { get; set; }

        /// <summary>
        /// Gets or sets the parent expense id.
        /// </summary>
        /// <value>
        /// The parent expense id.
        /// </value>
        public int ParentExpenseId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Expense"/> is recurring.
        /// </summary>
        /// <value>
        ///   <c>true</c> if recurring; otherwise, <c>false</c>.
        /// </value>
        public bool Recurring { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Expense"/> is complex.
        /// </summary>
        /// <value>
        ///   <c>true</c> if complex; otherwise, <c>false</c>.
        /// </value>
        public bool Complex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [credit expense].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [credit expense]; otherwise, <c>false</c>.
        /// </value>
        public bool CreditExpense { get; set; }

        /// <summary>
        /// Gets or sets the lesson user id.
        /// </summary>
        /// <value>
        /// The lesson user id.
        /// </value>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the name of the parent.
        /// </summary>
        /// <value>
        /// The name of the parent.
        /// </value>
        public string ParentName { get; set; }

       

    
    }

   
}