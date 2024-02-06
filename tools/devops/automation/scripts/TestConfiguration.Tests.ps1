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
    "containsDotNetTests": "true",
    "containsLegacyTests": "true",
  },
  {
    "label": "dotnettests",
    "splitByPlatforms": "true",
    "containsDotNetTests": "true",
    "containsLegacyTests": "false",
  }
]
"@
    
                $SupportedPlatforms = @"
[
  {
    "platform": "iOS",
    "isDotNetPlatform": "true",
    "isLegacyPlatform": "true"
  },
  {
    "platform": "macOS",
    "isDotNetPlatform": "true",
    "isLegacyPlatform": "true"
  },
  {
    "platform": "tvOS",
    "isDotNetPlatform": "true",
    "isLegacyPlatform": "true"
  },
  {
    "platform": "watchOS",
    "isDotNetPlatform": "false",
    "isLegacyPlatform": "true"
  },
  {
    "platform": "MacCatalyst",
    "isDotNetPlatform": "true",
    "isLegacyPlatform": "false"
  },
  {
    "platform": "Multiple",
    "isDotNetPlatform": "true",
    "isLegacyPlatform": "true"
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
      -XamarinLegacyEnabled "true" `
      -TestsLabels "extra-test-labels" `
      -StatusContext "status-context" `
      -TestPrefix "test-prefix_"
    Write-Host $config
    $config | Should -Be @"
{
  "cecil": {
    "LABEL": "cecil",
    "TESTS_LABELS": "extra-test-labels,run-cecil-tests",
    "LABEL_WITH_PLATFORM": "cecil",
    "STATUS_CONTEXT": "status-context - cecil",
    "TEST_PREFIX": "test-prefix_cecil",
    "TEST_PLATFORM": ""
  },
  "dotnettests_iOS": {
    "LABEL": "dotnettests",
    "TESTS_LABELS": "extra-test-labels,run-dotnettests-tests",
    "LABEL_WITH_PLATFORM": "dotnettests_iOS",
    "STATUS_CONTEXT": "status-context - dotnettests - iOS",
    "TEST_PREFIX": "test-prefix_dotnettests_iOS",
    "TEST_PLATFORM": "iOS",
    "TEST_FILTER": "Category != MultiPlatform"
  },
  "dotnettests_macOS": {
    "LABEL": "dotnettests",
    "TESTS_LABELS": "extra-test-labels,run-dotnettests-tests",
    "LABEL_WITH_PLATFORM": "dotnettests_macOS",
    "STATUS_CONTEXT": "status-context - dotnettests - macOS",
    "TEST_PREFIX": "test-prefix_dotnettests_macOS",
    "TEST_PLATFORM": "macOS",
    "TEST_FILTER": "Category != MultiPlatform"
  },
  "dotnettests_MacCatalyst": {
    "LABEL": "dotnettests",
    "TESTS_LABELS": "extra-test-labels,run-dotnettests-tests",
    "LABEL_WITH_PLATFORM": "dotnettests_MacCatalyst",
    "STATUS_CONTEXT": "status-context - dotnettests - MacCatalyst",
    "TEST_PREFIX": "test-prefix_dotnettests_MacCatalyst",
    "TEST_PLATFORM": "MacCatalyst",
    "TEST_FILTER": "Category != MultiPlatform"
  },
  "dotnettests_tvOS": {
    "LABEL": "dotnettests",
    "TESTS_LABELS": "extra-test-labels,run-dotnettests-tests",
    "LABEL_WITH_PLATFORM": "dotnettests_tvOS",
    "STATUS_CONTEXT": "status-context - dotnettests - tvOS",
    "TEST_PREFIX": "test-prefix_dotnettests_tvOS",
    "TEST_PLATFORM": "tvOS",
    "TEST_FILTER": "Category != MultiPlatform"
  },
  "dotnettests_Multiple": {
    "LABEL": "dotnettests",
    "TESTS_LABELS": "extra-test-labels,run-dotnettests-tests",
    "LABEL_WITH_PLATFORM": "dotnettests_Multiple",
    "STATUS_CONTEXT": "status-context - dotnettests - Multiple",
    "TEST_PREFIX": "test-prefix_dotnettests_Multiple",
    "TEST_PLATFORM": "",
    "TEST_FILTER": "Category = MultiPlatform"
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
      -XamarinLegacyEnabled "true" `
      -TestsLabels "extra-test-labels" `
      -StatusContext "status-context" `
      -TestPrefix "test-prefix_"
    Write-Host $config
    $config | Should -Be @"
{
  "cecil": {
    "LABEL": "cecil",
    "TESTS_LABELS": "extra-test-labels,run-cecil-tests",
    "LABEL_WITH_PLATFORM": "cecil",
    "STATUS_CONTEXT": "status-context - cecil",
    "TEST_PREFIX": "test-prefix_cecil",
    "TEST_PLATFORM": ""
  },
  "dotnettests_iOS": {
    "LABEL": "dotnettests",
    "TESTS_LABELS": "extra-test-labels,run-dotnettests-tests",
    "LABEL_WITH_PLATFORM": "dotnettests_iOS",
    "STATUS_CONTEXT": "status-context - dotnettests - iOS",
    "TEST_PREFIX": "test-prefix_dotnettests_iOS",
    "TEST_PLATFORM": "iOS",
    "TEST_FILTER": "Category != MultiPlatform"
  }
}
"@

  }

  It 'suceeds when no dotnet platforms enabled' {
    $EnabledPlatforms = ""

    $config = Get-TestConfiguration `
      -TestConfigurations $TestConfigurations `
      -SupportedPlatforms $SupportedPlatforms `
      -EnabledPlatforms $EnabledPlatforms `
      -XamarinLegacyEnabled "true" `
      -TestsLabels "extra-test-labels" `
      -StatusContext "status-context" `
      -TestPrefix "test-prefix_"
    Write-Host $config
    $config | Should -Be @"
{
  "cecil": {
    "LABEL": "cecil",
    "TESTS_LABELS": "extra-test-labels,run-cecil-tests",
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
        -XamarinLegacyEnabled "true" `
        -TestsLabels "extra-test-labels" `
        -StatusContext "status-context" `
        -TestPrefix "test-prefix_"
      Write-Host $config
      $config | Should -Be @"
{
  "cecil": {
    "LABEL": "cecil",
    "TESTS_LABELS": "extra-test-labels,run-cecil-tests",
    "LABEL_WITH_PLATFORM": "cecil",
    "STATUS_CONTEXT": "status-context - cecil",
    "TEST_PREFIX": "test-prefix_cecil",
    "TEST_PLATFORM": ""
  },
  "dotnettests_iOS": {
    "LABEL": "dotnettests",
    "TESTS_LABELS": "extra-test-labels,run-dotnettests-tests",
    "LABEL_WITH_PLATFORM": "dotnettests_iOS",
    "STATUS_CONTEXT": "status-context - dotnettests - iOS",
    "TEST_PREFIX": "test-prefix_dotnettests_iOS",
    "TEST_PLATFORM": "iOS",
    "TEST_FILTER": "Category != MultiPlatform"
  },
  "dotnettests_macOS": {
    "LABEL": "dotnettests",
    "TESTS_LABELS": "extra-test-labels,run-dotnettests-tests",
    "LABEL_WITH_PLATFORM": "dotnettests_macOS",
    "STATUS_CONTEXT": "status-context - dotnettests - macOS",
    "TEST_PREFIX": "test-prefix_dotnettests_macOS",
    "TEST_PLATFORM": "macOS",
    "TEST_FILTER": "Category != MultiPlatform"
  },
  "dotnettests_MacCatalyst": {
    "LABEL": "dotnettests",
    "TESTS_LABELS": "extra-test-labels,run-dotnettests-tests",
    "LABEL_WITH_PLATFORM": "dotnettests_MacCatalyst",
    "STATUS_CONTEXT": "status-context - dotnettests - MacCatalyst",
    "TEST_PREFIX": "test-prefix_dotnettests_MacCatalyst",
    "TEST_PLATFORM": "MacCatalyst",
    "TEST_FILTER": "Category != MultiPlatform"
  },
  "dotnettests_Multiple": {
    "LABEL": "dotnettests",
    "TESTS_LABELS": "extra-test-labels,run-dotnettests-tests",
    "LABEL_WITH_PLATFORM": "dotnettests_Multiple",
    "STATUS_CONTEXT": "status-context - dotnettests - Multiple",
    "TEST_PREFIX": "test-prefix_dotnettests_Multiple",
    "TEST_PLATFORM": "",
    "TEST_FILTER": "Category = MultiPlatform"
  }
}
"@
  }

  It 'does not generate legacy test jobs when XAMARIN_LEGACY is not set' {

    $TestConfigurations = @"
    [
      {
        "label": "cecil",
        "splitByPlatforms": "false",
        "containsDotNetTests": "true",
        "containsLegacyTests": "true",
      },
      {
        "label": "dotnettests",
        "splitByPlatforms": "true",
        "containsDotNetTests": "true",
        "containsLegacyTests": "false",
      },
      {
        "label": "install-source",
        "splitByPlatforms": "false",
        "containsDotNetTests": "false",
        "containsLegacyTests": "true",
      }
    ]
"@
    
    $EnabledPlatforms = "iOS macOS MacCatalyst"
    Write-Host "Test: It 'does not generate legacy test jobs when XAMARIN_LEGACY is not set'"
    $config = Get-TestConfiguration `
      -TestConfigurations $TestConfigurations `
      -SupportedPlatforms $SupportedPlatforms `
      -EnabledPlatforms $EnabledPlatforms `
      -XamarinLegacyEnabled "false" `
      -TestsLabels "extra-test-labels" `
      -StatusContext "status-context" `
      -TestPrefix "test-prefix_"
    #Write-Host $config
    $config | Should -Be @"
{
  "cecil": {
    "LABEL": "cecil",
    "TESTS_LABELS": "extra-test-labels,run-cecil-tests",
    "LABEL_WITH_PLATFORM": "cecil",
    "STATUS_CONTEXT": "status-context - cecil",
    "TEST_PREFIX": "test-prefix_cecil",
    "TEST_PLATFORM": ""
  },
  "dotnettests_iOS": {
    "LABEL": "dotnettests",
    "TESTS_LABELS": "extra-test-labels,run-dotnettests-tests",
    "LABEL_WITH_PLATFORM": "dotnettests_iOS",
    "STATUS_CONTEXT": "status-context - dotnettests - iOS",
    "TEST_PREFIX": "test-prefix_dotnettests_iOS",
    "TEST_PLATFORM": "iOS",
    "TEST_FILTER": "Category != MultiPlatform"
  },
  "dotnettests_macOS": {
    "LABEL": "dotnettests",
    "TESTS_LABELS": "extra-test-labels,run-dotnettests-tests",
    "LABEL_WITH_PLATFORM": "dotnettests_macOS",
    "STATUS_CONTEXT": "status-context - dotnettests - macOS",
    "TEST_PREFIX": "test-prefix_dotnettests_macOS",
    "TEST_PLATFORM": "macOS",
    "TEST_FILTER": "Category != MultiPlatform"
  },
  "dotnettests_MacCatalyst": {
    "LABEL": "dotnettests",
    "TESTS_LABELS": "extra-test-labels,run-dotnettests-tests",
    "LABEL_WITH_PLATFORM": "dotnettests_MacCatalyst",
    "STATUS_CONTEXT": "status-context - dotnettests - MacCatalyst",
    "TEST_PREFIX": "test-prefix_dotnettests_MacCatalyst",
    "TEST_PLATFORM": "MacCatalyst",
    "TEST_FILTER": "Category != MultiPlatform"
  },
  "dotnettests_Multiple": {
    "LABEL": "dotnettests",
    "TESTS_LABELS": "extra-test-labels,run-dotnettests-tests",
    "LABEL_WITH_PLATFORM": "dotnettests_Multiple",
    "STATUS_CONTEXT": "status-context - dotnettests - Multiple",
    "TEST_PREFIX": "test-prefix_dotnettests_Multiple",
    "TEST_PLATFORM": "",
    "TEST_FILTER": "Category = MultiPlatform"
  }
}
"@
  }

  It 'does generate legacy test jobs when XAMARIN_LEGACY is set' {

    $testConfigurations = @"
  [
    {
      "label": "cecil",
      "splitByPlatforms": "false",
      "containsDotNetTests": "true",
      "containsLegacyTests": "true",
    },
    {
      "label": "dotnettests",
      "splitByPlatforms": "true",
      "containsDotNetTests": "true",
      "containsLegacyTests": "false",
    },
    {
      "label": "install-source",
      "splitByPlatforms": "false",
      "containsDotNetTests": "false",
      "containsLegacyTests": "true",
    }
  ]
"@
  
  $EnabledPlatforms = "iOS macOS MacCatalyst"

  $config = Get-TestConfiguration `
    -TestConfigurations $testConfigurations `
    -SupportedPlatforms $SupportedPlatforms `
    -EnabledPlatforms $EnabledPlatforms `
    -XamarinLegacyEnabled "true" `
    -TestsLabels "extra-test-labels" `
    -StatusContext "status-context" `
    -TestPrefix "test-prefix_"
  Write-Host $config
  $config | Should -Be @"
{
  "cecil": {
    "LABEL": "cecil",
    "TESTS_LABELS": "extra-test-labels,run-cecil-tests",
    "LABEL_WITH_PLATFORM": "cecil",
    "STATUS_CONTEXT": "status-context - cecil",
    "TEST_PREFIX": "test-prefix_cecil",
    "TEST_PLATFORM": ""
  },
  "dotnettests_iOS": {
    "LABEL": "dotnettests",
    "TESTS_LABELS": "extra-test-labels,run-dotnettests-tests",
    "LABEL_WITH_PLATFORM": "dotnettests_iOS",
    "STATUS_CONTEXT": "status-context - dotnettests - iOS",
    "TEST_PREFIX": "test-prefix_dotnettests_iOS",
    "TEST_PLATFORM": "iOS",
    "TEST_FILTER": "Category != MultiPlatform"
  },
  "dotnettests_macOS": {
    "LABEL": "dotnettests",
    "TESTS_LABELS": "extra-test-labels,run-dotnettests-tests",
    "LABEL_WITH_PLATFORM": "dotnettests_macOS",
    "STATUS_CONTEXT": "status-context - dotnettests - macOS",
    "TEST_PREFIX": "test-prefix_dotnettests_macOS",
    "TEST_PLATFORM": "macOS",
    "TEST_FILTER": "Category != MultiPlatform"
  },
  "dotnettests_MacCatalyst": {
    "LABEL": "dotnettests",
    "TESTS_LABELS": "extra-test-labels,run-dotnettests-tests",
    "LABEL_WITH_PLATFORM": "dotnettests_MacCatalyst",
    "STATUS_CONTEXT": "status-context - dotnettests - MacCatalyst",
    "TEST_PREFIX": "test-prefix_dotnettests_MacCatalyst",
    "TEST_PLATFORM": "MacCatalyst",
    "TEST_FILTER": "Category != MultiPlatform"
  },
  "dotnettests_Multiple": {
    "LABEL": "dotnettests",
    "TESTS_LABELS": "extra-test-labels,run-dotnettests-tests",
    "LABEL_WITH_PLATFORM": "dotnettests_Multiple",
    "STATUS_CONTEXT": "status-context - dotnettests - Multiple",
    "TEST_PREFIX": "test-prefix_dotnettests_Multiple",
    "TEST_PLATFORM": "",
    "TEST_FILTER": "Category = MultiPlatform"
  },
  "install-source": {
    "LABEL": "install-source",
    "TESTS_LABELS": "extra-test-labels,run-install-source-tests",
    "LABEL_WITH_PLATFORM": "install-source",
    "STATUS_CONTEXT": "status-context - install-source",
    "TEST_PREFIX": "test-prefix_install_source",
    "TEST_PLATFORM": ""
  }
}
"@
  }

}
