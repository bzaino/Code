using System;
using System.Data.SqlClient;

namespace Asa.Salt.Web.Services.Data.Caching.Infrastructure.SqlDependency
{
    public class NotifierErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the SQL.
        /// </summary>
        /// <value>
        /// The SQL.
        /// </value>
        public string Sql { get; set; }

        /// <summary>
        /// Gets or sets the reason.
        /// </summary>
        /// <value>
        /// The reason.
        /// </value>
        public SqlNotificationEventArgs Reason { get; set; }
    }
}
