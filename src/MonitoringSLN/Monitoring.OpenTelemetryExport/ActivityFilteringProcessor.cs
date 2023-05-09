using System.Diagnostics;
using OpenTelemetry;

namespace Monitoring.OpenTelemetryExport;

public class ActivityFilteringProcessor : BaseProcessor<Activity>
{
    public override void OnStart(Activity activity)
    {
        // prevents all exporters from exporting internal activities
        if (activity.Kind == ActivityKind.Internal)
        {
            activity.IsAllDataRequested = false;
        }
    }
}