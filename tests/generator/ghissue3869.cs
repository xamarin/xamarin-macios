using Foundation;
using ObjCRuntime;

namespace GHIssue3869 {
	interface IFooDelegate { }

	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface FooDelegate {

		[Abstract]
		[Export ("doSomethingForIdentifier:parentContext:parentIdentifierPath:")]
		NSObject DoSomething (string identifier, NSObject parentContext, string [] parentIdentifierPath);
	}

	[BaseType (typeof (NSObject))]
	interface Foo {

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IFooDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }
	}
}