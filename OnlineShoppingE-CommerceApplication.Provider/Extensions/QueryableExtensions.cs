using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, OrderBy orderBy)
        {
            var expression = source.Expression;

            var parameter = Expression.Parameter(typeof(T), "x");
            var selector = Expression.PropertyOrField(parameter, orderBy.PropertyName);
            var method = string.Equals(orderBy.Order.ToString(), "desc", StringComparison.OrdinalIgnoreCase) ? "OrderByDescending" : "OrderBy";

            expression = Expression.Call(typeof(Queryable), method,
                new Type[] { source.ElementType, selector.Type },
                expression, Expression.Quote(Expression.Lambda(selector, parameter)));

            return source.Provider.CreateQuery<T>(expression);
        }
    }
    //public static class QueryableExtensions
    //{
    //    /// <summary>
    //    /// Build dynamic Query with order by.
    //    /// </summary>
    //    /// <typeparam name="TSource"></typeparam>
    //    /// <param name="query"></param>
    //    /// <param name="propertyName"></param>
    //    /// <param name="orderBy">asc or desc</param>
    //    /// <returns></returns>
    //    public static IOrderedQueryable<TSource> OrderBy<TSource>(
    //                 this IQueryable<TSource> query, string propertyName, bool orderBy)
    //    {
    //        var entityType = typeof(TSource);             //Create x=>x.PropName
    //        var propertyInfo = entityType.GetProperty(propertyName);
    //        ParameterExpression arg = Expression.Parameter(entityType, "x");
    //        MemberExpression property = Expression.Property(arg, propertyName);
    //        var selector = Expression.Lambda(property, new ParameterExpression[] { arg });             //Get System.Linq.Queryable.OrderBy() method.
    //        var enumarableType = typeof(Queryable);
    //        var method = enumarableType.GetMethods()
    //        .Where(m => m.Name == GetOrderByMethodName(orderBy) && m.IsGenericMethodDefinition)
    //        .Where(m =>
    //        {
    //            var parameters = m.GetParameters().ToList();
    //            //Put more restriction here to ensure selecting the right overload                
    //            return parameters.Count == 2;//overload that has 2 parameters
    //        }).Single();
    //        //The linq's OrderBy<TSource, TKey> has two generic types, which provided here
    //        MethodInfo genericMethod = method
    //                    .MakeGenericMethod(entityType,
    //                propertyInfo.PropertyType);             /*Call query.OrderBy(selector), with query and selector: x=> x.PropName
    //          Note that we pass the selector as Expression to the method and we don't compile it.
    //          By doing so EF can extract "order by" columns and generate SQL for it.*/
    //        var newQuery = (IOrderedQueryable<TSource>)genericMethod
    //            .Invoke(genericMethod, new object[] { query, selector });
    //        return newQuery;
    //    }
    //    private static string GetOrderByMethodName(bool direction)
    //    {
    //        string orderBy = direction ? "asc" : "desc";
    //        if (orderBy.ToLower().Contains("desc"))
    //            return "OrderByDescending";
    //        else
    //            return "OrderBy";
    //    }
    //}
}
