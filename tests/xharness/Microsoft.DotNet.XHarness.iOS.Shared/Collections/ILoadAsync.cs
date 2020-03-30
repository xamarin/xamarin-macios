using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Collections {
	public interface ILoadAsync {
		Task LoadAsync (ILog log, bool include_locked, bool force);
	}
}
