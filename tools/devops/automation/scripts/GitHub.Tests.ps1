<#
Github interaction unit tests.
#>

Import-Module ./GitHub -Force

Describe 'New-GitHubComment' {
    Context 'with all env variables present' -Skip {

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
    Context 'without an env variable' -Skip {
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
    Context 'file present' -Skip {

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
    Context 'all present variables' -Skip {

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
            $Script:context = "Testing"

            $Script:tempPath = [System.IO.Path]::GetTempFileName()
            $Script:message = "Test message in a bottle"
            Set-Content -Path $Script:tempPath -Value $message
        }

        AfterAll {
            Remove-Item -Path $Script:tempPath
        }

        It 'calls rest methods on a completed and succesful test run' {
            Mock New-GitHubCommentFromFile
            Mock Test-Path { return $true }

            # set job as a success
            Set-Item -Path "Env:TESTS_JOBSTATUS" -Value "Succeeded"

            New-GitHubSummaryComment -Context $Script:context -TestSummaryPath $Script:tempPath

            # assert rest calls
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
            Mock New-GitHubCommentFromFile
            Mock Test-Path { return $true }

            Set-Item -Path "Env:TESTS_JOBSTATUS" -Value "Failed"

            New-GitHubSummaryComment -Context $Script:context -TestSummaryPath $Script:tempPath

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
            Mock New-GitHubComment
            Mock Test-Path { return $false}

            Set-Item -Path "Env:TESTS_JOBSTATUS" -Value "Failed"

            New-GitHubSummaryComment -Context $Script:context -TestSummaryPath $Script:tempPath

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

    Context 'missing variables' -Skip {

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

            $Script:context = "Testing"

            $Script:tempPath = [System.IO.Path]::GetTempFileName()
            $Script:message = "Test message in a bottle"
            Set-Content -Path $Script:tempPath -Value $message
        }

        AfterAll {
            Remove-Item -Path $Script:tempPath
        }

        It 'throws and exception' {
            { New-GitHubSummaryComment -Context $Script:context -TestSummaryPath $Script:tempPath } | Should -Throw
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

Describe 'Get-GitHubPRInfo' {
    Context 'with all env variables present' -Skip {

        BeforeAll {
            $Script:envVariables = @{
                "GITHUB_TOKEN" = "GITHUB_TOKEN";
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
            $changeId = "ChangeId"

            Get-GitHubPRInfo -ChangeId $changeId

            # assert the call and compare the expected parameters to the received ones
            Assert-MockCalled -CommandName Invoke-RestMethod -Times 1 -Scope It -ParameterFilter {
                # validate each of the params and the payload
                if ($Uri -ne "https://api.github.com/repos/xamarin/xamarin-macios/pulls/$changeId") {
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

                return $True
            }

        }

        It 'calls the method with an error and throws' {
            Mock Invoke-RestMethod {
                throw [System.Exception]::("Test")
            }

            $changeId = "ChangeId"

            { Get-GitHubPRInfo -ChangeId $changeId } | Should -Throw
        }

    }
    Context 'without an env variable' -Skip {
        BeforeAll {
            # clear the env vars
            $envVariables = @{
                "GITHUB_TOKEN" = "GITHUB_TOKEN";
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

            $changeId = "ChangeId"

            { Get-GitHubPRInfo -ChangeId $changeId } | Should -Throw
            Assert-MockCalled -CommandName Invoke-RestMethod -Times 0 -Scope It 
        }
    }
}


Describe 'Convert-Markdown' {
    Context 'with all env variables present' {
        BeforeAll {
            $Script:envVariables = @{
                "GITHUB_TOKEN" = "GITHUB_TOKEN";
            }

            $Script:envVariables.GetEnumerator() | ForEach-Object { 
                $key = $_.Key
                Set-Item -Path "Env:$key" -Value $_.Value
            }
        }

        It 'calls the method successfully' {
            Mock New-GistWithFiles {
                return "https://gist.github.com/somethingsomething"
            } -ModuleName 'GitHub'
            $rootDirectory = "root"
            $inputContents = "[vsdrops](whatever) --- [gist](subdir/file) === [gist](inexistent/file)"

            $fullDirectory = Join-Path $rootDirectory "subdir"
            $fullPath = Join-Path $fullDirectory "file"
            New-Item -Path "." -Name $fullDirectory -ItemType "directory" -Force
            Set-Content -Path $fullPath -Value "content"

            $converted = Convert-Markdown -RootDirectory $rootDirectory -InputContents $inputContents -VSDropsPrefix "vsdropsprefix/"

            $converted | Should -BeExactly "[vsdrops](vsdropsprefix/whatever) --- [gist](https://gist.github.com/somethingsomething) === (could not create gist: file 'root/inexistent/file' does not exist)"

            Remove-Item -Path $rootDirectory -Recurse
        }
    }
}
