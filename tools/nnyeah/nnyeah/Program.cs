using System;
using System.IO;
using Mono.Options;
using System.Collections.Generic;
using Microsoft.MaciOS.Nnyeah.AssemblyComparator;
using Mono.Cecil;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.MaciOS.Nnyeah {
	class Program {
		static void Main (string [] args)
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

			if (infile is null || outfile is null) {
				ExitWithOptions (options, "File for conversion (--input) and output location (--output) are required options.");
			}

			// TODO - Long term this should default to files packaged within the tool but allow overrides
			if (xamarinAssembly is null || microsoftAssembly is null) {
				ExitWithOptions (options, "Support legacy and NET assemblies --xamarin-assembly and --microsoft-assembly are currently required options.");
			}

			if (doHelp) {
				ExitWithOptions (options);
			}

			if (!TryLoadTypeAndModuleMap (xamarinAssembly!, microsoftAssembly!, publicOnly: true,
				out var typeAndModuleMap, out var failureReason)) {
				Console.Error.WriteLine (Errors.E0011, failureReason);
			}
			ReworkFile (infile!, outfile!, verbose, forceOverwrite, suppressWarnings, typeAndModuleMap!);
		}

		static void ExitWithOptions (OptionSet options, string? message = null)
		{
			if (message is not null) {
				Console.Error.WriteLine (message);
			}
			PrintOptions (options, Console.Out);
			Environment.Exit (0);
		}

		static bool TryLoadTypeAndModuleMap (string earlier, string later, bool publicOnly,
			[NotNullWhen (returnValue: true)] out TypeAndMemberMap? result,
			[NotNullWhen (returnValue: false)] out string? reason)
		{
			try {
				using var ealierFile = TryOpenRead (earlier);
				using var laterFile = TryOpenRead (later);

				var earlierModule = ModuleDefinition.ReadModule (ealierFile);
				var laterModule = ModuleDefinition.ReadModule (laterFile);

				var comparingVisitor = new ComparingVisitor (earlierModule, laterModule, publicOnly);
				var map = new TypeAndMemberMap (laterModule);

				comparingVisitor.TypeEvents.NotFound += (s, e) => { map.TypesNotPresent.Add (e.Original); };
				comparingVisitor.TypeEvents.Found += (s, e) => { map.TypeMap.Add (e.Original, e.Mapped); };

				comparingVisitor.MethodEvents.NotFound += (s, e) => { map.MethodsNotPresent.Add (e.Original); };
				comparingVisitor.MethodEvents.Found += (s, e) => { map.MethodMap.Add (e.Original, e.Mapped); };

				comparingVisitor.FieldEvents.NotFound += (s, e) => { map.FieldsNotPresent.Add (e.Original); };
				comparingVisitor.FieldEvents.Found += (s, e) => { map.FieldMap.Add (e.Original, e.Mapped); };

				comparingVisitor.EventEvents.NotFound += (s, e) => { map.EventsNotPresent.Add (e.Original); };
				comparingVisitor.EventEvents.Found += (s, e) => { map.EventMap.Add (e.Original, e.Mapped); };

				comparingVisitor.PropertyEvents.NotFound += (s, e) => { map.PropertiesNotPresent.Add (e.Original); };
				comparingVisitor.PropertyEvents.Found += (s, e) => { map.PropertyMap.Add (e.Original, e.Mapped); };

				comparingVisitor.Visit ();
				result = map;
				reason = null;
				return true;
			} catch (Exception e) {
				result = null;
				reason = e.Message;
				return false;
			}
		}


		static void ReworkFile (string infile, string outfile, bool verbose, bool forceOverwrite,
			bool suppressWarnings, TypeAndMemberMap typeMap)
		{
			var warnings = new List<string> ();
			var transforms = new List<string> ();

			if (!File.Exists (infile)) {
				Console.Error.WriteLine (Errors.E0001, infile);
				Environment.Exit (1);
			}

			if (File.Exists (outfile) && !forceOverwrite) {
				Console.Error.WriteLine (Errors.E0002, outfile);
				Environment.Exit (1);
			}


			using var stm = new FileStream (infile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			var reworker = new Reworker (stm, typeMap);

			try {
				reworker.Load ();
			} catch (Exception e) {
				Console.Error.WriteLine (Errors.E0003, infile, e.Message);
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
				} catch (TypeNotFoundException e) {
					Console.Error.Write (Errors.E0012, e.TypeName);
					Environment.Exit (1);
				} catch (MemberNotFoundException e) {
					Console.Error.WriteLine (Errors.E0013, e.MemberName);
					Environment.Exit (1);
				} catch (Exception e) {
					Console.Error.WriteLine (Errors.E0004, e.Message);
					Environment.Exit (1);
				}
			} else {
				if (verbose) {
					Console.WriteLine (Errors.N0003);
				}
			}
		}

		static Stream TryOpenRead (string path)
		{
			try {
				var stm = new FileStream (path, FileMode.Open, FileAccess.Read);
				if (stm is null) {
					Console.Error.WriteLine (Errors.E0006, path, "");
					Environment.Exit (1);
				}
				return stm;
			} catch (Exception e) {
				Console.Error.WriteLine (Errors.E0006, path, e.Message);
				Environment.Exit (1);
			}
			return new MemoryStream (); // never reached, thanks code analysis
		}

		static void PrintOptions (OptionSet options, TextWriter writer)
		{
			options.WriteOptionDescriptions (writer);
			writer.WriteLine (Errors.N0007);
		}
	}
}
