//
// social.cs: API definition for Apple's Social Framework
//
// Authors:
//    Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2012-2013 Xamarin Inc
//

using System;
using ObjCRuntime;
using Foundation;
using Accounts;

#if !MONOMAC
using UIKit;
using SocialImage = UIKit.UIImage;
using SocialTextView = UIKit.UITextView;
using SocialTextViewDelegate = UIKit.UITextViewDelegate;
using SocialView = UIKit.UIView;
using SocialViewController = UIKit.UIViewController;
#endif
#if MONOMAC
using AppKit;
using SocialImage = AppKit.NSImage;
using SocialTextView = AppKit.NSTextView;
using SocialTextViewDelegate = AppKit.NSTextViewDelegate;
using SocialView = AppKit.NSView;
using SocialViewController = AppKit.NSViewController;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Social {
	/// <summary>NSString constants with the various service types supported by the Social framework</summary>
	///     <remarks>These constants are used typically when interacting with low-level Objective-C APIs.   In general, you can just use the higher level APIs that use strongly typed enumerations of type <see cref="T:Social.SLServiceKind" />.</remarks>
	[Static]
	interface SLServiceType {
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use Facebook SDK instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use Facebook SDK instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use Facebook SDK instead.")]
		[Field ("SLServiceTypeFacebook")]
		NSString Facebook { get; }

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use Twitter SDK instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use Twitter SDK instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use Twitter SDK instead.")]
		[Field ("SLServiceTypeTwitter")]
		NSString Twitter { get; }

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use Sina Weibo SDK instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use Sina Weibo SDK instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use Sina Weibo SDK instead.")]
		[Field ("SLServiceTypeSinaWeibo")]
		NSString SinaWeibo { get; }

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use Tencent Weibo SDK instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use Tencent Weibo SDK instead.")]
		[Field ("SLServiceTypeTencentWeibo")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use Tencent Weibo SDK instead.")]
		NSString TencentWeibo { get; }

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use LinkedIn SDK instead.")]
		[Field ("SLServiceTypeLinkedIn")]
		[NoiOS]
		[NoMacCatalyst]
		NSString LinkedIn { get; }
	}

	/// <summary>A request made to a social service.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/Social/Reference/SLRequest_Class/index.html">Apple documentation for <c>SLRequest</c></related>
	[BaseType (typeof (NSObject))]
	// init -> Objective-C exception thrown.  Name: NSInternalInconsistencyException Reason: SLRequestMultiPart must be obtained through!
	[DisableDefaultCtor]
	interface SLRequest {
		[Static]
		[Export ("requestForServiceType:requestMethod:URL:parameters:")]
		SLRequest Create (NSString serviceType, SLRequestMethod requestMethod, NSUrl url, [NullAllowed] NSDictionary parameters);

		[Deprecated (PlatformName.iOS, 15, 0, message: "Use the non-Apple SDK relating to your account type instead.")]
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use the non-Apple SDK relating to your account type instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use the non-Apple SDK relating to your account type instead.")]
		[Export ("account", ArgumentSemantic.Retain), NullAllowed]
		ACAccount Account { get; set; }

		[Export ("requestMethod")]
		SLRequestMethod RequestMethod { get; }

		[Export ("URL")]
		NSUrl Url { get; }

		[Export ("parameters")]
		NSDictionary Parameters { get; }

		[NoiOS] // just macOS
		[NoMacCatalyst]
		[Export ("addMultipartData:withName:type:")]
		void AddMultipartData (NSData data, string partName, string partType);

		[Export ("addMultipartData:withName:type:filename:")]
		void AddMultipartData (NSData data, string partName, string partType, string filename);

		[Export ("preparedURLRequest")]
		NSUrlRequest GetPreparedUrlRequest ();

		// async 
		[Export ("performRequestWithHandler:")]
		[Async (ResultTypeName = "SLRequestResult")]
		void PerformRequest (Action<NSData, NSHttpUrlResponse, NSError> handler);
	}

	/// <summary>A <see cref="T:UIKit.UIViewController" /> that manages the user experience for the composition of a post for a social service.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/NetworkingInternet/Reference/SLComposeViewController_Class/index.html">Apple documentation for <c>SLComposeViewController</c></related>
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (SocialViewController))]
	[DisableDefaultCtor] // see note on 'composeViewControllerForServiceType:'
	interface SLComposeViewController {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("serviceType")]
		NSString ServiceType { get; }

		[Export ("completionHandler", ArgumentSemantic.Copy)]
		Action<SLComposeViewControllerResult> CompletionHandler { get; set; }

		[Static]
		[Export ("composeViewControllerForServiceType:")]
		// note: Use this method to create a social compose view controller. Do not use any other methods.
		SLComposeViewController FromService (NSString serviceType);

		[Static]
		[Export ("isAvailableForServiceType:")]
		bool IsAvailable (NSString serviceType);

		[Export ("setInitialText:")]
		bool SetInitialText (string text);

		[Export ("addImage:")]
		bool AddImage (SocialImage image);

		[Export ("removeAllImages")]
		bool RemoveAllImages ();

		[Export ("addURL:")]
		bool AddUrl (NSUrl url);

		[Export ("removeAllURLs")]
		bool RemoveAllUrls ();
	}

	/// <summary>A standard UIViewController for composing data for social sharing.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/Social/Reference/SLComposeServiceViewController_Class/index.html">Apple documentation for <c>SLComposeServiceViewController</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (SocialViewController))]
	interface SLComposeServiceViewController : SocialTextViewDelegate {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("presentationAnimationDidFinish")]
		void PresentationAnimationDidFinish ();

		[Export ("textView")]
		SocialTextView TextView { get; }

		[Export ("contentText")]
		string ContentText { get; }

		[NullAllowed] // by default this property is null
		[Export ("placeholder")]
		string Placeholder { get; set; }

		[Export ("didSelectPost")]
		void DidSelectPost ();

		[Export ("didSelectCancel")]
		void DidSelectCancel ();

		[Export ("cancel")]
		void Cancel ();

		[Export ("isContentValid")]
		bool IsContentValid ();

		[Export ("validateContent")]
		void ValidateContent ();

		[NullAllowed] // by default this property is null
		[Export ("charactersRemaining", ArgumentSemantic.Strong)]
		NSNumber CharactersRemaining { get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("configurationItems")]
		SLComposeSheetConfigurationItem [] GetConfigurationItems ();

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("reloadConfigurationItems")]
		void ReloadConfigurationItems ();

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("pushConfigurationViewController:")]
		void PushConfigurationViewController (SocialViewController viewController);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("popConfigurationViewController")]
		void PopConfigurationViewController ();

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("loadPreviewView")]
		SocialView LoadPreviewView ();

		[NoMac]
		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("autoCompletionViewController", ArgumentSemantic.Strong)]
		SocialViewController AutoCompletionViewController { get; set; }

#if NET
		// Inlined manually from UITextViewDelegate/NSTextViewDelegate, because the one from the *Delegate type
		// has different availability attributes depending on the platform.
		[iOS (18, 0), MacCatalyst (18, 0), Mac (15, 0), MacCatalyst (18, 0), NoTV]
		[Export ("textViewWritingToolsWillBegin:")]
		new void WritingToolsWillBegin (SocialTextView textView);

		// Inlined manually from UITextViewDelegate/NSTextViewDelegate, because the one from the *Delegate type
		// has different availability attributes depending on the platform.
		[iOS (18, 0), MacCatalyst (18, 0), Mac (15, 0), MacCatalyst (18, 0), NoTV]
		[Export ("textViewWritingToolsDidEnd:")]
		new void WritingToolsDidEnd (SocialTextView textView);

		// Inlined manually from UITextViewDelegate/NSTextViewDelegate, because the one from the *Delegate type
		// has different availability attributes depending on the platform.
		[iOS (18, 0), MacCatalyst (18, 0), Mac (15, 0), MacCatalyst (18, 0), NoTV]
		[Export ("textView:writingToolsIgnoredRangesInEnclosingRange:")]
		new NSValue [] GetWritingToolsIgnoredRangesInEnclosingRange (SocialTextView textView, NSRange enclosingRange);
#endif
	}


	/// <summary>Allows users to configure properties of a post for social sharing.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/Social/Reference/SLComposeSheetConfigurationItem_Class/index.html">Apple documentation for <c>SLComposeSheetConfigurationItem</c></related>
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // designated
	interface SLComposeSheetConfigurationItem {

		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[NullAllowed] // by default this property is null
		[Export ("title")]
		string Title { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("value")]
		string Value { get; set; }

		[Export ("valuePending", ArgumentSemantic.Assign)]
		bool ValuePending { get; set; }

		[Export ("tapHandler", ArgumentSemantic.Copy)]
		Action TapHandler { get; set; }
	}
}
