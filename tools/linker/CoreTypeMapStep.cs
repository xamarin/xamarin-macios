//
// CoreTypeMapStep.cs
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012-2013 Xamarin Inc.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Mono.Cecil;
using Mono.Linker.Steps;
using Mono.Tuner;

using Xamarin.Bundler;
using Xamarin.Linker;
using Xamarin.Tuner;

namespace MonoTouch.Tuner {

	// This class is shared between Xamarin.Mac and Xamarin.iOS
	public class CoreTypeMapStep : TypeMapStep {
		HashSet<TypeDefinition> cached_isnsobject = new HashSet<TypeDefinition> ();
		Dictionary<TypeDefinition, bool?> isdirectbinding_value = new Dictionary<TypeDefinition, bool?> ();
		bool dynamic_registration_support_required;

		DerivedLinkContext LinkContext {
			get {
				return (DerivedLinkContext) base.Context;
			}
		}

		protected override void ProcessAssembly (AssemblyDefinition assembly)
		{
			if (LinkContext.App.Optimizations.RemoveDynamicRegistrar != false)
				dynamic_registration_support_required |= RequiresDynamicRegistrar (assembly, LinkContext.App.Optimizations.RemoveDynamicRegistrar == true);

			base.ProcessAssembly (assembly);
		}

		// If certain conditions are met, we can optimize away the code for the dynamic registrar.
		bool RequiresDynamicRegistrar (AssemblyDefinition assembly, bool warnIfRequired)
		{
			// Disable removing the dynamic registrar for XM/Classic to simplify the code a little bit.
			if (!Driver.IsUnified)
				return true;

			// We know that the SDK assemblies we ship don't use the methods we're looking for.
			if (Profile.IsSdkAssembly (assembly))
				return false;

			// The product assembly itself is safe as long as it's linked
			if (Profile.IsProductAssembly (assembly))
				return LinkContext.Annotations.GetAction (assembly) != Mono.Linker.AssemblyAction.Link;

			// Can't touch the forbidden fruit in the product assembly unless there's a reference to it
			var hasProductReference = false;
			foreach (var ar in assembly.MainModule.AssemblyReferences) {
				if (!Profile.IsProductAssembly (ar.Name))
					continue;
				hasProductReference = true;
				break;
			}
			if (!hasProductReference)
				return false;

			// Check if the assembly references any methods that require the dynamic registrar
			var productAssemblyName = ((MobileProfile) Profile.Current).ProductAssembly;
			var requires = false;
			foreach (var mr in assembly.MainModule.GetMemberReferences ()) {
				if (mr.DeclaringType == null || string.IsNullOrEmpty (mr.DeclaringType.Namespace))
					continue;
				
				var scope = mr.DeclaringType.Scope;
				var name = string.Empty;
				switch (scope.MetadataScopeType) {
				case MetadataScopeType.ModuleDefinition:
					name = ((ModuleDefinition) scope).Assembly.Name.Name;
					break;
				default:
					name = scope.Name;
					break;
				}
				if (name != productAssemblyName)
					continue;

				switch (mr.DeclaringType.Namespace) {
				case "ObjCRuntime":
					switch (mr.DeclaringType.Name) {
					case "Runtime":
						switch (mr.Name) {
						case "ConnectMethod":
							// Req 1: Nobody must call Runtime.ConnectMethod.
							if (warnIfRequired)
								Show2107 (assembly, mr);
							requires = true;
							break;
						case "RegisterAssembly":
							// Req 3: Nobody must call Runtime.RegisterAssembly
							if (warnIfRequired)
								Show2107 (assembly, mr);
							requires = true;
							break;
						}
						break;
					case "BlockLiteral":
						switch (mr.Name) {
						case "SetupBlock":
						case "SetupBlockUnsafe":
							// Req 2: Nobody must call BlockLiteral.SetupBlock[Unsafe].
							//
							// Fortunately the linker is able to rewrite calls to SetupBlock[Unsafe] to call
							// SetupBlockImpl (which doesn't need the dynamic registrar), which means we only have
							// to look in assemblies that aren't linked.
							if (LinkContext.Annotations.GetAction (assembly) == Mono.Linker.AssemblyAction.Link && LinkContext.App.Optimizations.OptimizeBlockLiteralSetupBlock == true)
								break;

							if (warnIfRequired)
								Show2107 (assembly, mr);

							requires = true;
							break;
						}
						break;
					case "TypeConverter":
						switch (mr.Name) {
						case "ToManaged":
							// Req 4: Nobody must call TypeConverter.ToManaged
							if (warnIfRequired)
								Show2107 (assembly, mr);
							requires = true;
							break;
						}
						break;
					}
					break;
				}
			}

			return requires;
		}

		void Show2107 (AssemblyDefinition assembly, MemberReference mr)
		{
			ErrorHelper.Warning (2107, $"It's not safe to remove the dynamic registrar, because {assembly.Name.Name} references '{mr.DeclaringType.FullName}.{mr.Name} ({string.Join (", ", ((MethodReference) mr).Parameters.Select ((v) => v.ParameterType.FullName))})'.");
		}

		protected override void EndProcess ()
		{
			base.EndProcess ();

			LinkContext.CachedIsNSObject = cached_isnsobject;
			LinkContext.IsDirectBindingValue = isdirectbinding_value;

			if (!LinkContext.App.Optimizations.RemoveDynamicRegistrar.HasValue) {
				// If dynamic registration is not required, and removal of the dynamic registrar hasn't already
				// been disabled, then we can remove it!
				LinkContext.App.Optimizations.RemoveDynamicRegistrar = !dynamic_registration_support_required;
				Driver.Log (4, "Optimization dynamic registrar removal: {0}", LinkContext.App.Optimizations.RemoveDynamicRegistrar.Value ? "enabled" : "disabled");
			}
		}

		protected override void MapType (TypeDefinition type)
		{
			base.MapType (type);

			// additional checks for NSObject to check if the type is a *generated* bindings
			// bonus: we cache, for every type, whether or not it inherits from NSObject (very useful later)
			if (!IsNSObject (type))
				return;
			
			// if not, it's a user type, the IsDirectBinding check is required by all ancestors
			SetIsDirectBindingValue (type);
		}
		
		// called once for each 'type' so it's a nice place to cache the result
		// and ensure later steps re-use the same, pre-computed, result
		bool IsNSObject (TypeDefinition type)
		{
			if (!type.IsNSObject (LinkContext))
				return false;
			cached_isnsobject.Add (type);
			return true;
		}

		bool IsWrapperType (TypeDefinition type)
		{
			var registerAttribute = LinkContext.StaticRegistrar.GetRegisterAttribute (type);
			return registerAttribute?.IsWrapper == true || registerAttribute?.SkipRegistration == true;
		}

		// Cache the results of the IsCIFilter check in a dictionary. It makes this method slightly faster
		// (total time spent in IsCIFilter when linking monotouch-test went from 11 ms to 3ms).
		static Dictionary<TypeReference, bool> ci_filter_types = new Dictionary<TypeReference, bool> ();
		bool IsCIFilter (TypeReference type)
		{
			if (type == null)
				return false;

			bool rv;
			if (!ci_filter_types.TryGetValue (type, out rv)) {
				rv = type.Is (Namespaces.CoreImage, "CIFilter") || IsCIFilter (type.Resolve ().BaseType);
				ci_filter_types [type] = rv;
			}
			return rv;
		}
		
		void SetIsDirectBindingValue (TypeDefinition type)
		{
			if (isdirectbinding_value.ContainsKey (type))
				return;

			// We have a special implementation of CIFilters, and we do not want to 
			// optimize anything for those classes to not risk optimizing this wrong.
			// This means we must set the IsDirectBinding value to null for CIFilter
			// and all its base classes to allow both code paths and determine at runtime.
			// References:
			// * https://github.com/xamarin/xamarin-macios/pull/3055
			// * https://bugzilla.xamarin.com/show_bug.cgi?id=15465
			if (IsCIFilter (type)) {
				isdirectbinding_value [type] = null;
				var base_type = type.BaseType.Resolve ();
				while (base_type != null && IsNSObject (base_type)) {
					isdirectbinding_value [base_type] = null;
					base_type = base_type.BaseType.Resolve ();
				}
				return;
			}

			var isWrapperType = IsWrapperType (type);

			if (!isWrapperType) {
				isdirectbinding_value [type] = false;

				// We must clear IsDirectBinding for any wrapper superclasses.
				var base_type = type.BaseType.Resolve ();
				while (base_type != null && IsNSObject (base_type)) {
					if (IsWrapperType (base_type))
						isdirectbinding_value [base_type] = null;
					base_type = base_type.BaseType.Resolve ();
				}
			} else {
				isdirectbinding_value [type] = true; // Let's try 'true' first, any derived non-wrapper classes will clear it if needed
			}
		}
	}
}