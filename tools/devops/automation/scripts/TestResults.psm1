using module ".\\GitHub.psm1"

class TestResults {
    [string] $ResultsPath # path to the file with the results
    [string] $TestsJobStatus # the value of the env var that lets us know if the tests passed or not can be null or empty
    [string] $Label
    [string] $Context
    hidden [int] $Passed
    hidden [int] $Failed

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
        $this.Passed = -1
        $this.Failed = -1
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

    [object] GetPassedTests() {
        if ($this.Passed -eq -1 -or $this.Failed -eq -1) {
            # the result file is diff if the result was a success or not
            if ($this.IsSuccess()) {
                $this.Failed = 0
                # in this case, the file contains a single line with the number and the following
                # pattern:
                # "# :tada: All 69 tests passed :tada:"
                $regexp = "(# :tada: All )(?<passed>[0-9]+)( tests passed :tada:)"
                $content = Get-Content $this.ResultsPath | Select -First 1
                if ($content -eq "# No tests selected.") {
                    $this.Passed = 0
                } elseif ($content -match $regexp) {
                    $this.Passed = $matches.passed -as [int]
                } else {
                    throw "Unknown result pattern '$content'"
                }
            } else {
                if ($this.TestsJobStatus -eq "") {
                    $this.Passed = -1
                    $this.Failed = -1
                } else {
                    # in this case, the file contains a lot more data, since it has the result summary + the list of
                    # failing tests, for example:
                    #
                    # # Test results
                    # <details>
                    # <summary>4 tests failed, 144 tests passed.</summary>
                    #
                    ## Failed tests
                    #
                    # * fsharp/watchOS 32-bits - simulator/Debug: Crashed
                    # * introspection/watchOS 32-bits - simulator/Debug: Crashed
                    # * dont link/watchOS 32-bits - simulator/Debug: Crashed
                    # * mono-native-compat/watchOS 32-bits - simulator/Debug: Crashed
                    #</details>
                    #
                    # first, we do know that the line we are looking for has <summary> so we find the line (I do not trust
                    # the format so we loop)
                    $content = ""
                    foreach ($line in Get-Content -Path $this.ResultsPath)
                    {
                        if ($line.Contains("<summary>")){
                            $content = $line
                            break
                        }
                    }
                    if ($content) {
                        # we need to parse with a regexp the following
                        # <summary>4 tests failed, 144 tests passed.</summary>
                        $regexp = "(\<summary\>)(?<failed>[0-9]+)( tests failed, )(?<passed>[0-9]+)( tests passed\.\</summary\>)"
                        if ($content -match $regexp) {
                            $this.Passed = $matches.passed -as [int]
                            $this.Failed = $matches.failed -as [int]
                        } else {
                            throw "Unknown result pattern '$content'"
                        }
                    } else {
                        throw "Unknown result pattern of a failed test"
                    }
                }
            }
        }
        return [PSCustomObject]@{
            Passed = $this.Passed
            Failed = $this.Failed
        }
    }
}

class ParallelTestsResults {
    [string] $Context
    [TestResults[]] $Results

    ParallelTestsResults (
        [TestResults[]] $results,
        [string] $context
    ) {
        $this.Results = $results
        $this.Context = $context
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

    [bool] IsSuccess() {
        $failingTests = $this.GetFailingTests()
        return $failingTests.Count -eq 0
    }

    [string] GetTotalTestCount() {
        # each test result knows the failure and the passes. 
        $passedTests = 0
        $failedTests = 0
        foreach($r in $this.Results)
        {
            $result = $r.GetPassedTests()
            $passedTests += $result.Passed 
            $failedTests += $result.Failed 
        }
        # we return the patterns we already know
        if ($failedTests -eq 0) {
            return ":tada: All $passedTests tests passed :tada:"
        } else {
            return "$failedTests tests failed, $passedTests tests passed."
        }
    }

    [void] WriteComment($stringBuilder) {
        $stringBuilder.AppendLine("# Test results")
        # We need to add a small summary at the top. We check if it was a success, if that is 
        # the case, we just need to state it and 
        $failingTests = $this.GetFailingTests()

        if ($failingTests.Count -eq 0) {
            $stringBuilder.AppendLine(":white_check_mark: All tests passed on $($this.Context).")
            $stringBuilder.AppendLine("")
            $stringBuilder.AppendLine($this.GetTotalTestCount())
            # enumerate the tests context and its tests, since it is nice to know
            $stringBuilder.AppendLine("")
            $stringBuilder.AppendLine("## Tests counts")
            foreach($r in $this.Results)
            {
                $stringBuilder.AppendLine("<details><summary>Tests for $($r.Label)</summary>")
                $result = $r.GetPassedTests()
                if ($result.Passed -eq 0) {
                    $stringBuilder.Append("$($r.Context) - No tests selected.")
                } else {
                    $stringBuilder.Append("$($r.Context) - All $($result.Passed) tests passed")
                }
                $stringBuilder.AppendLine("</details>")
            }
        } else {
            $stringBuilder.AppendLine(":x: Tests failed on $($this.Context)") 
            $stringBuilder.AppendLine("")
            $stringBuilder.AppendLine($this.GetTotalTestCount())
            $stringBuilder.AppendLine("")
            $stringBuilder.AppendLine("## Failures")
            # loop over all results and add the content
            foreach ($r in $failingTests)
            {
                # get the result, if -1, we had a crash, else we print the result
                $result = $r.GetPassedTests()
                $stringBuilder.AppendLine("<details><summary>Tests for $($r.Label)</summary>")
                if ($result.Passed -eq -1 -or $result.Failed -eq -1) {
                    $stringBuilder.AppendLine("Tests failed catastrophically on $($r.Context) (no summary found).")
                } else {
                    # create a detail per test result with the name of the test and will contain the exact summary
                    $r.WriteComment($stringBuilder)
                    $stringBuilder.AppendLine("</details>")
                }
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

function New-ParallelTestsResults {
    param (
        [object[]]
        $Results,
        [string]
        $Context
    )
    return [ParallelTestsResults]::new($Results, $Context)
}

Export-ModuleMember -Function New-TestResults
Export-ModuleMember -Function New-ParallelTestsResults
