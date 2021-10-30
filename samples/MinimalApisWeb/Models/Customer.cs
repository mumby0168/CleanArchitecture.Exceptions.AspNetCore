using CleanArchitecture.Exceptions;

namespace MinimalApisWeb.Models;

public class Customer
{
    public int Id { get; set; }

    public string EmailAddress { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public Customer(int id, string emailAddress, string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(emailAddress))
        {
            throw new DomainException<Customer>("A customer must provide an email address");
        }
        
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new DomainException<Customer>("A customer must provide an first name");
        }
        
        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new DomainException<Customer>("A customer must provide an second name");
        }

        Id = id;
        EmailAddress = emailAddress;
        FirstName = firstName;
        LastName = lastName;
    }
}