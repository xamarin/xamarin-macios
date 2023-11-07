using System;
using System.Drawing;
using Foundation;
using CoreMedia;
using ObjCRuntime;

namespace GH6863_method {

	[BaseType (typeof (NSObject))]
	interface MyFooClass {

		[Export ("stringMethod:")]
		void StringMethod ([BindAs (typeof (string))] NSString [] id_test);


	}
}
