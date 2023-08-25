using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Localization.MSBuild;
using Xamarin.Utils;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public abstract class AOTCompileTaskBase : XamarinTask {
		public ITaskItem [] AotArguments { get; set; } = Array.Empty<ITaskItem> ();

		[Required]
		public string AOTCompilerPath { get; set; } = string.Empty;

		[Required]
		public ITaskItem [] Assemblies { get; set; } = Array.Empty<ITaskItem> ();

		[Required]
		public string InputDirectory { get; set; } = string.Empty;

		[Required]
		public string MinimumOSVersion { get; set; } = string.Empty;

		[Required]
		public string OutputDirectory { get; set; } = string.Empty;

		[Required]
		public string SdkDevPath { get; set; } = string.Empty;

		#region Output
		[Output]
		public ITaskItem []? AssemblyFiles { get; set; }

		[Output]
		public ITaskItem []? FileWrites { get; set; }
		#endregion

		public override bool Execute ()
		{
			var inputs = new List<string> (Assemblies.Length);
			for (var i = 0; i < Assemblies.Length; i++) {
				inputs.Add (Path.GetFullPath (Assemblies [i].ItemSpec));
			}

			// All the assemblies to AOT must be in the same directory
			var assemblyDirectories = inputs.Select (v => Path.GetDirectoryName (Path.GetFullPath (v))).Distinct ().ToArray ();
			if (assemblyDirectories.Length > 1) {
				// The assemblies are not in the same directory, so copy them somewhere else (to InputDirectory)
				Directory.CreateDirectory (InputDirectory);
				for (var i = 0; i < inputs.Count; i++) {
					var newInput = Path.Combine (InputDirectory, Path.GetFileName (inputs [i]));
					File.Copy (inputs [i], newInput, true);
					inputs [i] = newInput;
				}
			} else {
				// The assemblies are all in the same directory, we can just use that as input.
				InputDirectory = assemblyDirectories [0];
			}

			Directory.CreateDirectory (OutputDirectory);

			var aotAssemblyFiles = new List<ITaskItem> ();

			var environment = new Dictionary<string, string> {
				{ "MONO_PATH", Path.GetFullPath (InputDirectory) },
			};

			var globalAotArguments = AotArguments?.Select (v => v.ItemSpec).ToList ();
			var listOfArguments = new List<(IList<string> Arguments, string Input)> ();
			for (var i = 0; i < Assemblies.Length; i++) {
				var asm = Assemblies [i];
				var input = inputs [i];
				var arch = Assemblies [i].GetMetadata ("Arch");
				var aotArguments = Assemblies [i].GetMetadata ("Arguments");
				var processArguments = Assemblies [i].GetMetadata ("ProcessArguments");
				var aotData = Assemblies [i].GetMetadata ("AOTData");
				var aotAssembly = Assemblies [i].GetMetadata ("AOTAssembly");

				var aotAssemblyItem = new TaskItem (aotAssembly);
				aotAssemblyItem.SetMetadata ("Arguments", "-Xlinker -rpath -Xlinker @executable_path/ -Qunused-arguments -x assembler -D DEBUG");
				aotAssemblyItem.SetMetadata ("Arch", arch);
				aotAssemblyFiles.Add (aotAssemblyItem);

				var arguments = new List<string> ();
				if (!StringUtils.TryParseArguments (aotArguments, out var parsedArguments, out var ex)) {
					Log.LogError (MSBStrings.E7071, /* Unable to parse the AOT compiler arguments: {0} ({1}) */ aotArguments, ex!.Message);
					return false;
				}
				if (!StringUtils.TryParseArguments (processArguments, out var parsedProcessArguments, out var ex2)) {
					Log.LogError (MSBStrings.E7071, /* Unable to parse the AOT compiler arguments: {0} ({1}) */ processArguments, ex2!.Message);
					return false;
				}
				arguments.Add ($"{string.Join (",", parsedArguments)}");
				if (globalAotArguments is not null)
					arguments.Add ($"--aot={string.Join (",", globalAotArguments)}");
				arguments.AddRange (parsedProcessArguments);
				arguments.Add (input);

				listOfArguments.Add (new (arguments, input));
			}

			Parallel.ForEach (listOfArguments, (arg) => {
				ExecuteAsync (AOTCompilerPath, arg.Arguments, environment: environment, sdkDevPath: SdkDevPath, showErrorIfFailure: false /* we show our own error below */)
					.ContinueWith ((v) => {
						if (v.Result.ExitCode != 0)
							Log.LogError ("Failed to AOT compile {0}, the AOT compiler exited with code {1}", Path.GetFileName (arg.Input), v.Result.ExitCode);

						return System.Threading.Tasks.Task.FromResult<Execution> (v.Result);
					})
					.Unwrap ()
					.Wait ();
			});

			AssemblyFiles = aotAssemblyFiles.ToArray ();

			// For Windows support it's necessary to have the files we're going to create as an Output parameter, so that the files are
			// created on the windows side too, which makes the Inputs/Outputs logic work properly when working from Windows.
			var objectFiles = Assemblies.Select (v => v.GetMetadata ("ObjectFile")).Where (v => !string.IsNullOrEmpty (v));
			var llvmFiles = Assemblies.Select (v => v.GetMetadata ("LLVMFile")).Where (v => !string.IsNullOrEmpty (v));
			FileWrites = objectFiles.Union (llvmFiles).Select (v => new TaskItem (v)).ToArray ();

			return !Log.HasLoggedErrors;

		}
	}
}
