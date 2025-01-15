// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Generator.Extensions;

static class SpecialTypeExtensions {
	
	/// <summary>
	/// Return the keyword for a given special type.
	/// </summary>
	/// <param name="self">The special type to convert.</param>
	/// <returns>The string representation of the keyword.</returns>
	public static string? GetKeyword (this SpecialType? self) => self switch {
		SpecialType.System_SByte => "sbyte",
		SpecialType.System_Byte => "byte",
		SpecialType.System_Int16 => "short",
		SpecialType.System_UInt16 => "ushort",
		SpecialType.System_Int32 => "int",
		SpecialType.System_UInt32 => "uint",
		SpecialType.System_Int64 => "long",
		SpecialType.System_UInt64 => "ulong",
		_ => null
	};
}
