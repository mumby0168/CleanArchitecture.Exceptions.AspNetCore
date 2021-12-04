using System.Net;

namespace CleanArchitecture.Exceptions.AspNetCore;

internal class CustomExceptionPolicy
{
    public HttpStatusCode StatusCode { get; set; }
    
    public Func<BaseCleanArchitectureException, IEnumerable<ErrorDto>>? HandleException { get; set; }
}   