using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QueryableDataBroker
{
    public class QuerySummary
    {
        public QuerySummary(IEnumerable<string> ids, int page, int pageSize, long total)
        {
            this.IDs = ids;
            this.Count = ids.Count();
            this.Page = page;
            this.PageSize = pageSize;
            this.Total = total;
        }

        public IEnumerable<string> IDs { get; set; }

        public int Count { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public long Total { get; set; }

        //public int Duration { get; set; }
    }
}