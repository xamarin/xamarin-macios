using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Xamarin.Utils;

#nullable enable

namespace Xamarin.Linker {
	public class ComputeAOTArguments : ConfigurationAwareStep {
		protected override string Name { get; } = "Compute AOT Arguments";
		protected override int ErrorCode { get; } = 2370;

		protected override void TryEndProcess ()
		{
			base.TryEndProcess ();

			var assembliesToAOT = new List<MSBuildItem> ();

			var app = Configuration.Application;
			var outputDirectory = Configuration.AOTOutputDirectory;
			var dedupFileName = Path.GetFileName (Configuration.DedupAssembly);
			var isDedupEnabled = Configuration.Target.Assemblies.Any (asm => Path.GetFileName (asm.FullPath) == dedupFileName);

			foreach (var asm in Configuration.Target.Assemblies) {
				var isAOTCompiled = asm.IsAOTCompiled;
				if (!isAOTCompiled)
					continue;

				var item = new MSBuildItem (Path.Combine (Configuration.IntermediateLinkDir, asm.FileName));

				var input = asm.FullPath;
				bool? isDedupAssembly = null;
				if (isDedupEnabled) {
					isDedupAssembly = Path.GetFileName (input) == dedupFileName;
				}
				var abis = app.Abis.Select (v => v.AsString ()).ToArray ();
				foreach (var abi in app.Abis) {
					var abiString = abi.AsString ();
					var arch = abi.AsArchString ();
					var aotAssembly = Path.Combine (outputDirectory, arch, Path.GetFileName (input) + ".s");
					var aotData = Path.Combine (outputDirectory, arch, Path.GetFileNameWithoutExtension (input) + ".aotdata");
					var llvmFile = Configuration.Application.IsLLVM ? Path.Combine (outputDirectory, arch, Path.GetFileName (input) + ".llvm.o") : string.Empty;
					var objectFile = Path.Combine (outputDirectory, arch, Path.GetFileName (input) + ".o");
					app.GetAotArguments (asm.FullPath, abi, outputDirectory, aotAssembly, llvmFile, aotData, isDedupAssembly, out var processArguments, out var aotArguments, Path.GetDirectoryName (Configuration.AOTCompiler)!);
					item.Metadata.Add ("Arguments", StringUtils.FormatArguments (aotArguments));
					item.Metadata.Add ("ProcessArguments", StringUtils.FormatArguments (processArguments));
					item.Metadata.Add ("Abi", abiString);
					item.Metadata.Add ("Arch", arch);
					item.Metadata.Add ("AOTData", aotData);
					item.Metadata.Add ("AOTAssembly", aotAssembly);
					item.Metadata.Add ("LLVMFile", llvmFile);
					item.Metadata.Add ("ObjectFile", objectFile);
					if (isDedupAssembly.HasValue && isDedupAssembly.Value)
						item.Metadata.Add ("IsDedupAssembly", isDedupAssembly.Value.ToString ());
				}

				assembliesToAOT.Add (item);
			}

			Configuration.WriteOutputForMSBuild ("_AssembliesToAOT", assembliesToAOT);
		}
	}
}
