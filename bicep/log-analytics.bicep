@description('Specifies the name of the log analytics workspace.')
param laName string = 'law-${uniqueString(resourceGroup().id)}'

@description('Specifies the name of the application insights resource.')
param appInsightName string = 'ai-${uniqueString(resourceGroup().id)}'

@description('Specifies the location for all resources.')
@allowed([
  'WestEurope'
  'EastUs'
  'NorthEurope'  
])
param location string

param resourceTags object = {
  Description: 'Log analytics and application insights for monitoring basics demo'
   Environment: 'Demo'
   ResourceType: 'Analytics'
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightName
  location: location
  kind: 'web'
  tags: resourceTags
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalytics.id    
  }
}

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: laName
  location: location  
  tags: resourceTags
  properties: {
    sku: {
      name: 'PerGB2018'
    }
  }
}

@description('Output log analytics ID')
output logAnalyticsId string = logAnalytics.id

@description('Output application insights ID')
output appInsightsId string = appInsights.id
