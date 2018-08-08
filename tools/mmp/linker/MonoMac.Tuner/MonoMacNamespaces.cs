// Copyright 2012-2013 Xamarin Inc. All rights reserved.
//#define DEBUG
using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Linker;
using Mono.Linker.Steps;
using Mono.Tuner;
using Xamarin.Linker;

using Xamarin.Bundler;

namespace MonoMac.Tuner {

	public class MonoMacNamespaces : IStep {

	Dictionary <string, string> NamespaceMapping = new Dictionary <string, string> {
		{ Constants.FoundationLibrary, Namespaces.Foundation },
		{ Constants.AppKitLibrary, Namespaces.AppKit },
		{ Constants.AddressBookLibrary, Namespaces.AddressBook },
		{ Constants.CoreTextLibrary, Namespaces.CoreText },
		{ Constants.WebKitLibrary, Namespaces.WebKit },
		{ Constants.QuartzLibrary, Namespaces.CoreAnimation },
		{ Constants.QTKitLibrary, Namespaces.QTKit },
		{ Constants.CoreLocationLibrary, Namespaces.CoreLocation },
		{ Constants.SecurityLibrary, Namespaces.Security },
		{ Constants.QuartzComposerLibrary, Namespaces.QuartzComposer },
		{ Constants.CoreWlanLibrary, Namespaces.CoreWlan },
		{ Constants.PdfKitLibrary, Namespaces.PdfKit },
		{ Constants.ImageKitLibrary, Namespaces.ImageKit },
		{ Constants.ScriptingBridgeLibrary, Namespaces.ScriptingBridge },
		{ Constants.AVFoundationLibrary, Namespaces.AVFoundation },
		{ Constants.CoreBluetoothLibrary, Namespaces.CoreBluetooth },
		{ Constants.GameKitLibrary, Namespaces.GameKit },
		{ Constants.GameControllerLibrary, Namespaces.GameController },
		{ Constants.JavaScriptCoreLibrary, Namespaces.JavaScriptCore },
		{ Constants.CoreAudioKitLibrary, Namespaces.CoreAudioKit },
		{ Constants.InputMethodKitLibrary, Namespaces.InputMethodKit },
		{ Constants.OpenALLibrary, Namespaces.OpenAL },
		{ Constants.MediaAccessibilityLibrary, Namespaces.MediaAccessibility },
		{ Constants.CoreMidiLibrary, Namespaces.CoreMIDI },
		{ Constants.MediaLibraryLibrary, Namespaces.MediaLibrary },
		{ Constants.GLKitLibrary, Namespaces.GLKit },
		{ Constants.SpriteKitLibrary, Namespaces.SpriteKit },
		{ Constants.CloudKitLibrary, Namespaces.CloudKit },
		{ Constants.LocalAuthenticationLibrary, Namespaces.LocalAuthentication },
		{ Constants.AccountsLibrary, Namespaces.Accounts },
		{ Constants.ContactsLibrary, Namespaces.Contacts },
		{ Constants.ContactsUILibrary, Namespaces.ContactsUI },
		{ Constants.MapKitLibrary, Namespaces.MapKit },
		{ Constants.EventKitLibrary, Namespaces.EventKit },
		{ Constants.SocialLibrary, Namespaces.Social },
		{ Constants.AVKitLibrary, Namespaces.AVKit },
		{ Constants.VideoToolboxLibrary, Namespaces.VideoToolbox },
		{ Constants.GameplayKitLibrary, Namespaces.GameplayKit },
		{ Constants.NetworkExtensionLibrary, Namespaces.NetworkExtension },
		{ Constants.MultipeerConnectivityLibrary, Namespaces.MultipeerConnectivity },
		{ Constants.MetalKitLibrary, Namespaces.MetalKit },
		{ Constants.ModelIOLibrary, Namespaces.ModelIO },
		{ Constants.IOBluetoothLibrary, Namespaces.IOBluetooth },
		{ Constants.IOBluetoothUILibrary, Namespaces.IOBluetoothUI },
		{ Constants.FinderSyncLibrary, Namespaces.FinderSync },
		{ Constants.NotificationCenterLibrary, Namespaces.NotificationCenter },
		{ Constants.SceneKitLibrary, Namespaces.SceneKit },
		{ Constants.StoreKitLibrary, Namespaces.StoreKit },
		{ Constants.MediaPlayerLibrary, Namespaces.MediaPlayer },
		{ Constants.IntentsLibrary, Namespaces.Intents },
		{ Constants.PhotosLibrary, Namespaces.Photos },
		{ Constants.PhotosUILibrary, Namespaces.PhotosUI },
		{ Constants.PrintCoreLibrary, Namespaces.PrintCore },
		{ Constants.CoreMLLibrary, Namespaces.CoreML },
		{ Constants.VisionLibrary, Namespaces.Vision },
		{ Constants.IOSurfaceLibrary, Namespaces.IOSurface },
		{ Constants.ExternalAccessoryLibrary, Namespaces.ExternalAccessory },
		{ Constants.MetalPerformanceShadersLibrary, Namespaces.MetalPerformanceShaders },
		{ Constants.AdSupportLibrary, Namespaces.AdSupport },
		{ Constants.NaturalLanguageLibrary, Namespaces.NaturalLanguage},
		{ Constants.NetworkLibrary, Namespaces.Network},
		{ Constants.VideoSubscriberAccountLibrary, Namespaces.VideoSubscriberAccount },
	};

		public void Process (LinkContext context)
		{
			var profile = (Profile.Current as BaseProfile);

			AssemblyDefinition assembly;
			if (!context.TryGetLinkedAssembly (profile.ProductAssembly, out assembly))
				return;

			HashSet<string> namespaces = new HashSet<string> ();
			foreach (TypeDefinition type in assembly.MainModule.Types) {
				namespaces.Add (type.Namespace);
			}

			// clean NSObject from loading them

			var nsobject = assembly.MainModule.GetType (Namespaces.Foundation + ".NSObject");
			var nsobject_cctor = nsobject.GetTypeConstructor ();

			var instructions = nsobject_cctor.Body.Instructions;
			for (int i = 0; i < instructions.Count; i++) {
				Instruction ins = instructions [i];
				if (ins.OpCode.Code != Code.Ldstr)
					continue;

				// To be safe we only remove the ones we know about *and*
				// only when we know the namespace is not being used by the app
				// Based on the list from xamcore/src/Foundation/NSObjectMac.cs
				bool remove_dlopen = false;

				string targetNamespace;
				if (NamespaceMapping.TryGetValue (ins.Operand as string, out targetNamespace)) {
					remove_dlopen = !namespaces.Contains (targetNamespace);
				}
#if DEBUG
				else {
					string libname = ins.Operand as string;
					if (libname.StartsWith ("/", StringComparison.Ordinal))
						Console.WriteLine ("Unprocessed library / namespace {0}", libname);
				}
#endif

				if (remove_dlopen) {
					FieldDefinition f = Nop (ins);
					if (f != null) {
						i += 3;
						nsobject.Fields.Remove (f);
					}
				}
			}
		}

		FieldDefinition Nop (Instruction ins)
		{
#if DEBUG
			if (ins.OpCode.Code != Code.Ldstr)
				throw new InvalidOperationException ("Expected Ldstr");
#endif
			// ldstr
			ins.OpCode = OpCodes.Nop;
			ins = ins.Next;
#if DEBUG
			if (ins.OpCode.Code != Code.Ldc_I4_1)
				throw new InvalidOperationException ("Expected Ldc_I4_1");
#endif
			// ldc.i4.1
			ins.OpCode = OpCodes.Nop;
			ins = ins.Next;
#if DEBUG
			if (ins.OpCode.Code != Code.Call)
				throw new InvalidOperationException ("Expected Call");
#endif
			// call Dlfcn::dlopen (string, int)
			ins.OpCode = OpCodes.Nop;
			ins = ins.Next;
#if DEBUG
			if (ins.OpCode.Code != Code.Stsfld)
				throw new InvalidOperationException ("Expected Stsfld");
#endif
			// sfsfld
			ins.OpCode = OpCodes.Nop;
			return ins.Operand as FieldDefinition;
		}
	}
}