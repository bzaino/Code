using ASA.Web.Services.Common;

namespace ASA.Web.Services.LessonsService.DataContracts.Lesson1
{
    public class Goal : BaseWebModel
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
        public decimal Value { get; set; }

        /// <summary>
        /// Gets or sets the lesson user id.
        /// </summary>
        /// <value>
        /// The lesson user id.
        /// </value>
        public int UserId { get; set; }

      
    }
}