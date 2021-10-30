namespace CleanArchitecture.Exceptions.AspNetCore;

public class ErrorDto
{
    public string Message { get; set; }

    public string Code { get; set; }

    public string ResourceName { get; set; }

    public ErrorDto(string message, string code, string? resourceName = null)
    {
        Message = message;
        Code = code;
        ResourceName = resourceName ?? "Undefined";
    }
}