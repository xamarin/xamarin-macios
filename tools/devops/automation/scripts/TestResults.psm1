using module ".\\GitHub.psm1"

class TestResults {
    [string] $ResultsPath # path to the file with the results
    [string] $TestsJobStatus # the value of the env var that lets us know if the tests passed or not can be null or empty
    [string] $Label
    [string] $Context
    hidden [int] $Passed
    hidden [int] $Failed
    hidden [string[]] $NotTestSummaryLabels = @("install-source")

    TestResults (
        [string] $path,
        [string] $status,
        [string] $label,
        [string] $context
    ) {
        Write-Debug "TestsResuts::new($path, $status, $label, $context)"
        $this.ResultsPath = $path
        $this.TestsJobStatus = $status
        $this.Label = $label
        $this.Context = $context
        $this.Passed = -1
        $this.Failed = -1
    }

    [bool] IsSuccess() {
        Write-Debug "\t$($this.Label) - IsSuccess()"
        if ($this.NotTestSummaryLabels.Contains($this.Label)) {
            Write-Debug "\t\t$($this.Label) - Found special label $($this.Label), checking only status."
            return $this.TestsJobStatus -eq "Succeeded"
        } else {
            $hasResultsPath = Test-Path $this.ResultsPath -PathType Leaf
            Write-Debug "\t\t$($this.Label) - Path $($this.ResultsPath) exits? $hasResultsPath"
            Write-Debug "\t\t$($this.Label) - Test status: $($this.TestsJobStatus)"
            return $hasResultsPath -and ($this.TestsJobStatus -eq "Succeeded")
        }
    }

    [void] WriteComment($stringBuilder) {
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
        Write-Debug "$($this.Label) - GetPassedTests()"
        if ($this.Passed -eq -1 -or $this.Failed -eq -1) {
            Write-Debug "\t$($this.Label) - Calcualte results."
            # the result file is diff if the result was a success or not
            if ($this.IsSuccess()) {
                Write-Debug "$($this.Label) - IsSuccess() => TRUE"
                $this.Failed = 0
                if ($this.NotTestSummaryLabels.Contains($this.Label)) {
                    Write-Debug "\t\t$($this.Label) - Found special label $($this.Label), adding a single pass."
                    $this.Passed = 1
                } else {
                    # in this case, the file contains a single line with the number and the following
                    # pattern:
                    # "# :tada: All 69 tests passed :tada:"
                    $regexp = "(# :tada: All )(?<passed>[0-9]+)( tests passed :tada:)"
                    $content = Get-Content $this.ResultsPath | Select -First 1
                    if ($content -eq "# No tests selected.") {
                        Write-Debug "\t\tNo tests selected"
                        $this.Passed = 0
                    } elseif ($content -match $regexp) {
                        Write-Debug "Did match regexp"
                        $this.Passed = $matches.passed -as [int]
                        Write-Debug "Passed tests count: $($this.Passed)"
                    } else {
                        throw "Unknown result pattern '$content'"
                    }
                }
            } else {
                Write-Debug "IsSuccess() => FALSE"
                $fileIsPresent = Test-Path $this.ResultsPath -PathType Leaf
                if ($this.TestsJobStatus -eq "" -or -not (Test-Path $this.ResultsPath -PathType Leaf)) {
                    Write-Debug "\t\tTests job status: $($this.TestsJobStatus)"
                    Write-Debug "\t\tNot Found results path: $fileIsPresent"
                    $this.Passed = -2
                    $this.Failed = -2
                } else {
                    if ($this.NotTestSummaryLabels.Contains($this.Label)) {
                        Write-Debug "\t\tFound special label $($this.Label), adding a single fail."
                        $this.Passed = 0
                        $this.Failed = 1
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
                                Write-Debug "\t\tMatched regexpt."
                                $this.Passed = $matches.passed -as [int]
                                $this.Failed = $matches.failed -as [int]
                                Write-Debug "\t\tPassed: $($this.Passed) Failed: $($this.Failed)"
                            } else {
                                throw "Unknown result pattern '$content'"
                            }
                        } else {
                            throw "Unknown result pattern of a failed test"
                        }
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
    [string] $TestPrefix
    [string] $VSDropsIndex
    [TestResults[]] $Results

    ParallelTestsResults (
        [TestResults[]] $results,
        [string] $context,
        [string] $testPrefix,
        [string] $vsDropsIndex
    ) {
        $this.Results = $results
        $this.Context = $context
        $this.TestPrefix = $testPrefix
        $this.VSDropsIndex = $vsDropsIndex
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

    [object] GetSuccessfulTests() {
        $successfulTests = [System.Collections.ArrayList]@()
        foreach ($result in $this.Results) {
            if ($result.IsSuccess()) {
                $successfulTests.Add($result)
            }
        }
        return $successfulTests
    }

    [bool] IsSuccess() {
        $failingTests = $this.GetFailingTests()
        return $failingTests.Count -eq 0
    }

    [string] GetTotalTestCount() {
        # each test result knows the failure and the passes.
        $passedTests = 0
        $failedTests = 0
        $crashedTests = 0
        foreach($r in $this.Results)
        {
            $result = $r.GetPassedTests()
            if ($result.Passed -eq -2 -or $result.Failed -eq -2) {
                $crashedTests += 1
            } else {
                $passedTests += $result.Passed
                $failedTests += $result.Failed
            }
        }

        # we return the patterns we already know
        if ($failedTests -eq 0) {
            return ":tada: All $passedTests tests passed :tada:"
        } else {
            return "$crashedTests tests crashed, $failedTests tests failed, $passedTests tests passed."
        }
    }

    [string] GetDownloadLinks($testResult) {
        $dropsIndex = "$($this.VSDropsIndex)/$($this.TestPrefix)$($testResult.Label)/;/tests/vsdrops_index.html"
        $artifactUrl = "$Env:SYSTEM_TEAMFOUNDATIONCOLLECTIONURI$Env:SYSTEM_TEAMPROJECT/_apis/build/builds/$Env:BUILD_BUILDID/artifacts?artifactName=HtmlReport-$($this.TestPrefix)$($testResult.Label)&api-version=6.0&`$format=zip"
        $downloadInfo = "[Html Report (VSDrops)]($dropsIndex) [Download]($artifactUrl)"
        return $downloadInfo
    }

    [void] PrintSuccessMessage($testResult, $stringBuilder) {
        $downloadInfo = $this.GetDownloadLinks($testResult)
        $result = $testResult.GetPassedTests()
        if ($result.Passed -eq 0) {
            $stringBuilder.AppendLine(":warning: $($testResult.Label): No tests selected. $downloadInfo")
        } else {
            $stringBuilder.AppendLine(":white_check_mark: $($testResult.Label): All $($result.Passed) tests passed. $downloadInfo")
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
                $this.PrintSuccessMessage($r, $stringBuilder)
            }
        } else {
            $stringBuilder.AppendLine(":x: Tests failed on $($this.Context)")
            $stringBuilder.AppendLine("")
            $stringBuilder.AppendLine($this.GetTotalTestCount())
            $stringBuilder.AppendLine("")
            $stringBuilder.AppendLine("## Failures")
            $stringBuilder.AppendLine("")
            # loop over all results and add the content
            foreach ($r in $failingTests)
            {
                $stringBuilder.AppendLine("### :x: $($r.Label) tests")
                $stringBuilder.AppendLine("")
                # print diff messages if the tests crash or if the tests did indeed fail
                # get the result, if -1, we had a crash, else we print the result
                $result = $r.GetPassedTests()
                if ($result.Passed -eq -2 -or $result.Failed -eq -2) {
                    $stringBuilder.AppendLine(":fire: Failed catastrophically on $($r.Context) (no summary found).")
                    $stringBuilder.AppendLine("")
                    $stringBuilder.AppendLine($this.GetDownloadLinks($r))
                    $stringBuilder.AppendLine("")
                } else {
                    # create a detail per test result with the name of the test and will contain the exact summary
                    $stringBuilder.AppendLine("<summary>$($result.Failed) tests failed, $($result.Passed) tests passed.</summary>")
                    if (Test-Path -Path $r.ResultsPath -PathType Leaf) {
                        $stringBuilder.AppendLine("<details>")
                        $stringBuilder.AppendLine("")
                        $foundTests = $false
                        foreach ($line in Get-Content -Path $r.ResultsPath)
                        {
                            if (-not $foundTests) {
                                $foundTests = $line.Contains("## Failed tests")
                            } else {
                                if (-not [string]::IsNullOrEmpty($line)) {
                                    $stringBuilder.AppendLine("$line") # the extra space is needed for the multiline list item
                                }
                            }
                        }
                        $stringBuilder.AppendLine("</details>")
                        $stringBuilder.AppendLine("")
                    } else {
                        $stringBuilder.AppendLine("<details>")
                        $stringBuilder.AppendLine(" Test has no summaty file.")
                        $stringBuilder.AppendLine("</details>")
                    }
                    $stringBuilder.AppendLine("</details>")
                    $stringBuilder.AppendLine("")
                    $stringBuilder.AppendLine($this.GetDownloadLinks($r))
                    $stringBuilder.AppendLine("")
                }
            }
            $successfulTests = $this.GetSuccessfulTests()
            $stringBuilder.AppendLine("## Successes")
            $stringBuilder.AppendLine("")
            foreach ($r in $successfulTests) {
                $this.PrintSuccessMessage($r, $stringBuilder)
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

<#
    .SYNOPSIS
        Creates a new ParallelTestsResult object.
#>
function New-ParallelTestsResults {
    param (
        [object[]]
        $Results,
        [string]
        $Context,
        [string]
        $TestPrefix,
        [string]
        $VSDropsIndex
    )
    return [ParallelTestsResults]::new($Results, $Context, $TestPrefix, $VSDropsIndex)
}

Export-ModuleMember -Function New-TestResults
Export-ModuleMember -Function New-ParallelTestsResults
