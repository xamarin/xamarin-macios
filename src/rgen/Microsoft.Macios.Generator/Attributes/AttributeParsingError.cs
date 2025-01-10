// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Macios.Generator.Attributes;

readonly struct AttributeParsingError<T> where T : Enum {
	public T Error { get; }

	[MemberNotNullWhen (true, nameof (Value))]
	public bool IsError => !Error.Equals (default (T));

	public object? Value { get; }

	public AttributeParsingError ()
	{
		Error = default!;
		Value = null;
	}
	public AttributeParsingError (T error, object? value)
	{
		Error = error;
		Value = (!error.Equals (default (T)) && value is null) ?
			throw new ArgumentNullException (nameof (value)) : value;
	}
}
