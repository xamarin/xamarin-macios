// We test that a third-party binding inheriting from UIViewController
// gets a proper NSCoding constructor.
using UIKit;

namespace Test {
	[BaseType (typeof (UIViewController))]
	public interface CustomController {

	}
}
