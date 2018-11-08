//
// The rule reports
//
// !missing-selector!
//             if headers defines a selector for which we have no bindings
//

using System;
using System.Collections.Generic;

using Mono.Cecil;

using Clang.Ast;
using System.Linq;

namespace Extrospection {

	public class SelectorCheck : BaseVisitor {

		HashSet<(string MethodDefinition, Helpers.ArgumentSemantic ArgumentSemantic)> qualified_export_arguments = new HashSet<(string, Helpers.ArgumentSemantic)> ();

		// most selectors will be found in [Export] attribtues
		public override void VisitManagedMethod (MethodDefinition method)
		{
			if (!method.HasCustomAttributes)
				return;

			var type = method.DeclaringType;
			// we do not process protocols here
			if (type.IsProtocol ())
				return;

			foreach (var ca in method.CustomAttributes) {
				switch (ca.Constructor.DeclaringType.Name) {
				case "ExportAttribute":
					//string selector = ca.ConstructorArguments [0].Value as string;
					//if (!known_selectors.Contains (selector))
					//known_selectors.Add (selector);

					var methodDefinition = method.GetName ();
					if (!string.IsNullOrEmpty (methodDefinition)) {
						var argumentSemantic = Helpers.ArgumentSemantic.None;
						if (ca.ConstructorArguments.Count > 1) {
							argumentSemantic = (Helpers.ArgumentSemantic)ca.ConstructorArguments [1].Value;
						}

						qualified_export_arguments.Add ((methodDefinition, argumentSemantic));
					}

					break;
				}
			}
		}

		public override void VisitObjCPropertyDecl (ObjCPropertyDecl decl)
		{
			// protocol members are checked in ObjCProtocolCheck
			if (decl.DeclContext is ObjCProtocolDecl)
				return;

			// check availability macros to see if the API is available on the OS and not deprecated
			if (!decl.IsAvailable ())
				return;

			var framework = Helpers.GetFramework (decl);
			if (framework == null)
				return;

			var nativeArgumentSemantic = decl.Attributes.ToArgumentSemantic ();

			// Ignore Copy, UnsafeUnretained and Weak for now
			if (nativeArgumentSemantic == Helpers.ArgumentSemantic.Copy || nativeArgumentSemantic == Helpers.ArgumentSemantic.UnsafeUnretained || nativeArgumentSemantic == Helpers.ArgumentSemantic.Weak)
				return;

			var nativeMethodDefinition = decl.QualifiedName;

			var exportArgs = qualified_export_arguments.FirstOrDefault (sel => sel.MethodDefinition.Contains (nativeMethodDefinition));

			if (!string.IsNullOrEmpty (exportArgs.MethodDefinition) && exportArgs.ArgumentSemantic != nativeArgumentSemantic)
				Log.On (framework).Add ($"!incorrect-argument-semantic! {nativeMethodDefinition} has ArgumentSemantic.{nativeArgumentSemantic.ToUsableString ()} instead of ArgumentSemantic.{exportArgs.ArgumentSemantic.ToUsableString ()}");
		}

		public override void VisitObjCMethodDecl (ObjCMethodDecl decl, VisitKind visitKind)
		{
			if (visitKind != VisitKind.Enter)
				return;

			// protocol members are checked in ObjCProtocolCheck
			if (decl.DeclContext is ObjCProtocolDecl)
				return;

			// don't process methods (or types) that are unavailable for the current platform
			if (!decl.IsAvailable () || !(decl.DeclContext as Decl).IsAvailable ())
				return;

			var framework = Helpers.GetFramework (decl);
			if (framework == null)
				return;

			string selector = decl.GetSelector ();
			if (String.IsNullOrEmpty (selector))
				return;

			var name = decl.QualifiedName;
			if (decl.IsClassMethod) {
				// we do not bind `+{type}:new` just instance `init`
				if (selector == "new")
					return;
				name = "+" + name;
			}

			bool found = qualified_export_arguments.Any (m => m.MethodDefinition == name);
			if (!found) {
				// a category could be inlined into the type it extend
				var category = decl.DeclContext as ObjCCategoryDecl;
				if (category != null) {
					var cname = category.Name;
					if (cname == null)
						name = GetCategoryBase (category) + name;
					else
						name = name.ReplaceFirstInstance (cname, GetCategoryBase (category));
					found = qualified_export_arguments.Any (m => m.MethodDefinition == name);
				}
			}
			if (!found)
				Log.On (framework).Add ($"!missing-selector! {name} not bound");
		}

		static string GetCategoryBase (ObjCCategoryDecl category)
		{
			// I really dislike doing this
			switch (category.Name) {
			case "UIResponderStandardEditActions":
				// we inlined this protocol in UIResponder but Apple has it on NSObject
				return "UIResponder";
			case "UIAccessibility":
				// we inlined this protocol in UIView... but Apple has it on NSObject
				return "UIView";
			case "UIAccessibilityAction":
				// we inlined this protocol in UIResponder but Apple has it on NSObject
				return "UIResponder";
			default:
				return Helpers.GetManagedName (category.ClassInterface.Name);
			}
		}
	}
}