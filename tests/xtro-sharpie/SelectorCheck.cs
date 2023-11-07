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
		Dictionary<string, List<Tuple<MethodDefinition, Helpers.ArgumentSemantic>>> qualified_properties = new Dictionary<string, List<Tuple<MethodDefinition, Helpers.ArgumentSemantic>>> ();

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
							argumentSemantic = (Helpers.ArgumentSemantic) ca.ConstructorArguments [1].Value;
							if (!qualified_properties.TryGetValue (methodDefinition, out var list))
								qualified_properties [methodDefinition] = list = new List<Tuple<MethodDefinition, Helpers.ArgumentSemantic>> ();
							list.Add (new Tuple<MethodDefinition, Helpers.ArgumentSemantic> (method, argumentSemantic));
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
			if (framework is null)
				return;

			var nativeArgumentSemantic = decl.Attributes.ToArgumentSemantic ();
			var nativeMethodDefinition = decl.QualifiedName;

			if (qualified_properties.TryGetValue (nativeMethodDefinition, out var managedArgumentSemanticList)) {
				foreach (var entry in managedArgumentSemanticList) {
					var method = entry.Item1;
					var managedArgumentSemantic = entry.Item2;

					if (managedArgumentSemantic != nativeArgumentSemantic) {
						// FIXME: only Copy mistakes are reported now
						if (managedArgumentSemantic == Helpers.ArgumentSemantic.Copy || nativeArgumentSemantic == Helpers.ArgumentSemantic.Copy) {
							// FIXME: rule disactivated for now
							// Log.On (framework).Add ($"!incorrect-argument-semantic! Native '{nativeMethodDefinition}' is declared as ({nativeArgumentSemantic.ToUsableString ().ToLowerInvariant ()}) but mapped to 'ArgumentSemantic.{managedArgumentSemantic.ToUsableString ()}' in '{method}'");
						}
					}
				}
			}
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

			// don't process deprecated methods (or types)
			if (decl.IsDeprecated () || (decl.DeclContext as Decl).IsDeprecated ())
				return;

			var framework = Helpers.GetFramework (decl);
			if (framework is null)
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
				if (category is not null) {
					var cname = category.Name;
					if (cname is null)
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
