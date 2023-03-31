using System.IO;
using Mono.Options;
using System.Xml.Linq;

#nullable enable

namespace ClassRedirector {
	public class Program {
		public static int Main (string [] args)
		{
			try {
				return Main2 (args);
			} catch (Exception e) {
				Console.Error.WriteLine (e);
				return 1;
			}
		}

		public static int Main2 (string [] args)
		{
			var doHelp = false;
			string inputDirectory = "";
			var options = new OptionSet () {
				{ "h|?|help", o => doHelp = true },
				{ "i=|input-directory=", d => inputDirectory = d },
			};

			if (doHelp) {
				options.WriteOptionDescriptions (Console.Out);
				Console.WriteLine ($"This program takes an input directory and looks for the file '{StaticRegistrarFile.Name}'.");
				Console.WriteLine ("Upon finding it, it uses that as a map for finding all C# classes defined in dll's in the");
				Console.WriteLine ("directory and rewrites the classes' definition and usage of class_ptr to be more");
				Console.WriteLine ("efficient and, if possible, to no longer need a static constructor.");
				Console.WriteLine ("This program also requires that the directory contains one of the Microsoft platform dlls.");
				Console.WriteLine ("Classes in the directory are modified in place.");
				return 0;
			}

			if (String.IsNullOrEmpty (inputDirectory)) {
				throw new Exception ($"input-directory is required");
			}

			if (!Directory.Exists (inputDirectory)) {
				throw new Exception ($"input-directory {inputDirectory} does not exist.");
			}

			if (!DirectoryIsWritable (inputDirectory)) {
				throw new Exception ($"input-directory {inputDirectory} is not writable");
			}

			var registrarFile = Path.Combine (inputDirectory, StaticRegistrarFile.Name);
			if (!File.Exists (registrarFile)) {
				throw new Exception ($"map file {registrarFile} does not exist.");
			}

			var dllsToProcess = CollectDlls (inputDirectory);
			var xamarinDll = FindXamarinDll (dllsToProcess);

			if (xamarinDll is null)
				throw new Exception ($"unable to find platform dll in {inputDirectory}");

			var map = ReadRegistrarFile (registrarFile);

			var rewriter = new Rewriter (map, xamarinDll, dllsToProcess);
			rewriter.Process ();

			return 0;
		}

		static bool DirectoryIsWritable (string path)
		{
			var info = new DirectoryInfo (path);
			return !info.Attributes.HasFlag (FileAttributes.ReadOnly);
		}

		static string [] CollectDlls (string dir)
		{
			return Directory.GetFiles (dir, "*.dll"); // GetFiles returns full paths
		}

		static string [] xamarinDlls = new string [] {
			"Microsoft.iOS.dll",
			"Microsoft.macOS.dll",
			"Microsoft.tvOS.dll",
		};

		static bool IsXamarinDll (string p)
		{
			return xamarinDlls.FirstOrDefault (dll => p.EndsWith (dll, StringComparison.Ordinal)) is not null;
		}

		static string? FindXamarinDll (string [] paths)
		{
			return paths.FirstOrDefault (IsXamarinDll);
		}

		static CSToObjCMap ReadRegistrarFile (string path)
		{
			var doc = XDocument.Load (path);
			var map = CSToObjCMap.FromXDocument (doc);
			if (map is null)
				throw new Exception ($"Unable to read static registrar map file {path}");
			return map;
		}
	}
}