using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

using NUnit.Framework;

using Xamarin;
using Xamarin.Tests;
using Xamarin.Utils;

public static class ProcessHelper {
	static int counter;

	static string log_directory;
	static string LogDirectory {
		get {
			if (log_directory is null)
				log_directory = Cache.CreateTemporaryDirectory ("execution-logs");
			return log_directory;
		}

	}

	public static void AssertRunProcess (string filename, string [] arguments, TimeSpan timeout, string workingDirectory, Dictionary<string, string> environment_variables, string message)
	{
		AssertRunProcess (filename, arguments, timeout, workingDirectory, environment_variables, message, out _);
	}

	public static void AssertRunProcess (string filename, string [] arguments, TimeSpan timeout, string workingDirectory, Dictionary<string, string> environment_variables, string message, out string logfile)
	{
		var exitCode = 0;
		var output = new List<string> ();

		Action<string> output_callback = (v) => {
			lock (output)
				output.Add ($"{DateTime.Now.ToString ("HH:mm:ss.fffffff")}: {v}");
		};

		if (environment_variables is null)
			environment_variables = new Dictionary<string, string> ();
		environment_variables ["XCODE_DEVELOPER_DIR_PATH"] = null;
		environment_variables ["DEVELOPER_DIR"] = Configuration.XcodeLocation;

		var watch = Stopwatch.StartNew ();
		exitCode = ExecutionHelper.Execute (filename, arguments, out var timed_out, workingDirectory, environment_variables, output_callback, output_callback, timeout);
		watch.Stop ();

		output_callback ($"Exit code: {exitCode} Timed out: {timed_out} Total duration: {watch.Elapsed.ToString ()}");

		// Write execution log to disk (and print the path)
		logfile = Path.Combine (LogDirectory, $"{filename}-{Interlocked.Increment (ref counter)}.log");
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

	public static void BuildSolution (string solution, string platform, string configuration, Dictionary<string, string> environment_variables, TimeSpan timeout, string target = "", string codesignKey = null)
	{
		// nuget restore
		var solution_dir = string.Empty;
		var solutions = new string [] { solution };
		var nuget_args = new List<string> ();
		nuget_args.Add ("restore");
		nuget_args.Add ("sln"); // replaced later
		nuget_args.Add ("-Verbosity");
		nuget_args.Add ("detailed");
		var slndir = Path.GetDirectoryName (solution);
		if (!solution.EndsWith (".sln", StringComparison.Ordinal)) {
			while ((solutions = Directory.GetFiles (slndir, "*.sln", SearchOption.TopDirectoryOnly)).Length == 0 && slndir.Length > 1)
				slndir = Path.GetDirectoryName (slndir);
			nuget_args.Add ("-SolutionDir");
			nuget_args.Add (slndir);
		}

		foreach (var sln in solutions) {
			nuget_args [1] = sln; // replacing here
			AssertRunProcess ("nuget", nuget_args.ToArray (), timeout, Configuration.SampleRootDirectory, environment_variables, "nuget restore");
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
		if (!string.IsNullOrEmpty (codesignKey))
			sb.Add ($"/p:CodesignKey={codesignKey}");

		environment_variables ["MTOUCH_ENV_OPTIONS"] = "--time --time --time --time -vvvv";
		environment_variables ["MMP_ENV_OPTIONS"] = "--time --time --time --time -vvvv";

		var watch = Stopwatch.StartNew ();
		var failed = false;
		string msbuild_logfile;
		try {
			AssertRunProcess ("msbuild", sb.ToArray (), timeout, Configuration.SampleRootDirectory, environment_variables, "build", out msbuild_logfile);
		} catch {
			failed = true;
			throw;
		} finally {
			watch.Stop ();
		}

		// Write performance data to disk
		var subdirs = Directory.GetDirectories (slndir, "*", SearchOption.AllDirectories);

		// First figure out which .app subdirectory is the actual .app. This is a bit more complicated than it would seem...
		var apps = subdirs.Where ((v) => {
			var names = v.Substring (slndir.Length).Split (Path.DirectorySeparatorChar);
			if (names.Length < 2)
				return false;

			if (!names [names.Length - 1].EndsWith (".app", StringComparison.Ordinal))
				return false;

			if (names.Any ((v2) => v2 == "copySceneKitAssets"))
				return false;

			var bin_idx = Array.IndexOf (names, "bin");
			var conf_idx = Array.IndexOf (names, configuration);
			if (bin_idx < 0 || conf_idx < 0)
				return false;

			if (bin_idx > conf_idx)
				return false;

			if (platform.Length > 0) {
				var platform_idx = Array.IndexOf (names, platform);
				if (platform_idx < 0)
					return false;

				if (bin_idx > platform_idx)
					return false;
			}

			return true;
		}).ToArray ();

		if (apps.Length > 1) {
			// Found more than one .app subdirectory, use additional logic to choose between them.
			var filtered_apps = apps.Where ((v) => {
				// If one .app is a subdirectory of another .app, we don't care about the former.
				if (apps.Any ((v2) => v2.Length < v.Length && v.StartsWith (v2, StringComparison.Ordinal)))
					return false;

				// If one .app is contained within another .app, we don't care about the former.
				var vname = Path.GetFileName (v);
				var otherApps = apps.Where ((v2) => v != v2);
				if (otherApps.Any ((v2) => {
					var otherSubdirs = subdirs.Where ((v3) => v3.StartsWith (v2, StringComparison.Ordinal));
					return otherSubdirs.Any ((v3) => Path.GetFileName (v3) == vname);
				}))
					return false;

				return true;
			}).ToArray ();
			if (apps.Length == 0)
				Assert.Fail ($"Filtered away all the .apps, from:\n\t{string.Join ("\n\t", apps)}");
			apps = filtered_apps;
		}

		if (apps.Length > 1) {
			Assert.Fail ($"Found more than one .app directory:\n\t{string.Join ("\n\t", apps)}");
		} else if (apps.Length == 0) {
			Assert.Fail ($"Found no .app directories for  platform: {platform} configuration: {configuration} target: {target}. All directories:\n\t{string.Join ("\n\t", subdirs)}");
		}

		var logfile = Path.Combine (LogDirectory, $"{Path.GetFileNameWithoutExtension (solution)}-perfdata-{Interlocked.Increment (ref counter)}.xml");

		var xmlSettings = new XmlWriterSettings {
			Indent = true,
		};
		var xml = XmlWriter.Create (logfile, xmlSettings);
		xml.WriteStartDocument (true);
		xml.WriteStartElement ("performance");
		xml.WriteStartElement ("sample-build");
		xml.WriteAttributeString ("mono-version", Configuration.MonoVersion);
		xml.WriteAttributeString ("os-version", Configuration.OSVersion);
		xml.WriteAttributeString ("xamarin-macios-hash", Configuration.TestedHash);
		xml.WriteAttributeString ("sample-repository", Configuration.GetCurrentRemoteUrl (slndir));
		xml.WriteAttributeString ("sample-hash", Configuration.GetCurrentHash (slndir));
		xml.WriteAttributeString ("agent-machinename", Environment.GetEnvironmentVariable ("AGENT_MACHINENAME"));
		xml.WriteAttributeString ("agent-name", Environment.GetEnvironmentVariable ("AGENT_NAME"));
		foreach (var app in apps) {
			xml.WriteStartElement ("test");
			xml.WriteAttributeString ("name", TestContext.CurrentContext.Test.FullName);
			xml.WriteAttributeString ("result", failed ? "failed" : "success");
			if (platform.Length > 0)
				xml.WriteAttributeString ("platform", platform);
			xml.WriteAttributeString ("configuration", configuration);
			if (!failed) {
				xml.WriteAttributeString ("duration", watch.ElapsedTicks.ToString ());
				xml.WriteAttributeString ("duration-formatted", watch.Elapsed.ToString ());

				var files = Directory.GetFiles (app, "*", SearchOption.AllDirectories).OrderBy ((v) => v).ToArray ();
				var lengths = files.Select ((v) => new FileInfo (v).Length).ToArray ();
				var total_size = lengths.Sum ();

				xml.WriteAttributeString ("total-size", total_size.ToString ());
				var appstart = Path.GetDirectoryName (app).Length;
				for (var i = 0; i < files.Length; i++) {
					xml.WriteStartElement ("file");
					xml.WriteAttributeString ("name", files [i].Substring (appstart + 1));
					xml.WriteAttributeString ("size", lengths [i].ToString ());
					xml.WriteEndElement ();
				}

				if (File.Exists (msbuild_logfile)) {
					var lines = File.ReadAllLines (msbuild_logfile);
					var target_perf_summary = new List<string> ();
					var task_perf_summary = new List<string> ();
					var timestamps = new List<string> ();
					for (var i = lines.Length - 1; i >= 0; i--) {
						if (lines [i].EndsWith ("Target Performance Summary:", StringComparison.Ordinal)) {
							for (var k = i + 1; k < lines.Length && lines [k].EndsWith ("calls", StringComparison.Ordinal); k++) {
								target_perf_summary.Add (lines [k].Substring (18).Trim ());
							}
						} else if (lines [i].EndsWith ("Task Performance Summary:", StringComparison.Ordinal)) {
							for (var k = i + 1; k < lines.Length && lines [k].EndsWith ("calls", StringComparison.Ordinal); k++) {
								task_perf_summary.Add (lines [k].Substring (18).Trim ());
							}
						} else if (lines [i].Contains ("!Timestamp")) {
							timestamps.Add (lines [i]);
						}
					}
					foreach (var tps in target_perf_summary) {
						var split = tps.Split (new char [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
						xml.WriteStartElement ("target");
						xml.WriteAttributeString ("name", split [2]);
						xml.WriteAttributeString ("ms", split [0]);
						xml.WriteAttributeString ("calls", split [3]);
						xml.WriteEndElement ();
					}
					foreach (var tps in task_perf_summary) {
						var split = tps.Split (new char [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
						xml.WriteStartElement ("task");
						xml.WriteAttributeString ("name", split [2]);
						xml.WriteAttributeString ("ms", split [0]);
						xml.WriteAttributeString ("calls", split [3]);
						xml.WriteEndElement ();
					}
					foreach (var ts in timestamps) {
						// Sample line:
						// 15:04:50.4609520:   !Timestamp Setup: 28 ms (TaskId:137)
						var splitFirst = ts.Split (new char [] { ':' }, StringSplitOptions.RemoveEmptyEntries);
						var splitSecondA = splitFirst [3].Split (new char [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
						var splitSecondB = splitFirst [4].Split (new char [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
						var name = string.Join (" ", splitSecondA.Skip (1));
						var level = splitSecondA [0].Count ((v) => v == '!').ToString ();
						var ms = splitSecondB [0];
						xml.WriteStartElement ("timestamp");
						xml.WriteAttributeString ("name", name);
						xml.WriteAttributeString ("level", level);
						xml.WriteAttributeString ("ms", ms);
						xml.WriteEndElement ();
					}
				}

				xml.WriteEndElement ();
			}

			xml.WriteEndElement (); // sample-build
			xml.WriteEndElement (); // performance
			xml.WriteEndDocument ();
			xml.Dispose ();

			TestContext.AddTestAttachment (logfile, $"Performance data");
		}
	}

	internal static string RunProcess (string filename, string arguments = "", string working_directory = null)
	{
		using (var p = Process.Start (filename, arguments)) {
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.UseShellExecute = false;
			if (!string.IsNullOrEmpty (working_directory))
				p.StartInfo.WorkingDirectory = working_directory;
			p.Start ();
			var output = p.StandardOutput.ReadToEnd ();
			p.WaitForExit ();
			return output;
		}
	}
}
