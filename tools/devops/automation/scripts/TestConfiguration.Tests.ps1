<#
TestConfiguration unit tests.
#>

$ScriptDir = Split-Path -parent $MyInvocation.MyCommand.Path
Import-Module $ScriptDir/TestConfiguration.psm1 -Force

Describe 'Get-TestConfiguration' {
  BeforeAll {
    $TestConfigurations = @"
[
  {
    "label": "cecil",
    "splitByPlatforms": "false",
    "testPrefix": "test-prefix_",
    "testStage": "simulator",
  },
  {
    "label": "dotnettests",
    "splitByPlatforms": "true",
    "needsMultiplePlatforms": "true",
    "testPrefix": "test-prefix_",
  },
  {
    "label": "monotouchtest",
    "splitByPlatforms": "true",
    "needsMultiplePlatforms": "false",
    "testPrefix": "test-prefix_",
  }
]
"@
    
                $SupportedPlatforms = @"
[
  {
    "platform": "iOS",
  },
  {
    "platform": "macOS",
  },
  {
    "platform": "tvOS",
  },
  {
    "platform": "MacCatalyst",
  },
  {
    "platform": "Multiple",
  }
]
"@
    
  }

  It 'includes Multiple tests for dotnettests' {
    $EnabledPlatforms = "iOS macOS MacCatalyst tvOS"

    $config = Get-TestConfiguration `
      -TestConfigurations $TestConfigurations `
      -SupportedPlatforms $SupportedPlatforms `
      -EnabledPlatforms $EnabledPlatforms `
      -TestsLabels "extra-test-labels" `
      -StatusContext "status-context"
    Write-Host $config
    $config | Should -Be @"
{
  "cecil": {
    "LABEL": "cecil",
    "TESTS_LABELS": "extra-test-labels,run-cecil-tests",
    "TEST_STAGE": "simulator",
    "LABEL_WITH_PLATFORM": "cecil",
    "STATUS_CONTEXT": "status-context - cecil",
    "TEST_PREFIX": "test-prefix_cecil",
    "TEST_PLATFORM": ""
  },
  "dotnettests_iOS": {
    "LABEL": "dotnettests",
    "TESTS_LABELS": "extra-test-labels,run-dotnettests-tests",
    "TEST_STAGE": "test-prefix_",
    "LABEL_WITH_PLATFORM": "dotnettests_iOS",
    "STATUS_CONTEXT": "status-context - dotnettests - iOS",
    "TEST_PREFIX": "test-prefix_dotnettests_iOS",
    "TEST_PLATFORM": "iOS",
    "TEST_FILTER": "Category != MultiPlatform"
  },
  "dotnettests_macOS": {
    "LABEL": "dotnettests",
    "TESTS_LABELS": "extra-test-labels,run-dotnettests-tests",
    "TEST_STAGE": "test-prefix_",
    "LABEL_WITH_PLATFORM": "dotnettests_macOS",
    "STATUS_CONTEXT": "status-context - dotnettests - macOS",
    "TEST_PREFIX": "test-prefix_dotnettests_macOS",
    "TEST_PLATFORM": "macOS",
    "TEST_FILTER": "Category != MultiPlatform"
  },
  "dotnettests_MacCatalyst": {
    "LABEL": "dotnettests",
    "TESTS_LABELS": "extra-test-labels,run-dotnettests-tests",
    "TEST_STAGE": "test-prefix_",
    "LABEL_WITH_PLATFORM": "dotnettests_MacCatalyst",
    "STATUS_CONTEXT": "status-context - dotnettests - MacCatalyst",
    "TEST_PREFIX": "test-prefix_dotnettests_MacCatalyst",
    "TEST_PLATFORM": "MacCatalyst",
    "TEST_FILTER": "Category != MultiPlatform"
  },
  "dotnettests_tvOS": {
    "LABEL": "dotnettests",
    "TESTS_LABELS": "extra-test-labels,run-dotnettests-tests",
    "TEST_STAGE": "test-prefix_",
    "LABEL_WITH_PLATFORM": "dotnettests_tvOS",
    "STATUS_CONTEXT": "status-context - dotnettests - tvOS",
    "TEST_PREFIX": "test-prefix_dotnettests_tvOS",
    "TEST_PLATFORM": "tvOS",
    "TEST_FILTER": "Category != MultiPlatform"
  },
  "dotnettests_Multiple": {
    "LABEL": "dotnettests",
    "TESTS_LABELS": "extra-test-labels,run-dotnettests-tests",
    "TEST_STAGE": "test-prefix_",
    "LABEL_WITH_PLATFORM": "dotnettests_Multiple",
    "STATUS_CONTEXT": "status-context - dotnettests - Multiple",
    "TEST_PREFIX": "test-prefix_dotnettests_Multiple",
    "TEST_PLATFORM": "",
    "TEST_FILTER": "Category = MultiPlatform"
  },
  "monotouchtest_ios": {
    "LABEL": "monotouchtest",
    "TESTS_LABELS": "extra-test-labels,run-monotouchtest-tests",
    "TEST_STAGE": "test-prefix_",
    "LABEL_WITH_PLATFORM": "monotouchtest_iOS",
    "STATUS_CONTEXT": "status-context - monotouchtest - iOS",
    "TEST_PREFIX": "test-prefix_monotouchtest_ios",
    "TEST_PLATFORM": "iOS",
    "TEST_FILTER": "Category != MultiPlatform"
  },
  "monotouchtest_macos": {
    "LABEL": "monotouchtest",
    "TESTS_LABELS": "extra-test-labels,run-monotouchtest-tests",
    "TEST_STAGE": "test-prefix_",
    "LABEL_WITH_PLATFORM": "monotouchtest_macOS",
    "STATUS_CONTEXT": "status-context - monotouchtest - macOS",
    "TEST_PREFIX": "test-prefix_monotouchtest_macos",
    "TEST_PLATFORM": "macOS",
    "TEST_FILTER": "Category != MultiPlatform"
  },
  "monotouchtest_maccatalyst": {
    "LABEL": "monotouchtest",
    "TESTS_LABELS": "extra-test-labels,run-monotouchtest-tests",
    "TEST_STAGE": "test-prefix_",
    "LABEL_WITH_PLATFORM": "monotouchtest_MacCatalyst",
    "STATUS_CONTEXT": "status-context - monotouchtest - MacCatalyst",
    "TEST_PREFIX": "test-prefix_monotouchtest_maccatalyst",
    "TEST_PLATFORM": "MacCatalyst",
    "TEST_FILTER": "Category != MultiPlatform"
  },
  "monotouchtest_tvos": {
    "LABEL": "monotouchtest",
    "TESTS_LABELS": "extra-test-labels,run-monotouchtest-tests",
    "TEST_STAGE": "test-prefix_",
    "LABEL_WITH_PLATFORM": "monotouchtest_tvOS",
    "STATUS_CONTEXT": "status-context - monotouchtest - tvOS",
    "TEST_PREFIX": "test-prefix_monotouchtest_tvos",
    "TEST_PLATFORM": "tvOS",
    "TEST_FILTER": "Category != MultiPlatform"
  }
}
"@

  }

  It 'does not include Multiple tests for dotnettests' {
    $EnabledPlatforms = "iOS"

    $config = Get-TestConfiguration `
      -TestConfigurations $TestConfigurations `
      -SupportedPlatforms $SupportedPlatforms `
      -EnabledPlatforms $EnabledPlatforms `
      -TestsLabels "extra-test-labels" `
      -StatusContext "status-context"
    Write-Host $config
    $config | Should -Be @"
{
  "cecil": {
    "LABEL": "cecil",
    "TESTS_LABELS": "extra-test-labels,run-cecil-tests",
    "TEST_STAGE": "simulator",
    "LABEL_WITH_PLATFORM": "cecil",
    "STATUS_CONTEXT": "status-context - cecil",
    "TEST_PREFIX": "test-prefix_cecil",
    "TEST_PLATFORM": ""
  },
  "dotnettests_iOS": {
    "LABEL": "dotnettests",
    "TESTS_LABELS": "extra-test-labels,run-dotnettests-tests",
    "TEST_STAGE": "test-prefix_",
    "LABEL_WITH_PLATFORM": "dotnettests_iOS",
    "STATUS_CONTEXT": "status-context - dotnettests - iOS",
    "TEST_PREFIX": "test-prefix_dotnettests_iOS",
    "TEST_PLATFORM": "iOS",
    "TEST_FILTER": "Category != MultiPlatform"
  },
  "monotouchtest_ios": {
    "LABEL": "monotouchtest",
    "TESTS_LABELS": "extra-test-labels,run-monotouchtest-tests",
    "TEST_STAGE": "test-prefix_",
    "LABEL_WITH_PLATFORM": "monotouchtest_iOS",
    "STATUS_CONTEXT": "status-context - monotouchtest - iOS",
    "TEST_PREFIX": "test-prefix_monotouchtest_ios",
    "TEST_PLATFORM": "iOS",
    "TEST_FILTER": "Category != MultiPlatform"
  }
}
"@

  }

  It 'succeeds when no dotnet platforms enabled' {
    $EnabledPlatforms = ""

    $config = Get-TestConfiguration `
      -TestConfigurations $TestConfigurations `
      -SupportedPlatforms $SupportedPlatforms `
      -EnabledPlatforms $EnabledPlatforms `
      -TestsLabels "extra-test-labels" `
      -StatusContext "status-context"
    Write-Host $config
    $config | Should -Be @"
{
  "cecil": {
    "LABEL": "cecil",
    "TESTS_LABELS": "extra-test-labels,run-cecil-tests",
    "TEST_STAGE": "simulator",
    "LABEL_WITH_PLATFORM": "cecil",
    "STATUS_CONTEXT": "status-context - cecil",
    "TEST_PREFIX": "test-prefix_cecil",
    "TEST_PLATFORM": ""
  }
}
"@
}

  It 'does not generate tvOS tests for dotnettests' {

      $EnabledPlatforms = "iOS macOS MacCatalyst"

      $config = Get-TestConfiguration `
        -TestConfigurations $TestConfigurations `
        -SupportedPlatforms $SupportedPlatforms `
        -EnabledPlatforms $EnabledPlatforms `
        -TestsLabels "extra-test-labels" `
        -StatusContext "status-context"
      Write-Host $config
      $config | Should -Be @"
{
  "cecil": {
    "LABEL": "cecil",
    "TESTS_LABELS": "extra-test-labels,run-cecil-tests",
    "TEST_STAGE": "simulator",
    "LABEL_WITH_PLATFORM": "cecil",
    "STATUS_CONTEXT": "status-context - cecil",
    "TEST_PREFIX": "test-prefix_cecil",
    "TEST_PLATFORM": ""
  },
  "dotnettests_iOS": {
    "LABEL": "dotnettests",
    "TESTS_LABELS": "extra-test-labels,run-dotnettests-tests",
    "TEST_STAGE": "test-prefix_",
    "LABEL_WITH_PLATFORM": "dotnettests_iOS",
    "STATUS_CONTEXT": "status-context - dotnettests - iOS",
    "TEST_PREFIX": "test-prefix_dotnettests_iOS",
    "TEST_PLATFORM": "iOS",
    "TEST_FILTER": "Category != MultiPlatform"
  },
  "dotnettests_macOS": {
    "LABEL": "dotnettests",
    "TESTS_LABELS": "extra-test-labels,run-dotnettests-tests",
    "TEST_STAGE": "test-prefix_",
    "LABEL_WITH_PLATFORM": "dotnettests_macOS",
    "STATUS_CONTEXT": "status-context - dotnettests - macOS",
    "TEST_PREFIX": "test-prefix_dotnettests_macOS",
    "TEST_PLATFORM": "macOS",
    "TEST_FILTER": "Category != MultiPlatform"
  },
  "dotnettests_MacCatalyst": {
    "LABEL": "dotnettests",
    "TESTS_LABELS": "extra-test-labels,run-dotnettests-tests",
    "TEST_STAGE": "test-prefix_",
    "LABEL_WITH_PLATFORM": "dotnettests_MacCatalyst",
    "STATUS_CONTEXT": "status-context - dotnettests - MacCatalyst",
    "TEST_PREFIX": "test-prefix_dotnettests_MacCatalyst",
    "TEST_PLATFORM": "MacCatalyst",
    "TEST_FILTER": "Category != MultiPlatform"
  },
  "dotnettests_Multiple": {
    "LABEL": "dotnettests",
    "TESTS_LABELS": "extra-test-labels,run-dotnettests-tests",
    "TEST_STAGE": "test-prefix_",
    "LABEL_WITH_PLATFORM": "dotnettests_Multiple",
    "STATUS_CONTEXT": "status-context - dotnettests - Multiple",
    "TEST_PREFIX": "test-prefix_dotnettests_Multiple",
    "TEST_PLATFORM": "",
    "TEST_FILTER": "Category = MultiPlatform"
  },
  "monotouchtest_ios": {
    "LABEL": "monotouchtest",
    "TESTS_LABELS": "extra-test-labels,run-monotouchtest-tests",
    "TEST_STAGE": "test-prefix_",
    "LABEL_WITH_PLATFORM": "monotouchtest_iOS",
    "STATUS_CONTEXT": "status-context - monotouchtest - iOS",
    "TEST_PREFIX": "test-prefix_monotouchtest_ios",
    "TEST_PLATFORM": "iOS",
    "TEST_FILTER": "Category != MultiPlatform"
  },
  "monotouchtest_macos": {
    "LABEL": "monotouchtest",
    "TESTS_LABELS": "extra-test-labels,run-monotouchtest-tests",
    "TEST_STAGE": "test-prefix_",
    "LABEL_WITH_PLATFORM": "monotouchtest_macOS",
    "STATUS_CONTEXT": "status-context - monotouchtest - macOS",
    "TEST_PREFIX": "test-prefix_monotouchtest_macos",
    "TEST_PLATFORM": "macOS",
    "TEST_FILTER": "Category != MultiPlatform"
  },
  "monotouchtest_maccatalyst": {
    "LABEL": "monotouchtest",
    "TESTS_LABELS": "extra-test-labels,run-monotouchtest-tests",
    "TEST_STAGE": "test-prefix_",
    "LABEL_WITH_PLATFORM": "monotouchtest_MacCatalyst",
    "STATUS_CONTEXT": "status-context - monotouchtest - MacCatalyst",
    "TEST_PREFIX": "test-prefix_monotouchtest_maccatalyst",
    "TEST_PLATFORM": "MacCatalyst",
    "TEST_FILTER": "Category != MultiPlatform"
  }
}
"@
  }

}
