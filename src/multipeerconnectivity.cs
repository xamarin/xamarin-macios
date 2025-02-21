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

	/// <summary>Identifies a device in a multipeer connectivity network.</summary>
	///     <remarks>
	///       <para>
	///         <see cref="P:MultipeerConnectivity.MCPeerID.DisplayName" /> must be unique among peers.</para>
	///     </remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/MultipeerConnectivity/Reference/MCPeerID_class/index.html">Apple documentation for <c>MCPeerID</c></related>
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

	/// <summary>A delegate that serves as the completion handler for <see cref="M:MultipeerConnectivity.MCSession.NearbyConnectionDataForPeer(MultipeerConnectivity.MCPeerID,MultipeerConnectivity.MCSessionNearbyConnectionDataForPeerCompletionHandler)" />.</summary>
	delegate void MCSessionNearbyConnectionDataForPeerCompletionHandler (NSData connectionData, NSError error);

	/// <include file="../docs/api/MultipeerConnectivity/MCSession.xml" path="/Documentation/Docs[@DocId='T:MultipeerConnectivity.MCSession']/*" />
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

		/// <summary>An array of the currently connected devices.</summary>
		///         <value>The array will be non-null, but of length 0, if no peers are connected.</value>
		///         <remarks>To be added.</remarks>
		[Export ("connectedPeers")]
		MCPeerID [] ConnectedPeers { get; }

		[Async]
		[return: NullAllowed]
		[Export ("sendResourceAtURL:withName:toPeer:withCompletionHandler:")]
		NSProgress SendResource (NSUrl resourceUrl, string resourceName, MCPeerID peerID, [NullAllowed] Action<NSError> completionHandler);

		[return: NullAllowed]
		[Export ("startStreamWithName:toPeer:error:")]
		NSOutputStream StartStream (string streamName, MCPeerID peerID, out NSError error);

		/// <summary>An object that can respond to the delegate protocol for this type</summary>
		///         <value>The instance that will respond to events and data requests.</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>   Methods must be decorated with the [Export ("selectorName")] attribute to respond to each method from the protocol.   Alternatively use the Delegate method which is strongly typed and does not require the [Export] attributes on methods.</para>
		///         </remarks>
		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		/// <summary>An instance of the MultipeerConnectivity.IMCSessionDelegate model class which acts as the class delegate.</summary>
		///         <value>The instance of the MultipeerConnectivity.IMCSessionDelegate model class</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>This is the strongly typed version of the object, developers should use the WeakDelegate property instead if they want to merely assign a class derived from NSObject that has been decorated with [Export] attributes.</para>
		///         </remarks>
		[Wrap ("WeakDelegate")]
		IMCSessionDelegate Delegate { get; set; }

		/// <summary>The peer ID associated with this device.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("myPeerID")]
		MCPeerID MyPeerID { get; }

		// we use NSArray because, when non-null, it contains a SecIdentity followed by 0..n SecCertificate - none are NSObject
		/// <summary>The security identity of this peer.</summary>
		///         <value>Location [0] holds a <c>SecIdentityRef</c> for the local peer. Additional values (if they exist) will be for connected peers.</value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("securityIdentity")]
		NSArray SecurityIdentity { get; }

		/// <summary>What type, if any, encryption s preferred.</summary>
		///         <value>The default value is <see cref="F:MultipeerConnectivity.MCEncryptionPreference.Optional" />.</value>
		///         <remarks>To be added.</remarks>
		[Export ("encryptionPreference")]
		MCEncryptionPreference EncryptionPreference { get; }

		/// <summary>Represents the value associated with the constant kMCSessionMaximumNumberOfPeers</summary>
		///         <value>The value is 8.</value>
		///         <remarks>To be added.</remarks>
		///         <altmember cref="P:MultipeerConnectivity.MCBrowserViewController.MaximumNumberOfPeers" />
		[Field ("kMCSessionMaximumNumberOfPeers")]
		nint MaximumNumberOfPeers { get; }

		/// <summary>Represents the value associated with the constant kMCSessionMinimumNumberOfPeers</summary>
		///         <value>The value is 2.
		///         </value>
		///         <remarks>To be added.</remarks>
		///         <altmember cref="P:MultipeerConnectivity.MCBrowserViewController.MinimumNumberOfPeers" />
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

	/// <summary>Interface representing the required methods (if any) of the protocol <see cref="T:MultipeerConnectivity.MCSessionDelegate" />.</summary>
	///     <remarks>
	///       <para>This interface contains the required methods (if any) from the protocol defined by <see cref="T:MultipeerConnectivity.MCSessionDelegate" />.</para>
	///       <para>If developers create classes that implement this interface, the implementation methods will automatically be exported to Objective-C with the matching signature from the method defined in the <see cref="T:MultipeerConnectivity.MCSessionDelegate" /> protocol.</para>
	///       <para>Optional methods (if any) are provided by the <see cref="T:MultipeerConnectivity.MCSessionDelegate_Extensions" /> class as extension methods to the interface, allowing developers to invoke any optional methods on the protocol.</para>
	///     </remarks>
	interface IMCSessionDelegate { }

	/// <summary>A delegate object whose functions relate to events in the <see cref="T:MultipeerConnectivity.MCSession" /> life-cycle, such as connection status changes and data reception.</summary>
	///     <remarks>
	///       <para>Callbacks to the <see cref="T:MultipeerConnectivity.MCSessionDelegate" /> object are likely to be made on background threads. Application developers who wish to update the display must use, for instance, <see cref="M:Foundation.NSObject.InvokeOnMainThread(ObjCRuntime.Selector,Foundation.NSObject)" />.</para>
	///     </remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/MultipeerConnectivity/Reference/MCSessionDelegateRef/index.html">Apple documentation for <c>MCSessionDelegate</c></related>
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

	/// <summary>Provides programmatic control for advertising the device for multipeer connectivity.</summary>
	///     <remarks>
	///       <para>Multipeer connectivity's discovery phase involves two roles: browsing and advertising. When an application makes itself available for connection, it is advertising. Advertising may be controlled by either a <see cref="T:MultipeerConnectivity.MCAdvertiserAssistant" /> or can be fully customized with a <see cref="T:MultipeerConnectivity.MCNearbyServiceAdvertiser" />. For a discussion of the discovery process, see the remarks for <see cref="T:MultipeerConnectivity.MCSession" />.</para>
	///     </remarks>
	///     
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/MultipeerConnectivity/Reference/MCNearbyServiceAdvertiserClassRef/index.html">Apple documentation for <c>MCNearbyServiceAdvertiser</c></related>
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

	/// <param name="accept">
	///       <see langword="true" /> if the invitation should be accepted.</param>
	///     <param name="session">The session to which the peer shouldbe connected.</param>
	///     <summary>The delegate that serves as the invitation handler in calls to <see cref="M:MultipeerConnectivity.MCNearbyServiceAdvertiserDelegate.DidReceiveInvitationFromPeer(MultipeerConnectivity.MCNearbyServiceAdvertiser,MultipeerConnectivity.MCPeerID,Foundation.NSData,MultipeerConnectivity.MCNearbyServiceAdvertiserInvitationHandler)" />.</summary>
	delegate void MCNearbyServiceAdvertiserInvitationHandler (bool accept, [NullAllowed] MCSession session);

	/// <summary>Interface representing the required methods (if any) of the protocol <see cref="T:MultipeerConnectivity.MCNearbyServiceAdvertiserDelegate" />.</summary>
	///     <remarks>
	///       <para>This interface contains the required methods (if any) from the protocol defined by <see cref="T:MultipeerConnectivity.MCNearbyServiceAdvertiserDelegate" />.</para>
	///       <para>If developers create classes that implement this interface, the implementation methods will automatically be exported to Objective-C with the matching signature from the method defined in the <see cref="T:MultipeerConnectivity.MCNearbyServiceAdvertiserDelegate" /> protocol.</para>
	///       <para>Optional methods (if any) are provided by the <see cref="T:MultipeerConnectivity.MCNearbyServiceAdvertiserDelegate_Extensions" /> class as extension methods to the interface, allowing developers to invoke any optional methods on the protocol.</para>
	///     </remarks>
	interface IMCNearbyServiceAdvertiserDelegate { }

	/// <summary>A delegate object that exposes events relating to advertising and invitations for multipeer connectivity for a <see cref="T:MultipeerConnectivity.MCNearbyServiceAdvertiser" /> object.</summary>
	///     <remarks>
	///       <para>For a discussion of the discovery process, see the remarks for <see cref="T:MultipeerConnectivity.MCSession" />.</para>
	///     </remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/MultipeerConnectivity/Reference/MCNearbyServiceAdvertiserDelegateProtocolRef/index.html">Apple documentation for <c>MCNearbyServiceAdvertiserDelegate</c></related>
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

	/// <summary>Allows programmatic browsing for devices advertising for multipeer connetivity.</summary>
	///     <remarks>
	///       <para>Multipeer connectivity's discovery phase involves two roles: browsing and advertising. When an application searches for peers with which to connect, it is browsing. Browsing may be controlled by either a <see cref="T:MultipeerConnectivity.MCBrowserViewController" /> or can be fully customized with a <see cref="T:MultipeerConnectivity.MCNearbyServiceBrowser" />. For a discussion of the discovery process, see the remarks for <see cref="T:MultipeerConnectivity.MCSession" />.</para>
	///     </remarks>
	///     
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/MultipeerConnectivity/Reference/MCNearbyServiceBrowserClassRef/index.html">Apple documentation for <c>MCNearbyServiceBrowser</c></related>
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

	/// <summary>A delegate object that exposes peer-discovery events for a <see cref="T:MultipeerConnectivity.MCNearbyServiceBrowser" /> object.</summary>
	///     <remarks>
	///       <para>For a discussion of peer discovery and connection, see <see cref="T:MultipeerConnectivity.MCSession" /> remarks.</para>
	///       <para>Methods of <see cref="T:MultipeerConnectivity.MCNearbyServiceBrowserDelegate" /> are typically called by the system on a background thread. Application developers who wish to modify the user interface must use, for instance, <see cref="M:Foundation.NSObject.InvokeOnMainThread(ObjCRuntime.Selector,Foundation.NSObject)" />.</para>
	///     </remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/MultipeerConnectivity/Reference/MCNearbyServiceBrowserDelegateRef/index.html">Apple documentation for <c>MCNearbyServiceBrowserDelegate</c></related>
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

	/// <summary>Interface representing the required methods (if any) of the protocol <see cref="T:MultipeerConnectivity.MCNearbyServiceBrowserDelegate" />.</summary>
	///     <remarks>
	///       <para>This interface contains the required methods (if any) from the protocol defined by <see cref="T:MultipeerConnectivity.MCNearbyServiceBrowserDelegate" />.</para>
	///       <para>If developers create classes that implement this interface, the implementation methods will automatically be exported to Objective-C with the matching signature from the method defined in the <see cref="T:MultipeerConnectivity.MCNearbyServiceBrowserDelegate" /> protocol.</para>
	///       <para>Optional methods (if any) are provided by the <see cref="T:MultipeerConnectivity.MCNearbyServiceBrowserDelegate_Extensions" /> class as extension methods to the interface, allowing developers to invoke any optional methods on the protocol.</para>
	///     </remarks>
	interface IMCNearbyServiceBrowserDelegate { }

	/// <include file="../docs/api/MultipeerConnectivity/MCBrowserViewController.xml" path="/Documentation/Docs[@DocId='T:MultipeerConnectivity.MCBrowserViewController']/*" />
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

	/// <summary>Interface representing the required methods (if any) of the protocol <see cref="T:MultipeerConnectivity.MCBrowserViewControllerDelegate" />.</summary>
	///     <remarks>
	///       <para>This interface contains the required methods (if any) from the protocol defined by <see cref="T:MultipeerConnectivity.MCBrowserViewControllerDelegate" />.</para>
	///       <para>If developers create classes that implement this interface, the implementation methods will automatically be exported to Objective-C with the matching signature from the method defined in the <see cref="T:MultipeerConnectivity.MCBrowserViewControllerDelegate" /> protocol.</para>
	///       <para>Optional methods (if any) are provided by the <see cref="T:MultipeerConnectivity.MCBrowserViewControllerDelegate_Extensions" /> class as extension methods to the interface, allowing developers to invoke any optional methods on the protocol.</para>
	///     </remarks>
	interface IMCBrowserViewControllerDelegate { }

	/// <summary>A delegate object that provides events relating to the presentation of discovered peers and the application user's selection or cancellation of them.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/MultipeerConnectivity/Reference/MCBrowserViewControllerDelegate/index.html">Apple documentation for <c>MCBrowserViewControllerDelegate</c></related>
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

	/// <summary>A convenience class that manages the process of advertising for multipeer connectivity and interacting with the application user.</summary>
	///     <remarks>
	///       <para>Multipeer connectivity's discovery phase involves two roles: browsing and advertising. When an application makes itself available for connection, it is advertising. Advertising may be controlled by either a <see cref="T:MultipeerConnectivity.MCAdvertiserAssistant" /> or can be fully customized with a <see cref="T:MultipeerConnectivity.MCNearbyServiceAdvertiser" />. For a discussion of the discovery process, see the remarks for <see cref="T:MultipeerConnectivity.MCSession" />.</para>
	///     </remarks>
	///     
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/MultipeerConnectivity/Reference/MCAdvertiserAssistant_class/index.html">Apple documentation for <c>MCAdvertiserAssistant</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: -[MCAdvertiserAssistant init]: unrecognized selector sent to instance 0x7ea7fa40
	interface MCAdvertiserAssistant {

		[DesignatedInitializer]
		[Export ("initWithServiceType:discoveryInfo:session:")]
		NativeHandle Constructor (string serviceType, [NullAllowed] NSDictionary info, MCSession session);

		/// <include file="../docs/api/MultipeerConnectivity/MCAdvertiserAssistant.xml" path="/Documentation/Docs[@DocId='P:MultipeerConnectivity.MCAdvertiserAssistant.DiscoveryInfo']/*" />
		[NullAllowed]
		[Export ("discoveryInfo")]
		NSDictionary DiscoveryInfo { get; }

		/// <summary>The <see cref="T:MultipeerConnectivity.MCSession" /> into which peers will be placed.</summary>
		///         <value>Passed in to the <format type="text/html"><a href="https://docs.microsoft.com/en-us/search/index?search=C:MultipeerConnectivity.MCAdvertiserAssistant(string,Foundation.NSDictionary, MultipeerConnectivity.MCSession)&amp;scope=Xamarin" title="C:MultipeerConnectivity.MCAdvertiserAssistant(string,Foundation.NSDictionary, MultipeerConnectivity.MCSession)">C:MultipeerConnectivity.MCAdvertiserAssistant(string,Foundation.NSDictionary, MultipeerConnectivity.MCSession)</a></format> constructor.</value>
		///         <remarks>To be added.</remarks>
		[Export ("session")]
		MCSession Session { get; }

		/// <summary>A string, between 1 and 15 characters long, identifying the network protocol being advertised.</summary>
		///         <value>Passed in to the <format type="text/html"><a href="https://docs.microsoft.com/en-us/search/index?search=C:MultipeerConnectivity.MCAdvertiserAssistant(string,Foundation.NSDictionary, MultipeerConnectivity.MCSession)&amp;scope=Xamarin" title="C:MultipeerConnectivity.MCAdvertiserAssistant(string,Foundation.NSDictionary, MultipeerConnectivity.MCSession)">C:MultipeerConnectivity.MCAdvertiserAssistant(string,Foundation.NSDictionary, MultipeerConnectivity.MCSession)</a></format> constructor.</value>
		///         <remarks>To be added.</remarks>
		[Export ("serviceType")]
		string ServiceType { get; }

		/// <summary>An object that can respond to the delegate protocol for this type</summary>
		///         <value>The instance that will respond to events and data requests.</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>   Methods must be decorated with the [Export ("selectorName")] attribute to respond to each method from the protocol.   Alternatively use the Delegate method which is strongly typed and does not require the [Export] attributes on methods.</para>
		///         </remarks>
		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		/// <summary>An instance of the MultipeerConnectivity.IMCAdvertiserAssistantDelegate model class which acts as the class delegate.</summary>
		///         <value>The instance of the MultipeerConnectivity.IMCAdvertiserAssistantDelegate model class</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>This is the strongly typed version of the object, developers should use the WeakDelegate property instead if they want to merely assign a class derived from NSObject that has been decorated with [Export] attributes.</para>
		///         </remarks>
		[Wrap ("WeakDelegate")]
		IMCAdvertiserAssistantDelegate Delegate { get; set; }

		[Export ("start")]
		void Start ();

		[Export ("stop")]
		void Stop ();
	}

	/// <summary>Interface representing the required methods (if any) of the protocol <see cref="T:MultipeerConnectivity.MCAdvertiserAssistantDelegate" />.</summary>
	///     <remarks>
	///       <para>This interface contains the required methods (if any) from the protocol defined by <see cref="T:MultipeerConnectivity.MCAdvertiserAssistantDelegate" />.</para>
	///       <para>If developers create classes that implement this interface, the implementation methods will automatically be exported to Objective-C with the matching signature from the method defined in the <see cref="T:MultipeerConnectivity.MCAdvertiserAssistantDelegate" /> protocol.</para>
	///       <para>Optional methods (if any) are provided by the <see cref="T:MultipeerConnectivity.MCAdvertiserAssistantDelegate_Extensions" /> class as extension methods to the interface, allowing developers to invoke any optional methods on the protocol.</para>
	///     </remarks>
	interface IMCAdvertiserAssistantDelegate { }

	/// <summary>A delegate object that provides events for the presentation or dismissal of an invitation by a <see cref="T:MultipeerConnectivity.MCAdvertiserAssistant" />.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/MultipeerConnectivity/Reference/MCAdvertiserAssistantDelegate_class/index.html">Apple documentation for <c>MCAdvertiserAssistantDelegate</c></related>
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
