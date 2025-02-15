// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Transformer.Attributes;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Microsoft.Macios.Generator.DataModel;

readonly partial struct Constructor {

	readonly ExportData? overrideExportData;

	/// <summary>
	/// The data of the export attribute used to mark the value as a property binding. 
	/// </summary>
	public ExportData? ExportMethodData {
		get => overrideExportData ?? ExportAttribute;
		init => overrideExportData = value;
	}

	public Constructor (string type,
		SymbolAvailability symbolAvailability,
		Dictionary<string, List<AttributeData>> attributes,
		ImmutableArray<Parameter> parameters)
	{
		Type = type;
		SymbolAvailability = symbolAvailability;
		Attributes = [];
		AttributesDictionary = attributes;
		Parameters = parameters;
		ExportMethodData = null;
	}

	/// <summary>
	/// Create a constructor from a method signature.
	/// </summary>
	/// <param name="method">The method to be used to create the constructor.</param>
	public Constructor (in Method method)
	{
		Type = method.Type;
		SymbolAvailability = method.SymbolAvailability;
		Attributes = method.Attributes;
		Parameters = method.Parameters;
		AttributesDictionary = method.AttributesDictionary;
		ExportMethodData = method.ExportMethodData;

		// modifiers cannot only be copied, we do not have a virtual/override constructor, it is either
		// public, private or internal. 
		var modifierBuilder = ImmutableArray.CreateBuilder<SyntaxToken> ();
		SyntaxKind modifier = SyntaxKind.PublicKeyword;
		foreach (var methodModifier in method.Modifiers) {
			var modifierKind = methodModifier.Kind ();
			if (modifierKind is
				SyntaxKind.PublicKeyword or SyntaxKind.ProtectedKeyword or SyntaxKind.InternalKeyword or SyntaxKind.PrivateKeyword) {
				modifier = modifierKind;
			}
		}

		modifierBuilder.Add (Token (modifier));
		// We will be adding partial because is part of the new API
		modifierBuilder.Add (Token (SyntaxKind.PartialKeyword));
		Modifiers = modifierBuilder.ToImmutable ();

	}
}
