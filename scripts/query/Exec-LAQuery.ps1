<#

.SYNOPSIS

Execute query against Log Analytics

.DESCRIPTION

Execute query against Log Analytics
 
.EXAMPLE

PS > Exec-LAQuery -QueryName "Get-VMs" -LogAnalyticsWorkspaceName "law-monitoring" -ResourceGroupName "monitoring-rg"

get back results from query which is stored in file queries\Get-VMs.query from log analytics workspace law-monitoring in resource group monitoring-rg
 
. LINK

http://github.com/vrhovnik
 
#>
param(
    [Parameter(Mandatory = $true, HelpMessage = "Name of the file with query inside the Queries folder")]
    $QueryName,   
    [Parameter(Mandatory = $false, HelpMessage = "Log Analytics workspace name")]
    $LogAnalyticsWorkspaceName = "law-monitoring",
    [Parameter(Mandatory = $false, HelpMessage = "name of the monitoring resource group")]
    $ResourceGroupName="monitoring-rg"
)

Write-Verbose "Query name: $QueryName"

$file = "queries\$QueryName.query"
Write-Verbose "File: $file"
if (-not (Test-Path $file))
{
    Write-Error "File $file does not exist, check the name of the parameter."
    return
}

$query = Get-Content -Path $file -Raw
Write-Output "Query: $query"

if ($LogAnalyticsWorkspaceName -eq "")
{
    Write-Error "Log Analytics workspace name is not set."
    return
}

if ($ResourceGroupName -eq "")
{
    Write-Error "Resource group name is not set."
    return
}

$Workspace = Get-AzOperationalInsightsWorkspace -ResourceGroupName $ResourceGroupName -Name $WorkspaceName
Write-Verbose "Getting workspace $WorkspaceName in resource group $ResourceGroupName"
$QueryResults = Invoke-AzOperationalInsightsQuery -Workspace $Workspace -Query $query | Select-Object -ExpandProperty Results
Write-Output "Query results: "
$QueryResults
