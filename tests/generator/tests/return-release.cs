using System;

using Foundation;

namespace NS {
	[Protocol]
	interface ProtocolWithReturnRelease {
		[Abstract, Export ("newRequiredObject")]
		[return: Release]
		NSObject CreateRequiredObject ();

		[Export ("newOptionalObject")]
		[return: Release]
		NSObject CreateOptionalObject ();
	}
}
