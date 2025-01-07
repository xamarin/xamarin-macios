using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Microsoft.Macios.Transformer;


//TODO: logging infra
// add cli header stuff
var parser = new CommandLineBuilder (new TransformCommand ())
			.UseDefaults ()
			.Build ();

return await parser.InvokeAsync (args).ConfigureAwait (false);
