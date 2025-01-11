// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
#pragma warning disable APL0003
using Microsoft.Macios.Generator.DataModel;

namespace Microsoft.Macios.Generator.Tests;

static class TestDataFactory {

	public static ReturnType ReturnTypeForString ()
		=> new (
			type: "string",
			isNullable: false,
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
			Parents = ["object"],
			IsNSObject = false,
			IsNativeObject = false,
		};

	public static ReturnType ReturnTypeForInt (bool isNullable = false)
		=> new (
			type: "int",
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
		};

	public static ReturnType ReturnTypeForBool ()
		=> new (
			type: "bool",
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
			]
		};

	public static ReturnType ReturnTypeForVoid ()
		=> new ("void") {
			Parents = ["System.ValueType", "object"],
		};

	public static ReturnType ReturnTypeForClass (string className)
		=> new (
			type: className,
			isReferenceType: true
		) {
			Parents = ["object"]
		};

	public static ReturnType ReturnTypeForStruct (string structName)
		=> new (
			type: structName
		) {
			Parents = ["System.ValueType", "object"]
		};

	public static ReturnType ReturnTypeForEnum (string enumName, bool isSmartEnum = false)
		=> new (
			type: enumName,
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
			]
		};

	public static ReturnType ReturnTypeForArray (string type, bool isNullable = false, bool isBlittable = false)
		=> new (
			type: type,
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
}
