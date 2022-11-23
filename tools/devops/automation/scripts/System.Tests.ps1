<#
System scripts unit tests. 
#>

Import-Module ./System -Force

Describe 'Clear-AfterTests' {
    Context 'default' -Skip {
        It 'removes the expected files' {

            Mock Remove-Item
            # mock test path to always return true, that is all dirs are present
            Mock Test-Path {
                return $True
            }

            $directories = @(
                "/Applications/Visual\ Studio*",
                "~/Library/Caches/VisualStudio",
                "~/Library/Logs/VisualStudio",
                "~/Library/VisualStudio",
                "~/Library/Preferences/Xamarin",
                "~/Library/Caches/com.xamarin.provisionator"
            )

            Clear-AfterTests 

            Assert-MockCalled -CommandName Remove-Item -Times $directories.Count -Scope It 
        }
    }
}
Describe 'Test-HDFreeSpace' {
    Context 'checks space' -Skip {
        It 'returns TRUE with enough space' {
            Mock Get-PSDrive {
                [PSCustomObject]@{ Free = 539715158016 }
            }

            Test-HDFreeSpace -Size 50 | Should -Be $True
        }

        It 'returns FALSE with not enough space' {
            Mock Get-PSDrive {
                [PSCustomObject]@{ Free = 900 }
            }

            Test-HDFreeSpace -Size 50 | Should -Be $False
        }
    }
}

Describe 'Clear-XamarinProcesses' {
    Context 'default' -Skip {
        It 'kills all processes' {
            Mock Start-Process

            # ensure that all the processes are correctly killed via pkill
            Clear-XamarinProcesses

            Assert-MockCalled -CommandName Start-Process -ParameterFilter { $FilePath -eq "pkill" -and $ArgumentList -eq "-9 mlaunch"} -Times 1 -Exactly
            Assert-MockCalled -CommandName Start-Process -ParameterFilter { $FilePath -eq "pkill" -and $ArgumentList -eq "-9 -f mono.*xharness.exe"} -Times 1 -Exactly
            Assert-MockCalled -CommandName Start-Process -ParameterFilter { $FilePath -eq "pkill" -and $ArgumentList -eq "-9 -f ssh.*rsync.*xamarin-storage"} -Times 1 -Exactly
        }
    }
}
