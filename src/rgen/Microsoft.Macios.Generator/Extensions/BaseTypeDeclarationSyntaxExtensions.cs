using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.Macios.Generator.Extensions;

static class BaseTypeDeclarationSyntaxExtensions {
	/// <summary>
	/// Return the fully qualified identifier for a given <see cref="BaseTypeDeclarationSyntax"/> by
	/// navigating the syntax tree and getting the namespace and class names.
	/// </summary>
	/// <param name="self">The declaration whose fully qualified name we want to retrieve.</param>
	/// <returns>A fully qualified identifier with all namespaces and classes found in the syntax tree.</returns>
	public static string GetFullyQualifiedIdentifier (this BaseTypeDeclarationSyntax self)
	{
		var root = self.SyntaxTree.GetRoot ();
		// check if the namespace is a file scoped one "namespace Foo;"
		var fileScoped = root.DescendantNodes ()
			.OfType<FileScopedNamespaceDeclarationSyntax> ()
			.FirstOrDefault ();

		var namespaces = self.Ancestors ()
			.OfType<NamespaceDeclarationSyntax> ()
			.Reverse ()
			.Select (ns => ns.Name.ToString ().Trim ())
			.ToArray ();

		// get all the classes
		var parents = self.Ancestors ()
			.OfType<BaseTypeDeclarationSyntax> ()
			.Reverse ()
			.Select (c => c.Identifier.ToFullString ().Trim ())
			.ToArray ();

		var sb = new StringBuilder ();
		if (fileScoped is not null)
			sb.Append ($"{fileScoped.Name}");
		// not need to add a '.' before the namespaces, you cannot have both field scope and namespace
		sb.AppendJoin (".", namespaces);
		if (parents.Length > 0) {
			sb.Append ('.');
			sb.AppendJoin (".", parents);
		}

		sb.Append ($".{self.Identifier.ToFullString ()}");
		return sb.ToString ().Trim ();
	}
}
