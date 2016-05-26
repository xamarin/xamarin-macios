using System;
using System.Collections.Generic;

using Mono.Cecil;

using Clang.Ast;

namespace Extrospection {

	public class ObjCInterfaceCheck : BaseVisitor {

		Dictionary<string, TypeDefinition> type_map = new Dictionary<string, TypeDefinition> ();

		public override void VisitManagedType (TypeDefinition type)
		{
			if (!type.HasCustomAttributes)
				return;

			string rname = null;
			bool wrapper = true;
			bool skip = false;

			foreach (var ca in type.CustomAttributes) {
				switch (ca.Constructor.DeclaringType.Name) {
				case "RegisterAttribute":
					rname = type.Name;
					if (ca.HasConstructorArguments) {
						rname = (ca.ConstructorArguments [0].Value as string);
						if (ca.ConstructorArguments.Count > 1)
							wrapper = (bool)ca.ConstructorArguments [1].Value;
					}
					if (ca.HasProperties) {
						foreach (var arg in ca.Properties) {
							switch (arg.Name) {
							case "Wrapper":
								wrapper = (bool)arg.Argument.Value;
								break;
							case "SkipRegistration":
								skip = (bool)arg.Argument.Value;
								break;
							}
						}
					}
					break;
				case "ProtocolAttribute":
					// exclude protocols
					return;
				}
			}
			if (!skip && wrapper && !String.IsNullOrEmpty (rname)) {
				TypeDefinition td;
				if (!type_map.TryGetValue (rname, out td))
					type_map.Add (rname, type);
				else {
					Console.WriteLine ("!duplicate-register! {0} exists as both {1} and {2}", rname, type.FullName, td.FullName);
				}
			}
		}

		public override void VisitObjCInterfaceDecl (ObjCInterfaceDecl decl, VisitKind visitKind)
		{
			if (visitKind != VisitKind.Enter)
				return;
			if (!decl.IsDefinition)
				return;

			var name = decl.Name;
			if (name.EndsWith ("Internal", StringComparison.Ordinal))
				return;

			// check availability macros to see if the API is available on the OS and not deprecated
			if (!decl.IsAvailable ())
				return;

			TypeDefinition td;
			if (!type_map.TryGetValue (name, out td)) {
				Console.WriteLine ("!missing-type! {0} not bound", name);
				// other checks can't be done without an actual type to inspect
				return;
			}

			// check base type
			var nbt = decl.SuperClass?.Name;
			var mbt = td.BaseType?.Resolve ().GetName ();
			if (nbt != mbt)
				Console.WriteLine ("!wrong-base-type! {0} expected {1} actual {2}", name, nbt, mbt);

			// check protocols
			foreach (var protocol in decl.Protocols) {
				var pname = protocol.Name;
				if (!ImplementProtocol (pname, td))
					Console.WriteLine ("!missing-protocol-conformance! {0} should conform to {1}", name, pname);
			}

			// TODO : check for extraneous protocols

			type_map.Remove (name);
		}

		public override void End ()
		{
			// at this stage anything else we have is not something we could find in Apple's headers
			foreach (var extra in type_map.Keys) {
				if (extra [0] == '_')
					continue;
				Console.WriteLine ("!unknown-type! {0} bound", extra);
			}
		}

		// - version check
		bool ImplementProtocol (string protocol, TypeDefinition td)
		{
			if (td == null)
				return false;
			if (td.HasInterfaces) {
				foreach (var intf in td.Interfaces) {
					if (protocol == GetProtocolName (intf?.Resolve ()))
						return true;
				}
			}
			return ImplementProtocol (protocol, td.BaseType?.Resolve ());
		}

		public static string GetProtocolName (TypeDefinition td)
		{
			if (!td.HasCustomAttributes)
				return null;

			foreach (var ca in td.CustomAttributes) {
				if (ca.Constructor.DeclaringType.Name == "ProtocolAttribute") {
					var name = td.Name;
					if (ca.HasProperties)
						name = ca.Properties [0].Argument.Value as string;
					return name;
				}
			}
			return null;
		}
	}
}