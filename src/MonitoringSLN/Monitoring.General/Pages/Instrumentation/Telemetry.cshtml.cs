using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Monitoring.General.Pages.Instrumentation;

public class TelemetryPageModel : PageModel
{
    private readonly TelemetryClient telemetry;
    private readonly ILogger<TelemetryPageModel> logger;

    public TelemetryPageModel(TelemetryClient telemetry, ILogger<TelemetryPageModel> logger)
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

    public void OnPost()
    {
        logger.LogInformation("Sending telemetry information");
        try
        {
            telemetry.TrackTrace(new TraceTelemetry("Sending custom trace")
            {
                SeverityLevel = SeverityLevel.Warning,
                Properties =
                {
                    { "MachineName", Environment.MachineName },
                    { "Event", "Manually called" },
                    { "CalledAt", $"Called {DateTime.Now}" },
                }
            });
            telemetry.Flush();
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            telemetry.TrackException(e);
        }

        var traceMessage = "Trace has been sent and logged";
        logger.LogInformation(traceMessage);
        Message = traceMessage;
    }

    [BindProperty]
    public string Message { get; set; }
}