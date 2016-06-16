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
#if !MONOMAC
using XamCore.UIKit;
#else
using XamCore.AppKit;
#endif

namespace XamCore.SafariServices {
	[Mac (10,12)][iOS (10,0)]
	[BaseType (typeof(NSObject))]
	interface SFContentBlockerState
	{
		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; }
	}

	[iOS (9,0)][Mac (10,12)]
	[BaseType (typeof (NSObject))]
	interface SFContentBlockerManager {
		[Static, Export ("reloadContentBlockerWithIdentifier:completionHandler:")]
		void ReloadContentBlocker (string identifier, [NullAllowed] Action<NSError> completionHandler);

		[iOS (10,0)]
		[Static]
		[Export ("getStateOfContentBlockerWithIdentifier:completionHandler:")]
		void GetStateOfContentBlocker (string identifier, Action<SFContentBlockerState, NSError> completionHandler);
	}

#if !MONOMAC
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

	[iOS (10,0)]
	[BaseType (typeof(NSObject))]
	interface SFSafariViewControllerConfiguration
	{
		[Export ("entersReaderIfAvailable")]
		bool EntersReaderIfAvailable { get; set; }

		[Export ("preferredBarTintColor", ArgumentSemantic.Assign)]
		UIColor PreferredBarTintColor { get; set; }
	}
	
	[iOS (9,0)]
	[BaseType (typeof (UIViewController))]
	[DisableDefaultCtor] // NSGenericException Reason: Misuse of SFSafariViewController interface. Use initWithURL:entersReaderIfAvailable:
	interface SFSafariViewController {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Deprecated (PlatformName.iOS, 10, 0, message: "Please use initWithURL:configuration:")]
		[Export ("initWithURL:entersReaderIfAvailable:")]
		IntPtr Constructor (NSUrl url, bool entersReaderIfAvailable);

		[Export ("initWithURL:")]
		IntPtr Constructor (NSUrl url);

		[iOS (10,0)]
		[Export ("initWithURL:configuration:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSUrl URL, SFSafariViewControllerConfiguration configuration);

		[NullAllowed] // by default this property is null
		[Export ("delegate", ArgumentSemantic.Assign)]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		SFSafariViewControllerDelegate Delegate { get; set; }

		[iOS (10, 0)]
		[Export ("configuration", ArgumentSemantic.Copy)]
		SFSafariViewControllerConfiguration Configuration { get; }
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
#else
	[Mac (10,12)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface SFSafariApplication
	{
		[Static]
		[Export ("getActiveWindowWithCompletionHandler:")]
		void GetActiveWindow (Action<SFSafariWindow> completionHandler);

		[Static]
		[Export ("openWindowWithURL:completionHandler:")]
		void OpenWindow (NSUrl url, [NullAllowed] Action<SFSafariWindow> completionHandler);

		[Static]
		[Export ("setToolbarItemsNeedUpdate")]
		void SetToolbarItemsNeedUpdate ();
	}

	[Mac (10,12)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface SFSafariPage
	{
		[Export ("dispatchMessageToScriptWithName:userInfo:")]
		void DispatchMessageToScript (string messageName, [NullAllowed] NSDictionary userInfo);

		[Export ("reload")]
		void Reload ();

		[Export ("getPagePropertiesWithCompletionHandler:")]
		void GetPageProperties (Action<SFSafariPageProperties> completionHandler);
	}

	[Mac (10,12)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface SFSafariExtensionHandling
	{
		[Export ("messageReceivedWithName:fromPage:userInfo:")]
		void MessageReceived (string messageName, SFSafariPage page, [NullAllowed] NSDictionary userInfo);

		[Export ("toolbarItemClickedInWindow:")]
		void ToolbarItemClicked (SFSafariWindow window);

		[Export ("validateToolbarItemInWindow:validationHandler:")]
		void ValidateToolbarItem (SFSafariWindow window, Action<bool, NSString> validationHandler);

		[Export ("contextMenuItemSelectedWithCommand:inPage:userInfo:")]
		void ContextMenuItemSelected (string command, SFSafariPage page, [NullAllowed] NSDictionary userInfo);

		[Export ("popoverWillShowInWindow:")]
		void PopoverWillShow (SFSafariWindow window);

		[Export ("popoverDidCloseInWindow:")]
		void PopoverDidClose (SFSafariWindow window);

		[Export ("popoverViewController")]
		SFSafariExtensionViewController PopoverViewController { get; }
	}

	[Mac (10,12)]
	[BaseType (typeof(NSObject))]
	interface SFSafariPageProperties
	{
		[Export ("url")]
		NSUrl Url { get; }

		[Export ("title")]
		string Title { get; }

		[Export ("usesPrivateBrowsing")]
		bool UsesPrivateBrowsing { get; }

		[Export ("active")]
		bool Active { [Bind ("isActive")] get; }
	}

	[Mac (10,12)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface SFSafariTab
	{
		[Export ("getActivePageWithCompletionHandler:")]
		void GetActivePage (Action<SFSafariPage> completionHandler);

		[Export ("getPagesWithCompletionHandler:")]
		void GetPages (Action<SFSafariPage[]> completionHandler);

		[Export ("activateWithCompletionHandler:")]
		void Activate ([NullAllowed] Action completionHandler);
	}

	[Mac (10,12)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface SFSafariToolbarItem
	{
		[Export ("setEnabled:withBadgeText:")]
		void SetEnabled (bool enabled, [NullAllowed] string badgeText);
	}

	[Mac (10,12)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface SFSafariWindow
	{
		[Export ("getActiveTabWithCompletionHandler:")]
		void GetActiveTab (Action<SFSafariTab> completionHandler);

		[Export ("openTabWithURL:makeActiveIfPossible:completionHandler:")]
		void OpenTab (NSUrl url, bool activateTab, [NullAllowed] Action<SFSafariTab> completionHandler);

		[Export ("getToolbarItemWithCompletionHandler:")]
		void GetToolbarItem (Action<SFSafariToolbarItem> completionHandler);
	}

	[Mac (10,12)]
	[BaseType (typeof(NSViewController))]
	interface SFSafariExtensionViewController
	{
	}

	[Mac (10,12, onlyOn64: true)]
	[BaseType (typeof(NSObject))]
	interface SFSafariExtensionHandler : NSExtensionRequestHandling, SFSafariExtensionHandling
	{
	}

	[Static]
	partial interface SFContentBlocker
	{
		[Mac (10, 12)]
		[Field ("SFContentBlockerErrorDomain")]
		NSString ErrorDomain { get; }
	}
#endif
}
