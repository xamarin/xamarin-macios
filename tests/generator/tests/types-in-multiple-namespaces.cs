using System;

using CoreFoundation;
using Foundation;
using ObjCRuntime;

namespace NS {
	delegate void D1 ([NullAllowed] Network.NWEndpoint remoteEndpoints);

	[BaseType (typeof (NSObject))]
	interface TypesInMultipleNamespaces {
		[Export ("someProperty")]
		D1 SomeProperty { get; set; }
	}
}
