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

            var results = broker.Find(queries);

            var shouldFind = Unicorns.GetAtLeast(u => 
                u.Name.StartsWith("da", StringComparison.CurrentCultureIgnoreCase) && 
                u.BirthDate >= new DateTime(2016, 1, 1)
                );

            Assert.True(results.SequenceEqual(shouldFind));
        }

        [Fact]
        public void CanCreateReport()
        {
            var broker = new QueryBroker<Unicorn, Guid>(Unicorns.All, u => u.Id);

            int skip = 2;
            int max = 3;

            var report = broker.Query(new PropertyQuery[] { }, skip, max);

            Assert.Equal(skip, report.Skipped);
            Assert.NotEmpty(report.IDs);
            Assert.InRange(report.IDs.Count(), 1, max);
            Assert.Equal(Unicorns.All.LongCount(), report.Total);
        }
    }
}
