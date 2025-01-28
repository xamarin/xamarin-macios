// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Transformer.Attributes;

readonly record struct ThreadSafeData {

	public bool Safe { get; } = true;

	public ThreadSafeData () : this (true) { }

	public ThreadSafeData (bool safe)
	{
		Safe = safe;
	}

	public static bool TryParse (AttributeData attributeData,
		[NotNullWhen (true)] out ThreadSafeData? data)
	{
		data = null;
		var count = attributeData.ConstructorArguments.Length;
		if (count == 0) {
			data = new ();
			return true;
		}
		bool safe = true;
		switch (count) {
		case 1:
			safe = (bool) attributeData.ConstructorArguments [0].Value!;
			break;
		default:
			// 0 should not be an option..
			return false;
		}

		data = new ThreadSafeData (safe);
		return true;
	}

}
