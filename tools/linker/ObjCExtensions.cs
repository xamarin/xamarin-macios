// Copyright 2013 Xamarin Inc. All rights reserved.

using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Tuner;

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
#if MONOMAC
			AppKit = profile.GetNamespace ("AppKit");
			CoreWlan = profile.GetNamespace ("CoreWlan");
			ImageKit = profile.GetNamespace ("ImageKit");
			PdfKit = profile.GetNamespace ("PdfKit");
			QTKit = profile.GetNamespace ("QTKit");
			QuartzComposer = profile.GetNamespace ("QuartzComposer");
			SceneKit = profile.GetNamespace ("SceneKit");
			ScriptingBridge = profile.GetNamespace ("ScriptingBridge");
			WebKit = profile.GetNamespace ("WebKit");
#else
			Registrar = profile.GetNamespace ("Registrar");
			UIKit = profile.GetNamespace ("UIKit");
#endif
		}

		public static string AddressBook { get; private set; }

		public static string AVFoundation { get; private set; }

		public static string CoreAnimation { get; private set; }

		public static string CoreBluetooth { get; private set; }

		public static string CoreFoundation { get; private set; }

		public static string CoreLocation { get; private set; }

		public static string CoreText { get; private set; }

		public static string Foundation { get; private set; }

		public static string GameKit { get; private set; }

		public static string ObjCRuntime { get; private set; }

		public static string Security { get; private set; }

		public static string StoreKit { get; private set; }

#if MONOMAC
		public static string AppKit { get; private set; }

		public static string CoreWlan { get; private set; }

		public static string ImageKit { get; private set; }

		public static string PdfKit { get; private set; }

		public static string QTKit { get; private set; }

		public static string QuartzComposer { get; private set; }

		public static string SceneKit { get; private set; }

		public static string ScriptingBridge { get; private set; }

		public static string WebKit { get; private set; }
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

		internal static HashSet<TypeDefinition> cached_isnsobject;

		public static bool IsNSObject (this TypeReference type)
		{
			return type.Resolve ().IsNSObject ();
		}

		// warning: *Is* means does 'type' inherits from MonoTouch.Foundation.NSObject ?
		public static bool IsNSObject (this TypeDefinition type)
		{
			if (cached_isnsobject != null)
				return cached_isnsobject.Contains (type);

			return type.Inherits (Namespaces.Foundation, "NSObject");
		}

		public static bool IsNativeObject (this TypeDefinition type)
		{
			return type.Implements (INativeObject);
		}
	}
}