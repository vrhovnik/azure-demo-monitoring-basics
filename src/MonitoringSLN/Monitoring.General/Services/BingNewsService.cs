﻿using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using Dapper;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Options;
using Monitoring.General.Options;
using Monitoring.Interfaces;
using Monitoring.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Monitoring.General.Services;

public class BingNewsService : INewsService
{
    private readonly HttpClient client;
    private readonly ILogger<BingNewsService> logger;
    private readonly TelemetryClient telemetryClient;
    private readonly BingServiceOptions bingServiceOptions;
    private readonly SqlOptions sqlOptions;

    public BingNewsService(HttpClient client,
        ILogger<BingNewsService> logger,
        TelemetryClient telemetryClient,
        IOptions<BingServiceOptions> bingServiceOptionsValue,
        IOptions<SqlOptions> sqlOptionsValue)
    {
        this.client = client;
        this.logger = logger;
        this.telemetryClient = telemetryClient;
        bingServiceOptions = bingServiceOptionsValue.Value;
        sqlOptions = sqlOptionsValue.Value;
        this.telemetryClient.Context.GlobalProperties["Service"] = "General";
    }

    public async Task<List<NewsModel>> GetNewsAsync(string searchQuery, Languages language)
    {
        logger.LogInformation("Calling News Service with key {NewsKey}", bingServiceOptions.NewsKey);
        telemetryClient.Context.Operation.Name = "GetNewsAsync";
        var timer = Stopwatch.StartNew();
        var list = new List<NewsModel>();
        var startTime = DateTime.UtcNow;
        try
        {
            timer.Start();

            var newsEndpoint =
                $"https://api.bing.microsoft.com/v7.0/search?q={Uri.EscapeDataString(searchQuery)}&count=10&mkt=en-US";
            using var requestNews = new HttpRequestMessage();
            requestNews.Headers.Add("Ocp-Apim-Subscription-Key", bingServiceOptions.NewsKey);
            requestNews.RequestUri = new Uri(newsEndpoint);

            // Establish an operation context and associated telemetry item:
            var newsResult = string.Empty;
            using var operation = telemetryClient.StartOperation<RequestTelemetry>("GetBingNews");
            {
                telemetryClient.TrackTrace("Sending data");

                var response = await client.SendAsync(requestNews).ConfigureAwait(false);
                newsResult = await response.Content.ReadAsStringAsync();

                operation.Telemetry.ResponseCode = "200";
                telemetryClient.StopOperation(operation);
            }

            logger.LogInformation("Checking results");

            if (string.IsNullOrEmpty(newsResult))
            {
                logger.LogInformation("No data has been returned, check query {Query}", searchQuery);
                return list;
            }

            logger.LogInformation("Data received {Data}, preparing data", newsResult);
            var searchResponse =
                JsonConvert.DeserializeObject<Dictionary<string, object>>(newsResult);

            var articles = searchResponse["news"] as JToken;
            var bingNewsReponse = articles.ToObject<BingNewsResponse>();
            if (bingNewsReponse == null)
            {
                logger.LogTrace("Data was not returned, check values and keys");
                return list;
            }

            foreach (var currentArticle in bingNewsReponse.Results)
            {
                if (!DateTime.TryParse(currentArticle.DatePublished, out var datePublished))
                    datePublished = DateTime.Now;

                var newsModel = new NewsModel
                {
                    Title = currentArticle.Name,
                    Url = currentArticle.Url,
                    Content = currentArticle.Description,
                    DatePublished = datePublished
                };
                list.Add(newsModel);
            }

            logger.LogInformation("Data prepared, checking language");
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            var properties = new Dictionary<string, string>
            {
                ["Method"] = "GetNewsAsync"
            };
            telemetryClient.TrackException(e, properties);
        }
        finally
        {
            timer.Stop();
            telemetryClient.TrackDependency("Microsoft.Bing.News", "GetNews", "BingNews", list.Count.ToString(),
                startTime, timer.Elapsed, "200", true);
        }

        async Task<string> TranslateTextAsync(string textToTranslate, Languages toLang)
        {
            //do the translation
            var endpoint = "https://api.cognitive.microsofttranslator.com";
            var langId = toLang == Languages.Slovenian ? "sl" : "pl";
            var route = $"/translate?api-version=3.0&from=en&to={langId}";

            var body = new object[] { new { Text = textToTranslate } };
            var requestBody = JsonConvert.SerializeObject(body);

            using var request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri(endpoint + route);
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            request.Headers.Add("Ocp-Apim-Subscription-Key", bingServiceOptions.TextTranslationKey);
            request.Headers.Add("Ocp-Apim-Subscription-Region", bingServiceOptions.Region);

            // Send the request and get response.
            var response = await client.SendAsync(request).ConfigureAwait(false);
            var result = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(result))
            {
                logger.LogInformation("No translation was returned");
                return "";
            }

            var bingTranslateResponse = JsonConvert.DeserializeObject<BingTranslateResponse[]>(result);
            return bingTranslateResponse[0].Translations.Length > 0
                ? bingTranslateResponse[0].Translations[0].TranslatedText
                : string.Empty;
        }

        if (language == Languages.English)
        {
            logger.LogInformation("Default language english, returning data");
            return list;
        }

        startTime = DateTime.UtcNow;
        timer.Start();
        try
        {
            logger.LogInformation("Requested languaged {Language}, translating..", language.ToString());
            foreach (var newsModel in list)
            {
                newsModel.Title = await TranslateTextAsync(newsModel.Title, language);
                newsModel.Content = await TranslateTextAsync(newsModel.Content, language);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            telemetryClient.TrackException(e);
        }
        finally
        {
            timer.Stop();
            telemetryClient.TrackDependency("Microsoft.Bing.Translator", "GetNews", "BingTranslator",
                list.Count.ToString(),
                startTime, timer.Elapsed, "200", true);
        }

        logger.LogInformation("Translation done, returning data");
        return list;
    }

    public async Task<bool> SaveToFavoritesAsync(NewsModel model)
    {
        logger.LogInformation("Connecting to sql and inserting value");
        logger.LogInformation("{Title} and date published {DatePublished}", model.Title, model.DatePublished);
        await using var connection = new SqlConnection(sqlOptions.ConnectionString);
        model.Id = Guid.NewGuid().ToString();
        try
        {
            await connection.ExecuteAsync(
                "INSERT INTO News(NewsId,Title,Content,Url,DatePublished)VALUES" +
                "(@newsId,@title,@content,@url,@pubDate);",
                new
                {
                    newsId = model.Id,
                    title = model.Title,
                    content = model.Content,
                    url = model.Url,
                    pubDate = model.DatePublished
                });
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            var properties = new Dictionary<string, string>
            {
                ["Method"] = "SaveToFavoritesAsync"
            };
            telemetryClient.TrackException(e,properties);
        }
        logger.LogInformation("Value inserted, returning to caller");
        return true;
    }

    public async Task<List<NewsModel>> GetFavoritesAsync()
    {
        logger.LogInformation("Connecting to sql and getting data");
        await using var connection = new SqlConnection(sqlOptions.ConnectionString);
        var sqlQuery = "SELECT T.NewsId,T.Title,T.Content,T.Url,T.DatePublished FROM News T ";
        var grid = await connection.QueryAsync<NewsModel>(sqlQuery);
        logger.LogInformation("Returned {NumberOfItems}", grid.Count());
        return grid.ToList();
    }
}