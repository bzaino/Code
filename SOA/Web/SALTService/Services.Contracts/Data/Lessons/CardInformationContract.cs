using System;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data.Lessons
{
    public class CardInformationContract
    {
        /// <summary>
        /// Gets or sets the balance.
        /// </summary>
        /// <value>
        /// The balance.
        /// </value>
        [DataMember]
        public decimal Balance { get; set; }

        /// <summary>
        /// Gets or sets the interest rate.
        /// </summary>
        /// <value>
        /// The interest rate.
        /// </value>
        [DataMember]
        public decimal InterestRate { get; set; }

        /// <summary>
        /// Gets or sets the monthly payment.
        /// </summary>
        /// <value>
        /// The monthly payment.
        /// </value>
        [DataMember]
        public decimal MonthlyPayment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [makes minimum payment].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [makes minimum payment]; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool MakesMinimumPayment { get; set; }



        /// <summary>
        /// Gets or sets the lesson user id.
        /// </summary>
        /// <value>
        /// The lesson user id.
        /// </value>
        [DataMember]
        public int LessonUserId { get; set; }
    }
}