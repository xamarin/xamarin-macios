using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;

using NUnit.Framework;

using Xamarin;
using Xamarin.Tests;
using Xamarin.Utils;

public static class ProcessHelper
{
	static int counter;

	static string log_directory;
	static string LogDirectory {
		get {
			if (log_directory == null)
				log_directory = Cache.CreateTemporaryDirectory ("execution-logs");
			return log_directory;
		}

	}

	public static void AssertRunProcess (string filename, string[] arguments, TimeSpan timeout, string workingDirectory, Dictionary<string, string> environment_variables, string message)
	{
		var exitCode = 0;
		var output = new List<string> ();

		Action<string> output_callback = (v) => {
			lock (output)
				output.Add (v);
		};

		if (environment_variables == null)
			environment_variables = new Dictionary<string, string> ();
		environment_variables ["XCODE_DEVELOPER_DIR_PATH"] = null;
		environment_variables ["DEVELOPER_DIR"] = Configuration.XcodeLocation;

		exitCode = ExecutionHelper.Execute (filename, arguments, out var timed_out, workingDirectory, environment_variables, output_callback, output_callback, timeout);

		// Write execution log to disk (and print the path)
		var logfile = Path.Combine (LogDirectory, $"{filename}-{Interlocked.Increment (ref counter)}.log");
		File.WriteAllLines (logfile, output);
		TestContext.AddTestAttachment (logfile, $"Execution log for {filename}");
		Console.WriteLine ("Execution log for {0}: {1}", filename, logfile);

		var errors = new List<string> ();
		var errorMessage = "";
		if ((!timed_out || exitCode != 0) && output.Count > 0) {
			var regex = new Regex (@"error\s*(MSB....)?(CS....)?(MT....)?(MM....)?:", RegexOptions.IgnoreCase | RegexOptions.Singleline);
			foreach (var line in output) {
				if (regex.IsMatch (line) && !errors.Contains (line))
					errors.Add (line);
			}
			if (errors.Count > 0)
				errorMessage = "\n\t[Summary of errors from the build output below]\n\t" + string.Join ("\n\t", errors);
		}
		Assert.IsFalse (timed_out, $"{message} timed out after {timeout.TotalMinutes} minutes{errorMessage}");
		Assert.AreEqual (0, exitCode, $"{message} failed (unexpected exit code){errorMessage}");
	}

	public static void BuildSolution (string solution, string platform, string configuration, Dictionary<string, string> environment_variables, string target = "")
	{
		// nuget restore
		var solution_dir = string.Empty;
		var solutions = new string [] { solution };
		var nuget_args = new List<string> ();
		nuget_args.Add ("restore");
		nuget_args.Add ("sln"); // replaced later
		nuget_args.Add ("-Verbosity");
		nuget_args.Add ("detailed");
		if (!solution.EndsWith (".sln", StringComparison.Ordinal)) {
			var slndir = Path.GetDirectoryName (solution);
			while ((solutions = Directory.GetFiles (slndir, "*.sln", SearchOption.TopDirectoryOnly)).Length == 0 && slndir.Length > 1)
				slndir = Path.GetDirectoryName (slndir);
			nuget_args.Add ("-SolutionDir");
			nuget_args.Add (slndir);
		}

		foreach (var sln in solutions) {
			nuget_args [1] = sln; // replacing here
			AssertRunProcess ("nuget", nuget_args.ToArray (), TimeSpan.FromMinutes (2), Configuration.SampleRootDirectory, environment_variables, "nuget restore");
		}

		// msbuild
		var sb = new List<string> ();
		sb.Add ("/verbosity:diag");
		if (!string.IsNullOrEmpty (platform))
			sb.Add ($"/p:Platform={platform}");
		if (!string.IsNullOrEmpty (configuration))
			sb.Add ($"/p:Configuration={configuration}");
		sb.Add (solution);
		if (!string.IsNullOrEmpty (target))
			sb.Add ($"/t:{target}");
		AssertRunProcess ("msbuild", sb.ToArray (), TimeSpan.FromMinutes (5), Configuration.SampleRootDirectory, environment_variables, "build");
	}
}
