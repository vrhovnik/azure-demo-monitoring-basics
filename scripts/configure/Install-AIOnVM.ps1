<#

.SYNOPSIS

install or update Application Insights extension on VMin resource group

.EXAMPLE

PS > Install-AIOnVM -Location "WestEurope" -ResourceGroup "monitoring-rg" -VmName "vm1" -ConnectionString "InstrumentationKey=xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
install or update Application Insights extension on VM vm1 in resource group monitoring-rg

.LINK

https://github.com/vrhovnik
 
#>
param(
    [Parameter(Mandatory = $true, HelpMessage = "The resource group name")]
    [string]
    $Location = "WestEurope",
    [Parameter(Mandatory = $true, helpMessage = "The name of the resource group")]
    [string]$ResourceGroup="monitoring-rg",
    [Parameter(Mandatory = $true, HelpMessage = "Name of the VM to install the extension on")]
    [string]
    $VmName,    
    [Parameter(Mandatory = $true, HelpMessage = "The connection string to the Application Insights resource"
    $ConnectionString    
)

Write-Ouput "Adding Application Insights extension to VM $VMName in resource group $ResourceGroup"

$publicCfgJsonString = @"
{
    "redfieldConfiguration": {
        "instrumentationKeyMap": {
        "filters": [
            {
            "appFilter": ".*",
            "machineFilter": ".*",
            "virtualPathFilter": ".*",
            "instrumentationSettings" : {
                "connectionString": "$ConnectionString"
            }
            }
        ]
        }
    }
    }
"@

Write-Verbose "Public config: $publicCfgJsonString"

$privateCfgJsonString = '{}'

Set-AzVMExtension -ResourceGroupName $ResourceGroup -VMName $VMName -Location $Location -Name "ApplicationMonitoringWindows" -Publisher "Microsoft.Azure.Diagnostics" -Type "ApplicationMonitoringWindows" -Version "2.8" -SettingString $publicCfgJsonString -ProtectedSettingString $privateCfgJsonString

Write-Output "Application Insights extension added to VM $VMName in resource group $ResourceGroup"
Write-Output "Check logs in folder C:\WindowsAzure\Logs\Plugins\Microsoft.Azure.Diagnostics.ApplicationMonitoringWindows\"