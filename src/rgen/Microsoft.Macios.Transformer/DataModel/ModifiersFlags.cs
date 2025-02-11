// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Microsoft.Macios.Generator.DataModel;

/// <summary>
/// Helper class to store the modifiers flags. It also provides a method that will
/// return the correct modifiers according to the flags;
/// </summary>
readonly record struct ModifiersFlags {

	public bool HasAbstractFlag { get; }
	public bool HasInternalFlag { get; }
	public bool HasNewFlag { get; }
	public bool HasOverrideFlag { get; }
	public bool HasStaticFlag { get; }

	/// <summary>
	/// Create a new structure with the provided flags.
	/// </summary>
	/// <param name="hasAbstractFlag">The state of the abstract flag.</param>
	/// <param name="hasInternalFlag">The state of the internal flag.</param>
	/// <param name="hasNewFlag">The state of the new flag.</param>
	/// <param name="hasOverrideFlag">The state of the override flag.</param>
	/// <param name="hasStaticFlag">The state of the static flag.</param>
	public ModifiersFlags (bool hasAbstractFlag, bool hasInternalFlag, bool hasNewFlag, bool hasOverrideFlag, bool hasStaticFlag)
	{
		HasAbstractFlag = hasAbstractFlag;
		HasInternalFlag = hasInternalFlag;
		HasNewFlag = hasNewFlag;
		HasOverrideFlag = hasOverrideFlag;
		HasStaticFlag = hasStaticFlag;
	}

	/// <summary>
	/// Returns the list of modifiers to be used with the provided set of flags for a method/property.
	/// </summary>
	/// <returns>The list of modifiers to use to write the transformed method/property.</returns>
	public ImmutableArray<SyntaxToken> ToMethodModifiersArray ()
	{
#pragma warning disable format
		// Modifiers are special because we might be dealing with several flags that the user has set in the method.
		// We have to add the partial keyword so that we can have the partial implementation of the method later generated
		// by the roslyn code generator
		return this switch {
			// internal static partial
			{ HasNewFlag: false, HasStaticFlag: true, HasInternalFlag: true } 
				=> [Token (SyntaxKind.InternalKeyword), Token (SyntaxKind.StaticKeyword), Token (SyntaxKind.PartialKeyword)],
			
			// public static partial
			{ HasNewFlag: false, HasStaticFlag: true, HasInternalFlag: false } 
				=> [Token (SyntaxKind.PublicKeyword), Token (SyntaxKind.StaticKeyword), Token (SyntaxKind.PartialKeyword)],
			
			// internal new static partial
			{ HasNewFlag: true, HasStaticFlag: true, HasInternalFlag: true} 
				=> [Token (SyntaxKind.InternalKeyword), Token (SyntaxKind.NewKeyword), Token (SyntaxKind.StaticKeyword), Token (SyntaxKind.PartialKeyword)],
			
			// public new static partial
			{ HasNewFlag: true, HasStaticFlag: true, HasInternalFlag: false }
				=> [Token (SyntaxKind.PublicKeyword), Token (SyntaxKind.NewKeyword), Token (SyntaxKind.StaticKeyword), Token (SyntaxKind.PartialKeyword)],
			
			// public new virtual partial
			{ HasNewFlag: true, HasStaticFlag: false, HasAbstractFlag: false, HasInternalFlag: false }
				=> [Token (SyntaxKind.PublicKeyword), Token (SyntaxKind.NewKeyword), Token (SyntaxKind.VirtualKeyword), Token (SyntaxKind.PartialKeyword)],
			
			// internal new virtual partial
			{ HasNewFlag: true, HasStaticFlag: false, HasAbstractFlag: false, HasInternalFlag: true }
				=> [Token (SyntaxKind.InternalKeyword), Token (SyntaxKind.NewKeyword), Token (SyntaxKind.VirtualKeyword), Token (SyntaxKind.PartialKeyword)],
			
			// public new abstract
			{ HasNewFlag: true, HasAbstractFlag: true, HasInternalFlag: false}
				=> [Token (SyntaxKind.PublicKeyword), Token (SyntaxKind.NewKeyword), Token (SyntaxKind.AbstractKeyword)],
			
			// internal new abstract
			{ HasNewFlag: true, HasAbstractFlag: true, HasInternalFlag: true}
				=> [Token (SyntaxKind.InternalKeyword), Token (SyntaxKind.NewKeyword), Token (SyntaxKind.AbstractKeyword)],
			
			// public override partial
			{ HasNewFlag: false, HasOverrideFlag: true, HasInternalFlag: false}
				=> [Token (SyntaxKind.PublicKeyword), Token (SyntaxKind.OverrideKeyword), Token (SyntaxKind.PartialKeyword)],
			
			// internal override partial
			{ HasNewFlag: false, HasOverrideFlag: true, HasInternalFlag: true}
				=> [Token (SyntaxKind.InternalKeyword), Token (SyntaxKind.OverrideKeyword), Token (SyntaxKind.PartialKeyword)],
			
			// public abstract 
			{ HasAbstractFlag: true, HasInternalFlag: false}
				=> [Token (SyntaxKind.PublicKeyword), Token (SyntaxKind.AbstractKeyword)],
			
			// internal abstract
			{ HasAbstractFlag: true, HasInternalFlag: true}
				=> [Token (SyntaxKind.InternalKeyword), Token (SyntaxKind.AbstractKeyword)],
			
			// general case, but internal
			{ HasInternalFlag: true} => 
				[Token (SyntaxKind.InternalKeyword), Token (SyntaxKind.VirtualKeyword), Token (SyntaxKind.PartialKeyword)],
			
			// general case
			_ => [Token (SyntaxKind.PublicKeyword), Token (SyntaxKind.VirtualKeyword), Token (SyntaxKind.PartialKeyword)]
		};
#pragma warning restore format
	}
	
	/// <summary>
	/// Returns the list of modifiers to be used with the provided set of flags for a method/property.
	/// </summary>
	/// <returns>The list of modifiers to use to write the transformed method/property.</returns>
	public ImmutableArray<SyntaxToken> ToClassModifiersArray ()
	{
#pragma warning disable format
		// Modifiers are special because we might be dealing with several flags that the user has set in the method.
		// We have to add the partial keyword so that we can have the partial implementation of the method later generated
		// by the roslyn code generator
		return this switch {
			// internal static partial
			{ HasNewFlag: false, HasStaticFlag: true, HasInternalFlag: true } 
				=> [Token (SyntaxKind.InternalKeyword), Token (SyntaxKind.StaticKeyword), Token (SyntaxKind.PartialKeyword)],
			
			// public static partial
			{ HasNewFlag: false, HasStaticFlag: true, HasInternalFlag: false } 
				=> [Token (SyntaxKind.PublicKeyword), Token (SyntaxKind.StaticKeyword), Token (SyntaxKind.PartialKeyword)],
			
			// internal new static partial
			{ HasNewFlag: true, HasStaticFlag: true, HasInternalFlag: true} 
				=> [Token (SyntaxKind.InternalKeyword), Token (SyntaxKind.NewKeyword), Token (SyntaxKind.StaticKeyword), Token (SyntaxKind.PartialKeyword)],
			
			// public new static partial
			{ HasNewFlag: true, HasStaticFlag: true, HasInternalFlag: false }
				=> [Token (SyntaxKind.PublicKeyword), Token (SyntaxKind.NewKeyword), Token (SyntaxKind.StaticKeyword), Token (SyntaxKind.PartialKeyword)],
			
			// public new virtual partial
			{ HasNewFlag: true, HasStaticFlag: false, HasAbstractFlag: false, HasInternalFlag: false }
				=> [Token (SyntaxKind.PublicKeyword), Token (SyntaxKind.NewKeyword), Token (SyntaxKind.PartialKeyword)],
			
			// internal new virtual partial
			{ HasNewFlag: true, HasStaticFlag: false, HasAbstractFlag: false, HasInternalFlag: true }
				=> [Token (SyntaxKind.InternalKeyword), Token (SyntaxKind.NewKeyword), Token (SyntaxKind.PartialKeyword)],
			
			// public new abstract
			{ HasNewFlag: true, HasAbstractFlag: true, HasInternalFlag: false}
				=> [Token (SyntaxKind.PublicKeyword), Token (SyntaxKind.NewKeyword), Token (SyntaxKind.AbstractKeyword)],
			
			// internal new abstract
			{ HasNewFlag: true, HasAbstractFlag: true, HasInternalFlag: true}
				=> [Token (SyntaxKind.InternalKeyword), Token (SyntaxKind.NewKeyword), Token (SyntaxKind.AbstractKeyword)],
			
			// public override partial
			{ HasNewFlag: false, HasOverrideFlag: true, HasInternalFlag: false}
				=> [Token (SyntaxKind.PublicKeyword), Token (SyntaxKind.OverrideKeyword), Token (SyntaxKind.PartialKeyword)],
			
			// internal override partial
			{ HasNewFlag: false, HasOverrideFlag: true, HasInternalFlag: true}
				=> [Token (SyntaxKind.InternalKeyword), Token (SyntaxKind.OverrideKeyword), Token (SyntaxKind.PartialKeyword)],
			
			// public abstract 
			{ HasAbstractFlag: true, HasInternalFlag: false}
				=> [Token (SyntaxKind.PublicKeyword), Token (SyntaxKind.AbstractKeyword)],
			
			// internal abstract
			{ HasAbstractFlag: true, HasInternalFlag: true}
				=> [Token (SyntaxKind.InternalKeyword), Token (SyntaxKind.AbstractKeyword)],
			
			// general case, but internal
			{ HasInternalFlag: true} => 
				[Token (SyntaxKind.InternalKeyword), Token (SyntaxKind.PartialKeyword)],
			
			// general case
			_ => [Token (SyntaxKind.PublicKeyword), Token (SyntaxKind.PartialKeyword)]
		};
#pragma warning restore format
	}

}
