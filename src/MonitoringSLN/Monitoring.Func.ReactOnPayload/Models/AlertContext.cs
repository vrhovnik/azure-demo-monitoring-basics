namespace Monitoring.Func.ReactOnPayload;

public class AlertContext
{
    public object properties { get; set; }
    public string conditionType { get; set; }
    public Condition condition { get; set; }
}