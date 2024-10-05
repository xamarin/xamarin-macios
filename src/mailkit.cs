using System;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using AVFoundation;
using AppKit;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace MailKit {
	[NoWatch, NoTV, NoiOS, NoMacCatalyst]
	[Native]
	[ErrorDomain ("MEComposeSessionErrorDomain")]
	public enum MEComposeSessionErrorCode : long {
		Recipients = 0,
		Headers = 1,
		Body = 2,
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst]
	[Native]
	public enum MEMessageActionMessageColor : long {
		None,
		Green,
		Yellow,
		Orange,
		Red,
		Purple,
		Blue,
		Gray,
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst]
	[Native]
	public enum MEMessageState : long {
		Received = 0,
		Draft = 1,
		Sending = 2,
	}

	[NoWatch, NoTV, NoMacCatalyst, NoiOS]
	[Native]
	public enum MEMessageEncryptionState : long {
		Unknown = 0,
		NotEncrypted = 1,
		Encrypted = 2,
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MEMessage : NSSecureCoding {
		[Export ("state", ArgumentSemantic.Assign)]
		MEMessageState State { get; }

		[Export ("encryptionState", ArgumentSemantic.Assign)]
		MEMessageEncryptionState EncryptionState { get; }

		[Export ("subject")]
		string Subject { get; }

		[Export ("fromAddress", ArgumentSemantic.Copy)]
		MEEmailAddress FromAddress { get; }

		[Export ("toAddresses", ArgumentSemantic.Copy)]
		MEEmailAddress [] ToAddresses { get; }

		[Export ("ccAddresses", ArgumentSemantic.Copy)]
		MEEmailAddress [] CcAddresses { get; }

		[Export ("bccAddresses", ArgumentSemantic.Copy)]
		MEEmailAddress [] BccAddresses { get; }

		[Export ("replyToAddresses", ArgumentSemantic.Copy)]
		MEEmailAddress [] ReplyToAddresses { get; }

		[Export ("allRecipientAddresses", ArgumentSemantic.Copy)]
		MEEmailAddress [] AllRecipientAddresses { get; }

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

	[NoWatch, NoTV, NoMacCatalyst, NoiOS]
	[Native]
	public enum MEMessageActionFlag : long {
		None,
		DefaultColor,
		Red,
		Orange,
		Yellow,
		Green,
		Blue,
		Purple,
		Gray,
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MEMessageAction : NSSecureCoding {
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
		[Export ("flagActionWithFlag:")]
		MEMessageAction SetFlagAction (MEMessageActionFlag flag);

		[Static]
		[Export ("setBackgroundColorActionWithColor:")]
		MEMessageAction SetBackgroundColorAction (MEMessageActionMessageColor color);
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MEMessageActionDecision : NSSecureCoding {
		[Static]
		[Export ("invokeAgainWithBody")]
		MEMessageActionDecision InvokeAgainWithBody { get; }

		[Static]
		[Export ("decisionApplyingAction:")]
		MEMessageActionDecision Apply (MEMessageAction action);

		[Static]
		[Export ("decisionApplyingActions:")]
		MEMessageActionDecision Apply (MEMessageAction [] actions);
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MEMessageEncodingResult : NSSecureCoding {
		[Export ("initWithEncodedMessage:signingError:encryptionError:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] MEEncodedOutgoingMessage encodedMessage, [NullAllowed] NSError signingError, [NullAllowed] NSError encryptionError);

		[NullAllowed, Export ("encodedMessage", ArgumentSemantic.Copy)]
		MEEncodedOutgoingMessage EncodedMessage { get; }

		[NullAllowed, Export ("signingError", ArgumentSemantic.Copy)]
		NSError SigningError { get; }

		[NullAllowed, Export ("encryptionError", ArgumentSemantic.Copy)]
		NSError EncryptionError { get; }
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MEMessageSecurityInformation : NSSecureCoding {
		[Export ("initWithSigners:isEncrypted:signingError:encryptionError:")]
		NativeHandle Constructor (MEMessageSigner [] signers, bool isEncrypted, [NullAllowed] NSError signingError, [NullAllowed] NSError encryptionError);

		[Export ("initWithSigners:isEncrypted:signingError:encryptionError:shouldBlockRemoteContent:localizedRemoteContentBlockingReason:")]
		NativeHandle Constructor (MEMessageSigner [] signers, bool isEncrypted, [NullAllowed] NSError signingError, [NullAllowed] NSError encryptionError, bool shouldBlockRemoteContent, [NullAllowed] string localizedRemoteContentBlockingReason);

		[Export ("signers", ArgumentSemantic.Strong)]
		MEMessageSigner [] Signers { get; }

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

	[NoWatch, NoTV, NoiOS, NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MEMessageSigner : NSSecureCoding {
		[Export ("initWithEmailAddresses:signatureLabel:context:")]
		NativeHandle Constructor (MEEmailAddress [] emailAddresses, string label, [NullAllowed] NSData context);

		[Export ("emailAddresses", ArgumentSemantic.Copy)]
		MEEmailAddress [] EmailAddresses { get; }

		[Export ("label")]
		string Label { get; }

		[Export ("context")]
		NSData Context { get; }
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MEOutgoingMessageEncodingStatus : NSSecureCoding {
		[Export ("initWithCanSign:canEncrypt:securityError:addressesFailingEncryption:")]
		NativeHandle Constructor (bool canSign, bool canEncrypt, [NullAllowed] NSError securityError, MEEmailAddress [] addressesFailingEncryption);

		[Export ("canSign")]
		bool CanSign { get; }

		[Export ("canEncrypt")]
		bool CanEncrypt { get; }

		[NullAllowed, Export ("securityError", ArgumentSemantic.Copy)]
		NSError SecurityError { get; }

		[Export ("addressesFailingEncryption", ArgumentSemantic.Copy)]
		MEEmailAddress [] AddressesFailingEncryption { get; }
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MEAddressAnnotation : NSSecureCoding {
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

	[NoWatch, NoTV, NoiOS, NoMacCatalyst]
	[Advice ("'MEComposeSession' is not available in UIKit on macOS.")]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MEComposeSession : NSSecureCoding {
		[Export ("sessionID", ArgumentSemantic.Strong)]
		NSUuid SessionId { get; }

		[Export ("mailMessage", ArgumentSemantic.Strong)]
		MEMessage MailMessage { get; }

		[Export ("composeContext", ArgumentSemantic.Strong)]
		MEComposeContext ComposeContext { get; }

		[Export ("reloadSession")]
		void ReloadSession ();
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MEDecodedMessage : NSSecureCoding {
		[NullAllowed, Export ("rawData", ArgumentSemantic.Copy)]
		NSData RawData { get; }

		[Export ("securityInformation", ArgumentSemantic.Strong)]
		MEMessageSecurityInformation SecurityInformation { get; }

		[NullAllowed, Export ("context")]
		NSData Context { get; }

		[NullAllowed, Export ("banner")]
		MEDecodedMessageBanner Banner { get; }

		[Export ("initWithData:securityInformation:context:")]
		NativeHandle Constructor ([NullAllowed] NSData rawData, MEMessageSecurityInformation securityInformation, [NullAllowed] NSData context);

		[Export ("initWithData:securityInformation:context:banner:")]
		NativeHandle Constructor ([NullAllowed] NSData rawData, MEMessageSecurityInformation securityInformation, [NullAllowed] NSData context, [NullAllowed] MEDecodedMessageBanner banner);
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	interface MEEncodedOutgoingMessage : NSSecureCoding {
		[Export ("initWithRawData:isSigned:isEncrypted:")]
		NativeHandle Constructor (NSData rawData, bool isSigned, bool isEncrypted);

		[Export ("rawData", ArgumentSemantic.Copy)]
		NSData RawData { get; }

		[Export ("isSigned")]
		bool IsSigned { get; }

		[Export ("isEncrypted")]
		bool IsEncrypted { get; }
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MEEmailAddress : NSSecureCoding, NSCopying {
		[Export ("initWithRawString:")]
		NativeHandle Constructor (string rawString);

		[Export ("rawString")]
		string RawString { get; }

		[NullAllowed, Export ("addressString")]
		string AddressString { get; }
	}

	interface IMEComposeSessionHandler { }

	[NoWatch, NoTV, NoiOS, NoMacCatalyst]
	[Protocol]
	interface MEComposeSessionHandler {
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

	interface IMEContentBlocker { }

	[NoWatch, NoTV, NoiOS, NoMacCatalyst]
	[Protocol]
	interface MEContentBlocker {
		[Abstract]
		[Export ("contentRulesJSON")]
		NSData ContentRulesJson { get; }
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst]
	[Protocol]
	interface MEExtension {
		[Export ("handlerForComposeSession:")]
		IMEComposeSessionHandler GetHandlerForComposeSession (MEComposeSession session);

		[Export ("handlerForMessageActions")]
		IMEMessageActionHandler HandlerForMessageActions { get; }

		[Export ("handlerForContentBlocker")]
		IMEContentBlocker HandlerForContentBlocker { get; }

		[Export ("handlerForMessageSecurity")]
		IMEMessageSecurityHandler HandlerForMessageSecurity { get; }
	}

	interface IMEMessageActionHandler { }

	[NoWatch, NoTV, NoiOS, NoMacCatalyst]
	[Protocol]
	interface MEMessageActionHandler {
		[Abstract]
		[Export ("decideActionForMessage:completionHandler:")]
		void DecideAction (MEMessage message, Action<MEMessageActionDecision> completionHandler);

		[Export ("requiredHeaders", ArgumentSemantic.Copy)]
		string [] RequiredHeaders { get; }
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst]
	[Protocol]
	interface MEMessageDecoder {
		[Abstract]
		[Export ("decodedMessageForMessageData:")]
		[return: NullAllowed]
		MEDecodedMessage DecodedMessage (NSData data);
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst]
	[Protocol]
	interface MEMessageEncoder {
		[Abstract]
		[Export ("getEncodingStatusForMessage:composeContext:completionHandler:")]
		void GetEncodingStatus (MEMessage message, MEComposeContext composeContext, Action<MEOutgoingMessageEncodingStatus> completionHandler);

		[Abstract]
		[Export ("encodeMessage:composeContext:completionHandler:")]
		void EncodeMessage (MEMessage message, MEComposeContext composeContext, Action<MEMessageEncodingResult> completionHandler);
	}

	[ErrorDomain ("MEMessageSecurityErrorDomain")]
	[NoWatch, NoTV, NoMacCatalyst, NoiOS]
	[Native]
	public enum MEMessageSecurityErrorCode : long {
		EncodingError = 0,
		DecodingError = 1,
	}

	interface IMEMessageSecurityHandler { }

	[NoWatch, NoTV, NoiOS, NoMacCatalyst]
	[Protocol]
	interface MEMessageSecurityHandler : MEMessageEncoder, MEMessageDecoder {
		[Abstract]
		[Export ("extensionViewControllerForMessageSigners:")]
		[return: NullAllowed]
		MEExtensionViewController GetExtensionViewController (MEMessageSigner [] messageSigners);

		[Abstract]
		[Export ("extensionViewControllerForMessageContext:")]
		[return: NullAllowed]
		MEExtensionViewController GetExtensionViewController (NSData messageContext);

		[Abstract]
		[Export ("primaryActionClickedForMessageContext:completionHandler:")]
		void SetPrimaryActionClicked (NSData messageContext, Action<MEExtensionViewController> completionHandler);
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst]
	[DefaultCtorVisibility (Visibility.Protected)]
	[BaseType (typeof (NSViewController))]
	interface MEExtensionViewController {
		[DesignatedInitializer]
		[Protected]
		[Export ("initWithNibName:bundle:")]
		NativeHandle Constructor ([NullAllowed] string nibNameOrNull, [NullAllowed] NSBundle nibBundleOrNull);
	}

	[NoWatch, NoTV, NoMacCatalyst, NoiOS]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MEExtensionManager {
		[Static]
		[Export ("reloadContentBlockerWithIdentifier:completionHandler:")]
		void ReloadContentBlocker (string identifier, [NullAllowed] Action<NSError> completionHandler);

		[Static]
		[Export ("reloadVisibleMessagesWithCompletionHandler:")]
		void ReloadVisibleMessages ([NullAllowed] Action<NSError> completionHandler);
	}

	[NoWatch, NoTV, NoMacCatalyst, NoiOS]
	[Native]
	public enum MEComposeUserAction : long {
		NewMessage = 1,
		Reply = 2,
		ReplyAll = 3,
		Forward = 4,
	}

	[NoWatch, NoTV, NoMacCatalyst, NoiOS]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MEComposeContext {
		[Export ("contextID", ArgumentSemantic.Strong)]
		NSUuid ContextId { get; }

		[NullAllowed, Export ("originalMessage", ArgumentSemantic.Strong)]
		MEMessage OriginalMessage { get; }

		[Export ("action")]
		MEComposeUserAction Action { get; }

		[Export ("isEncrypted")]
		bool IsEncrypted { get; }

		[Export ("shouldEncrypt")]
		bool ShouldEncrypt { get; }

		[Export ("isSigned")]
		bool IsSigned { get; }

		[Export ("shouldSign")]
		bool ShouldSign { get; }
	}

	[NoWatch, NoTV, NoMacCatalyst, NoiOS]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MEDecodedMessageBanner : NSSecureCoding, NSCopying {
		[Export ("title", ArgumentSemantic.Strong)]
		string Title { get; }

		[Export ("primaryActionTitle", ArgumentSemantic.Strong)]
		string PrimaryActionTitle { get; }

		[Export ("dismissable")]
		bool Dismissable { [Bind ("isDismissable")] get; }

		[Export ("initWithTitle:primaryActionTitle:dismissable:")]
		IntPtr Constructor (string title, string primaryActionTitle, bool dismissable);
	}
}
