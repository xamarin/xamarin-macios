// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.Extensions;
using Microsoft.Macios.Transformer.Attributes;
using Microsoft.Macios.Transformer.DataModel;
using Microsoft.Macios.Transformer.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Microsoft.Macios.Generator.DataModel;

readonly partial struct Binding {

	/// <summary>
	/// Represents the type of binding that the code changes are for.
	/// </summary>
	public BindingType BindingType => BindingInfo.BindingType;

	/// <summary>
	/// Returns the binding data of the binding for the given code changes.
	/// </summary>
	public BindingInfo BindingInfo { get; init; }


	readonly ImmutableArray<string> protocols = ImmutableArray<string>.Empty;

	/// <summary>
	/// Returns the list of protocols that the binding implements.
	/// The names of the protocols DO NOT include the 'I' prefix.
	/// </summary>
	public ImmutableArray<string> Protocols {
		get => protocols;
		init => protocols = value;
	}

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

	internal Binding (EnumDeclarationSyntax enumDeclaration, INamedTypeSymbol symbol, RootContext context)
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

	/// <summary>
	/// Retrieve the type of a binding based on its attributes.
	/// </summary>
	/// <param name="attributes">The dictionary with the attributes of the symbol.</param>
	/// <param name="baseTypeAttribute">The base type attribute of the binding.</param>
	/// <returns>The type of binding or BindingType.Unknown if it could not be calculated.</returns>
	static BindingType GetBindingType (Dictionary<string, List<AttributeData>> attributes, BaseTypeData? baseTypeAttribute)
	{
		BindingType bindingType;
		if (attributes.HasProtocolFlag () || attributes.HasModelFlag ()) {
			bindingType = BindingType.Protocol;
		} else if (attributes.HasCategoryFlag ()) {
			bindingType = BindingType.Category;
		} else if (attributes.HasCoreImageFilterAttribute ()) {
			bindingType = BindingType.CoreImageFilter;
		} else if (baseTypeAttribute is not null) {
			bindingType = BindingType.Class;
		} else {
			// we do not know what type of binding this is, we will discard it later
			bindingType = BindingType.Unknown;
		}

		return bindingType;
	}

	/// <summary>
	/// Retrieve the base class for a given binding based on its binding infor.
	/// </summary>
	/// <param name="symbol">The symbol whose base class we want to retrieve..</param>
	/// <param name="bindingInfo">The binding information.</param>
	/// <returns>The base class to use for the new style SDK based on the binding information.</returns>
	static string GetBaseClass (INamedTypeSymbol symbol, BindingInfo bindingInfo)
	{
		// collecting the base class and the interface is a little more complicated in the older SDK style because
		// we have to make the difference between a class base and a protocol base
#pragma warning disable format
		return bindingInfo.BindingType switch {
			// for classes, use the base type attribute if it exists, otherwise use the default
			BindingType.Class => bindingInfo.BaseTypeData?.BaseType ?? "Foundation.NSObject",
			BindingType.CoreImageFilter => bindingInfo.BaseTypeData?.BaseType ?? "Foundation.NSObject",
			// categories are extension classes and they always inherit from object
			BindingType.Category => "object",
			// protocols do not have a base class, if anything, they implement other protocols 
			BindingType.Protocol => string.Empty,
			// for unknown types, use the default
			_ => "object"
		};
#pragma warning restore format
	}

	/// <summary>
	/// Collects all the interfaces and protocols implemented by a given symbol.
	/// </summary>
	/// <param name="symbol">The symbol under query.</param>
	/// <param name="interfaces">An out array in which the interfaces will be added.</param>
	/// <param name="protocols">An out array in which the protocols will be added. Protocols will not have the 'I' prefix
	/// added.</param>
	static void GetInterfaceAndProtocols (INamedTypeSymbol symbol, out ImmutableArray<string> interfaces,
		out ImmutableArray<string> protocols)
	{
		// Collecting interfaces is different between the old than the new SDK styles. We need to make the difference
		// between actual interfaces and protocols. Protocols, in the old SDK definition, do not have the 'I' prefix, but
		// we should not only guide ourselves on the name, but also on the attributes that the user used.
		var interfacesBucket = ImmutableArray.CreateBuilder<string> ();
		var protocolsBucket = ImmutableArray.CreateBuilder<string> ();
		foreach (var symbolInterface in symbol.Interfaces) {
			// decide if we are dealing with a protocol/model or interface by looking at the attributes
			var interfaceAttrs = symbolInterface.GetAttributeData ();
			if (interfaceAttrs.HasProtocolFlag () || interfaceAttrs.HasModelFlag ()) {
				protocolsBucket.Add (symbolInterface.ToDisplayString ().Trim ());
			} else {
				interfacesBucket.Add (symbolInterface.ToDisplayString ().Trim ());
			}
		}
		interfaces = interfacesBucket.ToImmutable ();
		protocols = protocolsBucket.ToImmutable ();
	}

	/// <summary>
	/// Create a new binding based on the interface declaration. Because in the old SDK old the bindings
	/// are represented by an interface, this constructor will ensure that the correct binding type is set.
	/// </summary>
	/// <param name="interfaceDeclarationSyntax">An interface that declares a binding.</param>
	/// <param name="symbol"></param>
	/// <param name="context">The current compilation context.</param>
	internal Binding (InterfaceDeclarationSyntax interfaceDeclarationSyntax, INamedTypeSymbol symbol, RootContext context)
	{
		// basic properties of the binding
		FullyQualifiedSymbol = interfaceDeclarationSyntax.GetFullyQualifiedIdentifier ();
		UsingDirectives = interfaceDeclarationSyntax.SyntaxTree.CollectUsingStatements ();
		AttributesDictionary = symbol.GetAttributeData ();
		var baseTypeAttribute = symbol.GetBaseTypeData ();
		BindingInfo = new (baseTypeAttribute, GetBindingType (AttributesDictionary, baseTypeAttribute));
		name = symbol.Name;
		availability = symbol.GetAvailabilityForSymbol ();
		namespaces = symbol.GetNamespaceArray ();
		baseClass = GetBaseClass (symbol, BindingInfo);

		// retrieve the interfaces and protocols, notice that this are two out params
		GetInterfaceAndProtocols (symbol, out interfaces, out protocols);

		// use the helper struct to get the modifiers
		var flags = new ModifiersFlags (
			hasAbstractFlag: HasAbstractFlag,
			hasInternalFlag: HasInternalFlag,
			hasNewFlag: false,  // makes no sense on a class/interface
			hasOverrideFlag: false, // makes no sense on a class/interface
			hasStaticFlag: HasStaticFlag || BindingInfo.BindingType == BindingType.Category // add static for categories
		);
		Modifiers = flags.ToClassModifiersArray ();
	}

	/// <inheritdoc/>
	public override string ToString ()
	{
		var sb = new StringBuilder ("Changes: {");
		sb.Append ($"BindingData: '{BindingInfo}', Name: '{Name}', Namespace: [");
		sb.AppendJoin (", ", Namespace);
		sb.Append ($"], FullyQualifiedSymbol: '{FullyQualifiedSymbol}', Base: '{Base ?? "null"}', SymbolAvailability: {SymbolAvailability}, ");
		sb.Append ("Interfaces: [");
		sb.AppendJoin (", ", Interfaces);
		sb.Append ("], Protocols: [");
		sb.AppendJoin (", ", Protocols);
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
