using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QueryableDataBroker
{
    public struct PropertyQuery
    {
        public static PropertyQuery Create(string propertyName, string value = null)
        {
            var newQuery = new PropertyQuery() { Property = propertyName };

            if (!String.IsNullOrEmpty(value))
            {
                newQuery.AddCondition(value);
            }

            return newQuery;
        }

        public static readonly string CaseMod = "!!";
        public static readonly string ContainsRx = @"^\*(.+)\*$";
        public static readonly string RangeRx = @"^(.*)\*(.*)$";
        public static readonly string IsBoolRx = @"^(true|false)$";

        public string Property;

        public string Contains;
        public bool ContainsCaseMod;

        public string RangeStart;
        public string RangeEnd;
        public bool RangeCaseMod;

        public IEnumerable<string> IsEqual;
        public bool IsEqualCaseMod;

        public bool? IsBool;

        private IEnumerable<string> GetMatches(string input, string pattern)
        {
            var m = new Regex(pattern).Match(input);
            if (m.Success)
            {
                var matches = new List<string>();
                for (int i = 1; i < m.Groups.Count; i++)
                {
                    string val = m.Groups[i].Value;
                    matches.Add(val != String.Empty ? val : null);
                }

                return matches;
            }

            return null;
        }

        private string RemoveCaseMod(string input, out bool caseSensitive)
        {
            caseSensitive = false;

            if (!String.IsNullOrEmpty(input)) {

                caseSensitive = input.EndsWith(CaseMod);
                if (caseSensitive)
                {
                    return input.Substring(0, input.Length - CaseMod.Length);
                }
            }
            return input;
        }

        public PropertyQuery AddCondition(string value)
        {
            bool caseMod = false;
            value = this.RemoveCaseMod(value, out caseMod);

            var contains = GetMatches(value, ContainsRx);
            if (contains != null)
            {
                this.Contains = contains.First();
                this.ContainsCaseMod = caseMod;

                return this;
            }

            var range = GetMatches(value, RangeRx);
            if (range != null)
            {
                this.RangeStart = range.ElementAtOrDefault(0);
                this.RangeEnd = range.ElementAtOrDefault(1);
                this.RangeCaseMod = caseMod;

                return this;
            }

            var isBool = this.GetMatches(value, IsBoolRx);
            if (isBool != null)
            {
                this.IsBool = isBool.First() == "true";

                return this;
            }

            if (!String.IsNullOrEmpty(value))
            {
                this.IsEqual = Regex.Split(value, @"(?<!\s),(?!\s)");
                this.IsEqualCaseMod = caseMod;
            }

            return this;
        }

        //public override string ToString()
        //{    
        //    return "";
        //}
    }
}
