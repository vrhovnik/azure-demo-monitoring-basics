using Newtonsoft.Json;

namespace Monitoring.Models;

public class BingTranslateResponse
{
    [JsonProperty(PropertyName = "translations")]
    public Translations[] Translations { get; set; }
}

public class Translations
{
    [JsonProperty(PropertyName = "text")]
    public string TranslatedText { get; set; }
    public string to { get; set; }
}

public class BingNewsResponse
{
    public string id { get; set; }
    public string readLink { get; set; }
    [JsonProperty(PropertyName = "value")] public NewsObjectValue[] Results { get; set; }
}

public class NewsObjectValue
{
    public ContractualRules[] contractualRules { get; set; }
    [JsonProperty(PropertyName = "name")] public string Name { get; set; }
    [JsonProperty(PropertyName = "url")] public string Url { get; set; }
    public Image image { get; set; }

    [JsonProperty(PropertyName = "description")]
    public string Description { get; set; }

    public About[] about { get; set; }
    public Mentions[] mentions { get; set; }
    public Provider[] provider { get; set; }

    [JsonProperty(PropertyName = "datePublished")]
    public string DatePublished { get; set; }

    public string category { get; set; }
}

public class ContractualRules
{
    public string _type { get; set; }
    public string text { get; set; }
}

public class Image
{
    public string contentUrl { get; set; }
    public Thumbnail thumbnail { get; set; }
}

public class Thumbnail
{
    public string contentUrl { get; set; }
    public int width { get; set; }
    public int height { get; set; }
}

public class About
{
    public string readLink { get; set; }
    public string name { get; set; }
}

public class Mentions
{
    public string name { get; set; }
}

public class Provider
{
    public string _type { get; set; }
    public string name { get; set; }
    public Image1 image { get; set; }
}

public class Image1
{
    public Thumbnail1 thumbnail { get; set; }
}

public class Thumbnail1
{
    public string contentUrl { get; set; }
}