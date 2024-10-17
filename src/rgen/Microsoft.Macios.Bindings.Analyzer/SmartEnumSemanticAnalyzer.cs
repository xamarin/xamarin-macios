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
public class SmartEnumSemanticAnalyzer : DiagnosticAnalyzer, IBindingTypeAnalyzer<EnumDeclarationSyntax> {
	// All enum values must have a Field attribute
	internal static readonly DiagnosticDescriptor RBI0002 = new (
		"RBI0002",
		new LocalizableResourceString (nameof (Resources.RBI0002Title), Resources.ResourceManager, typeof (Resources)),
		new LocalizableResourceString (nameof (Resources.RBI0002MessageFormat), Resources.ResourceManager, typeof (Resources)),
		"Usage",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: new LocalizableResourceString (nameof (Resources.RBI0002Description), Resources.ResourceManager, typeof (Resources))
	);

	// All Field symbols cannot be empty or white space
	internal static readonly DiagnosticDescriptor RBI0003 = new (
		"RBI0003",
		new LocalizableResourceString (nameof (Resources.RBI0003Title), Resources.ResourceManager, typeof (Resources)),
		new LocalizableResourceString (nameof (Resources.RBI0003MessageFormat), Resources.ResourceManager, typeof (Resources)),
		"Usage",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: new LocalizableResourceString (nameof (Resources.RBI0003Description), Resources.ResourceManager, typeof (Resources))
	);

	// Do not allow duplicated backing fields
	internal static readonly DiagnosticDescriptor RBI0004 = new (
		"RBI0004",
		new LocalizableResourceString (nameof (Resources.RBI0004Title), Resources.ResourceManager, typeof (Resources)),
		new LocalizableResourceString (nameof (Resources.RBI0004MessageFormat), Resources.ResourceManager, typeof (Resources)),
		"Usage",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: new LocalizableResourceString (nameof (Resources.RBI0004Description), Resources.ResourceManager, typeof (Resources))
	);

	// If not an apple framework, we should provide the library path
	internal static readonly DiagnosticDescriptor RBI0005 = new (
		"RBI0005",
		new LocalizableResourceString (nameof (Resources.RBI0005Title), Resources.ResourceManager, typeof (Resources)),
		new LocalizableResourceString (nameof (Resources.RBI0005MessageFormat), Resources.ResourceManager, typeof (Resources)),
		"Usage",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: new LocalizableResourceString (nameof (Resources.RBI0005Description), Resources.ResourceManager, typeof (Resources))
	);

	// if apple framework, the library path should be empty
	internal static readonly DiagnosticDescriptor RBI0006 = new (
		"RBI0006",
		new LocalizableResourceString (nameof (Resources.RBI0006Title), Resources.ResourceManager, typeof (Resources)),
		new LocalizableResourceString (nameof (Resources.RBI0006MessageFormat), Resources.ResourceManager, typeof (Resources)),
		"Usage",
		DiagnosticSeverity.Warning,
		isEnabledByDefault: true,
		description: new LocalizableResourceString (nameof (Resources.RBI0006Description), Resources.ResourceManager, typeof (Resources))
	);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [RBI0002, RBI0003, RBI0004, RBI0005, RBI0006];

	public override void Initialize (AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis (GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution ();
		context.RegisterSyntaxNodeAction (AnalysisContext, SyntaxKind.EnumDeclaration);
	}

	void AnalysisContext (SyntaxNodeAnalysisContext context)
		=> this.AnalyzeBindingType (context);

	public ImmutableArray<Diagnostic> Analyze (PlatformName platformName, EnumDeclarationSyntax declarationNode, INamedTypeSymbol symbol)
	{
		// we want to ensure several things:
		// 1. All enum values are marked with a Field attribute
		// 2. All Field attributes have a symbol name
		// 3. All symbol names have to be unique
		// 4. If the Field attribute is not from a known apple library, the library name is set
		// 5. If the Field attribute is from a known apple library the lib should be null

		// based on the platform decide if we are dealing with a known apple framework, we want all, not just the
		// ones that are part of the simulator
		var appleFrameworks = Frameworks.GetFrameworks (platformName.ToApplePlatform (), false);
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
				// 1. All enum values are marked with a Field attribute, therefore add a diagnostic
				bucket.Add (Diagnostic.Create (RBI0002,
					fieldSymbol.Locations.First (),
					fieldSymbol.ToDisplayString ()));
				continue;
			}

			// Get all the FieldAttribute, parse it and add the data to the result
			if (attributes.TryGetValue (AttributesNames.EnumFieldAttribute, out var fieldAttrData)) {
				var fieldSyntax = fieldAttrData.ApplicationSyntaxReference?.GetSyntax ();
				if (fieldSyntax is null) {
					// if we cant get the syntax reference, we have a bug
					continue;
				}

				if (FieldData<EnumValue>.TryParse (fieldSyntax, fieldAttrData, out var fieldData)) {
					// only provide diagnostics if we managed to parse the FieldData, else we have a bug in the
					// analyzer
					if (string.IsNullOrWhiteSpace (fieldData.SymbolName)) {
						// 2. All Field attributes have a symbol name, therefore add a diagnostic
						bucket.Add (Diagnostic.Create (RBI0003,
							fieldSyntax.GetLocation (),
							fieldSymbol.ToDisplayString ()));
					} else if (!backingFields.Add (fieldData.SymbolName)) {
						// 3. All symbol names have to be unique
						bucket.Add (Diagnostic.Create (RBI0004,
							fieldSyntax.GetLocation (),
							fieldSymbol.ToDisplayString (), fieldData.SymbolName));
					}

					if (!isAppleFramework) {
						// 4. If the Field attribute is not from a known apple library, the library name is set
						if (string.IsNullOrWhiteSpace (fieldData.LibraryName)) {
							bucket.Add (Diagnostic.Create (RBI0005,
								fieldSyntax.GetLocation (),
								fieldSymbol.ToDisplayString ()));
						}
					} else {
						// 5. If the Field attribute is from a known apple library the lib should be null
						if (fieldData.LibraryName is not null) {
							bucket.Add (Diagnostic.Create (RBI0006,
								fieldSyntax.GetLocation (),
								fieldSymbol.ToDisplayString ()));
						}
					}
				} else {
					// report but msg
				}
			}
		}

		return bucket.ToImmutable ();
	}
}
