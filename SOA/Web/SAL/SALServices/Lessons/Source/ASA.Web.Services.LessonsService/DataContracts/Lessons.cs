using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.Common;
using System.Text.RegularExpressions;

namespace ASA.Web.Services.LessonsService.DataContracts
{
    public class Lessons : BaseWebModel
    {
        /// <summary>
        /// Gets or sets the user.: BaseWebModel
        /// </summary>
        /// <value>
        /// The user.
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
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        public User User { get; set; }

        public override bool IsValid()
        {
            bool bIsValid = false;
            var mv = new ASAModelValidator();
            bIsValid = base.IsValid();

            if (this.User != null)
                bIsValid &= this.User.IsValid();

            return bIsValid;
        }

        public void AddError(string errorMessage)
        {
            this.ErrorList.Add(new ErrorModel(errorMessage));
        }

    }
}
