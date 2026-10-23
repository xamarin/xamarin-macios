// Copyright 2017 Xamarin Inc.

using System.IO;
using System.Linq;

using Mono.Cecil;
using Mono.Linker;
using Mono.Linker.Steps;
using Mono.Tuner;

using Xamarin.Bundler;
using Xamarin.Tuner;

#nullable enable

namespace Xamarin.Linker.Steps {
	public class PreserveSmartEnumConversionsStep : AssemblyModifierStep {
		protected override string Name { get; } = "Smart Enum Conversion Preserver";
		protected override int ErrorCode { get; } = 2200;

		// When set for a given assembly, we can't modify it (that would break Hot Reload), so instead of
		// injecting [DynamicDependency] attributes into the referencing (possibly user) assembly, we collect
		// the framework-side conversion methods and emit an ILLink root-descriptor XML that preserves them
		// unconditionally. This only makes sense in the assembly-preparer, where the descriptor is consumed
		// by the subsequent trimmer pass.
		//
		// Assemblies that are trimmed can't be hot reloaded, so we can modify those, and it's important that
		// we do: preserving every smart enum conversion in our platform assembly unconditionally would keep
		// a lot of code (and thus native frameworks) alive that should have been trimmed away.
		bool UseXmlDescriptionFile (AssemblyDefinition assembly)
		{
			return Configuration.HotReloadCompatibleBuild && Annotations.GetAction (assembly) != AssemblyAction.Link;
		}

		// The framework-side conversion methods to preserve via the root-descriptor XML.
		readonly XmlDescriptor xmlDescriptor = new ();

		PreserveSmartEnumConversion? preserver;
		PreserveSmartEnumConversion Preserver {
			get {
				if (preserver is null)
					preserver = new PreserveSmartEnumConversion (DerivedLinkContext, Preserve);
				return preserver;
			}
		}

		protected override bool IsActiveFor (AssemblyDefinition assembly)
		{
			// We have to process assemblies that aren't linked, because type A from the unlinked assembly X
			// might reference the smart enum B from the linked assembly Y,
			// and we need to make sure that smart enum B's conversion methods aren't trimmed away - which
			// means adding dynamic dependency attributes to the methods in the unlinked assembly X,
			// which means we need to process the unlinked assembly X.

			// Hot Reload: we can't modify user assemblies when Hot Reload is enabled (otherwise Hot Reload won't
			// work), so in that case (see UseXmlDescriptionFile) we emit an xml root-descriptor instead of
			// injecting [DynamicDependency] attributes into the referencing assembly.

			// Unless an assembly is or references our platform assembly, then it won't have anything we need to preserve
			if (!Configuration.Profile.IsOrReferencesProductAssembly (assembly))
				return false;

			return true;
		}

		bool Preserve (Tuple<MethodDefinition, MethodDefinition> pair, bool alreadyProcessed, params MethodDefinition? [] conditions)
		{
			var conds = conditions.Where (v => v is not null).Cast<MethodDefinition> ().ToArray ();
			if (conds.Length == 0)
				return false;

			var modified = false;
			foreach (var condition in conds) {
				if (UseXmlDescriptionFile (condition.Module.Assembly)) {
					// Don't modify the (possibly user) assembly: collect the framework-side conversion methods
					// so they can be preserved unconditionally via a root-descriptor XML instead.
					xmlDescriptor.PreserveMethod (pair.Item1);
					xmlDescriptor.PreserveMethod (pair.Item2);
					continue;
				}

				modified |= abr.AddDynamicDependencyAttribute (condition, pair.Item1);
				modified |= abr.AddDynamicDependencyAttribute (condition, pair.Item2);
			}

			return modified;
		}

		protected override void TryEndProcess ()
		{
			if (xmlDescriptor.IsEmpty)
				return;

			var xmlPath = Path.Combine (Configuration.CacheDirectory, "preserve-smart-enum-conversions.xml");
			xmlDescriptor.Save (xmlPath);

			// The descriptor is consumed by the subsequent trimmer pass, which MSBuild wires into
			// TrimmerRootDescriptor after reading this output property (see _SetSmartEnumConversionsRootDescriptor).
			Configuration.SetOutputForMSBuild ("SmartEnumConversionsRootDescriptor", xmlPath);
		}

		protected override bool ProcessType (TypeDefinition type)
		{
			return base.ProcessMethods (type);
		}

		protected override bool ProcessMethod (MethodDefinition method)
		{
			static bool IsPropertyMethod (MethodDefinition method)
			{
				return method.IsGetter || method.IsSetter;
			}

			var modified = false;
			modified |= Preserver.ProcessAttributeProvider (method, method);
			modified |= Preserver.ProcessAttributeProvider (method.MethodReturnType, method);

			if (method.HasParameters) {
				foreach (var p in method.Parameters)
					modified |= Preserver.ProcessAttributeProvider (p, method);
			}
			if (IsPropertyMethod (method)) {
				foreach (var property in method.DeclaringType.Properties)
					if (property.GetMethod == method || property.SetMethod == method) {
						modified |= Preserver.ProcessAttributeProvider (property, property.GetMethod, property.SetMethod);
						break;
					}
			}
			return modified;
		}
	}

	class PreserveSmartEnumConversion {
		Dictionary<TypeDefinition, Tuple<MethodDefinition, MethodDefinition>> cache = new ();

		public DerivedLinkContext LinkContext { get; private set; }
		public Application App => LinkContext.App;

		Func<Tuple<MethodDefinition, MethodDefinition>, bool, MethodDefinition? [], bool> preserve { get; set; }

		public PreserveSmartEnumConversion (DerivedLinkContext linkContext, Func<Tuple<MethodDefinition, MethodDefinition>, bool, MethodDefinition? [], bool> preserve)
		{
			LinkContext = linkContext;
			this.preserve = preserve;
		}

		bool Preserve (Tuple<MethodDefinition, MethodDefinition> pair, bool alreadyProcessed, params MethodDefinition? [] conditions)
		{
			return preserve (pair, alreadyProcessed, conditions);
		}

		public bool ProcessAttributeProvider (ICustomAttributeProvider provider, params MethodDefinition [] conditions)
		{
			var modified = false;

			if (provider?.HasCustomAttributes != true)
				return modified;

			foreach (var ca in provider.CustomAttributes) {
				var tr = ca.Constructor.DeclaringType;

				if (!tr.Is ("ObjCRuntime", "BindAsAttribute"))
					continue;

				if (ca.ConstructorArguments.Count != 1) {
					ErrorHelper.Show (App, ErrorHelper.CreateWarning (LinkContext.App, 4124, provider, Errors.MT4124_E, provider.AsString (), ca.ConstructorArguments.Count));
					continue;
				}

				var managedType = ca.ConstructorArguments [0].Value as TypeReference;
				var managedEnumType = managedType?.GetElementType ().Resolve ();
				if (managedEnumType is null) {
					ErrorHelper.Show (App, ErrorHelper.CreateWarning (LinkContext.App, 4124, provider, Errors.MT4124_H, provider.AsString (), managedType?.FullName ?? "(null)"));
					continue;
				}

				// We only care about enums, BindAs attributes can be used for other types too.
				if (!managedEnumType.IsEnum)
					continue;

				if (cache.TryGetValue (managedEnumType, out var pair)) {
					// The pair was already marked if it was cached.
					modified |= Preserve (pair, true, conditions);
					continue;
				}

				// Find the Extension type
				TypeDefinition? extensionType = null;
				var extensionName = managedEnumType.Name + "Extensions";
				foreach (var type in managedEnumType.Module.Types) {
					if (type.Namespace != managedEnumType.Namespace)
						continue;
					if (type.Name != extensionName)
						continue;
					extensionType = type;
					break;
				}
				if (extensionType is null) {
					App.Log (1, $"Could not find a smart extension type for the enum {managedEnumType.FullName} (due to BindAs attribute on {provider.AsString ()}): most likely this is because the enum isn't a smart enum.");
					continue;
				}

				// Find the GetConstant/GetValue methods
				MethodDefinition? getConstant = null;
				MethodDefinition? getValue = null;

				foreach (var method in extensionType.Methods) {
					if (!method.IsStatic)
						continue;
					if (!method.HasParameters || method.Parameters.Count != 1)
						continue;
					if (method.Name == "GetConstant") {
						if (!method.ReturnType.Is ("Foundation", "NSString"))
							continue;
						if (method.Parameters [0].ParameterType != managedEnumType)
							continue;
						getConstant = method;
					} else if (method.Name == "GetValue") {
						if (!method.Parameters [0].ParameterType.Is ("Foundation", "NSString"))
							continue;
						if (method.ReturnType != managedEnumType)
							continue;
						getValue = method;
					}
				}

				if (getConstant is null) {
					App.Log (1, $"Could not find the GetConstant method on the supposedly smart extension type {extensionType.FullName} for the enum {managedEnumType.FullName} (due to BindAs attribute on {provider.AsString ()}): most likely this is because the enum isn't a smart enum.");
					continue;
				}

				if (getValue is null) {
					App.Log (1, $"Could not find the GetValue method on the supposedly smart extension type {extensionType.FullName} for the enum {managedEnumType.FullName} (due to BindAs attribute on {provider.AsString ()}): most likely this is because the enum isn't a smart enum.");
					continue;
				}

				pair = new Tuple<MethodDefinition, MethodDefinition> (getConstant, getValue);
				cache.Add (managedEnumType, pair);
				modified |= Preserve (pair, false, conditions);
			}

			return modified;
		}
	}
}
