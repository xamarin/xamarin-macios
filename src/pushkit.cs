using ObjCRuntime;
using Foundation;
using CoreFoundation;
using System;

namespace PushKit 
{
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKPushCredentials {
		[Export ("type", ArgumentSemantic.Copy)]
		string Type { get; }

		[Export ("token", ArgumentSemantic.Copy)]
		NSData Token { get; }
	}

	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKPushPayload {
		[Export ("type", ArgumentSemantic.Copy)]
		string Type { get; }

		[Export ("dictionaryPayload", ArgumentSemantic.Copy)]
		NSDictionary DictionaryPayload { get; }
	}

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
		NSData PushToken (string type);

		[DesignatedInitializer]
		[Export ("initWithQueue:")]
		IntPtr Constructor (DispatchQueue queue);
	}
	
	[iOS (8,0)]
	[Static]
	interface PKPushType {
		
		[Field ("PKPushTypeVoIP")]
		NSString Voip { get; }

		[iOS (9,0)]
		[Field ("PKPushTypeComplication")]
		NSString Complication { get; }

		[iOS (11,0)]
		[Field ("PKPushTypeFileProvider")]
		NSString FileProvider { get; }
	}

	[iOS (8,0)]
	[Model]
	[Protocol]
	[BaseType (typeof (NSObject))]
	interface PKPushRegistryDelegate {
		[Abstract]
		[Export ("pushRegistry:didUpdatePushCredentials:forType:"), EventArgs ("PKPushRegistryUpdated"), EventName ("CredentialsUpdated")]
		void DidUpdatePushCredentials (PKPushRegistry registry, PKPushCredentials credentials, string type);

		[Abstract] // now optional in iOS 11
		[Deprecated (PlatformName.iOS, 11,0, message: "Use the 'DidReceiveIncomingPushWithPayload' overload accepting an 'Action' argument instead.")]
		[Export ("pushRegistry:didReceiveIncomingPushWithPayload:forType:"), EventArgs ("PKPushRegistryRecieved"), EventName ("IncomingPushReceived")]
		void DidReceiveIncomingPush (PKPushRegistry registry, PKPushPayload payload, string type);

		[iOS (11,0)]
		[Export ("pushRegistry:didReceiveIncomingPushWithPayload:forType:withCompletionHandler:")]
		void DidReceiveIncomingPush (PKPushRegistry registry, PKPushPayload payload, string type, Action completion);

		[Export ("pushRegistry:didInvalidatePushTokenForType:"), EventArgs ("PKPushRegistryRecieved"), EventName ("PushTokenInvalidated")]
		void DidInvalidatePushToken (PKPushRegistry registry, string type);
	}
}
