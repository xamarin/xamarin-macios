using System;
using System.Drawing;
using Foundation;
using CoreMedia;
using CoreAnimation;
using ObjCRuntime;

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

		[BindAs (typeof (CMVideoDimensions))]
		[Export ("cmVideoDimensionsProperty")]
		NSValue CMVideoDimensionsProperty { get; }

		[BindAs (typeof (long))]
		[Export ("longProperty")]
		NSNumber LongProperty { get; set; }

		[BindAs (typeof (CAScroll []))]
		[Export ("scrollEnumArray")]
		NSString [] ScrollEnumArray { get; set; }

		[return: BindAs (typeof (CAScroll []))]
		[Export ("getScrollArrayEnum:")]
		NSString [] GetScrollArrayEnum ([BindAs (typeof (CAScroll []))] NSString [] arg1);

		// Bug #57797
		// [return: BindAs (typeof (CAScroll? []))]
		// [Export ("getScrollArrayNullableEnum:")]
		// NSString [] GetScrollArrayEnumNullable ([BindAs (typeof (CAScroll? []))] NSString [] arg1);

		[BindAs (typeof (CAScroll []))]
		[Export ("scrollEnumArray2")]
		NSNumber [] ScrollEnumArray2 { get; set; }

		[return: BindAs (typeof (CAScroll []))]
		[Export ("getScrollArrayEnum2:")]
		NSNumber [] GetScrollArrayEnum2 ([BindAs (typeof (CAScroll []))] NSNumber [] arg1);

		// Bug #57797
		// [return: BindAs (typeof (CAScroll? []))]
		// [Export ("getScrollArrayNullableEnum2:")]
		// NSNumber [] GetScrollArrayNullableEnum2 ([BindAs (typeof (CAScroll? []))] NSNumber [] arg1);

		[BindAs (typeof (CMTime []))]
		[Export ("timeEnumArray")]
		NSValue [] TimeEnumArray { get; set; }

		[return: BindAs (typeof (CMTime []))]
		[Export ("getTimeEnumArray:")]
		NSValue [] GetTimeEnumArray ([BindAs (typeof (CMTime []))] NSValue [] arg1);

		// Bug #57797
		// [return: BindAs (typeof (CMTime? []))]
		// [Export ("getTimeEnumNullableArray:")]
		// NSValue [] GetTimeEnumNullableArray ([BindAs (typeof (CMTime? []))] NSValue [] arg1);

		[BindAs (typeof (CAScroll))]
		[Export ("scrollFooEnum")]
		NSString ScrollFooEnum { get; set; }

		[BindAs (typeof (CAScroll?))]
		[Export ("scrollFooEnum2")]
		NSString ScrollFooEnum2 { get; set; }

		[return: BindAs (typeof (CAScroll))]
		[Export ("getScrollEnum3:arg2:")]
		NSString GetScrollEnum ([BindAs (typeof (CAScroll))] NSString arg1, [BindAs (typeof (CAScroll?))] NSString arg2);

		[BindAs (typeof (CAScroll))]
		[Export ("scrollEnum")]
		NSNumber ScrollEnum2 { get; set; }

		[return: BindAs (typeof (CAScroll))]
		[Export ("getScrollEnum:")]
		NSNumber GetScrollEnum2 ([BindAs (typeof (CAScroll))] NSNumber arg1);

		[return: BindAs (typeof (CAScroll?))]
		[Export ("getScrollEnumNullable:")]
		NSNumber GetScrollEnumNullable2 ([BindAs (typeof (CAScroll?))] NSNumber arg1);
	}
}
