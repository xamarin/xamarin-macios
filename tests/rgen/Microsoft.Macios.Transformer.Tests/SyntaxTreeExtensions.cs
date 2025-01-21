// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Transformer.Tests;

public static class SyntaxTreeExtensions {

	public static SyntaxTree? ForSource (this IEnumerable<SyntaxTree> trees, (string Source, string Path) source)
		=> trees.FirstOrDefault (t => t.FilePath == source.Path);
}
