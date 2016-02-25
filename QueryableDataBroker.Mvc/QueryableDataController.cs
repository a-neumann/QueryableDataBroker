using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace QueryableDataBroker.Mvc
{
    public abstract class QueryableDataController<T, K> : Controller where K : IEquatable<K>
    {
        public QueryBroker<T, K> QueryBroker { get; private set; }

		/// <summary>
		/// Restrict the count of returned objects, if no url query was passed
		/// </summary>
		public int MaxUnfilteredResults { get; set; }

		public QueryableDataController(IQueryable<T> items, Expression<Func<T, K>> keySelector)
        {
            var options = this.Configure(new QueryBrokerOptions<T>());

            this.QueryBroker = new QueryBroker<T, K>(items, keySelector, options);
        }

        /// <summary>
        /// If there is '$keyName' in the query, take '$keyName' instead of 'keyName'.
        /// </summary>
        private string GetUrlQueryValueSafe(IReadableStringCollection urlQuery, string keyName)
        {
            string result =
                urlQuery.ContainsKey("$" + keyName) ?
                urlQuery["$" + keyName].Last() :
                urlQuery.ContainsKey(keyName) ?
                urlQuery[keyName].Last() :
                null;

            return result;
        }

        [NonAction]
        public virtual QueryBrokerOptions<T> Configure(QueryBrokerOptions<T> options)
        {
            return options;
        }

        [NonAction]
        public IEnumerable<PropertyQuery> ParseQueries(IReadableStringCollection urlQuery, out int page, out int pageSize)
        {
            string _page = this.GetUrlQueryValueSafe(urlQuery, "page");
            page = Int32.TryParse(_page, out page) ? page : 1;

            string _pageSize = this.GetUrlQueryValueSafe(urlQuery, "pageSize");
            pageSize = Int32.TryParse(_pageSize, out pageSize) ? pageSize : 0;

            var queries = new List<PropertyQuery>();

            foreach (var kv in urlQuery)
            {
                string propName = kv.Key;
                string value = kv.Value.LastOrDefault();

                if (!String.IsNullOrEmpty(propName) && !propName.StartsWith("$") && !String.IsNullOrEmpty(value))
                {
                    queries.Add(PropertyQuery.Create(propName, value));
                }
            }

            return queries;
        }

        [HttpGet("{idList}")]
        public IEnumerable<T> Get(string idList)
        {
            if (String.IsNullOrWhiteSpace(idList))
            {
                if (String.IsNullOrWhiteSpace(this.Request.QueryString.Value))
				{
					var take = this.MaxUnfilteredResults;
					if(take <= -1)
					{
						return this.QueryBroker.Source
							.ToList();
					}

					return this.QueryBroker.Source
						.Take(this.MaxUnfilteredResults)
						.ToList();
				}

				int page, pageSize;
				var queries = this.ParseQueries(this.Request.Query, out page, out pageSize);

				return this.QueryBroker.GetItems(queries);
			}

            var ids = idList.Split(',');

            try
            {
                var results = this.QueryBroker.GetByIds(ids);
                return results.ToList();
            }
            catch(FormatException ex)
            {
				this.HttpBadRequest(ex);
				return null;
			}
        }

        [HttpGet("[action]")]
        public QuerySummary Query()
        {
            int page, pageSize;
            var queries = this.ParseQueries(this.Request.Query, out page, out pageSize);

            var summary = this.QueryBroker.Query(queries, page, pageSize);

            return summary;
        }
    }
}