using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.Common;
namespace ASA.Web.Services.LessonsService.DataContracts
{
    public class User :BaseWebModel
    {
        /// <summary>
        /// Gets or sets the user id.: Lessons
        /// </summary>
        /// <value>
        /// The user id.
        /// </value>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the individual id.
        /// </summary>
        /// <value>
        /// The individual id.
        /// </value>
        public Guid IndividualId { get; set; }

        /// <summary>
        /// Gets or sets the member id.
        /// </summary>
        /// <value>
        /// The member id.
        /// </value>
        public int MemberId { get; set; }

        /// <summary>
        /// Gets or sets the lesson1 step.
        /// </summary>
        /// <value>
        /// The lesson1 step.
        /// </value>
        public int Lesson1Step { get; set; }

        /// <summary>
        /// Gets or sets the lesson2 step.
        /// </summary>
        /// <value>
        /// The lesson2 step.
        /// </value>
        public int Lesson2Step { get; set; }

        /// <summary>
        /// Gets or sets the lesson3 step.
        /// </summary>
        /// <value>
        /// The lesson3 step.
        /// </value>
        public int Lesson3Step { get; set; }

        public void AddError(string errorMessage)
        {
            this.ErrorList.Add(new ErrorModel(errorMessage));
        }


    }
}
