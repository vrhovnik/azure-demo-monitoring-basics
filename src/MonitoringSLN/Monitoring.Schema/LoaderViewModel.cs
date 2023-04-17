namespace Monitoring.Schema;

public class LoaderViewModel
{
    public DateTimeOffset MyTime { get; set; }
    public string Computer { get; set; }
    public CustomLogViewModel AdditionalContext { get; set; }
}