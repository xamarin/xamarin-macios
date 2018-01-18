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

using XamCore.ObjCRuntime;
using XamCore.Foundation;
using XamCore.CoreGraphics;
using XamCore.Security;
#if MONOMAC
using XamCore.AppKit;
#else
using XamCore.UIKit;
#endif

namespace XamCore.WebKit
{
	[iOS (8,0), Mac (10,10, onlyOn64 : true)] // Not defined in 32-bit
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor ()] // Crashes during deallocation in Xcode 6 beta 2. radar 17377712.
	interface WKBackForwardListItem {

		[Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get; }

		[Export ("title")]
		string Title { get; }

		[Export ("initialURL", ArgumentSemantic.Copy)]
		NSUrl InitialUrl { get; }
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)] // Not defined in 32-bit
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor ()] // Crashes during deallocation in Xcode 6 beta 2. radar 17377712.
	interface WKBackForwardList {

		[Export ("currentItem", ArgumentSemantic.Strong)]
		WKBackForwardListItem CurrentItem { get; }

		[Export ("backItem", ArgumentSemantic.Strong)]
		WKBackForwardListItem BackItem { get; }

		[Export ("forwardItem", ArgumentSemantic.Strong)]
		WKBackForwardListItem ForwardItem { get; }

		[Export ("backList")]
		WKBackForwardListItem [] BackList { get; }

		[Export ("forwardList")]
		WKBackForwardListItem [] ForwardList { get; }

		[Export ("itemAtIndex:")]
		WKBackForwardListItem ItemAtIndex (nint index);
	}

	[Mac (10,13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(NSObject))]
	interface WKContentRuleList
	{
		[Export ("identifier")]
		string Identifier { get; }
	}

	[Mac (10,13, onlyOn64: true), iOS (11,0)]
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
	
	[iOS (8,0), Mac (10,10, onlyOn64 : true)] // Not defined in 32-bit
	[BaseType (typeof (NSObject))]
	interface WKFrameInfo : NSCopying {

		[Export ("mainFrame")]
		bool MainFrame { [Bind ("isMainFrame")] get; }

		[Export ("request", ArgumentSemantic.Copy)]
		NSUrlRequest Request { get; }

		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[Export ("securityOrigin")]
		WKSecurityOrigin SecurityOrigin { get; }

		[iOS (11,0)][Mac (10,13, onlyOn64 : true)]
		[NullAllowed, Export ("webView", ArgumentSemantic.Weak)]
		WKWebView WebView { get; }
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)] // Not defined in 32-bit
	[BaseType (typeof (NSObject))]
	interface WKNavigation {
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)] // Not defined in 32-bit
	[BaseType (typeof (NSObject))]
	interface WKNavigationAction {

		[Export ("sourceFrame", ArgumentSemantic.Copy)]
		WKFrameInfo SourceFrame { get; }

		[Export ("targetFrame", ArgumentSemantic.Copy)]
		WKFrameInfo TargetFrame { get; }

		[Export ("navigationType")]
		WKNavigationType NavigationType { get; }

		[Export ("request", ArgumentSemantic.Copy)]
		NSUrlRequest Request { get; }

#if MONOMAC
		[Export ("modifierFlags")]
		NSEventModifierMask ModifierFlags { get; }

		[Export ("buttonNumber")]
		nint ButtonNumber { get; }
#endif
	}

	[Mac (10,10, onlyOn64 : true), iOS (8,0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface WKNavigationDelegate {

		[Export ("webView:decidePolicyForNavigationAction:decisionHandler:")]
		void DecidePolicy (WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler);

		[Export ("webView:decidePolicyForNavigationResponse:decisionHandler:")]
		void DecidePolicy (WKWebView webView, WKNavigationResponse navigationResponse, Action<WKNavigationResponsePolicy> decisionHandler);

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

		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[Export ("webViewWebContentProcessDidTerminate:")]
		void ContentProcessDidTerminate (WKWebView webView);
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)] // Not defined in 32-bit
	[BaseType (typeof (NSObject))]
	interface WKNavigationResponse {

		[Export ("forMainFrame")]
		bool IsForMainFrame { [Bind ("isForMainFrame")] get; }

		[Export ("response", ArgumentSemantic.Copy)]
		NSUrlResponse Response { get; }

		[Export ("canShowMIMEType")]
		bool CanShowMimeType { get; }
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)] // Not defined in 32-bit
	[BaseType (typeof (NSObject))]
	interface WKPreferences : NSCoding {
		[Export ("minimumFontSize")]
		nfloat MinimumFontSize { get; set; }

		[Export ("javaScriptEnabled")]
		bool JavaScriptEnabled { get; set; }

		[Export ("javaScriptCanOpenWindowsAutomatically")]
		bool JavaScriptCanOpenWindowsAutomatically { get; set; }


#if MONOMAC
		[Export ("javaEnabled")]
		bool JavaEnabled { get; set; }

		[Export ("plugInsEnabled")]
		bool PlugInsEnabled { get; set; }

		// Headers says 10,12,3 but it is not available likely they meant 10,12,4
		[Mac (10,12,4, onlyOn64 : true)]
		[Export ("tabFocusesLinks")]
		bool TabFocusesLinks { get; set; }
#endif
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)] // Not defined in 32-bit
	[BaseType (typeof (NSObject))]
	interface WKScriptMessage {

		// May be typed as NSNumber, NSString, NSDate, NSArray,
		// NSDictionary, or NSNull, as it must map cleanly to JSON
		[Export ("body", ArgumentSemantic.Copy)]
		NSObject Body { get; }

		[Export ("webView", ArgumentSemantic.Weak)]
		WKWebView WebView { get; }

		[Export ("name")]
		string Name { get; }

		[Export ("frameInfo", ArgumentSemantic.Copy)]
		WKFrameInfo FrameInfo { get; }
	}

	[Mac (10,10, onlyOn64 : true), iOS (8,0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface WKScriptMessageHandler {

		[Export ("userContentController:didReceiveScriptMessage:")]
		[Abstract]
		void DidReceiveScriptMessage (WKUserContentController userContentController, WKScriptMessage message);
	}

	[iOS (9,0)][Mac(10,11, onlyOn64 : true)]
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

	
	[Mac (10,13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(NSObject))]
	interface WKSnapshotConfiguration : NSCopying {
		[Export ("rect")]
		CGRect Rect { get; set; }

		[Export ("snapshotWidth")]
		NSNumber SnapshotWidth { get; set; }
	}

#if XAMCORE_2_0
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
#endif
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
	
	[iOS (9,0), Mac(10,11, onlyOn64 : true)]
	[BaseType (typeof(NSObject))]
	interface WKWebsiteDataRecord
	{
		[Export ("displayName")]
		string DisplayName { get; }
	
		[Export ("dataTypes", ArgumentSemantic.Copy)]
		NSSet<NSString> DataTypes { get; }
	}

	[iOS (9,0), Mac(10,11, onlyOn64 : true)]
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
	}
	
	[iOS (9,0), Mac(10,11, onlyOn64 : true)]
	[BaseType (typeof(NSObject))]
	interface WKWebsiteDataStore : NSCoding {

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

	[Mac (10,12, onlyOn64 : true)][NoiOS, NoWatch, NoTV]
	[BaseType (typeof(NSObject))]
	interface WKOpenPanelParameters	{
		[Export ("allowsMultipleSelection")]
		bool AllowsMultipleSelection { get; }
	}
	
	[Mac (10,10, onlyOn64 : true), iOS (8,0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface WKUIDelegate {

		[Export ("webView:createWebViewWithConfiguration:forNavigationAction:windowFeatures:")]
		WKWebView CreateWebView (WKWebView webView, WKWebViewConfiguration configuration,
			WKNavigationAction navigationAction, WKWindowFeatures windowFeatures);

		[Export ("webView:runJavaScriptAlertPanelWithMessage:initiatedByFrame:completionHandler:")]
		void RunJavaScriptAlertPanel (WKWebView webView, string message, WKFrameInfo frame, Action completionHandler);

		[Export ("webView:runJavaScriptConfirmPanelWithMessage:initiatedByFrame:completionHandler:")]
		void RunJavaScriptConfirmPanel (WKWebView webView, string message, WKFrameInfo frame, Action<bool> completionHandler);

		[Export ("webView:runJavaScriptTextInputPanelWithPrompt:defaultText:initiatedByFrame:completionHandler:")]
		void RunJavaScriptTextInputPanel (WKWebView webView, string prompt, [NullAllowed] string defaultText,
			WKFrameInfo frame, Action<string> completionHandler);

		[Mac (10,12, onlyOn64 : true)][NoiOS, NoWatch, NoTV]
		[Export ("webView:runOpenPanelWithParameters:initiatedByFrame:completionHandler:")]
		void RunOpenPanel (WKWebView webView, WKOpenPanelParameters parameters, WKFrameInfo frame, Action<NSUrl[]> completionHandler);

		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[Export ("webViewDidClose:")]
		void DidClose (WKWebView webView);

		[iOS (10,0)][NoMac]
		[Export ("webView:shouldPreviewElement:")]
		bool ShouldPreviewElement (WKWebView webView, WKPreviewElementInfo elementInfo);

#if !MONOMAC
		[iOS (10,0)][NoMac]
		[Export ("webView:previewingViewControllerForElement:defaultActions:")]
		[return: NullAllowed]
		UIViewController GetPreviewingViewController (WKWebView webView, WKPreviewElementInfo elementInfo, IWKPreviewActionItem[] previewActions);

		[iOS (10,0)][NoMac]
		[Export ("webView:commitPreviewingViewController:")]
		void CommitPreviewingViewController (WKWebView webView, UIViewController previewingViewController);
#endif
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)] // Not defined in 32-bit
	[BaseType (typeof (NSObject))]
	interface WKUserContentController : NSCoding {

		[Export ("userScripts")]
		WKUserScript [] UserScripts { get; }

		[Export ("addUserScript:")]
		void AddUserScript (WKUserScript userScript);

		[Export ("removeAllUserScripts")]
		void RemoveAllUserScripts ();

		[Export ("addScriptMessageHandler:name:")]
		void AddScriptMessageHandler ([Protocolize] WKScriptMessageHandler scriptMessageHandler, string name);

		[Export ("removeScriptMessageHandlerForName:")]
		void RemoveScriptMessageHandler (string name);

		[Mac (10,13), iOS (11,0)]
		[Export ("addContentRuleList:")]
		void AddContentRuleList (WKContentRuleList contentRuleList);
	
		[Mac (10,13), iOS (11,0)]
		[Export ("removeContentRuleList:")]
		void RemoveContentRuleList (WKContentRuleList contentRuleList);
	
		[Mac (10,13), iOS (11,0)]
		[Export ("removeAllContentRuleLists")]
		void RemoveAllContentRuleLists ();
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)] // Not defined in 32-bit
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // all properties are getters
	interface WKUserScript : NSCopying {

		[Export ("initWithSource:injectionTime:forMainFrameOnly:")]
		IntPtr Constructor (NSString source, WKUserScriptInjectionTime injectionTime, bool isForMainFrameOnly);

		[Export ("source", ArgumentSemantic.Copy)]
		NSString Source { get; }

		[Export ("injectionTime")]
		WKUserScriptInjectionTime InjectionTime { get; }

		[Export ("forMainFrameOnly")]
		bool IsForMainFrameOnly { [Bind ("isForMainFrameOnly")] get; }
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)] // Not defined in 32-bit
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
#endif
	{

		[DesignatedInitializer]
		[Export ("initWithFrame:configuration:")]
		IntPtr Constructor (CGRect frame, WKWebViewConfiguration configuration);

		// (instancetype)initWithCoder:(NSCoder *)coder NS_UNAVAILABLE;
		// [Unavailable (PlatformName.iOS)]
		// [Unavailable (PlatformName.MacOSX)]
		// [Export ("initWithCoder:")]
		// IntPtr Constructor (NSCoder coder);

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
		string Title { get; }

		[Export ("URL", ArgumentSemantic.Copy)]
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

#if MONOMAC
		[Export ("allowsMagnification")]
		bool AllowsMagnification { get; set; }

		[Export ("magnification")]
		nfloat Magnification { get; set; }
#endif

		[Export ("loadRequest:")]
		WKNavigation LoadRequest (NSUrlRequest request);

		[Export ("loadHTMLString:baseURL:")]
		WKNavigation LoadHtmlString (NSString htmlString, [NullAllowed] NSUrl baseUrl);

		[Wrap ("LoadHtmlString ((NSString)htmlString, baseUrl)")]
		WKNavigation LoadHtmlString (string htmlString, NSUrl baseUrl);

		[Export ("goToBackForwardListItem:")]
		WKNavigation GoTo (WKBackForwardListItem item);

		[Export ("goBack")]
		WKNavigation GoBack ();

		[Export ("goForward")]
		WKNavigation GoForward ();

		[Export ("reload")]
		WKNavigation Reload ();

		[Export ("reloadFromOrigin")]
		WKNavigation ReloadFromOrigin ();

		[Export ("stopLoading")]
		void StopLoading ();

		[Export ("evaluateJavaScript:completionHandler:")]
		[Async]
		void EvaluateJavaScript (NSString javascript, [NullAllowed] WKJavascriptEvaluationResult completionHandler);

		[Wrap ("EvaluateJavaScript ((NSString)javascript, completionHandler)")]
		[Async]
		void EvaluateJavaScript (string javascript, WKJavascriptEvaluationResult completionHandler);

#if MONOMAC
		[Export ("setMagnification:centeredAtPoint:")]
		void SetMagnification (nfloat magnification, CGPoint centerPoint);
#else
		[Export ("scrollView", ArgumentSemantic.Strong)]
		UIScrollView ScrollView { get; }
#endif

		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[Export ("loadData:MIMEType:characterEncodingName:baseURL:")]
		[return: NullAllowed]
		WKNavigation LoadData (NSData data, string mimeType, string characterEncodingName, NSUrl baseUrl);

		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[Export ("loadFileURL:allowingReadAccessToURL:")]
		[return: NullAllowed]
		WKNavigation LoadFileUrl (NSUrl url, NSUrl readAccessUrl);
		
		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[Export ("customUserAgent")]
		string CustomUserAgent { get; set; }

		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[Deprecated (PlatformName.iOS, 10,0, message: "Use 'ServerTrust' property.")]
		[Deprecated (PlatformName.MacOSX, 10,12, message: "Use 'ServerTrust' property.")]
		[Export ("certificateChain", ArgumentSemantic.Copy)]
		SecCertificate[] CertificateChain { get; }

		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[Export ("allowsLinkPreview")]
		bool AllowsLinkPreview { get; set; }

		[iOS (10,0)][Mac (10,12, onlyOn64 : true)]
		[NullAllowed, Export ("serverTrust")]
		SecTrust ServerTrust { get; }

#if !MONOMAC
		[iOS (11,0)]
		[Async]
		[Export ("takeSnapshotWithConfiguration:completionHandler:")]
		void TakeSnapshot ([NullAllowed] WKSnapshotConfiguration snapshotConfiguration, Action<UIImage, NSError> completionHandler);
#else
		[Mac (10,13)]
		[Export ("takeSnapshotWithConfiguration:completionHandler:")]
		[Async]
		void TakeSnapshot ([NullAllowed] WKSnapshotConfiguration snapshotConfiguration, Action<NSImage, NSError> completionHandler);
#endif
		[Mac (10,13), iOS (11,0)]
		[Static]
		[Export ("handlesURLScheme:")]
		bool HandlesUrlScheme (string urlScheme);

	}

	delegate void WKJavascriptEvaluationResult (NSObject result, NSError error);

	[iOS (8,0), Mac (10,10, onlyOn64 : true)] // Not defined in 32-bit
	[BaseType (typeof (NSObject))]
	interface WKWebViewConfiguration : NSCopying, NSCoding {

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
		string ApplicationNameForUserAgent { get; set; }

		[iOS (9,0)][Mac (10,11)]
		[Export ("allowsAirPlayForMediaPlayback")]
		bool AllowsAirPlayForMediaPlayback { get; set; }

#if !MONOMAC
		[Export ("allowsInlineMediaPlayback")]
		bool AllowsInlineMediaPlayback { get; set; }

		[Availability (Introduced = Platform.iOS_8_0, Deprecated = Platform.iOS_9_0, Message = "Use 'RequiresUserActionForMediaPlayback' or 'MediaTypesRequiringUserActionForPlayback' instead.")]
		[Export ("mediaPlaybackRequiresUserAction")]
		bool MediaPlaybackRequiresUserAction { get; set; }

		[Availability (Introduced = Platform.iOS_8_0, Deprecated = Platform.iOS_9_0, Message = "Use 'AllowsAirPlayForMediaPlayback' instead.")]
		[Export ("mediaPlaybackAllowsAirPlay")]
		bool MediaPlaybackAllowsAirPlay { get; set; }

		[Export ("selectionGranularity")]
		WKSelectionGranularity SelectionGranularity { get; set; }

		[Availability (Introduced = Platform.iOS_9_0, Deprecated = Platform.iOS_10_0, Message = "Use 'MediaTypesRequiringUserActionForPlayback' instead.")]
		[Export ("requiresUserActionForMediaPlayback")]
		bool RequiresUserActionForMediaPlayback { get; set; }

		[iOS (9,0)]
		[Export ("allowsPictureInPictureMediaPlayback")]
		bool AllowsPictureInPictureMediaPlayback { get; set; }

		[iOS (10, 0)]
		[Export ("dataDetectorTypes", ArgumentSemantic.Assign)]
		WKDataDetectorTypes DataDetectorTypes { get; set; }
#endif
		[iOS (10,0)][Mac (10,12, onlyOn64: true)]
		[Export ("mediaTypesRequiringUserActionForPlayback", ArgumentSemantic.Assign)]
		WKAudiovisualMediaTypes MediaTypesRequiringUserActionForPlayback { get; set; }

		[iOS (10,0)]
		[NoMac]
		[Export ("ignoresViewportScaleLimits")]
		bool IgnoresViewportScaleLimits { get; set; }

#if XAMCORE_2_0
		[Mac (10,13), iOS (11,0)]
		[Export ("setURLSchemeHandler:forURLScheme:")]
		void SetUrlSchemeHandler ([NullAllowed] IWKUrlSchemeHandler urlSchemeHandler, string urlScheme);
	
		[Mac (10,13), iOS (11,0)]
		[Export ("urlSchemeHandlerForURLScheme:")]
		[return: NullAllowed]
		IWKUrlSchemeHandler GetUrlSchemeHandler (string urlScheme);
#endif
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)] // Not defined in 32-bit
	[BaseType (typeof (NSObject))]
	interface WKProcessPool : NSCoding {
		// as of Mac 10.10, iOS 8.0 Beta 2,
		// this interface is completely empty
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)] // Not defined in 32-bit
	[BaseType (typeof (NSObject))]
	interface WKWindowFeatures {
		// Filled in from open source headers

		[Internal, Export ("menuBarVisibility")]
		NSNumber menuBarVisibility { get; }

		[Internal, Export ("statusBarVisibility")]
		NSNumber statusBarVisibility { get; }

		[Internal, Export ("toolbarsVisibility")]
		NSNumber toolbarsVisibility { get; }

		[Internal, Export ("allowsResizing")]
		NSNumber allowsResizing { get; }

		[Internal, Export ("x")]
		NSNumber x { get; }

		[Internal, Export ("y")]
		NSNumber y { get; }

		[Internal, Export ("width")]
		NSNumber width { get; }

		[Internal, Export ("height")]
		NSNumber height { get; }
	}

#if !MONOMAC
	interface IWKPreviewActionItem {}

	[iOS (10,0)][NoMac]
	[Protocol]
	interface WKPreviewActionItem : UIPreviewActionItem {
		[Abstract]
		[Export ("identifier", ArgumentSemantic.Copy)]
		NSString Identifier { get; }
	}
#endif

	[iOS (10,0)][NoMac]
	[Static]
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
	[BaseType (typeof (NSObject))]
	interface WKPreviewElementInfo : NSCopying {
		[NullAllowed, Export ("linkURL")]
		NSUrl LinkUrl { get; }
	}
}
