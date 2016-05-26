using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace xharness
{
	public class XProcess
	{
		public Harness Harness;

		public int VerbosityLevel = 1;
		Process process = new Process ();
		CountdownEvent output_completed = new CountdownEvent (2);
		object output_lock = new object ();
		StringBuilder output = new StringBuilder ();

		public string FileName {
			get { return process.StartInfo.FileName; }
			set { process.StartInfo.FileName = value; }
		}

		public string Arguments {
			get { return process.StartInfo.Arguments; }
			set { process.StartInfo.Arguments = value; }
		}

		public Dictionary<string, string> EnvironmentVariables { get; set; }

		public void Start ()
		{
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.RedirectStandardOutput = true;

			if (EnvironmentVariables != null) {
				foreach (var kvp in EnvironmentVariables)
					process.StartInfo.EnvironmentVariables.Add (kvp.Key, kvp.Value);
			}

			process.OutputDataReceived += (object sender, DataReceivedEventArgs e) => 
			{
				if (e.Data == null) {
					output_completed.Signal ();
				} else {
					lock (output_lock) {
						output.AppendLine (e.Data);
						Harness.Log (VerbosityLevel, e.Data);
					}
				}
			};
			process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) => 
			{
				if (e.Data == null) {
					output_completed.Signal ();
				} else {
					lock (output_lock) {
						output.AppendLine (e.Data);
						Harness.Log (VerbosityLevel, e.Data);
					}
				}
			};

			Harness.Log ("{0} {1}", process.StartInfo.FileName, process.StartInfo.Arguments);
			process.Start ();
			process.BeginErrorReadLine ();
			process.BeginOutputReadLine ();
		}

		public bool WaitForExit (TimeSpan timeout)
		{
			return process.WaitForExit ((int) timeout.TotalMilliseconds);
		}

		public int ExitCode {
			get { return process.ExitCode; }
		}

		public string ReadCurrentOutput ()
		{
			lock (output_lock)
				return output.ToString ();
		}

		public int Id {
			get { return process.Id; }
		}
	}
}

