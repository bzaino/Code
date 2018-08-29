using System.Collections.Generic;

namespace Asa.Salt.Web.Services.Domain.Lessons.Lesson3
{
    public class StandardRepayment
    {
        /// <summary>
        /// Gets or sets the favorite repayment plan.
        /// </summary>
        /// <value>
        /// The favorite repayment plan.
        /// </value>
        public IList<string> FavoriteRepaymentPlans { get; set; }

        /// <summary>
        /// Gets or sets the lesson user id.
        /// </summary>
        /// <value>
        /// The lesson user id.
        /// </value>
        public int LessonUserId { get; set; }
    }
}