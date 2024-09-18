using attendance.Components;
using attendance.Services;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Microsoft.Extensions.Logging.ApplicationInsights;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddApplicationInsights(
        configureTelemetryConfiguration: (config) => 
//            config.ConnectionString = builder.Configuration.GetConnectionString("config"),

            config.ConnectionString = "InstrumentationKey=7ca9bdd1-32ad-4e0e-8cbf-d96b7a6383ad;IngestionEndpoint=https://eastus-8.in.applicationinsights.azure.com/;LiveEndpoint=https://eastus.livediagnostics.monitor.azure.com/;ApplicationId=80536a34-a344-4962-9410-02a9e7a44740",
            configureApplicationInsightsLoggerOptions: (options) => { }
    );

builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>("*", LogLevel.Trace);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.TryAddEnumerable(
    ServiceDescriptor.Scoped<CircuitHandler, BlazorNonceService>(sp => sp.GetRequiredService<BlazorNonceService>()));

builder.Services.AddScoped<BlazorNonceService>();
builder.Services.AddHttpContextAccessor(); // Register IHttpContextAccessor
builder.Services.AddScoped<HttpContextAccessorService>(); // Register HttpContextAccessorService

// Add Application Insights telemetry services
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.EnableAdaptiveSampling = false; // Optional: Disable adaptive sampling if needed
    options.EnableDebugLogger = true; // Optional: Enable debug logger for troubleshooting
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();