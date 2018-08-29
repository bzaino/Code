using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace Asa.Salt.Web.Services.Data.Caching.Infrastructure.SqlDependency
{
    public static class DbQueryExtension
    {
        public static ObjectQuery<T> ToObjectQuery<T>(this DbQuery<T> query)
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
                .Cast<ObjectQuery<T>>()
                .First();

            return objectQuery;
        }

        /// <summary>
        /// Converts the query to a Sql command.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public static SqlCommand ToSqlCommand<T>(this DbQuery<T> query)
        {
            var command = new SqlCommand { CommandText = query.ToString() };

            var objectQuery = query.ToObjectQuery();

            foreach (var param in objectQuery.Parameters)
            {
                command.Parameters.AddWithValue(param.Name, param.Value);
            }

            return command;
        }

        /// <summary>
        /// Converts the query to a trace string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public static string ToTraceString<T>(this DbQuery<T> query)
        {
            var objectQuery = query.ToObjectQuery();

            return objectQuery.ToTraceStringWithParameters();
        }

        /// <summary>
        /// Returns a trace string with parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public static string ToTraceStringWithParameters<T>(this ObjectQuery<T> query)
        {
            var traceString = query.ToTraceString() + "\n";

            return query.Parameters.Aggregate(traceString, (current, parameter) => current + (parameter.Name + " [" + parameter.ParameterType.FullName + "] = " + parameter.Value + "\n"));
        }
    }
}
