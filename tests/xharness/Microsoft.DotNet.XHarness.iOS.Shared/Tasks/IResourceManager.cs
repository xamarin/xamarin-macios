using System.Collections.Generic;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Tasks {

	/// <summary>
	/// Managed the resorces that can be used by the different tests tasks so that when ran async they do not step
	/// on eachother.
	/// </summary>
	public interface IResourceManager
	{
		Resource DesktopResource { get; }
		Resource NugetResource { get; }
		Resources GetDeviceResources (IEnumerable<IHardwareDevice> devices);
		IEnumerable<Resource> GetAll ();
	}
}
