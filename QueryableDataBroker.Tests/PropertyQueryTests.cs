using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace QueryableDataBroker.Tests
{
    public class PropertyQueryTests
    {
        [Fact]
        public void CanParseEqual()
        {
            string value = "foobar";
            var q = PropertyQuery.Create("whatever", value);

            Assert.Contains(value, q.IsEqual);
        }

        [Fact]
        public void CanParseEqualMultiple()
        {
            string valueA = "foo";
            string valueB = "bar";
            string values = valueA + "," + valueB;

            var q = PropertyQuery.Create("whatever", values);

            Assert.Contains(valueA, q.IsEqual);
            Assert.Contains(valueB, q.IsEqual);
        }

        [Fact]
        public void CanParseEqualCaseMod()
        {
            var q = PropertyQuery.Create("whatever", "foobar");

            Assert.False(q.IsEqualCaseMod);

            var q2 = PropertyQuery.Create("whatever", "foo,bar" + PropertyQuery.CaseMod);

            Assert.True(q2.IsEqualCaseMod);
        }

        [Fact]
        public void CanParseBool()
        {
            var q1 = PropertyQuery.Create("whatever", "false");

            Assert.NotNull(q1.IsBool);
            Assert.False(q1.IsBool);

            var q2 = PropertyQuery.Create("whatever", "true");

            Assert.True(q2.IsBool);
        }

        [Fact]
        public void CanParseContains()
        {
            var q = PropertyQuery.Create("whatever", "*foo*");
            Assert.Equal("foo", q.Contains);
            Assert.False(q.ContainsCaseMod);
        }

        [Fact]
        public void CanParseContainsCaseMod()
        {
            var q = PropertyQuery.Create("whatever", "*foo*" + PropertyQuery.CaseMod);
            Assert.Equal("foo", q.Contains);
            Assert.True(q.ContainsCaseMod);
        }

        [Fact]
        public void CanParseRangeStartEnd()
        {
            var q1 = PropertyQuery.Create("whatever", "100*");
            Assert.Equal("100", q1.RangeStart);
            Assert.True(String.IsNullOrEmpty(q1.RangeEnd));

            var q2 = PropertyQuery.Create("whatever", "*200");
            Assert.True(String.IsNullOrEmpty(q2.RangeStart));
            Assert.Equal("200", q2.RangeEnd);

            var q3 = PropertyQuery.Create("whatever", "100*200");
            Assert.Equal("100", q3.RangeStart);
            Assert.Equal("200", q3.RangeEnd);

            Assert.False(q1.RangeCaseMod || q2.RangeCaseMod || q3.RangeCaseMod);
        }

        [Fact]
        public void CanParseRangeStartEndCaseMod()
        {
            var q = PropertyQuery.Create("whatever", "foo*bar" + PropertyQuery.CaseMod);
            Assert.Equal("foo", q.RangeStart);
            Assert.Equal("bar", q.RangeEnd);
            Assert.True(q.RangeCaseMod);
        }
    }
}
