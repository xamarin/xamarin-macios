using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.Emitters;

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
			.Where (t => t.BindingAttributeFound)
			.Select ((t, _) => t.Declaration);

		context.RegisterSourceOutput (context.CompilationProvider.Combine (provider.Collect ()),
			((ctx, t) => GenerateCode (ctx, t.Left, t.Right)));
	}

	/// <summary>
	/// Generic method that can be used to filter/match a BaseTypeDeclarationSyntax with the BindingTypeAttribute.
	/// Because our generator is focused only on Enum, Interface and Class declarations we can use a generic method
	/// that will match the type + the presence of the attribute.
	/// </summary>
	/// <param name="context">Context used by the generator.</param>
	/// <typeparam name="T">The BaseTypeDeclarationSyntax we are interested in.</typeparam>
	/// <returns>A tuple that contains the BaseTypeDeclaration that was processed and a boolean that states if it should be processed or not.</returns>
	static (T Declaration, bool BindingAttributeFound) GetDeclarationForSourceGen<T> (GeneratorSyntaxContext context)
		where T : BaseTypeDeclarationSyntax
	{
		var classDeclarationSyntax = Unsafe.As<T> (context.Node);

		// Go through all attributes of the class.
		foreach (AttributeListSyntax attributeListSyntax in classDeclarationSyntax.AttributeLists)
			foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes) {
				if (context.SemanticModel.GetSymbolInfo (attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
					continue; // if we can't get the symbol, ignore it

				string attributeName = attributeSymbol.ContainingType.ToDisplayString ();

				// Check the full name of the [Binding] attribute.
				if (attributeName == AttributesNames.BindingAttribute)
					return (classDeclarationSyntax, true);
			}

		return (classDeclarationSyntax, false);
	}

	/// <summary>
	/// Collect the using statements from the class declaration root syntaxt tree and add them to the string builder
	/// that will be used to generate the code. This way we ensure that we have all the namespaces needed by the
	/// generated code.
	/// </summary>
	/// <param name="tree">Root syntax tree of the base type declaration.</param>
	/// <param name="sb">String builder that will be used for the generated code.</param>
	static void CollectUsingStatements (SyntaxTree tree, TabbedStringBuilder sb)
	{
		// collect all using from the syntax tree, add them to a hash to make sure that we don't have duplicates
		// and add those usings that we do know we need for bindings.
		var usingDirectives = tree.GetRoot ()
			.DescendantNodes ()
			.OfType<UsingDirectiveSyntax> ()
			.Select (d => d.Name.ToString ()).ToArray ();
		var usingDirectivesToKeep = new HashSet<string> (usingDirectives) {
			// add the using statements that we know we need and print them to the sb
		};
		foreach (var ns in usingDirectivesToKeep) {
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
	/// <param name="baseTypeDeclarations">The base type declarations marked by the BindingTypeAttribute.</param>
	/// <typeparam name="T">The type of type declaration.</typeparam>
	static void GenerateCode<T> (SourceProductionContext context, Compilation compilation,
		ImmutableArray<T> baseTypeDeclarations) where T : BaseTypeDeclarationSyntax
	{
		var rootContext = new RootBindingContext (compilation);
		foreach (var baseTypeDeclarationSyntax in baseTypeDeclarations) {
			var semanticModel = compilation.GetSemanticModel (baseTypeDeclarationSyntax.SyntaxTree);
			if (semanticModel.GetDeclaredSymbol (baseTypeDeclarationSyntax) is not INamedTypeSymbol namedTypeSymbol)
				continue;

			// init sb and add all the using statements from the base type declaration
			var sb = new TabbedStringBuilder (new ());
			// let people know this is generated code
			sb.AppendLine ("// <auto-generated>");

			// enable nullable!
			sb.AppendLine ();
			sb.AppendLine ("#nullable enable");
			sb.AppendLine ();

			CollectUsingStatements (baseTypeDeclarationSyntax.SyntaxTree, sb);


			// delegate semantic model and syntax tree analysis to the emitter who will generate the code and knows
			// best
			if (ContextFactory.TryCreate (rootContext, semanticModel, namedTypeSymbol, baseTypeDeclarationSyntax, out var symbolBindingContext)
				&& EmitterFactory.TryCreate (symbolBindingContext, sb, out var emitter)) {
				if (emitter.TryEmit (out var diagnostics)) {
					// only add file when we do generate code
					var code = sb.ToString ();
					context.AddSource ($"{emitter.SymbolName}.g.cs", SourceText.From (code, Encoding.UTF8));
				} else {
					// add to the diagnostics and continue to the next possible candidate
					foreach (Diagnostic diagnostic in diagnostics) {
						context.ReportDiagnostic (diagnostic);
					}
				}

			} else {
				// we don't have a emitter for this type, so we can't generate the code, add a diagnostic letting the
				// user we do not support what he is trying to do, this is a bug in the code generator and we
				// cannot recover from it. We do not want to crash but we generate no code.
				context.ReportDiagnostic (Diagnostic.Create (RBI0000,
					baseTypeDeclarationSyntax.GetLocation (),
					namedTypeSymbol.ToDisplayString ().Trim ()));
			}

		}
	}

}
