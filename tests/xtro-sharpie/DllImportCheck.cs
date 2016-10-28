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
		Dictionary<string,MethodDefinition> dllimports = new Dictionary<string, MethodDefinition> ();

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

			var framework = GetDeclaringHeaderFile (decl);
			// exclude non-framework code (e.g. all unix headers)
			// FIXME: we only process the headers in Frameworks, not all the UNIX headers
			// that still miss a few things (like objc runtime) but nothing that *requires* binding
			if (framework == null)
				return;

			// check availability macros to see if the API is available on the OS and not deprecated
			if (!decl.IsAvailable ())
				return;
			
			MethodDefinition md;
			if (!dllimports.TryGetValue (name, out md)) {

				// FIXME: we ignore some frameworks we have not bound (too many entries for our data files)
				// we do this late because, in some cases like vImage, we do bind a few API
				switch (framework) {
				// Accelerate and friends
				case "vImage":
				case "vecLib":
					return;
				// security
				case "GSS":
					return;
				// exposed in OpenTK-1.dll
				case "OpenAL":
				case "OpenGLES":
					return;
				case "ruby":
				case "Tcl":
				case "OpenGL":
					return;
				}
				// if we find functions without matching DllImport then we report them
				Console.WriteLine ("!missing-pinvoke! {0} is not bound", name);
				return;
			}

			dllimports.Remove (name);
		}

		public override void End ()
		{
			// at this stage anything else we have is not something we could find in Apple's headers
			// e.g. a typo in the name
			foreach (var extra in dllimports.Keys) {
				Console.WriteLine ("!unknown-pinvoke! {0} bound", extra);
			}
		}
	}
}
