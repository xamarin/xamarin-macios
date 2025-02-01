// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.Extensions;
using Microsoft.Macios.Transformer.Attributes;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Microsoft.Macios.Generator.DataModel;

readonly partial struct Method {

	readonly ExportData? overrideExportData;

	/// <summary>
	/// The data of the export attribute used to mark the value as a property binding. 
	/// </summary>
	public ExportData? ExportMethodData {
		get {
			return overrideExportData ?? ExportAttribute;
		}
		init => overrideExportData = value;
	}

	/// <summary>
	/// Returns the bind from data if present in the binding.
	/// </summary>
	public BindAsData? BindAs => BindAsAttribute;

	/// <summary>
	/// True if the method was exported with the MarshalNativeExceptions flag allowing it to support native exceptions.
	/// </summary>
	public bool MarshalNativeExceptions => HasMarshalNativeExceptionsFlag;
	
	const string constructorName = "Constructor";
	/// <summary>
	/// True if the method is considered to be a constructor in the old bindings.
	/// </summary>
	public bool IsConstructor => Name == constructorName;

	public Method (string type,
		string name,
		TypeInfo returnType,
		SymbolAvailability symbolAvailability,
		Dictionary<string, List<AttributeData>> attributes,
		ImmutableArray<Parameter> parameters)
	{
		Type = type;
		Name = name;
		AttributesDictionary = attributes;
		ReturnType = returnType;
		SymbolAvailability = symbolAvailability;
		Parameters = parameters;
		
#pragma warning disable format
		// Modifiers are special because we might be dealing with several flags that the user has set in the method.
		// We have to add the partial keyword so that we can have the partial implementation of the method later generated
		// by the roslyn code generator
		Modifiers = this switch {
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
				[Token (SyntaxKind.InternalKeyword), Token (SyntaxKind.VirtualKeyword), Token (SyntaxKind.VirtualKeyword)],
			
			// general case
			_ => [Token (SyntaxKind.PublicKeyword), Token (SyntaxKind.VirtualKeyword), Token (SyntaxKind.PartialKeyword)]
		};
#pragma warning restore format

	}

	public static bool TryCreate (MethodDeclarationSyntax declaration, SemanticModel semanticModel,
		[NotNullWhen (true)] out Method? change)
	{
		if (semanticModel.GetDeclaredSymbol (declaration) is not IMethodSymbol method) {
			change = null;
			return false;
		}
		var parametersBucket = ImmutableArray.CreateBuilder<Parameter> ();
		// loop over the parameters of the construct since changes on those implies a change in the generated code
		foreach (var parameter in method.Parameters) {
			var parameterDeclaration = declaration.ParameterList.Parameters [parameter.Ordinal];
			if (!Parameter.TryCreate (parameter, parameterDeclaration, semanticModel, out var parameterChange))
				continue;
			parametersBucket.Add (parameterChange.Value);
		}
		// get the attributes of the method, this are used for two reasons:
		// 1. The attributes might have flags such as [Internal] or [Abstract] that we need to set in the modifiers
		// 2. The attributes might have return attributes that we need to set in the return type
		var attributes = method.GetAttributeData ();
		change = new (
			type: method.ContainingSymbol.ToDisplayString ().Trim (), // we want the full name
			name: method.Name,
			returnType: new TypeInfo (method.ReturnType, attributes),
			symbolAvailability: method.GetAvailabilityForSymbol (), // special case, in the transformer we only translate, we do not merge
			attributes: attributes,
			parameters: parametersBucket.ToImmutableArray ());
		return true;
	}
}
