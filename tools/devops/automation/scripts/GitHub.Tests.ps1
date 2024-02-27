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
                "BUILD_SOURCEVERSION" = "BUILD_SOURCEVERSION";
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
                if ($Uri -ne "https://api.github.com/repos/xamarin/xamarin-macios/commits/BUILD_SOURCEVERSION/comments") {
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
                "BUILD_SOURCEVERSION" = "BUILD_SOURCEVERSION";
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
