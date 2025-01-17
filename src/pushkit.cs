using ObjCRuntime;
using Foundation;
using CoreFoundation;
using System;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace PushKit {
	/// <summary>Holds the <see cref="P:PushKit.PKPushCredentials.Token" /> that holds the user's credentials.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/PushKit/Reference/PKPushCredentials_Class/index.html">Apple documentation for <c>PKPushCredentials</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKPushCredentials {
		[Export ("type", ArgumentSemantic.Copy)]
		string Type { get; }

		[Export ("token", ArgumentSemantic.Copy)]
		NSData Token { get; }
	}

	/// <summary>Contains a dictionary of data for a push operation.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/PushKit/Reference/PKPushPayload_Class/index.html">Apple documentation for <c>PKPushPayload</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKPushPayload {
		[Export ("type", ArgumentSemantic.Copy)]
		string Type { get; }

		[Export ("dictionaryPayload", ArgumentSemantic.Copy)]
		NSDictionary DictionaryPayload { get; }
	}

	/// <summary>Allows the developer to register for remote pushes.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/PushKit/Reference/PKPushRegistry_Class/index.html">Apple documentation for <c>PKPushRegistry</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKPushRegistry {
		[Wrap ("WeakDelegate")]
		IPKPushRegistryDelegate Delegate { get; set; }

		[Export ("delegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Export ("desiredPushTypes", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSSet DesiredPushTypes { get; set; }

		[Export ("pushTokenForType:")]
		[return: NullAllowed]
		NSData PushToken (string type);

		[DesignatedInitializer]
		[Export ("initWithQueue:")]
		NativeHandle Constructor ([NullAllowed] DispatchQueue queue);
	}

	/// <summary>Holds the transports available for Push Kit (currently only Voice Over IP).</summary>
	[MacCatalyst (13, 1)]
	[Static]
	interface PKPushType {

		[Introduced (PlatformName.MacCatalyst, 14, 0)]
		[NoMac]
		[Field ("PKPushTypeVoIP")]
		NSString Voip { get; }

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use directly from watchOS instead.")]
		[NoMac]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use directly from watchOS instead.")]
		[Field ("PKPushTypeComplication")]
		NSString Complication { get; }

		[MacCatalyst (13, 1)]
		[Field ("PKPushTypeFileProvider")]
		NSString FileProvider { get; }
	}

	/// <summary>Interface representing the required methods (if any) of the protocol <see cref="T:PushKit.PKPushRegistryDelegate" />.</summary>
	///     <remarks>
	///       <para>This interface contains the required methods (if any) from the protocol defined by <see cref="T:PushKit.PKPushRegistryDelegate" />.</para>
	///       <para>If developers create classes that implement this interface, the implementation methods will automatically be exported to Objective-C with the matching signature from the method defined in the <see cref="T:PushKit.PKPushRegistryDelegate" /> protocol.</para>
	///       <para>Optional methods (if any) are provided by the <see cref="T:PushKit.PKPushRegistryDelegate_Extensions" /> class as extension methods to the interface, allowing developers to invoke any optional methods on the protocol.</para>
	///     </remarks>
	interface IPKPushRegistryDelegate { }

	/// <summary>Completion handler for registering a push operation.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/PushKit/Reference/PKPushRegistryDelegate_Protocol/index.html">Apple documentation for <c>PKPushRegistryDelegate</c></related>
	[MacCatalyst (13, 1)]
	[Model]
	[Protocol]
	[BaseType (typeof (NSObject))]
	interface PKPushRegistryDelegate {
		[Abstract]
		[Export ("pushRegistry:didUpdatePushCredentials:forType:"), EventArgs ("PKPushRegistryUpdated"), EventName ("CredentialsUpdated")]
		void DidUpdatePushCredentials (PKPushRegistry registry, PKPushCredentials credentials, string type);

		[NoMac]
#if !NET
		[Abstract] // now optional in iOS 11
#endif
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use the 'DidReceiveIncomingPushWithPayload' overload accepting an 'Action' argument instead.")]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the 'DidReceiveIncomingPushWithPayload' overload accepting an 'Action' argument instead.")]
		[Export ("pushRegistry:didReceiveIncomingPushWithPayload:forType:"), EventArgs ("PKPushRegistryRecieved"), EventName ("IncomingPushReceived")]
		void DidReceiveIncomingPush (PKPushRegistry registry, PKPushPayload payload, string type);

		[MacCatalyst (13, 1)]
		[Export ("pushRegistry:didReceiveIncomingPushWithPayload:forType:withCompletionHandler:")]
		void DidReceiveIncomingPush (PKPushRegistry registry, PKPushPayload payload, string type, Action completion);

		[Export ("pushRegistry:didInvalidatePushTokenForType:"), EventArgs ("PKPushRegistryRecieved"), EventName ("PushTokenInvalidated")]
		void DidInvalidatePushToken (PKPushRegistry registry, string type);
	}
}
