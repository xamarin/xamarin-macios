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

namespace Xamarin.Bundler {

	public interface IFileEnumerator
	{
		IEnumerable<string> Files { get; }
		string RootDir { get; }
	}

	public class FileSystemEnumerator : IFileEnumerator
	{
		DirectoryInfo Info;
		public IEnumerable<string> Files => Info.GetFiles ().Select (x => x.FullName);
		public string RootDir { get; private set; }

		public FileSystemEnumerator (string path)
		{
			RootDir = path;
			Info = new DirectoryInfo (path);
		}
	}

	public delegate int RunCommandDelegate (string path, string args, string[] env = null, StringBuilder output = null, bool suppressPrintOnErrors = false);

	public class AOTCompiler
	{
		enum AotType {
			Default,
			None,
			All,
			SDK,
			Explicit
		}

		AotType aotType = AotType.Default;
		public bool IsAOT => aotType != AotType.Default && aotType != AotType.None; 

		// Set to Key -> True when we've seen a given include/exclude during compile to catch errors
		Dictionary <string, bool> includedAssemblies = new Dictionary <string, bool> ();
		Dictionary <string, bool> excludedAssemblies = new Dictionary <string, bool> ();

		// Allows tests to stub out actual compilation and parallelism
		public RunCommandDelegate RunCommand { get; set; } = Driver.RunCommand; 
		public ParallelOptions ParallelOptions { get; set; } = new ParallelOptions () { MaxDegreeOfParallelism = Driver.Concurrency };

		public string Quote (string f) => Driver.Quote (f);

		public void Parse (string options)
		{
			// Syntax - all,sdk,none or "" if explicit then optional list of +/-'ed assemblies
			// Sections seperated by ,
			foreach (var option in options.Split (',')) {
				switch (option) {
				case "none":
					aotType = AotType.None;
					continue;
				case "all":
					aotType = AotType.All;
					continue;
				case "sdk":
					aotType = AotType.SDK;
					continue;
				}

				if (option.StartsWith ("+", StringComparison.Ordinal)) {
					if (aotType == AotType.Default)
						aotType = AotType.Explicit;
					includedAssemblies.Add (option.Substring (1), false);
					continue;
				}
				if (option.StartsWith ("-", StringComparison.Ordinal)) {
					if (aotType == AotType.Default)
						aotType = AotType.Explicit;
					excludedAssemblies.Add (option.Substring (1), false);
					continue;
				}
				throw new MonoMacException (20, true, "The valid options for '{0}' are '{1}'.", "--aot", "none, all, sdk, and an explicit list of assemblies.");
			}
			if (aotType == AotType.Default)
				throw new MonoMacException (20, true, "The valid options for '{0}' are '{1}'.", "--aot", "none, all, sdk, and an explicit list of assemblies.");
		}

		List<string> GetFilesToAOT (IFileEnumerator files)
		{
			var aotFiles = new List<string> ();
			foreach (var file in files.Files) {
				string fileName = Path.GetFileName (file);
				string extension = Path.GetExtension (file);
				if (extension != ".exe" && extension != ".dll")
					continue;

				if (excludedAssemblies.ContainsKey (fileName)) {
					excludedAssemblies[fileName] = true;
					continue;
				}

				if (includedAssemblies.ContainsKey (fileName)) {
					includedAssemblies[fileName] = true;
					aotFiles.Add (file);
					continue;
				}

				switch (aotType) {
				case AotType.All:
					aotFiles.Add (file);
					break;
				case AotType.SDK:
					if (fileName == "Xamarin.Mac.dll" || fileName == "System.dll" || fileName == "mscorlib.dll")
						aotFiles.Add (file);
					break;
				case AotType.Explicit:
					break; // In explicit, only included includedAssemblies included
				default:
					throw ErrorHelper.CreateError (0099, "Internal error \"GetFilesToAOT with aot: {0}\" Please file a bug report with a test case (http://bugzilla.xamarin.com).", aotType);
				}
			}

			var unusedIncludes = includedAssemblies.Where (pair => !pair.Value).Select (pair => pair.Key).ToList ();
			if (unusedIncludes.Count > 0)
				throw ErrorHelper.CreateError (3009, "AOT of '{0}' was requested but was not found", String.Join (" ", unusedIncludes));

			var unusedExcludes = excludedAssemblies.Where (pair => !pair.Value).Select (pair => pair.Key).ToList ();
			if (unusedExcludes.Count > 0)
				throw ErrorHelper.CreateError (3010, "Exclusion of AOT of '{0}' was requested but was not found", String.Join (" ", unusedExcludes));

			return aotFiles;
		}

		public void Compile (string path)
		{
			Compile (new FileSystemEnumerator (path));
		}

		public void Compile (IFileEnumerator files)
		{
			if (!IsAOT)
				throw ErrorHelper.CreateError (0099, "Internal error \"AOTBundle with aot: {0}\" Please file a bug report with a test case (http://bugzilla.xamarin.com).", aotType);

			const string MonoExePath = "/Library/Frameworks/Xamarin.Mac.framework/Commands/bmac-mobile-mono";
			var monoEnv = new string [] {"MONO_PATH", files.RootDir };

			Parallel.ForEach (GetFilesToAOT (files), ParallelOptions, file => {
				if (RunCommand (MonoExePath, String.Format ("--aot=hybrid {0}", Quote (file)), monoEnv) != 0)
					throw ErrorHelper.CreateError (3001, "Could not AOT the assembly '{0}'", file);
			});
		}
	}
}
