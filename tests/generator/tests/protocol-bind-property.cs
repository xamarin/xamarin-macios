using System;

using Foundation;

using ObjCRuntime;

namespace NS {
	[Protocol]
	interface MyProtocol {
		[Abstract]
		[Export ("abstractProperty")]
		bool AbstractProperty { [Bind ("isAbstractProperty")] get; set; }

		[Export ("optionalProperty")]
		bool OptionalProperty { [Bind ("isOptionalProperty")] get; set; }
	}
}
