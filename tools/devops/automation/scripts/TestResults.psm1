using module ".\\GitHub.psm1"

class TestResults {
    [string] $ResultsPath # path to the file with the results
    [string] $TestsJobStatus # the value of the env var that lets us know if the tests passed or not can be null or empty
    [string] $Label
    [string] $Title
    [string] $Platform
    [string] $Context
    [int] $Attempt
    hidden [int] $Passed
    hidden [int] $Failed
    hidden [string[]] $NotTestSummaryLabels = @("install-source")

    TestResults (
        [string] $path,
        [string] $status,
        [string] $label,
        [string] $title,
        [string] $platform,
        [string] $context,
        [int] $attempt
    ) {
        Write-Debug "TestsResults::new($path, $status, $label, $title, $platform, $context, $attempt)"
        $this.ResultsPath = $path
        $this.TestsJobStatus = $status
        $this.Label = $label
        $this.Title = $title
        $this.Platform = $platform
        $this.Context = $context
        $this.Attempt = $attempt
        $this.Passed = -1
        $this.Failed = -1
    }

    [bool] IsSuccess() {
        Write-Debug "`t$($this.GetLabelWithSuffix(`"`")) - IsSuccess()"
        if ($this.NotTestSummaryLabels.Contains($this.Label)) {
            Write-Debug "`t`t$($this.Label) - Found special label $($this.Label), checking only status."
            return $this.TestsJobStatus -eq "Succeeded"
        } else {
            $hasResultsPath = Test-Path $this.ResultsPath -PathType Leaf
            Write-Debug "`t`t$($this.GetLabelWithSuffix(`"`")) - Path $($this.ResultsPath) exists? $hasResultsPath"
            Write-Debug "`t`t$($this.GetLabelWithSuffix(`"`")) - Test status: $($this.TestsJobStatus)"
            return $hasResultsPath -and ($this.TestsJobStatus -eq "Succeeded")
        }
    }

    [string] GetAttemptText() {
        if ($this.Attempt -gt 1) {
            return " [attempt $($this.Attempt)]"
        }
        return ""
    }

    [string] GetLabelSuffix() {
        if ($this.Platform -eq "Multiple") {
            return " (Multiple platforms)"
        } elseif ($this.Platform -eq "") {
            return ""
        } else {
            return " ($($this.Platform))"
        }
    }

    [string] GetLabelWithSuffix([string] $infix) {
        return $this.Label + $infix + $this.GetLabelSuffix()
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
        Write-Debug "$($this.Title) - GetPassedTests()"
        if ($this.Passed -eq -1 -or $this.Failed -eq -1) {
            Write-Debug "`t$($this.Title) - Calculate results."
            # the result file is diff if the result was a success or not
            if ($this.IsSuccess()) {
                Write-Debug "`t$($this.Title) - IsSuccess() => TRUE"
                $this.Failed = 0
                if ($this.NotTestSummaryLabels.Contains($this.Label)) {
                    Write-Debug "`t`t$($this.Title) - Found special label $($this.Label), adding a single pass."
                    $this.Passed = 1
                } else {
                    # in this case, the file contains a single line with the number and the following
                    # pattern:
                    # "# :tada: All 69 tests passed :tada:"
                    $regexp = "(# :tada: All )(?<passed>[0-9]+)( tests passed :tada:)"
                    $content = Get-Content $this.ResultsPath | Select -First 1
                    if ($content -eq "# No tests selected.") {
                        Write-Debug "`t`tNo tests selected"
                        $this.Passed = 0
                    } elseif ($content -match $regexp) {
                        $this.Passed = $matches.passed -as [int]
                        Write-Debug "`tPassed tests count: $($this.Passed)"
                    } else {
                        throw "Unable to understand the test result '$content' for test '$($this.GetLabelWithSuffix(`"`"))'"
                    }
                }
            } else {
                Write-Debug "`t$($this.GetLabelWithSuffix(`"`")) - IsSuccess() => FALSE"
                $fileIsPresent = Test-Path $this.ResultsPath -PathType Leaf
                if ($this.TestsJobStatus -eq "" -or -not (Test-Path $this.ResultsPath -PathType Leaf)) {
                    Write-Debug "`t`tTests job status: $($this.TestsJobStatus)"
                    Write-Debug "`t`tNot Found results path: $fileIsPresent"
                    $this.Passed = -2
                    $this.Failed = -2
                } else {
                    if ($this.NotTestSummaryLabels.Contains($this.Label)) {
                        Write-Debug "`t`tFound special label $($this.Label), adding a single fail."
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
                                Write-Debug "`t`tMatched regexpt."
                                $this.Passed = $matches.passed -as [int]
                                $this.Failed = $matches.failed -as [int]
                            } else {
                                Write-Debug "`t`tAdding a single fail because unexpected <summary> contents found: $($content)"
                                $this.Passed = 0
                                $this.Failed = 1
                            }
                        } else {
                            Write-Debug "`t`tNo <summary> found, adding a single fail"
                            $this.Passed = 0
                            $this.Failed = 1
                        }
                    }
                }
            }
        }
        Write-Debug "`t$($this.GetLabelWithSuffix(`"`")) - Passed: $($this.Passed) Failed: $($this.Failed)"
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
        if ($failedTests -eq 0 -and $crashedTests -eq 0) {
            return ":tada: All $passedTests tests passed :tada:"
        } else {
            return "$crashedTests tests crashed, $failedTests tests failed, $passedTests tests passed."
        }
    }

    [string] GetDownloadLinks($testResult) {
        $dropsIndex = "$($this.VSDropsIndex)/$($this.TestPrefix)$($testResult.Title)-$($testResult.Attempt)/;/tests/vsdrops_index.html"
        $artifactUrl = "$Env:SYSTEM_TEAMFOUNDATIONCOLLECTIONURI$Env:SYSTEM_TEAMPROJECT/_apis/build/builds/$Env:BUILD_BUILDID/artifacts?artifactName=HtmlReport-$($this.TestPrefix)$($testResult.Title)-$($testResult.Attempt)&api-version=6.0&`$format=zip"
        $downloadInfo = "[Html Report (VSDrops)]($dropsIndex) [Download]($artifactUrl)"
        return $downloadInfo
    }

    [void] PrintSuccessMessage($testResult, $stringBuilder) {
        $downloadInfo = $this.GetDownloadLinks($testResult)
        $result = $testResult.GetPassedTests()
        $attemptText = $testResult.GetAttemptText()
        if ($result.Passed -eq 0) {
            $stringBuilder.AppendLine(":warning: $($testResult.GetLabelWithSuffix(`"`")): No tests selected.$attemptText $downloadInfo")
        } else {
            $stringBuilder.AppendLine(":white_check_mark: $($testResult.GetLabelWithSuffix(`"`")): All $($result.Passed) tests passed.$attemptText $downloadInfo")
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
                $attemptText = $r.GetAttemptText()
                $stringBuilder.AppendLine("### :x: $($r.GetLabelWithSuffix(`" tests`"))$attemptText")
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
                    $stringBuilder.AppendLine("<details>")
                    if (Test-Path -Path $r.ResultsPath -PathType Leaf) {
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
                    } else {
                        $stringBuilder.AppendLine(" Test has no summary file.")
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

        $stringBuilder.AppendLine()
        $stringBuilder.AppendLine("[comment]: <> (This is a test result report added by Azure DevOps)")
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
        $Title,
        [string]
        $Platform,
        [string]
        $Context,
        [int]
        $Attempt
    )
    return [TestResults]::new($Path, $Status, $Label, $Title, $Platform, $Context, $Attempt)
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

<#
    .SYNOPSIS
        Find test results in a directory
#>
function New-TestSummaryResults {
    param (
        [Parameter(Mandatory)]
        [string]
        $Path,
        [Parameter(Mandatory)]
        [string]
        $TestPrefix,
        [Parameter(Mandatory)]
        [string]
        $Dependencies,
        [string]
        $TestConfigurations,
        [string]
        $UploadPrefix=""
    )

    $testResults = [System.Collections.ArrayList]@()
    $dep = $Dependencies | ConvertFrom-Json -AsHashtable
    $outputs = $dep.tests.outputs
    $tests = [ordered]@{}
    foreach ($name in $outputs.Keys) {
        if ($name.EndsWith("TESTS_LABEL")) {
            $label = $outputs[$name]
            $namePrefix = $name.Replace("TESTS_LABEL","")
            $title = $name.Substring(0, $name.IndexOf('.'))
            $testResult = [PSCustomObject]@{
                Label = $label
                Title = $title
                Status = $outputs[$namePrefix + "TESTS_JOBSTATUS"]
                Bot = $outputs[$namePrefix + "TESTS_BOT"]
                Platform = $outputs[$namePrefix + "TESTS_PLATFORM"]
                Attempt = $outputs[$namePrefix + "TESTS_ATTEMPT"]
            }
            if ($tests.Contains($label)) {
                $testInfo = $tests[$label]
            } else {
                $testInfo = [System.Collections.ArrayList]@()
                $tests[$label] = $testInfo
            }
            $testInfo.Add($testResult) | Out-Null
            Write-Debug "Added $label to tests ($title) Name: $name status: $($testResult.Status) bot: $($testResult.Bot) Platform: $($testResult.Platform) Attempt: $($testResult.Attempt)"
        }
    }

    foreach ($testGroup in $tests.GetEnumerator()) {
        foreach ($testResult in $testGroup.Value) {
            Write-Debug "Result: $testResult"
            $label = $testResult.Label
            $title = $testResult.Title
            $status = $testResult.Status
            $testAttempt = $testResult.Attempt
            $platform = $testResult.Platform

            $testSummaryPath = Join-Path "$Path" "${UploadPrefix}TestSummary-$TestPrefix$title-$testAttempt" "TestSummary.md"

            Write-Debug "Test results for $label on attempt $testAttempt is '$status' in $testSummaryPath"

            if (-not (Test-Path -Path $testSummaryPath -PathType Leaf)) {
                Write-Host "WARNING: Path $testSummaryPath does not exist"
            }

            $result = New-TestResults -Path $testSummaryPath -Status $status -Label $label -Title $title -Platform $platform -Context "$Env:CONTEXT - $title" -Attempt $testAttempt
            $testResults += $result
        }
    }

    return $testResults
}

Export-ModuleMember -Function New-TestResults
Export-ModuleMember -Function New-ParallelTestsResults
Export-ModuleMember -Function New-TestSummaryResults
