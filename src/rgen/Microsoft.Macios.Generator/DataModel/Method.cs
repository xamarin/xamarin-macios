using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.Extensions;
using ObjCRuntime;

namespace Microsoft.Macios.Generator.DataModel;

readonly struct Method : IEquatable<Method> {

	/// <summary>
	/// Type name that owns the constructor.
	/// </summary>
	public string Type { get; }

	/// <summary>
	/// Method name.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Method return type.
	/// </summary>
	public ReturnType ReturnType { get; }

	/// <summary>
	/// The platform availability of the method.
	/// </summary>
	public SymbolAvailability SymbolAvailability { get; }

	/// <summary>
	/// The data of the export attribute used to mark the value as a property binding. 
	/// </summary>
	public ExportData<ObjCBindings.Method> ExportMethodData { get; }

	/// <summary>
	/// Get the attributes added to the constructor.
	/// </summary>
	public ImmutableArray<AttributeCodeChange> Attributes { get; } = [];

	/// <summary>
	/// Modifiers list.
	/// </summary>
	public ImmutableArray<SyntaxToken> Modifiers { get; } = [];

	/// <summary>
	/// Parameters list.
	/// </summary>
	public ImmutableArray<Parameter> Parameters { get; } = [];

	public Method (string type, string name, ReturnType returnType,
		SymbolAvailability symbolAvailability,
		ExportData<ObjCBindings.Method> exportMethodData,
		ImmutableArray<AttributeCodeChange> attributes,
		ImmutableArray<SyntaxToken> modifiers,
		ImmutableArray<Parameter> parameters)
	{
		Type = type;
		Name = name;
		ReturnType = returnType;
		SymbolAvailability = symbolAvailability;
		ExportMethodData = exportMethodData;
		Attributes = attributes;
		Modifiers = modifiers;
		Parameters = parameters;
	}

	public static bool TryCreate (MethodDeclarationSyntax declaration, SemanticModel semanticModel,
		[NotNullWhen (true)] out Method? change)
	{
		if (semanticModel.GetDeclaredSymbol (declaration) is not IMethodSymbol method) {
			change = null;
			return false;
		}

		var attributes = declaration.GetAttributeCodeChanges (semanticModel);
		var parametersBucket = ImmutableArray.CreateBuilder<Parameter> ();
		// loop over the parameters of the construct since changes on those implies a change in the generated code
		foreach (var parameter in method.Parameters) {
			var parameterDeclaration = declaration.ParameterList.Parameters [parameter.Ordinal];
			if (!Parameter.TryCreate (parameter, parameterDeclaration, semanticModel, out var parameterChange))
				continue;
			parametersBucket.Add (parameterChange.Value);
		}

		// DO NOT USE default if null, the reason is that it will set the ArgumentSemantics to be value 0, when
		// none is value 1. The reason for that is that the default of an enum is 0, that was a mistake 
		// in the old binding code.
		var exportData = method.GetExportData<ObjCBindings.Method> ()
						 ?? new (null, ArgumentSemantic.None, ObjCBindings.Method.Default);
		change = new (
			type: method.ContainingSymbol.ToDisplayString ().Trim (), // we want the full name
			name: method.Name,
			returnType: new (method.ReturnType),
			symbolAvailability: method.GetSupportedPlatforms (),
			exportMethodData: exportData,
			attributes: attributes,
			modifiers: [.. declaration.Modifiers],
			parameters: parametersBucket.ToImmutableArray ());
		return true;
	}

	/// <inheritdoc/>
	public bool Equals (Method other)
	{
		if (Type != other.Type)
			return false;
		if (Name != other.Name)
			return false;
		if (ReturnType != other.ReturnType)
			return false;
		if (SymbolAvailability != other.SymbolAvailability)
			return false;
		if (ExportMethodData != other.ExportMethodData)
			return false;

		var attrsComparer = new AttributesEqualityComparer ();
		if (!attrsComparer.Equals (Attributes, other.Attributes))
			return false;
		var modifiersComparer = new ModifiersEqualityComparer ();
		if (!modifiersComparer.Equals (Modifiers, other.Modifiers))
			return false;

		var paramComparer = new ParameterEqualityComparer ();
		return paramComparer.Equals (Parameters, other.Parameters);
	}

	/// <inheritdoc/>
	public override bool Equals (object? obj)
	{
		return obj is Method other && Equals (other);
	}

	/// <inheritdoc/>
	public override int GetHashCode ()
	{
		var hashCode = new HashCode ();
		hashCode.Add (Type);
		hashCode.Add (Name);
		hashCode.Add (ReturnType);
		foreach (var modifier in Modifiers) {
			hashCode.Add (modifier);
		}

		foreach (var attr in Attributes) {
			hashCode.Add (attr);
		}

		foreach (var parameter in Parameters) {
			hashCode.Add (parameter);
		}

		return hashCode.ToHashCode ();
	}

	public static bool operator == (Method left, Method right)
	{
		return left.Equals (right);
	}

	public static bool operator != (Method left, Method right)
	{
		return !left.Equals (right);
	}

	/// <inheritdoc/>
	public override string ToString ()
	{
		var sb = new StringBuilder ($"{{ Method: Type: {Type}, ");
		sb.Append ($"Name: {Name}, ");
		sb.Append ($"ReturnType: {ReturnType}, ");
		sb.Append ($"SymbolAvailability: {SymbolAvailability}, ");
		sb.Append ($"ExportMethodData: {ExportMethodData}, ");
		sb.Append ("Attributes: [");
		sb.AppendJoin (", ", Attributes);
		sb.Append ("], Modifiers: [");
		sb.AppendJoin (", ", Modifiers.Select (x => x.Text));
		sb.Append ("], Parameters: [");
		sb.AppendJoin (", ", Parameters);
		sb.Append ("] }}");
		return sb.ToString ();
	}
}
