<# 
# SYNOPSIS
# PowerShell script to setup web app and other tools to be installed on VM
#
# DESCRIPTION
# installs all neccessary file to be installed on VM with getting back address to be able to access it
#
# NOTES
# Author      : Bojan Vrhovnik
# GitHub      : https://github.com/vrhovnik
# Version 0.2.6
# SHORT CHANGE DESCRIPTION: added transcript to be able to see installation log, if anything wrong
#>

Set-StrictMode -Version Latest
$ErrorActionPreference="Stop"
$ProgressPreference="SilentlyContinue"

$registryPath = @{
    Path = 'HKLM:\Software\Policies\Microsoft\Windows\PowerShell\Transcription'
    Force = $True
}
New-Item @registryPath

$dwordOne = @{
    PropertyType = 'DWord'
    Value = 1
}
New-ItemProperty @registryPath -Name 'EnableTranscripting' @dwordOne
New-ItemProperty @registryPath -Name 'EnableInvocationHeader' @dwordOne
New-ItemProperty @registryPath -Name 'OutputDirectory' -PropertyType 'String' -Value 'C:\Temp'

Start-Transcript

#Write-Host "Install SQL express engine"
#Invoke-WebRequest "https://go.microsoft.com/fwlink/?LinkID=866658" -o "$PWD\sqlsetup.exe"
#
#$args = New-Object -TypeName System.Collections.Generic.List[System.String]
#$args.Add("/ACTION=install")
#$args.Add("/Q")
#$args.Add("/IACCEPTSQLSERVERLICENSETERMS")
#
#Write-Host "Installing SQL Express silently..."
#Start-Process -FilePath "$PWD\sqlsetup.exe" -ArgumentList $args -NoNewWindow -Wait -PassThru

# installing chocolatey to install additional services 
Write-Host "Installing chocolatey"

Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072;
Invoke-Expression ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))

Write-Host "Installing  dotnet SDK ... "
choco install -y dotnet-sdk
choco install -y dotnet-sdk --version=6.0.408 --side-by-side

Write-Host "Installing Git ... "
choco install -y git

Write-Host "Installing Azure CLI"
choco install -y azure-cli

Write-Host "Installing PowerShell AZ module"
choco install -y az.powershell -Force

Write-Host "Installing Sysinternals ZoomIt"
choco install -y sysinternals

# enable IIS
Write-Host "Continue with enabling IIS on the machine"
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServer
Enable-WindowsOptionalFeature -Online -FeatureName IIS-CommonHttpFeatures
Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpErrors
Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpRedirect
Enable-WindowsOptionalFeature -Online -FeatureName IIS-ApplicationDevelopment
Enable-WindowsOptionalFeature -online -FeatureName NetFx4Extended-ASPNET45
Enable-WindowsOptionalFeature -Online -FeatureName IIS-NetFxExtensibility45
Enable-WindowsOptionalFeature -Online -FeatureName IIS-HealthAndDiagnostics
Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpLogging
Enable-WindowsOptionalFeature -Online -FeatureName IIS-LoggingLibraries
Enable-WindowsOptionalFeature -Online -FeatureName IIS-RequestMonitor
Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpTracing
Enable-WindowsOptionalFeature -Online -FeatureName IIS-Security
Enable-WindowsOptionalFeature -Online -FeatureName IIS-RequestFiltering
Enable-WindowsOptionalFeature -Online -FeatureName IIS-Performance
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerManagementTools
Enable-WindowsOptionalFeature -Online -FeatureName IIS-IIS6ManagementCompatibility
Enable-WindowsOptionalFeature -Online -FeatureName IIS-Metabase
Enable-WindowsOptionalFeature -Online -FeatureName IIS-ManagementConsole
Enable-WindowsOptionalFeature -Online -FeatureName IIS-BasicAuthentication
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WindowsAuthentication
Enable-WindowsOptionalFeature -Online -FeatureName IIS-StaticContent
Enable-WindowsOptionalFeature -Online -FeatureName IIS-DefaultDocument
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebSockets
Enable-WindowsOptionalFeature -Online -FeatureName IIS-ApplicationInit
Enable-WindowsOptionalFeature -Online -FeatureName IIS-ISAPIExtensions
Enable-WindowsOptionalFeature -Online -FeatureName IIS-ISAPIFilter
Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpCompressionStatic
Enable-WindowsOptionalFeature -Online -FeatureName IIS-ASPNET45

Write-Host "Getting source code and storing it to $HOME/amaw"
$zipPath="https://codeload.github.com/vrhovnik/azure-demo-monitoring-basics/zip/refs/heads/main"
Set-Location $HOME
Invoke-WebRequest -Uri $zipPath -OutFile "$HOME\amaw.zip"
#extract to amaw folder
Expand-Archive -Path "$HOME\amaw.zip" -DestinationPath "$HOME\amaw" -Force

Write-Host "Changed location to $HOME/amaw/azure-demo-monitoring-basics-main/src/MonitoringSLN/Monitoring.Basic"
Set-Location "$HOME/amaw/azure-demo-monitoring-basics-main/src/MonitoringSLN/Monitoring.Basic"

$rootPath = "C:\Inetpub\wwwroot\"
Write-Host "Creating Folder Web and publishing solution Web to wwwroot"
New-Item -ItemType Directory "$rootPath\Web" -Force
dotnet publish --configuration Release -o "$rootPath\Web"

Write-Host "Create web applications directories in IIS - web"
New-WebApplication -Site "Default Web Site" -Name "web" -PhysicalPath "C:\Inetpub\wwwroot\Web" -ApplicationPool "DefaultAppPool"

# ASP.NET core hosting module download
# DIRECT LINK: https://download.visualstudio.microsoft.com/download/pr/c5e0609f-1db5-4741-add0-a37e8371a714/1ad9c59b8a92aeb5d09782e686264537/dotnet-hosting-6.0.8-win.exe
# GENERAL LINK https://dotnet.microsoft.com/permalink/dotnetcore-current-windows-runtime-bundle-installer
Write-Host "Getting ASP.NET Core hosting module to support .NET Core..."
Invoke-WebRequest -Uri "https://download.visualstudio.microsoft.com/download/pr/8de163f5-5d91-4dc3-9d01-e0b031a03dd9/0170b328d569a49f6f6a080064309161/dotnet-hosting-7.0.0-win.exe" -OutFile "$PWD\hosting.exe"

Write-Host "Installing ASP.NET Core hosting"
$args = New-Object -TypeName System.Collections.Generic.List[System.String]
$args.Add("/quiet")
$args.Add("/install")
$args.Add("/norestart")

Start-Process -FilePath "$PWD\hosting.exe" -ArgumentList $args -NoNewWindow -Wait -PassThru

net stop was /y
net start w3svc

Write-Host "Restart done, app is ready on https://localhost/web"

Stop-Transcript
