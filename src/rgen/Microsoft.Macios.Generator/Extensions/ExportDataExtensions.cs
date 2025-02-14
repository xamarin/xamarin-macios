// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Microsoft.Macios.Generator.Attributes;

namespace Microsoft.Macios.Generator.Extensions;

static class ExportDataExtensions {

	/// <summary>
	/// Return the selector field name for a given export data.
	/// </summary>
	/// <param name="self">The export data whose selector name we want to retrienve.</param>
	/// <param name="inlineSelectors">Id the selectors are inlined</param>
	/// <typeparam name="T">The type of export data.</typeparam>
	/// <returns>The selector handle name or null if it could not be calculated.</returns>
	public static string? GetSelectorFieldName<T> (this ExportData<T> self, bool inlineSelectors = false) where T : Enum
		=> self.Selector?.GetSelectorFieldName (inlineSelectors);
}
