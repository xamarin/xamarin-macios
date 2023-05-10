//
// The rule reports
//
// !missing-release-attribute-on-return-value!
//             for methods whose objc family indicates the returned value is retained, and the method doesn't have a [return: Release] attribute.
//

using System;
using System.Collections.Generic;

using Mono.Cecil;

using Clang.Ast;

namespace Extrospection {

	public class ReleaseAttributeCheck : BaseVisitor {

		// most selectors will be found in [Export] attributes
		public override void VisitManagedMethod (MethodDefinition method)
		{
			// Don't care about methods that don't have [Export] attributes
			if (!method.HasCustomAttributes)
				return;

			// We don't care about 'void' functions
			if (method.ReturnType.FullName == "System.Void")
				return;

			// Value types can't need '[return: Release]'
			if (method.ReturnType.IsValueType)
				return;

			string family = null;
			string selector = null;
			bool hasReleaseAttribute = false;

			if (method.MethodReturnType.HasCustomAttributes) {
				foreach (var ca in method.MethodReturnType.CustomAttributes) {
					switch (ca.Constructor.DeclaringType.Name) {
					case "ReleaseAttribute":
						hasReleaseAttribute = true;
						break;
					}
				}
			}

			foreach (var ca in method.CustomAttributes) {
				switch (ca.Constructor.DeclaringType.Name) {
				case "ExportAttribute":
					selector = (string) ca.ConstructorArguments [0].Value;

					// We need to compute the selector's method family
					// https://clang.llvm.org/docs/AutomaticReferenceCounting.html#method-families

					// A selector is in a certain selector family if ignoring any leading underscore the first component of the selector either consists entirely
					// of the name of the method family or it begins with that name followed by a character other than a lowercase letter
					var firstLetter = 0;
					var firstNonLowercaseLetter = selector.Length;
					for (var i = 0; i < selector.Length; i++) {
						var c = selector [i];

						if (firstLetter == i && c == '_') {
							// ... ignoring any leading underscores ...
							firstLetter++;
						} else if (c < 'a' || c > 'z') {
							firstNonLowercaseLetter = i;
							break;
						}
					}
					family = selector.Substring (0, firstNonLowercaseLetter - firstLetter);
					break;
				}
			}

			switch (family) {
			case "init": // in many cases we have custom init/constructor code, which seems to be correct, so ignore the 'init' family for now.
				break;
			case "alloc":
			case "copy":
			case "mutableCopy":
			case "new":
				if (!hasReleaseAttribute) {
					var framework = Helpers.GetFramework (method);
					Log.On (framework).Add ($"!missing-release-attribute-on-return-value! {method.FullName}'s selector's ('{selector}') Objective-C method family ('{family}') indicates that the native method returns a retained object, and as such a '[return: Release]' attribute is required.");
				}
				break;
			default:
				break;
			}

		}

		// We should also look at the native definition for family attributes: __attribute__((objc_method_family(...))
		// but unfortunately ObjectiveSharpie doesn't support getting the actual family from the attribute yet,
		// so this will have to wait. In any case there only seems to be a single method with the family attribute in the SDKs,
		// so we can live with a manual exception.
		//public override void VisitObjCMethodDecl (ObjCMethodDecl decl, VisitKind visitKind)
		//{
		//	if (visitKind != VisitKind.Enter)
		//		return;

		//	// don't process methods (or types) that are unavailable for the current platform
		//	if (!decl.IsAvailable () || !(decl.DeclContext as Decl).IsAvailable ())
		//		return;

		//	var framework = Helpers.GetFramework (decl);
		//	if (framework is null)
		//		return;

		//	string selector = decl.GetSelector ();
		//	if (string.IsNullOrEmpty (selector))
		//		return;

		//	foreach (var attr in decl.Attrs) {
		//		switch (attr.Kind) {
		//		case AttrKind.ObjCMethodFamily:
		//			ObjCMethodFamilyAttr familyAttr = (ObjCMethodFamilyAttr)attr;
		//			Console.WriteLine ("Family attribute {0} for {1}", familyAttr, selector);
		//			break;
		//		default:
		//			break;
		//		}
		//	}
		//}
	}
}
