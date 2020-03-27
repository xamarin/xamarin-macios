using System.Threading.Tasks;

namespace Microsoft.DotNet.XHarness.iOS.Collections {
	public interface IAsyncEnumerable {
		Task ReadyTask { get; }
	}
}
