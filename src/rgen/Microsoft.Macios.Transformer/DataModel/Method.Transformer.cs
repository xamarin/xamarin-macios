// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.Extensions;
using Microsoft.Macios.Transformer.Attributes;

namespace Microsoft.Macios.Generator.DataModel;

readonly partial struct Method {

	readonly ExportData? overrideExportData;

	/// <summary>
	/// The data of the export attribute used to mark the value as a property binding. 
	/// </summary>
	public ExportData? ExportMethodData {
		get => overrideExportData ?? ExportAttribute;
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

		// create a helper struct to retrieve the modifiers
		var flags = new ModifiersFlags (HasAbstractFlag, HasInternalFlag, HasNewFlag, HasOverrideFlag, HasStaticFlag);
		Modifiers = flags.ToMethodModifiersArray ();
	}

	public static bool TryCreate (MethodDeclarationSyntax declaration, RootContext context,
		[NotNullWhen (true)] out Method? change)
	{
		if (context.SemanticModel.GetDeclaredSymbol (declaration) is not IMethodSymbol method) {
			change = null;
			return false;
		}
		var parametersBucket = ImmutableArray.CreateBuilder<Parameter> ();
		// loop over the parameters of the construct since changes on those implies a change in the generated code
		foreach (var parameter in method.Parameters) {
			var parameterDeclaration = declaration.ParameterList.Parameters [parameter.Ordinal];
			if (!Parameter.TryCreate (parameter, parameterDeclaration, context.SemanticModel, out var parameterChange))
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

	/// <summary>
	/// Return the constructor representation of the method if it is a constructor in the bindings.
	/// </summary>
	/// <returns>A constructor data model if the method is one in the bindings or null otherwise.</returns>
	public Constructor? ToConstructor ()
		=> IsConstructor ? new Constructor (this) : null;

}
