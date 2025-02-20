// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using Microsoft.Macios.Generator.Context;

namespace Microsoft.Macios.Generator.DataModel;

/// <summary>
/// Custom code changes comparer used for the Roslyn code generation to invalidate caching.
/// </summary>
class BindingEqualityComparer : EqualityComparer<Binding> {
	/// <inheritdoc />
	public override bool Equals (Binding x, Binding y)
	{

		// order does not matter in the using directives, use a comparer that sorts them
		var ignoreOrderComparer = new CollectionComparer<string> (StringComparer.InvariantCulture);
		// things that mean a code change is the same:
		// - the fully qualified symbol is the same
		// - the binding type is the same
		// - the syntax node type is the same
		// - the members are the same
		// - the attributes are the same

		// this could be a massive 'or' but that makes it less readable
		if (x.Name != y.Name)
			return false;
		// order matters in the namespaces, therefore, use a comparer that does not reorder the collections
		var namespaceComparer = new CollectionComparer<string> ();
		if (!namespaceComparer.Equals (x.Namespace, y.Namespace))
			return false;
		if (x.FullyQualifiedSymbol != y.FullyQualifiedSymbol)
			return false;
		if (x.Base != y.Base)
			return false;
		if (!ignoreOrderComparer.Equals (x.Interfaces, y.Interfaces))
			return false;
		if (x.SymbolAvailability != y.SymbolAvailability)
			return false;
		if (x.BindingType != y.BindingType)
			return false;
		if (x.Attributes.Length != y.Attributes.Length)
			return false;
		if (!ignoreOrderComparer.Equals (x.UsingDirectives, y.UsingDirectives))
			return false;
		if (x.EnumMembers.Length != y.EnumMembers.Length)
			return false;
		if (x.Properties.Length != y.Properties.Length)
			return false;

		// compare the attrs, we need to sort them since attribute order does not matter
		var attrComparer = new AttributesEqualityComparer ();
		if (!attrComparer.Equals (x.Attributes, y.Attributes))
			return false;

		var modifiersComparer = new ModifiersEqualityComparer ();
		if (!modifiersComparer.Equals (x.Modifiers, y.Modifiers))
			return false;

		// compare the members
		var memberComparer = new EnumMembersEqualityComparer ();
		if (!memberComparer.Equals (x.EnumMembers, y.EnumMembers))
			return false;

		// compare properties
		var propertyComparer = new PropertiesEqualityComparer ();
		if (!propertyComparer.Equals (x.Properties, y.Properties))
			return false;

		// compare constructors
		var constructorComparer = new ConstructorsEqualityComparer ();
		if (!constructorComparer.Equals (x.Constructors, y.Constructors))
			return false;

		// compare events
		var eventComparer = new EventEqualityComparer ();
		if (!eventComparer.Equals (x.Events, y.Events))
			return false;

		var methodComparer = new MethodsEqualityComparer ();
		return methodComparer.Equals (x.Methods, y.Methods);
	}

	/// <inheritdoc />
	public override int GetHashCode (Binding obj)
	{
		return HashCode.Combine (obj.FullyQualifiedSymbol, obj.EnumMembers);
	}
}

class RootBindingEqualityComparer : EqualityComparer<(RootContext Context, Binding Binding)> {
	BindingEqualityComparer comparer = new ();
	/// <inheritdoc />
	public override bool Equals ((RootContext Context, Binding Binding) x, (RootContext Context, Binding Binding) y)
	{
		return comparer.Equals (x.Binding, y.Binding);
	}

	/// <inheritdoc />
	public override int GetHashCode ((RootContext Context, Binding Binding) obj)
	{
		return comparer.GetHashCode (obj.Binding);
	}
}
