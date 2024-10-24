using System.Collections.Generic;
using System.Collections.Immutable;
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
	internal static readonly DiagnosticDescriptor RBI0000 = new (
		"RBI0000",
		new LocalizableResourceString (nameof (Resources.RBI0000Title), Resources.ResourceManager, typeof (Resources)),
		new LocalizableResourceString (nameof (Resources.RBI0000MessageFormat), Resources.ResourceManager, typeof (Resources)),
		"Usage",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: new LocalizableResourceString (nameof (Resources.RBI0000Description), Resources.ResourceManager, typeof (Resources))
	);
	
	static readonly CodeChangesComparer comparer = new ();
	/// <inheritdoc cref="IIncrementalGenerator"/>
	public void Initialize (IncrementalGeneratorInitializationContext context)
	{
		// Add the binding generator attributes to the compilation. This are only available when the
		// generator is used, similar to how bgen works.
		foreach ((string fileName, string content) in ExtraSources.Sources) {
			context.RegisterPostInitializationOutput (ctx => ctx.AddSource (
				fileName, SourceText.From (content, Encoding.UTF8)));
		}

		// binding can use different 'types'. To be able to generate the code we are going to add a different
		// function for each of the 'types' we are interested in generating that will be added to the compiler
		// pipeline
		AddPipeline<ClassDeclarationSyntax> (context);
		AddPipeline<InterfaceDeclarationSyntax> (context);
		AddPipeline<EnumDeclarationSyntax> (context);
	}

	/// <summary>
	/// Generic method that adds a provider and a code generator to the pipeline.
	/// </summary>
	/// <param name="context">The compilation context</param>
	/// <typeparam name="T">The base type declaration that we are going to generate.</typeparam>
	static void AddPipeline<T> (IncrementalGeneratorInitializationContext context) where T : BaseTypeDeclarationSyntax
	{
		var provider = context.SyntaxProvider
			.CreateSyntaxProvider (
				static (s, _) => s is T,
				(ctx, _) => GetDeclarationForSourceGen<T> (ctx))
			.Where (t => t.BindingAttributeFound) // get the types with the binding attr
			.Select ((t, _) => t.Changes)
			.WithComparer (comparer);

		context.RegisterSourceOutput (context.CompilationProvider.Combine (provider.Collect ()),
			((ctx, t) => GenerateCode<T> (ctx, t.Left, t.Right)));
	}

	/// <summary>
	/// Generic method that can be used to filter/match a BaseTypeDeclarationSyntax with the BindingTypeAttribute.
	/// Because our generator is focused only on Enum, Interface and Class declarations we can use a generic method
	/// that will match the type + the presence of the attribute.
	/// </summary>
	/// <param name="context">Context used by the generator.</param>
	/// <typeparam name="T">The BaseTypeDeclarationSyntax we are interested in.</typeparam>
	/// <returns>A tuple that contains the BaseTypeDeclaration that was processed and a boolean that states if it should be processed or not.</returns>
	static (CodeChanges Changes, bool BindingAttributeFound) GetDeclarationForSourceGen<T> (GeneratorSyntaxContext context)
		where T : BaseTypeDeclarationSyntax
	{
		// we do know that the context node has to be one of the base type declarations
		var declarationSyntax = Unsafe.As<T> (context.Node);

		// check if we do have the binding attr, else there nothing to retrieve
		bool isBindingType = declarationSyntax.HasAttribute (context.SemanticModel, AttributesNames.BindingAttribute);

		if (!isBindingType) {
			// return an empty data + false
			return (default, false);
		}

		var codeChanges = CodeChanges.FromDeclaration (context.SemanticModel, declarationSyntax);
		// if code changes are null, return the default value and a false to later ignore the change
		return codeChanges is not null ?
			(codeChanges.Value, isBindingType) : (default, false);
	}

	/// <summary>
	/// Collect the using statements from the class declaration root syntaxt tree and add them to the string builder
	/// that will be used to generate the code. This way we ensure that we have all the namespaces needed by the
	/// generated code.
	/// </summary>
	/// <param name="tree">Root syntax tree of the base type declaration.</param>
	/// <param name="sb">String builder that will be used for the generated code.</param>
	/// <param name="emitter">The emitter that will generate the code. Provides any extra needed namespace.</param>
	static void CollectUsingStatements (SyntaxTree tree, TabbedStringBuilder sb, ICodeEmitter emitter)
	{
		// collect all using from the syntax tree, add them to a hash to make sure that we don't have duplicates
		// and add those usings that we do know we need for bindings.
		var usingDirectives = tree.GetRoot ()
			.DescendantNodes ()
			.OfType<UsingDirectiveSyntax> ()
			.Select (d => d.Name!.ToString ()).ToArray ();
		var usingDirectivesToKeep = new HashSet<string> (usingDirectives) {
			// add the using statements that we know we need and print them to the sb
		};

		// add those using statements needed by the emitter
		foreach (var ns in emitter.UsingStatements) {
			usingDirectivesToKeep.Add (ns);
		}

		// add them sorted so that we have testeable generated code
		foreach (var ns in usingDirectivesToKeep.OrderBy (s => s)) {
			if (string.IsNullOrEmpty (ns))
				continue;
			sb.AppendLine ($"using {ns};");
		}
	}

	/// <summary>
	/// Generic method that allows to call a emitter for ta type that will emit the binding code. All code generation
	/// is very similar. Get create a tabbed string builder to write the code with the needed using statemens from
	/// the original syntax tree and we pass it to the emitter that will generate the code.
	/// </summary>
	/// <param name="context">The generator context.</param>
	/// <param name="compilation">The compilation unit.</param>
	/// <param name="changesList">The base type declarations marked by the BindingTypeAttribute.</param>
	/// <typeparam name="T">The type of type declaration.</typeparam>
	static void GenerateCode<T> (SourceProductionContext context, Compilation compilation,
		ImmutableArray<CodeChanges> changesList) where T : BaseTypeDeclarationSyntax
	{
		var rootContext = new RootBindingContext (compilation);
		foreach (var change in changesList) {
			var declaration = Unsafe.As<T> (change.SymbolDeclaration);
			var semanticModel = compilation.GetSemanticModel (declaration.SyntaxTree);
			// This is a bug in the roslyn analyzer for roslyn generator https://github.com/dotnet/roslyn-analyzers/issues/7436
#pragma warning disable RS1039
			if (semanticModel.GetDeclaredSymbol (declaration) is not INamedTypeSymbol namedTypeSymbol)
#pragma warning restore RS1039
				continue;

			// init sb and add all the using statements from the base type declaration
			var sb = new TabbedStringBuilder (new ());
			sb.WriteHeader ();

			// delegate semantic model and syntax tree analysis to the emitter who will generate the code and knows
			// best
			if (ContextFactory.TryCreate (rootContext, semanticModel, namedTypeSymbol, declaration,
					out var symbolBindingContext)
				&& EmitterFactory.TryCreate (symbolBindingContext, sb, out var emitter)) {
				CollectUsingStatements (change.SymbolDeclaration.SyntaxTree, sb, emitter);

				if (emitter.TryEmit (out var diagnostics)) {
					// only add file when we do generate code
					var code = sb.ToString ();
					context.AddSource ($"{symbolBindingContext.Namespace}/{emitter.SymbolName}.g.cs",
						SourceText.From (code, Encoding.UTF8));
				} else {
					// add to the diagnostics and continue to the next possible candidate
					foreach (Diagnostic diagnostic in diagnostics) {
						context.ReportDiagnostic (diagnostic);
					}
				}
			} else {
				// we don't have a emitter for this type, so we can't generate the code, add a diagnostic letting the
				// user we do not support what he is trying to do
				context.ReportDiagnostic (Diagnostic.Create (RBI0000,
					declaration.GetLocation (),
					namedTypeSymbol.ToDisplayString ().Trim ()));
			}
		}
	}
}
