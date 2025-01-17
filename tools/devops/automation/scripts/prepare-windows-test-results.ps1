Set-Location "$Env:BUILD_SOURCESDIRECTORY\$Env:BUILD_REPOSITORY_TITLE"
$Env:DOTNET = "$Env:BUILD_SOURCESDIRECTORY\$Env:BUILD_REPOSITORY_TITLE\tests\dotnet\Windows\bin\dotnet\dotnet.exe"
& $Env:DOTNET run --project "$Env:BUILD_SOURCESDIRECTORY\$Env:BUILD_REPOSITORY_TITLE\scripts\create-html-report\create-html-report.csproj"

if ($Env:AGENT_JOBSTATUS -eq "Succeeded") {
    $jobStatus = "Succeeded"
} else {
    $jobStatus = "Failed"
}
Write-Host "##vso[task.setvariable variable=TESTS_JOBSTATUS;isOutput=true]$jobStatus"
