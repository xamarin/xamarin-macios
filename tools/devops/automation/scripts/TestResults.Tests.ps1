using module ".\\TestResults.psm1"

Describe "TestResults tests" {
    BeforeAll {
        $dataPath = Join-Path -Path $PSScriptRoot -ChildPath "test_data" 
        $dataPath = Join-Path -Path $dataPath -ChildPath "TestSummary.md"
        $label = "pwsh"
        $resultContext = "tests"
        $jobStatus = "Succeeded"
        $attempt = 1
    }

    It "is correctly created" {
        $testResult = [TestResults]::new($dataPath, $jobStatus, $label, $resultContext, $attempt)
        $testResult.ResultsPath | Should -Be $dataPath
        $testResult.TestsJobStatus | Should -Be $jobStatus
        $testResult.Label | Should -Be $label
        $testResult.Context | Should -Be $resultContext
    }

    It "is successfull" {
        $testResult = [TestResults]::new($dataPath, "Succeeded", $label, $resultContext, $attempt)
        $testResult.IsSuccess() | Should -Be $true
    }

    It "is failure" {
        $testResult = [TestResults]::new($dataPath, "Failure", $label, $resultContext, $attempt)
        $testResult.IsSuccess() | Should -Be $false
    }

    Context "missing file" {
        BeforeAll {
            $dataPath = Join-Path -Path $PSScriptRoot -ChildPath "test_data" 
            $dataPath = Join-Path -Path $dataPath -ChildPath "MissingFile.md"
            $testResult = [TestResults]::new($dataPath, $jobStatus, $label, $resultContext, $attempt)
        }

        It "writes the correct comment." {
            $sb = [System.Text.StringBuilder]::new()
            $testResult.WriteComment($sb)
            $content = $sb.ToString()
            $content.Contains(":fire: Tests failed catastrophically on $($testResult.Context) (no summary found).") | Should -Be $true 
        }

        It "returns the correct status." {
            $status = $testResult.GetStatus()
            $status.Status | Should -Be "failure"
            $status.Context | Should -Be $testResult.Context
            $status.Description | Should -Be "Tests failed catastrophically on $($testResult.Context) (no summary found)."
        }
    }

    Context "missing job status" {
        BeforeAll {
            $dataPath = Join-Path -Path $PSScriptRoot -ChildPath "test_data" 
            $dataPath = Join-Path -Path $dataPath -ChildPath "TestSummary.md"
            $testResult = [TestResults]::new($dataPath, "", $label, $resultContext, $attempt)
        }

        It "writes the correct comment." {
            $sb = [System.Text.StringBuilder]::new()
            $testResult.WriteComment($sb)
            $content = $sb.ToString()
            $content.Contains(":x: Tests didn't execute on $($testResult.Context).") | Should -Be $true 
        }

        It "returns the correct status." {
            $status = $testResult.GetStatus()
            $status.Status | Should -Be "error"
            $status.Context | Should -Be $testResult.Context
            $status.Description | Should -Be "Tests didn't execute on $($testResult.Context)."
        }
    }

    Context "success job status" {
        BeforeAll {
            $dataPath = Join-Path -Path $PSScriptRoot -ChildPath "test_data" 
            $dataPath = Join-Path -Path $dataPath -ChildPath "TestSummary.md"
            $testResult = [TestResults]::new($dataPath, $jobStatus, $label, $resultContext, $attempt)
        }

        It "writes the correct comment." {
            $sb = [System.Text.StringBuilder]::new()
            $testResult.WriteComment($sb)
            $content = $sb.ToString()
            $content.Contains(":white_check_mark: Tests passed on $($testResult.Context).") | Should -Be $true 

            # assert that each of the lines in the data file are present in the sb
            foreach ($line in Get-Content -Path $testResult.ResultsPath)
            {
                $content.Contains($line) | Should -Be $true 
            }
        }

        It "returns the correct status." {
            $status = $testResult.GetStatus()
            $status.Status | Should -Be "success"
            $status.Context | Should -Be $testResult.Context
            $status.Description | Should -Be "All tests passed on $($testResult.Context)."
        }
    }

    Context "error job status" {
        BeforeAll {
            $testResult = [TestResults]::new($dataPath, "Failure", $label, $resultContext, $attempt)
        }

        It "writes the correct comment." {
            $sb = [System.Text.StringBuilder]::new()
            $testResult.WriteComment($sb)
            $content = $sb.ToString()
            $content.Contains(":x: Tests failed on $($this.Context)") | Should -Be $true 

            # assert that each of the lines in the data file are present in the sb
            foreach ($line in Get-Content -Path $testResult.ResultsPath)
            {
                $content.Contains($line) | Should -Be $true 
            }
        }

        It "returns the correct status." {
            $status = $testResult.GetStatus()
            $status.Status | Should -Be "error"
            $status.Context | Should -Be $testResult.Context
            $status.Description | Should -Be "Tests failed on $($testResult.Context)."
        }
    }

    Context "new test summmary results" {
        It "finds the right stuff" {
            [Environment]::SetEnvironmentVariable("TESTS_JOBSTATUS_LINKER", "yay")
            [Environment]::SetEnvironmentVariable("TESTS_JOBSTATUS_INTROSPECTION", "nay")

            $testDirectory = Join-Path "." "subdir"
            New-Item -Path "$testDirectory" -ItemType "directory" -Force
            New-Item -Path "$testDirectory/TestSummary-prefixlinker-1" -Name "TestSummary.md" -Value "SummaryA" -Force
            New-Item -Path "$testDirectory/TestSummary-prefixlinker-2" -Name "TestSummary.md" -Value "SummaryB" -Force
            New-Item -Path "$testDirectory/TestSummary-prefixlinker-200" -Name "TestSummary.md" -Value "SummaryC" -Force
            New-Item -Path "$testDirectory/TestSummary-prefixlinker-3" -Name "TestSummary.md" -Value "SummaryD" -Force
            New-Item -Path "$testDirectory/TestSummary-prefixintrospection-2" -Name "TestSummary.md" -Value "SummaryE" -Force
            New-Item -Path "$testDirectory/TestSummary-prefixmtouch-3" -Name "TestSummary.md" -Value "SummaryF" -Force

            $labels = "linker;introspection".Split(";")
            $testResults = New-TestSummaryResults -Path "$testDirectory" -Labels $labels -TestPrefix "prefix"

            Remove-Item -Path $testDirectory -Recurse

            $testResults.count | Should -Be 2

            $testResults[0].Label | Should -Be "linker"
            $testResults[0].Context | Should -Be " - linker"
            $testResults[0].ResultsPath | Should -Be "$Env:PWD/subdir/TestSummary-prefixlinker-200/TestSummary.md"
            $testResults[0].TestsJobStatus | Should -Be "yay"

            $testResults[1].Label | Should -Be "introspection"
            $testResults[1].Context | Should -Be " - introspection"
            $testResults[1].ResultsPath | Should -Be "$Env:PWD/subdir/TestSummary-prefixintrospection-2/TestSummary.md"
            $testResults[1].TestsJobStatus | Should -Be "nay"
        }
    }

}
