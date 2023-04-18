<#

# .SYNOPSIS

creates custom table in Log Analytics workspace

.DESCRIPTION
 
Creates custom table in Log Analytics workspace
  
.EXAMPLE

PS > .\Create-CustomTable.ps1 -ResourceGroupName "WestEurope" -TableName "MyTable" -WorkspaceName "MyWorkspace"
create custom table in Log Analytics workspace
 
. LINK

https://learn.microsoft.com/en-us/azure/azure-monitor/agents/data-collection-text-log?tabs=portal
 
#>
param(
    [Parameter(Mandatory = $true,HelpMessage  = "Resource group name")]
    $ResourceGroupName = "WestEurope",
    [Parameter(Mandatory = $true,HelpMessage  = "Custom table name (without CL)")]
    $TableName,
    [Parameter(Mandatory = $true,HelpMessage  = "Workspace name")]
    $WorkspaceName
)

$tableParams = @'
{
   "properties": {
       "schema": {
              "name": "#TABLE#_CL",
              "columns": [
       {
                               "name": "TimeGenerated",
                               "type": "DateTime"
                       }, 
                      {
                               "name": "RawData",
                               "type": "String"
                      }
             ]
       }
   }
}
'@

$tableParams = $tableParams.Replace("#TABLE#", $TableName)
Write-Verbose "Table params: $tableParams"
$context = Get-AzContext
$subscription = $context.Subscription.Name
Write-Verbose "Subscription: $subscription in resource group $ResourceGroupName"
$path="/subscriptions/$subscription/resourcegroups/$ResourceGroupName/providers/microsoft.operationalinsights/workspaces/$WorkspaceName/tables/#TABLENAME#_CL?api-version=2021-12-01-preview"
$path = $path.Replace("#TABLENAME#", $TableName)
Write-Verbose "Path: $path"
Invoke-AzRestMethod -Path  -Method PUT -payload $tableParams
Write-Output "$TableName in $WorkspaceName in group $ResourceGroupName has been created"