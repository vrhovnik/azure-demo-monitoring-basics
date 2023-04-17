using Azure;
using Azure.Identity;
using Azure.Monitor.Query;
using Azure.Monitor.Query.Models;
using Htmx;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Monitoring.General.Options;
using Monitoring.General.ViewModels;

namespace Monitoring.General.Pages.Custom;

public class SearchPageModel : PageModel
{
    private readonly ILogger<SearchPageModel> logger;
    private MonitoringOptions monitoringOptions;

    public SearchPageModel(ILogger<SearchPageModel> logger,
        IOptions<MonitoringOptions> monitorOptionsValue)
    {
        this.logger = logger;
        monitoringOptions = monitorOptionsValue.Value;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        logger.LogInformation("Loading page Search custom logs at {DateLoaded}", DateTime.Now);

        var credential = new DefaultAzureCredential();
        LogsQueryClient logsQueryClient = new(credential);

        LogsBatchQuery batch = new();
        var query = $"{monitoringOptions.TableName}";
        var queryResult = batch.AddWorkspaceQuery(
            monitoringOptions.WorkspaceId,
            Query,
            new QueryTimeRange(TimeSpan.FromDays(1)));

        Response<LogsBatchQueryResultCollection> queryResponse =
            await logsQueryClient.QueryBatchAsync(batch).ConfigureAwait(false);

        var list = new List<CustomLogViewModel>();
        foreach (var logsTableRow in queryResponse.Value.GetResult(queryResult).Table.Rows)
        {
            var lv = (CustomLogViewModel)logsTableRow["AdditionalContext"];
            list.Add(lv);
        }

        if (!Request.IsHtmx()) return Page();

        Result = list;
        return Partial("_SearchResults", list);
    }

    [BindProperty(SupportsGet = true)] public string Query { get; set; }
    [BindProperty] public List<CustomLogViewModel> Result { get; set; } = new();
}