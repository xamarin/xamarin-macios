using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using NUnit.Framework;
using Xamarin.Utils;

#nullable disable // until we get around to fixing this file

namespace Xamarin.Tests {
	class XBuild {
		public static string ToolPath {
			get {
				return Configuration.XIBuildPath;
			}
		}

		public static string BuildXM (string project, string configuration = "IsDebug", string platform = "x86", string verbosity = null, TimeSpan? timeout = null, string [] arguments = null, string targets = "Clean,Build")
		{
			return Build (project, ApplePlatform.MacOSX, configuration, platform, verbosity, timeout, arguments, targets);
		}

		public static string BuildXI (string project, string configuration = "IsDebug", string platform = "iPhoneSimulator", string verbosity = null, TimeSpan? timeout = null, string [] arguments = null, string targets = "Clean,Build")
		{
			return Build (project, ApplePlatform.iOS, configuration, platform, verbosity, timeout, arguments, targets);
		}

		static string Build (string project, ApplePlatform applePlatform, string configuration = "IsDebug", string platform = "iPhoneSimulator", string verbosity = null, TimeSpan? timeout = null, string [] arguments = null, string targets = "Clean,Build")
		{
			return ExecutionHelper.Execute (ToolPath,
				new string [] {
					"--",
					$"/p:Configuration={configuration}",
					$"/p:Platform={platform}",
					$"/verbosity:{(string.IsNullOrEmpty (verbosity) ? "normal" : verbosity)}",
					"/r:True", // restore nuget packages which are used in some test cases
					$"/t:{targets}", // clean and then build, in case we left something behind in a shared dir
					project
				}.Union (arguments ?? new string [] { }).ToArray (),
				environmentVariables: Configuration.GetBuildEnvironment (applePlatform),
				timeout: timeout);
		}
	}

	static class ExecutionHelper {
		public static int Execute (string fileName, IList<string> arguments)
		{
			return Execute (fileName, arguments, null, null, null, null);
		}

		public static int Execute (string fileName, IList<string> arguments, TimeSpan? timeout)
		{
			return Execute (fileName, arguments, null, null, null, timeout);
		}

		public static int Execute (string fileName, IList<string> arguments, string working_directory = null, Action<string> stdout_callback = null, Action<string> stderr_callback = null, TimeSpan? timeout = null)
		{
			return Execute (fileName, arguments, timed_out: out var _, workingDirectory: working_directory, stdout_callback: stdout_callback, stderr_callback: stderr_callback, timeout: timeout);
		}

		public static int Execute (string fileName, IList<string> arguments, out StringBuilder output)
		{
			return Execute (fileName, arguments, out output, null, null);
		}

		public static int Execute (string fileName, IList<string> arguments, out StringBuilder output, string working_directory, TimeSpan? timeout = null)
		{
			output = new StringBuilder ();
			return Execute (fileName, arguments, out var _, workingDirectory: working_directory, stdout: output, stderr: output, timeout: timeout);
		}

		public static int Execute (string fileName, IList<string> arguments, out StringBuilder output, string working_directory, Dictionary<string, string> environment_variables, TimeSpan? timeout = null)
		{
			output = new StringBuilder ();
			return Execute (fileName, arguments, out var _, workingDirectory: working_directory, stdout: output, stderr: output, timeout: timeout, environment_variables: environment_variables);
		}

		public static int Execute (string fileName, IList<string> arguments, out bool timed_out, string workingDirectory = null, Dictionary<string, string> environment_variables = null, StringBuilder stdout = null, StringBuilder stderr = null, TimeSpan? timeout = null)
		{
			var rv = Execution.RunWithStringBuildersAsync (fileName, arguments, workingDirectory: workingDirectory, environment: environment_variables, standardOutput: stdout, standardError: stderr, timeout: timeout).Result;
			timed_out = rv.TimedOut;
			if (rv.TimedOut)
				Console.WriteLine ($"Command '{fileName} {StringUtils.FormatArguments (arguments)}' didn't finish in {timeout.Value.TotalMilliseconds} ms, and was killed.", timeout.Value.TotalMinutes);
			return rv.ExitCode;
		}

		public static int Execute (string fileName, IList<string> arguments, out bool timed_out, string workingDirectory = null, Dictionary<string, string> environment_variables = null, Action<string> stdout_callback = null, Action<string> stderr_callback = null, TimeSpan? timeout = null)
		{
			if (stdout_callback is null)
				stdout_callback = Console.WriteLine;
			if (stderr_callback is null)
				stderr_callback = Console.Error.WriteLine;

			var rv = Execution.RunWithCallbacksAsync (fileName, arguments, workingDirectory: workingDirectory, environment: environment_variables, standardOutput: stdout_callback, standardError: stderr_callback, timeout: timeout).Result;
			timed_out = rv.TimedOut;
			if (rv.TimedOut)
				Console.WriteLine ($"Command '{fileName} {StringUtils.FormatArguments (arguments)}' didn't finish in {timeout.Value.TotalMilliseconds} ms, and was killed.", timeout.Value.TotalMinutes);
			return rv.ExitCode;
		}

		public static int Execute (string fileName, IList<string> arguments, out string output, TimeSpan? timeout = null)
		{
			var sb = new StringBuilder ();
			var rv = Execute (fileName, arguments, timed_out: out var _, stdout: sb, stderr: sb, timeout: timeout);
			output = sb.ToString ();
			return rv;
		}

		public static int Execute (string fileName, IList<string> arguments, Dictionary<string, string> environmentVariables, StringBuilder stdout, StringBuilder stderr, TimeSpan? timeout = null, string workingDirectory = null)
		{
			return Execute (fileName, arguments, timed_out: out var _, workingDirectory: workingDirectory, environment_variables: environmentVariables, stdout: stdout, stderr: stderr, timeout: timeout);
		}

		public static string Execute (string fileName, IList<string> arguments, bool throwOnError = true, Dictionary<string, string> environmentVariables = null, bool hide_output = false, TimeSpan? timeout = null)
		{
			var rv = Execution.RunAsync (fileName, arguments, mergeOutput: true, environment: environmentVariables, timeout: timeout).Result;
			var output = rv.StandardOutput.ToString ();
			var throw_exc = throwOnError && rv.ExitCode != 0;
			if (!hide_output || throw_exc) {
				Console.WriteLine ($"{fileName} {StringUtils.FormatArguments (arguments)} (exit code: {rv.ExitCode})");
				Console.WriteLine (output);
				Console.WriteLine ("Exit code: {0}", rv.ExitCode);
			}
			if (throwOnError && rv.ExitCode != 0)
				throw new Exception ($"Execution failed with exit code {rv.ExitCode}:\n{fileName} {StringUtils.FormatArguments (arguments)}\nReview standard output/standard error for more details.");
			return output;
		}
	}
}
