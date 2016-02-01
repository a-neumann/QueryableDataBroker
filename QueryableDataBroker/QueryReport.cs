using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QueryableDataBroker
{
    public class QueryReport
    {
        public QueryReport(IEnumerable<string> ids, int skipped, long total)
        {
            this.Initialize(ids, skipped, total, DateTime.Now, DateTime.Now);
        }

        public QueryReport(IEnumerable<string> ids, int skipped, long total, DateTime started)
        {
            this.Initialize(ids, skipped, total, started, DateTime.Now);
        }

        private void Initialize(IEnumerable<string> ids, int skipped, long total, DateTime started, DateTime finished)
        {
            this.IDs = ids;
            this.ResultsCount = ids.Count();
            this.Skipped = skipped;
            this.Total = total;
            if (started < finished) {
                this.Duration = Convert.ToInt32((finished - started).TotalMilliseconds);
            }
        }

        public IEnumerable<string> IDs { get; set; }

        public int ResultsCount { get; set; }
        public int Skipped { get; set; }
        public long Total { get; set; }

        public int Duration { get; set; }
    }
}