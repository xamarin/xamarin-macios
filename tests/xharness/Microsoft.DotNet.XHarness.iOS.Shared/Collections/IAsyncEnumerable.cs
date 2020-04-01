using System.Threading.Tasks;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Collections {
	public interface IAsyncEnumerable {
		Task ReadyTask { get; }
	}
}
