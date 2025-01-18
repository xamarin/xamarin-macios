// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#pragma warning disable APL0003
using Microsoft.CodeAnalysis;
using TypeInfo = Microsoft.Macios.Generator.DataModel.TypeInfo;

namespace Microsoft.Macios.Generator.Tests;

static class TestDataFactory {
	public static TypeInfo ReturnTypeForString (bool isNullable = false)
		=> new (
			name: "string",
			specialType: SpecialType.System_String,
			isNullable: isNullable,
			isBlittable: false,
			isSmartEnum: false,
			isArray: false,
			isReferenceType: true
		) {
			Interfaces = [
				"System.Collections.Generic.IEnumerable<char>",
				"System.Collections.IEnumerable",
				"System.ICloneable",
				"System.IComparable",
				"System.IComparable<string?>",
				"System.IConvertible",
				"System.IEquatable<string?>",
				"System.IParsable<string>",
				"System.ISpanParsable<string>"
			],
			MetadataName = "String",
			Parents = ["object"],
			IsNSObject = false,
			IsINativeObject = false,
		};

	public static TypeInfo ReturnTypeForInt (bool isNullable = false)
		=> new (
			name: "int",
			specialType: SpecialType.System_Int32,
			isBlittable: !isNullable,
			isNullable: isNullable
		) {
			Parents = ["System.ValueType", "object"],
			Interfaces = isNullable
				? []
				: [
					"System.IComparable",
					"System.IComparable<int>",
					"System.IConvertible",
					"System.IEquatable<int>",
					"System.IFormattable",
					"System.IParsable<int>",
					"System.ISpanFormattable",
					"System.ISpanParsable<int>",
					"System.IUtf8SpanFormattable",
					"System.IUtf8SpanParsable<int>",
					"System.Numerics.IAdditionOperators<int, int, int>",
					"System.Numerics.IAdditiveIdentity<int, int>",
					"System.Numerics.IBinaryInteger<int>",
					"System.Numerics.IBinaryNumber<int>",
					"System.Numerics.IBitwiseOperators<int, int, int>",
					"System.Numerics.IComparisonOperators<int, int, bool>",
					"System.Numerics.IEqualityOperators<int, int, bool>",
					"System.Numerics.IDecrementOperators<int>",
					"System.Numerics.IDivisionOperators<int, int, int>",
					"System.Numerics.IIncrementOperators<int>",
					"System.Numerics.IModulusOperators<int, int, int>",
					"System.Numerics.IMultiplicativeIdentity<int, int>",
					"System.Numerics.IMultiplyOperators<int, int, int>",
					"System.Numerics.INumber<int>",
					"System.Numerics.INumberBase<int>",
					"System.Numerics.ISubtractionOperators<int, int, int>",
					"System.Numerics.IUnaryNegationOperators<int, int>",
					"System.Numerics.IUnaryPlusOperators<int, int>",
					"System.Numerics.IShiftOperators<int, int, int>",
					"System.Numerics.IMinMaxValue<int>",
					"System.Numerics.ISignedNumber<int>"
				],
			MetadataName = "Int32",
		};

	public static TypeInfo ReturnTypeForIntPtr (bool isNullable = false)
		=> new (
			name: "nint",
			specialType: SpecialType.System_IntPtr,
			isBlittable: !isNullable,
			isNullable: isNullable
		) {
			Parents = ["System.ValueType", "object"],
			Interfaces = isNullable
				? []
				: [
					"System.IComparable",
					"System.IComparable<nint>",
					"System.IEquatable<nint>",
					"System.IFormattable",
					"System.IParsable<nint>",
					"System.ISpanFormattable",
					"System.ISpanParsable<nint>",
					"System.IUtf8SpanFormattable",
					"System.IUtf8SpanParsable<nint>",
					"System.Numerics.IAdditionOperators<nint, nint, nint>",
					"System.Numerics.IAdditiveIdentity<nint, nint>",
					"System.Numerics.IBinaryInteger<nint>",
					"System.Numerics.IBinaryNumber<nint>",
					"System.Numerics.IBitwiseOperators<nint, nint, nint>",
					"System.Numerics.IComparisonOperators<nint, nint, bool>",
					"System.Numerics.IEqualityOperators<nint, nint, bool>",
					"System.Numerics.IDecrementOperators<nint>",
					"System.Numerics.IDivisionOperators<nint, nint, nint>",
					"System.Numerics.IIncrementOperators<nint>",
					"System.Numerics.IModulusOperators<nint, nint, nint>",
					"System.Numerics.IMultiplicativeIdentity<nint, nint>",
					"System.Numerics.IMultiplyOperators<nint, nint, nint>",
					"System.Numerics.INumber<nint>",
					"System.Numerics.INumberBase<nint>",
					"System.Numerics.ISubtractionOperators<nint, nint, nint>",
					"System.Numerics.IUnaryNegationOperators<nint, nint>",
					"System.Numerics.IUnaryPlusOperators<nint, nint>",
					"System.Numerics.IShiftOperators<nint, int, nint>",
					"System.Numerics.IMinMaxValue<nint>",
					"System.Numerics.ISignedNumber<nint>",
					"System.Runtime.Serialization.ISerializable"
				],
			MetadataName = "IntPtr",
		};

	public static TypeInfo ReturnTypeForBool ()
		=> new (
			name: "bool",
			specialType: SpecialType.System_Boolean,
			isBlittable: false
		) {
			Parents = ["System.ValueType", "object"],
			Interfaces = [
				"System.IComparable",
				"System.IComparable<bool>",
				"System.IConvertible",
				"System.IEquatable<bool>",
				"System.IParsable<bool>",
				"System.ISpanParsable<bool>"
			],
			MetadataName = "Boolean",
		};

	public static TypeInfo ReturnTypeForVoid ()
		=> new ("void", SpecialType.System_Void) { Parents = ["System.ValueType", "object"], };

	public static TypeInfo ReturnTypeForClass (string className, bool isNullable = false)
		=> new (
			name: className,
			isReferenceType: true,
			isNullable: isNullable
		) { Parents = ["object"] };

	public static TypeInfo ReturnTypeForGeneric (string genericName, bool isNullable = false)
		=> new (
			name: genericName,
			isReferenceType: false,
			isNullable: isNullable
		);

	public static TypeInfo ReturnTypeForStruct (string structName)
		=> new (
			name: structName
		) { Parents = ["System.ValueType", "object"] };

	public static TypeInfo ReturnTypeForEnum (string enumName, bool isSmartEnum = false)
		=> new (
			name: enumName,
			isBlittable: true,
			isSmartEnum: isSmartEnum
		) {
			Parents = [
				"System.Enum",
				"System.ValueType",
				"object"
			],
			Interfaces = [
				"System.IComparable",
				"System.IConvertible",
				"System.IFormattable",
				"System.ISpanFormattable"
			],
			EnumUnderlyingType = SpecialType.System_Int32,
		};

	public static TypeInfo ReturnTypeForArray (string type, bool isNullable = false, bool isBlittable = false)
		=> new (
			name: type,
			isNullable: isNullable,
			isBlittable: isBlittable,
			isArray: true,
			isReferenceType: true
		) {
			Parents = ["System.Array", "object"],
			Interfaces = [
				$"System.Collections.Generic.IList<{type}>",
				$"System.Collections.Generic.IReadOnlyList<{type}>",
				"System.Collections.ICollection",
				"System.Collections.IEnumerable",
				"System.Collections.IList",
				"System.Collections.IStructuralComparable",
				"System.Collections.IStructuralEquatable",
				"System.ICloneable"
			]
		};

	public static TypeInfo ReturnTypeForAction ()
		=> new (
			name: "System.Action",
			isNullable: false,
			isBlittable: false,
			isArray: false,
			isReferenceType: true
		) {
			Parents = [
				"System.MulticastDelegate",
				"System.Delegate",
				"object"
			],
			Interfaces = [
				"System.ICloneable",
				"System.Runtime.Serialization.ISerializable",
			]
		};

	public static TypeInfo ReturnTypeForAction (params string [] parameters)
		=> new (
			name: $"System.Action<{string.Join (", ", parameters)}>",
			isNullable: false,
			isBlittable: false,
			isArray: false,
			isReferenceType: true
		) {
			Parents = [
				"System.MulticastDelegate",
				"System.Delegate",
				"object"
			],
			Interfaces = [
				"System.ICloneable",
				"System.Runtime.Serialization.ISerializable",
			]
		};

	public static TypeInfo ReturnTypeForFunc (params string [] parameters)
		=> new (
			name: $"System.Func<{string.Join (", ", parameters)}>",
			isNullable: false,
			isBlittable: false,
			isArray: false,
			isReferenceType: true
		) {
			Parents = [
				"System.MulticastDelegate",
				"System.Delegate",
				"object"
			],
			Interfaces = [
				"System.ICloneable",
				"System.Runtime.Serialization.ISerializable",
			]
		};

	public static TypeInfo ReturnTypeForDelegate (string delegateName)
		=> new (
			name: delegateName,
			isNullable: false,
			isBlittable: false,
			isArray: false,
			isReferenceType: true
		) {
			Parents = [
				"System.MulticastDelegate",
				"System.Delegate",
				"object"
			],
			Interfaces = [
				"System.ICloneable",
				"System.Runtime.Serialization.ISerializable",
			]
		};
}
