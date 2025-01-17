// // Copyright (c) Microsoft Corporation.
// // Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Macios.Generator.DataModel;

class LibraryPathsComparer : EqualityComparer<(string LibraryName, string? LibraryPath)> {

	public override bool Equals ((string LibraryName, string? LibraryPath) x, (string LibraryName, string? LibraryPath) y)
		=> String.Compare (x.LibraryName, y.LibraryName, StringComparison.OrdinalIgnoreCase) == 0;

	public override int GetHashCode ([DisallowNull] (string LibraryName, string? LibraryPath) obj)
		=> HashCode.Combine (obj.LibraryName, obj.LibraryPath);
}
