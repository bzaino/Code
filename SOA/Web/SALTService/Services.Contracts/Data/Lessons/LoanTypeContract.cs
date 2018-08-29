using System;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data.Lessons
{
    public class LoanTypeContract
    {
        /// <summary>
        /// Gets or sets the name of the loan.
        /// </summary>
        /// <value>
        /// The name of the loan.
        /// </value>
        [DataMember]
        public string LoanName { get; set; }

        /// <summary>
        /// Gets or sets the loan amount.
        /// </summary>
        /// <value>
        /// The loan amount.
        /// </value>
        [DataMember]
        public Decimal LoanAmount { get; set; }


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