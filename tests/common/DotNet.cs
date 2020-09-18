using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Xamarin.Utils;

using NUnit.Framework;

namespace Xamarin.Tests {
	public static class DotNet {
		static string dotnet_executable;
		public static string Executable {
			get {
				if (dotnet_executable == null) {
					dotnet_executable = Configuration.EvaluateVariable ("DOTNET6");
					if (string.IsNullOrEmpty (dotnet_executable))
						throw new Exception ($"Could not find the dotnet executable.");
					if (!File.Exists (dotnet_executable))
						throw new FileNotFoundException ($"The dotnet executable '{dotnet_executable}' does not exist.");
				}
				return dotnet_executable;
			}
		}

		public static ExecutionResult AssertBuild (string project, Dictionary<string, string> properties = null)
		{
			return Execute ("build", project, properties, true);
		}

		public static ExecutionResult AssertBuildFailure (string project, Dictionary<string, string> properties = null)
		{
			var rv = Execute ("build", project, properties, false);
			Assert.AreNotEqual (0, rv.ExitCode, "Unexpected success");
			return rv;
		}

		public static ExecutionResult Execute (string verb, string project, Dictionary<string, string> properties, bool assert_success = true)
		{
			if (!File.Exists (project))
				throw new FileNotFoundException ($"The project file '{project}' does not exist.");
			verb = verb.ToLowerInvariant ();

			switch (verb) {
			case "clean":
			case "build":
				var args = new List<string> ();
				args.Add (verb);
				args.Add (project);
				if (properties != null) {
					foreach (var prop in properties)
						args.Add ($"/p:{prop.Key}={prop.Value}");
				}
				var binlogPath = Path.Combine (Path.GetDirectoryName (project), $"log-{verb}-{DateTime.Now:yyyyMMdd_HHmmss}.binlog");
				args.Add ($"/bl:{binlogPath}");
				var env = new Dictionary<string, string> ();
				env ["MSBuildSDKsPath"] = null;
				env ["MSBUILD_EXE_PATH"] = null;
				// This is a temporary variable to enable the .NET workload resolver, because it's opt-in for now.
				// Ref: https://github.com/dotnet/sdk/issues/13849
				env ["MSBuildEnableWorkloadResolver"] = "true";
				var output = new StringBuilder ();
				var rv = Execution.RunWithStringBuildersAsync (Executable, args, env, output, output, Console.Out, workingDirectory: Path.GetDirectoryName (project), timeout: TimeSpan.FromMinutes (10)).Result;
				if (assert_success && rv.ExitCode != 0) {
					Console.WriteLine ($"'{Executable} {StringUtils.FormatArguments (args)}' failed with exit code {rv}.");
					Console.WriteLine (output);
					Assert.AreEqual (0, rv.ExitCode, $"Exit code: {Executable} {StringUtils.FormatArguments (args)}");
				}
				return new ExecutionResult {
					StandardOutput = output,
					StandardError = output,
					ExitCode = rv.ExitCode,
					BinLogPath = binlogPath,
				};
			default:
				throw new NotImplementedException ($"Unknown dotnet action: '{verb}'");
			}
		}

		public static void CompareApps (string old_app, string new_app)
		{
			Console.WriteLine ($"Comparing:");
			Console.WriteLine ($"    {old_app}");
			Console.WriteLine ($"    {new_app}");

			var all_old_files = Directory.GetFiles (old_app, "*.*", SearchOption.AllDirectories).Select ((v) => v.Substring (old_app.Length + 1));
			var all_new_files = Directory.GetFiles (new_app, "*.*", SearchOption.AllDirectories).Select ((v) => v.Substring (new_app.Length + 1));

			var filter = new Func<IEnumerable<string>, IEnumerable<string>> ((lst) => {
				return lst.Where (v => {
					var extension = Path.GetExtension (v);

					switch (extension) {
					case ".exe":
					case ".dll":
					case ".pdb": // the set of BCL assemblies is quite different
						return false;
					case ".dylib": // ignore dylibs, they're not the same
						return false;

					// There's a lot of TODOs here, those correspond with missing features in .NET and will be removed as those features are implemented
					}

					var filename = Path.GetFileName (v);
					switch (filename) {
					case "MonoTouchDebugConfiguration.txt": // TODO
					case "PkgInfo": // TODO
					case "runtime-options.plist": // TODO
					case "Root.plist": // TODO
						return false;
					}

					var components = v.Split ('/');
					if (components.Any (v => v.EndsWith (".framework", StringComparison.Ordinal))) {
						return false; // TODO
					}

					return true;
				});
			});

			Console.WriteLine ("Files in old app:");
			foreach (var f in all_old_files.OrderBy (v => v))
				Console.WriteLine ($"\t{f}");
			Console.WriteLine ("Files in new app:");
			foreach (var f in all_new_files.OrderBy (v => v))
				Console.WriteLine ($"\t{f}");

			var old_files = filter (all_old_files);
			var new_files = filter (all_new_files);

			Console.WriteLine ("Files in old app (filtered):");
			foreach (var f in old_files.OrderBy (v => v))
				Console.WriteLine ($"\t{f}");
			Console.WriteLine ("Files in new app (filtered):");
			foreach (var f in new_files.OrderBy (v => v))
				Console.WriteLine ($"\t{f}");

			var extra_old_files = old_files.Except (new_files);
			var extra_new_files = new_files.Except (old_files);

			if (extra_new_files.Any ()) {
				Console.WriteLine ("Extra dotnet files:");
				foreach (var f in extra_new_files)
					Console.WriteLine ($"    {f}");
			}
			if (extra_old_files.Any ()) {
				Console.WriteLine ("Missing dotnet files:");
				foreach (var f in extra_old_files)
					Console.WriteLine ($"    {f}");
			}

			// Print out a size comparison. A size difference does not fail the test, because some size differences are normal.
			Console.WriteLine ("Size comparison:");
			foreach (var file in new_files) {
				var new_size = new FileInfo (Path.Combine (new_app, file)).Length;
				var old_size = new FileInfo (Path.Combine (old_app, file)).Length;
				if (new_size == old_size)
					continue;
				var diff = new_size - old_size;
				Console.WriteLine ($"\t{file}: {old_size} bytes -> {new_size} bytes. Diff: {diff}");
			}
			var total_old = all_old_files.Select (v => new FileInfo (Path.Combine (old_app, v)).Length).Sum ();
			var total_new = all_new_files.Select (v => new FileInfo (Path.Combine (new_app, v)).Length).Sum ();
			var total_diff = total_new - total_old;
			Console.WriteLine ();
			Console.WriteLine ($"\tOld app size: {total_old} bytes = {total_old / 1024.0:0.0} KB = {total_old / (1024.0 * 1024.0):0.0} MB");
			Console.WriteLine ($"\tNew app size: {total_new} bytes = {total_new / 1024.0:0.0} KB = {total_new / (1024.0 * 1024.0):0.0} MB");
			Console.WriteLine ($"\tSize comparison complete, total size change: {total_diff} bytes = {total_diff / 1024.0:0.0} KB = {total_diff / (1024.0 * 1024.0):0.0} MB");

			Assert.That (extra_new_files, Is.Empty, "Extra dotnet files");
			Assert.That (extra_old_files, Is.Empty, "Missing dotnet files");
		}
	}

	public class ExecutionResult {
		public StringBuilder StandardOutput;
		public StringBuilder StandardError;
		public int ExitCode;
		public bool TimedOut;
		public string BinLogPath;
	}
}
