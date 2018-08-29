using System;
using System.Collections.Generic;

namespace Asa.Salt.Web.Services.Domain.Lessons.Lesson3
{
    public class Deferment 
    {

        /// <summary>
        /// Gets or sets the in school deferment.
        /// </summary>
        /// <value>
        /// The in school deferment.
        /// </value>
        public InSchoolDeferment InSchoolDeferment { get; set; }

        /// <summary>
        /// Gets or sets the hardship deferment.
        /// </summary>
        /// <value>
        /// The hardship deferment.
        /// </value>
        public HardshipDeferment HardshipDeferment { get; set; }

        /// <summary>
        /// Gets or sets the forbearance.
        /// </summary>
        /// <value>
        /// The forbearance.
        /// </value>
        public Forbearance Forbearance { get; set; }

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

    public class InSchoolDeferment
    {
        /// <summary>
        /// Gets or sets the length of the deferment.
        /// </summary>
        /// <value>
        /// The length of the deferment.
        /// </value>
        public int DefermentMonths { get; set; }

    }

    public class HardshipDeferment
    {
       
        /// <summary>
        /// Gets or sets the length of the hardship deferment.
        /// </summary>
        /// <value>
        /// The length of the hardship deferment.
        /// </value>
        public int DefermentMonths { get; set; }

        /// <summary>
        /// Gets or sets the hardship deferment extra amount.
        /// </summary>
        /// <value>
        /// The hardship deferment extra amount.
        /// </value>
        public Decimal ExtraAmount { get; set; }

    }

    public class Forbearance
    {
     
        /// <summary>
        /// Gets or sets the length of the forbearance.
        /// </summary>
        /// <value>
        /// The length of the forbearance.
        /// </value>
        public int ForbearanceMonths { get; set; }

    }
}