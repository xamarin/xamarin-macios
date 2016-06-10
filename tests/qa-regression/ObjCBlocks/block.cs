using System;

using MonoMac.Foundation;

namespace Test {

	[BaseType (typeof (NSObject))]
	interface ObjCBlocksTest {

		[Export ("doInvoke:")]
		void DoInvoke (Action blockHandler);
	}
}
