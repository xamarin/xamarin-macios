// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Generator.Tests;

public readonly struct CompilationResult (Compilation compilation, ImmutableArray<SyntaxTree> syntaxTrees) {
	public Compilation Compilation { get; init; } = compilation;
	public ImmutableArray<SyntaxTree> SyntaxTrees { get; init; } = syntaxTrees;

	public void Deconstruct (out Compilation compilation, out ImmutableArray<SyntaxTree> trees)
	{
		compilation = Compilation;
		trees = SyntaxTrees;
	}
}
