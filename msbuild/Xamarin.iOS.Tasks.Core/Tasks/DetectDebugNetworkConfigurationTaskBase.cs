using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;
using Xamarin.MacDev;

namespace Xamarin.iOS.Tasks
{
	public abstract class DetectDebugNetworkConfigurationBase : Task
	{
		#region Inputs

		public string SessionId { get; set; }

		[Required]
		public bool DebugOverWiFi { get; set; }

		public string DebuggerHosts { get; set; }

		[Required]
		public bool SdkIsSimulator { get; set; }

		#endregion

		#region Outputs

		[Output]
		public string DebugIPAddresses { get; set; }

		#endregion


		public override bool Execute ()
		{
			Log.LogTaskName ("DetectDebugNetworkConfiguration");
			Log.LogTaskProperty ("DebugOverWiFi", DebugOverWiFi);
			Log.LogTaskProperty ("DebuggerHosts", DebuggerHosts);
			Log.LogTaskProperty ("SdkIsSimulator", SdkIsSimulator);

			var ips = new List<string> ();

			if (SdkIsSimulator) {
				ips.Add (IPAddress.Loopback.ToString ());
			} else if (DebugOverWiFi) {
				string [] hosts = null;

				if (!string.IsNullOrEmpty (DebuggerHosts))
					hosts = DebuggerHosts.Split (new [] { ';' }, StringSplitOptions.RemoveEmptyEntries);

				if (hosts == null || hosts.Length == 0) {
					try {
						ips.AddRange (Dns.GetHostEntry (Dns.GetHostName ()).AddressList.Select ((v) => v.ToString ()));
					} catch {
						Log.LogError ("Could not resolve host IPs for WiFi debugger settings");
						return false;
					}
				} else {
					foreach (var host in hosts) {
						IPAddress ip;

						if (IPAddress.TryParse (host, out ip))
							ips.Add (ip.ToString ());
					}
				}

				if (ips == null || ips.Count == 0) {
					Log.LogError ("This machine does not have any network adapters. This is required when debugging or profiling on device over WiFi.");
					return false;
				}
			}

			DebugIPAddresses = string.Join (";", ips.ToArray ());

			Log.LogTaskProperty ("DebugIPAddresses", DebugIPAddresses);

			return !Log.HasLoggedErrors;
		}
	}
}
