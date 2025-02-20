// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.Extensions;

namespace Microsoft.Macios.Generator.DataModel;

readonly partial struct Constructor {

	/// <summary>
	/// The data of the export attribute used to mark the value as a property binding. 
	/// </summary>
	public ExportData<ObjCBindings.Constructor> ExportMethodData { get; }

	public static bool TryCreate (ConstructorDeclarationSyntax declaration, RootContext context,
		[NotNullWhen (true)] out Constructor? change)
	{
		if (context.SemanticModel.GetDeclaredSymbol (declaration) is not IMethodSymbol constructor) {
			change = null;
			return false;
		}

		var attributes = declaration.GetAttributeCodeChanges (context.SemanticModel);
		var parametersBucket = ImmutableArray.CreateBuilder<Parameter> ();
		// loop over the parameters of the construct since changes on those implies a change in the generated code
		foreach (var parameter in constructor.Parameters) {
			var parameterDeclaration = declaration.ParameterList.Parameters [parameter.Ordinal];
			if (!Parameter.TryCreate (parameter, parameterDeclaration, context, out var parameterChange))
				continue;
			parametersBucket.Add (parameterChange.Value);
		}

		change = new (
			type: constructor.ContainingSymbol.Name, // we DO NOT want the full name
			symbolAvailability: constructor.GetSupportedPlatforms (),
			attributes: attributes,
			modifiers: [.. declaration.Modifiers],
			parameters: parametersBucket.ToImmutable ());
		return true;
	}
}
