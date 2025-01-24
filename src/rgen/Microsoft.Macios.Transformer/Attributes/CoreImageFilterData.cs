// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using MethodAttributes = Mono.Cecil.MethodAttributes;

namespace Microsoft.Macios.Transformer.Attributes;

readonly struct CoreImageFilterData : IEquatable<CoreImageFilterData> {
	
	public MethodAttributes DefaultCtorVisibility { get; init; }

	public MethodAttributes IntPtrCtorVisibility { get; init; }

	public MethodAttributes StringCtorVisibility { get; init; }

	public CoreImageFilterData ()
	{
		
		DefaultCtorVisibility = MethodAttributes.Public;
		IntPtrCtorVisibility = MethodAttributes.Private;
		StringCtorVisibility = MethodAttributes.Private;
	}


	public static bool TryParse (AttributeData attributeData,
		[NotNullWhen (true)] out CoreImageFilterData? data)
	{
		var defaultVisibility = MethodAttributes.Public;
		var intPtrVisibility = MethodAttributes.Private;
		var stringVisibility = MethodAttributes.Private;
		
		// there is not positional constructor for this attribute	
		foreach (var (argumentName, value) in attributeData.NamedArguments) {
			switch (argumentName) {
			case "DefaultCtorVisibility":
				defaultVisibility = (MethodAttributes) Convert.ToSingle((int) value.Value!);
				break;
			case "IntPtrCtorVisibility":
				intPtrVisibility = (MethodAttributes) Convert.ToSingle((int) value.Value!);
				break;
			case "StringCtorVisibility":
				stringVisibility = (MethodAttributes) Convert.ToSingle((int) value.Value!);
				break;
			default:
				data = null;
				return false;
			}
		}

		data = new () {
			DefaultCtorVisibility = defaultVisibility,
			IntPtrCtorVisibility = intPtrVisibility,
			StringCtorVisibility = stringVisibility,
		};
		return true;
	}
	
	public bool Equals (CoreImageFilterData other)
	{
		if (DefaultCtorVisibility != other.DefaultCtorVisibility)
			return false;
		if (IntPtrCtorVisibility != other.IntPtrCtorVisibility)
			return false;
		return StringCtorVisibility == other.StringCtorVisibility;
	}

	/// <inheritdoc />
	public override bool Equals (object? obj)
	{
		return obj is CoreImageFilterData other && Equals (other);
	}

	/// <inheritdoc />
	public override int GetHashCode ()
		=> HashCode.Combine (DefaultCtorVisibility, IntPtrCtorVisibility, StringCtorVisibility);

	public static bool operator == (CoreImageFilterData x, CoreImageFilterData y)
	{
		return x.Equals (y);
	}

	public static bool operator != (CoreImageFilterData x, CoreImageFilterData y)
	{
		return !(x == y);
	}
	
	/// <inheritdoc />
	public override string ToString ()
	{
		return $"{{ DefaultCtorVisibility: {DefaultCtorVisibility}, IntPtrCtorVisibility: {IntPtrCtorVisibility}, StringCtorVisibility: {StringCtorVisibility} }}";
	}
}
