//
// The rule reports
//
// !missing-requires-super!
//		when a managed member is missing its [RequiresSuper] attribute
//
// !extra-designated-initializer!
//		when a managed member has no business to have an [RequiresSuper] attribute
//

using System;
using System.Collections.Generic;

using Mono.Cecil;

using Clang.Ast;

namespace Extrospection {

	public class RequiresSuperCheck : BaseVisitor {

		static Dictionary<string,MethodDefinition> methods = new Dictionary<string,MethodDefinition> ();

		static MethodDefinition GetMethod (ObjCMethodDecl decl)
		{
			methods.TryGetValue (decl.GetName (), out var md);
			return md;
		}

		public override void VisitManagedMethod (MethodDefinition method)
		{
			var key = method.GetName ();
			if (key == null)
				return;

			// we still have one case to fix with duplicate selectors :|
			if (!methods.ContainsKey (key))
				methods.Add (key, method);
		}

		public override void VisitObjCMethodDecl (ObjCMethodDecl decl, VisitKind visitKind)
		{
			if (visitKind != VisitKind.Enter)
				return;

			// don't process methods (or types) that are unavailable for the current platform
			if (!decl.IsAvailable () || !(decl.DeclContext as Decl).IsAvailable ())
				return;

			var method = GetMethod (decl);
			if (method == null)
				return;

			var framework = Helpers.GetFramework (decl);
			if (framework == null)
				return;

			if (!decl.HasAttr<ObjCRequiresSuperAttr> ()) {
				if (method.RequiresSuper ())
					Log.On (framework).Add ($"!extra-requires-super! {method.GetName ()} is incorrectly decorated with an [RequiresSuper] attribute");
			} else if (!method.RequiresSuper ()) {
				Log.On (framework).Add ($"!missing-requires-super! {method.GetName ()} is missing an [RequiresSuper] attribute");
			}
		}
	}
}
