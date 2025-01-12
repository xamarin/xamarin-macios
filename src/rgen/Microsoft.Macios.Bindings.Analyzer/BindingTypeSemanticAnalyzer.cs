// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Macios.Bindings.Analyzer.Extensions;
using Microsoft.Macios.Generator;

namespace Microsoft.Macios.Bindings.Analyzer;

/// <summary>
/// Analyzer that ensures that the types that have been declared as binding types are partial and follow the correct
/// pattern.
/// </summary>
[DiagnosticAnalyzer (LanguageNames.CSharp)]
public class BindingTypeSemanticAnalyzer : DiagnosticAnalyzer, IBindingTypeAnalyzer<BaseTypeDeclarationSyntax> {
	/// <summary>
	/// All binding types should be partial.
	/// </summary>
	internal static readonly DiagnosticDescriptor RBI0001 = new (
		"RBI0001",
		new LocalizableResourceString (nameof (Resources.RBI0001Title), Resources.ResourceManager, typeof (Resources)),
		new LocalizableResourceString (nameof (Resources.RBI0001MessageFormat), Resources.ResourceManager,
			typeof (Resources)),
		"Usage",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: new LocalizableResourceString (nameof (Resources.RBI0001Description), Resources.ResourceManager,
			typeof (Resources))
	);

	/// <summary>
	/// BindingType&lt;Class&gt; can only decorate partial classes.
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
	/// BindingType&lt;Category&gt; can only decorate partial classes.
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
	/// BindingType&lt;Category&gt; can only decorate static classes.
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
	/// BindingType&lt;Protocol&gt; can only decorate interfaces.
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
	/// BindingType can only decorate enumerators.
	/// </summary>
	internal static readonly DiagnosticDescriptor RBI0006 = new (
		"RBI0006",
		new LocalizableResourceString (nameof (Resources.RBI0006Title), Resources.ResourceManager, typeof (Resources)),
		new LocalizableResourceString (nameof (Resources.RBI0006MessageFormat), Resources.ResourceManager,
			typeof (Resources)),
		"Usage",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: new LocalizableResourceString (nameof (Resources.RBI0006Description), Resources.ResourceManager,
			typeof (Resources))
	);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [
		RBI0001,
		RBI0002,
		RBI0003,
		RBI0004,
		RBI0005,
		RBI0006,
	];

	public override void Initialize (AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis (GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution ();
		context.RegisterSyntaxNodeAction (AnalysisContext,
			SyntaxKind.ClassDeclaration,
			SyntaxKind.InterfaceDeclaration,
			SyntaxKind.EnumDeclaration);
	}

	void AnalysisContext (SyntaxNodeAnalysisContext context)
		=> this.AnalyzeBindingType (context);

	static readonly HashSet<string> attributes = new HashSet<string> (AttributesNames.BindingTypes);
	public IReadOnlySet<string> AttributeNames => attributes;

	ImmutableArray<Diagnostic> ValidateClass (BaseTypeDeclarationSyntax declarationNode, INamedTypeSymbol symbol)
	{
		var bucket = ImmutableArray.CreateBuilder<Diagnostic> ();
		if (declarationNode is ClassDeclarationSyntax classDeclarationSyntax) {
			if (!classDeclarationSyntax.IsPartial ()) {
				var partialDiagnostic = Diagnostic.Create (
					descriptor: RBI0001, // Binding types should be declared as partial classes.
					location: declarationNode.Identifier.GetLocation (),
					messageArgs: symbol.ToDisplayString ());
				bucket.Add (partialDiagnostic);
			}
		} else {
			var notAClassDiagnostic = Diagnostic.Create (
				descriptor: RBI0002, // BindingType<Class> must be on a class
				location: declarationNode.Identifier.GetLocation (),
				messageArgs: symbol.ToDisplayString ());
			bucket.Add (notAClassDiagnostic);
		}

		return bucket.ToImmutable ();
	}

	ImmutableArray<Diagnostic> ValidateCategory (BaseTypeDeclarationSyntax declarationNode, INamedTypeSymbol symbol)
	{
		var bucket = ImmutableArray.CreateBuilder<Diagnostic> ();
		if (declarationNode is ClassDeclarationSyntax classDeclarationSyntax) {
			if (!classDeclarationSyntax.IsPartial ()) {
				var partialDiagnostic = Diagnostic.Create (
					descriptor: RBI0001, // Binding types should be declared as partial classes.
					location: declarationNode.Identifier.GetLocation (),
					messageArgs: symbol.ToDisplayString ());
				bucket.Add (partialDiagnostic);
			}

			if (!classDeclarationSyntax.IsStatic ()) {
				var partialDiagnostic = Diagnostic.Create (
					descriptor: RBI0004, // BindintType<Category> must be on a static class
					location: declarationNode.Identifier.GetLocation (),
					messageArgs: symbol.ToDisplayString ());
				bucket.Add (partialDiagnostic);
			}
		} else {
			var notAClassDiagnostic = Diagnostic.Create (
				descriptor: RBI0003, // BindingType<Category> must be on a class
				location: declarationNode.Identifier.GetLocation (),
				messageArgs: symbol.ToDisplayString ());
			bucket.Add (notAClassDiagnostic);
		}

		return bucket.ToImmutable ();
	}

	ImmutableArray<Diagnostic> ValidateProtocol (BaseTypeDeclarationSyntax declarationNode, INamedTypeSymbol symbol)
	{
		var bucket = ImmutableArray.CreateBuilder<Diagnostic> ();
		if (declarationNode is InterfaceDeclarationSyntax interfaceDeclarationSyntax) {
			if (!interfaceDeclarationSyntax.IsPartial ()) {
				var partialDiagnostic = Diagnostic.Create (
					descriptor: RBI0001, // Binding types should be declared as partial classes.
					location: declarationNode.Identifier.GetLocation (),
					messageArgs: symbol.ToDisplayString ());
				bucket.Add (partialDiagnostic);
			}
		} else {
			var notAInterfaceDiagnostic = Diagnostic.Create (
				descriptor: RBI0005, // BindingType<Protocol> must be on an interface 
				location: declarationNode.Identifier.GetLocation (),
				messageArgs: symbol.ToDisplayString ());
			bucket.Add (notAInterfaceDiagnostic);
		}

		return bucket.ToImmutable ();
	}

	public ImmutableArray<Diagnostic> ValidateSmartEnum (BaseTypeDeclarationSyntax declarationNode,
		INamedTypeSymbol symbol)
	{
		var bucket = ImmutableArray.CreateBuilder<Diagnostic> ();
		if (declarationNode is not EnumDeclarationSyntax) {
			var notAInterfaceDiagnostic = Diagnostic.Create (
				descriptor: RBI0006, // BindingType must be on an enumerator 
				location: declarationNode.Identifier.GetLocation (),
				messageArgs: symbol.ToDisplayString ());
			bucket.Add (notAInterfaceDiagnostic);
		}

		return bucket.ToImmutable ();
	}

	public ImmutableArray<Diagnostic> Analyze (string matchedAttribute, PlatformName _,
		BaseTypeDeclarationSyntax declarationNode, INamedTypeSymbol symbol)
		=> matchedAttribute switch {
			AttributesNames.BindingClassAttribute => ValidateClass (declarationNode, symbol),
			AttributesNames.BindingCategoryAttribute => ValidateCategory (declarationNode, symbol),
			AttributesNames.BindingProtocolAttribute => ValidateProtocol (declarationNode, symbol),
			AttributesNames.BindingAttribute => ValidateSmartEnum (declarationNode, symbol),
			_ => throw new InvalidOperationException ($"Not recognized attribute {matchedAttribute}.")
		};
}
