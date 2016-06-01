// Copyright 2013 Xamarin Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MonoTouch.Tuner;

using Mono.Cecil;
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
		Legacy,
		Dynamic,
		Static,
		LegacyStatic,
		LegacyDynamic,
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
		public string ExecutableName;
		public string RootAssembly;
		public BuildTarget BuildTarget;

		public Version DeploymentTarget;
		public bool EnableCxx;
		public bool EnableProfiling;
		bool? package_mdb;
		public bool PackageMdb {
			get { return package_mdb.Value; }
			set { package_mdb = value; }
		}
		bool? enable_msym;
		public bool EnableMSym {
			get { return enable_msym.Value; }
			set { enable_msym = value; }
		}
		public bool EnableRepl;

		public bool IsExtension;
		public List<string> Extensions = new List<string> (); // A list of the extensions this app contains.

		public bool FastDev;
		public string RegistrarOutputLibrary;

		public bool? EnablePie;
		public bool NativeStrip = true;
		public string SymbolList;
		public bool ManagedStrip = true;
		public List<string> NoSymbolStrip = new List<string> ();
		
		public bool? ThreadCheck;
		public DlsymOptions DlsymOptions;
		public List<Tuple<string, bool>> DlsymAssemblies;
		public bool? UseMonoFramework;
		public bool? PackageMonoFramework;

		//
		// Linker config
		//

		public bool LinkAway = true;
		public bool LinkerDumpDependencies { get; set; }
		public List<string> References = new List<string> ();
		
		public bool? BuildDSym;
		bool? generate_manifests;
		public bool GenerateManifests {
			get { return generate_manifests.Value; }
			set { generate_manifests = value; }
		}
		bool? sign;
		public bool Sign {
			get { return sign.Value; }
			set { sign = value; }
		}
		public bool Is32Build { get { return IsArchEnabled (Abi.Arch32Mask); } } // If we're targetting a 32 bit arch.
		public bool Is64Build { get { return IsArchEnabled (Abi.Arch64Mask); } } // If we're targetting a 64 bit arch.
		public bool IsDualBuild { get { return Is32Build && Is64Build; } } // if we're building both a 32 and a 64 bit version.
		public bool IsUnified { get { return !IsClassic; } } // this is true for watch
		public bool IsClassic { get { return Driver.TargetFramework.Identifier == "MonoTouch"; } }
		public bool IsLLVM { get { return IsArchEnabled (Abi.LLVM); } }

		public List<Target> Targets = new List<Target> ();

		//
		// Bundle config
		//
		public string BundleDisplayName;
		public string BundleId = "com.yourcompany.sample";
		public string MainNib = "MainWindow";
		public string Icon;
		public string CertificateName;

		public string UserGccFlags;

		// If we didn't link the final executable because the existing binary is up-to-date.
		bool cached_executable; 

		List<Abi> abis;
		HashSet<Abi> all_architectures; // all Abis used in the app, including extensions.

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
					if (assembly.StartsWith ("+")) {
						dlsym = true;
						asm = assembly.Substring (1);
					} else if (assembly.StartsWith ("-")) {
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

			switch (DlsymOptions) {
			case DlsymOptions.All:
				return true;
			case DlsymOptions.None:
				return false;
			}

			if (DlsymAssemblies != null) {
				asm = Path.GetFileNameWithoutExtension (assembly);
				foreach (var tuple in DlsymAssemblies) {
					if (string.Equals (tuple.Item1, asm, StringComparison.Ordinal))
						return tuple.Item2;
				}
			}

			if (EnableLLVMOnlyBitCode)
				return false;

			switch (Platform) {
			case ApplePlatform.iOS:
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
				if (IsTodayExtension) {
					return "nursery-size=512k,soft-heap-limit=8m";
				} else {
					return "nursery-size=512k";
				}
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
						all_architectures.Add (abi & Abi.ArchMask);
					foreach (var ext in Extensions) {
						var executable = GetStringFromInfoPList (ext, "CFBundleExecutable");
						if (string.IsNullOrEmpty (executable))
							throw ErrorHelper.CreateError (63, "Cannot find the executable in the extension {0} (no CFBundleExecutable entry in its Info.plist)", ext);
						foreach (var abi in Xamarin.MachO.GetArchitectures (Path.Combine (ext, executable)))
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

		string GetStringFromInfoPList (string key)
		{
			return GetStringFromInfoPList (AppDirectory, "Info.plist");
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
					abis.Add (IsDeviceBuild ? Abi.ARMv7 : Abi.i386);
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
				if (IsUnified) {
					if (IsDeviceBuild) {
						validAbis.Add (Abi.ARM64);
						validAbis.Add (Abi.ARM64 | Abi.LLVM);
					} else {
						validAbis.Add (Abi.x86_64);
					}
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

		public void RunRegistrar ()
		{
			// The static registrar.
			if (Registrar != RegistrarMode.Static)
				throw new MonoTouchException (67, "Invalid registrar: {0}", Registrar); // this is only called during our own build

			var registrar_m = RegistrarOutputLibrary;

			var resolvedAssemblies = new List<AssemblyDefinition> ();
			var ps = new ReaderParameters ();
			ps.AssemblyResolver = new MonoTouchResolver () {
				FrameworkDirectory = Driver.PlatformFrameworkDirectory,
				RootDirectory = Path.GetDirectoryName (RootAssembly),
			};
			resolvedAssemblies.Add (ps.AssemblyResolver.Resolve ("mscorlib"));

			var rootName = Path.GetFileNameWithoutExtension (RootAssembly);
			switch (rootName) {
			// MonoTouch.NUnitLite doesn't quite work yet, because its generated registrar code uses types
			// from the generated registrar code for MonoTouch.Dialog-1 (and there is no header file (yet)
			// for those types).
//			case "MonoTouch.NUnitLite":
//				resolvedAssemblies.Add (ps.AssemblyResolver.Resolve (rootName));
//				goto case "MonoTouch.Dialog-1";
			case "MonoTouch.Dialog-1":
				resolvedAssemblies.Add (ps.AssemblyResolver.Resolve (rootName));
				resolvedAssemblies.Add (ps.AssemblyResolver.Resolve (Driver.ProductAssembly));
				break;
			default:
				if (rootName == Driver.ProductAssembly) {
					resolvedAssemblies.Add (ps.AssemblyResolver.Resolve (rootName));
				} else {
					throw new MonoTouchException (66, "Invalid build registrar assembly: {0}", RootAssembly);
				}
				break;
			}

			BuildTarget = BuildTarget.Simulator;

			var registrar = new XamCore.Registrar.StaticRegistrar (this);
			registrar.GenerateSingleAssembly (resolvedAssemblies, Path.ChangeExtension (registrar_m, "h"), registrar_m, Path.GetFileNameWithoutExtension (RootAssembly));
		}

		public void Build ()
		{
			if (Driver.Force) {
				Driver.Log (3, "A full rebuild has been forced by the command line argument -f.");
				Cache.Clean ();
			} else if (!Cache.VerifyCache ()) {
				Driver.Force = true;
			}

			Initialize ();
			ValidateAbi ();
			SelectRegistrar ();
			ExtractNativeLinkInfo ();
			SelectNativeCompiler ();
			BuildApp ();
			WriteNotice ();
			BuildFatSharedLibraries ();
			CopyAotData ();
			BuildFinalExecutable ();
			BuildDsymDirectory ();
			BuildMSymDirectory ();
			StripNativeCode ();
			StripManagedCode ();
			GenerateAppManifests ();
			GenerateRuntimeOptions ();
			SignBundle ();

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

		bool implicit_monotouch_reference;
		public void SetDefaultFramework ()
		{
			// If no target framework was specified, check if we're referencing Xamarin.iOS.dll or monotouch.dll,
			// and then deduce the target framework.
			if (!Driver.HasTargetFramework) {
				foreach (var reference in References) {
					var name = Path.GetFileName (reference);
					switch (name) {
					case "monotouch.dll":
						Driver.TargetFramework = TargetFramework.MonoTouch_1_0;
						break;
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

			// Still nothing. Default to monotouch.dll.
			if (!Driver.HasTargetFramework) {
				implicit_monotouch_reference = true;
				Driver.TargetFramework = TargetFramework.MonoTouch_1_0;
				References.Add (Path.Combine (Driver.PlatformFrameworkDirectory, "monotouch.dll"));
			}
		}

		void Initialize ()
		{
			if (!File.Exists (RootAssembly))
				throw new MonoTouchException (7, true, "The root assembly '{0}' does not exist", RootAssembly);

			if (implicit_monotouch_reference)
				ErrorHelper.Warning (42, "No reference to either monotouch.dll or Xamarin.iOS.dll was found. A reference to monotouch.dll will be added.");
			
			// Add a reference to the platform assembly if none has been added, and check that we're not referencing
			// any platform assemblies from another platform.
			var platformAssemblyReference = false;
			foreach (var reference in References) {
				var name = Path.GetFileNameWithoutExtension (reference);
				if (name == Driver.ProductAssembly) {
					platformAssemblyReference = true;
				} else {
					switch (name) {
					case "monotouch":
					case "Xamarin.iOS":
					case "Xamarin.TVOS":
					case "Xamarin.WatchOS":
						throw ErrorHelper.CreateError (41, "Cannot reference '{0}' in a {1} app.", Path.GetFileName (reference), Driver.TargetFramework.Identifier);
					}
				}
			}
			if (!platformAssemblyReference) {
				ErrorHelper.Warning (85, "No reference to '{0}' was found. It will be added automatically.", Driver.ProductAssembly + ".dll");
				References.Add (Path.Combine (Driver.PlatformFrameworkDirectory, Driver.ProductAssembly + ".dll"));
			}

			var FrameworkDirectory = Driver.PlatformFrameworkDirectory;
			var RootDirectory = Path.GetDirectoryName (Path.GetFullPath (RootAssembly));

			((MonoTouchProfile) Profile.Current).SetProductAssembly (Driver.ProductAssembly);

			string root_wo_ext = Path.GetFileNameWithoutExtension (RootAssembly);
			if (Profile.IsSdkAssembly (root_wo_ext) || Profile.IsProductAssembly (root_wo_ext))
				throw new MonoTouchException (3, true, "Application name '{0}.exe' conflicts with an SDK or product assembly (.dll) name.", root_wo_ext);

			if (IsDualBuild) {
				var target32 = new Target (this);
				var target64 = new Target (this);

				target32.ArchDirectory = Path.Combine (Cache.Location, "32");
				target32.TargetDirectory = IsSimulatorBuild ? Path.Combine (AppDirectory, ".monotouch-32") : Path.Combine (target32.ArchDirectory, "Output");
				target32.AppTargetDirectory = Path.Combine (AppDirectory, ".monotouch-32");
				target32.Resolver.ArchDirectory = Path.Combine (Driver.PlatformFrameworkDirectory, "..", "..", "32bits");
				target32.Abis = SelectAbis (abis, Abi.Arch32Mask);

				target64.ArchDirectory = Path.Combine (Cache.Location, "64");
				target64.TargetDirectory = IsSimulatorBuild ? Path.Combine (AppDirectory, ".monotouch-64") : Path.Combine (target64.ArchDirectory, "Output");
				target64.AppTargetDirectory = Path.Combine (AppDirectory, ".monotouch-64");
				target64.Resolver.ArchDirectory = Path.Combine (Driver.PlatformFrameworkDirectory, "..", "..", "64bits");
				target64.Abis = SelectAbis (abis, Abi.Arch64Mask);

				Targets.Add (target64);
				Targets.Add (target32);
			} else {
				var target = new Target (this);

				target.TargetDirectory = AppDirectory;
				target.AppTargetDirectory = (IsSimulatorBuild || IsClassic) ? AppDirectory : Path.Combine (AppDirectory, Is64Build ? ".monotouch-64" : ".monotouch-32");
				target.ArchDirectory = Cache.Location;
				if (IsClassic) {
					target.Resolver.ArchDirectory = Driver.PlatformFrameworkDirectory;
				} else {
					target.Resolver.ArchDirectory = Path.Combine (Driver.PlatformFrameworkDirectory, "..", "..", Is32Build ? "32bits" : "64bits");
				}
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

			foreach (var target in Targets) {
				target.Resolver.FrameworkDirectory = Driver.PlatformFrameworkDirectory;
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
				ExecutableName = bundleExecutable ?? Path.GetFileNameWithoutExtension (RootAssembly);
			}

			if (ExecutableName != Path.GetFileNameWithoutExtension (AppDirectory))
				ErrorHelper.Warning (30, "The executable name ({0}) and the app name ({1}) are different, this may prevent crash logs from getting symbolicated properly.",
					ExecutableName, Path.GetFileName (AppDirectory));
			
			if (Is64Build && IsClassic)
				ErrorHelper.Error (37, "monotouch.dll is not 64-bit compatible. Either reference Xamarin.iOS.dll, or do not build for a 64-bit architecture (ARM64 or x86_64).");

			if (IsExtension && Platform == ApplePlatform.iOS && Driver.SDKVersion < new Version (8, 0))
				throw new MonoTouchException (45, true, "--extension is only supported when using the iOS 8.0 (or later) SDK.");

			if (IsExtension && Platform != ApplePlatform.iOS && Platform != ApplePlatform.WatchOS && Platform != ApplePlatform.TVOS)
				throw new MonoTouchException (72, true, "Extensions are not supported for the platform '{0}'.", Platform);

			if (!IsExtension && Platform == ApplePlatform.WatchOS)
				throw new MonoTouchException (77, true, "WatchOS projects must be extensions.");
		
#if ENABLE_BITCODE_ON_IOS
			if (Platform == ApplePlatform.iOS)
				DeploymentTarget = new Version (9, 0);
#endif

			if (DeploymentTarget == null) {
				DeploymentTarget = Xamarin.SdkVersions.GetVersion (Platform);
			} else if (DeploymentTarget < Xamarin.SdkVersions.GetMinVersion (Platform)) {
				throw new MonoTouchException (73, true, "Xamarin.iOS {0} does not support a deployment target of {1} (the minimum is {2}). Please select a newer deployment target in your project's Info.plist.", Constants.Version, DeploymentTarget, Xamarin.SdkVersions.GetMinVersion (Platform));
			} else if (DeploymentTarget > Xamarin.SdkVersions.GetVersion (Platform)) {
				throw new MonoTouchException (74, true, "Xamarin.iOS {0} does not support a deployment target of {1} (the maximum is {2}). Please select an older deployment target in your project's Info.plist or upgrade to a newer version of Xamarin.iOS.", Constants.Version, DeploymentTarget, Xamarin.SdkVersions.GetVersion (Platform));
			}

			if (Platform == ApplePlatform.iOS && FastDev && DeploymentTarget.Major < 7) {
				ErrorHelper.Warning (78, "Incremental builds are enabled with a deployment target < 7.0 (currently {0}). This is not supported (the resulting application will not launch on iOS 9), so the deployment target will be set to 7.0.", DeploymentTarget);
				DeploymentTarget = new Version (7, 0);
			}

			if (Driver.classic_only_arguments.Count > 0) {
				var exceptions = new List<Exception> ();
				foreach (var deprecated in Driver.classic_only_arguments) {
					switch (deprecated) {
					case "--nomanifest":
					case "--nosign":
						// These options default to 'true' (for Classic), so we can't deprecated them (for Classic).
						if (IsClassic)
							continue;
						break;
					}
					exceptions.Add (new MonoTouchException (16, IsUnified, "The option '{0}' has been deprecated.", deprecated));
				}
				ErrorHelper.Show (exceptions);
			}

			if (!generate_manifests.HasValue)
				generate_manifests = IsClassic;

			if (!sign.HasValue)
				sign = IsClassic;

			if (!package_mdb.HasValue) {
				package_mdb = EnableDebug;
			} else if (package_mdb.Value && IsLLVM) {
				ErrorHelper.Warning (3007, "Debug info files (*.mdb) will not be loaded when llvm is enabled.");
			}

			if (!enable_msym.HasValue)
				enable_msym = false; // Disable by default for C7 // !EnableDebug && IsDeviceBuild;

			if (!UseMonoFramework.HasValue && DeploymentTarget >= new Version (8, 0)) {
				if (IsExtension) {
					if (IsUnified) {
						UseMonoFramework = true;
						Driver.Log (2, "Automatically linking with Mono.framework because this is an extension");
					}
				} else if (IsUnified) {
					if (Extensions.Count > 0) {
						UseMonoFramework = true;
						Driver.Log (2, "Automatically linking with Mono.framework because this is an app with extensions");
					}
				}
			}

			if (!UseMonoFramework.HasValue)
				UseMonoFramework = false;
			
			if (UseMonoFramework.Value)
				Frameworks.Add (Path.Combine (Driver.ProductFrameworksDirectory, "Mono.framework"));

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
				if (DeploymentTarget < new Version (8, 0))
					throw ErrorHelper.CreateError (65, "Xamarin.iOS only supports embedded frameworks when deployment target is at least 8.0 (current deployment target: '{0}'; embedded frameworks: '{1}')", DeploymentTarget, string.Join (", ", Frameworks.ToArray ()));

				if (IsClassic)
					throw ErrorHelper.CreateError (64, "Xamarin.iOS only supports embedded frameworks with Unified projects.");
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

			if (LinkMode == LinkMode.None && Driver.SDKVersion < SdkVersions.GetVersion (Platform))
				throw ErrorHelper.CreateError (91, "This version of Xamarin.iOS requires the {0} {1} SDK (shipped with Xcode {2}) when the managed linker is disabled. Either upgrade Xcode, or enable the managed linker.", PlatformName, SdkVersions.GetVersion (Platform), SdkVersions.Xcode);

			Namespaces.Initialize ();

			var hasBitcodeCapableRuntime = false;
			switch (Platform) {
			case ApplePlatform.iOS:
#if ENABLE_BITCODE_ON_IOS
				hasBitcodeCapableRuntime = true;
#endif
				break;
			case ApplePlatform.TVOS:
			case ApplePlatform.WatchOS:
				hasBitcodeCapableRuntime = true;
				break;
			}
			if (hasBitcodeCapableRuntime && EnableProfiling && FastDev) {
				ErrorHelper.Warning (94, "Both profiling (--profiling) and incremental builds (--fastdev) is not supported when building for {0}. Incremental builds have ben disabled.", PlatformName);
				FastDev = false;
			}

			InitializeCommon ();

			Driver.Watch ("Resolve References", 1);
		}
		
		void SelectRegistrar ()
		{
			if (IsUnified) {
				// The old registrars are not implemented when using Xamarin.iOS.dll.
				if (Registrar == RegistrarMode.LegacyStatic || Registrar == RegistrarMode.LegacyDynamic || Registrar == RegistrarMode.Legacy)
					throw new MonoTouchException (38, true, "The legacy registrars (--registrar:legacy|legacystatic|legacydynamic) are not supported with the Unified API.");
			}

			// If the default values are changed, remember to update CanWeSymlinkTheApplication
			// and main.m (default value for xamarin_use_old_dynamic_registrar must match).
			if (Driver.enable_generic_nsobject && Registrar != RegistrarMode.Default)
				throw new MonoTouchException (22, true, "The options '--unsupported--enable-generics-in-registrar' and '--registrar' are not compatible.");

			if (Registrar == RegistrarMode.Default) {
				if (IsDeviceBuild) {
					Registrar = RegistrarMode.Static;
				} else { /* if (app.IsSimulatorBuild) */
					Registrar = RegistrarMode.Dynamic;
				}
			} else if (Registrar == RegistrarMode.Legacy) {
				if (IsDeviceBuild) {
					Registrar = RegistrarMode.LegacyStatic;
				} else { /* if (app.IsSimulatorBuild) */
					Registrar = RegistrarMode.LegacyDynamic;
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
				return Path.GetFileName (RootAssembly);
			}
		}

		public string Executable {
			get {
				return Path.Combine (AppDirectory, ExecutableName);
			}
		}

		void ManagedLink ()
		{
			foreach (var target in Targets)
				target.ManagedLink ();
		}

		void BuildApp ()
		{
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
			if (IsDualBuild && IsDeviceBuild && !Sign) {
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
						
					if (Driver.Verbosity > 0)
						Console.WriteLine ("Targets {0} and {1} found to be identical", f1, f2);
					// Don't use symlinks, since it just gets more complicated
					// For instance: on rebuild, when should the symlink be updated and when
					// should the target of the symlink be updated? And all the usages
					// must be audited to ensure the right thing is done...
					Driver.CopyAssembly (f1, f2);
				}
			}

			foreach (var target in Targets) {
				if (target.CanWeSymlinkTheApplication ())
					continue;

				target.ComputeLinkerFlags ();
				target.Compile ();
				target.NativeLink ();
			}
		}

		void WriteNotice ()
		{
			if (!IsDeviceBuild)
				return;

			if (Directory.Exists (Path.Combine (AppDirectory, "NOTICE")))
				throw new MonoTouchException (1016, true, "Failed to create the NOTICE file because a directory already exists with the same name.");

			try {
				// write license information inside the .app
				StringBuilder sb = new StringBuilder ();
				sb.Append ("Xamarin built applications contain open source software.  ");
				sb.Append ("For detailed attribution and licensing notices, please visit...");
				sb.AppendLine ().AppendLine ().Append ("http://xamarin.com/mobile-licensing").AppendLine ();
				var filename = Path.Combine (AppDirectory, "NOTICE");
				if (!File.Exists (filename) || File.ReadAllText (filename) != sb.ToString ()) {
					File.WriteAllText (Path.Combine (AppDirectory, "NOTICE"), sb.ToString ());
					Driver.Log (3, "Wrote '{0}'.", filename);
				} else {
					Driver.Log (3, "Target '{0}' is up-to-date.", filename);
				}
			} catch (Exception ex) {
				throw new MonoTouchException (1017, true, ex, "Failed to create the NOTICE file: {0}", ex.Message);
			}
		}

		void BuildFatSharedLibraries ()
		{
			// Create shared fat libraries.
			if (!FastDev)
				return;

			var hash = new Dictionary<string, List<string>> ();
			foreach (var target in Targets) {
				foreach (var a in target.Assemblies) {
					List<string> dylibs;

					if (a.Dylibs == null || a.Dylibs.Count () == 0)
						continue;

					if (!hash.TryGetValue (a.Dylib, out dylibs)) {
						dylibs = new List<string> ();
						hash [a.Dylib] = dylibs;
					}
					dylibs.AddRange (a.Dylibs);

					target.LinkWith (a.Dylib);
				}
			}

			foreach (var kvp in hash) {
				var dylib = kvp.Key;
				var dylibs = kvp.Value;
				if (!Application.IsUptodate (dylibs, new string [] { dylib })) {
					var cmd = new StringBuilder ();
					foreach (var lib in dylibs) {
						cmd.Append (Driver.Quote (lib));
						cmd.Append (' ');
					}
					cmd.Append ("-create -output ");
					cmd.Append (Driver.Quote (dylib));
					Driver.RunLipo (cmd.ToString ());
				} else {
					Driver.Log (3, "Target '{0}' is up-to-date.", dylib);
				}
			}
		}

		void CopyAotData ()
		{
			if (!IsDeviceBuild)
				return;

			foreach (var target in Targets) {
				foreach (var a in target.Assemblies) {
					foreach (var data in a.AotDataFiles) {
						Application.UpdateFile (data, Path.Combine (target.AppTargetDirectory, Path.GetFileName (data)));
					}
				}
			}
		}

		void BuildFinalExecutable ()
		{
			if (FastDev) {
				var libdir = Path.Combine (Driver.ProductSdkDirectory, "usr", "lib");
				var libmono_name = LibMono;
				if (!UseMonoFramework.Value) {
					var libmono_target = Path.Combine (AppDirectory, libmono_name);
					var libmono_source = Path.Combine (libdir, libmono_name);
					Application.UpdateFile (libmono_source, libmono_target);
				}

				var libprofiler_target = Path.Combine (AppDirectory, "libmono-profiler-log.dylib");
				var libprofiler_source = Path.Combine (libdir, "libmono-profiler-log.dylib");
				Application.UpdateFile (libprofiler_source, libprofiler_target);

				// Copy libXamarin.dylib to the app
				var libxamarin_target = Path.Combine (AppDirectory, LibXamarin);
				Application.UpdateFile (Path.Combine (Driver.MonoTouchLibDirectory, LibXamarin), libxamarin_target);

				if (UseMonoFramework.Value) {
					Driver.XcodeRun ("install_name_tool", "-change @executable_path/libmonosgen-2.0.dylib @rpath/Mono.framework/Mono " + Driver.Quote (libprofiler_target));
					Driver.XcodeRun ("install_name_tool", "-change @executable_path/libmonosgen-2.0.dylib @rpath/Mono.framework/Mono " + Driver.Quote (libxamarin_target));
				}
			}

			// Copy frameworks to the app bundle.
			if (!IsExtension) {
				var all_frameworks = new HashSet<string> ();
				all_frameworks.UnionWith (Frameworks);
				all_frameworks.UnionWith (WeakFrameworks);
				foreach (var t in Targets) {
					all_frameworks.UnionWith (t.Frameworks);
					all_frameworks.UnionWith (t.WeakFrameworks);
					foreach (var a in t.Assemblies) {
						if (a.Frameworks != null)
							all_frameworks.UnionWith (a.Frameworks);
						if (a.WeakFrameworks != null)
							all_frameworks.UnionWith (a.WeakFrameworks);
					}
				}
					
				if (PackageMonoFramework.Value) {
					// We may have to copy the Mono framework to the bundle even if we're not linking with it.
					all_frameworks.Add (Path.Combine (Driver.ProductSdkDirectory, "Frameworks", "Mono.framework"));
				}
				
				foreach (var fw in all_frameworks) {
					if (!fw.EndsWith (".framework"))
						continue;
					if (!Xamarin.MachO.IsDynamicFramework (Path.Combine (fw, Path.GetFileNameWithoutExtension (fw)))) {
						// We can have static libraries camouflaged as frameworks. We don't want those copied to the app.
						Driver.Log (1, "The framework {0} is a framework of static libraries, and will not be copied into the app.", fw);
						continue;
					}

					if (!File.Exists (Path.Combine (fw, "Info.plist")))
						throw ErrorHelper.CreateError (1304, "The embedded framework '{0}' in {1} is invalid: it does not contain an Info.plist.", Path.GetFileNameWithoutExtension (fw), fw);
					
					Application.UpdateDirectory (fw, Path.Combine (AppDirectory, "Frameworks"));
					if (IsDeviceBuild) {
						// Remove architectures we don't care about.
						Xamarin.MachO.SelectArchitectures (Path.Combine (AppDirectory, "Frameworks", Path.GetFileName (fw), Path.GetFileNameWithoutExtension (fw)), AllArchitectures);
					}
				}
			}

			if (IsSimulatorBuild || !IsDualBuild) {
				if (IsDeviceBuild)
					cached_executable = Targets [0].cached_executable;
				return;
			}

			if (IsSimulatorBuild || !IsDualBuild) {
				if (IsDeviceBuild)
					cached_executable = Targets [0].cached_executable;
				return;
			}

			if (IsUptodate (new string [] { Targets [0].Executable, Targets [1].Executable }, new string [] { Executable })) {
				cached_executable = true;
				Driver.Log (3, "Target '{0}' is up-to-date.", Executable);
				return;
			}

			var cmd = new StringBuilder ();
			foreach (var target in Targets) {
				cmd.Append (Driver.Quote (target.Executable));
				cmd.Append (' ');
			}
			cmd.Append ("-create -output ");
			cmd.Append (Driver.Quote (Executable));
			Driver.RunLipo (cmd.ToString ());

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

			Driver.CalculateCompilerPath ();
		}

		public string LibMono {
			get {
				if (FastDev) {
					return "libmonosgen-2.0.dylib";
				} else {
					return "libmonosgen-2.0.a";
				}
			}
		}

		public string LibXamarin {
			get {
				if (FastDev) {
					return EnableDebug ? "libxamarin-debug.dylib" : "libxamarin.dylib";
				} else {
					return EnableDebug ? "libxamarin-debug.a" : "libxamarin.a";
				}
			}
		}

		public void NativeLink ()
		{
			foreach (var target in Targets)
				target.NativeLink ();
		}
		
		// this will filter/remove warnings that are not helpful (e.g. complaining about non-matching armv6-6 then armv7-6 on fat binaries)
		// and turn the remaining of the warnings into MT5203 that MonoDevelop will be able to report as real warnings (not just logs)
		// it will also look for symbol-not-found errors and try to provide useful error messages.
		public static void ProcessNativeLinkerOutput (Target target, string output, IList<string> inputs, List<Exception> errors, bool error)
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
						if (!line.EndsWith (", referenced from:"))
							break;

						var symbol = line.Replace (", referenced from:", "").Trim ('\"', ' ');
						if (symbol.StartsWith ("_OBJC_CLASS_$_")) {
							errors.Add (new MonoTouchException (5211, error, 
																"Native linking failed, undefined Objective-C class: {0}. The symbol '{1}' could not be found in any of the libraries or frameworks linked with your application.",
							                                    symbol.Replace ("_OBJC_CLASS_$_", ""), symbol));
						} else {
							var member = target.GetMemberForSymbol (symbol.Substring (1));
							if (member != null) {
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
				} else if (line.StartsWith ("duplicate symbol") && line.EndsWith (" in:")) {
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
					if (line.StartsWith ("ld: "))
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

		public void BuildMSymDirectory ()
		{
			if (!EnableMSym)
				return;

			var msym_directory = string.Format ("{0}.msym", AppDirectory);
			foreach (var target in Targets) {
				var target_directory = Path.Combine (msym_directory, target.Is32Build ? "32" : "64");
				if (!Directory.Exists (target_directory))
					Directory.CreateDirectory (target_directory);
				foreach (var asm in target.Assemblies) {
					var msym_file = asm.FileName + ".msym";
					var src = Path.Combine (target.BuildDirectory, msym_file);
					if (File.Exists (src))
						UpdateFile (src, Path.Combine (target_directory, msym_file));
					asm.CopyToDirectory (target_directory, reload: false, only_copy: true);
				}
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

		IEnumerable<string> GetRequiredSymbols ()
		{
			foreach (var target in Targets) {
				foreach (var symbol in target.GetRequiredSymbols ())
					yield return symbol;
			}
		}

		bool WriteSymbolList (string filename)
		{
			var required_symbols = GetRequiredSymbols ().ToArray ();
			using (StreamWriter writer = new StreamWriter (filename)) {
				foreach (string symbol in required_symbols)
					writer.WriteLine ("_{0}", symbol);
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
				if (WriteSymbolList (symbol_file)) {
					Driver.RunStrip (String.Format ("-i -s \"{0}\" \"{1}\"", symbol_file, Executable));
				} else {
					Driver.RunStrip (String.Format ("\"{0}\"", Executable));
				}
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
					cached &= target.cached_executable;
				if (!cached)
					StripNativeCode (Executable);
			} else {
				foreach (var target in Targets) {
					if (!target.cached_executable)
						StripNativeCode (target.Executable);
				}
			}
		}

		public void StripManagedCode ()
		{
			foreach (var target in Targets)
				target.StripManagedCode ();

			// deduplicate assemblies between the .monotouch-32 and .monotouch-64 directories
			if (IsDualBuild && IsDeviceBuild && !Sign)
				DeduplicateDir ("..", Targets [0].AppTargetDirectory, Targets [1].AppTargetDirectory);
		}

		void DeduplicateDir (string base_dir, string d1, string d2)
		{
			foreach (var f1 in Directory.GetFileSystemEntries (d1)) {
				var f2 = Path.Combine (d2, Path.GetFileName (f1));
				if (Directory.Exists (f1))
					DeduplicateDir (Path.Combine (base_dir, ".."), f1, f2);
				else
					DeduplicateFile (base_dir, f1, f2);
			}
		}

		void DeduplicateFile (string base_dir, string f1, string f2)
		{
			if (!File.Exists (f2))
				return;

			if (Driver.IsSymlink (f2))
				return; // Already determined to be identical from a previous build.

			bool equal;
			var ext = Path.GetExtension (f1).ToUpperInvariant ();
			var is_assembly = ext == ".EXE" || ext == ".DLL";

			if (is_assembly) {
				equal = Cache.CompareAssemblies (f1, f2, true, true);
				if (!equal && Driver.Verbosity > 0)
					Console.WriteLine ("Assemblies {0} and {1} not found to be identical, cannot replace one with a symlink to the other.", f1, f2);
			} else {
				equal = Cache.CompareFiles (f1, f2, true);
				if (!equal && Driver.Verbosity > 0)
					Console.WriteLine ("Targets {0} and {1} not found to be identical, cannot replace one with a symlink to the other.", f1, f2);
			}
			if (!equal)
				return;

			var dest = Path.Combine (base_dir, f1.Substring (AppDirectory.Length + 1));
			Driver.FileDelete (f2);
			if (!Driver.Symlink (dest, f2)) {
				File.Copy (f1, f2);
			} else {
				if (Driver.Verbosity > 0)
					Console.WriteLine ("Targets {0} and {1} found to be identical, the later has been replaced with a symlink to the former.", f1, f2);
			}
		}

		public void SignBundle ()
		{
			if (!IsDeviceBuild || !Sign)
				return;

			var env_vars = new string [] {
				"CODESIGN_ALLOCATE", 
				Path.Combine (Driver.PlatformDirectory, "Developer", "usr", "bin", "codesign_allocate")
			};

			if (Driver.RunCommand ("codesign", String.Format ("-v -s \"{0}\" \"{1}\"", CertificateName, Executable), env_vars) != 0)
				ErrorHelper.Error (5307, "Failed to sign the executable. Please review the build log.");
		}

		public void GenerateRuntimeOptions ()
		{
			// only if the linker is disabled
			if (LinkMode != LinkMode.None)
				return;

			RuntimeOptions.Write (AppDirectory);
		}

		public void GenerateAppManifests ()
		{
			if (!GenerateManifests)
				return;
			
			using (var f = File.OpenWrite (Path.Combine (AppDirectory, "PkgInfo"))){
				f.Write (new byte [] { 0X41, 0X50, 0X50, 0X4C, 0x3f, 0x3f, 0x3f, 0x3f}, 0, 8);
			}

			var executable = Path.GetFileName (Executable);

			var sr = new StreamReader (typeof (Driver).Assembly.GetManifestResourceStream ("Info.plist.tmpl"));
			var all = sr.ReadToEnd ();
			var icon_str = (Icon != null) ? "\t<key>CFBundleIconFile</key>\n\t<string>" + Icon + "</string>\n\t" : "";

			using (var sw = new StreamWriter (Path.Combine (AppDirectory, "Info.plist"))){
				sw.WriteLine (
					all.Replace ("@BUNDLEDISPLAYNAME@", BundleDisplayName ?? executable).
					Replace ("@EXECUTABLE@", executable).
					Replace ("@BUNDLEID@", BundleId).
					Replace ("@BUNDLEICON@", icon_str).
					Replace ("@BUNDLENAME@", executable).
					Replace ("@MAINNIB@", MainNib));

			}
		}

		public void ProcessFrameworksForArguments (StringBuilder args, IEnumerable<string> frameworks, IEnumerable<string> weak_frameworks, IList<string> inputs)
		{
			bool any_user_framework = false;

			if (frameworks != null) {
				foreach (var fw in frameworks)
					ProcessFrameworkForArguments (args, fw, false, inputs, ref any_user_framework);
			}

			if (weak_frameworks != null) {
				foreach (var fw in weak_frameworks)
					ProcessFrameworkForArguments (args, fw, true, inputs, ref any_user_framework);
			}
			
			if (any_user_framework) {
				args.Append (" -Xlinker -rpath -Xlinker @executable_path/Frameworks");
				if (IsExtension)
					args.Append (" -Xlinker -rpath -Xlinker @executable_path/../../Frameworks");
			}

		}

		public static void ProcessFrameworkForArguments (StringBuilder args, string fw, bool is_weak, IList<string> inputs, ref bool any_user_framework)
		{
			var name = Path.GetFileNameWithoutExtension (fw);
			if (fw.EndsWith (".framework")) {
				// user framework, we need to pass -F to the linker so that the linker finds the user framework.
				any_user_framework = true;
				if (inputs != null)
					inputs.Add (Path.Combine (fw, name));
				args.Append (" -F ").Append (Driver.Quote (Path.GetDirectoryName (fw)));
			}
			args.Append (is_weak ? " -weak_framework " : " -framework ").Append (Driver.Quote (name));
		}
	}

	public class BuildTasks : List<BuildTask>
	{
		static void Execute (BuildTask v)
		{
			var next = v.Execute ();
			if (next != null)
				Parallel.ForEach (next, new ParallelOptions () { MaxDegreeOfParallelism = Environment.ProcessorCount }, Execute);
		}

		public void ExecuteInParallel ()
		{
			if (Count == 0)
				return;
			
			Parallel.ForEach (this, new ParallelOptions () { MaxDegreeOfParallelism = Environment.ProcessorCount }, Execute);
		}
	}

	public abstract class BuildTask
	{
		public IEnumerable<BuildTask> NextTasks;

		protected abstract void Build ();

		public IEnumerable<BuildTask> Execute ()
		{
			Build ();
			return NextTasks;
		}

		public virtual bool IsUptodate ()
		{
			return false;
		}
	}

	public abstract class ProcessTask : BuildTask
	{
		public ProcessStartInfo ProcessStartInfo;
		protected StringBuilder Output;
		
		protected string Command {
			get {
				var result = new StringBuilder ();
				if (ProcessStartInfo.EnvironmentVariables.ContainsKey ("MONO_PATH")) {
					result.Append ("MONO_PATH=");
					result.Append (ProcessStartInfo.EnvironmentVariables ["MONO_PATH"]);
					result.Append (' ');
				}
				result.Append (ProcessStartInfo.FileName);
				result.Append (' ');
				result.Append (ProcessStartInfo.Arguments);
				return result.ToString ();
			}
		}

		protected int Start ()
		{
			if (Driver.Verbosity > 0 || Driver.DryRun)
				Console.WriteLine (Command);
			
			if (Driver.DryRun)
				return 0;
			
			var info = ProcessStartInfo;
			var stdout_completed = new ManualResetEvent (false);
			var stderr_completed = new ManualResetEvent (false);

			Output = new StringBuilder ();

			using (var p = Process.Start (info)) {
				p.OutputDataReceived += (sender, e) => {
					if (e.Data != null) {
						lock (Output)
							Output.AppendLine (e.Data);
					} else {
						stdout_completed.Set ();
					}
				};
				
				p.ErrorDataReceived += (sender, e) => {
					if (e.Data != null) {
						lock (Output)
							Output.AppendLine (e.Data);
					} else {
						stderr_completed.Set ();
					}
				};
				
				p.BeginOutputReadLine ();
				p.BeginErrorReadLine ();
				
				p.WaitForExit ();
				
				stderr_completed.WaitOne (TimeSpan.FromSeconds (1));
				stdout_completed.WaitOne (TimeSpan.FromSeconds (1));

				if (p.ExitCode != 0)
					return p.ExitCode;

				if (Driver.Verbosity >= 2 && Output.Length > 0)
					Console.Error.WriteLine (Output.ToString ());
			}

			return 0;
		}
	}

	internal class MainTask : CompileTask {
		public static void Create (List<BuildTask> tasks, Target target, Abi abi, IEnumerable<Assembly> assemblies, string assemblyName, IList<string> registration_methods)
		{
			var arch = abi.AsArchString ();
			var ofile = Path.Combine (Cache.Location, "main." + arch + ".o");
			var ifile = Path.Combine (Cache.Location, "main." + arch + ".m");

			var files = assemblies.Select (v => v.FullPath);

			if (!Application.IsUptodate (files, new string [] { ifile })) {
				Driver.GenerateMain (assemblies, assemblyName, abi, ifile, registration_methods);
			} else {
				Driver.Log (3, "Target '{0}' is up-to-date.", ifile);
			}
			
			if (!Application.IsUptodate (ifile, ofile)) {
				var main = new MainTask ()
				{
					Target = target,
					Abi = abi,
					AssemblyName = assemblyName,
					InputFile = ifile,
					OutputFile = ofile,
					SharedLibrary = false,
					Language = "objective-c++",
				};
				main.CompilerFlags.AddDefine ("MONOTOUCH");
				tasks.Add (main);
			} else {
				Driver.Log (3, "Target '{0}' is up-to-date.", ofile);
			}
			
			target.LinkWith (ofile);
		}

		protected override void Build ()
		{
			if (Compile () != 0)
				throw new MonoTouchException (5103, true, "Failed to compile the file '{0}'. Please file a bug report at http://bugzilla.xamarin.com", InputFile);
		}
	}

	internal class RegistrarTask : CompileTask {
		public static void Create (List<BuildTask> tasks, IEnumerable<Abi> abis, Target target, string ifile)
		{
			foreach (var abi in abis)
				Create (tasks, abi, target, ifile);
		}

		public static void Create (List<BuildTask> tasks, Abi abi, Target target, string ifile)
		{
			var arch = abi.AsArchString ();
			var ofile = Path.Combine (Cache.Location, Path.GetFileNameWithoutExtension (ifile) + "." + arch + ".o");

			if (!Application.IsUptodate (ifile, ofile)) {
				tasks.Add (new RegistrarTask ()
				{
					Target = target,
					Abi = abi,
					InputFile = ifile,
					OutputFile = ofile,
					SharedLibrary = false,
					Language = "objective-c++",
				});
			} else {
				Driver.Log (3, "Target '{0}' is up-to-date.", ofile);
			}
			
			target.LinkWith (ofile);
		}

		protected override void Build ()
		{
			if (Driver.IsUsingClang) {
				// This is because iOS has a forward declaration of NSPortMessage, but no actual declaration.
				// They still use NSPortMessage in other API though, so it can't just be removed from our bindings.
				CompilerFlags.AddOtherFlag ("-Wno-receiver-forward-class");
			}

			if (Compile () != 0)
				throw new MonoTouchException (4109, true, "Failed to compile the generated registrar code. Please file a bug report at http://bugzilla.xamarin.com");
		}
	}

	public class AOTTask : ProcessTask {
		public string AssemblyName;

		// executed with Parallel.ForEach
		protected override void Build ()
		{
			var exit_code = base.Start ();

			if (exit_code == 0)
				return;

			Console.Error.WriteLine ("AOT Compilation exited with code {0}, command:\n{1}{2}", exit_code, Command, Output.Length > 0 ? ("\n" + Output.ToString ()) : string.Empty);
			if (Output.Length > 0) {
				List<Exception> exceptions = new List<Exception> ();
				foreach (var line in Output.ToString ().Split ('\n')) {
					if (line.StartsWith ("AOT restriction: Method '") && line.Contains ("must be static since it is decorated with [MonoPInvokeCallback]")) {
						exceptions.Add (new MonoTouchException (3002, true, line));
					}
				}
				if (exceptions.Count > 0)
					throw new AggregateException (exceptions.ToArray ());
			}

			throw new MonoTouchException (3001, true, "Could not AOT the assembly '{0}'", AssemblyName);
		}
	}

	public class LinkTask : CompileTask {
	}

	public class CompileTask : BuildTask {
		public Target Target;
		public Application App { get { return Target.App; } }
		public bool SharedLibrary;
		public string InputFile;
		public string OutputFile;
		public Abi Abi;
		public string AssemblyName;
		public string InstallName;
		public string Language;

		CompilerFlags compiler_flags;
		public CompilerFlags CompilerFlags {
			get { return compiler_flags ?? (compiler_flags = new CompilerFlags () { Target = Target }); }
			set { compiler_flags = value; }
		}

		public static void GetArchFlags (CompilerFlags flags, params Abi [] abis)
		{
			GetArchFlags (flags, (IEnumerable<Abi>) abis);
		}

		public static void GetArchFlags (CompilerFlags flags, IEnumerable<Abi> abis)
		{
			bool enable_thumb = false;

			foreach (var abi in abis) {
				var arch = abi.AsArchString ();
				flags.AddOtherFlag ($"-arch {arch}");

				enable_thumb |= (abi & Abi.Thumb) != 0;
			}

			if (enable_thumb)
				flags.AddOtherFlag ("-mthumb");
		}

		public static void GetCompilerFlags (CompilerFlags flags, string ifile, string language = null)
		{
			if (string.IsNullOrEmpty (ifile) || !ifile.EndsWith (".s"))
				flags.AddOtherFlag ("-gdwarf-2");

			if (!string.IsNullOrEmpty (ifile) && !ifile.EndsWith (".s")) {
				if (string.IsNullOrEmpty (language) || !language.Contains ("++")) {
					// error: invalid argument '-std=c99' not allowed with 'C++/ObjC++'
					flags.AddOtherFlag ("-std=c99");
				}
				flags.AddOtherFlag ($"-I{Driver.Quote (Path.Combine (Driver.ProductSdkDirectory, "usr", "include"))}");
			}
			flags.AddOtherFlag ($"-isysroot {Driver.Quote (Driver.FrameworkDirectory)}");
			flags.AddOtherFlag ("-Qunused-arguments"); // don't complain about unused arguments (clang reports -std=c99 and -Isomething as unused).
		}
		
		public static void GetSimulatorCompilerFlags (CompilerFlags flags, string ifile, Application app, string language = null)
		{
			GetCompilerFlags (flags, ifile, language);

			if (Driver.SDKVersion == new Version ())
				throw new MonoTouchException (25, true, "No SDK version was provided. Please add --sdk=X.Y to specify which iOS SDK should be used to build your application.");

			string sim_platform = Driver.PlatformDirectory;
			string plist = Path.Combine (sim_platform, "Info.plist");

			var dict = Driver.FromPList (plist);
			var dp = dict.Get<PDictionary> ("DefaultProperties");
			if (dp.GetString ("GCC_OBJC_LEGACY_DISPATCH") == "YES")
					flags.AddOtherFlag ("-fobjc-legacy-dispatch");
			string objc_abi = dp.GetString ("OBJC_ABI_VERSION");
			if (!String.IsNullOrWhiteSpace (objc_abi))
				flags.AddOtherFlag ($"-fobjc-abi-version={objc_abi}");
			
			plist = Path.Combine (Driver.FrameworkDirectory, "SDKSettings.plist");
			string min_prefix = Driver.CompilerPath.Contains ("clang") ? Driver.TargetMinSdkName : "iphoneos";
			dict = Driver.FromPList (plist);
			dp = dict.Get<PDictionary> ("DefaultProperties");
			if (app.DeploymentTarget == new Version ()) {
				string target = dp.GetString ("IPHONEOS_DEPLOYMENT_TARGET");
				if (!String.IsNullOrWhiteSpace (target))
					flags.AddOtherFlag ($"-m{min_prefix}-version-min={target}");
			} else {
				flags.AddOtherFlag ($"-m{min_prefix}-version-min={app.DeploymentTarget}");
			}
			string defines = dp.GetString ("GCC_PRODUCT_TYPE_PREPROCESSOR_DEFINITIONS");
			if (!String.IsNullOrWhiteSpace (defines))
				flags.AddDefine (defines.Replace (" ", String.Empty));
		}
		
		void GetDeviceCompilerFlags (CompilerFlags flags, string ifile)
		{
			GetCompilerFlags (flags, ifile, Language);
			
			flags.AddOtherFlag ($"-m{Driver.TargetMinSdkName}-version-min={App.DeploymentTarget.ToString ()}");

			if (App.EnableLLVMOnlyBitCode)
				// The AOT compiler doesn't optimize the bitcode so clang will do it
				flags.AddOtherFlag ("-O2 -fexceptions");
		}
		
		void GetSharedCompilerFlags (CompilerFlags flags, string install_name)
		{
			flags.AddOtherFlag ("-shared");
			if (!App.EnableMarkerOnlyBitCode)
				flags.AddOtherFlag ("-read_only_relocs suppress");
			flags.LinkWithMono ();
			flags.AddOtherFlag ($"-install_name @executable_path/{install_name}");
			flags.AddOtherFlag ("-fapplication-extension"); // fixes this: warning MT5203: Native linking warning: warning: linking against dylib not safe for use in application extensions: [..]/actionextension.dll.arm64.dylib
		}
		
		void GetStaticCompilerFlags (CompilerFlags flags)
		{
			flags.AddOtherFlag ("-c");
		}

		void GetBitcodeCompilerFlags (CompilerFlags flags)
		{
			flags.AddOtherFlag (App.EnableMarkerOnlyBitCode ? "-fembed-bitcode-marker" : "-fembed-bitcode");
		}

		protected override void Build ()
		{
			if (Compile () != 0)
				throw new MonoTouchException (3001, true, "Could not AOT the assembly '{0}'", AssemblyName);
		}

		public int Compile ()
		{
			if (App.IsDeviceBuild) {
				GetDeviceCompilerFlags (CompilerFlags, InputFile);
			} else {
				GetSimulatorCompilerFlags (CompilerFlags, InputFile, App, Language);
			}

			if (App.EnableBitCode)
				GetBitcodeCompilerFlags (CompilerFlags);
			GetArchFlags(CompilerFlags, Abi);

			if (SharedLibrary) {
				GetSharedCompilerFlags (CompilerFlags, InstallName);
			} else {
				GetStaticCompilerFlags (CompilerFlags);
			}

			if (App.EnableDebug)
				CompilerFlags.AddDefine ("DEBUG");

			CompilerFlags.AddOtherFlag ($"-o {Driver.Quote (OutputFile)}");

			if (!string.IsNullOrEmpty (Language))
				CompilerFlags.AddOtherFlag ($"-x {Language}");

			CompilerFlags.AddOtherFlag (Driver.Quote (InputFile));

			var rv = Driver.RunCommand (Driver.CompilerPath, CompilerFlags.ToString (), null, null);
			
			return rv;
		}
	}

	public class BitCodeify : BuildTask {
		public string Input { get; set; }
		public string OutputFile { get; set; }
		public ApplePlatform Platform { get; set; }
		public Abi Abi { get; set; }
		public Version DeploymentTarget { get; set; }

		protected override void Build ()
		{
			new BitcodeConverter (Input, OutputFile, Platform, Abi, DeploymentTarget).Convert ();
		}
	}
}

