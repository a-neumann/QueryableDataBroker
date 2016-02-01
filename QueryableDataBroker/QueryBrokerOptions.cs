using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Reflection;

namespace QueryableDataBroker
{
    public class QueryBrokerOptions<T>
    {
        public QueryBrokerOptions()
        {
            this.PropertyAccessRules = new Dictionary<string, Func<bool>>();
        }

        public QueryBrokerOptions<T> AllowPropertyAccess<P>(Expression<Func<T, P>> property, Func<bool> hasAccess)
        {
            var propName = ExpressionHelper.GetPropertyFromExpression(property).Name;

            if (hasAccess != null)
            {
                this.PropertyAccessRules[propName] = hasAccess;
            }

            return this;
        }

        public QueryBrokerOptions<T> AllowPropertyAccess<P>(Expression<Func<T, P>> property, bool hasAccess)
        {
            var propName = ExpressionHelper.GetPropertyFromExpression(property).Name;

            if (!hasAccess)
            {
                this.PropertyAccessRules[propName] = new Func<bool>(() => false);
            }
            else if (this.PropertyAccessRules.ContainsKey(propName))
            {
                this.PropertyAccessRules.Remove(propName);
            }

            return this;
        }

        public IDictionary<string, Func<bool>> PropertyAccessRules { get; private set; }
    }
}
