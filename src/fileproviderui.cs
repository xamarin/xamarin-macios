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

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace FileProviderUI {

	[ErrorDomain ("FPUIErrorDomain")]
	[Native]
	enum FPUIExtensionErrorCode : ulong {
		UserCancelled,
		Failed
	}

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

#if IOS
	[BaseType (typeof (UIViewController))]
#else
	[BaseType (typeof (NSViewController))]
#endif
	interface FPUIActionExtensionViewController {

		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("extensionContext", ArgumentSemantic.Strong)]
		FPUIActionExtensionContext ExtensionContext { get; }

		[Export ("prepareForError:")]
		void Prepare (NSError error);

		[Export ("prepareForActionWithIdentifier:itemIdentifiers:")]
		void Prepare (string actionIdentifier, NSString [] itemIdentifiers);
	}
}
