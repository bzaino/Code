using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asa.Salt.Web.Services.Domain.Lessons.Lesson3
{
    public class FavoriteRepaymentPlan
    {
        /// <summary>
        /// Gets or sets the favorite repayment plan id.
        /// </summary>
        /// <value>
        /// The favorite repayment plan id.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the repayment plan.
        /// </summary>
        /// <value>
        /// The repayment plan.
        /// </value>
        public string RepaymentPlan { get; set; }
    }
}
