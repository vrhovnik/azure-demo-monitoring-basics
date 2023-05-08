using Monitoring.Models;

namespace Monitoring.Interfaces;

public interface INewsService
{
    Task<List<NewsModel>> GetNewsAsync(string searchQuery, Languages language);
    Task<bool> SaveToFavoritesAsync(NewsModel model);
    Task<List<NewsModel>> GetFavoritesAsync();
}

public enum Languages
{
    English,
    Slovenian,
    Polish
}