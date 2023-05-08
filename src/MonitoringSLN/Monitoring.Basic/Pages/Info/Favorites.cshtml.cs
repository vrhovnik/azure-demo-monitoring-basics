using Microsoft.AspNetCore.Mvc.RazorPages;
using Monitoring.Interfaces;
using Monitoring.Models;

namespace Monitoring.Basic.Pages;

public class FavoritesPageModel : PageModel
{
    private readonly ILogger<FavoritesPageModel> logger;
    private readonly INewsService newsService;

    public FavoritesPageModel(ILogger<FavoritesPageModel> logger, INewsService newsService)
    {
        this.logger = logger;
        this.newsService = newsService;
    }

    public async Task OnGetAsync()
    {
        logger.LogInformation("Loading favorites page at {DateLoaded}", DateTime.Now);
        News = await newsService.GetFavoritesAsync();
        logger.LogInformation("Loaded {NumberOfItems}", News.Count);
    }

    public List<NewsModel> News { get; set; } = new();
}