using Foundation;
using ObjCRuntime;

namespace Test {

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
