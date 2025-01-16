// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.CommandLine;

namespace Microsoft.Macios.Transformer;

public class TransformCommand : Command {

	public TransformCommand () : base ("rgen-transform", "command to convert outdated bindings to be rgen compatible")
	{
		var input = new Option<string> (["--input", "-i"], "input directory to search for bgen bindings") {
			//single file support?
			IsRequired = true
		};
		AddOption (input);

		var output = new Option<string> (["--output", "-o"], "output directory to write rgen bindings") {
			IsRequired = true
		};
		AddOption (output);

		AddValidator (result => {
			if (!Directory.Exists (result.GetValueForOption (input))) {
				result.ErrorMessage = "Input directory does not exist";
			}
			if (!Directory.Exists (result.GetValueForOption (output))) {
				Directory.CreateDirectory (result.GetValueForOption (output)!);
			}
		});
		// this.AddOption (new Option<bool> (new string [] { "--verbose", "-v" }, "verbose output"));

		this.SetHandler (Execute);
	}

	public async Task Execute ()
	{
		// placeholder for loading/parsing/transforming/writing components
		await Task.CompletedTask;
	}
}
