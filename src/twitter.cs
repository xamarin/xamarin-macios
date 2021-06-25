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

	[Availability (Deprecated = Platform.iOS_6_0, Message = "Use the 'Social' framework.")]
	[BaseType (typeof (NSObject))]
	interface TWRequest {
		
		[Introduced (PlatformName.iOS, 6, 0, message: "Use the non-Apple SDK relating to your account type instead.")]
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use the non-Apple SDK relating to your account type instead.")]
		[Introduced (PlatformName.MacOSX, 10, 8, message: "Use the non-Apple SDK relating to your account type instead.")]
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use the non-Apple SDK relating to your account type instead.")]
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
