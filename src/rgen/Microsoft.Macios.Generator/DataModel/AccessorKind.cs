using Microsoft.CodeAnalysis.CSharp;

namespace Microsoft.Macios.Generator.DataModel;

/// <summary>
/// Identifies the kind of accessor of a property.
/// </summary>
enum AccessorKind {
	Unknown = 0,
	Getter,
	Setter,
	Initializer,
	Add,
	Remove,
}

static class AccessorKindExtensions {

	public static AccessorKind ToAccessorKind (this SyntaxKind self) => self switch {
		SyntaxKind.GetAccessorDeclaration => AccessorKind.Getter,
		SyntaxKind.SetAccessorDeclaration => AccessorKind.Setter,
		SyntaxKind.InitAccessorDeclaration => AccessorKind.Initializer,
		SyntaxKind.AddAccessorDeclaration => AccessorKind.Add,
		SyntaxKind.RemoveAccessorDeclaration => AccessorKind.Remove,
		_ => AccessorKind.Unknown,
	};
}
