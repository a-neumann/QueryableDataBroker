using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace QueryableDataBroker.Tests
{
    public class Unicorn
    {
        public Unicorn()
        {
            this.Id = Guid.NewGuid();
        }

        // Guid because unicorns are always unique
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int HornLenght { get; set; }
        public bool HasWings { get; set; }
        public DateTime BirthDate { get; set; }
    }

    public class Pony
    {
        public Pony()
        {
            _Increment++;
            this.Id = _Increment;
        }

        private static int _Increment { get; set; } = 100;

        // Ponys are not that unique...
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Unicorns
    {
        public static Unicorn Fred = new Unicorn() { Name = "Fred", HornLenght = 120, BirthDate = new DateTime(2016, 1, 1) };
        public static Unicorn Dave = new Unicorn() { Name = "Dave", HornLenght = 115, BirthDate = new DateTime(2015, 12, 12) };
        public static Unicorn Daria = new Unicorn() { Name = "Daria", HornLenght = 112, HasWings = true, BirthDate = new DateTime(2016, 1, 30) };
        public static Unicorn Cecil = new Unicorn() { Name = "Cecil", HornLenght = 128, HasWings = true, BirthDate = new DateTime(2016, 1, 25) };
        public static Unicorn Redneck = new Unicorn() { Name = "Redneck", HornLenght = 126, BirthDate = new DateTime(2015, 12, 22) };
        public static Unicorn Gloria = new Unicorn() { Name = "Gloria", HornLenght = 113, BirthDate = new DateTime(2016, 2, 1) };

        public static IQueryable<Unicorn> All = new List<Unicorn>() { Fred, Dave, Daria, Cecil, Redneck, Gloria }.AsQueryable();

        public static PropertyInfo GetProperty<P>(Expression<Func<Unicorn, P>> prop)
        {
            var memEx = (MemberExpression)prop.Body;
            return (PropertyInfo)memEx.Member;
        }

        public static IEnumerable<Unicorn> GetAtLeast(Func<Unicorn, bool> expression, int least = 1)
        {
            var results = All.Where(expression);
            if (results.Count() < least)
            {
                throw new Exception("Not enough results found.");
            }
            return results;
        }
    }

    public class Ponies
    {
        public static Pony Hank = new Pony() { Name = "Hank" };
        public static Pony Darcy = new Pony() { Name = "Darcy" };
        public static Pony Ron = new Pony() { Name = "Ron" };

        public static List<Pony> All = new List<Pony>() { Hank, Darcy, Ron };
    }
}
