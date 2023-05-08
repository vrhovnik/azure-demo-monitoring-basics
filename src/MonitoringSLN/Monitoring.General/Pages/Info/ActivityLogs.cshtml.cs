using Azure.Identity;
using Azure.Monitor.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Monitoring.General.Options;
using Monitoring.General.ViewModels;

namespace Monitoring.General.Pages.Info;

[Authorize]
public class ActivityLogsPageModel : PageModel
{
    private readonly ILogger<ActivityLogsPageModel> logger;
    private MonitoringOptions monitoringOptions;

    public ActivityLogsPageModel(ILogger<ActivityLogsPageModel> logger,
        IOptions<MonitoringOptions> monitoringOptionsValue)
    {
        this.logger = logger;
        monitoringOptions = monitoringOptionsValue.Value;
    }

    public async Task OnGet()
    {
        logger.LogInformation("Loaded activity logs at {DateLoaded}", DateTime.Now);

        var client = new LogsQueryClient(new DefaultAzureCredential());
        var response = await client.QueryWorkspaceAsync(
            monitoringOptions.WorkspaceId,
            "InsightsMetrics | top 20 by TimeGenerated",
            new QueryTimeRange(TimeSpan.FromDays(7)));

        var table = response.Value.Table;
        var list = new List<PerfResultViewModel>();
        foreach (var row in table.Rows)
        {
            list.Add(new PerfResultViewModel
            {
                Computer = row["Computer"].ToString(),
                ObjectName = row["Namespace"].ToString(),
                CounterName = row["Name"].ToString(),
                CounterValue = row["Val"].ToString()
            });
        }

        Result = list;
    }

    public List<PerfResultViewModel> Result { get; set; }
}