// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Macios.Transformer.Attributes;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Extensions;

namespace Microsoft.Macios.Generator.DataModel;

readonly partial struct Parameter {

	/// <summary>
	/// Returns the bind from data if present in the binding.
	/// </summary>
	public BindAsData? BindAs => BindAsAttribute;

	public static bool TryCreate (IParameterSymbol symbol, ParameterSyntax declaration, SemanticModel semanticModel,
		[NotNullWhen (true)] out Parameter? parameter)
	{
		DelegateInfo? delegateInfo = null;
		if (symbol.Type is INamedTypeSymbol namedTypeSymbol
			&& namedTypeSymbol.DelegateInvokeMethod is not null) {
			DelegateInfo.TryCreate (namedTypeSymbol.DelegateInvokeMethod, out delegateInfo);
		}

		// retrieve the parameter attributes because those might affect the parameter type, for example, the 
		// NullAllowed attribute can change the parameter type to be nullable.
		var parameterAttrs = symbol.GetAttributeData ();
		parameter = new (symbol.Ordinal, new (symbol.Type, parameterAttrs), symbol.Name) {
			IsOptional = symbol.IsOptional,
			IsParams = symbol.IsParams,
			IsThis = symbol.IsThis,
			DefaultValue = (symbol.HasExplicitDefaultValue) ? symbol.ExplicitDefaultValue?.ToString () : null,
			ReferenceKind = symbol.RefKind.ToReferenceKind (),
			Delegate = delegateInfo,
			Attributes = declaration.GetAttributeCodeChanges (semanticModel),
			AttributesDictionary = parameterAttrs,
		};
		return true;
	}
}
