if ("$Env:BUILD_REPOSITORY_TITLE" -eq "") {
    $remoteUrl = & git remote get-url --push origin
    $repoTitle = [System.IO.Path]::GetFilename($remoteUrl)
    $Env:BUILD_REPOSITORY_TITLE = $repoTitle
}

if ($Env:SYSTEM_DEFAULTWORKINGDIRECTORY.EndsWith($Env:BUILD_REPOSITORY_TITLE)) {
    $Env:BUILD_REPOSITORY_TITLE = ""
}

# Dump the environment to see what we're working with.
& "$Env:SYSTEM_DEFAULTWORKINGDIRECTORY\$Env:BUILD_REPOSITORY_TITLE\tools\devops\automation\scripts\show_bot_info.ps1"

# Set a few variables
$Env:DOTNET = "$Env:BUILD_SOURCESDIRECTORY\$Env:BUILD_REPOSITORY_TITLE\tests\dotnet\Windows\bin\dotnet\dotnet.exe"
$Env:DOTNET_DIR = "$Env:BUILD_SOURCESDIRECTORY\$Env:BUILD_REPOSITORY_TITLE\tests\dotnet\Windows\bin\dotnet\"
$Env:TESTS_USE_SYSTEM = "1"

# Compute the <platform>_NUGET_VERSION_NO_METADATA variables and set them in the environment
$configurationDotNetPlatforms = $Env:CONFIGURATION_DOTNET_PLATFORMS
$dotnetPlatforms = $configurationDotNetPlatforms.Split(' ', [StringSplitOptions]::RemoveEmptyEntries)
foreach ($platform in $dotnetPlatforms) {
  $variableName = "$($platform.ToUpper())_NUGET_VERSION_NO_METADATA"
  $variableValue = [Environment]::GetEnvironmentVariable("CONFIGURATION_" + $variableName)
  [Environment]::SetEnvironmentVariable($variableName, $variableValue)
  Write-Host "$variableName = $variableName"

  $variableName = "$($platform.ToUpper())_NUGET_SDK_NAME"
  $variableValue = [Environment]::GetEnvironmentVariable("CONFIGURATION_" + $variableName)
  [Environment]::SetEnvironmentVariable($variableName, $variableValue)
  Write-Host "$variableName = $variableValue"

  $variableName = "$($platform.ToUpper())_NUGET_REF_NAME"
  $variableValue = [Environment]::GetEnvironmentVariable("CONFIGURATION_" + $variableName)
  [Environment]::SetEnvironmentVariable($variableName, $variableValue)
  Write-Host "$variableName = $variableValue"

  $variableName = "DOTNET_$($platform.ToUpper())_RUNTIME_IDENTIFIERS"
  $variableValue = [Environment]::GetEnvironmentVariable("CONFIGURATION_" + $variableName)
  [Environment]::SetEnvironmentVariable($variableName, $variableValue)
  Write-Host "$variableName = $variableValue"

  $rids = $variableValue.Split(' ', [StringSplitOptions]::RemoveEmptyEntries)
  foreach ($rid in $rids) {
      $variableName = "$($rid)_NUGET_RUNTIME_NAME"
      $variableValue = [Environment]::GetEnvironmentVariable("CONFIGURATION_" + $variableName)
      [Environment]::SetEnvironmentVariable($variableName, $variableValue)
      Write-Host "$variableName = $variableValue"
  }
}

# Tell the tests how they can execute the C# compiler
$csc = Get-ChildItem "$Env:BUILD_SOURCESDIRECTORY\$Env:BUILD_REPOSITORY_TITLE\tests\dotnet\Windows\bin\dotnet" -Include csc.dll -Recurse -File | %{$_.FullName}
$Env:DOTNET_CSC_COMMAND = "$Env:DOTNET exec $csc".Replace("\", "/")
Write-Host "DOTNET_CSC_COMMAND: $Env:DOTNET_CSC_COMMAND"

# Tell the tests where the BCL is
$Env:DOTNET_BCL_DIR = Get-ChildItem "$Env:BUILD_SOURCESDIRECTORY\$Env:BUILD_REPOSITORY_TITLE\tests\dotnet\Windows\bin\dotnet\packs\Microsoft.NETCore.App.Ref" -Include System.dll -Recurse -File | %{$_.DirectoryName}
Write-Host "DOTNET_BCL_DIR: $Env:DOTNET_BCL_DIR"

# Finally we can run the tests
& $Env:BUILD_SOURCESDIRECTORY\$Env:BUILD_REPOSITORY_TITLE\tests\dotnet\Windows\bin\dotnet\dotnet.exe `
    test `
    "$Env:BUILD_SOURCESDIRECTORY/$Env:BUILD_REPOSITORY_TITLE/tests/bgen/bgen-tests.csproj" `
    "--results-directory:$Env:BUILD_SOURCESDIRECTORY/$Env:BUILD_REPOSITORY_TITLE/jenkins-results/windows/bgen-tests/" `
    "--logger:console;verbosity=detailed" `
    "--logger:trx;LogFileName=$Env:BUILD_SOURCESDIRECTORY/$Env:BUILD_REPOSITORY_TITLE/jenkins-results/windows/bgen-tests/results.trx" `
    "--logger:html;LogFileName=$Env:BUILD_SOURCESDIRECTORY/$Env:BUILD_REPOSITORY_TITLE/jenkins-results/windows/bgen-tests/results.html" `
    "--settings" "$Env:BUILD_SOURCESDIRECTORY/$Env:BUILD_REPOSITORY_TITLE/tests/dotnet/Windows/config.runsettings" `
    "-bl:$Env:BUILD_SOURCESDIRECTORY/$Env:BUILD_REPOSITORY_TITLE/jenkins-results/windows/bgen-tests/results.binlog"
