using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.DotNet.XHarness.iOS.Shared.Collections;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution.Mlaunch;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Hardware {

	public interface IDeviceLoaderFactory {
		IDeviceLoader CreateLoader ();
	}

	public class DeviceLoaderFactory : IDeviceLoaderFactory {
		readonly IProcessManager processManager;

		public DeviceLoaderFactory (IProcessManager processManager)
		{
			this.processManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
		}

		public IDeviceLoader CreateLoader () => new Devices (processManager);
	}

	public class Devices : IDeviceLoader {
		readonly IProcessManager processManager;
		bool loaded;

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

		public Devices (IProcessManager processManager)
		{
			this.processManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
		}

		Task ILoadAsync.LoadAsync (ILog log, bool include_locked, bool force)
		{
			return LoadAsync (log, extra_data: false, removed_locked: !include_locked, force: force);
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
						var arguments = new MlaunchArguments (
							new ListDevicesArgument (tmpfile),
							new XmlOutputFormatArgument ());

						if (extra_data)
							arguments.Add (new ListExtraDataArgument ());

						var task = processManager.ExecuteCommandAsync (arguments, log, timeout: TimeSpan.FromSeconds (120));
						log.WriteLine ("Launching {0} {1}", process.StartInfo.FileName, process.StartInfo.Arguments);

						var result = await task;

						if (!result.Succeeded)
							throw new Exception ("Failed to list devices.");
						log.WriteLine ("Result:");
						log.WriteLine (File.ReadAllText (tmpfile));
						log.Flush ();

						var doc = new XmlDocument ();
						doc.LoadWithoutNetworkAccess (tmpfile);

						foreach (XmlNode dev in doc.SelectNodes ("/MTouch/Device")) {
							var usable = dev.SelectSingleNode ("IsUsableForDebugging")?.InnerText;
							Device d = new Device {
								DeviceIdentifier = dev.SelectSingleNode ("DeviceIdentifier")?.InnerText,
								DeviceClass = (DeviceClass) Enum.Parse (typeof (DeviceClass), dev.SelectSingleNode ("DeviceClass")?.InnerText, true),
								CompanionIdentifier = dev.SelectSingleNode ("CompanionIdentifier")?.InnerText,
								Name = dev.SelectSingleNode ("Name")?.InnerText,
								BuildVersion = dev.SelectSingleNode ("BuildVersion")?.InnerText,
								ProductVersion = dev.SelectSingleNode ("ProductVersion")?.InnerText,
								ProductType = dev.SelectSingleNode ("ProductType")?.InnerText,
								InterfaceType = dev.SelectSingleNode ("InterfaceType")?.InnerText,
								IsUsableForDebugging = usable == null ? null : (bool?) (usable == "True"),
							};
							bool.TryParse (dev.SelectSingleNode ("IsLocked")?.InnerText, out var locked);
							d.IsLocked = locked;
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
