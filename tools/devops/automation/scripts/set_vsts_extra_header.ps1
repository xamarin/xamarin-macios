<#
  .SYNOPSIS
  Adds extra headers to a vsts repository so that we can perform git operaitons without the need of auth.

  .DESCRIPTION
  Devops does not use ssh to check out the repositories, it uses https. This is problematic when we are trying to do a
  git operation in any of our scripts. The way vsts uses git is to update the configuration of the 
  git operations to be performed without extra steps because we have added the token as part of the git configuration.

  .PARAMETER Repository
  Name of the VSTS repo we are interested in.

  .PARAMETER AccessToken
  The vsts token to be used to perform the git operations

  .PARAMETER RepositoryPath
  The reopsitory checkout location.

#>
param
(
    [Parameter(Mandatory)]
    [String]
    $Repository,

    [Parameter(Mandatory)]
    [String]
    $AccessToken,

    [Parameter(Mandatory)]
    [String]
    $RepositoryPath
)

# update the config of the repository url to use the correct authentication headers, for more information read: 
# https://learn.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate?view=azure-devops&tabs=Windows#use-a-pat
$headers = "Authorization: Basic " + [Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes(":" + $AccessToken))
git -C "$RepositoryPath" config "http.https://devdiv@dev.azure.com/devdiv/DevDiv/_git/$Repository.extraheader=$headers"
git -C "$RepositoryPath" config --get-all http.extraheader

