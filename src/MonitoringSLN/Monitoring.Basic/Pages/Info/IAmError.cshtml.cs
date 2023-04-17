using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Monitoring.Basic.Pages;

public class IAmErrorPageModel : PageModel
{
    private readonly ILogger<IAmErrorPageModel> logger;

    public IAmErrorPageModel(ILogger<IAmErrorPageModel> logger) => this.logger = logger;

    public void OnGet()
    {
        logger.LogWarning("Loaded error page at {DateLoaded} and I will throw an exception");
        throw new Exception("Loaded base and thrown an exception");
    }
}