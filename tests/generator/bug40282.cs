using System;
using Foundation;

namespace bug40282 {
	[Internal]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface FooDelegateInternal {
		[Export ("fooService:hasUInt32s:numberOfItems:forFooSession:")]
		bool FooServiceUInt32 (int service, IntPtr data, ushort numberOfItems, int session);

		[Export ("fooService:hasSInt16s:numberOfItems:forFooSession:")]
		bool FooServiceSInt16 (int service, IntPtr data, ushort numberOfItems, int session);

		[Export ("fooService:sessionDidFinish:")]
		void FooService (int service, int session);
	}
}
