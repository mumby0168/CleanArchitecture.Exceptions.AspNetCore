using System.Net;

namespace CleanArchitecture.Exceptions.AspNetCore;

public class CleanArchitectureExceptionsOptions
{
    private readonly Dictionary<Type, CustomExceptionPolicy> _customExceptionMappings;
    
    public CleanArchitectureExceptionsOptions()
    {
        _customExceptionMappings = new Dictionary<Type, CustomExceptionPolicy>();
    }
    
    public string? ApplicationName { get; set; }

    internal IReadOnlyDictionary<Type, CustomExceptionPolicy> CustomExceptionMappings => _customExceptionMappings;

    public CleanArchitectureExceptionsOptions ConfigureCustomException<TException>(HttpStatusCode statusCode, Func<BaseCleanArchitectureException, IEnumerable<ErrorDto>>? responseBuilder = null)
        where TException : BaseCleanArchitectureException
    {
        _customExceptionMappings.Add(typeof(TException), new CustomExceptionPolicy()
        {
            StatusCode = statusCode,
            HandleException = responseBuilder
        });
        
        return this;
    }
}