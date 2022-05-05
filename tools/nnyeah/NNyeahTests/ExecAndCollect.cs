using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Microsoft.MaciOS.Nnyeah.Tests {
	public class ExecAndCollect {
		public static string Run (string path, string args, string workingDirectory = "", bool verbose = false)
		{
			var output = new StringBuilder ();
			var exitCode = RunCommand (path, args, output: output, verbose: verbose, workingDirectory: workingDirectory);
			if (exitCode != 0)
				throw new Exception ($"Failed to execute (exit code {exitCode}): {path} {string.Join (" ", args)}\n{output.ToString ()}");
			return output.ToString ();
		}

		static void ReadStream (Stream stream, StringBuilder sb, ManualResetEvent completed)
		{
			var encoding = Encoding.UTF8;
			var decoder = encoding.GetDecoder ();
			var buffer = new byte [1024];
			var characters = new char [encoding.GetMaxCharCount (buffer.Length)];

			AsyncCallback? callback = null;
			callback = new AsyncCallback ((IAsyncResult ar) => {
				var read = stream.EndRead (ar);

				var chars = decoder.GetChars (buffer, 0, read, characters, 0);
				lock (sb)
					sb.Append (characters, 0, chars);

				if (read > 0) {
					stream.BeginRead (buffer, 0, buffer.Length, callback, null);
				} else {
					completed.Set ();
				}
			});
			stream.BeginRead (buffer, 0, buffer.Length, callback, null);
		}

		public static int RunCommand (string path, string args, IDictionary<string, string>? env = null, StringBuilder? output = null, bool verbose = false, string? workingDirectory = null)
		{
			var info = new ProcessStartInfo (path, args);
			info.UseShellExecute = false;
			info.RedirectStandardInput = false;
			info.RedirectStandardOutput = true;
			info.RedirectStandardError = true;
			if (workingDirectory is not null)
				info.WorkingDirectory = workingDirectory;

			if (output is null)
				output = new StringBuilder ();

			if (env != null) {
				foreach (var kvp in env) {
					if (kvp.Value == null) {
						if (info.EnvironmentVariables.ContainsKey (kvp.Key))
							info.EnvironmentVariables.Remove (kvp.Key);
					} else {
						info.EnvironmentVariables [kvp.Key] = kvp.Value;
					}
				}
			}

			if (info.EnvironmentVariables.ContainsKey ("XCODE_DEVELOPER_DIR_PATH")) {
				// VSfM adds this key, which confuses Xcode mightily if it doesn't match the value of xcode-select.
				// So just remove it, we don't need it for anything.
				info.EnvironmentVariables.Remove ("XCODE_DEVELOPER_DIR_PATH");
			}


			if (verbose) {
				var envOut = new StringBuilder ();
				foreach (var key in info.EnvironmentVariables.Keys) {
					if (key is string keyString) {
						var value = info.EnvironmentVariables [keyString];
						envOut.AppendLine ($"export {keyString}={value}");
					}
				}
				var pathArgs = $"{path} {args}";
				envOut.AppendLine (pathArgs);
				Console.Write (envOut.ToString ());
				Console.WriteLine (pathArgs);
			}

			var p = Process.Start (info);
			if (p is not null) {
				var error_output = new StringBuilder ();
				var stdout_completed = new ManualResetEvent (false);
				var stderr_completed = new ManualResetEvent (false);

				ReadStream (p.StandardOutput.BaseStream, output, stdout_completed);
				ReadStream (p.StandardError.BaseStream, error_output, stderr_completed);

				p.WaitForExit ();

				stderr_completed.WaitOne (TimeSpan.FromMinutes (1));
				stdout_completed.WaitOne (TimeSpan.FromMinutes (1));

				if (verbose) {
					if (output.Length > 0)
						Console.WriteLine (output);
					if (error_output.Length > 0)
						Console.WriteLine (error_output);
					if (p.ExitCode != 0)
						Console.Error.WriteLine ($"Process exited with code {p.ExitCode}");
				}
				if (p.ExitCode != 0 && error_output.Length > 0)
					output.Append (error_output);
				return p.ExitCode;
			}
			return -2;
		}
	}
}
