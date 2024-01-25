// Copyright 2011-2013 Xamarin Inc., All rights reserved.
// adapted from MonoTouchMarkStep.cs, itself
// adapted from xtouch/tools/mtouch/Touch.Tuner/ManualMarkStep.cs

using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Linker;
using Mono.Tuner;

using Registrar;

namespace Xamarin.Linker.Steps {

	// Generated backend fields inside <product>.dll are also removed if only used (i.e. set to null)
	// inside the Dispose method
	public abstract class CoreMarkStep : MobileMarkStep {

		readonly List<MethodDefinition> dispose_methods = new List<MethodDefinition> ();

		protected string ProductAssembly { get; private set; }

		bool RegisterProtocols {
			get { return LinkContext.App.Optimizations.RegisterProtocols == true; }
		}

		public override void Process (LinkContext context)
		{
			ProductAssembly = (Profile.Current as BaseProfile).ProductAssembly;

			base.Process (context);

			// adjust IL inside generated Dispose methods (overriding NSObject)
			// to remove references to fields that were not marked
			TypeDefinition nsobject = GetType (ProductAssembly, Namespaces.Foundation + ".NSObject");

			if (nsobject is not null) {
				foreach (MethodDefinition d in nsobject.Methods) {
					if (d.Name == "Dispose" && d.HasParameters) {
						ProcessDispose (d, d);
						break;
					}
				}
			}

			dispose_methods.Clear ();
		}

		void ProcessDispose (MethodDefinition bd, MethodDefinition cd)
		{
			bool skip = false;
			// did we detect some fields that could be removed ?
			if (dispose_methods.Contains (cd)) {
				// removed unmarked fields
				skip = FilterDispose (cd);
				// return value tells us if the Dispose method is now "empty" and could be skipped to the next
				// (non-empty) base.Dispose
				if (skip) {
					// if it does nothing then it should not be part of the final binary
					//cd.DeclaringType.Methods.Remove (cd);
				}
			}

			var overrides = Annotations.GetOverrides (cd);
			if (overrides is null)
				return;

			// every subclass-Dispose should be calling base-Dispose
			foreach (var overrideInfo in overrides) {
				var od = overrideInfo.Override;
				// we do not need to process unmarked code (it won't be part of the final binary)
				if (!Annotations.IsMarked (od))
					continue;
				// we do NOT process non-generated code - we could break user code
				if (!od.IsOptimizableCode (LinkContext))
					continue;

				ProcessDispose (skip ? bd : cd, od);

				// check if we need to replace the base.Dipose call
				if (bd == cd)
					continue;

				// replace "base.Dispose". In C# this always looks fine - but in IL it's the base type that's
				// used (and needs to change to remove the "empty" Dispose methods)
				foreach (Instruction ins in od.Body.Instructions) {
					if (ins.OpCode.Code != Code.Call)
						continue;

					// we can cross the assembly borders and the Dispose method might not be
					// part of the existing member references so we import it in such cases
					if (od.Module != bd.Module)
						ins.Operand = od.Module.ImportReference (bd);
					else
						ins.Operand = bd;
					break;
				}
			}
		}

		bool FilterDispose (MethodDefinition m)
		{
#if DEBUG
			var sp = m.DebugInformation.GetSequencePoint (m.Body.Instructions [0]);
			if (sp is not null) {
				string source = sp.Document.Url;
				if (!source.EndsWith (".g.cs", StringComparison.Ordinal))
					throw new InvalidProgramException (String.Format ("Attempt at modifying non-generated code for {0} : {1}", m, source));
			}
#endif
			bool remove_all_fields = true;
			var il = m.Body.Instructions;
			for (int i = il.Count - 1; i >= 0; i--) {
				Instruction ins = il [i];
				if (ins.OpCode.Code != Code.Stfld)
					continue;
				// if the field is not marked elsewhere (since we skipped Dispose)
				if (!Annotations.IsMarked (ins.Operand as FieldReference)) {
					// remove stfld, the previous ldnull and ldarg.0 instructions
					ins.OpCode = OpCodes.Nop;
					ins.Operand = null;
					ins = ins.Previous;
#if DEBUG
					if (ins.OpCode.Code != Code.Ldnull)
						throw new InvalidProgramException (String.Format ("Attempt at modifying wrong code pattern for {0}", m));
#endif
					ins.OpCode = OpCodes.Nop;
					ins.Operand = null;
					ins = ins.Previous;
#if DEBUG
					if (ins.OpCode.Code != Code.Ldarg_0)
						throw new InvalidProgramException (String.Format ("Attempt at modifying wrong code pattern for {0}", m));
#endif
					ins.OpCode = OpCodes.Nop;
					ins.Operand = null;
					i -= 2;
				} else
					remove_all_fields = false;
			}
			return remove_all_fields;
		}

		bool processing_generated_dispose;
		int skipped_fields;

		// looking for 'protected override void Dispose (bool disposing)' with generated code
		bool IsGeneratedDispose (MethodDefinition method)
		{
			if (!method.IsFamily || !method.IsVirtual || method.IsNewSlot || !method.HasParameters || !method.HasBody)
				return false;
			return ((method.Name == "Dispose") && method.IsOptimizableCode (LinkContext));
		}

		protected override TypeDefinition MarkType (TypeReference reference)
		{
			try {
				var td = base.MarkType (reference);
				if (td is null)
					return null;

				// We're removing the Protocol attribute, which points to its wrapper type.
				// But we need the wrapper type if the protocol interface is marked, so manually mark it.
				if (td.IsInterface) {
					var proto = LinkContext.StaticRegistrar.GetProtocolAttribute (td);
					if (proto?.WrapperType is not null)
						MarkType (proto.WrapperType);
				}

				// older generated bindings did not preserve the `Handler` field and
				// newer (mono 2019-02) linker can optimize them (enabled by default)
				// so we make sure our old bindings remains linker-safe
				if (td.IsAbstract && td.IsSealed && td.IsNested && td.HasFields) {
					var dt = td.DeclaringType;
					if (dt.Is ("ObjCRuntime", "Trampolines")) {
						var f = td.Fields [0];
						if (f.IsInitOnly && td.Fields.Count == 1 && f.Name == "Handler")
							MarkField (f);
					}
				}

				return td;
			} catch (Exception e) {
				// we need a way to know where (not just what) went wrong (e.g. debugging symbols being incorrect)
				e.Data ["TypeReference"] = reference.ToString ();
				e.Data ["AssemblyDefinition"] = reference.Module.Assembly.ToString ();
				throw;
			}
		}

		protected override bool AlwaysMarkTypeAsInstantiated (TypeDefinition td)
		{
			// if we mark a type then it should *always* keep it's `INativeObject` implementation
			// since it's required at build time (e.g. static registrar) and at runtime - even if the instantiation is not marked
			// as we have some special cases, e.g. `MTAudioProcessingTap.FromHandle`
			// ref: https://github.com/xamarin/xamarin-macios/issues/6711
			if (td.HasInterfaces && td.Implements ("ObjCRuntime.INativeObject"))
				return true;
			return base.AlwaysMarkTypeAsInstantiated (td);
		}

		protected override void ProcessMethod (MethodDefinition method)
		{
			// check for generated Dispose methods inside monotouch.dll
			processing_generated_dispose = IsGeneratedDispose (method);
			int skip = skipped_fields;
			try {
				base.ProcessMethod (method);
			} catch (Exception e) {
				// we need a way to know where (not just what) went wrong (e.g. debugging symbols being incorrect)
				e.Data ["MethodDefinition"] = method.ToString ();
				e.Data ["AssemblyDefinition"] = method.DeclaringType.Module.Assembly.ToString ();
				throw;
			}
			if (processing_generated_dispose) {
				// if some fields were skipped (i.e. only used inside Dispose)
				if (skip < skipped_fields)
					dispose_methods.Add (method);
				processing_generated_dispose = false;
			}
		}

		bool SkipField (FieldDefinition f)
		{
			if (f is null)
				return false;
			if (f.Name.StartsWith ("__mt_", StringComparison.Ordinal)) {
				skipped_fields++;
#if DEBUG
				Console.WriteLine ("SkipField {0}::{1} ({2})", f.DeclaringType.FullName, f.Name, skipped_fields);
#endif
				return true;
			}
			return false;
		}

		// we don't want to mark fields are only used in generated Dispose
		protected override void MarkInstruction (Instruction instruction)
		{
			if (processing_generated_dispose) {
				switch (instruction.OpCode.OperandType) {
				case OperandType.InlineField:
				case OperandType.InlineTok:
					if (SkipField (instruction.Operand as FieldDefinition))
						return;
					break;
				}
			}
			base.MarkInstruction (instruction);
		}

		protected override MethodDefinition MarkMethod (MethodReference reference)
		{
			var method = base.MarkMethod (reference);
			if (method is null)
				return null;

			var t = method.DeclaringType;

			// We have special processing that prevents protocol interfaces from being marked if they're
			// only used by being implemented by a class, but the linker will not mark interfaces if a method implemented by an interface
			// is marked: this means we need special processing to preserve a protocol interface whose methods have been implemented.
			if (RegisterProtocols && t.HasInterfaces && method.IsVirtual) {
				foreach (var r in t.Interfaces) {
					var i = r.InterfaceType.Resolve ();
					if (i is null)
						continue;
					if (Annotations.IsMarked (i))
						continue;
					if (!LinkContext.StaticRegistrar.HasAttribute (i, Namespaces.Foundation, "ProtocolAttribute"))
						continue;

					var isProtocolImplementation = false;
					// Are there any explicit overrides?
					foreach (var @override in method.Overrides) {
						if (!i.Methods.Contains (@override.Resolve ()))
							continue;
						isProtocolImplementation = true;
						break;
					}
					if (!isProtocolImplementation) {
						// Are there any implicit overrides (identical name and signature)?
						foreach (var imethod in i.Methods) {
							if (!StaticRegistrar.MethodMatch (imethod, method))
								continue;
							isProtocolImplementation = true;
							break;
						}

					}
					if (isProtocolImplementation) {
						MarkType (r.InterfaceType);
						Bundler.Driver.Log (9, "Marking {0} because the method {1} implements one of its methods.", r.InterfaceType, method.FullName);
					}
				}
			}

			// special processing to find [BlockProxy] attributes in _Extensions types
			// ref: https://bugzilla.xamarin.com/show_bug.cgi?id=23540
			if (LinkContext.Target.StaticRegistrar.MapProtocolMember (method, out var extensionMethod)) {
				// one cannot simply mark the `ca.ConstructorArguments [0].Value` type,
				// e.g. Trampolines.NIDActionArity1V26
				// as the relation to *_Extensions will be reflected at runtime
				MarkMethod (extensionMethod);
			}

			return method;
		}

		protected override bool ShouldMarkInterfaceImplementation (TypeDefinition type, InterfaceImplementation iface, TypeDefinition resolvedInterfaceType)
		{
			if (RegisterProtocols) {
				// If we're registering protocols, we can remove interfaces that represent protocols.
				// The linker will automatically mark interfaces a class implements, but we have to
				// override the linker behavior for interfaces that represent protocols for those 
				// interfaces to be removed.

				var isProtocol = type.IsNSObject (LinkContext) && resolvedInterfaceType.HasCustomAttribute (LinkContext, Namespaces.Foundation, "ProtocolAttribute");

				// We're not linking the current assembly, which means the interface should be marked.
				if (isProtocol && !IgnoreScope (type.Scope)) {
					LinkContext.StoreProtocolMethods (resolvedInterfaceType);
				}
			} else if (LinkContext.App.Registrar == Bundler.RegistrarMode.Dynamic) {
				// If we're using the dynamic registrar, we need to mark interfaces that represent protocols
				// even if it doesn't look like the interfaces are used, since we need them at runtime.
				var isProtocol = type.IsNSObject (LinkContext) && resolvedInterfaceType.HasCustomAttribute (LinkContext, Namespaces.Foundation, "ProtocolAttribute");
				if (isProtocol) {
					// return true only if not already marked (same check as `base` would do)
					// otherwise we can enqueue something everytime and never get an empty queue
					return !Annotations.IsMarked (iface);
				}
			}

			return base.ShouldMarkInterfaceImplementation (type, iface, resolvedInterfaceType);
		}
	}
}
