using System;

using Mono.Options;

namespace xharness
{
	class MainClass
	{
		public static int Main (string[] args)
		{
			var harness = new Harness ();

			Action showHelp = null;
			var os = new OptionSet () {
				{ "h|?|help", "Displays the help", (v) => showHelp () },
				{ "v|verbose", "Show verbose output", (v) => harness.Verbosity++ },
				{ "use-system:", "Use the system version of Xamarin.iOS/Xamarin.Mac or the locally build version. Default: the locally build version.", (v) => harness.UseSystem = v == "1" || v == "true" || string.IsNullOrEmpty (v) },
				// Configure
				{ "mac", "Configure for Xamarin.Mac instead of iOS.", (v) => harness.Mac = true },
				{ "configure", "Creates project files and makefiles.", (v) => harness.Action = HarnessAction.Configure },
				{ "autoconf", "Automatically decide what to configure.", (v) => harness.AutoConf = true },
				{ "rootdir=", "The root directory for the tests.", (v) => harness.RootDirectory = v },
				{ "project=", "Add a project file to process. This can be specified multiple times.", (v) => harness.IOSTestProjects.Add (new iOSTestProject (v)) },
				{ "watchos-container-template=", "The directory to use as a template for a watchos container app.", (v) => harness.WatchOSContainerTemplate = v },
				{ "watchos-app-template=", "The directory to use as a template for a watchos app.", (v) => harness.WatchOSAppTemplate = v },
				// Run
				{ "run=", "Executes a project.", (v) =>
					{
						harness.Action = HarnessAction.Run;
						harness.IOSTestProjects.Add (new iOSTestProject (v));
					}
				},
				{ "install=", "Installs a project.", (v) =>
					{
						harness.Action = HarnessAction.Install;
						harness.IOSTestProjects.Add (new iOSTestProject (v));
					}
				},
				{ "uninstall=", "Uninstalls a project.", (v) =>
					{
						harness.Action = HarnessAction.Uninstall;
						harness.IOSTestProjects.Add (new iOSTestProject (v));
					}
				},
				{ "sdkroot=", "Where Xcode is", (v) => harness.SdkRoot = v },
				{ "target=", "Where to run the project ([ios|watchos|tvos]-[device|simulator|simulator-32|simulator-64]).", (v) => harness.Target = v.ParseAsAppRunnerTarget () },
				{ "configuration=", "Which configuration to run (defaults to Debug).", (v) => harness.Configuration = v },
				{ "logdirectory=", "Where to store logs.", (v) => harness.LogDirectory = v },
				{ "logfile=", "Where to store the log.", (v) => harness.LogFile = v },
				{ "timeout=", "Timeout for a test run (in minutes). Default is 10 minutes.", (v) => harness.Timeout = double.Parse (v) },
				{ "jenkins:", "Execute test run for jenkins.", (v) =>
					{
						harness.JenkinsConfiguration = v;
						harness.Action = HarnessAction.Jenkins;
					}
				},
				{ "dry-run", "Only print what would be done.", (v) => harness.DryRun = true },
				{ "setenv:", "Set the specified environment variable when running apps.", (v) =>
					{
						var split = v.Split ('=');
						harness.EnvironmentVariables [split [0]] = split [1];
					}
				},
				{ "label=", "Comma-separated list of labels to select which tests to run.", (v) =>
					{
						harness.Labels.UnionWith (v.Split (new char [] { ',' }, StringSplitOptions.RemoveEmptyEntries));
					}
				},
				{ "markdown-summary=", "The path where a summary (in Markdown format) will be written.", (v) => harness.MarkdownSummaryPath = v },
				{ "periodic-command=", "A command to execute periodically.", (v) => harness.PeriodicCommand = v },
				{ "periodic-command-arguments=", "Arguments to the command to execute periodically.", (v) => harness.PeriodicCommandArguments = v },
				{ "periodic-interval=", "An interval (in minutes) between every attempt to execute the periodic command.", (v) => harness.PeriodicCommandInterval = TimeSpan.FromMinutes (double.Parse (v)) },
				{ "include-system-permission-tests:", "If tests that require system permissions (which could cause the OS to launch dialogs that hangs the test) should be executed or not. Default is to include such tests.", (v) => harness.IncludeSystemPermissionTests = ParseBool (v, "include-system-permission-tests") },
			};

			showHelp = () => {
				os.WriteOptionDescriptions (Console.Out);
				System.Environment.Exit (0);
			};

			var input = os.Parse (args);
			if (input.Count > 0)
				throw new Exception (string.Format ("Unknown arguments: {0}", string.Join (", ", input.ToArray ())));
			if (harness.Action == HarnessAction.None)
				showHelp ();

			// XS sets this, which breaks pretty much everything if it doesn't match what was passed to --sdkroot.
			Environment.SetEnvironmentVariable ("XCODE_DEVELOPER_DIR_PATH", null);

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
