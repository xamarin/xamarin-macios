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

	delegate void TWRequestHandler (NSData responseData, NSHttpUrlResponse urlResponse, NSError error);

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
