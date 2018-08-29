using System;

namespace Asa.Salt.Web.Services.Domain.Lessons
{
    public class Deferment 
    {
        /// <summary>
        /// Gets or sets the length of the deferment.
        /// </summary>
        /// <value>
        /// The length of the deferment.
        /// </value>
        public int DefermentLength { get; set; }

        /// <summary>
        /// Gets or sets the length of the hardship deferment.
        /// </summary>
        /// <value>
        /// The length of the hardship deferment.
        /// </value>
        public int HardshipDefermentLength { get; set; }

        /// <summary>
        /// Gets or sets the hardship deferment extra amount.
        /// </summary>
        /// <value>
        /// The hardship deferment extra amount.
        /// </value>
        public Decimal HardshipDefermentExtraAmount { get; set; }

        /// <summary>
        /// Gets or sets the length of the forbearance.
        /// </summary>
        /// <value>
        /// The length of the forbearance.
        /// </value>
        public int ForbearanceLength { get; set; }

        /// <summary>
        /// Gets or sets the lesson user id.
        /// </summary>
        /// <value>
        /// The lesson user id.
        /// </value>
        public int LessonUserId { get; set; }
    }
}