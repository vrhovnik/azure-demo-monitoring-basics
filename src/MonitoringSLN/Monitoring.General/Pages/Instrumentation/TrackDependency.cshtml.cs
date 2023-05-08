using System.Diagnostics;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Monitoring.General.Pages.Instrumentation;

public class TrackDependencyPageModel : PageModel
{
    private readonly ILogger<TrackDependencyPageModel> logger;
    private readonly TelemetryClient telemetryClient;

    public TrackDependencyPageModel(ILogger<TrackDependencyPageModel> logger, TelemetryClient telemetryClient)
    {
        this.logger = logger;
        this.telemetryClient = telemetryClient;
    }

    public void OnGet() => logger.LogInformation("Page Track dependecy loaded at {DateLoaded}", DateTime.Now);

    public async Task<RedirectToPageResult> OnPostAsync()
    {
        logger.LogInformation("Post for tracking dependency called");
        var timer = Stopwatch.StartNew();
        var startTime = DateTime.UtcNow;
        timer.Start();
        await Task.Delay(2000);
        timer.Stop();
        telemetryClient.TrackDependency("Monitoring.General.Pages.Instrumentation.DependencyPageModel",
            "Post", "Dependency call",
            $"this has been called at {DateTime.Now}",
            startTime, timer.Elapsed, "200", true);
        logger.LogInformation("Finished with action at {DateFinished}", DateTime.Now);
        Message = "Call has been done, check KQL in Application Insights";
        return RedirectToPage("/Instrumentation/TrackDependency");
    }

    [TempData] public string Message { get; set; }
}