using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Monitoring.Interfaces;
using Monitoring.Models;

namespace Monitoring.General.Pages.Instrumentation;

public class NewsPageModel : PageModel
{
    private readonly ILogger<NewsPageModel> logger;
    private readonly INewsService newsService;

    public NewsPageModel(ILogger<NewsPageModel> logger, INewsService newsService)
    {
        this.logger = logger;
        this.newsService = newsService;
    }

    public async Task OnGetAsync()
    {
        logger.LogInformation("Loaded page at {DateLoaded} with param {Query}", DateTime.Now, Query);
        if (!string.IsNullOrEmpty(Query))
        {
            logger.LogInformation("Searching through Bing News");
            var form = Request.Query;
            var language = Languages.English;
            if (form.ContainsKey("ddlLanguages"))
            {
                var value = form["ddlLanguages"];
                if (value == "en") language = Languages.English;
                if (value == "sl") language = Languages.Slovenian;
                if (value == "pl") language = Languages.Polish;
            }

            var newsModels = await newsService.GetNewsAsync(Query, language);
            News = newsModels;
            logger.LogInformation("Search finished, received {NumberOfItems} items with topic {Query}",
                newsModels.Count, Query);
        }
    }

    public async Task<RedirectToPageResult> OnPostAsync()
    {
        logger.LogInformation("Adding post to favorites and starting to read values.");
        var form = await Request.ReadFormAsync();
        var title = form["title"];
        var url = form["url"];
        var content = form["content"];
        var datePublished = form["datePublished"];
        logger.LogInformation("Title {Title}, Content {Content}, Date Published {DatePublished}, Url {url}",
            title, content, DateTime.Parse(datePublished), url);
        await newsService.SaveToFavoritesAsync(new NewsModel
        {
            Content = content,
            Title = title,
            Url = url,
            DatePublished = DateTime.Parse(datePublished)
        });
        return RedirectToPage("/Info/Favorites");
    }

    public List<NewsModel> News { get; set; } = new();
    [BindProperty(SupportsGet = true)] public string Query { get; set; }
}