/*
 * Copyright 2016 Microsoft Inc
 *
 * Authors:
 *   Chris Hamons <chris.hamons@xamarin.com> 
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Utils;

using Profile = Mono.Tuner.Profile;

namespace Xamarin.Bundler {

	public interface IFileEnumerator {
		IEnumerable<string> Files { get; }
		string RootDir { get; }
	}

	public class FileSystemEnumerator : IFileEnumerator {
		DirectoryInfo Info;
		public IEnumerable<string> Files => Info.GetFiles ().Select (x => x.FullName);
		public string RootDir { get; private set; }

		public FileSystemEnumerator (string path)
		{
			RootDir = path;
			Info = new DirectoryInfo (path);
		}
	}

	public delegate int RunCommandDelegate (string path, IList<string> args, Dictionary<string, string> env = null, StringBuilder output = null, bool suppressPrintOnErrors = false);

	public enum AOTCompilerType {
		Invalid,
		Bundled64,
		System64,
	}

	public enum AOTCompilationType {
		Default,
		None,
		All,
		Core,
		SDK,
		Explicit
	}

	public enum AOTKind {
		Default,
		Standard,
		Hybrid
	}

	public class AOTOptions {
		public bool IsAOT => CompilationType != AOTCompilationType.Default && CompilationType != AOTCompilationType.None;
		public bool IsHybridAOT => IsAOT && Kind == AOTKind.Hybrid;

		public AOTCompilationType CompilationType { get; private set; } = AOTCompilationType.Default;
		public AOTKind Kind { get; private set; } = AOTKind.Standard;

		public List<string> IncludedAssemblies { get; private set; } = new List<string> ();
		public List<string> ExcludedAssemblies { get; private set; } = new List<string> ();

		public AOTOptions (string options)
		{
			// Syntax - all,core,sdk,none or "" if explicit then optional list of +/-'ed assemblies
			// Sections seperated by ,
			string [] optionParts = options.Split (',');
			for (int i = 0; i < optionParts.Length; ++i) {
				string option = optionParts [i];

				AOTKind kind = AOTKind.Default;
				// Technically '|' is valid in a file name, so |hybrid.dll would be as well.
				// So check the left hand side for a valid option and pass if not
				if (option.Contains ("|")) {
					string [] optionTypeParts = option.Split ('|');
					if (optionTypeParts.Length != 2)
						throw new ProductException (20, true, Errors.MX0020, "--aot", "{none, all, core, sdk}{|hybrid}, then an optional explicit list of assemblies.");
					switch (optionTypeParts [0]) {
					case "none":
					case "core":
					case "sdk":
					case "all": {
						option = optionTypeParts [0];
						switch (optionTypeParts [1]) {
						case "hybrid":
							if (option != "all")
								throw new ProductException (114, true, Errors.MM0114);
							kind = AOTKind.Hybrid;
							break;
						case "standard":
							kind = AOTKind.Standard;
							break;
						default:
							throw new ProductException (20, true, Errors.MX0020, "--aot", "{none, all, core, sdk}{|hybrid}, then an optional explicit list of assemblies.");
						}
						break;
					}
					default:
						break;
					}
				}

				switch (option) {
				case "none":
					CompilationType = AOTCompilationType.None;
					if (kind != AOTKind.Default)
						Kind = kind;
					continue;
				case "all":
					CompilationType = AOTCompilationType.All;
					if (kind != AOTKind.Default)
						Kind = kind;
					continue;
				case "sdk":
					CompilationType = AOTCompilationType.SDK;
					if (kind != AOTKind.Default)
						Kind = kind;
					continue;
				case "core":
					CompilationType = AOTCompilationType.Core;
					if (kind != AOTKind.Default)
						Kind = kind;
					continue;
				}

				if (option.StartsWith ("+", StringComparison.Ordinal)) {
					if (CompilationType == AOTCompilationType.Default)
						CompilationType = AOTCompilationType.Explicit;
					IncludedAssemblies.Add (option.Substring (1));
					continue;
				}
				if (option.StartsWith ("-", StringComparison.Ordinal)) {
					if (CompilationType == AOTCompilationType.Default)
						CompilationType = AOTCompilationType.Explicit;
					ExcludedAssemblies.Add (option.Substring (1));
					continue;
				}
				throw new ProductException (20, true, Errors.MX0020, "--aot", "{none, all, core, sdk}{|hybrid}, then an optional explicit list of assemblies.");
			}
			if (CompilationType == AOTCompilationType.Default)
				throw new ProductException (20, true, Errors.MX0020, "--aot", "{none, all, core, sdk}{|hybrid}, then an optional explicit list of assemblies.");

		}
	}

	public class AOTCompiler {
		// Allows tests to stub out actual compilation and parallelism
		public RunCommandDelegate RunCommand { get; set; } = Driver.RunCommand;
		public ParallelOptions ParallelOptions { get; set; } = new ParallelOptions () { MaxDegreeOfParallelism = Driver.Concurrency };

		string xamarin_mac_prefix;
		public string XamarinMacPrefix {
			get {
				if (xamarin_mac_prefix is null)
					xamarin_mac_prefix = Driver.GetFrameworkCurrentDirectory (Driver.App);
				return xamarin_mac_prefix;
			}
			set {
				xamarin_mac_prefix = value;
			}
		}

		AOTOptions options;
		Abi [] abis;
		AOTCompilerType compilerType;
		bool IsRelease;
		bool IsModern;

		public AOTCompiler (AOTOptions options, IEnumerable<Abi> abis, AOTCompilerType compilerType, bool isModern, bool isRelease)
		{
			this.options = options;
			this.abis = abis.ToArray ();
			this.compilerType = compilerType;
			this.IsModern = isModern;
			this.IsRelease = isRelease;
		}

		public void Compile (string path)
		{
			Compile (new FileSystemEnumerator (path));
		}

		public void Compile (IFileEnumerator files)
		{
			if (!options.IsAOT)
				throw ErrorHelper.CreateError (0099, Errors.MX0099, $"\"AOTBundle with aot: {options.CompilationType}\" ");

			var monoEnv = new Dictionary<string, string> { { "MONO_PATH", files.RootDir } };
			List<string> filesToAOT = GetFilesToAOT (files);

			bool needsLipo = abis.Length > 1 && filesToAOT.Count > 0;
			string tempAotDir = needsLipo ? Path.GetDirectoryName (filesToAOT [0]) : null;
			if (needsLipo && Directory.Exists (tempAotDir)) {
				foreach (var abi in abis) {
					Directory.CreateDirectory (Path.Combine (tempAotDir, "aot", abi.AsArchString ()));
				}
			}

			Parallel.ForEach (filesToAOT.SelectMany (f => abis, (file, abi) => new Tuple<string, Abi> (file, abi)), ParallelOptions, tuple => {
				var file = tuple.Item1;
				var abi = tuple.Item2;

				var cmd = new List<string> ();
				var aotArgs = new List<string> ();
				aotArgs.Add ($"mtriple={abi.AsArchString ()}");
				if (options.IsHybridAOT)
					aotArgs.Add ("hybrid");
				if (needsLipo)
					aotArgs.Add ($"outfile={Path.Combine (tempAotDir, "aot", abi.AsArchString (), Path.GetFileName (file) + ".dylib")}");

				// check if needs to be removed: https://github.com/xamarin/xamarin-macios/issues/18693
				if (Driver.XcodeVersion.Major >= 15 && !Driver.App.DisableAutomaticLinkerSelection)
					aotArgs.Add ("ld-flags=-Xlinker -ld_classic");

				cmd.Add ($"--aot={string.Join (",", aotArgs)}");
				if (IsModern)
					cmd.Add ("--runtime=mobile");
				cmd.Add (file);
				if (RunCommand (GetMonoPath (abi), cmd, monoEnv) != 0)
					throw ErrorHelper.CreateError (3001, Errors.MX3001, "AOT", file);
			});

			// Lipo the result
			if (needsLipo) {
				Parallel.ForEach (filesToAOT, ParallelOptions, file => {
					string [] inputs = abis.Select (abi => Path.Combine (tempAotDir, "aot", abi.AsArchString (), Path.GetFileName (file) + ".dylib")).Where (File.Exists).ToArray ();
					string output = file + ".dylib";

					if (inputs.Length > 0)
						Driver.RunLipoAndCreateDsym (Driver.App, output, inputs);
				});
			}

			if (needsLipo && Directory.Exists (tempAotDir)) {
				Directory.Delete (Path.Combine (tempAotDir, "aot"), true);
			}

			if (IsRelease && options.IsHybridAOT) {
				Parallel.ForEach (filesToAOT, ParallelOptions, file => {
					if (RunCommand (StripCommand, new [] { file }) != 0)
						throw ErrorHelper.CreateError (3001, Errors.MX3001, "strip", file);
				});
			}

			if (IsRelease) {
				// mono --aot creates .dll.dylib.dSYM directories for each assembly AOTed
				// There isn't an easy was to disable this behavior
				// We move them (cheap) so they can be archived for release builds
				foreach (var file in filesToAOT) {
					var source = file + ".dylib.dSYM/";
					if (Directory.Exists (source)) {
						var dest = Path.GetFullPath (Path.Combine (source, "..", "..", "..", "..", Path.GetFileName (file) + ".dylib.dSYM/"));
						if (Directory.Exists (dest))
							Directory.Delete (dest, true);
						Directory.Move (source, dest);
					}
				}
			}
		}

		List<string> GetFilesToAOT (IFileEnumerator files)
		{
			// Make a dictionary of included/excluded files to track if we've missed some at the end
			Dictionary<string, bool> includedAssemblies = new Dictionary<string, bool> ();
			foreach (var item in options.IncludedAssemblies)
				includedAssemblies [item] = false;

			Dictionary<string, bool> excludedAssemblies = new Dictionary<string, bool> ();
			foreach (var item in options.ExcludedAssemblies)
				excludedAssemblies [item] = false;

			var aotFiles = new List<string> ();

			foreach (var file in files.Files) {
				string fileName = Path.GetFileName (file);
				string extension = Path.GetExtension (file);
				if (extension != ".exe" && extension != ".dll")
					continue;

				if (excludedAssemblies.ContainsKey (fileName)) {
					excludedAssemblies [fileName] = true;
					continue;
				}

				if (includedAssemblies.ContainsKey (fileName)) {
					includedAssemblies [fileName] = true;
					aotFiles.Add (file);
					continue;
				}

				switch (options.CompilationType) {
				case AOTCompilationType.All:
					aotFiles.Add (file);
					break;
				case AOTCompilationType.SDK:
					string fileNameNoExtension = Path.GetFileNameWithoutExtension (fileName);
					if (Profile.IsSdkAssembly (fileNameNoExtension) || fileName == "Xamarin.Mac.dll")
						aotFiles.Add (file);
					break;
				case AOTCompilationType.Core:
					if (fileName == "Xamarin.Mac.dll" || fileName == "System.dll" || fileName == "mscorlib.dll")
						aotFiles.Add (file);
					break;
				case AOTCompilationType.Explicit:
					break; // In explicit, only included includedAssemblies included
				default:
					throw ErrorHelper.CreateError (0099, Errors.MX0099, $"\"GetFilesToAOT with aot: {options.CompilationType}\"");
				}
			}

			var unusedIncludes = includedAssemblies.Where (pair => !pair.Value).Select (pair => pair.Key).ToList ();
			if (unusedIncludes.Count > 0)
				throw ErrorHelper.CreateError (3009, Errors.MM3009, String.Join (" ", unusedIncludes));

			var unusedExcludes = excludedAssemblies.Where (pair => !pair.Value).Select (pair => pair.Key).ToList ();
			if (unusedExcludes.Count > 0)
				throw ErrorHelper.CreateError (3010, Errors.MM3010, String.Join (" ", unusedExcludes));

			return aotFiles;
		}

		public const string StripCommand = "/Library/Frameworks/Mono.framework/Commands/mono-cil-strip";

		string GetMonoPath (Abi abi)
		{
			if (compilerType == AOTCompilerType.Bundled64) {
				switch (abi) {
				case Abi.ARM64:
					return Path.Combine (XamarinMacPrefix, "bin", "aarch64-darwin-mono-sgen");
				case Abi.x86_64:
					return Path.Combine (XamarinMacPrefix, "bin", "mono-sgen");
				default:
					throw ErrorHelper.CreateError (0099, Errors.MX0099, $"\"MonoPath with compilerType: {compilerType}\"");
				}
			} else if (compilerType == AOTCompilerType.System64 && abi == Abi.x86_64) {
				return "/Library/Frameworks/Mono.framework/Commands/mono64";
			} else {
				throw ErrorHelper.CreateError (0099, Errors.MX0099, $"\"MonoPath with compilerType: {compilerType}\"");
			}
		}
	}
}
