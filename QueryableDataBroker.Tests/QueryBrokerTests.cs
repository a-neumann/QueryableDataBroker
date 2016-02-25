using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using QueryableDataBroker;

namespace QueryableDataBroker.Tests
{
    public class QueryBrokerTests
    {
        [Fact]
        public void CanGetByIds_Guid()
        {
            var broker = new QueryBroker<Unicorn, Guid>(Unicorns.All, u => u.Id);

            string fredsId = Unicorns.Fred.Id.ToString();
            string davesId = Unicorns.Dave.Id.ToString();

            var results = broker.GetByIds(new[] { fredsId, davesId });

            Assert.Contains(Unicorns.Fred, results);
            Assert.Contains(Unicorns.Dave, results);
            Assert.DoesNotContain(Unicorns.Daria, results);
        }

        [Fact]
        public void CanGetByIds_Int()
        {
            var broker = new QueryBroker<Pony, int>(Ponies.All, p => p.Id);

            string ronsId = Ponies.Ron.Id.ToString();
            string darcysId = Ponies.Darcy.Id.ToString();

            var results = broker.GetByIds(new[] { ronsId, darcysId });

            Assert.Contains(Ponies.Ron, results);
            Assert.Contains(Ponies.Darcy, results);
            Assert.DoesNotContain(Ponies.Hank, results);
        }

        [Fact]
        public void CanApplyQueries()
        {
            var broker = new QueryBroker<Unicorn, Guid>(Unicorns.All, u => u.Id);

            var queries = new [] {
                PropertyQuery.Create("name", "da*"),
                PropertyQuery.Create("birthdate", "2016-01-01*"),
            };

            var results = broker.GetItems(queries);

            var shouldFind = Unicorns.GetAtLeast(u => 
                u.Name.StartsWith("da", StringComparison.CurrentCultureIgnoreCase) && 
                u.BirthDate >= new DateTime(2016, 1, 1)
                );

            Assert.True(results.SequenceEqual(shouldFind));
        }

        [Fact]
        public void CanCreateSummary()
        {
            var broker = new QueryBroker<Unicorn, Guid>(Unicorns.All, u => u.Id);

            var summary = broker.Query(new PropertyQuery[] { });

            Assert.Equal(1, summary.Page);
            Assert.NotEmpty(summary.IDs);
            Assert.InRange(summary.IDs.Count(), 1, Unicorns.All.Count());
            Assert.Equal(Unicorns.All.LongCount(), summary.Total);
        }
    }
}
