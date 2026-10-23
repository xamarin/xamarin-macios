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

		public string AppManifestPath { get; set; } = string.Empty;

		public ITaskItem [] AdditionalArguments { get; set; } = Array.Empty<ITaskItem> ();
		public string DeviceName { get; set; } = string.Empty;
		public ITaskItem [] EnvironmentVariables { get; set; } = Array.Empty<ITaskItem> ();
		public string Help { get; set; } = string.Empty;
		public string LaunchApp { get; set; } = string.Empty;
		public string InstallApp { get; set; } = string.Empty;
		public bool CaptureOutput { get; set; } // Set to true to capture output. If StandardOutput|ErrorPath is not set, write to the current terminal's stdout/stderr (requires WaitForExit)
		public string StandardOutputPath { get; set; } = string.Empty; // Set to a path to capture output there
		public string StandardErrorPath { get; set; } = string.Empty;// Set to a path to capture output there
		public string SupportedOSPlatformVersion { get; set; } = string.Empty;
		public bool WaitForExit { get; set; } // Required for capturing stdout/stderr output

		[Required]
		public string MlaunchPath { get; set; } = string.Empty;

		public ITaskItem [] Devices { get; set; } = Array.Empty<ITaskItem> ();
		public ITaskItem [] DiscardedDevices { get; set; } = Array.Empty<ITaskItem> ();

		[Output]
		public string MlaunchArguments { get; set; } = string.Empty;

		public IPhoneDeviceType DeviceType {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
					var plist = PDictionary.OpenFile (AppManifestPath);
					return plist.GetUIDeviceFamily ();
				default:
					throw new InvalidOperationException (string.Format (MSBStrings.InvalidPlatform, Platform));
				}
			}
		}

		sealed class SimulatorDeviceInfo {
			public string Identifier { get; set; } = string.Empty;
			public string Name { get; set; } = string.Empty;
			public long RuntimeVersion { get; set; }
			public int DeviceTypeOrder { get; set; } = int.MinValue;
			public bool IsCompatible { get; set; }
			public string? NotApplicableBecause { get; set; }
		}

		List<(long Min, long Max, string Identifier)>? GetDeviceTypes ()
		{
			var output = GetSimulatorList ();
			if (string.IsNullOrEmpty (output))
				return null;

			// Which product family are we looking for?
			string [] productFamilies;
			switch (DeviceType) {
			case IPhoneDeviceType.IPhone: // if we're looking for an iPhone, an iPad also works
				productFamilies = ["iPhone", "iPad"];
				break;
			case IPhoneDeviceType.IPad:
				productFamilies = ["iPad"];
				break;
			case IPhoneDeviceType.IPhoneAndIPad:
				productFamilies = ["iPhone", "iPad"];
				break;
			case IPhoneDeviceType.TV:
				productFamilies = ["Apple TV"];
				break;
			default:
				throw new InvalidOperationException ($"Invalid device type: {DeviceType}");
			}

			// Load mlaunch's output
			var xml = new XmlDocument ();
			xml.LoadXml (output);
			// Get the device types for the product family we're looking for
			var nodes = xml.SelectNodes ($"/MTouch/Simulator/SupportedDeviceTypes/SimDeviceType[{string.Join (" or ", productFamilies.Select (v => $"ProductFamilyId='{v}'"))}]")?.Cast<XmlNode> () ?? Array.Empty<XmlNode> ();
			// Create a list of them all
			var deviceTypes = new List<(long Min, long Max, string Identifier)> ();
			foreach (var node in nodes) {
				var minRuntimeVersionValue = node.SelectSingleNode ("MinRuntimeVersion")?.InnerText ?? string.Empty;
				var maxRuntimeVersionValue = node.SelectSingleNode ("MaxRuntimeVersion")?.InnerText ?? string.Empty;
				var identifier = node.SelectSingleNode ("Identifier")?.InnerText ?? string.Empty;
				if (!long.TryParse (minRuntimeVersionValue, out var minRuntimeVersion))
					continue;
				if (!long.TryParse (maxRuntimeVersionValue, out var maxRuntimeVersion))
					continue;
				deviceTypes.Add ((minRuntimeVersion, maxRuntimeVersion, identifier));
			}

			deviceTypes.Sort ((a, b) => {
				var rv = a.Min.CompareTo (b.Min);
				if (rv != 0)
					return rv;
				rv = a.Max.CompareTo (b.Max);
				if (rv != 0)
					return rv;
				return StringComparer.Ordinal.Compare (a.Identifier, b.Identifier);
			});

			return deviceTypes;
		}

		static bool TryGetSimulatorVersion (long versionValue, out Version? version)
		{
			if (versionValue <= 0) {
				version = null;
				return false;
			}

			var major = (int) ((versionValue >> 16) & 0xFF);
			var minor = (int) ((versionValue >> 8) & 0xFF);
			version = new Version (major, minor);
			return true;
		}

		string? simulator_list;
		string? GetSimulatorList ()
		{
			if (simulator_list is null) {
				var tmpfile = Path.GetTempFileName ();
				try {
					var output = new StringBuilder ();
					var result = ExecuteAsync (MlaunchPath, new string [] { "--listsim", tmpfile }).Result;
					if (result.ExitCode != 0)
						return string.Empty;
					simulator_list = File.ReadAllText (tmpfile);
				} finally {
					File.Delete (tmpfile);
				}
			}
			return simulator_list;
		}

		string? device_list;
		string? GetDeviceList ()
		{
			if (device_list is null) {
				var tmpfile = Path.GetTempFileName ();
				try {
					var output = new StringBuilder ();
					var result = ExecuteAsync (MlaunchPath, new string [] { $"--listdev:{tmpfile}", "--output-format:xml", "--use-amdevice:false" }).Result;
					if (result.ExitCode != 0)
						return string.Empty;
					device_list = File.ReadAllText (tmpfile);
				} finally {
					File.Delete (tmpfile);
				}
			}
			return device_list;
		}

		List<SimulatorDeviceInfo> GetSimulatorDevices ()
		{
			var rv = new List<SimulatorDeviceInfo> ();

			var output = GetSimulatorList ();
			if (string.IsNullOrEmpty (output))
				return rv;

			var deviceTypes = GetDeviceTypes ();
			if (deviceTypes is null)
				return rv;

			var deviceTypeOrders = deviceTypes
				.Select ((v, index) => (v.Identifier, Index: index))
				.ToDictionary (v => v.Identifier, v => v.Index, StringComparer.Ordinal);

			var xml = new XmlDocument ();
			xml.LoadXml (output);

			var runtimePrefix = $"com.apple.CoreSimulator.SimRuntime.{PlatformName}-";
			var runtimeVersions = new Dictionary<string, long> (StringComparer.Ordinal);
			var runtimeNodes = xml.SelectNodes ("/MTouch/Simulator/SupportedRuntimes/SimRuntime")?.Cast<XmlNode> () ?? Array.Empty<XmlNode> ();
			foreach (var node in runtimeNodes) {
				var identifier = node.SelectSingleNode ("Identifier")?.InnerText ?? string.Empty;
				var versionValue = node.SelectSingleNode ("Version")?.InnerText ?? string.Empty;
				if (long.TryParse (versionValue, out var version))
					runtimeVersions [identifier] = version;
			}

			var nodes = xml.SelectNodes ($"/MTouch/Simulator/AvailableDevices/SimDevice")?.Cast<XmlNode> () ?? Array.Empty<XmlNode> ();
			foreach (var node in nodes) {
				var device = new SimulatorDeviceInfo {
					Identifier = node.Attributes? ["UDID"]?.Value ?? string.Empty,
					Name = node.Attributes? ["Name"]?.Value ?? string.Empty,
				};

				var simDeviceType = node.SelectSingleNode ("SimDeviceType")?.InnerText ?? string.Empty;
				var simRuntime = node.SelectSingleNode ("SimRuntime")?.InnerText ?? string.Empty;
				runtimeVersions.TryGetValue (simRuntime, out var simRuntimeVersion);
				device.RuntimeVersion = simRuntimeVersion;

				string? notApplicableBecause = null;
				if (!simRuntime.StartsWith (runtimePrefix, StringComparison.Ordinal)) {
					notApplicableBecause = $" [Simulator runtime ({simRuntime}) does not match the requested platform ({PlatformName}) for this app]";
				} else if (!deviceTypeOrders.TryGetValue (simDeviceType, out var deviceTypeOrder)) {
					notApplicableBecause = $" [Simulator device type ({simDeviceType}) is not applicable for this app]";
				} else {
					device.IsCompatible = true;
					device.DeviceTypeOrder = deviceTypeOrder;
					if (Version.TryParse (SupportedOSPlatformVersion, out var supportedOSPlatformVersion) && TryGetSimulatorVersion (simRuntimeVersion, out var simRuntimeVersionValue) && simRuntimeVersionValue is not null && simRuntimeVersionValue < supportedOSPlatformVersion)
						notApplicableBecause = $" [OS version ({simRuntimeVersionValue}) lower than minimum supported platform version ({SupportedOSPlatformVersion}) for this app]";
				}

				device.NotApplicableBecause = notApplicableBecause;
				rv.Add (device);
			}

			return rv;
		}

		string SelectSimulatorDevice ()
		{
			var simulator = GetTaskItemsOfType (Devices, "Simulator").FirstOrDefault ();
			if (simulator is null) {
				var sb = new StringBuilder ();
				sb.AppendLine ("The 'Devices' item group does not contain any simulators.");
				AppendDiscardedDevices (sb, "", "Simulator");
				Log.LogError (sb.ToString ().TrimEnd ());
				return "";
			}

			return GetDeviceIdentifier (simulator);
		}

		List<(string Identifier, string Name, string? NotApplicableBecause)> GetDeviceListForSimulator ()
		{
			if (Devices.Length > 0 || DiscardedDevices.Length > 0)
				return GetDevicesFromTaskItems ("Simulator", Devices);

			return GetSimulatorDevices ()
				.Where (v => v.IsCompatible)
				.OrderByDescending (v => v.RuntimeVersion)
				.ThenByDescending (v => v.DeviceTypeOrder)
				.ThenBy (v => v.Name, StringComparer.Ordinal)
				.ThenBy (v => v.Identifier, StringComparer.Ordinal)
				.Select (v => (v.Identifier, v.Name, v.NotApplicableBecause))
				.ToList ();
		}

		List<(string Identifier, string Name, string? NotApplicableBecause)> GetDeviceListForDevice ()
		{
			if (Devices.Length > 0 || DiscardedDevices.Length > 0)
				return GetDevicesFromTaskItems ("Device", Devices);

			var rv = new List<(string Identifier, string Name, string? NotApplicableBecause)> ();

			var output = GetDeviceList ();
			if (string.IsNullOrEmpty (output))
				return rv;

			// Which product family are we looking for?
			string deviceClassCondition;
			switch (DeviceType) {
			case IPhoneDeviceType.TV:
				deviceClassCondition = "[DeviceClass='AppleTV']";
				break;
			case IPhoneDeviceType.IPad:
				deviceClassCondition = "[DeviceClass='iPad']";
				break;
			case IPhoneDeviceType.IPhone:
			case IPhoneDeviceType.IPhoneAndIPad:
				deviceClassCondition = "[DeviceClass='iPhone' or DeviceClass='iPad']";
				break;
			default:
				throw new InvalidOperationException ($"Invalid device type: {DeviceType}");
			}

			// Load mlaunch's output
			var xml = new XmlDocument ();
			xml.LoadXml (output);
			// Get the device types for the device classes we're looking for
			var nodes = xml.SelectNodes ($"/MTouch/Device{deviceClassCondition}")?.Cast<XmlNode> () ?? Array.Empty<XmlNode> ();
			foreach (var node in nodes) {
				var deviceIdentifier = node.SelectSingleNode ("DeviceIdentifier")?.InnerText ?? string.Empty;
				var name = node.SelectSingleNode ("Name")?.InnerText ?? string.Empty;
				var productVersionString = node.SelectSingleNode ("ProductVersion")?.InnerText;

				string? notApplicableBecause = null;
				if (Version.TryParse (productVersionString, out var productVersion) && Version.TryParse (SupportedOSPlatformVersion, out var supportedOSPlatformVersion) && productVersion < supportedOSPlatformVersion)
					notApplicableBecause = $" [OS version ({productVersionString}) lower than minimum supported platform version ({SupportedOSPlatformVersion}) for this app]";
				rv.Add ((deviceIdentifier, name, notApplicableBecause));
			}
			return rv;
		}

		static string GetDeviceIdentifier (ITaskItem device)
		{
			var udid = device.GetMetadata ("UDID");
			return string.IsNullOrEmpty (udid) ? device.ItemSpec : udid;
		}

		static string GetDeviceName (ITaskItem device)
		{
			var name = device.GetMetadata ("Name");
			if (string.IsNullOrEmpty (name))
				name = device.GetMetadata ("Description");
			return string.IsNullOrEmpty (name) ? GetDeviceIdentifier (device) : name;
		}

		static string FormatDevice (ITaskItem device)
		{
			var identifier = GetDeviceIdentifier (device);
			var name = GetDeviceName (device);
			return name == identifier ? identifier : $"{name} ({identifier})";
		}

		static ITaskItem [] GetTaskItemsOfType (ITaskItem [] items, string type)
		{
			return items
				.Where (v => string.Equals (v.GetMetadata ("Type"), type, StringComparison.OrdinalIgnoreCase))
				.ToArray ();
		}

		string GetApplicableDeviceType ()
		{
			return SdkIsSimulator ? "Simulator" : "Device";
		}

		void FilterTaskItemInputs ()
		{
			var type = GetApplicableDeviceType ();
			Devices = GetTaskItemsOfType (Devices, type);
			DiscardedDevices = GetTaskItemsOfType (DiscardedDevices, type);
		}

		static List<(string Identifier, string Name, string? NotApplicableBecause)> GetDevicesFromTaskItems (string type, ITaskItem [] items)
		{
			return items
				.Where (v => string.Equals (v.GetMetadata ("Type"), type, StringComparison.OrdinalIgnoreCase))
				.Select (v => {
					var reason = v.GetMetadata ("DiscardedReason");
					return (GetDeviceIdentifier (v), GetDeviceName (v), string.IsNullOrEmpty (reason) ? null : reason);
				})
				.ToList ();
		}

		void AppendDiscardedDevices (StringBuilder sb, string indent, string? type = null)
		{
			var discardedDevices = DiscardedDevices
				.Where (v => type is null || string.Equals (v.GetMetadata ("Type"), type, StringComparison.OrdinalIgnoreCase))
				.ToArray ();

			if (discardedDevices.Length == 0)
				return;

			sb.AppendLine ($"{indent}The following devices were discarded:");
			foreach (var device in discardedDevices) {
				var reason = device.GetMetadata ("DiscardedReason");
				if (string.IsNullOrEmpty (reason)) {
					sb.AppendLine ($"{indent}    {FormatDevice (device)}");
				} else {
					sb.AppendLine ($"{indent}    {FormatDevice (device)}: {reason}");
				}
			}
		}

		void LogNoAvailableDevicesError ()
		{
			var sb = new StringBuilder ();
			sb.AppendLine ("No applicable and available devices found.");
			AppendDiscardedDevices (sb, "", GetApplicableDeviceType ());
			Log.LogError (sb.ToString ().TrimEnd ());
		}

		protected string GenerateCommandLineCommands ()
		{
			var sb = new List<string> ();
			string? selectedSimulator = null;
			var deviceName = DeviceName;

			if (!string.IsNullOrEmpty (LaunchApp)) {
				sb.Add (SdkIsSimulator ? "--launchsim" : "--launchdev");
				sb.Add (LaunchApp);
			}

			if (!string.IsNullOrEmpty (InstallApp)) {
				sb.Add (SdkIsSimulator ? "--installsim" : "--installdev");
				sb.Add (InstallApp);
			}

			if (SdkIsSimulator && string.IsNullOrEmpty (deviceName)) {
				selectedSimulator = SelectSimulatorDevice ();
				deviceName = selectedSimulator;
			}

			if (!string.IsNullOrEmpty (deviceName)) {
				if (SdkIsSimulator) {
					sb.Add ("--device");

					if (!string.IsNullOrEmpty (selectedSimulator)) {
						sb.Add ($":v2:udid={selectedSimulator}");
					} else {
						// Figure out whether we got the exact name of a simulator, in which case construct the corresponding argument.
						string? simulator = null;
						var deviceList = GetDeviceListForSimulator ();
						var simulatorsByIdentifier = deviceList.Where (v => v.Identifier == deviceName).ToArray ();
						if (simulatorsByIdentifier.Length == 1) {
							simulator = simulatorsByIdentifier [0].Identifier;
						} else {
							var simulatorsByName = deviceList.Where (v => v.Name == deviceName).ToArray ();
							if (simulatorsByName.Length == 1)
								simulator = simulatorsByName [0].Identifier;
						}
						if (!string.IsNullOrEmpty (simulator)) {
							sb.Add ($":v2:udid={simulator}");
						} else {
							sb.Add (deviceName);
						}
					}
				} else {
					sb.Add ("--devname");
					sb.Add (deviceName);
				}
			}

			if (CaptureOutput && string.IsNullOrEmpty (StandardOutputPath))
				StandardOutputPath = GetTerminalName (1);

			if (CaptureOutput && string.IsNullOrEmpty (StandardErrorPath))
				StandardErrorPath = GetTerminalName (2);

			if (!string.IsNullOrEmpty (StandardOutputPath)) {
				sb.Add ("--stdout");
				sb.Add (StandardOutputPath);
			}

			if (!string.IsNullOrEmpty (StandardErrorPath)) {
				sb.Add ("--stderr");
				sb.Add (StandardErrorPath);
			}

			foreach (var envvar in EnvironmentVariables) {
				var hasValue = envvar.MetadataNames.Cast<string> ().Contains ("Value");
				if (hasValue) {
					var value = envvar.GetMetadata ("Value");
					sb.Add ("--setenv=" + envvar.ItemSpec + "=" + value);

				} else {
					sb.Add ("--setenv=" + envvar.ItemSpec);
				}
			}

			sb.Add (WaitForExit ? "--wait-for-exit:true" : "--wait-for-exit:false");

			// Add additional arguments at the end, so they can override any
			// other argument.
			foreach (var arg in AdditionalArguments)
				sb.Add (arg.ItemSpec);

			return StringUtils.FormatArguments (sb);
		}

		static string GetTerminalName (int fd)
		{
			if (isatty (fd) != 1)
				return string.Empty;

			return Marshal.PtrToStringAuto (ttyname (fd)) ?? string.Empty;
		}

		void ShowHelp ()
		{
			var sb = new StringBuilder ();
			var f = $"net{TargetFramework.Version}-{Platform.AsString ().ToLower ()}";
			var rid = Platform == ApplePlatform.TVOS ? "tvos-arm64" : "ios-arm64";

			sb.AppendLine ($"");
			sb.AppendLine ($"To run on physical device:");
			sb.AppendLine ($"    1. If the project has multiple target frameworks, select the desired target framework. Example: -f {f}");
			sb.AppendLine ($"    2. Pass a RuntimeIdentifier for a device. Example: -p:{rid}");
			sb.AppendLine ($"    3. Pass the name or identifier of the target device using '-p:DeviceName=<name or identifier of device>'");
			var devices = GetDeviceListForDevice ();
			if (devices.Count == 0) {
				sb.AppendLine ($"        There are no devices connected to this Mac that can be used to run this app.");
			} else {
				sb.AppendLine ($"        There are {devices.Count} device(s) connected to this Mac that can be used to run this app:");
				foreach (var d in devices)
					sb.AppendLine ($"            {d.Name} ({d.Identifier}) {d.NotApplicableBecause}");
				var firstDevice = devices.First ();
				sb.AppendLine ($"        Example: -p:DeviceName={firstDevice.Identifier} or -p:DeviceName={StringUtils.Quote (firstDevice.Name)}");
				sb.AppendLine ($"    For example:");
				var sampleDevice = firstDevice.Name == StringUtils.Quote (firstDevice.Name) ? firstDevice.Name : firstDevice.Identifier;
				sb.AppendLine ($"        dotnet run -f {f} -r {rid} -p:DeviceName={sampleDevice}");
			}
			AppendDiscardedDevices (sb, "        ", "Device");

			sb.AppendLine ($"");
			sb.AppendLine ($"To run in a simulator:");
			sb.AppendLine ($"    1. If the project has multiple target frameworks, select the desired target framework. Exmaple: -f {f}");
			sb.AppendLine ($"    2. Pass the name or identifier of the target simulator using '-p:DeviceName=<name or identifier of simulator>'");
			var simulators = GetDeviceListForSimulator ();
			if (simulators.Count == 0) {
				sb.AppendLine ($"        There are no simulators available that can be used to run this app. Please open Xcode, then the menu Window -> Devices and Simulators, select Simulators on the top left, and create a new simulator clicking on the plus sign on the bottom left.");
			} else {
				sb.AppendLine ($"        There are {simulators.Count} simulators(s) on this Mac that can be used to run this app:");
				foreach (var s in simulators)
					sb.AppendLine ($"            {s.Name} ({s.Identifier}) {s.NotApplicableBecause}");
				var firstSim = simulators.First ();
				sb.AppendLine ($"        Example: -p:DeviceName={firstSim.Identifier} or -p:DeviceName={StringUtils.Quote (firstSim.Name)}");
				sb.AppendLine ($"    For example:");
				var sampleDevice = firstSim.Name == StringUtils.Quote (firstSim.Name) ? firstSim.Name : firstSim.Identifier;
				sb.AppendLine ($"        dotnet run -f {f} -p:DeviceName={sampleDevice}");
			}
			AppendDiscardedDevices (sb, "        ", "Simulator");
			sb.AppendLine ();

			// Sadly the only way to have the help show up in the terminal reliably is to make it a warning
			Log.LogWarning (sb.ToString ());
		}

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return ExecuteRemotely ();

			if (!string.IsNullOrEmpty (Help)) {
				ShowHelp ();
				return !Log.HasLoggedErrors;
			}

			FilterTaskItemInputs ();
			if (Devices.Length == 0 && string.IsNullOrEmpty (DeviceName)) {
				LogNoAvailableDevicesError ();
				return false;
			}

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
