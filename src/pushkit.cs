using ObjCRuntime;
using Foundation;
using CoreFoundation;
using System;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace PushKit 
{
	[Watch (6,0)]
	[Mac (10,15)]
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKPushCredentials {
		[Export ("type", ArgumentSemantic.Copy)]
		string Type { get; }

		[Export ("token", ArgumentSemantic.Copy)]
		NSData Token { get; }
	}

	[Watch (6,0)]
	[Mac (10,15)]
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKPushPayload {
		[Export ("type", ArgumentSemantic.Copy)]
		string Type { get; }

		[Export ("dictionaryPayload", ArgumentSemantic.Copy)]
		NSDictionary DictionaryPayload { get; }
	}

	[Watch (6,0)]
	[Mac (10,15)]
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKPushRegistry {
		[Wrap ("WeakDelegate")]
		[Protocolize]
		PKPushRegistryDelegate Delegate { get; set; }

		[Export ("delegate", ArgumentSemantic.Weak)][NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Export ("desiredPushTypes", ArgumentSemantic.Copy)][NullAllowed]
		NSSet DesiredPushTypes { get; set; }

		[Export ("pushTokenForType:")]
		[return: NullAllowed]
		NSData PushToken (string type);

		[DesignatedInitializer]
		[Export ("initWithQueue:")]
		NativeHandle Constructor ([NullAllowed] DispatchQueue queue);
	}
	
	[iOS (8,0)]
	[Static]
	interface PKPushType {

		[Introduced (PlatformName.MacCatalyst, 14, 0)]
		[Watch (9,0)]
		[NoMac]
		[Field ("PKPushTypeVoIP")]
		NSString Voip { get; }

		[iOS (9,0)]
		[Deprecated (PlatformName.iOS, 13,0, message: "Use directly from watchOS instead.")]
		[Watch (6,0)]
		[NoMac]
		[NoMacCatalyst]
		[Field ("PKPushTypeComplication")]
		NSString Complication { get; }

		[iOS (11,0)]
		[NoWatch]
		[Mac (10,15)]
		[Field ("PKPushTypeFileProvider")]
		NSString FileProvider { get; }
	}

	[iOS (8,0)]
	[Watch (6,0)]
	[Mac (10,15)]
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
		[Deprecated (PlatformName.iOS, 11,0, message: "Use the 'DidReceiveIncomingPushWithPayload' overload accepting an 'Action' argument instead.")]
		[NoMacCatalyst]
		[Export ("pushRegistry:didReceiveIncomingPushWithPayload:forType:"), EventArgs ("PKPushRegistryRecieved"), EventName ("IncomingPushReceived")]
		void DidReceiveIncomingPush (PKPushRegistry registry, PKPushPayload payload, string type);

		[iOS (11,0)]
		[MacCatalyst (10,13)]
		[Export ("pushRegistry:didReceiveIncomingPushWithPayload:forType:withCompletionHandler:")]
		void DidReceiveIncomingPush (PKPushRegistry registry, PKPushPayload payload, string type, Action completion);

		[Export ("pushRegistry:didInvalidatePushTokenForType:"), EventArgs ("PKPushRegistryRecieved"), EventName ("PushTokenInvalidated")]
		void DidInvalidatePushToken (PKPushRegistry registry, string type);
	}
}
