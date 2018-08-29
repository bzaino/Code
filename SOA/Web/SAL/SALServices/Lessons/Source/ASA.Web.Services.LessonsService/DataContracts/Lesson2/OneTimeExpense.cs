using System;

namespace ASA.Web.Services.LessonsService.DataContracts.Lesson2
{
    public class OneTimeExpense 
    {
        /// <summary>
        /// Gets or sets the one time expense id.
        /// </summary>
        /// <value>
        /// The one time expense id.
        /// </value>
        public int OneTimeExpenseId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the month.
        /// </summary>
        /// <value>
        /// The month.
        /// </value>
        public int Month { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public decimal Value { get; set; }

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
        /// <param name="oneTimeExpense">The one time expense.</param>
        /// <returns></returns>
        public bool Equals( OneTimeExpense oneTimeExpense)
        {
            return oneTimeExpense.Name.Equals(this.Name, StringComparison.OrdinalIgnoreCase) &&
                   oneTimeExpense.Month == this.Month && oneTimeExpense.Value == this.Value;
        }
       
    }
}