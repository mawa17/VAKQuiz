using BlazorApp.Components;
using BlazorApp.Data;
using BlazorApp.Services;
using Microsoft.EntityFrameworkCore;

#region Builder
var builder = WebApplication.CreateBuilder(args);

// Enable detailed logs
builder.Logging.AddConsole();

// Register IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Named HttpClient with dynamic BaseAddress and optional cert bypass (DEV only)
builder.Services.AddHttpClient("Default", (serviceProvider, client) =>
{
    var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
    var request = httpContextAccessor.HttpContext?.Request;

    if (request == null)
        throw new InvalidOperationException("HttpContext is not available during HttpClient configuration.");

    // Dynamically set base address with correct scheme
    client.BaseAddress = new Uri($"{request.Scheme}://{request.Host}/");
}).ConfigurePrimaryHttpMessageHandler(() =>
new HttpClientHandler
{
    // Bypass cert validation
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});


// Register default HttpClient for DI
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Default"));

// Register QuickGrid's EF Adapter
builder.Services.AddQuickGridEntityFrameworkAdapter();

// DB Context with conditional config
builder.Services.AddDbContext<AppDbContext>(options =>
{
#if DEBUG
    options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection"));
    options.EnableSensitiveDataLogging();
    options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
#else
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProdConnection"));
#endif
});

// Razor Components & SSR
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        options.DetailedErrors = true;
    });

// Register custom services
builder.Services.AddCustomServices();
#endregion

#region Middleware
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseAntiforgery();

app.MapStaticAssets();

app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true,
    DefaultContentType = "application/json; charset=utf-8"
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Automatically apply EF Core migrations at runtime
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
#endregion