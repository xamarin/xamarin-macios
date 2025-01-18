//
// WatchConnectivity bindings
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using System.ComponentModel;
using ObjCRuntime;
using Foundation;

namespace WatchConnectivity {

	/// <summary>The reply handler for use with <see cref="M:WatchConnectivity.WCSessionDelegate_Extensions.DidReceiveMessageData(WatchConnectivity.IWCSessionDelegate,WatchConnectivity.WCSession,Foundation.NSData,WatchConnectivity.WCSessionReplyDataHandler)" />.</summary>
	delegate void WCSessionReplyHandler (NSDictionary<NSString, NSObject> replyMessage);
	/// <summary>The reply handler for use with <format type="text/html"><a href="https://docs.microsoft.com/en-us/search/index?search=Watch%20Connectivity%20WCSession%20Delegate%20Did%20Receive%20Message%20Data&amp;scope=Xamarin" title="M:WatchConnectivity.WCSessionDelegate.DidReceiveMessageData*">M:WatchConnectivity.WCSessionDelegate.DidReceiveMessageData*</a></format>.</summary>
	delegate void WCSessionReplyDataHandler (NSData replyMessage);

	/// <summary>Mediates the transfer of information between a WatchKit extension app and the container app on the device.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/WatchConnectivity/Reference/WCSession_class/index.html">Apple documentation for <c>WCSession</c></related>
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface WCSession {

		[Static]
		[Export ("isSupported")]
		bool IsSupported { get; }

		[Static]
		[Export ("defaultSession")]
		WCSession DefaultSession { get; }

		[Export ("delegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		IWCSessionDelegate Delegate { get; set; }

		[Export ("activateSession")]
		void ActivateSession ();

		[Export ("paired")]
		bool Paired { [Bind ("isPaired")] get; }

		[Export ("watchAppInstalled")]
		bool WatchAppInstalled { [Bind ("isWatchAppInstalled")] get; }

		[Export ("complicationEnabled")]
		bool ComplicationEnabled { [Bind ("isComplicationEnabled")] get; }

		[Export ("watchDirectoryURL")]
		[NullAllowed]
		NSUrl WatchDirectoryUrl { get; }

		[Export ("reachable")]
		bool Reachable { [Bind ("isReachable")] get; }

		[NoiOS]
		[Export ("iOSDeviceNeedsUnlockAfterRebootForReachability")]
		bool iOSDeviceNeedsUnlockAfterRebootForReachability { get; }

		[Export ("sendMessage:replyHandler:errorHandler:")]
		void SendMessage (NSDictionary<NSString, NSObject> message, [NullAllowed] WCSessionReplyHandler replyHandler, [NullAllowed] Action<NSError> errorHandler);

		[Export ("sendMessageData:replyHandler:errorHandler:")]
		void SendMessage (NSData data, [NullAllowed] WCSessionReplyDataHandler replyHandler, [NullAllowed] Action<NSError> errorHandler);

		[Export ("applicationContext", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> ApplicationContext { get; }

		[Export ("updateApplicationContext:error:")]
		bool UpdateApplicationContext (NSDictionary<NSString, NSObject> applicationContext, out NSError error);

		[Export ("receivedApplicationContext", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> ReceivedApplicationContext { get; }

		[Export ("transferUserInfo:")]
		WCSessionUserInfoTransfer TransferUserInfo (NSDictionary<NSString, NSObject> userInfo);

		[Export ("transferCurrentComplicationUserInfo:")]
		WCSessionUserInfoTransfer TransferCurrentComplicationUserInfo (NSDictionary<NSString, NSObject> userInfo);

		[Export ("outstandingUserInfoTransfers", ArgumentSemantic.Copy)]
		WCSessionUserInfoTransfer [] OutstandingUserInfoTransfers { get; }

		[Export ("transferFile:metadata:")]
		WCSessionFileTransfer TransferFile (NSUrl file, [NullAllowed] NSDictionary<NSString, NSObject> metadata);

		[Export ("outstandingFileTransfers", ArgumentSemantic.Copy)]
		WCSessionFileTransfer [] OutstandingFileTransfers { get; }

		[Field ("WCErrorDomain")]
		NSString ErrorDomain { get; }

		[Export ("activationState")]
		WCSessionActivationState ActivationState { get; }

		[Export ("hasContentPending")]
		bool HasContentPending { get; }

		[Export ("remainingComplicationUserInfoTransfers")]
		nuint RemainingComplicationUserInfoTransfers { get; }

		[NoiOS]
		[Export ("companionAppInstalled")]
		bool CompanionAppInstalled { [Bind ("isCompanionAppInstalled")] get; }
	}

	/// <summary>Interface representing the required methods (if any) of the protocol <see cref="T:WatchConnectivity.WCSessionDelegate" />.</summary>
	///     <remarks>
	///       <para>This interface contains the required methods (if any) from the protocol defined by <see cref="T:WatchConnectivity.WCSessionDelegate" />.</para>
	///       <para>If developers create classes that implement this interface, the implementation methods will automatically be exported to Objective-C with the matching signature from the method defined in the <see cref="T:WatchConnectivity.WCSessionDelegate" /> protocol.</para>
	///       <para>Optional methods (if any) are provided by the <see cref="T:WatchConnectivity.WCSessionDelegate_Extensions" /> class as extension methods to the interface, allowing developers to invoke any optional methods on the protocol.</para>
	///     </remarks>
	interface IWCSessionDelegate { }

	/// <summary>Delegate object whose methods, when overridden, allow the app developer to respond to messages sent between a WatchKit extension app and it's container app.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/WatchConnectivity/Reference/WCSessionDelegate_protocol/index.html">Apple documentation for <c>WCSessionDelegate</c></related>
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface WCSessionDelegate {
		[Export ("sessionWatchStateDidChange:")]
		void SessionWatchStateDidChange (WCSession session);

		[Export ("sessionReachabilityDidChange:")]
		void SessionReachabilityDidChange (WCSession session);

		[Export ("session:didReceiveMessage:")]
		void DidReceiveMessage (WCSession session, NSDictionary<NSString, NSObject> message);

		[Export ("session:didReceiveMessage:replyHandler:")]
		void DidReceiveMessage (WCSession session, NSDictionary<NSString, NSObject> message, WCSessionReplyHandler replyHandler);

		[Export ("session:didReceiveMessageData:")]
		void DidReceiveMessageData (WCSession session, NSData messageData);

		[Export ("session:didReceiveMessageData:replyHandler:")]
		void DidReceiveMessageData (WCSession session, NSData messageData, WCSessionReplyDataHandler replyHandler);

		[Export ("session:didReceiveApplicationContext:")]
		void DidReceiveApplicationContext (WCSession session, NSDictionary<NSString, NSObject> applicationContext);

		[Export ("session:didFinishUserInfoTransfer:error:")]
		void DidFinishUserInfoTransfer (WCSession session, WCSessionUserInfoTransfer userInfoTransfer, [NullAllowed] NSError error);

		[Export ("session:didReceiveUserInfo:")]
		void DidReceiveUserInfo (WCSession session, NSDictionary<NSString, NSObject> userInfo);

		[Export ("session:didFinishFileTransfer:error:")]
		void DidFinishFileTransfer (WCSession session, WCSessionFileTransfer fileTransfer, [NullAllowed] NSError error);

		[Export ("session:didReceiveFile:")]
		void DidReceiveFile (WCSession session, WCSessionFile file);

#if NET
		[Abstract] // OS 10 beta 1 SDK made this required
#endif
		[Export ("session:activationDidCompleteWithState:error:")]
		void ActivationDidComplete (WCSession session, WCSessionActivationState activationState, [NullAllowed] NSError error);

#if NET
		[Abstract] // OS 10 beta 1 SDK made this required
#endif
		[Export ("sessionDidBecomeInactive:")]
		void DidBecomeInactive (WCSession session);

#if NET
		[Abstract] // OS 10 beta 1 SDK made this required
#endif
		[Export ("sessionDidDeactivate:")]
		void DidDeactivate (WCSession session);

		[NoiOS]
		[Export ("sessionCompanionAppInstalledDidChange:")]
		void CompanionAppInstalledDidChange (WCSession session);
	}

	/// <summary>Holds data relating to a file being transferred between a WatchKit extension app and it's container app.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/WatchConnectivity/Reference/WCSessionFile_class/index.html">Apple documentation for <c>WCSessionFile</c></related>
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // no handle, doc: You do not create instances of this class directly.
	interface WCSessionFile {

		[Export ("fileURL")]
		NSUrl FileUrl { get; }

		[NullAllowed]
		[Export ("metadata", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> Metadata { get; }
	}

	/// <summary>Represents an ongoing file transfer between a WatchKit extension app and it's container app.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/WatchConnectivity/Reference/WCSessionFileTransfer_class/index.html">Apple documentation for <c>WCSessionFileTransfer</c></related>
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // no handle, doc: You do not create instances of this class yourself.
	interface WCSessionFileTransfer {

		[Export ("file")]
		WCSessionFile File { get; }

		[Export ("transferring")]
		bool Transferring { [Bind ("isTransferring")] get; }

		[Export ("cancel")]
		void Cancel ();

		[Export ("progress")]
		NSProgress Progress { get; }
	}

	/// <summary>Represents an ongoing data transfer between a WatchKit extension app and it's container app.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/WatchConnectivity/Reference/WCSessionUserInfoTransfer_class/index.html">Apple documentation for <c>WCSessionUserInfoTransfer</c></related>
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // no handle, doc: You do not create instances of this class yourself.
	interface WCSessionUserInfoTransfer : NSSecureCoding {

		[Export ("currentComplicationInfo")]
		bool CurrentComplicationInfo { [Bind ("isCurrentComplicationInfo")] get; }

		[Export ("userInfo", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> UserInfo { get; }

		[Export ("transferring")]
		bool Transferring { [Bind ("isTransferring")] get; }

		[Export ("cancel")]
		void Cancel ();
	}

}
