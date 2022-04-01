namespace Weather.API.Extensions;

public static class BootstrapExtensions
{
    public const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

    public static IServiceCollection ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.WithOrigins("https://localhost:7130")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        return services;
    }


}