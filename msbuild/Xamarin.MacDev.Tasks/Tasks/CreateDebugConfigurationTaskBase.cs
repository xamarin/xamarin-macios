using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.MacDev.Tasks {
	public abstract class CreateDebugConfigurationTaskBase : XamarinTask {
		#region Inputs

		[Required]
		public string AppBundleDir { get; set; }

		[Required]
		public bool DebugOverWiFi { get; set; }

		public string DebugIPAddresses { get; set; }

		[Required]
		public string DebuggerPort { get; set; }

		[Required]
		public bool SdkIsSimulator { get; set; }

		public string ConnectTimeout { get; set; }
		#endregion

		public override bool Execute ()
		{
			var ips = DebugIPAddresses?.Split (new char [] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			var path = Path.Combine (AppBundleDir, "MonoTouchDebugConfiguration.txt");
			var added = new HashSet<string> ();
			var builder = new StringBuilder ();

			if (ips is not null) {
				foreach (var ip in ips) {
					if (added.Contains (ip))
						continue;

					builder.Append ("IP: ");
					builder.AppendLine (ip);
					added.Add (ip);
				}
			}

			if (!DebugOverWiFi && !SdkIsSimulator)
				builder.AppendLine ("USB Debugging: 1");

			builder.Append ("Port: ");
			builder.AppendLine (DebuggerPort);

			if (!string.IsNullOrEmpty (ConnectTimeout)) {
				builder.Append ("Connect Timeout: ");
				builder.AppendLine (ConnectTimeout);
			}

			var text = builder.ToString ();

			try {
				if (!File.Exists (path) || File.ReadAllText (path) != text)
					File.WriteAllText (path, text);
			} catch (Exception ex) {
				Log.LogError (ex.Message);
			}

			return !Log.HasLoggedErrors;
		}
	}
}
