// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.Context;
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
	public BindingType BindingType => BindingInfo.BindingType;

	readonly BindingInfo bindingInfo = default;
	/// <summary>
	/// Represents the binding data that will be used to generate the code.
	/// </summary>
	public BindingInfo BindingInfo => bindingInfo;

	readonly string name = string.Empty;
	/// <summary>
	/// The name of the named type that generated the code change.
	/// </summary>
	public string Name => name;

	readonly ImmutableArray<string> namespaces = ImmutableArray<string>.Empty;
	/// <summary>
	/// The namespace that contains the named type that generated the code change.
	/// </summary>
	public ImmutableArray<string> Namespace => namespaces;

	readonly ImmutableArray<string> interfaces = ImmutableArray<string>.Empty;
	/// <summary>
	/// The list of interfaces implemented by the class/interface.
	/// </summary>
	public ImmutableArray<string> Interfaces {
		get => interfaces;
		init => interfaces = value;
	}

	/// <summary>
	/// Fully qualified name of the symbol that the code changes are for.
	/// </summary>
	public string FullyQualifiedSymbol { get; }

	readonly string? baseClass = null;

	/// <summary>
	/// The fully qualified name of an interface/class base.
	/// </summary>
	public string? Base {
		get => baseClass;
		init => baseClass = value;

	}

	readonly SymbolAvailability availability = new ();
	/// <summary>
	/// The platform availability of the named type.
	/// </summary>
	public SymbolAvailability SymbolAvailability => availability;

	/// <summary>
	/// Changes to the attributes of the symbol.
	/// </summary>
	public ImmutableArray<AttributeCodeChange> Attributes { get; init; } = [];

	readonly IReadOnlySet<string> usingDirectives = ImmutableHashSet<string>.Empty;

	/// <summary>
	/// The using directive added in the named type declaration.
	/// </summary>
	public IReadOnlySet<string> UsingDirectives {
		get => usingDirectives;
		init => usingDirectives = value;
	}

	/// <summary>
	/// True if the code changes are for a static symbol.
	/// </summary>
	public bool IsStatic { get; private init; }

	/// <summary>
	/// True if the code changes are for a partial symbol.
	/// </summary>
	public bool IsPartial { get; private init; }

	/// <summary>
	/// True if the code changes are for an abstract symbol.
	/// </summary>
	public bool IsAbstract { get; private init; }

	readonly ImmutableArray<SyntaxToken> modifiers = [];
	/// <summary>
	/// Modifiers list.
	/// </summary>
	public ImmutableArray<SyntaxToken> Modifiers {
		get => modifiers;
		init {
			modifiers = value;
			IsStatic = value.Any (m => m.IsKind (SyntaxKind.StaticKeyword));
			IsPartial = value.Any (m => m.IsKind (SyntaxKind.PartialKeyword));
			IsAbstract = value.Any (m => m.IsKind (SyntaxKind.AbstractKeyword));
		}
	}

	readonly ImmutableArray<EnumMember> enumMembers = [];

	/// <summary>
	/// Changes to the enum members of the symbol.
	/// </summary>
	public ImmutableArray<EnumMember> EnumMembers {
		get => enumMembers;
		init => enumMembers = value;
	}

	readonly ImmutableArray<Property> properties = [];

	/// <summary>
	/// Changes to the properties of the symbol.
	/// </summary>
	public ImmutableArray<Property> Properties {
		get => properties;
		init => properties = value;
	}

	readonly ImmutableArray<Constructor> constructors = [];

	/// <summary>
	/// Changes to the constructors of the symbol.
	/// </summary>
	public ImmutableArray<Constructor> Constructors {
		get => constructors;
		init => constructors = value;
	}

	readonly ImmutableArray<Event> events = [];

	/// <summary>
	/// Changes to the events of the symbol.
	/// </summary>
	public ImmutableArray<Event> Events {
		get => events;
		init => events = value;
	}

	readonly ImmutableArray<Method> methods = [];

	/// <summary>
	/// Changes to the methods of a symbol.
	/// </summary>
	public ImmutableArray<Method> Methods {
		get => methods;
		init => methods = value;
	}

	/// <summary>
	/// Returns all the library names and paths that are needed by the native code represented by the code change.
	/// </summary>
	public IEnumerable<(string LibraryName, string? LibraryPath)> LibraryPaths {
		get {
			// we want to return unique values, A library name should always point to the
			// same library path (either null or with a value). We keep track of the ones we already
			// returned in a set and skip if we already returned it.
			var visited = new HashSet<string> ();

			// return those libs needed by smart enums
			foreach (var enumMember in EnumMembers) {
				if (enumMember.FieldInfo is null)
					continue;
				var (_, libraryName, libraryPath) = enumMember.FieldInfo.Value;
				if (visited.Add (libraryName)) // if already visited, we cannot add it
					yield return (libraryName, libraryPath);
			}
			
			// return those libs needed by field properties
			foreach (var property in Properties) {
				if (property.ExportFieldData is null)
					continue;
				var (_, libraryName, libraryPath) = property.ExportFieldData.Value;
				if (visited.Add (libraryName)) // if already visited, we cannot add it
					yield return (libraryName, libraryPath);
			}
		}
	}

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
				AttributesNames.FieldPropertyAttribute, AttributesNames.ExportPropertyAttribute);
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

	delegate bool SkipDelegate<in T> (T declarationSyntax, SemanticModel semanticModel);

	delegate bool TryCreateDelegate<in T, TR> (T declaration, RootBindingContext context,
		[NotNullWhen (true)] out TR? change)
		where T : MemberDeclarationSyntax
		where TR : struct;

	static void GetMembers<T, TR> (TypeDeclarationSyntax baseDeclarationSyntax, RootBindingContext context,
		SkipDelegate<T> skip, TryCreateDelegate<T, TR> tryCreate, out ImmutableArray<TR> members)
		where T : MemberDeclarationSyntax
		where TR : struct
	{
		var bucket = ImmutableArray.CreateBuilder<TR> ();
		var declarations = baseDeclarationSyntax.Members.OfType<T> ();
		foreach (var declaration in declarations) {
			if (skip (declaration, context.SemanticModel))
				continue;
			if (tryCreate (declaration, context, out var change))
				bucket.Add (change.Value);
		}

		members = bucket.ToImmutable ();
	}

	/// <summary>
	/// Internal constructor added for testing purposes.
	/// </summary>
	/// <param name="bindingInfo">The binding data of binding for the given code changes.</param>
	/// <param name="name">The name of the named type that created the code change.</param>
	/// <param name="namespace">The namespace that contains the named type.</param>
	/// <param name="fullyQualifiedSymbol">The fully qualified name of the symbol.</param>
	/// <param name="symbolAvailability">The platform availability of the named symbol.</param>
	internal CodeChanges (BindingInfo bindingInfo, string name, ImmutableArray<string> @namespace,
		string fullyQualifiedSymbol, SymbolAvailability symbolAvailability)
	{
		this.bindingInfo = bindingInfo;
		this.name = name;
		this.namespaces = @namespace;
		FullyQualifiedSymbol = fullyQualifiedSymbol;
		this.availability = symbolAvailability;
	}

	/// <summary>
	/// Creates a new instance of the <see cref="CodeChanges"/> struct for a given enum declaration.
	/// </summary>
	/// <param name="enumDeclaration">The enum declaration that triggered the change.</param>
	/// <param name="context">The root binding context of the current compilation.</param>
	CodeChanges (EnumDeclarationSyntax enumDeclaration, RootBindingContext context)
	{
		context.SemanticModel.GetSymbolData (
			declaration: enumDeclaration,
			bindingType: BindingType.SmartEnum,
			name: out name,
			baseClass: out baseClass,
			interfaces: out interfaces,
			namespaces: out namespaces,
			symbolAvailability: out availability,
			bindingInfo: out bindingInfo);
		FullyQualifiedSymbol = enumDeclaration.GetFullyQualifiedIdentifier ();
		Attributes = enumDeclaration.GetAttributeCodeChanges (context.SemanticModel);
		UsingDirectives = enumDeclaration.SyntaxTree.CollectUsingStatements ();
		Modifiers = [.. enumDeclaration.Modifiers];
		var bucket = ImmutableArray.CreateBuilder<EnumMember> ();
		// loop over the fields and add those that contain a FieldAttribute
		var enumValueDeclarations = enumDeclaration.Members.OfType<EnumMemberDeclarationSyntax> ();
		foreach (var enumValueDeclaration in enumValueDeclarations) {
			if (Skip (enumValueDeclaration, context.SemanticModel))
				continue;
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
				symbolAvailability: enumValueSymbol.GetSupportedPlatforms (),
				attributes: enumValueDeclaration.GetAttributeCodeChanges (context.SemanticModel)
			);
			bucket.Add (enumMember);
		}

		EnumMembers = bucket.ToImmutable ();
	}

	/// <summary>
	/// Creates a new instance of the <see cref="CodeChanges"/> struct for a given class declaration.
	/// </summary>
	/// <param name="classDeclaration">The class declaration that triggered the change.</param>
	/// <param name="context">The root binding context of the current compilation.</param>
	CodeChanges (ClassDeclarationSyntax classDeclaration, RootBindingContext context)
	{
		context.SemanticModel.GetSymbolData (
			declaration: classDeclaration,
			bindingType: BindingType.Class,
			name: out name,
			baseClass: out baseClass,
			interfaces: out interfaces,
			namespaces: out namespaces,
			symbolAvailability: out availability,
			bindingInfo: out bindingInfo);
		FullyQualifiedSymbol = classDeclaration.GetFullyQualifiedIdentifier ();
		Attributes = classDeclaration.GetAttributeCodeChanges (context.SemanticModel);
		UsingDirectives = classDeclaration.SyntaxTree.CollectUsingStatements ();
		Modifiers = [.. classDeclaration.Modifiers];

		// use the generic method to get the members, we are using an out param to try an minimize the number of times
		// the value types are copied
		GetMembers<ConstructorDeclarationSyntax, Constructor> (classDeclaration, context, Skip,
			Constructor.TryCreate, out constructors);
		GetMembers<PropertyDeclarationSyntax, Property> (classDeclaration, context, Skip, Property.TryCreate,
			out properties);
		GetMembers<EventDeclarationSyntax, Event> (classDeclaration, context, Skip, Event.TryCreate, out events);
		GetMembers<MethodDeclarationSyntax, Method> (classDeclaration, context, Skip, Method.TryCreate,
			out methods);
	}

	/// <summary>
	/// Creates a new instance of the <see cref="CodeChanges"/> struct for a given interface declaration.
	/// </summary>
	/// <param name="interfaceDeclaration">The interface declaration that triggered the change.</param>
	/// <param name="context">The root binding context of the current compilation.</param>
	CodeChanges (InterfaceDeclarationSyntax interfaceDeclaration, RootBindingContext context)
	{
		context.SemanticModel.GetSymbolData (
			declaration: interfaceDeclaration,
			bindingType: BindingType.Protocol,
			name: out name,
			baseClass: out baseClass,
			interfaces: out interfaces,
			namespaces: out namespaces,
			symbolAvailability: out availability,
			bindingInfo: out bindingInfo);
		FullyQualifiedSymbol = interfaceDeclaration.GetFullyQualifiedIdentifier ();
		Attributes = interfaceDeclaration.GetAttributeCodeChanges (context.SemanticModel);
		UsingDirectives = interfaceDeclaration.SyntaxTree.CollectUsingStatements ();
		Modifiers = [.. interfaceDeclaration.Modifiers];
		// we do not init the constructors, we use the default empty array

		GetMembers<PropertyDeclarationSyntax, Property> (interfaceDeclaration, context.SemanticModel, Skip, Property.TryCreate,
			out properties);
		GetMembers<EventDeclarationSyntax, Event> (interfaceDeclaration, context.SemanticModel, Skip, Event.TryCreate,
			out events);
		GetMembers<MethodDeclarationSyntax, Method> (interfaceDeclaration, context.SemanticModel, Skip, Method.TryCreate,
			out methods);
	}

	/// <summary>
	/// Create a CodeChange from the provide base type declaration syntax. If the syntax is not supported,
	/// it will return null.
	/// </summary>
	/// <param name="baseTypeDeclarationSyntax">The declaration syntax whose change we want to calculate.</param>
	/// <param name="context">The root binding context of the current compilation.</param>
	/// <returns>A code change or null if it could not be calculated.</returns>
	public static CodeChanges? FromDeclaration (BaseTypeDeclarationSyntax baseTypeDeclarationSyntax,
		RootBindingContext context)
		=> baseTypeDeclarationSyntax switch {
			EnumDeclarationSyntax enumDeclarationSyntax => new CodeChanges (enumDeclarationSyntax, context),
			InterfaceDeclarationSyntax interfaceDeclarationSyntax => new CodeChanges (interfaceDeclarationSyntax,
				context),
			ClassDeclarationSyntax classDeclarationSyntax => new CodeChanges (classDeclarationSyntax, context),
			_ => null
		};

	/// <inheritdoc/>
	public override string ToString ()
	{
		var sb = new StringBuilder ("Changes: {");
		sb.Append ($"BindingData: '{BindingInfo}', Name: '{Name}', Namespace: [");
		sb.AppendJoin (", ", Namespace);
		sb.Append ($"], FullyQualifiedSymbol: '{FullyQualifiedSymbol}', Base: '{Base ?? "null"}', SymbolAvailability: {SymbolAvailability}, ");
		sb.Append ("Interfaces: [");
		sb.AppendJoin (", ", Interfaces);
		sb.Append ("], Attributes: [");
		sb.AppendJoin (", ", Attributes);
		sb.Append ("], UsingDirectives: [");
		sb.AppendJoin (", ", UsingDirectives);
		sb.Append ("], Modifiers: [");
		sb.AppendJoin (", ", Modifiers);
		sb.Append ("], EnumMembers: [");
		sb.AppendJoin (", ", EnumMembers);
		sb.Append ("], Constructors: [");
		sb.AppendJoin (", ", Constructors);
		sb.Append ("], Properties: [");
		sb.AppendJoin (", ", Properties);
		sb.Append ("], Methods: [");
		sb.AppendJoin (", ", Methods);
		sb.Append ("], Events: [");
		sb.AppendJoin (", ", Events);
		sb.Append ("] }");
		return sb.ToString ();
	}
}
