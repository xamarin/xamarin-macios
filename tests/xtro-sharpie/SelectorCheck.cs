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

namespace Extrospection {

	public class SelectorCheck : BaseVisitor {

		HashSet<string> qualified_selectors = new HashSet<string> ();
		Dictionary<string, Helpers.ArgumentSemantic> qualified_properties = new Dictionary<string, Helpers.ArgumentSemantic> ();

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
					var methodDefinition = method.GetName ();
					if (!string.IsNullOrEmpty (methodDefinition)) {
						var argumentSemantic = Helpers.ArgumentSemantic.Assign; // Default
						if (ca.ConstructorArguments.Count > 1) {
							argumentSemantic = (Helpers.ArgumentSemantic)ca.ConstructorArguments [1].Value;
							qualified_properties.Add (methodDefinition, argumentSemantic);
						}

						qualified_selectors.Add (methodDefinition);
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

			var nativeMethodDefinition = decl.QualifiedName;

			bool found = qualified_properties.TryGetValue (nativeMethodDefinition, out var managedArgumentSemantic);
			if (found && managedArgumentSemantic != nativeArgumentSemantic)
				Log.On (framework).Add ($"!incorrect-argument-semantic! Native '{nativeMethodDefinition}' has ({nativeArgumentSemantic.ToUsableString ().ToLowerInvariant ()}) instead of 'ArgumentSemantic.{managedArgumentSemantic.ToUsableString ()}'");
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
			bool found = qualified_selectors.Contains (name);
			if (!found) {
				// a category could be inlined into the type it extend
				var category = decl.DeclContext as ObjCCategoryDecl;
				if (category != null) {
					var cname = category.Name;
					if (cname == null)
						name = GetCategoryBase (category) + name;
					else
						name = name.ReplaceFirstInstance (cname, GetCategoryBase (category));
					found = qualified_selectors.Contains (name);
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