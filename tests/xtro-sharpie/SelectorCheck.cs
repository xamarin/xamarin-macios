//
// The rule reports
//
// !missing-selector!
//             if headers defines a selector for which we have no bindings
//
// !unknown-selector!
//             if we have a selector that is not part of the header files
//

using System;
using System.Collections.Generic;

using Mono.Cecil;

using Clang.Ast;

namespace Extrospection {

	public class SelectorCheck : BaseVisitor {
//		Dictionary<string,List<MethodDefinition>> exports = new Dictionary<string, List<MethodDefinition>> ();

		// missing
		//	-> it's not in the type or ancestor or their interface (protocols)
		//	-> it's not in a category

		// unknown
		//	-> quick check (HashSet) to see if it's used anywhere
		//	-> 

		// duplicate
		//	-> the selector is defined more than once for the same type

		HashSet<string> known_selectors = new HashSet<string> ();

		HashSet<string> qualified_selectors = new HashSet<string> ();

		//Dictionary<TypeDefinition,HashSet<string>> type_exports = new Dictionary<TypeDefinition,HashSet<string>> ();

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
					string selector = ca.ConstructorArguments [0].Value as string;
					if (!known_selectors.Contains (selector))
						known_selectors.Add (selector);

					qualified_selectors.Add (method.GetName ());
//
//					TypeDefinition type = method.DeclaringType;
//					HashSet<string> list;
//					if (!type_exports.TryGetValue (type, out list)) {
//						list = new HashSet<string> ();
//						type_exports.Add (type, list);
//					}
//					list.Add (selector);
					break;
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

			var framework = Helpers.GetFramework (decl);
			if (framework == null)
				return;

			string selector = decl.GetSelector ();
			if (String.IsNullOrEmpty (selector))
				return;
			
			var name = (decl.IsClassMethod ? "+" : String.Empty) + decl.QualifiedName;
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