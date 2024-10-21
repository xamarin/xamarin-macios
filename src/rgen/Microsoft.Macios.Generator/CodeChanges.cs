using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Extensions;

namespace Microsoft.Macios.Generator;

/// <summary>
/// Structure that represents a set of changes that were made by the user that need to be appliend to the
/// generated code.
/// </summary>
struct CodeChanges {
	public string FullyQualifiedSymbol { get; }
	public BaseTypeDeclarationSyntax SymbolDeclaration { get; }
	public ImmutableArray<string> Members { get; }

	/// <summary>
	/// Internal constructor added for testing purposes.
	/// </summary>
	/// <param name="fullyQualifiedSymbol">The fully qualified name of the symbol.</param>
	/// <param name="declaration">The symbol declaration syntax.</param>
	/// <param name="members">The members that had changed.</param>
	internal CodeChanges (string fullyQualifiedSymbol, BaseTypeDeclarationSyntax declaration, ImmutableArray<string> members)
	{
		FullyQualifiedSymbol = fullyQualifiedSymbol;
		SymbolDeclaration = declaration;
		Members = members;
	}

	CodeChanges (GeneratorSyntaxContext context, EnumDeclarationSyntax enumDeclaration)
	{
		FullyQualifiedSymbol = enumDeclaration.GetFullyQualifiedIdentifier ();
		SymbolDeclaration = enumDeclaration;
		var bucket = ImmutableArray.CreateBuilder<string> ();
		// loop over the fields and add those that contain a FieldAttribute
		var enumValueDeclaration = enumDeclaration.Members.OfType<EnumMemberDeclarationSyntax> ();
		foreach (var val in enumValueDeclaration) {
			if (val.HasAttribute (context, AttributesNames.FieldAttribute)) {
				bucket.Add (val.Identifier.ToFullString ());
			}
		}
		Members = bucket.ToImmutable ();
	}

	CodeChanges (GeneratorSyntaxContext context, ClassDeclarationSyntax classDeclaration)
	{
		FullyQualifiedSymbol = classDeclaration.GetFullyQualifiedIdentifier ();
		SymbolDeclaration = classDeclaration;
		// TODO: to be implemented once we add class support
		Members = [];
	}

	CodeChanges (GeneratorSyntaxContext context, InterfaceDeclarationSyntax interfaceDeclaration)
	{
		FullyQualifiedSymbol = interfaceDeclaration.GetFullyQualifiedIdentifier ();
		SymbolDeclaration = interfaceDeclaration;
		// TODO: to be implemented once we add protocol support
		Members = [];
	}

	public static CodeChanges? FromDeclaration (GeneratorSyntaxContext context,
		BaseTypeDeclarationSyntax baseTypeDeclarationSyntax)
		=> baseTypeDeclarationSyntax switch {
			EnumDeclarationSyntax enumDeclarationSyntax => new CodeChanges (context, enumDeclarationSyntax),
			InterfaceDeclarationSyntax interfaceDeclarationSyntax => new CodeChanges (context, interfaceDeclarationSyntax),
			ClassDeclarationSyntax classDeclarationSyntax => new CodeChanges (context, classDeclarationSyntax),
			_ => null
		};
}
