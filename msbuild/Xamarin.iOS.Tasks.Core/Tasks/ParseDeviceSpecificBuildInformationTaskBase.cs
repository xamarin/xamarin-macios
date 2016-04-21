using System;
using System.IO;

using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

using Xamarin.MacDev;
using Xamarin.MacDev.Tasks;

namespace Xamarin.iOS.Tasks
{
	public abstract class ParseDeviceSpecificBuildInformationTaskBase : Task
	{
		#region Inputs

		public string SessionId { get; set; }

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
			PDictionary plist, device;
			PString value;

			Log.LogTaskName ("ParseDeviceSpecificBuildInformation");
			Log.LogTaskProperty ("Architectures", Architectures);
			Log.LogTaskProperty ("IntermediateOutputPath", IntermediateOutputPath);
			Log.LogTaskProperty ("OutputPath", OutputPath);
			Log.LogTaskProperty ("TargetiOSDevice", TargetiOSDevice);

			if (!Enum.TryParse (Architectures, out architectures)) {
				Log.LogError ("Invalid architectures: '{0}'.", Architectures);
				return false;
			}

			if ((plist = PObject.FromString (TargetiOSDevice) as PDictionary) == null) {
				Log.LogError ("Failed to parse the target iOS device information.");
				return false;
			}

			if (!plist.TryGetValue ("device", out device)) {
				Log.LogError ("No target device found.");
				return false;
			}

			if (!device.TryGetValue ("architecture", out value)) {
				Log.LogError ("No device architecture information found.");
				return false;
			}

			if (!Enum.TryParse (value.Value, out deviceArchitectures) || deviceArchitectures == TargetArchitecture.Default) {
				Log.LogError ("Invalid target architecture: '{0}'", value.Value);
				return false;
			}

			if ((architectures & deviceArchitectures) == 0) {
				Log.LogError ("The target iOS device architecture {0} is not supported by the build configuration: {1}", architectures, deviceArchitectures);
				return false;
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
				Log.LogError ("No device model information found.");
				return false;
			}

			TargetDeviceModel = value.Value;

			if (!device.TryGetValue ("os-version", out value)) {
				Log.LogError ("No iOS version information found.");
				return false;
			}

			TargetDeviceOSVersion = value.Value;

			// Note: we replace ',' with '.' because the ',' breaks the Mono AOT compiler which tries to treat arguments with ','s in them as options.
			var dirName = "build-" + TargetDeviceModel.ToLowerInvariant ().Replace (",", ".") + "-" + TargetDeviceOSVersion;

			DeviceSpecificIntermediateOutputPath = Path.Combine (IntermediateOutputPath, dirName) + "/";
			DeviceSpecificOutputPath = Path.Combine (OutputPath, dirName) + "/";

			return !Log.HasLoggedErrors;
		}
	}
}
