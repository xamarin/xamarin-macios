// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.DataModel;
using Microsoft.Macios.Generator.Emitters;
using Microsoft.Macios.Generator.Extensions;

namespace Microsoft.Macios.Generator;

/// <summary>
/// A sample source generator that creates a custom report based on class properties. The target class should be
/// annotated with the 'Generators.ReportAttribute' attribute.
/// When using the source code as a baseline, an incremental source generator is preferable because it reduces
/// the performance overhead.
/// </summary>
[Generator]
public class BindingSourceGeneratorGenerator : IIncrementalGenerator {
	static readonly BindingEqualityComparer equalityComparer = new ();

	/// <inheritdoc cref="IIncrementalGenerator"/>
	public void Initialize (IncrementalGeneratorInitializationContext context)
	{
		// Add the binding generator attributes to the compilation. This are only available when the
		// generator is used, similar to how bgen works.
		foreach ((string fileName, string content) in ExtraSources.Sources) {
			context.RegisterPostInitializationOutput (ctx => ctx.AddSource (
				fileName, SourceText.From (content, Encoding.UTF8)));
		}

		// our bindings are special. Since we write shared code in the Library.g.cs and the Trampolines.g.cs
		// we need to listen to all the BaseTypeDeclarationSyntax changes. We do so, generate a data model with the
		// changes we are interested, and later we transform them. This allows use to be able to use a RootBindingContext
		// as a bag in which we can add information about libraries and trampolines needed by the bindings.
		var provider = context.SyntaxProvider
			.CreateSyntaxProvider (static (node, _) => IsValidNode (node),
				static (ctx, _) => GetChangesForSourceGen (ctx))
			.Where (tuple => tuple.BindingAttributeFound);

		var bindings = provider
			.Select (static (tuple, _) => tuple.Bindings)
			.WithComparer (equalityComparer);

		// ideally we could do a distinct, because each code change can return the same libs, this makes the library
		// generation more common than what we would like, but it is the smallest code generation.
		var libraryProvider = provider
			.Select ((tuple, _) => (tuple.RootBindingContext, tuple.Bindings.LibraryPaths));

		context.RegisterSourceOutput (context.CompilationProvider.Combine (bindings.Collect ()),
			((ctx, t) => GenerateCode (ctx, t.Right)));

		context.RegisterSourceOutput (context.CompilationProvider.Combine (libraryProvider.Collect ()),
			((ctx, t) => GenerateLibraryCode (ctx, t.Right)));
	}

	/// <summary>
	/// Returns if the node is a valid node for the binding generator.
	/// </summary>
	/// <param name="node">Node modified by the user.</param>
	/// <returns>True if the binding generator should consider the node for code generation.</returns>
	static bool IsValidNode (SyntaxNode node) => node switch {
		EnumDeclarationSyntax or ClassDeclarationSyntax or InterfaceDeclarationSyntax => true,
		_ => false,
	};

	static (RootContext RootBindingContext, Binding Bindings, bool BindingAttributeFound) GetChangesForSourceGen (GeneratorSyntaxContext context)
	{
		var bindingContext = new RootContext (context.SemanticModel);
		// we do know that the context node has to be one of the base type declarations
		var declarationSyntax = Unsafe.As<BaseTypeDeclarationSyntax> (context.Node);

		// check if we do have the binding attr, else there nothing to retrieve
		bool isBindingType = declarationSyntax.HasAtLeastOneAttribute (context.SemanticModel, AttributesNames.BindingTypes);

		if (!isBindingType) {
			// return empty data + false
			return (bindingContext, default, false);
		}

		var binding = Binding.FromDeclaration (declarationSyntax, bindingContext);
		// if code changes are null, return the default value and a false to later ignore the change
		return binding is not null
			? (bindingContext, binding.Value, isBindingType)
			: (bindingContext, default, false);
	}

	static void GenerateCode (SourceProductionContext context, in ImmutableArray<Binding> bindingsList)
	{
		// The process is as follows, get all the changes we have received from the incremental generator,
		// loop over them, and based on the CodeChange.BindingType we are going to build the symbol context
		// and emitter. Those are later used to generate the code.
		//
		// Once all the enums, classes and interfaces have been processed, we will use the data collected
		// in the RootBindingContext to generate the library and trampoline code.
		var sb = new TabbedStringBuilder (new ());
		foreach (var binding in bindingsList) {
			// init sb and add the header
			sb.Clear ();
			sb.WriteHeader ();
			if (EmitterFactory.TryCreate (binding, out var emitter)) {
				// write the using statements
				CollectUsingStatements (binding, sb, emitter);

				var bindingContext = new BindingContext (sb, binding);
				if (emitter.TryEmit (bindingContext, out var diagnostics)) {
					// only add a file when we do generate code
					var code = sb.ToString ();
					var namespacePath = Path.Combine (binding.Namespace.ToArray ());
					var fileName = emitter.GetSymbolName (binding);
					context.AddSource ($"{Path.Combine (namespacePath, fileName)}.g.cs",
						SourceText.From (code, Encoding.UTF8));
				} else {
					// add to the diagnostics and continue to the next possible candidate
					context.ReportDiagnostics (diagnostics);
				}
			} else {
				// we don't have an emitter for this type, so we can't generate the code, add a diagnostic letting the
				// user we do not support what they are trying to do
				context.ReportDiagnostic (Diagnostic.Create (
					Diagnostics
						.RBI0000, // An unexpected error ocurred while processing '{0}'. Please fill a bug report at https://github.com/xamarin/xamarin-macios/issues/new.
					null,
					binding.FullyQualifiedSymbol));
			}
		}
	}

	/// <summary>
	/// Code generator that emits the static classes that contain the pointers to the library used
	/// by the binding. This is a single generated file.
	/// </summary>
	/// <param name="context">Source production context.</param>
	/// <param name="libraryChanges">The root context of the current generation.</param>
	static void GenerateLibraryCode (SourceProductionContext context,
		ImmutableArray<(RootContext RootBindingContext, IEnumerable<(string LibraryName, string? LibraryPath)> LibraryPaths)> libraryChanges)
	{
		if (libraryChanges.Length == 0)
			return;
		// we have at least one, we can get the root binding changes from it
		var rootBindingContext = libraryChanges [0].RootBindingContext;
		var sb = new TabbedStringBuilder (new ());
		sb.WriteHeader ();

		// Each code change might have returned the same list of libraries, we need to get the distinct ones
		var libComparer = new LibraryPathsComparer ();
		var distinctLibraryPaths = libraryChanges
			.SelectMany (library => library.LibraryPaths)
			.Distinct (libComparer)
			.ToImmutableArray ();

		// no need to collect the using statements, this file is completely generated
		var emitter = new LibraryEmitter (rootBindingContext, sb);

		if (emitter.TryEmit (distinctLibraryPaths, out var diagnostics)) {
			// only add a file when we do generate code
			var code = sb.ToString ();
			context.AddSource ($"{Path.Combine (emitter.SymbolNamespace, emitter.SymbolName)}.g.cs",
				SourceText.From (code, Encoding.UTF8));
		} else {
			// add to the diagnostics and continue to the next possible candidate
			context.ReportDiagnostics (diagnostics);
		}
	}

	/// <summary>
	/// Collect the using statements from the named ype code changes and add them to the string builder
	/// that will be used to generate the code. This way we ensure that we have all the namespaces needed by the
	/// generated code.
	/// </summary>
	/// <param name="binding">The code changes for a given named type.</param>
	/// <param name="sb">String builder that will be used for the generated code.</param>
	/// <param name="emitter">The emitter that will generate the code. Provides any extra needed namespace.</param>
	static void CollectUsingStatements (in Binding binding, TabbedStringBuilder sb, ICodeEmitter emitter)
	{
		// collect all using from the syntax tree, add them to a hash to make sure that we don't have duplicates
		// and add those usings that we do know we need for bindings.
		var usingDirectivesToKeep = new SortedSet<string> (binding.UsingDirectives) {
			// add the using statements that we know we need and print them to the sb
		};

		// add those using statements needed by the emitter
		foreach (var ns in emitter.UsingStatements) {
			usingDirectivesToKeep.Add (ns);
		}

		// add them sorted so that we have testeable generated code
		foreach (var ns in usingDirectivesToKeep) {
			if (string.IsNullOrEmpty (ns))
				continue;
			sb.AppendLine ($"using {ns};");
		}
	}
}
