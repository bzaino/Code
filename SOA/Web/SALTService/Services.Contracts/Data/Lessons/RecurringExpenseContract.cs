using System;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data.Lessons
{
    public class RecurringExpenseContract
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [DataMember]
        public Decimal ExpenseAmount { get; set; }

        /// <summary>
        /// Gets or sets the frequency id.
        /// </summary>
        /// <value>
        /// The frequency id.
        /// </value>
        [DataMember]
        public int? FrequencyId { get; set; }

        /// <summary>
        /// Gets or sets the frequency.
        /// </summary>
        /// <value>
        /// The frequency.
        /// </value>
        [DataMember]
        public virtual FrequencyContract Frequency { get; set; }

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