//
// wkwebkit.cs: the "modern" (insanely limited) WebKit
//   API introduced in iOS 8.0 and Mac 10.10
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2014-2015 Xamarin Inc. All rights reserved.
//

using System;

using ObjCRuntime;
using Foundation;
using CoreGraphics;
using Security;
#if MONOMAC
using AppKit;
using UIColor=AppKit.NSColor;
using UIPreviewActionItem = Foundation.NSObject;
using UIScrollView = AppKit.NSScrollView;
using UIImage = AppKit.NSImage;
using IUIContextMenuInteractionCommitAnimating = Foundation.NSObject;
using UIContextMenuConfiguration = Foundation.NSObject;
using UIViewController = AppKit.NSViewController;
using IWKPreviewActionItem = Foundation.NSObject;
#else
using UIKit;
using NSPrintInfo = Foundation.NSObject;
using NSPrintOperation = Foundation.NSObject;
using NSEventModifierMask = System.Object;
using NSImage = UIKit.UIImage;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace WebKit
{
	[Mac (12,3), iOS (15,4), MacCatalyst (15,4)]
	[Native]
	public enum WKFullscreenState : long {
		NotInFullscreen,
		EnteringFullscreen,
		InFullscreen,
		ExitingFullscreen,
	}

	[iOS (8,0), Mac (10,10)] // Not defined in 32-bit
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor ()] // Crashes during deallocation in Xcode 6 beta 2. radar 17377712.
	interface WKBackForwardListItem {

		[Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get; }

		[Export ("title")]
		[NullAllowed]
		string Title { get; }

		[Export ("initialURL", ArgumentSemantic.Copy)]
		NSUrl InitialUrl { get; }
	}

	[iOS (8,0), Mac (10,10)] // Not defined in 32-bit
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor ()] // Crashes during deallocation in Xcode 6 beta 2. radar 17377712.
	interface WKBackForwardList {

		[Export ("currentItem", ArgumentSemantic.Strong)]
		[NullAllowed]
		WKBackForwardListItem CurrentItem { get; }

		[Export ("backItem", ArgumentSemantic.Strong)]
		[NullAllowed]
		WKBackForwardListItem BackItem { get; }

		[Export ("forwardItem", ArgumentSemantic.Strong)]
		[NullAllowed]
		WKBackForwardListItem ForwardItem { get; }

		[Export ("backList")]
		WKBackForwardListItem [] BackList { get; }

		[Export ("forwardList")]
		WKBackForwardListItem [] ForwardList { get; }

		[Export ("itemAtIndex:")]
		[return: NullAllowed]
		WKBackForwardListItem ItemAtIndex (nint index);
	}

	[Mac (10,13), iOS (11,0)]
	[BaseType (typeof(NSObject))]
	interface WKContentRuleList
	{
		[Export ("identifier")]
		string Identifier { get; }
	}

	[Mac (10,13), iOS (11,0)]
	[BaseType (typeof(NSObject))]
	interface WKContentRuleListStore
	{
		[Static]
		[Export ("defaultStore")]
		WKContentRuleListStore DefaultStore { get; }
	
		[Static]
		[Export ("storeWithURL:")]
		WKContentRuleListStore FromUrl (NSUrl url);
	
		[Export ("compileContentRuleListForIdentifier:encodedContentRuleList:completionHandler:")]
		[Async]
		void CompileContentRuleList (string identifier, string encodedContentRuleList, Action<WKContentRuleList, NSError> completionHandler);
	
		[Export ("lookUpContentRuleListForIdentifier:completionHandler:")]
		[Async]
		void LookUpContentRuleList (string identifier, Action<WKContentRuleList, NSError> completionHandler);
	
		[Export ("removeContentRuleListForIdentifier:completionHandler:")]
		[Async]
		void RemoveContentRuleList (string identifier, Action<NSError> completionHandler);
	
		[Export ("getAvailableContentRuleListIdentifiers:")]
		[Async]
		void GetAvailableContentRuleListIdentifiers (Action<string []> callback);
	}
	
	[Mac (10,13), iOS (11,0)]
	[BaseType (typeof(NSObject), Name = "WKHTTPCookieStore")]
	[DisableDefaultCtor]
	interface WKHttpCookieStore
	{
		[Export ("getAllCookies:")]
		[Async]
		void GetAllCookies (Action<NSHttpCookie []> completionHandler);
	
		[Export ("setCookie:completionHandler:")]
		[Async]
		void SetCookie (NSHttpCookie cookie, [NullAllowed] Action completionHandler);
	
		[Export ("deleteCookie:completionHandler:")]
		[Async]
		void DeleteCookie (NSHttpCookie cookie, [NullAllowed] Action completionHandler);
	
		[Export ("addObserver:")]
		void AddObserver (IWKHttpCookieStoreObserver observer);
	
		[Export ("removeObserver:")]
		void RemoveObserver (IWKHttpCookieStoreObserver observer);
	}

	interface IWKHttpCookieStoreObserver {}
	
	[Mac (10,13), iOS (11,0)]
	[Protocol (Name = "WKHTTPCookieStoreObserver")]
	interface WKHttpCookieStoreObserver
	{
		[Export ("cookiesDidChangeInCookieStore:")]
		void CookiesDidChangeInCookieStore (WKHttpCookieStore cookieStore);
	}
	
	[iOS (8,0), Mac (10,10)] // Not defined in 32-bit
	[BaseType (typeof (NSObject))]
	interface WKFrameInfo : NSCopying {

		[Export ("mainFrame")]
		bool MainFrame { [Bind ("isMainFrame")] get; }

		[Export ("request", ArgumentSemantic.Copy)]
		NSUrlRequest Request { get; }

		[iOS (9,0)][Mac (10,11)]
		[Export ("securityOrigin")]
		WKSecurityOrigin SecurityOrigin { get; }

		[iOS (11,0)][Mac (10,13)]
		[NullAllowed, Export ("webView", ArgumentSemantic.Weak)]
		WKWebView WebView { get; }
	}

	[iOS (8,0), Mac (10,10)] // Not defined in 32-bit
	[BaseType (typeof (NSObject))]
	interface WKNavigation {

		[Mac (10,15)]
		[iOS (13,0)]
		[Export ("effectiveContentMode")]
		WKContentMode EffectiveContentMode { get; }
	}

	[iOS (8,0), Mac (10,10)] // Not defined in 32-bit
	[BaseType (typeof (NSObject))]
	interface WKNavigationAction {

		[Export ("sourceFrame", ArgumentSemantic.Copy)]
		WKFrameInfo SourceFrame { get; }

		[Export ("targetFrame", ArgumentSemantic.Copy)]
		[NullAllowed]
		WKFrameInfo TargetFrame { get; }

		[Export ("navigationType")]
		WKNavigationType NavigationType { get; }

		[Export ("request", ArgumentSemantic.Copy)]
		NSUrlRequest Request { get; }

		[NoiOS][NoMacCatalyst]
		[Export ("modifierFlags")]
		NSEventModifierMask ModifierFlags { get; }

		[NoiOS][NoMacCatalyst]
		[Export ("buttonNumber")]
		nint ButtonNumber { get; }

		[Mac (11,3)][iOS (14,5)]
		[MacCatalyst (14,5)]
		[Export ("shouldPerformDownload")]
		bool ShouldPerformDownload { get; }
	}

	[Mac (10,10), iOS (8,0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface WKNavigationDelegate {

		[Export ("webView:decidePolicyForNavigationAction:decisionHandler:")]
		void DecidePolicy (WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler);

		[Export ("webView:decidePolicyForNavigationResponse:decisionHandler:")]
		void DecidePolicy (WKWebView webView, WKNavigationResponse navigationResponse, Action<WKNavigationResponsePolicy> decisionHandler);

		[Mac (10,15)]
		[iOS (13,0)]
		[Export ("webView:decidePolicyForNavigationAction:preferences:decisionHandler:")]
		void DecidePolicy (WKWebView webView, WKNavigationAction navigationAction, WKWebpagePreferences preferences, Action<WKNavigationActionPolicy, WKWebpagePreferences> decisionHandler);

		[Export ("webView:didStartProvisionalNavigation:")]
		void DidStartProvisionalNavigation (WKWebView webView, WKNavigation navigation);

		[Export ("webView:didReceiveServerRedirectForProvisionalNavigation:")]
		void DidReceiveServerRedirectForProvisionalNavigation (WKWebView webView, WKNavigation navigation);

		[Export ("webView:didFailProvisionalNavigation:withError:")]
		void DidFailProvisionalNavigation (WKWebView webView, WKNavigation navigation, NSError error);

		[Export ("webView:didCommitNavigation:")]
		void DidCommitNavigation (WKWebView webView, WKNavigation navigation);

		[Export ("webView:didFinishNavigation:")]
		void DidFinishNavigation (WKWebView webView, WKNavigation navigation);

		[Export ("webView:didFailNavigation:withError:")]
		void DidFailNavigation (WKWebView webView, WKNavigation navigation, NSError error);

		[Export ("webView:didReceiveAuthenticationChallenge:completionHandler:")]
		void DidReceiveAuthenticationChallenge (WKWebView webView, NSUrlAuthenticationChallenge challenge, Action<NSUrlSessionAuthChallengeDisposition,NSUrlCredential> completionHandler);

		[iOS (9,0)][Mac (10,11)]
		[Export ("webViewWebContentProcessDidTerminate:")]
		void ContentProcessDidTerminate (WKWebView webView);

		[Mac (11,0)][iOS (14,0)]
		[Export ("webView:authenticationChallenge:shouldAllowDeprecatedTLS:")]
		void ShouldAllowDeprecatedTls (WKWebView webView, NSUrlAuthenticationChallenge challenge, Action<bool> decisionHandler);

		[Mac (11,3)][iOS (14,5)]
		[MacCatalyst (14,5)]
		[Export ("webView:navigationAction:didBecomeDownload:")]
		void NavigationActionDidBecomeDownload (WKWebView webView, WKNavigationAction navigationAction, WKDownload download);

		[Mac (11,3)][iOS (14,5)]
		[MacCatalyst (14,5)]
		[Export ("webView:navigationResponse:didBecomeDownload:")]
		void NavigationResponseDidBecomeDownload (WKWebView webView, WKNavigationResponse navigationResponse, WKDownload download);
	}

	[iOS (8,0), Mac (10,10)] // Not defined in 32-bit
	[BaseType (typeof (NSObject))]
	interface WKNavigationResponse {

		[Export ("forMainFrame")]
		bool IsForMainFrame { [Bind ("isForMainFrame")] get; }

		[Export ("response", ArgumentSemantic.Copy)]
		NSUrlResponse Response { get; }

		[Export ("canShowMIMEType")]
		bool CanShowMimeType { get; }
	}

	[iOS (8,0), Mac (10,10)] // Not defined in 32-bit
	[BaseType (typeof (NSObject))]
	interface WKPreferences : NSSecureCoding {
		[Export ("minimumFontSize")]
		nfloat MinimumFontSize { get; set; }

		[Deprecated (PlatformName.MacOSX, 11,0, message: "Use 'WKWebPagePreferences.AllowsContentJavaScript' instead.")]
		[Deprecated (PlatformName.iOS, 14,0, message: "Use 'WKWebPagePreferences.AllowsContentJavaScript' instead.")]
		[Export ("javaScriptEnabled")]
		bool JavaScriptEnabled { get; set; }

		[Export ("javaScriptCanOpenWindowsAutomatically")]
		bool JavaScriptCanOpenWindowsAutomatically { get; set; }

		[NoiOS][NoMacCatalyst]
		[Deprecated (PlatformName.MacOSX, 10,15, message: "Feature no longer supported.")]
		[Export ("javaEnabled")]
		bool JavaEnabled { get; set; }

		[NoiOS][NoMacCatalyst]
		[Deprecated (PlatformName.MacOSX, 10,15, message: "Feature no longer supported.")]
		[Export ("plugInsEnabled")]
		bool PlugInsEnabled { get; set; }

		// Headers says 10,12,3 but it is not available likely they meant 10,12,4
		[NoiOS][NoMacCatalyst]
		[Mac (10,12,4)]
		[Export ("tabFocusesLinks")]
		bool TabFocusesLinks { get; set; }

		[Mac (10, 15), iOS (13, 0)]
		[Export ("fraudulentWebsiteWarningEnabled")]
		bool FraudulentWebsiteWarningEnabled { [Bind ("isFraudulentWebsiteWarningEnabled")] get; set; }

		[Internal]
		[Mac (11,3)][iOS (14,5)]
		[MacCatalyst (14,5)]
		[Export ("textInteractionEnabled")]
		bool _OldTextInteractionEnabled { get; set; }

		[Internal]
		[Mac (12,0)][iOS (15,0)]
		[MacCatalyst (15,0)]
		[Export ("isTextInteractionEnabled")]
		bool _NewGetTextInteractionEnabled ();

		[Mac (12,3), iOS (15,4), MacCatalyst (15,4)]
		[Export ("siteSpecificQuirksModeEnabled")]
		bool SiteSpecificQuirksModeEnabled { [Bind ("isSiteSpecificQuirksModeEnabled")] get; set; }

		[Mac (12,3), iOS (15,4), MacCatalyst (15,4)]
		[Export ("elementFullscreenEnabled")]
		bool ElementFullscreenEnabled { [Bind ("isElementFullscreenEnabled")] get; set; }
	}

	[iOS (8,0), Mac (10,10)] // Not defined in 32-bit
	[BaseType (typeof (NSObject))]
	interface WKScriptMessage {

		// May be typed as NSNumber, NSString, NSDate, NSArray,
		// NSDictionary, or NSNull, as it must map cleanly to JSON
		[Export ("body", ArgumentSemantic.Copy)]
		NSObject Body { get; }

		[Export ("webView", ArgumentSemantic.Weak)]
		[NullAllowed]
		WKWebView WebView { get; }

		[Export ("name")]
		string Name { get; }

		[Export ("frameInfo", ArgumentSemantic.Copy)]
		WKFrameInfo FrameInfo { get; }

		[Mac (11,0)][iOS (14,0)]
		[MacCatalyst (14,0)]
		[Export ("world")]
		WKContentWorld World { get; }
	}

	interface IWKScriptMessageHandler {}

	[Mac (10,10), iOS (8,0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface WKScriptMessageHandler {

		[Export ("userContentController:didReceiveScriptMessage:")]
		[Abstract]
		void DidReceiveScriptMessage (WKUserContentController userContentController, WKScriptMessage message);
	}

	[iOS (9,0)][Mac(10,11)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface WKSecurityOrigin {
		[Export ("protocol")]
		string Protocol { get; }

		[Export ("host")]
		string Host { get; }

		[Export ("port")]
		nint Port { get; }
	}

	
	[Mac (10,13), iOS (11,0)]
	[BaseType (typeof(NSObject))]
	interface WKSnapshotConfiguration : NSCopying {
		[Export ("rect")]
		CGRect Rect { get; set; }

		[Export ("snapshotWidth")]
		[NullAllowed]
		NSNumber SnapshotWidth { get; set; }

		[Mac (10,15)]
		[iOS (13,0)]
		[Export ("afterScreenUpdates")]
		bool AfterScreenUpdates { get; set; }
	}

	interface IWKUrlSchemeHandler {}
	[Mac (10,13), iOS (11,0)]
	[Protocol (Name = "WKURLSchemeHandler")]
	interface WKUrlSchemeHandler
	{
		[Abstract]
		[Export ("webView:startURLSchemeTask:")]
		void StartUrlSchemeTask (WKWebView webView, IWKUrlSchemeTask urlSchemeTask);
	
		[Abstract]
		[Export ("webView:stopURLSchemeTask:")]
		void StopUrlSchemeTask (WKWebView webView, IWKUrlSchemeTask urlSchemeTask);
	}

	interface IWKUrlSchemeTask {}

	[Mac (10,13), iOS (11,0)]
	[Protocol (Name = "WKURLSchemeTask")]
	interface WKUrlSchemeTask
	{
		[Abstract]
		[Export ("request", ArgumentSemantic.Copy)]
		NSUrlRequest Request { get; }
	
		[Abstract]
		[Export ("didReceiveResponse:")]
		void DidReceiveResponse (NSUrlResponse response);
	
		[Abstract]
		[Export ("didReceiveData:")]
		void DidReceiveData (NSData data);
	
		[Abstract]
		[Export ("didFinish")]
		void DidFinish ();
	
		[Abstract]
		[Export ("didFailWithError:")]
		void DidFailWithError (NSError error);
	}
	
	[iOS (9,0), Mac(10,11)]
	[BaseType (typeof(NSObject))]
	interface WKWebsiteDataRecord
	{
		[Export ("displayName")]
		string DisplayName { get; }
	
		[Export ("dataTypes", ArgumentSemantic.Copy)]
		NSSet<NSString> DataTypes { get; }
	}

	[iOS (9,0), Mac(10,11)]
	[Static]
	interface WKWebsiteDataType {
		[Field ("WKWebsiteDataTypeDiskCache", "WebKit")]
		NSString DiskCache { get; }
		
		[Field ("WKWebsiteDataTypeMemoryCache", "WebKit")]
		NSString MemoryCache { get; }
		
		[Field ("WKWebsiteDataTypeOfflineWebApplicationCache", "WebKit")]
		NSString OfflineWebApplicationCache { get; }
		
		[Field ("WKWebsiteDataTypeCookies", "WebKit")]
		NSString Cookies { get; }

		[Field ("WKWebsiteDataTypeSessionStorage")]
		NSString SessionStorage { get; }
		
		[Field ("WKWebsiteDataTypeLocalStorage", "WebKit")]
		NSString LocalStorage { get; }
		
		[Field ("WKWebsiteDataTypeWebSQLDatabases", "WebKit")]
		NSString WebSQLDatabases { get; }
		
		[Field ("WKWebsiteDataTypeIndexedDBDatabases", "WebKit")]
		NSString IndexedDBDatabases { get; }

		[Mac (10, 13, 4), iOS (11, 3)]
		[Field ("WKWebsiteDataTypeFetchCache")]
		NSString FetchCache { get; }

		[Mac (10, 13, 4), iOS (11, 3)]
		[Field ("WKWebsiteDataTypeServiceWorkerRegistrations")]
		NSString ServiceWorkerRegistrations { get; }
	}
	
	[iOS (9,0), Mac(10,11)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor] // NSGenericException Reason: Calling [WKWebsiteDataStore init] is not supported.
	interface WKWebsiteDataStore : NSSecureCoding {

		[Static]
		[Export ("defaultDataStore")]
		WKWebsiteDataStore DefaultDataStore { get; }
	
		[Static]
		[Export ("nonPersistentDataStore")]
		WKWebsiteDataStore NonPersistentDataStore { get; }
	
		[Export ("persistent")]
		bool Persistent { [Bind ("isPersistent")] get; }
	
		[Static]
		[Export ("allWebsiteDataTypes")]
		NSSet<NSString> AllWebsiteDataTypes { get; }
	
		[Export ("fetchDataRecordsOfTypes:completionHandler:")]
		[Async]
		void FetchDataRecordsOfTypes (NSSet<NSString> dataTypes, Action<NSArray> completionHandler);
	
		[Export ("removeDataOfTypes:forDataRecords:completionHandler:")]
		[Async]
		void RemoveDataOfTypes (NSSet<NSString> dataTypes, WKWebsiteDataRecord[] dataRecords, Action completionHandler);
	
		[Export ("removeDataOfTypes:modifiedSince:completionHandler:")]
		[Async]
		void RemoveDataOfTypes (NSSet<NSString> websiteDataTypes, NSDate date, Action completionHandler);

		[Mac (10, 13), iOS (11, 0)]
		[Export ("httpCookieStore")]
		WKHttpCookieStore HttpCookieStore { get; }
	}

	[Mac (10,12)][NoiOS, NoWatch, NoTV]
	[BaseType (typeof(NSObject))]
	interface WKOpenPanelParameters	{
		[Export ("allowsMultipleSelection")]
		bool AllowsMultipleSelection { get; }

		[Mac (10, 13, 4)]
		[Export ("allowsDirectories")]
		bool AllowsDirectories { get; }
	}
	
	[Mac (10,10), iOS (8,0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface WKUIDelegate {

		[Export ("webView:createWebViewWithConfiguration:forNavigationAction:windowFeatures:")]
		[return: NullAllowed]
		WKWebView CreateWebView (WKWebView webView, WKWebViewConfiguration configuration,
			WKNavigationAction navigationAction, WKWindowFeatures windowFeatures);

		[Export ("webView:runJavaScriptAlertPanelWithMessage:initiatedByFrame:completionHandler:")]
		void RunJavaScriptAlertPanel (WKWebView webView, string message, WKFrameInfo frame, Action completionHandler);

		[Export ("webView:runJavaScriptConfirmPanelWithMessage:initiatedByFrame:completionHandler:")]
		void RunJavaScriptConfirmPanel (WKWebView webView, string message, WKFrameInfo frame, Action<bool> completionHandler);

		[Export ("webView:runJavaScriptTextInputPanelWithPrompt:defaultText:initiatedByFrame:completionHandler:")]
		void RunJavaScriptTextInputPanel (WKWebView webView, string prompt, [NullAllowed] string defaultText,
			WKFrameInfo frame, Action<string> completionHandler);

		[Mac (10,12)][NoiOS, NoWatch, NoTV]
		[Export ("webView:runOpenPanelWithParameters:initiatedByFrame:completionHandler:")]
		void RunOpenPanel (WKWebView webView, WKOpenPanelParameters parameters, WKFrameInfo frame, Action<NSUrl[]> completionHandler);

		[iOS (9,0)][Mac (10,11)]
		[Export ("webViewDidClose:")]
		void DidClose (WKWebView webView);

		[iOS (10,0)][NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'SetContextMenuConfiguration' instead.")]
		[Export ("webView:shouldPreviewElement:")]
		bool ShouldPreviewElement (WKWebView webView, WKPreviewElementInfo elementInfo);

		[iOS (10,0)][NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'SetContextMenuConfiguration' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SetContextMenuConfiguration' instead.")]
		[Export ("webView:previewingViewControllerForElement:defaultActions:")]
		[return: NullAllowed]
		UIViewController GetPreviewingViewController (WKWebView webView, WKPreviewElementInfo elementInfo, IWKPreviewActionItem[] previewActions);

		[iOS (10,0)][NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'WillCommitContextMenu' instead.")]
		[Export ("webView:commitPreviewingViewController:")]
		void CommitPreviewingViewController (WKWebView webView, UIViewController previewingViewController);

		[MacCatalyst (13,1)]
		[iOS (13,0)][NoMac]
		[Export ("webView:contextMenuConfigurationForElement:completionHandler:")]
		void SetContextMenuConfiguration (WKWebView webView, WKContextMenuElementInfo elementInfo, Action<UIContextMenuConfiguration> completionHandler);

		[MacCatalyst (13,1)]
		[iOS (13,0)][NoMac]
		[Export ("webView:contextMenuForElement:willCommitWithAnimator:")]
		void WillCommitContextMenu (WKWebView webView, WKContextMenuElementInfo elementInfo, IUIContextMenuInteractionCommitAnimating animator);

		[iOS (13,0)][NoMac]
		[Export ("webView:contextMenuWillPresentForElement:")]
		void ContextMenuWillPresent (WKWebView webView, WKContextMenuElementInfo elementInfo);

		[iOS (13,0)][NoMac]
		[Export ("webView:contextMenuDidEndForElement:")]
		void ContextMenuDidEnd (WKWebView webView, WKContextMenuElementInfo elementInfo);
		
		[Async]
		[NoMac, NoTV, iOS (15,0), MacCatalyst (15,0)]
		[Export ("webView:requestDeviceOrientationAndMotionPermissionForOrigin:initiatedByFrame:decisionHandler:")]
		void RequestDeviceOrientationAndMotionPermission (WKWebView webView, WKSecurityOrigin origin, WKFrameInfo frame, Action<WKPermissionDecision> decisionHandler);
		
		[Async]
		[Mac (12,0), iOS (15,0), MacCatalyst (15,0), NoTV]
		[Export ("webView:requestMediaCapturePermissionForOrigin:initiatedByFrame:type:decisionHandler:")]
		void RequestMediaCapturePermission (WKWebView webView, WKSecurityOrigin origin, WKFrameInfo frame, WKMediaCaptureType type, Action<WKPermissionDecision> decisionHandler);
	}

	[iOS (8,0), Mac (10,10)] // Not defined in 32-bit
	[BaseType (typeof (NSObject))]
	interface WKUserContentController : NSSecureCoding {

		[Export ("userScripts")]
		WKUserScript [] UserScripts { get; }

		[Export ("addUserScript:")]
		void AddUserScript (WKUserScript userScript);

		[Export ("removeAllUserScripts")]
		void RemoveAllUserScripts ();

		[Export ("addScriptMessageHandler:name:")]
		void AddScriptMessageHandler ([Protocolize] WKScriptMessageHandler scriptMessageHandler, string name);

		[Mac (11,0), iOS (14,0)]
		[MacCatalyst (14,0)]
		[Export ("addScriptMessageHandler:contentWorld:name:")]
		void AddScriptMessageHandler (IWKScriptMessageHandler scriptMessageHandler, WKContentWorld world, string name);

		[Mac (11,0), iOS (14,0)]
		[MacCatalyst (14,0)]
		[Export ("addScriptMessageHandlerWithReply:contentWorld:name:")]
		void AddScriptMessageHandler (IWKScriptMessageHandlerWithReply scriptMessageHandlerWithReply, WKContentWorld contentWorld, string name);

		[Export ("removeScriptMessageHandlerForName:")]
		void RemoveScriptMessageHandler (string name);

		[Mac (11,0), iOS (14,0)]
		[MacCatalyst (14,0)]
		[Export ("removeScriptMessageHandlerForName:contentWorld:")]
		void RemoveScriptMessageHandler (string name, WKContentWorld contentWorld);

		[Mac (10,13), iOS (11,0)]
		[Export ("addContentRuleList:")]
		void AddContentRuleList (WKContentRuleList contentRuleList);
	
		[Mac (10,13), iOS (11,0)]
		[Export ("removeContentRuleList:")]
		void RemoveContentRuleList (WKContentRuleList contentRuleList);
	
		[Mac (10,13), iOS (11,0)]
		[Export ("removeAllContentRuleLists")]
		void RemoveAllContentRuleLists ();

		[Mac (11,0), iOS (14,0)]
		[MacCatalyst (14,0)]
		[Export ("removeAllScriptMessageHandlersFromContentWorld:")]
		void RemoveAllScriptMessageHandlers (WKContentWorld contentWorld);

		[Mac (11,0), iOS (14,0)]
		[Export ("removeAllScriptMessageHandlers")]
		void RemoveAllScriptMessageHandlers ();
	}

	[iOS (8,0), Mac (10,10)] // Not defined in 32-bit
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // all properties are getters
	interface WKUserScript : NSCopying {

		[Export ("initWithSource:injectionTime:forMainFrameOnly:")]
		NativeHandle Constructor (NSString source, WKUserScriptInjectionTime injectionTime, bool isForMainFrameOnly);

		[Mac (11,0), iOS (14,0)]
		[MacCatalyst (14,0)]
		[Export ("initWithSource:injectionTime:forMainFrameOnly:inContentWorld:")]
		NativeHandle Constructor (NSString source, WKUserScriptInjectionTime injectionTime, bool isForMainFrameOnly, WKContentWorld contentWorld);

		[Export ("source", ArgumentSemantic.Copy)]
		NSString Source { get; }

		[Export ("injectionTime")]
		WKUserScriptInjectionTime InjectionTime { get; }

		[Export ("forMainFrameOnly")]
		bool IsForMainFrameOnly { [Bind ("isForMainFrameOnly")] get; }
	}

	[iOS (8,0), Mac (10,10)] // Not defined in 32-bit
	[BaseType (
#if MONOMAC
		typeof (NSView)
#else
		typeof (UIView)
#endif
	)]
	[DisableDefaultCtor ()] // Crashes during deallocation in Xcode 6 beta 2. radar 17377712.
	interface WKWebView
#if MONOMAC
		: NSUserInterfaceValidations
		/* TODO , NSTextFinderClient  K_API_AVAILABLE(macos(WK_MAC_TBA)) in 11.4 beta 2 */
#endif
	{

		[DesignatedInitializer]
		[Export ("initWithFrame:configuration:")]
		NativeHandle Constructor (CGRect frame, WKWebViewConfiguration configuration);

		// (instancetype)initWithCoder:(NSCoder *)coder NS_UNAVAILABLE;
		// [Unavailable (PlatformName.iOS)]
		// [Unavailable (PlatformName.MacOSX)]
		// [Export ("initWithCoder:")]
		// NativeHandle Constructor (NSCoder coder);

		[Export ("configuration", ArgumentSemantic.Copy)]
		WKWebViewConfiguration Configuration { get; }

		[Export ("navigationDelegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		NSObject WeakNavigationDelegate { get; set; }

		[Wrap ("WeakNavigationDelegate")]
		[Protocolize]
		WKNavigationDelegate NavigationDelegate { get; set; }

		[Export ("UIDelegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		NSObject WeakUIDelegate { get; set; }

		[Wrap ("WeakUIDelegate")]
		[Protocolize]
		WKUIDelegate UIDelegate { get; set; }

		[Export ("backForwardList", ArgumentSemantic.Strong)]
		WKBackForwardList BackForwardList { get; }

		[Export ("title")]
		[NullAllowed]
		string Title { get; }

		[Export ("URL", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSUrl Url { get; }

		[Export ("loading")]
		bool IsLoading { [Bind ("isLoading")] get; }

		[Export ("estimatedProgress")]
		double EstimatedProgress { get; }

		[Export ("hasOnlySecureContent")]
		bool HasOnlySecureContent { get; }

		[Export ("canGoBack")]
		bool CanGoBack { get; }

		[Export ("canGoForward")]
		bool CanGoForward { get; }

		[Export ("allowsBackForwardNavigationGestures")]
		bool AllowsBackForwardNavigationGestures { get; set; }

		[NoiOS][NoMacCatalyst]
		[Export ("allowsMagnification")]
		bool AllowsMagnification { get; set; }

		[NoiOS][NoMacCatalyst]
		[Export ("magnification")]
		nfloat Magnification { get; set; }

		[Export ("loadRequest:")]
		[return: NullAllowed]
		WKNavigation LoadRequest (NSUrlRequest request);

		[Export ("loadHTMLString:baseURL:")]
		[return: NullAllowed]
		WKNavigation LoadHtmlString (NSString htmlString, [NullAllowed] NSUrl baseUrl);

		[Wrap ("LoadHtmlString ((NSString)htmlString, baseUrl)")]
		[return: NullAllowed]
		WKNavigation LoadHtmlString (string htmlString, NSUrl baseUrl);

		[Export ("goToBackForwardListItem:")]
		[return: NullAllowed]
		WKNavigation GoTo (WKBackForwardListItem item);

		[Export ("goBack")]
		[return: NullAllowed]
		WKNavigation GoBack ();

		[Export ("goForward")]
		[return: NullAllowed]
		WKNavigation GoForward ();

		[Export ("reload")]
		[return: NullAllowed]
		WKNavigation Reload ();

		[Export ("reloadFromOrigin")]
		[return: NullAllowed]
		WKNavigation ReloadFromOrigin ();

		[Export ("stopLoading")]
		void StopLoading ();

		[Export ("evaluateJavaScript:completionHandler:")]
		[Async]
		void EvaluateJavaScript (NSString javascript, [NullAllowed] WKJavascriptEvaluationResult completionHandler);

		[Wrap ("EvaluateJavaScript ((NSString)javascript, completionHandler)")]
		[Async]
		void EvaluateJavaScript (string javascript, WKJavascriptEvaluationResult completionHandler);

		[NoiOS][NoMacCatalyst]
		[Export ("setMagnification:centeredAtPoint:")]
		void SetMagnification (nfloat magnification, CGPoint centerPoint);

		[NoMac][MacCatalyst (13,1)]
		[Export ("scrollView", ArgumentSemantic.Strong)]
		UIScrollView ScrollView { get; }

		[iOS (9,0)][Mac (10,11)]
		[Export ("loadData:MIMEType:characterEncodingName:baseURL:")]
		[return: NullAllowed]
		WKNavigation LoadData (NSData data, string mimeType, string characterEncodingName, NSUrl baseUrl);

		[iOS (9,0)][Mac (10,11)]
		[Export ("loadFileURL:allowingReadAccessToURL:")]
		[return: NullAllowed]
		WKNavigation LoadFileUrl (NSUrl url, NSUrl readAccessUrl);
		
		[iOS (9,0)][Mac (10,11)]
		[Export ("customUserAgent")]
		[NullAllowed]
		string CustomUserAgent { get; set; }

		[iOS (9,0)][Mac (10,11)]
		[Deprecated (PlatformName.iOS, 10,0, message: "Use 'ServerTrust' property.")]
		[Deprecated (PlatformName.MacOSX, 10,12, message: "Use 'ServerTrust' property.")]
		[Export ("certificateChain", ArgumentSemantic.Copy)]
		SecCertificate[] CertificateChain { get; }

		[iOS (9,0)][Mac (10,11)]
		[Export ("allowsLinkPreview")]
		bool AllowsLinkPreview { get; set; }

		[iOS (10,0)][Mac (10,12)]
		[NullAllowed, Export ("serverTrust")]
		SecTrust ServerTrust { get; }

#if !MONOMAC
		[NoMac][MacCatalyst (13,1)]
		[iOS (11,0)]
		[Async]
		[Export ("takeSnapshotWithConfiguration:completionHandler:")]
		void TakeSnapshot ([NullAllowed] WKSnapshotConfiguration snapshotConfiguration, Action<UIImage, NSError> completionHandler);
#else
		[NoiOS][NoMacCatalyst]
		[Mac (10,13)]
		[Export ("takeSnapshotWithConfiguration:completionHandler:")]
		[Async]
		void TakeSnapshot ([NullAllowed] WKSnapshotConfiguration snapshotConfiguration, Action<NSImage, NSError> completionHandler);
#endif
		[Mac (10,13), iOS (11,0)]
		[Static]
		[Export ("handlesURLScheme:")]
		bool HandlesUrlScheme (string urlScheme);

		[Mac (11,0), iOS (14,0)]
		[MacCatalyst (14,0)]
		[Async]
		[Export ("evaluateJavaScript:inFrame:inContentWorld:completionHandler:")]
		void EvaluateJavaScript (string javaScriptString, [NullAllowed] WKFrameInfo frame, WKContentWorld contentWorld, [NullAllowed] Action<NSObject, NSError> completionHandler);

		[Mac (11,0), iOS (14,0)]
		[MacCatalyst (14,0)]
		[Async]
		[Export ("callAsyncJavaScript:arguments:inFrame:inContentWorld:completionHandler:")]
		void CallAsyncJavaScript (string functionBody, [NullAllowed] NSDictionary<NSString, NSObject> arguments, [NullAllowed] WKFrameInfo frame, WKContentWorld contentWorld, [NullAllowed] Action<NSObject, NSError> completionHandler);

		[Mac (11,0), iOS (14,0)]
		[MacCatalyst (14,0)]
		[Async]
		[Export ("createPDFWithConfiguration:completionHandler:")]
		void CreatePdf ([NullAllowed] WKPdfConfiguration pdfConfiguration, Action<NSData, NSError> completionHandler);

		[Mac (11,0), iOS (14,0)]
		[MacCatalyst (14,0)]
		[Async]
		[Export ("createWebArchiveDataWithCompletionHandler:")]
		void CreateWebArchive (Action<NSData, NSError> completionHandler);

		[Mac (11,0), iOS (14,0)]
		[MacCatalyst (14,0)]
		[Async]
		[Export ("findString:withConfiguration:completionHandler:")]
		void Find (string @string, [NullAllowed] WKFindConfiguration configuration, Action<WKFindResult> completionHandler);

		[Mac (11,0), iOS (14,0)]
		[MacCatalyst (14,0)]
		[NullAllowed, Export ("mediaType")]
		string MediaType { get; set; }

		[Mac (11,0), iOS (14,0)]
		[MacCatalyst (14,0)]
		[Export ("pageZoom")]
		nfloat PageZoom { get; set; }

		[NoiOS]
		[NoMacCatalyst]
		[Mac (11,0)]
		[Export ("printOperationWithPrintInfo:")]
		NSPrintOperation GetPrintOperation (NSPrintInfo printInfo);

		// Apple renamed those API since Xcode 12.5
		[Internal]
		[Mac (11,3)][iOS (14,5)]
		[MacCatalyst (14,5)]
 		[Export ("closeAllMediaPresentations")]
 		void _OldCloseAllMediaPresentations ();

		[Async]
		[Mac (12,0), iOS (15,0), MacCatalyst (15,0), NoTV]
		[Export ("closeAllMediaPresentationsWithCompletionHandler:")]
		void CloseAllMediaPresentations ([NullAllowed] Action completionHandler);

		[Internal]
		[Mac (11,3)][iOS (14,5)]
 		[MacCatalyst (14,5)]
 		[Async]
 		[Export ("pauseAllMediaPlayback:")]
 		void _OldPauseAllMediaPlayback ([NullAllowed] Action completionHandler);

		[Internal]
		[Async]
		[Mac (12,0), iOS (15,0), MacCatalyst (15,0), NoTV]
		[Export ("pauseAllMediaPlaybackWithCompletionHandler:")]
		void _NewPauseAllMediaPlayback ([NullAllowed] Action completionHandler);

		[Internal]
		[Mac (11,3)][iOS (14,5)]
 		[MacCatalyst (14,5)]
 		[Async]
 		[Export ("suspendAllMediaPlayback:")]
 		void _OldSuspendAllMediaPlayback ([NullAllowed] Action completionHandler);

		[Internal]
		[Mac (11,3)][iOS (14,5)]
 		[MacCatalyst (14,5)]
 		[Async]
 		[Export ("resumeAllMediaPlayback:")]
 		void _OldResumeAllMediaPlayback ([NullAllowed] Action completionHandler);

		[Async]
		[Mac (12,0), iOS (15,0), MacCatalyst (15,0), NoTV]
		[Export ("setAllMediaPlaybackSuspended:completionHandler:")]
		void SetAllMediaPlaybackSuspended (bool suspended, [NullAllowed] Action completionHandler);

		[Async]
		[Mac (12,0), iOS (15,0), MacCatalyst (15,0), NoTV]
		[Export ("requestMediaPlaybackStateWithCompletionHandler:")]
		void RequestMediaPlaybackState (Action<WKMediaPlaybackState> completionHandler);

		[Mac (11,3)][iOS (14,5)]
		[MacCatalyst (14,5)]
		[Async]
		[Export ("startDownloadUsingRequest:completionHandler:")]
		void StartDownload (NSUrlRequest request, Action<WKDownload> completionHandler);

		[Mac (11,3)][iOS (14,5)]
		[MacCatalyst (14,5)]
		[Async]
		[Export ("resumeDownloadFromResumeData:completionHandler:")]
		void ResumeDownload (NSData resumeData, Action<WKDownload> completionHandler);

		[Mac (12,0), iOS (15,0), MacCatalyst (15,0), NoTV]
		[Export ("cameraCaptureState")]
		WKMediaCaptureState CameraCaptureState { get; }
		
		[Mac (12,0), iOS (15,0), MacCatalyst (15,0), NoTV]
		[NullAllowed, Export ("interactionState", ArgumentSemantic.Copy)]
		NSObject InteractionState { get; set; }

		[Mac (12,0), iOS (15,0), MacCatalyst(15,0), NoTV]
		[Export ("loadFileRequest:allowingReadAccessToURL:")]
		WKNavigation LoadFileRequest (NSUrlRequest request, NSUrl readAccessURL);
		
		[Mac (12,0), iOS (15,0), MacCatalyst (15,0), NoTV]
		[Export ("loadSimulatedRequest:response:responseData:")]
		WKNavigation LoadSimulatedRequest (NSUrlRequest request, NSUrlResponse response, NSData data);

		[Mac (12,0), iOS (15,0), MacCatalyst (15,0), NoTV]
		[Export ("loadSimulatedRequest:responseHTMLString:")]
		WKNavigation LoadSimulatedRequest (NSUrlRequest request, string htmlString);
		
		[Mac (12,0), iOS (15,0), MacCatalyst (15,0), NoTV]
		[Export ("microphoneCaptureState")]
		WKMediaCaptureState MicrophoneCaptureState { get; }
		
		[Async]
		[Mac (12,0), iOS (15,0), MacCatalyst (15,0), NoTV]
		[Export ("setCameraCaptureState:completionHandler:")]
		void SetCameraCaptureState (WKMediaCaptureState state, [NullAllowed] Action completionHandler);
		
		[Async]
		[Mac (12,0), iOS (15,0), MacCatalyst (15,0), NoTV]
		[Export ("setMicrophoneCaptureState:completionHandler:")]
		void SetMicrophoneCaptureState (WKMediaCaptureState state, [NullAllowed] Action completionHandler);
		
		[iOS (15,0), MacCatalyst (15,0), Mac (12,0), NoTV]
		[Export ("themeColor")]
		[NullAllowed]
		UIColor ThemeColor { get; }

		[iOS (15,0), MacCatalyst (15,0), Mac (12,0), NoTV]
		[NullAllowed, Export ("underPageBackgroundColor", ArgumentSemantic.Copy)]
		UIColor UnderPageBackgroundColor { get; set; }

		[iOS (15,4), MacCatalyst (15,4), Mac (12,3), NoTV]
		[Export ("fullscreenState")]
		WKFullscreenState FullscreenState { get; }
	}

	delegate void WKJavascriptEvaluationResult (NSObject result, NSError error);

	[iOS (8,0), Mac (10,10)] // Not defined in 32-bit
	[BaseType (typeof (NSObject))]
	interface WKWebViewConfiguration : NSCopying, NSSecureCoding {

		[Export ("processPool", ArgumentSemantic.Retain)]
		WKProcessPool ProcessPool { get; set; }

		[Export ("preferences", ArgumentSemantic.Retain)]
		WKPreferences Preferences { get; set; }

		[Export ("userContentController", ArgumentSemantic.Retain)]
		WKUserContentController UserContentController { get; set; }

		[Export ("suppressesIncrementalRendering")]
		bool SuppressesIncrementalRendering { get; set; }

		[iOS (9,0), Mac(10,11)]
		[Export ("websiteDataStore", ArgumentSemantic.Strong)]
		WKWebsiteDataStore WebsiteDataStore { get; set; }

		[iOS (9,0), Mac(10,11)]
		[Export ("applicationNameForUserAgent")]
		[NullAllowed]
		string ApplicationNameForUserAgent { get; set; }

		[iOS (9,0)][Mac (10,11)]
		[Export ("allowsAirPlayForMediaPlayback")]
		bool AllowsAirPlayForMediaPlayback { get; set; }

		[NoMac][MacCatalyst (13,1)]
		[Export ("allowsInlineMediaPlayback")]
		bool AllowsInlineMediaPlayback { get; set; }

		[NoMac]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'RequiresUserActionForMediaPlayback' or 'MediaTypesRequiringUserActionForPlayback' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'RequiresUserActionForMediaPlayback' or 'MediaTypesRequiringUserActionForPlayback' instead.")]
		[Export ("mediaPlaybackRequiresUserAction")]
		bool MediaPlaybackRequiresUserAction { get; set; }

		[NoMac]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'AllowsAirPlayForMediaPlayback' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'AllowsAirPlayForMediaPlayback' instead.")]
		[Export ("mediaPlaybackAllowsAirPlay")]
		bool MediaPlaybackAllowsAirPlay { get; set; }

		[NoMac][MacCatalyst (13,1)]
		[Export ("selectionGranularity")]
		WKSelectionGranularity SelectionGranularity { get; set; }

		[NoMac]
		[iOS (9, 0)]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'MediaTypesRequiringUserActionForPlayback' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'MediaTypesRequiringUserActionForPlayback' instead.")]
		[Export ("requiresUserActionForMediaPlayback")]
		bool RequiresUserActionForMediaPlayback { get; set; }

		[NoMac][MacCatalyst (13,1)]
		[iOS (9,0)]
		[Export ("allowsPictureInPictureMediaPlayback")]
		bool AllowsPictureInPictureMediaPlayback { get; set; }

		[NoMac][MacCatalyst (13,1)]
		[iOS (10, 0)]
		[Export ("dataDetectorTypes", ArgumentSemantic.Assign)]
		WKDataDetectorTypes DataDetectorTypes { get; set; }

		[iOS (10,0)][Mac (10,12)]
		[Export ("mediaTypesRequiringUserActionForPlayback", ArgumentSemantic.Assign)]
		WKAudiovisualMediaTypes MediaTypesRequiringUserActionForPlayback { get; set; }

		[iOS (10,0)]
		[NoMac]
		[Export ("ignoresViewportScaleLimits")]
		bool IgnoresViewportScaleLimits { get; set; }

		[Mac (10,13), iOS (11,0)]
		[Export ("setURLSchemeHandler:forURLScheme:")]
		void SetUrlSchemeHandler ([NullAllowed] IWKUrlSchemeHandler urlSchemeHandler, string urlScheme);
	
		[Mac (10,13), iOS (11,0)]
		[Export ("urlSchemeHandlerForURLScheme:")]
		[return: NullAllowed]
		IWKUrlSchemeHandler GetUrlSchemeHandler (string urlScheme);

		[Mac (10,15)]
		[iOS (13,0)]
		[Export ("defaultWebpagePreferences", ArgumentSemantic.Copy)]
		[NullAllowed]
		WKWebpagePreferences DefaultWebpagePreferences { get; set; }

		[Mac (11,0)]
		[iOS (14,0)]
		[Export ("limitsNavigationsToAppBoundDomains")]
		bool LimitsNavigationsToAppBoundDomains { get; set; }

		[Mac (12, 0), iOS (15, 0), MacCatalyst (15,0), NoTV]
		[Export ("upgradeKnownHostsToHTTPS")]
		bool UpgradeKnownHostsToHttps { get; set; }
	}

	[iOS (8,0), Mac (10,10)] // Not defined in 32-bit
	[BaseType (typeof (NSObject))]
	interface WKProcessPool : NSSecureCoding {
		// as of Mac 10.10, iOS 8.0 Beta 2,
		// this interface is completely empty
	}

	[iOS (8,0), Mac (10,10)] // Not defined in 32-bit
	[BaseType (typeof (NSObject))]
	interface WKWindowFeatures {
		// Filled in from open source headers

		[Internal, Export ("menuBarVisibility")]
		[NullAllowed]
		NSNumber menuBarVisibility { get; }

		[Internal, Export ("statusBarVisibility")]
		[NullAllowed]
		NSNumber statusBarVisibility { get; }

		[Internal, Export ("toolbarsVisibility")]
		[NullAllowed]
		NSNumber toolbarsVisibility { get; }

		[Internal, Export ("allowsResizing")]
		[NullAllowed]
		NSNumber allowsResizing { get; }

		[Internal, Export ("x")]
		[NullAllowed]
		NSNumber x { get; }

		[Internal, Export ("y")]
		[NullAllowed]
		NSNumber y { get; }

		[Internal, Export ("width")]
		[NullAllowed]
		NSNumber width { get; }

		[Internal, Export ("height")]
		[NullAllowed]
		NSNumber height { get; }
	}

#if !MONOMAC
	interface IWKPreviewActionItem {}

	[iOS (10,0)][NoMac]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'TBD' instead.")]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'TBD' instead.")]
	[Protocol]
	interface WKPreviewActionItem : UIPreviewActionItem {
		[Abstract]
		[Export ("identifier", ArgumentSemantic.Copy)]
		NSString Identifier { get; }
	}
#endif

	[iOS (10,0)][NoMac]
	[Static]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'TBD' instead.")]
	interface WKPreviewActionItemIdentifier {
		[Field ("WKPreviewActionItemIdentifierOpen")]
		NSString Open { get; }

		[Field ("WKPreviewActionItemIdentifierAddToReadingList")]
		NSString AddToReadingList { get; }

		[Field ("WKPreviewActionItemIdentifierCopy")]
		NSString Copy { get; }

		[Field ("WKPreviewActionItemIdentifierShare")]
		NSString Share { get; }
	}

	[iOS (10,0)][NoMac]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'WKContextMenuElementInfo' instead.")]
	[BaseType (typeof (NSObject))]
	interface WKPreviewElementInfo : NSCopying {
		[NullAllowed, Export ("linkURL")]
		NSUrl LinkUrl { get; }
	}

	[Mac (10,15)]
	[iOS (13,0)]
	[Native]
	public enum WKContentMode : long {
		Recommended,
		Mobile,
		Desktop,
	}

	[Mac (10,15)]
	[iOS (13,0)]
	[BaseType (typeof (NSObject))]
	interface WKWebpagePreferences {

		[Export ("preferredContentMode", ArgumentSemantic.Assign)]
		WKContentMode PreferredContentMode { get; set; }

		[Mac (11,0)]
		[iOS (14,0)]
		[Export ("allowsContentJavaScript")]
		bool AllowsContentJavaScript { get; set; }
	}

	[NoMac]
	[iOS (13,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface WKContextMenuElementInfo {
		[NullAllowed, Export ("linkURL")]
		NSUrl LinkUrl { get; }
	}

	[Mac (11,0)][iOS (14,0)]
	[MacCatalyst (14,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface WKContentWorld {

		[Static]
		[Export ("pageWorld")]
		WKContentWorld Page { get; }

		[Static]
		[Export ("defaultClientWorld")]
		WKContentWorld DefaultClient { get; }

		[Static]
		[Export ("worldWithName:")]
		WKContentWorld Create (string name);

		[NullAllowed, Export ("name")]
		string Name { get; }
	}

	[Mac (11,0)][iOS (14,0)]
	[BaseType (typeof (NSObject))]
	interface WKFindConfiguration : NSCopying {

		[Export ("backwards")]
		bool Backwards { get; set; }

		[Export ("caseSensitive")]
		bool CaseSensitive { get; set; }

		[Export ("wraps")]
		bool Wraps { get; set; }
	}

	[Mac (11,0)][iOS (14,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface WKFindResult : NSCopying {

		[Export ("matchFound")]
		bool MatchFound { get; }
	}

	[Mac (11,0)][iOS (14,0)]
	[BaseType (typeof (NSObject), Name = "WKPDFConfiguration")]
	interface WKPdfConfiguration : NSCopying {

		[Export ("rect", ArgumentSemantic.Assign)]
		CGRect Rect { get; set; }
	}

	interface IWKScriptMessageHandlerWithReply {}

	[Mac (11,0)][iOS (14,0)]
	[Protocol]
	interface WKScriptMessageHandlerWithReply {

		[Abstract]
		[Export ("userContentController:didReceiveScriptMessage:replyHandler:")]
		void DidReceiveScriptMessage (WKUserContentController userContentController, WKScriptMessage message, Action<NSObject, NSString> replyHandler);
	}

	[Mac (11,3)][iOS (14,5)]
	[Native]
	enum WKDownloadRedirectPolicy : long {
		Cancel,
		Allow,
	}

	[Mac (11,3)][iOS (14,5)]
	[Native]
	enum WKMediaPlaybackState : ulong {
		None,
		Paused,
		Suspended,
		Playing,
	}

	interface IWKDownloadDelegate {}

	[Mac (11,3)][iOS (14,5)]
	[MacCatalyst (14,5)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface WKDownloadDelegate {

		[Abstract]
		[Export ("download:decideDestinationUsingResponse:suggestedFilename:completionHandler:")]
		void DecideDestination (WKDownload download, NSUrlResponse response, string suggestedFilename, Action<NSUrl> completionHandler);

		[Export ("download:willPerformHTTPRedirection:newRequest:decisionHandler:")]
		void WillPerformHttpRedirection (WKDownload download, NSHttpUrlResponse response, NSUrlRequest request, Action<WKDownloadRedirectPolicy> decisionHandler);

		[Export ("download:didReceiveAuthenticationChallenge:completionHandler:")]
		void DidReceiveAuthenticationChallenge (WKDownload download, NSUrlAuthenticationChallenge challenge, Action<NSUrlSessionAuthChallengeDisposition, NSUrlCredential> completionHandler);

		[Export ("downloadDidFinish:")]
		void DidFinish (WKDownload download);

		[Export ("download:didFailWithError:resumeData:")]
		void DidFail (WKDownload download, NSError error, [NullAllowed] NSData resumeData);
	}

	[Mac (11,3)][iOS (14,5)]
	[MacCatalyst (14,5)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface WKDownload : NSProgressReporting {

		[NullAllowed, Export ("originalRequest")]
		NSUrlRequest OriginalRequest { get; }

		[NullAllowed, Export ("webView", ArgumentSemantic.Weak)]
		WKWebView WebView { get; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IWKDownloadDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Async]
		[Export ("cancel:")]
		void Cancel ([NullAllowed] Action<NSData> completionHandler);
	}
}
