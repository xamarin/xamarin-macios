// Copyright 2013 Xamarin Inc. All rights reserved.

using Mono.Cecil;
using Mono.Tuner;

using Xamarin.Tuner;

namespace Xamarin.Linker {

	static class Namespaces {
		public const string Accounts = "Accounts";
		public const string AddressBook = "AddressBook";
		public const string AdSupport = "AdSupport";
		public const string AppKit = "AppKit";
		public const string AVFoundation = "AVFoundation";
		public const string AVKit = "AVKit";
		public const string CloudKit = "CloudKit";
		public const string Contacts = "Contacts";
		public const string ContactsUI = "ContactsUI";
		public const string CoreAnimation = "CoreAnimation";
		public const string CoreAudioKit = "CoreAudioKit";
		public const string CoreBluetooth = "CoreBluetooth";
		public const string CoreFoundation = "CoreFoundation";
		public const string CoreImage = "CoreImage";
		public const string CoreLocation = "CoreLocation";
		public const string CoreMIDI = "CoreMIDI";
		public const string CoreML = "CoreML";
		public const string CoreText = "CoreText";
		public const string CoreWlan = "CoreWlan";
		public const string EventKit = "EventKit";
		public const string ExternalAccessory = "ExternalAccessory";
		public const string FinderSync = "FinderSync";
		public const string Foundation = "Foundation";
		public const string GameController = "GameController";
		public const string GameKit = "GameKit";
		public const string GameplayKit = "GameplayKit";
		public const string GLKit = "GLKit";
		public const string ImageCaptureCore = "ImageCaptureCore";
		public const string ImageKit = "ImageKit";
		public const string InputMethodKit = "InputMethodKit";
		public const string Intents = "Intents";
		public const string IOBluetooth = "IOBluetooth";
		public const string IOBluetoothUI = "IOBluetoothUI";
		public const string IOSurface = "IOSurface";
		public const string iTunesLibrary = "iTunesLibrary";
		public const string JavaScriptCore = "JavaScriptCore";
		public const string LocalAuthentication = "LocalAuthentication";
		public const string MapKit = "MapKit";
		public const string MediaAccessibility = "MediaAccessibility";
		public const string MediaLibrary = "MediaLibrary";
		public const string MediaPlayer = "MediaPlayer";
		public const string MetalKit = "MetalKit";
		public const string MetalPerformanceShaders = "MetalPerformanceShaders";
		public const string ModelIO = "ModelIO";
		public const string MultipeerConnectivity = "MultipeerConnectivity";
		public const string NaturalLanguage = "NaturalLanguage";
		public const string Network = "Network";
		public const string NetworkExtension = "NetworkExtension";
		public const string NotificationCenter = "NotificationCenter";
		public const string ObjCRuntime = "ObjCRuntime";
		public const string OpenAL = "OpenAL";
		public const string PdfKit = "PdfKit";
		public const string Photos = "Photos";
		public const string PhotosUI = "PhotosUI";
		public const string PrintCore = "PrintCore";
		public const string QTKit = "QTKit";
		public const string QuartzComposer = "QuartzComposer";
		public const string Registrar = "Registrar";
		public const string SceneKit = "SceneKit";
		public const string ScriptingBridge = "ScriptingBridge";
		public const string Security = "Security";
		public const string Social = "Social";
		public const string SpriteKit = "SpriteKit";
		public const string StoreKit = "StoreKit";
		public const string UIKit = "UIKit";
		public const string VideoSubscriberAccount = "VideoSubscriberAccount";
		public const string VideoToolbox = "VideoToolbox";
		public const string Vision = "Vision";
		public const string WebKit = "WebKit";
	}

	static class ObjCExtensions {

		const string INativeObject = Namespaces.ObjCRuntime + ".INativeObject";
#if !NET
		public static bool IsNSObject (this TypeReference type, DerivedLinkContext link_context)
		{
			return type.Resolve ().IsNSObject (link_context);
		}

		// warning: *Is* means does 'type' inherits from MonoTouch.Foundation.NSObject ?
		public static bool IsNSObject (this TypeDefinition type, DerivedLinkContext link_context)
		{
			if (link_context?.CachedIsNSObject != null)
				return link_context.CachedIsNSObject.Contains (type);

			return type.Inherits (Namespaces.Foundation, "NSObject");
		}
#endif

		public static bool IsNativeObject (this TypeDefinition type)
		{
			return type.Implements (INativeObject);
		}
	}
}
