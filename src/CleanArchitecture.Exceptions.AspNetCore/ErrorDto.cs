using System.Security.Principal;
using Newtonsoft.Json;

namespace CleanArchitecture.Exceptions.AspNetCore;

public class ErrorDto
{
    public string Message { get; set; }

    public string Code { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? ResourceName { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? Property { get; set; }

    public ErrorDto(string message, string code, string? resourceName = null, string? property = null)
    {
        Message = message;
        Code = code;
        ResourceName = resourceName;
        Property = property;
    }
}