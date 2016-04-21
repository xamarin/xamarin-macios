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
using XamCore.ObjCRuntime;
using XamCore.Foundation;
using XamCore.CoreFoundation;
using XamCore.UIKit;

namespace XamCore.MessageUI {

	[BaseType (typeof (UINavigationController))]
	interface MFMailComposeViewController : UIAppearance {
		[Static, Export ("canSendMail")]
		bool CanSendMail { get; }

		[Export ("mailComposeDelegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakMailComposeDelegate { get; set; }

		[Wrap ("WeakMailComposeDelegate")]
		[Protocolize]
		MFMailComposeViewControllerDelegate MailComposeDelegate { get; set; }

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
	}

#if XAMCORE_3_0
	[BaseType (typeof (NSObject))]
#else
	[BaseType (typeof (UINavigationControllerDelegate))]
#endif
	[Model]
	[Protocol]
	interface MFMailComposeViewControllerDelegate {
		[Export ("mailComposeController:didFinishWithResult:error:")]
		void Finished (MFMailComposeViewController controller, MFMailComposeResult result, NSError error);
	}	

	interface MFMessageAvailabilityChangedEventArgs {
		[Export ("MFMessageComposeViewControllerTextMessageAvailabilityKey")]
		bool TextMessageAvailability { get; }
	}
	
	[Since (4,0)]
	[BaseType (typeof (UINavigationController))]
	interface MFMessageComposeViewController : UIAppearance {
		[Export ("messageComposeDelegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakMessageComposeDelegate { get; set; }
		
		[Wrap ("WeakMessageComposeDelegate")]
		[Protocolize]
		MFMessageComposeViewControllerDelegate MessageComposeDelegate { get; set;  }

		[Export ("recipients", ArgumentSemantic.Copy)]
		string [] Recipients { get; set;  }
		
		[Export ("body", ArgumentSemantic.Copy)]
		string Body { get; set;  }
		
		[Static]
		[Export ("canSendText")]
		bool CanSendText { get; }

		[Since (7,0)]
		[Static][Export ("canSendAttachments")]
		bool CanSendAttachments { get; }

		[Since (7,0)]
		[Static][Export ("canSendSubject")]
		bool CanSendSubject { get; }

		[Since (7,0)]
		[Static][Export ("isSupportedAttachmentUTI:")]
		bool IsSupportedAttachment (string uti);

		[Since (7,0)]
		[Export ("subject", ArgumentSemantic.Copy)]
		string Subject { get; set; }

		[Since (7,0)]
		[Export ("attachments")]
		NSDictionary[] GetAttachments ();

		[Since (7,0)]
		[Export ("addAttachmentURL:withAlternateFilename:")]
		bool AddAttachment (NSUrl attachmentURL, [NullAllowed] string alternateFilename);

		[Since (7,0)]
		[Export ("addAttachmentData:typeIdentifier:filename:")]
		bool AddAttachment (NSData attachmentData, string uti, [NullAllowed] string filename);

		[Since (7,0)]
		[Export ("disableUserAttachments")]
		void DisableUserAttachments ();

		[Since (5,0)]
		[Field ("MFMessageComposeViewControllerTextMessageAvailabilityDidChangeNotification")]
		[Notification (typeof (MFMessageAvailabilityChangedEventArgs))]
		NSString TextMessageAvailabilityDidChangeNotification { get; }

		[Since (5,0)]
		[Field ("MFMessageComposeViewControllerTextMessageAvailabilityKey")]
		NSString TextMessageAvailabilityKey { get; }

		[Since (7,0)]
		[Field ("MFMessageComposeViewControllerAttachmentAlternateFilename")]
		NSString AttachmentAlternateFilename { get; }

		[Since (7,0)]
		[Field ("MFMessageComposeViewControllerAttachmentURL")]
		NSString AttachmentURL { get; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface MFMessageComposeViewControllerDelegate {
		[Abstract]
		[Export ("messageComposeViewController:didFinishWithResult:")]
		void Finished (MFMessageComposeViewController controller, MessageComposeResult result);
	}
}
