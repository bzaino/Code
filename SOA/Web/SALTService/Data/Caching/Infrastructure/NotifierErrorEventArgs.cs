using System;
using System.Data.SqlClient;

namespace Data.Caching.Infrastructure
{
    public class NotifierErrorEventArgs : EventArgs
    {
        public string Sql { get; set; }
        public SqlNotificationEventArgs Reason { get; set; }
    }
}
