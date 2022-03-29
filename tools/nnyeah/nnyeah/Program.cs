using System;
using System.IO;
using System.Reflection;
using Mono.Options;
using System.Collections.Generic;

#nullable enable

namespace nnyeah {
    class Program {
        static void Main (string [] args)
        {
            var doHelp = false;
            string? infile = null, outfile = null;
            var verbose = false;
            var suppressWarnings = false;
            var warnings = new List<string> ();
            var transforms = new List<string> ();

            var options = new OptionSet () {
                { "h|?|help", o => doHelp = true },
                { "i=|input=", f => infile = f },
                { "o=|output=", f => outfile = f },
                { "v|verbose", o => verbose = true },
                { "s|suppress-warnings", o => suppressWarnings = true },
            };

            var extra = options.Parse (args);

            if (doHelp || infile is null || outfile is null) {
                PrintOptions (options, Console.Out);
                Environment.Exit (0);
			}

            using var stm = new FileStream (infile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var reworker = new Reworker (stm);

            try {
                reworker.Load ();
            } catch (Exception e) {
                Console.Error.WriteLine ($"Unable to read module from file {infile}: {e.Message}.");
                Environment.Exit (1);
			}

            reworker.WarningIssued += (s, e) => warnings.Add (e.HelpfulMessage ());
            reworker.Transformed += (s, e) => warnings.Add (e.HelpfulMessage ());

            if (reworker.NeedsReworking ()) {
                try {
                    using var ostm = new FileStream (outfile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                    reworker.Rework (ostm);
                    if (verbose) {
                        transforms.ForEach (Console.WriteLine);
                    }
                    if (!suppressWarnings) {
                        warnings.ForEach (Console.WriteLine);
                    }
                } catch (Exception e) {
                    Console.Error.Write ($"Unable to generate output file, unexpected exception: {e.Message}");
                    Environment.Exit (1);
				}
            } else {
                if (verbose) {
                    Console.WriteLine ("Package does not need changes - no output generated.");
				}
			}
        }

        static void PrintOptions (OptionSet options, TextWriter writer)
		{
            var location = Assembly.GetEntryAssembly ()?.Location;
            string exeName = (location is not null) ? Path.GetFileName (location) : "";
            writer.WriteLine ($"Usage:");
            writer.WriteLine ($"\t{exeName} [options] -o=output-directory -module-name=ModuleName");
            writer.WriteLine ($"\t{exeName} --demangle symbol [symbol...]");
            writer.WriteLine ("Options:");
            options.WriteOptionDescriptions (writer);
        }
    }
}    
