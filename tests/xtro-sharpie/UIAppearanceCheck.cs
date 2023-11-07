//
// The rule reports
//
// !missing-ui-appearance-support!
//		when an API does not have a [Appearance] attribute while ObjC headers have a UI_APPEARANCE_SELECTOR
//
// !extra-ui-appearance-support!
//		when an API is decorated with [Appearance] attribute but ObjC headers don't have a UI_APPEARANCE_SELECTOR
//

using System;
using System.Collections.Generic;

using Mono.Cecil;

using Clang.Ast;

namespace Extrospection {

	public class UIAppearanceCheck : BaseVisitor {

		static HashSet<TypeDefinition> appearance_types = new HashSet<TypeDefinition> ();
		static HashSet<MethodDefinition> appearance_methods = new HashSet<MethodDefinition> ();
		static Dictionary<string, MethodDefinition> methods = new Dictionary<string, MethodDefinition> ();

		static MethodDefinition GetMethod (ObjCMethodDecl decl)
		{
			methods.TryGetValue (decl.GetName (), out var md);
			return md;
		}

		public override void VisitManagedType (TypeDefinition type)
		{
			if (!type.HasNestedTypes)
				return;

			var tn = type.Name + "Appearance";
			foreach (var nt in type.NestedTypes) {
				if (nt.Name == tn)
					appearance_types.Add (nt);
			}
		}

		public override void VisitManagedMethod (MethodDefinition method)
		{
			var key = method.GetName ();
			if (key is not null)
				methods [key] = method;
		}

		public override void VisitObjCPropertyDecl (ObjCPropertyDecl decl)
		{
			// don't process methods (or types) that are unavailable for the current platform
			if (!decl.IsAvailable () || !(decl.DeclContext as Decl).IsAvailable ())
				return;

			// does not look exposed, but part of the dump
			if (decl.DumpToString ().IndexOf ("UI_APPEARANCE_SELECTOR", StringComparison.OrdinalIgnoreCase) < 0)
				return;

			var getter = decl.Getter;
			if (getter is not null)
				VisitObjCMethodDecl (getter);
			var setter = decl.Setter;
			if (setter is not null)
				VisitObjCMethodDecl (setter);
		}

		public override void VisitObjCMethodDecl (ObjCMethodDecl decl, VisitKind visitKind)
		{
			if (visitKind != VisitKind.Enter)
				return;

			// don't process methods (or types) that are unavailable for the current platform
			if (!decl.IsAvailable () || !(decl.DeclContext as Decl).IsAvailable ())
				return;

			// does not look exposed, but part of the dump
			if (decl.DumpToString ().IndexOf ("UI_APPEARANCE_SELECTOR", StringComparison.OrdinalIgnoreCase) < 0)
				return;

			VisitObjCMethodDecl (decl);
		}

		void VisitObjCMethodDecl (ObjCMethodDecl decl)
		{
			var framework = Helpers.GetFramework (decl);
			if (framework is null)
				return;

			var method = GetMethod (decl);
			if (method is null)
				return;

			var dt = method.DeclaringType;
			if (dt.HasNestedTypes) {
				foreach (var nt in dt.NestedTypes) {
					if (nt.Name != dt.Name + "Appearance")
						continue;
					// find matching method, including parameters (we have overloads)
					var fn = method.FullName;
					int ms = fn.IndexOf ("::");
					fn = fn.Insert (ms, $"/{dt.Name}Appearance");
					foreach (var m in nt.Methods) {
						if (m.FullName != fn)
							continue;
						appearance_methods.Add (m); // legit one
						return;
					}
				}
			}

			Log.On (framework).Add ($"!missing-ui-appearance-support! {method.GetName ()} is missing [Appearance]");
		}

		public override void End ()
		{
			// looking for extra [Appearance] attributes
			foreach (var t in appearance_types) {
				if (!t.HasMethods)
					continue;
				foreach (var m in t.Methods) {
					if (m.IsConstructor)
						continue;
					if (appearance_methods.Contains (m))
						continue;
					var framework = Helpers.GetFramework (m);
					var fn = m.FullName;
					// don't report on the *Appearance type - but where the attribute was used
					int ns = fn.IndexOf ('/');
					int ms = fn.IndexOf ("::", ns);
					fn = fn.Remove (ns, ms - ns);
					Log.On (framework).Add ($"!extra-ui-appearance-support! {fn} should NOT be decorated with [Appearance]");
				}
			}
		}
	}
}
