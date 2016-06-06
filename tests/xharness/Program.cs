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
				// Configure
				{ "mac", "Configure for Xamarin.Mac instead of iOS.", (v) => harness.Mac = true },
				{ "configure", "Creates project files and makefiles.", (v) => harness.Action = HarnessAction.Configure },
				{ "autoconf", "Automatically decide what to configure.", (v) => harness.AutoConf = true },
				{ "rootdir=", "The root directory for the tests.", (v) => harness.RootDirectory = v },
				{ "project=", "Add a project file to process. This can be specified multiple times.", (v) => harness.TestProjects.Add (new TestProject (v)) },
				{ "watchos-container-template=", "The directory to use as a template for a watchos container app.", (v) => harness.WatchOSContainerTemplate = v },
				{ "watchos-app-template=", "The directory to use as a template for a watchos app.", (v) => harness.WatchOSAppTemplate = v },
				// Run
				{ "run=", "Executes a project.", (v) =>
					{
						harness.Action = HarnessAction.Run;
						harness.TestProjects.Add (new TestProject (v));
					}
				},
				{ "install=", "Installs a project.", (v) =>
					{
						harness.Action = HarnessAction.Install;
						harness.TestProjects.Add (new TestProject (v));
					}
				},
				{ "sdkroot=", "Where Xcode is", (v) => harness.SdkRoot = v },
				{ "target=", "Where to run the project ([ios|watchos|tvos]-[device|simulator|simulator-32|simulator-64]).", (v) => harness.Target = v },
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
			return harness.Execute ();
		}
	}
}
