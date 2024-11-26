using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
	public ImmutableArray<AttributeCodeChange> Attributes { get; init; } = [];

	/// <summary>
	/// Changes to the enum members of the symbol.
	/// </summary>
	public ImmutableArray<EnumMember> EnumMembers { get; init; } = [];

	/// <summary>
	/// Changes to the properties of the symbol.
	/// </summary>
	public ImmutableArray<Property> Properties { get; init; } = [];

	/// <summary>
	/// Changes to the constructors of the symbol.
	/// </summary>
	public ImmutableArray<Constructor> Constructors { get; init; } = [];

	/// <summary>
	/// Changes to the events of the symbol.
	/// </summary>
	public ImmutableArray<Event> Events { get; init; } = [];
	
	/// <summary>
	/// Changes to the methods of a symbol.
	/// </summary>
	public ImmutableArray<Method> Methods { get; init; } = [];

	/// <summary>
	/// Decide if an enum value should be ignored as a change.
	/// </summary>
	/// <param name="enumMemberDeclarationSyntax">The enum declaration under test.</param>
	/// <param name="semanticModel">The semantic model of the compilation.</param>
	/// <returns>True if the enum value should be ignored. False otherwise.</returns>
	internal static bool Skip (EnumMemberDeclarationSyntax enumMemberDeclarationSyntax, SemanticModel semanticModel)
	{
		// for smart enums, we are only interested in the field that has a Field<EnumValue> attribute
		return !enumMemberDeclarationSyntax.HasAttribute (semanticModel, AttributesNames.EnumFieldAttribute);
	}

	/// <summary>
	/// Decide if a property should be ignored as a change.
	/// </summary>
	/// <param name="propertyDeclarationSyntax">The property declaration under test.</param>
	/// <param name="semanticModel">The semantic model of the compilation.</param>
	/// <returns>True if the property should be ignored. False otherwise.</returns>
	internal static bool Skip (PropertyDeclarationSyntax propertyDeclarationSyntax, SemanticModel semanticModel)
	{
		// valid properties are: 
		// 1. Partial
		// 2. One of the following:
		//	  1. Field properties
		//    2. Exported properties
		if (propertyDeclarationSyntax.Modifiers.Any (SyntaxKind.PartialKeyword)) {
			return !propertyDeclarationSyntax.HasAtLeastOneAttribute (semanticModel,
				AttributesNames.ExportFieldAttribute, AttributesNames.ExportPropertyAttribute);
		}

		return true;
	}

	internal static bool Skip (ConstructorDeclarationSyntax constructorDeclarationSyntax, SemanticModel semanticModel)
	{
		// TODO: we need to confirm this when we have support from the roslyn team.
		return false;
	}

	internal static bool Skip (EventDeclarationSyntax eventDeclarationSyntax, SemanticModel semanticModel)
	{
		// TODO: we need to confirm this when we have support from the roslyn team.
		return false;
	}

	internal static bool Skip (MethodDeclarationSyntax methodDeclarationSyntax, SemanticModel semanticModel)
	{
		// Valid methods are:
		// 1. Partial
		// 2. Contain the export attribute
		if (methodDeclarationSyntax.Modifiers.Any (SyntaxKind.PartialKeyword)) { 
			return !methodDeclarationSyntax.HasAttribute (semanticModel, AttributesNames.ExportMethodAttribute);
		}
		return true;
	}

	/// <summary>
	/// Internal constructor added for testing purposes.
	/// </summary>
	/// <param name="bindingType">The type of binding for the given code changes.</param>
	/// <param name="fullyQualifiedSymbol">The fully qualified name of the symbol.</param>
	/// <param name="declaration">The symbol declaration syntax.</param>
	internal CodeChanges (BindingType bindingType, string fullyQualifiedSymbol, BaseTypeDeclarationSyntax declaration)
	{
		BindingType = bindingType;
		FullyQualifiedSymbol = fullyQualifiedSymbol;
		SymbolDeclaration = declaration;
	}

	/// <summary>
	/// Creates a new instance of the <see cref="CodeChanges"/> struct for a given enum declaration.
	/// </summary>
	/// <param name="enumDeclaration">The enum declaration that triggered the change.</param>
	/// <param name="semanticModel">The semantic model of the compilation.</param>
	CodeChanges (EnumDeclarationSyntax enumDeclaration, SemanticModel semanticModel)
	{
		BindingType = BindingType.SmartEnum;
		FullyQualifiedSymbol = enumDeclaration.GetFullyQualifiedIdentifier ();
		SymbolDeclaration = enumDeclaration;
		Attributes = enumDeclaration.GetAttributeCodeChanges (semanticModel);
		var bucket = ImmutableArray.CreateBuilder<EnumMember> ();
		// loop over the fields and add those that contain a FieldAttribute
		var enumValueDeclaration = enumDeclaration.Members.OfType<EnumMemberDeclarationSyntax> ();
		foreach (var val in enumValueDeclaration) {
			if (Skip (val, semanticModel))
				continue;
			var memberName = val.Identifier.ToFullString ().Trim ();
			var attributes = val.GetAttributeCodeChanges (semanticModel);
			bucket.Add (new(memberName, attributes));
		}

		EnumMembers = bucket.ToImmutable ();
	}

	/// <summary>
	/// Creates a new instance of the <see cref="CodeChanges"/> struct for a given class declaration.
	/// </summary>
	/// <param name="classDeclaration">The class declaration that triggered the change.</param>
	/// <param name="semanticModel">The semantic model of the compilation.</param>
	CodeChanges (ClassDeclarationSyntax classDeclaration, SemanticModel semanticModel)
	{
		FullyQualifiedSymbol = classDeclaration.GetFullyQualifiedIdentifier ();
		SymbolDeclaration = classDeclaration;
		EnumMembers = []; // always empty since classes dot not have enum values
		Attributes = [];

		var properties = ImmutableArray.CreateBuilder<Property> ();
		var propertyDeclarations = classDeclaration.Members.OfType<PropertyDeclarationSyntax> ();
		foreach (var declaration in propertyDeclarations) {
			if (Skip (declaration, semanticModel))
				continue;
			if (Property.TryCreate (declaration, semanticModel, out var change))
				properties.Add (change.Value);
		}

		Properties = properties.ToImmutable ();

		var constructors = ImmutableArray.CreateBuilder<Constructor> ();
		var constructorDeclarations = classDeclaration.Members.OfType<ConstructorDeclarationSyntax> ();
		foreach (var declaration in constructorDeclarations) {
			if (Skip (declaration, semanticModel))
				continue;
			if (Constructor.TryCreate (declaration, semanticModel, out var change))
				constructors.Add (change.Value);
		}

		Constructors = constructors.ToImmutable ();

		var events = ImmutableArray.CreateBuilder<Event> ();
		var eventDeclarations = classDeclaration.Members.OfType<EventDeclarationSyntax> ();
		foreach (var declaration in eventDeclarations) {
			if (Skip (declaration, semanticModel))
				continue;
			if (Event.TryCreate (declaration, semanticModel, out var change))
				events.Add (change.Value);
		}

		Events = events.ToImmutable ();

		var methods = ImmutableArray.CreateBuilder<Method> ();
		var methodDeclarations = classDeclaration.Members.OfType<MethodDeclarationSyntax> ();
		foreach (MethodDeclarationSyntax declaration in methodDeclarations) {
			if (Skip (declaration, semanticModel))
				continue;
			if (Method.TryCreate (declaration, semanticModel, out var change))
				methods.Add (change.Value);
		}
		Methods = methods.ToImmutable ();
	}

	/// <summary>
	/// Creates a new instance of the <see cref="CodeChanges"/> struct for a given interface declaration.
	/// </summary>
	/// <param name="interfaceDeclaration">The interface declaration that triggered the change.</param>
	/// <param name="semanticModel">The semantic model of the compilation.</param>
	CodeChanges (InterfaceDeclarationSyntax interfaceDeclaration, SemanticModel semanticModel)
	{
		FullyQualifiedSymbol = interfaceDeclaration.GetFullyQualifiedIdentifier ();
		SymbolDeclaration = interfaceDeclaration;
		// TODO: to be implemented once we add protocol support
		EnumMembers = [];
		Attributes = [];
	}

	/// <summary>
	/// Create a CodeChange from the provide base type declaration syntax. If the syntax is not supported,
	/// it will return null.
	/// </summary>
	/// <param name="baseTypeDeclarationSyntax">The declaration syntax whose change we want to calculate.</param>
	/// <param name="semanticModel">The semantic model related to the syntax tree that contains the node.</param>
	/// <returns>A code change or null if it could not be calculated.</returns>
	public static CodeChanges? FromDeclaration (BaseTypeDeclarationSyntax baseTypeDeclarationSyntax,
		SemanticModel semanticModel)
		=> baseTypeDeclarationSyntax switch {
			EnumDeclarationSyntax enumDeclarationSyntax => new CodeChanges (enumDeclarationSyntax, semanticModel),
			InterfaceDeclarationSyntax interfaceDeclarationSyntax => new CodeChanges (interfaceDeclarationSyntax,
				semanticModel),
			ClassDeclarationSyntax classDeclarationSyntax => new CodeChanges (classDeclarationSyntax, semanticModel),
			_ => null
		};
}
