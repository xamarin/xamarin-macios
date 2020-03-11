using System;
using System.Collections.Generic;
using Mono.Options;
using Xharness.Utilities;

namespace Xharness {
	class MainClass {
		public static int Main (string [] args)
		{
			Action showHelp = null;

			var action = HarnessAction.None;
			var autoConf = false;
			var configuration = "Debug";
			var dryRun = false;
			var environmentVariables = new Dictionary<string, string> ();
			bool? includeSystemPermissionTests = null;
			var iOSTestProjects = new List<iOSTestProject> ();
			string jenkinsConfiguration = null;
			var labels = new HashSet<string> ();
			var logDirectory = Environment.CurrentDirectory;
			string logFile = null;
			var mac = false;
			string markdownSummaryPath = null;
			string periodicCommand = null;
			string periodicCommandArguments = null;
			TimeSpan periodicCommandInterval = TimeSpan.Zero;
			string rootDirectory = null;
			string sdkRoot = null;
			var target = AppRunnerTarget.None;
			var timeout = 15d;
			var useSystem = false;
			var verbosity = 0;
			string watchOSAppTemplate = null;
			string watchOSContainerTemplate = null;
			XmlResultJargon xmlJargon = XmlResultJargon.NUnitV3;

			var os = new OptionSet () {
				{ "h|?|help", "Displays the help", (v) => showHelp () },
				{ "v|verbose", "Show verbose output", (v) => verbosity++ },
				{ "use-system:", "Use the system version of Xamarin.iOS/Xamarin.Mac or the locally build version. Default: the locally build version.", (v) => useSystem = v == "1" || v == "true" || string.IsNullOrEmpty (v) },
				// Configure
				{ "mac", "Configure for Xamarin.Mac instead of iOS.", (v) => mac = true },
				{ "configure", "Creates project files and makefiles.", (v) => action = HarnessAction.Configure },
				{ "autoconf", "Automatically decide what to configure.", (v) => autoConf = true },
				{ "rootdir=", "The root directory for the tests.", (v) => rootDirectory = v },
				{ "project=", "Add a project file to process. This can be specified multiple times.", (v) => iOSTestProjects.Add (new iOSTestProject (v)) },
				{ "watchos-container-template=", "The directory to use as a template for a watchos container app.", (v) => watchOSContainerTemplate = v },
				{ "watchos-app-template=", "The directory to use as a template for a watchos app.", (v) => watchOSAppTemplate = v },
				// Run
				{ "run=", "Executes a project.", (v) =>
					{
						action = HarnessAction.Run;
						iOSTestProjects.Add (new iOSTestProject (v));
					}
				},
				{ "install=", "Installs a project.", (v) =>
					{
						action = HarnessAction.Install;
						iOSTestProjects.Add (new iOSTestProject (v));
					}
				},
				{ "uninstall=", "Uninstalls a project.", (v) =>
					{
						action = HarnessAction.Uninstall;
						iOSTestProjects.Add (new iOSTestProject (v));
					}
				},
				{ "sdkroot=", "Where Xcode is", (v) => sdkRoot = v },
				{ "sdkroot94=", "Where Xcode 9.4 is", (v) => Console.WriteLine ("--sdkroot94 is deprecated"), true },
				{ "target=", "Where to run the project ([ios|watchos|tvos]-[device|simulator|simulator-32|simulator-64]).", (v) => target = v.ParseAsAppRunnerTarget () },
				{ "configuration=", "Which configuration to run (defaults to Debug).", (v) => configuration = v },
				{ "logdirectory=", "Where to store logs.", (v) => logDirectory = v },
				{ "logfile=", "Where to store the log.", (v) => logFile = v },
				{ "timeout=", $"Timeout for a test run (in minutes). Default is {timeout} minutes.", (v) => timeout = double.Parse (v) },
				{ "jenkins:", "Execute test run for jenkins.", (v) =>
					{
						jenkinsConfiguration = v;
						action = HarnessAction.Jenkins;
					}
				},
				{ "dry-run", "Only print what would be done.", (v) => dryRun = true },
				{ "setenv:", "Set the specified environment variable when running apps.", (v) =>
					{
						var split = v.Split ('=');
						environmentVariables [split [0]] = split [1];
					}
				},
				{ "label=", "Comma-separated list of labels to select which tests to run.", (v) =>
					{
						labels.UnionWith (v.Split (new char [] { ',' }, StringSplitOptions.RemoveEmptyEntries));
					}
				},
				{ "markdown-summary=", "The path where a summary (in Markdown format) will be written.", (v) => markdownSummaryPath = v },
				{ "periodic-command=", "A command to execute periodically.", (v) => periodicCommand = v },
				{ "periodic-command-arguments=", "Arguments to the command to execute periodically.", (v) => periodicCommandArguments = v },
				{ "periodic-interval=", "An interval (in minutes) between every attempt to execute the periodic command.", (v) => periodicCommandInterval = TimeSpan.FromMinutes (double.Parse (v)) },
				{ "include-system-permission-tests:", "If tests that require system permissions (which could cause the OS to launch dialogs that hangs the test) should be executed or not. Default is to include such tests.", (v) => includeSystemPermissionTests = ParseBool (v, "include-system-permission-tests") },
				{ "xml-jargon:", "The xml format to be used for test results. Values can be nunitv2, nunitv3, xunit.", (v) =>
					{
						if (Enum.TryParse<XmlResultJargon> (v, out var jargon))
							xmlJargon = jargon;
						else
							xmlJargon = XmlResultJargon.Missing;
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

			if (xmlJargon == XmlResultJargon.Missing) {
				Console.WriteLine ("Unknown xml-jargon value provided. Values can be nunitv2, nunitv3, xunit");
				return 1;
			}

			// XS sets this, which breaks pretty much everything if it doesn't match what was passed to --sdkroot.
			Environment.SetEnvironmentVariable ("XCODE_DEVELOPER_DIR_PATH", null);

			var harness = new Harness (action,
				autoConf,
				configuration,
				dryRun,
				environmentVariables,
				includeSystemPermissionTests,
				iOSTestProjects,
				jenkinsConfiguration,
				labels,
				logDirectory,
				logFile,
				mac,
				markdownSummaryPath,
				periodicCommand,
				periodicCommandArguments,
				periodicCommandInterval,
				rootDirectory,
				sdkRoot,
				target,
				timeout,
				useSystem,
				verbosity,
				watchOSAppTemplate,
				watchOSContainerTemplate,
				xmlJargon);

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
