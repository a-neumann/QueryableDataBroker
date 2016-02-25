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

		//// GET: api/values
		//[HttpGet]
		//public IEnumerable<string> Get()
		//{
		//    return new string[] { "value1", "value2" };
		//}

		//// GET api/values/5
		//[HttpGet("{id}")]
		//public string Get(int id)
		//{
		//    return "value";
		//}

		//// POST api/values
		//[HttpPost]
		//public void Post([FromBody]string value)
		//{
		//}

		//// PUT api/values/5
		//[HttpPut("{id}")]
		//public void Put(int id, [FromBody]string value)
		//{
		//}

		//// DELETE api/values/5
		//[HttpDelete("{id}")]
		//public void Delete(int id)
		//{
		//}
	}
}
