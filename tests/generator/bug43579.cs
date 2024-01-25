using System;

using Foundation;

using ObjCRuntime;

namespace bug43579 {

	interface IFooDelegate { }

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface FooDelegate {

		// Events
		[Export ("FooDidEndEditing:"), EventArgs ("FooDelegateEnded"), EventName ("Ended")]
		void EditingEnded (Foo foo);

		[Export ("FooDidEndEditing:after:"), EventArgs ("FooDelegateEndedTime"), EventName ("EndedAfter")]
		void EditingEnded (Foo foo, double time);

		[Export ("FooDidEndEditing:after:index:"), IgnoredInDelegate]
		void EditingEnded (Foo foo, double time, int index);


		// Using Func
		[Export ("foo:dobar:"), DelegateName ("Func<Foo,int,bool>"), DefaultValue ("true")]
		bool DoBar (Foo foo, int bar);

		[Export ("foo:doBar:after:"), DelegateApiName ("DoBarAfter"), DelegateName ("Func<Foo,int,double,bool>"), DefaultValue ("true")]
		bool DoBar (Foo foo, int bar, double time);


		// Using an actual DelegateName
		[Export ("foo:doAnotherBar:"), DelegateName ("FooDelegateAnotherBar"), DefaultValue ("true")]
		bool DoAnotherBar (Foo foo, int bar);

		[Export ("foo:doAnotherBar:after:"), DelegateApiName ("DoAnotherBarAfter"), DelegateName ("FooDelegateAnotherBarTime"), DefaultValue ("true")]
		bool DoAnotherBar (Foo foo, int bar, double time);
	}

	[BaseType (typeof (NSObject), Delegates = new string [] { "Delegate" }, Events = new Type [] { typeof (FooDelegate) })]
	interface Foo {

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		IFooDelegate Delegate { get; set; }
	}
}
