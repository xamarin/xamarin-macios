using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.Macios.Generator.Extensions;

public static class BaseTypeDeclarationSyntaxExtensions {
	public static string GetFullyQualifiedIdentifier (this BaseTypeDeclarationSyntax self)
	{
		var root = self.SyntaxTree.GetRoot ();
		// check if the namespace is a file scoped one
		var fileScoped = root.DescendantNodes ()
			.OfType<FileScopedNamespaceDeclarationSyntax> ()
			.FirstOrDefault ();

		var namespaces = self.Ancestors ()
			.OfType<NamespaceDeclarationSyntax> ()
			.Reverse ()
			.ToArray ();

		// get all the classes
		var parents = self.Ancestors ()
			.OfType<BaseTypeDeclarationSyntax> ()
			.Reverse ()
			.ToArray ();

		var sb = new StringBuilder ();
		if (fileScoped is not null)
			sb.Append ($"{fileScoped.Name}.");
		foreach (var ns in namespaces) {
			sb.Append ($"{ns.Name}.");
		}

		foreach (var c in parents) {
			sb.Append ($"{c.Identifier.ToFullString ().Trim ()}.");
		}

		sb.Append (self.Identifier.ToFullString ());
		return sb.ToString ().Trim ();
	}
}
