using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Xharness.Collections;
using Xharness.Execution;
using Xharness.Execution.Mlaunch;
using Xharness.Logging;
using Xharness.Utilities;

namespace Xharness.Hardware {
	
	public interface IDeviceLoaderFactory {
		IDeviceLoader CreateLoader ();
	}

	public class DeviceLoaderFactory : IDeviceLoaderFactory {
		readonly IHarness harness;
		readonly IProcessManager processManager;

		public DeviceLoaderFactory (IHarness harness, IProcessManager processManager)
		{
			this.harness = harness ?? throw new ArgumentNullException (nameof (harness));
			this.processManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
		}

		public IDeviceLoader CreateLoader () => new Devices (harness, processManager);
	}

	public class Devices : IDeviceLoader {
		readonly IProcessManager processManager;
		bool loaded;

		public IHarness Harness { get; set; }

		BlockingEnumerableCollection<IHardwareDevice> connected_devices = new BlockingEnumerableCollection<IHardwareDevice> ();

		public IEnumerable<IHardwareDevice> ConnectedDevices => connected_devices;
		public IEnumerable<IHardwareDevice> Connected64BitIOS => connected_devices.Where (x => x.DevicePlatform == DevicePlatform.iOS && x.Supports64Bit);
		public IEnumerable<IHardwareDevice> Connected32BitIOS => connected_devices.Where (x => x.DevicePlatform == DevicePlatform.iOS && x.Supports32Bit);
		public IEnumerable<IHardwareDevice> ConnectedTV => connected_devices.Where (x => x.DevicePlatform == DevicePlatform.tvOS);
		public IEnumerable<IHardwareDevice> ConnectedWatch => connected_devices.Where (x => x.DevicePlatform == DevicePlatform.watchOS && x.Architecture == Architecture.ARMv7k);
		public IEnumerable<IHardwareDevice> ConnectedWatch32_64 {
			get {
				return connected_devices.Where ((x) => {
					return x.DevicePlatform == DevicePlatform.watchOS && x.Architecture == Architecture.ARM64_32;
				});
			}
		}

		public Devices (IHarness harness, IProcessManager processManager)
		{
			Harness = harness ?? throw new ArgumentNullException (nameof (harness));
			this.processManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
		}

		Task ILoadAsync.LoadAsync (ILog log, bool include_locked, bool force)
		{
			return LoadAsync (log, extra_data: false, removed_locked: !include_locked, force: force);
		}

		Device GetDevice (XmlNode deviceNone)
		{
			// get data, if we are missing some of them, we will return null, happens sometimes that we 
			// have some empty nodes. We could do this with try/catch, but we want to throw the min amount
			// of exceptions. We do know that we will have issues with the parsing of the DeviceClass, check
			// the value, and if is there, get the rest, else return null
			var usable = deviceNone.SelectSingleNode ("IsUsableForDebugging")?.InnerText;
			if (Enum.TryParse<DeviceClass> (deviceNone.SelectSingleNode ("DeviceClass")?.InnerText, true, out var deviceClass)) { 
				var device = new Device {
					DeviceIdentifier = deviceNone.SelectSingleNode ("DeviceIdentifier")?.InnerText,
					DeviceClass = deviceClass,
					CompanionIdentifier = deviceNone.SelectSingleNode ("CompanionIdentifier")?.InnerText,
					Name = deviceNone.SelectSingleNode ("Name")?.InnerText,
					BuildVersion = deviceNone.SelectSingleNode ("BuildVersion")?.InnerText,
					ProductVersion = deviceNone.SelectSingleNode ("ProductVersion")?.InnerText,
					ProductType = deviceNone.SelectSingleNode ("ProductType")?.InnerText,
					InterfaceType = deviceNone.SelectSingleNode ("InterfaceType")?.InnerText,
					IsUsableForDebugging = usable == null ? (bool?) null : ((bool?) (usable == "True")),
				};
				bool.TryParse (deviceNone.SelectSingleNode ("IsLocked")?.InnerText, out var locked);
				device.IsLocked = locked;
				return device;
			} else {
				return null;
			}
		}

		public async Task LoadAsync (ILog log, bool extra_data = false, bool removed_locked = false, bool force = false)
		{
			if (loaded) {
				if (!force)
					return;
				connected_devices.Reset ();
			}

			loaded = true;

			await Task.Run (async () => {
				var tmpfile = Path.GetTempFileName ();
				try {
					using (var process = new Process ()) {
						process.StartInfo.FileName = Harness.MlaunchPath;
						var arguments = new MlaunchArguments (
							new SdkRootArgument (Harness.XcodeRoot),
							new ListDevicesArgument (tmpfile),
							new XmlOutputFormatArgument ());

						if (extra_data)
							arguments.Add (new ListExtraDataArgument ());

						log.WriteLine ("Launching {0} {1}", process.StartInfo.FileName, process.StartInfo.Arguments);
						var rv = await processManager.RunAsync (process, arguments, log, timeout: TimeSpan.FromSeconds (120));
						if (!rv.Succeeded)
							throw new Exception ("Failed to list devices.");
						log.WriteLine ("Result:");
						log.WriteLine (File.ReadAllText (tmpfile));
						log.Flush ();

						var doc = new XmlDocument ();
						doc.LoadWithoutNetworkAccess (tmpfile);

						foreach (XmlNode dev in doc.SelectNodes ("/MTouch/Device")) {
							var d = GetDevice (dev);
							if (d == null)
								continue;
							if (removed_locked && d.IsLocked) {
								log.WriteLine ($"Skipping device {d.Name} ({d.DeviceIdentifier}) because it's locked.");
								continue;
							}
							if (d.IsUsableForDebugging.HasValue && !d.IsUsableForDebugging.Value) {
								log.WriteLine ($"Skipping device {d.Name} ({d.DeviceIdentifier}) because it's not usable for debugging.");
								continue;
							}
							connected_devices.Add (d);
						}
					}
				} finally {
					connected_devices.SetCompleted ();
					File.Delete (tmpfile);
					log.Flush ();
				}
			});
		}

		public IHardwareDevice FindCompanionDevice (ILog log, IHardwareDevice device)
		{
			var companion = ConnectedDevices.Where ((v) => v.DeviceIdentifier == device.CompanionIdentifier);
			if (companion.Count () == 0)
				throw new Exception ($"Could not find the companion device for '{device.Name}'");

			if (companion.Count () > 1)
				log.WriteLine ("Found {0} companion devices for {1}?!?", companion.Count (), device.Name);

			return companion.First ();
		}
	}
}
