using Foundation;
using ObjCRuntime;

namespace GHIssue5416 {

	[BaseType (typeof (NSObject))]
	interface NullAllowedWarning {
		[Export ("setter", ArgumentSemantic.Assign)]
		NSString Setter { get; [NullAllowed] set; }
	}
}
