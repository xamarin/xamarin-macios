using System;
using System.Collections.Generic;

using Mono.Cecil;

using Clang.Ast;

namespace Extrospection {

	class EnumCheck : BaseVisitor {
		class ManagedValue {
			public FieldDefinition Field;
			public EnumConstantDecl Decl;
		}

		Dictionary<string, TypeDefinition> enums = new Dictionary<string, TypeDefinition> (StringComparer.InvariantCultureIgnoreCase);
		Dictionary<string, TypeDefinition> obsoleted_enums = new Dictionary<string, TypeDefinition> ();
		Dictionary<object, ManagedValue> managed_values = new Dictionary<object, ManagedValue> ();
		Dictionary<object, (string Name, EnumConstantDecl Decl)> native_values = new Dictionary<object, (string Name, EnumConstantDecl Decl)> ();

		public override void VisitManagedType (TypeDefinition type)
		{
			// exclude non enum and nested enums, e.g. bunch of Selector enums in CTFont
			if (!type.IsEnum || type.IsNested)
				return;

			var name = type.Name;

			// exclude obsolete enums, presumably we already know there's something wrong with them if they've been obsoleted.
			if (type.IsObsolete ()) {
				obsoleted_enums [name] = type;
				return;
			}

			if (AttributeHelpers.HasAnyObsoleted (type)) {
				obsoleted_enums [name] = type;
				return;
			}

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
			if (name is null)
				return;

			// check availability macros to see if the API is available on the OS and not deprecated
			if (!decl.IsAvailable ())
				return;

			var framework = Helpers.GetFramework (decl);
			if (framework is null)
				return;

			var mname = Helpers.GetManagedName (name);

			// If our enum is obsoleted, then don't process it.
			if (obsoleted_enums.ContainsKey (mname))
				return;

			if (!enums.TryGetValue (mname, out var type)) {
				if (!decl.IsDeprecated ()) // don't report deprecated enums as unbound
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
			if (!decl.IsDeprecated ()) {
				if (native) {
					if (!IsNative (type))
						Log.On (framework).Add ($"!missing-enum-native! {name}");
				} else {
					if (IsNative (type))
						Log.On (framework).Add ($"!extra-enum-native! {name}");
				}
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

			native_values.Clear ();
			managed_values.Clear ();

			// collect all the native enum values
			var nativeConstant = signed ? (object) 0L : (object) 0UL;
			foreach (var value in decl.Values) {
				if ((value.InitExpr is not null) && value.InitExpr.EvaluateAsInt (decl.AstContext, out var integer)) {
					if (signed) {
						nativeConstant = integer.SExtValue;
					} else {
						nativeConstant = integer.ZExtValue;
					}
				}

				if (native_values.TryGetValue (nativeConstant, out var entry)) {
					// the same constant might be used for multiple values - some deprecated and some not,
					// only overwrite if the current value isn't deprecated
					if (!value.IsDeprecated ())
						native_values [nativeConstant] = new (value.ToString (), value);
				} else {
					native_values [nativeConstant] = new (value.ToString (), value);
				}
				// assume, sequentially assigned (in case next `value.InitExpr` is null)
				var t = nativeConstant.GetType ();
				if (signed) {
					nativeConstant = 1L + (long) nativeConstant;
				} else {
					nativeConstant = 1UL + (ulong) nativeConstant;
				}
			}

			// collect all the managed enum values
			var fields = type.Fields;
			foreach (var f in fields) {
				// skip special `value__`
				if (f.IsRuntimeSpecialName && !f.IsStatic)
					continue;
				if (f.IsObsolete ())
					continue;

				object managedValue;
				if (signed) {
					managedValue = Convert.ToInt64 (f.Constant);
				} else {
					managedValue = Convert.ToUInt64 (f.Constant);
				}
				managed_values [managedValue] = new ManagedValue { Field = f };
			}

			foreach (var kvp in native_values) {
				var value = kvp.Key;
				var valueDecl = kvp.Value.Decl;
				var valueName = kvp.Value.Name;

				if (managed_values.TryGetValue (value, out var entry)) {
					entry.Decl = valueDecl;
				} else {
					// only for unsigned (flags) native enums we allow all bits set on 32 bits (UInt32.MaxValue)
					// to be equal to all bit set on 64 bits (UInt64.MaxValue) since the MaxValue differs between
					// 32bits (e.g. watchOS) and 64bits (all others) platforms
					if (!signed && native && (ulong) value == UInt32.MaxValue && managed_values.Remove (UInt64.MaxValue))
						continue;

					// couldn't find a matching managed enum value for the native enum value
					// don't report deprecated native enum values (or if the native enum itself is deprecated) as missing
					if (!valueDecl.IsDeprecated () && !decl.IsDeprecated ())
						Log.On (framework).Add ($"!missing-enum-value! {type.Name} native value {valueName} = {value} not bound");
				}
			}

			foreach (var kvp in managed_values) {
				var value = kvp.Key;
				var valueField = kvp.Value.Field;
				var valueDecl = kvp.Value.Decl;
				var fieldName = valueField.Name;

				// A 0 might be a valid extra value sometimes
				var isZero = signed ? (long) value == 0 : (ulong) value == 0;
				if (isZero && IsExtraZeroValid (type.Name, fieldName))
					continue;

				if (valueDecl is null) {
					// we have a managed enum value, but no corresponding native value
					// this is only an issue if the managed enum value is available and not obsoleted
					if (valueField.IsAvailable () && !valueField.IsObsolete ())
						Log.On (framework).Add ($"!extra-enum-value! Managed value {value} for {type.Name}.{fieldName} not found in native headers");
					continue;
				}

				if (!valueDecl.IsAvailable ()) {
					// if the native enum value isn't available, the managed one shouldn't be either
					if (valueField.IsAvailable () && !IsErrorEnum (type))
						Log.On (framework).Add ($"!extra-enum-value! Managed value {value} for {type.Name}.{fieldName} is available for the current platform while the value in the native header is not");
					continue;
				}
			}

			if (native_size != managed_size && !decl.IsDeprecated ())
				Log.On (framework).Add ($"!wrong-enum-size! {name} managed {managed_size} vs native {native_size}");
		}

		static bool IsErrorEnum (TypeDefinition type)
		{
			if (!type.IsEnum)
				return false;

			if (type.Name.EndsWith ("Error", StringComparison.Ordinal))
				return true;
			if (type.Name.EndsWith ("ErrorCode", StringComparison.Ordinal))
				return true;

			if (!type.HasCustomAttributes)
				return false;
			foreach (var ca in type.CustomAttributes) {
				if (ca.AttributeType.Name == "ErrorDomainAttribute")
					return true;
			}
			return false;
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
				if (obsoleted_enums.ContainsKey (t.Name))
					continue;
				if (!IsNative (t))
					continue;
				var framework = Helpers.GetFramework (t);
				Log.On (framework).Add ($"!unknown-native-enum! {extra.Key} bound");
			}
		}
	}
}
