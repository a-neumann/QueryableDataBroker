using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryableDataBroker.TypeHandlers
{
    public class StringHandler<T> : IPropertyTypeHandler<T>
    {
        private const string _Contains = "Contains";
        private const string _IndexOf = "IndexOf";
        private const string _StartsWith = "StartsWith";
        private const string _EndsWith = "EndsWith";
        private const string _Equals = "Equals";

        public IEnumerable<Type> SupportedTypes
        {
            get { return new Type[] { typeof(string) }; }
        }

        public IQueryable<T> Contains(IQueryable<T> source, PropertyInfo property, string value, bool caseMod = false)
        {
            if (String.IsNullOrEmpty(value))
            {
                return source;
            }

            var itemX = Expression.Parameter(typeof(T), "x");
            var prop = Expression.Property(itemX, property);

            Expression exp = null;

            if (caseMod)
            {
                exp = ExpressionHelper.CallMethod(prop, _Contains, value);
            }
            else
            {
                var idxOf = ExpressionHelper.CallMethod(prop, _IndexOf, value, StringComparison.CurrentCultureIgnoreCase);
                exp = Expression.NotEqual(idxOf, Expression.Constant(-1));
            }

            var lamda = ExpressionHelper.Lambda<T, bool>(exp, itemX);
            return source.Where(lamda);
        }

        public IQueryable<T> StartsEndsWith(IQueryable<T> source, PropertyInfo property, string start, string end, bool caseMod = false)
        {
            var comp = caseMod ? 
                StringComparison.CurrentCulture : 
                StringComparison.CurrentCultureIgnoreCase;

            if (!String.IsNullOrEmpty(end))
            {
                var itemX = Expression.Parameter(typeof(T), "x");
                var prop = Expression.Property(itemX, property);

                var endsWith = ExpressionHelper.CallMethod(prop, _EndsWith, end, comp);

                var lambda = ExpressionHelper.Lambda<T, bool>(endsWith, itemX);
                source = source.Where(lambda);
            }

            if (!String.IsNullOrEmpty(start))
            {
                var itemX = Expression.Parameter(typeof(T), "x");
                var prop = Expression.Property(itemX, property);

                var stratsWith = ExpressionHelper.CallMethod(prop, _StartsWith, start, comp);

                var lambda = ExpressionHelper.Lambda<T, bool>(stratsWith, itemX);
                source = source.Where(lambda);
            }

            return source;
        }

        public IQueryable<T> IsEqual(IQueryable<T> source, PropertyInfo property, IEnumerable<string> values, bool caseMod = false)
        {
            if (values != null)
            {
                var comp = caseMod ?
                    StringComparison.CurrentCulture :
                    StringComparison.CurrentCultureIgnoreCase;

                var itemX = Expression.Parameter(typeof(T), "x");
                var prop = Expression.Property(itemX, property);

                var valuesArr = values.ToArray();
                Expression exp = ExpressionHelper.CallMethod(prop, _Equals, valuesArr[0], comp);

                for (int i = 1; i < valuesArr.Length; i++)
                {
                    exp = Expression.OrElse(exp, ExpressionHelper.CallMethod(prop, _Equals, valuesArr[i], comp));
                }

                var lambda = ExpressionHelper.Lambda<T, bool>(exp, itemX);
                source = source.Where(lambda);
            }

            return source;
        }

        public IQueryable<T> ApplyQuery(IQueryable<T> source, PropertyInfo property, PropertyQuery query)
        {
            source = this.Contains(source, property, query.Contains, query.ContainsCaseMod);
            source = this.StartsEndsWith(source, property, query.RangeStart, query.RangeEnd, query.RangeCaseMod);
            source = this.IsEqual(source, property, query.IsEqual, query.IsEqualCaseMod);

            return source;
        }
    }
}