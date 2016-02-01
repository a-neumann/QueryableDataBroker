using QueryableDataBroker.TypeHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace QueryableDataBroker.Tests
{
    public class StringHandlerTests
    {
        private IEnumerable<Unicorn> ApplQueryToHandler(string queryValue)
        {
            var handler = new StringHandler<Unicorn>();
            var prop = Unicorns.GetProperty(u => u.Name);

            var q = PropertyQuery.Create(prop.Name, queryValue);

            return handler.ApplyQuery(Unicorns.All, prop, q);
        }

        [Fact]
        public void CanApplyContains()
        {
            var nameContains_EC = Unicorns.GetAtLeast(u => u.Name.IndexOf("EC", StringComparison.CurrentCultureIgnoreCase) != -1);
            var results = this.ApplQueryToHandler("*EC*");

            Assert.True(results.SequenceEqual(nameContains_EC));
        }

        [Fact]
        public void CanApplyContains_CaseMod()
        {
            var nameContains_da = Unicorns.All.Where(u => u.Name.Contains("da"));
            var nameContains_da_Results = this.ApplQueryToHandler("*da*" + PropertyQuery.CaseMod);

            Assert.Empty(nameContains_da);
            Assert.Empty(nameContains_da_Results);

            var nameContains_Da = Unicorns.GetAtLeast(u => u.Name.Contains("Da"));
            var nameContains_Da_Results = this.ApplQueryToHandler("*Da*" + PropertyQuery.CaseMod);

            Assert.True(nameContains_Da_Results.SequenceEqual(nameContains_Da));
        }

        [Fact]
        public void CanApplyStartsWith()
        {
            var startsWith_da = Unicorns.GetAtLeast(u => u.Name.StartsWith("da", StringComparison.CurrentCultureIgnoreCase));
            var results = this.ApplQueryToHandler("da*");

            Assert.True(results.SequenceEqual(startsWith_da), "Cannot get results starting with string.");
        }

        [Fact]
        public void CanApplyStartsWith_CaseMod()
        {
            var nameStartsWith_da = Unicorns.All.Where(u => u.Name.StartsWith("da"));
            var nameStartsWith_da_Results = this.ApplQueryToHandler("da*" + PropertyQuery.CaseMod);

            Assert.Empty(nameStartsWith_da);
            Assert.Empty(nameStartsWith_da_Results);

            var nameStartsWith_Da = Unicorns.GetAtLeast(u => u.Name.StartsWith("Da"));
            var nameStartsWith_Da_Results = this.ApplQueryToHandler("Da*" + PropertyQuery.CaseMod);

            Assert.True(nameStartsWith_Da_Results.SequenceEqual(nameStartsWith_Da), "Cannot get results starting with string.");
        }

        [Fact]
        public void CanApplyEndsWith()
        {
            var EndsWith_IA = Unicorns.GetAtLeast(u => u.Name.EndsWith("IA", StringComparison.CurrentCultureIgnoreCase));
            var results = this.ApplQueryToHandler("*IA");

            Assert.True(results.SequenceEqual(EndsWith_IA), "Cannot get results ending with string.");
        }

        [Fact]
        public void CanApplyEndsWith_CaseMod()
        {
            var nameEndsWith_iA = Unicorns.All.Where(u => u.Name.EndsWith("iA"));
            var nameEndsWith_iA_Results = this.ApplQueryToHandler("*iA" + PropertyQuery.CaseMod);

            Assert.Empty(nameEndsWith_iA);
            Assert.Empty(nameEndsWith_iA_Results);

            var nameEndsWith_ia = Unicorns.GetAtLeast(u => u.Name.EndsWith("ia"));
            var nameEndsWith_ia_Results = this.ApplQueryToHandler("*ia" + PropertyQuery.CaseMod);

            Assert.True(nameEndsWith_ia_Results.SequenceEqual(nameEndsWith_ia), "Cannot get results ending with string.");
        }

        [Fact]
        public void CanApplyIsEqual()
        {
            var daria = Unicorns.GetAtLeast(u => u.Name.Equals("daria", StringComparison.CurrentCultureIgnoreCase));
            var results = this.ApplQueryToHandler("daria");

            Assert.True(results.SequenceEqual(daria), "Cannot get results with string equals.");
        }

        [Fact]
        public void CanApplyIsEqual_CaseMod()
        {
            var daria = Unicorns.All.Where(u => u.Name == "daria");
            var daria_Results = this.ApplQueryToHandler("daria" + PropertyQuery.CaseMod);

            Assert.Empty(daria);
            Assert.Empty(daria_Results);

            var Daria = Unicorns.GetAtLeast(u => u.Name == "Daria");
            var Daria_Results = this.ApplQueryToHandler("Daria" + PropertyQuery.CaseMod);

            Assert.True(Daria_Results.SequenceEqual(Daria), "Cannot get results with string equals.");
        }
    }
}