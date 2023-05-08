using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Metrics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Monitoring.General.Pages.Instrumentation;

public class SendMetricsPageModel : PageModel
{
    private readonly ILogger<SendMetricsPageModel> logger;
    private readonly TelemetryClient telemetryClient;

    public SendMetricsPageModel(ILogger<SendMetricsPageModel> logger, TelemetryClient telemetryClient)
    {
        this.logger = logger;
        this.telemetryClient = telemetryClient;
    }

    public void OnGet()
    {
        logger.LogInformation("Page send metrics was loaded at {DateLoaded}.", DateTime.Now);
    }

    public async Task<RedirectToPageResult> OnPostSimpleAsync()
    {
        logger.LogInformation("Simulating sending simple metrics");
        var metric = telemetryClient.GetMetric("EmailSent");
        metric.TrackValue(110);
        await Task.Delay(5000);
        metric.TrackValue(130);
        await Task.Delay(2000);
        var series = metric.GetAllSeries();
        var msg = "";
        foreach (var pair in series)
        {
            var keys = "";
            foreach (var currentKey in pair.Key)
            {
                keys += currentKey + ";";
            }

            msg +=
                $"keys: {keys} with namespace: {pair.Value.MetricIdentifier.MetricNamespace} and dimensions: {pair.Value.MetricIdentifier.DimensionsCount}";
        }

        Message = msg;
        logger.LogInformation("Current Values in database {Values}", msg);
        return RedirectToPage("/Instrumentation/SendMetrics");
    }

    [TempData] public string Message { get; set; }

    public async Task OnPostMultiAsync()
    {
        logger.LogInformation("Simulating sending simple metrics");

        var metConfig = new MetricConfiguration(seriesCountLimit: 100, valuesPerDimensionLimit: 2,
            new MetricSeriesConfigurationForMeasurement(restrictToUInt32Values: false));

        var metric = telemetryClient.GetMetric("EmailSent", "Departments", metConfig);
        metric.TrackValue(120, "IT");
        await Task.Delay(5000);
        metric.TrackValue(130, "CEO");
        metric.TrackValue(10000, "Marketing");

        var series = metric.GetAllSeries();
        var msg = "";
        foreach (var pair in series)
        {
            var keys = "";
            foreach (var currentKey in pair.Key)
            {
                keys += currentKey + ";";
            }

            msg +=
                $"keys: {keys} with namespace: {pair.Value.MetricIdentifier.MetricNamespace} and dimensions: {pair.Value.MetricIdentifier.DimensionsCount}";
        }

        Message = msg;

        logger.LogInformation("Current Values in database {Values}", msg);
    }
}