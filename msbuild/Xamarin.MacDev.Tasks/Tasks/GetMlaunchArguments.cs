using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;

using Microsoft.Build.Framework;

using Xamarin.Localization.MSBuild;
using Xamarin.MacDev;
using Xamarin.MacDev.Tasks;
using Xamarin.Messaging.Build.Client;
using Xamarin.Utils;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public class GetMlaunchArguments : XamarinTask, ICancelableTask {

		[Required]
		public bool SdkIsSimulator { get; set; }

		[Required]
		public string SdkVersion { get; set; } = string.Empty;

		[Required]
		public string AppBundlePath { get; set; } = string.Empty;

		[Required]
		public string AppManifestPath { get; set; } = string.Empty;

		[Required]
		public string SdkDevPath { get; set; } = string.Empty;

		public ITaskItem [] AdditionalArguments { get; set; } = Array.Empty<ITaskItem> ();
		public string DeviceName { get; set; } = string.Empty;
		public ITaskItem [] EnvironmentVariables { get; set; } = Array.Empty<ITaskItem> ();
		public string LaunchApp { get; set; } = string.Empty;
		public string InstallApp { get; set; } = string.Empty;
		public bool CaptureOutput { get; set; } // Set to true to capture output. If StandardOutput|ErrorPath is not set, write to the current terminal's stdout/stderr (requires WaitForExit)
		public string StandardOutputPath { get; set; } = string.Empty; // Set to a path to capture output there
		public string StandardErrorPath { get; set; } = string.Empty;// Set to a path to capture output there
		public bool WaitForExit { get; set; } // Required for capturing stdout/stderr output

		[Required]
		public string MlaunchPath { get; set; } = string.Empty;

		[Output]
		public string MlaunchArguments { get; set; } = string.Empty;

		public IPhoneDeviceType DeviceType {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
					var plist = PDictionary.FromFile (AppManifestPath);
					return plist.GetUIDeviceFamily ();
				default:
					throw new InvalidOperationException (string.Format (MSBStrings.InvalidPlatform, Platform));
				}
			}
		}

		List<string>? GetDeviceTypes ()
		{
			var tmpfile = Path.GetTempFileName ();
			try {
				var output = new StringBuilder ();
				var result = ExecuteAsync (MlaunchPath, new string [] { "--listsim", tmpfile }, SdkDevPath).Result;
				if (result.ExitCode != 0)
					return null;

				// Which product family are we looking for?
				string productFamily;
				switch (DeviceType) {
				case IPhoneDeviceType.IPhone:
				case IPhoneDeviceType.IPad:
				case IPhoneDeviceType.TV:
				case IPhoneDeviceType.Watch:
					productFamily = DeviceType.ToString ();
					break;
				case IPhoneDeviceType.IPhoneAndIPad:
					productFamily = "IPad";
					break;
				default:
					throw new InvalidOperationException ($"Invalid device type: {DeviceType}");
				}

				// Load mlaunch's output
				var xml = new XmlDocument ();
				xml.Load (tmpfile);
				// Get the device types for the product family we're looking for
				var nodes = xml.SelectNodes ($"/MTouch/Simulator/SupportedDeviceTypes/SimDeviceType[ProductFamilyId='{productFamily}']").Cast<XmlNode> ();
				// Create a list of them all
				var deviceTypes = new List<(long Min, long Max, string Identifier)> ();
				foreach (var node in nodes) {
					var minRuntimeVersionValue = node.SelectSingleNode ("MinRuntimeVersion").InnerText;
					var maxRuntimeVersionValue = node.SelectSingleNode ("MaxRuntimeVersion").InnerText;
					var identifier = node.SelectSingleNode ("Identifier").InnerText;
					if (!long.TryParse (minRuntimeVersionValue, out var minRuntimeVersion))
						continue;
					if (!long.TryParse (maxRuntimeVersionValue, out var maxRuntimeVersion))
						continue;
					deviceTypes.Add ((minRuntimeVersion, maxRuntimeVersion, identifier));
				}
				// Sort by minRuntimeVersion, this is a rudimentary way of sorting so that the last device is at the end.
				deviceTypes.Sort ((a, b) => a.Min.CompareTo (b.Min));
				// Return the sorted list
				return deviceTypes.Select (v => v.Identifier).ToList ();
			} finally {
				File.Delete (tmpfile);
			}
		}

		protected string GenerateCommandLineCommands ()
		{
			var sb = new CommandLineArgumentBuilder ();

			if (!string.IsNullOrEmpty (LaunchApp)) {
				sb.Add (SdkIsSimulator ? "--launchsim" : "--launchdev");
				sb.AddQuoted (LaunchApp);
			}

			if (!string.IsNullOrEmpty (InstallApp)) {
				sb.Add (SdkIsSimulator ? "--installsim" : "--installdev");
				sb.AddQuoted (InstallApp);
			}

			if (SdkIsSimulator && string.IsNullOrEmpty (DeviceName)) {
				var simruntime = $"com.apple.CoreSimulator.SimRuntime.{PlatformName}-{SdkVersion.Replace ('.', '-')}";
				var simdevicetypes = GetDeviceTypes ();
				string simdevicetype;

				if (simdevicetypes?.Count > 0) {
					// Use the latest device type we can find. This seems to be what Xcode does by default.
					simdevicetype = simdevicetypes.Last ();
				} else {
					// We couldn't find any device types, so pick one.
					switch (Platform) {
					case ApplePlatform.iOS:
						// Don't try to launch an iPad-only app on an iPhone
						if (DeviceType == IPhoneDeviceType.IPad) {
							simdevicetype = "com.apple.CoreSimulator.SimDeviceType.iPad--7th-generation-";
						} else {
							simdevicetype = "com.apple.CoreSimulator.SimDeviceType.iPhone-11";
						}
						break;
					case ApplePlatform.TVOS:
						simdevicetype = "com.apple.CoreSimulator.SimDeviceType.Apple-TV-4K-1080p";
						break;
					case ApplePlatform.WatchOS:
						simdevicetype = "com.apple.CoreSimulator.SimDeviceType.Apple-Watch-Series-5-40mm";
						break;
					default:
						throw new InvalidOperationException (string.Format (MSBStrings.InvalidPlatform, Platform));
					}
				}
				DeviceName = $":v2:runtime={simruntime},devicetype={simdevicetype}";
			}

			if (!string.IsNullOrEmpty (DeviceName)) {
				if (SdkIsSimulator) {
					sb.Add ("--device");
				} else {
					sb.Add ("--devname");
				}
				sb.AddQuoted (DeviceName);
			}

			if (CaptureOutput && string.IsNullOrEmpty (StandardOutputPath))
				StandardOutputPath = GetTerminalName (1);

			if (CaptureOutput && string.IsNullOrEmpty (StandardErrorPath))
				StandardErrorPath = GetTerminalName (2);

			if (!string.IsNullOrEmpty (StandardOutputPath)) {
				sb.Add ("--stdout");
				sb.AddQuoted (StandardOutputPath);
			}

			if (!string.IsNullOrEmpty (StandardErrorPath)) {
				sb.Add ("--stderr");
				sb.AddQuoted (StandardErrorPath);
			}

			foreach (var envvar in EnvironmentVariables)
				sb.AddQuoted ("--setenv=" + envvar.ItemSpec);

			sb.Add (WaitForExit ? "--wait-for-exit:true" : "--wait-for-exit:false");

			// Add additional arguments at the end, so they can override any
			// other argument.
			foreach (var arg in AdditionalArguments)
				sb.AddQuoted (arg.ItemSpec);

			return sb.ToString ();
		}

		static string GetTerminalName (int fd)
		{
			if (isatty (fd) != 1)
				return string.Empty;

			return Marshal.PtrToStringAuto (ttyname (fd));
		}

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			MlaunchArguments = GenerateCommandLineCommands ();
			return !Log.HasLoggedErrors;
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}

		[DllImport ("/usr/lib/libc.dylib")]
		extern static IntPtr ttyname (int filedes);

		[DllImport ("/usr/lib/libc.dylib")]
		extern static int isatty (int fd);
	}
}
