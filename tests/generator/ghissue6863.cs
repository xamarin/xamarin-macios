using System;
using System.Drawing;
using Foundation;
using CoreMedia;
using ObjCRuntime;

namespace GH6863 {

	[BaseType (typeof (NSObject))]
	interface MyFooClass {

		[BindAs (typeof (string))]
		[Export ("stringProp")]
		NSString[] StringProp { get; }
		
	}
}
