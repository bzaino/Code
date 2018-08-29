using System;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data.Lessons
{
    public class DefermentContract
    {
        /// <summary>
        /// Gets or sets the length of the deferment.
        /// </summary>
        /// <value>
        /// The length of the deferment.
        /// </value>
        [DataMember]
        public int DefermentLength { get; set; }

        /// <summary>
        /// Gets or sets the length of the hardship deferment.
        /// </summary>
        /// <value>
        /// The length of the hardship deferment.
        /// </value>
        [DataMember]
        public int HardshipDefermentLength { get; set; }

        /// <summary>
        /// Gets or sets the hardship deferment extra amount.
        /// </summary>
        /// <value>
        /// The hardship deferment extra amount.
        /// </value>
        [DataMember]
        public Decimal HardshipDefermentExtraAmount { get; set; }

        /// <summary>
        /// Gets or sets the length of the forbearance.
        /// </summary>
        /// <value>
        /// The length of the forbearance.
        /// </value>
        [DataMember]
        public int ForbearanceLength { get; set; }



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