// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Macios.Bindings.Analyzer.Extensions;
using Microsoft.Macios.Generator;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Extensions;
using ObjCBindings;

namespace Microsoft.Macios.Bindings.Analyzer;

/// <summary>
/// Analyzer to ensure that all enum values in an SmartEnum contains a Field attribute.
/// </summary>
[DiagnosticAnalyzer (LanguageNames.CSharp)]
public class SmartEnumsAnalyzer : DiagnosticAnalyzer, IBindingTypeAnalyzer<EnumDeclarationSyntax> {
	/// <summary>
	/// All enum values must have a Field attribute
	/// </summary>
	internal static readonly DiagnosticDescriptor RBI0008 = new (
		"RBI0008",
		new LocalizableResourceString (nameof (Resources.RBI0008Title), Resources.ResourceManager, typeof (Resources)),
		new LocalizableResourceString (nameof (Resources.RBI0008MessageFormat), Resources.ResourceManager,
			typeof (Resources)),
		"Usage",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: new LocalizableResourceString (nameof (Resources.RBI0008Description), Resources.ResourceManager,
			typeof (Resources))
	);

	/// <summary>
	/// Do not allow duplicated backing fields
	/// </summary>
	internal static readonly DiagnosticDescriptor RBI0009 = new (
		"RBI0009",
		new LocalizableResourceString (nameof (Resources.RBI0009Title), Resources.ResourceManager, typeof (Resources)),
		new LocalizableResourceString (nameof (Resources.RBI0009MessageFormat), Resources.ResourceManager,
			typeof (Resources)),
		"Usage",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: new LocalizableResourceString (nameof (Resources.RBI0009Description), Resources.ResourceManager,
			typeof (Resources))
	);

	/// <summary>
	/// Fields must be a valid identifier
	/// </summary>
	internal static readonly DiagnosticDescriptor RBI0010 = new (
		"RBI0010",
		new LocalizableResourceString (nameof (Resources.RBI0010Title), Resources.ResourceManager, typeof (Resources)),
		new LocalizableResourceString (nameof (Resources.RBI0010MessageFormat), Resources.ResourceManager,
			typeof (Resources)),
		"Usage",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: new LocalizableResourceString (nameof (Resources.RBI0010Description), Resources.ResourceManager,
			typeof (Resources))
	);

	/// <summary>
	/// If not an apple framework, we should provide the library path
	/// </summary>
	internal static readonly DiagnosticDescriptor RBI0011 = new (
		"RBI0011",
		new LocalizableResourceString (nameof (Resources.RBI0011Title), Resources.ResourceManager, typeof (Resources)),
		new LocalizableResourceString (nameof (Resources.RBI0011MessageFormat), Resources.ResourceManager,
			typeof (Resources)),
		"Usage",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: new LocalizableResourceString (nameof (Resources.RBI0011Description), Resources.ResourceManager,
			typeof (Resources))
	);

	/// <summary>
	/// if apple framework, the library path should be empty
	/// </summary>
	internal static readonly DiagnosticDescriptor RBI0012 = new (
		"RBI0012",
		new LocalizableResourceString (nameof (Resources.RBI0012Title), Resources.ResourceManager, typeof (Resources)),
		new LocalizableResourceString (nameof (Resources.RBI0012MessageFormat), Resources.ResourceManager,
			typeof (Resources)),
		"Usage",
		DiagnosticSeverity.Warning,
		isEnabledByDefault: true,
		description: new LocalizableResourceString (nameof (Resources.RBI0012Description), Resources.ResourceManager,
			typeof (Resources))
	);

	/// <summary>
	/// User used the wrong flag for the attribute
	/// </summary>
	internal static readonly DiagnosticDescriptor RBI0013 = new (
		"RBI0013",
		new LocalizableResourceString (nameof (Resources.RBI0013Title), Resources.ResourceManager, typeof (Resources)),
		new LocalizableResourceString (nameof (Resources.RBI0013MessageFormat), Resources.ResourceManager,
			typeof (Resources)),
		"Usage",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: new LocalizableResourceString (nameof (Resources.RBI0013Description), Resources.ResourceManager,
			typeof (Resources))
	);


	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [
		RBI0008,
		RBI0009,
		RBI0010,
		RBI0011,
		RBI0012,
		RBI0013,
	];

	public override void Initialize (AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis (GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution ();
		context.RegisterSyntaxNodeAction (AnalysisContext, SyntaxKind.EnumDeclaration);
	}

	void AnalysisContext (SyntaxNodeAnalysisContext context)
		=> this.AnalyzeBindingType (context);

	static readonly HashSet<string> attributes = [AttributesNames.BindingAttribute];
	public IReadOnlySet<string> AttributeNames => attributes;

	public ImmutableArray<Diagnostic> Analyze (string matchedAttribute, PlatformName platformName, EnumDeclarationSyntax declarationNode,
		INamedTypeSymbol symbol)
	{
		// we want to ensure several things:
		// 1. All enum values are marked with a Field attribute
		// 2. All Field attributes have a symbol name
		// 3. All symbol names have to be unique
		// 4. If the Field attribute is not from a known apple library, the library name is set
		// 5. If the Field attribute is from a known apple library the lib should be null
		// 6. The user used the correct attribute flag.

		// based on the platform decide if we are dealing with a known apple framework, we want all, not just the
		// ones that are part of the simulator
		if (platformName == PlatformName.None) {
			// we could not identify the platform, we have a bug or we are not on an apple target, do nothing
			return [];
		}

		var appleFrameworks = Frameworks.GetFrameworks (platformName.ToApplePlatform (), false);
		if (appleFrameworks is null) {
			// we could not get the frameworks, we have a bug
			return [];
		}

		var isAppleFramework = appleFrameworks.Find (symbol.ContainingNamespace.Name) is not null;

		// bucket with all the diagnostics we have found
		var bucket = ImmutableArray.CreateBuilder<Diagnostic> ();

		var members = symbol.GetMembers ().OfType<IFieldSymbol> ().ToArray ();
		// we do not allow backing fields to be duplicated, keep track of those added so far to raise
		// a diagnostic if we find a duplicate
		var backingFields = new Dictionary<string, string> ();
		foreach (var fieldSymbol in members) {
			var attributes = fieldSymbol.GetAttributeData ();
			if (attributes.Count == 0) {
				//  All enum values are marked with a Field attribute, therefore add a diagnostic
				bucket.Add (Diagnostic.Create (
					RBI0008, // Smart enum values must be tagged with an Field<EnumValue> attribute.
					fieldSymbol.Locations.First (),
					fieldSymbol.ToDisplayString ()));
				continue;
			}

			// Get the FieldAttribute, parse it and add the data to the result
			if (attributes.TryGetValue (AttributesNames.EnumFieldAttribute, out var fieldAttrDataList)
				&& fieldAttrDataList.Count == 1) {
				var fieldAttrData = fieldAttrDataList [0];
				var fieldSyntax = fieldAttrData.ApplicationSyntaxReference?.GetSyntax ();
				if (fieldSyntax is null) {
					continue;
				}

				if (FieldData<EnumValue>.TryParse (fieldAttrData, out var fieldData, out var errorTuple)) {
					// only provide diagnostics if we managed to parse the FieldData, else we have a bug in the
					// analyzer
					if (backingFields.TryGetValue (fieldData.Value.SymbolName, out var previousEnumValue)) {
						// All symbol names have to be unique
						bucket.Add (Diagnostic.Create (
							RBI0009, // The backing field '{0}' for the enum value '{1}' is already in use for the enum value '{2}'
							fieldSyntax.GetLocation (),
							fieldSymbol.ToDisplayString (), fieldData.Value.SymbolName,
							fieldSymbol.ToDisplayString ().Trim (), previousEnumValue));
					} else {
						// store the enum value so that we can provide a better error for the user.
						backingFields [fieldData.Value.SymbolName] = fieldSymbol.ToDisplayString ().Trim ();
					}

					if (!isAppleFramework) {
						// If the Field attribute is not from a known apple library, the library name is set
						if (string.IsNullOrWhiteSpace (fieldData.Value.LibraryName)) {
							bucket.Add (Diagnostic.Create (
								RBI0011, // Non Apple framework bindings must provide a library name.
								fieldSyntax.GetLocation (),
								fieldSymbol.ToDisplayString ()));
						}
					} else {
						// If the Field attribute is from a known apple library, the lib should be null
						if (fieldData.Value.LibraryName is not null) {
							bucket.Add (Diagnostic.Create (
								RBI0012, // Do not provide the LibraryName for known Apple frameworks.
								fieldSyntax.GetLocation (),
								fieldSymbol.ToDisplayString ()));
						}
					}
				} else if (errorTuple.IsError) {
					// based on the parsing error, report a diagnostic
					switch (errorTuple.Error) {
					case FieldData<EnumValue>.ParsingError.NotIdentifier:
						// Backing field is not a valid identifier.
						bucket.Add (Diagnostic.Create (RBI0010,
							fieldSyntax
								.GetLocation (), // Smart enum backing field must represent a valid C# identifier to be used.
							fieldSymbol.ToDisplayString (), errorTuple.Value));
						break;
					}
				}
			} else {
				// validate that the user used the correct flag, that means that we should not find a field attr
				var fieldAttr = attributes.Keys
					.FirstOrDefault (s => s.StartsWith (AttributesNames.FieldAttribute));
				if (fieldAttr is not null) {
					bucket.Add (Diagnostic.Create (
						RBI0013, // Enum values must be tagged with Field<EnumValue>.
						fieldSymbol.Locations.First (),
						fieldAttr, fieldSymbol.ToDisplayString ()));
				}
			}
		}

		return bucket.ToImmutable ();
	}
}
