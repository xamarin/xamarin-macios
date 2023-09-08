using System.Runtime.Versioning;
using Foundation;
using UIKit;

namespace iosbindinglib {
	[Protocol, Model]
	[BaseType(typeof(UIApplicationDelegate))]
	public interface TestDelegate
	{
	}
}
