using System;
using System.Drawing;
using Foundation;
using CoreMedia;
using ObjCRuntime;

namespace GH6863_property {

	[BaseType (typeof (NSObject))]
	interface MyFooClass {

		[BindAs (typeof (string))]
		[Export ("stringProp")]
		NSString [] StringProp { get; }

	}
}
