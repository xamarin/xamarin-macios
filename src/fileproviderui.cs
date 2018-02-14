//
// FileProvider C# bindings
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0

using System;
using ObjCRuntime;
using Foundation;
using UIKit;
using FileProvider;

namespace FileProviderUI {

	[iOS (11,0)]
	[ErrorDomain ("FPUIErrorDomain")]
	[Native]
	enum FPUIExtensionErrorCode : ulong {
		UserCancelled,
		Failed
	}

	[iOS (11,0)]
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
	[BaseType (typeof (UIViewController))]
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
#endif
