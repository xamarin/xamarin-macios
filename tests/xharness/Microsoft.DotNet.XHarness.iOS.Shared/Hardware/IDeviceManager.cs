using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Hardware {
	public interface IDeviceLoader {
		Task LoadDevices (ILog log, bool includeLocked = false, bool forceRefresh = false, bool listExtraData = false);
	}
}
