using Newtonsoft.Json;

namespace Monitoring.Func.ReactOnPayload;

public class AllOf
{
    public string metricName { get; set; }
    public string metricNamespace { get; set; }
    [JsonProperty("operator")]
    public string AlertOperator { get; set; }
    public string threshold { get; set; }
    public string timeAggregation { get; set; }
    public Dimensions[] dimensions { get; set; }
    public double metricValue { get; set; }
}