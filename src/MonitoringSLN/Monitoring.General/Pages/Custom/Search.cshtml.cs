using System.Runtime.Serialization.Formatters.Binary;
using Azure;
using Azure.Core.Serialization;
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
        var queryResult = batch.AddWorkspaceQuery(
            // monitoringOptions.WorkspaceId,
            "4ac7af67-2c10-4c17-b3d9-ee7bad1a4621",
            "AdrianBojan_CL",
            new QueryTimeRange(TimeSpan.FromDays(1)),
            new LogsQueryOptions { IncludeStatistics = true });

        Response<LogsBatchQueryResultCollection> queryResponse =
            await logsQueryClient.QueryBatchAsync(batch).ConfigureAwait(false);

        var list = new List<CustomLogViewModel>();
        var data = queryResponse.Value.GetResult(queryResult);
        foreach (var logsTableRow in data.Table.Rows)
        {
            var addContext = logsTableRow["AdditionalContext"] as BinaryData;
            var lv = addContext.ToObject<CustomLogViewModel>(new JsonObjectSerializer());
            list.Add(lv);
        }

        Result = list;
        if (!Request.IsHtmx()) return Page();

        return Partial("_SearchResults", list);
    }

    [BindProperty(SupportsGet = true)] public string Query { get; set; }
    [BindProperty] public List<CustomLogViewModel> Result { get; set; } = new();
}