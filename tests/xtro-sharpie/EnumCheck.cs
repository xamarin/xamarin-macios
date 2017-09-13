using System;
using System.Collections.Generic;

using Mono.Cecil;

using Clang.Ast;

namespace Extrospection {

	class EnumCheck : BaseVisitor {

		Dictionary<string,TypeDefinition> enums = new Dictionary<string, TypeDefinition> ();

		public override void VisitManagedType (TypeDefinition type)
		{
			// exclude non enum and nested enums, e.g. bunch of Selector enums in CTFont
			if (!type.IsEnum || type.IsNested)
				return;
			
			var name = type.Name;
			TypeDefinition td;
			// e.g. WatchKit.WKErrorCode and WebKit.WKErrorCode :-(
			if (!enums.TryGetValue (name, out td))
				enums.Add (name, type);
			else if (td.Namespace.StartsWith ("OpenTK.", StringComparison.Ordinal)) {
				// OpenTK duplicate a lots of enums between it's versions
			} else {
				Console.WriteLine ("!duplicate-type-name! {0} enum exists as both {1} and {2}", name, type.FullName, td.FullName);
			}
		}

		public override void VisitEnumDecl (EnumDecl decl, VisitKind visitKind)
		{
			if (visitKind != VisitKind.Enter)
				return;
			if (!decl.IsDefinition)
				return;

			string name = decl.Name;
			if (name == null)
				return;

			// check availability macros to see if the API is available on the OS and not deprecated
			if (!decl.IsAvailable ())
				return;

			var mname = Helpers.GetManagedName (name);
			TypeDefinition type;
			if (!enums.TryGetValue (mname, out type)) {
				Console.WriteLine ("!missing-enum! {0} not bound", name);
				return;
			} else
				enums.Remove (mname);

			int native_size = 4;
			bool native = false;
			// FIXME: this can be simplified
			switch (decl.IntegerQualType.ToString ()) {
			case "NSInteger":
			case "NSUInteger":
			case "CFIndex":
			case "CFOptionFlags":
				native_size = 8; // in managed code it's always the largest size
				native = true;
				break;
			case "unsigned long":
			case "unsigned int":
			case "int32_t":
			case "uint32_t":
			case "int":
			case "GLint":
			case "GLuint":
			case "GLenum":
			case "SInt32":
			case "UInt32":
			case "OptionBits": // UInt32
			case "long":
			case "FourCharCode":
			case "OSStatus":
				break;
			case "int64_t":
			case "uint64_t":
			case "unsigned long long":
			case "CVOptionFlags": // uint64_t
				native_size = 8;
				break;
			case "UInt16":
			case "int16_t":
			case "uint16_t":
			case "short":
				native_size = 2;
				break;
			case "UInt8":
			case "int8_t":
			case "uint8_t":
				native_size = 1;
				break;
			default:
				throw new NotImplementedException (decl.IntegerQualType.ToString ());
			}

			// check correct [Native] decoration
			if (native) {
				if (!IsNative (type))
					Console.WriteLine ("!missing-enum-native! {0}", name);
			} else {
				if (IsNative (type))
					Console.WriteLine ("!extra-enum-native! {0}", name);
			}

			int managed_size = 4;
			switch (GetEnumUnderlyingType (type).Name) {
			case "Byte":
			case "SByte":
				managed_size = 1;
				break;
			case "Int16":
			case "UInt16":
				managed_size = 2;
				break;
			case "Int32":
			case "UInt32":
				break;
			case "Int64":
			case "UInt64":
				managed_size = 8;
				break;
			default:
				throw new NotImplementedException ();
			}
			if (native_size != managed_size)
				Console.WriteLine ("!wrong-enum-size! {0} managed {1} vs native {2}", name, managed_size, native_size);
		}

		static bool IsNative (TypeDefinition type)
		{
			if (!type.HasCustomAttributes)
				return false;
			
			foreach (var ca in type.CustomAttributes) {
				if (ca.Constructor.DeclaringType.Name == "NativeAttribute")
					return true;
			}
			return false;
		}

		public static TypeReference GetEnumUnderlyingType (TypeDefinition self)
		{
			var fields = self.Fields;

			for (int i = 0; i < fields.Count; i++) {
				var field = fields [i];
				if (!field.IsStatic)
					return field.FieldType;
			}

			throw new ArgumentException ();
		}

		public override void End ()
		{
			// report any [Native] decorated enum for which we could not find a match in the header files
			// e.g. a typo in the name
			foreach (var extra in enums) {
				if (IsNative (extra.Value))
					Console.WriteLine ("!unknown-native-enum! {0} bound", extra.Key);
			}
		}
	}
}