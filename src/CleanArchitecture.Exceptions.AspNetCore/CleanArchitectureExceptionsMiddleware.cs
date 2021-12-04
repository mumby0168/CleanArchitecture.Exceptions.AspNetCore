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
        catch (BaseCleanArchitectureException exception) when (exception.GetType().Name.StartsWith(nameof(ResourceNotFoundException)))
        {
            _logger.LogInformation(
                "Handling resource not found exception with message {ResourceNotFoundExceptionMessage} and code {ResourceNotFoundExceptionCode}",
                exception.Message, exception.Code);
            context.Response.StatusCode = (int) HttpStatusCode.NotFound;
            await context.Response.WriteAsync(BuildErrorResponse(exception));
        }
        catch (BaseCleanArchitectureException exception) when (exception.GetType().Name.StartsWith(nameof(ResourceExistsException)))
        {
            _logger.LogInformation(
                "Handling resource exists exception with message {ResourceExistsExceptionMessage} and code {ResourceExistsExceptionCode}",
                exception.Message, exception.Code);
            context.Response.StatusCode = (int) HttpStatusCode.Conflict;
            await context.Response.WriteAsync(BuildErrorResponse(exception));
        }
        catch (BaseCleanArchitectureException exception) when (exception.GetType().Name.StartsWith(nameof(DomainException)))
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

                var policy = _options.CurrentValue.CustomExceptionMappings[exceptionType];

                context.Response.StatusCode = (int) policy.StatusCode;

                if (policy.HandleException is not null)
                {
                    await context.Response.WriteAsync(BuildErrorResponse(policy.HandleException(exception)));
                    return;
                }
                    
                await context.Response.WriteAsync(BuildErrorResponse(exception));
            }
        }
    }

    private string BuildErrorResponse(IEnumerable<ErrorDto> errors)
    {
        var response = new ErrorResponse(errors, _options.CurrentValue.ApplicationName);
        return JsonConvert.SerializeObject(response, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
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