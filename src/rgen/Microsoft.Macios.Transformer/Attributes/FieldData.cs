// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Macios.Transformer.Attributes;

public struct FieldData : IEquatable<FieldData> {

	public bool Equals (FieldData other)
	{
		throw new NotImplementedException ();
	}

	/// <inheritdoc />
	public override bool Equals (object? obj)
	{
		return obj is ExportData other && Equals (other);
	}

	/// <inheritdoc />
	public override int GetHashCode ()
	{
		throw new NotImplementedException ();
	}

	public static bool operator == (FieldData x, FieldData y)
	{
		return x.Equals (y);
	}

	public static bool operator != (FieldData x, FieldData y)
	{
		return !(x == y);
	}
}
