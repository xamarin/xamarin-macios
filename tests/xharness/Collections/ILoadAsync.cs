using System.Threading.Tasks;
using Xharness;
using Xharness.Logging;

namespace Xharness.Collections{
	public interface ILoadAsync {
		Task LoadAsync (ILog log, bool include_locked, bool force);
		IHarness Harness { get; set; }
	}
}
