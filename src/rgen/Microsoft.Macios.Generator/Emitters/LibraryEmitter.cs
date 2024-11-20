using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Context;

namespace Microsoft.Macios.Generator.Emitters;

class LibraryEmitter (
	RootBindingContext context,
	TabbedStringBuilder builder) : ICodeEmitter {
	public string SymbolNamespace => "ObjCRuntime";
	public string SymbolName => "Libraries";
	INamedTypeSymbol? LibrarySymbol { get; } = context.Compilation.GetTypeByMetadataName ("ObjCRuntime.Libraries");

	bool IsSymbolPresent (string className)
		=> LibrarySymbol is not null &&
		   LibrarySymbol.GetMembers ().OfType<INamedTypeSymbol> ()
			   .Any (v => v.Name == className);

	public IEnumerable<string> UsingStatements { get; } = [];

	public bool TryEmit ([NotNullWhen (false)] out ImmutableArray<Diagnostic>? diagnostics)
	{
		diagnostics = null;

		builder.AppendLine ("using Foundation;");
		builder.AppendLine ("using ObjCBindings;");
		builder.AppendLine ("using ObjCRuntime;");
		builder.AppendLine ("using System;");

		// keep track if we added a lib, if not we don't need to generate the class
		bool added = false;

		builder.AppendLine ();
		builder.AppendLine ($"namespace ObjCRuntime;");
		builder.AppendLine ();

		builder.AppendGeneratedCodeAttribute ();
		using (var classBlock = builder.CreateBlock ($"static partial class {SymbolName}", true)) {
			foreach (var (name, path) in context.Libraries.OrderBy (v => v.Key,
						 StringComparer.Ordinal)) {
				// verify if the symbol is already defined by the runtime
				var className = name.Replace (".", string.Empty);
				if (IsSymbolPresent (className))
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
