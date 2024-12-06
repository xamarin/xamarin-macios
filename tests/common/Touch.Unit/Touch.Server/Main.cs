// Main.cs: Touch.Unit Simple Server
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2011-2012 Xamarin Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

using Mono.Options;

// a simple, blocking (i.e. one device/app at the time), listener
class SimpleListener {

	static byte[] buffer = new byte [16 * 1024];

	TcpListener server;
	ManualResetEvent stopped = new ManualResetEvent (false);
	ManualResetEvent connected = new ManualResetEvent (false);
	
	IPAddress Address { get; set; }
	int Port { get; set; }
	string LogPath { get; set; }
	string LogFile { get; set; }
	bool AutoExit { get; set; }

	public bool WaitForConnection (TimeSpan ts)
	{
		return connected.WaitOne (ts);
	}

	public void Cancel ()
	{
		try {
			// wait a second just in case more data arrives.
			if (!stopped.WaitOne (TimeSpan.FromSeconds (1))) 
				server.Stop ();
		} catch {
			// We might have stopped already, so just swallow any exceptions.
		}
	}

	public void Initialize ()
	{
		server = new TcpListener (Address, Port);
		server.Start ();

		if (Port == 0)
			Port = ((IPEndPoint) server.LocalEndpoint).Port;

		Console.WriteLine ("Touch.Unit Simple Server listening on: {0}:{1}", Address, Port);
	}
	
	public int Start (bool skipheader = false)
	{
		bool processed;

		try {
			
			do {
				using (TcpClient client = server.AcceptTcpClient ()) {
					processed = Processing (client, skipheader);
				}
			} while (!AutoExit || !processed);
		}
		catch (Exception e) {
			Console.WriteLine ("[{0}] : {1}", DateTime.Now, e);
			return 1;
		}
		finally {
			try {
				server.Stop ();
			} finally {
				stopped.Set ();
			}
		}
		
		return 0;
	}

	public bool Processing (TcpClient client, bool skipHeader = false)
	{
		string logfile = Path.Combine (LogPath, LogFile ?? DateTime.UtcNow.Ticks.ToString () + ".log");
		string remote = client.Client.RemoteEndPoint.ToString ();
		Console.WriteLine ("Connection from {0} saving logs to {1}", remote, logfile);
		connected.Set ();

		using (FileStream fs = File.OpenWrite (logfile)) {
			if (!skipHeader) {
				// a few extra bits of data only available from this side
				string header = String.Format("[Local Date/Time:\t{1}]{0}[Remote Address:\t{2}]{0}",
					Environment.NewLine, DateTime.Now, remote);
				byte[] array = Encoding.UTF8.GetBytes(header);
				fs.Write (array, 0, array.Length);
				fs.Flush ();
			}
			// now simply copy what we receive
			int i;
			int total = 0;
			NetworkStream stream = client.GetStream ();
			while ((i = stream.Read (buffer, 0, buffer.Length)) != 0) {
				fs.Write (buffer, 0, i);
				fs.Flush ();
				total += i;
			}
			
			if (total < 16) {
				// This wasn't a test run, but a connection from the app (on device) to find
				// the ip address we're reachable on.
				return false;
			}
		}
		
		return true;
	}

	static void ShowHelp (OptionSet os)
	{
		Console.WriteLine ("Usage: mono Touch.Server.exe [options]");
		os.WriteOptionDescriptions (Console.Out);
	}

	public static int Main (string[] args)
	{ 
		Console.WriteLine ("Touch.Unit Simple Server");
		Console.WriteLine ("Copyright 2011, Xamarin Inc. All rights reserved.");
		
		bool help = false;
		bool verbose = false;
		IPAddress ipAddress = IPAddress.Any;
		ushort port = 0;
		string log_path = ".";
		string log_file = null;
		string launchdev = null;
		string launchsim = null;
		bool autoexit = false;
		bool skipheader = false;
		string device_name = String.Empty;
		string device_type = String.Empty;
		TimeSpan? timeout = null;
		TimeSpan? startup_timeout = null;
		var mtouch_arguments = new List<string> ();

		var os = new OptionSet () {
			{ "h|?|help", "Display help", v => help = true },
			{ "verbose", "Display verbose output", v => verbose = true },
			{ "ip=", "IP address to listen (default: Any)", v => ipAddress = IPAddress.Parse (v) },
			{ "port=", "TCP port to listen (default: Any)", v => port = ushort.Parse (v) },
			{ "logpath=", "Path to save the log files (default: .)", v => log_path = v },
			{ "logfile=", "Filename to save the log to (default: automatically generated)", v => log_file = v },
			{ "launchdev=", "Run the specified app on a device (specify using bundle identifier)", v => launchdev = v },
			{ "launchsim=", "Run the specified app on the simulator (specify using path to *.app directory)", v => launchsim = v },
			{ "autoexit", "Exit the server once a test run has completed (default: false)", v => autoexit = true },
			{ "skipheader", "Exclude the header from the logfile (default: false)", v => skipheader = true },
			{ "devname=", "Specify the device to connect to", v => device_name = v},
			{ "device=", "Specifies the device type to launch the simulator", v => device_type = v },
			{ "timeout=", "Specifies a timeout (in minutes), after which the simulator app will be killed (ignored for device runs)", v => timeout = TimeSpan.FromMinutes (double.Parse (v)) },
			{ "startup-timeout=", "Specifies a timeout (in seconds) for the simulator app to connect to Touch.Server (ignored for device runs)", v => startup_timeout = TimeSpan.FromSeconds (double.Parse (v)) },
			{ "mtouch-argument=", "Specifies an extra mtouch argument when launching the application", v => mtouch_arguments.Add (v) },
		};
		
		try {
			os.Parse (args);
			if (help)
				ShowHelp (os);
			
			var listener = new SimpleListener ();
			
			listener.Address = ipAddress;
			listener.Port = port;
			listener.LogPath = log_path ?? ".";
			listener.LogFile = log_file;
			listener.AutoExit = autoexit;
			listener.Initialize ();
			
			string mt_root = Environment.GetEnvironmentVariable ("MONOTOUCH_ROOT");
			if (String.IsNullOrEmpty (mt_root))
				mt_root = "/Library/Frameworks/Xamarin.iOS.framework/Versions/Current";

			string mtouch = Path.Combine (mt_root, "bin", "mlaunch");
			if (!File.Exists (mtouch))
				mtouch = Path.Combine (mt_root, "usr", "bin", "mlaunch");

			Process proc = null;
			if (launchdev != null) {
				ThreadPool.QueueUserWorkItem ((v) => {
					{
						proc = new Process ();
						StringBuilder output = new StringBuilder ();
						StringBuilder procArgs = new StringBuilder ();
						string sdk_root = Environment.GetEnvironmentVariable ("XCODE_DEVELOPER_ROOT");
						if (!String.IsNullOrEmpty (sdk_root))
							procArgs.Append ("--sdkroot ").Append (sdk_root);
						procArgs.Append (" --launchdev ");
						procArgs.Append (Quote (launchdev));
						if (!String.IsNullOrEmpty (device_name))
							procArgs.Append (" --devname=").Append (Quote (device_name));
						procArgs.Append (" -argument=-connection-mode -argument=none");
						procArgs.Append (" -argument=-app-arg:-autostart");
						procArgs.Append (" -argument=-app-arg:-autoexit");
						procArgs.Append (" -argument=-app-arg:-enablenetwork");
						procArgs.AppendFormat (" -argument=-app-arg:-hostport:{0}", listener.Port);
						procArgs.Append (" -argument=-app-arg:-hostname:");
						foreach (var arg in mtouch_arguments)
							procArgs.Append (" ").Append (arg);
						var ipAddresses = System.Net.Dns.GetHostEntry (System.Net.Dns.GetHostName ()).AddressList;
						for (int i = 0; i < ipAddresses.Length; i++) {
							if (i > 0)
								procArgs.Append (',');
							procArgs.Append (ipAddresses [i].ToString ());
						}
						proc.StartInfo.FileName = mtouch;
						proc.StartInfo.Arguments = procArgs.ToString ();
						proc.StartInfo.UseShellExecute = false;
						proc.StartInfo.RedirectStandardOutput = true;
						proc.StartInfo.RedirectStandardError = true;
						proc.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e) {
							lock (output) {
								output.Append ("[mtouch stderr] ");
								output.AppendLine (e.Data);
							}
						};
						proc.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e) {
							lock (output) {
								output.Append ("[mtouch stdout] ");
								output.AppendLine (e.Data);
							}
						};
						proc.Start ();
						proc.BeginErrorReadLine ();
						proc.BeginOutputReadLine ();
						proc.WaitForExit ();
						if (proc.ExitCode != 0)
							listener.Cancel ();
						Console.WriteLine (output.ToString ());
					}
				});
			}

			var lastErrorDataReceived = new AutoResetEvent (true);
			var lastOutDataReceived = new AutoResetEvent (true);
			if (launchsim != null) {
				lastErrorDataReceived.Reset ();
				lastOutDataReceived.Reset ();

				ThreadPool.QueueUserWorkItem ((v) => {
					{
						proc = new Process ();
						int pid = 0;
						StringBuilder output = new StringBuilder ();
						StringBuilder procArgs = new StringBuilder ();
						string sdk_root = Environment.GetEnvironmentVariable ("XCODE_DEVELOPER_ROOT");
						if (!String.IsNullOrEmpty (sdk_root))
							procArgs.Append ("--sdkroot ").Append (sdk_root);
						procArgs.Append (" --launchsim ");
						procArgs.Append (Quote (launchsim));
						if (!string.IsNullOrEmpty (device_type))
							procArgs.Append (" --device ").Append (device_type);
						procArgs.Append (" -argument=-connection-mode -argument=none");
						procArgs.Append (" -argument=-app-arg:-autostart");
						procArgs.Append (" -argument=-app-arg:-autoexit");
						procArgs.Append (" -argument=-app-arg:-enablenetwork");
						procArgs.Append (" -argument=-app-arg:-hostname:127.0.0.1");
						procArgs.AppendFormat (" -argument=-app-arg:-hostport:{0}", listener.Port);
						foreach (var arg in mtouch_arguments)
							procArgs.Append (" ").Append (arg);
						proc.StartInfo.FileName = mtouch;
						proc.StartInfo.Arguments = procArgs.ToString ();
						proc.StartInfo.UseShellExecute = false;
						proc.StartInfo.RedirectStandardError = true;
						proc.StartInfo.RedirectStandardOutput = true;
						proc.StartInfo.RedirectStandardInput = true;

						proc.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e) {
							if (e.Data == null) {
								Console.WriteLine ("[mtouch stderr EOS]");
								lastErrorDataReceived.Set ();
								return;
							}
							Console.WriteLine ("[mtouch stderr {0}] {1}", DateTime.Now.ToLongTimeString (), e.Data);
						};
						proc.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e) {
							if (e.Data == null){
								Console.WriteLine ("[mtouch stdout EOS]");
								lastOutDataReceived.Set ();
								return;
							}
							Console.WriteLine ("[mtouch stdout {0}] {1}", DateTime.Now.ToLongTimeString (), e.Data);

							if (e.Data.StartsWith ("Application launched. PID = ")) {
								var pidstr = e.Data.Substring ("Application launched. PID = ".Length);
								if (!int.TryParse (pidstr, out pid)) {
									Console.WriteLine ("Could not parse pid: {0}", pidstr);
								} else if (startup_timeout.HasValue) {
									ThreadPool.QueueUserWorkItem ((v2) =>
										{
											if (!listener.WaitForConnection (startup_timeout.Value))
												KillPid (proc, pid, 1000, startup_timeout.Value, "Startup");
										});
								}
							}
						};
						if (verbose)
							Console.WriteLine ("{0} {1}", proc.StartInfo.FileName, proc.StartInfo.Arguments);
						proc.Start ();
						proc.BeginErrorReadLine ();
						proc.BeginOutputReadLine ();
						if (timeout.HasValue) {
							if (!proc.WaitForExit ((int) timeout.Value.TotalMilliseconds)) {
								if (pid != 0) {
									KillPid (proc, pid, 5000, timeout.Value, "Completion");
								} else {
									proc.StandardInput.WriteLine (); // this kills as well, but we won't be able to send SIGQUIT to get a stack trace.
								}
								if (!proc.WaitForExit (5000 /* wait another 5 seconds for mtouch to finish as well */))
									Console.WriteLine ("mtouch didn't complete within 5s of killing the simulator app. Touch.Server will exit anyway.");
							}
						} else {
							proc.WaitForExit ();
						}
						listener.Cancel ();
					}
				});
			}
			
			var result = listener.Start (skipheader);
			if (proc != null && !proc.WaitForExit (30000 /* wait another 30 seconds for mtouch to finish as well */))
				Console.WriteLine ("mtouch didn't complete within 30s of the simulator app exiting. Touch.Server will exit anyway.");
			// Wait up to 2 seconds to receive the last of the error/output data. This will only be received *after*
			// mtouch has exited.
			lastErrorDataReceived.WaitOne (2000);
			lastOutDataReceived.WaitOne (2000);
			return result;
		} catch (OptionException oe) {
			Console.WriteLine ("{0} for options '{1}'", oe.Message, oe.OptionName);
			return 1;
		} catch (Exception ex) {
			Console.WriteLine (ex);
			return 1;
		}
	}   

	static void KillPid (Process proc, int pid, int kill_separation, TimeSpan timeout, string type)
	{
		Console.WriteLine ("{2} timeout ({1} s) reached, will now send SIGQUIT to the app (PID: {0})", pid, timeout.TotalSeconds, type);
		kill (pid, 3 /* SIGQUIT */); // print managed stack traces.
		if (!proc.WaitForExit (kill_separation /* wait for at most 5 seconds to see if something happens */)) {
			Console.WriteLine ("{2} timeout ({1} s) reached, will now send SIGABRT to the app (PID: {0})", pid, timeout.TotalSeconds, type);
			kill (pid, 6 /* SIGABRT */); // print native stack traces.
			if (!proc.WaitForExit (kill_separation /* wait another 5 seconds */)) {
				Console.WriteLine ("{2} timeout ({1} s) reached, will now send SIGKILL to the app (PID: {0})", pid, timeout.TotalSeconds, type);
				kill (pid, 9 /* SIGKILL */); // terminate unconditionally.
			}
		}
	}

	static string Quote (string f)
	{
		if (f.IndexOf (' ') == -1 && f.IndexOf ('\'') == -1)
			return f;

		var s = new StringBuilder ();

		s.Append ('"');
		foreach (var c in f){
			if (c == '"' || c == '\\')
				s.Append ('\\');

			s.Append (c);
		}
		s.Append ('"');

		return s.ToString ();
	}

   [DllImport ("libc")]
   private static extern void kill (int pid, int sig);
}
