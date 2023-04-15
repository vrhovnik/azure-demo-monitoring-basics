using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Monitoring.General.Pages.Instrumentation;

public class DashboardPageModel : PageModel
{
    private readonly TelemetryClient telemetry;
    private readonly ILogger<DashboardPageModel> logger;

    public DashboardPageModel(TelemetryClient telemetry,Logger<DashboardPageModel> logger)
    {
        this.telemetry = telemetry;
        telemetry.Context.Device.Id = Environment.MachineName;
        this.logger = logger;
    }

    public void OnGet()
    {
        var pageViewTelemetry = new PageViewTelemetry("IndexPageModel");
        logger.LogInformation("Loading page at {DateLoaded}", DateTime.Now);
        pageViewTelemetry.Properties["Page"] = "Dashboard";
        telemetry.TrackPageView(pageViewTelemetry);
    }
}