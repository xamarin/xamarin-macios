using module ".\\GitHub.psm1"

class TestResults {
    [string] $ResultsPath # path to the file with the results
    [string] $TestsJobStatus # the value of the env var that lets us know if the tests passed or not can be null or empty
    [string] $Label
    [string] $Context

    TestResults (
        [string] $path,
        [string] $status,
        [string] $label,
        [string] $context
    ) {
        $this.ResultsPath = $path
        $this.TestsJobStatus = $status
        $this.Label = $label
        $this.Context = $context
    }

    [bool] IsSuccess() {
        return $this.TestsJobStatus -eq "Succeeded"
    }

    [void] WriteComment($stringBuilder) {
        $stringBuilder.AppendLine("## Test results - $(this.Context)")
        $stringBuilder.AppendLine("")
        if (-not (Test-Path $this.ResultsPath -PathType Leaf)) {
             $stringBuilder.AppendLine(":fire: Tests failed catastrophically on $($this.Context) (no summary found).") 
        } else {
            if ($this.TestsJobStatus -eq "") {
                $stringBuilder.AppendLine(":x: Tests didn't execute on $($this.Context).")
            } elseif ($this.IsSuccess()) {
                $stringBuilder.AppendLine(":white_check_mark: Tests passed on $($this.Context).")
                $stringBuilder.AppendLine("")
                foreach ($line in Get-Content -Path $this.ResultsPath)
                {
                    $stringBuilder.AppendLine($line)
                }
            } else {
                $stringBuilder.AppendLine(":x: Tests failed on $($this.Context)") 
                $stringBuilder.AppendLine("")
                foreach ($line in Get-Content -Path $this.ResultsPath)
                {
                    $stringBuilder.AppendLine($line)
                }
            }
        }

    }

    [object] GetStatus() {
        $status = $null
        if (-not (Test-Path $this.ResultsPath -PathType Leaf)) {
            $status = [GitHubStatus]::new("failure", "Tests failed catastrophically on $($this.Context) (no summary found).", $this.Context)
        } else {
            if ($this.TestsJobStatus -eq "") {
                $status = [GitHubStatus]::new("error", "Tests didn't execute on $($this.Context).", $this.Context)
            } elseif ($this.IsSuccess()) {
                $status = [GitHubStatus]::new("success", "All tests passed on $($this.Context).", $this.Context)
            } else {
                $status = [GitHubStatus]::new("error", "Tests failed on $($this.Context).", $this.Context)
            }
        } 
        return $status
    }
}

class ParallelTestsResults {
    [string] $Context
    [TestResults[]] $Results

    ParallelTestsResults (
        [string] $context,
        [TestResults[]] $results
    ) {
        $this.Content = $context
        $this.Results = $results
    }

    [object] GetFailingTests() {
        $failingTests = [System.Collections.ArrayList]@()
        foreach ($result in $this.Results) {
            if (-not $result.IsSuccess()) {
                $failingTests.Add($result)
            }
        }
        return $failingTests
    }

    [string] GetTestCount() {
        return ""
    }

    [void] WriteComment($stringBuilder) {
        $stringBuilder.AppendLine("# Test results")
        # We need to add a small summary at the top. We check if it was a success, if that is 
        # the case, we just need to state it and 
        $failingTests = $this.GetFailingTests()

        if ($failingTests.Count -eq 0) {
            $stringBuilder.AppendLine(":white_check_mark: All tests passed on $($this.Context).")
            $stringBuilder.AppendLine("")
            $stringBuilder.AppendLine($this.GetTestCount())
        } else {
            $stringBuilder.AppendLine(":x: Tests failed on $($this.Context)") 
            $stringBuilder.AppendLine("")
            $stringBuilder.AppendLine($this.GetTestCount())
            $stringBuilder.AppendLine("")
            $stringBuilder.AppendLine("## Failures")
            # loop over all results and add the content
            foreach ($result in $failingTests)
            {
                # create a detail per test result with the name of the test and will contain the exact summary
                $stringBuilder.AppendLine("<details><summary>Tests for $($result.Label)</summary>")
                $result.WriteComment($stringBuilder)
                $stringBuilder.AppendLine("</details>")
            }
        }
    }
}

<# 
    .SYNOPSIS
        Creates a new TestResults object.
#>
function New-TestResults {
    param (
        [string]
        $Path,
        [string]
        $Status,
        [string]
        $Label,
        [string]
        $Context
    )
    return [TestResults]::new($Path, $Status, $Label, $Context)
}

Export-ModuleMember -Function New-TestResults
