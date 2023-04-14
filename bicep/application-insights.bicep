@description('Specifies the location for all resources.')
@allowed([
  'WestEurope'
  'EastUs'
  'NorthEurope'  
])
param location string

@description('Specifies the name of the application insights resource.')
param appInsightName string = 'ai-${uniqueString(resourceGroup().id)}'

@description('Resource Id of the log analytics workspace which the data will be ingested to.')
param logAnalyticsId string

param resourceTags object = {
  Description: 'Log analytics for monitoring basics demo'
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
    WorkspaceResourceId: logAnalyticsId    
  }
}
