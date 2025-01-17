Write-Host "Running tests on $($Env:AGENT_NAME)"

Write-Host "##vso[task.setvariable variable=TESTS_BOT;isOutput=true]$($Env:AGENT_NAME)"
Write-Host "##vso[task.setvariable variable=TESTS_LABEL;isOutput=true]$($Env:TESTS_LABEL)"
Write-Host "##vso[task.setvariable variable=TESTS_PLATFORM;isOutput=true]$($Env:TESTS_PLATFORM)"
Write-Host "##vso[task.setvariable variable=TESTS_ATTEMPT;isOutput=true]$($Env:SYSTEM_JOBATTEMPT)"
Write-Host "##vso[task.setvariable variable=TESTS_TITLE;isOutput=true]$($Env:LABEL_WITH_PLATFORM)"
