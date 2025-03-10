using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Assessments.Web.Infrastructure;

/// <summary> 
/// Custom TelemetryInitializer that overrides the default sdk behavior of treating response code 404 (not found) as failed request
/// https://stackoverflow.com/questions/37533431/how-to-tell-application-insights-to-ignore-404-responses
/// https://learn.microsoft.com/en-us/azure/azure-monitor/app/api-filtering-sampling#addmodify-properties-itelemetryinitializer
/// </summary>
public class TelemetryInitializer : ITelemetryInitializer
{
    public void Initialize(ITelemetry telemetry)
    {
        if (telemetry is not RequestTelemetry requestTelemetry)
            return;

        // check for 404 Not Found
        if (requestTelemetry.ResponseCode is not "404")
            return;

        // if we set the success property, the sdk won't change it
        requestTelemetry.Success = true;

        // allow us to filter these requests in the portal
        requestTelemetry.Properties["Overridden404"] = "true";
    }
}