// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.Extensions;

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
		BindFrom,
	}

	/// <summary>
	/// Returns the bind from data if present in the binding.
	/// </summary>
	public BindFromData? BindAs { get; init; }

	/// <summary>
	/// Returns if the parameter needs a null check when the code is generated.
	/// </summary>
	public bool NeedsNullCheck {
		get {
			if (ReferenceKind != ReferenceKind.None)
				return false;
			if (Type.IsNullable)
				return false;
			return Type.IsWrapped || Type.IsReferenceType;
		}
	}

	public static bool TryCreate (IParameterSymbol symbol, ParameterSyntax declaration, RootContext context,
		[NotNullWhen (true)] out Parameter? parameter)
	{
		DelegateInfo? delegateInfo = null;
		if (symbol.Type is INamedTypeSymbol namedTypeSymbol
			&& namedTypeSymbol.DelegateInvokeMethod is not null) {
			DelegateInfo.TryCreate (namedTypeSymbol.DelegateInvokeMethod, out delegateInfo);
		}

		parameter = new (symbol.Ordinal, new (symbol.Type, context.Compilation), symbol.Name) {
			BindAs = symbol.GetBindFromData (),
			IsOptional = symbol.IsOptional,
			IsParams = symbol.IsParams,
			IsThis = symbol.IsThis,
			DefaultValue = (symbol.HasExplicitDefaultValue) ? symbol.ExplicitDefaultValue?.ToString () : null,
			ReferenceKind = symbol.RefKind.ToReferenceKind (),
			Delegate = delegateInfo,
			Attributes = declaration.GetAttributeCodeChanges (context.SemanticModel),
		};
		return true;
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
			VariableType.BindFrom => $"nsb_{cleanedName}",
			_ => null
		};
	}
}
