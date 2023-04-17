using Azure.Core;
using Azure.Identity;
using Azure.Monitor.Ingestion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Monitoring.General.Options;
using Monitoring.General.ViewModels;

namespace Monitoring.General.Pages.Custom;

[Authorize]
public class SendLogPageModel : PageModel
{
    private readonly ILogger<SendLogPageModel> logger;
    private MonitoringOptions monitoringOptions;

    public SendLogPageModel(ILogger<SendLogPageModel> logger,
        IOptions<MonitoringOptions> monitoringOptionsValue)
    {
        monitoringOptions = monitoringOptionsValue.Value;
        this.logger = logger;
    }

    public void OnGet()
    {
        logger.LogInformation("Loading page SendPageModel at {DateLoaded}", DateTime.Now);
    }

    public async Task OnPost()
    {
        logger.LogInformation("Sending custom log {Name} to Azure Monitor endpoint {DCE}",
            LogViewModel.Name, monitoringOptions.DCE);

        var currentTime = DateTimeOffset.UtcNow;
        var currentSender = new LoaderViewModel
        {
            MyTime = currentTime,
            Computer = Environment.MachineName,
            AdditionalContext = new
                CustomLogViewModel
                {
                    Name = LogViewModel.Name,
                    CounterValue = LogViewModel.CounterValue,
                    CounterName = LogViewModel.CounterName
                }
        };
        var data = BinaryData.FromObjectAsJson(new[] { currentSender });

        var client =
            new LogsIngestionClient(new Uri(monitoringOptions.DCE, UriKind.RelativeOrAbsolute),
                new DefaultAzureCredential());
        try
        {
            var response = await client.UploadAsync(
                monitoringOptions.RuleId,
                monitoringOptions.StreamName,
                RequestContent.Create(data));
            if (response.IsError)
                logger.LogWarning("we received error code.");
            else
            {
                logger.LogInformation("Data has been written to Azure Monitor {DateLoaded}.", DateTime.Now);
                Message = "Data has been written to Azure Monitor";
            }
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
        }
    }

    [BindProperty] public string Message { get; set; }
    [BindProperty] public CustomLogViewModel LogViewModel { get; set; }
}