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

	/// <summary>Extension context for reporting unwanted communication.</summary>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSExtensionContext))]
	interface ILClassificationUIExtensionContext {

		[Export ("readyForClassificationResponse")]
		bool ReadyForClassificationResponse { [Bind ("isReadyForClassificationResponse")] get; set; }
	}

	/// <summary>Base class for view controllers for Unwanted Communication Reporting extensions.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/documentation/sms_and_call_reporting/sms_and_call_spam_reporting?language=objc">SMS and Call Spam Reporting</related>
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
