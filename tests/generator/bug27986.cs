using System;

using Foundation;
using ObjCRuntime;
using System.Drawing;

namespace bug27986 {
	interface IFooProtocol { }

	[Model, Protocol, Preserve (AllMembers = true)]
	[BaseType (typeof (NSObject))]
	interface FooProtocol {

		[Preserve]
		[Export ("doSomething:atIndex:")]
		void DoSomething (NSObject @object, int index);

		[Abstract]
		[Preserve (Conditional = true)]
		[Export ("doSomething:atIndex2:")]
		void DoSomething2 (NSObject @object, int index);

		[Abstract]
		[Preserve (Conditional = true)]
		[Export ("center")]
		NSObject Center { get; set; }

		[Preserve]
		[Export ("center2")]
		NSObject Center2 { get; set; }
	}


	[BaseType (typeof (NSObject))]
	[Preserve (AllMembers = true)]
	interface Widget {

		[Preserve (Conditional = true)]
		[Export ("initWithElmo:")]
		IntPtr Constructor (uint elmo);

		[Preserve]
		[Export ("doSomething:atIndex:")]
		void DoSomething (NSObject @object, int index);

		[Preserve (Conditional = true)]
		[Export ("center")]
		NSObject Center { get; set; }

		[Export ("noCenter")]
		PointF NoCenter { [Preserve (Conditional = true)] get; [Preserve] set; }

		[Preserve]
		[Field ("FooField", "__Internal")]
		NSString FooField { get; }

		[Field ("IntField", "__Internal")]
		int IntField { [Preserve (Conditional = true)] get; [Preserve] set; }

		[Wrap ("Center")]
		[Preserve]
		NSString CenterWrap { get; set; }

		[Preserve (Conditional = true)]
		[Export ("delegate")]
		IFooProtocol Delegate { get; set; }
	}
}
