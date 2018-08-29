using System.Runtime.Serialization;
using Asa.Salt.Web.Common.Types.Enums;

namespace Asa.Salt.Web.Services.Contracts.Data.Lessons
{
    public class PostLessonResultContract<TType>
    {
        
        /// <summary>
        /// Gets or sets the update status.
        /// </summary>
        /// <value>
        /// The update status.
        /// </value>
        [DataMember]
        public LessonUpdateStatus UpdateStatus { get; set; }


        /// <summary>
        /// Gets or sets the lesson.
        /// </summary>
        /// <value>
        /// The lesson.
        /// </value>
        [DataMember]
        public virtual TType Lesson { get; set; }
    }
}
