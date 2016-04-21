using System;
using System.IO;
using System.Net;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;

namespace Xamarin.iOS.Tasks
{
	public abstract class CreateDebugConfigurationTaskBase : Task
	{
		#region Inputs

		public string SessionId { get; set; }

		[Required]
		public string AppBundleDir { get; set; }

		[Required]
		public bool DebugOverWiFi { get; set; }

		public string DebuggerHosts { get; set; }

		[Required]
		public string DebuggerPort { get; set; }

		[Required]
		public bool SdkIsSimulator { get; set; }

		#endregion

		public override bool Execute ()
		{
			var path = Path.Combine (AppBundleDir, "MonoTouchDebugConfiguration.txt");
			var ips = new List<IPAddress> ();

			Log.LogTaskName ("CreateDebugConfiguration");
			Log.LogTaskProperty ("AppBundleDir", AppBundleDir);
			Log.LogTaskProperty ("DebugOverWiFi", DebugOverWiFi);
			Log.LogTaskProperty ("DebuggerHosts", DebuggerHosts);
			Log.LogTaskProperty ("DebuggerPort", DebuggerPort);
			Log.LogTaskProperty ("SdkIsSimulator", SdkIsSimulator);

			if (SdkIsSimulator) {
				ips.Add (IPAddress.Loopback);
			} else if (DebugOverWiFi) {
				string[] hosts = null;

				if (!string.IsNullOrEmpty (DebuggerHosts))
					hosts = DebuggerHosts.Split (new [] { ';' }, StringSplitOptions.RemoveEmptyEntries);

				if (hosts == null || hosts.Length == 0) {
					try {
						ips.AddRange (Dns.GetHostEntry (Dns.GetHostName ()).AddressList);
					} catch {
						Log.LogError ("Could not resolve host IPs for WiFi debugger settings");
						return false;
					}
				} else {
					foreach (var host in hosts) {
						IPAddress ip;

						if (IPAddress.TryParse (host, out ip))
							ips.Add (ip);
					}
				}

				if (ips == null || ips.Count == 0) {
					Log.LogError ("This machine does not have any network adapters. This is required when debugging or profiling on device over WiFi.");
					return false;
				}
			}

			try {
				using (var stream = new FileStream (path, FileMode.Create, FileAccess.Write)) {
					using (var writer = new StreamWriter (stream)) {
						var added = new HashSet<string> ();
						foreach (var ip in ips) {
							string str = ip.ToString ();

							if (added.Contains (str))
								continue;

							writer.WriteLine ("IP: {0}", str);
							added.Add (str);
						}

						if (!DebugOverWiFi && !SdkIsSimulator)
							writer.WriteLine ("USB Debugging: 1");

						writer.WriteLine ("Port: {0}", DebuggerPort);
					}
				}
			} catch (Exception ex) {
				Log.LogError (ex.Message);
			}

			return !Log.HasLoggedErrors;
		}
	}
}
