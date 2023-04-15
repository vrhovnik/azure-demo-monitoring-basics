﻿<#

# .SYNOPSIS

Install all required resources for Azure Demo Monitoring Basics from scratch.

.DESCRIPTION
 
This script will install all required resources for Azure Demo Monitoring Basics from scratch
with the use of Bicep templates. It will also install Bicep and Azure CLI if selected and get environment 
variables from local.env file. It will compile containers and push them to Azure Container Registry. 
It will configure AKS cluster to use Azure Container Registry, deploy VM with script to install all 
software on VM to be ready for deployment.  
  
.EXAMPLE

PS > Init-Environment -Location "WestEurope" -InstallModules -InstallBicep -UseEnvFile
install modules, bicep and deploy resources to West Europe

.EXAMPLE

PS > Init-Environment  -Location "WestEurope" 
install all required resources to West Europe
 
. LINK

http://github.com/vrhovnik
 
#>
param(
    [Parameter(Mandatory = $false)]
    $Location = "WestEurope",
    [Parameter(Mandatory = $false)]
    $EnvFileToReadFrom = "local.env",
    [Parameter(Mandatory = $false)]
    [switch]$InstallModules,
    [Parameter(Mandatory = $false)]
    [switch]$InstallBicep,
    [Parameter(Mandatory = $false)]
    [switch]$InstallAzCli,
    [Parameter(Mandatory = $false)]
    [switch]$UseEnvFile
)

$ProgressPreference = 'SilentlyContinue';
Start-Transcript -Path "$HOME/Downloads/bootstrapper.log" -Force
# Write-Output "Sign in to Azure account." 
# login to Azure account
# Connect-AzAccount

if ($InstallModules)
{
    Write-Output "Install Az module and register providers."
    #install Az module
    Install-Module -Name Az -Scope CurrentUser -Repository PSGallery -Force
    Install-Module -Name Az.App

    #register providers
    Register-AzResourceProvider -ProviderNamespace Microsoft.App
    # add support for log analytics
    Register-AzResourceProvider -ProviderNamespace Microsoft.OperationalInsights
    Write-Output "Modules installed and registered, continuing to Azure deployment nad if selected, Bicep install."
}

if ($InstallBicep)
{
    # install bicep
    Write-Output "Installing Bicep."
    # & Install-Bicep.ps1
    Start-Process powershell.exe -FilePath Install-Bicep.ps1 -NoNewWindow -Wait
    Write-Output "Bicep installed, continuing to Azure deployment."
}

if ($InstallAzCli)
{
    Write-Output "Installing Azure CLI."

    Invoke-WebRequest -Uri https://aka.ms/installazurecliwindows -OutFile .\AzureCLI.msi;
    Start-Process msiexec.exe -Wait -ArgumentList '/I AzureCLI.msi /quiet';
    ## cleanup of Azure CLI
    Remove-Item .\AzureCLI.msi
    Write-Output "Az CLI installed, continuing to Azure deployment."
}

if ($UseEnvFile)
{
    Get-Content $EnvFileToReadFrom | ForEach-Object {
        $name, $value = $_.split('=')
        Set-Content env:\$name $value
        Write-Information "Writing $name to environment variable with $value."
    }
}

# create resource group if it doesn't exist with bicep file stored in bicep folder
$groupNameExport = New-AzSubscriptionDeployment -Location $Location -TemplateFile "bicep\rg.bicep" -TemplateParameterFile "bicep\rg.parameters.json" -Verbose
Write-Information $groupNameExport
$groupName = $groupNameExport.Outputs.rgName.Value
Write-Verbose "The resource group name is $groupName"

# deploy log analytics file if not already deployed
New-AzResourceGroupDeployment -ResourceGroupName $groupName -TemplateFile "bicep\log-analytics.bicep" -TemplateParameterFile "bicep\log-analytics.parameters.json" -Verbose
# deploy azure storage file if not already deployed
New-AzResourceGroupDeployment -ResourceGroupName $groupName -TemplateFile "bicep\storage.bicep" -TemplateParameterFile "bicep\storage.parameters.json" -Verbose
# deploy azure registry file if not already deployed
New-AzResourceGroupDeployment -ResourceGroupName $groupName -TemplateFile "bicep\registry.bicep" -TemplateParameterFile "bicep\registry.parameters.json" -Verbose
Write-Output "Compiling the containers for registry "
Write-Verbose "Getting name of the registry"
$registry = Get-Content "bicep\registry.parameters.json" | ConvertFrom-Json
$registryName = $registry.parameters.acrName.value
Write-Verbose "Registry name is $registryName"
#compile containers
$TagName = "latest"
Write-Verbose "Compile containers with latest tag"
Get-ChildItem -Path "containers" | ForEach-Object {
    $imageName = $_.Name.Split('-')[1]
    $dockerFile = $_.FullName
    Write-Output "Building image $imageName with tag $TagName based on $dockerFile"
    $imageNameWithTag = "$( $imageName ):$TagName"
    Write-Output "Taging image with $imageNameWithTag"
    Write-Information "Call data with AZ cli as we don't have support in Azure PowerShell for this yet."
    az acr build --registry $registryName --image $imageNameWithTag -f $dockerFile $SourceFolder
}

New-AzResourceGroupDeployment -ResourceGroupName $groupName -TemplateFile "bicep\aks.bicep" -TemplateParameterFile "bicep\aks.parameters.json" -Verbose
$aks = Get-Content "bicep\aks.parameters.json" | ConvertFrom-Json
$aksName = $aks.parameters.aksClusterName.value
Write-Output "Applying registry $registryName to cluster $aksName"
Set-AzAksCluster -Name $aksName -ResourceGroupName $groupName -AcrNameToAttach $registryName
# get azure cluster credentials
Import-AzAksCredential -ResourceGroupName $groupName -Name $aksName -Verbose
Write-Output "Credentials to access cluster $aksName in resource group $groupName are imported. Deploying VM."
# deploy azure VM
New-AzResourceGroupDeployment -ResourceGroupName $groupName -TemplateFile "bicep\vm.bicep" -TemplateParameterFile "bicep\vm.parameters.json" -Verbose
#deploy load testing
New-AzResourceGroupDeployment -ResourceGroupName $groupName -TemplateFile "bicep\load-testing.bicep" -TemplateParameterFile "bicep\load-testing.parameters.json" -Verbose

Stop-Transcript

# open file for viewing
Start-Process notepad.exe -ArgumentList "$HOME/Downloads/bootstrapper.log"
