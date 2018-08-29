using System;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Runtime.Caching;

namespace Asa.Salt.Web.Services.Data.Caching.Infrastructure.SqlDependency
{
    public class EntityChangeMonitor<TEntity, TDbContext> : ChangeMonitor,
        IDisposable
        where TDbContext : DbContext, new()
        where TEntity : class
    {
        private DbContext _context;

        /// <summary>
        /// The query
        /// </summary>
        private Expression<Func<TEntity, bool>> _query;

        /// <summary>
        /// The change notifier
        /// </summary>
        private EntityChangeNotifier<TEntity, TDbContext> _changeNotifier;

        /// <summary>
        /// The unique id
        /// </summary>
        private readonly string _uniqueId;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityChangeMonitor{TEntity, TDbContext}" /> class.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="timeout">The timeout.</param>
        public EntityChangeMonitor(Expression<Func<TEntity, bool>> query, TimeSpan? timeout)
        {
            _context = new TDbContext();
            _query = query;
            _uniqueId = Guid.NewGuid().ToString();
            _changeNotifier = new EntityChangeNotifier<TEntity, TDbContext>(_query, timeout);

            _changeNotifier.Error += _changeNotifier_Error;
            _changeNotifier.Changed += _changeNotifier_Changed;

            InitializationComplete();
        }

        /// <summary>
        /// Handles the Error event of the _changeNotifier control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="NotifierErrorEventArgs"/> instance containing the event data.</param>
        void _changeNotifier_Error(object sender, NotifierErrorEventArgs e)
        {
            base.OnChanged(null);
        }

        /// <summary>
        /// The OnChange event of the change notifier.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        void _changeNotifier_Changed(object sender, EntityChangeEventArgs<TEntity> e)
        {
            base.OnChanged(e.Results);
        }

        /// <summary>
        /// Releases all managed and unmanaged resources and any references to the <see cref="T:System.Runtime.Caching.ChangeMonitor" /> instance. This overload must be implemented by derived change-monitor classes.
        /// </summary>
        /// <param name="disposing">true to release managed and unmanaged resources and any references to a <see cref="T:System.Runtime.Caching.ChangeMonitor" /> instance; false to release only unmanaged resources. When false is passed, the <see cref="M:System.Runtime.Caching.ChangeMonitor.Dispose(System.Boolean)" /> method is called by a finalizer thread and any external managed references are likely no longer valid because they have already been garbage collected.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _query = null;

                if (_changeNotifier != null)
                {
                    _changeNotifier.Error -= _changeNotifier_Error;
                    _changeNotifier.Changed -= _changeNotifier_Changed;

                    _changeNotifier.Dispose();
                    _changeNotifier = null;
                }
                _context.Dispose();
                _context = null;
            }
        }

        /// <summary>
        /// Gets a value that represents the <see cref="T:System.Runtime.Caching.ChangeMonitor" /> class instance.
        /// </summary>
        /// <returns>The identifier for a change-monitor instance.</returns>
        public override string UniqueId
        {
            get { return _uniqueId; }
        }
    }
}
