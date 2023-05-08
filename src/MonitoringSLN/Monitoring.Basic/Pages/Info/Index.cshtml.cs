using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Monitoring.Basic.Pages;

public class IndexPageModel : PageModel
{
    private readonly ILogger<IndexPageModel> logger;

    public IndexPageModel(ILogger<IndexPageModel> logger)
    {
        this.logger = logger;
    }

    public void OnGet() => logger.LogInformation("Loaded page at {DateLoaded}", DateTime.Now);
}