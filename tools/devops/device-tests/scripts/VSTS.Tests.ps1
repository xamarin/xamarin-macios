<#
VSTS interaction unit tests.
#>

Import-Module ./VSTS -Force

Describe 'Stop-Pipeline' {
    Context 'with all the env vars present' {

        BeforeAll {
            $Script:envVariables = @{
                "SYSTEM_TEAMFOUNDATIONCOLLECTIONURI" = "SYSTEM_TEAMFOUNDATIONCOLLECTIONURI";
                "SYSTEM_TEAMPROJECT" = "SYSTEM_TEAMPROJECT";
                "BUILD_BUILDID" = "BUILD_BUILDID";
                "SYSTEM_ACCESSTOKEN" = "SYSTEM_ACCESSTOKEN"
            }

            $envVariables.GetEnumerator() | ForEach-Object { 
                $key = $_.Key
                Set-Item -Path "Env:$key" -Value $_.Value
            }
        }

        It 'performs the rest call' {
            Mock Invoke-RestMethod {
                return @{"status"=200;}
            }

            Stop-Pipeline

            $expectedUri = "SYSTEM_TEAMFOUNDATIONCOLLECTIONURISYSTEM_TEAMPROJECT/_apis/build/builds/BUILD_BUILDID?api-version=5.1"
            Assert-MockCalled -CommandName Invoke-RestMethod -Times 1 -Scope It -ParameterFilter {
                # validate the paremters
                if ($Uri -ne $expectedUri) {
                    return $False
                }
                
                if ($Headers.Authorization -ne ("Bearer {0}" -f $envVariables["SYSTEM_ACCESSTOKEN"])) {
                    return $False
                }

                if ($Method -ne "PATCH") {
                    return $False
                }

                if ($ContentType -ne "application/json") {
                    return $False
                }

                # compare the payload
                $bodyObj = ConvertFrom-Json $Body
                if ($bodyObj.status -ne "Cancelling") {
                    return $False
                }
                return $True
            }
        }

        It 'performs the rest method with an error' {
            Mock Invoke-RestMethod {
                throw [System.Exception]::("Test")
            }
            #set env vars
            { Stop-Pipeline } | Should -Throw
        }
    }

    Context 'without an env var' {
        BeforeAll {
            $Script:envVariables = @{
                "SYSTEM_TEAMFOUNDATIONCOLLECTIONURI" = "SYSTEM_TEAMFOUNDATIONCOLLECTIONURI";
                "SYSTEM_TEAMPROJECT" = "SYSTEM_TEAMPROJECT";
                "BUILD_BUILDID" = "BUILD_BUILDID";
                "SYSTEM_ACCESSTOKEN" = "SYSTEM_ACCESSTOKEN" 
            }

            $Script:envVariables.GetEnumerator() | ForEach-Object { 
                $key = $_.Key
                Set-Item -Path "Env:$key" -Value $_.Value
                Remove-Item -Path "Env:$key"
            }
        }

        It 'fails calling the rest method' {
            Mock Invoke-RestMethod {
                return @{"status"=200;}
            }

            { Stop-Pipeline } | Should -Throw
            Assert-MockCalled -CommandName Invoke-RestMethod -Times 0 -Scope It 
        }
    }
}