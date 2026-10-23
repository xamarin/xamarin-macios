//#define VERBOSE_COMPARISON

using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using Xamarin.Utils;

#nullable enable

namespace Xamarin.Tests {
	public static class DotNet {
		static string? dotnet_executable;
		public static string Executable {
			get {
				if (dotnet_executable is null) {
					dotnet_executable = Environment.GetEnvironmentVariable ("DOTNET") ?? Configuration.GetVariable ("DOTNET", null);
					if (string.IsNullOrEmpty (dotnet_executable))
						throw new Exception ($"Could not find the dotnet executable.");
					if (!File.Exists (dotnet_executable))
						throw new FileNotFoundException ($"The dotnet executable '{dotnet_executable}' does not exist.");
				}
				return dotnet_executable;
			}
		}

		public static ExecutionResult AssertPack (string project, Dictionary<string, string>? properties = null, bool? msbuildParallelism = null)
		{
			return Execute ("pack", project, properties, true, msbuildParallelism: msbuildParallelism);
		}

		public static ExecutionResult AssertPackFailure (string project, Dictionary<string, string>? properties = null, bool? msbuildParallelism = null)
		{
			var rv = Execute ("pack", project, properties, false, msbuildParallelism: msbuildParallelism);
			Assert.That (rv.ExitCode, Is.Not.EqualTo (0), "Unexpected success");
			return rv;
		}

		public static ExecutionResult AssertPublish (string project, Dictionary<string, string>? properties = null)
		{
			return Execute ("publish", project, properties, true);
		}

		public static ExecutionResult AssertPublishFailure (string project, Dictionary<string, string>? properties = null)
		{
			var rv = Execute ("publish", project, properties, false);
			Assert.That (rv.ExitCode, Is.Not.EqualTo (0), "Unexpected success");
			return rv;
		}

		public static ExecutionResult AssertRestore (string project, Dictionary<string, string>? properties = null)
		{
			return Execute ("restore", project, properties, true);
		}

		public static ExecutionResult Restore (string project, Dictionary<string, string>? properties = null)
		{
			return Execute ("restore", project, properties, false);
		}

		public static ExecutionResult AssertBuild (string project, Dictionary<string, string>? properties = null, string? target = null, TimeSpan? timeout = null)
		{
			return Execute ("build", project, properties, true, target: target, timeout: timeout);
		}

		public static ExecutionResult AssertRun (string project, Dictionary<string, string>? properties = null, TimeSpan? timeout = null, Dictionary<string, string>? environmentVariables = null)
		{
			var extraArguments = environmentVariables?.SelectMany (kvp => new string [] { "-e", $"{kvp.Key}={kvp.Value}" })?.ToArray () ?? [];
			return Execute ("run", project, properties, true, timeout: timeout, extraArguments: extraArguments);
		}

		public static ExecutionResult AssertBuildFailure (string project, Dictionary<string, string>? properties = null, string? target = null)
		{
			var rv = Execute ("build", project, properties, false, target: target);
			Assert.That (rv.ExitCode, Is.Not.EqualTo (0), "Unexpected success");
			return rv;
		}

		public static ExecutionResult Build (string project, Dictionary<string, string>? properties = null)
		{
			return Execute ("build", project, properties, false);
		}

		public static ExecutionResult AssertNew (string outputDirectory, string template, string? name = null, string? language = null)
		{
			Directory.CreateDirectory (outputDirectory);

			var args = new List<string> ();
			args.Add ("new");
			args.Add (template);
			if (!string.IsNullOrEmpty (name)) {
				args.Add ("--name");
				args.Add (name!);
			}

			if (!string.IsNullOrEmpty (language)) {
				args.Add ("--language");
				args.Add (language);
			}

			var env = new Dictionary<string, string?> ();
			env ["MSBuildSDKsPath"] = null;
			env ["MSBUILD_EXE_PATH"] = null;
			var rv = Execution.RunAsync (Executable, args, env, Console.Out, workingDirectory: outputDirectory, timeout: TimeSpan.FromMinutes (10)).Result;
			var output = rv.Output.MergedOutput;
			if (rv.ExitCode != 0) {
				Console.WriteLine ($"'{Executable} {StringUtils.FormatArguments (args)}' failed with exit code {rv.ExitCode}.");
				Console.WriteLine (output);
				Assert.That (rv.ExitCode, Is.EqualTo (0), $"Exit code: {Executable} {StringUtils.FormatArguments (args)}");
			}
			return new ExecutionResult (output, output, rv.ExitCode, rv.Duration);
		}

		public static ExecutionResult InstallWorkload (params string [] workloads)
		{
			var args = new List<string> ();
			args.Add ("workload");
			args.Add ("install");
			args.AddRange (workloads);
			args.Add ("-v");
			args.Add ("diag");
			args.Add ("--skip-manifest-update");

			var env = new Dictionary<string, string?> ();
			env ["MSBuildSDKsPath"] = null;
			env ["MSBUILD_EXE_PATH"] = null;

			var rv = Execution.RunAsync (Executable, args, env, Console.Out, workingDirectory: Configuration.SourceRoot, timeout: TimeSpan.FromMinutes (10)).Result;
			var output = rv.Output.MergedOutput;
			if (rv.ExitCode != 0) {
				var msg = new StringBuilder ();
				msg.AppendLine ($"'dotnet workload install' failed with exit code {rv.ExitCode}");
				msg.AppendLine ($"Full command: {Executable} {StringUtils.FormatArguments (args)}");
				msg.AppendLine (output);
				Console.WriteLine (msg);
				Assert.Fail (msg.ToString ());
			}
			return new ExecutionResult (output, output, rv.ExitCode, rv.Duration);
		}

		public static ExecutionResult InstallTool (string tool, string path)
		{
			var installed = ExecuteCommand (Executable, "tool", "list", "--tool-path", path);
			if (!installed.StandardOutput.ToString ().Contains (tool))
				installed = ExecuteCommand (Executable, "tool", "install", tool, "--tool-path", path);
			return installed;
		}

		public static ExecutionResult RunTool (string tool, params string [] args) => ExecuteCommand (tool, args);

		public static string GetProperty (string projectPath, string propertyName, Dictionary<string, string>? properties = null)
		{
			if (!File.Exists (projectPath))
				throw new FileNotFoundException ($"The project file '{projectPath}' does not exist.");

			var args = new List<string> ();
			args.Add ("build");
			args.Add (projectPath);
			args.Add ($"-getProperty:{propertyName}");
			args.Add ("-nologo");
			args.Add ("-verbosity:quiet");

			if (properties is not null) {
				foreach (var prop in properties) {
					if (prop.Value.IndexOfAny (new char [] { ';' }) >= 0) {
						args.Add ($"/p:{prop.Key}=\"{prop.Value}\"");
					} else {
						args.Add ($"/p:{prop.Key}={prop.Value}");
					}
				}
			}

			var env = new Dictionary<string, string?> ();
			env ["MSBuildSDKsPath"] = null;
			env ["MSBUILD_EXE_PATH"] = null;

			var rv = Execution.RunAsync (Executable, args, env, null, workingDirectory: Path.GetDirectoryName (projectPath), timeout: TimeSpan.FromMinutes (2)).Result;
			var output = rv.Output.MergedOutput;

			if (rv.ExitCode != 0)
				throw new Exception ($"Failed to get property '{propertyName}' from project '{projectPath}'. Exit code: {rv.ExitCode}. Output: {output}");

			// Extract the property value from the output
			return output.Trim ();
		}

		public static ExecutionResult ExecuteCommand (string exe, params string [] args)
		{
			return ExecuteCommand (exe, null, args);
		}

		public static ExecutionResult ExecuteCommand (string exe, Dictionary<string, string?>? environment, params string [] args)
		{
			var env = environment ?? new Dictionary<string, string?> ();
			env ["MSBuildSDKsPath"] = null;
			env ["MSBUILD_EXE_PATH"] = null;

			var rv = Execution.RunAsync (exe, args, env, Console.Out, workingDirectory: Configuration.SourceRoot, timeout: TimeSpan.FromMinutes (10)).Result;
			var output = rv.Output.MergedOutput;
			if (rv.ExitCode != 0) {
				// Write the complete output to the console. This ends up in a separate
				// log file, so it's fine if it's big.
				Console.WriteLine ($"'{exe}' failed with exit code {rv.ExitCode}");
				Console.WriteLine ($"Full command: {exe} {StringUtils.FormatArguments (args)}");
				Console.WriteLine (output);

				// Only include the last few lines of the output in the failure message.
				// The failure message ends up embedded in the test results (and thus in
				// the HTML report), so including the entire output can make those files
				// enormous (hundreds of MBs) when the command was executed with diagnostic
				// verbosity.
				var msg = new StringBuilder ();
				msg.AppendLine ($"'{exe}' failed with exit code {rv.ExitCode}");
				msg.AppendLine ($"Full command: {exe} {StringUtils.FormatArguments (args)}");
				msg.AppendLine (GetLastLines (rv.Output.MergedOutputLines, 100));
				Assert.Fail (msg.ToString ());
			}
			return new ExecutionResult (output, output, rv.ExitCode, rv.Duration);
		}

		// Returns the last 'count' lines of the given output, prefixed with a note if any
		// lines were omitted.
		static string GetLastLines (IList<string> lines, int count)
		{
			if (lines.Count <= count)
				return string.Join ("\n", lines);

			var lastLines = lines.Skip (lines.Count - count);
			return $"[Output truncated to the last {count} lines (of {lines.Count} lines); see the full log for the complete output]{Environment.NewLine}" +
				string.Join ("\n", lastLines);
		}

		public static ExecutionResult Execute (string verb, string project, Dictionary<string, string>? properties, bool assert_success = true, string? target = null, bool? msbuildParallelism = null, TimeSpan? timeout = null, params string [] extraArguments)
		{
			if (!File.Exists (project))
				throw new FileNotFoundException ($"The project file '{project}' does not exist.");
			verb = verb.ToLowerInvariant ();

			switch (verb) {
			case "clean":
			case "build":
			case "pack":
			case "publish":
			case "restore":
			case "run":
				var args = new List<string> ();
				args.Add (verb);
				args.Add (project);
				if (properties is not null) {
					Dictionary<string, string>? generatedProps = null;
					foreach (var prop in properties) {
						// Some properties must be specified on the command line to work properly ("TargetFrameworks").
						// Some tests require certain properties to be specified in a file.
						// So we support both, where prefixing a property name with "cmdline:" forces it to be passed on the command line,
						// while prefixing it with "file:" forces it to be specified in a file.
						// No prefix means the default (which can change): currently on the command line.
						var cmdline = true; // default
						var key = prop.Key;
						var value = prop.Value;
						if (key.StartsWith ("file:", StringComparison.Ordinal)) {
							key = key.Substring ("file:".Length);
							cmdline = false;
						} else if (key.StartsWith ("cmdline:", StringComparison.Ordinal)) {
							key = key.Substring ("cmdline:".Length);
							cmdline = true;
						}
						if (cmdline) {
							if (prop.Value.IndexOfAny (new char [] { ';' }) >= 0) {
								// https://github.com/dotnet/msbuild/issues/471
								// Escaping the semi colon like the issue suggests at one point doesn't work, because in
								// that case MSBuild won't split the string into its parts for tasks that take a string[].
								// This means that a task that takes a "string[] RuntimeIdentifiers" will get an array with
								// a single element, where that single element is the whole RuntimeIdentifiers string.
								// Example task: https://github.com/dotnet/sdk/blob/ffca47e9a36652da2e7041360f2201a2ba197194/src/Tasks/Microsoft.NET.Build.Tasks/ProcessFrameworkReferences.cs#L45
								args.Add ($"/p:{key}=\"{value}\"");
							} else {
								args.Add ($"/p:{key}={value}");
							}
						} else {
							if (generatedProps is null)
								generatedProps = new Dictionary<string, string> ();
							generatedProps.Add (key, value);
						}
					}
					if (generatedProps is not null) {
						var settings = new XmlWriterSettings ();
						var sb = new StringBuilder ();
						var xml = XmlWriter.Create (sb, settings);
						xml.WriteStartElement ("Project");
						xml.WriteStartElement ("PropertyGroup");
						foreach (var prop in generatedProps) {
							xml.WriteStartElement (prop.Key);
							xml.WriteString (prop.Value);
							xml.WriteEndElement ();
						}
						xml.WriteEndElement ();
						xml.WriteEndElement ();
						xml.Flush ();

						var generatedProjectFile = Path.Combine (Cache.CreateTemporaryDirectory (), "GeneratedProjectFile.props");
						File.WriteAllText (generatedProjectFile, sb.ToString ());
						args.Add ($"/p:GeneratedProjectFile={generatedProjectFile}");
					}
				}
				if (!string.IsNullOrEmpty (target))
					args.Add ("/t:" + target);
				var binlogPath = Path.Combine (Path.GetDirectoryName (project)!, $"log-{verb}-{DateTime.Now:yyyyMMdd_HHmmss}.binlog");
				args.Add ($"/bl:{binlogPath}");
				Console.WriteLine ($"Binlog: {binlogPath}");

				// Work around https://github.com/dotnet/msbuild/issues/8845
				args.Add ("/v:diag");
				args.Add ("/consoleloggerparameters:Verbosity=Quiet");
				// vb does not have preview lang, so we force it to latest
				if (project.EndsWith (".vbproj", StringComparison.OrdinalIgnoreCase))
					args.Add ("/p:LangVersion=latest");
				// End workaround

				if (msbuildParallelism.HasValue) {
					if (msbuildParallelism.Value) {
						args.Add ("-maxcpucount"); // this means "use as many processes as there are cpus"
					} else {
						args.Add ("-maxcpucount:1");
					}
				}
				args.AddRange (extraArguments);

				var env = new Dictionary<string, string?> ();
				env ["MSBuildSDKsPath"] = null;
				env ["MSBUILD_EXE_PATH"] = null;
				timeout ??= TimeSpan.FromMinutes (10);
				var rv = Execution.RunAsync (Executable, args, env, Console.Out, workingDirectory: Path.GetDirectoryName (project), timeout: timeout).Result;
				var output = rv.Output.MergedOutput;
				if (assert_success && rv.ExitCode != 0) {
					Console.WriteLine ($"'{Executable} {StringUtils.FormatArguments (args)}' failed with exit code {rv.ExitCode}.");
					Console.WriteLine (output);
					if (rv.ExitCode != 0) {
						var msg = new StringBuilder ();
						if (rv.TimedOut) {
							msg.AppendLine ($"'dotnet {verb}' timed out after {timeout}");
						} else {
							msg.AppendLine ($"'dotnet {verb}' failed with exit code {rv.ExitCode}");
						}
						msg.AppendLine ($"Full command: {Executable} {StringUtils.FormatArguments (args)}");
#if !MSBUILD_TASKS
						try {
							var errors = BinLog.GetBuildLogErrors (binlogPath).ToArray ();
							if (errors.Any ()) {
								var errorsToList = errors.Take (10).ToArray ();
								msg.AppendLine ($"Listing first {errorsToList.Length} error(s) (of {errors.Length} error(s)):");
								foreach (var error in errorsToList)
									msg.AppendLine ($"    {string.Join ($"{Environment.NewLine}        ", error.ToString ().Split ('\n', '\r'))}");
							}
						} catch (Exception e) {
							msg.AppendLine ($"Failed to list errors in the binlog {binlogPath}: {e}");
						}
#endif
						Assert.Fail (msg.ToString ());
					}
					Assert.That (rv.ExitCode, Is.EqualTo (0), $"Exit code: {Executable} {StringUtils.FormatArguments (args)}");
				}
				return new ExecutionResult (output, output, rv.ExitCode, rv.Duration) {
					BinLogPath = binlogPath,
				};
			default:
				throw new NotImplementedException ($"Unknown dotnet action: '{verb}'");
			}
		}

		public static string GetProperty (string projectPath, string name, string? target = null, Dictionary<string, string>? properties = null, Dictionary<string, string?>? environment = null)
		{
			return Get (projectPath, name, "Property", target, properties, environment);
		}

		// returns json
		public static string GetItems (string projectPath, string name, string? target = null, Dictionary<string, string>? properties = null, Dictionary<string, string?>? environment = null)
		{
			return Get (projectPath, name, "Item", target, properties, environment);
		}

		static string Get (string projectPath, string name, string what, string? target = null, Dictionary<string, string>? properties = null, Dictionary<string, string?>? environment = null)
		{
			if (!File.Exists (projectPath))
				throw new FileNotFoundException ($"The project file '{projectPath}' does not exist.");

			var outputFile = Path.Combine (Cache.CreateTemporaryDirectory (), "evaluateOutput.txt");

			var args = new List<string> ();
			args.Add ("build");
			args.Add (projectPath);
			if (!string.IsNullOrEmpty (target))
				args.Add ($"-target:{target}");
			args.Add ($"-get{what}:{name}");
			args.Add ("-nologo");
			args.Add ("-verbosity:quiet");
			args.Add ($"-getResultOutputFile:{outputFile}");
			var binlogPath = Path.Combine (Path.GetDirectoryName (projectPath)!, $"log-get{what}-{DateTime.Now:yyyyMMdd_HHmmss}.binlog");
			args.Add ($"-bl:{binlogPath}");
			args.Add ($"-v:diag");
			Console.WriteLine ($"Binlog: {binlogPath}");

			if (properties is not null) {
				foreach (var prop in properties) {
					if (prop.Value.IndexOfAny (new char [] { ';' }) >= 0) {
						args.Add ($"/p:{prop.Key}=\"{prop.Value}\"");
					} else {
						args.Add ($"/p:{prop.Key}={prop.Value}");
					}
				}
			}
			ExecuteCommand (Executable, environment, args.ToArray ());
			return File.ReadAllText (outputFile);
		}

		public static void CompareApps (string old_app, string new_app)
		{
#if VERBOSE_COMPARISON
			Console.WriteLine ($"Comparing:");
			Console.WriteLine ($"    {old_app}");
			Console.WriteLine ($"    {new_app}");
#endif

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
					}

					var filename = Path.GetFileName (v);
					switch (filename) {
					case "icudt.dat":
					case "icudt_hybrid.dat":
						return false; // ICU data file only present on .NET
					case "runtime-options.plist":
						return false; // the .NET runtime will deal with selecting the http handler, no need for us to do anything
					case "runtimeconfig.bin":
						return false; // this file is present for .NET apps, but not legacy apps.
					case "embedded.mobileprovision":
					case "CodeResources":
						return false; // sometimes we don't sign in .NET when we do in legacy (if there's an empty Entitlements.plist file)
					}

					var components = v.Split ('/');
					if (components.Any (v => v.EndsWith (".framework", StringComparison.Ordinal))) {
						return false; // This is Mono.framework, which is waiting for https://github.com/dotnet/runtime/issues/42846
					}

					return true;
				});
			});

			var old_files = filter (all_old_files);
			var new_files = filter (all_new_files);

			var extra_old_files = old_files.Except (new_files);
			var extra_new_files = new_files.Except (old_files);

#if VERBOSE_COMPARISON
			Console.WriteLine ("Files in old app:");
			foreach (var f in all_old_files.OrderBy (v => v))
				Console.WriteLine ($"\t{f}");
			Console.WriteLine ("Files in new app:");
			foreach (var f in all_new_files.OrderBy (v => v))
				Console.WriteLine ($"\t{f}");

			Console.WriteLine ("Files in old app (filtered):");
			foreach (var f in old_files.OrderBy (v => v))
				Console.WriteLine ($"\t{f}");
			Console.WriteLine ("Files in new app (filtered):");
			foreach (var f in new_files.OrderBy (v => v))
				Console.WriteLine ($"\t{f}");

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
#endif

			Assert.That (extra_new_files, Is.Empty, "Extra dotnet files");
			Assert.That (extra_old_files, Is.Empty, "Missing dotnet files");
		}
	}

	public class ExecutionResult {
		public string StandardOutput;
		public string StandardError;
		public int ExitCode;
		public bool TimedOut;
		public string BinLogPath;
		public TimeSpan Duration;

		public ExecutionResult (string stdout, string stderr, int exitCode, TimeSpan duration)
		{
			StandardOutput = stdout;
			StandardError = stderr;
			ExitCode = exitCode;
			BinLogPath = string.Empty;
			Duration = duration;
		}
	}
}
