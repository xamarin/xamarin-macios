using module ".\\TestResults.psm1"

$DebugPreference = "Continue"

Describe "TestResults tests" {
    BeforeAll {
        $dataPath = Join-Path -Path $PSScriptRoot -ChildPath "test_data" 
        $dataPath = Join-Path -Path $dataPath -ChildPath "TestSummary.md"
        $label = "pwsh"
        $title = "title"
        $platform = "iOS"
        $resultContext = "tests"
        $suite = [TestSuite]::new($label)
        $testConfig = [TestConfiguration]::new($suite, $title, $platform, $resultContext)
        $jobStatus = "Succeeded"
        $attempt = 1
        $matrix = @"
{
    "cecil":
    {
        "LABEL": "cecil",
        "TESTS_LABELS": "--label=skip-all-tests,run-ios-64-tests,run-ios-simulator-tests,run-tvos-tests,run-watchos-tests,run-mac-tests,run-maccatalyst-tests,run-dotnet-tests,run-system-permission-tests,run-legacy-xamarin-tests,run-cecil-tests",
        "LABEL_WITH_PLATFORM": "cecil",
        "STATUS_CONTEXT": "VSTS: simulator tests - cecil",
        "TEST_PREFIX": "simulator_cecil",
        "TEST_PLATFORM": ""
    },
    "dotnettests_iOS":
    {
        "LABEL": "dotnettests",
        "TESTS_LABELS": "--label=skip-all-tests,run-ios-64-tests,run-ios-simulator-tests,run-tvos-tests,run-watchos-tests,run-mac-tests,run-maccatalyst-tests,run-dotnet-tests,run-system-permission-tests,run-legacy-xamarin-tests,run-dotnettests-tests",
        "LABEL_WITH_PLATFORM": "dotnettests_iOS",
        "STATUS_CONTEXT": "VSTS: simulator tests - dotnettests - iOS",
        "TEST_PREFIX": "simulator_dotnettests_iOS",
        "TEST_PLATFORM": "iOS",
        "TEST_FILTER": "Category != MultiPlatform"
    },
    "dotnettests_macOS":
    {
        "LABEL": "dotnettests",
        "TESTS_LABELS": "--label=skip-all-tests,run-ios-64-tests,run-ios-simulator-tests,run-tvos-tests,run-watchos-tests,run-mac-tests,run-maccatalyst-tests,run-dotnet-tests,run-system-permission-tests,run-legacy-xamarin-tests,run-dotnettests-tests",
        "LABEL_WITH_PLATFORM": "dotnettests_macOS",
        "STATUS_CONTEXT": "VSTS: simulator tests - dotnettests - macOS",
        "TEST_PREFIX": "simulator_dotnettests_macOS",
        "TEST_PLATFORM": "macOS",
        "TEST_FILTER": "Category != MultiPlatform"
    },
    "dotnettests_tvOS":
    {
        "LABEL": "dotnettests",
        "TESTS_LABELS": "--label=skip-all-tests,run-ios-64-tests,run-ios-simulator-tests,run-tvos-tests,run-watchos-tests,run-mac-tests,run-maccatalyst-tests,run-dotnet-tests,run-system-permission-tests,run-legacy-xamarin-tests,run-dotnettests-tests",
        "LABEL_WITH_PLATFORM": "dotnettests_tvOS",
        "STATUS_CONTEXT": "VSTS: simulator tests - dotnettests - tvOS",
        "TEST_PREFIX": "simulator_dotnettests_tvOS",
        "TEST_PLATFORM": "tvOS",
        "TEST_FILTER": "Category != MultiPlatform"
    },
    "dotnettests_MacCatalyst":
    {
        "LABEL": "dotnettests",
        "TESTS_LABELS": "--label=skip-all-tests,run-ios-64-tests,run-ios-simulator-tests,run-tvos-tests,run-watchos-tests,run-mac-tests,run-maccatalyst-tests,run-dotnet-tests,run-system-permission-tests,run-legacy-xamarin-tests,run-dotnettests-tests",
        "LABEL_WITH_PLATFORM": "dotnettests_MacCatalyst",
        "STATUS_CONTEXT": "VSTS: simulator tests - dotnettests - MacCatalyst",
        "TEST_PREFIX": "simulator_dotnettests_MacCatalyst",
        "TEST_PLATFORM": "MacCatalyst",
        "TEST_FILTER": "Category != MultiPlatform"
    },
    "dotnettests_Multiple":
    {
        "LABEL": "dotnettests",
        "TESTS_LABELS": "--label=skip-all-tests,run-ios-64-tests,run-ios-simulator-tests,run-tvos-tests,run-watchos-tests,run-mac-tests,run-maccatalyst-tests,run-dotnet-tests,run-system-permission-tests,run-legacy-xamarin-tests,run-dotnettests-tests",
        "LABEL_WITH_PLATFORM": "dotnettests_Multiple",
        "STATUS_CONTEXT": "VSTS: simulator tests - dotnettests - Multiple",
        "TEST_PREFIX": "simulator_dotnettests_Multiple",
        "TEST_PLATFORM": "",
        "TEST_FILTER": "Category = MultiPlatform"
    }
}
"@
        $dependencies = @"
{
  "tests": {
    "outputs": {
      "cecil.bash21.TESTS_BOT": "xambot-1199.Ventura",
      "cecil.runTests.TESTS_JOBSTATUS": "Succeeded",
      "cecil.bash21.TESTS_LABEL": "cecil",
      "cecil.bash21.TESTS_PLATFORM": "",
      "cecil.bash21.TESTS_ATTEMPT": "1",

      "dotnettests_tvOS.runTests.TESTS_BOT": "XAMMINI-006.Ventura",
      "dotnettests_tvOS.runTests.TESTS_JOBSTATUS": "Failed",
      "dotnettests_tvOS.runTests.TESTS_LABEL": "dotnettests",
      "dotnettests_tvOS.runTests.TESTS_PLATFORM": "tvOS",
      "dotnettests_tvOS.runTests.TESTS_ATTEMPT": "1",

      "dotnettests_MacCatalyst.runTests.TESTS_BOT": "XAMMINI-013.Ventura",
      "dotnettests_MacCatalyst.runTests.TESTS_JOBSTATUS": "Failed",
      "dotnettests_MacCatalyst.runTests.TESTS_LABEL": "dotnettests",
      "dotnettests_MacCatalyst.runTests.TESTS_PLATFORM": "MacCatalyst",
      "dotnettests_MacCatalyst.runTests.TESTS_ATTEMPT": "1",

      "dotnettests_macOS.runTests.TESTS_BOT": "XAMMINI-011.Ventura",
      "dotnettests_macOS.runTests.TESTS_JOBSTATUS": "Failed",
      "dotnettests_macOS.runTests.TESTS_LABEL": "dotnettests",
      "dotnettests_macOS.runTests.TESTS_PLATFORM": "macOS",
      "dotnettests_macOS.runTests.TESTS_ATTEMPT": "1",

      "dotnettests_iOS.runTests.TESTS_BOT": "XAMMINI-010.Ventura",
      "dotnettests_iOS.runTests.TESTS_JOBSTATUS": "Failed",
      "dotnettests_iOS.runTests.TESTS_LABEL": "dotnettests",
      "dotnettests_iOS.runTests.TESTS_PLATFORM": "iOS",
      "dotnettests_iOS.runTests.TESTS_ATTEMPT": "1",

      "dotnettests_Multiple.runTests.TESTS_BOT": "XAMMINI-008.Ventura",
      "dotnettests_Multiple.runTests.TESTS_JOBSTATUS": "Succeeded",
      "dotnettests_Multiple.runTests.TESTS_LABEL": "dotnettests",
      "dotnettests_Multiple.runTests.TESTS_PLATFORM": "Multiple",
      "dotnettests_Multiple.runTests.TESTS_ATTEMPT": "1",
    }
  }
}
"@
        $stageDependencies = @"
{    
  "configure_build": {
    "configure": {
      "outputs": {
        "test_matrix.TEST_MATRIX": "$($matrix.Replace("`n", "\n").Replace("`"", "\`""))"
      }
    }
  }
}
"@

        $dependenciesWithMissingResults = @"
{
  "tests": {
    "outputs": {
      "cecil.runTests.TESTS_BOT": "xambot-1199.Ventura",
      "cecil.runTests.TESTS_JOBSTATUS": "Succeeded",
      "cecil.runTests.TESTS_LABEL": "cecil",
      "cecil.runTests.TESTS_PLATFORM": "",
      "cecil.runTests.TESTS_ATTEMPT": "1",

      "dotnettests_Multiple.runTests.TESTS_BOT": "XAMMINI-008.Ventura",
      "dotnettests_Multiple.runTests.TESTS_JOBSTATUS": "Succeeded",
      "dotnettests_Multiple.runTests.TESTS_LABEL": "dotnettests",
      "dotnettests_Multiple.runTests.TESTS_PLATFORM": "Multiple",
      "dotnettests_Multiple.runTests.TESTS_ATTEMPT": "1",
    }
  }
}
"@
    }

    It "is correctly created" {
        $testResult = [TestResult]::new($dataPath, $jobStatus, $testConfig, $attempt)
        $testResult.ResultsPath | Should -Be $dataPath
        $testResult.TestsJobStatus | Should -Be $jobStatus
        $testResult.Label | Should -Be $label
        $testResult.Title | Should -Be $title
        $testResult.Platform | Should -Be $platform
        $testResult.Context | Should -Be $resultContext
    }

    It "is successfull" {
        $testResult = [TestResult]::new($dataPath, "Succeeded", $testConfig, $attempt)
        $testResult.IsSuccess() | Should -Be $true
    }

    It "is failure" {
        $testResult = [TestResult]::new($dataPath, "Failure", $testConfig, $attempt)
        $testResult.IsSuccess() | Should -Be $false
    }

    Context "missing file" {
        BeforeAll {
            $dataPath = Join-Path -Path $PSScriptRoot -ChildPath "test_data" 
            $dataPath = Join-Path -Path $dataPath -ChildPath "MissingFile.md"
            $testResult = [TestResult]::new($dataPath, $jobStatus, $testConfig, $attempt)
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
            $testResult = [TestResult]::new($dataPath, "", $testConfig, $attempt)
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
            $testResult = [TestResult]::new($dataPath, $jobStatus, $testConfig, $attempt)
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
            $testResult = [TestResult]::new($dataPath, "Failure", $testConfig, $attempt)
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

    Context "new test summmary results" -Skip {
        It "finds the right stuff" {
            $testDirectory = Join-Path "." "subdir"
            New-Item -Path "$testDirectory" -ItemType "directory" -Force
            New-Item -Path "$testDirectory/TestSummary-prefixcecil-1" -Name "TestSummary.md" -Value "SummaryA" -Force
            New-Item -Path "$testDirectory/TestSummary-prefixcecil-2" -Name "TestSummary.md" -Value "SummaryB" -Force
            New-Item -Path "$testDirectory/TestSummary-prefixdotnettests_iOS-1" -Name "TestSummary.md" -Value "SummaryC" -Force
            New-Item -Path "$testDirectory/TestSummary-prefixdotnettests_tvOS-1" -Name "TestSummary.md" -Value "SummaryD" -Force
            New-Item -Path "$testDirectory/TestSummary-prefixdotnettests_MacCatalyst-1" -Name "TestSummary.md" -Value "SummaryE" -Force
            New-Item -Path "$testDirectory/TestSummary-prefixdotnettests_macOS-1" -Name "TestSummary.md" -Value "SummaryF" -Force
            New-Item -Path "$testDirectory/TestSummary-prefixdotnettests_Multiple-1" -Name "TestSummary.md" -Value "SummaryF" -Force

            $testResults = New-TestSummaryResults -Path "$testDirectory" -TestPrefix "prefix" -Dependencies "$dependencies"

            # Remove-Item -Path $testDirectory -Recurse

            Write-Host $testResults

            $testResults.count | Should -Be 12

            $testResults[0].Label | Should -Be "linker"
            $testResults[0].Context | Should -Be " - linker"
            $testResults[0].ResultsPath | Should -Be "$(get-location)/subdir/TestSummary-prefixlinker-200/TestSummary.md"
            $testResults[0].TestsJobStatus | Should -Be "yay"

            $testResults[1].Label | Should -Be "introspection"
            $testResults[1].Context | Should -Be " - introspection"
            $testResults[1].ResultsPath | Should -Be "$(get-location)/subdir/TestSummary-prefixintrospection-2/TestSummary.md"
            $testResults[1].TestsJobStatus | Should -Be "nay"

            $testResults[2].Label | Should -Be "monotouch_test"
            $testResults[2].Context | Should -Be " - monotouch_test"
            $testResults[2].ResultsPath | Should -Be "./subdir/TestSummary-prefixmonotouch_test-1/TestSummary.md"
            $testResults[2].TestsJobStatus | Should -Be ""
        }

        It "computes the right summary with missing test results 2" {
            $VerbosePreference = "Continue"
            $DebugPreference = "Continue"
            $Env:MyVerbosePreference = 'Continue'

            $testDirectory = Join-Path "." "subdir"
            New-Item -Path "$testDirectory" -ItemType "directory" -Force
            New-Item -Path "$testDirectory/TestSummary-prefixcecil-1" -Name "TestSummary.md" -Value "# :tada: All 1 tests passed :tada:" -Force
            New-Item -Path "$testDirectory/TestSummary-prefixdotnettests_Multiple-1" -Name "TestSummary.md" -Value "# :tada: All 7 tests passed :tada:" -Force

            $parallelResults = New-ParallelTestsResults -Path "$testDirectory" -TestPrefix "prefix" -Dependencies "$dependenciesWithMissingResults" -Context "context" -VSDropsIndex "vsdropsIndex"

            $parallelResults.IsSuccess() | Should -Be $false

            $sb = [System.Text.StringBuilder]::new()
            $parallelResults.WriteComment($sb)

            Remove-Item -Path $testDirectory -Recurse

            $content = $sb.ToString()

            Write-Host $content

            $content | Should -Be "# Test results
:x: Tests failed on context

0 tests crashed, 5 tests failed, 27 tests passed.

## Failures

### :x: dotnettests tests (MacCatalyst)

<summary>5 tests failed, 6 tests passed.</summary>
<details>

</details>

[Html Report (VSDrops)](vsdropsIndex/prefixdotnettests_MacCatalyst-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-prefixdotnettests_MacCatalyst-1&api-version=6.0&`$format=zip)

## Successes

:white_check_mark: cecil: All 1 tests passed. [Html Report (VSDrops)](vsdropsIndex/prefixcecil-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-prefixcecil-1&api-version=6.0&`$format=zip)
:white_check_mark: dotnettests (tvOS): All 4 tests passed. [Html Report (VSDrops)](vsdropsIndex/prefixdotnettests_tvOS-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-prefixdotnettests_tvOS-1&api-version=6.0&`$format=zip)
:white_check_mark: dotnettests (macOS): All 6 tests passed. [Html Report (VSDrops)](vsdropsIndex/prefixdotnettests_macOS-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-prefixdotnettests_macOS-1&api-version=6.0&`$format=zip)
:white_check_mark: dotnettests (iOS): All 3 tests passed. [Html Report (VSDrops)](vsdropsIndex/prefixdotnettests_iOS-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-prefixdotnettests_iOS-1&api-version=6.0&`$format=zip)
:white_check_mark: dotnettests (Multiple platforms): All 7 tests passed. [Html Report (VSDrops)](vsdropsIndex/prefixdotnettests_Multiple-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-prefixdotnettests_Multiple-1&api-version=6.0&`$format=zip)

[comment]: <> (This is a test result report added by Azure DevOps)
"
        }
    }

    Context "new test summmary results" {
        It "computes the right summary with missing test results" {
            $VerbosePreference = "Continue"
            $Env:MyVerbosePreference = 'Continue'

            $testDirectory = Join-Path "." "subdir"
            New-Item -Path "$testDirectory" -ItemType "directory" -Force
            New-Item -Path "$testDirectory/TestSummary-prefixcecil-1" -Name "TestSummary.md" -Value "# :tada: All 1 tests passed :tada:" -Force
            New-Item -Path "$testDirectory/TestSummary-prefixcecil-2" -Name "TestSummary.md" -Value "# :tada: All 2 tests passed :tada:" -Force
            New-Item -Path "$testDirectory/TestSummary-prefixdotnettests_iOS-1" -Name "TestSummary.md" -Value "# :tada: All 3 tests passed :tada:" -Force
            New-Item -Path "$testDirectory/TestSummary-prefixdotnettests_tvOS-1" -Name "TestSummary.md" -Value "# :tada: All 4 tests passed :tada:" -Force
            New-Item -Path "$testDirectory/TestSummary-prefixdotnettests_MacCatalyst-1" -Name "TestSummary.md" -Value "# :tada: All 5 tests passed :tada:" -Force
            New-Item -Path "$testDirectory/TestSummary-prefixdotnettests_macOS-1" -Name "TestSummary.md" -Value "# :tada: All 6 tests passed :tada:" -Force
            New-Item -Path "$testDirectory/TestSummary-prefixdotnettests_Multiple-1" -Name "TestSummary.md" -Value "# :tada: All 7 tests passed :tada:" -Force


            $parallelResults = New-ParallelTestsResults -Path "$testDirectory" -TestPrefix "prefix" -Dependencies "$dependencies" -StageDependencies "$stageDependencies" -Context "context" -VSDropsIndex "vsdropsIndex"

            $parallelResults.IsSuccess() | Should -Be $false

            $sb = [System.Text.StringBuilder]::new()
            $parallelResults.WriteComment($sb)

            Remove-Item -Path $testDirectory -Recurse

            $content = $sb.ToString()

            Write-Host $content.Replace("&$", "&``$")

            $content | Should -Be "# Test results
:x: Tests failed on context

0 tests crashed, 4 tests failed, 8 tests passed.

## Failures

### :x: dotnettests tests (iOS)

<summary>1 tests failed, 0 tests passed.</summary>
<details>

</details>

[Html Report (VSDrops)](vsdropsIndex/prefixdotnettests_iOS-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-prefixdotnettests_iOS-1&api-version=6.0&`$format=zip)

### :x: dotnettests tests (macOS)

<summary>1 tests failed, 0 tests passed.</summary>
<details>

</details>

[Html Report (VSDrops)](vsdropsIndex/prefixdotnettests_macOS-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-prefixdotnettests_macOS-1&api-version=6.0&`$format=zip)

### :x: dotnettests tests (tvOS)

<summary>1 tests failed, 0 tests passed.</summary>
<details>

</details>

[Html Report (VSDrops)](vsdropsIndex/prefixdotnettests_tvOS-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-prefixdotnettests_tvOS-1&api-version=6.0&`$format=zip)

### :x: dotnettests tests (MacCatalyst)

<summary>1 tests failed, 0 tests passed.</summary>
<details>

</details>

[Html Report (VSDrops)](vsdropsIndex/prefixdotnettests_MacCatalyst-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-prefixdotnettests_MacCatalyst-1&api-version=6.0&`$format=zip)

## Successes

:white_check_mark: cecil: All 1 tests passed. [Html Report (VSDrops)](vsdropsIndex/prefixcecil-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-prefixcecil-1&api-version=6.0&`$format=zip)
:white_check_mark: dotnettests (Multiple platforms): All 7 tests passed. [Html Report (VSDrops)](vsdropsIndex/prefixdotnettests_Multiple-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-prefixdotnettests_Multiple-1&api-version=6.0&`$format=zip)

[comment]: <> (This is a test result report added by Azure DevOps)
"
        }

        It "computes the right summary with failing tests" {
            $VerbosePreference = "Continue"
            $Env:MyVerbosePreference = 'Continue'

            $testDirectory = Join-Path "." "subdir"
            New-Item -Path "$testDirectory" -ItemType "directory" -Force
            New-Item -Path "$testDirectory/TestSummary-prefixcecil-1" -Name "TestSummary.md" -Value "# :tada: All 1 tests passed :tada:" -Force
            New-Item -Path "$testDirectory/TestSummary-prefixdotnettests_iOS-1" -Name "TestSummary.md" -Value "# :tada: All 3 tests passed :tada:" -Force
            New-Item -Path "$testDirectory/TestSummary-prefixdotnettests_tvOS-1" -Name "TestSummary.md" -Value "# :tada: All 4 tests passed :tada:" -Force
            New-Item -Path "$testDirectory/TestSummary-prefixdotnettests_MacCatalyst-1" -Name "TestSummary.md" -Value "<summary>5 tests failed, 6 tests passed.</summary>" -Force
            New-Item -Path "$testDirectory/TestSummary-prefixdotnettests_macOS-1" -Name "TestSummary.md" -Value "# :tada: All 6 tests passed :tada:" -Force
            New-Item -Path "$testDirectory/TestSummary-prefixdotnettests_Multiple-1" -Name "TestSummary.md" -Value "# :tada: All 7 tests passed :tada:" -Force

            $parallelResults = New-ParallelTestsResults -Path "$testDirectory" -TestPrefix "prefix" -Dependencies "$dependencies" -StageDependencies "$stageDependencies" -Context "context" -VSDropsIndex "vsdropsIndex"

            $parallelResults.IsSuccess() | Should -Be $false

            $sb = [System.Text.StringBuilder]::new()
            $parallelResults.WriteComment($sb)

            Remove-Item -Path $testDirectory -Recurse

            $content = $sb.ToString()

            Write-Host $content.Replace("&$", "&``$")

            $content | Should -Be "# Test results
:x: Tests failed on context

0 tests crashed, 8 tests failed, 14 tests passed.

## Failures

### :x: dotnettests tests (iOS)

<summary>1 tests failed, 0 tests passed.</summary>
<details>

</details>

[Html Report (VSDrops)](vsdropsIndex/prefixdotnettests_iOS-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-prefixdotnettests_iOS-1&api-version=6.0&`$format=zip)

### :x: dotnettests tests (macOS)

<summary>1 tests failed, 0 tests passed.</summary>
<details>

</details>

[Html Report (VSDrops)](vsdropsIndex/prefixdotnettests_macOS-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-prefixdotnettests_macOS-1&api-version=6.0&`$format=zip)

### :x: dotnettests tests (tvOS)

<summary>1 tests failed, 0 tests passed.</summary>
<details>

</details>

[Html Report (VSDrops)](vsdropsIndex/prefixdotnettests_tvOS-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-prefixdotnettests_tvOS-1&api-version=6.0&`$format=zip)

### :x: dotnettests tests (MacCatalyst)

<summary>5 tests failed, 6 tests passed.</summary>
<details>

</details>

[Html Report (VSDrops)](vsdropsIndex/prefixdotnettests_MacCatalyst-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-prefixdotnettests_MacCatalyst-1&api-version=6.0&`$format=zip)

## Successes

:white_check_mark: cecil: All 1 tests passed. [Html Report (VSDrops)](vsdropsIndex/prefixcecil-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-prefixcecil-1&api-version=6.0&`$format=zip)
:white_check_mark: dotnettests (Multiple platforms): All 7 tests passed. [Html Report (VSDrops)](vsdropsIndex/prefixdotnettests_Multiple-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-prefixdotnettests_Multiple-1&api-version=6.0&`$format=zip)

[comment]: <> (This is a test result report added by Azure DevOps)
"
        }
    }
}
