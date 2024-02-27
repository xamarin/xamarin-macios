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
using System.Diagnostics;

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
		protected override int ErrorCode { get; } = 2390;

		Profile Profile => new Profile (Configuration);

		// Get the reverse mapping from assemblies to assemblies which reference them directly.
		Dictionary<AssemblyDefinition, HashSet<AssemblyDefinition>> _reversedReferences;
		Dictionary<AssemblyDefinition, HashSet<AssemblyDefinition>> GetReversedReferences ()
		{
			if (_reversedReferences is not null)
				return _reversedReferences;

			_reversedReferences = new Dictionary<AssemblyDefinition, HashSet<AssemblyDefinition>> ();
			foreach (var assembly in Configuration.Assemblies) {
				if (!_reversedReferences.ContainsKey (assembly))
					_reversedReferences.Add (assembly, new HashSet<AssemblyDefinition> ());

				foreach (var reference in assembly.MainModule.AssemblyReferences) {
					var resolvedReference = Configuration.Context.GetLoadedAssembly (reference.Name);
					if (resolvedReference is null)
						continue;

					if (!_reversedReferences.TryGetValue (resolvedReference, out var referrers)) {
						referrers = new HashSet<AssemblyDefinition> ();
						_reversedReferences.Add (resolvedReference, referrers);
					}

					referrers.Add (assembly);
				}
			}
			return _reversedReferences;
		}

		Dictionary<AssemblyDefinition, bool> _transitivelyReferencesProduct;
		bool TransitivelyReferencesProduct (AssemblyDefinition assembly)
		{
			if (_transitivelyReferencesProduct is not null) {
				Debug.Assert (_transitivelyReferencesProduct.ContainsKey (assembly));
				return _transitivelyReferencesProduct.TryGetValue (assembly, out bool result) && result;
			}

			_transitivelyReferencesProduct = new Dictionary<AssemblyDefinition, bool> ();

			// A depth-first search is insufficient because there are reference cycles, so we
			// get the set of transitive references, and do a reverse BFS.
			var reversedReferences = GetReversedReferences ();
			Debug.Assert (reversedReferences.ContainsKey (assembly));
			var referencesProductToProcess = new Queue<AssemblyDefinition> ();

			// We start the BFS from the product assembly.
			foreach (var reference in reversedReferences.Keys) {
				if (Profile.IsProductAssembly (reference)) {
					_transitivelyReferencesProduct.Add (reference, true);
					referencesProductToProcess.Enqueue (reference);
				}
			}

			// Scan the reverse references to find out which referencing assemblies
			// are reachable from the product assembly (that is, transitively reference the product).
			while (referencesProductToProcess.TryDequeue (out var reference)) {
				foreach (var referrer in reversedReferences [reference]) {
					if (_transitivelyReferencesProduct.TryGetValue (referrer, out bool referencesProduct)) {
						Debug.Assert (referencesProduct);
						// Any which were already determined to reference the product assembly
						// don't need to be scanned again.
						continue;
					}

					_transitivelyReferencesProduct.Add (referrer, true);
					referencesProductToProcess.Enqueue (referrer);
				}
			}

			// Any remaining references that we didn't discover during the search
			// don't reference the product assembly.
			foreach (var reference in reversedReferences.Keys)
				_transitivelyReferencesProduct.TryAdd (reference, false);

			return _transitivelyReferencesProduct [assembly];
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
			if (type is null)
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
				while (base_type is not null && IsNSObject (base_type)) {
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
				while (base_type is not null && IsNSObject (base_type)) {
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
