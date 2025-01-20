// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Macios.Transformer.Attributes;

public struct ExportData : IEquatable<ExportData> {
	
	public bool Equals (ExportData other)
	{
		throw new NotImplementedException();
	}
	
	/// <inheritdoc />
	public override bool Equals (object? obj)
	{
		return obj is ExportData other && Equals (other);
	}

	/// <inheritdoc />
	public override int GetHashCode ()
	{
		throw new NotImplementedException();
	}

	public static bool operator == (ExportData x, ExportData y)
	{
		return x.Equals (y);
	}

	public static bool operator != (ExportData x, ExportData y)
	{
		return !(x == y);
	}
}
