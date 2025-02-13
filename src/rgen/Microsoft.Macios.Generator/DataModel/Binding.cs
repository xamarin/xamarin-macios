// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.Context;

namespace Microsoft.Macios.Generator.DataModel;

/// <summary>
/// Structure that represents a set of changes that were made by the user that need to be applied to the
/// generated code.
/// </summary>
[StructLayout (LayoutKind.Auto)]
readonly partial struct Binding {

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

	delegate bool SkipDelegate<in T> (T declarationSyntax, SemanticModel semanticModel);

	delegate bool TryCreateDelegate<in T, TR> (T declaration, RootContext context,
		[NotNullWhen (true)] out TR? change)
		where T : MemberDeclarationSyntax
		where TR : struct;

	static void GetMembers<T, TR> (TypeDeclarationSyntax baseDeclarationSyntax, RootContext context,
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
}
