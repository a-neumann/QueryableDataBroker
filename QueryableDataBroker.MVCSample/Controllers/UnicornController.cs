using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using QueryableDataBroker.Mvc;
using QueryableDataBroker.MVCSample.Models;
using QueryableDataBroker.MVCSample.DAL;

namespace QueryableDataBroker.MVCSample.Controllers
{
	[Route("[controller]")]
	public class UnicornController : QueryableDataController<Unicorn, Guid>
    {
        public UnicornController(SampleContext context) 
            : base(context.Unicorns, u => u.Id)
        {
            if(!context.Unicorns.Any())
            {
                context.Unicorns.AddRange(new[] {
                    new Unicorn() { Id = new Guid("66fc62cf-27d8-439e-bb6a-e786db9f1a2a"), Name = "Fred", HornLenght = 120, BirthDate = new DateTime(2016, 1, 1) },
                    new Unicorn() { Id = new Guid("6acc7787-ce29-4f4e-802a-8954ff65bf00"), Name = "Dave", HornLenght = 115, BirthDate = new DateTime(2015, 12, 12) },
                    new Unicorn() { Id = new Guid("d7f2c96e-f2ec-4155-bb78-e127f213dd6c"), Name = "Daria", HornLenght = 112, HasWings = true, BirthDate = new DateTime(2016, 1, 30) }
                });
                context.SaveChanges();
            }
        }

        public override QueryBrokerOptions<Unicorn> Configure(QueryBrokerOptions<Unicorn> options)
        {
			return options
				.AllowPropertyAccess(u => u.Age, false);
                //.AllowPropertyAccess(u => u.MagicSecret, this.CanReadMagicSecret);
        }

		[Route("[action]")]
		public HttpStatusCodeResult CanRespond()
		{
			return this.Ok();
		}

		// additional actions are pre defined in the QueryableDataController base class
	}
}
