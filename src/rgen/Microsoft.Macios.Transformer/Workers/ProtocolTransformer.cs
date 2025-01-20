// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Marille;
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Transformer.Workers;

public class ProtocolTransformer (string destinationDirectory) : ITransformer<(string Path, string SymbolName)> {

	public bool UseBackgroundThread { get => true; }
	public Task ConsumeAsync ((string Path, string SymbolName) message, CancellationToken token = new ())
	{
		Console.WriteLine ($"ProtocolTransformer: Transforming class {message.SymbolName} for path {message.Path} to {destinationDirectory}");
		return Task.Delay (10);
	}

	public Task ConsumeAsync ((string Path, string SymbolName) message, Exception exception,
		CancellationToken token = new CancellationToken ())
	{
		return Task.CompletedTask;
	}

	public (string Path, string SymbolName) CreateMessage (SyntaxTree treeNode, ISymbol symbol)
	{
		return (treeNode.FilePath, symbol.Name);
	}

	public void Dispose () { }

	public ValueTask DisposeAsync () => ValueTask.CompletedTask;

}
