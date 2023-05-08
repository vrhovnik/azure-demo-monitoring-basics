using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Monitoring.General.Pages.Instrumentation;

public class TrackTracePageModel : PageModel
{
    private readonly ILogger<TrackTracePageModel> logger;
    private readonly TelemetryClient telemetryClient;

    public TrackTracePageModel(ILogger<TrackTracePageModel> logger, TelemetryClient telemetryClient)
    {
        this.logger = logger;
        this.telemetryClient = telemetryClient;
    }

    public void OnGet() => logger.LogInformation("Load track trace page model at {DateLoaded}", DateTime.Now);

    public void OnPost()
    {
        logger.LogInformation("Sending custom data");
        //simulate ping server and return data 
        telemetryClient.TrackTrace("Slow database response",
            SeverityLevel.Warning,
            new Dictionary<string, string> { { "database", "NewsDb" } });
    }
}