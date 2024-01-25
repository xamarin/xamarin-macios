using Foundation;

using ObjCRuntime;

namespace GHIssue5444 {

	[BaseType (typeof (NSObject))]
	interface CardScheme {
		[Export ("colorScheme", ArgumentSemantic.Assign)]
		new NSString ColorScheme { get; set; }

		[Export ("shapeScheme", ArgumentSemantic.Assign)]
		new NSString ShapeScheme { get; set; }

		NSString SemanticColorScheme {
			[Wrap ("Runtime.GetNSObject<NSString> (ColorScheme.Handle, false)")]
			get;
			[Wrap ("ColorScheme = value")]
			set;
		}
	}
}
