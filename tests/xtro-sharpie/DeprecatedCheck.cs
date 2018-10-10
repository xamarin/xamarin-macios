using System;
using System.Collections.Generic;
using System.Linq;
using Clang;
using Clang.Ast;
using Mono.Cecil;

namespace Extrospection
{
	public class DeprecatedCheck : BaseVisitor
	{
		Dictionary<string, VersionTuple> ObjCDeprecatedItems = new Dictionary<string, VersionTuple> ();
		Dictionary<string, VersionTuple> ObjCDeprecatedSelectors = new Dictionary<string, VersionTuple> ();

		List<TypeDefinition> ManagedTypes = new List<TypeDefinition> ();

		public override void End ()
		{
			foreach (var objcEntry in ObjCDeprecatedItems)
				ProcessObjcEntry (objcEntry.Key, objcEntry.Value);

			foreach (var objcEntry in ObjCDeprecatedSelectors)
				ProcessObjcSelector (objcEntry.Key, objcEntry.Value);
		}

		void ProcessObjcEntry (string objcClassName, VersionTuple objcVersion)
		{
			TypeDefinition managedType = ManagedTypes.FirstOrDefault (x => Helpers.GetName (x) == objcClassName && x.IsPublic);
			if (managedType != null) {					
				var framework = Helpers.GetFramework (managedType);
				if (framework != null)
					ProcessItem (managedType, Helpers.GetName (managedType), objcVersion, framework);
			}
		}

		void ProcessObjcSelector (string fullname, VersionTuple objcVersion)
		{
			string[] nameParts = fullname.Split (new string[] { "::" }, StringSplitOptions.None);

			string objcClassName = nameParts[0];
			string selector = nameParts[1];

			TypeDefinition managedType = ManagedTypes.FirstOrDefault (x => Helpers.GetName (x) == objcClassName);
			if (managedType != null) {
				var framework = Helpers.GetFramework (managedType);
				if (framework == null)
					return;

				// If the entire type is deprecated, call it good enough
				if (HasAnyDeprecationAttribute (managedType.CustomAttributes))
					return;

				var matchingMethod = managedType.Methods.FirstOrDefault (x => x.GetSelector () == selector && x.IsPublic);
				if (matchingMethod != null)
					ProcessItem (matchingMethod, fullname, objcVersion, framework);
			}
		}

		public void ProcessItem (ICustomAttributeProvider item, string itemName, VersionTuple objcVersion, string framework)
		{
			// Our bindings do not need have [Deprecated] for ancicent versions we don't support anymore
			if (VersionHelpers.VersionTooOldToCare (objcVersion))
				return;

			// In some cases we've used [Advice] when entire types are deprecated
			if (HasAnyAdviceAttribute (item))
				return;

			if (!HasAnyDeprecationAttribute (item.CustomAttributes))
			{
				Log.On (framework).Add ($"!deprecated-attribute-missing! {itemName} missing a [Deprecated] attribute");
				return;
			}

			// Don't version check us when Apple does __attribute__((availability(macos, introduced=10.0, deprecated=100000)));
			if (objcVersion.Major == 100000)
				return;

			// Some APIs have both a [Deprecated] and [Obsoleted]. Bias towards [Obsoleted].
			Version managedVersion;
			bool foundObsoleted = AttributeHelpers.FindObsolete (item.CustomAttributes, out managedVersion);
			if (foundObsoleted) {
				if (managedVersion != null && !ManagedBeforeOrEqualToObjcVersion (objcVersion, managedVersion))
					Log.On (framework).Add ($"!deprecated-attribute-wrong! {itemName} has {managedVersion} not {objcVersion} on [Obsoleted] attribute");
				return;
			}

			bool foundDeprecated = AttributeHelpers.FindDeprecated (item.CustomAttributes, out managedVersion);
			if (foundDeprecated && managedVersion != null && !ManagedBeforeOrEqualToObjcVersion (objcVersion, managedVersion))
				Log.On (framework).Add ($"!deprecated-attribute-wrong! {itemName} has {managedVersion} not {objcVersion} on [Deprecated] attribute");
		}

		public static bool ManagedBeforeOrEqualToObjcVersion (VersionTuple objcVersionTuple, Version managedVersion)
		{
			// Often header files will soft deprecate APIs versions before a formal deprecation (10.7 soft vs 10.10 formal). Accept older deprecation values
			return managedVersion <= VersionHelpers.Convert (objcVersionTuple);
		}

		static bool HasAnyAdviceAttribute (ICustomAttributeProvider item)
		{
			if (AttributeHelpers.HasAdviced (item.CustomAttributes))
				return true;

			// Properites are a special case for [Advice], as it is generated on the property itself and not the individual get_ \ set_ methods
			// Cecil does not have a link between the MethodDefinition we have and the hosting PropertyDefinition, so we have to dig to find the match
			if (item is MethodDefinition method) {
				PropertyDefinition property = method.DeclaringType.Properties.FirstOrDefault (p => p.GetMethod == method || p.SetMethod == method);
				if (property != null && AttributeHelpers.HasAdviced (property.CustomAttributes))
					return true;
			}
			return false;
		}

		static bool HasAnyDeprecationAttribute (IEnumerable<CustomAttribute> attributes)
		{
			// This allows us to accept [Deprecated (iOS)] for watch and tv, which many of our bindings currently have
			// If we want to force seperate tv\watch attributes remove GetRelatedPlatforms and just check Helpers.Platform
			Platforms[] platforms = GetRelatedPlatforms ();
			foreach (var attribute in attributes) {
				if (platforms.Any (x => AttributeHelpers.HasDeprecated (attribute, x)) || platforms.Any (x => AttributeHelpers.HasObsoleted (attribute, x)))
					return true;
			}
			return false;
		}

		static Platforms[] GetRelatedPlatforms ()
		{
			// TV and Watch also implictly accept iOS
			switch (Helpers.Platform)
			{
			case Platforms.macOS:
				return new Platforms[] { Platforms.macOS };
			case Platforms.iOS:
				return new Platforms[] { Platforms.iOS };
			case Platforms.tvOS:
				return new Platforms[] { Platforms.iOS, Platforms.tvOS };
			case Platforms.watchOS:
				return new Platforms[] { Platforms.iOS, Platforms.watchOS };
			default:
				throw new InvalidOperationException ($"Unknown {Helpers.Platform} in GetPlatforms");
			}
		}

		public override void VisitManagedType (TypeDefinition type)
		{
			ManagedTypes.Add (type);
		}

		public override void VisitObjCCategoryDecl (ObjCCategoryDecl decl, VisitKind visitKind) => VisitItem (decl, visitKind);
		public override void VisitObjCInterfaceDecl (ObjCInterfaceDecl decl, VisitKind visitKind) => VisitItem (decl, visitKind);

		void VisitItem (NamedDecl decl, VisitKind visitKind)
		{
			if (visitKind == VisitKind.Enter && AttributeHelpers.FindObjcDeprecated (decl.Attrs, out VersionTuple version))
				ObjCDeprecatedItems[decl.Name] = version;
		}

		public override void VisitObjCMethodDecl (ObjCMethodDecl decl, VisitKind visitKind)
		{
			if (visitKind == VisitKind.Enter && AttributeHelpers.FindObjcDeprecated(decl.Attrs, out VersionTuple version))
				ObjCDeprecatedSelectors[decl.QualifiedName] = version;
		}
	}
}
