<#
  .SYNOPSIS
  Updates the maccore origin url to contain the given token, allowing this way the CI to checkout the repo.

  .DESCRIPTION
  Devops does not use ssh to check out the repositories, it uses https. This is problematic when we are trying to do a
  git operation in any of our scripts. To fix that we can prepend the github token to the checkout url, this allows all
  git operations to be performed without extra steps because the PAT is part of the url.

  The url pattern for this to wori is "https://$GithubToken@github.com/xamarin/maccore.git"

  .PARAMETER GithubToken
  The token to be used in the origin url domain.

  .PARAMETER MaccorePath
  Maccore checkout location.

#>
param
(
    [Parameter(Mandatory)]
    [String]
    $GithubToken,

    [Parameter(Mandatory)]
    [String]
    $MaccorePath
)

# maccore is special, we use fetch there in some bash scripts, but VSTS uses https.. and some pools don't like the above.. :/
git -C "$MaccorePath" remote remove origin
git -C "$MaccorePath" remote add origin "https://$GithubToken@github.com/xamarin/maccore.git"
git -C "$MaccorePath" remote # don't add -v else we see the pat
