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
					dotnet_executable = Configuration.EvaluateVariable ("DOTNET5");
					if (string.IsNullOrEmpty (dotnet_executable))
						throw new Exception ($"Could not find the dotnet executable.");
					if (!File.Exists (dotnet_executable))
						throw new FileNotFoundException ($"The dotnet executable '{dotnet_executable}' does not exist.");
				}
				return dotnet_executable;
			}
		}

		public static void AssertBuild (string project, Dictionary<string, string> properties = null, string verbosity = "diagnostic")
		{
			Execute ("build", project, properties, out var _, verbosity, true);
		}

		public static int Execute (string verb, string project, Dictionary<string, string> properties, out StringBuilder output, string verbosity = "diagnostic", bool assert_success = true)
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
				if (!string.IsNullOrEmpty (verbosity))
					args.Add ($"/verbosity:{verbosity}");
				args.Add ($"/bl:{Path.Combine (Path.GetDirectoryName (project), "log.binlog")}");
				var env = new Dictionary<string, string> ();
				env ["MSBuildSDKsPath"] = null;
				env ["MSBUILD_EXE_PATH"] = null;
				output = new StringBuilder ();
				var rv = ExecutionHelper.Execute (Executable, args, env, output, output, workingDirectory: Path.GetDirectoryName (project), timeout: TimeSpan.FromMinutes (10));
				if (rv != 0) {
					Console.WriteLine ($"'{Executable} {StringUtils.FormatArguments (args)}' failed with exit code {rv}.");
					Console.WriteLine (output);
				}
				if (assert_success && rv != 0) {
					var errors = output.ToString ().Split (new char [] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Where (v => v.Contains ("error", StringComparison.OrdinalIgnoreCase));
					errors = errors.Take (10); // only the first 10 lines
					var msg = string.Empty;
					if (errors.Any ())
						msg = "\n\t" + string.Join ("\n\t", errors);
					Assert.AreEqual (0, rv, $"Exit code: {Executable} {StringUtils.FormatArguments (args)}{msg}");
				}
				return rv;
			default:
				throw new NotImplementedException ($"Unknown dotnet action: '{verb}'");
			}
		}
	}
}
