//
// newstandkit.cs: Definitions for the iOS5 NewsstandKit
//
// Copyright 2011, 2012 Xamarin, Inc.
//
// Author:
//  Miguel de Icaza
//
using Foundation;
using ObjCRuntime;

namespace NewsstandKit {

	[BaseType (typeof (NSObject))]
	// <quote>You create an NKAssetDownload instance using the NKIssue method addAssetWithRequest:</quote> -> http://developer.apple.com/library/ios/#documentation/StoreKit/Reference/NKAssetDownload_Class/NKAssetDownload/NKAssetDownload.html
	// init returns NIL
	[DisableDefaultCtor]
	interface NKAssetDownload {
		[Export ("issue", ArgumentSemantic.Weak)]
		NKIssue Issue { get;  }

		[Export ("identifier", ArgumentSemantic.Copy)]
		string Identifier { get;  }

		[Export ("userInfo", ArgumentSemantic.Copy)]
		NSDictionary UserInfo { get; set;  }

		[Export ("URLRequest", ArgumentSemantic.Copy)]
		NSUrlRequest UrlRequest { get;  }

		[Export ("downloadWithDelegate:")]
		NSUrlConnection DownloadWithDelegate ([Protocolize] NSUrlConnectionDownloadDelegate downloadDelegate);
	}

	[BaseType (typeof (NSObject))]
	// <quote>An NKIssue object must have a name and a date. When you create the object using the addIssueWithName:date: method of the NKLibrary class, you must supply these two values.</quote>
	// http://developer.apple.com/library/ios/#documentation/StoreKit/Reference/NKIssue_Class/NKIssue/NKIssue.html#//apple_ref/occ/cl/NKIssue
	// init returns NIL
	[DisableDefaultCtor]
	interface NKIssue {
		[Export ("downloadingAssets", ArgumentSemantic.Copy)]
		NKAssetDownload [] DownloadingAssets { get;  }

		[Export ("contentURL", ArgumentSemantic.Copy)]
		NSUrl ContentUrl { get;  }

		[Export ("status")]
		NKIssueContentStatus Status { get;  }

		[Export ("name", ArgumentSemantic.Copy)]
		string Name { get;  }

		[Export ("date", ArgumentSemantic.Copy)]
		NSDate Date { get;  }

		[Export ("addAssetWithRequest:")]
		NKAssetDownload AddAsset (NSUrlRequest request);

		[Field ("NKIssueDownloadCompletedNotification")]
		[Notification]
		NSString DownloadCompletedNotification { get; }
	}

	[BaseType (typeof (NSObject))]
	// init returns NIL -> sharedLibrary
	[DisableDefaultCtor]
	interface NKLibrary {
		[Export ("issues", ArgumentSemantic.Strong)]
		NKIssue [] Issues { get;  }

		[Export ("downloadingAssets", ArgumentSemantic.Strong)]
		NKAssetDownload [] DownloadingAssets { get;  }

		[Export ("currentlyReadingIssue", ArgumentSemantic.Strong)]
		NKIssue CurrentlyReadingIssue { get; set;  }

		[Static]
		[Export ("sharedLibrary")]
		NKLibrary SharedLibrary { get; }

		[Export ("issueWithName:")]
		NKIssue GetIssue (string name);

		[Export ("addIssueWithName:date:")]
		NKIssue AddIssue (string name, NSDate date);

		[Export ("removeIssue:")]
		void RemoveIssue (NKIssue issue);
	}
	
}
