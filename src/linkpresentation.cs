//
// LinkPresentation C# bindings
//
// Authors:
//	TJ Lambert  <t-anlamb@microsoft.com>
//
// Copyright 2019 Microsoft Corporation All rights reserved.
//

#if MONOMAC
using AppKit;
using UIView = AppKit.NSView;
#else
using UIKit;
#endif

using System;
using CoreGraphics;
using Foundation;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace LinkPresentation {

	[ErrorDomain ("LPErrorDomain")]
	[iOS (13, 0)]
	[TV (14, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum LPErrorCode : long {
		Unknown = 1,
		MetadataFetchFailed,
		MetadataFetchCancelled,
		MetadataFetchTimedOut,
	}

	[iOS (13, 0)]
	[TV (14, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface LPLinkMetadata : NSCopying, NSSecureCoding {

		[NullAllowed, Export ("originalURL", ArgumentSemantic.Retain)]
		NSUrl OriginalUrl { get; set; }

		[NullAllowed, Export ("URL", ArgumentSemantic.Retain)]
		NSUrl Url { get; set; }

		[NullAllowed, Export ("title", ArgumentSemantic.Copy)]
		string Title { get; set; }

		[NullAllowed, Export ("iconProvider", ArgumentSemantic.Retain)]
		NSItemProvider IconProvider { get; set; }

		[NullAllowed, Export ("imageProvider", ArgumentSemantic.Retain)]
		NSItemProvider ImageProvider { get; set; }

		[NullAllowed, Export ("videoProvider", ArgumentSemantic.Retain)]
		NSItemProvider VideoProvider { get; set; }

		[NullAllowed, Export ("remoteVideoURL", ArgumentSemantic.Retain)]
		NSUrl RemoteVideoUrl { get; set; }
	}

	[iOS (13, 0)]
	[TV (14, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIView))]
	interface LPLinkView {

		[Export ("initWithFrame:")]
		[DesignatedInitializer]
		NativeHandle Constructor (CGRect rect);

		[Export ("initWithURL:")]
		NativeHandle Constructor (NSUrl url);

		[Export ("initWithMetadata:")]
		NativeHandle Constructor (LPLinkMetadata metadata);

		[Export ("metadata", ArgumentSemantic.Copy)]
		LPLinkMetadata Metadata { get; set; }
	}

	[iOS (13, 0)]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface LPMetadataProvider {

		[Async]
		[Export ("startFetchingMetadataForURL:completionHandler:")]
		void StartFetchingMetadata (NSUrl url, Action<LPLinkMetadata, NSError> completionHandler);

		[Async]
		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("startFetchingMetadataForRequest:completionHandler:")]
		void StartFetchingMetadata (NSUrlRequest request, Action<LPLinkMetadata, NSError> completionHandler);

		[Export ("cancel")]
		void Cancel ();

		[Export ("shouldFetchSubresources")]
		bool ShouldFetchSubresources { get; set; }

		[Export ("timeout")]
		double Timeout { get; set; }
	}
}
