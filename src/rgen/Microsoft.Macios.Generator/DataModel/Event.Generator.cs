// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.Extensions;

namespace Microsoft.Macios.Generator.DataModel;

readonly partial struct Event {
	
	public static bool TryCreate (EventDeclarationSyntax declaration, RootBindingContext context,
		[NotNullWhen (true)] out Event? change)
	{
		var memberName = declaration.Identifier.ToFullString ().Trim ();
		// get the symbol from the property declaration
		if (context.SemanticModel.GetDeclaredSymbol (declaration) is not IEventSymbol eventSymbol) {
			change = null;
			return false;
		}

		var type = eventSymbol.Type.ToDisplayString ().Trim ();
		var attributes = declaration.GetAttributeCodeChanges (context.SemanticModel);
		ImmutableArray<Accessor> accessorCodeChanges = [];
		if (declaration.AccessorList is not null && declaration.AccessorList.Accessors.Count > 0) {
			// calculate any possible changes in the accessors of the property
			var accessorsBucket = ImmutableArray.CreateBuilder<Accessor> ();
			foreach (var accessorDeclaration in declaration.AccessorList.Accessors) {
				if (context.SemanticModel.GetDeclaredSymbol (accessorDeclaration) is not ISymbol accessorSymbol)
					continue;
				var kind = accessorDeclaration.Kind ().ToAccessorKind ();
				var accessorAttributeChanges = accessorDeclaration.GetAttributeCodeChanges (context.SemanticModel);
				accessorsBucket.Add (new (
					accessorKind: kind,
					symbolAvailability: accessorSymbol.GetSupportedPlatforms (),
					exportPropertyData: null,
					attributes: accessorAttributeChanges,
					modifiers: [.. accessorDeclaration.Modifiers]));
			}

			accessorCodeChanges = accessorsBucket.ToImmutable ();
		}

		change = new (memberName, type, eventSymbol.GetSupportedPlatforms (), attributes,
			[.. declaration.Modifiers], accessorCodeChanges);
		return true;
	}
}
