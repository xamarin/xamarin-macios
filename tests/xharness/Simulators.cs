using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace xharness
{
	public class Simulators
	{
		public Harness Harness;

		public List<SimRuntime> SupportedRuntimes = new List<SimRuntime> ();
		public List<SimDeviceType> SupportedDeviceTypes = new List<SimDeviceType> ();
		public List<SimDevice> AvailableDevices = new List<SimDevice> ();
		public List<SimDevicePair> AvailableDevicePairs = new List<SimDevicePair> ();

		public async Task LoadAsync (LogFile log)
		{
			if (SupportedRuntimes.Count > 0)
				return;
			
			var tmpfile = Path.GetTempFileName ();
			try {
				using (var process = new Process ()) {
					process.StartInfo.FileName = Harness.MlaunchPath;
					process.StartInfo.Arguments = string.Format ("--sdkroot {0} --listsim {1}", Harness.XcodeRoot, tmpfile);
					log.WriteLine ("Launching {0} {1}", process.StartInfo.FileName, process.StartInfo.Arguments);
					await process.RunAsync (log.Path, false);
					log.WriteLine ("Result:");
					log.WriteLine (File.ReadAllText (tmpfile));
					var simulator_data = new XmlDocument ();
					simulator_data.LoadWithoutNetworkAccess (tmpfile);
					foreach (XmlNode sim in simulator_data.SelectNodes ("/MTouch/Simulator/SupportedRuntimes/SimRuntime")) {
						SupportedRuntimes.Add (new SimRuntime ()
						{
							Name = sim.SelectSingleNode ("Name").InnerText,
							Identifier = sim.SelectSingleNode ("Identifier").InnerText,
							Version = long.Parse (sim.SelectSingleNode ("Version").InnerText),
						});
					}
					foreach (XmlNode sim in simulator_data.SelectNodes ("/MTouch/Simulator/SupportedDeviceTypes/SimDeviceType")) {
						SupportedDeviceTypes.Add (new SimDeviceType ()
						{
							Name = sim.SelectSingleNode ("Name").InnerText,
							Identifier = sim.SelectSingleNode ("Identifier").InnerText,
							ProductFamilyId = sim.SelectSingleNode ("ProductFamilyId").InnerText,
							MinRuntimeVersion = long.Parse (sim.SelectSingleNode ("MinRuntimeVersion").InnerText),
							MaxRuntimeVersion = long.Parse (sim.SelectSingleNode ("MaxRuntimeVersion").InnerText),
							Supports64Bits = bool.Parse (sim.SelectSingleNode ("Supports64Bits").InnerText),
						});
					}
					foreach (XmlNode sim in simulator_data.SelectNodes ("/MTouch/Simulator/AvailableDevices/SimDevice")) {
						AvailableDevices.Add (new SimDevice ()
						{
							Name = sim.Attributes ["Name"].Value,
							UDID = sim.Attributes ["UDID"].Value,
							SimRuntime = sim.SelectSingleNode ("SimRuntime").InnerText,
							SimDeviceType = sim.SelectSingleNode ("SimDeviceType").InnerText,
							DataPath = sim.SelectSingleNode ("DataPath").InnerText,
							LogPath = sim.SelectSingleNode ("LogPath").InnerText,
						});
					}
					foreach (XmlNode sim in simulator_data.SelectNodes ("/MTouch/Simulator/AvailableDevicePairs/SimDevicePair")) {
						AvailableDevicePairs.Add (new SimDevicePair ()
						{
							UDID = sim.Attributes ["UDID"].Value,
							Companion = sim.SelectSingleNode ("Companion").InnerText,
							Gizmo = sim.SelectSingleNode ("Gizmo").InnerText,

						});
					}
				}
			} finally {
				File.Delete (tmpfile);
			}
		}
	}

	public class SimRuntime
	{
		public string Name;
		public string Identifier;
		public long Version;
	}

	public class SimDeviceType
	{
		public string Name;
		public string Identifier;
		public string ProductFamilyId;
		public long MinRuntimeVersion;
		public long MaxRuntimeVersion;
		public bool Supports64Bits;
	}

	public class SimDevice
	{
		public string UDID;
		public string Name;
		public string SimRuntime;
		public string SimDeviceType;
		public string DataPath;
		public string LogPath;

		public string SystemLog { get { return Path.Combine (LogPath, "system.log"); } }
	}

	public class SimDevicePair
	{
		public string UDID;
		public string Companion;
		public string Gizmo;
	}

	public class Devices
	{
		public Harness Harness;

		public List<Device> ConnectedDevices = new List<Device> ();

		public async Task LoadAsync ()
		{
			if (ConnectedDevices.Count > 0)
				return;

			var tmpfile = Path.GetTempFileName ();
			try {
				using (var process = new Process ()) {
					process.StartInfo.FileName = Harness.MlaunchPath;
					process.StartInfo.Arguments = string.Format ("--sdkroot {0} --listdev={1} --output-format=xml", Harness.XcodeRoot, tmpfile);
					await process.RunAsync (tmpfile, false);

					var doc = new XmlDocument ();
					doc.LoadWithoutNetworkAccess (tmpfile);

					foreach (XmlNode dev in doc.SelectNodes ("/MTouch/Device")) {
						ConnectedDevices.Add (new Device ()
						{
							DeviceIdentifier = dev.SelectSingleNode ("DeviceIdentifier")?.InnerText,
							DeviceClass = dev.SelectSingleNode ("DeviceClass")?.InnerText,
							CompanionIdentifier = dev.SelectSingleNode ("CompanionIdentifier")?.InnerText,
							Name = dev.SelectSingleNode ("Name")?.InnerText,
						});
					}
				}
			} finally {
				File.Delete (tmpfile);
			}
		}
	}

	public class Device
	{
		public string DeviceIdentifier;
		public string DeviceClass;
		public string CompanionIdentifier;
		public string Name;
	}
}

