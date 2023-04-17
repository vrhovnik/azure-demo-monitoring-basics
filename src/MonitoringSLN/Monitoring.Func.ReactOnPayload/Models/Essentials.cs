namespace Monitoring.Func.ReactOnPayload;

public class Essentials
{
    public string alertId { get; set; }
    public string alertRule { get; set; }
    public string severity { get; set; }
    public string signalType { get; set; }
    public string monitorCondition { get; set; }
    public string monitoringService { get; set; }
    public string[] alertTargetIDs { get; set; }
    public string[] configurationItems { get; set; }
    public string originAlertId { get; set; }
    public string firedDateTime { get; set; }
    public string resolvedDateTime { get; set; }
    public string description { get; set; }
    public string essentialsVersion { get; set; }
    public string alertContextVersion { get; set; }
}