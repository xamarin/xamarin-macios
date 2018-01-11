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

namespace Twitter {

	delegate void TWRequestHandler (NSData responseData, NSHttpUrlResponse urlResponse, NSError error);

#if !XAMCORE_2_0
	delegate void TWTweetComposeHandler (TWTweetComposeViewControllerResult result);
#endif
	
	[Availability (Deprecated = Platform.iOS_6_0, Message = "Use the 'Social' framework.")]
	[BaseType (typeof (NSObject))]
	interface TWRequest {
		[NullAllowed] // by default this property is null
		[Export ("account")]
		ACAccount Account { get; set;  }

		[Export ("requestMethod")]
		TWRequestMethod RequestMethod { get; }

		[Export ("URL")]
		NSUrl Url { get;  }

		[Export ("parameters")]
		NSDictionary Parameters { get;  }

		[Export ("initWithURL:parameters:requestMethod:")]
		IntPtr Constructor (NSUrl url, [NullAllowed] NSDictionary parameters, TWRequestMethod requestMethod);

		[Export ("addMultiPartData:withName:type:")]
		void AddMultiPartData (NSData data, string name, string type);

		[Export ("signedURLRequest")]
		NSUrlRequest SignedUrlRequest { get; }

		[Export ("performRequestWithHandler:")]
		[Async (ResultTypeName = "TWRequestResult")]
		void PerformRequest (TWRequestHandler handler);
	}

	[Availability (Deprecated = Platform.iOS_6_0, Message = "Use the 'Social' framework.")]
	[BaseType (typeof (UIViewController))]
	interface TWTweetComposeViewController {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

#if XAMCORE_2_0
		[Export ("completionHandler")]
		Action<TWTweetComposeViewControllerResult> CompletionHandler { get; set; }
#else
		[Export ("setCompletionHandler:")]
		void SetCompletionHandler (TWTweetComposeHandler handler);
#endif

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