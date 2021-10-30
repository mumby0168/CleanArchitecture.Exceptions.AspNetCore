namespace CleanArchitecture.Exceptions.AspNetCore;

public class ErrorResponse
{
    public string ApplicationName { get; set; }
    
    public IEnumerable<ErrorDto> Errors { get; set; }

    public ErrorResponse(IEnumerable<ErrorDto> errors, string? applicationName = null)
    {
        ApplicationName = applicationName ?? "Undefined";
        Errors = errors;
    }
}