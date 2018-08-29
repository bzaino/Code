using System;

namespace Asa.Salt.Web.Services.Domain.Lessons
{
    public class Goal 
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the months.
        /// </summary>
        /// <value>
        /// The months.
        /// </value>
        public decimal Months { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public decimal TargetAmount { get; set; }

        /// <summary>
        /// Gets or sets the lesson user id.
        /// </summary>
        /// <value>
        /// The lesson user id.
        /// </value>
        public int LessonUserId { get; set; }

      
    }
}