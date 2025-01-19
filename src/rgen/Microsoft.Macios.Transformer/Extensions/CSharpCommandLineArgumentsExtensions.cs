// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Microsoft.Macios.Transformer.Extensions;

public static class CSharpCommandLineArgumentsExtensions {
	public static ImmutableArray<PortableExecutableReference> GetReferences (this CSharpCommandLineArguments self,
		string workingDirectory, string sdkDirectory)
	{
		var rspReferences = self.MetadataReferences.Select (r => {
			var path = Path.IsPathRooted (r.Reference)
				? r.Reference
				: Path.Combine (workingDirectory, r.Reference);
			return path;
		});
		var sdkReferences= Directory.GetFiles(sdkDirectory, "*.dll");;

		return rspReferences
			.Union (sdkReferences)
			.Distinct ()
			.Select (p => MetadataReference.CreateFromFile (p))
			.ToImmutableArray ();
	}

	public static ImmutableArray<SyntaxTree> GetSourceFiles (this CSharpCommandLineArguments self,
		CSharpParseOptions options)
	{
		var sources = self.SourceFiles.Select (s => s.Path)
			.ToImmutableArray ();
		return sources.Select (
				s => CSharpSyntaxTree.ParseText (File.ReadAllText (s), options, s))
			.ToImmutableArray ();
	}
}
