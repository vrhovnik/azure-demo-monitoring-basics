<#

 .SYNOPSIS

Sends an availability test result to Application Insights.

.DESCRIPTION

Availability test results are the ideal telemetry type to test with. The reason is that the ingestion pipeline never samples out availability test results. If you send a request telemetry record, it could get sampled out when you have enabled ingestion sampling.
 
.EXAMPLE

PS > Send-AIAvailability.ps1 -ConnectionString "InstrumentationKey=00000000-0000-0000-0000-000000000000;IngestionEndpoint=https://dc.services.visualstudio.com/"

Sends an availability test result to Application Insights using the connection string.
 
. LINK

http://github.com/vrhovnik
 
#>
param(
    [Parameter(Mandatory = $false, HelpMessage = "Connection string to Application Insights")]
    $ConnectionString = "",
    [Parameter(Mandatory = $false, HelpMessage = "Instrumentation key to Application Insights")]
    $InstrumentationKey = ""
)

function ParseConnectionString
{
    param ([string]$ConnectionString)
    $Map = @{
    }
    foreach ($Part in $ConnectionString.Split(";"))
    {
        $KeyValue = $Part.Split("=")
        $Map.Add($KeyValue[0], $KeyValue[1])
    }
    return $Map
}
# If ikey is the only parameter supplied, we'll send telemetry to the global ingestion endpoint instead of regional endpoint found in connection strings
If (($InstrumentationKey) -and ("" -eq $ConnectionString))
{
    $ConnectionString = "InstrumentationKey=$InstrumentationKey;IngestionEndpoint=https://dc.services.visualstudio.com/"
}
$map = ParseConnectionString($ConnectionString)
$url = $map["IngestionEndpoint"] + "v2/track"
Write-Output "Sending telemetry to $url"
$ikey = $map["InstrumentationKey"]
Write-Verbose "Instrumentation key: $ikey"
$lmUrl = $map["LiveEndpoint"]
Write-Verbose "Live metrics endpoint: $lmUrl"
$time = (Get-Date).ToUniversalTime().ToString("o")
Write-Verbose "Time: $time"

$requestData = @"
{
   "data": {
      "baseType": "RequestData",
      "baseData": {
        "ver": 2,
        "id": "22093920382029384",
        "name": "GET /msftsupport/requestdata/",
        "starttime": "$time",
        "duration": "00:00:01.0000000",
        "success": true,
        "responseCode": "200",
        "url": "https://localhost:8080/requestData/sampleurl",
        "httpMethod": "GET"
       }
   },
   "ver": 1,
   "iKey": "$ikey",
   "name": "Microsoft.ApplicationInsights.Request",
   "time": "$time",
   "sampleRate": 100,
   "flags": 0
}
"@

Write-Verbose "Request data: $requestData"

# Uncomment one or more of the following lines to test client TLS/SSL protocols other than the machine default option
# [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::SSL3
# [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::TLS
# [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::TLS11
# [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::TLS12
# [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::TLS13
$ProgressPreference = "SilentlyContinue"
Write-Verbose "Sending request telemetry..."
Invoke-WebRequest -Uri $url -Method POST -Body $requestData -UseBasicParsing
Write-Verbose "Request telemetry sent."