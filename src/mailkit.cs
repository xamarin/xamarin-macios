using System;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using AVFoundation;

namespace MailKit {
	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[Native, Advice ("'MEComposeSessionErrorCode' is not available in UIKit on macOS.")]
	[ErrorDomain ("MEComposeSessionErrorDomain")]
	public enum MEComposeSessionErrorCode : long
	{
		Recipients = 0,
		Headers = 1,
		Body = 2,
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[Native, Advice ("'MEMessageActionMessageColor' is not available in UIKit on macOS.")]
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
	[Native, Advice ("'MEMessageState' is not available in UIKit on macOS.")]
	public enum MEMessageState : long
	{
		Received = 0,
		Draft = 1,
		Sending = 2,
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12, 0)]
	[Field ("MEComposeSessionErrorDomain"), Advice ("'MEComposeSessionErrorDomain' is not available in UIKit on macOS.")]
	NSString MEComposeSessionErrorDomain { get; }

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[Advice ("'MEMessage' is not available in UIKit on macOS.")]
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
		NSDate DateSent { get; }

		[Export ("dateReceived", ArgumentSemantic.Copy)]
		NSDate DateReceived { get; }

		[NullAllowed, Export ("headers", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSArray<NSString>> Headers { get; }

		[NullAllowed, Export ("rawData", ArgumentSemantic.Copy)]
		NSData RawData { get; }
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[Advice ("'MEMessageAction' is not available in UIKit on macOS.")]
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

		[Static]
		[Export ("flagAction")]
		MEMessageAction Flag { get; }

		[Static]
		[Export ("unflagAction")]
		MEMessageAction Unflag { get; }

		[Static]
		[Export ("setColorActionWithColor:")]
		MEMessageAction SetColor (MEMessageActionMessageColor color);
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[Advice ("'MEMessageActionDecision' is not available in UIKit on macOS.")]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MEMessageActionDecision : NSSecureCoding
	{
		[Static]
		[Export ("invokeAgainWithBody")]
		MEMessageActionDecision InvokeAgainWithBody { get; }

		[Static]
		[Export ("applyAction:")]
		MEMessageActionDecision Apply (MEMessageAction action);
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[Advice ("'MEMessageEncodingResult' is not available in UIKit on macOS.")]
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
	[Advice ("'MEMessageSecurityInformation' is not available in UIKit on macOS.")]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MEMessageSecurityInformation : NSSecureCoding
	{
		[Export ("initWithSigners:isEncrypted:signingError:encryptionError:")]
		IntPtr Constructor (MEMessageSigner[] signers, bool isEncrypted, [NullAllowed] NSError signingError, [NullAllowed] NSError encryptionError);

		[Export ("signers", ArgumentSemantic.Strong)]
		MEMessageSigner[] Signers { get; }

		[Export ("isEncrypted")]
		bool IsEncrypted { get; }

		[NullAllowed, Export ("signingError", ArgumentSemantic.Strong)]
		NSError SigningError { get; }

		[NullAllowed, Export ("encryptionError", ArgumentSemantic.Strong)]
		NSError EncryptionError { get; }
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[Advice ("'MEMessageSigner' is not available in UIKit on macOS.")]
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
	[Advice ("'MEOutgoingMessageEncodingStatus' is not available in UIKit on macOS.")]
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
	[Advice ("'MEAddressAnnotation' is not available in UIKit on macOS.")]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MEAddressAnnotation : NSSecureCoding
	{
		[Static]
		[Export ("errorWithLocalizedDescription:")]
		MEAddressAnnotation AnnotateError (string localizedDescription);

		[Static]
		[Export ("warningWithLocalizedDescription:")]
		MEAddressAnnotation AnnotateWarning (string localizedDescription);

		[Static]
		[Export ("successWithLocalizedDescription:")]
		MEAddressAnnotation AnnotateSuccess (string localizedDescription);
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[Advice ("'MEComposeSession' is not available in UIKit on macOS.")]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MEComposeSession : NSSecureCoding
	{
		[Export ("sessionID", ArgumentSemantic.Strong)]
		NSUuid SessionID { get; }

		[Export ("mailMessage", ArgumentSemantic.Strong)]
		MEMessage MailMessage { get; }

		[Export ("reloadSession")]
		void ReloadSession ();
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[Advice ("'MEDecodedMessage' is not available in UIKit on macOS.")]
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
	[Advice ("'MEEncodedOutgoingMessage' is not available in UIKit on macOS.")]
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

	delegate void SessionHandler (NSDictionary<MEEmailAddress, MEAddressAnnotation> sessionDictionary);

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[Protocol, Advice ("'MEComposeSessionHandler' is not available in UIKit on macOS.")]
	[BaseType (typeof (NSObject))]
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

		[Async]
		[Export ("session:annotateAddressesWithCompletionHandler:")]
		void AnnotateAddress (MEComposeSession session, SessionHandler completionHandler);

		[Async]
		[Export ("session:canSendMessageWithCompletionHandler:")]
		void AllowMessageSend (MEComposeSession session, Action<NSError> completionHandler);

		[Export ("additionalHeadersForSession:")]
		NSDictionary<NSString, NSArray<NSString>> GetAdditionalHeaders (MEComposeSession session);
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[Protocol, Advice ("'MEContentBlocker' is not available in UIKit on macOS.")]
	[BaseType (typeof (NSObject))]
	interface MEContentBlocker
	{
		[Abstract]
		[Export ("contentRulesJSON")]
		NSData ContentRulesJson { get; }
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[Protocol, Advice ("'MEExtension' is not available in UIKit on macOS.")]
	[BaseType (typeof (NSObject))]
	interface MEExtension
	{
		[Export ("handlerForComposeSession:")]
		MEComposeSessionHandler HandlerForComposeSession (MEComposeSession session);

		[Export ("handlerForMessageActions")]
		MEMessageActionHandler HandlerForMessageActions { get; }

		[Export ("handlerForContentBlocker")]
		MEContentBlocker HandlerForContentBlocker { get; }

		[Export ("handlerForMessageSecurity")]
		MEMessageSecurityHandler HandlerForMessageSecurity { get; }
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[Protocol, Advice ("'MEMessageActionHandler' is not available in UIKit on macOS.")]
	[BaseType (typeof (NSObject))]
	interface MEMessageActionHandler
	{
		[Abstract]
		[Export ("decideActionForMessage:completionHandler:")]
		void CompletionHandler (MEMessage message, Action<MEMessageActionDecision> completionHandler);

		[Export ("requiredHeaders", ArgumentSemantic.Copy)]
		string[] RequiredHeaders { get; }
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[Protocol, Advice ("'MEMessageDecoder' is not available in UIKit on macOS.")]
	[BaseType (typeof (NSObject))]
	interface MEMessageDecoder
	{
		[Abstract]
		[Export ("decodedMessageForMessageData:")]
		[return: NullAllowed]
		MEDecodedMessage DecodedMessage (NSData data);
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[Protocol, Advice ("'MEMessageEncoder' is not available in UIKit on macOS.")]
	[BaseType (typeof (NSObject))]
	interface MEMessageEncoder
	{
		[Abstract]
		[Export ("getEncodingStatusForMessage:completionHandler:")]
		void GetEncodingStatus (MEMessage message, Action<MEOutgoingMessageEncodingStatus> completionHandler);

		[Abstract]
		[Export ("encodeMessage:shouldSign:shouldEncrypt:completionHandler:")]
		void EncodeMessage (MEMessage message, bool shouldSign, bool shouldEncrypt, Action<MEMessageEncodingResult> completionHandler);
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12,0)]
	[Protocol, Advice ("'MEMessageSecurityHandler' is not available in UIKit on macOS.")]
	interface MEMessageSecurityHandler : MEMessageEncoder, MEMessageDecoder
	{
		[Abstract]
		[Export ("extensionViewControllerForMessageSigners:")]
		[return: NullAllowed]
		MEExtensionViewController ExtensionViewController (MEMessageSigner[] messageSigners);
	}

}