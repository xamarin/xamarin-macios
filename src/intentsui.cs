//
// IntentsUI bindings
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0 // The IntentsUI framework relies on Intents which is only available in Unified

using System;
using XamCore.CoreGraphics;
using XamCore.Foundation;
using XamCore.Intents;
using XamCore.ObjCRuntime;
using XamCore.UIKit;

namespace XamCore.IntentsUI {

	[Introduced (PlatformName.iOS, 10, 0)]
	[Native]
	public enum INUIHostedViewContext : nuint {
		SiriSnippet,
		MapsCard
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Category]
	[Internal] // We are exposing this members as manual bindings in INImage.INImage
	[BaseType (typeof (INImage))]
	interface INImage_IntentsUI {

		[Static]
		[Export ("imageWithCGImage:")]
		INImage ImageWithCGImage (CGImage image);

		[Static]
		[Export ("imageWithUIImage:")]
		INImage ImageWithUIImage (UIImage image);

		[Static]
		[Export ("imageSizeForIntentResponse:")]
		CGSize ImageSizeForIntentResponse (INIntentResponse response);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Protocol]
	interface INUIHostedViewControlling {

		[Abstract]
		[Export ("configureWithInteraction:context:completion:")]
		void Configure (INInteraction interaction, INUIHostedViewContext context, Action<CGSize> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Category]
	[BaseType (typeof (NSExtensionContext))]
	interface NSExtensionContext_INUIHostedViewControlling {

		[Export ("hostedViewMinimumAllowedSize")]
		CGSize GetHostedViewMinimumAllowedSize ();

		[Export ("hostedViewMaximumAllowedSize")]
		CGSize GetHostedViewMaximumAllowedSize ();
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Protocol]
	interface INUIHostedViewSiriProviding {

		[Export ("displaysMap")]
		bool DisplaysMap { get; }

		[Export ("displaysMessage")]
		bool DisplaysMessage { get; }

		// FIXME: The selector name is correct according to Headers
		// but it has a typo Transcation so Apple might fix it in a future release
		[Export ("displaysPaymentTranscation")]
		bool DisplaysPaymentTransaction { get; }
	}
}

#endif
