//
// Messages bindings
//
// Authors:
//	Vincent Dondain <vincent@xamarin.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

using System;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

#if !NET
using NativeHandle = System.IntPtr;
#endif

#if !MONOMAC
namespace Messages {

	[MacCatalyst (14, 0)]
	[Native]
	public enum MSMessagesAppPresentationStyle : ulong {
		Compact,
		Expanded,
		Transcript,
	}

	[MacCatalyst (14, 0)]
	[Native]
	public enum MSStickerSize : long {
		Small,
		Regular,
		Large
	}

	[MacCatalyst (14, 0)]
	[Native]
	[ErrorDomain ("MSMessagesErrorDomain")]
	public enum MSMessageErrorCode : long {
		Unknown = -1,
		FileNotFound = 1,
		FileUnreadable,
		ImproperFileType,
		ImproperFileUrl,
		StickerFileImproperFileAttributes,
		StickerFileImproperFileSize,
		StickerFileImproperFileFormat,
		UrlExceedsMaxSize,
		SendWithoutRecentInteraction,
		SendWhileNotVisible,
		APIUnavailableInPresentationContext,
	}

	[MacCatalyst (14, 0)]
	[iOS (12, 0)]
	[Native]
	public enum MSMessagesAppPresentationContext : long {
		Messages,
		Media,
	}

	[MacCatalyst (14, 0)]
	[Protocol]
	interface MSMessagesAppTranscriptPresentation {
		[Abstract]
		[Export ("contentSizeThatFits:")]
		CGSize GetContentSizeThatFits (CGSize size);
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (UIViewController))]
	interface MSMessagesAppViewController : MSMessagesAppTranscriptPresentation {
		// inlined ctor
		[Export ("initWithNibName:bundle:")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[NullAllowed, Export ("activeConversation", ArgumentSemantic.Strong)]
		MSConversation ActiveConversation { get; }

		[Export ("presentationStyle", ArgumentSemantic.Assign)]
		MSMessagesAppPresentationStyle PresentationStyle { get; }

		[Export ("requestPresentationStyle:")]
		void Request (MSMessagesAppPresentationStyle presentationStyle);

		[Export ("dismiss")]
		void Dismiss ();

		[Export ("willBecomeActiveWithConversation:")]
		void WillBecomeActive (MSConversation conversation);

		[Export ("didBecomeActiveWithConversation:")]
		void DidBecomeActive (MSConversation conversation);

		[Export ("willResignActiveWithConversation:")]
		void WillResignActive (MSConversation conversation);

		[Export ("didResignActiveWithConversation:")]
		void DidResignActive (MSConversation conversation);

		[Export ("willSelectMessage:conversation:")]
		void WillSelectMessage (MSMessage message, MSConversation conversation);

		[Export ("didSelectMessage:conversation:")]
		void DidSelectMessage (MSMessage message, MSConversation conversation);

		[Export ("didReceiveMessage:conversation:")]
		void DidReceiveMessage (MSMessage message, MSConversation conversation);

		[Export ("didStartSendingMessage:conversation:")]
		void DidStartSendingMessage (MSMessage message, MSConversation conversation);

		[Export ("didCancelSendingMessage:conversation:")]
		void DidCancelSendingMessage (MSMessage message, MSConversation conversation);

		[Export ("willTransitionToPresentationStyle:")]
		void WillTransition (MSMessagesAppPresentationStyle presentationStyle);

		[Export ("didTransitionToPresentationStyle:")]
		void DidTransition (MSMessagesAppPresentationStyle presentationStyle);

		[iOS (12, 0)]
		[MacCatalyst (14, 0)]
		[Export ("presentationContext")]
		MSMessagesAppPresentationContext PresentationContext { get; }
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	interface MSConversation {
		[Export ("localParticipantIdentifier")]
		NSUuid LocalParticipantIdentifier { get; }

		[Export ("remoteParticipantIdentifiers")]
		NSUuid [] RemoteParticipantIdentifiers { get; }

		[NullAllowed, Export ("selectedMessage")]
		MSMessage SelectedMessage { get; }

		[Export ("insertMessage:completionHandler:")]
		[Async]
		void InsertMessage (MSMessage message, [NullAllowed] Action<NSError> completionHandler);

		[Export ("insertSticker:completionHandler:")]
		[Async]
		void InsertSticker (MSSticker sticker, [NullAllowed] Action<NSError> completionHandler);

		[Export ("insertText:completionHandler:")]
		[Async]
		void InsertText (string text, [NullAllowed] Action<NSError> completionHandler);

		[Export ("insertAttachment:withAlternateFilename:completionHandler:")]
		[Async]
		void InsertAttachment (NSUrl url, [NullAllowed] string filename, [NullAllowed] Action<NSError> completionHandler);

		[MacCatalyst (14, 0)]
		[Export ("sendMessage:completionHandler:")]
		[Async]
		void SendMessage (MSMessage message, [NullAllowed] Action<NSError> completionHandler);

		[MacCatalyst (14, 0)]
		[Export ("sendSticker:completionHandler:")]
		[Async]
		void SendSticker (MSSticker sticker, [NullAllowed] Action<NSError> completionHandler);

		[MacCatalyst (14, 0)]
		[Export ("sendText:completionHandler:")]
		[Async]
		void SendText (string text, [NullAllowed] Action<NSError> completionHandler);

		[MacCatalyst (14, 0)]
		[Export ("sendAttachment:withAlternateFilename:completionHandler:")]
		[Async]
		void SendAttachment (NSUrl url, [NullAllowed] string filename, [NullAllowed] Action<NSError> completionHandler);
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // designated
	interface MSMessage : NSCopying, NSSecureCoding {
		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("initWithSession:")]
		[DesignatedInitializer]
		NativeHandle Constructor (MSSession session);

		[NullAllowed, Export ("session")]
		MSSession Session { get; }

		[MacCatalyst (14, 0)]
		[Export ("pending")]
		bool Pending { [Bind ("isPending")] get; }

		[Export ("senderParticipantIdentifier")]
		NSUuid SenderParticipantIdentifier { get; }

		[NullAllowed, Export ("layout", ArgumentSemantic.Copy)]
		MSMessageLayout Layout { get; set; }

		[NullAllowed, Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get; set; }

		[Export ("shouldExpire")]
		bool ShouldExpire { get; set; }

		[NullAllowed, Export ("accessibilityLabel")]
		string AccessibilityLabel { get; set; }

		[NullAllowed, Export ("summaryText")]
		string SummaryText { get; set; }

		[NullAllowed, Export ("error", ArgumentSemantic.Copy)]
		NSError Error { get; set; }
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[Abstract] // as per docs
	[DisableDefaultCtor]
	interface MSMessageLayout : NSCopying { }

	[MacCatalyst (14, 0)]
	[BaseType (typeof (MSMessageLayout))]
	interface MSMessageTemplateLayout {
		[NullAllowed, Export ("caption")]
		string Caption { get; set; }

		[NullAllowed, Export ("subcaption")]
		string Subcaption { get; set; }

		[NullAllowed, Export ("trailingCaption")]
		string TrailingCaption { get; set; }

		[NullAllowed, Export ("trailingSubcaption")]
		string TrailingSubcaption { get; set; }

		[NullAllowed, Export ("image", ArgumentSemantic.Strong)]
		UIImage Image { get; set; }

		[NullAllowed, Export ("mediaFileURL", ArgumentSemantic.Copy)]
		NSUrl MediaFileUrl { get; set; }

		[NullAllowed, Export ("imageTitle")]
		string ImageTitle { get; set; }

		[NullAllowed, Export ("imageSubtitle")]
		string ImageSubtitle { get; set; }
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	interface MSSession : NSSecureCoding { }

	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MSSticker {
		[Export ("initWithContentsOfFileURL:localizedDescription:error:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSUrl fileUrl, string localizedDescription, [NullAllowed] out NSError error);

		[iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("initWithFileURL:identifier:localizedDescription:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSUrl url, NSUuid identifier, string localizedDescription);

		[Export ("imageFileURL", ArgumentSemantic.Strong)]
		NSUrl ImageFileUrl { get; }

		[Export ("localizedDescription")]
		string LocalizedDescription { get; }
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (UIView))]
	interface MSStickerView {
		// inlined ctor
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Export ("initWithFrame:sticker:")]
		NativeHandle Constructor (CGRect frame, [NullAllowed] MSSticker sticker);

		[NullAllowed, Export ("sticker", ArgumentSemantic.Strong)]
		MSSticker Sticker { get; set; }

		[Export ("animationDuration")]
		double AnimationDuration { get; }

		[Export ("startAnimating")]
		void StartAnimating ();

		[Export ("stopAnimating")]
		void StopAnimating ();

		[Export ("isAnimating")]
		bool IsAnimating { get; }
	}

	interface IMSStickerBrowserViewDataSource { }

	[MacCatalyst (14, 0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface MSStickerBrowserViewDataSource {
		[Abstract]
		[Export ("numberOfStickersInStickerBrowserView:")]
		nint GetNumberOfStickers (MSStickerBrowserView stickerBrowserView);

		[Abstract]
		[Export ("stickerBrowserView:stickerAtIndex:")]
		MSSticker GetSticker (MSStickerBrowserView stickerBrowserView, nint index);
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (UIView))]
	interface MSStickerBrowserView {
		[Export ("initWithFrame:")]
		[DesignatedInitializer]
		NativeHandle Constructor (CGRect frame);

		[Export ("initWithFrame:stickerSize:")]
		[DesignatedInitializer]
		NativeHandle Constructor (CGRect frame, MSStickerSize stickerSize);

		[Export ("stickerSize", ArgumentSemantic.Assign)]
		MSStickerSize StickerSize { get; }

		[NullAllowed, Export ("dataSource", ArgumentSemantic.Weak)]
		IMSStickerBrowserViewDataSource DataSource { get; set; }

		[Export ("contentOffset", ArgumentSemantic.Assign)]
		CGPoint ContentOffset { get; set; }

		[Export ("contentInset", ArgumentSemantic.Assign)]
		UIEdgeInsets ContentInset { get; set; }

		[Export ("setContentOffset:animated:")]
		void SetContentOffset (CGPoint contentOffset, bool animated);

		[Export ("reloadData")]
		void ReloadData ();
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (UIViewController))]
	interface MSStickerBrowserViewController : MSStickerBrowserViewDataSource {
		[Export ("initWithStickerSize:")]
		[DesignatedInitializer]
		NativeHandle Constructor (MSStickerSize stickerSize);

		[Export ("stickerBrowserView", ArgumentSemantic.Strong)]
		MSStickerBrowserView StickerBrowserView { get; }

		[Export ("stickerSize")]
		MSStickerSize StickerSize { get; }
	}

	[MacCatalyst (14, 0)]
	[BaseType (typeof (MSMessageLayout))]
	[DisableDefaultCtor]
	interface MSMessageLiveLayout {
		[Export ("initWithAlternateLayout:")]
		[DesignatedInitializer]
		NativeHandle Constructor (MSMessageTemplateLayout alternateLayout);

		[Export ("alternateLayout")]
		MSMessageTemplateLayout AlternateLayout { get; }
	}
}
#endif // !MONOMAC
