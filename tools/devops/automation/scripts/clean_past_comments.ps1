param
(

    [Parameter(Mandatory)]
    [String]
    $GithubToken,

    [Parameter(Mandatory)]
    [String]
    $RepositoryUri,

    [Parameter(Mandatory)]
    [String]
    $SourceBranch,

    [Parameter(Mandatory)]
    [String]
    $CommentToHide
)

Import-Module $Env:SYSTEM_DEFAULTWORKINGDIRECTORY/xamarin-macios/tools/devops/automation/scripts/MaciosCI.psd1

$comments = New-GitHubCommentsObjectFromUrl -Url "$RepositoryUri" -Token $GithubToken

$prId = "$SourceBranch".Replace("refs/pull/", "").Replace("/merge", "")
$prComments = $comments.GetCommentsForPR($prId)

$botComments = [System.Collections.ArrayList]@()
foreach ($c in $prComments) {
  if ($c.Author -eq "vs-mobiletools-engineering-service2") {
    if ($c.Body.Contains($CommentToHide)) {
      $botComments.Add($c)
    }
  }
}
$comments.MinimizeComments($botComments)
