using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using Xamarin.Bundler;
using Xamarin.Utils;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Linker;
using Mono.Tuner;

using ObjCRuntime;
using Registrar;
using System.Globalization;

#nullable enable

namespace Xamarin.Linker {
	// Class to contain (trampoline) info for every assembly in the app bundle
	public class AssemblyTrampolineInfos : Dictionary<AssemblyDefinition, AssemblyTrampolineInfo> {
		Dictionary<MethodDefinition, TrampolineInfo>? map;
		public bool TryFindInfo (MethodDefinition method, [NotNullWhen (true)] out TrampolineInfo? info)
		{
			if (map is null) {
				map = new Dictionary<MethodDefinition, TrampolineInfo> ();
				foreach (var kvp in this) {
					foreach (var ai in kvp.Value) {
						map.Add (ai.Target, ai);
					}
				}
			}
			return map.TryGetValue (method, out info);
		}
	}

	// Class to contain all the trampoline infos for an assembly
	// Also between a type and its ID.
	public class AssemblyTrampolineInfo : List<TrampolineInfo> {
		Dictionary<TypeDefinition, uint> registered_type_map = new ();

		public TypeDefinition? RegistrarType;

		public void RegisterType (TypeDefinition td, uint id)
		{
			registered_type_map.Add (td, id);
		}

		public bool TryGetRegisteredTypeIndex (TypeDefinition td, out uint id)
		{
			return registered_type_map.TryGetValue (td, out id);
		}

		public void SetIds ()
		{
			for (var i = 0; i < Count; i++)
				this [i].Id = i;
		}
	}

	// Class to contain info for each exported method, with its UCO trampoline.
	public class TrampolineInfo {
		public MethodDefinition Trampoline;
		public MethodDefinition Target;
		public string UnmanagedCallersOnlyEntryPoint;
		public int Id;

		public TrampolineInfo (MethodDefinition trampoline, MethodDefinition target, string entryPoint)
		{
			this.Trampoline = trampoline;
			this.Target = target;
			this.UnmanagedCallersOnlyEntryPoint = entryPoint;
			this.Id = -1;
		}
	}

	public class ManagedRegistrarStep : ConfigurationAwareStep {
		protected override string Name { get; } = "ManagedRegistrar";
		protected override int ErrorCode { get; } = 2430;

		AppBundleRewriter abr { get { return Configuration.AppBundleRewriter; } }
		List<Exception> exceptions = new List<Exception> ();

		void AddException (Exception exception)
		{
			if (exceptions is null)
				exceptions = new List<Exception> ();
			exceptions.Add (exception);
		}

		protected override void TryProcess ()
		{
			base.TryProcess ();

			App.SelectRegistrar ();
			if (App.Registrar != RegistrarMode.ManagedStatic)
				return;

			Configuration.Target.StaticRegistrar.Register (Configuration.GetNonDeletedAssemblies (this));
		}

		protected override void TryEndProcess (out List<Exception>? exceptions)
		{
			base.TryEndProcess ();

			if (App.Registrar != RegistrarMode.ManagedStatic) {
				exceptions = null;
				return;
			}

			// Report back any exceptions that occurred during the processing.
			exceptions = this.exceptions;

			// Mark some stuff we use later on.
			abr.SetCurrentAssembly (abr.PlatformAssembly);
			Annotations.Mark (abr.RegistrarHelper_Register.Resolve ());
			abr.ClearCurrentAssembly ();
		}

		protected override void TryProcessAssembly (AssemblyDefinition assembly)
		{
			base.TryProcessAssembly (assembly);

			if (App.Registrar != RegistrarMode.ManagedStatic)
				return;

			if (Annotations.GetAction (assembly) == AssemblyAction.Delete)
				return;

			// No SDK assemblies will have anything we need to register
			if (Configuration.Profile.IsSdkAssembly (assembly))
				return;

			if (!assembly.MainModule.HasAssemblyReferences)
				return;

			// In fact, unless an assembly references our platform assembly, then it won't have anything we need to register
			if (!Configuration.Profile.IsProductAssembly (assembly) && !assembly.MainModule.AssemblyReferences.Any (v => Configuration.Profile.IsProductAssembly (v.Name)))
				return;

			if (!assembly.MainModule.HasTypes)
				return;

			abr.SetCurrentAssembly (assembly);

			var current_trampoline_lists = new AssemblyTrampolineInfo ();
			Configuration.AssemblyTrampolineInfos [assembly] = current_trampoline_lists;
			var proxyInterfaces = new List<TypeDefinition> ();

			var modified = false;
			foreach (var type in assembly.MainModule.Types)
				modified |= ProcessType (type, current_trampoline_lists, proxyInterfaces);

			foreach (var additionalType in proxyInterfaces)
				assembly.MainModule.Types.Add (additionalType);

			// Make sure the linker saves any changes in the assembly.
			DerivedLinkContext.Annotations.SetCustomAnnotation ("ManagedRegistrarStep", assembly, current_trampoline_lists);
			if (modified)
				abr.SaveCurrentAssembly ();

			// TODO: Move this to a separate "MakeEverythingWorkWithNativeAOTStep" linker step
			if (App.XamarinRuntime == XamarinRuntime.NativeAOT && Configuration.Profile.IsProductAssembly (assembly)) {
				ImplementNSObjectRegisterToggleRefMethodStub ();
			}

			abr.ClearCurrentAssembly ();
		}

		bool ProcessType (TypeDefinition type, AssemblyTrampolineInfo infos, List<TypeDefinition> proxyInterfaces)
		{
			var modified = false;
			if (type.HasNestedTypes) {
				foreach (var nested in type.NestedTypes)
					modified |= ProcessType (nested, infos, proxyInterfaces);
			}

			// Figure out if there are any types we need to process
			var process = false;

			process |= IsNSObject (type);
			process |= StaticRegistrar.GetCategoryAttribute (type) is not null;

			var registerAttribute = StaticRegistrar.GetRegisterAttribute (type);
			if (registerAttribute is not null && registerAttribute.IsWrapper)
				return modified;

			if (!process)
				return modified;

			// Figure out if there are any methods we need to process
			var methods_to_wrap = new HashSet<MethodDefinition> ();
			if (type.HasMethods) {
				foreach (var method in type.Methods)
					ProcessMethod (method, methods_to_wrap);
			}

			if (type.HasProperties) {
				foreach (var prop in type.Properties) {
					ProcessProperty (prop, methods_to_wrap);
				}
			}

			// Create an UnmanagedCallersOnly method for each method we need to wrap
			foreach (var method in methods_to_wrap) {
				try {
					CreateUnmanagedCallersMethod (method, infos, proxyInterfaces);
				} catch (Exception e) {
					AddException (ErrorHelper.CreateError (99, e, "Failed to create an UnmanagedCallersOnly trampoline for {0}: {1}", method.FullName, e.Message));
				}
			}

			return true;
		}

		void ProcessMethod (MethodDefinition method, HashSet<MethodDefinition> methods_to_wrap)
		{
			if (!(method.IsConstructor && !method.IsStatic)) {
				var ea = StaticRegistrar.GetExportAttribute (method);
				if (ea is null && !method.IsVirtual)
					return;
			}

			if (!StaticRegistrar.TryFindMethod (method, out _)) {
				// If the registrar doesn't know about a method, then we don't need to generate an UnmanagedCallersOnly trampoline for it
				return;
			}

			methods_to_wrap.Add (method);
		}

		void ProcessProperty (PropertyDefinition property, HashSet<MethodDefinition> methods_to_wrap)
		{
			var ea = StaticRegistrar.GetExportAttribute (property);
			if (ea is null)
				return;

			if (property.GetMethod is not null)
				methods_to_wrap.Add (property.GetMethod);

			if (property.SetMethod is not null)
				methods_to_wrap.Add (property.SetMethod);
		}

		static string Sanitize (string str)
		{
			str = str.Replace ('.', '_');
			str = str.Replace ('/', '_');
			str = str.Replace ('`', '_');
			str = str.Replace ('<', '_');
			str = str.Replace ('>', '_');
			str = str.Replace ('$', '_');
			str = str.Replace ('@', '_');
			str = StaticRegistrar.EncodeNonAsciiCharacters (str);
			str = str.Replace ('\\', '_');
			return str;
		}

		// Set the XAMARIN_MSR_TRACE environment variable at build time to inject tracing statements.
		// Note that the tracing is quite basic, because we don't want to add a unique string to
		// each method we emit, because there's a fairly low limit in the IL file format for constant
		// strings - around 4mb IIRC - so we're emitting a call to a method that will do most of the
		// heavy work.
		// Note that Cecil doesn't complain if a file has too many string constants, it will happily
		// emit garbage and really weird things start happening at runtime.
		bool? trace;
		void Trace (ILProcessor il, string message)
		{
			if (!trace.HasValue)
				trace = !string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("XAMARIN_MSR_TRACE"));
			if (trace.Value) {
				il.Emit (OpCodes.Ldstr, message);
				il.Emit (OpCodes.Call, abr.Runtime_TraceCaller);
			}
		}

		int counter;
		void CreateUnmanagedCallersMethod (MethodDefinition method, AssemblyTrampolineInfo infos, List<TypeDefinition> proxyInterfaces)
		{
			var baseMethod = StaticRegistrar.GetBaseMethodInTypeHierarchy (method);
			var placeholderType = abr.System_IntPtr;
			var name = $"callback_{counter++}_{Sanitize (method.DeclaringType.FullName)}_{Sanitize (method.Name)}";

			var callbackType = method.DeclaringType.NestedTypes.SingleOrDefault (v => v.Name == "__Registrar_Callbacks__");
			if (callbackType is null) {
				callbackType = new TypeDefinition (string.Empty, "__Registrar_Callbacks__", TypeAttributes.NestedPrivate | TypeAttributes.Sealed | TypeAttributes.Class);
				callbackType.BaseType = abr.System_Object;
				method.DeclaringType.NestedTypes.Add (callbackType);
			}

			var callback = callbackType.AddMethod (name, MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig, placeholderType);
			callback.CustomAttributes.Add (CreateUnmanagedCallersAttribute (name));
			infos.Add (new TrampolineInfo (callback, method, name));

			// If the target method is marked, then we must mark the trampoline as well.
			method.CustomAttributes.Add (abr.CreateDynamicDependencyAttribute (callback.Name, callbackType));

			callback.AddParameter ("pobj", abr.System_IntPtr);

			var isGeneric = method.DeclaringType.HasGenericParameters;
			if (isGeneric && method.IsStatic) {
				throw ErrorHelper.CreateError (4130 /* The registrar cannot export static methods in generic classes ('{0}'). */, method.FullName);
			} else if (isGeneric && !method.IsConstructor) {
				// We generate a proxy interface for each generic NSObject subclass. In the static UnmanagedCallersOnly methods we don't
				// know the generic parameters of the type we're working with and we need to use this trick to be able to call methods on the
				// generic type without using reflection. This is an example of the code we generate in addition to user code:
				//
				//
				// internal interface __IRegistrarGenericTypeProxy__CustomNSObject_1__
				// {
				//     void __IRegistrarGenericTypeProxy__CustomNSObject_1____SomeMethod (IntPtr p0);
				// }
				//
				// public class CustomNSObject<T> : NSObject, __IRegistrarGenericTypeProxy__CustomNSObject_1__
				//     where T : NSObject
				// {
				//     [Export ("someMethod:")]
				//     public void SomeMethod (T someInput)
				//     {
				//         // ...
				//     }
				//
				//     // generated implementation of the proxy interface:
				//     public void __IRegistrarGenericTypeProxy__CustomNSObject_1____SomeMethod (IntPtr sel, IntPtr p0, IntPtr* exception_gchandle)
				//     {
				//         try {
				//             var obj0 = Runtime.GetNSObject<T> (p0);
				//             SomeMethod (obj0);
				//         } catch (Exception ex) {
				//             *exception_gchandle = Runtime.AllocGCHandle (ex);
				//         }
				//     }
				//
				//     // generated registrar callbacks:
				//     private static class __Registrar_Callbacks__
				//     {
				//         [UnmanagedCallersOnly (EntryPoint = "_callback_1_CustomNSObject_1_SomeMethod")]
				//         public unsafe static void callback_1_CustomNSObject_1_SomeMethod (IntPtr pobj, IntPtr sel, IntPtr p0, IntPtr* exception_gchandle)
				//         {
				//             var proxy = (__IRegistrarGenericTypeProxy__CustomNSObject_1__)Runtime.GetNSObject (pobj);
				//             proxy.__IRegistrarGenericTypeProxy__CustomNSObject_1____SomeMethod (sel, p0, exception_gchandle);
				//         }
				//     }
				// }

				var proxyInterfaceName = $"__IRegistrarGenericTypeProxy__{Sanitize (method.DeclaringType.FullName)}__";
				TypeDefinition? proxyInterface = proxyInterfaces.SingleOrDefault (v => v.Name == proxyInterfaceName && v.Namespace == "ObjCRuntime");
				if (proxyInterface is null) {
					proxyInterface = new TypeDefinition ("ObjCRuntime", proxyInterfaceName, TypeAttributes.NotPublic | TypeAttributes.Interface | TypeAttributes.Abstract);
					method.DeclaringType.Interfaces.Add (new InterfaceImplementation (proxyInterface));
					proxyInterfaces.Add (proxyInterface);
				}

				var methodName = $"{proxyInterfaceName}_{method.Name}";
				var interfaceMethod = proxyInterface.AddMethod (methodName, MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Abstract | MethodAttributes.Virtual, placeholderType);
				var implementationMethod = method.DeclaringType.AddMethod (methodName, MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.Final, placeholderType);

				// the callback will only call the proxy method and the proxy method will perform all the conversions
				EmitCallToExportedMethod (method, implementationMethod);

				// now copy the return type and params (incl. sel and exception_gchandle) to the UCO itself
				// and also to the proxy interface 
				callback.ReturnType = implementationMethod.ReturnType;
				interfaceMethod.ReturnType = implementationMethod.ReturnType;

				foreach (var parameter in implementationMethod.Parameters) {
					callback.AddParameter (parameter.Name, parameter.ParameterType);
					interfaceMethod.AddParameter (parameter.Name, parameter.ParameterType);
				}

				// we need to wait until we know all the parameters of the interface method before we generate this method
				EmitCallToProxyMethod (method, callback, interfaceMethod);
			} else {
				EmitCallToExportedMethod (method, callback);
			}
		}

		public void EmitCallToProxyMethod (MethodDefinition method, MethodDefinition callback, MethodDefinition proxyInterfaceMethod)
		{
			_ = callback.CreateBody (out var il);

			// We don't know the generic parameters of the type we're working with but we know it is a NSObject and it
			// implements the proxy interface. The generic parameters will be resolved in the proxy method through the v-table.
			il.Emit (OpCodes.Ldarg_0);
			il.Emit (OpCodes.Call, abr.Runtime_GetNSObject__System_IntPtr);
			il.Emit (OpCodes.Castclass, proxyInterfaceMethod.DeclaringType);

			if (callback.HasParameters) {
				// skip the first argument (the handle of the object)
				foreach (var parameter in callback.Parameters.Skip (1)) {
					il.Emit (OpCodes.Ldarg, parameter);
				}
			}

			il.Emit (OpCodes.Callvirt, proxyInterfaceMethod);
			il.Emit (OpCodes.Ret);
		}

		public void EmitCallToExportedMethod (MethodDefinition method, MethodDefinition callback)
		{
			var baseMethod = StaticRegistrar.GetBaseMethodInTypeHierarchy (method);
			var placeholderType = abr.System_IntPtr;
			ParameterDefinition? callSuperParameter = null;
			VariableDefinition? returnVariable = null;
			MethodReference? ctor = null;
			var leaveTryInstructions = new List<Instruction> ();
			var isVoid = method.ReturnType.Is ("System", "Void");

			var body = callback.CreateBody (out var il);
			var placeholderInstruction = il.Create (OpCodes.Nop);
			var placeholderNextInstruction = il.Create (OpCodes.Nop);
			var postProcessing = new List<Instruction> ();
			var categoryAttribute = StaticRegistrar.GetCategoryAttribute (method.DeclaringType);
			var isCategory = categoryAttribute is not null;
			var isInstanceCategory = isCategory && StaticRegistrar.HasThisAttribute (method);
			var isGeneric = method.DeclaringType.HasGenericParameters;

			Trace (il, $"ENTER");

			if (!isVoid || method.IsConstructor)
				returnVariable = body.AddVariable (placeholderType);

			// Our code emission is intermingled with creating the method signature (the code to convert between native and managed values is also the best location
			// to determine exactly which are the corresponding native and managed types in the method signatures). Unfortunately we might need to skip code emission
			// after a certain point (if we detected a failure scenario where we're just throwing an exception, there's no need to keep generating code for that 
			// particular scenario), but we still need to create the proper method signatures. Thus this hack: we keep emitting code, but the last important instruction
			// and later on remove everything after this instruction. Maybe at a later point I'll figure out a way to make the code emission conditional without
			// littering the logic with conditional statements.
			Instruction? skipEverythingAfter = null;

			if (isInstanceCategory) {
				il.Emit (OpCodes.Ldarg_0);
				EmitConversion (method, il, method.Parameters [0].ParameterType, true, 0, out var nativeType, postProcessing);
			} else if (method.IsStatic) {
				// nothing to do
			} else if (method.IsConstructor) {
				callSuperParameter = new ParameterDefinition ("call_super", ParameterAttributes.None, new PointerType (abr.System_Byte));
				var postLeaveInstructionPlaceholder = il.Create (OpCodes.Nop);
				// if (Runtime.HasNSObject (p0)) {
				il.Emit (OpCodes.Ldarg_0);
				il.Emit (OpCodes.Call, abr.Runtime_HasNSObject);
				il.Emit (OpCodes.Brfalse, postLeaveInstructionPlaceholder);
				var postLeaveBranch = il.Body.Instructions.Last ();
				// *call_super = 1;
				il.Emit (OpCodes.Ldarg, callSuperParameter);
				il.Emit (OpCodes.Ldc_I4_1);
				il.Emit (OpCodes.Stind_I1);
				// return rv;
				il.Emit (OpCodes.Ldarg_0);
				il.Emit (OpCodes.Call, abr.NativeObject_op_Implicit_NativeHandle);
				il.Emit (OpCodes.Stloc, returnVariable);
				il.Emit (OpCodes.Leave, placeholderInstruction);
				// }
				leaveTryInstructions.Add (il.Body.Instructions.Last ());

				if (isGeneric) {
					il.Emit (OpCodes.Ldc_I4, 4133);
					postLeaveBranch.Operand = il.Body.Instructions.Last ();
					il.Emit (OpCodes.Ldstr, $"Cannot construct an instance of the type '{method.DeclaringType.FullName}' from Objective-C because the type is generic.");
					il.Emit (OpCodes.Call, abr.Runtime_CreateRuntimeException);
					il.Emit (OpCodes.Throw);
					// We're throwing an exception, so there's no need for any more code.
					skipEverythingAfter = il.Body.Instructions.Last ();
				} else {
					// Whenever there's an NSObject constructor that we call from a registrar callback, we need to create
					// a separate constructor that will first set the `handle` and `flags` values of the NSObject before
					// calling the original constructor. Here's an example of the code we generate:
					//
					// // The original constructor:
					// public .ctor (T0 p0, T1 p1, ...) { /* ... */ }
					//
					// // The generated constructor with pre-initialization: 
					// public .ctor (T0 p0, T1 p1, ..., IntPtr nativeHandle, IManagedRegistrar dummy) {
					//     this.handle = (NativeHandle)nativeHandle;
					//     this.flags = 2; // Flags.NativeRef == 2
					//     this..ctor (p0, p1, ...);
					// }
					//
					// - This code can't be expressed in C# and it can only be expressed directly in IL.
					// - The reason we need to do this is because the base NSObject parameterless constructor 
					//   would allocate a new Objective-C object if `handle` is a zero pointer.
					// - The `IManagedRegistrar` dummy parameter is used only to make sure that the signature
					//   is unique and there aren't any conflicts. The IManagedRegistrar type is internal and
					//   we only make it public through a custom linker step.

					ctor = CloneConstructorWithNativeHandle (method);
					method.DeclaringType.Methods.Add (ctor.Resolve ());

					il.Emit (OpCodes.Nop);
					postLeaveBranch.Operand = il.Body.Instructions.Last ();
				}
			} else if (isGeneric) {
				// this is a proxy method and we can simply use `this` without any conversion
				il.Emit (OpCodes.Ldarg_0);
			} else {
				// instance method
				il.Emit (OpCodes.Ldarg_0);
				EmitConversion (method, il, method.DeclaringType, true, -1, out var nativeType, postProcessing);
			}

			callback.AddParameter ("sel", abr.System_IntPtr);

			var managedParameterCount = 0;
			var nativeParameterOffset = isInstanceCategory ? 1 : 2;
			var parameterStart = isInstanceCategory ? 1 : 0;
			if (method.HasParameters)
				managedParameterCount = method.Parameters.Count;

			if (method.HasParameters) {
				for (var p = parameterStart; p < managedParameterCount; p++) {
					var nativeParameter = callback.AddParameter ($"p{p}", placeholderType);
					var nativeParameterIndex = p + nativeParameterOffset;
					var managedParameterType = method.Parameters [p].ParameterType;
					var baseParameter = baseMethod.Parameters [p];
					var isOutParameter = IsOutParameter (method, p, baseParameter);

					if (!isOutParameter) {
						il.EmitLoadArgument (nativeParameterIndex);
					}

					if (EmitConversion (method, il, managedParameterType, true, p, out var nativeType, postProcessing, isOutParameter, nativeParameterIndex)) {
						nativeParameter.ParameterType = nativeType;
					} else {
						nativeParameter.ParameterType = placeholderType;
						AddException (ErrorHelper.CreateError (99, "Unable to emit conversion for parameter {2} of type {0}. Method: {1}", method.Parameters [p].ParameterType, GetMethodSignatureWithSourceCode (method), p));
					}
				}
			}

			if (callSuperParameter is not null)
				callback.Parameters.Add (callSuperParameter);

			callback.AddParameter ("exception_gchandle", new PointerType (abr.System_IntPtr));

			if (ctor is not null) {
				// in addition to the params of the original ctor we pass also the native handle and a null
				// value for the dummy (de-duplication) parameter
				il.Emit (OpCodes.Ldarg_0);
				il.Emit (OpCodes.Ldnull);
				il.Emit (OpCodes.Newobj, ctor);
			} else if (isGeneric && !method.IsConstructor) {
				var targetMethod = method.DeclaringType.CreateMethodReferenceOnGenericType (method, method.DeclaringType.GenericParameters.ToArray ());
				il.Emit (OpCodes.Call, targetMethod);
			} else if (method.IsStatic) {
				il.Emit (OpCodes.Call, method);
			} else {
				il.Emit (OpCodes.Callvirt, method);
			}

			if (returnVariable is not null) {
				if (EmitConversion (method, il, method.ReturnType, false, -1, out var nativeReturnType, postProcessing)) {
					returnVariable.VariableType = nativeReturnType;
					callback.ReturnType = nativeReturnType;
				} else {
					AddException (ErrorHelper.CreateError (99, "Unable to emit conversion for return value of type {0}. Method: {1}", method.ReturnType, GetMethodSignatureWithSourceCode (method)));
				}
				il.Emit (OpCodes.Stloc, returnVariable);
			} else {
				callback.ReturnType = abr.System_Void;
			}

			body.Instructions.AddRange (postProcessing);

			Trace (il, $"EXIT");

			il.Emit (OpCodes.Leave, placeholderInstruction);
			leaveTryInstructions.Add (il.Body.Instructions.Last ());

			if (skipEverythingAfter is not null) {
				var skipIndex = body.Instructions.IndexOf (skipEverythingAfter);
				for (var i = body.Instructions.Count - 1; i > skipIndex; i--)
					body.Instructions.RemoveAt (i);
			}

			AddExceptionHandler (il, returnVariable, placeholderNextInstruction, isGeneric && !method.IsConstructor, out var eh, out var leaveEHInstruction);

			// Generate code to return null/default value/void
			if (returnVariable is not null) {
				var returnType = returnVariable.VariableType!;
				if (returnType.IsValueType) {
					// return default(<struct type>)
					il.Emit (OpCodes.Ldloca, returnVariable);
					il.Emit (OpCodes.Initobj, returnType);
					il.Emit (OpCodes.Ldloc, returnVariable);
				} else {
					il.Emit (OpCodes.Ldnull);
				}
			}
			il.Emit (OpCodes.Ret);

			// Generate code to return the return value
			Instruction leaveTryInstructionOperand;
			if (returnVariable is not null) {
				il.Emit (OpCodes.Ldloc, returnVariable);
				leaveTryInstructionOperand = il.Body.Instructions.Last ();
				il.Emit (OpCodes.Ret);
			} else {
				// Here we can re-use the ret instruction from the previous block.
				leaveTryInstructionOperand = il.Body.Instructions.Last ();
			}

			// Replace any 'placeholderNextInstruction' operands with the actual next instruction.
			foreach (var instr in body.Instructions) {
				if (object.ReferenceEquals (instr.Operand, placeholderNextInstruction))
					instr.Operand = instr.Next;
			}

			foreach (var instr in leaveTryInstructions)
				instr.Operand = leaveTryInstructionOperand;
			eh.HandlerEnd = (Instruction) leaveEHInstruction.Operand;
		}

		void AddExceptionHandler (ILProcessor il, VariableDefinition? returnVariable, Instruction placeholderNextInstruction, bool isGeneric, out ExceptionHandler eh, out Instruction leaveEHInstruction)
		{
			var body = il.Body;
			var method = body.Method;

			// Exception handler
			eh = new ExceptionHandler (ExceptionHandlerType.Catch);
			eh.CatchType = abr.System_Exception;
			eh.TryStart = il.Body.Instructions [0];
			il.Body.ExceptionHandlers.Add (eh);

			var exceptionVariable = body.AddVariable (abr.System_Exception);
			il.Emit (OpCodes.Stloc, exceptionVariable);
			eh.HandlerStart = il.Body.Instructions.Last ();
			eh.TryEnd = eh.HandlerStart;
			il.Emit (OpCodes.Ldarg, isGeneric ? method.Parameters.Count : method.Parameters.Count - 1);
			il.Emit (OpCodes.Ldloc, exceptionVariable);
			il.Emit (OpCodes.Call, abr.Runtime_AllocGCHandle);
			il.Emit (OpCodes.Stind_I);
			Trace (il, $"EXCEPTION");
			il.Emit (OpCodes.Leave, placeholderNextInstruction);
			leaveEHInstruction = body.Instructions.Last ();
		}

		static string GetMethodSignature (MethodDefinition method)
		{
			return $"{method?.ReturnType?.FullName ?? "(null)"} {method?.DeclaringType?.FullName ?? "(null)"}::{method?.Name ?? "(null)"} ({string.Join (", ", method?.Parameters?.Select (v => v?.ParameterType?.FullName + " " + v?.Name) ?? Array.Empty<string> ())})";
		}

		static string GetMethodSignatureWithSourceCode (MethodDefinition method)
		{
			var rv = GetMethodSignature (method);
			if (method.HasBody && method.DebugInformation.HasSequencePoints) {
				var seq = method.DebugInformation.SequencePoints [0];
				rv += " " + seq.Document.Url + ":" + seq.StartLine.ToString () + " ";
			}
			return rv;
		}

		bool IsNSObject (TypeReference type)
		{
			if (type is ArrayType)
				return false;

			if (type is ByReferenceType)
				return false;

			if (type is PointerType)
				return false;

			if (type is GenericParameter)
				return false;

			return type.IsNSObject (DerivedLinkContext);
		}

		BindAsAttribute? GetBindAsAttribute (MethodDefinition method, int parameter)
		{
			if (StaticRegistrar.IsPropertyAccessor (method, out var property)) {
				return StaticRegistrar.GetBindAsAttribute (property);
			} else {
				return StaticRegistrar.GetBindAsAttribute (method, parameter);
			}
		}

		// This emits a conversion between the native and the managed representation of a parameter or return value,
		// and returns the corresponding native type. The returned nativeType will (must) be a blittable type.
		bool EmitConversion (MethodDefinition method, ILProcessor il, TypeReference type, bool toManaged, int parameter, [NotNullWhen (true)] out TypeReference? nativeType, List<Instruction> postProcessing, bool isOutParameter = false, int nativeParameterIndex = -1)
		{
			nativeType = null;

			if (!(parameter == -1 && !method.IsStatic && method.DeclaringType == type)) {
				var bindAsAttribute = GetBindAsAttribute (method, parameter);
				if (bindAsAttribute is not null) {
					if (toManaged) {
						GenerateConversionToManaged (method, il, bindAsAttribute.OriginalType, type, "descriptiveMethodName", parameter, out nativeType);
						return true;
					} else {
						GenerateConversionToNative (method, il, type, bindAsAttribute.OriginalType, "descriptiveMethodName", out nativeType);
						return true;
					}
				}
			}

			if (type.Is ("System", "Void")) {
				if (parameter == -1 && method.IsConstructor) {
					if (toManaged) {
						AddException (ErrorHelper.CreateError (99, "Don't know how (9) to convert ctor. Method: {0}", GetMethodSignatureWithSourceCode (method)));
					} else {
						il.Emit (OpCodes.Call, abr.NativeObjectExtensions_GetHandle);
						nativeType = abr.ObjCRuntime_NativeHandle;
						return true;
					}
				}
				AddException (ErrorHelper.CreateError (99, "Can't convert System.Void. Method: {0}", GetMethodSignatureWithSourceCode (method)));
				return false;
			}

			if (type.IsValueType) {
				if (type.Is ("System", "Boolean")) {
					// no conversion necessary either way
					nativeType = abr.System_Byte;
					return true;
				}

				if (type.Is ("System", "Char")) {
					// no conversion necessary either way
					nativeType = abr.System_UInt16;
					return true;
				}

				// no conversion necessary if we're any other value type
				nativeType = type;
				return true;
			}

			if (type is PointerType pt) {
				var elementType = pt.ElementType;
				if (!elementType.IsValueType)
					AddException (ErrorHelper.CreateError (99, "Unexpected pointer type {0}: must be a value type. Method: {1}", type, GetMethodSignatureWithSourceCode (method)));
				// no conversion necessary either way
				nativeType = type;
				return true;
			}

			if (type is ByReferenceType brt) {
				if (toManaged) {
					var elementType = brt.ElementType;
					if (elementType is GenericParameter gp) {
						if (!StaticRegistrar.VerifyIsConstrainedToNSObject (gp, out var constrained)) {
							AddException (ErrorHelper.CreateError (99, "Incorrectly constrained generic parameter. Method: {0}", GetMethodSignatureWithSourceCode (method)));
							return false;
						}
						elementType = constrained;
					}

					if (elementType.IsValueType) {
						// call !!0& [System.Runtime]System.Runtime.CompilerServices.Unsafe::AsRef<int32>(void*)
						var mr = new GenericInstanceMethod (abr.CurrentAssembly.MainModule.ImportReference (abr.Unsafe_AsRef));
						if (isOutParameter)
							il.EmitLoadArgument (nativeParameterIndex);
						mr.GenericArguments.Add (elementType);
						il.Emit (OpCodes.Call, mr);
						// reference types aren't blittable, so the managed signature must have be a pointer type
						nativeType = new PointerType (elementType);
						return true;
					}

					MethodReference? native_to_managed = null;
					MethodReference? managed_to_native = null;
					Instruction? addBeforeNativeToManagedCall = null;

					if (elementType is ArrayType elementArrayType) {
						if (elementArrayType.ElementType.Is ("System", "String")) {
							native_to_managed = abr.RegistrarHelper_NSArray_string_native_to_managed;
							managed_to_native = abr.RegistrarHelper_NSArray_string_managed_to_native;
						} else {
							native_to_managed = abr.RegistrarHelper_NSArray_native_to_managed.CreateGenericInstanceMethod (elementArrayType.ElementType);
							managed_to_native = abr.RegistrarHelper_NSArray_managed_to_native.CreateGenericInstanceMethod (elementArrayType.ElementType);
						}
						nativeType = new PointerType (abr.ObjCRuntime_NativeHandle);
					} else if (elementType.Is ("System", "String")) {
						native_to_managed = abr.RegistrarHelper_string_native_to_managed;
						managed_to_native = abr.RegistrarHelper_string_managed_to_native;
						nativeType = new PointerType (abr.ObjCRuntime_NativeHandle);
					} else if (elementType.IsNSObject (DerivedLinkContext)) {
						native_to_managed = abr.RegistrarHelper_NSObject_native_to_managed.CreateGenericInstanceMethod (elementType);
						managed_to_native = abr.RegistrarHelper_NSObject_managed_to_native;
						nativeType = new PointerType (abr.System_IntPtr);
					} else if (StaticRegistrar.IsNativeObject (DerivedLinkContext, elementType)) {
						var nativeObjType = StaticRegistrar.GetInstantiableType (type.Resolve (), exceptions, GetMethodSignature (method));
						addBeforeNativeToManagedCall = il.Create (OpCodes.Ldtoken, method.Module.ImportReference (nativeObjType)); // implementation type
						native_to_managed = abr.RegistrarHelper_INativeObject_native_to_managed.CreateGenericInstanceMethod (elementType);
						managed_to_native = abr.RegistrarHelper_INativeObject_managed_to_native;
						nativeType = new PointerType (abr.System_IntPtr);
					} else {
						AddException (ErrorHelper.CreateError (99, "Don't know how (4) to convert {0} between managed and native code. Method: {1}", type.FullName, GetMethodSignatureWithSourceCode (method)));
						return false;
					}

					if (managed_to_native is not null && native_to_managed is not null) {
						EnsureVisible (method, managed_to_native);
						EnsureVisible (method, native_to_managed);

						// brt.ElementType might be a generic type, so it should be use here it instead of elementType
						var indirectVariable = il.Body.AddVariable (brt.ElementType);
						// We store a copy of the value in a separate variable, to detect if it changes.
						var copyIndirectVariable = il.Body.AddVariable (brt.ElementType);

						// We don't read the input for 'out' parameters, it might be garbage.
						if (!isOutParameter) {
							il.Emit (OpCodes.Ldloca, indirectVariable);
							il.Emit (OpCodes.Ldloca, copyIndirectVariable);
							if (addBeforeNativeToManagedCall is not null)
								il.Append (addBeforeNativeToManagedCall);
							il.Emit (OpCodes.Call, native_to_managed);
						}

						if (IsOpenType (brt.ElementType)) {
							// for generic types try to verify that the variable is of the correct type
							// by casting it
							il.Emit (OpCodes.Ldloc, indirectVariable);
							il.Emit (OpCodes.Unbox_Any, brt.ElementType);
							il.Emit (OpCodes.Stloc, indirectVariable);
						}

						il.Emit (OpCodes.Ldloca, indirectVariable);

						postProcessing.Add (il.CreateLoadArgument (nativeParameterIndex));
						postProcessing.Add (il.Create (OpCodes.Ldloc, indirectVariable));
						postProcessing.Add (il.Create (OpCodes.Ldloc, copyIndirectVariable));
						postProcessing.Add (il.CreateLdc (isOutParameter));
						postProcessing.Add (il.Create (OpCodes.Call, managed_to_native));
						return true;
					}
				}

				AddException (ErrorHelper.CreateError (99, "Don't know how (2) to convert {0} between managed and native code. Method: {1}", type.FullName, GetMethodSignatureWithSourceCode (method)));
				return false;
			}

			if (isOutParameter) {
				AddException (ErrorHelper.CreateError (99, "Parameter must be ByReferenceType to be an out parameter. Method: {0}", GetMethodSignatureWithSourceCode (method)));
				return false;
			}

			if (type is ArrayType at) {
				var elementType = at.GetElementType ();
				if (elementType.Is ("System", "String")) {
					il.Emit (OpCodes.Call, toManaged ? abr.CFArray_StringArrayFromHandle : abr.RegistrarHelper_CreateCFArray);
					nativeType = abr.ObjCRuntime_NativeHandle;
					return true;
				}

				GenericParameter? gp = elementType as GenericParameter;
				if (gp is not null) {
					if (!StaticRegistrar.VerifyIsConstrainedToNSObject (gp, out var constrained)) {
						AddException (ErrorHelper.CreateError (99, "Incorrectly constrained generic parameter. Method: {0}", GetMethodSignatureWithSourceCode (method)));
						return false;
					}
					elementType = constrained;
				}

				var isNSObject = elementType.IsNSObject (DerivedLinkContext);
				var isNativeObject = StaticRegistrar.IsNativeObject (elementType);
				if (isNSObject || isNativeObject) {
					if (toManaged) {
						var gim = new GenericInstanceMethod (abr.NSArray_ArrayFromHandle_1);
						if (gp is not null) {
							var gemericParameter = method.DeclaringType.GenericParameters.Single (x => x.Name == gp.Name);
							gim.GenericArguments.Add (gemericParameter);
						} else {
							gim.GenericArguments.Add (elementType);
						}
						il.Emit (OpCodes.Call, gim);
					} else {
						var retain = StaticRegistrar.HasReleaseAttribute (method);
						il.Emit (retain ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
						il.Emit (OpCodes.Call, abr.RegistrarHelper_ManagedArrayToNSArray);
					}
					nativeType = abr.ObjCRuntime_NativeHandle;
					return true;
				}

				AddException (ErrorHelper.CreateError (99, "Don't know how (3) to convert array element type {1} for array type {0} between managed and native code. Method: {2}", type.FullName, elementType.FullName, GetMethodSignatureWithSourceCode (method)));
				return false;
			}

			if (IsNSObject (type)) {
				if (toManaged) {
					var ea = StaticRegistrar.CreateExportAttribute (method);
					if (ea is not null && ea.ArgumentSemantic == ArgumentSemantic.Copy)
						il.Emit (OpCodes.Call, abr.Runtime_CopyAndAutorelease);
					if (IsOpenType (type)) {
						il.Emit (OpCodes.Call, abr.Runtime_GetNSObject__System_IntPtr);
						// cast to the generic type to verify that the item is actually of the correct type
						il.Emit (OpCodes.Unbox_Any, type);
					} else {
						il.Emit (OpCodes.Ldarg_1); // SEL
						il.Emit (OpCodes.Ldtoken, method);
						il.EmitLdc (parameter == -1); // evenInFinalizerQueue
						il.Emit (OpCodes.Call, abr.Runtime_GetNSObject_T___System_IntPtr_System_IntPtr_System_RuntimeMethodHandle_bool.CreateGenericInstanceMethod (type));
						var tmpVariable = il.Body.AddVariable (type);
						il.Emit (OpCodes.Stloc, tmpVariable);
						il.Emit (OpCodes.Ldloc, tmpVariable);
					}

					nativeType = abr.System_IntPtr;
				} else {
					if (parameter == -1) {
						var retain = StaticRegistrar.HasReleaseAttribute (method);
						il.Emit (OpCodes.Dup);
						if (retain) {
							il.Emit (OpCodes.Call, abr.Runtime_RetainNSObject);
						} else {
							il.Emit (OpCodes.Call, abr.Runtime_RetainAndAutoreleaseNSObject);
						}
					} else {
						il.Emit (OpCodes.Call, abr.NativeObjectExtensions_GetHandle);
					}
					nativeType = abr.ObjCRuntime_NativeHandle;
				}
				return true;
			}

			if (StaticRegistrar.IsNativeObject (DerivedLinkContext, type)) {
				if (toManaged) {
					if (IsOpenType (type)) {
						il.Emit (OpCodes.Call, abr.Runtime_GetNSObject__System_IntPtr);
						// cast to the generic type to verify that the item is actually of the correct type
						il.Emit (OpCodes.Unbox_Any, type);
					} else {
						StaticRegistrar.GetInstantiableType (type.Resolve (), exceptions, GetMethodSignature (method), out var ctor);
						EnsureVisible (method, ctor);
						var targetType = method.Module.ImportReference (type);
						var handleVariable = il.Body.AddVariable (abr.System_IntPtr);
						var objectVariable = il.Body.AddVariable (targetType);
						var loadHandle = il.Create (OpCodes.Ldloc, handleVariable);
						var loadObjectVariable = il.Create (OpCodes.Ldloc, objectVariable);
						il.Emit (OpCodes.Stloc, handleVariable);
						// objectVariable = null
						il.Emit (OpCodes.Ldnull);
						il.Emit (OpCodes.Stloc, objectVariable);
						// if (handle == IntPtr.Zero)
						//     goto done;
						il.Emit (OpCodes.Ldloc, handleVariable); // handle
						il.Emit (OpCodes.Ldsfld, abr.System_IntPtr_Zero);
						il.Emit (OpCodes.Beq, loadObjectVariable);
						// objectVariable = TryGetNSObject (handle, false) as TargetType
						il.Emit (OpCodes.Ldloc, handleVariable); // handle
						il.Emit (OpCodes.Ldc_I4_0); // false
						il.Emit (OpCodes.Call, abr.Runtime_TryGetNSObject);
						il.Emit (OpCodes.Castclass, targetType);
						il.Emit (OpCodes.Stloc, objectVariable);
						// if (objectVariable is null)
						//     objectVariable = new TargetType (handle, false)
						il.Emit (OpCodes.Ldloc, objectVariable);
						il.Emit (OpCodes.Brfalse, loadHandle);
						il.Emit (OpCodes.Br, loadObjectVariable);
						il.Append (loadHandle);
						if (ctor.Parameters [0].ParameterType.Is ("ObjCRuntime", "NativeHandle"))
							il.Emit (OpCodes.Call, abr.NativeObject_op_Implicit_NativeHandle);
						il.Emit (OpCodes.Ldc_I4_0); // false
						il.Emit (OpCodes.Newobj, method.Module.ImportReference (ctor));
						il.Emit (OpCodes.Stloc, objectVariable);
						// done:
						// (load objectVariable on the stack)
						il.Append (loadObjectVariable);
					}
					nativeType = abr.System_IntPtr;
				} else {
					if (parameter == -1) {
						var retain = StaticRegistrar.HasReleaseAttribute (method);
						var isNSObject = IsNSObject (type);
						if (retain) {
							il.Emit (OpCodes.Call, isNSObject ? abr.Runtime_RetainNSObject : abr.Runtime_RetainNativeObject);
						} else {
							il.Emit (OpCodes.Call, isNSObject ? abr.Runtime_RetainAndAutoreleaseNSObject : abr.Runtime_RetainAndAutoreleaseNativeObject);
						}
					} else {
						il.Emit (OpCodes.Call, abr.NativeObjectExtensions_GetHandle);
					}
					nativeType = abr.ObjCRuntime_NativeHandle;
				}
				return true;
			}

			if (type.Is ("System", "String")) {
				il.Emit (OpCodes.Call, toManaged ? abr.CFString_FromHandle : abr.CFString_CreateNative);
				nativeType = abr.ObjCRuntime_NativeHandle;
				return true;
			}

			if (StaticRegistrar.IsDelegate (type.Resolve ())) {
				if (!StaticRegistrar.TryFindMethod (method, out var objcMethod)) {
					AddException (ErrorHelper.CreateError (99, "Unable to find method {0}", GetMethodSignature (method)));
					return false;
				}
				if (toManaged) {
					var createMethod = StaticRegistrar.GetBlockWrapperCreator (objcMethod, parameter);
					if (createMethod is null) {
						AddException (ErrorHelper.CreateWarning (App, 4174 /* Unable to locate the block to delegate conversion method for the method {0}'s parameter #{1}. */, method, Errors.MT4174, method.FullName, parameter + 1));
						// var blockCopy = BlockLiteral.Copy (block);
						var tmpVariable = il.Body.AddVariable (abr.System_IntPtr);
						il.Emit (OpCodes.Call, abr.BlockLiteral_Copy);
						il.Emit (OpCodes.Stloc, tmpVariable);
						// var blockWrapperCreator = Runtime.GetBlockWrapperCreator ((MethodInfo) MethodBase.GetMethodFromHandle (ldtoken <method>), parameter);
						il.Emit (OpCodes.Ldtoken, method);
						il.Emit (OpCodes.Call, abr.MethodBase_GetMethodFromHandle__RuntimeMethodHandle);
						il.Emit (OpCodes.Castclass, abr.System_Reflection_MethodInfo);
						il.Emit (OpCodes.Ldc_I4, parameter);
						il.Emit (OpCodes.Call, abr.Runtime_GetBlockWrapperCreator);
						// Runtime.CreateBlockProxy (blockWrapperCreator, blockCopy)
						il.Emit (OpCodes.Ldloc, tmpVariable);
						il.Emit (OpCodes.Call, abr.Runtime_CreateBlockProxy);
					} else {
						EnsureVisible (method, createMethod);
						// var blockCopy = BlockLiteral.Copy (block)
						// Runtime.ReleaseBlockWhenDelegateIsCollected (blockCopy, <create method> (blockCopy))
						il.Emit (OpCodes.Call, abr.BlockLiteral_Copy);
						il.Emit (OpCodes.Dup);
						il.Emit (OpCodes.Call, method.Module.ImportReference (createMethod));
						il.Emit (OpCodes.Call, abr.Runtime_ReleaseBlockWhenDelegateIsCollected);
					}
					il.Emit (OpCodes.Castclass, method.Module.ImportReference (type));
				} else {
					FieldDefinition? delegateProxyField = null;
					MethodDefinition? createBlockMethod = null;

					if (!DerivedLinkContext.StaticRegistrar.TryComputeBlockSignature (method, trampolineDelegateType: type, out var exception, out var signature)) {
						AddException (ErrorHelper.CreateWarning (99, exception, "Error while converting block/delegates: {0}", exception.ToString ()));
					} else {
						var delegateProxyType = StaticRegistrar.GetDelegateProxyType (objcMethod);
						if (delegateProxyType is null) {
							exceptions.Add (ErrorHelper.CreateWarning (App, 4176, method, Errors.MT4176 /* "Unable to locate the delegate to block conversion type for the return value of the method {0}." */, method.FullName));
						} else {
							createBlockMethod = StaticRegistrar.GetCreateBlockMethod (delegateProxyType);
							if (createBlockMethod is null) {
								delegateProxyField = delegateProxyType.Fields.SingleOrDefault (v => v.Name == "Handler");
								if (delegateProxyField is null) {
									AddException (ErrorHelper.CreateWarning (99, "No delegate proxy field on {0}", delegateProxyType.FullName));
								}
							}
						}
					}

					// the delegate is already on the stack
					if (createBlockMethod is not null) {
						EnsureVisible (method, createBlockMethod);
						il.Emit (OpCodes.Call, method.Module.ImportReference (createBlockMethod));
						il.Emit (OpCodes.Call, abr.RegistrarHelper_GetBlockPointer);
					} else if (delegateProxyField is not null) {
						EnsureVisible (method, delegateProxyField);
						il.Emit (OpCodes.Ldsfld, method.Module.ImportReference (delegateProxyField));
						il.Emit (OpCodes.Ldstr, signature);
						il.Emit (OpCodes.Call, abr.BlockLiteral_CreateBlockForDelegate);
					} else {
						il.Emit (OpCodes.Ldtoken, method);
						il.Emit (OpCodes.Call, abr.RegistrarHelper_GetBlockForDelegate);
					}
				}
				nativeType = abr.System_IntPtr;
				return true;
			}

			AddException (ErrorHelper.CreateError (99, "Don't know how (1) to convert {0} between managed and native code: {1}. Method: {2}", type.FullName, type.GetType ().FullName, GetMethodSignatureWithSourceCode (method)));
			return false;
		}

		internal static bool IsOpenType (TypeReference tr)
		{
			if (tr is GenericParameter)
				return true;

			if (tr is GenericInstanceType git) {
				foreach (var ga in git.GenericArguments) {
					if (IsOpenType (ga))
						return true;
				}
				return false;
			}

			if (tr is TypeSpecification ts)
				return IsOpenType (ts.ElementType);

			if (tr is TypeDefinition td)
				return td.HasGenericParameters;

			return IsOpenType (tr.Resolve ());
		}

		static void EnsureVisible (MethodDefinition caller, FieldDefinition field)
		{
			field.IsPublic = true;
			EnsureVisible (caller, field.DeclaringType);
		}

		static void EnsureVisible (MethodDefinition caller, TypeDefinition type)
		{
			if (type.IsNested) {
				type.IsNestedPublic = true;
				EnsureVisible (caller, type.DeclaringType);
			} else {
				type.IsPublic = true;
			}
		}

		static void EnsureVisible (MethodDefinition caller, MethodReference method)
		{
			var md = method.Resolve ();
			md.IsPublic = true;
			EnsureVisible (caller, md.DeclaringType);
		}

		bool IsOutParameter (MethodDefinition method, int parameter, ParameterDefinition baseParameter)
		{
			return method.Parameters [parameter].IsOut || baseParameter.IsOut;
		}

		StaticRegistrar StaticRegistrar {
			get { return DerivedLinkContext.StaticRegistrar; }
		}

		CustomAttribute CreateUnmanagedCallersAttribute (string entryPoint)
		{
			var unmanagedCallersAttribute = new CustomAttribute (abr.UnmanagedCallersOnlyAttribute_Constructor);
			// Mono didn't prefix the entry point with an underscore until .NET 8: https://github.com/dotnet/runtime/issues/79491
			var entryPointPrefix = Driver.TargetFramework.Version.Major < 8 ? "_" : string.Empty;
			unmanagedCallersAttribute.Fields.Add (new CustomAttributeNamedArgument ("EntryPoint", new CustomAttributeArgument (abr.System_String, entryPointPrefix + entryPoint)));
			return unmanagedCallersAttribute;
		}

		void GenerateConversionToManaged (MethodDefinition method, ILProcessor il, TypeReference inputType, TypeReference outputType, string descriptiveMethodName, int parameter, out TypeReference nativeCallerType)
		{
			// This is a mirror of the native method xamarin_generate_conversion_to_managed (for the dynamic registrar).
			// It's also a mirror of the method StaticRegistrar.GenerateConversionToManaged.
			// These methods must be kept in sync.
			var managedType = outputType;
			var nativeType = inputType;

			var isManagedNullable = StaticRegistrar.IsNullable (managedType);

			var underlyingManagedType = managedType;
			var underlyingNativeType = nativeType;

			var isManagedArray = StaticRegistrar.IsArray (managedType);
			var isNativeArray = StaticRegistrar.IsArray (nativeType);

			nativeCallerType = abr.System_IntPtr;

			if (isManagedArray != isNativeArray)
				throw ErrorHelper.CreateError (99, Errors.MX0099, $"can't convert from '{inputType.FullName}' to '{outputType.FullName}' in {descriptiveMethodName}");

			if (isManagedArray) {
				if (isManagedNullable)
					throw ErrorHelper.CreateError (99, Errors.MX0099, $"can't convert from '{inputType.FullName}' to '{outputType.FullName}' in {descriptiveMethodName}");
				underlyingNativeType = StaticRegistrar.GetElementType (nativeType);
				underlyingManagedType = StaticRegistrar.GetElementType (managedType);
			} else if (isManagedNullable) {
				underlyingManagedType = StaticRegistrar.GetNullableType (managedType);
			}

			string? func = null;
			MethodReference? conversionFunction = null;
			MethodReference? conversionFunction2 = null;
			if (underlyingNativeType.Is ("Foundation", "NSNumber")) {
				func = StaticRegistrar.GetNSNumberToManagedFunc (underlyingManagedType, inputType, outputType, descriptiveMethodName, out var _);
			} else if (underlyingNativeType.Is ("Foundation", "NSValue")) {
				func = StaticRegistrar.GetNSValueToManagedFunc (underlyingManagedType, inputType, outputType, descriptiveMethodName, out var _);
			} else if (underlyingNativeType.Is ("Foundation", "NSString")) {
				if (!StaticRegistrar.IsSmartEnum (underlyingManagedType, out var getConstantMethod, out var getValueMethod)) {
					// method linked away!? this should already be verified
					AddException (ErrorHelper.CreateError (99, Errors.MX0099, $"the smart enum {underlyingManagedType.FullName} doesn't seem to be a smart enum after all"));
					return;
				}

				var gim = new GenericInstanceMethod (abr.Runtime_GetNSObject_T___System_IntPtr);
				gim.GenericArguments.Add (abr.Foundation_NSString);
				conversionFunction = gim;

				conversionFunction2 = abr.CurrentAssembly.MainModule.ImportReference (getValueMethod);
			} else {
				throw ErrorHelper.CreateError (99, Errors.MX0099, $"can't convert from '{inputType.FullName}' to '{outputType.FullName}' in {descriptiveMethodName}");
			}

			if (func is not null) {
				conversionFunction = abr.GetMethodReference (abr.PlatformAssembly, abr.ObjCRuntime_BindAs, func, func, (v) =>
						v.IsStatic, out MethodDefinition conversionFunctionDefinition);
				EnsureVisible (method, conversionFunctionDefinition.DeclaringType);
			}

			if (isManagedArray) {
				il.Emit (OpCodes.Ldftn, conversionFunction);
				if (conversionFunction2 is not null) {
					il.Emit (OpCodes.Ldftn, conversionFunction2);
					var gim = new GenericInstanceMethod (abr.BindAs_ConvertNSArrayToManagedArray2);
					gim.GenericArguments.Add (underlyingManagedType);
					gim.GenericArguments.Add (abr.Foundation_NSString);
					il.Emit (OpCodes.Call, gim);
				} else {
					var gim = new GenericInstanceMethod (abr.BindAs_ConvertNSArrayToManagedArray);
					gim.GenericArguments.Add (underlyingManagedType);
					il.Emit (OpCodes.Call, gim);
				}
				nativeCallerType = abr.System_IntPtr;
			} else {
				if (isManagedNullable) {
					il.Emit (OpCodes.Ldftn, conversionFunction);
					if (conversionFunction2 is not null) {
						il.Emit (OpCodes.Ldftn, conversionFunction2);
						var gim = new GenericInstanceMethod (abr.BindAs_CreateNullable2);
						gim.GenericArguments.Add (underlyingManagedType);
						gim.GenericArguments.Add (abr.Foundation_NSString);
						il.Emit (OpCodes.Call, gim);
					} else {
						var gim = new GenericInstanceMethod (abr.BindAs_CreateNullable);
						gim.GenericArguments.Add (underlyingManagedType);
						il.Emit (OpCodes.Call, gim);
					}
					nativeCallerType = abr.System_IntPtr;
				} else {
					il.Emit (OpCodes.Call, conversionFunction);
					if (conversionFunction2 is not null)
						il.Emit (OpCodes.Call, conversionFunction2);
					nativeCallerType = abr.System_IntPtr;
				}
			}
		}

		void GenerateConversionToNative (MethodDefinition method, ILProcessor il, TypeReference inputType, TypeReference outputType, string descriptiveMethodName, out TypeReference nativeCallerType)
		{
			// This is a mirror of the native method xamarin_generate_conversion_to_native (for the dynamic registrar).
			// It's also a mirror of the method StaticRegistrar.GenerateConversionToNative.
			// These methods must be kept in sync.
			var managedType = inputType;
			var nativeType = outputType;

			var isManagedNullable = StaticRegistrar.IsNullable (managedType);

			var underlyingManagedType = managedType;
			var underlyingNativeType = nativeType;

			var isManagedArray = StaticRegistrar.IsArray (managedType);
			var isNativeArray = StaticRegistrar.IsArray (nativeType);

			nativeCallerType = abr.System_IntPtr;

			if (isManagedArray != isNativeArray)
				throw ErrorHelper.CreateError (99, Errors.MX0099, $"can't convert from '{inputType.FullName}' to '{outputType.FullName}' in {descriptiveMethodName}");

			if (isManagedArray) {
				if (isManagedNullable)
					throw ErrorHelper.CreateError (99, Errors.MX0099, $"can't convert from '{inputType.FullName}' to '{outputType.FullName}' in {descriptiveMethodName}");
				underlyingNativeType = StaticRegistrar.GetElementType (nativeType);
				underlyingManagedType = StaticRegistrar.GetElementType (managedType);
			} else if (isManagedNullable) {
				underlyingManagedType = StaticRegistrar.GetNullableType (managedType);
			}

			string? func = null;
			MethodReference? conversionFunction = null;
			MethodReference? conversionFunction2 = null;
			MethodReference? conversionFunction3 = null;
			if (underlyingNativeType.Is ("Foundation", "NSNumber")) {
				func = StaticRegistrar.GetManagedToNSNumberFunc (underlyingManagedType, inputType, outputType, descriptiveMethodName);
			} else if (underlyingNativeType.Is ("Foundation", "NSValue")) {
				func = StaticRegistrar.GetManagedToNSValueFunc (underlyingManagedType, inputType, outputType, descriptiveMethodName);
			} else if (underlyingNativeType.Is ("Foundation", "NSString")) {
				if (!StaticRegistrar.IsSmartEnum (underlyingManagedType, out var getConstantMethod, out var getValueMethod)) {
					// method linked away!? this should already be verified
					ErrorHelper.Show (ErrorHelper.CreateError (99, Errors.MX0099, $"the smart enum {underlyingManagedType.FullName} doesn't seem to be a smart enum after all"));
					return;
				}

				conversionFunction = abr.CurrentAssembly.MainModule.ImportReference (getConstantMethod);
				conversionFunction2 = abr.NativeObjectExtensions_GetHandle;
				conversionFunction3 = abr.NativeObject_op_Implicit_IntPtr;
			} else {
				AddException (ErrorHelper.CreateError (99, Errors.MX0099, $"can't convert from '{inputType.FullName}' to '{outputType.FullName}' in {descriptiveMethodName}"));
				return;
			}

			if (func is not null) {
				conversionFunction = abr.GetMethodReference (abr.PlatformAssembly, abr.ObjCRuntime_BindAs, func, func, (v) =>
						v.IsStatic, out MethodDefinition conversionFunctionDefinition);
				EnsureVisible (method, conversionFunctionDefinition.DeclaringType);
			}

			if (isManagedArray) {
				il.Emit (OpCodes.Ldftn, conversionFunction);
				if (conversionFunction2 is not null) {
					il.Emit (OpCodes.Ldftn, conversionFunction2);
					var gim = new GenericInstanceMethod (abr.BindAs_ConvertManagedArrayToNSArray2);
					gim.GenericArguments.Add (underlyingManagedType);
					gim.GenericArguments.Add (abr.Foundation_NSString);
					il.Emit (OpCodes.Call, gim);
				} else {
					var gim = new GenericInstanceMethod (abr.BindAs_ConvertManagedArrayToNSArray);
					gim.GenericArguments.Add (underlyingManagedType);
					il.Emit (OpCodes.Call, gim);
				}
			} else {
				var tmpVariable = il.Body.AddVariable (managedType);

				var trueTarget = il.Create (OpCodes.Nop);
				var endTarget = il.Create (OpCodes.Nop);
				if (isManagedNullable) {
					il.Emit (OpCodes.Stloc, tmpVariable);
					il.Emit (OpCodes.Ldloca, tmpVariable);
					var mr = abr.System_Nullable_1.CreateMethodReferenceOnGenericType (abr.Nullable_HasValue, underlyingManagedType);
					il.Emit (OpCodes.Call, mr);
					il.Emit (OpCodes.Brtrue, trueTarget);
					il.Emit (OpCodes.Ldc_I4_0);
					il.Emit (OpCodes.Conv_I);
					il.Emit (OpCodes.Br, endTarget);
					il.Append (trueTarget);
					il.Emit (OpCodes.Ldloca, tmpVariable);
					il.Emit (OpCodes.Call, abr.System_Nullable_1.CreateMethodReferenceOnGenericType (abr.Nullable_Value, underlyingManagedType));
				}
				il.Emit (OpCodes.Call, conversionFunction);
				if (conversionFunction2 is not null) {
					il.Emit (OpCodes.Call, conversionFunction2);
					if (conversionFunction3 is not null)
						il.Emit (OpCodes.Call, conversionFunction3);
				}
				if (isManagedNullable)
					il.Append (endTarget);
			}
		}

		MethodDefinition CloneConstructorWithNativeHandle (MethodDefinition ctor)
		{
			var clonedCtor = new MethodDefinition (ctor.Name, ctor.Attributes, ctor.ReturnType);
			clonedCtor.IsPublic = false;

			// clone the original parameters firsts
			foreach (var parameter in ctor.Parameters) {
				clonedCtor.AddParameter (parameter.Name, parameter.ParameterType);
			}

			// add a native handle param + a dummy parameter that we know for a fact won't be used anywhere
			// to make the signature of the new constructor unique
			var handleParameter = clonedCtor.AddParameter ("nativeHandle", abr.System_IntPtr);
			var dummyParameter = clonedCtor.AddParameter ("dummy", abr.ObjCRuntime_IManagedRegistrar);

			var body = clonedCtor.CreateBody (out var il);

			// ensure visible
			abr.Foundation_NSObject_HandleField.Resolve ().IsFamily = true;
#if NET
			abr.Foundation_NSObject_FlagsSetterMethod.Resolve ().IsFamily = true;
#else
			abr.Foundation_NSObject_FlagsField.Resolve ().IsFamily = true;
#endif

			// store the handle and flags first
			il.Emit (OpCodes.Ldarg_0);
			il.Emit (OpCodes.Ldarg, handleParameter);
#if NET
			il.Emit (OpCodes.Call, abr.NativeObject_op_Implicit_NativeHandle);
#endif
			il.Emit (OpCodes.Stfld, abr.CurrentAssembly.MainModule.ImportReference (abr.Foundation_NSObject_HandleField));

			il.Emit (OpCodes.Ldarg_0);
			il.Emit (OpCodes.Ldc_I4_2); // Flags.NativeRef == 2
#if NET
			il.Emit (OpCodes.Call, abr.Foundation_NSObject_FlagsSetterMethod);
#else
			il.Emit (OpCodes.Stfld, abr.Foundation_NSObject_FlagsField);
#endif

			// call the original constructor with all of the original parameters
			il.Emit (OpCodes.Ldarg_0);
			foreach (var parameter in clonedCtor.Parameters.SkipLast (2)) {
				il.Emit (OpCodes.Ldarg, parameter);
			}

			il.Emit (OpCodes.Call, ctor);
			il.Emit (OpCodes.Ret);

			return clonedCtor;
		}

		void ImplementNSObjectRegisterToggleRefMethodStub ()
		{
			// The NSObject.RegisterToggleRef method is a Mono icall that is unused in NativeAOT.
			// The method isn't included on all platforms but when it is present, we need to modify it
			// so that ILC can trim it and it doesn't report the following warning:
			// 
			//    ILC: Method '[Microsoft.iOS]Foundation.NSObject.RegisterToggleRef(NSObject,native int,bool)' will always throw because:
			//         Invalid IL or CLR metadata in 'Void Foundation.NSObject.RegisterToggleRef(Foundation.NSObject, IntPtr, Boolean)'
			//
			if (abr.TryGet_NSObject_RegisterToggleRef (out var registerToggleRef)) {
				registerToggleRef!.IsPublic = false;
				registerToggleRef!.IsInternalCall = false;

				registerToggleRef!.CreateBody (out var il);
				il.Emit (OpCodes.Ret);
			}
		}
	}
}
