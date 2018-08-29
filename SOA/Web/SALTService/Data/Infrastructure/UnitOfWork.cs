using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;
using System.Transactions;
using IsolationLevel = System.Transactions.IsolationLevel;

namespace Asa.Salt.Web.Services.Data.Infrastructure
{
    public class UnitOfWork<TDbContext>: IUnitOfWork  where TDbContext : DbContext
    {
        /// <summary>
        /// The database context
        /// </summary>
        private readonly TDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork{TDbContext}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <exception cref="System.ArgumentNullException">context wasn't supplied</exception>
        public UnitOfWork(TDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context wasn't supplied");
            }

            _dbContext = context;
        }

        /// <summary>
        /// Commits the transaction.
        /// </summary>
        public void Commit()
        {
            Save();
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        private void Save()
        {
            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    var dbValues = entry.GetDatabaseValues();
                    if (dbValues != null) entry.OriginalValues.SetValues(dbValues);
                }

                Save();
            }
            catch (Exception ex)
            {
                //added to resolve compiler warnings.
                string s = String.Empty;
                s = ex.Message;

                var changedEntries = _dbContext.ChangeTracker.Entries().Where(x => x.State != EntityState.Unchanged).ToList();

                foreach (var entry in changedEntries.Where(x => x.State == EntityState.Modified))
                {
                    entry.CurrentValues.SetValues(entry.OriginalValues);
                    entry.State = EntityState.Unchanged;
                }

                foreach (var entry in changedEntries.Where(x => x.State == EntityState.Added))
                    entry.State = EntityState.Detached;
                
                foreach (var entry in changedEntries.Where(x => x.State == EntityState.Deleted))
                    entry.State = EntityState.Unchanged;
                

                throw;
            }

        }
    }
}
