using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NerdDinner.Web.Common
{
    /// <summary>
    /// Static Utility Extensions class
    /// </summary>
    public static class UtilityExtensions
    {
        public static bool Contains(this string source, string target, StringComparison comp)
        {
            return source.IndexOf(target, comp) >= 0;
        }

        /// <summary>
        /// Ensures that an argument is not null
        /// </summary>
        /// <typeparam name="T">type of the argument</typeparam>
        /// <param name="argument">value of the argument</param>
        /// <param name="argumentName">name of the argument</param>
        public static void EnsureArgumentNotNull<T>(this T argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        /// <summary>
        /// Took from: http://www.codewrecks.com/blog/index.php/2012/03/23/order-by-a-property-expressed-as-string-in-a-linq-query/ 
        /// </summary>
        /// <typeparam name="T">type of the queryable entity</typeparam>
        /// <param name="query">The queryable entity</param>
        /// <param name="propertyName">sort property name</param>
        /// <param name="descending">sort order</param>
        /// <returns></returns>
        public static IQueryable<T> OrderByPropertyName<T>(this IQueryable<T> query, string propertyName, bool descending = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }

            Type type = typeof(T);
            ParameterExpression arg = Expression.Parameter(type, "x");

            PropertyInfo pi = type.GetProperty(
                propertyName, 
                BindingFlags.IgnoreCase |
                BindingFlags.Public |
                BindingFlags.Instance);

            if (pi == null)
            {
                throw new ArgumentException(Resources.PropertyNotFoundOnObject, "propertyName");
            }

            Expression expr = Expression.Property(arg, pi);
            type = pi.PropertyType;

            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

            String methodName = descending ? "OrderByDescending" : "OrderBy";
            object result = typeof(Queryable).GetMethods().Single(
                method => method.Name == methodName
                        && method.IsGenericMethodDefinition
                        && method.GetGenericArguments().Length == 2
                        && method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), type)
                .Invoke(null, new object[] { query, lambda });
            return (IQueryable<T>)result;
        }
    }
}
