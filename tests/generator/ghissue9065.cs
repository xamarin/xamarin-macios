using Foundation;

using ObjCRuntime;

namespace GHIssue9065 {

	[Sealed]
	[BaseType (typeof (NSObject))]
	interface Widget : INSCopying {
		[Export ("doSomething:atIndex:")]
		[Static]
		void DoSomething (NSObject obj, int index);
	}
}
