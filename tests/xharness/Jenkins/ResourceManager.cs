using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;

using Xharness.Jenkins.TestTasks;

#nullable enable
namespace Xharness.Jenkins {
	class ResourceManager : IResourceManager {
		readonly Dictionary<string, Resource> deviceResources = new Dictionary<string, Resource> ();


		public Resource DesktopResource { get; } = new Resource ("Desktop", Environment.ProcessorCount);
		public Resource NugetResource { get; } = new Resource ("Nuget", 1); // nuget is not parallel-safe :(

		public ResourceManager () { }

		public Resources GetDeviceResources (IEnumerable<IHardwareDevice> devices)
		{
			List<Resource> resources = new List<Resource> ();
			lock (deviceResources) {
				foreach (var device in devices) {
					if (!deviceResources.TryGetValue (device.UDID, out var res))
						deviceResources.Add (device.UDID, res = new Resource (device.UDID, 1, device.Name));
					resources.Add (res);
				}
			}
			return new Resources (resources);
		}

		public IEnumerable<Resource> GetAll ()
			=> deviceResources.Values.Concat (new Resource [] { DesktopResource, NugetResource });
	}
}
