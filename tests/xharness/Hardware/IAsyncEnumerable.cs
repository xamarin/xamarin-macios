using System.Threading.Tasks;

namespace xharness.Hardware {
	public interface IAsyncEnumerable {
		Task ReadyTask { get; }
	}
}
