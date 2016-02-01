using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq.Expressions;
using QueryableDataBroker.TypeHandlers;

namespace QueryableDataBroker
{
    public interface IQueryableDataBroker<T>
    {
        IQueryable<T> GetByIds(IEnumerable<string> ids);
        IQueryable<T> Find(IEnumerable<PropertyQuery> queries);
        QueryReport Query(IEnumerable<PropertyQuery> queries, int skip = 0, int get = 0);
    }

    public class QueryBroker<T, K> : IQueryableDataBroker<T> where K : IEquatable<K>
    {
        public QueryBroker(IEnumerable<T> source, Expression<Func<T, K>> keySelector, QueryBrokerOptions<T> options = null)
        {
            this.Initialize(source.AsQueryable(), keySelector, options);
        }

        public QueryBroker(IQueryable<T> source, Expression<Func<T, K>> keySelector, QueryBrokerOptions<T> options = null)
        {
            this.Initialize(source, keySelector, options);
        }

        private void Initialize(IQueryable<T> source, Expression<Func<T, K>> keySelector, QueryBrokerOptions<T> options)
        {
            this.KeySelector = keySelector;
            this.Source = source;
            this.PropertyTypeHandlers = KnownHandlers;
            this.PropertyAccessRules = options != null ? 
                options.PropertyAccessRules : 
                new Dictionary<string, Func<bool>>();
        }

        private readonly static IList<IPropertyTypeHandler<T>> KnownHandlers = new List<IPropertyTypeHandler<T>>() {
            new BooleanHandler<T>(),
            new DateTimeHandler<T>(),
            new NumericHandler<T>(),
            new StringHandler<T>()
        };

        public IQueryable<T> Source { get; private set; }
        public IList<IPropertyTypeHandler<T>> PropertyTypeHandlers { get; private set; }
        public int GetMax { get; set; }

        public IEnumerable<PropertyInfo> Properties
        {
            get
            {
                var props = typeof(T).GetRuntimeProperties();
                return props.Where(p => p.CanRead);
            }
        }

        private IDictionary<string, Func<bool>> PropertyAccessRules { get; set; }

        private Expression<Func<T, K>> KeySelector { get; set; }

        private Type EntityType
        {
            get
            {
                return typeof(T);
            }
        }

        private Type KeyType
        {
            get
            {
                return typeof(K);
            }
        }

        private object ConvertToKey(string id)
        {
            var targetType = this.KeyType;

            if (targetType == typeof(Guid))
            {
                return Guid.Parse(id);
            }

            return Convert.ChangeType(id, this.KeyType);
        }

        public IQueryable<T> GetByIds(IEnumerable<string> ids)
        {
            List<K> keys = ids.Select(i => (K)this.ConvertToKey(i)).ToList();
            var keysConst = Expression.Constant(keys);

            var paramExp = this.KeySelector.Parameters[0];
            var propEx = (MemberExpression)this.KeySelector.Body;
            var idsContain = Expression.Call(keysConst, "Contains", Type.EmptyTypes, propEx);

            var lambda = ExpressionHelper.Lambda<T, bool>(idsContain, paramExp);

            return this.Source.Where(lambda);
        }

        public IQueryable<T> Find(IEnumerable<PropertyQuery> queries)
        {
            var filtered = this.Source;

            foreach(var q in queries)
            {
                var prop = this.Properties.FirstOrDefault(p => p.Name.Equals(q.Property, StringComparison.CurrentCultureIgnoreCase));
                if (prop != null)
                {
                    Func<bool> accessRule;
                    if (this.PropertyAccessRules.TryGetValue(prop.Name, out accessRule) && !accessRule())
                    {
                        throw new MemberAccessException("Access denied for property " + prop.Name);
                    }

                    var handler = this.PropertyTypeHandlers.FirstOrDefault(h => h.SupportedTypes.Contains(prop.PropertyType));
                    if (handler != null)
                    {
                        filtered = handler.ApplyQuery(filtered, prop, q);
                    }
                }
            }

            return filtered;
        }

        public QueryReport Query(IEnumerable<PropertyQuery> queries, int skip = 0, int get = 0)
        {
            var start = DateTime.Now;

            var filtered = this.Find(queries).Skip(skip);

            get = get > 0 ? get : this.GetMax;
            if (get > 0)
            {
                filtered = filtered.Take(get);
            }

            var paramExp = this.KeySelector.Parameters[0];
            var keyPropEx = (MemberExpression)this.KeySelector.Body;
            var keyToString = ExpressionHelper.CallMethod(keyPropEx, "ToString");
            var keysToString = ExpressionHelper.Lambda<T, string>(keyToString, paramExp);

            var keys = filtered.Select(keysToString.Compile()).ToList<string>();
            long total = this.Source.LongCount();

            var res = new QueryReport(keys, skip, total, start);

            return res;
        }
    }
}
