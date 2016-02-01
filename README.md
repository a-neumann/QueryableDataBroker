#QueryableDataBroker

Description and details following soon

See [QueryableDataBroker.Tests](./QueryableDataBroker.Tests) for details

```cs
public class Unicorn
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    // ...
}
```

```cs
// Unicorns.All - IQueryable<Unicorn>
var broker = new QueryBroker<Unicorn, Guid>(Unicorns.All, u => u.Id);

var queries = new [] {
    PropertyQuery.Create("name", "da*"),
    PropertyQuery.Create("birthdate", "2016-01-01*"),
};

// results are all Unicorns from Unicorns.All with
//     Name starting with "da" and 
//     BrithDate greater than 2016-01-10
var results = broker.Find(queries);
```

##ASP.Net MVC use case example with Controller following *very* soon!