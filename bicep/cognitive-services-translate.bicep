@description('Display name of Text Translation API account')
param accountName string = 'TextTranslation'

@description('SKU for Text Translation API')
@allowed([
  'F0'
  'S1'
  'S2'
  'S3'
  'S4'
])
param SKU string = 'S1'

@description('Location for the account')
param location string = resourceGroup().location

param resourceTags object = {
  Description: 'cognitive services for monitoring basics demo'
   Environment: 'Demo'
   ResourceType: 'CognitiveServices'
}

resource account 'Microsoft.CognitiveServices/accounts@2022-03-01' = {
  name: accountName
  location: location
  kind: 'TextTranslation'
  tags: resourceTags
  sku: {
    name: SKU
  } 
}