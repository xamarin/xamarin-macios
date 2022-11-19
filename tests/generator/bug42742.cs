using System;

using Foundation;
using ObjCRuntime;
using System.Drawing;

namespace bug42742 {
	interface IFooProtocol { }

	[Advice ("FooProtocol")]
	[Model, Protocol]
	[BaseType (typeof (NSObject))]
	interface FooProtocol {

		[Advice ("doSomething:atIndex:")]
		[Export ("doSomething:atIndex:")]
		void DoSomething (NSObject @object, int index);

		[Abstract]
		[Advice ("doSomething:atIndex2:")]
		[Export ("doSomething:atIndex2:")]
		void DoSomething2 (NSObject @object, int index);

		[Abstract]
		[Advice ("center")]
		[Export ("center")]
		NSObject Center { get; set; }

		[Advice ("center2")]
		[Export ("center2")]
		NSObject Center2 { get; set; }
	}


	[BaseType (typeof (NSObject))]
	[Advice ("Widget")]
	interface Widget {

		[Advice ("initWithElmo:")]
		[Export ("initWithElmo:")]
		IntPtr Constructor (uint elmo);

		[Advice ("doSomething:atIndex:")]
		[Export ("doSomething:atIndex:")]
		void DoSomething (NSObject @object, int index);

		[Advice ("center")]
		[Export ("center")]
		NSObject Center { get; set; }

		[Export ("noCenter")]
		PointF NoCenter { [Advice ("noCenterGet")] get; [Advice ("noCenterSet")] set; }

		[Advice ("FooField")]
		[Field ("FooField", "__Internal")]
		NSString FooField { get; }

		[Field ("IntField", "__Internal")]
		int IntField { [Advice ("IntFieldGet")] get; [Advice ("IntFieldSet")] set; }

		[Wrap ("Center")]
		[Advice ("CenterWrap")]
		NSString CenterWrap { get; set; }

		[Advice ("delegate")]
		[Export ("delegate")]
		IFooProtocol Delegate { get; set; }
	}
}
