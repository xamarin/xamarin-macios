//
// The rule reports
//
// !duplicate-field-name!
//		if we have duplicated [Field] / code to export the same field more than once
//
// !missing-field!
//		if headers defines fields that we have not bound as [Field]
//		NOTE: we still have amnually bound fields :(
//
// !unknown-field!
//		if we have [Field] that were not found in headers
//		NOTE: introspection tests checks if those exists at runtime, i.e. they might
//			just be undocumented (or not in the headers)
//

using System;
using System.Collections.Generic;

using Mono.Cecil;

using Clang.Ast;

namespace Extrospection {

	public class FieldCheck : BaseVisitor {

		Dictionary<string,MemberReference> fields = new Dictionary<string, MemberReference> ();

		public override void VisitManagedType (TypeDefinition type)
		{
			if (type.HasProperties) {
				foreach (var p in type.Properties) {
					if (!p.HasCustomAttributes)
						continue;
					var getter = p.GetMethod;
					// mostly static getters but not in the case of generated EventArgs.g.cs
					if (getter == null)
						continue;

					CheckAttributes (p.FullName, p);
				}
			}

			if (type.HasFields) {
				foreach (var f in type.Fields) {
					if (!f.HasCustomAttributes || !f.IsStatic)
						continue;
					CheckAttributes (f.FullName, f);
				}
			}
		}

		void CheckAttributes (string memberName, ICustomAttributeProvider p)
		{
			foreach (var ca in p.CustomAttributes) {
				if (ca.Constructor.DeclaringType.Name != "FieldAttribute")
					continue;

				var name = ca.ConstructorArguments [0].Value as string;

				if (!fields.TryGetValue (name, out var mr))
					fields.Add (name, p as MemberReference);
				else {
					// not critical and quite noisy with current API profile
					// Console.WriteLine ("!duplicate-field-name! {0} [Field] exists as both {1} and {2}", name, memberName, mr.FullName);
				}
			}
		}

		public override void VisitVarDecl (VarDecl decl)
		{
			if (!decl.IsExternC)
				return;
			if (!decl.PresumedLoc.FileName.Contains (".framework"))
				return;

			if (!decl.IsAvailable ())
				return;

			var framework = Helpers.GetFramework (decl);
			if (framework == null)
				return;

			var name = decl.ToString ();
			if (!fields.TryGetValue (name, out var mr)) {
				Log.On (framework).Add ($"!missing-field! {name} not bound");
			} else
				fields.Remove (name);
		}

		public override void End ()
		{
			// at this stage anything else we have is not something we could find in Apple's headers
			foreach (var kvp in fields) {
				var extra = kvp.Key;
				var framework = Helpers.GetFramework (kvp.Value);
				Log.On (framework).Add ($"!unknown-field! {extra} bound");
			}
		}
	}
}