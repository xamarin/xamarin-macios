#if !XAMCORE_2_0
#if MONOMAC
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
#else
using Foundation;
using ObjCRuntime;
#endif
#else
using Foundation;
using ObjCRuntime;
#endif

namespace Test
{
	
	[StrongDictionary ("AdvertisementDataKeys")]
	interface AdvertisementData {
		// ensure that the generator continues to work with classic
		NSDictionary ServiceData { get; set; }
	}

	[Static, Internal]
	interface AdvertisementDataKeys {
		[Field ("MyFooFieldA", "libFoo.a")]
		NSString ServiceDataKey { get; }
	}
}
