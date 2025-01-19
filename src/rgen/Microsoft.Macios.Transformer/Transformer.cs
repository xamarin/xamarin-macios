// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Text;
using Marille;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Extensions;
using Microsoft.Macios.Transformer.Extensions;
using Microsoft.Macios.Transformer.Workers;

namespace Microsoft.Macios.Transformer;

/// <summary>
/// Main class that performs the transformation of the bindings. This class will set all the necessary wiring
/// to be able to process the different transformations per binding type.
/// </summary>
class Transformer {
	readonly string destinationDirectory;
	readonly Compilation compilation;
	readonly HashSet<string>? namespaceFilter;
	readonly Dictionary<string, ITransformer<(string Path, string SymbolName)>> transformers = new ();

	internal Transformer (string destination, Compilation compilationResult, IEnumerable<string>? namespaces = null)
	{
		destinationDirectory = destination;
		compilation = compilationResult;
		if (namespaces is not null)
			namespaceFilter = new HashSet<string> (namespaces);

		ITransformer<(string Path, string SymbolName)> [] defaultTransformers = [
			new CategoryTransformer (destinationDirectory),
			new ClassTransformer (destinationDirectory),
			new ProtocolTransformer (destinationDirectory),
			new SmartEnumTransformer (destinationDirectory),
			new StrongDictionaryTransformer (destinationDirectory),
			new CopyTransformer (destinationDirectory),
			new ErrorDomainTransformer (destinationDirectory),
		];
		// init the dict of transformers to access them via the name of the class
		foreach (var transformer in defaultTransformers) {
			transformers.Add (transformer.GetType ().Name, transformer);
		}
	}

	internal async Task<Hub> CreateHub ()
	{
		// create the hub that will manage the channels
		var hub = new Hub ();

		// use as many threads as the system allows
		var configuration = new TopicConfiguration { Mode = ChannelDeliveryMode.AtLeastOnceSync };
		foreach (var (topicName, transformer) in transformers) {
			await hub.CreateAsync (topicName, configuration, transformer, transformer);
		}

		return hub;
	}

	internal static string? SelectTopic (INamedTypeSymbol symbol)
	{
		// get the attrs, based on those return the correct topic to use
		var attrs = symbol.GetAttributeData ();
		if (symbol.TypeKind == TypeKind.Enum) {
			// simplest case, an error domain	
			if (attrs.ContainsKey (AttributesNames.ErrorDomainAttribute))
				return nameof(ErrorDomainTransformer);

			// in this case, we need to check if the enum is a smart enum. 
			// Smart enum: One of the enum members contains a FieldAttribute. Does NOT have to be ALL
			var enumMembers = symbol.GetMembers ().OfType<IFieldSymbol> ().ToArray ();
			foreach (var enumField in enumMembers) {
				var fieldAttrs = enumField.GetAttributeData ();
				if (fieldAttrs.ContainsKey (AttributesNames.FieldAttribute)) {
					return nameof(SmartEnumTransformer);
				}
			}

			// we have either a native enum of a regular enum, we will use the copy worker
			return nameof(CopyTransformer);
		}
		
		if (attrs.ContainsKey (AttributesNames.BaseTypeAttribute)) {
			// if can be a class or a protocol, check if the protocol attribute is present
			if (attrs.ContainsKey (AttributesNames.ProtocolAttribute) ||
				attrs.ContainsKey (AttributesNames.ModelAttribute)) 
				return nameof(ProtocolTransformer);
			if (attrs.ContainsKey (AttributesNames.CategoryAttribute))
				return nameof(CategoryTransformer);
			return nameof(ClassTransformer);
		}
		
		if (attrs.ContainsKey (AttributesNames.StrongDictionaryAttribute))
			return nameof(StrongDictionaryTransformer);

		return null;
	}

	internal bool Skip (SyntaxTree syntaxTree, ISymbol symbol, [NotNullWhen (false)] out string? outputDirectory)
	{
		outputDirectory = null;
		var symbolNamespace = symbol.ContainingNamespace.ToString ();
		if (symbolNamespace is null)
			// skip we could not retrieve the namespace
			return true;

		if (namespaceFilter is not null && !namespaceFilter.Contains (symbolNamespace)) {
			// TODO we could do this better by looking at the tree
			Console.WriteLine (
				$"Skipping {symbol.Name} because namespace {symbolNamespace} was not included in the transformation");
			// filtered out
			return true;
		}
		outputDirectory = Path.Combine (destinationDirectory, symbolNamespace);
		// If the syntax tree comes from the output directory, we skip it because this is a manual binding
		return syntaxTree.FilePath.StartsWith (outputDirectory);
	}

	internal async Task Execute ()
	{
		// few things to do here:
		// 1. Ignore the syntax trees that either have a path ending with the extension of the generator since that
		//    is generated code.
		// 2. Ignore the syntax trees that are not from the namespace if that filter was added. The simplest way
		//    to get the namespace is to get the symbol that is related to the tree, since we are doing that
		//    operation, we keep the symbol and use it to create our data mode.
		// 3. Create the data model for the transformation.
		// 4. Base on the data model push the data model object to a channel to be consumed.

		// create the hub that will manage the channels
		var hub = await CreateHub ();

		// with the hub created, loop over the syntax trees and create the messages to be sent to the hub
		foreach (var tree in compilation.SyntaxTrees) {
			var model = compilation.GetSemanticModel (tree);
			// the bindings have A LOT of interfaces, we cannot get a symbol for the entire tree
			var declarations = (await tree.GetRootAsync ())
				.DescendantNodes ()
				.OfType<BaseTypeDeclarationSyntax> ().ToArray ();

			Console.WriteLine ($"Found {declarations.Length} interfaces in {tree.FilePath}");

			// loop over the declarations and send them to the hub
			foreach (var declaration in declarations) {
				var symbol = model.GetDeclaredSymbol (declaration);
				if (symbol is null) {
					// skip the transformation because the symbol is null
					continue;
				}

				if (Skip (tree, symbol, out var outputDirectory))
					// matched the filter
					continue;

				// create the destination directory if needed, this is the only location we should be creating directories
				Directory.CreateDirectory (outputDirectory);

				var topicName = SelectTopic (symbol);
				if (topicName is not null && transformers.TryGetValue (topicName, out var transformer)) {
					await hub.PublishAsync (topicName, transformer.CreateMessage (tree, symbol));
				}
			}
		}

		// all messages have been sent, wait for them to be consumed, at that point all transformations have been
		// completed
		await hub.CloseAllAsync ();
	}

	public static Task Execute (string destinationDirectory, string rspFile, string workingDirectory,
		string sdkDirectory)
	{
		Console.WriteLine ("Executing transformation");
		// the transformation works as follows. We first need to parse the rsp file to create a compilation
		// to do so we relay on the csharp compiler, there is not much we really need to do. If the parsing 
		// is wrong, we throw an exception. 
		var parseResult = CSharpCommandLineParser.Default.ParseRsp (
			rspFile, workingDirectory, sdkDirectory);

		// add NET to the preprocessor directives
		var preprocessorDirectives = parseResult.ParseOptions.PreprocessorSymbolNames.ToList ();
		preprocessorDirectives.Add ("NET");

		// fixing the parsing options, we must have an issue in the rsp
		var updatedParseOptions = parseResult.ParseOptions
			.WithLanguageVersion (LanguageVersion.Latest)
			.WithPreprocessorSymbols (preprocessorDirectives)
			.WithDocumentationMode (DocumentationMode.None);

		var references = parseResult.GetReferences (workingDirectory, sdkDirectory);
		var parsedSource = parseResult.GetSourceFiles (updatedParseOptions);

		var compilation = CSharpCompilation.Create (
			assemblyName: $"{parseResult.CompilationName}-transformer",
			syntaxTrees: parsedSource,
			references: references,
			options: parseResult.CompilationOptions);

		var diagnostics = compilation.GetDiagnostics ();
		Console.WriteLine ($"Diagnostics length {diagnostics.Length}");
		// collect all the compilation errors, ignoring the warnings, if any error is found, we throw an exception
		var errors = diagnostics.Where (d => d.Severity == DiagnosticSeverity.Error).ToArray ();
		if (errors.Length > 0) {
			var sb = new StringBuilder ();
			foreach (var resultError in errors) {
				sb.AppendLine ($"{resultError}");
			}
			throw new Exception ($"Error during workspace compilation: {sb}");
		}

		// create a new transformer with the compilation result and the syntax trees
		var transformer = new Transformer (destinationDirectory, compilation);
		return transformer.Execute ();
	}
}
