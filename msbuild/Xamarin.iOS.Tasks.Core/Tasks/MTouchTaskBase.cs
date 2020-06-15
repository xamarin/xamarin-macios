using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;
using Xamarin.MacDev;
using Xamarin.Utils;
using Xamarin.Localization.MSBuild;

namespace Xamarin.iOS.Tasks
{
	public abstract class MTouchTaskBase : BundlerToolTaskBase
	{
		class GccOptions
		{
			public CommandLineArgumentBuilder Arguments { get; private set; }
			public HashSet<string> WeakFrameworks { get; private set; }
			public HashSet<string> Frameworks { get; private set; }
			public bool Cxx { get; set; }

			public GccOptions ()
			{
				Arguments = new CommandLineArgumentBuilder ();
				WeakFrameworks = new HashSet<string> ();
				Frameworks = new HashSet<string> ();
			}
		}

		IPhoneSdkVersion minimumOSVersion;
//		IPhoneDeviceType deviceType;

		#region Inputs

		public string Architectures { get; set; }

		public string CompiledEntitlements { get; set; }

		[Required]
		public bool EnableBitcode { get; set; }

		public string License { get; set; }

		[Required]
		public string ExecutableName { get; set; }

		[Required]
		public bool FastDev { get; set; }
		
		public ITaskItem[] LinkDescriptions { get; set; }

		public string Interpreter { get; set; }

		[Required]
		public bool LinkerDumpDependencies { get; set; }

		[Required]
		public string ProjectDir { get; set; }

		[Required]
		public bool SdkIsSimulator { get; set; }

		[Required]
		public string SymbolsList { get; set; }

		[Required]
		public bool UseLlvm { get; set; }

		[Required]
		public bool UseFloat32 { get; set; }

		[Required]
		public bool UseThumb { get; set; }

		[Required]
		public ITaskItem[] AppExtensionReferences { get; set; }

		#endregion

		#region Outputs

		[Output]
		public string CompiledArchitectures { get; set; }

		// This property is required for VS to write the output native executable files
		// and ensure the Inputs/Outputs of the msbuild target works correcly
		[Output]
		public ITaskItem NativeExecutable { get; set; }

		[Output]
		public ITaskItem[] CopiedFrameworks { get; set; }

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

		void BuildNativeReferenceFlags (GccOptions gcc)
		{
			if (NativeReferences == null)
				return;

			foreach (var item in NativeReferences) {
				var value = item.GetMetadata ("Kind");
				NativeReferenceKind kind;
				bool boolean;

				if (string.IsNullOrEmpty (value) || !Enum.TryParse (value, out kind)) {
					Log.LogWarning (MSBStrings.W0051, item.ItemSpec);
					continue;
				}

				if (kind == NativeReferenceKind.Static) {
					var libName = Path.GetFileName (item.ItemSpec);

					if (libName.EndsWith (".a", StringComparison.Ordinal))
						libName = libName.Substring (0, libName.Length - 2);

					if (libName.StartsWith ("lib", StringComparison.Ordinal))
						libName = libName.Substring (3);

					if (!string.IsNullOrEmpty (Path.GetDirectoryName (item.ItemSpec)))
						gcc.Arguments.AddQuoted ("-L" + Path.GetDirectoryName (item.ItemSpec));

					gcc.Arguments.AddQuoted ("-l" + libName);

					value = item.GetMetadata ("ForceLoad");

					if (!string.IsNullOrEmpty (value) && bool.TryParse (value, out boolean) && boolean) {
						gcc.Arguments.Add ("-force_load");
						gcc.Arguments.AddQuoted (item.ItemSpec);
					}

					value = item.GetMetadata ("IsCxx");

					if (!string.IsNullOrEmpty (value) && bool.TryParse (value, out boolean) && boolean)
						gcc.Cxx = true;
				} else if (kind == NativeReferenceKind.Framework) {
					gcc.Frameworks.Add (item.ItemSpec);
				} else {
					Log.LogWarning (MSBStrings.W0052, item.ItemSpec);
					continue;
				}

				value = item.GetMetadata ("NeedsGccExceptionHandling");
				if (!string.IsNullOrEmpty (value) && bool.TryParse (value, out boolean) && boolean) {
					if (!gcc.Arguments.Contains ("-lgcc_eh"))
						gcc.Arguments.Add ("-lgcc_eh");
				}

				value = item.GetMetadata ("WeakFrameworks");
				if (!string.IsNullOrEmpty (value)) {
					foreach (var framework in value.Split (' ', '\t'))
						gcc.WeakFrameworks.Add (framework);
				}

				value = item.GetMetadata ("Frameworks");
				if (!string.IsNullOrEmpty (value)) {
					foreach (var framework in value.Split (' ', '\t'))
						gcc.Frameworks.Add (framework);
				}

				// Note: these get merged into gccArgs by our caller
				value = item.GetMetadata ("LinkerFlags");
				if (!string.IsNullOrEmpty (value)) {
					var linkerFlags = CommandLineArgumentBuilder.Parse (value);

					foreach (var flag in linkerFlags)
						gcc.Arguments.AddQuoted (flag);
				}
			}
		}

		static bool EntitlementsRequireLinkerFlags (string path)
		{
			try {
				var plist = PDictionary.FromFile (path);

				// FIXME: most keys do not require linking in the entitlements file, so we
				// could probably add some smarter logic here to iterate over all of the
				// keys in order to determine whether or not we really need to link with
				// the entitlements or not.
				return plist.Count != 0;
			} catch {
				return false;
			}
		}

		void BuildEntitlementFlags (GccOptions gcc)
		{
			if (SdkIsSimulator && !string.IsNullOrEmpty (CompiledEntitlements) && EntitlementsRequireLinkerFlags (CompiledEntitlements)) {
				gcc.Arguments.AddQuoted (new [] { "-Xlinker", "-sectcreate", "-Xlinker", "__TEXT", "-Xlinker", "__entitlements" });
				gcc.Arguments.Add ("-Xlinker");
				gcc.Arguments.AddQuoted (Path.GetFullPath (CompiledEntitlements));
			}
		}

		static string Unquote (string text, int startIndex)
		{
			if (startIndex >= text.Length)
				return string.Empty;

			if (text[startIndex] != '"')
				return text.Substring (startIndex);

			var builder = new StringBuilder ();
			var escaped = false;
			var quoted = true;

			for (int i = startIndex; i < text.Length && quoted; i++) {
				switch (text[i]) {
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
					builder.Append (text[i]);
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
				return null;
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
			case "none":    args.AddLine ("--nolink"); break;
			}

			args.AddQuotedLine ($"--sdk={SdkVersion}");

			if (!minimumOSVersion.IsUseDefault)
				args.AddQuotedLine ($"--targetver={minimumOSVersion.ToString ()}");

			if (UseFloat32 /* We want to compile 32-bit floating point code to use 32-bit floating point operations */)
				args.AddLine ("--aot-options=-O=float32");
			else
				args.AddLine ("--aot-options=-O=-float32");

			if (LinkDescriptions != null) {
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

			// Output the CompiledArchitectures
			CompiledArchitectures = architectures.ToString ();

			args.AddLine ($"--abi={abi}");

			// output symbols to preserve when stripping
			args.AddQuotedLine ($"--symbollist={Path.GetFullPath (SymbolsList)}");

			// don't have mtouch generate the dsyms...
			args.AddLine ("--dsym=no");

			var gcc = new GccOptions ();

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
					// { "solutiondir",  proj.ParentSolution != null ? proj.ParentSolution.BaseDirectory : proj.BaseDirectory },
					{ "appbundledir", AppBundleDir },
					{ "targetpath",   Path.Combine (Path.GetDirectoryName (target), Path.GetFileName (target)) },
					{ "targetdir",    Path.GetDirectoryName (target) },
					{ "targetname",   Path.GetFileName (target) },
					{ "targetext",    Path.GetExtension (target) },
				};

				for (int i = 0; i < extraArgs.Length; i++) {
					var argument = extraArgs[i];
					int startIndex = 0;

					while (argument.Length > startIndex && argument[startIndex] == '-')
						startIndex++;

					int endIndex = startIndex;
					while (endIndex < argument.Length && argument[endIndex] != '=')
						endIndex++;

					int length = endIndex - startIndex;

					if (length == 9 && string.CompareOrdinal (argument, startIndex, "gcc_flags", 0, 9) == 0) {
						// user-defined -gcc_flags argument
						string flags = null;

						if (endIndex < extraArgs[i].Length) {
							flags = Unquote (argument, endIndex + 1);
						} else if (i + 1 < extraArgs.Length) {
							flags = extraArgs[++i];
						}

						if (!string.IsNullOrEmpty (flags)) {
							var gccArgs = CommandLineArgumentBuilder.Parse (flags);

							for (int j = 0; j < gccArgs.Length; j++)
								gcc.Arguments.Add (StringParserService.Parse (gccArgs[j], customTags));
						}
					} else {
						// other user-defined mtouch arguments
						unescapedArgs.Add (StringParserService.Parse (argument, customTags));
					}
				}
			}

			BuildNativeReferenceFlags (gcc);
			BuildEntitlementFlags (gcc);

			foreach (var framework in gcc.Frameworks)
				args.AddQuotedLine ($"--framework={framework}");

			foreach (var framework in gcc.WeakFrameworks)
				args.AddQuotedLine ($"--weak-framework={framework}");

			if (gcc.Cxx)
				args.AddLine ("--cxx");

			if (gcc.Arguments.Length > 0)
				unescapedArgs.Add ($"--gcc_flags={gcc.Arguments.ToString ()}");

			foreach (var asm in References) {
				if (IsFrameworkItem(asm)) {
					args.AddQuotedLine ($"--reference={ResolveFrameworkFile (asm.ItemSpec)}");
				} else {
					args.AddQuotedLine ($"--reference={Path.GetFullPath (asm.ItemSpec)}");
				}
			}

			foreach (var ext in AppExtensionReferences)
				args.AddQuotedLine ($"--app-extension={Path.GetFullPath (ext.ItemSpec)}");

			if (!string.IsNullOrWhiteSpace (License))
				args.AddLine ($"--license={License}");

			return CreateResponseFile (args, unescapedArgs);
		}

		static bool IsFrameworkItem (ITaskItem item)
		{
			bool isFrameworkFile;

			return (bool.TryParse(item.GetMetadata("FrameworkFile"), out isFrameworkFile) && isFrameworkFile) ||
				item.GetMetadata ("ResolvedFrom") == "{TargetFrameworkDirectory}" || 
				item.GetMetadata ("ResolvedFrom") == "ImplicitlyExpandDesignTimeFacades";
		}

		public override bool Execute ()
		{
			PDictionary plist;
			PString value;

			try {
				plist = PDictionary.FromFile (AppManifest.ItemSpec);
			} catch (Exception ex) {
				Log.LogError (null, null, null, AppManifest.ItemSpec, 0, 0, 0, 0, MSBStrings.E0055, ex.Message);
				return false;
			}

//			deviceType = plist.GetUIDeviceFamily ();

			if (plist.TryGetValue (ManifestKeys.MinimumOSVersion, out value)) {
				if (!IPhoneSdkVersion.TryParse (value.Value, out minimumOSVersion)) {
					Log.LogError (null, null, null, AppManifest.ItemSpec, 0, 0, 0, 0, MSBStrings.E0011, value);
					return false;
				}
			} else {
				switch (Platform) {
				case ApplePlatform.iOS:
					IPhoneSdkVersion sdkVersion;
					if (!IPhoneSdkVersion.TryParse (SdkVersion, out sdkVersion)) {
						Log.LogError (null, null, null, AppManifest.ItemSpec, 0, 0, 0, 0, MSBStrings.E0056, SdkVersion);
						return false;
					}

					minimumOSVersion = sdkVersion;
					break;
				case ApplePlatform.WatchOS:
				case ApplePlatform.TVOS:
					minimumOSVersion = IPhoneSdkVersion.UseDefault;
					break;
				default:
					throw new InvalidOperationException (string.Format ("Invalid framework: {0}", Platform));
				}
			}

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

		ITaskItem[] GetCopiedFrameworks ()
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
	
		static string ResolveFrameworkFileOrFacade (string frameworkDir, string fileName)
		{
			var facadeFile = Path.Combine (IPhoneSdks.MonoTouch.LibDir, "mono", frameworkDir, "Facades", fileName);

			if (File.Exists (facadeFile))
				return facadeFile;

			var frameworkFile = Path.Combine (IPhoneSdks.MonoTouch.LibDir, "mono", frameworkDir, fileName);
			if (File.Exists (frameworkFile))
				return frameworkFile;

			return null;
		}
	}
}
