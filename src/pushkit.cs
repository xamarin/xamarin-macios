using XamCore.ObjCRuntime;
using XamCore.Foundation;
using XamCore.CoreFoundation;
using System;

namespace XamCore.PushKit 
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
	}

	[iOS (8,0)]
	[Model]
	[Protocol]
	[BaseType (typeof (NSObject))]
	interface PKPushRegistryDelegate {
		[Abstract]
		[Export ("pushRegistry:didUpdatePushCredentials:forType:"), EventArgs ("PKPushRegistryUpdated"), EventName ("CredentialsUpdated")]
		void DidUpdatePushCredentials (PKPushRegistry registry, PKPushCredentials credentials, string type);

		[Abstract]
		[Export ("pushRegistry:didReceiveIncomingPushWithPayload:forType:"), EventArgs ("PKPushRegistryRecieved"), EventName ("IncomingPushReceived")]
		void DidReceiveIncomingPush (PKPushRegistry registry, PKPushPayload payload, string type);

		[Export ("pushRegistry:didInvalidatePushTokenForType:"), EventArgs ("PKPushRegistryRecieved"), EventName ("PushTokenInvalidated")]
		void DidInvalidatePushToken (PKPushRegistry registry, string type);
	}
}
