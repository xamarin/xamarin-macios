//
// Driver.cs: a preprocessor frontend to mcs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2013-2015 Xamarin Inc. All rights reserved.

using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;

#if !WINDOWS_BUILD
using Mono.CompilerServices.SymbolWriter;
#endif

namespace Xamarin.Pmcs
{
	static class Driver
	{
		static Settings settings;

		static int Main (string [] args)
		{
			settings = new Settings ();

			try {
				settings.Parse (args);
			} catch (Mono.Options.OptionException e) {
				Console.Error.WriteLine ("pmcs Option Error: {0}: {1}", e.OptionName, e.Message);
				Console.Error.WriteLine ();
				settings.Usage (Console.Error, showLogo: false);
				return -1;
			}

			if (settings.Verbose) {
				Console.Error.WriteLine ("pmcs settings:");
				Console.Error.WriteLine (settings);
				Console.Error.WriteLine ();
			}

			if (settings.ShowHelp) {
				settings.Usage (Console.Error);
				return -1;
			}

			if (settings.CleanMode) {
				Clean ();
				return 0;
			}

			return Run ();
		}

		static int Run ()
		{
			var tempPrefix = settings.Profile.Name;
			if (String.IsNullOrEmpty (tempPrefix))
				tempPrefix = Guid.NewGuid ().ToString ();
			tempPrefix = "~.pmcs-" + tempPrefix + ".";

			var ppSources = new List<string> ();
			string ppSourcesFile = null;
			if (settings.AssemblyOutFile != null)
				ppSourcesFile = Path.Combine (
					Path.GetDirectoryName (settings.AssemblyOutFile),
					"." + Path.GetFileName (settings.AssemblyOutFile) + tempPrefix + "sources"
				);

			var preprocessor = new Preprocessor {
				Profile = settings.Profile
			};

			var startTime = DateTime.Now;
			int preprocessCount = 0;

			Action<string> preprocessFile = path => {
				var fileName = Path.GetFileName (path);
				if (settings.Profile.IgnorePaths.Contains (fileName)) {
					if (settings.Verbose) {
						Console.Error.WriteLine ("pmcs: skipping {0}", fileName);
					}
					lock (ppSources)
						ppSources.Add (path);
					return;
				}

				var outPath = Path.Combine (Path.GetDirectoryName (path), tempPrefix + fileName);
				var writer = settings.PreprocessToStdout
					? Console.Out
					: new StreamWriter (outPath) as TextWriter;

				using (var reader = new StreamReader (path)) {
					if (!settings.OmitLineDirective)
						writer.WriteLine ("#line 1 \"" + Path.GetFullPath (path) + "\"");
					preprocessor.Preprocess (path, reader, writer);
				}

				if (writer != Console.Out) {
					writer.Dispose ();
				}

				if (!settings.PreprocessToStdout) {
					if (settings.PreprocessInPlace) {
						lock (ppSources)
							ppSources.Add (path);
						File.Delete (path);
						File.Move (outPath, path);
					} else {
						lock (ppSources)
							ppSources.Add (outPath);
					}
				}

				preprocessCount++;
			};

			if (settings.PreprocessToStdout)
				settings.CompilerPaths.ForEach (preprocessFile);
			else
				Parallel.ForEach (settings.CompilerPaths, preprocessFile);

			if (!settings.PreprocessToStdout && ppSourcesFile != null) {
				using (var sourcesWriter = new StreamWriter (ppSourcesFile)) {
					foreach (var path in ppSources) {
						sourcesWriter.WriteLine (path);
					}
				}
			}

			if (settings.Verbose || settings.Time) {
				Console.Error.WriteLine ("{0} / {1} sources preprocessed in {2}s",
					preprocessCount, settings.CompilerPaths.Count, (DateTime.Now - startTime).TotalSeconds);
			}

			if (ppSourcesFile != null)
				settings.Profile.CompilerOptions.Add ("@" + ppSourcesFile);

			var exit = 0;

			if (!settings.SkipCompilation && !settings.PreprocessToStdout) {
				var proc = Process.Start (new ProcessStartInfo {
					FileName = settings.Profile.CompilerExecutable,
					Arguments = String.Join (" ", settings.Profile.CompilerOptions),
					WorkingDirectory = Environment.CurrentDirectory,
					UseShellExecute = false
				});

				proc.WaitForExit ();
				exit = proc.ExitCode;
			}

			var mdbFile = settings.AssemblyOutFile + ".mdb";
			if (!settings.SkipMdbRebase && File.Exists (mdbFile))
				MdbRebase (mdbFile, tempPrefix);

			if (settings.KeepPreprocessedFiles || settings.PreprocessToStdout)
				return exit;

			if (ppSourcesFile != null)
				File.Delete (ppSourcesFile);

			foreach (var path in ppSources) {
				if (Path.GetFileName (path).StartsWith (tempPrefix, StringComparison.Ordinal)) {
					File.Delete (path);
				}
			}

			return exit;
		}

		static void MdbRebase (string mdbFile, string toRemove)
		{
#if WINDOWS_BUILD
			Console.Error.WriteLine ("Warning: skipping MDB rebasing of {0} (not supported on Windows)", mdbFile);
#else
			using (var input = MonoSymbolFile.ReadSymbolFile (mdbFile)) {
				var output = new MonoSymbolFile ();

				foreach (var source in input.Sources) {
					source.FileName = Path.Combine (
						Path.GetDirectoryName (source.FileName),
						Path.GetFileName (source.FileName).Replace (toRemove, String.Empty)
					);

					output.AddSource (source);
				}

				foreach (var compileUnit in input.CompileUnits) {
					compileUnit.ReadAll ();
					output.AddCompileUnit (compileUnit);
				}

				foreach (var method in input.Methods) {
					method.ReadAll ();
					output.AddMethod (method);
				}

				var tmpMdb = Path.GetTempFileName ();

				using (var stream = new FileStream (tmpMdb, FileMode.Create))
					output.CreateSymbolFile (input.Guid, stream);

				File.Delete (mdbFile);
				File.Move (tmpMdb, mdbFile);
			}
#endif
		}

		static void Clean ()
		{
			foreach (var file in Directory.GetFiles (Environment.CurrentDirectory, "*.pmcs*.sources")) {
				foreach (var source in File.ReadLines (file)) {
					File.Delete (source);
				}
				File.Delete (file);
			}
		}
	}
}
