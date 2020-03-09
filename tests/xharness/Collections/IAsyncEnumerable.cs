using System.Threading.Tasks;

namespace xharness.Collections {
	public interface IAsyncEnumerable {
		Task ReadyTask { get; }
	}
}
