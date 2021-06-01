//
// CoreTypeMapStep.cs
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012-2013 Xamarin Inc.
//

using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Linker.Steps;
using Mono.Tuner;

using Xamarin.Linker;
using Xamarin.Tuner;

namespace MonoTouch.Tuner {

	// This class is shared between Xamarin.Mac and Xamarin.iOS
	public class CoreTypeMapStep :
#if NET
		ConfigurationAwareStep
#else
		TypeMapStep
#endif
	{

#if NET
		protected override string Name { get; } = "CoreTypeMap";
		protected override int ErrorCode { get; } = 2381;

		Profile Profile => new Profile (Configuration);

		Dictionary<AssemblyDefinition, bool> _transitivelyReferencesProduct = new Dictionary<AssemblyDefinition, bool> ();
		bool TransitivelyReferencesProduct (AssemblyDefinition assembly)
		{
			if (_transitivelyReferencesProduct.TryGetValue (assembly, out bool result))
				return result;

			if (Profile.IsProductAssembly (assembly)) {
				_transitivelyReferencesProduct.Add (assembly, true);
				return true;
			}

			foreach (var reference in assembly.MainModule.AssemblyReferences) {
				var resolvedReference = Configuration.Context.GetLoadedAssembly (reference.Name);
				if (resolvedReference == null)
					continue;

				if (TransitivelyReferencesProduct (resolvedReference)) {
					_transitivelyReferencesProduct.Add (assembly, true);
					return true;
				}
			}

			_transitivelyReferencesProduct.Add (assembly, false);
			return false;
		}

		protected override void TryProcessAssembly (AssemblyDefinition assembly)
		{
			// We are only interested in types transitively derived from NSObject,
			// which lives in the product assembly.
			if (!TransitivelyReferencesProduct (assembly))
				return;

			foreach (var type in assembly.MainModule.Types)
				ProcessType (type);
		}

		void ProcessType (TypeDefinition type)
		{
			MapType (type);

			if (!type.HasNestedTypes)
				return;

			foreach (var nestedType in type.NestedTypes)
				ProcessType (nestedType);
		}

		DerivedLinkContext LinkContext => Configuration.DerivedLinkContext;
#else
		DerivedLinkContext LinkContext {
			get {
				return (DerivedLinkContext) base.Context;
			}
		}
#endif

		HashSet<TypeDefinition> cached_isnsobject = new HashSet<TypeDefinition> ();
		Dictionary<TypeDefinition, bool?> isdirectbinding_value = new Dictionary<TypeDefinition, bool?> ();

#if NET
		protected override void TryEndProcess ()
		{
#else
		protected override void EndProcess ()
		{
			base.EndProcess ();
#endif

			LinkContext.CachedIsNSObject = cached_isnsobject;
			LinkContext.IsDirectBindingValue = isdirectbinding_value;
		}

		protected
#if !NET
		override
#endif
		void MapType (TypeDefinition type)
		{
#if !NET
			base.MapType (type);
#endif

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
				rv = type.Is (Namespaces.CoreImage, "CIFilter") || IsCIFilter (Context.Resolve (type).BaseType);
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
				var base_type = Context.Resolve (type.BaseType);
				while (base_type != null && IsNSObject (base_type)) {
					isdirectbinding_value [base_type] = null;
					base_type = Context.Resolve (base_type.BaseType);
				}
				return;
			}

			var isWrapperType = IsWrapperType (type);

			if (!isWrapperType) {
				isdirectbinding_value [type] = false;

				// We must clear IsDirectBinding for any wrapper superclasses.
				var base_type = Context.Resolve (type.BaseType);
				while (base_type != null && IsNSObject (base_type)) {
					if (IsWrapperType (base_type))
						isdirectbinding_value [base_type] = null;
					base_type = Context.Resolve (base_type.BaseType);
				}
			} else {
				isdirectbinding_value [type] = true; // Let's try 'true' first, any derived non-wrapper classes will clear it if needed
			}
		}
	}
}