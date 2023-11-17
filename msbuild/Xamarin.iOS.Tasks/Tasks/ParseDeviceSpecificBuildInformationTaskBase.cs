using System;
using System.IO;

using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

using Xamarin.MacDev;
using Xamarin.MacDev.Tasks;
using Xamarin.Utils;
using Xamarin.Localization.MSBuild;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.iOS.Tasks {
	public abstract class ParseDeviceSpecificBuildInformationTaskBase : XamarinTask {
		#region Inputs

		[Required]
		public string Architectures { get; set; }

		[Required]
		public string IntermediateOutputPath { get; set; }

		[Required]
		public string OutputPath { get; set; }

		[Required]
		public string TargetiOSDevice { get; set; }

		#endregion

		#region Outputs

		[Output]
		public string DeviceSpecificIntermediateOutputPath { get; set; }

		[Output]
		public string DeviceSpecificOutputPath { get; set; }

		[Output]
		public string TargetArchitectures { get; set; }

		[Output]
		public string TargetDeviceModel { get; set; }

		[Output]
		public string TargetDeviceOSVersion { get; set; }

		#endregion

		public override bool Execute ()
		{
			TargetArchitecture architectures, deviceArchitectures, target = TargetArchitecture.Default;
			string targetOperatingSystem;
			PDictionary plist, device;
			PString value, os;

			switch (Platform) {
			case ApplePlatform.WatchOS:
				targetOperatingSystem = "watchOS";
				break;
			case ApplePlatform.TVOS:
				targetOperatingSystem = "tvOS";
				break;
			default:
				targetOperatingSystem = "iOS";
				break;
			}

			if (!Enum.TryParse (Architectures, out architectures)) {
				Log.LogError (MSBStrings.E0057, Architectures);
				return false;
			}

			if ((plist = PObject.FromString (TargetiOSDevice) as PDictionary) is null) {
				Log.LogError (MSBStrings.E0058);
				return false;
			}

			if (!plist.TryGetValue ("device", out device)) {
				Log.LogError (MSBStrings.E0059);
				return false;
			}

			if (!device.TryGetValue ("architecture", out value)) {
				Log.LogError (MSBStrings.E0060);
				return false;
			}

			if (!Enum.TryParse (value.Value, out deviceArchitectures) || deviceArchitectures == TargetArchitecture.Default) {
				Log.LogError (MSBStrings.E0061, value.Value);
				return false;
			}

			if (!device.TryGetValue ("os", out os)) {
				Log.LogError (MSBStrings.E0062);
				return false;
			}

			if (os.Value != targetOperatingSystem || (architectures & deviceArchitectures) == 0) {
				// the TargetiOSDevice property conflicts with the build configuration (*.user file?), do not build this project for a specific device
				DeviceSpecificIntermediateOutputPath = IntermediateOutputPath;
				DeviceSpecificOutputPath = OutputPath;
				TargetArchitectures = Architectures;
				TargetDeviceOSVersion = string.Empty;
				TargetDeviceModel = string.Empty;

				return !Log.HasLoggedErrors;
			}

			for (int bit = 0; bit < 32; bit++) {
				var architecture = (TargetArchitecture) (1 << bit);

				if ((architectures & architecture) == 0)
					continue;

				if ((deviceArchitectures & architecture) != 0)
					target = architecture;
			}

			TargetArchitectures = target.ToString ();

			if (!device.TryGetValue ("model", out value)) {
				Log.LogError (MSBStrings.E0063);
				return false;
			}

			TargetDeviceModel = value.Value;

			if (!device.TryGetValue ("os-version", out value)) {
				Log.LogError (MSBStrings.E0064);
				return false;
			}

			TargetDeviceOSVersion = value.Value;

			// Note: we replace ',' with '.' because the ',' breaks the Mono AOT compiler which tries to treat arguments with ','s in them as options.
			var dirName = TargetDeviceModel.ToLowerInvariant ().Replace (",", ".") + "-" + TargetDeviceOSVersion;

			DeviceSpecificIntermediateOutputPath = Path.Combine (IntermediateOutputPath, "device-builds", dirName) + "/";
			DeviceSpecificOutputPath = Path.Combine (OutputPath, "device-builds", dirName) + "/";

			return !Log.HasLoggedErrors;
		}
	}
}
