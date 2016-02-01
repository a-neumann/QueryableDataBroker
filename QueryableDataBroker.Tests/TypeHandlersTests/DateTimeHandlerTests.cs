using QueryableDataBroker.TypeHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace QueryableDataBroker.Tests
{
    public class DateTimeHandlerTests
    {
        private IEnumerable<Unicorn> ApplQueryToHandler(string queryValue)
        {
            var handler = new DateTimeHandler<Unicorn>();
            var prop = Unicorns.GetProperty(u => u.BirthDate);

            var q = PropertyQuery.Create(prop.Name, queryValue);

            return handler.ApplyQuery(Unicorns.All, prop, q);
        }

        [Fact]
        public void CanApplyRange_From()
        {
            var born2016 = Unicorns.GetAtLeast(u => u.BirthDate >= new DateTime(2016, 01, 01), 2);
            var results = this.ApplQueryToHandler("2016-01-01*");

            Assert.True(results.SequenceEqual(born2016), "Cannot get results with range from x to *");
        }

        [Fact]
        public void CanApplyRange_To()
        {
            var born2015 = Unicorns.GetAtLeast(u => u.BirthDate <= new DateTime(2015, 12, 31), 2);
            var results = this.ApplQueryToHandler("*2015-12-31");

            Assert.True(results.SequenceEqual(born2015), "Cannot get results with range from * to x");
        }

        [Fact]
        public void CanApplyRange_FromTo()
        {
            var bornInDateRange = Unicorns.GetAtLeast(u =>
                u.BirthDate >= new DateTime(2015, 12, 15) &&
                u.BirthDate <= new DateTime(2016, 1, 15)
                );
            var results = this.ApplQueryToHandler("2015-12-15*2016-1-15");

            Assert.True(results.SequenceEqual(bornInDateRange), "Cannot get results with range from x to x");
        }

        [Fact]
        public void CanApplyIsEqual()
        {
            var born1st1st = Unicorns.GetAtLeast(u => u.BirthDate == new DateTime(2016, 01, 01));
            var results = this.ApplQueryToHandler("2016-01-01");

            Assert.True(results.SequenceEqual(born1st1st));
        }
    }
}