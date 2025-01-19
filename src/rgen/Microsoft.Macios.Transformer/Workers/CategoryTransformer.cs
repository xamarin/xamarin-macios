// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Marille;

namespace Microsoft.Macios.Transformer.Workers;

public class CategoryTransformer (string destinationDirectory) : IWorker<(string Path, string SymbolName)>, IErrorWorker<(string Path, string Example)> {
	
	public Task ConsumeAsync ((string Path, string SymbolName) message, CancellationToken token = new ())
	{
		Console.WriteLine($"CategoryTransformer: Transforming class {message.SymbolName} for path {message.Path} to {destinationDirectory}");
		return Task.Delay (10);
	}
	
	public void Dispose () { }

	public ValueTask DisposeAsync () => ValueTask.CompletedTask;

	public Task ConsumeAsync ((string Path, string Example) message, Exception exception,
		CancellationToken token = new CancellationToken ())
	{
		return Task.CompletedTask;
	}

	public bool UseBackgroundThread { get => true; }
}
