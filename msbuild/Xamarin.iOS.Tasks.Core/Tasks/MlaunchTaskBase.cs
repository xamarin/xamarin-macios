using System;
using System.IO;
using System.Runtime.InteropServices;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev;
using Xamarin.MacDev.Tasks;
using Xamarin.Utils;

namespace Xamarin.iOS.Tasks {
	public abstract class MlaunchTaskBase : XamarinToolTask {

		[Required]
		public string ToolsDirectory { get; set; }

		[Required]
		public bool SdkIsSimulator { get; set; }

		[Required]
		public string SdkVersion { get; set; }

		[Required]
		public string AppBundlePath { get; set; }

		public string DeviceName { get; set; }
		public string LaunchApp { get; set; }
		public string InstallApp { get; set; }
		public bool CaptureOutput { get; set; } // Set to true to capture output. If StandardOutput|ErrorPath is not set, write to the current terminal's stdout/stderr (requires WaitForExit)
		public string StandardOutputPath { get; set; } // Set to a path to capture output there
		public string StandardErrorPath { get; set; } // Set to a path to capture output there
		public bool WaitForExit { get; set; } // Required for capturing stdout/stderr output

		public IPhoneDeviceType DeviceType {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
					var plist = PDictionary.FromFile (GetAppManifest (AppBundlePath));
					return plist.GetUIDeviceFamily ();
				default:
					throw new NotImplementedException ($"Unknown platform: {Platform}"); // FIXME: better error
				}
			}
		}

		protected override string ToolName => "mlaunch";
		protected override string GenerateFullPathToTool ()
		{
			return Path.Combine (ToolsDirectory, "bin", ToolName);
		}

		protected override string GenerateCommandLineCommands ()
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
				string simdevicetype;

				// FIXME: the device types should be dynamically computed somehow. Either add that support to mlaunch, or get a list of possible device types using simctl or mlaunch.
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
				case ApplePlatform.MacOSX:
					throw new PlatformNotSupportedException ($"Invalid platform: {Platform}"); // FIXME: better error
				default:
					throw new NotImplementedException ($"Unknown platform: {Platform}"); // FIXME: better error
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

			if (WaitForExit)
				sb.Add ("--wait-for-exit");

			return sb.ToString ();
		}

		static string GetTerminalName (int fd)
		{
			if (isatty (fd) != 1)
				return null;

			return Marshal.PtrToStringAuto (ttyname (fd));
		}


		[DllImport ("/usr/lib/libc.dylib")]
		extern static IntPtr ttyname (int filedes);

		[DllImport ("/usr/lib/libc.dylib")]
		extern static int isatty (int fd);
	}
}
