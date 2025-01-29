// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Macios.Generator.DataModel;

readonly partial struct Parameter {

	public enum VariableType {
		Handle,
		NSArray,
		BlockLiteral,
		PrimitivePointer,
	}

	/// <summary>
	/// Returns the name of the aux variable that would have needed for the given parameter. Use the
	/// variable type to name it.
	/// </summary>
	/// <param name="variableType">The type of aux variable.</param>
	/// <returns>The name of the aux variable to use.</returns>
	public string? GetNameForVariableType (VariableType variableType)
		=> variableType switch {
			VariableType.Handle => $"{Name}__handle__",
			VariableType.NSArray => $"nsa_{Name}",
			VariableType.BlockLiteral => $"block_ptr_{Name}",
			VariableType.PrimitivePointer => $"converted_{Name}",
			_ => null
		};
}
