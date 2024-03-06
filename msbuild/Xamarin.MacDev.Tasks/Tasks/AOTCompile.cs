using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Mono.Cecil;

using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;
using Xamarin.Utils;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public class AOTCompile : XamarinTask, ITaskCallback, ICancelableTask {
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
			public bool IsUpToDate;

			// Tarjan's SCC algoritm
			public bool OnStack;
			public int Index;
			public int LowLink;

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

		bool ComputeUpToDate (IEnumerable<ITaskItem> items)
		{
			// Implements Tarjan's algorithm for finding strongly connected components.
			// https://en.wikipedia.org/wiki/Tarjan%27s_strongly_connected_components_algorithm
			//
			// We recursively compute up-to-date state for each assembly and its references.
			// When we encounter a strongly connected component (cycle) we ensure that either
			// all the files in the SCC are marked as up-to-date or none.

			int index = 0;
			var stack = new Stack<AssemblyInfo> ();
			bool success = true;

			foreach (ITaskItem assembly in items) {
				var key = GetAssemblyName (assembly);
				if (!assemblyInfos.ContainsKey (key)) {
					var info = new AssemblyInfo (assembly);
					assemblyInfos [key] = info;
					success &= ComputeUpToDate (info, stack, ref index);
				}
			}

			return success;
		}

		bool ComputeUpToDate (AssemblyInfo info, Stack<AssemblyInfo> stack, ref int index)
		{
			bool success = true;

			info.Index = index;
			info.LowLink = index;
			index++;
			stack.Push (info);
			info.OnStack = true;

			info.IsUpToDate = ComputeUpToDate (info.TaskItem);

			// Walk all referenced assemblies
			var assemblyPath = info.TaskItem.ItemSpec;
			using var ad = AssemblyDefinition.ReadAssembly (assemblyPath, new ReaderParameters { ReadingMode = ReadingMode.Deferred });
			foreach (var ar in ad.MainModule.AssemblyReferences) {
				var referencedItems = Assemblies.Where (v => string.Equals (GetAssemblyName (v), ar.Name, StringComparison.OrdinalIgnoreCase)).ToArray ();
				if (referencedItems.Length == 0) {
					Log.LogMessage (MessageImportance.Low, $"Ignoring unresolved assembly {ar.Name} (referenced from {assemblyPath}).");
					continue;
				} else if (referencedItems.Length > 1) {
					Log.LogError (MSBStrings.E7117 /* The assembly {0} was passed multiple times as an input assembly (referenced from {1}). */, ar.Name, assemblyPath);
					success = false;
					continue;
				}

				var referencedItem = referencedItems [0];
				var key = GetAssemblyName (referencedItem);
				if (!assemblyInfos.TryGetValue (key, out var referenceInfo)) {
					// Referenced assembly has not yet been visited; recurse on it
					referenceInfo = new AssemblyInfo (referencedItem);
					assemblyInfos [key] = referenceInfo;
					success &= ComputeUpToDate (referenceInfo, stack, ref index);
					if (info.IsUpToDate && !referenceInfo.IsUpToDate) {
						Log.LogMessage (MessageImportance.Low, $"The assembly {assemblyPath} is not up-to-date with regards to the reference {referenceInfo.TaskItem.ItemSpec}.");
						info.IsUpToDate = false;
					}
					info.LowLink = Math.Min (info.LowLink, referenceInfo.LowLink);
				} else if (referenceInfo.OnStack) {
					// Referenced assembly is in stack and hence in the current SCC
					info.LowLink = Math.Min (info.LowLink, referenceInfo.Index);
				}
			}

			// If this is a root node of SCC, pop the stack
			if (info.Index == info.LowLink) {
				bool sccIsUpToDate = true;

				// Walk the SCC on the stack and determine whether the whole
				// component is up-to-date or not.
				foreach (var itemOnStack in stack) {
					if (itemOnStack == info) {
						break;
					}
					sccIsUpToDate &= itemOnStack.IsUpToDate;
				}

				// Remove the SCC from the stack and update IsUpToDate for each item.
				AssemblyInfo popped;
				do {
					popped = stack.Pop ();
					popped.OnStack = false;

					if (!sccIsUpToDate) {
						// If any assembly in the SCC is not up-to-date then the whole SCC is not
						// up to date.
						popped.IsUpToDate = false;
						Log.LogMessage (MessageImportance.Low, $"The assembly {popped.TaskItem.ItemSpec} in a cycle is not up-to-date.");
					} else {
						Log.LogMessage (MessageImportance.Low, $"The AOT-compiled code for {popped.TaskItem.ItemSpec} is up-to-date.");
					}
				} while (popped != info);
			}

			return success;
		}

		bool ComputeUpToDate (ITaskItem assembly)
		{
			var assemblyPath = assembly.ItemSpec;
			var finfo = new FileInfo (assemblyPath);
			if (!finfo.Exists) {
				Log.LogError (MSBStrings.E0158 /* The file {0} does not exist. */, assemblyPath);
				return false;
			}

			// ObjectFile is required
			var objectFile = assembly.GetMetadata ("ObjectFile");
			if (string.IsNullOrEmpty (objectFile)) {
				Log.LogError (MSBStrings.E7116 /* The assembly {0} does not provide an 'ObjectFile' metadata. */, assembly.ItemSpec);
				return false;
			}
			var objectFileInfo = new FileInfo (objectFile);
			if (!IsUpToDate (finfo, objectFileInfo)) {
				Log.LogMessage (MessageImportance.Low, "The assembly {0} is not up-to-date with regards to the object file {1}", assemblyPath, objectFile);
				return false;
			}

			// LLVMFile is optional
			var llvmFile = assembly.GetMetadata ("LLVMFile");
			if (!string.IsNullOrEmpty (llvmFile)) {
				var llvmFileInfo = new FileInfo (llvmFile);
				if (!IsUpToDate (finfo, llvmFileInfo)) {
					Log.LogMessage (MessageImportance.Low, "The assembly {0} is not up-to-date with regards to the llvm file {1}", assemblyPath, llvmFile);
					return false;
				}
			}

			return true;
		}

		bool IsUpToDate (ITaskItem assembly)
		{
			var assemblyPath = assembly.ItemSpec;
			var key = GetAssemblyName (assembly);
			if (assemblyInfos.TryGetValue (key, out var info)) {
				return info.IsUpToDate;
			}

			return false;
		}

		bool IsUpToDate (FileInfo input, FileInfo output)
		{
			if (!output.Exists)
				return false;
			return input.LastWriteTimeUtc <= output.LastWriteTimeUtc;
		}

		public override bool Execute ()
		{
			if (Assemblies.Length == 0) {
				// Not sure how this can happen, since Assemblies is [Required], but it seems to happen once in a while.
				Log.LogMessage (MessageImportance.Low, "Nothing to AOT-compile");
				return true;
			}

			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

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
			if (!ComputeUpToDate (Assemblies)) {
				return false;
			}
			var assembliesToAOT = Assemblies.Where (asm => !IsUpToDate (asm)).ToList ();
			if (assembliesToAOT.Count == 0) {
				Log.LogMessage (MessageImportance.Low, $"All the AOT-compiled code is up-to-date.");
				return !Log.HasLoggedErrors;
			}

			// If any assembly changed, then we must re-generate the dedup assembly.
			var dedupAssembly = Assemblies.SingleOrDefault (asm => {
				Boolean.TryParse (asm.GetMetadata ("IsDedupAssembly"), out var isDedupAssembly);
				return isDedupAssembly;
			});
			if (dedupAssembly is not null && !assembliesToAOT.Contains (dedupAssembly))
				assembliesToAOT.Add (dedupAssembly);

			Directory.CreateDirectory (OutputDirectory);

			var aotAssemblyFiles = new List<ITaskItem> ();
			var globalAotArguments = AotArguments?.Select (v => v.ItemSpec).ToList ();
			var listOfArguments = new List<(IList<string> Arguments, string Input)> ();
			for (var i = 0; i < assembliesToAOT.Count; i++) {
				var asm = assembliesToAOT [i];
				var input = asm.GetMetadata ("Input");
				var arch = asm.GetMetadata ("Arch");
				var aotArguments = asm.GetMetadata ("Arguments");
				var processArguments = asm.GetMetadata ("ProcessArguments");
				var aotData = asm.GetMetadata ("AOTData");
				var aotAssembly = asm.GetMetadata ("AOTAssembly");
				var isDedupAssembly = object.ReferenceEquals (asm, dedupAssembly);

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
				arguments.Add ($"--path={Path.GetFullPath (InputDirectory)}");
				arguments.Add ($"{string.Join (",", parsedArguments)}");
				if (globalAotArguments?.Any () == true)
					arguments.Add ($"--aot={string.Join (",", globalAotArguments)}");
				arguments.AddRange (parsedProcessArguments);
				if (isDedupAssembly)
					arguments.AddRange (inputs);
				else
					arguments.Add (input);

				listOfArguments.Add (new (arguments, input));
			}

			Parallel.ForEach (listOfArguments, (arg) => {
				ExecuteAsync (AOTCompilerPath, arg.Arguments, sdkDevPath: SdkDevPath, showErrorIfFailure: false /* we show our own error below */)
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

		public bool ShouldCopyToBuildServer (ITaskItem item) => false;

		public bool ShouldCreateOutputFile (ITaskItem item) => true;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied ()
		{
			var compiler = new TaskItem (AOTCompilerPath);
			compiler.SetMetadata ("IsUnixExecutable", "true");

			yield return compiler;
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}
	}
}
