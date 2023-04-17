using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Monitoring.Basic.Pages;

public class ThrowExceptionPageModel : PageModel
{
    private readonly ILogger<ThrowExceptionPageModel> logger;

    public ThrowExceptionPageModel(ILogger<ThrowExceptionPageModel> logger) => this.logger = logger;

    public void OnGet()
    {
        logger.LogInformation("Page loaded {DateLoaded}. Throwing an exception", DateTime.Now);
        throw new Exception($"Throwing an exception at {DateTime.Now}");
    }
}