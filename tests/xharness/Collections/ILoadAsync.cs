using System.Threading.Tasks;
using Xharness;
using Microsoft.DotNet.XHarness.iOS.Logging;

namespace Xharness.Collections{
	public interface ILoadAsync {
		Task LoadAsync (ILog log, bool include_locked, bool force);
	}
}
