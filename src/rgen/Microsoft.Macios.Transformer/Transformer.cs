// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Marille;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Extensions;
using Microsoft.Macios.Transformer.Extensions;
using Microsoft.Macios.Transformer.Workers;
using Serilog;
using Xamarin.Utils;

namespace Microsoft.Macios.Transformer;

/// <summary>
/// Main class that performs the transformation of the bindings. This class will set all the necessary wiring
/// to be able to process the different transformations per binding type.
/// </summary>
class Transformer {
	readonly static ILogger logger = Log.ForContext<Transformer> ();
	readonly string destinationDirectory;
	readonly ImmutableArray<(ApplePlatform Platform, Compilation Compilation)> compilations;
	readonly HashSet<string>? namespaceFilter;
	readonly Dictionary<string, ITransformer<(string Path, string SymbolName)>> transformers = new();

	internal Transformer (string destination,
		ImmutableArray<(ApplePlatform Platform, Compilation Compilation)> compilationsResult,
		IEnumerable<string>? namespaces = null)
	{
		destinationDirectory = destination;
		compilations = compilationsResult;
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
			logger.Information ("Created new topic '{TopicName}'", topicName);
		}

		return hub;
	}

	internal static string? SelectTopic (INamedTypeSymbol symbol)
	{
		// get the attrs, based on those return the correct topic to use
		var attrs = symbol.GetAttributeData ();
		logger.Debug ("Symbol '{SymbolName}' has [{Attributes}] attributes", symbol.Name,
			string.Join (", ", attrs.Keys));
		logger.Debug ("Symbol '{SymbolName}' kind is '{SymbolKind}'", symbol.Name, symbol.TypeKind);

		if (symbol.TypeKind == TypeKind.Enum) {
			// simplest case, an error domain	
			if (attrs.ContainsKey (AttributesNames.ErrorDomainAttribute)) {
				logger.Debug ("Symbol '{SymbolName}' is an error domain", symbol.Name);
				return nameof(ErrorDomainTransformer);
			}

			// in this case, we need to check if the enum is a smart enum. 
			// Smart enum: One of the enum members contains a FieldAttribute. Does NOT have to be ALL
			var enumMembers = symbol.GetMembers ().OfType<IFieldSymbol> ().ToArray ();
			foreach (var enumField in enumMembers) {
				var fieldAttrs = enumField.GetAttributeData ();
				if (fieldAttrs.ContainsKey (AttributesNames.FieldAttribute)) {
					logger.Debug ("Symbol '{SymbolName}' is a smart enum", symbol.Name);
					return nameof(SmartEnumTransformer);
				}
			}

			// we have either a native enum of a regular enum, we will use the copy worker
			logger.Debug ("Symbol '{SymbolName}' is a regular enum", symbol.Name);
			return nameof(CopyTransformer);
		}

		if (attrs.ContainsKey (AttributesNames.BaseTypeAttribute)) {
			// if can be a class or a protocol, check if the protocol attribute is present
			if (attrs.ContainsKey (AttributesNames.ProtocolAttribute) ||
			    attrs.ContainsKey (AttributesNames.ModelAttribute)) {
				logger.Debug ("Symbol '{SymbolName}' is a protocol", symbol.Name);
				return nameof(ProtocolTransformer);
			}

			if (attrs.ContainsKey (AttributesNames.CategoryAttribute)) {
				logger.Debug ("Symbol '{SymbolName}' is a category", symbol.Name);
				return nameof(CategoryTransformer);
			}

			logger.Debug ("Symbol '{SymbolName}' is a class", symbol.Name);
			return nameof(ClassTransformer);
		}

		if (attrs.ContainsKey (AttributesNames.StrongDictionaryAttribute)) {
			logger.Debug ("Symbol '{SymbolName}' is a strong dictionary", symbol.Name);
			return nameof(StrongDictionaryTransformer);
		}

		logger.Warning ("Symbol '{SymbolName}' could not be matched to a transformer", symbol.Name);
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
			logger.Information ("Skipping '{SymbolName}' because namespace it was not included in the transformation",
				symbol.Name, symbolNamespace);
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
		// TODO: this is a temporary nested loop to exercise the channels, this will be removed
		foreach (var (platform, compilation) in compilations) {
			foreach (var tree in compilation.SyntaxTrees) {
				var model = compilation.GetSemanticModel (tree);
				// the bindings have A LOT of interfaces, we cannot get a symbol for the entire tree
				var declarations = (await tree.GetRootAsync ())
					.DescendantNodes ()
					.OfType<BaseTypeDeclarationSyntax> ().ToArray ();

				logger.Debug ("Found '{Declarations}' interfaces in '{FilePath}'", declarations.Length, tree.FilePath);

				// loop over the declarations and send them to the hub
				foreach (var declaration in declarations) {
					var symbol = model.GetDeclaredSymbol (declaration);
					if (symbol is null) {
						// skip the transformation because the symbol is null
						logger.Warning ("Could not get the symbol for '{Declaration}'", declaration.Identifier);
						continue;
					}

					if (Skip (tree, symbol, out var outputDirectory)) {
						// matched the filter
						logger.Information ("Skipping '{SymbolName}' because it was filtered out", symbol.Name);
						continue;
					}

					// create the destination directory if needed, this is the only location we should be creating directories
					Directory.CreateDirectory (outputDirectory);

					var topicName = SelectTopic (symbol);
					if (topicName is not null && transformers.TryGetValue (topicName, out var transformer)) {
						await hub.PublishAsync (topicName, transformer.CreateMessage (tree, symbol));
						logger.Information ("Published '{SymbolName}' to '{TopicName}'", symbol.Name, topicName);
					}
				}
			}
		}

		// all messages have been sent, wait for them to be consumed, at that point all transformations have been
		// completed
		await hub.CloseAllAsync ();
	}

	static Task<CompilationResult>
		CreateCompilationAsync (ApplePlatform platform, string rspFile, string workingDirectory, string sdkDirectory)
		=> Task.Run (() => {
			logger.Debug ("Executing compilation for '{Platform}'", platform);
			return CreateCompilation (platform, rspFile, workingDirectory, sdkDirectory);
		});

	static CompilationResult CreateCompilation (
		ApplePlatform platform, string rspFile, string workingDirectory, string sdkDirectory)
	{
		// perform the compilation on a background thread, that way we can have several of them at the same time
		var parseResult = CSharpCommandLineParser.Default.ParseRsp (
			rspFile, workingDirectory, sdkDirectory);
		logger.Information ("Rsp file {RspFile} parsed with {ParseErrors} errors", rspFile, parseResult.Errors.Length);

		// add NET to the preprocessor directives
		var preprocessorDirectives = parseResult.ParseOptions.PreprocessorSymbolNames.ToList ();
		preprocessorDirectives.Add ("NET");

		// fixing the parsing options, we must have an issue in the rsp
		var updatedParseOptions = parseResult.ParseOptions
			.WithLanguageVersion (LanguageVersion.Latest)
			.WithPreprocessorSymbols (preprocessorDirectives)
			.WithDocumentationMode (DocumentationMode.None);

		var references = parseResult.GetReferences (workingDirectory, sdkDirectory);
		logger.Information ("References {References}", references.Length);
		var parsedSource = parseResult.GetSourceFiles (updatedParseOptions);
		logger.Information ("Parsed {Files} files", parsedSource.Length);

		var compilation = CSharpCompilation.Create (
			assemblyName: $"{parseResult.CompilationName}-transformer",
			syntaxTrees: parsedSource,
			references: references,
			options: parseResult.CompilationOptions);
		var errors = compilation.GetDiagnostics ()
			.Where (d => d.Severity == DiagnosticSeverity.Error)
			.ToImmutableArray ();
		return new (platform, compilation, errors);
	}

	public static async Task Execute (string destinationDirectory,
		List<(ApplePlatform Platform, string RspPath)> rspFiles, string workingDirectory,
		string sdkDirectory)
	{
		logger.Information ("Executing transformation");
		// use the async method to run several compilations in parallel, that way we do not block on eachother
		var compilationTasks = new List<Task<CompilationResult>> ();
		foreach (var (platform, rspPath) in rspFiles) {
			compilationTasks.Add (CreateCompilationAsync (platform, rspPath, workingDirectory, sdkDirectory));
		}

		var compilations = await Task.WhenAll (compilationTasks);

		// verify we have no errors in the compilations
		foreach (var (platform, api, errors) in compilations) {
			if (errors.Length == 0)
				continue;

			logger.Error ("Compilation failed from {Platform} with {ErrorCount} errors", platform, errors.Length);
			var sb = new StringBuilder ();
			foreach (var error in errors) {
				sb.AppendLine (error.ToString ());
			}

			throw new Exception (sb.ToString ());
		}

		ImmutableArray<(ApplePlatform Platform, Compilation Compilation)> compilationTuples = [
			..compilations
				.Select (c => c.ToTuple ())
		];
		// create a new transformer with the compilation result and the syntax trees
		var transformer = new Transformer (destinationDirectory, compilationTuples);
		await transformer.Execute ();
	}
}
