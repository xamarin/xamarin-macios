using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;

using NUnit.Framework;

namespace Xamarin.Tests
{
	class ToolMessage
	{
		public bool IsError;
		public bool IsWarning { get { return !IsError; } }
		public string Prefix;
		public int Number;
		public string PrefixedNumber { get { return Prefix + Number.ToString (); } }
		public string Message;
	//	public string Filename;
	//	public int LineNumber;
	}

	class Tool
	{
		StringBuilder output = new StringBuilder ();

		List<string> output_lines;

		List<ToolMessage> messages = new List<ToolMessage> ();

		public Dictionary<string, string> EnvironmentVariables { get; set; }
		public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds (60);

		public IEnumerable<ToolMessage> Messages { get { return messages; } }
		List<string> OutputLines {
			get {
				if (output_lines == null) {
					output_lines = new List<string> ();
					output_lines.AddRange (output.ToString ().Split ('\n'));
				}
				return output_lines;
			}
		}

		public int Execute (string arguments, params string [] args)
		{
			return Execute (Configuration.MtouchPath, arguments, args);
		}

		public int Execute (string toolPath, string arguments, params string [] args)
		{
			output.Clear ();
			output_lines = null;

			var rv = ExecutionHelper.Execute (toolPath, string.Format (arguments, args), EnvironmentVariables, output, output);

			if (rv != 0) {
				if (output.Length > 0)
					Console.WriteLine (output);
			}

			ParseMessages ();

			return rv;
		}

		void ParseMessages ()
		{
			messages.Clear ();

			foreach (var l in output.ToString ().Split ('\n')) {
				var line = l;
				var msg = new ToolMessage ();
				if (line.StartsWith ("error ", StringComparison.Ordinal)) {
					msg.IsError = true;
					line = line.Substring (6);
				} else if (line.StartsWith ("warning ", StringComparison.Ordinal)) {
					msg.IsError = false;
					line = line.Substring (8);
				} else {
					// something else
					continue;
				}
				if (line.Length < 7)
					continue; // something else
				msg.Prefix = line.Substring (0, 2);
				if (!int.TryParse (line.Substring (2, 4), out msg.Number))
					continue; // something else
				msg.Message = line.Substring (8);

				messages.Add (msg);
			}
		}

		public bool HasErrorPattern (string prefix, int number, string messagePattern)
		{
			foreach (var msg in messages) {
				if (msg.IsError && msg.Prefix == prefix && msg.Number == number && Regex.IsMatch (msg.Message, messagePattern))
					return true;
			}
			return false;
		}

		public bool HasError (string prefix, int number, string message)
		{
			foreach (var msg in messages) {
				if (msg.IsError && msg.Prefix == prefix && msg.Number == number && msg.Message == message)
					return true;
			}
			return false;
		}

		public void AssertErrorPattern (int number, string messagePattern)
		{
			AssertErrorPattern ("MT", number, messagePattern);
		}

		public void AssertErrorPattern (string prefix, int number, string messagePattern)
		{
			if (!messages.Any ((msg) => msg.Prefix == prefix && msg.Number == number))
				Assert.Fail (string.Format ("The error '{0}{1:0000}' was not found in the output.", prefix, number));

			if (messages.Any ((msg) => Regex.IsMatch (msg.Message, messagePattern)))
				return;
			
			var details = messages.Where ((msg) => msg.Prefix == prefix && msg.Number == number && !Regex.IsMatch (msg.Message, messagePattern)).Select ((msg) => string.Format ("\tThe message '{0}' did not match the pattern '{1}'.", msg.Message, messagePattern));
			Assert.Fail (string.Format ("The error '{0}{1:0000}: {2}' was not found in the output:\n{3}", prefix, number, messagePattern, string.Join ("\n", details.ToArray ())));
		}

		public void AssertError (int number, string message)
		{
			AssertError ("MT", number, message);
		}

		public void AssertError (string prefix, int number, string message)
		{
			if (!messages.Any ((msg) => msg.Prefix == prefix && msg.Number == number))
				Assert.Fail (string.Format ("The error '{0}{1:0000}' was not found in the output.", prefix, number));

			if (messages.Any ((msg) => msg.Message == message))
				return;

			var details = messages.Where ((msg) => msg.Prefix == prefix && msg.Number == number && msg.Message != message).Select ((msg) => string.Format ("\tMessage #{2} did not match:\n\t\tactual:   '{0}'\n\t\texpected: '{1}'", msg.Message, message, messages.IndexOf (msg) + 1));
			Assert.Fail (string.Format ("The error '{0}{1:0000}: {2}' was not found in the output:\n{3}", prefix, number, message, string.Join ("\n", details.ToArray ())));
		}

		public void AssertWarningPattern (int number, string messagePattern)
		{
			AssertWarningPattern ("MT", number, messagePattern);
		}

		public void AssertWarningPattern (string prefix, int number, string messagePattern)
		{
			if (!messages.Any ((msg) => msg.Prefix == prefix && msg.Number == number))
				Assert.Fail (string.Format ("The warning '{0}{1:0000}' was not found in the output.", prefix, number));

			if (messages.Any ((msg) => Regex.IsMatch (msg.Message, messagePattern)))
				return;

			var details = messages.Where ((msg) => msg.Prefix == prefix && msg.Number == number && !Regex.IsMatch (msg.Message, messagePattern)).Select ((msg) => string.Format ("\tThe message '{0}' did not match the pattern '{1}'.", msg.Message, messagePattern));
			Assert.Fail (string.Format ("The warning '{0}{1:0000}: {2}' was not found in the output:\n{3}", prefix, number, messagePattern, string.Join ("\n", details.ToArray ())));
		}

		public void AssertWarning (int number, string message)
		{
			AssertWarning ("MT", number, message);
		}

		public void AssertWarning (string prefix, int number, string message)
		{
			if (!messages.Any ((msg) => msg.Prefix == prefix && msg.Number == number))
				Assert.Fail (string.Format ("The warning '{0}{1:0000}' was not found in the output.", prefix, number));

			if (messages.Any ((msg) => msg.Message == message))
				return;

			var details = messages.Where ((msg) => msg.Prefix == prefix && msg.Number == number && msg.Message != message).Select ((msg) => string.Format ("\tMessage #{2} did not match:\n\t\tactual:   '{0}'\n\t\texpected: '{1}'", msg.Message, message, messages.IndexOf (msg) + 1));
			Assert.Fail (string.Format ("The warning '{0}{1:0000}: {2}' was not found in the output:\n{3}", prefix, number, message, string.Join ("\n", details.ToArray ())));
		}

		public void AssertNoWarnings ()
		{
			var warnings = messages.Where ((v) => v.IsWarning);
			if (!warnings.Any ())
				return;

			Assert.Fail ("No warnings expected, but got:\n{0}\t", string.Join ("\n\t", warnings.Select ((v) => v.Message).ToArray ()));
		}

		public bool HasOutput (string line)
		{
			return OutputLines.Contains (line);
		}

		public bool HasOutputPattern (string linePattern)
		{
			foreach (var line in OutputLines) {
				if (Regex.IsMatch (line, linePattern, RegexOptions.CultureInvariant))
					return true;
			}

			return false;
		}

		public void AssertOutputPattern (string linePattern)
		{
			if (!HasOutputPattern (linePattern))
				Assert.Fail (string.Format ("The output does not contain the line '{0}'", linePattern));
		}
	}

	class XBuild
	{
		public static string ToolPath {
			get
			{
				return "/Library/Frameworks/Mono.framework/Commands/xbuild";
			}
		}

		public static void Build (string project, string configuration = "Debug", string platform = "iPhoneSimulator", string verbosity = null, TimeSpan? timeout = null)
		{
			ExecutionHelper.Execute (ToolPath, string.Format ("/p:Configuration={0} /p:Platform={1} {2} \"{3}\"", configuration, platform, verbosity == null ? string.Empty : "/verbosity:" + verbosity, project), timeout: timeout);
		}
	}

	static class ExecutionHelper {
		static int Execute (ProcessStartInfo psi, StringBuilder stdout, StringBuilder stderr, TimeSpan? timeout = null)
		{
			var watch = new Stopwatch ();
			watch.Start ();

			try {
				psi.UseShellExecute = false;
				psi.RedirectStandardError = true;
				psi.RedirectStandardOutput = true;
				Console.WriteLine ("{0} {1}", psi.FileName, psi.Arguments);
				using (var p = new Process ()) {
					p.StartInfo = psi;
					// mtouch/mmp writes UTF8 data outside of the ASCII range, so we need to make sure
					// we read it in the same format. This also means we can't use the events to get
					// stdout/stderr, because mono's Process class parses those using Encoding.Default.
					p.StartInfo.StandardOutputEncoding = Encoding.UTF8;
					p.StartInfo.StandardErrorEncoding = Encoding.UTF8;
					p.Start ();

					var outReader = new Thread (() =>
						{
							string l;
							while ((l = p.StandardOutput.ReadLine ()) != null) {
								lock (stdout)
									stdout.AppendLine (l);
							}
						})
					{
						IsBackground = true,
					};
					outReader.Start ();

					var errReader = new Thread (() =>
						{
							string l;
							while ((l = p.StandardError.ReadLine ()) != null) {
								lock (stderr)
									stderr.AppendLine (l);
							}
						})
					{
						IsBackground = true,
					};
					errReader.Start ();

					if (timeout == null)
						timeout = TimeSpan.FromMinutes (5);
					if (!p.WaitForExit ((int) timeout.Value.TotalMilliseconds)) {
						Console.WriteLine ("Command didn't finish in {0} minutes:", timeout.Value.TotalMinutes);
						Console.WriteLine ("{0} {1}", p.StartInfo.FileName, p.StartInfo.Arguments);
						Console.WriteLine ("Will now kill the process");
						kill (p.Id, 9);
						if (!p.WaitForExit (1000 /* killing should be fairly quick */)) {
							Console.WriteLine ("Kill failed to kill in 1 second !?");
							return 1;
						}
					}

					outReader.Join (TimeSpan.FromSeconds (1));
					errReader.Join (TimeSpan.FromSeconds (1));

					return p.ExitCode;
				}
			} finally {
				Console.WriteLine ("{0} Executed in {1}: {2} {3}", DateTime.Now, watch.Elapsed.ToString (), psi.FileName, psi.Arguments);
			}
		}

		public static int Execute (string fileName, string arguments, out string output, TimeSpan? timeout = null)
		{
			var sb = new StringBuilder ();
			var psi = new ProcessStartInfo ();
			psi.FileName = fileName;
			psi.Arguments = arguments;
			var rv = Execute (psi, sb, sb, timeout);
			output = sb.ToString ();
			return rv;
		}

		public static int Execute (string fileName, string arguments, Dictionary<string, string> environmentVariables, StringBuilder stdout, StringBuilder stderr, TimeSpan? timeout = null)
		{
			if (stdout == null)
				stdout = new StringBuilder ();
			if (stderr == null)
				stderr = new StringBuilder ();

			var psi = new ProcessStartInfo ();
			psi.FileName = fileName;
			psi.Arguments = arguments;
			if (environmentVariables != null) {
				var envs = psi.EnvironmentVariables;
				foreach (var kvp in environmentVariables) {
					envs [kvp.Key] = kvp.Value;
				}
			}

			return Execute (psi, stdout, stderr, timeout);
		}

		[DllImport ("libc")]
		private static extern void kill (int pid, int sig);

		public static string Execute (string fileName, string arguments, bool throwOnError = true, Dictionary<string,string> environmentVariables = null,
			bool hide_output = false, TimeSpan? timeout = null
		)
		{
			StringBuilder output = new StringBuilder ();
			int exitCode = Execute (fileName, arguments, environmentVariables, output, output, timeout);
			if (!hide_output) {
				Console.WriteLine ("{0} {1}", fileName, arguments);
				Console.WriteLine (output);
				Console.WriteLine ("Exit code: {0}", exitCode);
			}
			if (throwOnError && exitCode != 0)
				throw new TestExecutionException (output.ToString ());
			return output.ToString ();
		}
	}

	class TestExecutionException : Exception {
		public TestExecutionException (string output)
			: base (output)
		{
		}
	}
}
