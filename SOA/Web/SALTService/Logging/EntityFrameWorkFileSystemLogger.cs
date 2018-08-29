using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Asa.Salt.Web.Services.Logging
{
    public static class EntityFrameworkExtensions
    {

        /// <summary>
        /// Returns the trace string with parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public static string ToTraceStringWithParameters<T>(this ObjectQuery<T> query)
        {
            string traceString = query.ToTraceString() + "\n";

            return query.Parameters.Aggregate(traceString, (current, parameter) => current + (parameter.Name + " [" + parameter.ParameterType.FullName + "] = " + parameter.Value + "\n"));
        }

        /// <summary>
        /// To the object query.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public static ObjectQuery<TEntity> ToObjectQuery<TEntity>(this DbQuery<TEntity> query)
        {
            var internalQuery = query.GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.Name == "_internalQuery")
                .Select(field => field.GetValue(query))
                .First();

            var objectQuery = internalQuery.GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.Name == "_objectQuery")
                .Select(field => field.GetValue(internalQuery))
                .Cast<ObjectQuery<TEntity>>()
                .First();

            return objectQuery;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class EntityFrameWorkFileSystemLogger : IQueryLogger
    {

        /// <summary>
        /// To the trace string.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="context">The context.</param>
        public void LogSql<TEntity>(Expression<Func<TEntity, bool>> query, DbContext context)
            where TEntity : class
        {
            var dbQuery = context.Set<TEntity>().Where(query) as DbQuery<TEntity>;
            var oquery = dbQuery.ToObjectQuery();
            System.Diagnostics.Debug.WriteLine(string.Format("Time: {0} Query: {1}",
                                                             DateTime.Now.ToShortDateString(), oquery.ToTraceStringWithParameters()));

        }






        public void LogSql<TEntity>(EntityCommand query, DbContext context) where TEntity : class
        {
            throw new NotImplementedException();
        }
    }
}


