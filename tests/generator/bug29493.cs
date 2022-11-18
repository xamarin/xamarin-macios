using System;

using UIKit;
using Foundation;
using ObjCRuntime;
using CoreGraphics;

namespace Bug29493 {
	[Category]
	[BaseType (typeof (UIColor))]
	interface UIColorExtensions {
		// +(UIColor *)jbill_messageBubbleGreenColor;
		[Static]
		[Export ("jbill_messageBubbleGreenColor")]
		UIColor MessageBubbleGreenColor ();

		[Export ("jbill_messageBubbleGreenColor")]
		UIColor MessageBubbleGreenColor2 ();
	}

	[Category]
	[BaseType (typeof (NSUrlSession))]
	interface NSUrlExtensions {
		// +(UIColor *)jbill_messageBubbleGreenColor;
		[Static]
		[Export ("jbill_messageBubbleGreenColor")]
		UIColor MessageBubbleGreenColor ();

		[Export ("jbill_messageBubbleGreenColor")]
		UIColor MessageBubbleGreenColor2 ();
	}

	[BaseType (typeof (NSObject))]
	interface HelloKitty {

	}

	[Category]
	[BaseType (typeof (HelloKitty))]
	interface HelloKittyExtensions {
		// +(UIColor *)jbill_messageBubbleGreenColor;
		[Static]
		[Export ("jbill_messageBubbleGreenColor")]
		UIColor MessageBubbleGreenColor ();

		[Export ("jbill_messageBubbleGreenColor")]
		UIColor MessageBubbleGreenColor2 ();
	}

	[BaseType (typeof (NSObject), Name = "NSBatman")]
	interface BecauseImBatman {

	}

	[Category]
	[BaseType (typeof (BecauseImBatman))]
	interface BecauseImBatmanExtensions {
		// +(UIColor *)jbill_messageBubbleGreenColor;
		[Static]
		[Export ("jbill_messageBubbleGreenColor")]
		UIColor MessageBubbleGreenColor ();

		[Export ("jbill_messageBubbleGreenColor")]
		UIColor MessageBubbleGreenColor2 ();
	}

	[BaseType (typeof (BecauseImBatman), Name = "NSBatman2")]
	interface BecauseImBatman2 {

	}

	[Category]
	[BaseType (typeof (BecauseImBatman2))]
	interface BecauseImBatman2Extensions {
		// +(UIColor *)jbill_messageBubbleGreenColor;
		[Static]
		[Export ("jbill_messageBubbleGreenColor")]
		UIColor MessageBubbleGreenColor ();

		[Export ("jbill_messageBubbleGreenColor")]
		UIColor MessageBubbleGreenColor2 ();
	}

	[BaseType (typeof (BecauseImBatman))]
	interface Superman {

	}

	[Category]
	[BaseType (typeof (Superman))]
	interface SupermanExtensions {
		// +(UIColor *)jbill_messageBubbleGreenColor;
		[Static]
		[Export ("jbill_messageBubbleGreenColor")]
		UIColor MessageBubbleGreenColor ();

		[Export ("jbill_messageBubbleGreenColor")]
		UIColor MessageBubbleGreenColor2 ();
	}
}
