using System;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using AVFoundation;
using AppKit;

namespace MailKit {
	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[Native]
	[ErrorDomain ("MEComposeSessionErrorDomain")]
	public enum MEComposeSessionErrorCode : long
	{
		Recipients = 0,
		Headers = 1,
		Body = 2,
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[Native]
	public enum MEMessageActionMessageColor : long
	{
		None,
		Green,
		Yellow,
		Orange,
		Red,
		Purple,
		Blue,
		Gray,
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[Native]
	public enum MEMessageState : long
	{
		Received = 0,
		Draft = 1,
		Sending = 2,
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MEMessage : NSSecureCoding
	{
		[Export ("state", ArgumentSemantic.Assign)]
		MEMessageState State { get; }

		[Export ("subject")]
		string Subject { get; }

		[Export ("fromAddress", ArgumentSemantic.Copy)]
		MEEmailAddress FromAddress { get; }

		[Export ("toAddresses", ArgumentSemantic.Copy)]
		MEEmailAddress[] ToAddresses { get; }

		[Export ("ccAddresses", ArgumentSemantic.Copy)]
		MEEmailAddress[] CcAddresses { get; }

		[Export ("bccAddresses", ArgumentSemantic.Copy)]
		MEEmailAddress[] BccAddresses { get; }

		[Export ("replyToAddresses", ArgumentSemantic.Copy)]
		MEEmailAddress[] ReplyToAddresses { get; }

		[Export ("allRecipientAddresses", ArgumentSemantic.Copy)]
		MEEmailAddress[] AllRecipientAddresses { get; }

		[Export ("dateSent", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSDate DateSent { get; }

		[Export ("dateReceived", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSDate DateReceived { get; }

		[NullAllowed, Export ("headers", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSArray<NSString>> Headers { get; }

		[NullAllowed, Export ("rawData", ArgumentSemantic.Copy)]
		NSData RawData { get; }
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MEMessageAction : NSSecureCoding
	{
		[Static]
		[Export ("moveToTrashAction")]
		MEMessageAction MoveToTrash { get; }

		[Static]
		[Export ("moveToArchiveAction")]
		MEMessageAction MoveToArchive { get; }

		[Static]
		[Export ("moveToJunkAction")]
		MEMessageAction MoveToJunk { get; }

		[Static]
		[Export ("markAsReadAction")]
		MEMessageAction MarkAsRead { get; }

		[Static]
		[Export ("markAsUnreadAction")]
		MEMessageAction MarkAsUnread { get; }

#if false // does not respond (nor work in ObjC) with macOS 12 beta 6
		[Static]
		[Export ("flagAction")]
		MEMessageAction Flag { get; }

		[Static]
		[Export ("unflagAction")]
		MEMessageAction Unflag { get; }

		[Static]
		[Export ("setColorActionWithColor:")]
		MEMessageAction SetColor (MEMessageActionMessageColor color);
#endif
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MEMessageActionDecision : NSSecureCoding
	{
		[Static]
		[Export ("invokeAgainWithBody")]
		MEMessageActionDecision InvokeAgainWithBody { get; }

		[Static]
		[Export ("decisionApplyingAction:")]
		MEMessageActionDecision Apply (MEMessageAction action);

		[Static]
		[Export ("decisionApplyingActions:")]
		MEMessageActionDecision Apply (MEMessageAction[] actions);
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MEMessageEncodingResult : NSSecureCoding
	{
		[Export ("initWithEncodedMessage:signingError:encryptionError:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] MEEncodedOutgoingMessage encodedMessage, [NullAllowed] NSError signingError, [NullAllowed] NSError encryptionError);

		[NullAllowed, Export ("encodedMessage", ArgumentSemantic.Copy)]
		MEEncodedOutgoingMessage EncodedMessage { get; }

		[NullAllowed, Export ("signingError", ArgumentSemantic.Copy)]
		NSError SigningError { get; }

		[NullAllowed, Export ("encryptionError", ArgumentSemantic.Copy)]
		NSError EncryptionError { get; }
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MEMessageSecurityInformation : NSSecureCoding
	{
		[Export ("initWithSigners:isEncrypted:signingError:encryptionError:")]
		IntPtr Constructor (MEMessageSigner[] signers, bool isEncrypted, [NullAllowed] NSError signingError, [NullAllowed] NSError encryptionError);

		[Export ("initWithSigners:isEncrypted:signingError:encryptionError:shouldBlockRemoteContent:localizedRemoteContentBlockingReason:")]
		IntPtr Constructor (MEMessageSigner[] signers, bool isEncrypted, [NullAllowed] NSError signingError, [NullAllowed] NSError encryptionError, bool shouldBlockRemoteContent, [NullAllowed] string localizedRemoteContentBlockingReason);

		[Export ("signers", ArgumentSemantic.Strong)]
		MEMessageSigner[] Signers { get; }

		[Export ("isEncrypted")]
		bool IsEncrypted { get; }

		[NullAllowed, Export ("signingError", ArgumentSemantic.Strong)]
		NSError SigningError { get; }

		[NullAllowed, Export ("encryptionError", ArgumentSemantic.Strong)]
		NSError EncryptionError { get; }

		[NullAllowed, Export ("localizedRemoteContentBlockingReason", ArgumentSemantic.Strong)]
		string LocalizedRemoteContentBlockingReason { get; }

		[Export ("shouldBlockRemoteContent")]
		bool ShouldBlockRemoteContent { get; }
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MEMessageSigner : NSSecureCoding
	{
		[Export ("initWithEmailAddresses:signatureLabel:context:")]
		IntPtr Constructor (MEEmailAddress[] emailAddresses, string label, [NullAllowed] NSData context);

		[Export ("emailAddresses", ArgumentSemantic.Copy)]
		MEEmailAddress[] EmailAddresses { get; }

		[Export ("label")]
		string Label { get; }

		[Export ("context")]
		NSData Context { get; }
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MEOutgoingMessageEncodingStatus : NSSecureCoding
	{
		[Export ("initWithCanSign:canEncrypt:securityError:addressesFailingEncryption:")]
		IntPtr Constructor (bool canSign, bool canEncrypt, [NullAllowed] NSError securityError, MEEmailAddress[] addressesFailingEncryption);

		[Export ("canSign")]
		bool CanSign { get; }

		[Export ("canEncrypt")]
		bool CanEncrypt { get; }

		[NullAllowed, Export ("securityError", ArgumentSemantic.Copy)]
		NSError SecurityError { get; }

		[Export ("addressesFailingEncryption", ArgumentSemantic.Copy)]
		MEEmailAddress[] AddressesFailingEncryption { get; }
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MEAddressAnnotation : NSSecureCoding
	{
		[Static]
		[Export ("errorWithLocalizedDescription:")]
		MEAddressAnnotation CreateErrorAnnotation (string localizedDescription);

		[Static]
		[Export ("warningWithLocalizedDescription:")]
		MEAddressAnnotation CreateWarningAnnotation (string localizedDescription);

		[Static]
		[Export ("successWithLocalizedDescription:")]
		MEAddressAnnotation CreateSuccessAnnotation (string localizedDescription);
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[Advice ("'MEComposeSession' is not available in UIKit on macOS.")]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MEComposeSession : NSSecureCoding
	{
		[Export ("sessionID", ArgumentSemantic.Strong)]
		NSUuid SessionId { get; }

		[Export ("mailMessage", ArgumentSemantic.Strong)]
		MEMessage MailMessage { get; }

		[Export ("reloadSession")]
		void ReloadSession ();
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MEDecodedMessage : NSSecureCoding
	{
		[Export ("initWithData:securityInformation:")]
		IntPtr Constructor ([NullAllowed] NSData rawData, MEMessageSecurityInformation securityInformation);

		[NullAllowed, Export ("rawData", ArgumentSemantic.Copy)]
		NSData RawData { get; }

		[Export ("securityInformation", ArgumentSemantic.Strong)]
		MEMessageSecurityInformation SecurityInformation { get; }
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[BaseType (typeof (NSObject))]
	interface MEEncodedOutgoingMessage : NSSecureCoding
	{
		[Export ("initWithRawData:isSigned:isEncrypted:")]
		IntPtr Constructor (NSData rawData, bool isSigned, bool isEncrypted);

		[Export ("rawData", ArgumentSemantic.Copy)]
		NSData RawData { get; }

		[Export ("isSigned")]
		bool IsSigned { get; }

		[Export ("isEncrypted")]
		bool IsEncrypted { get; }
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MEEmailAddress : NSSecureCoding, NSCopying
	{
		[Export ("initWithRawString:")]
		IntPtr Constructor (string rawString);

		[Export ("rawString")]
		string RawString { get; }

		[NullAllowed, Export ("addressString")]
		string AddressString { get; }
	}

	interface IMEComposeSessionHandler {}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[Protocol]
	interface MEComposeSessionHandler
	{
		[Abstract]
		[Export ("mailComposeSessionDidBegin:")]
		void MailComposeSessionDidBegin (MEComposeSession session);

		[Abstract]
		[Export ("mailComposeSessionDidEnd:")]
		void MailComposeSessionDidEnd (MEComposeSession session);

		[Abstract]
		[Export ("viewControllerForSession:")]
		MEExtensionViewController GetViewController (MEComposeSession session);

		[Export ("session:annotateAddressesWithCompletionHandler:")]
		void AnnotateAddress (MEComposeSession session, Action<NSDictionary<MEEmailAddress, MEAddressAnnotation>> completionHandler);

		[Export ("session:canSendMessageWithCompletionHandler:")]
		void AllowMessageSend (MEComposeSession session, Action<NSError> completionHandler);

		[Export ("additionalHeadersForSession:")]
		NSDictionary<NSString, NSArray<NSString>> GetAdditionalHeaders (MEComposeSession session);
	}

	interface IMEContentBlocker {}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[Protocol]
	interface MEContentBlocker
	{
		[Abstract]
		[Export ("contentRulesJSON")]
		NSData ContentRulesJson { get; }
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[Protocol]
	interface MEExtension
	{
		[Export ("handlerForComposeSession:")]
		IMEComposeSessionHandler GetHandlerForComposeSession (MEComposeSession session);

		[Export ("handlerForMessageActions")]
		IMEMessageActionHandler HandlerForMessageActions { get; }

		[Export ("handlerForContentBlocker")]
		IMEContentBlocker HandlerForContentBlocker { get; }

		[Export ("handlerForMessageSecurity")]
		IMEMessageSecurityHandler HandlerForMessageSecurity { get; }
	}

	interface IMEMessageActionHandler {}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[Protocol]
	interface MEMessageActionHandler
	{
		[Abstract]
		[Export ("decideActionForMessage:completionHandler:")]
		void DecideAction (MEMessage message, Action<MEMessageActionDecision> completionHandler);

		[Export ("requiredHeaders", ArgumentSemantic.Copy)]
		string[] RequiredHeaders { get; }
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[Protocol]
	interface MEMessageDecoder
	{
		[Abstract]
		[Export ("decodedMessageForMessageData:")]
		[return: NullAllowed]
		MEDecodedMessage DecodedMessage (NSData data);
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[Protocol]
	interface MEMessageEncoder
	{
		[Abstract]
		[Export ("getEncodingStatusForMessage:completionHandler:")]
		void GetEncodingStatus (MEMessage message, Action<MEOutgoingMessageEncodingStatus> completionHandler);

		[Abstract]
		[Export ("encodeMessage:shouldSign:shouldEncrypt:completionHandler:")]
		void EncodeMessage (MEMessage message, bool shouldSign, bool shouldEncrypt, Action<MEMessageEncodingResult> completionHandler);
	}

	interface IMEMessageSecurityHandler {}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[Protocol]
	interface MEMessageSecurityHandler : MEMessageEncoder, MEMessageDecoder
	{
		[Abstract]
		[Export ("extensionViewControllerForMessageSigners:")]
		[return: NullAllowed]
		MEExtensionViewController GetExtensionViewController (MEMessageSigner[] messageSigners);
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[DefaultCtorVisibility (Visibility.Protected)]
	[BaseType (typeof (NSViewController))]
	interface MEExtensionViewController
	{
		[DesignatedInitializer]
		[Protected]
		[Export ("initWithNibName:bundle:")]
		IntPtr Constructor ([NullAllowed] string nibNameOrNull, [NullAllowed] NSBundle nibBundleOrNull);
	}

	[NoWatch, NoTV, NoMacCatalyst, NoiOS, Mac (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MEExtensionManager
	{
		[Static]
		[Export ("reloadContentBlockerWithIdentifier:completionHandler:")]
		void ReloadContentBlocker (string identifier, [NullAllowed] Action<NSError> completionHandler);
	}
}
