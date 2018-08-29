using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asa.Salt.Web.Services.Domain
{
    public partial class SurveyOption
    {
        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public override int Id
        {
            get { return this.SurveyOptionId; }
        }

        /// <summary>
        /// Gets or sets the total response count.
        /// </summary>
        /// <value>
        /// The total response count.
        /// </value>
        public int TotalResponseCount { get; set; }
    }
}
