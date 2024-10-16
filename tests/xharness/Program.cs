using System;
using Mono.Options;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;
using Microsoft.DotNet.XHarness.iOS.Shared.XmlResults;
using Microsoft.DotNet.XHarness.Common;

namespace Xharness {
	class MainClass {
		public static int Main (string [] args)
		{
			Microsoft.DotNet.XHarness.iOS.Shared.SdkVersions.OverrideVersions (
				xcode: Xamarin.SdkVersions.Xcode,
				osx: Xamarin.SdkVersions.OSX,
				iOS: Xamarin.SdkVersions.iOS,
				watchOS: "99.99",
				tVOS: Xamarin.SdkVersions.TVOS,
				minOSX: Xamarin.SdkVersions.MinOSX,
				miniOS: Xamarin.SdkVersions.MiniOS,
				minWatchOS: "99.99",
				minTVOS: Xamarin.SdkVersions.MinTVOS,
				miniOSSimulator: Xamarin.SdkVersions.MiniOSSimulator,
				minWatchOSSimulator: "99.99",
				minWatchOSCompanionSimulator: "99.99",
				minTVOSSimulator: Xamarin.SdkVersions.MinTVOSSimulator,
				maxiOSSimulator: Xamarin.SdkVersions.MaxiOSSimulator,
				maxWatchOSSimulator: "99.99",
				maxWatchOSCompanionSimulator: "99.99",
				maxTVOSSimulator: Xamarin.SdkVersions.MaxTVOSSimulator,
				maxiOSDeploymentTarget: Xamarin.SdkVersions.MaxiOSDeploymentTarget,
				maxWatchDeploymentTarget: "99.99",
				maxTVOSDeploymentTarget: Xamarin.SdkVersions.MaxTVOSDeploymentTarget);

			Action showHelp = null;

			var action = HarnessAction.None;
			var configuration = new HarnessConfiguration ();

			var os = new OptionSet () {
				{ "h|?|help", "Displays the help", (v) => showHelp () },
				{ "v|verbose", "Show verbose output", (v) => configuration.Verbosity++ },
				{ "use-system:", "Use the system version of Xamarin.iOS/Xamarin.Mac or the locally build version. Default: the locally build version.", (v) => configuration.UseSystemXamarinIOSMac = v == "1" || v == "true" || string.IsNullOrEmpty (v) },
				{ "autoconf", "Automatically decide what to configure.", (v) => configuration.AutoConf = true },
				{ "rootdir=", "The root directory for the tests.", (v) => HarnessConfiguration.RootDirectory = v },
				{ "project=", "Add a project file to process. This can be specified multiple times.", (v) => configuration.IOSTestProjects.Add (new iOSTestProject (TestLabel.None, v)) },
				// Run
				{ "run=", "Executes a project.", (v) =>
					{
						action = HarnessAction.Run;
						configuration.IOSTestProjects.Add (new iOSTestProject (TestLabel.None, v));
					}
				},
				{ "install=", "Installs a project.", (v) =>
					{
						action = HarnessAction.Install;
						configuration.IOSTestProjects.Add (new iOSTestProject (TestLabel.None, v));
					}
				},
				{ "uninstall=", "Uninstalls a project.", (v) =>
					{
						action = HarnessAction.Uninstall;
						configuration.IOSTestProjects.Add (new iOSTestProject (TestLabel.None, v));
					}
				},
				{ "sdkroot=", "Where Xcode is", (v) => configuration.SdkRoot = v },
				{ "target=", "Where to run the project ([ios|tvos]-[device|simulator).", (v) => configuration.Target = v.ParseAsAppRunnerTarget () },
				{ "configuration=", "Which configuration to run (defaults to Debug).", (v) => configuration.BuildConfiguration = v },
				{ "logdirectory=", "Where to store logs.", (v) => configuration.LogDirectory = v },
				{ "logfile=", "Where to store the log.", (v) => Console.WriteLine("The logfile option is deprecated. Please use logdirectory."), true },
				{ "timeout=", $"Timeout for a test run (in minutes). Default is {configuration.TimeoutInMinutes} minutes.", (v) => configuration.TimeoutInMinutes = double.Parse (v) },
				{ "jenkins:", "Execute test run for jenkins.", (v) =>
					{
						configuration.JenkinsConfiguration = v;
						action = HarnessAction.Jenkins;
					}
				},
				{ "dry-run", "Only print what would be done.", (v) => configuration.DryRun = true },
				{ "setenv:", "Set the specified environment variable when running apps.", (v) =>
					{
						var split = v.Split ('=');
						configuration.EnvironmentVariables [split [0]] = split [1];
					}
				},
				{ "label=", "Comma-separated list of labels to select which tests to run.", (v) =>
					{
						configuration.Labels.UnionWith (v.Split (new char [] { ',' }, StringSplitOptions.RemoveEmptyEntries));
					}
				},
				{ "markdown-summary=", "The path where a summary (in Markdown format) will be written.", (v) => configuration.MarkdownSummaryPath = v },
				{ "periodic-command=", "A command to execute periodically.", (v) => configuration.PeriodicCommand = v },
				{ "periodic-command-arguments=", "Arguments to the command to execute periodically.", (v) => configuration.PeriodicCommandArguments = v },
				{ "periodic-interval=", "An interval (in minutes) between every attempt to execute the periodic command.", (v) => configuration.PeriodicCommandInterval = TimeSpan.FromMinutes (double.Parse (v)) },
				{ "include-system-permission-tests:", "If tests that require system permissions (which could cause the OS to launch dialogs that hangs the test) should be executed or not. Default is to include such tests.", (v) => configuration.IncludeSystemPermissionTests = ParseBool (v, "include-system-permission-tests") },
				{ "xml-jargon:", "The xml format to be used for test results. Values can be nunitv2, nunitv3, xunit.", (v) =>
					{
						if (Enum.TryParse<XmlResultJargon> (v, out var jargon))
							configuration.XmlJargon = jargon;
						else
							configuration.XmlJargon = XmlResultJargon.Missing;
					}
				},

			};

			showHelp = () => {
				os.WriteOptionDescriptions (Console.Out);
				System.Environment.Exit (0);
			};

			var input = os.Parse (args);
			if (input.Count > 0)
				throw new Exception (string.Format ("Unknown arguments: {0}", string.Join (", ", input.ToArray ())));

			if (action == HarnessAction.None)
				showHelp ();

			if (configuration.XmlJargon == XmlResultJargon.Missing) {
				Console.WriteLine ("Unknown xml-jargon value provided. Values can be nunitv2, nunitv3, xunit");
				return 1;
			}

			// XS sets this, which breaks pretty much everything if it doesn't match what was passed to --sdkroot.
			Environment.SetEnvironmentVariable ("XCODE_DEVELOPER_DIR_PATH", null);

			// MSBuild gets confused sometimes, and runs into some sort of deadlock. Disable node re-use to try to mitigate that.
			// Ref: https://github.com/xamarin/maccore/issues/2444
			Environment.SetEnvironmentVariable ("MSBUILDDISABLENODEREUSE", "1");

			var harness = new Harness (new XmlResultParser (), action, configuration);

			return harness.Execute ();
		}

		internal static bool TryParseBool (string value, out bool result)
		{
			if (string.IsNullOrEmpty (value)) {
				result = true;
				return true;
			}

			switch (value.ToLowerInvariant ()) {
			case "1":
			case "yes":
			case "true":
			case "enable":
				result = true;
				return true;
			case "0":
			case "no":
			case "false":
			case "disable":
				result = false;
				return true;
			default:
				return bool.TryParse (value, out result);
			}
		}

		internal static bool ParseBool (string value, string name, bool show_error = true)
		{
			bool result;
			if (!TryParseBool (value, out result))
				throw new Exception ($"Could not parse the command line argument '-{name}:{value}'");
			return result;
		}
	}
}
