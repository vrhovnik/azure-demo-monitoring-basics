<#

# .SYNOPSIS

Sends test logs to Azure Monitor

.DESCRIPTION

Sends test logs to Azure Monitor
 
.EXAMPLE

PS > Send-CustomLogs.ps1 -Log "C:\temp\test.log" -Table "Test_CL" -AppId "00000000-0000-0000-0000-000000000000" -AppSecret "00000000-0000-0000-0000-000000000000" -TenantId "00000000-0000-0000-0000-000000000000" -DataCollectionUrl "https://dc.services.visualstudio.com/v2/track" -DcrImmutableId "00000000-0000-0000-0000-000000000000"

sending test.log to custom table Test_CL with appid 0000000 and secret 0000000 and tenantid 0000000 and datacollectionurl https://dc.services.visualstudio.com/v2/track and dcrimmutableid 0000000
 
. LINK

https://learn.microsoft.com/en-us/azure/azure-monitor/logs/tutorial-logs-ingestion-portal
 
#>
[cmdletbinding()]
param (
    [Parameter(HelpMessage = "Log file to be forwarded")]
    [ValidateNotNullOrEmpty()]
    [string]$Log = "C:\Users\bovrhovn\Downloads\monitoring.json",
    [Parameter(HelpMessage = "The name of the custom log table, including '_CL' suffix", Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]$Table,
    [Parameter(HelpMessage = "Service principal application id")]
    [ValidateNotNullOrEmpty()]
    [string]$AppId,
    [Parameter(HelpMessage = "Service principal secret")]
    [ValidateNotNullOrEmpty()]
    [string]$AppSecret,
    [Parameter(HelpMessage = "Service principal tenant id")]
    [ValidateNotNullOrEmpty()]
    [string]$TenantId,
    [Parameter(HelpMessage = "Data collection URL")]
    [ValidateNotNullOrEmpty()]
    [string]$DataCollectionUrl,
    [Parameter(HelpMessage = "Data collection id (ImmutableId)")]
    [ValidateNotNullOrEmpty()]
    [string]$DcrImmutableId
)

Write-Output "Logging into Azure with service principal $AppId and obtaining bearer token."
## Obtain a bearer token used to authenticate against the data collection endpoint
$scope = [System.Web.HttpUtility]::UrlEncode("https://monitor.azure.com//.default")
$body = "client_id=$AppId&scope=$scope&client_secret=$AppSecret&grant_type=client_credentials";
$headers = @{ "Content-Type" = "application/x-www-form-urlencoded" };
$uri = "https://login.microsoftonline.com/$TenantId/oauth2/v2.0/token"
$bearerToken = (Invoke-RestMethod -Uri $uri -Method "Post" -Body $body -Headers $headers).access_token
Write-Information "Token is $bearerToken"

Write-Output "Logged in, getting content to be sent to Log Analytics"

$body = Get-Content $Log

if ($body -eq "")
{
    Write-Output "No content to be sent to Log Analytics"
    return
}

Write-Information $body

Write-Output "Sending data to the endpoint $DataCollectionUrl"

$headers = @{ "Authorization" = "Bearer $bearerToken"; "Content-Type" = "application/json" };
$uri = "$DataCollectionUrl/dataCollectionRules/$DcrImmutableId/streams/Custom-$Table" + "?api-version=2021-11-01-preview";
Write-information "$uri to be called with $body"

$uploadResponse = Invoke-RestMethod -Uri $uri -Method "Post" -Body $body -Headers $headers;

Write-Verbose "Uploaded response is $uploadResponse"
Write-Output "Writing logs from $Log has finished!"