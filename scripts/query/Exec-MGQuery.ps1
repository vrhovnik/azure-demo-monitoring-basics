<#

.SYNOPSIS

Execute query against Microsoft Graph

.DESCRIPTION

Execute query against Microsoft Graph and get back results. You will need to sign in to Microsoft Graph first.
 
.EXAMPLE

PS > Exec-MGQuery -QueryName "Get-VMs"

get back results from query which is stored in file queries\Get-VMs.query from Microsoft Graph
 
. LINK

http://github.com/vrhovnik
 
#>
param(
    [Parameter(Mandatory = $true, HelpMessage = "Name of the file with query inside the Queries folder")]
    $QueryName
)

Write-Verbose "Query name: $QueryName"

$file = "queries\Graph\$QueryName.query"
Write-Verbose "File: $file"
if (-not (Test-Path $file))
{
    Write-Error "File $file does not exist, check the name of the parameter."
    return
}

$query = Get-Content -Path $file -Raw
Write-Output "Query: $query"
$token = (Get-AzAccessToken -ResourceUrl "https://graph.microsoft.com").Token
Write-Verbose "Token: $token"
Connect-MgGraph -AccessToken $token 
$data = Search-AzGraph -Query $query
Write-Output "Data: $data"
$data
