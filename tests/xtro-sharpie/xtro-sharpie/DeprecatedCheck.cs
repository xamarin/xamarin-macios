using System;
using System.Collections.Generic;
using System.Linq;
using Clang;
using Clang.Ast;
using Mono.Cecil;

namespace Extrospection {
	public class DeprecatedCheck : BaseVisitor {
		Dictionary<string, VersionTuple> ObjCDeprecatedItems = new Dictionary<string, VersionTuple> ();
		Dictionary<string, VersionTuple> ObjCDeprecatedSelectors = new Dictionary<string, VersionTuple> ();
		Dictionary<string, VersionTuple> PlainCDeprecatedFunctions = new Dictionary<string, VersionTuple> ();

		List<TypeDefinition> ManagedTypes = new List<TypeDefinition> ();
		Dictionary<string, MethodDefinition> dllimports = new Dictionary<string, MethodDefinition> ();

		public override void VisitManagedMethod (MethodDefinition method)
		{
			if (!method.IsPInvokeImpl || !method.HasPInvokeInfo)
				return;

			// we don't decorate OpenTK types with availability
			var dt = method.DeclaringType;
			var ns = dt.IsNested ? dt.DeclaringType.Namespace : dt.Namespace;
			if (ns.StartsWith ("OpenTK.", StringComparison.Ordinal))
				return;

			var info = method.PInvokeInfo;
			if (info.Module.Name == "__Internal")
				return;

			// there are duplicated declarations, only the last will be reported
			dllimports [info.EntryPoint] = method;
		}

		public override void End ()
		{
			foreach (var objcEntry in ObjCDeprecatedItems)
				ProcessObjcEntry (objcEntry.Key, objcEntry.Value);

			foreach (var objcEntry in ObjCDeprecatedSelectors)
				ProcessObjcSelector (objcEntry.Key, objcEntry.Value);

			foreach (var cEntry in PlainCDeprecatedFunctions)
				ProcessCFunction (cEntry.Key, cEntry.Value);
		}

		void ProcessObjcEntry (string objcClassName, VersionTuple objcVersion)
		{
			TypeDefinition managedType = ManagedTypes.FirstOrDefault (x => Helpers.GetName (x) == objcClassName && x.IsPublic);
			if (managedType is not null) {
				var framework = Helpers.GetFramework (managedType);
				if (framework is not null)
					ProcessItem (managedType, Helpers.GetName (managedType), objcVersion, framework);
			}
		}

		void ProcessObjcSelector (string fullname, VersionTuple objcVersion)
		{
			var class_method = fullname [0] == '+';
			var n = fullname.IndexOf ("::");
			string objcClassName = fullname.Substring (class_method ? 1 : 0, n);
			string selector = fullname.Substring (n + 2);

			TypeDefinition managedType = ManagedTypes.FirstOrDefault (x => Helpers.GetName (x) == objcClassName);
			if (managedType is not null) {
				var framework = Helpers.GetFramework (managedType);
				if (framework is null)
					return;

				// If the entire type is deprecated, call it good enough
				if (AttributeHelpers.HasAnyDeprecationForCurrentPlatform (managedType))
					return;

				var matchingMethod = managedType.Methods.FirstOrDefault (x => x.GetSelector () == selector && x.IsPublic && x.IsStatic == class_method);
				if (matchingMethod is not null)
					ProcessItem (matchingMethod, fullname, objcVersion, framework);
			}
		}

		void ProcessCFunction (string fullname, VersionTuple objcVersion)
		{
			if (dllimports.TryGetValue (fullname, out var method)) {
				var dt = method.DeclaringType;
				var framework = Helpers.GetFramework (dt);
				if (framework is null)
					return;

				// If the entire type is deprecated, call it good enough
				if (AttributeHelpers.HasAnyDeprecationForCurrentPlatform (dt))
					return;

				ProcessItem (method, fullname, objcVersion, framework);
			}
		}

		public void ProcessItem (ICustomAttributeProvider item, string itemName, VersionTuple objcVersion, string framework)
		{
			// Our bindings do not need have [Deprecated] for ancient versions we don't support anymore
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
				if (managedVersion is not null && !ManagedBeforeOrEqualToObjcVersion (objcVersion, managedVersion))
					Log.On (framework).Add ($"!deprecated-attribute-wrong! {itemName} has {managedVersion} not {objcVersion} on [Obsoleted] attribute");
				return;
			}

			bool foundDeprecated = AttributeHelpers.FindDeprecated (item, out managedVersion);
			if (foundDeprecated && managedVersion is not null && !ManagedBeforeOrEqualToObjcVersion (objcVersion, managedVersion))
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
			if (visitKind == VisitKind.Enter && AttributeHelpers.FindObjcDeprecated (decl.Attrs, out VersionTuple version)) {
				// `(anonymous)` has a null name
				var name = decl.Name;
				if (name is not null)
					ObjCDeprecatedItems [name] = version;
			}
		}

		public override void VisitObjCMethodDecl (ObjCMethodDecl decl, VisitKind visitKind)
		{
			if (visitKind == VisitKind.Enter && AttributeHelpers.FindObjcDeprecated (decl.Attrs, out VersionTuple version)) {
				var qn = decl.QualifiedName;
				if (decl.IsClassMethod)
					qn = "+" + qn;
				ObjCDeprecatedSelectors [qn] = version;
			}
		}

		public override void VisitFunctionDecl (FunctionDecl decl, VisitKind visitKind)
		{
			if (visitKind == VisitKind.Enter && AttributeHelpers.FindObjcDeprecated (decl.Attrs, out VersionTuple version))
				PlainCDeprecatedFunctions [decl.QualifiedName] = version;
		}
	}
}
