using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Monitoring.General.Options;
using Monitoring.General.Services;
using Monitoring.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<AzureAdOptions>().Bind(builder.Configuration.GetSection("AzureAd"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddOptions<MonitoringOptions>().Bind(builder.Configuration.GetSection("Monitoring"));
builder.Services.AddOptions<BingServiceOptions>().Bind(builder.Configuration.GetSection("BingService"));

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
        options.Conventions
            .AddPageRoute("/Info/Dashboard", ""))
    .AddMicrosoftIdentityUI();

builder.Services.AddHttpClient<INewsService, BingNewsService>();
builder.Services.AddHealthChecks();
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddServiceProfiler();

var app = builder.Build();

if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/Error");

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("/health").AllowAnonymous();
    endpoints.MapRazorPages();
    endpoints.MapControllers();
});
app.Run();