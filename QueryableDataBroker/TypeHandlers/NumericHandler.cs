using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace QueryableDataBroker.TypeHandlers
{
    public class NumericHandler<T> : IPropertyTypeHandler<T>
    {
        public IEnumerable<Type> SupportedTypes
        {
            get { return new Type[] {
                typeof(int),
                typeof(long),
                typeof(decimal),
                typeof(double),
                typeof(float),
                typeof(short),
                typeof(byte)
            }; }
        }

        public IQueryable<T> Range(IQueryable<T> source, PropertyInfo property, string start, string end)
        {
            var type = property.PropertyType;

            if (!String.IsNullOrWhiteSpace(start))
            {
                var min = Convert.ChangeType(start, type);
                var lambda = ExpressionHelper.Compare<T>(property, min, ComparisonMode.GreaterThanEqual);
                source = source.Where(lambda);
            }

            if (!String.IsNullOrWhiteSpace(end))
            {
                var max = Convert.ChangeType(end, type);
                var lambda = ExpressionHelper.Compare<T>(property, max, ComparisonMode.LessThanEqual);
                source = source.Where(lambda);
            }
            return source;
        }

        public IQueryable<T> IsEqual(IQueryable<T> source, PropertyInfo property, IEnumerable<string> values)
        {
            var type = property.PropertyType;

            if (values != null)
            {
                var lambda = ExpressionHelper.IsEqualToAny<T>(property, type, values.ToArray());
                source = source.Where(lambda);
            }
            return source;
        }

        public IQueryable<T> ApplyQuery(IQueryable<T> source, PropertyInfo property, PropertyQuery query)
        {
            source = this.IsEqual(source, property, query.IsEqual);
            source = this.Range(source, property, query.RangeStart, query.RangeEnd);

            return source;
        }
    }
}
