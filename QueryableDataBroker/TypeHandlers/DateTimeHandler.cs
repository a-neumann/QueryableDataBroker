using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace QueryableDataBroker.TypeHandlers
{
    public class DateTimeHandler<T> : IPropertyTypeHandler<T>
    {
        public IEnumerable<Type> SupportedTypes
        {
            get { return new Type[] { typeof(DateTime) }; }
        }

        public IQueryable<T> Range(IQueryable<T> source, PropertyInfo property, string start, string end)
        {
            if (!String.IsNullOrWhiteSpace(start))
            {
                var val = DateTime.Parse(start);
                var lambda = ExpressionHelper.Compare<T>(property, val, ComparisonMode.GreaterThanEqual);
                source = source.Where(lambda);
            }

            if (!String.IsNullOrWhiteSpace(end))
            {
                var val = DateTime.Parse(end);
                var lambda = ExpressionHelper.Compare<T>(property, val, ComparisonMode.LessThanEqual);
                source = source.Where(lambda);
            }
            return source;
        }

        public IQueryable<T> IsEqual(IQueryable<T> source, PropertyInfo property, IEnumerable<string> values)
        {
            if (values != null)
            {
                var itemX = Expression.Parameter(typeof(T), "x");
                var prop = Expression.Property(itemX, property);
                Expression exp = prop;

                bool first = true;

                foreach (var val in values)
                {
                    var date = DateTime.Parse(val);
                    var dateConst = Expression.Constant(date, typeof(DateTime));

                    if (first)
                    {
                        exp = Expression.Equal(exp, dateConst);
                    }
                    else
                    {
                        var eq = Expression.Equal(prop, dateConst);
                        exp = Expression.OrElse(exp, eq);
                    }

                    first = false;
                }

                var lambda = ExpressionHelper.Lambda<T, bool>(exp, itemX);
                source = source.Where(lambda);
            }
            return source;
        }

        public IQueryable<T> ApplyQuery(IQueryable<T> source, PropertyInfo property, PropertyQuery query)
        {
            source = this.Range(source, property, query.RangeStart, query.RangeEnd);
            source = this.IsEqual(source, property, query.IsEqual);
            return source;
        }
    }
}
