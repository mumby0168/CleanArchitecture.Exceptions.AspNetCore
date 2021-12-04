using CleanArchitecture.Exceptions;
using MinimalApisWeb.Models;

namespace MinimalApisWeb.Exceptions;

public class ComplexValidationException : BaseCleanArchitectureException
{
    public string[] Errors { get; }

    public ComplexValidationException(params string[] errors) : base("A validation error occured")
    {
        Errors = errors;
    }
}