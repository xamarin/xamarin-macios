using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;
using Xamarin.MacDev;

namespace Xamarin.iOS.Tasks
{
	[Flags]
	enum TargetArchitecture
	{
		Default      = 0,

		i386         = 1,
		x86_64       = 2,

		ARMv6        = 4,
		ARMv7        = 8,
		ARMv7s       = 16,
		ARMv7k       = 32,
		ARM64        = 64,

		// Note: needed for backwards compatability
		ARMv6_ARMv7  = ARMv6 | ARMv7,
	}

	enum NativeReferenceKind
	{
		Static,
		Dynamic,
		Framework
	}

	public abstract class MTouchTaskBase : ToolTask
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

		public string SessionId { get; set; }

		[Required]
		public string AppBundleDir { get; set; }

		[Required]
		public ITaskItem AppManifest { get; set; }

		public string Architectures { get; set; }

		public string ArchiveSymbols { get; set; }

		public string CompiledEntitlements { get; set; }

		[Required]
		public bool Debug { get; set; }

		[Required]
		public bool EnableBitcode { get; set; }

		[Required]
		public bool EnableGenericValueTypeSharing { get; set; }

		public string License { get; set; }

		[Required]
		public string ExecutableName { get; set; }

		public string ExtraArgs { get; set; }

		[Required]
		public bool FastDev { get; set; }

		[Required]
		public string HttpClientHandler { get; set; }

		public string I18n { get; set; }

		public string IntermediateOutputPath { get; set; }

		[Required]
		public bool IsAppExtension { get; set; }

		public ITaskItem[] LinkDescriptions { get; set; }

		[Required]
		public bool EnableSGenConc { get; set; }

		[Required]
		public bool UseInterpreter { get; set; }

		[Required]
		public bool LinkerDumpDependencies { get; set; }

		[Required]
		public string LinkMode { get; set; }

		[Required]
		public ITaskItem MainAssembly { get; set; }

		public ITaskItem[] NativeReferences { get; set; }

		// Note: This property is used by XVS in order to calculate the Mac-equivalent paths for the MainAssembly and possibly other properties.
		[Required]
		public string OutputPath { get; set; }

		[Required]
		public bool Profiling { get; set; }

		[Required]
		public string ProjectDir { get; set; }

		[Required]
		public ITaskItem[] References { get; set; }

		[Required]
		public string ResponseFilePath { get; set; }

		[Required]
		public bool SdkIsSimulator { get; set; }

		[Required]
		public string SdkRoot {	get; set; }

		[Required]
		public string SdkVersion { get; set; }

		[Required]
		public string SymbolsList { get; set; }

		[Required]
		public string TargetFrameworkIdentifier { get; set; }

		[Required]
		public string TargetFrameworkVersion { get; set; }

		[Required]
		public bool UseLlvm { get; set; }

		[Required]
		public bool UseFloat32 { get; set; }

		[Required]
		public bool UseThumb { get; set; }

		public int Verbosity { get; set; }

		[Required]
		public ITaskItem[] AppExtensionReferences { get; set; }

		#endregion

		#region Outputs

		[Output]
		public string CompiledArchitectures { get; set; }

		// This property is required for VS to write the output native executable files
		// and ensure the Inputs/Outputs of the msbuild target works correcly
		[Required]
		[Output]
		public ITaskItem NativeExecutable { get; set; }

		#endregion

		public PlatformFramework Framework {
			get { return PlatformFrameworkHelper.GetFramework (TargetFrameworkIdentifier); }
		}

		protected override string ToolName {
			get { return "mtouch"; }
		}

		protected override string GenerateFullPathToTool ()
		{
			if (!string.IsNullOrEmpty (ToolPath))
				return Path.Combine (ToolPath, ToolExe);

			var path = Path.Combine (IPhoneSdks.MonoTouch.BinDir, ToolExe);

			return File.Exists (path) ? path : ToolExe;
		}

		protected override int ExecuteTool (string pathToTool, string responseFileCommands, string commandLineCommands)
		{
			// First we need to create the output directory if it does not exist
			Directory.CreateDirectory (AppBundleDir);

			return base.ExecuteTool (pathToTool, responseFileCommands, commandLineCommands);
		}

		protected override void LogEventsFromTextOutput (string singleLine, MessageImportance messageImportance)
		{
			//There is a bug (Bug30038) in the Microsoft.Build.Utilities.ToolTask implementation of Mono
			//that fails to parse the tool output if the line lenght is just one char.
			//This code is just a workaround and should be removed once the bug is fixed.
			if (!string.IsNullOrEmpty (singleLine) && singleLine.TrimStart ().Length == 1)
				singleLine = singleLine + " ";
			
			base.LogEventsFromTextOutput (singleLine, messageImportance);
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
					Log.LogWarning ("Unknown native reference type for '{0}'.", item.ItemSpec);
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
					Log.LogWarning ("Dynamic native references are not supported: '{0}'", item.ItemSpec);
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
			var args = new CommandLineArgumentBuilder ();
			List<string> unescapedArgs = new List<string> ();

			TargetArchitecture architectures;
			bool msym;

			if (string.IsNullOrEmpty (Architectures) || !Enum.TryParse (Architectures, out architectures))
				architectures = TargetArchitecture.Default;

			if (architectures == TargetArchitecture.ARMv6) {
				Log.LogError ("Target architecture ARMv6 is no longer supported in Xamarin.iOS. Please select a supported architecture.");
				return null;
			}

			if (!string.IsNullOrEmpty (IntermediateOutputPath)) {
				Directory.CreateDirectory (IntermediateOutputPath);

				args.AddQuotedLine ($"--cache={Path.GetFullPath (IntermediateOutputPath)}");
			}

			args.AddQuotedLine ((SdkIsSimulator ? "--sim=" : "--dev=") + Path.GetFullPath (AppBundleDir));

			if (AppleSdkSettings.XcodeVersion.Major >= 5 && IPhoneSdks.MonoTouch.Version.CompareTo (new IPhoneSdkVersion (6, 3, 7)) < 0)
				args.AddLine ("--compiler=clang");

			args.AddQuotedLine ($"--executable={ExecutableName}");

			if (IsAppExtension)
				args.AddLine ("--extension");

			if (Debug) {
				if (FastDev && !SdkIsSimulator)
					args.AddLine ("--fastdev");

				args.AddLine ("--debug");
			}

			if (Profiling)
				args.AddLine ("--profiling");

			if (LinkerDumpDependencies)
				args.AddLine ("--linkerdumpdependencies");

			if (EnableSGenConc)
				args.AddLine ("--sgen-conc");

			if (UseInterpreter)
				args.Add ("--interpreter");

			switch (LinkMode.ToLowerInvariant ()) {
			case "sdkonly": args.AddLine ("--linksdkonly"); break;
			case "none":    args.AddLine ("--nolink"); break;
			}

			if (!string.IsNullOrEmpty (I18n))
				args.AddQuotedLine ($"--i18n={I18n}");

			args.AddQuotedLine ($"--sdkroot={SdkRoot}");

			args.AddQuotedLine ($"--sdk={SdkVersion}");

			if (!minimumOSVersion.IsUseDefault)
				args.AddQuotedLine ($"--targetver={minimumOSVersion.ToString ()}");

			if (UseFloat32 /* We want to compile 32-bit floating point code to use 32-bit floating point operations */)
				args.AddLine ("--aot-options=-O=float32");
			else
				args.AddLine ("--aot-options=-O=-float32");

			if (!EnableGenericValueTypeSharing)
				args.AddLine ("--gsharedvt=false");

			if (LinkDescriptions != null) {
				foreach (var desc in LinkDescriptions)
					args.AddQuotedLine ($"--xml={desc.ItemSpec}");
			}

			if (EnableBitcode) {
				switch (Framework) {
				case PlatformFramework.WatchOS:
					args.AddLine ("--bitcode=full");
					break;
				case PlatformFramework.TVOS:
					args.AddLine ("--bitcode=asmonly");
					break;
				default:
					throw new InvalidOperationException (string.Format ("Bitcode is currently not supported on {0}.", Framework));
				}
			}

			if (!string.IsNullOrEmpty (HttpClientHandler))
				args.AddLine ($"--http-message-handler={HttpClientHandler}");

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

			if (!string.IsNullOrEmpty (ArchiveSymbols) && bool.TryParse (ArchiveSymbols.Trim (), out msym))
				args.AddLine ($"--msym={(msym ? "yes" : "no")}");

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
					args.AddQuotedLine ($"-r={ResolveFrameworkFile (asm.ItemSpec)}");
				} else {
					args.AddQuotedLine ($"-r={Path.GetFullPath (asm.ItemSpec)}");
				}
			}

			foreach (var ext in AppExtensionReferences)
				args.AddQuotedLine ($"--app-extension={Path.GetFullPath (ext.ItemSpec)}");

			args.AddLine ($"--target-framework={TargetFrameworkIdentifier},{TargetFrameworkVersion}");

			args.AddQuotedLine ($"--root-assembly={Path.GetFullPath (MainAssembly.ItemSpec)}");

			// We give the priority to the ExtraArgs to set the mtouch verbosity.
			if (string.IsNullOrEmpty (ExtraArgs) || (!string.IsNullOrEmpty (ExtraArgs) && !ExtraArgs.Contains ("-q") && !ExtraArgs.Contains ("-v")))
				args.AddLine (GetVerbosityLevel (Verbosity));

			if (!string.IsNullOrWhiteSpace (License))
				args.AddLine ($"--license={License}");

			// Generate a response file
			var responseFile = Path.GetFullPath (ResponseFilePath);

			if (File.Exists (responseFile))
				File.Delete (responseFile);

			try {
				using (var fs = File.Create (responseFile)) {
					using (var writer = new StreamWriter (fs))
						writer.Write (args);
				}
			} catch (Exception ex) {
				Log.LogWarning ("Failed to create response file '{0}': {1}", responseFile, ex);
			}

			// Some arguments can not safely go in the response file and are 
			// added seperatly. They must go _after_ the response file
			// as they may override options passed in the response file
			var actualArgs = new CommandLineArgumentBuilder ();

			actualArgs.AddQuoted ($"@{responseFile}");

			foreach (var arg in unescapedArgs)
				actualArgs.AddQuoted (arg);

			return actualArgs.ToString ();
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
				Log.LogError (null, null, null, AppManifest.ItemSpec, 0, 0, 0, 0, "Could not load Info.plist: {0}", ex.Message);
				return false;
			}

//			deviceType = plist.GetUIDeviceFamily ();

			if (plist.TryGetValue (ManifestKeys.MinimumOSVersion, out value)) {
				if (!IPhoneSdkVersion.TryParse (value.Value, out minimumOSVersion)) {
					Log.LogError (null, null, null, AppManifest.ItemSpec, 0, 0, 0, 0, "Could not parse MinimumOSVersion '{0}'", value);
					return false;
				}
			} else {
				switch (Framework) {
				case PlatformFramework.iOS:
					IPhoneSdkVersion sdkVersion;
					if (!IPhoneSdkVersion.TryParse (SdkVersion, out sdkVersion)) {
						Log.LogError (null, null, null, AppManifest.ItemSpec, 0, 0, 0, 0, "Could not parse SdkVersion '{0}'", SdkVersion);
						return false;
					}

					minimumOSVersion = sdkVersion;
					break;
				case PlatformFramework.WatchOS:
				case PlatformFramework.TVOS:
					minimumOSVersion = IPhoneSdkVersion.UseDefault;
					break;
				default:
					throw new InvalidOperationException (string.Format ("Invalid framework: {0}", Framework));
				}
			}

			Directory.CreateDirectory (AppBundleDir);

			return base.Execute ();
		}

		string ResolveFrameworkFile (string fullName)
		{
			// It may have been resolved to an existing local full path
			// already, such as when building from XS on the Mac.
			if (File.Exists (fullName))
				return fullName;

			var frameworkDir = TargetFrameworkIdentifier;
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

		static string GetVerbosityLevel (int v) {
			string result = "";
			// The values here come from: https://github.com/mono/monodevelop/blob/143f9b6617123a0841a5cc5a2a4e13b309535792/main/src/core/MonoDevelop.Projects.Formats.MSBuild/MonoDevelop.Projects.MSBuild.Shared/RemoteBuildEngineMessages.cs#L186
			// Assume 'Normal (2)' is the default verbosity (no change), and the other values follow from there.
			switch (v) {
			case 0:
				result = "-q -q -q -q";
				break;
			case 1:
				result = "-q -q";
				break;
			case 2:
				result = "";
				break;
			case 3:
				result = "-v -v";
				break;
			case 4:
				result = "-v -v -v -v";
				break;
			}
			return result;
		}
	}
}
