using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Mono.Cecil;

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

		class AssemblyInfo {
			public ITaskItem TaskItem;
			public bool? IsUpToDate;

			public AssemblyInfo (ITaskItem item)
			{
				TaskItem = item;
			}
		}

		Dictionary<string, AssemblyInfo> assemblyInfos = new Dictionary<string, AssemblyInfo> (StringComparer.OrdinalIgnoreCase);

		string GetAssemblyName (ITaskItem item)
		{
			return Path.GetFileNameWithoutExtension (item.ItemSpec);
		}

		bool IsUpToDate (ITaskItem assembly)
		{
			var assemblyPath = assembly.ItemSpec;
			var key = GetAssemblyName (assembly);
			if (assemblyInfos.TryGetValue (key, out var info)) {
				if (!info.IsUpToDate.HasValue) {
					Log.LogError (MSBStrings.E7119 /* Encountered an assembly reference cycle related to the assembly {0}. */, assemblyPath);
					info.IsUpToDate = false;
					return false;
				}
				return info.IsUpToDate.Value;
			}

			info = new AssemblyInfo (assembly);
			assemblyInfos [key] = info;

			var finfo = new FileInfo (assemblyPath);
			if (!finfo.Exists) {
				Log.LogError (MSBStrings.E0158 /* The file {0} does not exist. */, assemblyPath);
				info.IsUpToDate = false;
				return false;
			}

			// ObjectFile is required
			var objectFile = assembly.GetMetadata ("ObjectFile");
			if (string.IsNullOrEmpty (objectFile)) {
				Log.LogError (MSBStrings.E7116 /* The assembly {0} does not provide an 'ObjectFile' metadata. */, assembly.ItemSpec);
				info.IsUpToDate = false;
				return false;
			}
			var objectFileInfo = new FileInfo (objectFile);
			if (!IsUpToDate (finfo, objectFileInfo)) {
				Log.LogMessage (MessageImportance.Low, "The assembly {0} is not up-to-date with regards to the object file {1}", assemblyPath, objectFile);
				info.IsUpToDate = false;
				return false;
			}

			// LLVMFile is optional
			var llvmFile = assembly.GetMetadata ("LLVMFile");
			if (!string.IsNullOrEmpty (llvmFile)) {
				var llvmFileInfo = new FileInfo (llvmFile);
				if (!IsUpToDate (finfo, llvmFileInfo)) {
					Log.LogMessage (MessageImportance.Low, "The assembly {0} is not up-to-date with regards to the llvm file {1}", assemblyPath, llvmFile);
					info.IsUpToDate = false;
					return false;
				}
			}

			// We know now the assembly itself is up-to-date, but what about every referenced assembly?
			// This assembly must be AOT-compiled again if any referenced assembly has changed as well.
			using var ad = AssemblyDefinition.ReadAssembly (assembly.ItemSpec, new ReaderParameters { ReadingMode = ReadingMode.Deferred });
			foreach (var ar in ad.MainModule.AssemblyReferences) {
				var referencedItems = Assemblies.Where (v => string.Equals (GetAssemblyName (v), ar.Name, StringComparison.OrdinalIgnoreCase)).ToArray ();
				if (referencedItems.Length == 0) {
					Log.LogMessage (MessageImportance.Low, $"Ignoring unresolved assembly {ar.Name} (referenced from {assemblyPath}).");
					continue;
				} else if (referencedItems.Length > 1) {
					Log.LogError (MSBStrings.E7117 /* The assembly {0} was passed multiple times as an input assembly (referenced from {1}). */, ar.Name, assemblyPath);
					info.IsUpToDate = false;
					return false;
				}
				var referencedItem = referencedItems [0];
				if (!IsUpToDate (referencedItem)) {
					info.IsUpToDate = false;
					Log.LogMessage (MessageImportance.Low, "The assembly {0} is not up-to-date with regards to the reference {1}", assemblyPath, ar.Name);
					return false;
				}
			}

			Log.LogMessage (MessageImportance.Low, $"The AOT-compiled code for {assemblyPath} is up-to-date.");
			info.IsUpToDate = true;
			return true;
		}

		bool IsUpToDate (FileInfo input, FileInfo output)
		{
			if (!output.Exists)
				return false;
			return input.LastWriteTimeUtc <= output.LastWriteTimeUtc;
		}

		public override bool Execute ()
		{
			var inputs = new List<string> (Assemblies.Length);
			for (var i = 0; i < Assemblies.Length; i++) {
				var input = Path.GetFullPath (Assemblies [i].ItemSpec);
				inputs.Add (input);
				Assemblies [i].SetMetadata ("Input", input);
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

			// Figure out which assemblies need to be aot'ed, and which are up-to-date.
			var assembliesToAOT = Assemblies.Where (asm => !IsUpToDate (asm)).ToArray ();
			if (assembliesToAOT.Length == 0) {
				Log.LogMessage (MessageImportance.Low, $"All the AOT-compiled code is up-to-date.");
				return !Log.HasLoggedErrors;
			}

			Directory.CreateDirectory (OutputDirectory);

			var aotAssemblyFiles = new List<ITaskItem> ();

			var environment = new Dictionary<string, string?> {
				{ "MONO_PATH", Path.GetFullPath (InputDirectory) },
			};

			var globalAotArguments = AotArguments?.Select (v => v.ItemSpec).ToList ();
			var listOfArguments = new List<(IList<string> Arguments, string Input)> ();
			for (var i = 0; i < assembliesToAOT.Length; i++) {
				var asm = assembliesToAOT [i];
				var input = asm.GetMetadata ("Input");
				var arch = asm.GetMetadata ("Arch");
				var aotArguments = asm.GetMetadata ("Arguments");
				var processArguments = asm.GetMetadata ("ProcessArguments");
				var aotData = asm.GetMetadata ("AOTData");
				var aotAssembly = asm.GetMetadata ("AOTAssembly");

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
							Log.LogError (MSBStrings.E7118 /* Failed to AOT compile {0}, the AOT compiler exited with code {1} */, Path.GetFileName (arg.Input), v.Result.ExitCode);

						return System.Threading.Tasks.Task.FromResult<Execution> (v.Result);
					})
					.Unwrap ()
					.Wait ();
			});

			AssemblyFiles = aotAssemblyFiles.ToArray ();

			// For Windows support it's necessary to have the files we're going to create as an Output parameter, so that the files are
			// created on the windows side too, which makes the Inputs/Outputs logic work properly when working from Windows.
			var objectFiles = assembliesToAOT.Select (v => v.GetMetadata ("ObjectFile")).Where (v => !string.IsNullOrEmpty (v));
			var llvmFiles = assembliesToAOT.Select (v => v.GetMetadata ("LLVMFile")).Where (v => !string.IsNullOrEmpty (v));
			FileWrites = objectFiles.Union (llvmFiles).Select (v => new TaskItem (v)).ToArray ();

			return !Log.HasLoggedErrors;

		}
	}
}
