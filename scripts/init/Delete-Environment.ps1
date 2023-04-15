<#

# .SYNOPSIS

Deletes resource group and all associated resources inside Azure subscription.

.DESCRIPTION
 
Deletes resource group and all associated resources inside Azure subscription.
  
.EXAMPLE

PS > Delete-Environment -ResourceGroupName "monitoring-rg" 
deletes resource group and all resources inside
 
. LINK

http://github.com/vrhovnik
 
#>
param(
    [Parameter(Mandatory = $true,HelpMessage  = "Location to delete resources")]
    $ResourceGroupName = "WestEurope"
)

Write-Verbose "Delete resource group $ResourceGroupName"
Remove-AzResourceGroup -Name $ResourceGroupName -Force
Write-Output "Resource group $ResourceGroupName deleted."