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
        $testConfig = [TestConfiguration]::new($suite, $title, $platform, $resultContext, "testStage")
        $jobStatus = "Succeeded"
        $attempt = 1
        $matrix = @"
{
    "dotnettests_ios": {
        "LABEL": "dotnettests",
        "TESTS_LABELS": "--label=skip-all-tests,run-ios-tests,run-ios-simulator-tests,run-tvos-tests,run-mac-tests,run-maccatalyst-tests,run-system-permission-tests,run-dotnettests-tests",
        "TEST_STAGE": "testStage",
        "LABEL_WITH_PLATFORM": "dotnettests_iOS",
        "STATUS_CONTEXT": "VSTS: simulator tests - dotnettests - iOS",
        "TEST_simulator_tests": "simulator_dotnettests_ios",
        "TEST_PLATFORM": "iOS",
        "TEST_FILTER": "Category != MultiPlatform"
      },
      "dotnettests_tvos": {
        "LABEL": "dotnettests",
        "TESTS_LABELS": "--label=skip-all-tests,run-ios-tests,run-ios-simulator-tests,run-tvos-tests,run-mac-tests,run-maccatalyst-tests,run-system-permission-tests,run-dotnettests-tests",
        "TEST_STAGE": "testStage",
        "LABEL_WITH_PLATFORM": "dotnettests_tvOS",
        "STATUS_CONTEXT": "VSTS: simulator tests - dotnettests - tvOS",
        "TEST_simulator_tests": "simulator_dotnettests_tvos",
        "TEST_PLATFORM": "tvOS",
        "TEST_FILTER": "Category != MultiPlatform"
      },
      "dotnettests_maccatalyst": {
        "LABEL": "dotnettests",
        "TESTS_LABELS": "--label=skip-all-tests,run-ios-tests,run-ios-simulator-tests,run-tvos-tests,run-mac-tests,run-maccatalyst-tests,run-system-permission-tests,run-dotnettests-tests",
        "TEST_STAGE": "testStage",
        "LABEL_WITH_PLATFORM": "dotnettests_MacCatalyst",
        "STATUS_CONTEXT": "VSTS: simulator tests - dotnettests - MacCatalyst",
        "TEST_simulator_tests": "simulator_dotnettests_maccatalyst",
        "TEST_PLATFORM": "MacCatalyst",
        "TEST_FILTER": "Category != MultiPlatform"
      },
      "dotnettests_macos": {
        "LABEL": "dotnettests",
        "TESTS_LABELS": "--label=skip-all-tests,run-ios-tests,run-ios-simulator-tests,run-tvos-tests,run-mac-tests,run-maccatalyst-tests,run-system-permission-tests,run-dotnettests-tests",
        "TEST_STAGE": "testStage",
        "LABEL_WITH_PLATFORM": "dotnettests_macOS",
        "STATUS_CONTEXT": "VSTS: simulator tests - dotnettests - macOS",
        "TEST_simulator_tests": "simulator_dotnettests_macos",
        "TEST_PLATFORM": "macOS",
        "TEST_FILTER": "Category != MultiPlatform"
      },
      "dotnettests_multiple": {
        "LABEL": "dotnettests",
        "TESTS_LABELS": "--label=skip-all-tests,run-ios-tests,run-ios-simulator-tests,run-tvos-tests,run-mac-tests,run-maccatalyst-tests,run-system-permission-tests,run-dotnettests-tests",
        "TEST_STAGE": "testStage",
        "LABEL_WITH_PLATFORM": "dotnettests_Multiple",
        "STATUS_CONTEXT": "VSTS: simulator tests - dotnettests - Multiple",
        "TEST_simulator_tests": "simulator_dotnettests_multiple",
        "TEST_PLATFORM": "",
        "TEST_FILTER": "Category = MultiPlatform"
      },
    "cecil":
    {
        "LABEL": "cecil",
        "TESTS_LABELS": "--label=skip-all-tests,run-ios-64-tests,run-ios-simulator-tests,run-tvos-tests,run-mac-tests,run-maccatalyst-tests,run-system-permission-tests,run-cecil-tests",
        "TEST_STAGE": "testStage",
        "LABEL_WITH_PLATFORM": "cecil",
        "STATUS_CONTEXT": "VSTS: simulator tests - cecil",
        "TEST_simulator_tests": "simulator_cecil",
        "TEST_PLATFORM": ""
    }
}
"@
        $stageDependencies = @"
{    
  "build_macos_tests": {
    "build_macos_tests_job": {
      "result": "Succeeded"
    }
  },
  "configure_build": {
    "configure": {
      "outputs": {
        "test_matrix.TEST_MATRIX": "$($matrix.Replace("`n", "\n").Replace("`"", "\`""))"
      }
    }
  },
    "simulator_tests": {
        "tests": {
            "outputs": {
                "dotnettests_tvos.runTests.TESTS_JOBSTATUS": "Succeeded",
                "dotnettests_tvos.fix_commit.GIT_HASH": "fa3d1deb4e2d0ac358f2e0ac80e3d305ca541848",
                "dotnettests_tvos.DownloadPipelineArtifact1.BuildNumber": "8894907",
                "dotnettests_tvos.Bash23.TESTS_ATTEMPT": "1",
                "dotnettests_tvos.Bash23.TESTS_BOT": "XAMMINI-012.Ventura",
                "dotnettests_tvos.Bash23.TESTS_JOBSTATUS": "Failed",
                "dotnettests_tvos.Bash23.TESTS_LABEL": "dotnettests",
                "dotnettests_tvos.Bash23.TESTS_PLATFORM": "",
                "dotnettests_tvos.Bash23.TESTS_TITLE": "dotnettests_tvos",
                "dotnettests_tvos.DownloadPipelineArtifact2.BuildNumber": "8894907",
                "dotnettests_maccatalyst.fix_commit.GIT_HASH": "fa3d1deb4e2d0ac358f2e0ac80e3d305ca541848",
                "dotnettests_maccatalyst.DownloadPipelineArtifact1.BuildNumber": "8894907",
                "dotnettests_maccatalyst.Bash23.TESTS_ATTEMPT": "1",
                "dotnettests_maccatalyst.Bash23.TESTS_BOT": "XAMBOT-1023.Ventura",
                "dotnettests_maccatalyst.Bash23.TESTS_JOBSTATUS": "Failed",
                "dotnettests_maccatalyst.Bash23.TESTS_LABEL": "dotnettests",
                "dotnettests_maccatalyst.Bash23.TESTS_PLATFORM": "",
                "dotnettests_maccatalyst.Bash23.TESTS_TITLE": "dotnettests_maccatalyst",
                "dotnettests_maccatalyst.DownloadPipelineArtifact2.BuildNumber": "8894907",
                "dotnettests_macos.fix_commit.GIT_HASH": "fa3d1deb4e2d0ac358f2e0ac80e3d305ca541848",
                "dotnettests_macos.DownloadPipelineArtifact1.BuildNumber": "8894907",
                "dotnettests_macos.Bash23.TESTS_ATTEMPT": "1",
                "dotnettests_macos.Bash23.TESTS_BOT": "XAMMINI-015.Ventura",
                "dotnettests_macos.Bash23.TESTS_JOBSTATUS": "Failed",
                "dotnettests_macos.Bash23.TESTS_LABEL": "dotnettests",
                "dotnettests_macos.Bash23.TESTS_PLATFORM": "",
                "dotnettests_macos.Bash23.TESTS_TITLE": "dotnettests_macos",
                "dotnettests_macos.DownloadPipelineArtifact2.BuildNumber": "8894907",
                "dotnettests_macos.runTests.TESTS_JOBSTATUS": "Succeeded",
                "dotnettests_ios.fix_commit.GIT_HASH": "fa3d1deb4e2d0ac358f2e0ac80e3d305ca541848",
                "dotnettests_ios.DownloadPipelineArtifact1.BuildNumber": "8894907",
                "dotnettests_ios.Bash23.TESTS_ATTEMPT": "1",
                "dotnettests_ios.Bash23.TESTS_BOT": "XAMMINI-014.Ventura",
                "dotnettests_ios.Bash23.TESTS_JOBSTATUS": "Failed",
                "dotnettests_ios.Bash23.TESTS_LABEL": "dotnettests",
                "dotnettests_ios.Bash23.TESTS_PLATFORM": "",
                "dotnettests_ios.Bash23.TESTS_TITLE": "dotnettests_ios",
                "dotnettests_ios.DownloadPipelineArtifact2.BuildNumber": "8894907",
                "dotnettests_ios.runTests.TESTS_JOBSTATUS": "Succeeded",
                "dotnettests_multiple.fix_commit.GIT_HASH": "fa3d1deb4e2d0ac358f2e0ac80e3d305ca541848",
                "dotnettests_multiple.DownloadPipelineArtifact1.BuildNumber": "8894907",
                "dotnettests_multiple.Bash23.TESTS_ATTEMPT": "1",
                "dotnettests_multiple.Bash23.TESTS_BOT": "XAMMINI-010.Ventura",
                "dotnettests_multiple.Bash23.TESTS_JOBSTATUS": "Failed",
                "dotnettests_multiple.Bash23.TESTS_LABEL": "dotnettests",
                "dotnettests_multiple.Bash23.TESTS_PLATFORM": "",
                "dotnettests_multiple.Bash23.TESTS_TITLE": "dotnettests_multiple",
                "dotnettests_multiple.DownloadPipelineArtifact2.BuildNumber": "8894907",
                "dotnettests_multiple.runTests.TESTS_JOBSTATUS": "Succeeded",
                "cecil.fix_commit.GIT_HASH": "fa3d1deb4e2d0ac358f2e0ac80e3d305ca541848",
                "cecil.DownloadPipelineArtifact1.BuildNumber": "8894907",
                "cecil.Bash23.TESTS_ATTEMPT": "1",
                "cecil.Bash23.TESTS_BOT": "XAMMINI-013.Ventura",
                "cecil.Bash23.TESTS_JOBSTATUS": "Failed",
                "cecil.Bash23.TESTS_LABEL": "cecil",
                "cecil.Bash23.TESTS_PLATFORM": "",
                "cecil.Bash23.TESTS_TITLE": "cecil",
                "cecil.DownloadPipelineArtifact2.BuildNumber": "8894907",
                "cecil.runTests.TESTS_JOBSTATUS": "Succeeded"
            },
            "identifier": null,
            "name": "tests",
            "attempt": 1,
            "startTime": null,
            "finishTime": null,
            "state": "NotStarted",
            "result": "Failed"
        }
    }
}
"@

        $stageDependenciesWithMoreFailingTests = @"
{    
  "build_macos_tests": {
    "build_macos_tests_job": {
      "result": "Succeeded"
    }
  },
  "configure_build": {
    "configure": {
      "outputs": {
        "test_matrix.TEST_MATRIX": "$($matrix.Replace("`n", "\n").Replace("`"", "\`""))"
      }
    }
  },
    "simulator_tests": {
        "tests": {
            "outputs": {
                "dotnettests_tvos.runTests.TESTS_JOBSTATUS": "Succeeded",
                "dotnettests_tvos.fix_commit.GIT_HASH": "fa3d1deb4e2d0ac358f2e0ac80e3d305ca541848",
                "dotnettests_tvos.DownloadPipelineArtifact1.BuildNumber": "8894907",
                "dotnettests_tvos.Bash23.TESTS_ATTEMPT": "1",
                "dotnettests_tvos.Bash23.TESTS_BOT": "XAMMINI-012.Ventura",
                "dotnettests_tvos.Bash23.TESTS_JOBSTATUS": "Failed",
                "dotnettests_tvos.Bash23.TESTS_LABEL": "dotnettests",
                "dotnettests_tvos.Bash23.TESTS_PLATFORM": "",
                "dotnettests_tvos.Bash23.TESTS_TITLE": "dotnettests_tvos",
                "dotnettests_tvos.DownloadPipelineArtifact2.BuildNumber": "8894907",
                "dotnettests_maccatalyst.fix_commit.GIT_HASH": "fa3d1deb4e2d0ac358f2e0ac80e3d305ca541848",
                "dotnettests_maccatalyst.DownloadPipelineArtifact1.BuildNumber": "8894907",
                "dotnettests_maccatalyst.Bash23.TESTS_ATTEMPT": "1",
                "dotnettests_maccatalyst.Bash23.TESTS_BOT": "XAMBOT-1023.Ventura",
                "dotnettests_maccatalyst.Bash23.TESTS_JOBSTATUS": "Failed",
                "dotnettests_maccatalyst.Bash23.TESTS_LABEL": "dotnettests",
                "dotnettests_maccatalyst.Bash23.TESTS_PLATFORM": "",
                "dotnettests_maccatalyst.Bash23.TESTS_TITLE": "dotnettests_maccatalyst",
                "dotnettests_maccatalyst.DownloadPipelineArtifact2.BuildNumber": "8894907",
                "dotnettests_macos.fix_commit.GIT_HASH": "fa3d1deb4e2d0ac358f2e0ac80e3d305ca541848",
                "dotnettests_macos.DownloadPipelineArtifact1.BuildNumber": "8894907",
                "dotnettests_macos.Bash23.TESTS_ATTEMPT": "1",
                "dotnettests_macos.Bash23.TESTS_BOT": "XAMMINI-015.Ventura",
                "dotnettests_macos.Bash23.TESTS_JOBSTATUS": "Failed",
                "dotnettests_macos.Bash23.TESTS_LABEL": "dotnettests",
                "dotnettests_macos.Bash23.TESTS_PLATFORM": "",
                "dotnettests_macos.Bash23.TESTS_TITLE": "dotnettests_macos",
                "dotnettests_macos.DownloadPipelineArtifact2.BuildNumber": "8894907",
                "dotnettests_macos.runTests.TESTS_JOBSTATUS": "Failed",
                "dotnettests_ios.fix_commit.GIT_HASH": "fa3d1deb4e2d0ac358f2e0ac80e3d305ca541848",
                "dotnettests_ios.DownloadPipelineArtifact1.BuildNumber": "8894907",
                "dotnettests_ios.Bash23.TESTS_ATTEMPT": "1",
                "dotnettests_ios.Bash23.TESTS_BOT": "XAMMINI-014.Ventura",
                "dotnettests_ios.Bash23.TESTS_JOBSTATUS": "Failed",
                "dotnettests_ios.Bash23.TESTS_LABEL": "dotnettests",
                "dotnettests_ios.Bash23.TESTS_PLATFORM": "",
                "dotnettests_ios.Bash23.TESTS_TITLE": "dotnettests_ios",
                "dotnettests_ios.DownloadPipelineArtifact2.BuildNumber": "8894907",
                "dotnettests_ios.runTests.TESTS_JOBSTATUS": "Succeeded",
                "dotnettests_multiple.fix_commit.GIT_HASH": "fa3d1deb4e2d0ac358f2e0ac80e3d305ca541848",
                "dotnettests_multiple.DownloadPipelineArtifact1.BuildNumber": "8894907",
                "dotnettests_multiple.Bash23.TESTS_ATTEMPT": "1",
                "dotnettests_multiple.Bash23.TESTS_BOT": "XAMMINI-010.Ventura",
                "dotnettests_multiple.Bash23.TESTS_JOBSTATUS": "Failed",
                "dotnettests_multiple.Bash23.TESTS_LABEL": "dotnettests",
                "dotnettests_multiple.Bash23.TESTS_PLATFORM": "",
                "dotnettests_multiple.Bash23.TESTS_TITLE": "dotnettests_multiple",
                "dotnettests_multiple.DownloadPipelineArtifact2.BuildNumber": "8894907",
                "dotnettests_multiple.runTests.TESTS_JOBSTATUS": "Succeeded",
                "cecil.fix_commit.GIT_HASH": "fa3d1deb4e2d0ac358f2e0ac80e3d305ca541848",
                "cecil.DownloadPipelineArtifact1.BuildNumber": "8894907",
                "cecil.Bash23.TESTS_ATTEMPT": "1",
                "cecil.Bash23.TESTS_BOT": "XAMMINI-013.Ventura",
                "cecil.Bash23.TESTS_JOBSTATUS": "Failed",
                "cecil.Bash23.TESTS_LABEL": "cecil",
                "cecil.Bash23.TESTS_PLATFORM": "",
                "cecil.Bash23.TESTS_TITLE": "cecil",
                "cecil.DownloadPipelineArtifact2.BuildNumber": "8894907",
                "cecil.runTests.TESTS_JOBSTATUS": "Succeeded"
            },
            "identifier": null,
            "name": "tests",
            "attempt": 1,
            "startTime": null,
            "finishTime": null,
            "state": "NotStarted",
            "result": "Failed"
        }
    }
}
"@

        $stageDependenciesWithMissingResults = @"
{
  "build_macos_tests": {
    "build_macos_tests_job": {
      "result": "Succeeded"
    }
  },
  "configure_build": {
    "configure": {
      "outputs": {
        "test_matrix.TEST_MATRIX": "$($matrix.Replace("`n", "\n").Replace("`"", "\`""))"
      }
    }
  },
    "simulator_tests": {
        "tests": {
            "outputs": {
                "cecil.fix_commit.GIT_HASH": "fa3d1deb4e2d0ac358f2e0ac80e3d305ca541848",
                "cecil.DownloadPipelineArtifact1.BuildNumber": "8894907",
                "cecil.Bash23.TESTS_ATTEMPT": "1",
                "cecil.Bash23.TESTS_BOT": "XAMMINI-013.Ventura",
                "cecil.Bash23.TESTS_JOBSTATUS": "Failed",
                "cecil.Bash23.TESTS_LABEL": "cecil",
                "cecil.Bash23.TESTS_PLATFORM": "",
                "cecil.DownloadPipelineArtifact2.BuildNumber": "8894907",
                "cecil.runTests.TESTS_JOBSTATUS": "Succeeded",

                "dotnettests_multiple.fix_commit.GIT_HASH": "fa3d1deb4e2d0ac358f2e0ac80e3d305ca541848",
                "dotnettests_multiple.DownloadPipelineArtifact1.BuildNumber": "8894907",
                "dotnettests_multiple.Bash23.TESTS_ATTEMPT": "1",
                "dotnettests_multiple.Bash23.TESTS_BOT": "XAMMINI-010.Ventura",
                "dotnettests_multiple.Bash23.TESTS_JOBSTATUS": "Failed",
                "dotnettests_multiple.Bash23.TESTS_LABEL": "dotnettests",
                "dotnettests_multiple.Bash23.TESTS_PLATFORM": "",
                "dotnettests_multiple.DownloadPipelineArtifact2.BuildNumber": "8894907",
                "dotnettests_multiple.runTests.TESTS_JOBSTATUS": "Succeeded"
            },
            "identifier": null,
            "name": "tests",
            "attempt": 1,
            "startTime": null,
            "finishTime": null,
            "state": "NotStarted",
            "result": "Failed"
        }
    }
}
"@
        $stageDependenciesWithBuildFailure = @"
{
  "build_macos_tests": {
    "build_macos_tests_job": {
      "outputs": {
        "fix_commit.GIT_HASH": "8a881722232ef37ed73f8926acd113a6ccc8eafd",
        "configuration.BuildNugets": "True",
        "configuration.BuildPkgs": "True",
        "configuration.PR_ID": "20434",
        "configuration.SignPkgs": "True",
        "build.TESTS_BOT": "XAMBOT-1001.Sonoma"
      },
      "identifier": null,
      "name": "build_macos_tests_job",
      "attempt": 1,
      "startTime": null,
      "finishTime": null,
      "state": "NotStarted",
      "result": "Failed"
    }
  }
}
"@

        $stageDependenciesWithWindowsResults = @"
{
  "configure_build": {
    "configure": {
      "outputs": {
        "agent.version": "4.252.0",
        "JobExtension_Init.agent.version": "4.252.0",
        "__system_2.agent.version": "4.252.0",
        "__system_1.agent.version": "4.252.0",
        "__system_3.agent.version": "4.252.0",
        "__system_4.agent.version": "4.252.0",
        "Checkout1.agent.version": "4.252.0",
        "fix_commit.agent.version": "4.252.0",
        "fix_commit.GIT_HASH": "cd6f17c5cb9b0fabbe0e2af9fc17d57e8aab3c1a",
        "Checkout2.agent.version": "4.252.0",
        "Checkout3.agent.version": "4.252.0",
        "__system_postcheckout_1.agent.version": "4.252.0",
        "Download.agent.version": "4.252.0",
        "PowerShell1.agent.version": "4.252.0",
        "configure_platforms.agent.version": "4.252.0",
        "configure_platforms.DOTNET_IOS_RUNTIME_IDENTIFIERS": "ios-arm64 iossimulator-x64 iossimulator-arm64",
        "configure_platforms.DOTNET_PLATFORMS": "iOS",
        "configure_platforms.INCLUDE_DOTNET_IOS": "1",
        "configure_platforms.INCLUDE_DOTNET_MACCATALYST": "",
        "configure_platforms.INCLUDE_DOTNET_MACOS": "",
        "configure_platforms.INCLUDE_DOTNET_TVOS": "",
        "configure_platforms.INCLUDE_IOS": "1",
        "configure_platforms.INCLUDE_MAC": "",
        "configure_platforms.INCLUDE_MACCATALYST": "",
        "configure_platforms.INCLUDE_TVOS": "",
        "configure_platforms.IOS_NUGET_OS_VERSION": "18.2",
        "configure_platforms.IOS_NUGET_REF_NAME": "Microsoft.iOS.Ref.net9.0_18.2",
        "configure_platforms.IOS_NUGET_SDK_NAME": "Microsoft.iOS.Sdk.net9.0_18.2",
        "configure_platforms.IOS_NUGET_VERSION_NO_METADATA": "18.2.9527-ci.dev-rolf-windows-tests-html-report",
        "configure_platforms.ios-arm64_NUGET_RUNTIME_NAME": "Microsoft.iOS.Runtime.ios-arm64.net9.0_18.2",
        "configure_platforms.iossimulator-arm64_NUGET_RUNTIME_NAME": "Microsoft.iOS.Runtime.iossimulator-arm64.net9.0_18.2",
        "configure_platforms.iossimulator-x64_NUGET_RUNTIME_NAME": "Microsoft.iOS.Runtime.iossimulator-x64.net9.0_18.2",
        "configure_platforms.MACCATALYST_NUGET_OS_VERSION": "18.2",
        "configure_platforms.MACOS_NUGET_OS_VERSION": "15.2",
        "configure_platforms.TVOS_NUGET_OS_VERSION": "18.2",
        "labels.agent.version": "4.252.0",
        "labels.prBuild": "False",
        "decisions.agent.version": "4.252.0",
        "decisions.RUN_MAC_TESTS": "false",
        "decisions.RUN_WINDOWS_TESTS": "true",
        "PowerShell2.agent.version": "4.252.0",
        "test_matrix.agent.version": "4.252.0",
        "test_matrix.DOTNET_IOS_RUNTIME_IDENTIFIERS": "ios-arm64 iossimulator-x64 iossimulator-arm64",
        "test_matrix.DOTNET_PLATFORMS": "iOS",
        "test_matrix.INCLUDE_DOTNET_IOS": "1",
        "test_matrix.IOS_NUGET_REF_NAME": "Microsoft.iOS.Ref.net9.0_18.2",
        "test_matrix.IOS_NUGET_SDK_NAME": "Microsoft.iOS.Sdk.net9.0_18.2",
        "test_matrix.IOS_NUGET_VERSION_NO_METADATA": "18.2.9527-ci.dev-rolf-windows-tests-html-report",
        "test_matrix.ios-arm64_NUGET_RUNTIME_NAME": "",
        "test_matrix.iossimulator-arm64_NUGET_RUNTIME_NAME": "",
        "test_matrix.iossimulator-x64_NUGET_RUNTIME_NAME": "",
        "test_matrix.PARENT_BUILD_BUILD_BINARIESDIRECTORY": "/mnt/vss/_work/1/b",
        "test_matrix.PARENT_BUILD_BUILD_BUILDID": "11141344",
        "test_matrix.PARENT_BUILD_BUILD_BUILDNUMBER": "20250306.21",
        "test_matrix.PARENT_BUILD_BUILD_BUILDURI": "vstfs:///Build/Build/11141344",
        "test_matrix.PARENT_BUILD_BUILD_DEFINITIONNAME": "xamarin-macios-pr",
        "test_matrix.PARENT_BUILD_BUILD_REASON": "IndividualCI",
        "test_matrix.PARENT_BUILD_BUILD_REPOSITORY_ID": "dotnet/macios",
        "test_matrix.PARENT_BUILD_BUILD_REPOSITORY_NAME": "dotnet/macios",
        "test_matrix.PARENT_BUILD_BUILD_REPOSITORY_PROVIDER": "GitHub",
        "test_matrix.PARENT_BUILD_BUILD_REPOSITORY_URI": "https://github.com/dotnet/macios",
        "test_matrix.PARENT_BUILD_BUILD_SOURCEBRANCH": "refs/heads/dev/rolf/windows-tests-html-report",
        "test_matrix.PARENT_BUILD_BUILD_SOURCEBRANCHNAME": "windows-tests-html-report",
        "test_matrix.PARENT_BUILD_BUILD_SOURCEVERSION": "cd6f17c5cb9b0fabbe0e2af9fc17d57e8aab3c1a",
        "test_matrix.SIMULATOR_TEST_MATRIX": "{\"cecil\":{\"LABEL\":\"cecil\",\"TESTS_LABELS\":\"--label=skip-all-tests,run-ios-tests,run-ios-simulator-tests,run-tvos-tests,run-mac-tests,run-maccatalyst-tests,run-system-permission-tests,run-cecil-tests\",\"TEST_STAGE\":\"simulator_tests\",\"LABEL_WITH_PLATFORM\":\"cecil\",\"STATUS_CONTEXT\":\"VSTS: simulator tests - cecil\",\"TEST_PREFIX\":\"simulator_testscecil\",\"TEST_PLATFORM\":\"\"}}",
        "test_matrix.TEST_MATRIX": "{\"windows\":{\"LABEL\":\"windows\",\"TESTS_LABELS\":\"--label=skip-all-tests,run-ios-tests,run-ios-simulator-tests,run-tvos-tests,run-mac-tests,run-maccatalyst-tests,run-system-permission-tests,run-windows-tests\",\"TEST_STAGE\":\"windows_integration\",\"LABEL_WITH_PLATFORM\":\"windows\",\"STATUS_CONTEXT\":\"VSTS: simulator tests - windows\",\"TEST_PREFIX\":\"windows_integrationwindows\",\"TEST_PLATFORM\":\"\"},\"cecil\":{\"LABEL\":\"cecil\",\"TESTS_LABELS\":\"--label=skip-all-tests,run-ios-tests,run-ios-simulator-tests,run-tvos-tests,run-mac-tests,run-maccatalyst-tests,run-system-permission-tests,run-cecil-tests\",\"TEST_STAGE\":\"simulator_tests\",\"LABEL_WITH_PLATFORM\":\"cecil\",\"STATUS_CONTEXT\":\"VSTS: simulator tests - cecil\",\"TEST_PREFIX\":\"simulator_testscecil\",\"TEST_PLATFORM\":\"\"}}",
        "PublishPipelineArtifact.agent.version": "4.252.0",
        "__system_post_1.agent.version": "4.252.0",
        "__system_post_2.agent.version": "4.252.0",
        "JobExtension_Final.agent.version": "4.252.0"
      },
      "identifier": null,
      "name": "configure",
      "attempt": 1,
      "startTime": null,
      "finishTime": null,
      "state": "NotStarted",
      "result": "Succeeded"
    }
  },
  "simulator_tests": {
    "tests": {
      "outputs": {
        "cecil.agent.version": "3.248.0",
        "cecil.JobExtension_Init.agent.version": "3.248.0",
        "cecil.__system_1.agent.version": "3.248.0",
        "cecil.__system_2.agent.version": "3.248.0",
        "cecil.__system_3.agent.version": "3.248.0",
        "cecil.DownloadSecureFile1.agent.version": "3.248.0",
        "cecil.DownloadSecureFile1.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-AppleWWDRCAG3-2.cer",
        "cecil.lingeringprocesstask.agent.version": "3.248.0",
        "cecil.DownloadSecureFile2.agent.version": "3.248.0",
        "cecil.DownloadSecureFile2.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-developer-id-application-luis-aguilera-jul-2029.p12",
        "cecil.DownloadSecureFile3.agent.version": "3.248.0",
        "cecil.DownloadSecureFile3.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-developer-id-installer-luis-aguilera-jul-2029.p12",
        "cecil.DownloadSecureFile4.agent.version": "3.248.0",
        "cecil.DownloadSecureFile4.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-la-dev-apple.p12",
        "cecil.DownloadSecureFile5.agent.version": "3.248.0",
        "cecil.DownloadSecureFile5.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-la-dev-iPhone.p12",
        "cecil.DownloadSecureFile6.agent.version": "3.248.0",
        "cecil.DownloadSecureFile6.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-la-distr-apple.p12",
        "cecil.DownloadSecureFile7.agent.version": "3.248.0",
        "cecil.DownloadSecureFile7.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-la-distr-iphone.p12",
        "cecil.DownloadSecureFile8.agent.version": "3.248.0",
        "cecil.DownloadSecureFile8.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-la-mac-app-dev.p12",
        "cecil.DownloadSecureFile9.agent.version": "3.248.0",
        "cecil.DownloadSecureFile9.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-la-mac-app-distr.p12",
        "cecil.DownloadSecureFile10.agent.version": "3.248.0",
        "cecil.DownloadSecureFile10.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-la-mac-installer-distr.p12",
        "cecil.DownloadSecureFile11.agent.version": "3.248.0",
        "cecil.DownloadSecureFile11.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-vseng-xamarin-mac-devices-2.p12",
        "cecil.DownloadSecureFile12.agent.version": "3.248.0",
        "cecil.DownloadSecureFile12.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-vseng-xamarin-mac-devices.p12",
        "cecil.DownloadSecureFile13.agent.version": "3.248.0",
        "cecil.DownloadSecureFile13.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-vsengxamarinmacdevices.mobileprovision",
        "cecil.DownloadSecureFile14.agent.version": "3.248.0",
        "cecil.DownloadSecureFile14.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-WildCardiOSDevelopment.mobileprovision",
        "cecil.DownloadSecureFile15.agent.version": "3.248.0",
        "cecil.DownloadSecureFile15.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-WildCardiOSDistribution.mobileprovision",
        "cecil.DownloadSecureFile17.agent.version": "3.248.0",
        "cecil.DownloadSecureFile17.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-WildCardMacDistribution.provisionprofile",
        "cecil.DownloadSecureFile16.agent.version": "3.248.0",
        "cecil.DownloadSecureFile16.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-WildCardMacAppDevelopment.provisionprofile",
        "cecil.DownloadSecureFile18.agent.version": "3.248.0",
        "cecil.DownloadSecureFile18.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-WildCardtvOSDevelopment.mobileprovision",
        "cecil.artifactDropTask.agent.version": "3.248.0",
        "cecil.DownloadSecureFile19.agent.version": "3.248.0",
        "cecil.DownloadSecureFile19.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-WildCardtvOSDistribution.mobileprovision",
        "cecil.__system_4.agent.version": "3.248.0",
        "cecil.DumpEnv.agent.version": "3.248.0",
        "cecil.Checkout1.agent.version": "3.248.0",
        "cecil.fix_commit.agent.version": "3.248.0",
        "cecil.fix_commit.GIT_HASH": "cd6f17c5cb9b0fabbe0e2af9fc17d57e8aab3c1a",
        "cecil.Checkout2.agent.version": "3.248.0",
        "cecil.Checkout3.agent.version": "3.248.0",
        "cecil.__system_postcheckout_1.agent.version": "3.248.0",
        "cecil.Bash1.agent.version": "3.248.0",
        "cecil.osname.agent.version": "3.248.0",
        "cecil.PowerShell1.agent.version": "3.248.0",
        "cecil.PowerShell2.agent.version": "3.248.0",
        "cecil.Bash2.agent.version": "3.248.0",
        "cecil.Bash3.agent.version": "3.248.0",
        "cecil.Bash4.agent.version": "3.248.0",
        "cecil.PowerShell3.agent.version": "3.248.0",
        "cecil.disableCodeQLOnArm64.agent.version": "3.248.0",
        "cecil.Bash5.agent.version": "3.248.0",
        "cecil.PowerShell4.agent.version": "3.248.0",
        "cecil.Bash6.agent.version": "3.248.0",
        "cecil.Bash8.agent.version": "3.248.0",
        "cecil.Bash7.agent.version": "3.248.0",
        "cecil.PowerShell5.agent.version": "3.248.0",
        "cecil.PowerShell6.agent.version": "3.248.0",
        "cecil.PowerShell7.agent.version": "3.248.0",
        "cecil.Bash9.agent.version": "3.248.0",
        "cecil.Bash10.agent.version": "3.248.0",
        "cecil.PowerShell8.agent.version": "3.248.0",
        "cecil.PowerShell10.agent.version": "3.248.0",
        "cecil.PowerShell9.agent.version": "3.248.0",
        "cecil.PowerShell11.agent.version": "3.248.0",
        "cecil.Bash11.agent.version": "3.248.0",
        "cecil.PowerShell12.agent.version": "3.248.0",
        "cecil.Bash12.agent.version": "3.248.0",
        "cecil.Bash13.agent.version": "3.248.0",
        "cecil.Download1.agent.version": "3.248.0",
        "cecil.Download2.agent.version": "3.248.0",
        "cecil.Download3.agent.version": "3.248.0",
        "cecil.PowerShell13.agent.version": "3.248.0",
        "cecil.Bash14.agent.version": "3.248.0",
        "cecil.PowerShell14.agent.version": "3.248.0",
        "cecil.Bash15.agent.version": "3.248.0",
        "cecil.Bash16.agent.version": "3.248.0",
        "cecil.provisionator.agent.version": "3.248.0",
        "cecil.UseDotNet.agent.version": "3.248.0",
        "cecil.Bash17.agent.version": "3.248.0",
        "cecil.Bash18.agent.version": "3.248.0",
        "cecil.Bash19.agent.version": "3.248.0",
        "cecil.PowerShell15.agent.version": "3.248.0",
        "cecil.PowerShell15.TESTS_ATTEMPT": "1",
        "cecil.PowerShell15.TESTS_BOT": "VSM-XAM-104.Sequoia.arm64",
        "cecil.PowerShell15.TESTS_LABEL": "cecil",
        "cecil.PowerShell15.TESTS_PLATFORM": "",
        "cecil.PowerShell15.TESTS_TITLE": "cecil",
        "cecil.PowerShell16.agent.version": "3.248.0",
        "cecil.PowerShell17.agent.version": "3.248.0",
        "cecil.PowerShell18.agent.version": "3.248.0",
        "cecil.PowerShell19.agent.version": "3.248.0",
        "cecil.Bash20.agent.version": "3.248.0",
        "cecil.PowerShell20.agent.version": "3.248.0",
        "cecil.Bash21.agent.version": "3.248.0",
        "cecil.Bash22.agent.version": "3.248.0",
        "cecil.runTests.agent.version": "3.248.0",
        "cecil.runTests.TESTS_JOBSTATUS": "Succeeded",
        "cecil.collectSimulatorInfo.agent.version": "3.248.0",
        "cecil.PublishPipelineArtifact1.agent.version": "3.248.0",
        "cecil.PublishPipelineArtifact2.agent.version": "3.248.0",
        "cecil.PowerShell21.agent.version": "3.248.0",
        "cecil.CountNUnitTestResults.agent.version": "3.248.0",
        "cecil.PublishTestResults.agent.version": "3.248.0",
        "cecil.ArchiveFiles.agent.version": "3.248.0",
        "cecil.PublishPipelineArtifact3.agent.version": "3.248.0",
        "cecil.CmdLine.agent.version": "3.248.0",
        "cecil.PublishPipelineArtifact4.agent.version": "3.248.0",
        "cecil.Bash23.agent.version": "3.248.0",
        "cecil.PowerShell22.agent.version": "3.248.0",
        "cecil.PowerShell23.agent.version": "3.248.0",
        "cecil.Bash24.agent.version": "3.248.0",
        "cecil.list1.agent.version": "3.248.0",
        "cecil.Bash25.agent.version": "3.248.0",
        "cecil.list2.agent.version": "3.248.0",
        "cecil.list3.agent.version": "3.248.0",
        "cecil.Bash26.agent.version": "3.248.0",
        "cecil.list4.agent.version": "3.248.0",
        "cecil.PowerShell24.agent.version": "3.248.0",
        "cecil.PublishPipelineArtifact5.agent.version": "3.248.0",
        "cecil.__system_post_2.agent.version": "3.248.0",
        "cecil.__system_post_1.agent.version": "3.248.0",
        "cecil.JobExtension_Final.agent.version": "3.248.0"
      },
      "identifier": null,
      "name": "tests",
      "attempt": 1,
      "startTime": null,
      "finishTime": null,
      "state": "NotStarted",
      "result": "Succeeded"
    }
  },
  "windows_integration": {
    "mac_reservation": {
      "outputs": {
        "agent.version": "3.246.0",
        "JobExtension_Init.agent.version": "3.246.0",
        "__system_1.agent.version": "3.246.0",
        "__system_2.agent.version": "3.246.0",
        "__system_3.agent.version": "3.246.0",
        "DownloadSecureFile1.agent.version": "3.246.0",
        "DownloadSecureFile1.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-AppleWWDRCAG3-2.cer",
        "DownloadSecureFile3.agent.version": "3.246.0",
        "DownloadSecureFile3.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-developer-id-installer-luis-aguilera-jul-2029.p12",
        "DownloadSecureFile2.agent.version": "3.246.0",
        "DownloadSecureFile2.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-developer-id-application-luis-aguilera-jul-2029.p12",
        "DownloadSecureFile4.agent.version": "3.246.0",
        "DownloadSecureFile4.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-la-dev-apple.p12",
        "DownloadSecureFile5.agent.version": "3.246.0",
        "DownloadSecureFile5.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-la-dev-iPhone.p12",
        "DownloadSecureFile7.agent.version": "3.246.0",
        "DownloadSecureFile7.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-la-distr-iphone.p12",
        "DownloadSecureFile6.agent.version": "3.246.0",
        "DownloadSecureFile6.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-la-distr-apple.p12",
        "DownloadSecureFile8.agent.version": "3.246.0",
        "DownloadSecureFile8.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-la-mac-app-dev.p12",
        "DownloadSecureFile9.agent.version": "3.246.0",
        "DownloadSecureFile9.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-la-mac-app-distr.p12",
        "DownloadSecureFile10.agent.version": "3.246.0",
        "DownloadSecureFile10.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-la-mac-installer-distr.p12",
        "DownloadSecureFile11.agent.version": "3.246.0",
        "DownloadSecureFile11.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-vseng-xamarin-mac-devices-2.p12",
        "DownloadSecureFile12.agent.version": "3.246.0",
        "DownloadSecureFile12.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-vseng-xamarin-mac-devices.p12",
        "DownloadSecureFile13.agent.version": "3.246.0",
        "DownloadSecureFile13.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-vsengxamarinmacdevices.mobileprovision",
        "DownloadSecureFile14.agent.version": "3.246.0",
        "DownloadSecureFile14.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-WildCardiOSDevelopment.mobileprovision",
        "DownloadSecureFile15.agent.version": "3.246.0",
        "DownloadSecureFile15.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-WildCardiOSDistribution.mobileprovision",
        "DownloadSecureFile16.agent.version": "3.246.0",
        "DownloadSecureFile16.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-WildCardMacAppDevelopment.provisionprofile",
        "DownloadSecureFile17.agent.version": "3.246.0",
        "DownloadSecureFile17.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-WildCardMacDistribution.provisionprofile",
        "DownloadSecureFile18.agent.version": "3.246.0",
        "DownloadSecureFile18.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-WildCardtvOSDevelopment.mobileprovision",
        "DownloadSecureFile19.agent.version": "3.246.0",
        "DownloadSecureFile19.secureFilePath": "/Users/builder/azdo/_work/_temp/macios-WildCardtvOSDistribution.mobileprovision",
        "AzureKeyVault.agent.version": "3.246.0",
        "__system_4.agent.version": "3.246.0",
        "Checkout1.agent.version": "3.246.0",
        "fix_commit.agent.version": "3.246.0",
        "fix_commit.GIT_HASH": "cd6f17c5cb9b0fabbe0e2af9fc17d57e8aab3c1a",
        "Checkout2.agent.version": "3.246.0",
        "Checkout3.agent.version": "3.246.0",
        "__system_postcheckout_1.agent.version": "3.246.0",
        "PowerShell1.agent.version": "3.246.0",
        "Bash1.agent.version": "3.246.0",
        "disableCodeQLOnArm64.agent.version": "3.246.0",
        "macInfo.agent.version": "3.246.0",
        "macInfo.AGENT_BUILD_SOURCESDIRECTORY": "/Users/builder/azdo/_work/28/s",
        "macInfo.AGENT_IP": "10.64.32.145",
        "macInfo.AGENT_NAME": "VSM-XAM-49.Sonoma.arm64",
        "macInfo.AGENT_POOL": "VSEng-VSMac-Xamarin-Shared",
        "macInfo.AGENT_SYSTEM_DEFAULTWORKINGDIRECTORY": "/Users/builder/azdo/_work/28/s",
        "PowerShell2.agent.version": "3.246.0",
        "Bash2.agent.version": "3.246.0",
        "PowerShell3.agent.version": "3.246.0",
        "PowerShell4.agent.version": "3.246.0",
        "PowerShell5.agent.version": "3.246.0",
        "Bash3.agent.version": "3.246.0",
        "PowerShell6.agent.version": "3.246.0",
        "PowerShell7.agent.version": "3.246.0",
        "PowerShell8.agent.version": "3.246.0",
        "PowerShell9.agent.version": "3.246.0",
        "PowerShell10.agent.version": "3.246.0",
        "AzureCLI.agent.version": "3.246.0",
        "PowerShell11.agent.version": "3.246.0",
        "Bash4.agent.version": "3.246.0",
        "provisionator.agent.version": "3.246.0",
        "UseDotNet.agent.version": "3.246.0",
        "Bash5.agent.version": "3.246.0",
        "Bash6.agent.version": "3.246.0",
        "Download1.agent.version": "3.246.0",
        "Download2.agent.version": "3.246.0",
        "Download3.agent.version": "3.246.0",
        "Download4.agent.version": "3.246.0",
        "PowerShell12.agent.version": "3.246.0",
        "Download5.agent.version": "3.246.0",
        "Bash7.agent.version": "3.246.0",
        "Bash8.agent.version": "3.246.0",
        "Bash9.agent.version": "3.246.0",
        "__system_post_1.agent.version": "3.246.0",
        "__system_post_2.agent.version": "3.246.0",
        "JobExtension_Final.agent.version": "3.246.0"
      },
      "identifier": null,
      "name": "mac_reservation",
      "attempt": 1,
      "startTime": null,
      "finishTime": null,
      "state": "NotStarted",
      "result": "Succeeded"
    },
    "tests": {
      "outputs": {
        "fix_commit.GIT_HASH": "cd6f17c5cb9b0fabbe0e2af9fc17d57e8aab3c1a",
        "PowerShell6.TESTS_ATTEMPT": "1",
        "PowerShell6.TESTS_BOT": "VSM-XAM-126",
        "PowerShell6.TESTS_LABEL": "windows",
        "PowerShell6.TESTS_PLATFORM": "",
        "PowerShell6.TESTS_TITLE": "windows",
        "configuration.DOTNET_IOS_RUNTIME_IDENTIFIERS": "ios-arm64 iossimulator-x64 iossimulator-arm64",
        "configuration.DOTNET_PLATFORMS": "iOS",
        "configuration.INCLUDE_DOTNET_IOS": "1",
        "configuration.IOS_NUGET_REF_NAME": "Microsoft.iOS.Ref.net9.0_18.2",
        "configuration.IOS_NUGET_SDK_NAME": "Microsoft.iOS.Sdk.net9.0_18.2",
        "configuration.IOS_NUGET_VERSION_NO_METADATA": "18.2.9527-ci.dev-rolf-windows-tests-html-report",
        "configuration.ios-arm64_NUGET_RUNTIME_NAME": "",
        "configuration.iossimulator-arm64_NUGET_RUNTIME_NAME": "",
        "configuration.iossimulator-x64_NUGET_RUNTIME_NAME": "",
        "configuration.PARENT_BUILD_BUILD_BINARIESDIRECTORY": "/mnt/vss/_work/1/b",
        "configuration.PARENT_BUILD_BUILD_BUILDID": "11141344",
        "configuration.PARENT_BUILD_BUILD_BUILDNUMBER": "20250306.21",
        "configuration.PARENT_BUILD_BUILD_BUILDURI": "vstfs:///Build/Build/11141344",
        "configuration.PARENT_BUILD_BUILD_DEFINITIONNAME": "xamarin-macios-pr",
        "configuration.PARENT_BUILD_BUILD_REASON": "IndividualCI",
        "configuration.PARENT_BUILD_BUILD_REPOSITORY_ID": "dotnet/macios",
        "configuration.PARENT_BUILD_BUILD_REPOSITORY_NAME": "dotnet/macios",
        "configuration.PARENT_BUILD_BUILD_REPOSITORY_PROVIDER": "GitHub",
        "configuration.PARENT_BUILD_BUILD_REPOSITORY_URI": "https://github.com/dotnet/macios",
        "configuration.PARENT_BUILD_BUILD_SOURCEBRANCH": "refs/heads/dev/rolf/windows-tests-html-report",
        "configuration.PARENT_BUILD_BUILD_SOURCEBRANCHNAME": "windows-tests-html-report",
        "configuration.PARENT_BUILD_BUILD_SOURCEVERSION": "cd6f17c5cb9b0fabbe0e2af9fc17d57e8aab3c1a",
        "PowerShell22.TESTS_JOBSTATUS": "Succeeded"
      },
      "identifier": null,
      "name": "tests",
      "attempt": 1,
      "startTime": null,
      "finishTime": null,
      "state": "NotStarted",
      "result": "Succeeded"
    },
    "mac_reenable": {
      "outputs": {
        "fix_commit.GIT_HASH": "cd6f17c5cb9b0fabbe0e2af9fc17d57e8aab3c1a"
      },
      "identifier": null,
      "name": "mac_reenable",
      "attempt": 1,
      "startTime": null,
      "finishTime": null,
      "state": "NotStarted",
      "result": "Succeeded"
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

    Context "skipped due to beta version mismatch" {
        BeforeAll {
            $skipDir = Join-Path -Path $TestDrive -ChildPath "skipped_beta"
            New-Item -Path $skipDir -ItemType Directory -Force | Out-Null
            $skipPath = Join-Path -Path $skipDir -ChildPath "TestSummary.md"
            Set-Content -Path $skipPath -Value "# ⚠️ arm64 - Mac Golden Gate (27): Tests skipped, incorrect beta version`n`nTests skipped: current macOS build version '26A5368g' does not match expected '26A5372a'.`n"
            $testResult = [TestResult]::new($skipPath, "Succeeded", $testConfig, $attempt)
        }

        It "is a success" {
            $testResult.IsSuccess() | Should -Be $true
        }

        It "does not throw and reports zero passed tests." {
            $result = $testResult.GetPassedTests()
            $result.Passed | Should -Be 0
            $result.Failed | Should -Be 0
            $testResult.Skipped | Should -Be $true
        }
    }

    Context "new test summmary results" -Skip {
        It "finds the right stuff" {
            $testDirectory = Join-Path "." "subdir"
            New-Item -Path "$testDirectory" -ItemType "directory" -Force
            New-Item -Path "$testDirectory/TestSummary-simulator_testscecil-1" -Name "TestSummary.md" -Value "SummaryA" -Force
            New-Item -Path "$testDirectory/TestSummary-simulator_testscecil-2" -Name "TestSummary.md" -Value "SummaryB" -Force
            New-Item -Path "$testDirectory/TestSummary-simulator_testsdotnettests_iOS-1" -Name "TestSummary.md" -Value "SummaryC" -Force
            New-Item -Path "$testDirectory/TestSummary-simulator_testsdotnettests_tvOS-1" -Name "TestSummary.md" -Value "SummaryD" -Force
            New-Item -Path "$testDirectory/TestSummary-simulator_testsdotnettests_MacCatalyst-1" -Name "TestSummary.md" -Value "SummaryE" -Force
            New-Item -Path "$testDirectory/TestSummary-simulator_testsdotnettests_macOS-1" -Name "TestSummary.md" -Value "SummaryF" -Force
            New-Item -Path "$testDirectory/TestSummary-simulator_testsdotnettests_Multiple-1" -Name "TestSummary.md" -Value "SummaryF" -Force

            $testResults = New-TestSummaryResults -Path "$testDirectory" -Testsimulator_tests "simulator_tests" -StageDependencies "$stageDependencies"

            # Remove-Item -Path $testDirectory -Recurse

            Write-Host $testResults

            $testResults.count | Should -Be 12

            $testResults[0].Label | Should -Be "linker"
            $testResults[0].Context | Should -Be " - linker"
            $testResults[0].ResultsPath | Should -Be "$(get-location)/subdir/TestSummary-simulator_testslinker-200/TestSummary.md"
            $testResults[0].TestsJobStatus | Should -Be "yay"

            $testResults[1].Label | Should -Be "introspection"
            $testResults[1].Context | Should -Be " - introspection"
            $testResults[1].ResultsPath | Should -Be "$(get-location)/subdir/TestSummary-simulator_testsintrospection-2/TestSummary.md"
            $testResults[1].TestsJobStatus | Should -Be "nay"

            $testResults[2].Label | Should -Be "monotouch_test"
            $testResults[2].Context | Should -Be " - monotouch_test"
            $testResults[2].ResultsPath | Should -Be "./subdir/TestSummary-simulator_testsmonotouch_test-1/TestSummary.md"
            $testResults[2].TestsJobStatus | Should -Be ""
        }

        It "computes the right summary with missing test results 2" {
            $VerbosePreference = "Continue"
            $DebugPreference = "Continue"
            $Env:MyVerbosePreference = 'Continue'

            $testDirectory = Join-Path "." "subdir"
            New-Item -Path "$testDirectory" -ItemType "directory" -Force
            New-Item -Path "$testDirectory/TestSummary-simulator_testscecil-1" -Name "TestSummary.md" -Value "# :tada: All 1 tests passed :tada:" -Force
            New-Item -Path "$testDirectory/TestSummary-simulator_testsdotnettests_Multiple-1" -Name "TestSummary.md" -Value "# :tada: All 7 tests passed :tada:" -Force

            $parallelResults = New-ParallelTestsResults -Path "$testDirectory" -Testsimulator_tests "simulator_tests" -StageDependencies "$stageDependenciesWithMissingResults" -Context "context" -VSDropsIndex "vsdropsIndex"

            $parallelResults.IsSuccess() | Should -Be $false

            $sb = [System.Text.StringBuilder]::new()
            $parallelResults.WriteComment($sb)

            Remove-Item -Path $testDirectory -Recurse

            $content = $sb.ToString()

            Write-Host $content

            $content | Should -Be "[comment]: <> (This is a test result report added by Azure DevOps)

# Test results
:x: Tests failed on context

0 tests crashed, 5 tests failed, 27 tests passed.

## Failures

### :x: dotnettests tests (MacCatalyst)

<details>
<summary>5 tests failed, 6 tests passed.</summary>

</details>

[Html Report (VSDrops)](vsdropsIndex/simulator_testsdotnettests_MacCatalyst-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-simulator_testsdotnettests_MacCatalyst-1&api-version=6.0&`$format=zip)

## Successes

:white_check_mark: cecil: All 1 tests passed. [Html Report (VSDrops)](vsdropsIndex/simulator_testscecil-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-simulator_testscecil-1&api-version=6.0&`$format=zip)
:white_check_mark: dotnettests (tvOS): All 4 tests passed. [Html Report (VSDrops)](vsdropsIndex/simulator_testsdotnettests_tvOS-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-simulator_testsdotnettests_tvOS-1&api-version=6.0&`$format=zip)
:white_check_mark: dotnettests (macOS): All 6 tests passed. [Html Report (VSDrops)](vsdropsIndex/simulator_testsdotnettests_macOS-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-simulator_testsdotnettests_macOS-1&api-version=6.0&`$format=zip)
:white_check_mark: dotnettests (iOS): All 3 tests passed. [Html Report (VSDrops)](vsdropsIndex/simulator_testsdotnettests_iOS-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-simulator_testsdotnettests_iOS-1&api-version=6.0&`$format=zip)
:white_check_mark: dotnettests (Multiple platforms): All 7 tests passed. [Html Report (VSDrops)](vsdropsIndex/simulator_testsdotnettests_Multiple-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-simulator_testsdotnettests_Multiple-1&api-version=6.0&`$format=zip)

"
        }
    }

    Context "new test summmary results" {
        It "computes the right summary with windows tests" {
            $VerbosePreference = "Continue"
            $DebugPreference = "Continue"
            $Env:MyVerbosePreference = 'Continue'

            $testDirectory = Join-Path "." "subdir"
            New-Item -Path "$testDirectory" -ItemType "directory" -Force
            New-Item -Path "$testDirectory/TestSummary-simulator_testscecil-1" -Name "TestSummary.md" -Value "# :tada: All 1 tests passed :tada:" -Force
            New-Item -Path "$testDirectory/TestSummary-windows_integrationwindows-1" -Name "TestSummary.md" -Value "# :tada: All 2 tests passed :tada:" -Force

            $parallelResults = New-ParallelTestsResults -Path "$testDirectory" -StageDependencies "$stageDependenciesWithWindowsResults" -Context "context" -VSDropsIndex "vsdropsIndex"

            $parallelResults.IsSuccess() | Should -Be $true

            $sb = [System.Text.StringBuilder]::new()
            $parallelResults.WriteComment($sb)

            Remove-Item -Path $testDirectory -Recurse

            $content = $sb.ToString()

            Write-Host $content

            $content | Should -Be "[comment]: <> (This is a test result report added by Azure DevOps)

# Test results
:white_check_mark: All tests passed on context.

:tada: All 3 tests passed :tada:

## Tests counts
:white_check_mark: cecil: All 1 tests passed. [Html Report (VSDrops)](vsdropsIndex/simulator_testscecil-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-simulator_testscecil-1&api-version=6.0&`$format=zip)
:white_check_mark: windows: All 2 tests passed. [Html Report (VSDrops)](vsdropsIndex/windows_integrationwindows-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-windows_integrationwindows-1&api-version=6.0&`$format=zip)

"
        }

        It "computes the right summary with missing test results" {
            $VerbosePreference = "Continue"
            $Env:MyVerbosePreference = 'Continue'

            $testDirectory = Join-Path "." "subdir"
            New-Item -Path "$testDirectory" -ItemType "directory" -Force
            New-Item -Path "$testDirectory/TestSummary-simulator_testscecil-1" -Name "TestSummary.md" -Value "# :tada: All 1 tests passed :tada:" -Force
            New-Item -Path "$testDirectory/TestSummary-simulator_testscecil-2" -Name "TestSummary.md" -Value "# :tada: All 2 tests passed :tada:" -Force
            New-Item -Path "$testDirectory/TestSummary-simulator_testsdotnettests_ios-1" -Name "TestSummary.md" -Value "# :tada: All 3 tests passed :tada:" -Force
            New-Item -Path "$testDirectory/TestSummary-simulator_testsdotnettests_tvos-1" -Name "TestSummary.md" -Value "# :tada: All 4 tests passed :tada:" -Force
            New-Item -Path "$testDirectory/TestSummary-simulator_testsdotnettests_maccatalyst-1" -Name "TestSummary.md" -Value "# :tada: All 5 tests passed :tada:" -Force
            New-Item -Path "$testDirectory/TestSummary-simulator_testsdotnettests_macos-1" -Name "TestSummary.md" -Value "# :tada: All 6 tests passed :tada:" -Force
            New-Item -Path "$testDirectory/TestSummary-simulator_testsdotnettests_multiple-1" -Name "TestSummary.md" -Value "# :tada: All 7 tests passed :tada:" -Force


            $parallelResults = New-ParallelTestsResults -Path "$testDirectory" -StageDependencies "$stageDependencies" -Context "context" -VSDropsIndex "vsdropsIndex"

            $parallelResults.IsSuccess() | Should -Be $false

            $sb = [System.Text.StringBuilder]::new()
            $parallelResults.WriteComment($sb)

            Remove-Item -Path $testDirectory -Recurse

            $content = $sb.ToString()

            Write-Host $content.Replace("&$", "&``$")

            $content | Should -Be "[comment]: <> (This is a test result report added by Azure DevOps)

# Test results
:x: Tests failed on context

0 tests crashed, 1 tests failed, 21 tests passed.

## Failures

### :x: dotnettests tests (MacCatalyst)

<details>
<summary>1 tests failed, 0 tests passed.</summary>
Test results reported success, but the tests job failed.
</details>

[Html Report (VSDrops)](vsdropsIndex/testStagedotnettests_maccatalyst-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-testStagedotnettests_maccatalyst-1&api-version=6.0&`$format=zip)

## Successes

:white_check_mark: cecil: All 1 tests passed. [Html Report (VSDrops)](vsdropsIndex/testStagececil-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-testStagececil-1&api-version=6.0&`$format=zip)
:white_check_mark: dotnettests (iOS): All 3 tests passed. [Html Report (VSDrops)](vsdropsIndex/testStagedotnettests_ios-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-testStagedotnettests_ios-1&api-version=6.0&`$format=zip)
:white_check_mark: dotnettests (macOS): All 6 tests passed. [Html Report (VSDrops)](vsdropsIndex/testStagedotnettests_macos-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-testStagedotnettests_macos-1&api-version=6.0&`$format=zip)
:white_check_mark: dotnettests (Multiple platforms): All 7 tests passed. [Html Report (VSDrops)](vsdropsIndex/testStagedotnettests_multiple-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-testStagedotnettests_multiple-1&api-version=6.0&`$format=zip)
:white_check_mark: dotnettests (tvOS): All 4 tests passed. [Html Report (VSDrops)](vsdropsIndex/testStagedotnettests_tvos-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-testStagedotnettests_tvos-1&api-version=6.0&`$format=zip)

"
        }

        It "computes the right summary with failing tests" {
            $VerbosePreference = "Continue"
            $Env:MyVerbosePreference = 'Continue'

            $testDirectory = Join-Path "." "subdir"
            New-Item -Path "$testDirectory" -ItemType "directory" -Force
            New-Item -Path "$testDirectory/TestSummary-simulator_testscecil-1" -Name "TestSummary.md" -Value "# :tada: All 1 tests passed :tada:" -Force
            New-Item -Path "$testDirectory/TestSummary-simulator_testsdotnettests_ios-1" -Name "TestSummary.md" -Value "# :tada: All 3 tests passed :tada:" -Force
            New-Item -Path "$testDirectory/TestSummary-simulator_testsdotnettests_tvos-1" -Name "TestSummary.md" -Value "# :tada: All 4 tests passed :tada:" -Force
            New-Item -Path "$testDirectory/TestSummary-simulator_testsdotnettests_maccatalyst-1" -Name "TestSummary.md" -Value "<summary>5 tests failed, 6 tests passed.</summary>" -Force
            New-Item -Path "$testDirectory/TestSummary-simulator_testsdotnettests_macos-1" -Name "TestSummary.md" -Value "# Test results

<details>
<summary>1 tests failed, 4 tests passed.</summary>

## Failed tests

 * Roslyn Transformer tests: TimedOut (Execution timed out after 30 minutes.)
</details>" -Force
            New-Item -Path "$testDirectory/TestSummary-simulator_testsdotnettests_multiple-1" -Name "TestSummary.md" -Value "# :tada: All 7 tests passed :tada:" -Force

            $parallelResults = New-ParallelTestsResults -Path "$testDirectory" -StageDependencies "$stageDependenciesWithMoreFailingTests" -Context "context" -VSDropsIndex "vsdropsIndex"

            Write-Host "New-ParallelTestsResults return value:"
            Write-Host $parallelResults

            $parallelResults.IsSuccess() | Should -Be $false

            $sb = [System.Text.StringBuilder]::new()
            $parallelResults.WriteComment($sb)

            Remove-Item -Path $testDirectory -Recurse

            $content = $sb.ToString()

            Write-Host $content.Replace("&$", "&``$")

            $content | Should -Be "[comment]: <> (This is a test result report added by Azure DevOps)

# Test results
:x: Tests failed on context

0 tests crashed, 6 tests failed, 25 tests passed.

## Failures

### :x: dotnettests tests (MacCatalyst)

<details>
<summary>5 tests failed, 6 tests passed.</summary>
</details>

[Html Report (VSDrops)](vsdropsIndex/testStagedotnettests_maccatalyst-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-testStagedotnettests_maccatalyst-1&api-version=6.0&`$format=zip)

### :x: dotnettests tests (macOS)

<details>
<summary>1 tests failed, 4 tests passed.</summary>

## Failed tests

 * Roslyn Transformer tests: TimedOut (Execution timed out after 30 minutes.)
</details>

[Html Report (VSDrops)](vsdropsIndex/testStagedotnettests_macos-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-testStagedotnettests_macos-1&api-version=6.0&`$format=zip)

## Successes

:white_check_mark: cecil: All 1 tests passed. [Html Report (VSDrops)](vsdropsIndex/testStagececil-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-testStagececil-1&api-version=6.0&`$format=zip)
:white_check_mark: dotnettests (iOS): All 3 tests passed. [Html Report (VSDrops)](vsdropsIndex/testStagedotnettests_ios-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-testStagedotnettests_ios-1&api-version=6.0&`$format=zip)
:white_check_mark: dotnettests (Multiple platforms): All 7 tests passed. [Html Report (VSDrops)](vsdropsIndex/testStagedotnettests_multiple-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-testStagedotnettests_multiple-1&api-version=6.0&`$format=zip)
:white_check_mark: dotnettests (tvOS): All 4 tests passed. [Html Report (VSDrops)](vsdropsIndex/testStagedotnettests_tvos-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-testStagedotnettests_tvos-1&api-version=6.0&`$format=zip)

"
        }
    }

    Context "macOS bot test results" {
        It "includes macOS test results in the unified comment" {
            $VerbosePreference = "Continue"
            $DebugPreference = "Continue"

            $macMatrix = @"
{
    "cecil": {
        "LABEL": "cecil",
        "TESTS_LABELS": "--label=skip-all-tests,run-cecil-tests",
        "TEST_STAGE": "simulator_tests",
        "LABEL_WITH_PLATFORM": "cecil",
        "STATUS_CONTEXT": "VSTS: simulator tests - cecil",
        "TEST_PREFIX": "simulator_testscecil",
        "TEST_PLATFORM": ""
    },
    "mac_monterey": {
        "LABEL": "mac_monterey",
        "TESTS_LABELS": "--label=skip-all-tests,run-mac_monterey-tests",
        "TEST_STAGE": "mac_12_m1",
        "LABEL_WITH_PLATFORM": "mac_monterey",
        "STATUS_CONTEXT": " - mac_monterey",
        "TEST_PREFIX": "mac_12_m1mac_monterey",
        "TEST_PLATFORM": "",
        "DISPLAY_NAME": "Tests on macOS Monterey (12)",
        "IS_MAC_TEST": "true"
    }
}
"@
            $macStageDeps = @"
{
  "configure_build": {
    "configure": {
      "outputs": {
        "test_matrix.TEST_MATRIX": "$($macMatrix.Replace("`n", "\n").Replace("`"", "\`""))"
      }
    }
  },
  "simulator_tests": {
    "tests": {
      "outputs": {
        "cecil.PowerShell15.TESTS_ATTEMPT": "1",
        "cecil.PowerShell15.TESTS_BOT": "XAMMINI-013.Ventura",
        "cecil.PowerShell15.TESTS_LABEL": "cecil",
        "cecil.PowerShell15.TESTS_PLATFORM": "",
        "cecil.PowerShell15.TESTS_TITLE": "cecil",
        "cecil.runTests.TESTS_JOBSTATUS": "Succeeded"
      },
      "identifier": null,
      "name": "tests",
      "attempt": 1,
      "startTime": null,
      "finishTime": null,
      "state": "NotStarted",
      "result": "Succeeded"
    }
  },
  "mac_12_m1": {
    "tests": {
      "outputs": {
        "PowerShell1.TESTS_ATTEMPT": "1",
        "PowerShell1.TESTS_BOT": "XAMMINI-020.Monterey",
        "PowerShell1.TESTS_LABEL": "mac_monterey",
        "PowerShell1.TESTS_PLATFORM": "",
        "PowerShell1.TESTS_TITLE": "mac_monterey",
        "runTests.TESTS_JOBSTATUS": "Succeeded"
      },
      "identifier": null,
      "name": "tests",
      "attempt": 1,
      "startTime": null,
      "finishTime": null,
      "state": "NotStarted",
      "result": "Succeeded"
    }
  }
}
"@
            $testDirectory = Join-Path "." "subdir"
            New-Item -Path "$testDirectory" -ItemType "directory" -Force
            New-Item -Path "$testDirectory/TestSummary-simulator_testscecil-1" -Name "TestSummary.md" -Value "# :tada: All 1 tests passed :tada:" -Force
            New-Item -Path "$testDirectory/TestSummary-mac_12_m1mac_monterey-1" -Name "TestSummary.md" -Value "# :tada: All 5 tests passed :tada:" -Force

            $parallelResults = New-ParallelTestsResults -Path "$testDirectory" -StageDependencies "$macStageDeps" -Context "context" -VSDropsIndex "vsdropsIndex"

            $parallelResults.IsSuccess() | Should -Be $true

            $sb = [System.Text.StringBuilder]::new()
            $parallelResults.WriteComment($sb)

            Remove-Item -Path $testDirectory -Recurse

            $content = $sb.ToString()

            Write-Host $content

            $content | Should -Be "[comment]: <> (This is a test result report added by Azure DevOps)

# Test results
:white_check_mark: All tests passed on context.

:tada: All 6 tests passed :tada:

## Tests counts
:white_check_mark: cecil: All 1 tests passed. [Html Report (VSDrops)](vsdropsIndex/simulator_testscecil-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-simulator_testscecil-1&api-version=6.0&`$format=zip)

## macOS tests

:white_check_mark: Tests on macOS Monterey (12): All 5 tests passed. [Html Report (VSDrops)](vsdropsIndex/mac_12_m1mac_monterey-1/;/tests/vsdrops_index.html) [Download](/_apis/build/builds//artifacts?artifactName=HtmlReport-mac_12_m1mac_monterey-1&api-version=6.0&`$format=zip)

"
        }

        It "sorts macOS test results by version number" {
            $VerbosePreference = "Continue"
            $DebugPreference = "Continue"

            $sortMatrix = @"
{
    "mac_ventura": {
        "LABEL": "mac_ventura",
        "TESTS_LABELS": "--label=skip-all-tests,run-mac_ventura-tests",
        "TEST_STAGE": "mac_13_m1",
        "LABEL_WITH_PLATFORM": "mac_ventura",
        "STATUS_CONTEXT": " - mac_ventura",
        "TEST_PREFIX": "mac_13_m1mac_ventura",
        "TEST_PLATFORM": "",
        "DISPLAY_NAME": "Tests on macOS Ventura (13)",
        "IS_MAC_TEST": "true"
    },
    "mac_monterey": {
        "LABEL": "mac_monterey",
        "TESTS_LABELS": "--label=skip-all-tests,run-mac_monterey-tests",
        "TEST_STAGE": "mac_12_m1",
        "LABEL_WITH_PLATFORM": "mac_monterey",
        "STATUS_CONTEXT": " - mac_monterey",
        "TEST_PREFIX": "mac_12_m1mac_monterey",
        "TEST_PLATFORM": "",
        "DISPLAY_NAME": "Tests on macOS Monterey (12)",
        "IS_MAC_TEST": "true"
    },
    "mac_sequoia": {
        "LABEL": "mac_sequoia",
        "TESTS_LABELS": "--label=skip-all-tests,run-mac_sequoia-tests",
        "TEST_STAGE": "mac_15_arm64",
        "LABEL_WITH_PLATFORM": "mac_sequoia",
        "STATUS_CONTEXT": " - mac_sequoia",
        "TEST_PREFIX": "mac_15_arm64mac_sequoia",
        "TEST_PLATFORM": "",
        "DISPLAY_NAME": "Tests on macOS Sequoia (15)",
        "IS_MAC_TEST": "true"
    },
    "mac_sonoma": {
        "LABEL": "mac_sonoma",
        "TESTS_LABELS": "--label=skip-all-tests,run-mac_sonoma-tests",
        "TEST_STAGE": "mac_14_x64",
        "LABEL_WITH_PLATFORM": "mac_sonoma",
        "STATUS_CONTEXT": " - mac_sonoma",
        "TEST_PREFIX": "mac_14_x64mac_sonoma",
        "TEST_PLATFORM": "",
        "DISPLAY_NAME": "Tests on macOS Sonoma (14)",
        "IS_MAC_TEST": "true"
    }
}
"@
            $sortStageDeps = @"
{
  "configure_build": {
    "configure": {
      "outputs": {
        "test_matrix.TEST_MATRIX": "$($sortMatrix.Replace("`n", "\n").Replace("`"", "\`""))"
      }
    }
  },
  "mac_13_m1": {
    "tests": {
      "outputs": {
        "PowerShell1.TESTS_ATTEMPT": "1",
        "PowerShell1.TESTS_BOT": "BOT-013",
        "PowerShell1.TESTS_LABEL": "mac_ventura",
        "PowerShell1.TESTS_PLATFORM": "",
        "PowerShell1.TESTS_TITLE": "mac_ventura",
        "runTests.TESTS_JOBSTATUS": "Succeeded"
      },
      "identifier": null, "name": "tests", "attempt": 1, "startTime": null, "finishTime": null, "state": "NotStarted", "result": "Succeeded"
    }
  },
  "mac_12_m1": {
    "tests": {
      "outputs": {
        "PowerShell1.TESTS_ATTEMPT": "1",
        "PowerShell1.TESTS_BOT": "BOT-012",
        "PowerShell1.TESTS_LABEL": "mac_monterey",
        "PowerShell1.TESTS_PLATFORM": "",
        "PowerShell1.TESTS_TITLE": "mac_monterey",
        "runTests.TESTS_JOBSTATUS": "Succeeded"
      },
      "identifier": null, "name": "tests", "attempt": 1, "startTime": null, "finishTime": null, "state": "NotStarted", "result": "Succeeded"
    }
  },
  "mac_15_arm64": {
    "tests": {
      "outputs": {
        "PowerShell1.TESTS_ATTEMPT": "1",
        "PowerShell1.TESTS_BOT": "BOT-015",
        "PowerShell1.TESTS_LABEL": "mac_sequoia",
        "PowerShell1.TESTS_PLATFORM": "",
        "PowerShell1.TESTS_TITLE": "mac_sequoia",
        "runTests.TESTS_JOBSTATUS": "Succeeded"
      },
      "identifier": null, "name": "tests", "attempt": 1, "startTime": null, "finishTime": null, "state": "NotStarted", "result": "Succeeded"
    }
  },
  "mac_14_x64": {
    "tests": {
      "outputs": {
        "PowerShell1.TESTS_ATTEMPT": "1",
        "PowerShell1.TESTS_BOT": "BOT-014",
        "PowerShell1.TESTS_LABEL": "mac_sonoma",
        "PowerShell1.TESTS_PLATFORM": "",
        "PowerShell1.TESTS_TITLE": "mac_sonoma",
        "runTests.TESTS_JOBSTATUS": "Succeeded"
      },
      "identifier": null, "name": "tests", "attempt": 1, "startTime": null, "finishTime": null, "state": "NotStarted", "result": "Succeeded"
    }
  }
}
"@
            $testDirectory = Join-Path "." "subdir_sort"
            New-Item -Path "$testDirectory" -ItemType "directory" -Force
            New-Item -Path "$testDirectory/TestSummary-mac_12_m1mac_monterey-1" -Name "TestSummary.md" -Value "# :tada: All 5 tests passed :tada:" -Force
            New-Item -Path "$testDirectory/TestSummary-mac_13_m1mac_ventura-1" -Name "TestSummary.md" -Value "# :tada: All 5 tests passed :tada:" -Force
            New-Item -Path "$testDirectory/TestSummary-mac_14_x64mac_sonoma-1" -Name "TestSummary.md" -Value "# :tada: All 5 tests passed :tada:" -Force
            New-Item -Path "$testDirectory/TestSummary-mac_15_arm64mac_sequoia-1" -Name "TestSummary.md" -Value "# :tada: All 5 tests passed :tada:" -Force

            $parallelResults = New-ParallelTestsResults -Path "$testDirectory" -StageDependencies "$sortStageDeps" -Context "context" -VSDropsIndex "vsdropsIndex"

            $sb = [System.Text.StringBuilder]::new()
            $parallelResults.WriteComment($sb)

            Remove-Item -Path $testDirectory -Recurse

            $content = $sb.ToString()
            Write-Host $content

            # Verify the order is by version: Monterey(12), Ventura(13), Sonoma(14), Sequoia(15)
            $montereyIdx = $content.IndexOf("Tests on macOS Monterey (12)")
            $venturaIdx = $content.IndexOf("Tests on macOS Ventura (13)")
            $sonomaIdx = $content.IndexOf("Tests on macOS Sonoma (14)")
            $sequoiaIdx = $content.IndexOf("Tests on macOS Sequoia (15)")

            $montereyIdx | Should -BeLessThan $venturaIdx
            $venturaIdx | Should -BeLessThan $sonomaIdx
            $sonomaIdx | Should -BeLessThan $sequoiaIdx
        }
    }

    Context "VSDrops publish failed" {
        It "shows publish failed text instead of VSDrops link" {
            $VerbosePreference = "Continue"
            $DebugPreference = "Continue"

            $vsdropsMatrix = @"
{
    "cecil": {
        "LABEL": "cecil",
        "TESTS_LABELS": "--label=skip-all-tests,run-cecil-tests",
        "TEST_STAGE": "simulator_tests",
        "LABEL_WITH_PLATFORM": "cecil",
        "STATUS_CONTEXT": "VSTS: simulator tests - cecil",
        "TEST_PREFIX": "simulator_testscecil",
        "TEST_PLATFORM": ""
    }
}
"@
            $vsdropsFailedStageDeps = @"
{
  "configure_build": {
    "configure": {
      "outputs": {
        "test_matrix.TEST_MATRIX": "$($vsdropsMatrix.Replace("`n", "\n").Replace("`"", "\`""))"
      }
    }
  },
  "simulator_tests": {
    "tests": {
      "outputs": {
        "cecil.PowerShell15.TESTS_ATTEMPT": "1",
        "cecil.PowerShell15.TESTS_BOT": "XAMMINI-013.Ventura",
        "cecil.PowerShell15.TESTS_LABEL": "cecil",
        "cecil.PowerShell15.TESTS_PLATFORM": "",
        "cecil.PowerShell15.TESTS_TITLE": "cecil",
        "cecil.runTests.TESTS_JOBSTATUS": "Succeeded",
        "cecil.setVSDropsPublishResult.VSDROPS_PUBLISHED": "Failed"
      },
      "identifier": null,
      "name": "tests",
      "attempt": 1,
      "startTime": null,
      "finishTime": null,
      "state": "NotStarted",
      "result": "Succeeded"
    }
  }
}
"@
            $testDirectory = Join-Path "." "subdir"
            New-Item -Path "$testDirectory" -ItemType "directory" -Force
            New-Item -Path "$testDirectory/TestSummary-simulator_testscecil-1" -Name "TestSummary.md" -Value "# :tada: All 1 tests passed :tada:" -Force

            $parallelResults = New-ParallelTestsResults -Path "$testDirectory" -StageDependencies "$vsdropsFailedStageDeps" -Context "context" -VSDropsIndex "vsdropsIndex"

            $parallelResults.IsSuccess() | Should -Be $true

            $sb = [System.Text.StringBuilder]::new()
            $parallelResults.WriteComment($sb)

            Remove-Item -Path $testDirectory -Recurse

            $content = $sb.ToString()

            Write-Host $content

            $content | Should -Not -Match ([regex]::Escape("[Html Report (VSDrops)]"))
            $content | Should -BeLike "*(:warning: Html Report Publish failed :warning:)*"
            $content | Should -BeLike "*[Download]*"
        }
    }
}
