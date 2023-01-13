//
// IdentityLookupUI C# bindings
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2018-2019 Microsoft Corporation.
//

using System;
using Foundation;
using ObjCRuntime;
using UIKit;
using IdentityLookup;

namespace IdentityLookupUI {

	[iOS (12, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSExtensionContext))]
	interface ILClassificationUIExtensionContext {

		[Export ("readyForClassificationResponse")]
		bool ReadyForClassificationResponse { [Bind ("isReadyForClassificationResponse")] get; set; }
	}

	[iOS (12, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIViewController))]
	interface ILClassificationUIExtensionViewController {

		[Export ("extensionContext", ArgumentSemantic.Strong)]
		ILClassificationUIExtensionContext ExtensionContext { get; }

		[Export ("prepareForClassificationRequest:")]
		void Prepare (ILClassificationRequest request);

		[Export ("classificationResponseForRequest:")]
		ILClassificationResponse GetClassificationResponse (ILClassificationRequest request);
	}
}
