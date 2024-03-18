//
// MessageUI.cs: This file describes the API that the generator will produce for MessageUI
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2009, Novell, Inc.
// Copyright 2011, 2013 Xamarin, Inc.
//

using System;
using ObjCRuntime;
using Foundation;
using CoreFoundation;
using Messages;
using UIKit;

namespace MessageUI {

	[BaseType (typeof (UINavigationController))]
	interface MFMailComposeViewController : UIAppearance {
		[Static, Export ("canSendMail")]
		bool CanSendMail { get; }

		[Export ("mailComposeDelegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		NSObject WeakMailComposeDelegate { get; set; }

		[Wrap ("WeakMailComposeDelegate")]
		IMFMailComposeViewControllerDelegate MailComposeDelegate { get; set; }

		[Export ("setSubject:")]
		void SetSubject (string subject);

		[Export ("setToRecipients:")]
		void SetToRecipients ([NullAllowed] string [] recipients);

		[Export ("setCcRecipients:")]
		void SetCcRecipients ([NullAllowed] string [] ccRecipients);

		[Export ("setBccRecipients:")]
		void SetBccRecipients ([NullAllowed] string [] bccRecipients);

		[Export ("setMessageBody:isHTML:")]
		void SetMessageBody (string body, bool isHtml);

		[Export ("addAttachmentData:mimeType:fileName:")]
		void AddAttachmentData (NSData attachment, string mimeType, string fileName);

		[MacCatalyst (13, 1)]
		[Export ("setPreferredSendingEmailAddress:")]
		void SetPreferredSendingEmailAddress (string emailAddress);
	}

	interface IMFMailComposeViewControllerDelegate { }

#if XAMCORE_3_0
	[BaseType (typeof (NSObject))]
#else
	[BaseType (typeof (UINavigationControllerDelegate))]
#endif
	[Model]
	[Protocol]
	interface MFMailComposeViewControllerDelegate {
		[Export ("mailComposeController:didFinishWithResult:error:")]
		void Finished (MFMailComposeViewController controller, MFMailComposeResult result, [NullAllowed] NSError error);
	}

	interface MFMessageAvailabilityChangedEventArgs {
		[Export ("MFMessageComposeViewControllerTextMessageAvailabilityKey")]
		bool TextMessageAvailability { get; }
	}

	[BaseType (typeof (UINavigationController))]
	interface MFMessageComposeViewController : UIAppearance {
		[Export ("messageComposeDelegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakMessageComposeDelegate { get; set; }

		[Wrap ("WeakMessageComposeDelegate")]
		IMFMessageComposeViewControllerDelegate MessageComposeDelegate { get; set; }

		[NullAllowed]
		[Export ("recipients", ArgumentSemantic.Copy)]
		string [] Recipients { get; set; }

		[NullAllowed]
		[Export ("body", ArgumentSemantic.Copy)]
		string Body { get; set; }

		[Static]
		[Export ("canSendText")]
		bool CanSendText { get; }

		[Static]
		[Export ("canSendAttachments")]
		bool CanSendAttachments { get; }

		[Static]
		[Export ("canSendSubject")]
		bool CanSendSubject { get; }

		[Static]
		[Export ("isSupportedAttachmentUTI:")]
		bool IsSupportedAttachment (string uti);

		[NullAllowed]
		[Export ("subject", ArgumentSemantic.Copy)]
		string Subject { get; set; }

		[return: NullAllowed]
		[Export ("attachments")]
		NSDictionary [] GetAttachments ();

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("message", ArgumentSemantic.Copy)]
		MSMessage Message { get; set; }

		[Export ("addAttachmentURL:withAlternateFilename:")]
		bool AddAttachment (NSUrl attachmentURL, [NullAllowed] string alternateFilename);

		[Export ("addAttachmentData:typeIdentifier:filename:")]
		bool AddAttachment (NSData attachmentData, string uti, string filename);

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch, NoTV]
		[Export ("insertCollaborationItemProvider:")]
		bool InsertCollaboration (NSItemProvider itemProvider);

		[Export ("disableUserAttachments")]
		void DisableUserAttachments ();

		[Field ("MFMessageComposeViewControllerTextMessageAvailabilityDidChangeNotification")]
		[Notification (typeof (MFMessageAvailabilityChangedEventArgs))]
		NSString TextMessageAvailabilityDidChangeNotification { get; }

		[Field ("MFMessageComposeViewControllerTextMessageAvailabilityKey")]
		NSString TextMessageAvailabilityKey { get; }

		[Field ("MFMessageComposeViewControllerAttachmentAlternateFilename")]
		NSString AttachmentAlternateFilename { get; }

		[Field ("MFMessageComposeViewControllerAttachmentURL")]
		NSString AttachmentURL { get; }

		[Async]
		[iOS (17, 0), NoMacCatalyst, NoWatch, NoTV]
		[Export ("setUPIVerificationCodeSendCompletion:")]
		void SetUpiVerificationCodeSendCompletion (Action<bool> completion);
	}

	interface IMFMessageComposeViewControllerDelegate { }

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface MFMessageComposeViewControllerDelegate {
		[Abstract]
		[Export ("messageComposeViewController:didFinishWithResult:")]
		void Finished (MFMessageComposeViewController controller, MessageComposeResult result);
	}
}
