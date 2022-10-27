using Foundation;

namespace TestCase {

	delegate void TestHandler (int result, NSError err);

	[BaseType (typeof (NSObject))]
	interface Foo {

		[Async, Static, Export ("someMethod:")]
		bool SomeMethod (TestHandler callback);
	}
}
