using System;
using System.Collections.Generic;

using Mono.Cecil;

using Clang.Ast;

namespace Extrospection {

	public class ObjCInterfaceCheck : BaseVisitor {

		Dictionary<string, TypeDefinition> type_map = new Dictionary<string, TypeDefinition> ();
		Dictionary<string, TypeDefinition> type_map_copy = new Dictionary<string, TypeDefinition> ();

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
				if (!type_map.TryGetValue (rname, out td)) {
					type_map.Add (rname, type);
					type_map_copy.Add (rname, type);
				} else {
					// always report in the same order (for unique error messages)
					var sorted = Helpers.Sort (type, td);
					var framework = Helpers.GetFramework (sorted.Item1);
					Log.On (framework).Add ($"!duplicate-register! {rname} exists as both {sorted.Item1.FullName} and {sorted.Item2.FullName}");
				}
			}
		}

		public override void VisitObjCCategoryDecl (ObjCCategoryDecl decl, VisitKind visitKind)
		{
			if (visitKind != VisitKind.Enter)
				return;

			var categoryName = decl.Name;
			if (categoryName == null)
				return;

			// check availability macros to see if the API is available on the OS and not deprecated
			if (!decl.IsAvailable ())
				return;

			var framework = Helpers.GetFramework (decl);
			if (framework == null)
				return;

			var ciName = decl.ClassInterface.Name;
			if (!type_map_copy.TryGetValue (ciName, out var td)) {
				// other checks can't be done without an actual type to inspect
				return;
			}

			// check protocols
			foreach (var protocol in decl.Protocols) {
				var pname = protocol.Name;
				if (!ImplementProtocol (pname, td))
					Log.On (framework).Add ($"!missing-protocol-conformance! {ciName} should conform to {pname} (defined in '{categoryName}' category)");
			}
		}

		public override void VisitObjCInterfaceDecl (ObjCInterfaceDecl decl, VisitKind visitKind)
		{
			if (visitKind != VisitKind.Enter)
				return;
			if (!decl.IsDefinition)
				return;

			var name = decl.Name;

			// check availability macros to see if the API is available on the OS and not deprecated
			if (!decl.IsAvailable ())
				return;

			var framework = Helpers.GetFramework (decl);
			if (framework == null)
				return;

			if (!type_map.TryGetValue (name, out var td)) {
				Log.On (framework).Add ($"!missing-type! {name} not bound");
				// other checks can't be done without an actual type to inspect
				return;
			}

			// check base type
			var nbt = decl.SuperClass?.Name;
			var mbt = td.BaseType?.Resolve ().GetName ();
			if (nbt != mbt)
				Log.On (framework).Add ($"!wrong-base-type! {name} expected {nbt} actual {mbt}");

			// check protocols
			foreach (var protocol in decl.Protocols) {
				var pname = protocol.Name;
				if (!ImplementProtocol (pname, td))
					Log.On (framework).Add ($"!missing-protocol-conformance! {name} should conform to {pname}");
			}

			// TODO : check for extraneous protocols

			type_map.Remove (name);
		}

		public override void End ()
		{
			// at this stage anything else we have is not something we could find in Apple's headers
			foreach (var kvp in type_map) {
				var extra = kvp.Key;
				if (extra [0] == '_')
					continue;
				var type = kvp.Value;
				// internal inner classes are not mapped to native ones
				if (type.IsNestedAssembly)
					continue;
				var framework = Helpers.MapFramework (type.Namespace);
				Log.On (framework).Add ($"!unknown-type! {extra} bound");
			}
		}

		// - version check
		bool ImplementProtocol (string protocol, TypeDefinition td)
		{
			if (td == null)
				return false;
			if (td.HasInterfaces) {
				foreach (var intf in td.Interfaces) {
					TypeReference ifaceType;
#if CECIL_0_10
					ifaceType = intf?.InterfaceType;
#else
					ifaceType = intf;
#endif
					if (protocol == GetProtocolName (ifaceType?.Resolve ()))
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