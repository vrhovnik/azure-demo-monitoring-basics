# Demos about Azure Monitoring basics

![Azure Monitor](https://learn.microsoft.com/en-us/azure/azure-monitor/media/overview/overview_2023_02.png)

## Prerequisites

1. an active [Azure](https://www.azure.com) subscription - [MSDN](https://my.visualstudio.com) or trial
   or [Azure Pass](https://microsoftazurepass.com) is fine - you can also do all of the work
   in [Azure Shell](https://shell.azure.com) (all tools installed) and by
   using [Github Codespaces](https://docs.github.com/en/codespaces/developing-in-codespaces/creating-a-codespace)
2. [PowerShell](https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-windows?view=powershell-7.2)
   installed - we do recommend an editor like [Visual Studio Code](https://code.visualstudio.com) to be able to write
   scripts, YAML pipelines and connect to repos to submit changes.
3. [OPTIONAL] [Azure CLI](https://learn.microsoft.com/en-us/cli/azure/) installed to work with Azure or Azure PowerShell
   module installed
4. [OPTIONAL] [Windows Terminal](https://learn.microsoft.com/en-us/windows/terminal/install) to be able to work with
   multiple terminal Windows with ease

If you will be working on your local machines, you will need to have:

1. [Powershell](https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-windows)
   installed
2. git installed - instructions step by step [here](https://docs.github.com/en/get-started/quickstart/set-up-git)
3. [.NET](https://dot.net) installed to run the application if you want to run it locally
4. Kubectl installed to work with Kubernetes - you can
   use [Install-AzAskKubectl](https://learn.microsoft.com/en-us/powershell/module/az.aks/install-azaksclitool?view=azps-9.6.0)
   cmdlet
5. an editor (besides notepad) to see and work with code, scripts and more (for
   example [Visual Studio Code](https://code.visualstudio.com) or [NeoVim](https://neovim.io/))

## Basics

Going through Azure Monitor solution with focus on practical areas:

1. Collectors (built-in, ...)
2. Storage of metrics and logs with focus on KQL
3. Usage in depth (insights, visualization, analyze, respond)
4. Alerts
5. Change analysis.

Focusing on defining monitoring strategies with best practices on top of Azure Cloud Adoption Framework.

To prepare the environment, you can run the following script in your PWSH terminal:

```powershell
#to go root folder where you cloned the repo f.e. C:\Work\Projects\azure-demo-monitoring-basics
Set-Location "C:\Work\Projects\azure-demo-monitoring-basics\"
Add-DirToSystemEnv -RestartCurrentSession
Init-Environment -Location "WestEurope"

```

It will create resource for you to be able to work with:

1. Resource group with name and tags (you can change values in [parameters](bicep/rg.parameters.json) file)
2. Storage account to store blob storage and to be able to be set as diagnostics source ((you can change values
   in [parameters](bicep/storage.parameters.json) file))
3. Log Analytics workspace with per gb sku (you can change values in [parameters](bicep/log-analytics.parameters.json)
   file)
4. Application Insights assigned to Log Analytics workspace (you can change values
   in [parameters](bicep/application-insights.parameters.json) file)
5. Virtual Machine with tools deployed and application with SQL installed (you can change values
   in [parameters](bicep/vm.parameters.json) file)
6. Azure Registry to store containers and images (you can change values in [parameters](bicep/registry.parameters.json)
   file). It will compile containers from *containers* folder and push them to the registry.
7. Azure Kubernetes Service with connection to Azure Container Registry (you can change values
   in [parameters](bicep/aks.parameters.json) file). After creating the resource, it will attach registry and get
   connection information and store them to .kubeconfig file to be able to access the cluster. It will deploy newly
   created containers as deployment inside namespace with public access points.
8. Azure Virtual Machine and will install basic web application with SQL, Sysinternals, .NET and other tools to be able
   to work
   with the environment.
9. Azure Load Testing resource to be able to run load tests on the application.

To connect to the cluster, follow
this [instructions](https://learn.microsoft.com/en-us/azure/aks/learn/quick-kubernetes-deploy-bicep?tabs=azure-powershell%2CCLI#connect-to-the-cluster).

_Note:_ It will take around 1h to setup everything.

To install agent via policy, you can use this [definition](https://ms.portal.azure.com/#view/Microsoft_Azure_Policy/InitiativeDetailBlade/id/%2Fproviders%2FMicrosoft.Authorization%2FpolicySetDefinitions%2F0d1b56c6-6d1f-4a5d-8695-b15efbea6b49/scopes%7E/%5B%22%2Fsubscriptions%2Fae71ef11-a03f-4b4f-a0e6-ef144727c711%22%5D).

## Demos

To try out different functionalities we have sample scripts and applications to be able to work with.

To control different settings, you can use environment variables. The easiest way to set them is via PowerShell and
his [provider](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_providers?view=powershell-7.3)
option.

When all settings are set, you can run the application with dotnet run command.

Example below:

```powershell

Set-Item Env:ApplicationInsigths__InstrumentationKey "12345678"
dotnet run

```

## Containers

To use Azure Container Apps or Azure Kubernetes Service you will need to store containers to registry to be pulled from.

In our example we are using Azure Container Registry and also their build task options.

Dockerfiles for containers are located in [containers folder](containers).

To compile containers in **one go**, navigate to [scripts folder](scripts/init) and run Compile-Container script:

```powershell

Compile-Containers

```

To modify the deployment, you can configure the following parameters:

1. **ResourceGroupName** - name of the resource group where to store the container
2. **RegistryName** - name of the container registry
3. **FolderName**  - name of the folder where you can find Dockerfiles (by default folder containers)
4. **TagName** - tag name for the container (by default latest)
5. **SourceFolder** - folder where to look for the source code (by default src)
6. **InstallCli** - if you want to install Azure CLI (by default false)

For example to build containers with tag 1.0.0:

```powershell

Compile-Containers -TagName 1.0.0

```

## Create and load environment variables

The easiest way to create environment variables is to store it to file, exclude that file from getting into source
control and load variables via PowerShell (as demonstrated below).

Example of env variable inside env file:
AuthOptions__ApiKey=1234567890

You can check out [docs for dotnet run](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-run) for more options
how to run the app with parameters.

If you want to automate the process, you can use PowerShell script to prepare the environment for you.

```powershell

Get-Content $PathToENVFile | ForEach-Object {
    $name, $value = $_.split('=')
    Set-Content env:\$name $value
}

```

Prepare file (example [here](./scripts/init/example.changetoenv) - rename to .env) and exclude *.env files from
putting it to the repo. More [here](https://docs.github.com/en/get-started/getting-started-with-git/ignoring-files).
Open PowerShell and run the upper command by replacing PathToENVFile the path to your file in double quotes.

## Integration with first and 3rd party solutions

Integration with different solution and reacting on different items, focusing on:

1. Integrating with first party solution (serverless, web hooks, API, ..)
2. Observability, recovery actions and SLO
3. Integrating with hybrid solutions (on-premises, 3rd party clouds, etc.)
4. Traces and custom events with built-in options and REST / custom development

Explaining and using Azure Managed Grafana and Prometheus exporter with Azure Monitor, different sources, and streaming
sources.

## Additional information

You can read about different techniques and options here:

1. [What-The-Hack initiative](https://aka.ms/wth)
2. [GitHub and DevOps](https://resources.github.com/devops/)
3. [Azure Samples](https://github.com/Azure-Samples)
   or [use code browser](https://docs.microsoft.com/en-us/samples/browse/?products=azure)
4. [Azure Architecture Center](https://docs.microsoft.com/en-us/azure/architecture/)
5. [Application Architecture Guide](https://docs.microsoft.com/en-us/azure/architecture/guide/)
6. [Cloud Adoption Framework](https://docs.microsoft.com/en-us/azure/cloud-adoption-framework/)
7. [Well-Architected Framework](https://docs.microsoft.com/en-us/azure/architecture/framework/)
8. [Microsoft Learn](https://docs.microsoft.com/en-us/learn/roles/solutions-architect)
9. [Azure Container Apps](https://learn.microsoft.com/en-us/azure/container-apps/)
10. [Azure Kubernetes Service](https://learn.microsoft.com/en-us/azure/aks/intro-kubernetes)
11. [Sysinternals](https://docs.microsoft.com/en-us/sysinternals/)

## Credits

1. [Spectre.Console](https://spectreconsole.net/) - Spectre.Console is a .NET Standard 2.0 library that makes it easier
   to create beautiful console applications.
2. [MetroApps](https://mahapps.com/) - MahApps.Metro is a framework that allows developers to cobble together a Metro or
   Modern UI for their own WPF applications with minimal effort.
3. [HTMX](https://htmx.org) - htmx gives you access to AJAX, CSS Transitions, WebSockets and Server Sent Events directly
   in HTML, using attributes, so you can build modern user interfaces with the simplicity and power of hypertext.
4. [QuestPDF](https://github.com/QuestPDF/QuestPDF) - QuestPDF is an open-source .NET library for PDF documents
   generation.
5. [Mermaid.js](https://github.com/mermaid-js/mermaid) - generating dialogs from markdown files -
   thanks [Adrian](https://github.com/snobu)

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
