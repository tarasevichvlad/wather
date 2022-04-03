using Microsoft.Extensions.DependencyInjection;

namespace Weather.Tools.Parser;

public static class BootstrapExtensions
{
    public static IServiceCollection ConfigureParser(this IServiceCollection services)
    {
        services.AddSingleton<IParser, Parser>();

        return services;
    }
}