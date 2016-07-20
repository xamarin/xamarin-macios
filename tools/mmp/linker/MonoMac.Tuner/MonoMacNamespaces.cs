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
				switch (ins.Operand as string) {
				case Constants.FoundationLibrary:
					// note: every app has Foundation - even if I could not find a sample that required it pre-loaded
					remove_dlopen = !namespaces.Contains (Namespaces.Foundation);
					break;
				case Constants.AppKitLibrary:
					// note: every app has AppKit - even if I could not find a sample that required it pre-loaded
					remove_dlopen = !namespaces.Contains (Namespaces.AppKit);
					break;
				case Constants.AddressBookLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.AddressBook);
					break;
				case Constants.CoreTextLibrary:
					// FIXME: CoreTextArcMonoMac does not need it (i.e. maybe we could always remove it)
					remove_dlopen = !namespaces.Contains (Namespaces.CoreText);
					break;
				case Constants.WebKitLibrary:
					// WhereIsMyMac sample won't work without it
					remove_dlopen = !namespaces.Contains (Namespaces.WebKit);
					break;
				case Constants.QuartzLibrary:
					// FIXME: AnimatedClock does not need it (i.e. maybe we could always remove it)
					remove_dlopen = !namespaces.Contains (Namespaces.CoreAnimation);
					break;
				case Constants.QTKitLibrary:
					// QTRecorder sample won't work without it
					// StillMotion sample won't work without it
					remove_dlopen = !namespaces.Contains (Namespaces.QTKit);
					break;
				case Constants.CoreLocationLibrary:
					// WhereIsMyMac sample won't work without it
					remove_dlopen = !namespaces.Contains (Namespaces.CoreLocation);
					break;
				case Constants.SecurityLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.Security);
					break;
				case Constants.QuartzComposerLibrary:
					// CAQuartzComposition sample won't work without it
					remove_dlopen = !namespaces.Contains (Namespaces.QuartzComposer);
					break;
				case Constants.CoreWlanLibrary:
					// CoreWLANWirelessManager sample won't work without it
					remove_dlopen = !namespaces.Contains (Namespaces.CoreWlan);
					break;
				case Constants.PdfKitLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.PdfKit);
					break;
				case Constants.ImageKitLibrary:
					// ImageKitDemo sample won't work without it
					remove_dlopen = !namespaces.Contains (Namespaces.ImageKit);
					break;
				case Constants.ScriptingBridgeLibrary:
					// no working (attic) sample
					remove_dlopen = !namespaces.Contains (Namespaces.ScriptingBridge);
					break;
				case Constants.AVFoundationLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.AVFoundation);
					break;
				case Constants.CoreBluetoothLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.CoreBluetooth);
					break;
				case Constants.GameKitLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.GameKit);
					break;
//				case Constants.GameControllerLibrary:
					// no sample
//					remove_dlopen = !namespaces.Contains (Namespaces.GameController);
//					break;
				case Constants.JavaScriptCoreLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.JavaScriptCore);
					break;
				case Constants.CoreAudioKitLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.CoreAudioKit);
					break;
				case Constants.InputMethodKitLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.InputMethodKit);
					break;
				case Constants.OpenALLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.OpenAL);
					break;
				case Constants.MediaAccessibilityLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.MediaAccessibility);
					break;
				case Constants.CoreMidiLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.CoreMIDI);
					break;
				case Constants.MediaLibraryLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.MediaLibrary);
					break;
				case Constants.GLKitLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.GLKit);
					break;
				case Constants.SpriteKitLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.SpriteKit);
					break;
				case Constants.CloudKitLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.CloudKit);
					break;
				case Constants.LocalAuthenticationLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.LocalAuthentication);
					break;
				case Constants.AccountsLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.Accounts);
					break;
				case Constants.ContactsLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.Contacts);
					break;
				case Constants.ContactsUILibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.ContactsUI);
					break;
				case Constants.MapKitLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.MapKit);
					break;
				case Constants.EventKitLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.EventKit);
					break;
				case Constants.SocialLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.Social);
					break;
				case Constants.AVKitLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.AVKit);
					break;
				case Constants.VideoToolboxLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.VideoToolbox);
					break;
				case Constants.GameplayKitLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.GameplayKit);
					break;
				case Constants.NetworkExtensionLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.NetworkExtension);
					break;
				case Constants.MultipeerConnectivityLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.MultipeerConnectivity);
					break;
				case Constants.MetalKitLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.MetalKit);
					break;
				case Constants.ModelIOLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.ModelIO);
					break;
				case Constants.IOBluetoothLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.IOBluetooth);
					break;
				case Constants.IOBluetoothUILibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.IOBluetoothUI);
					break;
				case Constants.FinderSyncLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.FinderSync);
					break;
				case Constants.NotificationCenterLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.NotificationCenter);
					break;
				case Constants.SceneKitLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.SceneKit);
					break;
				case Constants.StoreKitLibrary:
					// no sample
					remove_dlopen = !namespaces.Contains (Namespaces.StoreKit);
					break;
#if DEBUG
				default:
					string libname = ins.Operand as string;
					if (libname.StartsWith ("/", StringComparison.Ordinal))
						Console.WriteLine ("Unprocessed library / namespace {0}", libname);
					break;
#endif
				}

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