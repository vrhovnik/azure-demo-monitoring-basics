<#

 .SYNOPSIS

Test endpoint with open telemetry exporter via Application Insights

.DESCRIPTION

Test endpoint with open telemetry exporter via Application Insights
 
.EXAMPLE

PS > Test-OpenTelemetry.ps1 -Url "https://site.com/api/telemetry"

Test telementry with 200 records to the endpoint

.EXAMPLE

PS > Test-OpenTelemetry.ps1 -Url "https://site.com/api/telemetry" -RecordNumber 1000

Test telementry with 1000 records to the endpoint
   
. LINK

http://github.com/vrhovnik
 
#>

param(
    [Parameter(Mandatory = $true, HelpMessage = "URL to test application open telemetry exporter")]
    $Url,
    [Parameter(Mandatory = $false, HelpMessage = "Number of records")]    
    $RecordNumber = 200 
)

Write-Information "Testing $Url and sending $RecordNumber records to the endpoint"
for ($currentRequestNumber = 0; $currentRequestNumber -lt $array.Count; $currentRequestNumber++) {
    $body = (@{
        username = "Foo $recordNumber"
        password = "Bar $recordNumber"
    } | ConvertTo-Json)
    Write-Information "Sending $body"
    Invoke-WebRequest $Url -Method Post -ContentType 'application/json' -Body $body    
}

Write-Information "Done with sending $RecordNumber records to the endpoint $Url"  