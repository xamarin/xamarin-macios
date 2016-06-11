#if !XAMCORE_2_0
#if MONOMAC
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
#endif
#else
using Foundation;
using ObjCRuntime;
#endif

namespace Test
{
	
	[StrongDictionary ("AdvertisementDataKeys")]
	interface AdvertisementData {
#if XAMCORE_2_0
		// property under tests, the generator should create a compilable property
		NSDictionary <CBUUID, NSData> ServiceData { get; set; }
#else
		// ensure that the generator continues to work with classic
		NSDictionary ServiceData { get; set; }
#endif
	}

	[Static, Internal]
	interface AdvertisementDataKeys {
		[Field ("MyFooFieldA", "libFoo.a")]
		NSString ServiceDataKey { get; }
	}
}
