using System;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data.Lessons
{
    public class ExpenseContract
    {
        /// <summary>
        /// Gets or sets the expense id.
        /// </summary>
        /// <value>
        /// The expense id.
        /// </value>
         [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        [DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [DataMember]
        public Decimal ExpenseAmount { get; set; }

        /// <summary>
        /// Gets or sets the expense date.
        /// </summary>
        /// <value>
        /// The expense date.
        /// </value>
        [DataMember]
        public DateTime ExpenseDate { get; set; }

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
        /// Gets or sets the parent expense id.
        /// </summary>
        /// <value>
        /// The parent expense id.
        /// </value>
        [DataMember]
        public int ParentExpenseId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ExpenseContract"/> is recurring.
        /// </summary>
        /// <value>
        ///   <c>true</c> if recurring; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool Recurring { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ExpenseContract"/> is complex.
        /// </summary>
        /// <value>
        ///   <c>true</c> if complex; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool HasTopLevelExpense { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [credit expense].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [credit expense]; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool PaidByCreditCard { get; set; }

        /// <summary>
        /// Gets or sets the name of the parent.
        /// </summary>
        /// <value>
        /// The name of the parent.
        /// </value>
        [DataMember]
        public string ParentExpenseType { get; set; }



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