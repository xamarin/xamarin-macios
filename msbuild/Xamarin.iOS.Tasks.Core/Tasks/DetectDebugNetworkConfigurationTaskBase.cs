using System;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Net.NetworkInformation;

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
			var ips = new List<string> ();

			if (SdkIsSimulator) {
				ips.Add (IPAddress.Loopback.ToString ());
			} else if (DebugOverWiFi) {
				string [] hosts = null;

				if (!string.IsNullOrEmpty (DebuggerHosts))
					hosts = DebuggerHosts.Split (new [] { ';' }, StringSplitOptions.RemoveEmptyEntries);

				if (hosts == null || hosts.Length == 0) {
					var properties = IPGlobalProperties.GetIPGlobalProperties ();
					var hostName = properties.HostName;

					try {
						var entry = Dns.GetHostEntry (hostName);

						ips.AddRange (entry.AddressList.Select (v => v.ToString ()));
					} catch {
						using (var socket = new Socket (SocketType.Dgram, ProtocolType.Udp)) {
							try {
								socket.Connect ("8.8.8.8", 53);

								var ipEndPoint = (IPEndPoint) socket.LocalEndPoint;

								ips.Add (ipEndPoint.Address.ToString ());
							} catch {
								Log.LogError (7001, null, "Could not resolve host IPs for WiFi debugger settings.");
								return false;
							}
						}
					}
				} else {
					foreach (var host in hosts) {
						IPAddress ip;

						if (IPAddress.TryParse (host, out ip))
							ips.Add (ip.ToString ());
					}
				}

				if (ips == null || ips.Count == 0) {
					Log.LogError (7002, null, "This machine does not have any network adapters. This is required when debugging or profiling on device over WiFi.");
					return false;
				}
			}

			DebugIPAddresses = string.Join (";", ips.ToArray ());

			Log.LogTaskProperty ("DebugIPAddresses", DebugIPAddresses);

			return !Log.HasLoggedErrors;
		}
	}
}
