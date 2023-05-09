# Demos about Azure Monitoring basics

The instructions about how the [demos](https://github.com/vrhovnik/azure-demo-monitoring-basics#demos) are structured
and how to run them with step by step instructions is
available [here](https://github.com/vrhovnik/azure-demo-monitoring-basics/blob/main/README.md).

![App insights overview](https://learn.microsoft.com/en-us/azure/azure-monitor/app/media/overview-dashboard/0001-dashboard.png#lightbox)

## Pre-requisites

If you didn't prepared the environment to follow along, you can run the following script in your PWSH terminal:

```powershell
#to go root folder where you cloned the repo f.e. C:\Work\Projects\azure-demo-monitoring-basics
Set-Location "C:\Work\Projects\azure-demo-monitoring-basics\"
Add-DirToSystemEnv -RestartCurrentSession
Init-Environment -Location "WestEurope"

```

If not already connected to AKS cluster, follow
this [instructions](https://learn.microsoft.com/en-us/azure/aks/learn/quick-kubernetes-deploy-bicep?tabs=azure-powershell%2CCLI#connect-to-the-cluster).

## Application Insights

Going through [Application Insights](https://learn.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview?tabs=net)
solution with focus on practical areas:

1. Usage in depth (insights, visualization, analyze, respond)
2. Alerts
3. Integration with DevOps (CI/CD)
4. Smart Detection
5. Integration options

Focusing on defining monitoring strategies with best practices on top of Azure Cloud Adoption Framework. Check
out [this](https://docs.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/monitoring-and-management)
article for more details.

In order to define how many Application Insights resources should I deploy, check
out [this official guidance](https://learn.microsoft.com/en-us/azure/azure-monitor/app/separate-resources).

## Credits

1. [HTMX](https://htmx.org) - htmx gives you access to AJAX, CSS Transitions, WebSockets and Server Sent Events directly
   in HTML, using attributes, so you can build modern user interfaces with the simplicity and power of hypertext.
2. [Bootstrap](https://getbootstrap.com) - Bootstrap is an open source toolkit for developing with HTML, CSS, and JS.
3. [Application Insights](https://learn.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview?tabs=net) -
   extension of Azure Monitor to provide APM (Application Performance Monitoring)
4. [Access control](https://learn.microsoft.com/en-us/azure/azure-monitor/app/resources-roles-access-control#select-a-role) - how to define access control for Application Insights 

## Useful links to check out

1. [Automatically tracked dependencies](https://learn.microsoft.com/en-us/azure/azure-monitor/app/asp-net-dependencies#automatically-tracked-dependencies)
2. [Standard metrics](https://learn.microsoft.com/en-us/azure/azure-monitor/app/standard-metrics)
3. [Custom events limit](https://learn.microsoft.com/en-us/azure/azure-monitor/app/api-custom-events-metrics#limits)
4. [App insights data model](https://learn.microsoft.com/en-us/azure/azure-monitor/app/data-model-complete)
5. [Connection string](https://learn.microsoft.com/en-us/azure/azure-monitor/app/sdk-connection-string?tabs=dotnet5)
6. [Types of sampling](https://learn.microsoft.com/en-us/azure/azure-monitor/app/sampling?tabs=net-core-new#types-of-sampling)
7. [Workshop step by step](https://azuredevopslabs.com/labs/azuredevops/appinsights/)


# Contributing

This project welcomes contributions and suggestions. Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
