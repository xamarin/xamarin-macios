//
// multipeerconnectivity.cs: binding for iOS (7+) MultipeerConnectivity framework
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2013 Xamarin, Inc.
#if XAMCORE_2_0 || !MONOMAC // MultipeerConnectivity is 64-bit only on OS X
using System;

using XamCore.Foundation;
using XamCore.ObjCRuntime;
using XamCore.Security;
#if MONOMAC
using XamCore.AppKit;
#else
using XamCore.UIKit;
#endif

namespace XamCore.MultipeerConnectivity {

	[Since (7,0)][Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: -[MCPeerID init]: unrecognized selector sent to instance 0x7d721090
	public partial interface MCPeerID : NSCopying, NSSecureCoding {

		[DesignatedInitializer]
		[Export ("initWithDisplayName:")]
		IntPtr Constructor (string myDisplayName);

		[Export ("displayName")]
		string DisplayName { get; }
	}

	public delegate void MCSessionNearbyConnectionDataForPeerCompletionHandler (NSData connectionData, NSError error);

	[Since (7,0)][Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // crash when calling `description` selector
	public partial interface MCSession {

		[Export ("initWithPeer:")]
		IntPtr Constructor (MCPeerID myPeerID);

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
		[Export ("sendResourceAtURL:withName:toPeer:withCompletionHandler:")]
		NSProgress SendResource (NSUrl resourceUrl, string resourceName, MCPeerID peerID, [NullAllowed] Action<NSError> completionHandler);

		[Export ("startStreamWithName:toPeer:error:")]
		NSOutputStream StartStream (string streamName, MCPeerID peerID, out NSError error);

		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		MCSessionDelegate Delegate { get; set; }

		[Export ("myPeerID")]
		MCPeerID MyPeerID { get; }

		// we use NSArray because, when non-null, it contains a SecIdentity followed by 0..n SecCertificate - none are NSObject
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

	[Since (7,0)][Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	public partial interface MCSessionDelegate {
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
		void DidFinishReceivingResource (MCSession session, string resourceName, MCPeerID fromPeer, NSUrl localUrl, NSError error);

		[Abstract]
		[Export ("session:didReceiveStream:withName:fromPeer:")]
		void DidReceiveStream (MCSession session, NSInputStream stream, string streamName, MCPeerID peerID);

		[Export ("session:didReceiveCertificate:fromPeer:certificateHandler:")]
		bool DidReceiveCertificate (MCSession session, [NullAllowed] SecCertificate[] certificate, MCPeerID peerID, Action<bool> certificateHandler);
	}

	[Since (7,0)][Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSInvalidArgumentException -[MCNearbyServiceAdvertiser init]: unrecognized selector sent to instance 0x19195e50
	public partial interface MCNearbyServiceAdvertiser {

		[DesignatedInitializer]
		[Export ("initWithPeer:discoveryInfo:serviceType:")]
		IntPtr Constructor (MCPeerID myPeerID, [NullAllowed] NSDictionary info, string serviceType);

		[Export ("startAdvertisingPeer")]
		void StartAdvertisingPeer ();

		[Export ("stopAdvertisingPeer")]
		void StopAdvertisingPeer ();

		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		MCNearbyServiceAdvertiserDelegate Delegate { get; set; }

		[Export ("myPeerID")]
		MCPeerID MyPeerID { get; }

		[Export ("discoveryInfo")]
		NSDictionary DiscoveryInfo { get; }

		[Export ("serviceType")]
		string ServiceType { get; }
	}

	public delegate void MCNearbyServiceAdvertiserInvitationHandler (bool accept, MCSession session);

	[Since (7,0)][Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	public partial interface MCNearbyServiceAdvertiserDelegate {

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("advertiser:didReceiveInvitationFromPeer:withContext:invitationHandler:")]
		void DidReceiveInvitationFromPeer (MCNearbyServiceAdvertiser advertiser, MCPeerID peerID, [NullAllowed] NSData context, MCNearbyServiceAdvertiserInvitationHandler invitationHandler);

		[Export ("advertiser:didNotStartAdvertisingPeer:")]
		void DidNotStartAdvertisingPeer (MCNearbyServiceAdvertiser advertiser, NSError error);
	}

	[Since (7,0)][Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSInvalidArgumentException -[MCNearbyServiceBrowser init]: unrecognized selector sent to instance 0x15519a70
	public partial interface MCNearbyServiceBrowser {

		[DesignatedInitializer]
		[Export ("initWithPeer:serviceType:")]
		IntPtr Constructor (MCPeerID myPeerID, string serviceType);

		[Export ("startBrowsingForPeers")]
		void StartBrowsingForPeers ();

		[Export ("stopBrowsingForPeers")]
		void StopBrowsingForPeers ();

		[Export ("invitePeer:toSession:withContext:timeout:")]
		void InvitePeer (MCPeerID peerID, MCSession session, [NullAllowed] NSData context, double timeout);

		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		MCNearbyServiceBrowserDelegate Delegate { get; set; }

		[Export ("myPeerID")]
		MCPeerID MyPeerID { get; }

		[Export ("serviceType")]
		string ServiceType { get; }
	}

	[Since (7,0)][Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	public partial interface MCNearbyServiceBrowserDelegate {

#if XAMCORE_2_0
		[Abstract]
#endif
		[Mac (10,9)]
		[Export ("browser:foundPeer:withDiscoveryInfo:")]
		void FoundPeer (MCNearbyServiceBrowser browser, MCPeerID peerID, [NullAllowed] NSDictionary info);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Mac (10,9)]
		[Export ("browser:lostPeer:")]
		void LostPeer (MCNearbyServiceBrowser browser, MCPeerID peerID);

		[Mac (10,9)]
		[Export ("browser:didNotStartBrowsingForPeers:")]
		void DidNotStartBrowsingForPeers (MCNearbyServiceBrowser browser, NSError error);
	}

	public interface IMCNearbyServiceBrowserDelegate {}

#if MONOMAC
	[Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (NSViewController))]
#else
	[Since (7,0)]
	[BaseType (typeof (UIViewController))]
#endif
	[DisableDefaultCtor] // NSInvalidArgumentException -[MCPeerPickerViewController initWithNibName:bundle:]: unrecognized selector sent to instance 0x15517b90
	public partial interface MCBrowserViewController : MCNearbyServiceBrowserDelegate {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[DesignatedInitializer]
		[Export ("initWithBrowser:session:")]
		IntPtr Constructor (MCNearbyServiceBrowser browser, MCSession session);

		[Export ("initWithServiceType:session:")]
		IntPtr Constructor (string serviceType, MCSession session);

		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		MCBrowserViewControllerDelegate Delegate { get; set; }

		[Export ("minimumNumberOfPeers", ArgumentSemantic.Assign)]
		nuint MinimumNumberOfPeers { get; set; }

		[Export ("maximumNumberOfPeers", ArgumentSemantic.Assign)]
		nuint MaximumNumberOfPeers { get; set; }

		[Export ("browser")]
		MCNearbyServiceBrowser Browser { get; }

		[Export ("session")]
		MCSession Session { get; }
	}

	[Since (7,0)][Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	public partial interface MCBrowserViewControllerDelegate {

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("browserViewControllerWasCancelled:")]
		void WasCancelled (MCBrowserViewController browserViewController);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("browserViewControllerDidFinish:")]
		void DidFinish (MCBrowserViewController browserViewController);

		// optional

		[Export ("browserViewController:shouldPresentNearbyPeer:withDiscoveryInfo:")]
		bool ShouldPresentNearbyPeer (MCBrowserViewController browserViewController, MCPeerID peerID, [NullAllowed] NSDictionary info);
	}

	[Since (7,0)][Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: -[MCAdvertiserAssistant init]: unrecognized selector sent to instance 0x7ea7fa40
	interface MCAdvertiserAssistant {

		[DesignatedInitializer]
		[Export ("initWithServiceType:discoveryInfo:session:")]
		IntPtr Constructor (string serviceType, [NullAllowed] NSDictionary info, MCSession session);

		[Export ("discoveryInfo")]
		NSDictionary DiscoveryInfo { get; }

		[Export ("session")]
		MCSession Session { get; }

		[Export ("serviceType")]
		string ServiceType { get; }

		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		MCAdvertiserAssistantDelegate Delegate { get; set; }

		[Export ("start")]
		void Start ();

		[Export ("stop")]
		void Stop ();
	}

	[Since (7,0)][Mac (10,10, onlyOn64 : true)]
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
#endif
