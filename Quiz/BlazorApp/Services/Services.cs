using BlazorApp.Services.Collection;
namespace BlazorApp.Services;

public static class Services
{
    public static void AddCustomServices(this IServiceCollection services)
    {
        services.AddScoped<AppDbContextService>();
        services.AddSingleton<IDataService, DataService>();
        services.AddScoped<IBasicAuthService, BasicAuthService>();
        services.AddScoped<IAlertService, AlertService>();
    }
}