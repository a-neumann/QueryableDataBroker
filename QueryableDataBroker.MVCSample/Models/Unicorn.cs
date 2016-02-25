using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QueryableDataBroker.MVCSample.Models
{
    public class Unicorn
    {
        public Unicorn()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public int HornLenght { get; set; }
        public bool HasWings { get; set; }
        public DateTime BirthDate { get; set; }
        public string MagicSecret { get; set; }

        public int Age
        {
            get
            {
                int age = DateTime.Today.Year - this.BirthDate.Year;
                return age;
            }
        }
    }
}
