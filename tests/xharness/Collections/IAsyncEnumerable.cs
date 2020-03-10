using System.Threading.Tasks;

namespace Xharness.Collections {
	public interface IAsyncEnumerable {
		Task ReadyTask { get; }
	}
}
