using QueryableDataBroker.TypeHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace QueryableDataBroker.Tests
{
    public class BooleanHandlerTests
    {
        private IEnumerable<Unicorn> ApplQueryToHandler(string queryValue)
        {
            var handler = new BooleanHandler<Unicorn>();
            var prop = Unicorns.GetProperty(u => u.HasWings);

            var q = PropertyQuery.Create(prop.Name, queryValue);

            return handler.ApplyQuery(Unicorns.All, prop, q);
        }

        [Fact]
        public void CanApplyQuery()
        {
            var withWings = Unicorns.GetAtLeast(u => u.HasWings, 2);
            var results1 = this.ApplQueryToHandler("true");

            Assert.True(results1.SequenceEqual(withWings));

            var wingless = Unicorns.GetAtLeast(u => !u.HasWings, 2);
            var results2 = this.ApplQueryToHandler("false");

            Assert.True(results2.SequenceEqual(wingless));
        }
    }
}
