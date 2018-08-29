using System;

namespace ASA.Web.Services.LessonsService.DataContracts.Lesson2
{    
    public class RecurringExpense 
    {
        /// <summary>
        /// Gets or sets the recurring expense id.
        /// </summary>
        /// <value>
        /// The recurring expense id.
        /// </value>
        public int RecurringExpenseId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

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
        /// Gets or sets the lesson user id.
        /// </summary>
        /// <value>
        /// The lesson user id.
        /// </value>
        public int UserId { get; set; }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="recurringExpense">The recurring expense.</param>
        /// <returns></returns>
        public bool Equals(RecurringExpense recurringExpense)
        {
            return recurringExpense.Name.Equals(this.Name, StringComparison.OrdinalIgnoreCase) &&
                   recurringExpense.FrequencyId == this.FrequencyId && recurringExpense.Value == this.Value;
        }

    }
}