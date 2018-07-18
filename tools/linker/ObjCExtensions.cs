// Copyright 2013 Xamarin Inc. All rights reserved.

using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Tuner;

using Xamarin.Tuner;

namespace Xamarin.Linker {

	static class Namespaces {

		internal static void Initialize ()
		{
			if (AddressBook != null)
				return;

			var profile = (Profile.Current as BaseProfile);
			AddressBook = profile.GetNamespace ("AddressBook");
			AVFoundation = profile.GetNamespace ("AVFoundation");
			CoreAnimation = profile.GetNamespace ("CoreAnimation");
			CoreBluetooth = profile.GetNamespace ("CoreBluetooth");
			CoreFoundation = profile.GetNamespace ("CoreFoundation");
			CoreLocation = profile.GetNamespace ("CoreLocation");
			CoreText = profile.GetNamespace ("CoreText");
			Foundation = profile.GetNamespace ("Foundation");
			GameKit = profile.GetNamespace ("GameKit");
			ObjCRuntime = profile.GetNamespace ("ObjCRuntime");
			Security = profile.GetNamespace ("Security");
			StoreKit = profile.GetNamespace ("StoreKit");
			GameController = profile.GetNamespace ("GameController");
			JavaScriptCore = profile.GetNamespace ("JavaScriptCore");
			CoreAudioKit = profile.GetNamespace ("CoreAudioKit");
			InputMethodKit = profile.GetNamespace ("InputMethodKit");
			OpenAL = profile.GetNamespace ("OpenAL");
			MediaAccessibility = profile.GetNamespace ("MediaAccessibility");
			CoreMIDI = profile.GetNamespace ("CoreMIDI");
			MediaLibrary = profile.GetNamespace ("MediaLibrary");
			GLKit = profile.GetNamespace ("GLKit");
			SpriteKit = profile.GetNamespace ("SpriteKit");
			CloudKit = profile.GetNamespace ("CloudKit");
			LocalAuthentication = profile.GetNamespace ("LocalAuthentication");
			Accounts = profile.GetNamespace ("Accounts");
			Contacts = profile.GetNamespace ("Contacts");
			ContactsUI = profile.GetNamespace ("ContactsUI");
			MapKit = profile.GetNamespace ("MapKit");
			EventKit = profile.GetNamespace ("EventKit");
			Social = profile.GetNamespace ("Social");
			AVKit = profile.GetNamespace ("AVKit");
			VideoToolbox = profile.GetNamespace ("VideoToolbox");
			GameplayKit = profile.GetNamespace ("GameplayKit");
			NetworkExtension = profile.GetNamespace ("NetworkExtension");
			MultipeerConnectivity = profile.GetNamespace ("MultipeerConnectivity");
			MetalKit = profile.GetNamespace ("MetalKit");
			MetalPerformanceShaders = profile.GetNamespace ("MetalPerformanceShaders");
			ModelIO = profile.GetNamespace ("ModelIO");
			Intents = profile.GetNamespace ("Intents");
			Photos = profile.GetNamespace ("Photos");
			CoreML = profile.GetNamespace ("CoreML");
			Vision = profile.GetNamespace ("Vision");
			IOSurface = profile.GetNamespace ("IOSurface");
			PdfKit = profile.GetNamespace ("PdfKit");
			CoreImage = profile.GetNamespace ("CoreImage");
			AdSupport = profile.GetNamespace ("AdSupport");
			NaturalLanguage = profile.GetNamespace ("NaturalLanguage");
			VideoSubscriberAccount = profile.GetNamespace ("VideoSubscriberAccount");
#if MONOMAC
			PhotosUI = profile.GetNamespace ("PhotosUI");
			IOBluetooth = profile.GetNamespace ("IOBluetooth");
			IOBluetoothUI = profile.GetNamespace ("IOBluetoothUI");
			FinderSync = profile.GetNamespace ("FinderSync");
			NotificationCenter = profile.GetNamespace ("NotificationCenter");
			AppKit = profile.GetNamespace ("AppKit");
			CoreWlan = profile.GetNamespace ("CoreWlan");
			ImageKit = profile.GetNamespace ("ImageKit");
			QTKit = profile.GetNamespace ("QTKit");
			QuartzComposer = profile.GetNamespace ("QuartzComposer");
			SceneKit = profile.GetNamespace ("SceneKit");
			ScriptingBridge = profile.GetNamespace ("ScriptingBridge");
			WebKit = profile.GetNamespace ("WebKit");
			MediaPlayer = profile.GetNamespace ("MediaPlayer");
			PrintCore = profile.GetNamespace ("PrintCore");
			ExternalAccessory = profile.GetNamespace ("ExternalAccessory");
			iTunesLibrary= profile.GetNamespace ("iTunesLibrary");
#else
			Registrar = profile.GetNamespace ("Registrar");
			UIKit = profile.GetNamespace ("UIKit");
#endif
		}

		public static string JavaScriptCore { get; private set; }
		public static string CoreAudioKit { get; private set; }
		public static string InputMethodKit { get; private set; }
		public static string OpenAL { get; private set; }
		public static string MediaAccessibility { get; private set; }
		public static string CoreMIDI { get; private set; }
		public static string MediaLibrary { get; private set; }
		public static string GLKit { get; private set; }
		public static string SpriteKit { get; private set; }
		public static string CloudKit { get; private set; }
		public static string LocalAuthentication { get; private set; }
		public static string Accounts { get; private set; }
		public static string Contacts { get; private set; }
		public static string ContactsUI { get; private set; }
		public static string MapKit { get; private set; }
		public static string EventKit { get; private set; }
		public static string Social { get; private set; }
		public static string AVKit { get; private set; }
		public static string VideoToolbox { get; private set; }
		public static string GameplayKit { get; private set; }
		public static string NetworkExtension { get; private set; }
		public static string MultipeerConnectivity { get; private set; }
		public static string MetalKit { get; private set; }
		public static string MetalPerformanceShaders { get; private set; }
		public static string ModelIO { get; private set; }

		public static string AddressBook { get; private set; }

		public static string AVFoundation { get; private set; }

		public static string CoreAnimation { get; private set; }

		public static string CoreBluetooth { get; private set; }

		public static string CoreFoundation { get; private set; }

		public static string CoreLocation { get; private set; }

		public static string CoreText { get; private set; }

		public static string Foundation { get; private set; }

		public static string GameKit { get; private set; }
		
		public static string GameController { get; private set; }

		public static string ObjCRuntime { get; private set; }

		public static string Security { get; private set; }

		public static string StoreKit { get; private set; }

		public static string Intents { get; private set; }

		public static string Photos { get; private set; }

		public static string CoreML { get; private set; }

		public static string Vision { get; private set; }

		public static string IOSurface { get; private set; }

		public static string PdfKit { get; private set; }

		public static string CoreImage { get; private set; }

		public static string AdSupport { get; private set; }

		public static string NaturalLanguage { get; private set; }

		public static string VideoSubscriberAccount { get; private set; }
#if MONOMAC
		public static string PhotosUI { get; private set; }
		public static string IOBluetooth { get; private set; }
		public static string IOBluetoothUI { get; private set; }
		public static string FinderSync { get; private set; }
		public static string NotificationCenter { get; private set; }

		public static string AppKit { get; private set; }

		public static string CoreWlan { get; private set; }

		public static string ImageKit { get; private set; }

		public static string QTKit { get; private set; }

		public static string QuartzComposer { get; private set; }

		public static string SceneKit { get; private set; }

		public static string ScriptingBridge { get; private set; }

		public static string WebKit { get; private set; }
		public static string MediaPlayer { get; private set; }
		public static string PrintCore { get; private set; }
		public static string ExternalAccessory { get; private set; }
		public static string iTunesLibrary { get; private set; }
#else
		public static string Registrar { get; private set; }

		public static string UIKit { get; private set; }
#endif
	}

	static class ObjCExtensions {

		static string inativeobject;

		static string INativeObject {
			get {
				if (inativeobject == null)
					inativeobject = Namespaces.ObjCRuntime + ".INativeObject";
				return inativeobject;
			}
		}

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

		public static bool IsNativeObject (this TypeDefinition type)
		{
			return type.Implements (INativeObject);
		}
	}
}