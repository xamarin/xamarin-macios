#if !XAMCORE_2_0
using MonoMac.Foundation;
#else
using Foundation;
#endif

namespace Test
{
	[BaseType (typeof (NSObject))]
	interface TestBMACLib
	{
		[Export ("addTwoRows:withSecond:")]
		int Add (int first, int second);
	}
}