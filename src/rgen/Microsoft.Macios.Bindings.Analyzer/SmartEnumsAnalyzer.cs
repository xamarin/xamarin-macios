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
	internal static readonly DiagnosticDescriptor RBI0002 = new (
		"RBI0002",
		new LocalizableResourceString (nameof (Resources.RBI0002Title), Resources.ResourceManager, typeof (Resources)),
		new LocalizableResourceString (nameof (Resources.RBI0002MessageFormat), Resources.ResourceManager,
			typeof (Resources)),
		"Usage",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: new LocalizableResourceString (nameof (Resources.RBI0002Description), Resources.ResourceManager,
			typeof (Resources))
	);

	/// <summary>
	/// Do not allow duplicated backing fields
	/// </summary>
	internal static readonly DiagnosticDescriptor RBI0003 = new (
		"RBI0003",
		new LocalizableResourceString (nameof (Resources.RBI0003Title), Resources.ResourceManager, typeof (Resources)),
		new LocalizableResourceString (nameof (Resources.RBI0003MessageFormat), Resources.ResourceManager,
			typeof (Resources)),
		"Usage",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: new LocalizableResourceString (nameof (Resources.RBI0003Description), Resources.ResourceManager,
			typeof (Resources))
	);

	/// <summary>
	/// Fields must be a valid identifier
	/// </summary>
	internal static readonly DiagnosticDescriptor RBI0004 = new (
		"RBI0004",
		new LocalizableResourceString (nameof (Resources.RBI0004Title), Resources.ResourceManager, typeof (Resources)),
		new LocalizableResourceString (nameof (Resources.RBI0004MessageFormat), Resources.ResourceManager,
			typeof (Resources)),
		"Usage",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: new LocalizableResourceString (nameof (Resources.RBI0004Description), Resources.ResourceManager,
			typeof (Resources))
	);

	/// <summary>
	/// If not an apple framework, we should provide the library path
	/// </summary>
	internal static readonly DiagnosticDescriptor RBI0005 = new (
		"RBI0005",
		new LocalizableResourceString (nameof (Resources.RBI0005Title), Resources.ResourceManager, typeof (Resources)),
		new LocalizableResourceString (nameof (Resources.RBI0005MessageFormat), Resources.ResourceManager,
			typeof (Resources)),
		"Usage",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: new LocalizableResourceString (nameof (Resources.RBI0005Description), Resources.ResourceManager,
			typeof (Resources))
	);

	/// <summary>
	/// if apple framework, the library path should be empty
	/// </summary>
	internal static readonly DiagnosticDescriptor RBI0006 = new (
		"RBI0006",
		new LocalizableResourceString (nameof (Resources.RBI0006Title), Resources.ResourceManager, typeof (Resources)),
		new LocalizableResourceString (nameof (Resources.RBI0006MessageFormat), Resources.ResourceManager,
			typeof (Resources)),
		"Usage",
		DiagnosticSeverity.Warning,
		isEnabledByDefault: true,
		description: new LocalizableResourceString (nameof (Resources.RBI0006Description), Resources.ResourceManager,
			typeof (Resources))
	);

	/// <summary>
	/// User used the wrong flag for the attribute
	/// </summary>
	internal static readonly DiagnosticDescriptor RBI0007 = new (
		"RBI0007",
		new LocalizableResourceString (nameof (Resources.RBI0007Title), Resources.ResourceManager, typeof (Resources)),
		new LocalizableResourceString (nameof (Resources.RBI0007MessageFormat), Resources.ResourceManager,
			typeof (Resources)),
		"Usage",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: new LocalizableResourceString (nameof (Resources.RBI0007Description), Resources.ResourceManager,
			typeof (Resources))
	);


	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		[RBI0002, RBI0003, RBI0004, RBI0005, RBI0006, RBI0007];

	public override void Initialize (AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis (GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution ();
		context.RegisterSyntaxNodeAction (AnalysisContext, SyntaxKind.EnumDeclaration);
	}

	void AnalysisContext (SyntaxNodeAnalysisContext context)
		=> this.AnalyzeBindingType (context);

	public ImmutableArray<Diagnostic> Analyze (PlatformName platformName, EnumDeclarationSyntax declarationNode,
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
		var backingFields = new HashSet<string> ();
		foreach (var fieldSymbol in members) {
			var attributes = fieldSymbol.GetAttributeData ();
			if (attributes.Count == 0) {
				//  All enum values are marked with a Field attribute, therefore add a diagnostic
				bucket.Add (Diagnostic.Create (
					RBI0002, // Smart enum values must be tagged with an Field<EnumValue> attribute.
					fieldSymbol.Locations.First (),
					fieldSymbol.ToDisplayString ()));
				continue;
			}

			// Get all the FieldAttribute, parse it and add the data to the result
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
					if (!backingFields.Add (fieldData.Value.SymbolName)) {
						// All symbol names have to be unique
						bucket.Add (Diagnostic.Create (
							RBI0003, // Smart enum backing field cannot appear more than once.
							fieldSyntax.GetLocation (),
							fieldSymbol.ToDisplayString (), fieldData.Value.SymbolName));
					}

					if (!isAppleFramework) {
						// If the Field attribute is not from a known apple library, the library name is set
						if (string.IsNullOrWhiteSpace (fieldData.Value.LibraryName)) {
							bucket.Add (Diagnostic.Create (
								RBI0005, // Non Apple framework bindings must provide a library name.
								fieldSyntax.GetLocation (),
								fieldSymbol.ToDisplayString ()));
						}
					} else {
						// If the Field attribute is from a known apple library, the lib should be null
						if (fieldData.Value.LibraryName is not null) {
							bucket.Add (Diagnostic.Create (
								RBI0006, // Do not provide the LibraryName for known Apple frameworks.
								fieldSyntax.GetLocation (),
								fieldSymbol.ToDisplayString ()));
						}
					}
				} else if (errorTuple.IsError) {
					// based on the parsing error, report a diagnostic
					switch (errorTuple.Error) {
					case FieldData<EnumValue>.ParsingError.NotIdentifier:
						// Backing field is not a valid identifier.
						bucket.Add (Diagnostic.Create (RBI0004,
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
						RBI0007, // Enum values must be tagged with Field<EnumValue>.
						fieldSymbol.Locations.First (),
						fieldAttr, fieldSymbol.ToDisplayString ()));
				}
			}
		}

		return bucket.ToImmutable ();
	}
}
