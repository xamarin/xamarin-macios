using System;
using System.IO;
using Mono.Options;
using System.Collections.Generic;
using Microsoft.MaciOS.Nnyeah.AssemblyComparator;
using Mono.Cecil;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.MaciOS.Nnyeah {
	// The standard entry point for assembly conversion
	public class AssemblyConverter
	{
		string XamarinAssembly;
		string MicrosoftAssembly;
		string Infile;
		string Outfile;
		bool Verbose;
		bool ForceOverwrite;
		bool SuppressWarnings;
		NNyeahAssemblyResolver Resolver;

		public static void Convert (string xamarinAssembly, string microsoftAssembly, string infile, string outfile, bool verbose, bool forceOverwrite, bool suppressWarnings)
		{
			var converter = new AssemblyConverter (xamarinAssembly, microsoftAssembly, infile, outfile, verbose, forceOverwrite, suppressWarnings);
			converter.Convert ();
		}

		AssemblyConverter (string xamarinAssembly, string microsoftAssembly, string infile, string outfile, bool verbose, bool forceOverwrite, bool suppressWarnings)
		{
			XamarinAssembly = xamarinAssembly;
			MicrosoftAssembly = microsoftAssembly;
			Infile = infile;
			Outfile = outfile;
			Verbose = verbose;
			ForceOverwrite = forceOverwrite;
			SuppressWarnings = suppressWarnings;

			Resolver = new NNyeahAssemblyResolver (Infile, XamarinAssembly);
		}

		void Convert ()
		{
			try {
				var map = new TypeAndModuleMap (XamarinAssembly, MicrosoftAssembly, Resolver);
				ReworkFile (map);
			}
			catch (Exception e) {
				Console.Error.WriteLine (Errors.E0011, e.Message);
			}
		}

		void ReworkFile (TypeAndModuleMap map)
		{
			var warnings = new List<string> ();
			var transforms = new List<string> ();

			if (!File.Exists (Infile)) {
				throw new ConversionException (Errors.E0001, Infile);
			}

			if (File.Exists (Outfile) && !ForceOverwrite) {
				throw new ConversionException (Errors.E0002, Outfile);
			}

			if (CreateReworker (map) is Reworker reworker) {
				reworker.WarningIssued += (_, e) => warnings.Add (e.HelpfulMessage ());
				reworker.Transformed += (_, e) => warnings.Add (e.HelpfulMessage ());

				try {
					using var ostm = new FileStream (Outfile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
					reworker.Rework (ostm);
					if (Verbose) {
						transforms.ForEach (Console.WriteLine);
					}
					if (!SuppressWarnings) {
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
				Console.WriteLine (Errors.N0003);
			}
		}

		Reworker? CreateReworker (TypeAndModuleMap map)
		{
			try {
				var stm = new FileStream (Infile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				var module = ModuleDefinition.ReadModule (stm, new ReaderParameters { AssemblyResolver = Resolver });
				var moduleContainer = new ModuleContainer (module, map.XamarinModule, map.MicrosoftModule);

				return Reworker.CreateReworker (stm, moduleContainer, map.TypeMap);
			} catch (Exception e) {
				throw new ConversionException (Errors.E0003, Infile, e.Message);
			}
		}
	}

	public class TypeAndModuleMap {
		public TypeAndMemberMap TypeMap;
		public ModuleDefinition XamarinModule;
		public ModuleDefinition MicrosoftModule;

		public TypeAndModuleMap (string infile, string outfile, NNyeahAssemblyResolver resolver)
		{        
			using var inFileStream = TryOpenRead (infile);
			using var outFileStream = TryOpenRead (outfile);

			XamarinModule = ModuleDefinition.ReadModule (inFileStream, new ReaderParameters { AssemblyResolver = resolver });
			MicrosoftModule = ModuleDefinition.ReadModule (outFileStream, new ReaderParameters { AssemblyResolver = resolver });

			var comparingVisitor = new ComparingVisitor (XamarinModule, MicrosoftModule, resolver, true);
			TypeMap = new TypeAndMemberMap ();

			comparingVisitor.TypeEvents.NotFound += (_, e) => { 
				switch (e.Original.ToString()) {
					case "System.nint":
					case "System.nuint":
					case "System.nfloat":
						break;
					case null:
						throw new InvalidOperationException ("Null NotFound type event");
					default:
						TypeMap.TypesNotPresent.Add (e.Original);
						break;
				}
			};
			comparingVisitor.TypeEvents.Found += (s, e) => { TypeMap.TypeMap.Add (e.Original, e.Mapped); };

			comparingVisitor.MethodEvents.NotFound += (s, e) => { TypeMap.MethodsNotPresent.Add (e.Original); };
			comparingVisitor.MethodEvents.Found += (s, e) => { TypeMap.MethodMap.Add (e.Original, e.Mapped); };

			comparingVisitor.FieldEvents.NotFound += (s, e) => { TypeMap.FieldsNotPresent.Add (e.Original); };
			comparingVisitor.FieldEvents.Found += (s, e) => { TypeMap.FieldMap.Add (e.Original, e.Mapped); };

			comparingVisitor.EventEvents.NotFound += (s, e) => { TypeMap.EventsNotPresent.Add (e.Original); };
			comparingVisitor.EventEvents.Found += (s, e) => { TypeMap.EventMap.Add (e.Original, e.Mapped); };

			comparingVisitor.PropertyEvents.NotFound += (s, e) => { TypeMap.PropertiesNotPresent.Add (e.Original); };
			comparingVisitor.PropertyEvents.Found += (s, e) => { TypeMap.PropertyMap.Add (e.Original, e.Mapped); };

			comparingVisitor.Visit ();
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
	}
}
