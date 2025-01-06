using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Generator.DataModel;

/// <summary>
/// Readonly structure that describes a delegate callback passed as a parameter.
/// </summary>
readonly struct DelegateInfo : IEquatable<DelegateInfo> {

	/// <summary>
	/// Type name that owns the method.
	/// </summary>
	public string Type { get; }

	/// <summary>
	/// Method name.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Method return type.
	/// </summary>
	public string ReturnType { get; }

	/// <summary>
	/// Parameters list.
	/// </summary>
	public ImmutableArray<DelegateParameter> Parameters { get; } = [];

	public DelegateInfo (string type, string name, string returnType, ImmutableArray<DelegateParameter> parameters)
	{
		Type = type;
		Name = name;
		ReturnType = returnType;
		Parameters = parameters;
	}

	public static bool TryCreate (IMethodSymbol method, [NotNullWhen (true)] out DelegateInfo? change)
	{
		var parametersBucket = ImmutableArray.CreateBuilder<DelegateParameter> ();
		// loop over the parameters of the construct since changes on those implies a change in the generated code
		foreach (var parameter in method.Parameters) {
			if (!DelegateParameter.TryCreate (parameter, out var parameterChange))
				continue;
			parametersBucket.Add (parameterChange.Value);
		}

		change = new (
			type: method.ContainingSymbol.ToDisplayString ().Trim (), // we want the full name
			name: method.Name,
			returnType: method.ReturnType.ToDisplayString ().Trim (),
			parameters: parametersBucket.ToImmutableArray ());
		return true;
	}

	/// <inheritdoc/>
	public bool Equals (DelegateInfo other)
	{
		if (Type != other.Type)
			return false;
		if (Name != other.Name)
			return false;
		if (ReturnType != other.ReturnType)
			return false;

		var paramComparer = new DelegateParameterEqualityComparer ();
		return paramComparer.Equals (Parameters, other.Parameters);
	}

	/// <inheritdoc/>
	public override bool Equals (object? obj)
	{
		return obj is DelegateInfo other && Equals (other);
	}

	/// <inheritdoc/>
	public override int GetHashCode ()
	{
		var hashCode = new HashCode ();
		hashCode.Add (Type);
		hashCode.Add (Name);
		hashCode.Add (ReturnType);

		foreach (var parameter in Parameters) {
			hashCode.Add (parameter);
		}

		return hashCode.ToHashCode ();
	}

	public static bool operator == (DelegateInfo left, DelegateInfo right)
	{
		return left.Equals (right);
	}

	public static bool operator != (DelegateInfo left, DelegateInfo right)
	{
		return !left.Equals (right);
	}

	/// <inheritdoc/>
	public override string ToString ()
	{
		var sb = new StringBuilder ($"{{ Type: {Type}, ");
		sb.Append ($"Name: {Name}, ");
		sb.Append ($"ReturnType: {ReturnType}, ");
		sb.Append ("Parameters: [");
		sb.AppendJoin (", ", Parameters);
		sb.Append ("] }}");
		return sb.ToString ();
	}
}
