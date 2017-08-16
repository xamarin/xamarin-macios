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

	[iOS (11,0)]
	[Native]
	public enum INUIInteractiveBehavior : nuint {
		None,
		NextView,
		Launch,
		GenericAction,
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Protocol]
	interface INUIHostedViewControlling {

		[Abstract]
		[Export ("configureWithInteraction:context:completion:")]
		void Configure (INInteraction interaction, INUIHostedViewContext context, Action<CGSize> completion);

		[iOS (11,0)]
		[Export ("configureViewForParameters:ofInteraction:interactiveBehavior:context:completion:")]
		void ConfigureView (NSSet<INParameter> parameters, INInteraction interaction, INUIInteractiveBehavior interactiveBehavior, INUIHostedViewContext context, Action<bool, NSSet<INParameter>, CGSize> completion);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Category]
	[BaseType (typeof (NSExtensionContext))]
	interface NSExtensionContext_INUIHostedViewControlling {

		[Export ("hostedViewMinimumAllowedSize")]
		CGSize GetHostedViewMinimumAllowedSize ();

		[Export ("hostedViewMaximumAllowedSize")]
		CGSize GetHostedViewMaximumAllowedSize ();

		[iOS (11,0)]
		[Export ("interfaceParametersDescription")]
		string GetInterfaceParametersDescription ();
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Protocol]
	interface INUIHostedViewSiriProviding {

		[Export ("displaysMap")]
		bool DisplaysMap { get; }

		[Export ("displaysMessage")]
		bool DisplaysMessage { get; }

		[Export ("displaysPaymentTransaction")]
		bool DisplaysPaymentTransaction { get; }
	}
}

#endif
