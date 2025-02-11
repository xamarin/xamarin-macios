// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Microsoft.Macios.Generator.Extensions;

static class SpecialTypeExtensions {

	public static string? GetKeyword (this SpecialType? self)
		=> self?.GetKeyword ();

	/// <summary>
	/// Return the keyword for a given special type.
	/// </summary>
	/// <param name="self">The special type to convert.</param>
	/// <returns>The string representation of the keyword.</returns>
	public static string GetKeyword (this SpecialType self)
	{
		var kind = self switch {
			SpecialType.System_Void => SyntaxKind.VoidKeyword,
			SpecialType.System_Boolean => SyntaxKind.BoolKeyword,
			SpecialType.System_Byte => SyntaxKind.ByteKeyword,
			SpecialType.System_SByte => SyntaxKind.SByteKeyword,
			SpecialType.System_Int16 => SyntaxKind.ShortKeyword,
			SpecialType.System_UInt16 => SyntaxKind.UShortKeyword,
			SpecialType.System_Int32 => SyntaxKind.IntKeyword,
			SpecialType.System_UInt32 => SyntaxKind.UIntKeyword,
			SpecialType.System_Int64 => SyntaxKind.LongKeyword,
			SpecialType.System_UInt64 => SyntaxKind.ULongKeyword,
			SpecialType.System_Double => SyntaxKind.DoubleKeyword,
			SpecialType.System_Single => SyntaxKind.FloatKeyword,
			SpecialType.System_Decimal => SyntaxKind.DecimalKeyword,
			SpecialType.System_String => SyntaxKind.StringKeyword,
			SpecialType.System_Char => SyntaxKind.CharKeyword,
			SpecialType.System_Object => SyntaxKind.ObjectKeyword,
			// Note that "dynamic" is a contextual keyword, so it should never show up here.
			_ => SyntaxKind.None
		};
		return Token (kind).Text;
	}
}
