using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace QueryableDataBroker.TypeHandlers
{
    public interface IPropertyTypeHandler<T>
    {
        IEnumerable<Type> SupportedTypes { get; }
        IQueryable<T> ApplyQuery(IQueryable<T> source, PropertyInfo property, PropertyQuery query);
    }
}