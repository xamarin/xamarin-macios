// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Macios.Generator.DataModel;

readonly partial struct Parameter {

	public enum VariableType {
		BlockLiteral,
		Handle,
		NSArray,
		NSString,
		NSStringStruct,
		PrimitivePointer,
		StringPointer,
	}

	/// <summary>
	/// Returns the name of the aux variable that would have needed for the given parameter. Use the
	/// variable type to name it.
	/// </summary>
	/// <param name="variableType">The type of aux variable.</param>
	/// <returns>The name of the aux variable to use.</returns>
	public string? GetNameForVariableType (VariableType variableType)
	{
		var cleanedName = Name.Replace ("@", "");
		return variableType switch {
			VariableType.BlockLiteral => $"block_ptr_{cleanedName}",
			VariableType.Handle => $"{cleanedName}__handle__",
			VariableType.NSArray => $"nsa_{cleanedName}",
			VariableType.NSString => $"ns{cleanedName}",
			VariableType.NSStringStruct => $"_s{cleanedName}",
			VariableType.PrimitivePointer => $"converted_{cleanedName}",
			VariableType.StringPointer => $"_p{cleanedName}",
			_ => null
		};
	}
}
