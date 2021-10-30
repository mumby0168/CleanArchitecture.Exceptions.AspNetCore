using CleanArchitecture.Exceptions;

namespace MinimalApisWeb.Exceptions;

public class ForbiddenException : BaseCleanArchitectureException
{
    public ForbiddenException(string message) : base(message)
    {
        Code = "oops_this_failed";
    }
}