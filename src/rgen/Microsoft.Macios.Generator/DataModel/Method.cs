using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Extensions;

namespace Microsoft.Macios.Generator.DataModel;

readonly struct Method : IEquatable<Method> {
	
	public string Type { get; }
	public string Name { get; }
	public string ReturnType { get; }
	
	public ImmutableArray<AttributeCodeChange> Attributes { get; } = [];

	public ImmutableArray<SyntaxToken> Modifiers { get; } = [];

	public ImmutableArray<Parameter> Parameters { get; } = [];

	public Method (string type, string name, string returnType, ImmutableArray<AttributeCodeChange> attributes,
		ImmutableArray<SyntaxToken> modifiers,
		ImmutableArray<Parameter> parameters)
	{
		Type = type;
		Name = name;
		ReturnType = returnType;
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
			parametersBucket.Add (new (parameter.Ordinal, parameter.Type.ToDisplayString ().Trim (),
				parameter.Name) {
				IsOptional = parameter.IsOptional,
				IsParams = parameter.IsParams,
				IsThis = parameter.IsThis,
				IsNullable = parameter.NullableAnnotation == NullableAnnotation.Annotated,
				DefaultValue = (parameter.HasExplicitDefaultValue) ? parameter.ExplicitDefaultValue?.ToString () : null,
				ReferenceKind = parameter.RefKind.ToReferenceKind (),
				Attributes = parameterDeclaration.GetAttributeCodeChanges (semanticModel),
			});
		}

		change = new(
			type: method.ContainingSymbol.ToDisplayString ().Trim (), // we want the full name
			name: method.Name,
			returnType: method.ReturnType.ToDisplayString ().Trim(),
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
		var attrsComparer = new AttributesEqualityComparer ();
		if (!attrsComparer.Equals (Attributes, other.Attributes))
			return false;
		var modifiersComparer = new ModifiersComparer ();
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
