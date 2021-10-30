using System.Net;

namespace CleanArchitecture.Exceptions.AspNetCore;

public class CleanArchitectureExceptionsOptions
{
    private readonly Dictionary<Type, HttpStatusCode> _customExceptionMappings;
    
    public CleanArchitectureExceptionsOptions()
    {
        _customExceptionMappings = new Dictionary<Type, HttpStatusCode>();
    }
    
    public string? ApplicationName { get; set; }

    public IReadOnlyDictionary<Type, HttpStatusCode> CustomExceptionMappings => _customExceptionMappings;

    public CleanArchitectureExceptionsOptions ConfigureCustomException<TException>(HttpStatusCode statusCode)
        where TException : BaseCleanArchitectureException
    {
        _customExceptionMappings.Add(typeof(TException), statusCode);
        return this;
    }
}