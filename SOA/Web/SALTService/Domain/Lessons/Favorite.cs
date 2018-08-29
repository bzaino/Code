using System;

namespace Asa.Salt.Web.Services.Domain.Lessons
{
    public class Favorite
    {
        /// <summary>
        /// Gets or sets the name of the repayment.
        /// </summary>
        /// <value>
        /// The name of the repayment.
        /// </value>
        public string RepaymentName { get; set; }

        /// <summary>
        /// Gets or sets the lesson user id.
        /// </summary>
        /// <value>
        /// The lesson user id.
        /// </value>
        public int LessonUserId { get; set; }
    }
}