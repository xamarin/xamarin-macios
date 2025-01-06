using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.Macios.Generator.Extensions;

static class SyntaxTreeExtensions {

	public static IReadOnlySet<string> CollectUsingStatements (this SyntaxTree self)
	{
		// collect all using from the syntax tree, add them to a hash to make sure that we don't have duplicates
		// and add those usings that we do know we need for bindings.
		var usingDirectives = self.GetRoot ()
			.DescendantNodes ()
			.OfType<UsingDirectiveSyntax> ()
			.Select (d => d.Name!.ToString ()).ToArray ();

		// remove any possible duplicates by using a set
		return new HashSet<string> (usingDirectives);
	}

}
