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

		Dictionary<string, string> unmanagedCallersOnlyMap = new ();
		Dictionary<AssemblyDefinition, AssemblyDefinition?> relocatedCompanions = new ();
		Dictionary<TypeDefinition, ILookup<string, MethodDefinition>> callbackMethodsByName = new ();

		// Whether the registrar trampolines for the given method should be relocated into the
		// per-assembly companion assembly (_<Asm>.TypeMap.dll) instead of being emitted into the
		// user assembly. This is required for Hot Reload (user assemblies must stay unmodified).
		// This applies to both non-generic and generic declaring types (generic types dispatch to
		// a relocated generic helper by reflection, see the generic branch in
		// CreateUnmanagedCallersMethod).
		bool ShouldRelocateTrampolines (MethodDefinition method)
		{
			return Configuration.HotReloadCompatibleBuild
				&& App.Registrar == RegistrarMode.TrimmableStatic;
		}

		// Finds the companion assembly (_<Asm>.TypeMap) for the given user assembly among the loaded
		// assemblies. Used in the post-processing pass, where the companion was produced by an earlier
		// pass (and the RegistrarCompanionAssemblies dictionary is empty because it's a fresh process).
		AssemblyDefinition? FindRelocatedCompanionAssembly (AssemblyDefinition userAssembly)
		{
			if (relocatedCompanions.TryGetValue (userAssembly, out var companion))
				return companion;

			var companionName = RegistrarCompanionAssembly.GetName (userAssembly);
			foreach (var assembly in Configuration.Assemblies) {
				if (assembly.Name.Name == companionName) {
					relocatedCompanions.Add (userAssembly, assembly);
					return assembly;
				}
			}
			relocatedCompanions.Add (userAssembly, null);
			return null;
		}

		void AddException (Exception exception)
		{
			if (exceptions is null)
				exceptions = new List<Exception> ();
			exceptions.Add (exception);
		}

		protected override void TryProcess ()
		{
			base.TryProcess ();

			if (App.Registrar != RegistrarMode.ManagedStatic && App.Registrar != RegistrarMode.TrimmableStatic)
				return;

			if (App.IsPostProcessingAssemblies) {
				var ucoMapPath = Configuration.UnmanagedCallersOnlyMapPath;
				if (File.Exists (ucoMapPath)) {
					foreach (var line in File.ReadAllLines (ucoMapPath)) {
						var parts = line.Split ('|');
						if (parts.Length != 2) {
							Console.WriteLine ($"Warning: Invalid line in unmanaged_callers_only_map.txt: {line}");
							continue;
						}
						var methodFullName = parts [0];
						var ucoEntryPoint = parts [1];
						unmanagedCallersOnlyMap.Add (methodFullName, ucoEntryPoint);
					}
					File.Delete (ucoMapPath);
				}
			}

			Configuration.Application.StaticRegistrar.Register (Configuration.GetNonDeletedAssemblies (this));
		}

		protected override void TryEndProcess (out List<Exception>? exceptions)
		{
			base.TryEndProcess ();

			if (App.Registrar != RegistrarMode.ManagedStatic && App.Registrar != RegistrarMode.TrimmableStatic) {
				exceptions = null;
				return;
			}

			// Report back any exceptions that occurred during the processing.
			exceptions = this.exceptions;

			if (App.PrepareAssemblies && !App.InCustomTrimmerStep) {
				var ucoMapPath = Configuration.UnmanagedCallersOnlyMapPath;
				using (var writer = new StreamWriter (ucoMapPath, false)) {
					foreach (var entry in unmanagedCallersOnlyMap.Select (kvp => $"{kvp.Key}|{kvp.Value}").OrderBy (v => v)) {
						writer.WriteLine (entry);
					}
				}
			}

#if !ASSEMBLY_PREPARER
			// Mark some stuff we use later on.
			if (App.InCustomTrimmerStep && App.PrepareAssemblies == false) {
				abr.SetCurrentAssembly (abr.PlatformAssembly);
				Annotations.Mark (abr.RegistrarHelper_Register.Resolve ());
				abr.ClearCurrentAssembly ();
			}
#endif
		}

		protected override void TryProcessAssembly (AssemblyDefinition assembly)
		{
			base.TryProcessAssembly (assembly);

			if (App.Registrar != RegistrarMode.ManagedStatic && App.Registrar != RegistrarMode.TrimmableStatic)
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
			if (App.XamarinRuntime == XamarinRuntime.NativeAOT && Configuration.Profile.IsProductAssembly (assembly) && !App.IsPostProcessingAssemblies) {
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
			var isNSObject = IsNSObject (type);

			// The factory methods must be added before trimming: either in the assembly preparer (when
			// PrepareAssemblies=true), or inside ILLink itself (when PrepareAssemblies=false). They must not
			// be added again when post-processing assemblies, since they're already there at that point.
			if (App.Registrar == RegistrarMode.TrimmableStatic && !type.IsAbstract && !type.IsInterface && !App.IsPostProcessingAssemblies
				&& (!Configuration.HotReloadCompatibleBuild || Annotations.GetAction (type.Module.Assembly) == AssemblyAction.Link)) {
				if (isNSObject) {
					var ctorRef = AppBundleRewriter.FindNSObjectConstructor (type);
					if (ctorRef is not null) {
						var ctor = abr.CurrentAssembly.MainModule.ImportReference (ctorRef);

						// Generic types can't be instantiated by the type map, because the concrete generic
						// arguments aren't known there, so they must provide their own factory method.
						if (type.HasGenericParameters)
							modified |= abr.ImplementConstructNSObjectFactoryMethod (DerivedLinkContext, type, ctor);
						// Implement INativeObject._Xamarin_ConstructINativeObject
						modified |= abr.ImplementConstructINativeObjectFactoryMethod (DerivedLinkContext, type, ctor);
					}
				} else if (type.IsNativeObject ()) {
					var ctorRef = AppBundleRewriter.FindINativeObjectConstructor (type);
					if (ctorRef is not null) {
						var ctor = abr.CurrentAssembly.MainModule.ImportReference (ctorRef);

						// Implement INativeObject._Xamarin_ConstructINativeObject
						modified |= abr.ImplementConstructINativeObjectFactoryMethod (DerivedLinkContext, type, ctor);
					}
				}
			}

			process |= isNSObject;
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
					if (App.IsPostProcessingAssemblies) {
						// We need to load what the PrepareAssemblies task did/produced
						CollectUnmanagedCallersMethod (method, infos, proxyInterfaces);
					} else {
						modified |= CreateUnmanagedCallersMethod (method, infos, proxyInterfaces);
					}
				} catch (Exception e) {
					AddException (ErrorHelper.CreateError (99, e, "Failed to create an UnmanagedCallersOnly trampoline for {0}: {1}", method.FullName, e.Message));
				}
			}

			return modified;
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

		void CollectUnmanagedCallersMethod (MethodDefinition method, AssemblyTrampolineInfo infos, List<TypeDefinition> proxyInterfaces)
		{
			if (!unmanagedCallersOnlyMap.TryGetValue (method.FullName, out var ucoName)) {
				AddException (ErrorHelper.CreateWarning (App, 99, method, $"Couldn't find an entry in the unmanaged_callers_only_map for method {method.FullName}."));
				return;
			}

			TypeDefinition? callbackType;
			if (ShouldRelocateTrampolines (method)) {
				// The trampolines were relocated into the companion assembly (_<Asm>.TypeMap.dll),
				// which was produced by the earlier PrepareAssemblies pass and is loaded again here.
				// Find the top-level '__Registrar_Callbacks__' type there.
				var companionAssembly = FindRelocatedCompanionAssembly (method.Module.Assembly);
				callbackType = companionAssembly?.MainModule.Types.SingleOrDefault (v => v.Name == "__Registrar_Callbacks__" && v.DeclaringType is null);
				if (callbackType is null) {
					AddException (ErrorHelper.CreateWarning (App, 99, method, $"Couldn't find the __Registrar_Callbacks__ type in the companion assembly for method {method.FullName}."));
					return;
				}
			} else {
				callbackType = method.DeclaringType.NestedTypes.SingleOrDefault (v => v.Name == "__Registrar_Callbacks__");
				if (callbackType is null) {
					AddException (ErrorHelper.CreateWarning (App, 99, method, $"Couldn't find the __Registrar_Callbacks__ nested type for method {method.FullName}."));
					return;
				}
			}

			if (!callbackMethodsByName.TryGetValue (callbackType, out var callbackMethods)) {
				callbackMethods = callbackType.Methods.ToLookup (v => v.Name);
				callbackMethodsByName.Add (callbackType, callbackMethods);
			}
			var candidates = callbackMethods [ucoName].ToArray ();
			if (candidates.Length != 1) {
				AddException (ErrorHelper.CreateWarning (App, 99, method, $"Didn't find exactly one matching callback method in __Registrar_Callbacks__ for method {method.FullName}, found {candidates.Length}"));
				return;
			}
			var callback = candidates [0];

			var info = new TrampolineInfo (callback, method, ucoName);
			if (this.App.Registrar == RegistrarMode.TrimmableStatic) {
				// Don't set Id here, it's not used.
			} else if (int.TryParse (ucoName.Split ('_') [1], NumberStyles.None, CultureInfo.InvariantCulture, out var id)) {
				info.Id = id;
			} else {
				AddException (ErrorHelper.CreateError (App, 99, method, $"Failed to parse the ID from the DynamicDependencyAttribute for method {method.FullName}, the trampoline won't be registered correctly. The member signature was: {ucoName}"));
			}
			infos.Add (info);
		}

		int counter;
		AssemblyDefinition? last_assembly;
		bool CreateUnmanagedCallersMethod (MethodDefinition method, AssemblyTrampolineInfo infos, List<TypeDefinition> proxyInterfaces)
		{
			var modified = false;

			if (last_assembly != method.Module.Assembly) {
				counter = 0;
				last_assembly = method.Module.Assembly;
			}

			var baseMethod = StaticRegistrar.GetBaseMethodInTypeHierarchy (method);
			var name = $"callback_{counter++}_{Sanitize (method.DeclaringType.FullName)}_{Sanitize (method.Name)}";

			unmanagedCallersOnlyMap.Add (method.FullName, name);

			var relocate = ShouldRelocateTrampolines (method);
			RegistrarCompanionAssembly? companion = null;
			if (relocate) {
				// Switch the AppBundleRewriter to the companion assembly so that everything we
				// emit below (the trampoline itself, and all the references it uses) ends up in
				// the companion assembly instead of the user assembly.
				abr.ClearCurrentAssembly ();
				companion = RegistrarCompanionAssembly.GetOrCreate (Configuration, method.Module.Assembly);
				abr.SetCurrentAssembly (companion.Assembly);
			} else {
				// We're emitting the trampoline (and, for generic types, a proxy interface and its
				// implementation) into the user assembly, so make sure it gets saved. This matters
				// when HotReloadCompatibleBuild is enabled but this particular method isn't relocated
				// (e.g. it's declared in a generic type): the user assembly is modified here even
				// though other methods in the same assembly may have been relocated.
				modified = true;
			}

			var placeholderType = abr.System_IntPtr;

			TypeDefinition callbackType;
			if (companion is not null) {
				// A single top-level '__Registrar_Callbacks__' type in the companion holds all the
				// trampolines for the user assembly. The callback names are unique so there are no
				// collisions between the trampolines of different user types.
				if (companion.CallbacksType is null) {
					var companionCallbacksType = new TypeDefinition (string.Empty, "__Registrar_Callbacks__", TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class);
					companionCallbacksType.BaseType = abr.System_Object;
					companion.Assembly.MainModule.Types.Add (companionCallbacksType);
					companion.CallbacksType = companionCallbacksType;
					callbackType = companionCallbacksType;
				} else {
					callbackType = companion.CallbacksType;
				}
			} else {
				var existingCallbackType = method.DeclaringType.NestedTypes.SingleOrDefault (v => v.Name == "__Registrar_Callbacks__");
				if (existingCallbackType is null) {
					existingCallbackType = new TypeDefinition (string.Empty, "__Registrar_Callbacks__", TypeAttributes.NestedPrivate | TypeAttributes.Sealed | TypeAttributes.Class);
					existingCallbackType.BaseType = abr.System_Object;
					method.DeclaringType.NestedTypes.Add (existingCallbackType);
				}
				callbackType = existingCallbackType;
			}

			var callback = callbackType.AddMethod (name, MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig, placeholderType);
			callback.CustomAttributes.Add (CreateUnmanagedCallersAttribute (name, method.DeclaringType));
			infos.Add (new TrampolineInfo (callback, method, name));

			// If the target method is marked, then we must mark the trampoline as well.
			// When relocating we must not modify the user assembly, so we skip adding the
			// [DynamicDependency] attribute to the user method. The trampoline is instead kept alive
			// by the reference to it (via ldftn) from the companion's LookupUnmanagedFunction method.
			if (!relocate)
				method.CustomAttributes.Add (abr.CreateDynamicDependencyAttribute (callback.Name, callbackType));

			callback.AddParameter (abr.System_IntPtr); // pobj

			var isGeneric = method.DeclaringType.HasGenericParameters;
			if (isGeneric && method.IsStatic) {
				throw ErrorHelper.CreateError (4130 /* The registrar cannot export static methods in generic classes ('{0}'). */, method.FullName);
			} else if (relocate && isGeneric && !method.IsConstructor) {
				// When relocating trampolines for Hot Reload we must not modify the user assembly,
				// so the proxy-interface trick used below (which adds an interface implementation to
				// the user type) is not an option. Instead we emit a *generic* helper method into
				// the companion whose body performs the same type-parameter-aware conversions, and
				// the static UnmanagedCallersOnly callback dispatches to it by reflection (closing
				// the helper over the instance's actual generic arguments at runtime). This is only
				// used under a Hot-Reload-compatible build, which always runs under the JIT.
				EmitGenericCompanionTrampoline (method, callback, callbackType);
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

					// The trimmer may just remove the interface implementation, because it thinks it's not used - which it technically
					// isn't, because the consuming code is generated in the ManagedRegistrarLookupTables step, which happens after trimming.
					var attrib = abr.CreateDynamicDependencyAttribute (DynamicallyAccessedMemberTypes.Interfaces, method.DeclaringType);
					abr.AddAttributeToStaticConstructor (method.DeclaringType, attrib);
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

				// make the interface method depend on the implementation method, so that the trimmer doesn't remove the implementation method
				abr.AddDynamicDependencyAttribute (interfaceMethod, implementationMethod);

				foreach (var parameter in implementationMethod.Parameters) {
					callback.AddParameter (parameter.Name, parameter.ParameterType);
					interfaceMethod.AddParameter (parameter.Name, parameter.ParameterType);
				}

				// we need to wait until we know all the parameters of the interface method before we generate this method
				EmitCallToProxyMethod (method, callback, interfaceMethod);
			} else {
				EmitCallToExportedMethod (method, callback);
			}

			if (relocate) {
				// Everything we emitted above may reference the user assembly directly (e.g. via
				// method.Module.ImportReference or by using the user method/type as an operand).
				// Re-import all those references into the companion module so the trampoline is a
				// valid member of the companion assembly, then restore the user assembly as the
				// current assembly (which is what the rest of the processing expects).
				ImportCallbackReferences (callback);
				abr.ClearCurrentAssembly ();
				abr.SetCurrentAssembly (method.Module.Assembly);
			}

			return modified;
		}

		// Emits the relocated generic trampoline for a method declared in a generic type, when
		// relocating into the companion assembly for Hot Reload. Rather than adding a proxy
		// interface implementation to the user type (which would modify the user assembly), we emit
		// a generic helper method into the companion whose body performs the type-parameter-aware
		// conversions, and make the static callback dispatch to it by reflection (closing the helper
		// over the instance's actual generic arguments at runtime). The 'callback' passed in is the
		// UnmanagedCallersOnly trampoline (with only its 'pobj' parameter added so far).
		void EmitGenericCompanionTrampoline (MethodDefinition method, MethodDefinition callback, TypeDefinition callbackType)
		{
			var module = callback.Module;
			var placeholderType = abr.System_IntPtr;

			// Create the generic helper method, mirroring the user type's generic parameters.
			var implMethod = new MethodDefinition (callback.Name + "__impl", MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig, placeholderType);
			callbackType.Methods.Add (implMethod);

			var genericParameterMap = new Dictionary<GenericParameter, GenericParameter> ();
			foreach (var userGenericParameter in method.DeclaringType.GenericParameters) {
				var implGenericParameter = new GenericParameter (userGenericParameter.Name, implMethod) {
					Attributes = userGenericParameter.Attributes,
				};
				implMethod.GenericParameters.Add (implGenericParameter);
				genericParameterMap [userGenericParameter] = implGenericParameter;
			}

			// Copy the generic parameter constraints in a second pass, since a constraint may
			// reference another generic parameter of the same type. Constraints must be copied so
			// that the closed instantiations we create at runtime (via MakeGenericMethod) are valid,
			// and so that the 'castclass' to the user generic type inside the body verifies.
			for (var i = 0; i < method.DeclaringType.GenericParameters.Count; i++) {
				var userGenericParameter = method.DeclaringType.GenericParameters [i];
				var implGenericParameter = implMethod.GenericParameters [i];
				foreach (var constraint in userGenericParameter.Constraints) {
					var constraintType = ImportTypeReference (module, RemapGenericParameter (constraint.ConstraintType, genericParameterMap));
					implGenericParameter.Constraints.Add (new GenericParameterConstraint (constraintType));
				}
			}

			// Add the 'self' parameter (arg0), typed as the user generic type closed over the
			// helper's own generic parameters (e.g. CustomNSObject<T>). This mirrors the 'this' of
			// the instance implementation, so EmitCallToExportedMethod can push it with 'ldarg.0'.
			var selfType = ImportTypeReference (module, method.DeclaringType.CreateGenericInstanceType (implMethod.GenericParameters.ToArray ()));
			implMethod.AddParameter (selfType); // self

			// Emit the body (parameter/return conversions + the call to the exported method). Because
			// implMethod has its own generic parameters, EmitCallToExportedMethod remaps the user
			// type's generic parameters to the helper's parameters and uses them for the call.
			EmitCallToExportedMethod (method, implMethod);
			ImportCallbackReferences (implMethod);

			// Build the static UnmanagedCallersOnly callback that dispatches to the helper. Its
			// signature mirrors the helper's (minus 'self', plus the native 'exception_gchandle'
			// pointer): [pobj, sel, p0..., IntPtr* exception_gchandle].
			callback.ReturnType = implMethod.ReturnType;
			for (var i = 1; i < implMethod.Parameters.Count - 1; i++) {
				callback.AddParameter (implMethod.Parameters [i].ParameterType);
			}
			callback.AddParameter (new PointerType (abr.System_IntPtr)); // exception_gchandle

			EmitGenericReflectionDispatch (method, callback, implMethod);

			// Keep the helper (and everything its body references, including the user method) alive
			// through trimming. This is emitted on the companion side only; the user assembly is not
			// modified. The 'ldtoken' reference to implMethod from the callback also keeps it alive.
			abr.AddDynamicDependencyAttribute (callback, implMethod);
		}

		// Emits the body of the UnmanagedCallersOnly callback for a relocated generic trampoline. It
		// resolves the instance, boxes the native arguments into an object array, and calls the
		// runtime helper which closes the generic helper method over the instance's actual generic
		// arguments and invokes it. The exception GCHandle is returned by the helper (reflection
		// can't marshal an 'IntPtr*' output) and written back through the native pointer here.
		void EmitGenericReflectionDispatch (MethodDefinition method, MethodDefinition callback, MethodDefinition implMethod)
		{
			var body = callback.CreateBody (out var il);

			// The helper's leading arguments: 'self' followed by the native arguments ('sel', p0...).
			// This excludes the trailing 'out IntPtr exception_gchandle' slot.
			var leadingCount = implMethod.Parameters.Count - 1;

			// var self = Runtime.GetNSObject (pobj);
			var selfVar = body.AddVariable (abr.Foundation_NSObject);
			il.Emit (OpCodes.Ldarg_0);
			il.Emit (OpCodes.Call, abr.Runtime_GetNSObject__System_IntPtr);
			il.Emit (OpCodes.Stloc, selfVar);

			// var args = new object [leadingCount + 1];
			// The array is allocated with one extra (trailing) slot for the helper's
			// 'out IntPtr exception_gchandle' parameter, so that the runtime helper can pass the
			// array straight to MethodInfo.Invoke without having to allocate and copy a larger one.
			var argsVar = body.AddVariable (new ArrayType (abr.System_Object));
			il.Emit (OpCodes.Ldc_I4, leadingCount + 1);
			il.Emit (OpCodes.Newarr, abr.System_Object);
			il.Emit (OpCodes.Stloc, argsVar);

			// args [0] = self;
			il.Emit (OpCodes.Ldloc, argsVar);
			il.Emit (OpCodes.Ldc_I4_0);
			il.Emit (OpCodes.Ldloc, selfVar);
			il.Emit (OpCodes.Stelem_Ref);

			// args [i] = (object) <native argument i>;   (i: 1..leadingCount-1 -> sel, p0, ...)
			for (var i = 1; i < leadingCount; i++) {
				var parameterType = callback.Parameters [i].ParameterType;
				il.Emit (OpCodes.Ldloc, argsVar);
				il.Emit (OpCodes.Ldc_I4, i);
				il.EmitLoadArgument (i);
				il.Emit (OpCodes.Box, parameterType);
				il.Emit (OpCodes.Stelem_Ref);
			}

			// IntPtr exception_gchandle;
			var exceptionVar = body.AddVariable (abr.System_IntPtr);

			// var rv = Runtime.InvokeGenericRegistrarTrampoline (self, ldtoken (UserType), ldtoken (implMethod), args, out exception_gchandle);
			il.Emit (OpCodes.Ldloc, selfVar);
			il.Emit (OpCodes.Ldtoken, method.DeclaringType);
			il.Emit (OpCodes.Ldtoken, implMethod);
			il.Emit (OpCodes.Ldloc, argsVar);
			il.Emit (OpCodes.Ldloca, exceptionVar);
			il.Emit (OpCodes.Call, abr.Runtime_InvokeGenericRegistrarTrampoline);

			var isVoid = callback.ReturnType.Is ("System", "Void");
			VariableDefinition? resultVar = null;
			if (isVoid) {
				il.Emit (OpCodes.Pop);
			} else {
				resultVar = body.AddVariable (abr.System_Object);
				il.Emit (OpCodes.Stloc, resultVar);
			}

			// *exception_gchandle = exception_gchandle;
			il.EmitLoadArgument (callback.Parameters.Count - 1);
			il.Emit (OpCodes.Ldloc, exceptionVar);
			il.Emit (OpCodes.Stind_I);

			// return (<nativeReturnType>) rv;
			if (!isVoid) {
				var returnType = callback.ReturnType;
				if (returnType.IsValueType) {
					// InvokeGenericRegistrarTrampoline returns null on failure (the exception is
					// reported through exception_gchandle). Unbox_Any on a null value type would
					// throw a NullReferenceException that escapes the UnmanagedCallersOnly boundary,
					// so return default (<nativeReturnType>) instead when the result is null.
					var unboxResult = il.Create (OpCodes.Nop);
					var returnResult = il.Create (OpCodes.Nop);
					var defaultVar = body.AddVariable (returnType);
					il.Emit (OpCodes.Ldloc, resultVar);
					il.Emit (OpCodes.Brtrue, unboxResult);
					// rv is null: return default (<nativeReturnType>)
					il.Emit (OpCodes.Ldloca, defaultVar);
					il.Emit (OpCodes.Initobj, returnType);
					il.Emit (OpCodes.Ldloc, defaultVar);
					il.Emit (OpCodes.Br, returnResult);
					// rv is not null: return (<nativeReturnType>) rv
					il.Append (unboxResult);
					il.Emit (OpCodes.Ldloc, resultVar);
					il.Emit (OpCodes.Unbox_Any, returnType);
					il.Append (returnResult);
				} else {
					il.Emit (OpCodes.Ldloc, resultVar);
					il.Emit (OpCodes.Unbox_Any, returnType);
				}
			}
			il.Emit (OpCodes.Ret);

			body.FinalizeGeneratedBody ();
		}

		// Recursively substitutes generic parameters in a type reference according to the given map.
		// Used when relocating a generic type's trampoline into a companion generic helper method:
		// the user type's generic parameters (owned by the type) are remapped to the helper method's
		// generic parameters.
		static TypeReference RemapGenericParameter (TypeReference type, IDictionary<GenericParameter, GenericParameter> map)
		{
			switch (type) {
			case GenericParameter genericParameter:
				return map.TryGetValue (genericParameter, out var mapped) ? mapped : genericParameter;
			case GenericInstanceType git: {
				var result = new GenericInstanceType (RemapGenericParameter (git.ElementType, map));
				foreach (var argument in git.GenericArguments)
					result.GenericArguments.Add (RemapGenericParameter (argument, map));
				return result;
			}
			case ArrayType array:
				return new ArrayType (RemapGenericParameter (array.ElementType, map), array.Rank);
			case PointerType pointer:
				return new PointerType (RemapGenericParameter (pointer.ElementType, map));
			case ByReferenceType byReference:
				return new ByReferenceType (RemapGenericParameter (byReference.ElementType, map));
			case PinnedType pinned:
				return new PinnedType (RemapGenericParameter (pinned.ElementType, map));
			case RequiredModifierType required:
				return new RequiredModifierType (RemapGenericParameter (required.ModifierType, map), RemapGenericParameter (required.ElementType, map));
			case OptionalModifierType optional:
				return new OptionalModifierType (RemapGenericParameter (optional.ModifierType, map), RemapGenericParameter (optional.ElementType, map));
			default:
				return type;
			}
		}

		// Re-imports every type/method/field reference used by the trampoline (its signature, local
		// variables, instruction operands and exception handlers) into the module the trampoline
		// belongs to. This is needed when we relocate a trampoline into the companion assembly,
		// because the emission logic imports some references into the user module.
		void ImportCallbackReferences (MethodDefinition callback)
		{
			var module = callback.Module;

			callback.ReturnType = ImportTypeReference (module, callback.ReturnType);

			foreach (var parameter in callback.Parameters)
				parameter.ParameterType = ImportTypeReference (module, parameter.ParameterType);

			if (!callback.HasBody)
				return;

			var body = callback.Body;

			foreach (var variable in body.Variables)
				variable.VariableType = ImportTypeReference (module, variable.VariableType);

			foreach (var instruction in body.Instructions) {
				switch (instruction.Operand) {
				case MethodReference methodReference:
					instruction.Operand = ImportMethodReference (module, methodReference);
					break;
				case TypeReference typeReference:
					instruction.Operand = ImportTypeReference (module, typeReference);
					break;
				case FieldReference fieldReference:
					instruction.Operand = ImportFieldReference (module, fieldReference);
					break;
				}
			}

			foreach (var handler in body.ExceptionHandlers) {
				if (handler.CatchType is not null)
					handler.CatchType = ImportTypeReference (module, handler.CatchType);
			}
		}

		// Recursively re-imports a type reference into the given module, handling type
		// specifications (generic instances, arrays, pointers, by-references and modifiers)
		// whose nested types may still belong to another module. ModuleDefinition.ImportReference
		// short-circuits when the outermost element already belongs to the target module, leaving
		// nested types (e.g. the generic arguments of a generic instance) pointing at the original
		// module, which makes writing the module fail.
		static TypeReference ImportTypeReference (ModuleDefinition module, TypeReference type)
		{
			switch (type) {
			case GenericParameter:
				// Generic parameters belong to their owner and must not be imported.
				return type;
			case GenericInstanceType git: {
				var imported = new GenericInstanceType (ImportTypeReference (module, git.ElementType));
				foreach (var argument in git.GenericArguments)
					imported.GenericArguments.Add (ImportTypeReference (module, argument));
				return imported;
			}
			case ArrayType array:
				return new ArrayType (ImportTypeReference (module, array.ElementType), array.Rank);
			case PointerType pointer:
				return new PointerType (ImportTypeReference (module, pointer.ElementType));
			case ByReferenceType byReference:
				return new ByReferenceType (ImportTypeReference (module, byReference.ElementType));
			case PinnedType pinned:
				return new PinnedType (ImportTypeReference (module, pinned.ElementType));
			case SentinelType sentinel:
				return new SentinelType (ImportTypeReference (module, sentinel.ElementType));
			case RequiredModifierType required:
				return new RequiredModifierType (ImportTypeReference (module, required.ModifierType), ImportTypeReference (module, required.ElementType));
			case OptionalModifierType optional:
				return new OptionalModifierType (ImportTypeReference (module, optional.ModifierType), ImportTypeReference (module, optional.ElementType));
			default:
				return module.ImportReference (type);
			}
		}

		// Imports a method reference into the given module. ModuleDefinition.ImportReference can't
		// be relied upon to re-import nested types (it leaves them pointing at the original module
		// when the element method/type already belongs to the target module), so we rebuild the
		// reference with each nested type explicitly imported. This applies both to generic
		// instance methods (their generic arguments) and to methods declared on a generic instance
		// type (the declaring type's generic arguments, e.g. Nullable<nfloat>). Otherwise the module
		// would end up referencing a type declared in another module, and writing it would fail.
		static MethodReference ImportMethodReference (ModuleDefinition module, MethodReference methodReference)
		{
			if (methodReference is GenericInstanceMethod gim) {
				var imported = new GenericInstanceMethod (ImportMethodReference (module, gim.ElementMethod));
				foreach (var argument in gim.GenericArguments)
					imported.GenericArguments.Add (ImportTypeReference (module, argument));
				return imported;
			}

			if (methodReference.DeclaringType is GenericInstanceType) {
				var rebuilt = new MethodReference (methodReference.Name, ImportTypeReference (module, methodReference.ReturnType), ImportTypeReference (module, methodReference.DeclaringType)) {
					HasThis = methodReference.HasThis,
					ExplicitThis = methodReference.ExplicitThis,
					CallingConvention = methodReference.CallingConvention,
				};
				foreach (var parameter in methodReference.Parameters)
					rebuilt.Parameters.Add (new ParameterDefinition (parameter.Name, parameter.Attributes, ImportTypeReference (module, parameter.ParameterType)));
				return rebuilt;
			}

			return module.ImportReference (methodReference);
		}

		// Imports a field reference into the given module, rebuilding it when the declaring type is
		// a generic instance so the declaring type's generic arguments are re-imported (see
		// ImportMethodReference for the rationale).
		static FieldReference ImportFieldReference (ModuleDefinition module, FieldReference fieldReference)
		{
			if (fieldReference.DeclaringType is GenericInstanceType)
				return new FieldReference (fieldReference.Name, ImportTypeReference (module, fieldReference.FieldType), ImportTypeReference (module, fieldReference.DeclaringType));

			return module.ImportReference (fieldReference);
		}

		public void EmitCallToProxyMethod (MethodDefinition method, MethodDefinition callback, MethodDefinition proxyInterfaceMethod)
		{
			var body = callback.CreateBody (out var il);

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

			body.FinalizeGeneratedBody ();
		}

		public void EmitCallToExportedMethod (MethodDefinition method, MethodDefinition callback)
		{
			var baseMethod = StaticRegistrar.GetBaseMethodInTypeHierarchy (method);
			var placeholderType = abr.System_IntPtr;
			ParameterDefinition? callSuperParameter = null;
			VariableDefinition? returnVariable = null;
			VariableDefinition? relocatedCtorObjVar = null;
			var relocate = ShouldRelocateTrampolines (method);
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

			// When 'callback' is itself a generic method (i.e. it has its own generic parameters), we
			// are emitting the relocated generic helper into the companion assembly: a static method
			// mirroring the user type's generic parameters, whose 'self' argument (arg0) is the user
			// type closed over the helper's own generic parameters. The body is identical to the
			// instance implementation, except every reference to the user type's generic parameters
			// must be remapped to the helper's generic parameters.
			var genericCompanionImpl = callback.HasGenericParameters;
			Dictionary<GenericParameter, GenericParameter>? genericParameterMap = null;
			if (genericCompanionImpl) {
				genericParameterMap = new Dictionary<GenericParameter, GenericParameter> ();
				for (var i = 0; i < method.DeclaringType.GenericParameters.Count; i++)
					genericParameterMap [method.DeclaringType.GenericParameters [i]] = callback.GenericParameters [i];
			}
			TypeReference RemapType (TypeReference tr) => genericParameterMap is null ? tr : RemapGenericParameter (tr, genericParameterMap);

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
				} else if (method.DeclaringType.IsAbstract) {
					il.Emit (OpCodes.Ldc_I4, 4180);
					postLeaveBranch.Operand = il.Body.Instructions.Last ();
					il.Emit (OpCodes.Ldstr, $"Cannot construct an instance of the type '{method.DeclaringType.FullName}' from Objective-C because the type is abstract.");
					il.Emit (OpCodes.Call, abr.Runtime_CreateRuntimeException);
					il.Emit (OpCodes.Throw);
					// We're throwing an exception, so there's no need for any more code.
					skipEverythingAfter = il.Body.Instructions.Last ();
				} else {
					var declType = abr.CurrentAssembly.MainModule.ImportReference (method.DeclaringType);
					relocatedCtorObjVar = body.AddVariable (declType);

					// Reuse a compatible wrapper stored in the native object while an initializer
					// is running, or create an uninitialized wrapper for a newly surfaced object.
					il.Emit (OpCodes.Ldarg_0);
					var firstFactoryInstruction = il.Body.Instructions.Last ();
					il.Emit (OpCodes.Ldc_I4_2); // Flags.NativeRef == 2
					il.Emit (OpCodes.Call, abr.Foundation_NSObject_GetOrCreateNSObjectForConstructor.CreateGenericInstanceMethod (declType));
					il.Emit (OpCodes.Stloc, relocatedCtorObjVar);

					// Push the object so it ends up underneath the constructor arguments emitted next.
					il.Emit (OpCodes.Ldloc, relocatedCtorObjVar);

					postLeaveBranch.Operand = firstFactoryInstruction;

					if (relocate) {
						var companion = Configuration.RegistrarCompanionAssemblies [method.Module.Assembly];
						companion.AccessesAssemblies.Add (abr.PlatformAssembly);
					}
				}
			} else if (isGeneric) {
				// this is a proxy method and we can simply use `this` without any conversion
				il.Emit (OpCodes.Ldarg_0);
			} else {
				// instance method
				il.Emit (OpCodes.Ldarg_0);
				EmitConversion (method, il, method.DeclaringType, true, -1, out var nativeType, postProcessing);
			}

			callback.AddParameter (abr.System_IntPtr); // sel

			var managedParameterCount = 0;
			var nativeParameterOffset = isInstanceCategory ? 1 : 2;
			var parameterStart = isInstanceCategory ? 1 : 0;
			if (method.HasParameters)
				managedParameterCount = method.Parameters.Count;

			if (method.HasParameters) {
				for (var p = parameterStart; p < managedParameterCount; p++) {
					var nativeParameter = callback.AddParameter (placeholderType); // p{p}
					var nativeParameterIndex = p + nativeParameterOffset;
					var managedParameterType = RemapType (method.Parameters [p].ParameterType);
					var baseParameter = baseMethod.Parameters [p];
					var isOutParameter = IsOutParameter (method, p, baseParameter);

					if (!isOutParameter) {
						il.EmitLoadArgument (nativeParameterIndex);
					}

					if (EmitConversion (method, il, managedParameterType, true, p, out var nativeType, postProcessing, isOutParameter, nativeParameterIndex)) {
						// In the generic companion helper the trampoline is dispatched via reflection,
						// which boxes each native argument into an 'object []' and calls
						// MethodInfo.Invoke. A pointer-typed native parameter (which is what
						// out/ref/pointer parameters produce) can't survive that round-trip: a pointer
						// isn't boxable and Invoke can't marshal it. Represent such parameters as a
						// plain 'IntPtr' instead - it's a boxable, pointer-sized value type, and loading
						// it with 'ldarg' yields a native int that the already-emitted body uses
						// directly as the pointer (native int is usable wherever an unmanaged pointer is
						// expected, for both reads and writes). The native ABI is unchanged since a
						// pointer and an IntPtr are both pointer-sized, and the out/ref write-back still
						// happens through that same pointer value inside the helper body.
						nativeParameter.ParameterType = genericCompanionImpl && nativeType is PointerType ? abr.System_IntPtr : nativeType;
					} else {
						nativeParameter.ParameterType = placeholderType;
						AddException (ErrorHelper.CreateError (99, "Unable to emit conversion for parameter {2} of type {0}. Method: {1}", method.Parameters [p].ParameterType, GetMethodSignatureWithSourceCode (method), p));
					}
				}
			}

			if (callSuperParameter is not null)
				callback.Parameters.Add (callSuperParameter);

			// For the relocated generic companion helper the trampoline is dispatched via reflection,
			// which can't marshal an 'IntPtr*' (pointer) argument, so we use a byref 'out IntPtr'
			// (which reflection handles as a standard output parameter). The runtime helper writes the
			// resulting GCHandle into the UnmanagedCallersOnly callback's native 'IntPtr*' pointer.
			if (genericCompanionImpl) {
				callback.AddParameter (new ByReferenceType (abr.System_IntPtr)); // exception_gchandle
			} else {
				callback.AddParameter (new PointerType (abr.System_IntPtr)); // exception_gchandle
			}

			if (relocatedCtorObjVar is not null) {
				// The object was allocated earlier and pushed onto the stack (underneath the
				// constructor arguments), so we can call the real constructor directly on it, and
				// then push the object again for the return-value conversion (which converts it to
				// the native handle).
				il.Emit (OpCodes.Call, method);
				il.Emit (OpCodes.Ldloc, relocatedCtorObjVar);
			} else if (isGeneric && !method.IsConstructor) {
				var genericArguments = genericCompanionImpl
					? callback.GenericParameters.ToArray ()
					: method.DeclaringType.GenericParameters.ToArray ();
				var targetMethod = method.DeclaringType.CreateMethodReferenceOnGenericType (method, genericArguments);
				il.Emit (OpCodes.Call, targetMethod);
			} else if (method.IsStatic) {
				il.Emit (OpCodes.Call, method);
			} else {
				il.Emit (OpCodes.Callvirt, method);
			}

			if (returnVariable is not null) {
				if (EmitConversion (method, il, RemapType (method.ReturnType), false, -1, out var nativeReturnType, postProcessing)) {
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

			AddExceptionHandler (il, returnVariable, placeholderNextInstruction, out var eh, out var leaveEHInstruction);

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

			body.FinalizeGeneratedBody ();
		}

		void AddExceptionHandler (ILProcessor il, VariableDefinition? returnVariable, Instruction placeholderNextInstruction, out ExceptionHandler eh, out Instruction leaveEHInstruction)
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
			// The exception_gchandle output pointer is always the last parameter of the trampoline
			// (whether the trampoline is a static callback or an instance/static implementation
			// method). Account for the implicit 'this' argument when the method has one.
			il.Emit (OpCodes.Ldarg, (method.HasThis ? 1 : 0) + method.Parameters.Count - 1);
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
			BindAsAttribute? attribute;
			var isPropertyAccessor = StaticRegistrar.IsPropertyAccessor (method, out var property);
			if (isPropertyAccessor) {
				attribute = StaticRegistrar.GetBindAsAttribute (property);
			} else {
				attribute = StaticRegistrar.GetBindAsAttribute (method, parameter);
			}

			if (attribute is not null || parameter < 0 || isPropertyAccessor || method.IsConstructor || method.DeclaringType.IsInterface || StaticRegistrar.GetCategoryAttribute (method.DeclaringType) is not null)
				return attribute;

			// Parameter attributes aren't inherited from protocol methods, so look up the mapped interface method.
			var methodMap = StaticRegistrar.PrepareInterfaceMethodMapping (method.DeclaringType);
			if (methodMap is null || !methodMap.TryGetValue (method, out var interfaceMethods))
				return null;

			List<MethodDefinition>? bindAsInterfaceMethods = null;
			foreach (var interfaceMethod in interfaceMethods) {
				var interfaceAttribute = StaticRegistrar.GetBindAsAttribute (interfaceMethod, parameter);
				if (interfaceAttribute is null)
					continue;

				bindAsInterfaceMethods ??= new List<MethodDefinition> ();
				bindAsInterfaceMethods.Add (interfaceMethod);
				attribute = interfaceAttribute;
			}

			if (bindAsInterfaceMethods is not null && interfaceMethods.Count != 1)
				throw new AggregateException (Shared.GetMT4127 (method, interfaceMethods));

			return attribute;
		}

		// This emits a conversion between the native and the managed representation of a parameter or return value,
		// and returns the corresponding native type. The returned nativeType will (must) be a blittable type.
		bool EmitConversion (MethodDefinition method, ILProcessor il, TypeReference type, bool toManaged, int parameter, [NotNullWhen (true)] out TypeReference? nativeType, List<Instruction> postProcessing, bool isOutParameter = false, int nativeParameterIndex = -1)
		{
			nativeType = null;

			if (!(parameter == -1 && !method.IsStatic && method.DeclaringType == type)) {
				var bindAsAttribute = GetBindAsAttribute (method, parameter);
				if (bindAsAttribute is not null) {
					if (bindAsAttribute.OriginalType is null) {
						AddException (ErrorHelper.CreateError (99, "BindAs attribute without OriginalType. Method: {0}", GetMethodSignatureWithSourceCode (method)));
						return false;
					}
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
					var elementType = brt.ElementType!;
					if (elementType is GenericParameter gp) {
						if (!StaticRegistrar.VerifyIsConstrainedToNSObject (gp, out var constrained)) {
							AddException (ErrorHelper.CreateError (99, "Incorrectly constrained generic parameter. Method: {0}", GetMethodSignatureWithSourceCode (method)));
							return false;
						}
						if (constrained is null) {
							AddException (ErrorHelper.CreateError (99, "Incorrectly constrained generic parameter (2). Method: {0}", GetMethodSignatureWithSourceCode (method)));
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

				var gp = elementType as GenericParameter;
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
							// When emitting the relocated generic companion helper, the generic parameter has
							// already been remapped to one of the helper method's own generic parameters, and
							// must be used as-is (a type generic parameter isn't valid in a static method).
							var gemericParameter = gp.Type == GenericParameterType.Method ? gp : method.DeclaringType.GenericParameters.Single (x => x.Name == gp.Name);
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

				AddException (ErrorHelper.CreateError (99, "Don't know how (3) to convert array element type {1} for array type {0} between managed and native code. Method: {2}", type.FullName, elementType?.FullName, GetMethodSignatureWithSourceCode (method)));
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
						if (App.TrimExportAttributes != false)
							App.TrimExportAttributesBlockers.Add (ExportAttributeRemovalBlocker.RuntimeGetBlockWrapperCreatorRequired);
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
						if (App.TrimExportAttributes != false)
							App.TrimExportAttributesBlockers.Add (ExportAttributeRemovalBlocker.RegistrarHelperGetBlockForDelegateRequired);
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

		void EnsureVisible (MethodDefinition caller, FieldDefinition field)
		{
			if (ShouldRelocateTrampolines (caller)) {
				Configuration.RegistrarCompanionAssemblies [caller.Module.Assembly].AccessesAssemblies.Add (field.Module.Assembly);
				return;
			}

			field.IsPublic = true;
			EnsureVisible (caller, field.DeclaringType);
		}

		void EnsureVisible (MethodDefinition caller, TypeDefinition type)
		{
			if (ShouldRelocateTrampolines (caller)) {
				Configuration.RegistrarCompanionAssemblies [caller.Module.Assembly].AccessesAssemblies.Add (type.Module.Assembly);
				return;
			}

			if (type.IsNested) {
				type.IsNestedPublic = true;
				EnsureVisible (caller, type.DeclaringType);
			} else {
				type.IsPublic = true;
			}
		}

		void EnsureVisible (MethodDefinition caller, MethodReference method)
		{
			var md = method.Resolve ();
			if (ShouldRelocateTrampolines (caller)) {
				Configuration.RegistrarCompanionAssemblies [caller.Module.Assembly].AccessesAssemblies.Add (md.Module.Assembly);
				return;
			}

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

		CustomAttribute CreateUnmanagedCallersAttribute (string entryPoint, TypeReference? associatedSourceType = null)
		{
			var unmanagedCallersAttribute = new CustomAttribute (abr.UnmanagedCallersOnlyAttribute_Constructor);

			// The AssociatedSourceType field tells the NativeAOT compiler (ILC) that the trampoline's
			// native export is only needed if the associated type is kept. This only works safely when
			// all of these are true:
			// * We're compiling for NativeAOT (only ILC understands the field; the CoreCLR runtime's
			//   attribute parser even rejects any UnmanagedCallersOnly named argument it doesn't know).
			// * We're using the trimmable static registrar, whose native glue references the trampoline
			//   symbols directly. This means ILC may drop the native export for a trampoline whose
			//   associated type can't be constructed, while the generated native registrar code still
			//   references it - which would result in an undefined symbol at native link time.
			// * We're preparing assemblies (PrepareAssemblies), which is the mode that has a post-ILC
			//   step (the _PostprocessAssembliesAfterIlc target) that regenerates the native registrar
			//   code after ILC, routing any trampoline that didn't survive ILC through the dlsym fallback
			//   instead of a direct native reference. Without that reconciliation step (i.e. in the
			//   non-prepare mode) we must not let ILC drop any trampoline export, so we don't emit the
			//   field there.
			if (associatedSourceType is not null
				&& App.XamarinRuntime == XamarinRuntime.NativeAOT
				&& App.Registrar == RegistrarMode.TrimmableStatic
				&& App.PrepareAssemblies) {
				// Import the type into the current assembly, otherwise Cecil will serialize the Type argument
				// without an assembly-qualified name when 'associatedSourceType' is a TypeDefinition from another
				// assembly (because a TypeDefinition's Scope is its own module), and ILC won't be able to resolve
				// it (the trampolines are emitted into the companion assembly, while the associated type is in the
				// user assembly).
				var importedSourceType = abr.CurrentAssembly.MainModule.ImportReference (associatedSourceType);
				unmanagedCallersAttribute.Fields.Add (new CustomAttributeNamedArgument ("AssociatedSourceType", new CustomAttributeArgument (abr.System_Type, importedSourceType)));
			}

			if (App.XamarinRuntime != XamarinRuntime.CoreCLR)
				unmanagedCallersAttribute.Fields.Add (new CustomAttributeNamedArgument ("EntryPoint", new CustomAttributeArgument (abr.System_String, entryPoint)));

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
				if (underlyingManagedType is null)
					throw ErrorHelper.CreateError (99, Errors.MX0099, $"can't convert from '{inputType.FullName}' to '{outputType.FullName}' in {descriptiveMethodName}: {managedType.FullName} is not a nullable type");
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
				if (underlyingManagedType is null)
					throw ErrorHelper.CreateError (99, Errors.MX0099, $"can't convert from '{inputType.FullName}' to '{outputType.FullName}' in {descriptiveMethodName}: {managedType.FullName} is not a nullable type");
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
					ErrorHelper.Show (App, ErrorHelper.CreateError (99, Errors.MX0099, $"the smart enum {underlyingManagedType.FullName} doesn't seem to be a smart enum after all"));
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

				var body = registerToggleRef!.CreateBody (out var il);
				il.Emit (OpCodes.Ret);
				body.FinalizeGeneratedBody ();
			}
		}
	}
}
