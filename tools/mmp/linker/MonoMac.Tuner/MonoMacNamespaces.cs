// Copyright 2012-2013 Xamarin Inc. All rights reserved.
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