// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text;
using Marille;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Transformer.Extensions;
using Microsoft.Macios.Transformer.Workers;

namespace Microsoft.Macios.Transformer;

/// <summary>
/// Main class that performs the transformation of the bindings. This class will set all the necessary wiring
/// to be able to process the different transformations per binding type.
/// </summary>
class Transformer {
	string destinationDirectory;
	readonly Compilation compilation;
	readonly ImmutableArray<SyntaxTree> sources;
	HashSet<string>? namespaceFilter;

	internal Transformer (string destination, Compilation compilationResult, ImmutableArray<SyntaxTree> syntaxTrees,
		IEnumerable<string>? namespaces = null)
	{
		destinationDirectory = destination;
		compilation = compilationResult;
		sources = syntaxTrees;
		if (namespaces is not null)
			namespaceFilter = new HashSet<string> (namespaces);
	}

	internal async Task<Hub> CreateHub ()
	{
		// create the hub that will manage the channels
		var hub = new Hub ();

		// use as many threads as the system allows
		var topicConfiguration = new TopicConfiguration { Mode = ChannelDeliveryMode.AtLeastOnceSync };

		// create the channels, because the app is alreay asyc there is nothing to deal with threads.
		var categories = new CategoryTransformer (destinationDirectory);
		await hub.CreateAsync (nameof (CategoryTransformer), topicConfiguration, categories, categories);

		var classes = new ClassTransformer (destinationDirectory);
		await hub.CreateAsync (nameof (ClassTransformer), topicConfiguration, classes, classes);

		var protocols = new ProtocolTransformer (destinationDirectory);
		await hub.CreateAsync (nameof (ProtocolTransformer), topicConfiguration, protocols, protocols);

		var smartEnums = new SmartEnumTransformer (destinationDirectory);
		await hub.CreateAsync (nameof (SmartEnumTransformer), topicConfiguration, smartEnums, smartEnums);

		var strongDictionaries = new StrongDictionaryTransformer (destinationDirectory);
		await hub.CreateAsync (nameof (StrongDictionaryTransformer), topicConfiguration, strongDictionaries, strongDictionaries);

		return hub;
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
		foreach (var tree in sources) {
			var model = compilation.GetSemanticModel (tree);
			// the bindings have A LOT of interfaces, we cannot get a symbol for the entire tree
			var declarations = tree.GetRoot ()
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

				var namespaceName = symbol.ContainingNamespace.ToString ();
				if (namespaceName is null)
					// skip we could not retrieve the namespace
					continue;

				if (namespaceFilter is not null && !namespaceFilter.Contains (namespaceName)) {
					// TODO we could do this better by looking at the tree
					Console.WriteLine ($"Skipping {symbol.Name} because namespace {namespaceName} was not included in the transformation");
					// filtered out
					continue;
				}

				// create the destination directory if needed, this is the only location we should be creating directories
				var currentDirectory = Path.Combine (destinationDirectory, namespaceName);
				Directory.CreateDirectory (currentDirectory);

				var message = (tree.FilePath, symbol.Name);
				// push the message to the hub, we are doing this to all channels, this will need to be filered in the 
				// future
				Console.WriteLine ($"Publishing message to all channels: {message}");
				await hub.PublishAsync (nameof (CategoryTransformer), message);
				await hub.PublishAsync (nameof (ClassTransformer), message);
				await hub.PublishAsync (nameof (ProtocolTransformer), message);
				await hub.PublishAsync (nameof (SmartEnumTransformer), message);
				await hub.PublishAsync (nameof (StrongDictionaryTransformer), message);
			}
		}

		// all messages have been sent, wait for them to be consumed, at that point all transformations have been
		// completed
		await hub.CloseAllAsync ();
	}


	public static Task Execute (string destinationDirectory, string rspFile, string workingDirectory, string sdkDirectory)
	{
		Console.WriteLine ("Executing transformation");
		// the transformation works as follows. We first need to parse the rsp file to create a compilation
		// to do so we relay on the csharp compiler, there is not much we really need to do. If the parsing 
		// is wrong, we throw an exception. 
		var parseResult = CSharpCommandLineParser.Default.ParseRsp (
			rspFile, workingDirectory, sdkDirectory);

		// add NET to the preprocessor directives
		var preprocesorDirectives = parseResult.ParseOptions.PreprocessorSymbolNames.ToList ();
		preprocesorDirectives.Add ("NET");

		// fixing the parsing options, we must have an issue in the rsp
		var updatedParseOptions = parseResult.ParseOptions
			.WithLanguageVersion (LanguageVersion.Latest)
			.WithPreprocessorSymbols (preprocesorDirectives)
			.WithDocumentationMode (DocumentationMode.None);

		var references = parseResult.GetReferences (workingDirectory, sdkDirectory);
		var parsedSource = parseResult.GetSourceFiles (updatedParseOptions);

		var compilation = CSharpCompilation.Create (
			assemblyName: $"{parseResult.CompilationName}-transformer",
			syntaxTrees: parsedSource,
			references: references,
			options: parseResult.CompilationOptions);

		var diagnostics = compilation.GetDiagnostics ();
		Console.WriteLine ($"Diganostics legth {diagnostics.Length}");
		// collect all the compilation errors, ignoring the warnings, if any error is found, we throw an exception
		var errors = diagnostics.Where (d => d.Severity == DiagnosticSeverity.Error).ToArray ();
		if (errors.Length > 0) {
			var sb = new StringBuilder ();
			foreach (var resultError in errors) {
				sb.AppendLine ($"{resultError}");
			}
			Console.WriteLine (sb);
			throw new Exception ($"Error during workspace compilation: {sb}");
		}

		// create a new transformer with the compilation result and the syntax trees
		var transformer = new Transformer (destinationDirectory, compilation, parsedSource);
		return transformer.Execute ();
	}
}
