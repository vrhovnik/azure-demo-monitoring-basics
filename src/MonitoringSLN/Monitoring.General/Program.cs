using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.EventCounterCollector;
using Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Monitoring.General.Filters;
using Monitoring.General.Options;
using Monitoring.General.Services;
using Monitoring.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<AzureAdOptions>().Bind(builder.Configuration.GetSection("AzureAd"));
    // .ValidateDataAnnotations()
    // .ValidateOnStart();
builder.Services.AddOptions<MonitoringOptions>().Bind(builder.Configuration.GetSection("Monitoring"));
builder.Services.AddOptions<BingServiceOptions>().Bind(builder.Configuration.GetSection("BingService"));
builder.Services.AddOptions<SqlOptions>().Bind(builder.Configuration.GetSection("SqlService"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
        options.Conventions
            .AddPageRoute("/Info/Dashboard", ""))
    .AddMicrosoftIdentityUI();

//fixed rate sampling: https://learn.microsoft.com/en-us/azure/azure-monitor/app/sampling?tabs=net-core-new#types-of-sampling
// builder.Services.Configure<TelemetryConfiguration>(telemetryConfiguration =>
// {
//     var builder = telemetryConfiguration.DefaultTelemetrySink.TelemetryProcessorChainBuilder;
//     // Using fixed rate sampling
//     double fixedSamplingPercentage = 10;
//     builder.UseSampling(fixedSamplingPercentage);
// });

// builder.Services.Configure<TelemetryConfiguration>(telemetryConfiguration =>
// {
//     var builder = telemetryConfiguration.DefaultTelemetrySink.TelemetryProcessorChainBuilder;
//     // Using adaptive sampling
//     builder.UseAdaptiveSampling(maxTelemetryItemsPerSecond: 5);
//     // Alternately, the following configures adaptive sampling with 5 items per second, and also excludes DependencyTelemetry from being subject to sampling:
//     // configuration.DefaultTelemetrySink.TelemetryProcessorChainBuilder.UseAdaptiveSampling(maxTelemetryItemsPerSecond:5, excludedTypes: "Dependency");
// });

//check out https://learn.microsoft.com/en-us/azure/azure-monitor/app/asp-net-core?tabs=netcorenew%2Cnetcore6#use-applicationinsightsserviceoptions
var aiOptions = new Microsoft.ApplicationInsights.AspNetCore.Extensions.ApplicationInsightsServiceOptions
{
    // Disables adaptive sampling.
    //EnableAdaptiveSampling = false,
    // Disables QuickPulse (Live Metrics stream).
    //EnableQuickPulseMetricStream = false,
    EnableHeartbeat = true
};
//builder.Services.AddSingleton(typeof(ITelemetryChannel), new ServerTelemetryChannel() {StorageFolder = @"c:\Work\applicationinsightsdata" });
builder.Services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, _) =>
{
    module.EnableW3CHeadersInjection = true;
    module.EnableSqlCommandTextInstrumentation = true;
});

// The following removes all default counters from EventCounterCollectionModule, and adds a single one.
// builder.Services.ConfigureTelemetryModule<EventCounterCollectionModule>((module, o) =>
// {
//     module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "gen-0-size"));
// });

builder.Services.AddHttpClient<INewsService, BingNewsService>();
builder.Services.AddHealthChecks();
builder.Services.AddServiceProfiler();
builder.Services.AddApplicationInsightsTelemetry(aiOptions);
//builder.Services.AddApplicationInsightsTelemetryProcessor<SuccessfulDependencyFilter>();
//builder.Services.AddSingleton<ITelemetryInitializer, MyTelemetryInitializer>();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    //TelemetryConfiguration.Active.TelemetryChannel.DeveloperMode = true;
    app.UseExceptionHandler("/Error");
}

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