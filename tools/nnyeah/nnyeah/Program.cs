using System;

using Mono.Options;

namespace Microsoft.MaciOS.Nnyeah {
	public class Program {
		static int Main (string [] args)
		{
			try {
				return Main2 (args);
			} catch (ConversionException e) {
				Console.Error.WriteLine (e.Message);
				return 1;
			} catch (Exception e) {
				Console.Error.WriteLine (Errors.N0008);
				Console.Error.WriteLine (e);
				return 1;
			}
		}

		static int Main2 (string [] args)
		{
			var doHelp = false;
			string? infile = null, outfile = null;
			var verbose = false;
			var forceOverwrite = false;
			var suppressWarnings = false;
			string? xamarinAssembly = null, microsoftAssembly = null;

			var options = new OptionSet () {
				{ "h|?|help", o => doHelp = true },
				{ "i=|input=", f => infile = f },
				{ "o=|output=", f => outfile = f },
				{ "v|verbose", o => verbose = true },
				{ "f|force-overwrite", o => forceOverwrite = true },
				{ "s|suppress-warnings", o => suppressWarnings = true },
				{ "x|xamarin-assembly=", f => xamarinAssembly = f },
				{ "m|microsoft-assembly=", f => microsoftAssembly = f },
			};

			try {
				var extra = options.Parse (args);
			} catch {
				doHelp = true;
			}

			if (doHelp) {
				options.WriteOptionDescriptions (Console.Out);
				Console.Out.WriteLine (Errors.N0007);
				return 0;
			}

			if (infile is null || outfile is null) {
				throw new ConversionException (Errors.E0014);
			}

			// TODO - Long term this should default to files packaged within the tool but allow overrides
			if (xamarinAssembly is null || microsoftAssembly is null) {
				throw new ConversionException (Errors.E0015);
			}

			return AssemblyConverter.Convert (xamarinAssembly, microsoftAssembly!, infile!, outfile!, verbose, forceOverwrite, suppressWarnings);
		}
	}

	public class ConversionException : Exception {
		public ConversionException (string message) : base (message)
		{
		}

		public ConversionException (string message, params object? [] args) : base (string.Format (message, args))
		{
		}
	}
}
