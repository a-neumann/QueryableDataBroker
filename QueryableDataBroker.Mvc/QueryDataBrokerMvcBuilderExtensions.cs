using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QueryableDataBroker.Mvc
{
    public static class QueryDataBrokerMvcBuilderExtensions
    {
        public static IMvcBuilder AddQueryableDataBroker(this IMvcBuilder builder)
        {
            builder.AddMvcOptions(o => o.OutputFormatters.Add(new QuerySummaryFormatter()));
            return builder;
        }
    }
}
