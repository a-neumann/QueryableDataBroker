using QueryableDataBroker.TypeHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace QueryableDataBroker.Tests
{
    public class NumericHandlerTests
    {
        private IEnumerable<Unicorn> ApplQueryToHandler(string queryValue)
        {
            var handler = new NumericHandler<Unicorn>();
            var prop = Unicorns.GetProperty(u => u.HornLenght);

            var q = PropertyQuery.Create(prop.Name, queryValue);

            return handler.ApplyQuery(Unicorns.All, prop, q);
        }

        [Fact]
        public void CanApplyRange_From()
        {
            var gt115 = Unicorns.GetAtLeast(u => u.HornLenght >= 115);
            var results = this.ApplQueryToHandler("115*");

            Assert.True(results.SequenceEqual(gt115), "Cannot get results with range from x to *");
       }

        [Fact]
        public void CanApplyRange_To()
        {
            var lt120 = Unicorns.GetAtLeast(u => u.HornLenght <= 120);
            var results = this.ApplQueryToHandler("*120");

            Assert.True(results.SequenceEqual(lt120), "Cannot get results with range from * to x");
        }

        [Fact]
        public void CanApplyRange_FromTo()
        {
            var hornLengthRange = Unicorns.GetAtLeast(u =>
                u.HornLenght >= 115 &&
                u.HornLenght <= 120
                );
            var results = this.ApplQueryToHandler("115*120");

            Assert.True(results.SequenceEqual(hornLengthRange), "Cannot get results with range from * to *");
        }

        [Fact]
        public void CanApplyIsEqual()
        {
            var length118 = Unicorns.GetAtLeast(u => u.HornLenght == 128);
            var results = this.ApplQueryToHandler("128");

            Assert.True(results.SequenceEqual(length118));
        }
    }
}
