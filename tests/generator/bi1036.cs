using Foundation;

namespace NS {
	[BaseType (typeof (NSObject))]
	interface Foo {
		[Export ("foo:")]
		[Async (ResultTypeName = "Result")]
		void Method (string arg);
	}
}
