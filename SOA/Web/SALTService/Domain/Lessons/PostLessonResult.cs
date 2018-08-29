using Asa.Salt.Web.Common.Types.Enums;

namespace Asa.Salt.Web.Services.Domain.Lessons
{
    public class PostLessonResult<TLessonType>
    {

        /// <summary>
        /// Gets or sets the update status.
        /// </summary>
        /// <value>
        /// The update status.
        /// </value>
        public LessonUpdateStatus UpdateStatus { get; set; }


        /// <summary>
        /// Gets or sets the lesson.
        /// </summary>
        /// <value>
        /// The lesson.
        /// </value>
        public virtual TLessonType Lesson { get; set; }
    }
}
