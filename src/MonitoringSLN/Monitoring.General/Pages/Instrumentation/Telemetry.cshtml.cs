using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Monitoring.General.Pages.Instrumentation;

public class TelemetryPageModel : PageModel
{
    private readonly TelemetryClient telemetry;
    private readonly ILogger<TelemetryPageModel> logger;

    public TelemetryPageModel(TelemetryClient telemetry,Logger<TelemetryPageModel> logger)
    {
        this.telemetry = telemetry;
        telemetry.Context.Device.Id = Environment.MachineName;
        this.logger = logger;
    }

    public void OnGet()
    {
        var pageViewTelemetry = new PageViewTelemetry("IndexPageModel");
        logger.LogInformation("Loading page at {DateLoaded}", DateTime.Now);
        pageViewTelemetry.Properties["Page"] = "Telemetry";
        telemetry.TrackPageView(pageViewTelemetry);
        logger.LogInformation("Sent page TelemetryPageModel info to Application Insights.");
        
    }
}