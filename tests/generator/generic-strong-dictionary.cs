using Foundation;
using ObjCRuntime;

namespace Test
{
	
	[StrongDictionary ("AdvertisementDataKeys")]
	interface AdvertisementData {
		// property under tests, the generator should create a compilable property
		NSDictionary <CBUUID, NSData> ServiceData { get; set; }
	}

	[Static, Internal]
	interface AdvertisementDataKeys {
		[Field ("MyFooFieldA", "libFoo.a")]
		NSString ServiceDataKey { get; }
	}
}
