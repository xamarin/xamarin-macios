//
// safariservices.cs: binding for iOS (7+) SafariServices framework
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//   Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc.
// Copyright 2019 Microsoft Corporation
//

using System;

using Foundation;
using ObjCRuntime;
#if !MONOMAC
using UIKit;
using NSRunningApplication = System.Object;
using NSImage = UIKit.UIImage;
using NSViewController = UIKit.UIViewController;
#else
using AppKit;
using UIImage = AppKit.NSImage;
using UIEventAttribution = Foundation.NSObject;
using UIColor = AppKit.NSColor;
using UIActivity = Foundation.NSObject;
using UIViewController = AppKit.NSViewController;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace SafariServices {
	delegate void SFExtensionValidationHandler (bool shouldHide, NSString text);

	[Introduced (PlatformName.MacCatalyst, 13, 4)]
	[BaseType (typeof (NSObject))]
	interface SFContentBlockerState {
		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; }
	}

	[Introduced (PlatformName.MacCatalyst, 13, 4)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface SFContentBlockerManager {

#if !XAMCORE_5_0
		[Obsolete ("Constructor marked as unavailable.")]
		[Export ("init")]
		NativeHandle Constructor ();
#endif

		[Async]
		[Static, Export ("reloadContentBlockerWithIdentifier:completionHandler:")]
		void ReloadContentBlocker (string identifier, [NullAllowed] Action<NSError> completionHandler);

		[MacCatalyst (13, 4)]
		[Static]
		[Async]
		[Export ("getStateOfContentBlockerWithIdentifier:completionHandler:")]
		void GetStateOfContentBlocker (string identifier, Action<SFContentBlockerState, NSError> completionHandler);
	}

	[NoMac]
	[Introduced (PlatformName.MacCatalyst, 13, 4)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSGenericException Misuse of SSReadingList interface. Use class method defaultReadingList.
	partial interface SSReadingList {

		[NullAllowed]
		[Static, Export ("defaultReadingList")]
		SSReadingList DefaultReadingList { get; }

		[Static, Export ("supportsURL:")]
		// Apple says it's __nonnull so let's be safe and maintain compatibility with our current behaviour
		[PreSnippet ("if (url is null) return false;", Optimizable = true)]
		bool SupportsUrl ([NullAllowed] NSUrl url);

		[Export ("addReadingListItemWithURL:title:previewText:error:")]
		bool Add (NSUrl url, [NullAllowed] string title, [NullAllowed] string previewText, out NSError error);

#if !NET
		[Field ("SSReadingListErrorDomain")]
		NSString ErrorDomain { get; }
#endif
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIViewController))]
	[DisableDefaultCtor] // NSGenericException Reason: Misuse of SFSafariViewController interface. Use initWithURL:entersReaderIfAvailable:
	interface SFSafariViewController {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[MacCatalyst (13, 1)]
		[Export ("initWithURL:configuration:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSUrl url, SFSafariViewControllerConfiguration configuration);

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use '.ctor (NSUrl, SFSafariViewControllerConfiguration)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use '.ctor (NSUrl, SFSafariViewControllerConfiguration)' instead.")]
		[DesignatedInitializer]
		[Export ("initWithURL:entersReaderIfAvailable:")]
		NativeHandle Constructor (NSUrl url, bool entersReaderIfAvailable);

		[Export ("initWithURL:")]
		NativeHandle Constructor (NSUrl url);

		[NullAllowed] // by default this property is null
		[Export ("delegate", ArgumentSemantic.Assign)]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		ISFSafariViewControllerDelegate Delegate { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("preferredBarTintColor", ArgumentSemantic.Assign)]
		UIColor PreferredBarTintColor { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("preferredControlTintColor", ArgumentSemantic.Assign)]
		UIColor PreferredControlTintColor { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("configuration", ArgumentSemantic.Copy)]
		SFSafariViewControllerConfiguration Configuration { get; }

		[MacCatalyst (13, 1)]
		[Export ("dismissButtonStyle", ArgumentSemantic.Assign)]
		SFSafariViewControllerDismissButtonStyle DismissButtonStyle { get; set; }

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Static]
		[Export ("prewarmConnectionsToURLs:")]
		SFSafariViewControllerPrewarmingToken PrewarmConnections (NSUrl [] urls);
	}

	interface ISFSafariViewControllerDelegate { }

	[NoMac]
	[MacCatalyst (13, 1)]
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

		[MacCatalyst (13, 1)]
		[Export ("safariViewController:excludedActivityTypesForURL:title:")]
		string [] GetExcludedActivityTypes (SFSafariViewController controller, NSUrl url, [NullAllowed] string title);

		[MacCatalyst (13, 1)]
		[Export ("safariViewController:initialLoadDidRedirectToURL:")]
		void InitialLoadDidRedirectToUrl (SFSafariViewController controller, NSUrl url);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("safariViewControllerWillOpenInBrowser:")]
		void WillOpenInBrowser (SFSafariViewController controller);
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface SFSafariViewControllerConfiguration : NSCopying {
		[Export ("entersReaderIfAvailable")]
		bool EntersReaderIfAvailable { get; set; }

		[Export ("barCollapsingEnabled")]
		bool BarCollapsingEnabled { get; set; }

		[NullAllowed]
		[iOS (15, 0), MacCatalyst (15, 0), NoMac, NoTV, NoWatch]
		[Export ("activityButton", ArgumentSemantic.Copy)]
		SFSafariViewControllerActivityButton ActivityButton { get; set; }

		[NullAllowed]
		[NoWatch, NoTV, iOS (15, 2), MacCatalyst (15, 2), NoMac]
		[Export ("eventAttribution", ArgumentSemantic.Copy)]
		UIEventAttribution EventAttribution { get; set; }
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	delegate void SFAuthenticationCompletionHandler ([NullAllowed] NSUrl callbackUrl, [NullAllowed] NSError error);

	[NoMac]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'ASWebAuthenticationSession' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ASWebAuthenticationSession' instead.")]
	interface SFAuthenticationSession {
		[Export ("initWithURL:callbackURLScheme:completionHandler:")]
		NativeHandle Constructor (NSUrl url, [NullAllowed] string callbackUrlScheme, SFAuthenticationCompletionHandler completionHandler);

		[Export ("start")]
		bool Start ();

		[Export ("cancel")]
		void Cancel ();
	}

	[NoiOS]
	[NoTV]
	[NoWatch]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SFSafariApplication {
		[Static]
		[Async]
		[Export ("getActiveWindowWithCompletionHandler:")]
		void GetActiveWindow (Action<SFSafariWindow> completionHandler);

		[Static]
		[Async]
		[Export ("getAllWindowsWithCompletionHandler:")]
		void GetAllWindows (Action<SFSafariWindow []> completionHandler);

		[Static]
		[Async]
		[Export ("openWindowWithURL:completionHandler:")]
		void OpenWindow (NSUrl url, [NullAllowed] Action<SFSafariWindow> completionHandler);

		[Static]
		[Export ("setToolbarItemsNeedUpdate")]
		void SetToolbarItemsNeedUpdate ();

		[Static]
		[Export ("showPreferencesForExtensionWithIdentifier:completionHandler:")]
		void ShowPreferencesForExtension (string identifier, [NullAllowed] Action<NSError> completionHandler);

		[Advice ("Unavailable to extensions.")]
		[Static]
		[Async]
		[Export ("dispatchMessageWithName:toExtensionWithIdentifier:userInfo:completionHandler:")]
		void DispatchMessage (string messageName, string identifier, [NullAllowed] NSDictionary<NSString, NSObject> userInfo, [NullAllowed] Action<NSError> completionHandler);

		[Static]
		[Async]
		[Export ("getHostApplicationWithCompletionHandler:")]
		void GetHostApplication (Action<NSRunningApplication> completionHandler);
	}

	[NoiOS]
	[NoTV]
	[NoWatch]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SFSafariPage : NSSecureCoding, NSCopying {
		[Export ("dispatchMessageToScriptWithName:userInfo:")]
		void DispatchMessageToScript (string messageName, [NullAllowed] NSDictionary userInfo);

		[Export ("reload")]
		void Reload ();

		[Async]
		[Export ("getPagePropertiesWithCompletionHandler:")]
		void GetPageProperties (Action<SFSafariPageProperties> completionHandler);

		[Async]
		[Export ("getContainingTabWithCompletionHandler:")]
		void GetContainingTab (Action<SFSafariTab> completionHandler);

		[Async]
		[Export ("getScreenshotOfVisibleAreaWithCompletionHandler:")]
		void GetScreenshotOfVisibleArea (Action<NSImage> completionHandler);
	}

	[NoiOS]
	[NoTV]
	[NoWatch]
	[NoMacCatalyst]
	[Protocol]
	interface SFSafariExtensionHandling {
		[Export ("messageReceivedWithName:fromPage:userInfo:")]
		void MessageReceived (string messageName, SFSafariPage page, [NullAllowed] NSDictionary userInfo);

		[Export ("toolbarItemClickedInWindow:")]
		void ToolbarItemClicked (SFSafariWindow window);

		[Async (ResultTypeName = "SFValidationResult")]
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

		[Async (ResultTypeName = "SFExtensionValidationResult")]
		[Export ("validateContextMenuItemWithCommand:inPage:userInfo:validationHandler:")]
		void ValidateContextMenuItem (string command, SFSafariPage page, [NullAllowed] NSDictionary<NSString, NSObject> userInfo, SFExtensionValidationHandler validationHandler);

		[Export ("messageReceivedFromContainingAppWithName:userInfo:")]
		void MessageReceivedFromContainingApp (string messageName, [NullAllowed] NSDictionary<NSString, NSObject> userInfo);

		[Export ("additionalRequestHeadersForURL:completionHandler:")]
		void AdditionalRequestHeaders (NSUrl url, Action<NSDictionary<NSString, NSString>> completionHandler);

		[Export ("contentBlockerWithIdentifier:blockedResourcesWithURLs:onPage:")]
		void ContentBlocker (string contentBlockerIdentifier, NSUrl [] urls, SFSafariPage page);

		[Export ("page:willNavigateToURL:")]
		void WillNavigate (SFSafariPage page, [NullAllowed] NSUrl url);
	}

	[NoiOS]
	[NoTV]
	[NoWatch]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SFSafariExtension {

		[Static]
		[Async]
		[Export ("getBaseURIWithCompletionHandler:")]
		void GetBaseUri (Action<NSUrl> completionHandler);
	}

	[NoiOS]
	[NoTV]
	[NoWatch]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	interface SFSafariPageProperties {
		[NullAllowed]
		[Export ("url")]
		NSUrl Url { get; }

		[NullAllowed]
		[Export ("title")]
		string Title { get; }

		[Export ("usesPrivateBrowsing")]
		bool UsesPrivateBrowsing { get; }

		[Export ("active")]
		bool Active { [Bind ("isActive")] get; }
	}

	[NoiOS]
	[NoTV]
	[NoWatch]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SFSafariTab : NSSecureCoding, NSCopying {
		[Async]
		[Export ("getActivePageWithCompletionHandler:")]
		void GetActivePage (Action<SFSafariPage> completionHandler);

		[Async]
		[Export ("getPagesWithCompletionHandler:")]
		void GetPages (Action<SFSafariPage []> completionHandler);

		[Async]
		[Export ("getContainingWindowWithCompletionHandler:")]
		void GetContainingWindow (Action<SFSafariWindow> completionHandler);

		[Async]
		[Export ("activateWithCompletionHandler:")]
		void Activate ([NullAllowed] Action completionHandler);

		[Export ("navigateToURL:")]
		void NavigateTo (NSUrl url);

		[Export ("close")]
		void Close ();
	}

	[NoiOS]
	[NoTV]
	[NoWatch]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SFSafariToolbarItem : NSSecureCoding, NSCopying {
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'SetEnabled (bool)' or 'SetBadgeText' instead.")]
		[Export ("setEnabled:withBadgeText:")]
		void SetEnabled (bool enabled, [NullAllowed] string badgeText);

		[Export ("setEnabled:")]
		void SetEnabled (bool enabled);

		[Export ("setBadgeText:")]
		void SetBadgeText ([NullAllowed] string badgeText);

		[Export ("setImage:")]
		void SetImage ([NullAllowed] NSImage image);

		[Export ("setLabel:")]
		void SetLabel ([NullAllowed] string label);

		[Export ("showPopover")]
		void ShowPopover ();
	}

	[NoiOS]
	[NoTV]
	[NoWatch]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SFSafariWindow : NSSecureCoding, NSCopying {
		[Async]
		[Export ("getActiveTabWithCompletionHandler:")]
		void GetActiveTab (Action<SFSafariTab> completionHandler);

		[Async]
		[Export ("getAllTabsWithCompletionHandler:")]
		void GetAllTabs (Action<SFSafariTab []> completionHandler);

		[Async]
		[Export ("openTabWithURL:makeActiveIfPossible:completionHandler:")]
		void OpenTab (NSUrl url, bool activateTab, [NullAllowed] Action<SFSafariTab> completionHandler);

		[Async]
		[Export ("getToolbarItemWithCompletionHandler:")]
		void GetToolbarItem (Action<SFSafariToolbarItem> completionHandler);

		[Export ("close")]
		void Close ();
	}

	[NoiOS]
	[NoTV]
	[NoWatch]
	[NoMacCatalyst]
	[BaseType (typeof (NSViewController))]
	interface SFSafariExtensionViewController {
		[Export ("initWithNibName:bundle:")]
		NativeHandle Constructor ([NullAllowed] string nibNameOrNull, [NullAllowed] NSBundle nibBundleOrNull);

		[Export ("dismissPopover")]
		void DismissPopover ();
	}

	[NoiOS]
	[NoTV]
	[NoWatch]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	interface SFSafariExtensionHandler : NSExtensionRequestHandling, SFSafariExtensionHandling {
	}

	//	TODO - Needs Safari Extension support to test
	// 	
	// 	[BaseType (typeof(NSObject))]
	// 	interface SFSafariExtensionManager
	// 	{
	// 		[Static]
	// 		[Export ("getStateOfSafariExtensionWithIdentifier:completionHandler:")]
	// 		void GetStateOfSafariExtension (string identifier, Action<SFSafariExtensionState, NSError> completionHandler);
	// 	}
	//
	// 	
	// 	[BaseType (typeof(NSObject))]
	// 	interface SFSafariExtensionState
	// 	{
	// 		[Export ("enabled")]
	// 		bool Enabled { [Bind ("isEnabled")] get; }
	// 	}

	[NoiOS]
	[NoTV]
	[NoWatch]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SFUniversalLink {

		[Export ("initWithWebpageURL:")]
		NativeHandle Constructor (NSUrl url);

		[Export ("webpageURL")]
		NSUrl WebpageUrl { get; }

		[Export ("applicationURL")]
		NSUrl ApplicationUrl { get; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }
	}

	[Static]
	[iOS (15, 0), MacCatalyst (15, 0), NoTV, NoWatch]
	[DisableDefaultCtor]
	interface SFExtension {
		[Field ("SFExtensionMessageKey")]
		NSString MessageKey { get; }

		[iOS (17, 0), Mac (14, 0), NoMacCatalyst]
		[Field ("SFExtensionProfileKey")]
		NSString ProfileKey { get; }
	}

	[iOS (15, 0), MacCatalyst (15, 0), NoMac, NoTV, NoWatch]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SFSafariViewControllerActivityButton : NSCopying, NSSecureCoding {
		[Export ("initWithTemplateImage:extensionIdentifier:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIImage templateImage, string extensionIdentifier);

		[NullAllowed, Export ("templateImage", ArgumentSemantic.Copy)]
		UIImage TemplateImage { get; }

		[NullAllowed, Export ("extensionIdentifier")]
		string ExtensionIdentifier { get; }
	}

	[iOS (15, 0), MacCatalyst (15, 0), NoMac, NoTV, NoWatch]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SFSafariViewControllerPrewarmingToken /* Privately conforms to NSCoding and NSSecureCoding */
	{
		[Export ("invalidate")]
		void Invalidate ();
	}

	[iOS (16, 0), MacCatalyst (16, 0), NoMac, NoTV, NoWatch]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SFSafariViewControllerDataStore {
		[Static]
		[Export ("defaultDataStore", ArgumentSemantic.Strong)]
		SFSafariViewControllerDataStore DefaultDataStore { get; }

		[Async]
		[Export ("clearWebsiteDataWithCompletionHandler:")]
		void ClearWebsiteData ([NullAllowed] Action completion);
	}

}
