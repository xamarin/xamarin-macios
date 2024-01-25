using Foundation;

using LibraryA;

namespace LibraryB {
	[BaseType (typeof (NSObject))]
	interface B {
		[Export ("a")]
		A GetA ();
	}
}

