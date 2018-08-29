using System;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.LessonsService.DataContracts.Lesson1
{
    public class Income : BaseWebModel
    {
        /// <summary>
        /// Gets or sets the frequency id.
        /// </summary>
        /// <value>
        /// The frequency id.
        /// </value>
        public int? FrequencyId { get; set; }

        /// <summary>
        /// Gets or sets the frequency.
        /// </summary>
        /// <value>
        /// The frequency.
        /// </value>
        public virtual Frequency Frequency { get; set; }

        /// <summary>
        /// Gets or sets the name of the income type.
        /// </summary>
        /// <value>
        /// The name of the income type.
        /// </value>
        public string IncomeTypeName { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public Decimal Value { get; set; }

        /// <summary>
        /// Gets or sets the lesson user id.
        /// </summary>
        /// <value>
        /// The lesson user id.
        /// </value>
        public int UserId { get; set; }

      

    }
}