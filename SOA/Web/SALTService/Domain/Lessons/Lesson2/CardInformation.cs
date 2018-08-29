namespace Asa.Salt.Web.Services.Domain.Lessons.Lesson2
{
    public class CardInformation
    {
        /// <summary>
        /// Gets or sets the balance.
        /// </summary>
        /// <value>
        /// The balance.
        /// </value>
        public decimal Balance { get; set; }

        /// <summary>
        /// Gets or sets the interest rate.
        /// </summary>
        /// <value>
        /// The interest rate.
        /// </value>
        public decimal InterestRate { get; set; }

        /// <summary>
        /// Gets or sets the monthly payment.
        /// </summary>
        /// <value>
        /// The monthly payment.
        /// </value>
        public decimal MonthlyPaymentAmount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [makes minimum payment].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [makes minimum payment]; otherwise, <c>false</c>.
        /// </value>
        public bool MakesMinimumPayment { get; set; }


        /// <summary>
        /// Gets or sets the lesson user id.
        /// </summary>
        /// <value>
        /// The lesson user id.
        /// </value>
        public int LessonUserId { get; set; }
    }
} 