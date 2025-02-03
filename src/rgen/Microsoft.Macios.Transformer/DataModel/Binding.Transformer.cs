// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.Extensions;
using Microsoft.Macios.Transformer.DataModel;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Microsoft.Macios.Generator.DataModel;

readonly partial struct Binding {

	/// <summary>
	/// Represents the type of binding that the code changes are for.
	/// </summary>
	public BindingType BindingType => BindingInfo.BindingType;

	/// <summary>
	/// Returhs the binding data of the binding for the given code changes.
	/// </summary>
	public BindingInfo BindingInfo { get; init; }

	/// <summary>
	/// Internal constructor added for testing purposes.
	/// </summary>
	/// <param name="symbolName">The name of the named type that created the code change.</param>
	/// <param name="namespace">The namespace that contains the named type.</param>
	/// <param name="fullyQualifiedSymbol">The fully qualified name of the symbol.</param>
	/// <param name="info">the binding info struct.</param>
	/// <param name="symbolAvailability">The platform availability of the named symbol.</param>
	/// <param name="attributes">The dictioanary with the attributes the user used with the binding.</param>
	internal Binding (string symbolName, 
		ImmutableArray<string> @namespace,
		string fullyQualifiedSymbol, 
		BindingInfo info,
		SymbolAvailability symbolAvailability,
		Dictionary<string, List<AttributeData>> attributes)
	{
		name = symbolName;
		namespaces = @namespace;
		availability = symbolAvailability;
		AttributesDictionary = attributes;
		BindingInfo = info;
		FullyQualifiedSymbol = fullyQualifiedSymbol;
	}

	internal Binding (EnumDeclarationSyntax enumDeclaration, ISymbol symbol, RootContext context)
	{
		SemanticModelExtensions.GetSymbolData (
			symbol: symbol,
			name: out name,
			baseClass: out baseClass,
			interfaces: out interfaces,
			namespaces: out namespaces,
			symbolAvailability: out availability);
		BindingInfo = new BindingInfo (null, BindingType.SmartEnum);
		FullyQualifiedSymbol = enumDeclaration.GetFullyQualifiedIdentifier ();
		UsingDirectives = enumDeclaration.SyntaxTree.CollectUsingStatements ();
		// smart enums are expected to be public, we might need to change this in the future
		Modifiers = [Token (SyntaxKind.PublicKeyword)];
		var bucket = ImmutableArray.CreateBuilder<EnumMember> ();
		var enumValueDeclarations = enumDeclaration.Members.OfType<EnumMemberDeclarationSyntax> ();
		foreach (var enumValueDeclaration in enumValueDeclarations) {
			if (context.SemanticModel.GetDeclaredSymbol (enumValueDeclaration) is not IFieldSymbol enumValueSymbol) {
				continue;
			}
			var fieldData = enumValueSymbol.GetFieldData ();
			// try and compute the library for this enum member
			if (fieldData is null || !context.TryComputeLibraryName (fieldData.Value.LibraryName, Namespace [^1],
					out string? libraryName, out string? libraryPath))
				// could not calculate the library for the enum, do not add it
				continue;
			var enumMember = new EnumMember (
				name: enumValueDeclaration.Identifier.ToFullString ().Trim (),
				libraryName: libraryName,
				libraryPath: libraryPath,
				fieldData: enumValueSymbol.GetFieldData (),
				symbolAvailability: enumValueSymbol.GetAvailabilityForSymbol () // no parent availability, just the symbol
			);
			bucket.Add (enumMember);
		}

		EnumMembers = bucket.ToImmutable ();
	}
}
