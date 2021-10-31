# CleanArchitecture.Exceptions.AspNetCore
Some middleware to be used to handle exceptions provided by `CleanArchitecture.Exceptions`

## Getting started

See the sample app below on how to get started

```csharp
using System.Net;
using CleanArchitecture.Exceptions;
using CleanArchitecture.Exceptions.AspNetCore;
using Microsoft.EntityFrameworkCore;
using MinimalApisWeb;
using MinimalApisWeb.Exceptions;
using MinimalApisWeb.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContext<CustomersDbContext>(options => options.UseInMemoryDatabase("Db1"));

builder.Services.AddCleanArchitectureExceptionsHandler(options =>
{
    options.ApplicationName = "MinimalApisWeb";
    options.ConfigureCustomException<ForbiddenException>(HttpStatusCode.Forbidden);
});

var app = builder.Build();

app.UseCleanArchitectureExceptionsHandler();

app.MapGet("/", () => "MinimalApisWeb");

app.MapGet("/customers", async (CustomersDbContext database) => await database.Customers.ToListAsync());

app.MapGet("/customers/{id}", async (int id, CustomersDbContext database) =>
{
    var customer = await database.Customers.FindAsync(id);

    if (customer is null)
    {
        throw new ResourceNotFoundException<Customer>($"A customer with the id {id} was not found");
    }

    return customer;
});

app.MapPost("/customers", async (Customer customer, CustomersDbContext database) =>
{
    var existingCustomer = await database.Customers.FirstOrDefaultAsync(x => x.EmailAddress == customer.EmailAddress);

    if (existingCustomer is not null)
    {
        throw new ResourceExistsException<Customer>($"A customer with the email address {customer.EmailAddress} already exists");
    }

    database.Customers.Add(customer);
    await database.SaveChangesAsync();

    return customer;
});

app.MapGet("/forbidden", _ => throw new ForbiddenException("This is a custom exception"));

app.Run();
```

## Response Schema

An error returned from the middleware will look as shown below

```json
{
  "applicationName": "MinimalApisWeb",
  "errors": [
    {
      "message": "A customer with the id 12 was not found",
      "code": "resource_not_found",
      "resourceName": "Customer"
    }
  ]
}
```
