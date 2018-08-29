using System;
using System.Collections.Generic;
using Asa.Salt.Web.Common.Types.Constants;
using Asa.Salt.Web.Common.Types.Enums;
using Asa.Salt.Web.Services.Domain;
using Asa.Salt.Web.Services.Domain.Lessons;
using Asa.Salt.Web.Services.Domain.Lessons.Lesson1;
using Asa.Salt.Web.Services.Domain.Lessons.Lesson2;
using Asa.Salt.Web.Services.Domain.Lessons.Lesson3;

namespace Asa.Salt.Web.Services.BusinessServices.Interfaces
{
    public interface ILessonsService
    {

        /// <summary>
        /// Gets the reference data.
        /// </summary>
        /// <param name="refDataType">Type of the ref data.</param>
        /// <returns></returns>
        IList<RefLessonLookupData> GetReferenceData(RefLessonLookupDataTypes refDataType);

        /// <summary>
        /// Gets the member lesson.
        /// </summary>
        /// <param name="lessonUserId">The lesson user id.</param>
        /// <returns></returns>
        IList<MemberLesson> GetUserLessons(int lessonUserId);

        /// <summary>
        /// Creates the user lesson.
        /// </summary>
        /// <param name="userLesson">The user lesson.</param>
        /// <returns></returns>
        MemberLesson StartUserLesson(MemberLesson userLesson);

        /// <summary>
        /// Deletes the user lesson question response.
        /// </summary>
        /// <param name="lessonUserId">The lesson user id.</param>
        /// <param name="lessonId">The lesson id.</param>
        /// <param name="questionId">The question id.</param>
        /// <param name="questionResponseId">The question response id.</param>
        /// <param name="groupNumber">The group number.</param>
        /// <returns></returns>
        bool DeleteUserLessonQuestionResponses(int lessonUserId, int lessonId, int? questionId, int? questionResponseId, int? groupNumber);

        /// <summary>
        /// Updates the user lesson.
        /// </summary>
        /// <param name="userLesson">The user lesson.</param>
        /// <returns></returns>
        bool UpdateUserLesson(MemberLesson userLesson);

        /// <summary>
        /// Associates the lessons with the user.
        /// </summary>
        /// <param name="lessonUserId">The user lesson id.</param>
        /// <param name="userId">The user id.</param>
        bool AssociateLessonsWithUser(int lessonUserId, int userId);

        /// <summary>
        /// Posts the users lesson1 data.
        /// </summary>
        /// <param name="lesson">The lesson.</param>
        /// <returns></returns>
        PostLessonResult<Lesson1> PostLesson1(Lesson1 lesson);

        /// <summary>
        /// Posts the users lesson2 data.
        /// </summary>
        /// <param name="lesson">The lesson.</param>
        /// <returns></returns>
        PostLessonResult<Lesson2> PostLesson2(Lesson2 lesson);

        /// <summary>
        /// Posts the users lesson3 data.
        /// </summary>
        /// <param name="lesson">The lesson.</param>
        /// <returns></returns>
        PostLessonResult<Lesson3> PostLesson3(Lesson3 lesson);

        /// <summary>
        /// Gets the user's lesson1 results.
        /// </summary>
        /// <param name="lessonUserId">The lesson user id.</param>
        /// <param name="lessonStepId">The lesson step id.</param>
        /// <returns></returns>
        Lesson1 GetUserLesson1Results(int lessonUserId, int? lessonStepId = null);

        /// <summary>
        /// Gets the user's lesson2 results.
        /// </summary>
        /// <param name="lessonUserId">The lesson user id.</param>
        /// <param name="lessonStepId">The lesson step id.</param>
        /// <returns></returns>
        Lesson2 GetUserLesson2Results(int lessonUserId, int? lessonStepId = null);

        /// <summary>
        /// Gets the user lesson3 results.
        /// </summary>
        /// <param name="lessonUserId">The lesson user id.</param>
        /// <param name="lessonStepId">The lesson step id.</param>
        /// <returns></returns>
        Lesson3 GetUserLesson3Results(int lessonUserId, int? lessonStepId = null);
    }
}
