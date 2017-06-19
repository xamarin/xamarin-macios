using Foundation;
using ObjCRuntime;

namespace Tests {

	[Introduced (PlatformName.iOS, 11, 0)]
	[DefaultCtorAvailability (AvailabilityKind.Obsoleted, PlatformName.iOS, 11, 0, message: "Do not use")]
	[BaseType (typeof(NSObject))]
	interface FooClass {
		[Export ("doSomething")]
		void DoSomething ();
	}
}