using Foundation;

#if !FRAMEWORK_TEST
namespace Simple {
	[BaseType (typeof (NSObject))]
	interface SimpleClass {
		[Export ("doIt")]
		int DoIt ();
	}
}
#endif
