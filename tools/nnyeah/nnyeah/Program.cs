using System;
using System.IO;
using Mono.Options;
using System.Collections.Generic;
using Microsoft.MaciOS.Nnyeah.AssemblyComparator;
using Mono.Cecil;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.MaciOS.Nnyeah {
	public class ConversionException : Exception {
		public ConversionException (string message) : base (message)
		{
		}

		public ConversionException (string message, params object? [] args) : base (string.Format(message, args))
		{
		}
	}

	public class Program {
		static int Main (string [] args)
		{
			try {
				Main2 (args);
				return 0;
			}
			catch (ConversionException e) {
				Console.Error.WriteLine (e.Message);
				return 1;
			} catch (Exception e) {
				Console.Error.WriteLine (Errors.N0008);
				Console.Error.WriteLine (e);
				return 1;
			}
		}

		static void Main2 (string [] args)
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
				throw new ConversionException (Errors.E0014);
			}

			// TODO - Long term this should default to files packaged within the tool but allow overrides
			if (xamarinAssembly is null || microsoftAssembly is null) {
				throw new ConversionException (Errors.E0015);
			}

			if (doHelp) {
				PrintOptions (options, Console.Out);
			}
			else {
				ProcessAssembly (xamarinAssembly!, microsoftAssembly!, infile!, outfile!, verbose, forceOverwrite, suppressWarnings);
			}
		}

		public static void ProcessAssembly (string xamarinAssembly,
			string microsoftAssembly, string infile, string outfile, bool verbose,
			bool forceOverwrite, bool suppressWarnings)
		{
			if (!TryLoadTypeAndModuleMap (xamarinAssembly!, microsoftAssembly!, publicOnly: true,
				out var typeAndModuleMap, out var failureReason, out var xamarinModule,
				out var microsoftModule)) {
				Console.Error.WriteLine (Errors.E0011, failureReason);
			}
			ReworkFile (infile!, outfile!, verbose, forceOverwrite, suppressWarnings, typeAndModuleMap!,
				xamarinModule!, microsoftModule!);
		}

		static bool TryLoadTypeAndModuleMap (string earlier, string later, bool publicOnly,
			[NotNullWhen (returnValue: true)] out TypeAndMemberMap? result,
			[NotNullWhen (returnValue: false)] out string? reason,
			[NotNullWhen (returnValue: true)] out ModuleDefinition? xamarinModule,
			[NotNullWhen (returnValue: true)] out ModuleDefinition? microsoftModule)
		{
			try {
				using var ealierFile = TryOpenRead (earlier);
				using var laterFile = TryOpenRead (later);

				var earlierModule = ModuleDefinition.ReadModule (ealierFile);
				var laterModule = ModuleDefinition.ReadModule (laterFile);

				var comparingVisitor = new ComparingVisitor (earlierModule, laterModule, publicOnly);
				var map = new TypeAndMemberMap ();

				comparingVisitor.TypeEvents.NotFound += (_, e) => { 
					switch (e.Original.ToString()) {
						case "System.nint":
						case "System.nuint":
						case "System.nfloat":
							break;
						case null:
							throw new InvalidOperationException ("Null NotFound type event");
						default:
							map.TypesNotPresent.Add (e.Original);
							break;
					}
				};
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
				xamarinModule = earlierModule;
				microsoftModule = laterModule;
				return true;
			} catch (Exception e) {
				result = null;
				reason = e.Message;
				xamarinModule = null;
				microsoftModule = null;
				return false;
			}
		}

		static Reworker? CreateReworker (string infile, TypeAndMemberMap typeMap,
			ModuleDefinition xamarinModule, ModuleDefinition microsoftModule)
		{
			try {
				var stm = new FileStream (infile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				var module = ModuleDefinition.ReadModule (stm);
				var moduleContainer = new ModuleContainer (module, xamarinModule, microsoftModule);

				return Reworker.CreateReworker (stm, moduleContainer, typeMap);
			} catch (Exception e) {
				throw new ConversionException (Errors.E0003, infile, e.Message);
			}
		}


		static void ReworkFile (string infile, string outfile, bool verbose, bool forceOverwrite,
			bool suppressWarnings, TypeAndMemberMap typeMap, ModuleDefinition xamarinModule,
			ModuleDefinition microsoftModule)
		{
			var warnings = new List<string> ();
			var transforms = new List<string> ();

			if (!File.Exists (infile)) {
				throw new ConversionException (Errors.E0001, infile);
			}

			if (File.Exists (outfile) && !forceOverwrite) {
				throw new ConversionException (Errors.E0002, outfile);
			}

			if (CreateReworker (infile, typeMap, xamarinModule, microsoftModule) is Reworker reworker) {
				reworker.WarningIssued += (_, e) => warnings.Add (e.HelpfulMessage ());
				reworker.Transformed += (_, e) => warnings.Add (e.HelpfulMessage ());

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
					throw new ConversionException (Errors.E0012, e.TypeName);
				} catch (MemberNotFoundException e) {
					throw new ConversionException (Errors.E0013, e.MemberName);
				} catch (Exception e) {
					throw new ConversionException (Errors.E0004, e.Message);
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
					throw new ConversionException (Errors.E0006, path, "");
				}
				return stm;
			} catch (Exception e) {
				throw new ConversionException (Errors.E0006, path, e.Message);
			}
		}

		static void PrintOptions (OptionSet options, TextWriter writer)
		{
			options.WriteOptionDescriptions (writer);
			writer.WriteLine (Errors.N0007);
		}
	}
}
