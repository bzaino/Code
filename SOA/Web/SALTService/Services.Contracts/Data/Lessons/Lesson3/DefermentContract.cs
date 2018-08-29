using System;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data.Lessons.Lesson3
{
    public class DefermentContract
    {

        /// <summary>
        /// Gets or sets the in school deferment.
        /// </summary>
        /// <value>
        /// The in school deferment.
        /// </value>
        [DataMember]
        public InSchoolDefermentContract InSchoolDeferment { get; set; }

        /// <summary>
        /// Gets or sets the hardship deferment.
        /// </summary>
        /// <value>
        /// The hardship deferment.
        /// </value>
        [DataMember]
        public HardshipDefermentContract HardshipDeferment { get; set; }

        /// <summary>
        /// Gets or sets the forbearance.
        /// </summary>
        /// <value>
        /// The forbearance.
        /// </value>
        [DataMember]
        public ForbearanceContract Forbearance { get; set; }

        /// <summary>
        /// Gets or sets the lesson user id.
        /// </summary>
        /// <value>
        /// The lesson user id.
        /// </value>
        [DataMember]
        public int LessonUserId { get; set; }
    }

    public class InSchoolDefermentContract
    {
    
        /// <summary>
        /// Gets or sets the length of the deferment.
        /// </summary>
        /// <value>
        /// The length of the deferment.
        /// </value>
        [DataMember]
        public int DefermentMonths { get; set; }

    }

    public class  HardshipDefermentContract
    {
        /// <summary>
        /// Gets or sets the length of the hardship deferment.
        /// </summary>
        /// <value>
        /// The length of the hardship deferment.
        /// </value>
        [DataMember]
        public int DefermentMonths { get; set; }

        /// <summary>
        /// Gets or sets the hardship deferment extra amount.
        /// </summary>
        /// <value>
        /// The hardship deferment extra amount.
        /// </value>
        [DataMember]
        public Decimal ExtraAmount { get; set; }

    }

    public class ForbearanceContract
    {
    
        /// <summary>
        /// Gets or sets the length of the forbearance.
        /// </summary>
        /// <value>
        /// The length of the forbearance.
        /// </value>
        [DataMember]
        public int ForbearanceMonths { get; set; }

    }
}