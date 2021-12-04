using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Exceptions.AspNetCore;

public static class Extensions
{
    public static IServiceCollection AddCleanArchitectureExceptionsHandler(this IServiceCollection services, Action<CleanArchitectureExceptionsOptions>? optionsBuilder = null)
    {
        services.AddSingleton<CleanArchitectureExceptionsMiddleware>();
        
        services.AddOptions<CleanArchitectureExceptionsOptions>()
            .Configure<IConfiguration>(
                (settings, configuration) =>
                    configuration.GetSection(nameof(CleanArchitectureExceptionsOptions)).Bind(settings));


        if (optionsBuilder is not null)
        {
            services.PostConfigure(optionsBuilder);
        }
        
        return services;
    }

    public static IApplicationBuilder UseCleanArchitectureExceptionsHandler(this IApplicationBuilder app) =>
        app.UseMiddleware<CleanArchitectureExceptionsMiddleware>();

    public static T As<T>(this BaseCleanArchitectureException exception) where T : BaseCleanArchitectureException => 
        (T) exception;
}