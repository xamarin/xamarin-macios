<#
System scripts unit tests. 
#>

Import-Module ./System -Force

Describe 'Clear-HD' {

    Context 'when there are no errors and all files are removed' {

        It 'removes all files' {
            # call the method, and check that remove-item was correctly called with each of the files we want to remove

            Mock Remove-Item
            # mock test path to always return true, that is all dirs are present
            Mock Test-Path {
                return $True
            }

            $directories = @(
                "~/.android",
                "~/.Trash/*",
                "~/.nuget",
                "~/.local/share/NuGet",
                "~/Downloads/*",
                "~/Library/Developer/*",
                "~/Library/Caches/*",
                "/Library/Frameworks/Xamarin.Android.framework/Versions",
                "/Library/Developer/CoreSimulator/Profiles/Runtimes",
                "/Applications/Xcode10*.app",
                "/Applications/Xcode_10*.app",
                "/Applications/Xcode11-GM.app",
                "/Applications/Xcode11-GM2.app",
                "/Applications/Xcode11.app",
                "/Applications/Xcode111.app",
                "/Applications/Visual Studio (Preview).app"
            )

            Clear-HD

            Assert-MockCalled -CommandName Remove-Item -Times $directories.Count -Scope It 

        }

        It 'removes only present files' {

            $directories = @(
                "~/.android",
                "~/.Trash/*",
                "~/.nuget",
                "~/.local/share/NuGet",
                "~/Downloads/*",
                "~/Library/Developer/*",
                "~/Library/Caches/*",
                "/Library/Frameworks/Xamarin.Android.framework/Versions",
                "/Library/Developer/CoreSimulator/Profiles/Runtimes",
                "/Applications/Xcode10*.app",
                "/Applications/Xcode_10*.app",
                "/Applications/Xcode11-GM.app",
                "/Applications/Xcode11-GM2.app",
                "/Applications/Xcode11.app",
                "/Applications/Xcode111.app",
                "/Applications/Visual Studio (Preview).app"
            )

            $presentDirectories = @(
                "~/.android",
                "~/.Trash/*",
                "~/.nuget",
                "~/.local/share/NuGet",
                "~/Downloads/*"
            )

            Mock Remove-Item
            Mock Write-Debug
            # mock test path to always return true, that is all dirs are present
            Mock Test-Path {
                return $Path -in $presentDirectories
            }

            Clear-HD

            $debugCalls = $directories.Count - $presentDirectories.Count
            Assert-MockCalled -CommandName Remove-Item -Times $presentDirectories.Count -Scope It 
            Assert-MockCalled -CommandName Write-Debug -Times $debugCalls -Scope It 
        }
    }
}

Describe 'Clear-AfterTests' {
    Context 'default' {
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
    Context 'checks space' {
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
    Context 'default' {
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
