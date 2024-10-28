using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Generator;

public static class Dlfcn {

	/// <summary>
	/// Returns the name of the getter method for the given type in the Dlfcn library.
	/// </summary>
	/// <param name="type">The function name to use based on the type.</param>
	/// <returns></returns>
	public static string? GetGetterMethodName (SpecialType type)
		=> type switch {
			SpecialType.System_Int16 => "GetInt16",
			SpecialType.System_Int32 => "GetInt32",
			SpecialType.System_Int64 => "GetInt64",
			SpecialType.System_UInt16 => "GetUInt16",
			SpecialType.System_UInt32 => "GetUInt32",
			SpecialType.System_UInt64 => "GetUInt64",
			_ => null, // unknown, return null and let the caller deal with it.
		};
}
