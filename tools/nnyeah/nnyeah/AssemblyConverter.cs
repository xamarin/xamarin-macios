using System;
using System.IO;
using Mono.Options;
using System.Collections.Generic;
using Microsoft.MaciOS.Nnyeah.AssemblyComparator;
using Mono.Cecil;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.MaciOS.Nnyeah {
	// The standard entry point for assembly conversion
	public class AssemblyConverter {
		string XamarinAssembly;
		string MicrosoftAssembly;
		string Infile;
		string Outfile;
		bool Verbose;
		bool ForceOverwrite;
		bool SuppressWarnings;
		NNyeahAssemblyResolver Resolver;

		public static int Convert (string? xamarinAssembly, string microsoftAssembly, string infile, string outfile, bool verbose, bool forceOverwrite, bool suppressWarnings)
		{
			var converter = new AssemblyConverter (xamarinAssembly, microsoftAssembly, infile, outfile, verbose, forceOverwrite, suppressWarnings);
			return converter.Convert ();
		}

		AssemblyConverter (string? xamarinAssembly, string microsoftAssembly, string infile, string outfile, bool verbose, bool forceOverwrite, bool suppressWarnings)
		{
			if (!TryGetTargetPlatform (microsoftAssembly, out var platform)) {
				throw new ConversionException (Errors.E0018, Infile);
			}

			XamarinAssembly = xamarinAssembly ?? GetPlatformModulePath (platform.Value);
			MicrosoftAssembly = microsoftAssembly;
			Infile = infile;
			Outfile = outfile;
			Verbose = verbose;
			ForceOverwrite = forceOverwrite;
			SuppressWarnings = suppressWarnings;

			Resolver = new NNyeahAssemblyResolver (Infile, XamarinAssembly);
		}

		int Convert ()
		{
			try {
				var map = new TypeAndModuleMap (XamarinAssembly, MicrosoftAssembly, Resolver);
				ReworkFile (map);
				return 0;
			} catch (Exception e) {
				Console.Error.WriteLine (Errors.E0011, e.Message);
				return 1;
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
				reworker.Transformed += (_, e) => transforms.Add (e.HelpfulMessage ());

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
					var error = IsLikelyAnIntPtrConstructor (e.MemberName) ?
						Errors.E0017 : Errors.E0013;
					throw new ConversionException (error, e.MemberName);
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

		static bool TryGetTargetPlatform (string msAssembly, [NotNullWhen (returnValue: true)] out PlatformName? platform)
		{
			// we're using the name of the supplied microsoft assembly to get the target platform.
			// why?
			// initially I tried looking inside the input assembly but that is not reliable.
			// there were a number of cases where it would fail but we lack the context here to handle
			// it gracefully. Instead, it's much more reliable to assume that the microsoft assembly and
			// the input assembly are going to be in sync and ensure that the legacy assembly will match that.
			var file = Path.GetFileNameWithoutExtension (msAssembly);
			if (file.EndsWith (".iOS", StringComparison.OrdinalIgnoreCase)) {
				platform = PlatformName.iOS;
				return true;
			} else if (file.EndsWith (".macOS", StringComparison.InvariantCultureIgnoreCase)) {
				platform = PlatformName.macOS;
				return true;
			}
			platform = null;
			return false;
		}

		static string GetPlatformModulePath (PlatformName platform)
		{
			var path = platform switch {
				PlatformName.iOS => "/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.iOS/Xamarin.iOS.dll",
				PlatformName.macOS => "/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono/Xamarin.Mac/Xamarin.Mac.dll",
				_ => throw new NotSupportedException ()
			};
			if (!File.Exists (path)) {
				throw new ConversionException (Errors.E0019, path);
			}
			return path;
		}

		static bool IsLikelyAnIntPtrConstructor (string signature)
		{
			return signature.Contains ("::.ctor(System.IntPtr");
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
				switch (e.Original.ToString ()) {
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
