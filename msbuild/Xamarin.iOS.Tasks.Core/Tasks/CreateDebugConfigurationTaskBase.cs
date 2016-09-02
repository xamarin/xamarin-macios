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

		public string DebugIPAddresses { get; set; }

		[Required]
		public string DebuggerPort { get; set; }

		[Required]
		public bool SdkIsSimulator { get; set; }

		#endregion

		public override bool Execute ()
		{
			var path = Path.Combine (AppBundleDir, "MonoTouchDebugConfiguration.txt");

			Log.LogTaskName ("CreateDebugConfiguration");
			Log.LogTaskProperty ("AppBundleDir", AppBundleDir);
			Log.LogTaskProperty ("DebugOverWiFi", DebugOverWiFi);
			Log.LogTaskProperty ("DebugIPAddresses", DebugIPAddresses);
			Log.LogTaskProperty ("DebuggerPort", DebuggerPort);
			Log.LogTaskProperty ("SdkIsSimulator", SdkIsSimulator);

			var ips = DebugIPAddresses?.Split (new char [] { ';' }, StringSplitOptions.RemoveEmptyEntries);

			try {
				using (var stream = new FileStream (path, FileMode.Create, FileAccess.Write)) {
					using (var writer = new StreamWriter (stream)) {
						var added = new HashSet<string> ();
						if (ips != null) {
							foreach (var ip in ips) {
								string str = ip.ToString ();

								if (added.Contains (str))
									continue;

								writer.WriteLine ("IP: {0}", str);
								added.Add (str);
							}
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
