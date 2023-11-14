// Copyright 2013 Xamarin Inc. All rights reserved.

using Mono.Cecil;
using Mono.Tuner;

using Xamarin.Tuner;

namespace Xamarin.Linker {

	static class Namespaces {
		public const string Accounts = nameof (Accounts);
		public const string AddressBook = nameof (AddressBook);
		public const string AdSupport = nameof (AdSupport);
		public const string AppKit = nameof (AppKit);
		public const string AVFoundation = nameof (AVFoundation);
		public const string AVKit = nameof (AVKit);
		public const string AVRouting = nameof (AVRouting);
#if !NET
		public const string Chip = nameof (Chip);
#endif
		public const string Cinematic = nameof (Cinematic);
		public const string CloudKit = nameof (CloudKit);
		public const string Contacts = nameof (Contacts);
		public const string ContactsUI = nameof (ContactsUI);
		public const string CoreAnimation = nameof (CoreAnimation);
		public const string CoreAudioKit = nameof (CoreAudioKit);
		public const string CoreBluetooth = nameof (CoreBluetooth);
		public const string CoreFoundation = nameof (CoreFoundation);
		public const string CoreImage = nameof (CoreImage);
		public const string CoreLocation = nameof (CoreLocation);
		public const string CoreMIDI = nameof (CoreMIDI);
		public const string CoreML = nameof (CoreML);
		public const string CoreText = nameof (CoreText);
		public const string CoreWlan = nameof (CoreWlan);
		public const string EventKit = nameof (EventKit);
		public const string ExternalAccessory = nameof (ExternalAccessory);
		public const string FinderSync = nameof (FinderSync);
		public const string Foundation = nameof (Foundation);
		public const string GameController = nameof (GameController);
		public const string GameKit = nameof (GameKit);
		public const string GameplayKit = nameof (GameplayKit);
		public const string GLKit = nameof (GLKit);
		public const string HealthKit = nameof (HealthKit);
		public const string ImageCaptureCore = nameof (ImageCaptureCore);
		public const string ImageKit = nameof (ImageKit);
		public const string InputMethodKit = nameof (InputMethodKit);
		public const string Intents = nameof (Intents);
		public const string IOBluetooth = nameof (IOBluetooth);
		public const string IOBluetoothUI = nameof (IOBluetoothUI);
		public const string IOSurface = nameof (IOSurface);
		public const string iTunesLibrary = nameof (iTunesLibrary);
		public const string JavaScriptCore = nameof (JavaScriptCore);
		public const string LocalAuthentication = nameof (LocalAuthentication);
		public const string MapKit = nameof (MapKit);
		public const string MediaAccessibility = nameof (MediaAccessibility);
		public const string MediaLibrary = nameof (MediaLibrary);
		public const string MediaPlayer = nameof (MediaPlayer);
		public const string MetalFX = nameof (MetalFX);
		public const string MetalKit = nameof (MetalKit);
		public const string MetalPerformanceShaders = nameof (MetalPerformanceShaders);
		public const string MetalPerformanceShadersGraph = nameof (MetalPerformanceShadersGraph);
		public const string ModelIO = nameof (ModelIO);
		public const string MultipeerConnectivity = nameof (MultipeerConnectivity);
		public const string NaturalLanguage = nameof (NaturalLanguage);
		public const string Network = nameof (Network);
		public const string NetworkExtension = nameof (NetworkExtension);
		public const string NotificationCenter = nameof (NotificationCenter);
		public const string ObjCRuntime = nameof (ObjCRuntime);
		public const string OpenAL = nameof (OpenAL);
		public const string PdfKit = nameof (PdfKit);
		public const string Photos = nameof (Photos);
		public const string PhotosUI = nameof (PhotosUI);
		public const string PrintCore = nameof (PrintCore);
#if !NET
		public const string QTKit = nameof (QTKit);
#endif
		public const string QuartzComposer = nameof (QuartzComposer);
		public const string Registrar = nameof (Registrar);
		public const string SceneKit = nameof (SceneKit);
		public const string ScriptingBridge = nameof (ScriptingBridge);
		public const string Security = nameof (Security);
		public const string SensitiveContentAnalysis = nameof (SensitiveContentAnalysis);
		public const string Social = nameof (Social);
		public const string SpriteKit = nameof (SpriteKit);
		public const string StoreKit = nameof (StoreKit);
		public const string Symbols = nameof (Symbols);
		public const string ThreadNetwork = nameof (ThreadNetwork);
		public const string UIKit = nameof (UIKit);
		public const string VideoSubscriberAccount = nameof (VideoSubscriberAccount);
		public const string VideoToolbox = nameof (VideoToolbox);
		public const string Vision = nameof (Vision);
		public const string WebKit = nameof (WebKit);
	}

	static class ObjCExtensions {

		const string INativeObject = Namespaces.ObjCRuntime + ".INativeObject";
		public static bool IsNSObject (this TypeReference type, DerivedLinkContext link_context)
		{
			return
#if NET
				link_context.LinkerConfiguration.Context.Resolve (type)
#else
				type.Resolve ()
#endif
				.IsNSObject (link_context);
		}

		// warning: *Is* means does 'type' inherits from MonoTouch.Foundation.NSObject ?
		public static bool IsNSObject (this TypeDefinition type, DerivedLinkContext link_context)
		{
			if (link_context?.CachedIsNSObject is not null)
				return link_context.CachedIsNSObject.Contains (type);

			return type.Inherits (Namespaces.Foundation, "NSObject"
#if NET
				, link_context.LinkerConfiguration.Context
#endif
			);
		}

		public static bool IsNativeObject (this TypeDefinition type)
		{
			return type.Implements (INativeObject);
		}
	}
}
