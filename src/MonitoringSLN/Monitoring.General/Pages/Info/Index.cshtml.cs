using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Monitoring.General.Pages.Info;

public class IndexPageModel : PageModel
{
    private readonly ILogger<IndexPageModel> logger;

    public IndexPageModel(ILogger<IndexPageModel> logger)
    {
        this.logger = logger;
    }

    public void OnGet()
    {
        logger.LogInformation("Loaded info page at {DateLoaded}", DateTime.Now);
    }

    [BindProperty] public string ServerName { get; } = Environment.MachineName;
}