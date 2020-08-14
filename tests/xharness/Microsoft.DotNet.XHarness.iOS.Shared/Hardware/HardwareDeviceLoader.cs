using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.DotNet.XHarness.iOS.Shared.Collections;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution.Mlaunch;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Hardware {

	public interface IHardwareDeviceLoader : IDeviceLoader {
		IEnumerable<IHardwareDevice> ConnectedDevices { get; }
		IEnumerable<IHardwareDevice> Connected64BitIOS { get; }
		IEnumerable<IHardwareDevice> Connected32BitIOS { get; }
		IEnumerable<IHardwareDevice> ConnectedTV { get; }
		IEnumerable<IHardwareDevice> ConnectedWatch { get; }
		IEnumerable<IHardwareDevice> ConnectedWatch32_64 { get; }

		Task<IHardwareDevice> FindCompanionDevice (ILog log, IHardwareDevice device);

		Task<IHardwareDevice> FindDevice (RunMode runMode, ILog log, bool includeLocked, bool force);
	}

	public class HardwareDeviceLoader : IHardwareDeviceLoader {
		readonly SemaphoreSlim semaphore = new SemaphoreSlim (1);
		readonly IProcessManager processManager;
		bool loaded;

		readonly BlockingEnumerableCollection<IHardwareDevice> connectedDevices = new BlockingEnumerableCollection<IHardwareDevice> ();

		public IEnumerable<IHardwareDevice> ConnectedDevices => connectedDevices;
		public IEnumerable<IHardwareDevice> Connected64BitIOS => connectedDevices.Where (x => x.DevicePlatform == DevicePlatform.iOS && x.Supports64Bit);
		public IEnumerable<IHardwareDevice> Connected32BitIOS => connectedDevices.Where (x => x.DevicePlatform == DevicePlatform.iOS && x.Supports32Bit);
		public IEnumerable<IHardwareDevice> ConnectedTV => connectedDevices.Where (x => x.DevicePlatform == DevicePlatform.tvOS);
		public IEnumerable<IHardwareDevice> ConnectedWatch => connectedDevices.Where (x => x.DevicePlatform == DevicePlatform.watchOS && x.Architecture == Architecture.ARMv7k);
		public IEnumerable<IHardwareDevice> ConnectedWatch32_64 => connectedDevices.Where (x => x.DevicePlatform == DevicePlatform.watchOS && x.Architecture == Architecture.ARM64_32);

		public HardwareDeviceLoader (IProcessManager processManager)
		{
			this.processManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
		}

		public async Task LoadDevices (ILog log, bool includeLocked = false, bool forceRefresh = false, bool listExtraData = false)
		{
			await semaphore.WaitAsync ();

			if (loaded) {
				if (!forceRefresh) {
					semaphore.Release ();
					return;
				}
				connectedDevices.Reset ();
			}

			var tmpfile = Path.GetTempFileName ();
			try {
				using (var process = new Process ()) {
					var arguments = new MlaunchArguments (
						new ListDevicesArgument (tmpfile),
						new XmlOutputFormatArgument ());

					if (listExtraData)
						arguments.Add (new ListExtraDataArgument ());

					var task = processManager.RunAsync (process, arguments, log, timeout: TimeSpan.FromSeconds (120));
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
						var d = GetDevice (dev);
						if (d == null)
							continue;
						if (!includeLocked && d.IsLocked) {
							log.WriteLine ($"Skipping device {d.Name} ({d.DeviceIdentifier}) because it's locked.");
							continue;
						}
						if (d.IsUsableForDebugging.HasValue && !d.IsUsableForDebugging.Value) {
							log.WriteLine ($"Skipping device {d.Name} ({d.DeviceIdentifier}) because it's not usable for debugging.");
							continue;
						}
						connectedDevices.Add (d);
					}

					loaded = true;
				}
			} finally {
				connectedDevices.SetCompleted ();
				File.Delete (tmpfile);
				log.Flush ();
				semaphore.Release ();
			}
		}

		public async Task<IHardwareDevice> FindDevice (RunMode runMode, ILog log, bool includeLocked, bool force)
		{
			DeviceClass [] deviceClasses = runMode switch
			{
				RunMode.iOS => new [] { DeviceClass.iPhone, DeviceClass.iPad, DeviceClass.iPod },
				RunMode.WatchOS => new [] { DeviceClass.Watch },
				RunMode.TvOS => new [] { DeviceClass.AppleTV },// Untested
				_ => throw new ArgumentException (nameof (runMode)),
			};

			await LoadDevices (log, false, false);

			IEnumerable<IHardwareDevice> compatibleDevices = ConnectedDevices.Where (v => deviceClasses.Contains (v.DeviceClass) && v.IsUsableForDebugging != false);
			IHardwareDevice device;
			if (compatibleDevices.Count () == 0) {
				throw new NoDeviceFoundException ($"Could not find any applicable devices with device class(es): {string.Join (", ", deviceClasses)}");
			} else if (compatibleDevices.Count () > 1) {
				device = compatibleDevices
					.OrderBy (dev => Version.TryParse (dev.ProductVersion, out Version v) ? v : new Version ())
					.First ();

				log.WriteLine ("Found {0} devices for device class(es) '{1}': '{2}'. Selected: '{3}' (because it has the lowest version).",
					compatibleDevices.Count (),
					string.Join ("', '", deviceClasses),
					string.Join ("', '", compatibleDevices.Select ((v) => v.Name).ToArray ()),
					device.Name);
			} else {
				device = compatibleDevices.First ();
			}

			return device;
		}

		public async Task<IHardwareDevice> FindCompanionDevice (ILog log, IHardwareDevice device)
		{
			await LoadDevices (log, false, false);

			var companion = ConnectedDevices.Where ((v) => v.DeviceIdentifier == device.CompanionIdentifier);
			if (companion.Count () == 0)
				throw new Exception ($"Could not find the companion device for '{device.Name}'");

			if (companion.Count () > 1)
				log.WriteLine ("Found {0} companion devices for {1}?!?", companion.Count (), device.Name);

			return companion.First ();
		}

		Device GetDevice (XmlNode deviceNone)
		{
			// get data, if we are missing some of them, we will return null, happens sometimes that we 
			// have some empty nodes. We could do this with try/catch, but we want to throw the min amount
			// of exceptions. We do know that we will have issues with the parsing of the DeviceClass, check
			// the value, and if is there, get the rest, else return null
			var usable = deviceNone.SelectSingleNode ("IsUsableForDebugging")?.InnerText;
			if (Enum.TryParse<DeviceClass> (deviceNone.SelectSingleNode ("DeviceClass")?.InnerText, true, out var deviceClass)) {
				return new Device (
					deviceIdentifier: deviceNone.SelectSingleNode ("DeviceIdentifier")?.InnerText,
					deviceClass: deviceClass,
					companionIdentifier: deviceNone.SelectSingleNode ("CompanionIdentifier")?.InnerText,
					name: deviceNone.SelectSingleNode ("Name")?.InnerText,
					buildVersion: deviceNone.SelectSingleNode ("BuildVersion")?.InnerText,
					productVersion: deviceNone.SelectSingleNode ("ProductVersion")?.InnerText,
					productType: deviceNone.SelectSingleNode ("ProductType")?.InnerText,
					interfaceType: deviceNone.SelectSingleNode ("InterfaceType")?.InnerText,
					isUsableForDebugging: usable == null ? (bool?) null : usable == "True",
					isLocked: bool.TryParse (deviceNone.SelectSingleNode ("IsLocked")?.InnerText, out var locked) && locked);
			} else {
				return null;
			}
		}
	}
}
