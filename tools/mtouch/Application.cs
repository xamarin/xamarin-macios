// Copyright 2013 Xamarin Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Text;

using MonoTouch.Tuner;

using Mono.Tuner;
using Xamarin.Linker;

using Xamarin.Utils;
using Xamarin.MacDev;

namespace Xamarin.Bundler {

	public enum BitCodeMode {
		None = 0,
		ASMOnly = 1,
		LLVMOnly = 2,
		MarkerOnly = 3,
	}

	[Flags]
	public enum Abi {
		None   =   0,
		i386   =   1,
		ARMv6  =   2,
		ARMv7  =   4,
		ARMv7s =   8,
		ARM64 =   16,
		x86_64 =  32,
		Thumb  =  64,
		LLVM   = 128,
		ARMv7k = 256,
		SimulatorArchMask = i386 | x86_64,
		DeviceArchMask = ARMv6 | ARMv7 | ARMv7s | ARMv7k | ARM64,
		ArchMask = SimulatorArchMask | DeviceArchMask,
		Arch64Mask = x86_64 | ARM64,
		Arch32Mask = i386 | ARMv6 | ARMv7 | ARMv7s | ARMv7k,
	}

	public static class AbiExtensions {
		public static string AsString (this Abi self)
		{
			var rv = (self & Abi.ArchMask).ToString ();
			if ((self & Abi.LLVM) == Abi.LLVM)
				rv += "+LLVM";
			if ((self & Abi.Thumb) == Abi.Thumb)
				rv += "+Thumb";
			return rv;
		}

		public static string AsArchString (this Abi self)
		{
			return (self & Abi.ArchMask).ToString ().ToLowerInvariant ();
		}
	}

	public enum RegistrarMode {
		Default,
		Dynamic,
		Static,
	}

	public enum BuildTarget {
		Simulator,
		Device,
	}

	public enum DlsymOptions
	{
		Default,
		All,
		None,
		Custom,
	}

	public partial class Application
	{
		public const string ProductName = "Xamarin.iOS";
		public const string Error91LinkerSuggestion = "set the managed linker behaviour to Link Framework SDKs Only";

		public string ExecutableName;
		public BuildTarget BuildTarget;

		public bool EnableCxx;
		public bool EnableProfiling;
		bool? package_managed_debug_symbols;
		public bool PackageManagedDebugSymbols {
			get { return package_managed_debug_symbols.Value; }
			set { package_managed_debug_symbols = value; }
		}
		bool? enable_msym;
		public bool EnableMSym {
			get { return enable_msym.Value; }
			set { enable_msym = value; }
		}
		public bool EnableRepl;

		public bool IsExtension;
		public List<string> Extensions = new List<string> (); // A list of the extensions this app contains.
		public List<Application> AppExtensions = new List<Application> ();

		public bool? EnablePie;
		public bool NativeStrip = true;
		public string SymbolList;
		public bool ManagedStrip = true;
		public List<string> NoSymbolStrip = new List<string> ();

		public DlsymOptions DlsymOptions;
		public List<Tuple<string, bool>> DlsymAssemblies;
		public bool? UseMonoFramework;
		public bool? PackageMonoFramework;

		public bool NoFastSim;
		public bool NoDevCodeShare;
		public bool IsCodeShared { get; private set; }

		// The list of assemblies that we do generate debugging info for.
		public bool DebugAll;
		public List<string> DebugAssemblies = new List<string> ();

		public bool? DebugTrack;

		public string Compiler = string.Empty;
		public string CompilerPath;

		public string AotArguments = "static,asmonly,direct-icalls,";
		public string AotOtherArguments = string.Empty;
		public bool? LLVMAsmWriter;
		public Dictionary<string, string> LLVMOptimizations = new Dictionary<string, string> ();

		public Dictionary<string, string> EnvironmentVariables = new Dictionary<string, string> ();

		//
		// Linker config
		//

		public bool LinkAway = true;
		public bool LinkerDumpDependencies { get; set; }
		public List<string> References = new List<string> ();
		
		public bool? BuildDSym;
		public bool Is32Build { get { return IsArchEnabled (Abi.Arch32Mask); } } // If we're targetting a 32 bit arch.
		public bool Is64Build { get { return IsArchEnabled (Abi.Arch64Mask); } } // If we're targetting a 64 bit arch.
		public bool IsDualBuild { get { return Is32Build && Is64Build; } } // if we're building both a 32 and a 64 bit version.
		public bool IsLLVM { get { return IsArchEnabled (Abi.LLVM); } }

		bool RequiresXcodeHeaders => LinkMode == LinkMode.None;

		public List<Target> Targets = new List<Target> ();

		public string UserGccFlags;

		// If we didn't link the final executable because the existing binary is up-to-date.
		bool cached_executable; 

		List<Abi> abis;
		HashSet<Abi> all_architectures; // all Abis used in the app, including extensions.

		BuildTasks build_tasks;

		Dictionary<string, Tuple<AssemblyBuildTarget, string>> assembly_build_targets = new Dictionary<string, Tuple<AssemblyBuildTarget, string>> ();

		public AssemblyBuildTarget LibMonoLinkMode {
			get {
				if (Embeddinator) {
					return AssemblyBuildTarget.StaticObject;
				} else if (HasFrameworks || UseMonoFramework.Value) {
					return AssemblyBuildTarget.Framework;
				} else if (HasDynamicLibraries) {
					return AssemblyBuildTarget.DynamicLibrary;
				} else {
					return AssemblyBuildTarget.StaticObject;
				}
			}
		}
		public AssemblyBuildTarget LibXamarinLinkMode {
			get {
				if (Embeddinator) {
					return AssemblyBuildTarget.StaticObject;
				} else if (HasFrameworks) {
					return AssemblyBuildTarget.Framework;
				} else if (HasDynamicLibraries) {
					return AssemblyBuildTarget.DynamicLibrary;
				} else {
					return AssemblyBuildTarget.StaticObject;
				}
			}
		}
		public AssemblyBuildTarget LibPInvokesLinkMode => LibXamarinLinkMode;
		public AssemblyBuildTarget LibProfilerLinkMode => OnlyStaticLibraries ? AssemblyBuildTarget.StaticObject : AssemblyBuildTarget.DynamicLibrary;

		Dictionary<string, BundleFileInfo> bundle_files = new Dictionary<string, BundleFileInfo> ();

		public bool OnlyStaticLibraries {
			get {
				return assembly_build_targets.All ((abt) => abt.Value.Item1 == AssemblyBuildTarget.StaticObject);
			}
		}

		public bool HasDynamicLibraries {
			get {
				return assembly_build_targets.Any ((abt) => abt.Value.Item1 == AssemblyBuildTarget.DynamicLibrary);
			}
		}

		public bool HasAnyDynamicLibraries {
			get {
				if (LibMonoLinkMode == AssemblyBuildTarget.DynamicLibrary)
					return true;
				if (LibXamarinLinkMode == AssemblyBuildTarget.DynamicLibrary)
					return true;
				if (LibPInvokesLinkMode == AssemblyBuildTarget.DynamicLibrary)
					return true;
				if (LibProfilerLinkMode == AssemblyBuildTarget.DynamicLibrary)
					return true;
				return HasDynamicLibraries;
			}
		}

		public bool HasFrameworks {
			get {
				return assembly_build_targets.Any ((abt) => abt.Value.Item1 == AssemblyBuildTarget.Framework);
			}
		}

		public void ClearAssemblyBuildTargets ()
		{
			assembly_build_targets.Clear ();
		}

		public void AddAssemblyBuildTarget (string value)
		{
			var eq_index = value.IndexOf ('=');
			if (eq_index == -1)
				throw ErrorHelper.CreateError (10, "Could not parse the command line arguments: --assembly-build-target={0}", value);

			var assembly_name = value.Substring (0, eq_index);
			string target, name;

			var eq_index2 = value.IndexOf ('=', eq_index + 1);
			if (eq_index2 == -1) {
				target = value.Substring (eq_index + 1);
				if (assembly_name == "@all" || assembly_name == "@sdk") {
					name = string.Empty;
				} else {
					name = assembly_name;
				}
			} else {
				target = value.Substring (eq_index + 1, eq_index2 - eq_index - 1);
				name = value.Substring (eq_index2 + 1);
			}

			int invalid_idx;
			if ((invalid_idx = name.IndexOfAny (new char [] { '/', '\\' })) != -1)
				throw ErrorHelper.CreateError (106, "The assembly build target name '{0}' is invalid: the character '{1}' is not allowed.", name, name [invalid_idx]);

			if (assembly_build_targets.ContainsKey (assembly_name))
				throw ErrorHelper.CreateError (101, "The assembly '{0}' is specified multiple times in --assembly-build-target arguments.", assembly_name);

			AssemblyBuildTarget build_target;
			switch (target) {
			case "staticobject":
				build_target = AssemblyBuildTarget.StaticObject;
				break;
			case "dynamiclibrary":
				build_target = AssemblyBuildTarget.DynamicLibrary;
				break;
			case "framework":
				build_target = AssemblyBuildTarget.Framework;

				if (name.EndsWith (".framework", StringComparison.Ordinal))
					name = name.Substring (0, name.Length - ".framework".Length);

				break;
			default:
				throw ErrorHelper.CreateError (10, "Could not parse the command line arguments: --assembly-build-target={0}", value);
			}

			assembly_build_targets [assembly_name] = new Tuple<AssemblyBuildTarget, string> (build_target, name);
		}

		public bool ContainsGroupedSdkAssemblyBuildTargets {
			get {
				// The logic here must match the default logic in 'SelectAssemblyBuildTargets' (because we will execute this method before 'SelectAssemblyBuildTargets' is executed)
				Tuple<AssemblyBuildTarget, string> value;
				if (!assembly_build_targets.TryGetValue ("@sdk", out value))
					return IsCodeShared;
				return !string.IsNullOrEmpty (value.Item2);
			}
		}

		void SelectAssemblyBuildTargets ()
		{
			Tuple<AssemblyBuildTarget, string> all = null;
			Tuple<AssemblyBuildTarget, string> sdk = null;
			List<Exception> exceptions = null;

			if (IsSimulatorBuild && !Embeddinator) {
				if (assembly_build_targets.Count > 0) {
					var first = assembly_build_targets.First ();
					if (assembly_build_targets.Count == 1 && first.Key == "@all" && first.Value.Item1 == AssemblyBuildTarget.DynamicLibrary && first.Value.Item2 == string.Empty) {
						ErrorHelper.Warning (126, "Incremental builds have been disabled because incremental builds are not supported in the simulator.");
					} else {
						ErrorHelper.Warning (125, "The --assembly-build-target command-line argument is ignored in the simulator.");
					}
					assembly_build_targets.Clear ();
				}
				return;
			}
			
			if (assembly_build_targets.Count == 0) {
				// The logic here must match the logic in 'ContainsGroupedSdkAssemblyBuildTarget' (because we will execute 'ContainsGroupedSdkAssemblyBuildTargets' before this is executed)
				assembly_build_targets.Add ("@all", new Tuple<AssemblyBuildTarget, string> (AssemblyBuildTarget.StaticObject, ""));
				if (IsCodeShared) {
					// If we're sharing code, then we can default to creating a Xamarin.Sdk.framework for SDK assemblies,
					// and static objects for the rest of the assemblies.
					assembly_build_targets.Add ("@sdk", new Tuple<AssemblyBuildTarget, string> (AssemblyBuildTarget.Framework, "Xamarin.Sdk"));
				}
			}

			assembly_build_targets.TryGetValue ("@all", out all);
			assembly_build_targets.TryGetValue ("@sdk", out sdk);

			foreach (var target in Targets) {
				var asm_build_targets = new Dictionary<string, Tuple<AssemblyBuildTarget, string>> (assembly_build_targets);

				foreach (var assembly in target.Assemblies) {
					Tuple<AssemblyBuildTarget, string> build_target;
					var asm_name = assembly.Identity;

					if (asm_build_targets.TryGetValue (asm_name, out build_target)) {
						asm_build_targets.Remove (asm_name);
					} else if (sdk != null && (Profile.IsSdkAssembly (asm_name) || Profile.IsProductAssembly (asm_name))) {
						build_target = sdk;
					} else {
						build_target = all;
					}

					if (build_target == null) {
						if (exceptions == null)
							exceptions = new List<Exception> ();
						exceptions.Add (ErrorHelper.CreateError (105, "No assembly build target was specified for '{0}'.", assembly.Identity));
						continue;
					}

					assembly.BuildTarget = build_target.Item1;
					// The default build target name is the assembly's filename, including the extension,
					// so that for instance for System.dll, we'd end up with a System.dll.framework
					// (this way it doesn't clash with the system's System.framework).
					assembly.BuildTargetName = string.IsNullOrEmpty (build_target.Item2) ? Path.GetFileName (assembly.FileName) : build_target.Item2;
				}

				foreach (var abt in asm_build_targets) {
					if (abt.Key == "@all" || abt.Key == "@sdk")
						continue;

					if (exceptions == null)
						exceptions = new List<Exception> ();
					exceptions.Add (ErrorHelper.CreateError (108, "The assembly build target '{0}' did not match any assemblies.", abt.Key));
				}

				if (exceptions != null)
					continue;

				var grouped = target.Assemblies.GroupBy ((a) => a.BuildTargetName);
				foreach (var @group in grouped) {
					var assemblies = @group.AsEnumerable ().ToArray ();

					// Check that all assemblies in a group have the same build target
					for (int i = 1; i < assemblies.Length; i++) {
						if (assemblies [0].BuildTarget != assemblies [i].BuildTarget)
							throw ErrorHelper.CreateError (102, "The assemblies '{0}' and '{1}' have the same target name ('{2}'), but different targets ('{3}' and '{4}').",
														   assemblies [0].Identity, assemblies [1].Identity, assemblies [0].BuildTargetName, assemblies [0].BuildTarget, assemblies [1].BuildTarget);
					}

					// Check that static objects must consist of only one assembly
					if (assemblies.Length != 1 && assemblies [0].BuildTarget == AssemblyBuildTarget.StaticObject)
						throw ErrorHelper.CreateError (103, "The static object '{0}' contains more than one assembly ('{1}'), but each static object must correspond with exactly one assembly.",
													   assemblies [0].BuildTargetName, string.Join ("', '", assemblies.Select ((a) => a.Identity).ToArray ()));
				}
			}


			if (exceptions != null)
				throw new AggregateException (exceptions);
		}

		public string GetLLVMOptimizations (Assembly assembly)
		{
			string opt;
			if (LLVMOptimizations.TryGetValue (assembly.FileName, out opt))
				return opt;
			if (LLVMOptimizations.TryGetValue ("all", out opt))
				return opt;
			return null;
		}

		public void SetDlsymOption (string asm, bool dlsym)
		{
			if (DlsymAssemblies == null)
				DlsymAssemblies = new List<Tuple<string, bool>> ();

			DlsymAssemblies.Add (new Tuple<string, bool> (Path.GetFileNameWithoutExtension (asm), dlsym));

			DlsymOptions = DlsymOptions.Custom;
		}

		public void ParseDlsymOptions (string options)
		{
			bool dlsym;
			if (Driver.TryParseBool (options, out dlsym)) {
				DlsymOptions = dlsym ? DlsymOptions.All : DlsymOptions.None;
			} else {
				DlsymAssemblies = new List<Tuple<string, bool>> ();

				var assemblies = options.Split (',');
				foreach (var assembly in assemblies) {
					var asm = assembly;
					if (assembly.StartsWith ("+", StringComparison.Ordinal)) {
						dlsym = true;
						asm = assembly.Substring (1);
					} else if (assembly.StartsWith ("-", StringComparison.Ordinal)) {
						dlsym = false;
						asm = assembly.Substring (1);
					} else {
						dlsym = true;
					}
					DlsymAssemblies.Add (new Tuple<string, bool> (Path.GetFileNameWithoutExtension (asm), dlsym));
				}

				DlsymOptions = DlsymOptions.Custom;
			}
		}

		public bool UseDlsym (string assembly)
		{
			string asm;

			if (DlsymAssemblies != null) {
				asm = Path.GetFileNameWithoutExtension (assembly);
				foreach (var tuple in DlsymAssemblies) {
					if (string.Equals (tuple.Item1, asm, StringComparison.Ordinal))
						return tuple.Item2;
				}
			}

			switch (DlsymOptions) {
			case DlsymOptions.All:
				return true;
			case DlsymOptions.None:
				return false;
			}

			if (EnableLLVMOnlyBitCode)
				return false;

			switch (Platform) {
			case ApplePlatform.iOS:
				return !Profile.IsSdkAssembly (Path.GetFileNameWithoutExtension (assembly));
			case ApplePlatform.TVOS:
			case ApplePlatform.WatchOS:
				return false;
			default:
				throw ErrorHelper.CreateError (71, "Unknown platform: {0}. This usually indicates a bug in Xamarin.iOS; please file a bug report at http://bugzilla.xamarin.com with a test case.", Platform);
			}
		}

		public string MonoGCParams {
			get {
				// Configure sgen to use a small nursery
				string ret = "nursery-size=512k";
				if (IsTodayExtension || Platform == ApplePlatform.WatchOS) {
					// A bit test shows different behavior
					// Sometimes apps are killed with ~100mb allocated,
					// but I've seen apps allocate up to 240+mb as well
					ret += ",soft-heap-limit=8m";
				}
				if (EnableSGenConc)
					ret += ",major=marksweep-conc";
				else
					ret += ",major=marksweep";
				return ret;
			}
		}

		public bool IsDeviceBuild { 
			get { return BuildTarget == BuildTarget.Device; } 
		}

		public bool IsSimulatorBuild { 
			get { return BuildTarget == BuildTarget.Simulator; } 
		}

		public IEnumerable<Abi> Abis {
			get { return abis; }
		}

		public BitCodeMode BitCodeMode { get; set; }

		public bool EnableAsmOnlyBitCode { get { return BitCodeMode == BitCodeMode.ASMOnly; } }
		public bool EnableLLVMOnlyBitCode { get { return BitCodeMode == BitCodeMode.LLVMOnly; } }
		public bool EnableMarkerOnlyBitCode { get { return BitCodeMode == BitCodeMode.MarkerOnly; } }
		public bool EnableBitCode { get { return BitCodeMode != BitCodeMode.None; } }

		public ICollection<Abi> AllArchitectures {
			get {
				if (all_architectures == null) {
					all_architectures = new HashSet<Abi> ();
					foreach (var abi in abis)
						all_architectures.Add (abi);
					foreach (var ext in AppExtensions) {
						foreach (var abi in ext.Abis)
							all_architectures.Add (abi);
					}
				}
				return all_architectures;
			}
		}

		public bool IsTodayExtension {
			get {
				return ExtensionIdentifier == "com.apple.widget-extension";
			}
		}

		public bool IsWatchExtension {
			get {
				return ExtensionIdentifier == "com.apple.watchkit";
			}
		}

		public bool IsTVExtension {
			get {
				return ExtensionIdentifier == "com.apple.tv-services";
			}
		}

		public bool HasFrameworksDirectory {
			get {
				if (!IsExtension)
					return true;

				if (IsWatchExtension && Platform == ApplePlatform.WatchOS)
					return true;
				
				return false;
			}
		}

		public string ExtensionIdentifier {
			get {
				if (!IsExtension)
					return null;

				var info_plist = Path.Combine (AppDirectory, "Info.plist");
				var plist = Driver.FromPList (info_plist);
				var dict = plist.Get<PDictionary> ("NSExtension");
				if (dict == null)
					return null;
				return dict.GetString ("NSExtensionPointIdentifier");
			}
		}

		public string BundleId {
			get {
				return GetStringFromInfoPList ("CFBundleIdentifier");
			}
		}

		string GetStringFromInfoPList (string key)
		{
			return GetStringFromInfoPList (AppDirectory, key);
		}

		string GetStringFromInfoPList (string directory, string key)
		{
			var info_plist = Path.Combine (directory, "Info.plist");
			if (!File.Exists (info_plist))
				return null;

			var plist = Driver.FromPList (info_plist);
			if (!plist.ContainsKey (key))
				return null;
			return plist.GetString (key);
		}

		public void SetDefaultAbi ()
		{
			if (abis == null)
				abis = new List<Abi> ();
			
			switch (Platform) {
			case ApplePlatform.iOS:
				if (abis.Count == 0) {
					if (DeploymentTarget == null || DeploymentTarget.Major >= 11) {
						abis.Add (IsDeviceBuild ? Abi.ARM64 : Abi.x86_64);
					} else {
						abis.Add (IsDeviceBuild ? Abi.ARMv7 : Abi.i386);
					}
				}
				break;
			case ApplePlatform.WatchOS:
				if (abis.Count == 0)
					throw ErrorHelper.CreateError (76, "No architecture specified (using the --abi argument). An architecture is required for {0} projects.", "Xamarin.WatchOS");
				break;
			case ApplePlatform.TVOS:
				if (abis.Count == 0)
					throw ErrorHelper.CreateError (76, "No architecture specified (using the --abi argument). An architecture is required for {0} projects.", "Xamarin.TVOS");
				break;
			default:
				throw ErrorHelper.CreateError (71, "Unknown platform: {0}. This usually indicates a bug in Xamarin.iOS; please file a bug report at http://bugzilla.xamarin.com with a test case.", Platform);
			}
		}

		public void ValidateAbi ()
		{
			var validAbis = new List<Abi> ();
			switch (Platform) {
			case ApplePlatform.iOS:
				if (IsDeviceBuild) {
					validAbis.Add (Abi.ARMv7);
					validAbis.Add (Abi.ARMv7 | Abi.Thumb);
					validAbis.Add (Abi.ARMv7 | Abi.LLVM);
					validAbis.Add (Abi.ARMv7 | Abi.LLVM | Abi.Thumb);
					validAbis.Add (Abi.ARMv7s);
					validAbis.Add (Abi.ARMv7s | Abi.Thumb);
					validAbis.Add (Abi.ARMv7s | Abi.LLVM);
					validAbis.Add (Abi.ARMv7s | Abi.LLVM | Abi.Thumb);
				} else {
					validAbis.Add (Abi.i386);
				}
				if (IsDeviceBuild) {
					validAbis.Add (Abi.ARM64);
					validAbis.Add (Abi.ARM64 | Abi.LLVM);
				} else {
					validAbis.Add (Abi.x86_64);
				}
				break;
			case ApplePlatform.WatchOS:
				if (IsDeviceBuild) {
					validAbis.Add (Abi.ARMv7k);
					validAbis.Add (Abi.ARMv7k | Abi.LLVM);
				} else {
					validAbis.Add (Abi.i386);
				}
				break;
			case ApplePlatform.TVOS:
				if (IsDeviceBuild) {
					validAbis.Add (Abi.ARM64);
					validAbis.Add (Abi.ARM64 | Abi.LLVM);
				} else {
					validAbis.Add (Abi.x86_64);
				}
				break;
			default:
				throw ErrorHelper.CreateError (71, "Unknown platform: {0}. This usually indicates a bug in Xamarin.iOS; please file a bug report at http://bugzilla.xamarin.com with a test case.", Platform);
			}

			foreach (var abi in abis) {
				if (!validAbis.Contains (abi))
					throw ErrorHelper.CreateError (75, "Invalid architecture '{0}' for {1} projects. Valid architectures are: {2}", abi, Platform, string.Join (", ", validAbis.Select ((v) => v.AsString ()).ToArray ()));
			}
		}

		public void ClearAbi ()
		{
			abis = null;
		}

		// This is to load the symbols for all assemblies, so that we can give better error messages
		// (with file name / line number information).
		public void LoadSymbols ()
		{
			foreach (var t in Targets)
				t.LoadSymbols ();
		}

		public void ParseAbi (string abi)
		{
			var res = new List<Abi> ();
			foreach (var str in abi.Split (new char [] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
				Abi value;
				switch (str) {
				case "i386":
					value = Abi.i386;
					break;
				case "x86_64":
					value = Abi.x86_64;
					break;
				case "armv7":
					value = Abi.ARMv7;
					break;
				case "armv7+llvm":
					value = Abi.ARMv7 | Abi.LLVM;
					break;
				case "armv7+llvm+thumb2":
					value = Abi.ARMv7 | Abi.LLVM | Abi.Thumb;
					break;
				case "armv7s":
					value = Abi.ARMv7s;
					break;
				case "armv7s+llvm":
					value = Abi.ARMv7s | Abi.LLVM;
					break;
				case "armv7s+llvm+thumb2":
					value = Abi.ARMv7s | Abi.LLVM | Abi.Thumb;
					break;
				case "arm64":
					value = Abi.ARM64;
					break;
				case "arm64+llvm":
					value = Abi.ARM64 | Abi.LLVM;
					break;
				case "armv7k":
					value = Abi.ARMv7k;
					break;
				case "armv7k+llvm":
					value = Abi.ARMv7k | Abi.LLVM;
					break;
				default:
					throw new MonoTouchException (15, true, "Invalid ABI: {0}. Supported ABIs are: i386, x86_64, armv7, armv7+llvm, armv7+llvm+thumb2, armv7s, armv7s+llvm, armv7s+llvm+thumb2, armv7k, armv7k+llvm, arm64 and arm64+llvm.", str);
				}

				// merge this value with any existing ARMv? already specified.
				// this is so that things like '--armv7 --thumb' work correctly.
				if (abis != null) {
					for (int i = 0; i < abis.Count; i++) {
						if ((abis [i] & Abi.ArchMask) == (value & Abi.ArchMask)) {
							value |= abis [i];
							break;
						}
					}
				}

				res.Add (value);
			}

			// We replace any existing abis, to keep the old behavior where '--armv6 --armv7' would 
			// enable only the last abi specified and disable the rest.
			abis = res;
		}

		public static string GetArchitectures (IEnumerable<Abi> abis)
		{
			var res = new List<string> ();

			foreach (var abi in abis)
				res.Add (abi.AsArchString ());

			return string.Join (", ", res.ToArray ());
		}

		public bool IsArchEnabled (Abi arch)
		{
			return IsArchEnabled (abis, arch);
		}

		public static bool IsArchEnabled (IEnumerable<Abi> abis, Abi arch)
		{
			foreach (var abi in abis) {
				if ((abi & arch) != 0)
					return true;
			}
			return false;
		}

		public void BuildAll ()
		{
			var allapps = new List<Application> ();
			allapps.Add (this); // We need to build the main app first, so that any extensions sharing code can reference frameworks built in the main app.
			allapps.AddRange (AppExtensions);

			VerifyCache ();
			allapps.ForEach ((v) => v.BuildInitialize ());
			DetectCodeSharing ();
			allapps.ForEach ((v) => v.BuildManaged ());
			allapps.ForEach ((v) => v.BuildNative ());
			BuildBundle ();
			allapps.ForEach ((v) => v.BuildEnd ());
		}

		void VerifyCache ()
		{
			var valid = true;

			// First make sure that all the caches (both for the container app and any app extensions) are valid.
			// Due to code sharing it's safest to rebuild everything if any cache ends up out-of-date.
			if (Driver.Force) {
				Driver.Log (3, $"A full rebuild has been forced by the command line argument -f.");
				valid = false;
			} else if (!Cache.IsCacheValid ()) {
				Driver.Log (3, $"A full rebuild has been forced because the cache for {Name} is not valid.");
				valid = false;
			} else {
				foreach (var appex in AppExtensions) {
					if (appex.Cache.IsCacheValid ())
						continue;

					Driver.Log (3, $"A full rebuild has been forced because the cache for {appex.Name} is not valid.");
					valid = false;
					break;
				}
			}

			if (valid)
				return;

			// Something's not valid anymore, so clean everything.
			Cache.Clean ();
			// Make sure everything is rebuilt no matter what, the cache is not
			// the only location taken into account when determing if something
			// needs to be rebuilt.
			Driver.Force = true;
			AppExtensions.ForEach ((v) => v.Cache.Clean ());
		}

		void BuildInitialize ()
		{
			Initialize ();
			ValidateAbi ();
			SelectRegistrar ();
			ExtractNativeLinkInfo ();
			SelectNativeCompiler ();
		}

		void BuildManaged ()
		{
			ProcessAssemblies ();
		}

		void BuildNative ()
		{
			// Everything that can be parallelized is put into a list of tasks,
			// which are then executed at the end. 
			build_tasks = new BuildTasks ();

			Driver.Watch ("Generating build tasks", 1);

			CompilePInvokeWrappers ();
			BuildApp ();

			if (Driver.Dot)
				build_tasks.Dot (Path.Combine (Cache.Location, "build.dot"));

			Driver.Watch ("Building build tasks", 1);
			build_tasks.Execute ();
		}

		void BuildEnd ()
		{
			// TODO: make more of the below actions parallelizable

			BuildDsymDirectory ();
			BuildMSymDirectory ();
			StripNativeCode ();
			BundleAssemblies ();

			WriteNotice ();
			GenerateRuntimeOptions ();

			if (Cache.IsCacheTemporary) {
				// If we used a temporary directory we created ourselves for the cache
				// (in which case it's more a temporary location where we store the 
				// temporary build products than a cache), it will not be used again,
				// so just delete it.
				try {
					Directory.Delete (Cache.Location, true);
				} catch {
					// Don't care.
				}
			} else {
				// Write the cache data as the last step, so there is no half-done/incomplete (but yet detected as valid) cache.
				Cache.ValidateCache ();
			}

			Console.WriteLine ("{0} built successfully.", AppDirectory);
		}

		bool no_framework;
		public void SetDefaultFramework ()
		{
			// If no target framework was specified, check if we're referencing Xamarin.iOS.dll.
			// It's an error if neither target framework nor Xamarin.iOS.dll is not specified
			if (!Driver.HasTargetFramework) {
				foreach (var reference in References) {
					var name = Path.GetFileName (reference);
					switch (name) {
					case "Xamarin.iOS.dll":
						Driver.TargetFramework = TargetFramework.Xamarin_iOS_1_0;
						break;
					case "Xamarin.TVOS.dll":
					case "Xamarin.WatchOS.dll":
						throw ErrorHelper.CreateError (86, "A target framework (--target-framework) must be specified when building for TVOS or WatchOS.");
					}

					if (Driver.HasTargetFramework)
						break;
				}
			}

			if (!Driver.HasTargetFramework) {
				// Set a default target framework to show errors in the least confusing order.
				Driver.TargetFramework = TargetFramework.Xamarin_iOS_1_0;
				no_framework = true;
			}
		}

		string FormatAssemblyBuildTargets ()
		{
			var sb = new StringBuilder ();
			foreach (var foo in assembly_build_targets) {
				if (sb.Length > 0)
					sb.Append (" ");
				sb.Append ("--assembly-build-target:");
				sb.Append (foo.Key);
				sb.Append ("=").Append (foo.Value.Item1.ToString ().ToLower ());
				if (!string.IsNullOrEmpty (foo.Value.Item2))
					sb.Append ("=").Append (foo.Value.Item2);
			}
			return sb.ToString ();
		}

		void DetectCodeSharing ()
		{
			if (AppExtensions.Count == 0)
				return;

			if (!IsDeviceBuild)
				return;

			if (NoDevCodeShare) {
				// This is not a warning because then there would be no way to get a warning-less build if you for some reason wanted
				// a configuration that ends up disabling code sharing. In other words: if you want a configuration that causes mtouch
				// to disable code sharing, explicitly disabling code sharing will shut up all warnings about it.
				Driver.Log (2, "Native code sharing has been disabled in the main app; no code sharing with extensions will occur.");
				return;
			}

			if (Platform == ApplePlatform.iOS && DeploymentTarget.Major < 8) {
				// This is a limitation it's technically possible to fix (we can build all extensions into frameworks, and the main app to static objects).
				// It would make our code a bit more complicated though, and would only be valuable for apps that target iOS 6 or iOS 7 and has more than one extension.
				ErrorHelper.Warning (112, "Native code sharing has been disabled because {0}", $"the container app's deployment target is earlier than iOS 8.0 (it's {DeploymentTarget}).");
				return;
			}

			// No I18N assemblies can be included
			if (I18n != Mono.Linker.I18nAssemblies.None) {
				// This is a limitation it's technically possible to fix.
				ErrorHelper.Warning (112, "Native code sharing has been disabled because the container app includes I18N assemblies ({0}).", I18n);
				return;
			}

			List<Application> candidates = new List<Application> ();
			foreach (var appex in AppExtensions) {
				if (appex.IsWatchExtension)
					continue;

				if (appex.NoDevCodeShare) {
					// This is not a warning because then there would be no way to get a warning-less build if you for some reason wanted
					// a configuration that ends up disabling code sharing. In other words: if you want a configuration that causes mtouch
					// to disable code sharing, explicitly disabling code sharing will shut up all warnings about it.
					Driver.Log (2, "Native code sharing has been disabled in the extension {0}; no code sharing with the main will occur for this extension.", appex.Name);
					continue;
				}

				if (BitCodeMode != appex.BitCodeMode) {
					ErrorHelper.Warning (113, "Native code sharing has been disabled for the extension '{0}' because {1}", appex.Name, $"the bitcode options differ between the container app ({appex.BitCodeMode}) and the extension ({BitCodeMode}).");
					continue;
				}

				bool applicable = true;
				// The --assembly-build-target arguments must be identical.
				// We can probably lift this requirement (at least partially) at some point,
				// but for now it makes our code simpler.
				if (assembly_build_targets.Count != appex.assembly_build_targets.Count) {
					ErrorHelper.Warning (113, "Native code sharing has been disabled for the extension '{0}' because {1}", appex.Name, $"the --assembly-build-target options are different between the container app ({FormatAssemblyBuildTargets ()}) and the extension ({appex.FormatAssemblyBuildTargets ()}).");
					continue;
				}

				foreach (var key in assembly_build_targets.Keys) {
					Tuple<AssemblyBuildTarget, string> appex_value;
					if (!appex.assembly_build_targets.TryGetValue (key, out appex_value)) {
						ErrorHelper.Warning (113, "Native code sharing has been disabled for the extension '{0}' because {1}", appex.Name, $"the --assembly-build-target options are different between the container app ({FormatAssemblyBuildTargets ()}) and the extension ({appex.FormatAssemblyBuildTargets ()}).");
						applicable = false;
						break;
					}

					var value = assembly_build_targets [key];
					if (value.Item1 != appex_value.Item1 || value.Item2 != appex_value.Item2) {
						ErrorHelper.Warning (113, "Native code sharing has been disabled for the extension '{0}' because {1}", appex.Name, $"the --assembly-build-target options are different between the container app ({FormatAssemblyBuildTargets ()}) and the extension ({appex.FormatAssemblyBuildTargets ()}).");
						applicable = false;
						break;
					}
				}

				if (!applicable)
					continue;
				
				// No I18N assemblies can be included
				if (appex.I18n != Mono.Linker.I18nAssemblies.None) {
					ErrorHelper.Warning (113, "Native code sharing has been disabled for the extension '{0}' because {1}", appex.Name, $"the I18N assemblies are different between the container app ({I18n}) and the extension ({appex.I18n}).");
					continue;
				}

				// All arguments to the AOT compiler must be identical
				if (AotArguments != appex.AotArguments) {
					ErrorHelper.Warning (113, "Native code sharing has been disabled for the extension '{0}' because {1}", appex.Name, $"the arguments to the AOT compiler are different between the container app ({AotArguments}) and the extension ({appex.AotArguments}).");
					continue;
				}

				if (AotOtherArguments != appex.AotOtherArguments) {
					ErrorHelper.Warning (113, "Native code sharing has been disabled for the extension '{0}' because {1}", appex.Name, $"the other arguments to the AOT compiler are different between the container app ({AotOtherArguments}) and the extension ({appex.AotOtherArguments}).");
					continue;
				}

				if (IsLLVM != appex.IsLLVM) {
					ErrorHelper.Warning (113, "Native code sharing has been disabled for the extension '{0}' because {1}", appex.Name, $"LLVM is not enabled or disabled in both the container app ({IsLLVM}) and the extension ({appex.IsLLVM}).");
					continue;
				}

				if (LinkMode != appex.LinkMode) {
					ErrorHelper.Warning (113, "Native code sharing has been disabled for the extension '{0}' because {1}", appex.Name, $"the managed linker settings are different between the container app ({LinkMode}) and the extension ({appex.LinkMode}).");
					continue;
				}

				if (LinkMode != LinkMode.None) {
					var linkskipped_same = !LinkSkipped.Except (appex.LinkSkipped).Any () && !appex.LinkSkipped.Except (LinkSkipped).Any ();
					if (!linkskipped_same) {
						ErrorHelper.Warning (113, "Native code sharing has been disabled for the extension '{0}' because {1}", appex.Name, $"the skipped assemblies for the managed linker are different between the container app ({string.Join (", ", LinkSkipped)}) and the extension ({string.Join (", ", appex.LinkSkipped)}).");
						continue;
					}

					if (Definitions.Count > 0) {
						ErrorHelper.Warning (112, "Native code sharing has been disabled because {0}", $"the container app has custom xml definitions for the managed linker ({string.Join (", ", Definitions)}).");
						continue;
					}

					if (appex.Definitions.Count > 0) {
						ErrorHelper.Warning (113, "Native code sharing has been disabled for the extension '{0}' because {1}", appex.Name, $"the extension has custom xml definitions for the managed linker ({string.Join (", ", appex.Definitions)}).");
						continue;
					}
				}

				// Check that the Abis are matching
				foreach (var abi in appex.Abis) {
					var matching = abis.FirstOrDefault ((v) => (v & Abi.ArchMask) == (abi & Abi.ArchMask));
					if (matching == Abi.None) {
						// Example: extension has arm64+armv7, while the main app has only arm64.
						ErrorHelper.Warning (113, "Native code sharing has been disabled for the extension '{0}' because {1}", appex.Name, $"the container app does not build for the ABI {abi} (while the extension is building for this ABI).");
						applicable = false;
						break;
					} else if (matching != abi) {
						// Example: extension has arm64+llvm, while the main app has only arm64.
						ErrorHelper.Warning (113, "Native code sharing has been disabled for the extension '{0}' because {1}", appex.Name, $"the container app is building for the ABI {matching}, which is not compatible with the extension's ABI ({abi}).");
						applicable = false;
						break;
					}
				}

				// Check if there aren't referenced assemblies from different sources
				foreach (var target in Targets) {
					var appexTarget = appex.Targets.SingleOrDefault ((v) => v.Is32Build == target.Is32Build);
					if (appexTarget == null)
						continue; // container is fat, appex isn't. This is not a problem.
					foreach (var kvp in appexTarget.Assemblies.Hashed) {
						Assembly asm;
						if (!target.Assemblies.TryGetValue (kvp.Key, out asm))
							continue; // appex references an assembly the main app doesn't. This is fine.
						if (asm.FullPath != kvp.Value.FullPath && !Cache.CompareFiles (asm.FullPath, kvp.Value.FullPath, true)) {
							applicable = false; // app references an assembly with the same name as the main app, but from a different location and not identical. This is not fine.
							ErrorHelper.Warning (113, "Native code sharing has been disabled for the extension '{0}' because {1}", appex.Name, $"the container app is referencing the assembly '{asm.Identity}' from '{asm.FullPath}', while the extension references a different version from '{kvp.Value.FullPath}'.");
							break;
						}
					}
				}

				if (!applicable)
					continue;

				candidates.Add (appex);
				appex.IsCodeShared = true;
				IsCodeShared = true;

				Driver.Log (2, "The app '{1}' and the extension '{0}' will share code.", appex.Name, Name);
			}

			if (candidates.Count > 0)
				SharedCodeApps.AddRange (candidates);
		}

		void Initialize ()
		{
			if (EnableDebug && IsLLVM)
				ErrorHelper.Warning (3003, "Debugging is not supported when building with LLVM. Debugging has been disabled.");

			if (!IsLLVM && (EnableAsmOnlyBitCode || EnableLLVMOnlyBitCode))
				throw ErrorHelper.CreateError (3008, "Bitcode support requires the use of LLVM (--abi=arm64+llvm etc.)");

			if (IsLLVM && Platform == ApplePlatform.WatchOS && BitCodeMode != BitCodeMode.LLVMOnly) {
				ErrorHelper.Warning (111, "Bitcode has been enabled because this version of Xamarin.iOS does not support building watchOS projects using LLVM without enabling bitcode.");
				BitCodeMode = BitCodeMode.LLVMOnly;
			}

			if (EnableDebug) {
				if (!DebugTrack.HasValue) {
					DebugTrack = IsSimulatorBuild;
				}
			} else {
				if (DebugTrack.HasValue) {
					ErrorHelper.Warning (32, "The option '--debugtrack' is ignored unless '--debug' is also specified.");
				}
				DebugTrack = false;
			}

			if (EnableAsmOnlyBitCode)
				LLVMAsmWriter = true;

			List<Exception> exceptions = null;
			foreach (var root in RootAssemblies) {
				if (File.Exists (root))
					continue;

				if (exceptions == null)
					exceptions = new List<Exception> ();

				if (root [0] == '-' || root [0] == '/') {
					exceptions.Add (ErrorHelper.CreateError (18, "Unknown command line argument: '{0}'", root));
				} else {
					exceptions.Add (ErrorHelper.CreateError (7, "The root assembly '{0}' does not exist", root));
				}
			}
			if (exceptions?.Count > 0)
				throw new AggregateException (exceptions);


			if (no_framework)
				throw ErrorHelper.CreateError (96, "No reference to Xamarin.iOS.dll was found.");

			// Add a reference to the platform assembly if none has been added, and check that we're not referencing
			// any platform assemblies from another platform.
			var platformAssemblyReference = false;
			foreach (var reference in References) {
				var name = Path.GetFileNameWithoutExtension (reference);
				if (name == Driver.GetProductAssembly (this)) {
					platformAssemblyReference = true;
				} else {
					switch (name) {
					case "Xamarin.iOS":
					case "Xamarin.TVOS":
					case "Xamarin.WatchOS":
						throw ErrorHelper.CreateError (41, "Cannot reference '{0}' in a {1} app.", Path.GetFileName (reference), Driver.TargetFramework.Identifier);
					}
				}
			}
			if (!platformAssemblyReference) {
				ErrorHelper.Warning (85, "No reference to '{0}' was found. It will be added automatically.", Driver.GetProductAssembly (this) + ".dll");
				References.Add (Path.Combine (Driver.GetPlatformFrameworkDirectory (this), Driver.GetProductAssembly (this) + ".dll"));
			}

			((MonoTouchProfile)Profile.Current).SetProductAssembly (Driver.GetProductAssembly (this));

			var FrameworkDirectory = Driver.GetPlatformFrameworkDirectory (this);
			foreach (var root in RootAssemblies) {
				string root_wo_ext = Path.GetFileNameWithoutExtension (root);
				if (Profile.IsSdkAssembly (root_wo_ext) || Profile.IsProductAssembly (root_wo_ext))
					throw new MonoTouchException (3, true, "Application name '{0}.exe' conflicts with an SDK or product assembly (.dll) name.", root_wo_ext);
			}

			if (IsDualBuild) {
				var target32 = new Target (this);
				var target64 = new Target (this);

				target32.ArchDirectory = Path.Combine (Cache.Location, "32");
				target32.TargetDirectory = IsSimulatorBuild ? Path.Combine (AppDirectory, ".monotouch-32") : Path.Combine (target32.ArchDirectory, "Output");
				target32.AppTargetDirectory = Path.Combine (AppDirectory, ".monotouch-32");
				target32.Resolver.ArchDirectory = Driver.GetArch32Directory (this);
				target32.Abis = SelectAbis (abis, Abi.Arch32Mask);

				target64.ArchDirectory = Path.Combine (Cache.Location, "64");
				target64.TargetDirectory = IsSimulatorBuild ? Path.Combine (AppDirectory, ".monotouch-64") : Path.Combine (target64.ArchDirectory, "Output");
				target64.AppTargetDirectory = Path.Combine (AppDirectory, ".monotouch-64");
				target64.Resolver.ArchDirectory = Driver.GetArch64Directory (this);
				target64.Abis = SelectAbis (abis, Abi.Arch64Mask);

				Targets.Add (target64);
				Targets.Add (target32);
			} else {
				var target = new Target (this);

				target.TargetDirectory = AppDirectory;
				target.AppTargetDirectory = IsSimulatorBuild ? AppDirectory : Path.Combine (AppDirectory, Is64Build ? ".monotouch-64" : ".monotouch-32");
				target.ArchDirectory = Cache.Location;
				target.Resolver.ArchDirectory = Path.Combine (FrameworkDirectory, "..", "..", Is32Build ? "32bits" : "64bits");
				target.Abis = abis;

				Targets.Add (target);

				// Make sure there aren't any lingering .monotouch-* directories.
				if (IsSimulatorBuild) {
					var dir = Path.Combine (AppDirectory, ".monotouch-32");
					if (Directory.Exists (dir))
						Directory.Delete (dir, true);
					dir = Path.Combine (AppDirectory, ".monotouch-64");
					if (Directory.Exists (dir))
						Directory.Delete (dir, true);
				}
			}

			var RootDirectory = Path.GetDirectoryName (Path.GetFullPath (RootAssemblies [0]));
			foreach (var target in Targets) {
				target.Resolver.FrameworkDirectory = FrameworkDirectory;
				target.Resolver.RootDirectory = RootDirectory;
				target.Resolver.EnableRepl = EnableRepl;
				target.ManifestResolver.EnableRepl = EnableRepl;
				target.ManifestResolver.FrameworkDirectory = target.Resolver.FrameworkDirectory;
				target.ManifestResolver.RootDirectory = target.Resolver.RootDirectory;
				target.ManifestResolver.ArchDirectory = target.Resolver.ArchDirectory;
				target.Initialize (target == Targets [0]);

				if (!Directory.Exists (target.TargetDirectory))
					Directory.CreateDirectory (target.TargetDirectory);
			}

			if (string.IsNullOrEmpty (ExecutableName)) {
				var bundleExecutable = GetStringFromInfoPList ("CFBundleExecutable");
				ExecutableName = bundleExecutable ?? Path.GetFileNameWithoutExtension (RootAssemblies [0]);
			}

			if (ExecutableName != Path.GetFileNameWithoutExtension (AppDirectory))
				ErrorHelper.Warning (30, "The executable name ({0}) and the app name ({1}) are different, this may prevent crash logs from getting symbolicated properly.",
					ExecutableName, Path.GetFileName (AppDirectory));
			
			if (IsExtension && Platform == ApplePlatform.iOS && SdkVersion < new Version (8, 0))
				throw new MonoTouchException (45, true, "--extension is only supported when using the iOS 8.0 (or later) SDK.");

			if (IsExtension && Platform != ApplePlatform.iOS && Platform != ApplePlatform.WatchOS && Platform != ApplePlatform.TVOS)
				throw new MonoTouchException (72, true, "Extensions are not supported for the platform '{0}'.", Platform);

			if (!IsExtension && Platform == ApplePlatform.WatchOS)
				throw new MonoTouchException (77, true, "WatchOS projects must be extensions.");
		
#if ENABLE_BITCODE_ON_IOS
			if (Platform == ApplePlatform.iOS)
				DeploymentTarget = new Version (9, 0);
#endif

			if (DeploymentTarget == null)
				DeploymentTarget = Xamarin.SdkVersions.GetVersion (Platform);

			if (Platform == ApplePlatform.iOS && (HasDynamicLibraries || HasFrameworks) && DeploymentTarget.Major < 8) {
				ErrorHelper.Warning (78, "Incremental builds are enabled with a deployment target < 8.0 (currently {0}). This is not supported (the resulting application will not launch on iOS 9), so the deployment target will be set to 8.0.", DeploymentTarget);
				DeploymentTarget = new Version (8, 0);
			}

			if (!package_managed_debug_symbols.HasValue) {
				package_managed_debug_symbols = EnableDebug;
			} else if (package_managed_debug_symbols.Value && IsLLVM) {
				ErrorHelper.Warning (3007, "Debug info files (*.mdb / *.pdb) will not be loaded when llvm is enabled.");
			}

			if (!enable_msym.HasValue)
				enable_msym = !EnableDebug && IsDeviceBuild;

			if (!UseMonoFramework.HasValue && DeploymentTarget >= new Version (8, 0)) {
				if (IsExtension) {
					UseMonoFramework = true;
					Driver.Log (2, $"The extension {Name} will automatically link with Mono.framework.");
				} else if (Extensions.Count > 0) {
					UseMonoFramework = true;
					Driver.Log (2, "Automatically linking with Mono.framework because this is an app with extensions");
				}
			}

			if (!UseMonoFramework.HasValue)
				UseMonoFramework = false;
			
			if (UseMonoFramework.Value)
				Frameworks.Add (Path.Combine (Driver.GetProductFrameworksDirectory (this), "Mono.framework"));

			if (!PackageMonoFramework.HasValue) {
				if (!IsExtension && Extensions.Count > 0 && !UseMonoFramework.Value) {
					// The main app must package the Mono framework if we have extensions, even if it's not linking with
					// it. This happens when deployment target < 8.0 for the main app.
					PackageMonoFramework = true;
				} else {
					// Package if we're not an extension and we're using the mono framework.
					PackageMonoFramework = UseMonoFramework.Value && !IsExtension;
				}
			}

			if (Frameworks.Count > 0) {
				switch (Platform) {
				case ApplePlatform.iOS:
					if (DeploymentTarget < new Version (8, 0))
						throw ErrorHelper.CreateError (65, "Xamarin.iOS only supports embedded frameworks when deployment target is at least 8.0 (current deployment target: '{0}'; embedded frameworks: '{1}')", DeploymentTarget, string.Join (", ", Frameworks.ToArray ()));
					break;
				case ApplePlatform.WatchOS:
					if (DeploymentTarget < new Version (2, 0))
						throw ErrorHelper.CreateError (65, "Xamarin.iOS only supports embedded frameworks when deployment target is at least 2.0 (current deployment target: '{0}'; embedded frameworks: '{1}')", DeploymentTarget, string.Join (", ", Frameworks.ToArray ()));
					break;
				case ApplePlatform.TVOS:
					// All versions of tvOS support extensions
					break;
				default:
					throw ErrorHelper.CreateError (71, "Unknown platform: {0}. This usually indicates a bug in Xamarin.iOS; please file a bug report at http://bugzilla.xamarin.com with a test case.", Platform);
				}
			}

			if (IsDeviceBuild) {
				switch (BitCodeMode) {
				case BitCodeMode.ASMOnly:
					if (Platform == ApplePlatform.WatchOS)
						throw ErrorHelper.CreateError (83, "asm-only bitcode is not supported on watchOS. Use either --bitcode:marker or --bitcode:full.");
					break;
				case BitCodeMode.LLVMOnly:
				case BitCodeMode.MarkerOnly:
					break;
				case BitCodeMode.None:
					// If neither llvmonly nor asmonly is enabled, enable markeronly.
					if (Platform == ApplePlatform.TVOS || Platform == ApplePlatform.WatchOS)
						BitCodeMode = BitCodeMode.MarkerOnly;
					break;
				}
			}

			if (EnableBitCode && IsSimulatorBuild)
				throw ErrorHelper.CreateError (84, "Bitcode is not supported in the simulator. Do not pass --bitcode when building for the simulator.");

			Namespaces.Initialize ();

			if (Embeddinator) {
				// The assembly we're embedding doesn't necessarily reference our platform assembly, but we still need it.
				RootAssemblies.Add (Path.Combine (Driver.GetPlatformFrameworkDirectory (this), Driver.GetProductAssembly (this) + ".dll"));
			}

			if (Platform == ApplePlatform.iOS) {
				if (DeploymentTarget.Major >= 11 && Is32Build) {
					var invalidArches = abis.Where ((v) => (v & Abi.Arch32Mask) != 0);
					throw ErrorHelper.CreateError (116, $"Invalid architecture: {invalidArches.First ()}. 32-bit architectures are not supported when deployment target is 11 or later.");
				}
			}

			InitializeCommon ();

			Driver.Watch ("Resolve References", 1);
		}
		
		void SelectRegistrar ()
		{
			// If the default values are changed, remember to update CanWeSymlinkTheApplication
			// and main.m (default value for xamarin_use_old_dynamic_registrar must match).
			if (Registrar == RegistrarMode.Default) {
				if (IsDeviceBuild) {
					Registrar = RegistrarMode.Static;
				} else { /* if (app.IsSimulatorBuild) */
					Registrar = RegistrarMode.Dynamic;
				}
			}

			foreach (var target in Targets)
				target.SelectStaticRegistrar ();
		}

		// Select all abi from the list matching the specified mask.
		List<Abi> SelectAbis (IEnumerable<Abi> abis, Abi mask)
		{
			var rv = new List<Abi> ();
			foreach (var abi in abis) {
				if ((abi & mask) != 0)
					rv.Add (abi);
			}
			return rv;
		}

		public string AssemblyName {
			get {
				return Path.GetFileName (RootAssemblies [0]);
			}
		}

		public string Executable {
			get {
				if (Embeddinator)
					return Path.Combine (AppDirectory, "Frameworks", ExecutableName + ".framework", ExecutableName);
				return Path.Combine (AppDirectory, ExecutableName);
			}
		}

		void ProcessAssemblies ()
		{
			// This can be parallelized once we determine the linker doesn't use any static state.
			foreach (var target in Targets)	{
				if (target.CanWeSymlinkTheApplication ()) {
					target.Symlink ();
				} else {
					target.ProcessAssemblies ();
				}
			}

			// Deduplicate files from the Build directory. We need to do this before the AOT
			// step, so that we can ignore timestamp/GUID in assemblies (the GUID is
			// burned into the AOT assembly, so after that we'll need the original assembly.
			if (IsDualBuild && IsDeviceBuild) {
				// All the assemblies are now in BuildDirectory.
				var t1 = Targets [0];
				var t2 = Targets [1];

				foreach (var f1 in Directory.GetFileSystemEntries (t1.BuildDirectory)) {
					var f2 = Path.Combine (t2.BuildDirectory, Path.GetFileName (f1));
					if (!File.Exists (f2))
						continue;
					var ext = Path.GetExtension (f1).ToUpperInvariant ();
					var is_assembly = ext == ".EXE" || ext == ".DLL";
					if (!is_assembly)
						continue;

					if (!Cache.CompareAssemblies (f1, f2, true))
						continue;

					Driver.Log (1, "Targets {0} and {1} found to be identical", f1, f2);
					// Don't use symlinks, since it just gets more complicated
					// For instance: on rebuild, when should the symlink be updated and when
					// should the target of the symlink be updated? And all the usages
					// must be audited to ensure the right thing is done...
					Driver.CopyAssembly (f1, f2);
				}
			}
		}

		void CompilePInvokeWrappers ()
		{
			foreach (var target in Targets)
				target.CompilePInvokeWrappers ();
		}

		void BuildApp ()
		{
			SelectAssemblyBuildTargets (); // This must be done after the linker has run, since the linker may bring in more assemblies than only those referenced explicitly.

			foreach (var target in Targets) {
				if (target.CanWeSymlinkTheApplication ())
					continue;

				target.ComputeLinkerFlags ();
				target.Compile ();
				target.NativeLink (build_tasks);
			}
		}

		void WriteNotice ()
		{
			if (!IsDeviceBuild || IsExtension)
				return;

			if (Embeddinator)
				return;

			WriteNotice (AppDirectory);
		}

		void WriteNotice (string directory)
		{
			var path = Path.Combine (directory, "NOTICE");
			if (Directory.Exists (path))
				throw new MonoTouchException (1016, true, "Failed to create the NOTICE file because a directory already exists with the same name.");

			try {
				// write license information inside the .app
				StringBuilder sb = new StringBuilder ();
				sb.Append ("Xamarin built applications contain open source software.  ");
				sb.Append ("For detailed attribution and licensing notices, please visit...");
				sb.AppendLine ().AppendLine ().Append ("http://xamarin.com/mobile-licensing").AppendLine ();
				Driver.WriteIfDifferent (path, sb.ToString ());
			} catch (Exception ex) {
				throw new MonoTouchException (1017, true, ex, "Failed to create the NOTICE file: {0}", ex.Message);
			}
		}

		public static void CopyMSymData (string src, string dest)
		{
			if (string.IsNullOrEmpty (src) || string.IsNullOrEmpty (dest))
				return;
			if (!Directory.Exists (src)) // got no aot data
				return;

			var p = new Process ();
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardError = true;
			p.StartInfo.FileName = "mono-symbolicate";
			p.StartInfo.Arguments = $"store-symbols \"{src}\" \"{dest}\"";

			try {
				if (p.Start ()) {
					var error = p.StandardError.ReadToEnd();
					p.WaitForExit ();
					if (p.ExitCode == 0)
						return;
					else {
						ErrorHelper.Warning (95, $"Aot files could not be copied to the destination directory {dest}: {error}"); 
						return;
					}
				}

				ErrorHelper.Warning (95, $"Aot files could not be copied to the destination directory {dest}: Could not start process."); 
				return;
			}
			catch (Exception e) {
				ErrorHelper.Warning (95, e, $"Aot files could not be copied to the destination directory {dest}: Could not start process."); 
				return;
			}
		}

		void BuildBundle ()
		{
			Driver.Watch ($"Building app bundle for {Name}", 1);

			// First we must build every appex's bundle, otherwise we won't be able
			// to copy any frameworks the appex is using to the container app.
			foreach (var appex in AppExtensions)
				appex.BuildBundle ();

			// Make sure we bundle Mono.framework if we need to.
			if (PackageMonoFramework == true) {
				BundleFileInfo info;
				var name = "Frameworks/Mono.framework";
				bundle_files [name] = info = new BundleFileInfo ();
				info.Sources.Add (GetLibMono (AssemblyBuildTarget.Framework));
			}

			// Collect files to bundle from every target
			if (Targets.Count == 1) {
				bundle_files = Targets [0].BundleFiles;
			} else {
				foreach (var target in Targets) {
					foreach (var kvp in target.BundleFiles) {
						BundleFileInfo info;
						if (!bundle_files.TryGetValue (kvp.Key, out info))
							bundle_files [kvp.Key] = info = new BundleFileInfo () { DylibToFramework = kvp.Value.DylibToFramework };
						info.Sources.UnionWith (kvp.Value.Sources);
					}
				}
			}

			// And from ourselves
			var all_assemblies = Targets.SelectMany ((v) => v.Assemblies);
			var all_frameworks = Frameworks.Concat (all_assemblies.SelectMany ((v) => v.Frameworks));
			var all_weak_frameworks = WeakFrameworks.Concat (all_assemblies.SelectMany ((v) => v.WeakFrameworks));
			foreach (var fw in all_frameworks.Concat (all_weak_frameworks)) {
				BundleFileInfo info;
				if (!Path.GetFileName (fw).EndsWith (".framework", StringComparison.Ordinal))
					continue;
				var key = $"Frameworks/{Path.GetFileName (fw)}";
				if (!bundle_files.TryGetValue (key, out info))
					bundle_files [key] = info = new BundleFileInfo ();
				info.Sources.Add (fw);
			}

			// We also need to add any frameworks from extensions
			foreach (var appex in AppExtensions) {
				foreach (var bf in appex.bundle_files.ToList ()) {
					if (!Path.GetFileName (bf.Key).EndsWith (".framework", StringComparison.Ordinal) && !bf.Value.DylibToFramework)
						continue;

					Driver.Log (3, "Copying {0} to the app's Frameworks directory because it's used by the extension {1}", bf.Key, Path.GetFileName (appex.Name));

					var appex_info = bf.Value;
					BundleFileInfo info;
					if (!bundle_files.TryGetValue (bf.Key, out info))
						bundle_files [bf.Key] = info = new BundleFileInfo ();
					info.Sources.UnionWith (appex_info.Sources);
					if (appex_info.DylibToFramework)
						info.DylibToFramework = true;
				}
			}

			// Finally copy all the files & directories
			foreach (var kvp in bundle_files) {
				var name = kvp.Key;
				var info = kvp.Value;
				var targetPath = Path.Combine (AppDirectory, name);
				var files = info.Sources;
				var isFramework = Directory.Exists (files.First ());

				if (!HasFrameworksDirectory && (isFramework || info.DylibToFramework))
					continue; // Don't copy frameworks to app extensions (except watch extensions), they go into the container app.

				if (!files.All ((v) => Directory.Exists (v) == isFramework))
					throw ErrorHelper.CreateError (99, $"Internal error: 'can't process a mix of dylibs and frameworks: {string.Join (", ", files)}'. Please file a bug report with a test case (https://bugzilla.xamarin.com).");

				if (isFramework) {
					// This is a framework
					if (files.Count > 1) {
						// If we have multiple frameworks, check if they're identical, and remove any duplicates
						var firstFile = files.First ();
						foreach (var otherFile in files.Where ((v) => v != firstFile).ToArray ()) {
							if (Cache.CompareDirectories (firstFile, otherFile, ignore_cache: true)) {
								Driver.Log (6, $"Framework '{name}' included from both '{firstFile}' and '{otherFile}', but they are identical, so the latter will be ignored.");
								files.Remove (otherFile);
								continue;
							}
						}
					}
					if (files.Count != 1) {
						var exceptions = new List<Exception> ();
						var fname = Path.GetFileName (name);
						exceptions.Add (ErrorHelper.CreateError (1035, $"Cannot include different versions of the framework '{fname}'"));
						foreach (var file in files)
							exceptions.Add (ErrorHelper.CreateError (1036, $"Framework '{fname}' included from: {file} (Related to previous error)"));
						throw new AggregateException (exceptions);
					}
					if (info.DylibToFramework)
						throw ErrorHelper.CreateError (99, $"Internal error: 'can't convert frameworks to frameworks: {files.First ()}'. Please file a bug report with a test case (https://bugzilla.xamarin.com).");
					var framework_src = files.First ();
					var framework_filename = Path.Combine (framework_src, Path.GetFileNameWithoutExtension (framework_src));
					if (!MachO.IsDynamicFramework (framework_filename)) {
						Driver.Log (1, "The framework {0} is a framework of static libraries, and will not be copied to the app.", framework_src);
					} else {
						var macho_file = Path.Combine (targetPath, Path.GetFileNameWithoutExtension (framework_src));
						var macho_info = new FileInfo (macho_file);
						var macho_last_write_time = macho_info.LastWriteTimeUtc; // this returns a date in the 17th century if the file doesn't exist.
						UpdateDirectory (framework_src, Path.GetDirectoryName (targetPath));
						if (IsDeviceBuild) {
							// Remove architectures we don't care about.
							MachO.SelectArchitectures (macho_file, AllArchitectures);
							// Strip bitcode if needed.
							macho_info.Refresh ();
							if (macho_info.LastWriteTimeUtc > macho_last_write_time) {
								// bitcode_strip will always touch the file, but we only want to strip it if it was updated.
								StripBitcode (macho_file);
							}
						}
					}
				} else {
					var targetDirectory = Path.GetDirectoryName (targetPath);
					if (!IsUptodate (files, new string [] { targetPath })) {
						Directory.CreateDirectory (targetDirectory);
						if (files.Count == 1) {
							CopyFile (files.First (), targetPath);
						} else {
							var sb = new StringBuilder ();
							foreach (var lib in files) {
								sb.Append (StringUtils.Quote (lib));
								sb.Append (' ');
							}
							sb.Append ("-create -output ");
							sb.Append (StringUtils.Quote (targetPath));
							Driver.RunLipo (sb.ToString ());
						}
						if (LibMonoLinkMode == AssemblyBuildTarget.Framework)
							Driver.XcodeRun ("install_name_tool", "-change @rpath/libmonosgen-2.0.dylib @rpath/Mono.framework/Mono " + StringUtils.Quote (targetPath));
					} else {
						Driver.Log (3, "Target '{0}' is up-to-date.", targetPath);
					}

					if (info.DylibToFramework) {
						var bundleName = Path.GetFileName (name);
						CreateFrameworkInfoPList (Path.Combine (targetDirectory, "Info.plist"), bundleName, BundleId + Path.GetFileNameWithoutExtension (bundleName), bundleName);
						CreateFrameworkNotice (targetDirectory);
					}
				}
			}

			if (Embeddinator) {
				if (IsSimulatorBuild) {
					var frameworkName = ExecutableName;
					var frameworkDirectory = Path.Combine (AppDirectory, "Frameworks", frameworkName + ".framework");
					var frameworkExecutable = Path.Combine (frameworkDirectory, frameworkName);
					Directory.CreateDirectory (frameworkDirectory);
					if (IsDualBuild) {
						if (Lipo (frameworkExecutable, Targets [0].Executable, Targets [1].Executable))
							cached_executable = true;
					} else {
						UpdateFile (Targets [0].Executable, frameworkExecutable);
					}
					CreateFrameworkInfoPList (Path.Combine (frameworkDirectory, "Info.plist"), frameworkName, BundleId + frameworkName, frameworkName);
				}
			} else if (IsDeviceBuild) {
				// If building a fat app, we need to lipo the two different executables we have together
				if (IsDualBuild) {
					if (Lipo (Executable, Targets [0].Executable, Targets [1].Executable))
						cached_executable = true;
				} else {
					cached_executable = Targets [0].CachedExecutable;
				}
			}
		}

		public void StripBitcode (string macho_file)
		{
			var sb = new StringBuilder ();
			sb.Append (StringUtils.Quote (macho_file)).Append (" ");
			switch (BitCodeMode) {
			case BitCodeMode.ASMOnly:
			case BitCodeMode.LLVMOnly:
				// do nothing, since we don't know neither if bitcode is needed (if we're publishing) or if native code is needed (not publishing).
				return;
			case BitCodeMode.MarkerOnly:
				sb.Append ("-m ");
				break;
			case BitCodeMode.None:
				sb.Append ("-r ");
				break;
			}
			sb.Append ("-o ");
			sb.Append (StringUtils.Quote (macho_file));
			Driver.XcodeRun ("bitcode_strip", sb.ToString ());
		}

		// Returns true if is up-to-date
		static bool Lipo (string output, params string [] inputs)
		{
			if (IsUptodate (inputs, new string [] { output })) {
				Driver.Log (3, "Target '{0}' is up-to-date.", output);
				return true;
			} else {
				var cmd = new StringBuilder ();
				foreach (var input in inputs) {
					cmd.Append (StringUtils.Quote (input));
					cmd.Append (' ');
				}
				cmd.Append ("-create -output ");
				cmd.Append (StringUtils.Quote (output));
				Driver.RunLipo (cmd.ToString ());
				return false;
			}
		}

		public void ExtractNativeLinkInfo ()
		{
			var exceptions = new List<Exception> ();

			foreach (var target in Targets)
				target.ExtractNativeLinkInfo (exceptions);

			if (exceptions.Count > 0)
				throw new AggregateException (exceptions);

			Driver.Watch ("Extracted native link info", 1);
		}

		public void SelectNativeCompiler ()
		{
			foreach (var t in Targets) {
				foreach (var a in t.Assemblies) {
					if (a.EnableCxx) {	
						EnableCxx = true;
						break;
					}
				}
			}

			Driver.CalculateCompilerPath (this);
		}

		public string GetLibMono (AssemblyBuildTarget build_target)
		{
			switch (build_target) {
			case AssemblyBuildTarget.StaticObject:
				return Path.Combine (Driver.GetMonoTouchLibDirectory (this), "libmonosgen-2.0.a");
			case AssemblyBuildTarget.DynamicLibrary:
				return Path.Combine (Driver.GetMonoTouchLibDirectory (this), "libmonosgen-2.0.dylib");
			case AssemblyBuildTarget.Framework:
				return Path.Combine (Driver.GetProductSdkDirectory (this), "Frameworks", "Mono.framework");
			default:
				throw ErrorHelper.CreateError (100, "Invalid assembly build target: '{0}'. Please file a bug report with a test case (http://bugzilla.xamarin.com).", build_target);
			}
		}

		public string GetLibXamarin (AssemblyBuildTarget build_target)
		{
			switch (build_target) {
			case AssemblyBuildTarget.StaticObject:
				return Path.Combine (Driver.GetMonoTouchLibDirectory (this), EnableDebug ? "libxamarin-debug.a" : "libxamarin.a");
			case AssemblyBuildTarget.DynamicLibrary:
				return Path.Combine (Driver.GetMonoTouchLibDirectory (this), EnableDebug ? "libxamarin-debug.dylib" : "libxamarin.dylib");
			case AssemblyBuildTarget.Framework:
				return Path.Combine (Driver.GetProductSdkDirectory (this), "Frameworks", EnableDebug ? "Xamarin-debug.framework" : "Xamarin.framework");
			default:
				throw ErrorHelper.CreateError (100, "Invalid assembly build target: '{0}'. Please file a bug report with a test case (http://bugzilla.xamarin.com).", build_target);
			}
		}

		// this will filter/remove warnings that are not helpful (e.g. complaining about non-matching armv6-6 then armv7-6 on fat binaries)
		// and turn the remaining of the warnings into MT5203 that MonoDevelop will be able to report as real warnings (not just logs)
		// it will also look for symbol-not-found errors and try to provide useful error messages.
		public static void ProcessNativeLinkerOutput (Target target, string output, IEnumerable<string> inputs, List<Exception> errors, bool error)
		{
			List<string> lines = new List<string> (output.Split (new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));

			// filter
			for (int i = 0; i < lines.Count; i++) {
				string line = lines [i];

				if (errors.Count > 100)
					return;

				if (line.Contains ("ld: warning: ignoring file ") && 
					line.Contains ("file was built for") && 
					line.Contains ("which is not the architecture being linked") &&
				// Only ignore warnings related to the object files we've built ourselves (assemblies, main.m, registrar.m)
					inputs.Any ((v) => line.Contains (v))) {
					continue;
				} else if (line.Contains ("ld: symbol(s) not found for architecture") && errors.Count > 0) {
					continue;
				} else if (line.Contains ("clang: error: linker command failed with exit code 1")) {
					continue;
				} else if (line.Contains ("was built for newer iOS version (5.1.1) than being linked (5.1)")) {
					continue;
				}

				if (line.Contains ("Undefined symbols for architecture")) {
					while (++i < lines.Count) {
						line = lines [i];
						if (!line.EndsWith (", referenced from:", StringComparison.Ordinal))
							break;

						var symbol = line.Replace (", referenced from:", "").Trim ('\"', ' ');
						if (symbol.StartsWith ("_OBJC_CLASS_$_", StringComparison.Ordinal)) {
							errors.Add (new MonoTouchException (5211, error, 
																"Native linking failed, undefined Objective-C class: {0}. The symbol '{1}' could not be found in any of the libraries or frameworks linked with your application.",
							                                    symbol.Replace ("_OBJC_CLASS_$_", ""), symbol));
						} else {
							var members = target.GetAllSymbols ().Find (symbol.Substring (1))?.Members;
							if (members != null && members.Any ()) {
								var member = members.First (); // Just report the first one.
								// Neither P/Invokes nor fields have IL, so we can't find the source code location.
								errors.Add (new MonoTouchException (5214, error,
									"Native linking failed, undefined symbol: {0}. " +
									"This symbol was referenced by the managed member {1}.{2}. " +
									"Please verify that all the necessary frameworks have been referenced and native libraries linked.",
									symbol, member.DeclaringType.FullName, member.Name));
							} else {
								errors.Add (new MonoTouchException (5210, error, 
							                                    "Native linking failed, undefined symbol: {0}. " +
																"Please verify that all the necessary frameworks have been referenced and native libraries are properly linked in.",
							                                    symbol));
							}
						}

						// skip all subsequent lines related to the same error.
						// we skip all subsequent lines with more indentation than the initial line.
						var indent = GetIndentation (line);
						while (i + 1 < lines.Count) {
							line = lines [i + 1];
							if (GetIndentation (lines [i + 1]) <= indent)
								break;
							i++;
						}
					}
				} else if (line.StartsWith ("duplicate symbol", StringComparison.Ordinal) && line.EndsWith (" in:", StringComparison.Ordinal)) {
					var symbol = line.Replace ("duplicate symbol ", "").Replace (" in:", "").Trim ();
					errors.Add (new MonoTouchException (5212, error, "Native linking failed, duplicate symbol: '{0}'.", symbol));

					var indent = GetIndentation (line);
					while (i + 1 < lines.Count) {
						line = lines [i + 1];
						if (GetIndentation (lines [i + 1]) <= indent)
							break;
						i++;
						errors.Add (new MonoTouchException (5213, error, "Duplicate symbol in: {0} (Location related to previous error)", line.Trim ()));
					}
				} else {
					if (line.StartsWith ("ld: ", StringComparison.Ordinal))
						line = line.Substring (4);

					line = line.Trim ();

					if (error) {
						errors.Add (new MonoTouchException (5209, error, "Native linking error: {0}", line));
					} else {
						errors.Add (new MonoTouchException (5203, error, "Native linking warning: {0}", line));
					}
				}
			}
		}

		static int GetIndentation (string line)
		{
			int rv = 0;
			if (line.Length == 0)
				return 0;

			while (true) {
				switch (line [rv]) {
				case ' ':
				case '\t':
					rv++;
					break;
				default:
					return rv;
				}
			};
		}

		// return the ids found in a macho file
		List<Guid> GetUuids (MachOFile file)
		{
			var result = new List<Guid> ();
			foreach (var cmd in file.load_commands) {
				if (cmd is UuidCommand) {
					var uuidCmd = cmd as UuidCommand;
					result.Add (new Guid (uuidCmd.uuid));
				}
			}
			return result;
		}

		// This method generates the manifest that is required by the symbolication in order to be able to debug the application, 
		// The following is an example of the manifest to be generated:
		// <mono-debug version=”1”>
		//	<app-id>com.foo.bar</app-id>
		//	<build-date>datetime</build-date>
		//	<build-id>build-id</build-id>
		//	<build-id>build-id</build-id>
		// </mono-debug>
		// where:
		// 
		// app-id: iOS/Android/Mac app/package ID. Currently for verification and user info only but in future may be used to find symbols automatically.
		// build-date: Local time in DateTime “O” format. For user info only.
		// build-id: The build UUID. Needed for HockeyApp to find the mSYM folder matching the app build. There may be more than one, as in the case of iOS multi-arch.
		void GenerateMSymManifest (Target target, string target_directory)
		{
			var manifestPath = Path.Combine (target_directory, "manifest.xml");
			if (String.IsNullOrEmpty (target_directory))
				throw new ArgumentNullException (nameof (target_directory));
			var root = new XElement ("mono-debug",
				new XAttribute("version", 1),
				new XElement ("app-id", BundleId),
				new XElement ("build-date", DateTime.Now.ToString ("O")));
				
			var file = MachO.Read (target.Executable);
			
			if (file is MachO) {
				var mfile = file as MachOFile;
				var uuids = GetUuids (mfile);
				foreach (var str in uuids) {
					root.Add (new XElement ("build-id", str));
				}
			} else if (file is IEnumerable<MachOFile>) {
				var ffile = file as IEnumerable<MachOFile>;
				foreach (var fentry in ffile) {
					var uuids = GetUuids (fentry);
					foreach (var str in uuids) {
						root.Add (new XElement ("build-id", str));
					}
				}
				
			} else {
				// do not write a manifest
				return;
			}

			// Write only if we need to update the manifest
			Driver.WriteIfDifferent (manifestPath, root.ToString ());
		}

		void CopyAotData (string src, string dest)
		{
			if (string.IsNullOrEmpty (src) || string.IsNullOrEmpty (dest)) {
				ErrorHelper.Warning (95, $"Aot files could not be copied to the destination directory {dest}"); 
				return;
			}
				
			var dir = new DirectoryInfo (src);
			if (!dir.Exists) {
				ErrorHelper.Warning (95, $"Aot files could not be copied to the destination directory {dest}"); 
				return;
			}

			var dirs = dir.GetDirectories ();
			if (!Directory.Exists (dest))
				Directory.CreateDirectory (dest);
				
			var files = dir.GetFiles ();
			foreach (var file in files) {
				var tmp = Path.Combine (dest, file.Name);
				file.CopyTo (tmp, true);
			}

			foreach (var subdir in dirs) {
				var tmp = Path.Combine (dest, subdir.Name);
				CopyAotData (subdir.FullName, tmp);
			}
		}

		public void BuildMSymDirectory ()
		{
			if (!EnableMSym)
				return;

			if (IsExtension && IsCodeShared) // we already have the data from the app
				return;

			var target_directory = string.Format ("{0}.mSYM", AppDirectory);
			if (!Directory.Exists (target_directory))
				Directory.CreateDirectory (target_directory);

			foreach (var target in Targets) {
				GenerateMSymManifest (target, target_directory);
				var msymdir = Path.Combine (target.BuildDirectory, "Msym");
				if (!Directory.Exists (msymdir)) {
					ErrorHelper.Warning (118, $"The directory {msymdir} containing the mono symbols could not be found.");
					continue;
				}
				// copy aot data must be done BEFORE we do copy the msym one
				CopyAotData (msymdir, target_directory);
				
				// copy all assemblies under mvid and with the dll and mdb/pdb
				var tmpdir =  Path.Combine (msymdir, "Msym", "tmp");
				if (!Directory.Exists (tmpdir))
					Directory.CreateDirectory (tmpdir);
					
				foreach (var asm in target.Assemblies) {
					asm.CopyToDirectory (tmpdir, reload: false, only_copy: true);
					asm.CopyAotDataFilesToDirectory (tmpdir);
				}
				// mono-symbolicate knows best
				CopyMSymData (target_directory, tmpdir);
			}
		}

		public void BuildDsymDirectory ()
		{
			if (!BuildDSym.HasValue)
				BuildDSym = IsDeviceBuild;

			if (!BuildDSym.Value)
				return;

			string dsym_dir = string.Format ("{0}.dSYM", AppDirectory);
			bool cached_dsym = false;

			if (cached_executable)
				cached_dsym = IsUptodate (new string [] { Executable }, Directory.EnumerateFiles (dsym_dir, "*", SearchOption.AllDirectories));

			if (!cached_dsym) {
				if (Directory.Exists (dsym_dir))
					Directory.Delete (dsym_dir, true);
				
				Driver.CreateDsym (AppDirectory, ExecutableName, dsym_dir);
			} else {
				Driver.Log (3, "Target '{0}' is up-to-date.", dsym_dir);
			}
			Driver.Watch ("Linking DWARF symbols", 1);
		}

		IEnumerable<Symbol> GetRequiredSymbols ()
		{
			foreach (var target in Targets) {
				foreach (var symbol in target.GetRequiredSymbols ())
					yield return symbol;
			}
		}

		bool WriteSymbolList (string filename)
		{
			var required_symbols = GetRequiredSymbols ();
			using (StreamWriter writer = new StreamWriter (filename)) {
				foreach (var symbol in required_symbols)
					writer.WriteLine ("_{0}", symbol.Name);
				foreach (var symbol in NoSymbolStrip)
					writer.WriteLine ("_{0}", symbol);
				writer.Flush ();
				return writer.BaseStream.Position > 0;
			}
		}

		void StripNativeCode (string name)
		{
			if (NativeStrip && IsDeviceBuild && !EnableDebug && string.IsNullOrEmpty (SymbolList)) {
				string symbol_file = Path.Combine (Cache.Location, "symbol-file");
				var args = new StringBuilder ();
				if (WriteSymbolList (symbol_file)) {
					args.Append ("-i ");
					args.Append ("-s ").Append (StringUtils.Quote (symbol_file)).Append (" ");
				}
				if (Embeddinator)
					args.Append ("-ux ");
				args.Append (StringUtils.Quote (Executable));
				Driver.RunStrip (args.ToString ());
				Driver.Watch ("Native Strip", 1);
			}

			if (!string.IsNullOrEmpty (SymbolList))
				WriteSymbolList (SymbolList);
		}

		public void StripNativeCode ()
		{
			if (IsDualBuild) {
				bool cached = true;
				foreach (var target in Targets)
					cached &= target.CachedExecutable;
				if (!cached)
					StripNativeCode (Executable);
			} else {
				foreach (var target in Targets) {
					if (!target.CachedExecutable)
						StripNativeCode (target.Executable);
				}
			}
		}

		public void BundleAssemblies ()
		{
			var strip = ManagedStrip && IsDeviceBuild && !EnableDebug && !PackageManagedDebugSymbols;

			var grouped = Targets.SelectMany ((Target t) => t.Assemblies).GroupBy ((Assembly asm) => asm.Identity);
			foreach (var @group in grouped) {
				var filename = @group.Key;
				var assemblies = @group.AsEnumerable ().ToArray ();
				var build_target = assemblies [0].BuildTarget;
				var size_specific = assemblies.Length > 1 && !Cache.CompareAssemblies (assemblies [0].FullPath, assemblies [1].FullPath, true, true);

				if (IsExtension && !IsWatchExtension) {
					var codeShared = assemblies.Count ((v) => v.IsCodeShared);
					if (codeShared > 0) {
						if (codeShared != assemblies.Length)
							throw ErrorHelper.CreateError (99, $"Internal error: all assemblies in a joined build target must have the same code sharing options ({string.Join (", ", assemblies.Select ((v) => v.Identity + "=" + v.IsCodeShared))}). Please file a bug report with a test case (http://bugzilla.xamarin.com).");

						continue; // These resources will be found in the main app.
					}
				}

				// Determine where to put the assembly
				switch (build_target) {
				case AssemblyBuildTarget.StaticObject:
				case AssemblyBuildTarget.DynamicLibrary:
					if (size_specific) {
						assemblies [0].CopyToDirectory (assemblies [0].Target.AppTargetDirectory, copy_debug_symbols: PackageManagedDebugSymbols, strip: strip, only_copy: true);
						assemblies [1].CopyToDirectory (assemblies [1].Target.AppTargetDirectory, copy_debug_symbols: PackageManagedDebugSymbols, strip: strip, only_copy: true);
					} else {
						assemblies [0].CopyToDirectory (AppDirectory, copy_debug_symbols: PackageManagedDebugSymbols, strip: strip, only_copy: true);
					}
					foreach (var asm in assemblies)
						asm.CopyAotDataFilesToDirectory (size_specific ? asm.Target.AppTargetDirectory : AppDirectory);
					break;
				case AssemblyBuildTarget.Framework:
					// Put our resources in a subdirectory in the framework
					// But don't use 'Resources', because the app ends up being undeployable:
					// "PackageInspectionFailed: Failed to load Info.plist from bundle at path /private/var/installd/Library/Caches/com.apple.mobile.installd.staging/temp.CR0vmK/extracted/testapp.app/Frameworks/TestApp.framework"
					var target_name = assemblies [0].BuildTargetName;
					var resource_directory = Path.Combine (AppDirectory, "Frameworks", $"{target_name}.framework", "MonoBundle");
					if (IsSimulatorBuild)
						resource_directory = Path.Combine (resource_directory, "simulator");
					if (size_specific) {
						assemblies [0].CopyToDirectory (Path.Combine (resource_directory, Path.GetFileName (assemblies [0].Target.AppTargetDirectory)), copy_debug_symbols: PackageManagedDebugSymbols, strip: strip, only_copy: true);
						assemblies [1].CopyToDirectory (Path.Combine (resource_directory, Path.GetFileName (assemblies [1].Target.AppTargetDirectory)), copy_debug_symbols: PackageManagedDebugSymbols, strip: strip, only_copy: true);
					} else {
						assemblies [0].CopyToDirectory (resource_directory, copy_debug_symbols: PackageManagedDebugSymbols, strip: strip, only_copy: true);
					}
					foreach (var asm in assemblies)
						asm.CopyAotDataFilesToDirectory (size_specific ? Path.Combine (resource_directory, Path.GetFileName (asm.Target.AppTargetDirectory)) : resource_directory);
					break;
				default:
					throw ErrorHelper.CreateError (100, "Invalid assembly build target: '{0}'. Please file a bug report with a test case (http://bugzilla.xamarin.com).", build_target);
				}
			}
		}

		public void GenerateRuntimeOptions ()
		{
			// only if the linker is disabled
			if (LinkMode != LinkMode.None)
				return;

			RuntimeOptions.Write (AppDirectory);
		}

		public void CreateFrameworkNotice (string output_path)
		{
			if (!Embeddinator)
				return;

			WriteNotice (output_path);
		}

		public void CreateFrameworkInfoPList (string output_path, string framework_name, string bundle_identifier, string bundle_name)
		{
			var sb = new StringBuilder ();
			sb.AppendLine ("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
			sb.AppendLine ("<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">");
			sb.AppendLine ("<plist version=\"1.0\">");
			sb.AppendLine ("<dict>");
			sb.AppendLine ("        <key>CFBundleDevelopmentRegion</key>");
			sb.AppendLine ("        <string>en</string>");
			sb.AppendLine ("        <key>CFBundleIdentifier</key>");
			sb.AppendLine ($"        <string>{bundle_identifier}</string>");
			sb.AppendLine ("        <key>CFBundleInfoDictionaryVersion</key>");
			sb.AppendLine ("        <string>6.0</string>");
			sb.AppendLine ("        <key>CFBundleName</key>");
			sb.AppendLine ($"        <string>{bundle_name}</string>");
			sb.AppendLine ("        <key>CFBundlePackageType</key>");
			sb.AppendLine ("        <string>FMWK</string>");
			sb.AppendLine ("        <key>CFBundleShortVersionString</key>");
			sb.AppendLine ("        <string>1.0</string>");
			sb.AppendLine ("        <key>CFBundleSignature</key>");
			sb.AppendLine ("        <string>????</string>");
			sb.AppendLine ("        <key>CFBundleVersion</key>");
			sb.AppendLine ("        <string>1.0</string>");
			sb.AppendLine ("        <key>NSPrincipalClass</key>");
			sb.AppendLine ("        <string></string>");
			sb.AppendLine ("        <key>CFBundleExecutable</key>");
			sb.AppendLine ($"        <string>{framework_name}</string>");
			sb.AppendLine ("        <key>BuildMachineOSBuild</key>");
			sb.AppendLine ("        <string>13F34</string>");
			sb.AppendLine ("        <key>CFBundleSupportedPlatforms</key>");
			sb.AppendLine ("        <array>");
			sb.AppendLine ($"                <string>{Driver.GetPlatform (this)}</string>");
			sb.AppendLine ("        </array>");
			sb.AppendLine ("        <key>DTCompiler</key>");
			sb.AppendLine ("        <string>com.apple.compilers.llvm.clang.1_0</string>");
			sb.AppendLine ("        <key>DTPlatformBuild</key>");
			sb.AppendLine ("        <string>12D508</string>");
			sb.AppendLine ("        <key>DTPlatformName</key>");
			sb.AppendLine ($"        <string>{Driver.GetPlatform (this).ToLowerInvariant ()}</string>");
			sb.AppendLine ("        <key>DTPlatformVersion</key>");
			sb.AppendLine ($"        <string>{SdkVersions.GetVersion (Platform)}</string>");
			sb.AppendLine ("        <key>DTSDKBuild</key>");
			sb.AppendLine ("        <string>12D508</string>");
			sb.AppendLine ("        <key>DTSDKName</key>");
			sb.AppendLine ($"        <string>{Driver.GetPlatform (this)}{SdkVersion}</string>");
			sb.AppendLine ("        <key>DTXcode</key>");
			sb.AppendLine ("        <string>0620</string>");
			sb.AppendLine ("        <key>DTXcodeBuild</key>");
			sb.AppendLine ("        <string>6C131e</string>");
			sb.AppendLine ("        <key>MinimumOSVersion</key>");
			sb.AppendLine ($"        <string>{DeploymentTarget.ToString ()}</string>");
			sb.AppendLine ("        <key>UIDeviceFamily</key>");
			sb.AppendLine ("        <array>");
			switch (Platform) {
			case ApplePlatform.iOS:
				sb.AppendLine ("                <integer>1</integer>");
				sb.AppendLine ("                <integer>2</integer>");
				break;
			case ApplePlatform.TVOS:
				sb.AppendLine ("                <integer>3</integer>");
				break;
			case ApplePlatform.WatchOS:
				sb.AppendLine ("                <integer>4</integer>");
				break;
			default:
				throw ErrorHelper.CreateError (71, "Unknown platform: {0}. This usually indicates a bug in Xamarin.iOS; please file a bug report at http://bugzilla.xamarin.com with a test case.", Platform);
			}
			sb.AppendLine ("        </array>");

			sb.AppendLine ("</dict>");
			sb.AppendLine ("</plist>");

			Driver.WriteIfDifferent (output_path, sb.ToString ());
		}
	}
}
