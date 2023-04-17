using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Monitoring.Basic.Settings;

namespace Monitoring.Basic.Pages;

public class GeneratePageModel : PageModel
{
    private readonly ILogger<GeneratePageModel> logger;
    private AppSettings appSettings;

    public GeneratePageModel(ILogger<GeneratePageModel> logger, IOptions<AppSettings> appSettingsValue)
    {
        this.logger = logger;
        appSettings = appSettingsValue.Value;
    }

    public async Task OnGetAsync()
    {
        var stopWatch = Stopwatch.StartNew();
        logger.LogInformation("Page Generate loaded at {DateLoaded}", DateTime.Now);
        var random = new Random(100);
        var range = random.Next(10, appSettings.RecordNumber);
        for (var currentNumber = 0; currentNumber < range; currentNumber++)
        {
            if (currentNumber % 2 == 0)
                logger.LogInformation("Writing some logs to stream - current log {Number}", currentNumber);
            else if (currentNumber % 3 == 0)
                logger.LogTrace("Writing some traces to stream - current log {Number}", currentNumber);
            else
                logger.LogError("Writing some errors to stream - current log {Number}", currentNumber);
        }
        logger.LogInformation("Finished at {DateFinished}!", DateTime.Now);
        stopWatch.Stop();
        Message = $"Loaded {appSettings.RecordNumber} log information and it took {stopWatch.ElapsedMilliseconds} ms ({stopWatch.Elapsed.Seconds} s).";
    }

    [BindProperty]
    public string Message { get; set; }
}