using System;
using Foundation;
using ObjCRuntime;

namespace Accessibility {

	[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14,0)]
	[Native]
	public enum AXCustomContentImportance : ulong
	{
		Default,
		High,
	}

	[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AXCustomContent : NSCopying, NSSecureCoding
	{
		[Static]
		[Export ("customContentWithLabel:value:")]
		AXCustomContent Create (string label, string value);

		[Static]
		[Export ("customContentWithAttributedLabel:attributedValue:")]
		AXCustomContent Create (NSAttributedString label, NSAttributedString value);

		[Export ("label")]
		string Label { get; }

		[Export ("attributedLabel", ArgumentSemantic.Copy)]
		NSAttributedString AttributedLabel { get; }

		[Export ("value")]
		string Value { get; }

		[Export ("attributedValue", ArgumentSemantic.Copy)]
		NSAttributedString AttributedValue { get; }

		[Export ("importance", ArgumentSemantic.Assign)]
		AXCustomContentImportance Importance { get; set; }
	}

	[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14,0)]
	[Protocol]
	interface AXCustomContentProvider
	{
		[Abstract]
		[NullAllowed, Export ("accessibilityCustomContent", ArgumentSemantic.Copy)]
		AXCustomContent[] AccessibilityCustomContent { get; set; }
	}

}
