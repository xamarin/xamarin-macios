using Foundation;
using ObjCRuntime;

namespace GHIssue5416 {

	[BaseType (typeof (NSObject))]
	interface NullAllowedWarning {
		[Export ("methodWhen:data:", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSString Method (NSDate d, NSObject o);
	}
}
