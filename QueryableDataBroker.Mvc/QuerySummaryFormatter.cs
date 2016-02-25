using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryableDataBroker.Mvc
{
    public class QuerySummaryFormatter : OutputFormatter
    {
        public QuerySummaryFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv"));
            SupportedEncodings.Add(Encoding.UTF8);
        }

        public override bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            return context.ObjectType == typeof(QuerySummary);
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            var model = (QuerySummary)context.Object;
            var response = context.HttpContext.Response;

            using (var csv = new StringWriter())
            {
                foreach (string id in model.IDs)
                {
                    csv.WriteLine(id);
                }
                response.WriteAsync(csv.ToString());
            }

            return Task.FromResult(response);
        }

        public override void WriteResponseHeaders(OutputFormatterWriteContext context)
        {
            var model = (QuerySummary)context.Object;
            var headers = context.HttpContext.Response.Headers;

            string paginationString = String.Join(",", 
                model.Page.ToString(), 
                model.PageSize.ToString(), 
                model.Total.ToString());

            headers.Add("X-Pagination", paginationString);
        }
    }
}