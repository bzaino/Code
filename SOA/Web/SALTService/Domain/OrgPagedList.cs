using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asa.Salt.Web.Common.Types.Enums;

namespace Asa.Salt.Web.Services.Domain
{
    public class OrgPagedList
    {
        /// <summary>
        /// Gets or sets the organizations.
        /// </summary>
        /// <value>
        /// The organizations.
        /// </value>
        public IList<RefOrganization> Organizations { get; set; }

        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        /// <value>
        /// The current page.
        /// </value>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>
        /// The size of the page.
        /// </value>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the total page count.
        /// </summary>
        /// <value>
        /// The total page count.
        /// </value>
        public int TotalPageCount { get; set; }

        /// <summary>
        /// Gets or sets the total organization count.
        /// </summary>
        /// <value>
        /// The total organization count.
        /// </value>
        public int TotalOrgCount { get; set; }
    }
}
