using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Extensions;

namespace Microsoft.Macios.Generator.DataModel;

/// <summary>
/// Structure that represents a set of changes that were made by the user that need to be applied to the
/// generated code.
/// </summary>
readonly struct CodeChanges {
	/// <summary>
	/// Represents the type of binding that the code changes are for.
	/// </summary>
	public BindingType BindingType { get; } = BindingType.Unknown;

	/// <summary>
	/// Fully qualified name of the symbol that the code changes are for.
	/// </summary>
	public string FullyQualifiedSymbol { get; }

	/// <summary>
	/// Base symbol declaration that triggered the code changes.
	/// </summary>
	public BaseTypeDeclarationSyntax SymbolDeclaration { get; }

	/// <summary>
	/// Changes to the attributes of the symbol.
	/// </summary>
	public ImmutableArray<AttributeCodeChange> Attributes { get; }

	/// <summary>
	/// Changes to the members of the symbol.
	/// </summary>
	public ImmutableArray<MemberCodeChange> Members { get; }

	/// <summary>
	/// Internal constructor added for testing purposes.
	/// </summary>
	/// <param name="bindingType">The type of binding for the given code changes.</param>
	/// <param name="fullyQualifiedSymbol">The fully qualified name of the symbol.</param>
	/// <param name="declaration">The symbol declaration syntax.</param>
	/// <param name="attributes">The list of attributes changed.</param>
	/// <param name="members">The list of members changed.</param>
	internal CodeChanges (BindingType bindingType, string fullyQualifiedSymbol, BaseTypeDeclarationSyntax declaration,
		ImmutableArray<AttributeCodeChange> attributes,
		ImmutableArray<MemberCodeChange> members)
	{
		BindingType = bindingType;
		FullyQualifiedSymbol = fullyQualifiedSymbol;
		SymbolDeclaration = declaration;
		Attributes = attributes;
		Members = members;
	}

	/// <summary>
	/// Creates a new instance of the <see cref="CodeChanges"/> struct for a given enum declaration.
	/// </summary>
	/// <param name="semanticModel">The semantic model of the compilation.</param>
	/// <param name="enumDeclaration">The enum declaration that triggered the change.</param>
	CodeChanges (SemanticModel semanticModel, EnumDeclarationSyntax enumDeclaration)
	{
		BindingType = BindingType.SmartEnum;
		FullyQualifiedSymbol = enumDeclaration.GetFullyQualifiedIdentifier ();
		SymbolDeclaration = enumDeclaration;
		Attributes = enumDeclaration.GetAttributeCodeChanges (semanticModel);
		var bucket = ImmutableArray.CreateBuilder<MemberCodeChange> ();
		// get all the attributes of the enum, changes in them might trigger a re-generation
		var enumAttributes = enumDeclaration.GetAttributeCodeChanges (semanticModel);
		// loop over the fields and add those that contain a FieldAttribute
		var enumValueDeclaration = enumDeclaration.Members.OfType<EnumMemberDeclarationSyntax> ();
		foreach (var val in enumValueDeclaration) {
			if (!val.HasAttribute (semanticModel, AttributesNames.EnumFieldAttribute))
				// for smart enums, we are only interested in the field that have a Field<EnumValue> attribute
				continue;
			var memberName = val.Identifier.ToFullString ().Trim ();
			var attributes = val.GetAttributeCodeChanges (semanticModel);
			bucket.Add (new (memberName, attributes));
		}
		Members = bucket.ToImmutable ();
	}

	/// <summary>
	/// Creates a new instance of the <see cref="CodeChanges"/> struct for a given class declaration.
	/// </summary>
	/// <param name="semanticModel">The semantic model of the compilation.</param>
	/// <param name="classDeclaration">The class declaration that triggered the change.</param>
	CodeChanges (SemanticModel semanticModel, ClassDeclarationSyntax classDeclaration)
	{
		FullyQualifiedSymbol = classDeclaration.GetFullyQualifiedIdentifier ();
		SymbolDeclaration = classDeclaration;
		// TODO: to be implemented once we add class support
		Members = [];
		Attributes = [];
	}

	/// <summary>
	/// Creates a new instance of the <see cref="CodeChanges"/> struct for a given interface declaration.
	/// </summary>
	/// <param name="semanticModel">The semantic model of the compilation.</param>
	/// <param name="interfaceDeclaration">The interface declaration that triggered the change.</param>
	CodeChanges (SemanticModel semanticModel, InterfaceDeclarationSyntax interfaceDeclaration)
	{
		FullyQualifiedSymbol = interfaceDeclaration.GetFullyQualifiedIdentifier ();
		SymbolDeclaration = interfaceDeclaration;
		// TODO: to be implemented once we add protocol support
		Members = [];
		Attributes = [];
	}

	/// <summary>
	/// Create a CodeChange from the provide base type declaration syntax. If the syntax is not supported,
	/// it will return null.
	/// </summary>
	/// <param name="semanticModel">The semantic model related to the syntax tree that contains the node.</param>
	/// <param name="baseTypeDeclarationSyntax">The declaration syntax whose change we want to calculate.</param>
	/// <returns>A code change or null if it could not be calculated.</returns>
	public static CodeChanges? FromDeclaration (SemanticModel semanticModel,
		BaseTypeDeclarationSyntax baseTypeDeclarationSyntax)
		=> baseTypeDeclarationSyntax switch {
			EnumDeclarationSyntax enumDeclarationSyntax => new CodeChanges (semanticModel, enumDeclarationSyntax),
			InterfaceDeclarationSyntax interfaceDeclarationSyntax => new CodeChanges (semanticModel,
				interfaceDeclarationSyntax),
			ClassDeclarationSyntax classDeclarationSyntax => new CodeChanges (semanticModel, classDeclarationSyntax),
			_ => null
		};
}
