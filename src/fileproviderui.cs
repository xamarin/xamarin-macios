//
// FileProvider C# bindings
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

using System;
using ObjCRuntime;
using Foundation;
#if IOS
using UIKit;
#else
using AppKit;
#endif
using FileProvider;

namespace FileProviderUI {

	[iOS (11,0)]
	[Mac (10,15)]
	[ErrorDomain ("FPUIErrorDomain")]
	[Native]
	enum FPUIExtensionErrorCode : ulong {
		UserCancelled,
		Failed
	}

	[iOS (11,0)]
	[Mac (10,15)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSExtensionContext))]
	interface FPUIActionExtensionContext {

		[NullAllowed, Export ("domainIdentifier")]
		string DomainIdentifier { get; }

		[Export ("completeRequest")]
		void CompleteRequest ();

		[Export ("cancelRequestWithError:")]
		void CancelRequest (NSError error);
	}

	[iOS (11,0)]
	[Mac (10,15)]
#if IOS
	[BaseType (typeof (UIViewController))]
#else
	[BaseType (typeof (NSViewController))]
#endif
	interface FPUIActionExtensionViewController {

		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("extensionContext", ArgumentSemantic.Strong)]
		FPUIActionExtensionContext ExtensionContext { get; }

		[Export ("prepareForError:")]
		void Prepare (NSError error);

		[Export ("prepareForActionWithIdentifier:itemIdentifiers:")]
		void Prepare (string actionIdentifier, NSString [] itemIdentifiers);
	}
}
