dotnet run "$Env:BUILD_SOURCESDIRECTORY/xamarin-macios/tools/devops/automation/scripts/csharp/create-html-report/create-html-report.csproj"

if ($Env:AGENT_JOBSTATUS -eq "Succeeded") {
    $jobStatus = "Succeeded"
} else {
    $jobStatus = "Failed"
}
Write-Host "##vso[task.setvariable variable=TESTS_JOBSTATUS;isOutput=true]$jobStatus"
