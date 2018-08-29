using ASA.Web.Services.Common;

namespace ASA.Web.Services.LessonsService.DataContracts
{
    public class Frequency : BaseWebModel
    {

        /// <summary>
        /// Gets or sets the frequency id.
        /// </summary>
        /// <value>
        /// The frequency id.
        /// </value>
        public int FrequencyId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the months.
        /// </summary>
        /// <value>
        /// The months.
        /// </value>
        public double Months { get; set; }

      
    }
}