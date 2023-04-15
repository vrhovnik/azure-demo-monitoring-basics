@description('Specifies the name of the load testing resource.')
param loadTestingName string = 'alt-${uniqueString(resourceGroup().id)}'

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

resource loadTestingForMonitoring 'Microsoft.LoadTestService/loadTests@2022-12-01' = {
  name: loadTestingName
  location: location
  tags: resourceTags  
  properties: {
    description: 'Load testing for monitoring basics demo'    
  }
}