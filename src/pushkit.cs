using ObjCRuntime;
using Foundation;
using CoreFoundation;
using System;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace PushKit {
	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKPushCredentials {
		[Export ("type", ArgumentSemantic.Copy)]
		string Type { get; }

		[Export ("token", ArgumentSemantic.Copy)]
		NSData Token { get; }
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKPushPayload {
		[Export ("type", ArgumentSemantic.Copy)]
		string Type { get; }

		[Export ("dictionaryPayload", ArgumentSemantic.Copy)]
		NSDictionary DictionaryPayload { get; }
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKPushRegistry {
		[Wrap ("WeakDelegate")]
		[Protocolize]
		PKPushRegistryDelegate Delegate { get; set; }

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

	[MacCatalyst (13, 1)]
	[Static]
	interface PKPushType {

		[Introduced (PlatformName.MacCatalyst, 14, 0)]
		[Watch (9, 0)]
		[NoMac]
		[Field ("PKPushTypeVoIP")]
		NSString Voip { get; }

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use directly from watchOS instead.")]
		[Watch (6, 0)]
		[NoMac]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use directly from watchOS instead.")]
		[Field ("PKPushTypeComplication")]
		NSString Complication { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Field ("PKPushTypeFileProvider")]
		NSString FileProvider { get; }
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[Model]
	[Protocol]
	[BaseType (typeof (NSObject))]
	interface PKPushRegistryDelegate {
		[Abstract]
		[Export ("pushRegistry:didUpdatePushCredentials:forType:"), EventArgs ("PKPushRegistryUpdated"), EventName ("CredentialsUpdated")]
		void DidUpdatePushCredentials (PKPushRegistry registry, PKPushCredentials credentials, string type);

		[NoWatch]
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
