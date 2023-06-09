﻿@description('Specifies the name of the managed grafana.')
param managedGrafanaName string = 'mg-${uniqueString(resourceGroup().id)}'

@description('Azure Monitor Workspace Resource Id.')
param azureMonitorWorkspaceResourceId string

@description('Specifies the location for all resources.')
@allowed([
  'WestEurope'
  'EastUs'
  'NorthEurope'  
])
param location string

param resourceTags object = {
  Description: 'Managed Grafana for Dashboard'
   Environment: 'Demo'
   ResourceType: 'Analytics'
}

resource managedGrafanaForDashboard 'Microsoft.Dashboard/grafana@2022-08-01' = {
  name: managedGrafanaName
  location: location
  tags: resourceTags
  sku: {
    name: 'string'
  } 
  properties: {    
    autoGeneratedDomainNameLabelScope: 'TenantReuse'    
    grafanaIntegrations: {
      azureMonitorWorkspaceIntegrations: [
        {
          azureMonitorWorkspaceResourceId: azureMonitorWorkspaceResourceId
        }
      ]
    }
  }
}