using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Localization.MSBuild;
using Xamarin.MacDev.Models;
using Xamarin.Messaging.Build.Client;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks;

public class GetAvailableDevices : XamarinTask, ICancelableTask {
	// The app manifest is optional, we won't compile it if it hasn't already been compiled,
	// but we'll read it if it exist (in the app bundle).
	public string AppBundleManifestPath { get; set; } = string.Empty;

	[Output]
	public ITaskItem [] Devices { get; set; } = Array.Empty<ITaskItem> ();

	[Output]
	public ITaskItem [] DiscardedDevices { get; set; } = Array.Empty<ITaskItem> ();

	public string RuntimeIdentifier { get; set; } = "";

	public bool Verbose { get; set; }

	CancellationTokenSource? cancellationTokenSource;

	public override bool Execute ()
	{
		if (ShouldExecuteRemotely ())
			return ExecuteRemotely ();

		cancellationTokenSource = new CancellationTokenSource ();

		// run devicectl and simctl in parallel
		var devicectlTask = RunDeviceCtlAsync ();
		var simctlTask = RunSimCtlAsync ();

		System.Threading.Tasks.Task.WhenAll (new [] { devicectlTask, simctlTask }).Wait (cancellationTokenSource.Token);

		var devices = new List<DeviceInfo> ();
		var simulators = simctlTask.Result.ToList ();
		var simulatorUdids = new HashSet<string> (simulators.Select (d => d.Udid), StringComparer.OrdinalIgnoreCase);
		foreach (var device in devicectlTask.Result) {
			if (!simulatorUdids.Contains (device.Udid))
				devices.Add (device);
		}
		devices.AddRange (simulators);

		// filter to the current platform
		foreach (var d in devices.Where (d => !d.Discarded && d.Platform != Platform))
			d.DiscardedReason = $"Device platform '{d.Platform.AsString ()}' does not match the requested platform '{Platform.AsString ()}'";

		// filter to the current RuntimeIdentifier, if specified
		if (!string.IsNullOrEmpty (RuntimeIdentifier)) {
			foreach (var d in devices.Where (d => string.IsNullOrEmpty (d.DiscardedReason) && d.RuntimeIdentifiers.Any () && !d.RuntimeIdentifiers.Contains (RuntimeIdentifier)))
				d.DiscardedReason = $"Device runtime identifier(s) '{string.Join ("', '", d.RuntimeIdentifiers)}' incompatible with the requested runtime identifier '{RuntimeIdentifier}'";
		}

		// filter to values we find in the app manifest, if it exists
		if (File.Exists (AppBundleManifestPath)) {
			var appManifest = PDictionary.OpenFile (AppBundleManifestPath);

			var uiDeviceFamily = appManifest.GetUIDeviceFamily ();
			// an iPhone app can run on an iPad, but an iPad app cannot run on an iPhone
			var onlyShowIPads = uiDeviceFamily == IPhoneDeviceType.IPad;
			if (onlyShowIPads) {
				foreach (var d in devices.Where (d => !d.Discarded && d.DeviceType != IPhoneDeviceType.IPad))
					d.DiscardedReason = "Device is not an iPad, but the app only supports iPads";
			}

			string minimumOSVersionString;
			switch (Platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				minimumOSVersionString = appManifest.GetMinimumOSVersion ();
				break;
			case ApplePlatform.MacCatalyst:
			case ApplePlatform.MacOSX:
				minimumOSVersionString = appManifest.GetMinimumSystemVersion ();
				break;
			default:
				throw new InvalidOperationException (string.Format (MSBStrings.InvalidPlatform, Platform));
			}
			if (Version.TryParse (minimumOSVersionString, out var minimumOSVersion)) {
				foreach (var d in devices.Where (d => !d.Discarded && d.MinimumOSVersion < minimumOSVersion))
					d.DiscardedReason = $"Device OS version '{d.MinimumOSVersion}' is lower than the app's minimum OS version '{minimumOSVersion}'";
			}
		}

		// Set the 'RuntimeIdentifier' metadata on every device
		foreach (var d in devices) {
			if (!d.RuntimeIdentifiers.Any ())
				continue;
			d.Item.SetMetadata ("RuntimeIdentifier", d.RuntimeIdentifiers.Single ());
		}

		DiscardedDevices = devices.Where (d => d.Discarded).Select (v => {
			v.Item.SetMetadata ("DiscardedReason", v.DiscardedReason);
			return v.Item;
		}).ToArray ();

		// sort the devices, so we return them in a stable order
		// Booted simulators come first so the interactive device selection shows running simulators at the top.
		var sortedDevices = devices
			.Where (d => !d.Discarded) // discard discarded devices
			.OrderByDescending (d => d.Item.GetMetadata ("State") == "Booted") // booted simulators first
			.ThenByDescending (d => d.MinimumOSVersion) // newer devices first (probably has duplicates)
			.ThenBy (d => d.Item.GetMetadata ("Name")) // then sort by name (may have duplicates)
			.ThenBy (d => d.Item.ItemSpec); // and finally by UDID, which should be unique

		Devices = sortedDevices.Select (d => d.Item).ToArray ();

		if (Verbose) {
			foreach (var dd in DiscardedDevices)
				Log.LogWarning ("Discarded '{0}' because: {1}", dd.ItemSpec, dd.GetMetadata ("DiscardedReason"));
		}

		return true;
	}

	class DeviceInfo {
		public ITaskItem Item { get; set; }
		public string Udid { get; set; }
		public IEnumerable<string> RuntimeIdentifiers { get; set; }
		public ApplePlatform Platform { get; set; }
		public IPhoneDeviceType DeviceType { get; set; }
		public Version MinimumOSVersion { get; set; }
		public string DiscardedReason { get; set; }
		public bool Discarded { get => !string.IsNullOrEmpty (DiscardedReason); }
		public DeviceInfo (ITaskItem item, string udid, IEnumerable<string> runtimeIdentifiers, ApplePlatform platform, IPhoneDeviceType deviceType, Version minimumOSVersion, string discardedReason)
		{
			Item = item;
			Udid = udid;
			RuntimeIdentifiers = runtimeIdentifiers;
			Platform = platform;
			DeviceType = deviceType;
			MinimumOSVersion = minimumOSVersion;
			DiscardedReason = discardedReason;
		}
	}

	protected virtual async System.Threading.Tasks.Task<string> ExecuteCtlAsync (params string [] args)
	{
		var tmpfile = Path.GetTempFileName ();
		try {
			var arguments = new List<string> (args) {
				"--json-output=" + tmpfile
			};
			await ExecuteAsync ("xcrun", arguments, cancellationToken: cancellationTokenSource!.Token);
			return File.ReadAllText (tmpfile);
		} finally {
			File.Delete (tmpfile);
		}
	}

	async System.Threading.Tasks.Task<IEnumerable<DeviceInfo>> RunDeviceCtlAsync ()
	{
		var json = await ExecuteCtlAsync ("devicectl", "list", "devices");
		var rv = new List<DeviceInfo> ();

		// Use shared parser from Xamarin.MacDev for devicectl JSON extraction
		var parsedDevices = DeviceCtlOutputParser.ParseDevices (json);

		foreach (var device in parsedDevices) {
			var udid = device.Udid;

			if (string.IsNullOrEmpty (udid))
				udid = $"<unknown udid #{rv.Count + 1}>";

			var item = new TaskItem (udid);
			item.SetMetadata ("Name", device.Name);
			item.SetMetadata ("BuildVersion", device.BuildVersion);
			item.SetMetadata ("DeviceClass", device.DeviceClass);
			item.SetMetadata ("HardwareModel", device.HardwareModel);
			item.SetMetadata ("Platform", device.Platform);
			item.SetMetadata ("ProductType", device.ProductType);
			item.SetMetadata ("SerialNumber", device.SerialNumber);
			item.SetMetadata ("UniqueChipID", device.UniqueChipID?.ToString () ?? string.Empty);
			item.SetMetadata ("CPUArchitecture", device.CpuArchitecture);
			item.SetMetadata ("TransportType", device.TransportType);
			item.SetMetadata ("PairingState", device.PairingState);

			// we provide the following metadata for both simulator and device
			var deviceDescription = device.Name;
			if (!string.IsNullOrEmpty (device.OSVersion) && !deviceDescription.Contains (device.OSVersion))
				deviceDescription = $"{deviceDescription} - {device.Platform} {device.OSVersion}";
			item.SetMetadata ("Description", deviceDescription);
			item.SetMetadata ("Type", "Device");
			item.SetMetadata ("OSVersion", device.OSVersion);
			item.SetMetadata ("UDID", udid);
			// Capitalize the first letter of PairingState for the Status metadata.
			// Default to "Unpaired" when devicectl doesn't report a pairing state.
			var pairingState = device.PairingState;
			if (string.IsNullOrEmpty (pairingState))
				pairingState = "unpaired";
			item.SetMetadata ("Status", char.ToUpperInvariant (pairingState [0]) + pairingState.Substring (1));

			// compute the platform and runtime identifier
			var runtimeIdentifier = "";
			ApplePlatform platform;
			IPhoneDeviceType deviceType;
			var discardedReason = "";
			switch (device.DeviceClass.ToLowerInvariant ()) {
			case "iphone":
			case "ipod":
				runtimeIdentifier += "ios-";
				platform = ApplePlatform.iOS;
				deviceType = IPhoneDeviceType.IPhone;
				break;
			case "ipad":
				runtimeIdentifier += "ios-";
				platform = ApplePlatform.iOS;
				deviceType = IPhoneDeviceType.IPad;
				break;
			case "appletv":
				runtimeIdentifier += "tvos-";
				platform = ApplePlatform.TVOS;
				deviceType = IPhoneDeviceType.TV;
				break;
			case "applewatch":
			case "visionos":
			default:
				platform = ApplePlatform.None;
				deviceType = IPhoneDeviceType.NotSet;
				discardedReason = $"'{device.DeviceClass}' devices are not supported";
				break;
			}

			if (string.IsNullOrEmpty (discardedReason)) {
				switch (device.CpuArchitecture.ToLowerInvariant ()) {
				case "arm64":
				case "arm64e":
					// arm64 and arm64e are both arm64 for our purposes
					runtimeIdentifier += "arm64";
					break;
				default:
					discardedReason = $"Unknown CPU architecture '{device.CpuArchitecture}'";
					break;
				}
			}

			Version.TryParse (device.OSVersion, out var minimumOSVersion);

			rv.Add (new DeviceInfo (item, udid, [runtimeIdentifier], platform, deviceType, minimumOSVersion ?? new Version (0, 0), discardedReason));
		}
		return rv;
	}

	async System.Threading.Tasks.Task<IEnumerable<DeviceInfo>> RunSimCtlAsync ()
	{
		var json = await ExecuteCtlAsync ("simctl", "list", "--json");
		var rv = new List<DeviceInfo> ();

		// Use shared parser from Xamarin.MacDev for device and runtime extraction
		var parsedDevices = SimctlOutputParser.ParseDevices (json);
		var parsedRuntimes = SimctlOutputParser.ParseRuntimes (json);

		// Index runtimes by identifier for SupportedArchitectures lookup
		var runtimesByIdentifier = new Dictionary<string, SimulatorRuntimeInfo> ();
		foreach (var rt in parsedRuntimes)
			runtimesByIdentifier [rt.Identifier] = rt;

		// Use shared parser for devicetypes (productFamily, min/maxRuntime)
		var parsedDeviceTypes = SimctlOutputParser.ParseDeviceTypes (json);
		var deviceTypes = new Dictionary<string, SimulatorDeviceTypeInfo> ();
		foreach (var dt in parsedDeviceTypes)
			deviceTypes [dt.Identifier] = dt;

		foreach (var device in parsedDevices) {
			var hasRuntime = runtimesByIdentifier.TryGetValue (device.RuntimeIdentifier, out var runtimeInfo);
			var runtimePlatform = hasRuntime ? runtimeInfo!.Platform : string.Empty;
			var runtimeVersion = hasRuntime ? runtimeInfo!.Version : device.OSVersion;
			var supportedArchitectures = hasRuntime ? runtimeInfo!.SupportedArchitectures : new List<string> ();

			var item = new TaskItem (device.Udid);
			item.SetMetadata ("Runtime", device.RuntimeIdentifier);
			item.SetMetadata ("IsAvailable", device.IsAvailable.ToString ());
			item.SetMetadata ("AvailabilityError", device.AvailabilityError);
			item.SetMetadata ("DeviceTypeIdentifier", device.DeviceTypeIdentifier);
			item.SetMetadata ("State", device.State);
			item.SetMetadata ("Name", device.Name);
			item.SetMetadata ("SupportedArchitectures", string.Join (",", supportedArchitectures));

			// we provide the following metadata for both simulator and device
			var description = device.Name;
			if (!string.IsNullOrEmpty (runtimeVersion) && !description.Contains (runtimeVersion)) {
				if (!string.IsNullOrEmpty (runtimePlatform))
					description = $"{description} - {runtimePlatform} {runtimeVersion}";
				else
					description = $"{description} - {runtimeVersion}";
			}
			if (!string.IsNullOrEmpty (device.State))
				description = $"{description} ({device.State})";
			item.SetMetadata ("Description", description);
			item.SetMetadata ("Type", "Simulator");
			item.SetMetadata ("OSVersion", runtimeVersion);
			item.SetMetadata ("UDID", device.Udid);
			item.SetMetadata ("Status", device.State);

			var discardedReason = "";
			var runtimeIdentifier = "";
			var runtimeIdentifiers = new List<string> ();
			if (device.IsAvailable) {
				switch (runtimePlatform.ToLowerInvariant ()) {
				case "ios":
					runtimeIdentifier += "iossimulator-";
					break;
				case "tvos":
					runtimeIdentifier += "tvossimulator-";
					break;
				default:
					discardedReason = $"'{runtimePlatform}' simulators are not supported";
					break;
				}

				// pick the first architecture as the simulator architecture
				if (string.IsNullOrEmpty (discardedReason)) {
					foreach (var arch in supportedArchitectures) {
						switch (arch.ToLowerInvariant ()) {
						case "x64":
						case "x86_64":
							break;
						case "arm64":
							runtimeIdentifiers.Add (runtimeIdentifier + "arm64");
							if (!CanRunArm64)
								discardedReason = $"Can't run an arm64 simulator on an x86_64 macOS desktop.";
							break;
						default:
							discardedReason = $"Unknown CPU architecture '{arch}'";
							break;
						}
					}
					if (runtimeIdentifiers.Count == 0 && string.IsNullOrEmpty (discardedReason))
						discardedReason = MSBStrings.M0170;
				}
			} else {
				discardedReason = $"Device is not available: {device.AvailabilityError}";
			}

			var platform = ApplePlatform.None;
			if (string.IsNullOrEmpty (discardedReason)) {
				switch (device.Platform.ToLowerInvariant ()) {
				case "ios":
					platform = ApplePlatform.iOS;
					break;
				case "tvos":
					platform = ApplePlatform.TVOS;
					break;
				case "watchos":
				case "visionos":
				default:
					discardedReason = $"'{device.Platform}' simulators are not supported";
					break;
				}
			}
			var deviceType = IPhoneDeviceType.NotSet;
			var minimumOSVersion = new Version (0, 0);
			if (string.IsNullOrEmpty (discardedReason)) {
				if (deviceTypes.TryGetValue (device.DeviceTypeIdentifier, out var deviceTypeInfo)) {
					switch (deviceTypeInfo.ProductFamily.ToLowerInvariant ()) {
					case "iphone":
					case "ipod":
						deviceType = IPhoneDeviceType.IPhone;
						break;
					case "ipad":
						deviceType = IPhoneDeviceType.IPad;
						break;
					case "appletv":
					case "apple tv":
						deviceType = IPhoneDeviceType.TV;
						break;
					default:
						discardedReason = $"Unknown product family '{deviceTypeInfo.ProductFamily}'";
						break;
					}
					if (Version.TryParse (runtimeVersion, out var parsedMinimumOSVersion))
						minimumOSVersion = parsedMinimumOSVersion;
				} else {
					discardedReason = $"Unknown device type identifier '{device.DeviceTypeIdentifier}'";
				}
			}

			rv.Add (new DeviceInfo (item, device.Udid, runtimeIdentifiers, platform, deviceType, minimumOSVersion, discardedReason));
		}
		return rv;
	}

	public void Cancel ()
	{
		if (ShouldExecuteRemotely ())
			BuildConnection.CancelAsync (BuildEngine4).Wait ();

		cancellationTokenSource?.Cancel ();
	}

	// Return true if the current machine can run ARM64 binaries.
	static bool? canRunArm64;
	public static bool CanRunArm64 {
		get {
			if (!canRunArm64.HasValue) {
				int rv = 0;
				IntPtr size = (IntPtr) sizeof (int);
				if (sysctlbyname ("hw.optional.arm64", ref rv, ref size, IntPtr.Zero, IntPtr.Zero) == 0) {
					canRunArm64 = rv == 1;
				} else {
					canRunArm64 = false;
				}
			}
			return canRunArm64.Value;
		}
	}

	[DllImport ("libc")]
	static extern int sysctlbyname (string name, ref int value, ref IntPtr size, IntPtr zero, IntPtr zeroAgain);
}
