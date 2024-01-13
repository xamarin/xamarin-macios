//
// multipeerconnectivity.cs: binding for iOS (7+) MultipeerConnectivity framework
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2013 Xamarin, Inc.

using System;

using Foundation;
using ObjCRuntime;
using Security;
#if MONOMAC
using AppKit;
using UIViewController = AppKit.NSViewController;
#else
using UIKit;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace MultipeerConnectivity {

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: -[MCPeerID init]: unrecognized selector sent to instance 0x7d721090
	partial interface MCPeerID : NSCopying, NSSecureCoding {

		[DesignatedInitializer]
		[Export ("initWithDisplayName:")]
		NativeHandle Constructor (string myDisplayName);

		[Export ("displayName")]
		string DisplayName { get; }
	}

	delegate void MCSessionNearbyConnectionDataForPeerCompletionHandler (NSData connectionData, NSError error);

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // crash when calling `description` selector
	partial interface MCSession {

		[Export ("initWithPeer:")]
		NativeHandle Constructor (MCPeerID myPeerID);

		// Note: it should be a constructor but it's use of an NSArray of different types makes it hard to provide a 
		// nice binding, i.e. the first item of NSArray must be an SecIdentity followed by (0...) SecCertificate
		[Internal]
		[Export ("initWithPeer:securityIdentity:encryptionPreference:")]
		IntPtr Init (MCPeerID myPeerID, [NullAllowed] NSArray identity, MCEncryptionPreference encryptionPreference);

		[Export ("sendData:toPeers:withMode:error:")]
		bool SendData (NSData data, MCPeerID [] peerIDs, MCSessionSendDataMode mode, out NSError error);

		[Export ("disconnect")]
		void Disconnect ();

		[Export ("connectedPeers")]
		MCPeerID [] ConnectedPeers { get; }

		[Async]
		[return: NullAllowed]
		[Export ("sendResourceAtURL:withName:toPeer:withCompletionHandler:")]
		NSProgress SendResource (NSUrl resourceUrl, string resourceName, MCPeerID peerID, [NullAllowed] Action<NSError> completionHandler);

		[return: NullAllowed]
		[Export ("startStreamWithName:toPeer:error:")]
		NSOutputStream StartStream (string streamName, MCPeerID peerID, out NSError error);

		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		IMCSessionDelegate Delegate { get; set; }

		[Export ("myPeerID")]
		MCPeerID MyPeerID { get; }

		// we use NSArray because, when non-null, it contains a SecIdentity followed by 0..n SecCertificate - none are NSObject
		[NullAllowed]
		[Export ("securityIdentity")]
		NSArray SecurityIdentity { get; }

		[Export ("encryptionPreference")]
		MCEncryptionPreference EncryptionPreference { get; }

		[Field ("kMCSessionMaximumNumberOfPeers")]
		nint MaximumNumberOfPeers { get; }

		[Field ("kMCSessionMinimumNumberOfPeers")]
		nint MinimumNumberOfPeers { get; }

		#region Custom Discovery Category

		[Async]
		[Export ("nearbyConnectionDataForPeer:withCompletionHandler:")]
		void NearbyConnectionDataForPeer (MCPeerID peerID, MCSessionNearbyConnectionDataForPeerCompletionHandler completionHandler);

		[Export ("connectPeer:withNearbyConnectionData:")]
		void ConnectPeer (MCPeerID peerID, NSData data);

		[Export ("cancelConnectPeer:")]
		void CancelConnectPeer (MCPeerID peerID);

		#endregion
	}

	interface IMCSessionDelegate { }

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	partial interface MCSessionDelegate {
		[Abstract]
		[Export ("session:peer:didChangeState:")]
		void DidChangeState (MCSession session, MCPeerID peerID, MCSessionState state);

		[Abstract]
		[Export ("session:didReceiveData:fromPeer:")]
		void DidReceiveData (MCSession session, NSData data, MCPeerID peerID);

		[Abstract]
		[Export ("session:didStartReceivingResourceWithName:fromPeer:withProgress:")]
		void DidStartReceivingResource (MCSession session, string resourceName, MCPeerID fromPeer, NSProgress progress);

		[Abstract]
		[Export ("session:didFinishReceivingResourceWithName:fromPeer:atURL:withError:")]
		void DidFinishReceivingResource (MCSession session, string resourceName, MCPeerID fromPeer, [NullAllowed] NSUrl localUrl, [NullAllowed] NSError error);

		[Abstract]
		[Export ("session:didReceiveStream:withName:fromPeer:")]
		void DidReceiveStream (MCSession session, NSInputStream stream, string streamName, MCPeerID peerID);

		[Export ("session:didReceiveCertificate:fromPeer:certificateHandler:")]
		bool DidReceiveCertificate (MCSession session, [NullAllowed] SecCertificate [] certificate, MCPeerID peerID, Action<bool> certificateHandler);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSInvalidArgumentException -[MCNearbyServiceAdvertiser init]: unrecognized selector sent to instance 0x19195e50
	partial interface MCNearbyServiceAdvertiser {

		[DesignatedInitializer]
		[Export ("initWithPeer:discoveryInfo:serviceType:")]
		NativeHandle Constructor (MCPeerID myPeerID, [NullAllowed] NSDictionary info, string serviceType);

		[Export ("startAdvertisingPeer")]
		void StartAdvertisingPeer ();

		[Export ("stopAdvertisingPeer")]
		void StopAdvertisingPeer ();

		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		IMCNearbyServiceAdvertiserDelegate Delegate { get; set; }

		[Export ("myPeerID")]
		MCPeerID MyPeerID { get; }

		[NullAllowed]
		[Export ("discoveryInfo")]
		NSDictionary DiscoveryInfo { get; }

		[Export ("serviceType")]
		string ServiceType { get; }
	}

	delegate void MCNearbyServiceAdvertiserInvitationHandler (bool accept, [NullAllowed] MCSession session);

	interface IMCNearbyServiceAdvertiserDelegate { }

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	partial interface MCNearbyServiceAdvertiserDelegate {

		[Abstract]
		[Export ("advertiser:didReceiveInvitationFromPeer:withContext:invitationHandler:")]
		void DidReceiveInvitationFromPeer (MCNearbyServiceAdvertiser advertiser, MCPeerID peerID, [NullAllowed] NSData context, MCNearbyServiceAdvertiserInvitationHandler invitationHandler);

		[Export ("advertiser:didNotStartAdvertisingPeer:")]
		void DidNotStartAdvertisingPeer (MCNearbyServiceAdvertiser advertiser, NSError error);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSInvalidArgumentException -[MCNearbyServiceBrowser init]: unrecognized selector sent to instance 0x15519a70
	partial interface MCNearbyServiceBrowser {

		[DesignatedInitializer]
		[Export ("initWithPeer:serviceType:")]
		NativeHandle Constructor (MCPeerID myPeerID, string serviceType);

		[Export ("startBrowsingForPeers")]
		void StartBrowsingForPeers ();

		[Export ("stopBrowsingForPeers")]
		void StopBrowsingForPeers ();

		[Export ("invitePeer:toSession:withContext:timeout:")]
		void InvitePeer (MCPeerID peerID, MCSession session, [NullAllowed] NSData context, double timeout);

		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		IMCNearbyServiceBrowserDelegate Delegate { get; set; }

		[Export ("myPeerID")]
		MCPeerID MyPeerID { get; }

		[Export ("serviceType")]
		string ServiceType { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	partial interface MCNearbyServiceBrowserDelegate {

		[Abstract]
		[Export ("browser:foundPeer:withDiscoveryInfo:")]
		void FoundPeer (MCNearbyServiceBrowser browser, MCPeerID peerID, [NullAllowed] NSDictionary info);

		[Abstract]
		[Export ("browser:lostPeer:")]
		void LostPeer (MCNearbyServiceBrowser browser, MCPeerID peerID);

		[Export ("browser:didNotStartBrowsingForPeers:")]
		void DidNotStartBrowsingForPeers (MCNearbyServiceBrowser browser, NSError error);
	}

	interface IMCNearbyServiceBrowserDelegate { }

	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIViewController))]
	[DisableDefaultCtor] // NSInvalidArgumentException -[MCPeerPickerViewController initWithNibName:bundle:]: unrecognized selector sent to instance 0x15517b90
	partial interface MCBrowserViewController : MCNearbyServiceBrowserDelegate {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[DesignatedInitializer]
		[Export ("initWithBrowser:session:")]
		NativeHandle Constructor (MCNearbyServiceBrowser browser, MCSession session);

		[Export ("initWithServiceType:session:")]
		NativeHandle Constructor (string serviceType, MCSession session);

		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		IMCBrowserViewControllerDelegate Delegate { get; set; }

		[Export ("minimumNumberOfPeers", ArgumentSemantic.Assign)]
		nuint MinimumNumberOfPeers { get; set; }

		[Export ("maximumNumberOfPeers", ArgumentSemantic.Assign)]
		nuint MaximumNumberOfPeers { get; set; }

#if !MONOMAC
		[NullAllowed]
#endif
		[Export ("browser")]
		MCNearbyServiceBrowser Browser { get; }

		[Export ("session")]
		MCSession Session { get; }
	}

	interface IMCBrowserViewControllerDelegate { }

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	partial interface MCBrowserViewControllerDelegate {

		[Abstract]
		[Export ("browserViewControllerWasCancelled:")]
		void WasCancelled (MCBrowserViewController browserViewController);

		[Abstract]
		[Export ("browserViewControllerDidFinish:")]
		void DidFinish (MCBrowserViewController browserViewController);

		// optional

		[Export ("browserViewController:shouldPresentNearbyPeer:withDiscoveryInfo:")]
		bool ShouldPresentNearbyPeer (MCBrowserViewController browserViewController, MCPeerID peerID, [NullAllowed] NSDictionary info);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: -[MCAdvertiserAssistant init]: unrecognized selector sent to instance 0x7ea7fa40
	interface MCAdvertiserAssistant {

		[DesignatedInitializer]
		[Export ("initWithServiceType:discoveryInfo:session:")]
		NativeHandle Constructor (string serviceType, [NullAllowed] NSDictionary info, MCSession session);

		[NullAllowed]
		[Export ("discoveryInfo")]
		NSDictionary DiscoveryInfo { get; }

		[Export ("session")]
		MCSession Session { get; }

		[Export ("serviceType")]
		string ServiceType { get; }

		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		IMCAdvertiserAssistantDelegate Delegate { get; set; }

		[Export ("start")]
		void Start ();

		[Export ("stop")]
		void Stop ();
	}

	interface IMCAdvertiserAssistantDelegate { }

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface MCAdvertiserAssistantDelegate {

		[Export ("advertiserAssistantDidDismissInvitation:")]
		void DidDismissInvitation (MCAdvertiserAssistant advertiserAssistant);

		[Export ("advertiserAssistantWillPresentInvitation:")]
		void WillPresentInvitation (MCAdvertiserAssistant advertiserAssistant);
	}
}
