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
    $SourcesDirectory,

    [Parameter(Mandatory)]
    [String]
    $GithubFailureCommentFile,

    [Parameter(Mandatory)]
    [String]
    $StatusContext
)

Import-Module $Env:SYSTEM_DEFAULTWORKINGDIRECTORY\xamarin-macios\tools\devops\automation\scripts\MaciosCI.psd1
$statuses = New-GitHubStatusesObjectFromUrl -Url "$RepositoryUri" -Token $GitHubToken

Write-Host "Found tests"
$testsPath = "$SourcesDirectory/artifacts/mac-test-package/tests"
Write-Host "Tests path is $testsPath"

# print enviroment
dir env:

[System.Collections.Generic.List[string]]$failures = @()

# Claim that the tests timed out before we start
Set-Content -Path "$GithubFailureCommentFile" -Value "Tests timed out"

$macTest = @("dontlink", "introspection", "linksdk", "linkall", "xammac_tests", "monotouch-test")
foreach ($t in $macTest) {
  $testName = "exec-$t"
  Write-Host "Execution test $testName"
  make -d -C $testsPath $testName -f packaged-macos-tests.mk
  if ($LastExitCode -eq 0) {
    Write-Host "$t succeeded"
  } else {
    Write-Host "$t failed with error $LastExitCode"
    $failures.Add($t)
  }
}
if ($failures.Count -ne 0) {
  # post status and comment in the build
  $failedTestsStr = [string]::Join(",",$failures)
  # build message
  $msg = [System.Text.StringBuilder]::new()
  $msg.AppendLine("Failed tests are:")
  $msg.AppendLine("")
  foreach ($test in $failures)
  {
      $msg.AppendLine("* $test")
  }

  # We failed, so write to the comment file why we failed.
  Set-Content -Path "$GithubFailureCommentFile" -Value "$msg"

  exit 1
} else {
  # We succeeded, so remove the failure comment file.
  Remove-Item -Path "$GithubFailureCommentFile"

  exit 0
}
