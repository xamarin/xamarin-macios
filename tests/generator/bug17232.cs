using System;

using Foundation;

using ObjCRuntime;

namespace xam.bug17232 {
	[BaseType (typeof (NSObject))]
	interface FieldsTest {

		[Field ("MyFooFieldA", "libFoo.a")]
		NSString FooFieldA { get; }

		[Field ("MyFooFieldB", "libFoo.a")]
		NSString FooFieldB { get; set; }

		[Field ("MyBarFieldA", "Frameworks/Bar.framework/Bar")]
		NSString BarFieldA { get; }

		[Field ("MyBarFieldB", "Frameworks/Bar.framework/Bar")]
		NSString BarFieldB { get; set; }

		[Field ("MyBarFieldA", "Xyz")]
		NSString XyzFieldA { get; }

		[Field ("XyzFieldB", "Xyz")]
		NSString XyzFieldB { get; set; }

		[Field ("MyUnboundField", Constants.CoreGraphicsLibrary)]
		NSString UnboundField { get; set; }

		[Field ("MyUnboundUIKitField", "UIKit")]
		NSString UnboundUIKitField { get; set; }
	}
}
