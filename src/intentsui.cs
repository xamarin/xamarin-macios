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
using CoreGraphics;
using Foundation;
using Intents;
using ObjCRuntime;
using UIKit;

namespace IntentsUI {

	[iOS (10, 0)]
	[Native]
	public enum INUIHostedViewContext : ulong {
		SiriSnippet,
		MapsCard
	}

	[iOS (11,0)]
	[Native]
	public enum INUIInteractiveBehavior : ulong {
		None,
		NextView,
		Launch,
		GenericAction,
	}

	[iOS (11,0)]
	delegate void INUIHostedViewControllingConfigureViewHandler (bool success, NSSet<INParameter> configuredParameters, CGSize desiredSize);

	[iOS (10, 0)]
	[Protocol]
	interface INUIHostedViewControlling {

#if !XAMCORE_4_0 // Apple made this member optional in iOS 11
		[Abstract]
#endif
		[Export ("configureWithInteraction:context:completion:")]
		void Configure (INInteraction interaction, INUIHostedViewContext context, Action<CGSize> completion);

		[iOS (11,0)]
		[Export ("configureViewForParameters:ofInteraction:interactiveBehavior:context:completion:")]
		void ConfigureView (NSSet<INParameter> parameters, INInteraction interaction, INUIInteractiveBehavior interactiveBehavior, INUIHostedViewContext context, INUIHostedViewControllingConfigureViewHandler completionHandler);
	}

	[iOS (10, 0)]
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

	[iOS (10, 0)]
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
