using System;
using Foundation;
using ObjCRuntime;
using CoreMedia;

namespace GHIssue5692 {

	[BaseType (typeof (NSObject))]
	interface Foo {
		[Export ("enumerateSampleBuffers:")]
		void Enumerate (Action<CMSampleBuffer, NSError> handler);
	}
}
