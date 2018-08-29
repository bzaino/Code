using System;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data.Lessons
{
    public class FavoriteContract
    {
        /// <summary>
        /// Gets or sets the name of the repayment.
        /// </summary>
        /// <value>
        /// The name of the repayment.
        /// </value>
        [DataMember]
        public string RepaymentName { get; set; }



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