//
// The rule reports
//
// !missing-protocol!
//		if headers defines a protocol that we have not bound as [Protocol]
//
// !incorrect-protocol-member!
//		if we have @required members without [Abstract] or @optional members with [Abstract]
//
// !missing-protocol-member!
//		if we have protocol members (found in header files) but not in the interface
//
// !extra-protocol-member!
//		if we have protocol members in the interface that are NOT found in header files
//
// Limitations
//
// * .NET interfaces does not allow constructors, so we cannot check for `init*` members
//
// * .NET interfaces cannot have static members, so we cannot check for [Static] members
//
// Notes: Both limitations could be _mostly_ lifted by another tests that would check types conformance to a protocol
//

using System;
using System.Collections.Generic;
using System.Text;

using Mono.Cecil;

using Clang.Ast;

namespace Extrospection {
	
	public class ObjCProtocolCheck : BaseVisitor {

		Dictionary<string, TypeDefinition> protocol_map = new Dictionary<string, TypeDefinition> ();

		public override void VisitManagedType (TypeDefinition type)
		{
			if (!type.HasCustomAttributes)
				return;

			string pname = null;
			bool informal = false;

			foreach (var ca in type.CustomAttributes) {
				switch (ca.Constructor.DeclaringType.Name) {
				case "ProtocolAttribute":
					if (!ca.HasProperties)
						continue;
					foreach (var p in ca.Properties) {
						switch (p.Name) {
						case "Name":
							pname = p.Argument.Value as string;
							break;
						case "IsInformal":
							informal = (bool) p.Argument.Value;
							break;
						}
					}
					break;
				}
			}
			if (!informal && !String.IsNullOrEmpty (pname))
				protocol_map.Add (pname, type);
		}

		public override void VisitObjCProtocolDecl (ObjCProtocolDecl decl, VisitKind visitKind)
		{
			if (visitKind != VisitKind.Enter)
				return;
			if (!decl.IsDefinition)
				return;

			// check availability macros to see if the API is available on the OS and not deprecated
			if (!decl.IsAvailable ())
				return;

			var name = decl.Name;
			TypeDefinition td;
			if (!protocol_map.TryGetValue (name, out td)) {
				Console.WriteLine ("!missing-protocol! {0} not bound", name);
				// other checks can't be done without an actual protocol to inspect
				return;
			}

			// build type selector-required map
			var map = new Dictionary<string, bool> ();
			foreach (var ca in td.CustomAttributes) {
				string export = null;
				string g_export = null;
				string s_export = null;
				bool is_required = false;
				bool is_property = false;
				bool is_static = false;
				switch (ca.Constructor.DeclaringType.Name) {
				case "ProtocolMemberAttribute":
					foreach (var p in ca.Properties) {
						switch (p.Name) {
						case "Selector":
							export = p.Argument.Value as string;
							break;
						case "GetterSelector":
							g_export = p.Argument.Value as string;
							break;
						case "SetterSelector":
							s_export = p.Argument.Value as string;
							break;
						case "IsRequired":
							is_required = (bool)p.Argument.Value;
							break;
						case "IsProperty":
							is_property = (bool)p.Argument.Value;
							break;
						case "IsStatic":
							is_static = (bool)p.Argument.Value;
							break;
						}
					}
					break;
				}
				if (is_property) {
					if (is_static) {
						g_export = "+" + g_export;
						s_export = "+" + s_export;
					}
					if (g_export != null)
						map.Add (g_export, is_required);
					if (s_export != null)
						map.Add (s_export, is_required);
				} else {
					if (is_static)
						export = "+" + export;
					if (export != null)
						map.Add (export, is_required);
				}
			}

			var remaining = new Dictionary<string, bool> (map);

			// check that required members match the [Abstract] members
			foreach (ObjCMethodDecl method in decl.Methods) {
				// some members might not be part of the current platform
				if (!method.IsAvailable ())
					continue;
				
				var selector = GetSelector (method);
				if (selector == null)
					continue;

				// a .NET interface cannot have constructors - so we cannot enforce that on the interface
				if (IsInit (selector))
					continue;

				bool is_abstract;
				if (map.TryGetValue (selector, out is_abstract)) {
					bool required = method.ImplementationControl == ObjCImplementationControl.Required;
					if (required) {
						if (!is_abstract)
							Console.WriteLine ("!incorrect-protocol-member! {0} is REQUIRED and should be abstract", GetName (decl, method));
					} else {
						if (is_abstract)
							Console.WriteLine ("!incorrect-protocol-member! {0} is OPTIONAL and should NOT be abstract", GetName (decl, method));
					}
					remaining.Remove (selector);
				} else if (!method.IsClassMethod) {
					// a .NET interface cannot have static methods - so we can only report missing instance methods
					Console.WriteLine ("!missing-protocol-member! {0} not found", GetName (decl, method));
					remaining.Remove (selector);
				}
			}

			foreach (var selector in remaining.Keys)
				Console.WriteLine ("!extra-protocol-member! unexpected selector {0}::{1} found", decl.Name, selector);
			remaining.Clear ();
			map.Clear ();

			protocol_map.Remove (name);
		}

		static string GetSelector (ObjCMethodDecl method)
		{
			var result = method.Selector.ToString ();
			if (result != null)
				return result;
			if (method.IsPropertyAccessor || (method.DeclContext is ObjCProtocolDecl))
				return method.Name;
			return null;
		}

		static string GetName (ObjCProtocolDecl decl, ObjCMethodDecl method)
		{
			var sb = new StringBuilder ();
			if (method.IsClassMethod)
				sb.Append ('+');
			sb.Append (decl.Name);
			sb.Append ("::");
			sb.Append (GetSelector (method));
			return sb.ToString ();
		}

		bool IsInit (string selector)
		{
			return selector.StartsWith ("init", StringComparison.Ordinal) && Char.IsUpper (selector [4]);
		}

		public override void End ()
		{
			// at this stage anything else we have is not something we could find in Apple's headers
			foreach (var extra in protocol_map.Keys) {
				Console.WriteLine ("!unknown-protocol! {0} bound", extra);
			}
		}
	}
}