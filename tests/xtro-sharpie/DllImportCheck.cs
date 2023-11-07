//
// The rule reports
//
// !missing-pinvoke!
//		if headers defines functions that we have not bound with [DllImport]
//
// !unknown-pinvoke!
//		if we have [DllImport] that were not found in headers
//		NOTE: introspection tests checks if those exists at runtime, i.e. they might
//			just be undocumented (or not in the headers)
//

using System;
using System.Collections.Generic;

using Mono.Cecil;

using Clang.Ast;

namespace Extrospection {

	// track all [DllImport]
	class DllImportCheck : BaseVisitor {

		// dupes :|
		Dictionary<string, MethodDefinition> dllimports = new Dictionary<string, MethodDefinition> ();

		public override void VisitManagedMethod (MethodDefinition method)
		{
			if (!method.IsPInvokeImpl || !method.HasPInvokeInfo)
				return;

			var info = method.PInvokeInfo;
			if (info.Module.Name == "__Internal")
				return;

			// there are duplicates declarations
			// TODO: right now we only check the first one, as the priority is knowing if we have (or not) bindings for them
			var name = info.EntryPoint;
			if (!dllimports.ContainsKey (name))
				dllimports.Add (name, method);
		}

		public override void VisitFunctionDecl (FunctionDecl decl, VisitKind visitKind)
		{
			if (visitKind != VisitKind.Enter)
				return;
			// skip macros : we generally implement them but their name is lost (so no matching is possible)
			if (!decl.IsExternC)
				return;

			var name = decl.Name;
			// do not consider _* or __* as public API that should be bound
			if (name [0] == '_')
				return;

			var framework = Helpers.GetFramework (decl);
			if (framework is null)
				return;

			// check availability macros to see if the API is available on the OS and not deprecated
			if (!decl.IsAvailable ())
				return;

			if (!dllimports.ContainsKey (name)) {
				// if we find functions without matching DllImport then we report them
				// but don't report deprecated functions
				if (!decl.IsDeprecated ())
					Log.On (framework).Add ($"!missing-pinvoke! {name} is not bound");
				return;
			}

			dllimports.Remove (name);
		}

		public override void End ()
		{
			// at this stage anything else we have is not something we could find in Apple's headers
			// e.g. a typo in the name
			foreach (var kvp in dllimports) {
				var extra = kvp.Key;
				var framework = Helpers.GetFramework (kvp.Value);
				Log.On (framework).Add ($"!unknown-pinvoke! {extra} bound");
			}
		}
	}
}
