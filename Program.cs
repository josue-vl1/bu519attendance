using attendance.Components;
using attendance.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpContextAccessor(); // Register IHttpContextAccessor
//builder.Services.AddScoped<HttpContextAccessorService>(); // Register HttpContextAccessorService

// Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(120); // Set session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add Application Insights
builder.Services.AddApplicationInsightsTelemetry("7ca9bdd1-32ad-4e0e-8cbf-d96b7a6383ad");
builder.Services.AddMemoryCache();

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

app.UseRouting(); // Add UseRouting before UseAntiforgery

app.UseSession(); // Add session middleware

app.UseAntiforgery(); // Add UseAntiforgery after UseRouting

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.Run();