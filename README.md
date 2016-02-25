## Overview

Imagine you could create an ASP.Net MVC API Controller with almost zero configuration, that serves a collection of models by understanding URL queries as filters on property values.

## Setup

Inside *Startup.cs*

```cs
public void ConfigureServices(IServiceCollection services)
{
	// Add EF as our datasource
    services.AddEntityFramework()
        .AddInMemoryDatabase()
        .AddDbContext<SampleContext>();

    // Adds the QuerySummary formatter to MVC (we won't use it in this example)
    services.AddMvc().AddQueryableDataBroker();
}
```

Your EF context has a DataSet of unicorns for example

```cs
public class Unicorn
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    // ...
}
```

Create a new MVC controller that will act as our API endpoint

The base constructor obtains two parameters:

- The collection as IQueryable<YourModelType> that, in this case is the DataSet
- An Expression that points at the model types key property

```cs
[Route("[controller]")]
public class UnicornController : QueryableDataController<Unicorn, Guid>
{
    public UnicornController(SampleContext context) 
        : base(context.Unicorns, u => u.Id)
    {
	}
}
```

You're ready to run the application.

## REST API

### Syntax:

`http://yourhost/yourcontroller?{Property name}={Property query}&page={Start at page Number}&pageSize={Items per page}`

- Property name are case insensitive
- The property query syntax may vary per property type
- Property query value ranges are declared like `min*max`
- String comparison can be done like `startswith*endswith`
- `page` and `pageSize` URL queries are reserved for built in pagination

If your model uses `page` or `pageSize` as property names, you can still paginate by prepending a dollar sign to both. e.g. `$page`

Example:

`http://yourhost/unicorn?name=Da*&hornlength=100*150&brithdate=*01-01-2016&page=1&pageSize=20`

### Supported Property Types:

- String
- Int32, Int64, Int16
- DateTime
- Boolean