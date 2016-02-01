using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace QueryableDataBroker.TypeHandlers
{
    public class BooleanHandler<T> : IPropertyTypeHandler<T>
    {
        public IEnumerable<Type> SupportedTypes
        {
            get { return new Type[] { typeof(bool) }; }
        }

        public IQueryable<T> IsBool(IQueryable<T> source, PropertyInfo property, bool? value)
        {
            if (value != null)
            {
                bool isTrue = (bool)value;

                var itemX = Expression.Parameter(typeof(T), "x");
                var prop = Expression.Property(itemX, property);

                var boolExp = isTrue ? 
                    Expression.IsTrue(prop) : 
                    Expression.IsFalse(prop);

                var lambda = ExpressionHelper.Lambda<T, bool>(boolExp, itemX);
                source = source.Where(lambda);
            }
            return source;
        }

        public IQueryable<T> ApplyQuery(IQueryable<T> source, PropertyInfo property, PropertyQuery query)
        {
            return this.IsBool(source, property, query.IsBool);
        }
    }
}