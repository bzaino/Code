using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace Asa.Salt.Web.Services.Data.Caching.Infrastructure.SqlDependency
{
    internal static class EntityChangeNotifier
    {
        /// <summary>
        /// The connection strings
        /// </summary>
        private static List<string> _connectionStrings;
        /// <summary>
        /// A synchronization object.
        /// </summary>
        private static object _lockObj = new object();

        static EntityChangeNotifier()
        {
            _connectionStrings = new List<string>();

            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                foreach (var cs in _connectionStrings)
                    System.Data.SqlClient.SqlDependency.Stop(cs);
            };
        }

        internal static void AddConnectionString(string cs)
        {
            lock (_lockObj)
            {
                if (!_connectionStrings.Contains(cs))
                {
                    System.Data.SqlClient.SqlDependency.Start(cs);
                    _connectionStrings.Add(cs);
                }
            }
        }
    }

    public class EntityChangeNotifier<TEntity, TDbContext>
        : IDisposable
        where TDbContext : DbContext, new()
        where TEntity : class
    {
        /// <summary>
        /// The db context
        /// </summary>
        private DbContext _context;

        /// <summary>
        /// The query being registered with broker services
        /// </summary>
        private Expression<Func<TEntity, bool>> _query;

        /// <summary>
        /// The timeout interval
        /// </summary>
        private readonly TimeSpan? _timeoutInterval;

        /// <summary>
        /// The connection string
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// Occurs when [changed].
        /// </summary>
        public event EventHandler<EntityChangeEventArgs<TEntity>> Changed;

        /// <summary>
        /// Occurs when [error].
        /// </summary>
        public event EventHandler<NotifierErrorEventArgs> Error;

        /// <summary>
        /// The sql dependency object
        /// </summary>
        private System.Data.SqlClient.SqlDependency _sqlDependency;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityChangeNotifier{TEntity, TDbContext}" /> class.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="timeout">The timeout. If a value isn't specified, the default value of 0 will use the server timeout value</param>
        public EntityChangeNotifier(Expression<Func<TEntity, bool>> query, TimeSpan? timeout)
        {
            _context = new TDbContext();
            _query = query;
            _connectionString = _context.Database.Connection.ConnectionString;
            _timeoutInterval = timeout;

            EntityChangeNotifier.AddConnectionString(_connectionString);

            RegisterNotification();
        }

        /// <summary>
        /// Registers the notification.
        /// </summary>
        private void RegisterNotification()
        {
            _context = new TDbContext();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = GetCommand())
                {
                    command.Connection = connection;
                    connection.Open();

                    _sqlDependency = new System.Data.SqlClient.SqlDependency(command, null, _timeoutInterval.HasValue && _timeoutInterval.Value.TotalSeconds > 0 ? (int)Math.Round(_timeoutInterval.Value.TotalSeconds) : 0);

                    _sqlDependency.OnChange -= _sqlDependency_OnChange;
                    _sqlDependency.OnChange += _sqlDependency_OnChange;

                    // NOTE: You have to execute the command, or the notification will never fire.
                    using (var reader = command.ExecuteReader())
                    {
                    }
                }
            }
        }


        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <returns></returns>
        private SqlCommand GetCommand()
        {
            var q = GetCurrent();

            return q.ToSqlCommand();
        }

        /// <summary>
        /// Gets the current.
        /// </summary>
        /// <returns></returns>
        private DbQuery<TEntity> GetCurrent()
        {
            var query = _context.Set<TEntity>().Where(_query).AsNoTracking() as DbQuery<TEntity>;

            return query;
        }

        /// <summary>
        /// Handles the OnChange event of the _sqlDependency control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SqlNotificationEventArgs"/> instance containing the event data.</param>
        private void _sqlDependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (_context == null)
                return;

            _sqlDependency.OnChange -= _sqlDependency_OnChange;

            if (e.Type == SqlNotificationType.Subscribe || e.Info == SqlNotificationInfo.Error)
            {
                var args = new NotifierErrorEventArgs
                {
                    Reason = e,
                    Sql = GetCurrent().ToString()
                };

                OnError(args);
            }
            else
            {
                var args = new EntityChangeEventArgs<TEntity>
                {
                    Results = GetCurrent,
                    ContinueListening = false
                };

                OnChanged(args);

                if (args.ContinueListening)
                {
                    RegisterNotification();
                }
            }
        }

        /// <summary>
        /// Called when [changed].
        /// </summary>
        /// <param name="e">The e.</param>
        protected virtual void OnChanged(EntityChangeEventArgs<TEntity> e)
        {
            if (Changed != null)
            {
                Changed(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:Error" /> event.
        /// </summary>
        /// <param name="e">The <see cref="NotifierErrorEventArgs"/> instance containing the event data.</param>
        protected virtual void OnError(NotifierErrorEventArgs e)
        {
            if (Error != null)
            {
                Error(this, e);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }

                if (_sqlDependency != null)
                {
                    _sqlDependency.OnChange -= _sqlDependency_OnChange;
                    _sqlDependency = null;
                }
                _query = null;

                Changed = null;
                Error = null;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
