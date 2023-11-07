using Foundation;

namespace Test {
	[BaseType (typeof (NSObject))]
	interface TestBMACLib {
		[Export ("addTwoRows:withSecond:")]
		int Add (int first, int second);
	}
}
