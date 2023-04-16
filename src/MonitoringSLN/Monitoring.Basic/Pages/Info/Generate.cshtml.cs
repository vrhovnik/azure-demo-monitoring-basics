using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Monitoring.Basic.Pages;

public class GeneratePageModel : PageModel
{
    private readonly ILogger<GeneratePageModel> logger;

    public GeneratePageModel(ILogger<GeneratePageModel> logger)
    {
        this.logger = logger;
    }

    public async Task OnGetAsync()
    {
        var random = new Random(100);
        var range = random.Next(10, 200);
        for (var currentNumber = 0; currentNumber < range; currentNumber++)
        {
            logger.LogInformation("Writing some logs to stream - current log {Number}", currentNumber);
            await Task.Delay(1000);
        }
        logger.LogInformation("Finished!");
    }
}