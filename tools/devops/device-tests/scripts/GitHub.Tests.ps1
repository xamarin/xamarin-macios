<#
Github interaction unit tests.
#>

Import-Module ./GitHub -Force

Describe 'Set-GitHubStatus' {
    Context 'with all env variables present' {

        It 'calls the rest method succesfully' {
            # set the required enviroments in the context
            $envVariables = @{
                "SYSTEM_TEAMFOUNDATIONCOLLECTIONURI" = "SYSTEM_TEAMFOUNDATIONCOLLECTIONURI";
                "SYSTEM_TEAMPROJECT" = "SYSTEM_TEAMPROJECT";
                "BUILD_BUILDID" = "BUILD_BUILDID";
                "SYSTEM_JOBNAME" = "SYSTEM_JOBNAME";
                "SYSTEM_STAGEDISPLAYNAME" = "SYSTEM_STAGEDISPLAYNAME"
                "BUILD_REVISION" = "BUILD_REVISION";
                "GITHUB_TOKEN" = "GITHUB_TOKEN"
            }

            $envVariables.GetEnumerator() | ForEach-Object { 
                $key = $_.Key
                Set-Item -Path "Env:$key" -Value $_.Value
            }

            Mock Invoke-RestMethod {
                return @{"status"=200;}
            }
            $status = "error"
            $context = "My context"
            $description = "Testing Status API"

            Set-GitHubStatus -Status $status -Description $description -Context $context

            # assert the call and compare the expected parameters to the received ones
            Assert-MockCalled -CommandName Invoke-RestMethod -Times 1 -Scope It -ParameterFilter {
                # validate each of the params and the payload
                if ($Uri -ne "https://api.github.com/repos/xamarin/xamarin-macios/statuses/BUILD_REVISION") {
                    return $False
                }
                if ($Headers.Authorization -ne ("token {0}" -f $envVariables["GITHUB_TOKEN"])) {
                    return $False
                }
                if ($Method -ne "POST") {
                    return $False
                }
                if ($ContentType -ne "application/json") {
                    return $False
                }
                # compare the payload
                $bodyObj = ConvertFrom-Json $Body
                if ($bodyObj.state -ne $status) {
                    return $False
                }

                if ($bodyObj.context -ne $context) {
                    return $False
                }

                if ($bodyObj.description -ne $description) {
                    return $False
                }

                return $True
            }
        }

        It 'calls the rest method with an error' {
            # set the required enviroments in the context
            $envVariables = @{
                "SYSTEM_TEAMFOUNDATIONCOLLECTIONURI" = "SYSTEM_TEAMFOUNDATIONCOLLECTIONURI";
                "SYSTEM_TEAMPROJECT" = "SYSTEM_TEAMPROJECT";
                "BUILD_BUILDID" = "BUILD_BUILDID";
                "SYSTEM_JOBNAME" = "SYSTEM_JOBNAME";
                "SYSTEM_STAGEDISPLAYNAME" = "SYSTEM_STAGEDISPLAYNAME"
                "BUILD_REVISION" = "BUILD_REVISION";
                "GITHUB_TOKEN" = "GITHUB_TOKEN"
            }

            $envVariables.GetEnumerator() | ForEach-Object { 
                $key = $_.Key
                Set-Item -Path "Env:$key" -Value $_.Value
            }
            Mock Invoke-RestMethod {
                throw [System.Exception]::("Test")
            }
            #set env vars
            { Set-GitHubStatus -Status $status -Description $description -Context $context } | Should -Throw
        }
    }
    Context 'without an env var' {
        It 'failed calling the rest method' {
            Mock Invoke-RestMethod {
                return @{"status"=200;}
            }

            # clear the env vars
            $envVariables = @{
                "SYSTEM_TEAMFOUNDATIONCOLLECTIONURI" = "SYSTEM_TEAMFOUNDATIONCOLLECTIONURI";
                "SYSTEM_TEAMPROJECT" = "SYSTEM_TEAMPROJECT";
                "BUILD_BUILDID" = "BUILD_BUILDID";
                "SYSTEM_JOBNAME" = "SYSTEM_JOBNAME";
                "SYSTEM_STAGEDISPLAYNAME" = "SYSTEM_STAGEDISPLAYNAME"
                "BUILD_REVISION" = "BUILD_REVISION";
                "GITHUB_TOKEN" = "GITHUB_TOKEN"
            }
            $envVariables.GetEnumerator() | ForEach-Object { 
                $key = $_.Key
                Remove-Item -Path "Env:$key"
            }

            $status = "error"
            $context = "My context"
            $description = "Testing Status API"

            { Set-GitHubStatus -Status $status -Description $description -Context $context } | Should -Throw
            Assert-MockCalled -CommandName Invoke-RestMethod -Times 0 -Scope It 
        }
    }
}

Describe 'New-GitHubComment' {
    Context 'with all env variables present' {

        BeforeAll {
            $Script:envVariables = @{
                "SYSTEM_TEAMFOUNDATIONCOLLECTIONURI" = "SYSTEM_TEAMFOUNDATIONCOLLECTIONURI";
                "SYSTEM_TEAMPROJECT" = "SYSTEM_TEAMPROJECT";
                "BUILD_BUILDID" = "BUILD_BUILDID";
                "SYSTEM_JOBNAME" = "SYSTEM_JOBNAME";
                "SYSTEM_STAGEDISPLAYNAME" = "SYSTEM_STAGEDISPLAYNAME"
                "BUILD_REVISION" = "BUILD_REVISION";
                "GITHUB_TOKEN" = "GITHUB_TOKEN";
                "BUILD_DEFINITIONNAME" = "BUILD_DEFINITIONNAME"
            }

            $Script:envVariables.GetEnumerator() | ForEach-Object { 
                $key = $_.Key
                Set-Item -Path "Env:$key" -Value $_.Value
            }
        }

        It 'calls the method succesfully' {
            Mock Invoke-RestMethod {
                return @{"status"=200;}
            }
            $header = "The header"
            $description = "Testing Comments API"
            $message = "This is a test"
            $emoji = ":tada:"

            New-GitHubComment -Header $header -Description $description -Message $message -Emoji $emoji

            # assert the call and compare the expected parameters to the received ones
            Assert-MockCalled -CommandName Invoke-RestMethod -Times 1 -Scope It -ParameterFilter {
                # validate each of the params and the payload
                if ($Uri -ne "https://api.github.com/repos/xamarin/xamarin-macios/commits/BUILD_REVISION/comments") {
                    return $False
                }
                if ($Headers.Authorization -ne ("token {0}" -f $envVariables["GITHUB_TOKEN"])) {
                    return $False
                }
                if ($Method -ne "POST") {
                    return $False
                }
                if ($ContentType -ne "application/json") {
                    return $False
                }
                # compare the payload
                $bodyObj = ConvertFrom-Json $Body
                $body = $bodyObj.body
                if ($bodyObj.body -eq $null) {
                    return $False
                }

                return $True
            }

        }

        It 'calls the method with an error and throws' {
            Mock Invoke-RestMethod {
                throw [System.Exception]::("Test")
            }
            $header = "The header"
            $description = "Testing Comments API"
            $message = "This is a test"
            $emoji = ":tada:"

            { New-GitHubComment -Header $header -Description $description -Message $message -Emoji $emoji } | Should -Throw
        }

    }
    Context 'without an env variable' {
        BeforeAll {
            # clear the env vars
            $envVariables = @{
                "SYSTEM_TEAMFOUNDATIONCOLLECTIONURI" = "SYSTEM_TEAMFOUNDATIONCOLLECTIONURI";
                "SYSTEM_TEAMPROJECT" = "SYSTEM_TEAMPROJECT";
                "BUILD_BUILDID" = "BUILD_BUILDID";
                "SYSTEM_JOBNAME" = "SYSTEM_JOBNAME";
                "SYSTEM_STAGEDISPLAYNAME" = "SYSTEM_STAGEDISPLAYNAME"
                "BUILD_REVISION" = "BUILD_REVISION";
                "GITHUB_TOKEN" = "GITHUB_TOKEN";
                "BUILD_DEFINITIONNAME" = "BUILD_DEFINITIONNAME"
            }
            $envVariables.GetEnumerator() | ForEach-Object { 
                $key = $_.Key
                Remove-Item -Path "Env:$key"
            }
        }
        It 'throws and error' {

            Mock Invoke-RestMethod {
                return @{"status"=200;}
            }

            $header = "The header"
            $description = "Testing Comments API"
            $message = "This is a test"
            $emoji = ":tada:"

            { New-GitHubComment -Header $header -Description $description -Message $message -Emoji $emoji } | Should -Throw
            Assert-MockCalled -CommandName Invoke-RestMethod -Times 0 -Scope It 
        }
    }
}

Describe New-GitHubCommentFromFile {
    Context 'file present' {

        BeforeAll {
            $Script:tempPath = [System.IO.Path]::GetTempFileName()
            $Script:message = "Test message in a bottle"
            Set-Content -Path $Script:tempPath -Value $message
        }

        AfterAll {
            Remove-Item -Path $Script:tempPath
        }

        It 'calls the inner method' {
            Mock New-GitHubComment

            $header = "My test"
            $description = "Le description"
            $emoji = ":tada:"

            New-GitHubCommentFromFile -Header $header -Description $description -Path $Script:tempPath -Emoji $emoji

            #just assert that the method was called with the expected values
            Assert-MockCalled -CommandName New-GitHubComment -Times 1 -Scope It -ParameterFilter {
                if ($Header -ne $header) {
                    return $False
                }
                if ($Description -ne $description) {
                    return $False
                }
                if ($Emoji -ne $emoji) {
                    return $False
                }
                if ($Message -like $Script:message) {
                    return $False
                }
                return $True
            }
        }
    }
    Context 'file missing' {
        It 'throws and error' {

            $header = "My test"
            $description = "Le description"
            $emoji = ":tada:"

            { New-GitHubCommentFromFile -Header $header -Description $description -Path "missing/path" -Emoji $emoji } | Should -Throw
        }
    }
}

Describe 'New-GitHubSummaryComment' {
    Context 'all present variables' {

        BeforeAll {
            # clear the env vars
            $Script:envVariables = @{
                "SYSTEM_TEAMFOUNDATIONCOLLECTIONURI" = "SYSTEM_TEAMFOUNDATIONCOLLECTIONURI";
                "SYSTEM_TEAMPROJECT" = "SYSTEM_TEAMPROJECT";
                "BUILD_BUILDID" = "BUILD_BUILDID";
                "SYSTEM_JOBNAME" = "SYSTEM_JOBNAME";
                "SYSTEM_STAGEDISPLAYNAME" = "SYSTEM_STAGEDISPLAYNAME"
                "BUILD_REVISION" = "BUILD_REVISION";
                "GITHUB_TOKEN" = "GITHUB_TOKEN";
                "BUILD_DEFINITIONNAME" = "BUILD_DEFINITIONNAME";
                "SYSTEM_DEFAULTWORKINGDIRECTORY" = "SYSTEM_DEFAULTWORKINGDIRECTORY"
            }
            $Script:envVariables.GetEnumerator() | ForEach-Object { 
                $key = $_.Key
                Set-Item -Path "Env:$key" -Value $_.Value
            }
            $Script:xamarinStoragePath = "/path/to/xamarin/storage"
            $Script:context = "Testing"

            $Script:tempPath = [System.IO.Path]::GetTempFileName()
            $Script:message = "Test message in a bottle"
            Set-Content -Path $Script:tempPath -Value $message
        }

        AfterAll {
            Remove-Item -Path $Script:tempPath
        }

        It 'calls rest methods on a completed and succesful test run' {
            Mock Set-GitHubStatus
            Mock New-GitHubCommentFromFile
            Mock Test-Path { return $true }

            # set job as a success
            Set-Item -Path "Env:TESTS_JOBSTATUS" -Value "Succeeded"

            New-GitHubSummaryComment -Context $Script:context -XamarinStoragePath $Script:xamarinStoragePath -TestSummaryPath $Script:tempPath

            # assert rest calls
            Assert-MockCalled -CommandName Set-GitHubStatus -Times 1 -Scope It -ParameterFilter {
                if ($Status -ne "success") {
                    return $False
                }

                if ($Context -ne $Script:context) {
                    return $False
                }

                if ($Description -ne "Device tests passed on $Script:context.") {
                    return $False
                }

                return $True
            }

            Assert-MockCalled -CommandName New-GitHubCommentFromFile -Times 1 -Scope It -ParameterFilter {
                if (-not ($Header -like "Device tests passed on $Script:context*")) {
                    return $False
                }

                if (-not ($Description -like "Device tests passed on $Script:context*")) {
                    return $False
                }

                if ($Path -ne $Script:tempPath) {
                    return $False
                }

                return $True
            }
        }

        It 'calls rest methods on a completed failed test run' {
            Mock Set-GitHubStatus
            Mock New-GitHubCommentFromFile
            Mock Test-Path { return $true }

            Set-Item -Path "Env:TESTS_JOBSTATUS" -Value "Failed"

            New-GitHubSummaryComment -Context $Script:context -XamarinStoragePath $Script:xamarinStoragePath -TestSummaryPath $Script:tempPath

            Assert-MockCalled -CommandName Set-GitHubStatus -Times 1 -Scope It -ParameterFilter {
                if ($Status -ne "failure") {
                    return $False
                }

                if ($Context -ne $Script:context) {
                    return $False
                }

                if ($Description -ne "Device tests failed on $Script:context.") {
                    return $False
                }

                return $True
            }

            Assert-MockCalled -CommandName New-GitHubCommentFromFile -Times 1 -Scope It -ParameterFilter {
                if (-not ($Header -like "Device tests failed on $Script:context*")) {
                    return $False
                }

                if (-not ($Description -like "Device tests failed on $Script:context*")) {
                    return $False
                }

                if ($Path -ne $Script:tempPath) {
                    return $False
                }

                return $True
            }
        }

        It 'calls rest methods on a failed test run (TestSummay.md missing)' {
            Mock Set-GitHubStatus
            Mock New-GitHubComment
            Mock Test-Path { return $false}

            Set-Item -Path "Env:TESTS_JOBSTATUS" -Value "Failed"

            New-GitHubSummaryComment -Context $Script:context -XamarinStoragePath $Script:xamarinStoragePath -TestSummaryPath $Script:tempPath

            Assert-MockCalled -CommandName Set-GitHubStatus -Times 1 -Scope It -ParameterFilter {
                if ($Status -ne "failure") {
                    return $False
                }

                if ($Context -ne $Script:context) {
                    return $False
                }

                if (-not ($Description -like "Tests failed catastrophically on $Script:context (no summary found).")) {
                    return $False
                }

                return $True
            }

            Assert-MockCalled -CommandName New-GitHubComment -Times 1 -Scope It -ParameterFilter {
                if ($Header -ne "Tests failed catastrophically on $Script:context (no summary found).") {
                    return $False
                }

                if (-not ($Description -like "Result file $Script:tempPath not found.*")) {
                    return $False
                }

                return $True
            }
        }

    }

    Context 'missing variables' {

        BeforeAll {
            $Script:envVariables = @{
                "SYSTEM_TEAMFOUNDATIONCOLLECTIONURI" = "SYSTEM_TEAMFOUNDATIONCOLLECTIONURI";
                "SYSTEM_TEAMPROJECT" = "SYSTEM_TEAMPROJECT";
                "BUILD_BUILDID" = "BUILD_BUILDID";
                "SYSTEM_JOBNAME" = "SYSTEM_JOBNAME";
                "SYSTEM_STAGEDISPLAYNAME" = "SYSTEM_STAGEDISPLAYNAME"
                "BUILD_REVISION" = "BUILD_REVISION";
                "GITHUB_TOKEN" = "GITHUB_TOKEN";
                "BUILD_DEFINITIONNAME" = "BUILD_DEFINITIONNAME"
            }

            $envVariables.GetEnumerator() | ForEach-Object { 
                $key = $_.Key
                Remove-Item -Path "Env:$key"
            }

            $Script:xamarinStoragePath = "/path/to/xamarin/storage"
            $Script:context = "Testing"

            $Script:tempPath = [System.IO.Path]::GetTempFileName()
            $Script:message = "Test message in a bottle"
            Set-Content -Path $Script:tempPath -Value $message
        }

        AfterAll {
            Remove-Item -Path $Script:tempPath
        }

        It 'throws and exception' {
            { New-GitHubSummaryComment -Context $Script:context -XamarinStoragePath $Script:xamarinStoragePath -TestSummaryPath $Script:tempPath } | Should -Throw
        }
    }
}

Describe 'Test-JobSuccess' {
    Context 'succesfull' {
        Test-JobSuccess -Status "Succeeded" | Should -Be $True
    }

    Context 'known failures' {
        Test-JobSuccess -Status "Canceled" | Should -Be $False
        Test-JobSuccess -Status "Failed" | Should -Be $False
        Test-JobSuccess -Status "SucceededWithIssues" | Should -Be $False
    }

    Context 'unknonw value' {
        Test-JobSuccess -Status "Random value" | Should -Be $False
    }
}