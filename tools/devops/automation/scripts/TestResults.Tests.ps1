using module ".\\TestResults.psm1"

Describe "TestResults tests" {
    BeforeAll {
        $dataPath = Join-Path -Path $PSScriptRoot -ChildPath "test_data" 
        $dataPath = Join-Path -Path $dataPath -ChildPath "TestSummary.md"
        $label = "pwsh"
        $resultContext = "tests"
        $jobStatus = "Succeeded"
    }

    It "is correctly created" {
        $testResult = [TestResults]::new($dataPath, $jobStatus, $label, $resultContext)
        $testResult.ResultsPath | Should -Be $dataPath
        $testResult.TestsJobStatus | Should -Be $jobStatus
        $testResult.Label | Should -Be $label
        $testResult.Context | Should -Be $resultContext
    }

    It "is successfull" {
        $testResult = [TestResults]::new($dataPath, "Succeeded", $label, $resultContext)
        $testResult.IsSuccess() | Should -Be $true
    }

    It "is failure" {
        $testResult = [TestResults]::new($dataPath, "Failure", $label, $resultContext)
        $testResult.IsSuccess() | Should -Be $false
    }

    Context "missing file" {
        BeforeAll {
            $dataPath = Join-Path -Path $PSScriptRoot -ChildPath "test_data" 
            $dataPath = Join-Path -Path $dataPath -ChildPath "MissingFile.md"
            $testResult = [TestResults]::new($dataPath, $jobStatus, $label, $resultContext)
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
            $testResult = [TestResults]::new($dataPath, "", $label, $resultContext)
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
            $testResult = [TestResults]::new($dataPath, $jobStatus, $label, $resultContext)
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
            $testResult = [TestResults]::new($dataPath, "Failure", $label, $resultContext)
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

}
