//
// safariservices.cs: binding for iOS (7+) SafariServices framework
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//   Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc.

using System;

using XamCore.Foundation;
using XamCore.ObjCRuntime;
using XamCore.UIKit;

namespace XamCore.SafariServices {

	[Since (7,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSGenericException Misuse of SSReadingList interface. Use class method defaultReadingList.
	public partial interface SSReadingList {

		[NullAllowed]
		[Static, Export ("defaultReadingList")]
		SSReadingList DefaultReadingList { get; }

		[Static, Export ("supportsURL:")]
		// Apple says it's __nonnull so let's be safe and maintain compatibility with our current behaviour
		[PreSnippet ("if (url == null) return false;")]
		bool SupportsUrl ([NullAllowed] NSUrl url);

		[Export ("addReadingListItemWithURL:title:previewText:error:")]
		bool Add (NSUrl url, [NullAllowed] string title, [NullAllowed] string previewText, out NSError error);

		[Field ("SSReadingListErrorDomain")]
		NSString ErrorDomain { get; }
	}

	[iOS (9,0)]
	[BaseType (typeof (NSObject))]
	interface SFContentBlockerManager {
		[Static, Export ("reloadContentBlockerWithIdentifier:completionHandler:")]
		void ReloadContentBlocker (string identifier, [NullAllowed] Action<NSError> completionHandler);
	}
	
	[iOS (9,0)]
	[BaseType (typeof (UIViewController))]
	[DisableDefaultCtor] // NSGenericException Reason: Misuse of SFSafariViewController interface. Use initWithURL:entersReaderIfAvailable:
	interface SFSafariViewController {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[DesignatedInitializer]
		[Export ("initWithURL:entersReaderIfAvailable:")]
		IntPtr Constructor (NSUrl url, bool entersReaderIfAvailable);

		[Export ("initWithURL:")]
		IntPtr Constructor (NSUrl url);

		[NullAllowed] // by default this property is null
		[Export ("delegate", ArgumentSemantic.Assign)]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		SFSafariViewControllerDelegate Delegate { get; set; }
	}

	[iOS (9,0)]
	[Model]
	[BaseType (typeof (NSObject))]
	[Protocol]
	partial interface SFSafariViewControllerDelegate {
		[Export ("safariViewController:activityItemsForURL:title:")]
		UIActivity [] GetActivityItems (SFSafariViewController controller, NSUrl url, [NullAllowed] string title);

		[Export ("safariViewControllerDidFinish:")]
		void DidFinish (SFSafariViewController controller);

		[Export ("safariViewController:didCompleteInitialLoad:")]
		void DidCompleteInitialLoad (SFSafariViewController controller, bool didLoadSuccessfully);
	}
}
