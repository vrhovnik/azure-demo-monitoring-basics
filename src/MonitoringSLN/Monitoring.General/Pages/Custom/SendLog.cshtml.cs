using System.Net;
using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Monitor.Ingestion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Rest.Azure.Authentication;
using Monitoring.General.Options;
using Monitoring.General.ViewModels;

namespace Monitoring.General.Pages.Custom;

[Authorize]
public class SendLogPageModel : PageModel
{
    private readonly ILogger<SendLogPageModel> logger;
    private MonitoringOptions monitoringOptions;
    private AzureAdOptions azureAdOptions;

    public SendLogPageModel(ILogger<SendLogPageModel> logger,
        IOptions<AzureAdOptions> azureAdValue,
        IOptions<MonitoringOptions> monitoringOptionsValue)
    {
        monitoringOptions = monitoringOptionsValue.Value;
        azureAdOptions = azureAdValue.Value;
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
        var data = BinaryData.FromObjectAsJson(
            new[]
            {
                new
                {
                    Time = currentTime,
                    Computer = Environment.MachineName,
                    AdditionalContext = new
                        CustomLogViewModel
                        {
                            Name = LogViewModel.Name,
                            CounterValue = LogViewModel.CounterValue,
                            CounterName = LogViewModel.CounterName
                        }
                }
            });

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