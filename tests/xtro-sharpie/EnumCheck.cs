using System;
using System.Collections.Generic;

using Mono.Cecil;

using Clang.Ast;

namespace Extrospection {

	class EnumCheck : BaseVisitor {

		Dictionary<string,TypeDefinition> enums = new Dictionary<string, TypeDefinition> (StringComparer.InvariantCultureIgnoreCase);
		Dictionary<long, FieldDefinition> managed_signed_values = new Dictionary<long, FieldDefinition> ();
		Dictionary<ulong, FieldDefinition> managed_unsigned_values = new Dictionary<ulong, FieldDefinition> ();
		Dictionary<long, string> native_signed_values = new Dictionary<long, string> ();
		Dictionary<ulong, string> native_unsigned_values = new Dictionary<ulong,string> ();

		public override void VisitManagedType (TypeDefinition type)
		{
			// exclude non enum and nested enums, e.g. bunch of Selector enums in CTFont
			if (!type.IsEnum || type.IsNested)
				return;
			
			// exclude obsolete enums, presumably we already know there's something wrong with them if they've been obsoleted.
			if (type.IsObsolete ())
				return;

			var name = type.Name;
			// e.g. WatchKit.WKErrorCode and WebKit.WKErrorCode :-(
			if (!enums.TryGetValue (name, out var td))
				enums.Add (name, type);
			else {
				var (t1, t2) = Helpers.Sort (type, td);
				if (t1.Namespace.StartsWith ("OpenTK.", StringComparison.Ordinal)) {
					// OpenTK duplicate a lots of enums between it's versions
				} else if (t1.IsNotPublic && String.IsNullOrEmpty (t1.Namespace)) {
					// ignore special, non exposed types
				} else {
					var framework = Helpers.GetFramework (t1);
					Log.On (framework).Add ($"!duplicate-type-name! {name} enum exists as both {t1.FullName} and {t2.FullName}");
				}
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

			var framework = Helpers.GetFramework (decl);
			if (framework == null)
				return;
			
			var mname = Helpers.GetManagedName (name);
			if (!enums.TryGetValue (mname, out var type)) {
				Log.On (framework).Add ($"!missing-enum! {name} not bound");
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
			case "AVAudioInteger":
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
					Log.On (framework).Add ($"!missing-enum-native! {name}");
			} else {
				if (IsNative (type))
					Log.On (framework).Add ($"!extra-enum-native! {name}");
			}

			int managed_size = 4;
			bool signed = true;
			switch (GetEnumUnderlyingType (type).Name) {
			case "Byte":
				signed = false;
				managed_size = 1;
				break;
			case "SByte":
				managed_size = 1;
				break;
			case "Int16":
				managed_size = 2;
				break;
			case "UInt16":
				signed = false;
				managed_size = 2;
				break;
			case "Int32":
				break;
			case "UInt32":
				signed = false;
				break;
			case "Int64":
				managed_size = 8;
				break;
			case "UInt64":
				signed = false;
				managed_size = 8;
				break;
			default:
				throw new NotImplementedException ();
			}

			var fields = type.Fields;
			if (signed) {
				managed_signed_values.Clear ();
				native_signed_values.Clear ();
				foreach (var f in fields) {
					// skip special `value__`
					if (f.IsRuntimeSpecialName && !f.IsStatic)
						continue;
					if (!f.IsObsolete ())
						managed_signed_values [Convert.ToInt64 (f.Constant)] = f;
				}

				long n = 0;
				foreach (var value in decl.Values) {
					if ((value.InitExpr != null) && value.InitExpr.EvaluateAsInt (decl.AstContext, out var integer))
						n = integer.SExtValue;

					native_signed_values [n] = value.ToString ();
					// assume, sequentially assigned (in case next `value.InitExpr` is null)
					n++;
				}

				foreach (var value in native_signed_values.Keys) {
					if (!managed_signed_values.ContainsKey (value))
						Log.On (framework).Add ($"!missing-enum-value! {type.Name} native value {native_signed_values [value]} = {value} not bound");
					else
						managed_signed_values.Remove (value);
				}

				foreach (var value in managed_signed_values.Keys) {
					if ((value == 0) && IsExtraZeroValid (type.Name, managed_signed_values [0].Name))
						continue;
					// value could be decorated with `[No*]` and those should not be reported
					if (managed_signed_values [value].IsAvailable ())
						Log.On (framework).Add ($"!extra-enum-value! Managed value {value} for {type.Name}.{managed_signed_values [value].Name} not found in native headers");
				}
			} else {
				managed_unsigned_values.Clear ();
				native_unsigned_values.Clear ();
				foreach (var f in fields) {
					// skip special `value__`
					if (f.IsRuntimeSpecialName && !f.IsStatic)
						continue;
					if (!f.IsObsolete ())
						managed_unsigned_values [Convert.ToUInt64 (f.Constant)] = f;
				}

				ulong n = 0;
				foreach (var value in decl.Values) {
					if ((value.InitExpr != null) && value.InitExpr.EvaluateAsInt (decl.AstContext, out var integer))
						n = integer.ZExtValue;

					native_unsigned_values [n] = value.ToString ();
					// assume, sequentially assigned (in case next `value.InitExpr` is null)
					n++;
				}

				foreach (var value in native_unsigned_values.Keys) {
					if (!managed_unsigned_values.ContainsKey (value)) {
						// only for unsigned (flags) native enums we allow all bits set on 32 bits (UInt32.MaxValue)
						// to be equal to all bit set on 64 bits (UInt64.MaxValue) since the MaxValue differs between
						// 32bits (e.g. watchOS) and 64bits (all others) platforms
						var log = true;
						if (native && (value == UInt32.MaxValue)) {
							log = !managed_unsigned_values.ContainsKey (UInt64.MaxValue);
							managed_unsigned_values.Remove (UInt64.MaxValue);
						}
						if (log)
							Log.On (framework).Add ($"!missing-enum-value! {type.Name} native value {native_unsigned_values [value]} = {value} not bound");
					} else
						managed_unsigned_values.Remove (value);
				}

				foreach (var value in managed_unsigned_values.Keys) {
					if ((value == 0) && IsExtraZeroValid (type.Name, managed_unsigned_values [0].Name))
						continue;
					// value could be decorated with `[No*]` and those should not be reported
					if (managed_unsigned_values [value].IsAvailable ())
						Log.On (framework).Add ($"!extra-enum-value! Managed value {value} for {type.Name}.{managed_unsigned_values [value].Name} not found in native headers");
				}
			}

			if (native_size != managed_size)
				Log.On (framework).Add ($"!wrong-enum-size! {name} managed {managed_size} vs native {native_size}");
		}

		static bool IsExtraZeroValid (string typeName, string valueName)
		{
			switch (valueName) {
			// we often add `None = 0` to flags, when none exists
			case "None":
				return true;
			// `Ok = 0` and `Success = 0` are often added to errors enums
			case "Ok":
			case "Success":
				if (typeName.EndsWith ("ErrorCode", StringComparison.Ordinal))
					return true;
				if (typeName.EndsWith ("Status", StringComparison.Ordinal))
					return true;
				break;
			// used in HealthKit for a default value
			case "NotApplicable":
				return typeName.StartsWith ("HK", StringComparison.Ordinal);
			}
			return false;
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
				var t = extra.Value;
				if (!IsNative (t))
					continue;
				var framework = Helpers.GetFramework (t);
				Log.On (framework).Add ($"!unknown-native-enum! {extra.Key} bound");
			}
		}
	}
}