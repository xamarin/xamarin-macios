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
				if (AttributeHelpers.HasAnyDeprecationForCurrentPlatform (managedType))
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
			// TODO - This is a hack, we shouldn't be doing ^
			if (AttributeHelpers.HasAnyAdvice (item))
				return;

			if (!AttributeHelpers.HasAnyDeprecationForCurrentPlatform (item)) {
				Log.On (framework).Add ($"!deprecated-attribute-missing! {itemName} missing a [Deprecated] attribute");
				return;
			}

			// Don't version check us when Apple does __attribute__((availability(macos, introduced=10.0, deprecated=100000)));
			// #define __API_TO_BE_DEPRECATED 100000
			if (objcVersion.Major == 100000) 
				return;

			// Some APIs have both a [Deprecated] and [Obsoleted]. Bias towards [Obsoleted].
			Version managedVersion;
			bool foundObsoleted = AttributeHelpers.FindObsolete (item, out managedVersion);
			if (foundObsoleted) {
				if (managedVersion != null && !ManagedBeforeOrEqualToObjcVersion (objcVersion, managedVersion))
					Log.On (framework).Add ($"!deprecated-attribute-wrong! {itemName} has {managedVersion} not {objcVersion} on [Obsoleted] attribute");
				return;
			}

			bool foundDeprecated = AttributeHelpers.FindDeprecated (item, out managedVersion);
			if (foundDeprecated && managedVersion != null && !ManagedBeforeOrEqualToObjcVersion (objcVersion, managedVersion))
				Log.On (framework).Add ($"!deprecated-attribute-wrong! {itemName} has {managedVersion} not {objcVersion} on [Deprecated] attribute");
		}

		public static bool ManagedBeforeOrEqualToObjcVersion (VersionTuple objcVersionTuple, Version managedVersion)
		{
			// Often header files will soft deprecate APIs versions before a formal deprecation (10.7 soft vs 10.10 formal). Accept older deprecation values
			return managedVersion <= VersionHelpers.Convert (objcVersionTuple);
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
