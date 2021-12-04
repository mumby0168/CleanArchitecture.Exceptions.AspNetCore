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
    options.ConfigureComplexValidationException();
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
        throw new ResourceExistsException<Customer>(
            $"A customer with the email address {customer.EmailAddress} already exists");
    }

    database.Customers.Add(customer);
    await database.SaveChangesAsync();

    return customer;
});

app.MapGet("/forbidden", _ => throw new ForbiddenException("This is a custom exception"));

app.MapGet("/complex", _ => throw new ComplexValidationException("This is wrong", "So is this", "And this"));

app.Run();

public static class CustomExceptionRegistrationExtensions
{
    public static CleanArchitectureExceptionsOptions ConfigureComplexValidationException(
        this CleanArchitectureExceptionsOptions options) =>
        options.ConfigureCustomException<ComplexValidationException>(HttpStatusCode.BadRequest, exception => 
            exception.As<ComplexValidationException>()
                .Errors
                .Select(x => new ErrorDto(x, "custom_validation_exception")));
}