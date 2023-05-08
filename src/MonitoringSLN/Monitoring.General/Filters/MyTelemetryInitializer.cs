using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Monitoring.General.Filters;

public class MyTelemetryInitializer : ITelemetryInitializer
{
    public void Initialize(ITelemetry telemetry)
    {
        if (string.IsNullOrEmpty(telemetry.Context.Cloud.RoleName))
        {
            telemetry.Context.Cloud.RoleName = "OneMonitoringToRuleThemAll";
        }
        // var requestTelemetry = telemetry as RequestTelemetry;
        // // Is this a TrackRequest() ?
        // if (requestTelemetry == null) return;
        // int code;
        // bool parsed = Int32.TryParse(requestTelemetry.ResponseCode, out code);
        // if (!parsed) return;
        // if (code >= 400 && code < 500)
        // {
        //     // If we set the Success property, the SDK won't change it:
        //     requestTelemetry.Success = true;
        //
        //     // Allow us to filter these requests in the portal:
        //     requestTelemetry.Properties["Overridden400s"] = "true";
        // }
        
    }
}