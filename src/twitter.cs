//
// ios5-twitter.cs: Twitter bindings
//
// Authors:
//   Miguel de Icaza
//

using System;
using Foundation;
using ObjCRuntime;
using UIKit;
using Twitter;
using Accounts;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Twitter {

	/// <summary>A delegate that is used as the <c>handler</c> in calls to <see cref="M:Twitter.TWRequest.PerformRequest(Twitter.TWRequestHandler)" />.</summary>
	delegate void TWRequestHandler (NSData responseData, NSHttpUrlResponse urlResponse, NSError error);

	/// <summary>A Twitter request.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/Twitter/Reference/TWRequestClassRef/index.html">Apple documentation for <c>TWRequest</c></related>
	[Deprecated (PlatformName.iOS, 6, 0, message: "Use the 'Social' framework.")]
	[BaseType (typeof (NSObject))]
	interface TWRequest {

		[NullAllowed] // by default this property is null
		[Export ("account")]
		ACAccount Account { get; set; }

		[Export ("requestMethod")]
		TWRequestMethod RequestMethod { get; }

		[Export ("URL")]
		NSUrl Url { get; }

		[Export ("parameters")]
		NSDictionary Parameters { get; }

		[Export ("initWithURL:parameters:requestMethod:")]
		NativeHandle Constructor (NSUrl url, [NullAllowed] NSDictionary parameters, TWRequestMethod requestMethod);

		[Export ("addMultiPartData:withName:type:")]
		void AddMultiPartData (NSData data, string name, string type);

		[Export ("signedURLRequest")]
		NSUrlRequest SignedUrlRequest { get; }

		[Export ("performRequestWithHandler:")]
		[Async (ResultTypeName = "TWRequestResult")]
		void PerformRequest (TWRequestHandler handler);
	}

	/// <summary>A <see cref="T:UIKit.UIViewController" /> that manages the user experience of composing a tweet.</summary>
	///     
	///     <related type="recipe" href="https://developer.xamarin.com/ios/Recipes/Shared_Resources/Twitter/Send_a_Tweet">Send a Tweet</related>
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/Twitter/Reference/TWTweetSheetViewControllerClassRef/index.html">Apple documentation for <c>TWTweetComposeViewController</c></related>
	[Deprecated (PlatformName.iOS, 6, 0, message: "Use the 'Social' framework.")]
	[BaseType (typeof (UIViewController))]
	interface TWTweetComposeViewController {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("completionHandler")]
		Action<TWTweetComposeViewControllerResult> CompletionHandler { get; set; }

		[Static]
		[Export ("canSendTweet")]
		bool CanSendTweet { get; }

		[Export ("setInitialText:")]
		bool SetInitialText (string text);

		[Export ("addImage:")]
		bool AddImage (UIImage image);

		[Export ("removeAllImages")]
		bool RemoveAllImages ();

		[Export ("addURL:")]
		bool AddUrl (NSUrl url);

		[Export ("removeAllURLs")]
		bool RemoveAllUrls ();
	}

}
