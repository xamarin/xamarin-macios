// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Macios.Generator.DataModel;

/// <summary>
/// This struct works as a union to store the possible BindingTypeData that can be present in the bindings.
/// </summary>
readonly struct BindingInfo : IEquatable<BindingInfo> {
	
	/// <inheritdoc />
	public bool Equals (BindingInfo other)
	{
		throw new NotImplementedException();
	}
	
	/// <inheritdoc />
	public override bool Equals (object? obj)
	{
		return obj is BindingInfo other && Equals (other);
	}

	/// <inheritdoc />
	public override int GetHashCode ()
	{
		throw new NotImplementedException ();
	}
	
	public static bool operator == (BindingInfo x, BindingInfo y)
	{
		return x.Equals (y);
	}

	public static bool operator != (BindingInfo x, BindingInfo y)
	{
		return !(x == y);
	}
}
