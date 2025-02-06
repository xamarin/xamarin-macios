// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.IO;

namespace Microsoft.Macios.Generator.Emitters;

class LibraryEmitter (
	RootContext context,
	TabbedStringBuilder builder) {
	public string SymbolNamespace => "ObjCRuntime";
	public string SymbolName => "Libraries";
	INamedTypeSymbol? LibrarySymbol { get; } = context.Compilation.GetTypeByMetadataName ("ObjCRuntime.Libraries");

	/// <summary>
	/// Verify if the symbol is already defined by the runtime
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	bool IsSymbolPresent (string name)
	{
		var className = name.Replace (".", string.Empty);
		return LibrarySymbol is not null &&
		   LibrarySymbol.GetMembers ().OfType<INamedTypeSymbol> ()
			   .Any (v => v.Name == className);
	}

	public IEnumerable<string> UsingStatements { get; } = [];

	public bool TryEmit (ImmutableArray<(string LibraryName, string? LibraryPath)> libraries,
		[NotNullWhen (false)] out ImmutableArray<Diagnostic>? diagnostics)
	{
		diagnostics = null;
		// we do not want to generate the library partial class if we already contain all the symbols,
		// this happens because we are using rgen while bgen is still a thing. Filter those libraries out
		var filteredLibs = libraries.Where (info => !IsSymbolPresent (info.LibraryName)).ToArray ();
		if (filteredLibs.Length == 0) {
			return true;
		}

		builder.WriteLine ("using Foundation;");
		builder.WriteLine ("using ObjCBindings;");
		builder.WriteLine ("using ObjCRuntime;");
		builder.WriteLine ("using System;");

		// keep track if we added a lib, if not we don't need to generate the class
		bool added = false;

		builder.WriteLine ();
		builder.WriteLine ($"namespace ObjCRuntime;");
		builder.WriteLine ();

		builder.AppendGeneratedCodeAttribute ();
		using (var classBlock = builder.CreateBlock ($"static partial class {SymbolName}", true)) {
			foreach (var (name, path) in filteredLibs.OrderBy (v => v.LibraryName,
						 StringComparer.Ordinal)) {
				var className = name.Replace (".", string.Empty);
				using (var nestedClass =
					   classBlock.CreateBlock ($"static public class {className}", true)) {
					if (name == "__Internal") {
						nestedClass.WriteLine ("static public readonly IntPtr Handle = Dlfcn.dlopen (null, 0);");
					} else if (context.IsSystemLibrary (name)) {
						nestedClass.WriteLine (
							$"static public readonly IntPtr Handle = Dlfcn._dlopen (Constants.{name}Library, 0);");
					} else {
						nestedClass.WriteLine (
							$"static public readonly IntPtr Handle = Dlfcn.dlopen (\"{path}\", 0);");
					}
				}

				added = true;
			}
		}

		return added;
	}
}
