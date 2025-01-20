// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Marille;
using Microsoft.CodeAnalysis;
using Serilog;

namespace Microsoft.Macios.Transformer.Workers;

public class SmartEnumTransformer (string destinationDirectory) : ITransformer<(string Path, string SymbolName)> {

	readonly static ILogger logger = Log.ForContext<SmartEnumTransformer> ();
	public bool UseBackgroundThread { get => true; }

	public Task ConsumeAsync ((string Path, string SymbolName) message, CancellationToken token = new ())
	{
		logger.Information ("Transforming {SymbolName} for path {Path} to {DestinationDirectory}",
			message.SymbolName, message.Path, destinationDirectory);
		return Task.Delay (10);
	}

	public Task ConsumeAsync ((string Path, string SymbolName) message, Exception exception,
		CancellationToken token = new CancellationToken ())
	{
		logger.Error (exception, "Error transforming {SymbolName} for path {Path} to {DestinationDirectory}:",
			message.SymbolName, message.Path, destinationDirectory);
		return Task.CompletedTask;
	}

	public (string Path, string SymbolName) CreateMessage (SyntaxTree treeNode, ISymbol symbol)
	{
		return (treeNode.FilePath, symbol.Name);
	}

	public void Dispose () { }

	public ValueTask DisposeAsync () => ValueTask.CompletedTask;

}
