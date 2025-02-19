// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Xamarin.Utils;

namespace Microsoft.Macios.Transformer;

/// <summary>
/// Holds the result of a compilation. This is a helper record to make code cleaner in the transformer.
/// </summary>
/// <param name="Platform">The platform the compilation targeted.</param>
/// <param name="Compilation">The compilation result.</param>
/// <param name="Errors">All compilation errors.</param>
record CompilationResult (ApplePlatform Platform, Compilation Compilation, ImmutableArray<Diagnostic> Errors) {

	public (ApplePlatform Platform, Compilation compilation) ToTuple ()
		=> (Platform, Compilation);
}
