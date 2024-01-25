// Copyright 2012-2013 Xamarin Inc. All rights reserved.
//#define DEBUG
using System;
using System.Collections.Generic;
using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Linker;
using Mono.Linker.Steps;
using Mono.Tuner;

using Xamarin.Linker;

using Xamarin.Tuner;

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

			// Compute our map
			// there can be multiple namespaces for the same library
			var frameworks = Frameworks.GetFrameworks (((DerivedLinkContext) context).App.Platform, false);
			var map = new Dictionary<string, List<string>> ();
			foreach (var fw in frameworks.Values) {
				if (!map.TryGetValue (fw.LibraryPath, out var list))
					map [fw.LibraryPath] = list = new List<string> ();
				list.Add (fw.Namespace);
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

				if (map.TryGetValue (ins.Operand as string, out var targetNamespaces)) {
					remove_dlopen = !targetNamespaces.Any (v => namespaces.Contains (v));
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
					if (f is not null) {
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
