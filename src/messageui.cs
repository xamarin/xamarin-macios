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

	[iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	enum MFMailComposeControllerDeferredAction : long {
		None,
		AdjustInsertionPoint,
		AddMissingRecipients,
	}

	/// <summary>Provides a user interface for composing and sending email messages.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/MessageUI/Reference/MFMailComposeViewController_class/index.html">Apple documentation for <c>MFMailComposeViewController</c></related>
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

	/// <summary>Interface representing the required methods (if any) of the protocol <see cref="T:MessageUI.MFMailComposeViewControllerDelegate" />.</summary>
	///     <remarks>
	///       <para>This interface contains the required methods (if any) from the protocol defined by <see cref="T:MessageUI.MFMailComposeViewControllerDelegate" />.</para>
	///       <para>If developers create classes that implement this interface, the implementation methods will automatically be exported to Objective-C with the matching signature from the method defined in the <see cref="T:MessageUI.MFMailComposeViewControllerDelegate" /> protocol.</para>
	///       <para>Optional methods (if any) are provided by the <see cref="T:MessageUI.MFMailComposeViewControllerDelegate_Extensions" /> class as extension methods to the interface, allowing developers to invoke any optional methods on the protocol.</para>
	///     </remarks>
	interface IMFMailComposeViewControllerDelegate { }

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface MFMailComposeViewControllerDelegate {
		[Export ("mailComposeController:didFinishWithResult:error:")]
		void Finished (MFMailComposeViewController controller, MFMailComposeResult result, [NullAllowed] NSError error);
	}

	/// <summary>Provides data for the  event.</summary>
	interface MFMessageAvailabilityChangedEventArgs {
		[Export ("MFMessageComposeViewControllerTextMessageAvailabilityKey")]
		bool TextMessageAvailability { get; }
	}

	/// <summary>Provides a user interface for composing and sending messages.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/MessageUI/Reference/MFMessageComposeViewController_class/index.html">Apple documentation for <c>MFMessageComposeViewController</c></related>
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

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoTV]
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
		[iOS (17, 0), NoMacCatalyst, NoTV]
		[Export ("setUPIVerificationCodeSendCompletion:")]
		void SetUpiVerificationCodeSendCompletion (Action<bool> completion);
	}

	/// <summary>Interface representing the required methods (if any) of the protocol <see cref="T:MessageUI.MFMessageComposeViewControllerDelegate" />.</summary>
	///     <remarks>
	///       <para>This interface contains the required methods (if any) from the protocol defined by <see cref="T:MessageUI.MFMessageComposeViewControllerDelegate" />.</para>
	///       <para>If developers create classes that implement this interface, the implementation methods will automatically be exported to Objective-C with the matching signature from the method defined in the <see cref="T:MessageUI.MFMessageComposeViewControllerDelegate" /> protocol.</para>
	///       <para>Optional methods (if any) are provided by the <see cref="T:MessageUI.MFMessageComposeViewControllerDelegate_Extensions" /> class as extension methods to the interface, allowing developers to invoke any optional methods on the protocol.</para>
	///     </remarks>
	interface IMFMessageComposeViewControllerDelegate { }

	/// <summary>Used to receive notifications from the MFMessageComposeViewController class.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/MessageUI/Reference/MFMessageComposeViewControllerDelegate_protocol/index.html">Apple documentation for <c>MFMessageComposeViewControllerDelegate</c></related>
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface MFMessageComposeViewControllerDelegate {
		[Abstract]
		[Export ("messageComposeViewController:didFinishWithResult:")]
		void Finished (MFMessageComposeViewController controller, MessageComposeResult result);
	}
}
