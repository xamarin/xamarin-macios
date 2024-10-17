using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.Extensions;

namespace Microsoft.Macios.Generator.Emitters;

class LibraryEmitter (
	RootBindingContext context,
	TabbedStringBuilder builder,
	ImmutableArray<CodeChanges> codeChangesList) : ICodeEmitter {
	public string SymbolName => "Libraries";
	INamedTypeSymbol? LibrarySymbol { get; } = context.Compilation.GetTypeByMetadataName ("ObjCRuntime.Libraries");

	bool IsSymbolPreset (string className)
		=> LibrarySymbol is not null &&
		   LibrarySymbol.GetMembers ().OfType<INamedTypeSymbol> ()
			   .Any (v => v.Name == className);

	public IEnumerable<string> UsingStatements { get; } = [];

	void GetEnumLibraries (INamedTypeSymbol symbol)
	{
		var typeNamespace = symbol.ContainingNamespace.Name;
		if (!symbol.TryGetEnumFields (out var members, out var diagnostics))
			return;
		// Libs in this case are coming from the FieldAttribute of ech of the members
		foreach (var enumField in members) {
			if (!context.TryComputeLibraryName (enumField.FieldData.LibraryName, typeNamespace,
				    out _, out _)) {
				return;
			}
		}
	}

	void GetClassLibraries (INamedTypeSymbol symbol)
	{
	}

	void GetInterfaceLibraries (INamedTypeSymbol symbol)
	{
	}

	public bool TryEmit ([NotNullWhen (false)] out ImmutableArray<Diagnostic>? diagnostics)
	{
		diagnostics = [];

		builder.AppendLine ("using Foundation;");
		builder.AppendLine ("using ObjCBindings;");
		builder.AppendLine ("using ObjCRuntime;");
		builder.AppendLine ("using System;");

		// keep track if we added a lib, if not we don't need to generate the class
		bool added = false;

		// go over the code changes and retrieve any library data information with the help of the root context
		foreach (var codeChange in codeChangesList) {
			var semanticModel = context.Compilation.GetSemanticModel (codeChange.SymbolDeclaration.SyntaxTree);
#pragma warning disable RS1039
			if (semanticModel.GetDeclaredSymbol (codeChange.SymbolDeclaration) is not INamedTypeSymbol symbol)
#pragma warning restore RS1039
				continue;
			switch (symbol.TypeKind) {
			case TypeKind.Enum:
				GetEnumLibraries (symbol);
				break;
			case TypeKind.Class:
				GetClassLibraries (symbol);
				break;
			case TypeKind.Interface:
				GetInterfaceLibraries (symbol);
				break;
			}
		}

	    builder.AppendLine ();
	    builder.AppendLine ($"namespace ObjCRuntime;");
	    builder.AppendLine ();

		builder.AppendGeneratedCodeAttribute ();
		using (var classBlock = builder.CreateBlock ($"static partial class {SymbolName}", true)) {
			foreach (var (name, path) in context.Libraries.OrderBy (v => v.Key,
						 StringComparer.Ordinal)) {
				// verify if the symbol is already defined by the runtime
				var className = name.Replace (".", string.Empty);
				if (IsSymbolPreset (className))
					continue;

				using (var nestedClass =
					   classBlock.CreateBlock ($"static public class {className}", true)) {
					if (name == "__Internal") {
						nestedClass.AppendLine ("static public readonly IntPtr Handle = Dlfcn.dlopen (null, 0);");
					} else if (context.IsSystemLibrary (name)) {
						nestedClass.AppendLine (
							$"static public readonly IntPtr Handle = Dlfcn._dlopen (Constants.{name}Library, 0);");
					} else {
						nestedClass.AppendLine (
							$"static public readonly IntPtr Handle = Dlfcn.dlopen (\"{path}\", 0);");
					}
				}

				added = true;
			}
		}

		return added;
	}
}
