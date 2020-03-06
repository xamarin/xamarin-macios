using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Mono.Options;

namespace DummyTestProcess {

	// The following is a dummy console application that we are going to be using
	// for testing purposes. The application takes a number of commands that will
	// help to test diff situations:
	//
	// * Exit codes.
	// * Timeouts
	// * Child processes.
	// * Provided parameters
	//
	// The console worjs as following:
	//
	// -exit-code: Provide the exit code that will be returned after execution
	// -timeout: The time the application will be idle. This will be used to
	//			 fake a timeout in the xharness ide.
	// -children: Create a number of childre processes.
	//
	// By default, the application will alsways print the arguments that have been
	// provided and will run for the given timeout time.
	class MainClass {

		public static void Main (string [] args)
		{
			var timeout = TimeSpan.FromMinutes(1);
			var childrenCount = 0;
			var exitCode = 0;
			var showHelp = false;
			string stdout = "";
			string stderr = "";
			List<Process> children;

			var os = new OptionSet () {
				{ "h|?|help", "Displays the help", (v) => showHelp = true },
				{ "exit-code=", "Exit code to be used by the application", (v) => int.TryParse (v, out exitCode) },
				{ "stdout=", "Message to print in stdout. Default is null",  (v) => stdout = v },
				{ "stderr=", "Message to print in stderr. Default is null", (v) => stderr = v },
				{ "timeout=", "Timeout for a test run (in seconds). Default is 1 minute.", (v) =>
					{
						if (int.TryParse (v, out var newTimeout)) {
							timeout = TimeSpan.FromSeconds(newTimeout);
						}
					}
				},
				{ "children=", "The number of children processes that will be created by the application", (v) => int.TryParse (v, out childrenCount) },
			};

			_ = os.Parse (args); // dont care about any extra args, ignore them


			if (showHelp) {
				os.WriteOptionDescriptions (Console.Out);
				Environment.Exit (0);
			}

			// spawn the number of children needed, we spawn the same process with the same args BUT no children
			if (childrenCount > 0) {
				Console.WriteLine (childrenCount);
				children = new List<Process> (childrenCount);
				for (var i = 0; i < childrenCount; i++) {
					Console.WriteLine (i);
					var p = new Process ();
					p.StartInfo.FileName = "mono";
					p.StartInfo.Arguments = $"DummyTestProcess.exe -exit-code={exitCode} -timeout={timeout.Seconds}";
					p.Start ();
					children.Add (p);
				}
			}
			// write something to the stdout and stderr to test the output
			if (!string.IsNullOrEmpty (stdout))
				Console.WriteLine (stdout);
			if (!string.IsNullOrEmpty (stderr))
				Console.Error.WriteLine (stderr);
			// sleep for the required timeout
			Thread.Sleep (timeout);
			Environment.Exit (exitCode);

		}
	}
}
