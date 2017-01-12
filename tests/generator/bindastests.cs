using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.CoreMedia;

namespace BindAsTests {

	[BaseType (typeof (NSObject))]
	interface MyFooClass {

		[return: BindAs (typeof (bool?))]
		[Export ("boolMethod:a:b:c:d:")]
		NSNumber BoolMethod (int arg0, string arg1, [BindAs (typeof (RectangleF))] NSValue arg2, [BindAs (typeof (bool?))] NSNumber arg3, int arg4);

		[Export ("stringMethod:a:b:c:d:")]
		string stringMethod (int arg0, string arg1, [BindAs (typeof (RectangleF))] NSValue arg2, [BindAs (typeof (bool?))] NSNumber arg3, int arg4);

		[return: BindAs (typeof (bool?))]
		[Export ("boolMethod:")]
		NSNumber BoolMethod (int arg1);

		[return: BindAs (typeof (int?))]
		[Export ("intMethod")]
		NSNumber IntMethod ();

		[return: BindAs (typeof (sbyte?))]
		[Export ("sbyteMethod")]
		NSNumber SbyteMethod ();

		[return: BindAs (typeof (bool?))]
		[Static, Export ("boolMethodS")]
		NSNumber BoolMethodS ();

		[return: BindAs (typeof (int?))]
		[Static, Export ("intMethodS")]
		NSNumber IntMethodS ();

		[return: BindAs (typeof (sbyte))]
		[Static, Export ("sbyteMethodS")]
		NSNumber SbyteMethodS ();

		[return: BindAs (typeof (CMTime))]
		[Export ("cmtimeMethod")]
		NSValue CMTimeMethod ();

		[return: BindAs (typeof (PointF))]
		[Static, Export ("pointMethodS:")]
		NSValue PointFMethodS (sbyte arg1);

		[BindAs (typeof (bool?))]
		[Export ("boolProperty")]
		NSNumber BoolProperty { get; }

		[BindAs (typeof (double?))]
		[Export ("doubleProperty")]
		NSNumber DoubleProperty { get; set; }

		[BindAs (typeof (RectangleF?))]
		[Static, Export ("rectangleFPropertyS")]
		NSValue RectangleFPropertyS { get; set; }

		[BindAs (typeof (SizeF))]
		[Export ("sizeFFProperty")]
		NSValue SizeFProperty { get; set; }

		[BindAs (typeof (CMTimeRange))]
		[Export ("cmTimeRangeProperty")]
		NSValue CMTimeRangeProperty { get; }

		[BindAs (typeof (long))]
		[Export ("longProperty")]
		NSNumber LongProperty { get; set; }
	}
}