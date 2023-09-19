<#
TestConfiguration unit tests.
#>

Import-Module ./TestConfiguration -Force

Describe 'Get-TestConfiguration' {
    Context 'import' {
        It 'gets the right values' {
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

            $config = Get-TestConfiguration `
              -TestConfigurations $TestConfigurations `
              -SupportedPlatforms $SupportedPlatforms `
              -TestsLabels "extra-test-labels" `
              -StatusContext "status-context" `
              -TestPrefix "test-prefix"

            #Write-Host "Test Config: $config"
        }

    }
}
