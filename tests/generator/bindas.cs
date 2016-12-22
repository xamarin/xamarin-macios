using System;
using System.Drawing;

using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.CoreGraphics;
using MonoTouch.CoreMedia;

namespace ABCBinding {
	[BaseType (typeof (NSObject))]
	interface MyFooClass {

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
