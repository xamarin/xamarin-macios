using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;
using Xamarin.MacDev;
using Xamarin.Messaging.Build.Client;
using Xamarin.Utils;
using Xamarin.Localization.MSBuild;

#nullable enable

namespace Xamarin.iOS.Tasks {
	public class MTouch : BundlerToolTaskBase, ITaskCallback {
		#region Inputs

		public string Architectures { get; set; } = string.Empty;

		public string CompiledEntitlements { get; set; } = string.Empty;

		public ITaskItem [] ConfigFiles { get; set; } = Array.Empty<ITaskItem> ();

		[Required]
		public bool EnableBitcode { get; set; }

		public string License { get; set; } = string.Empty;

		[Required]
		public string ExecutableName { get; set; } = string.Empty;

		[Required]
		public bool FastDev { get; set; }

		public ITaskItem [] LinkDescriptions { get; set; } = Array.Empty<ITaskItem> ();

		public string Interpreter { get; set; } = string.Empty;

		[Required]
		public bool LinkerDumpDependencies { get; set; }

		[Required]
		public string ProjectDir { get; set; } = string.Empty;

		[Required]
		public bool SdkIsSimulator { get; set; }

		[Required]
		public string SymbolsList { get; set; } = string.Empty;

		[Required]
		public bool UseLlvm { get; set; }

		[Required]
		public bool UseFloat32 { get; set; }

		[Required]
		public bool UseThumb { get; set; }

		[Required]
		public ITaskItem [] AppExtensionReferences { get; set; } = Array.Empty<ITaskItem> ();

		#endregion

		#region Outputs

		// This property is required for VS to write the output native executable files
		// and ensure the Inputs/Outputs of the msbuild target works correcly
		[Output]
		public ITaskItem? NativeExecutable { get; set; }

		[Output]
		public ITaskItem [] CopiedFrameworks { get; set; } = Array.Empty<ITaskItem> ();

		#endregion

		protected override string ToolName {
			get { return "mtouch"; }
		}

		protected override int ExecuteTool (string pathToTool, string responseFileCommands, string commandLineCommands)
		{
			// First we need to create the output directory if it does not exist
			Directory.CreateDirectory (AppBundleDir);

			return base.ExecuteTool (pathToTool, responseFileCommands, commandLineCommands);
		}

		static string Unquote (string text, int startIndex)
		{
			if (startIndex >= text.Length)
				return string.Empty;

			if (text [startIndex] != '"')
				return text.Substring (startIndex);

			var builder = new StringBuilder ();
			var escaped = false;
			var quoted = true;

			for (int i = startIndex; i < text.Length && quoted; i++) {
				switch (text [i]) {
				case '\\':
					if (escaped)
						builder.Append ('\\');
					escaped = !escaped;
					break;
				case '"':
					if (escaped) {
						builder.Append ('"');
						escaped = false;
					} else {
						quoted = false;
					}
					break;
				default:
					builder.Append (text [i]);
					escaped = false;
					break;
				}
			}

			return builder.ToString ();
		}

		protected override string GenerateCommandLineCommands ()
		{
			var args = GenerateCommandLineArguments ();
			List<string> unescapedArgs = new List<string> ();

			TargetArchitecture architectures;

			if (string.IsNullOrEmpty (Architectures) || !Enum.TryParse (Architectures, out architectures))
				architectures = TargetArchitecture.Default;

			if (architectures == TargetArchitecture.ARMv6) {
				Log.LogError (MSBStrings.E0053);
				return string.Empty;
			}

			args.AddQuotedLine ((SdkIsSimulator ? "--sim=" : "--dev=") + Path.GetFullPath (AppBundleDir));

			args.AddQuotedLine ($"--executable={ExecutableName}");

			if (IsAppExtension)
				args.AddLine ("--extension");

			if (Debug) {
				if (FastDev && !SdkIsSimulator)
					args.AddLine ("--fastdev");
			}

			if (LinkerDumpDependencies)
				args.AddLine ("--linkerdumpdependencies");

			if (!string.IsNullOrEmpty (Interpreter))
				args.AddLine ($"--interpreter={Interpreter}");

			switch (LinkMode.ToLowerInvariant ()) {
			case "sdkonly": args.AddLine ("--linksdkonly"); break;
			case "none": args.AddLine ("--nolink"); break;
			}

			args.AddQuotedLine ($"--sdk={SdkVersion}");

			if (UseFloat32 /* We want to compile 32-bit floating point code to use 32-bit floating point operations */)
				args.AddLine ("--aot-options=-O=float32");
			else
				args.AddLine ("--aot-options=-O=-float32");

			if (LinkDescriptions is not null) {
				foreach (var desc in LinkDescriptions)
					args.AddQuotedLine ($"--xml={desc.ItemSpec}");
			}

			if (EnableBitcode) {
				switch (Platform) {
				case ApplePlatform.WatchOS:
					args.AddLine ("--bitcode=full");
					break;
				case ApplePlatform.TVOS:
					args.AddLine ("--bitcode=asmonly");
					break;
				default:
					throw new InvalidOperationException (string.Format ("Bitcode is currently not supported on {0}.", Platform));
				}
			}

			string thumb = UseThumb && UseLlvm ? "+thumb2" : "";
			string llvm = UseLlvm ? "+llvm" : "";
			string abi = "";

			if (SdkIsSimulator) {
				if (architectures.HasFlag (TargetArchitecture.i386))
					abi += (abi.Length > 0 ? "," : "") + "i386";

				if (architectures.HasFlag (TargetArchitecture.x86_64))
					abi += (abi.Length > 0 ? "," : "") + "x86_64";

				if (string.IsNullOrEmpty (abi)) {
					architectures = TargetArchitecture.i386;
					abi = "i386";
				}
			} else {
				if (architectures == TargetArchitecture.Default)
					architectures = TargetArchitecture.ARMv7;

				if (architectures.HasFlag (TargetArchitecture.ARMv7))
					abi += (abi.Length > 0 ? "," : "") + "armv7" + llvm + thumb;

				if (architectures.HasFlag (TargetArchitecture.ARMv7s))
					abi += (abi.Length > 0 ? "," : "") + "armv7s" + llvm + thumb;

				if (architectures.HasFlag (TargetArchitecture.ARM64)) {
					// Note: ARM64 does not have thumb.
					abi += (abi.Length > 0 ? "," : "") + "arm64" + llvm;
				}

				if (architectures.HasFlag (TargetArchitecture.ARMv7k))
					abi += (abi.Length > 0 ? "," : "") + "armv7k" + llvm;

				if (architectures.HasFlag (TargetArchitecture.ARM64_32))
					abi += (abi.Length > 0 ? "," : "") + "arm64_32" + llvm;

				if (string.IsNullOrEmpty (abi))
					abi = "armv7" + llvm + thumb;
			}

			args.AddLine ($"--abi={abi}");

			// output symbols to preserve when stripping
			args.AddQuotedLine ($"--symbollist={Path.GetFullPath (SymbolsList)}");

			// don't have mtouch generate the dsyms...
			args.AddLine ("--dsym=no");

			var gcc = new LinkerOptions ();

			if (!string.IsNullOrEmpty (ExtraArgs)) {
				var extraArgs = CommandLineArgumentBuilder.Parse (ExtraArgs);
				var target = MainAssembly.ItemSpec;
				string projectDir;

				if (ProjectDir.StartsWith ("~/", StringComparison.Ordinal)) {
					// Note: Since the Visual Studio plugin doesn't know the user's home directory on the Mac build host,
					// it simply uses paths relative to "~/". Expand these paths to their full path equivalents.
					var home = Environment.GetFolderPath (Environment.SpecialFolder.UserProfile);

					projectDir = Path.Combine (home, ProjectDir.Substring (2));
				} else {
					projectDir = ProjectDir;
				}

				var customTags = new Dictionary<string, string> (StringComparer.OrdinalIgnoreCase) {
					{ "projectdir",   projectDir },
					// Apparently msbuild doesn't propagate the solution path, so we can't get it.
					// { "solutiondir",  proj.ParentSolution is not null ? proj.ParentSolution.BaseDirectory : proj.BaseDirectory },
					{ "appbundledir", AppBundleDir },
					{ "targetpath",   Path.Combine (Path.GetDirectoryName (target), Path.GetFileName (target)) },
					{ "targetdir",    Path.GetDirectoryName (target) },
					{ "targetname",   Path.GetFileName (target) },
					{ "targetext",    Path.GetExtension (target) },
				};

				for (int i = 0; i < extraArgs.Length; i++) {
					var argument = extraArgs [i];
					int startIndex = 0;

					while (argument.Length > startIndex && argument [startIndex] == '-')
						startIndex++;

					int endIndex = startIndex;
					while (endIndex < argument.Length && argument [endIndex] != '=')
						endIndex++;

					int length = endIndex - startIndex;

					if (length == 9 && string.CompareOrdinal (argument, startIndex, "gcc_flags", 0, 9) == 0) {
						// user-defined -gcc_flags argument
						string? flags = null;

						if (endIndex < extraArgs [i].Length) {
							flags = Unquote (argument, endIndex + 1);
						} else if (i + 1 < extraArgs.Length) {
							flags = extraArgs [++i];
						}

						if (!string.IsNullOrEmpty (flags)) {
							var gccArgs = CommandLineArgumentBuilder.Parse (flags);

							for (int j = 0; j < gccArgs.Length; j++)
								gcc.Arguments.Add (StringParserService.Parse (gccArgs [j], customTags));
						}
					} else {
						// other user-defined mtouch arguments
						unescapedArgs.Add (StringParserService.Parse (argument, customTags));
					}
				}
			}

			gcc.BuildNativeReferenceFlags (Log, NativeReferences);
			gcc.Arguments.AddQuoted (LinkNativeCodeTaskBase.GetEmbedEntitlementsInExecutableLinkerFlags (CompiledEntitlements));

			foreach (var framework in gcc.Frameworks)
				args.AddQuotedLine ($"--framework={Path.GetFullPath (framework)}");

			foreach (var framework in gcc.WeakFrameworks)
				args.AddQuotedLine ($"--weak-framework={Path.GetFullPath (framework)}");

			if (gcc.Cxx)
				args.AddLine ("--cxx");

			if (gcc.Arguments.Length > 0)
				unescapedArgs.Add ($"--gcc_flags={gcc.Arguments.ToString ()}");

			foreach (var asm in References) {
				if (IsFrameworkItem (asm)) {
					args.AddQuotedLine ($"--reference={ResolveFrameworkFile (asm.ItemSpec)}");
				} else {
					args.AddQuotedLine ($"--reference={Path.GetFullPath (asm.ItemSpec)}");
				}
			}

			foreach (var ext in AppExtensionReferences)
				args.AddQuotedLine ($"--app-extension={Path.GetFullPath (ext.ItemSpec)}");

			if (!string.IsNullOrWhiteSpace (License))
				args.AddLine ($"--license={License}");

			return args.CreateResponseFile (this, ResponseFilePath, unescapedArgs);
		}

		static bool IsFrameworkItem (ITaskItem item)
		{
			bool isFrameworkFile;

			return (bool.TryParse (item.GetMetadata ("FrameworkFile"), out isFrameworkFile) && isFrameworkFile) ||
				item.GetMetadata ("ResolvedFrom") == "{TargetFrameworkDirectory}" ||
				item.GetMetadata ("ResolvedFrom") == "ImplicitlyExpandDesignTimeFacades";
		}

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return ExecuteRemotely ();
			return ExecuteLocally ();
		}

		bool ExecuteRemotely ()
		{
			var frameworkAssemblies = References.Where (x => x.IsFrameworkItem ());

			//Avoid having duplicated entries, which can happen with netstandard libraries that uses
			//some Reference Assemblies from NuGet packages
			var otherAssemblies = References
				.Where (x => !x.IsFrameworkItem ())
				.Where (x => !frameworkAssemblies.Any (f => f.GetMetadata ("Filename") == x.GetMetadata ("Filename")));

			TaskItemFixer.FixItemSpecs (Log, item => OutputPath, MainAssembly);
			TaskItemFixer.FixFrameworkItemSpecs (Log, item => OutputPath, TargetFramework.Identifier, frameworkAssemblies.ToArray ());
			TaskItemFixer.FixItemSpecs (Log, item => OutputPath, otherAssemblies.ToArray ());
			TaskItemFixer.ReplaceItemSpecsWithBuildServerPath (AppExtensionReferences, SessionId);

			var references = new List<ITaskItem> ();

			references.AddRange (frameworkAssemblies);
			references.AddRange (otherAssemblies);

			ConfigFiles = GetConfigFiles (references).ToArray ();

			TaskItemFixer.FixItemSpecs (Log, item => OutputPath, ConfigFiles);

			References = references.OrderBy (x => x.ItemSpec).ToArray ();

			return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;
		}

		bool ExecuteLocally ()
		{
			Directory.CreateDirectory (AppBundleDir);

			var executableLastWriteTime = default (DateTime);
			var executable = Path.Combine (AppBundleDir, ExecutableName);

			if (File.Exists (executable))
				executableLastWriteTime = File.GetLastWriteTimeUtc (executable);

			var result = base.Execute ();

			CopiedFrameworks = GetCopiedFrameworks ();

			if (File.Exists (executable) && File.GetLastWriteTimeUtc (executable) != executableLastWriteTime)
				NativeExecutable = new TaskItem (executable);

			return result;
		}

		ITaskItem [] GetCopiedFrameworks ()
		{
			var copiedFrameworks = new List<ITaskItem> ();
			var frameworksDir = Path.Combine (AppBundleDir, "Frameworks");

			if (Directory.Exists (frameworksDir)) {
				foreach (var dir in Directory.EnumerateDirectories (frameworksDir, "*.framework")) {
					var framework = Path.Combine (dir, Path.GetFileNameWithoutExtension (dir));

					if (File.Exists (framework))
						copiedFrameworks.Add (new TaskItem (framework));
				}
			}

			return copiedFrameworks.ToArray ();
		}

		string ResolveFrameworkFile (string fullName)
		{
			// It may have been resolved to an existing local full path
			// already, such as when building from XS on the Mac.
			if (Path.IsPathRooted (fullName) && File.Exists (fullName))
				return fullName;

			var frameworkDir = TargetFramework.Identifier;
			var fileName = Path.GetFileName (fullName);

			return ResolveFrameworkFileOrFacade (frameworkDir, fileName) ?? fullName;
		}

		static string? ResolveFrameworkFileOrFacade (string frameworkDir, string fileName)
		{
			var facadeFile = Path.Combine (Sdks.XamIOS.LibDir, "mono", frameworkDir, "Facades", fileName);

			if (File.Exists (facadeFile))
				return facadeFile;

			var frameworkFile = Path.Combine (Sdks.XamIOS.LibDir, "mono", frameworkDir, fileName);
			if (File.Exists (frameworkFile))
				return frameworkFile;

			return null;
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => !item.IsFrameworkItem ();

		public bool ShouldCreateOutputFile (ITaskItem item) => true;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied ()
		{
			if (NativeReferences is null)
				yield break;

			foreach (var nativeRef in NativeReferences) {
				var path = nativeRef.ItemSpec;
				// look for frameworks, if the library is part of one then bring all related files
				var dir = Path.GetDirectoryName (path);
				if (Path.GetExtension (dir) == ".framework") {
					foreach (var item in CreateItemsForAllFilesRecursively (dir)) {
						// don't return the native library itself (it's the original input, not something additional)
						if (item.ItemSpec != path)
							yield return item;
					}
				}
			}
		}

		public override void Cancel ()
		{
			base.Cancel ();

			if (!string.IsNullOrEmpty (SessionId))
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}

		IEnumerable<ITaskItem> GetConfigFiles (IEnumerable<ITaskItem> references)
		{
			var assemblies = new List<ITaskItem> { MainAssembly };

			assemblies.AddRange (references);

			foreach (var item in assemblies) {
				var configFile = item.ItemSpec + ".config";

				if (File.Exists (configFile))
					yield return new TaskItem (configFile);
			}
		}
	}
}
