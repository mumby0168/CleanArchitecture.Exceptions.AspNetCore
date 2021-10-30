using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CleanArchitecture.Exceptions.AspNetCore;

public class CleanArchitectureExceptionsMiddleware : IMiddleware
{
    private readonly ILogger<CleanArchitectureExceptionsMiddleware> _logger;
    private readonly IOptionsMonitor<CleanArchitectureExceptionsOptions> _options;

    public CleanArchitectureExceptionsMiddleware(ILogger<CleanArchitectureExceptionsMiddleware> logger,
        IOptionsMonitor<CleanArchitectureExceptionsOptions> options)
    {
        _logger = logger;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ResourceNotFoundException exception)
        {
            _logger.LogInformation(
                "Handling resource not found exception with message {ResourceNotFoundExceptionMessage} and code {ResourceNotFoundExceptionCode}",
                exception.Message, exception.Code);
            context.Response.StatusCode = (int) HttpStatusCode.NotFound;
            await context.Response.WriteAsync(BuildErrorResponse(exception));
        }
        catch (ResourceExistsException exception)
        {
            _logger.LogInformation(
                "Handling resource exists exception with message {ResourceExistsExceptionMessage} and code {ResourceExistsExceptionCode}",
                exception.Message, exception.Code);
            context.Response.StatusCode = (int) HttpStatusCode.Conflict;
            await context.Response.WriteAsync(BuildErrorResponse(exception));
        }
        catch (DomainException exception)
        {
            _logger.LogInformation(
                "Handling domain exception with message {DomainExceptionMessage} and code {DomainExceptionCode}",
                exception.Message, exception.Code);
            context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
            await context.Response.WriteAsync(BuildErrorResponse(exception));
        }
        catch (BaseCleanArchitectureException exception)
        {
            var exceptionType = exception.GetType();
            if (_options.CurrentValue.CustomExceptionMappings.ContainsKey(exceptionType))
            {
                _logger.LogInformation(
                    "Handling custom exception derived from BaseCleanArchitectureException type custom type {CustomExceptionTypeName}",
                    exceptionType.Name);
                context.Response.StatusCode = (int) _options.CurrentValue.CustomExceptionMappings[exceptionType];
                await context.Response.WriteAsync(BuildErrorResponse(exception));
            }
        }
    }

    private string BuildErrorResponse(BaseCleanArchitectureException exception)
    {
        var error = new ErrorDto(exception.Message, exception.Code, exception.Type?.Name);
        var response = new ErrorResponse(new[] {error}, _options.CurrentValue.ApplicationName);
        return JsonConvert.SerializeObject(response, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
    }
}