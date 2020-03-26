using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Logging;

namespace Microsoft.DotNet.XHarness.iOS.Collections {
	public interface ILoadAsync {
		Task LoadAsync (ILog log, bool include_locked, bool force);
	}
}
