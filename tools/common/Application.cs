using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Linker;

using Xamarin.Linker;
using Xamarin.MacDev;
using Xamarin.Utils;

using ObjCRuntime;

#if MONOTOUCH
using PlatformResolver = MonoTouch.Tuner.MonoTouchResolver;
#elif MMP
using PlatformResolver = Xamarin.Bundler.MonoMacResolver;
#elif NET
using PlatformResolver = Xamarin.Linker.DotNetResolver;
#else
#error Invalid defines
#endif

namespace Xamarin.Bundler {

	public enum BuildTarget {
		None,
		Simulator,
		Device,
	}

	public enum MonoNativeMode {
		None,
		Compat,
		Unified,
	}

	[Flags]
	public enum RegistrarOptions {
		Default = 0,
		Trace = 1,
	}

	public enum RegistrarMode {
		Default,
		Dynamic,
		PartialStatic,
		Static,
	}

	public partial class Application
	{
		public Cache Cache;
		public string AppDirectory = ".";
		public bool DeadStrip = true;
		public bool EnableDebug;
		// The list of assemblies that we do generate debugging info for.
		public bool DebugAll;
		public bool UseInterpreter;
		public List<string> DebugAssemblies = new List<string> ();
		internal RuntimeOptions RuntimeOptions;
		public Optimizations Optimizations = new Optimizations ();
		public RegistrarMode Registrar = RegistrarMode.Default;
		public RegistrarOptions RegistrarOptions = RegistrarOptions.Default;
		public SymbolMode SymbolMode;
		public HashSet<string> IgnoredSymbols = new HashSet<string> ();

		public HashSet<string> Frameworks = new HashSet<string> ();
		public HashSet<string> WeakFrameworks = new HashSet<string> ();

		public bool IsExtension;
		public ApplePlatform Platform { get { return Driver.TargetFramework.Platform; } }

		// Linker config
		public LinkMode LinkMode = LinkMode.Full;
		public List<string> LinkSkipped = new List<string> ();
		public List<string> Definitions = new List<string> ();
		public I18nAssemblies I18n;
		public List<string> WarnOnTypeRef = new List<string> ();

		public bool? EnableCoopGC;
		public bool EnableSGenConc;
		public bool EnableProfiling;
		public bool? DebugTrack;

		public Dictionary<string, string> EnvironmentVariables = new Dictionary<string, string> ();

		public MarshalObjectiveCExceptionMode MarshalObjectiveCExceptions;
		public MarshalManagedExceptionMode MarshalManagedExceptions;

		bool is_default_marshal_managed_exception_mode;
		public bool IsDefaultMarshalManagedExceptionMode {
			get { return is_default_marshal_managed_exception_mode || MarshalManagedExceptions == MarshalManagedExceptionMode.Default; }
			set { is_default_marshal_managed_exception_mode = value; }
		}
		public List<string> RootAssemblies = new List<string> ();
		public List<string> References = new List<string> ();
		public List<Application> SharedCodeApps = new List<Application> (); // List of appexes we're sharing code with.
		public string RegistrarOutputLibrary;

		public BuildTarget BuildTarget;

		bool RequiresXcodeHeaders {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
					return LinkMode == LinkMode.None;
				case ApplePlatform.MacOSX:
					return Registrar == RegistrarMode.Static && LinkMode == LinkMode.None;
				default:
					throw ErrorHelper.CreateError (71, Errors.MX0071, Platform, ProductName);
				}
			}
		}

		public string LocalBuildDir {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
					return "_ios-build";
				case ApplePlatform.MacOSX:
					return "_mac-build";
				default:
					throw ErrorHelper.CreateError (71, Errors.MX0071, Platform, ProductName);
				}
			}
		}

		public string FrameworkLocationVariable {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
					return "MD_MTOUCH_SDK_ROOT";
				case ApplePlatform.MacOSX:
					return "XAMMAC_FRAMEWORK_PATH";
				default:
					throw ErrorHelper.CreateError (71, Errors.MX0071, Platform, ProductName);
				}
			}
		}

		public bool IsDeviceBuild {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
					return BuildTarget == BuildTarget.Device;
				case ApplePlatform.MacOSX:
					return false;
				default:
					throw ErrorHelper.CreateError (71, Errors.MX0071, Platform, ProductName);
				}
			}
		}

		public bool IsSimulatorBuild {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
					return BuildTarget == BuildTarget.Simulator;
				case ApplePlatform.MacOSX:
					return false;
				default:
					throw ErrorHelper.CreateError (71, Errors.MX0071, Platform, ProductName);
				}
			}
		}

		public static int Concurrency => Driver.Concurrency;
		public Version DeploymentTarget;
		public Version SdkVersion;
	
		public MonoNativeMode MonoNativeMode { get; set; }
		List<Abi> abis;
		public bool IsLLVM { get { return IsArchEnabled (Abi.LLVM); } }

		public bool Embeddinator { get; set; }

		public List<Target> Targets = new List<Target> ();

		bool? package_managed_debug_symbols;
		public bool PackageManagedDebugSymbols {
			get { return package_managed_debug_symbols.Value; }
			set { package_managed_debug_symbols = value; }
		}

		public string TlsProvider;
		public string HttpMessageHandler;
		// If we're targetting a 32 bit arch.
		bool? is32bits;
		public bool Is32Build {
			get {
				if (!is32bits.HasValue)
					is32bits = IsArchEnabled (Abi.Arch32Mask);
				return is32bits.Value;
			}
		}

		public string GetProductName ()
		{
			return ProductName;
		}

		// If we're targetting a 64 bit arch.
		bool? is64bits;
		public bool Is64Build {
			get {
				if (!is64bits.HasValue)
					is64bits = IsArchEnabled (Abi.Arch64Mask);
				return is64bits.Value;
			}
		}

		public bool IsDualBuild { get { return Is32Build && Is64Build; } } // if we're building both a 32 and a 64 bit version.

		public Application ()
		{
		}

		public Application (string [] arguments)
		{
			CreateCache (arguments);
		}

		public void CreateCache (string [] arguments)
		{
			Cache = new Cache (arguments);
		}

		public bool DynamicRegistrationSupported {
			get {
				return Optimizations.RemoveDynamicRegistrar != true;
			}
		}

		public void ParseI18nAssemblies (string i18n)
		{
			var assemblies = I18nAssemblies.None;

			foreach (var part in i18n.Split (',')) {
				var assembly = part.Trim ();
				if (string.IsNullOrEmpty (assembly))
					continue;

				try {
					assemblies |= (I18nAssemblies) Enum.Parse (typeof (I18nAssemblies), assembly, true);
				} catch {
					throw new FormatException ("Unknown value for i18n: " + assembly);
				}
			}

			I18n = assemblies;
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

		// This is just a name for this app to show in log/error messages, etc.
		public string Name {
			get { return Path.GetFileNameWithoutExtension (AppDirectory); }
		}

		public bool RequiresPInvokeWrappers {
			get {
#if MTOUCH
				if (IsSimulatorBuild)
					return false;
				return MarshalObjectiveCExceptions == MarshalObjectiveCExceptionMode.ThrowManagedException || MarshalObjectiveCExceptions == MarshalObjectiveCExceptionMode.Abort;
#else
				return false;
#endif
			}
		}

		public string PlatformName {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
					return "iOS";
				case ApplePlatform.TVOS:
					return "tvOS";
				case ApplePlatform.WatchOS:
					return "watchOS";
				case ApplePlatform.MacOSX:
					return "macOS";
				default:
					throw new NotImplementedException ();
				}
			}
		}
		
		public static bool IsUptodate (string source, string target, bool check_contents = false, bool check_stamp = true)
		{
			return FileCopier.IsUptodate (source, target, check_contents, check_stamp);
		}

		public static void RemoveResource (ModuleDefinition module, string name)
		{
			for (int i = 0; i < module.Resources.Count; i++) {
				EmbeddedResource embedded = module.Resources[i] as EmbeddedResource;
				
				if (embedded == null || embedded.Name != name)
					continue;
				
				module.Resources.RemoveAt (i);
				break;
			}
		}

		public static void SaveAssembly (AssemblyDefinition assembly, string destination)
		{
			var main = assembly.MainModule;
			bool symbols = main.HasSymbols;
			if (symbols) {
				var provider = new DefaultSymbolReaderProvider ();
				main.ReadSymbols (provider.GetSymbolReader (main, main.FileName));
			}

			var wp = new WriterParameters () {
				WriteSymbols = symbols,
				SymbolWriterProvider = symbols ? new CustomSymbolWriterProvider () : null,
			};

			// re-write symbols, if available, so the new tokens will match
			assembly.Write (destination, wp);

			if (!symbols) {
				// if we're not saving the symbols then we must not leave stale/old files to be used by other tools
				string dest_mdb = destination + ".mdb";
				if (File.Exists (dest_mdb))
					File.Delete (dest_mdb);
				string dest_pdb = Path.ChangeExtension (destination, ".pdb");
				if (File.Exists (dest_pdb))
					File.Delete (dest_pdb);
			}
		}

		public static void ExtractResource (ModuleDefinition module, string name, string path, bool remove)
		{
			for (int i = 0; i < module.Resources.Count; i++) {
				EmbeddedResource embedded = module.Resources[i] as EmbeddedResource;
				
				if (embedded == null || embedded.Name != name)
					continue;
				
				string dirname = Path.GetDirectoryName (path);
				if (!Directory.Exists (dirname))
					Directory.CreateDirectory (dirname);

				using (Stream ostream = File.OpenWrite (path)) {
					embedded.GetResourceStream ().CopyTo (ostream);
				}
				
				if (remove)
					module.Resources.RemoveAt (i);
				
				break;
			}
		}

		// Returns true if the source file was copied to the target or false if it was already up to date.
		public static bool UpdateFile (string source, string target, bool check_contents = false)
		{
			if (!Application.IsUptodate (source, target, check_contents)) {
				CopyFile (source, target);
				return true;
			}
			else {
				Driver.Log (3, "Target '{0}' is up-to-date", target);
				return false;
			}
		}

		// Checks if any of the source files have a time stamp later than any of the target files.
		//
		// If check_stamp is true, the function will use the timestamp of a "target".stamp file
		// if it's later than the timestamp of the "target" file itself.
		public static bool IsUptodate (IEnumerable<string> sources, IEnumerable<string> targets, bool check_stamp = true)
		{
			if (Driver.Force)
				return false;

			DateTime max_source = DateTime.MinValue;
			string max_s = null;

			if (sources.Count () == 0 || targets.Count () == 0)
				throw ErrorHelper.CreateError (1013, Errors.MT1013);

			foreach (var s in sources) {
				var sfi = new FileInfo (s);
				if (!sfi.Exists) {
					Driver.Log (3, "Prerequisite '{0}' does not exist.", s);
					return false;
				}

				var st = sfi.LastWriteTimeUtc;
				if (st > max_source) {
					max_source = st;
					max_s = s;
				}
			}


			foreach (var t in targets) {
				var tfi = new FileInfo (t);
				if (!tfi.Exists) {
					Driver.Log (3, "Target '{0}' does not exist.", t);
					return false;
				}

				if (check_stamp) {
					var tfi_stamp = new FileInfo (t + ".stamp");
					if (tfi_stamp.Exists && tfi_stamp.LastWriteTimeUtc > tfi.LastWriteTimeUtc) {
						Driver.Log (3, "Target '{0}' has a stamp file with newer timestamp ({1} > {2}), using the stamp file's timestamp", t, tfi_stamp.LastWriteTimeUtc, tfi.LastWriteTimeUtc);
						tfi = tfi_stamp;
					}
				}

				var lwt = tfi.LastWriteTimeUtc;
				if (max_source > lwt) {
					Driver.Log (3, "Prerequisite '{0}' is newer than target '{1}' ({2} vs {3}).", max_s, t, max_source, lwt);
					return false;
				}
			}

			Driver.Log (3, "Prerequisite(s) '{0}' are all older than the target(s) '{1}'.", string.Join ("', '", sources.ToArray ()), string.Join ("', '", targets.ToArray ()));

			return true;
		}
		
		public static void UpdateDirectory (string source, string target)
		{
			FileCopier.UpdateDirectory (source, target);
		}

		[DllImport (Constants.libSystemLibrary)]
		static extern int readlink (string path, IntPtr buf, int len);

		// A file copy that will replace symlinks with the source file
		// File.Copy will copy the source to the target of the symlink instead
		// of replacing the symlink.
		public static void CopyFile (string source, string target)
		{
			if (readlink (target, IntPtr.Zero, 0) != -1) {
				// Target is a symlink, delete it.
				File.Delete (target);
			} else if (File.Exists (target)) {
				// Also delete the target file if it already exists,
				// since it may not have write permissions.
				File.Delete (target);
			}

			var dir = Path.GetDirectoryName (target);
			if (!Directory.Exists (dir))
				Directory.CreateDirectory (dir);

			File.Copy (source, target, true);
			// Make sure the target file is r/w.
			var attrs = File.GetAttributes (target);
			if ((attrs & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
				File.SetAttributes (target, attrs & ~FileAttributes.ReadOnly);
			Driver.Log (1, "Copied {0} to {1}", source, target);
		}

		public static void TryDelete (string path)
		{
			try {
				if (File.Exists (path))
					File.Delete (path);
			} catch {
			}
		}

		public void InitializeCommon ()
		{
			SelectRegistrar ();
			SelectMonoNative ();

			RuntimeOptions = RuntimeOptions.Create (this, HttpMessageHandler, TlsProvider);

			if (RequiresXcodeHeaders && SdkVersion < SdkVersions.GetVersion (this)) {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
					throw ErrorHelper.CreateError (180, Errors.MX0180, ProductName, PlatformName, SdkVersions.GetVersion (this), SdkVersions.Xcode);
				case ApplePlatform.MacOSX:
					throw ErrorHelper.CreateError (179, Errors.MX0179, ProductName, PlatformName, SdkVersions.GetVersion (this), SdkVersions.Xcode);
				default:
					// Default to the iOS error message, it's better than showing MX0071 (unknown platform), which would be completely unrelated
					goto case ApplePlatform.iOS;
				}
			}

			if (DeploymentTarget != null) {
				if (DeploymentTarget < Xamarin.SdkVersions.GetMinVersion (this))
					throw new ProductException (73, true, Errors.MT0073, Constants.Version, DeploymentTarget, Xamarin.SdkVersions.GetMinVersion (this), PlatformName, ProductName);
				if (DeploymentTarget > Xamarin.SdkVersions.GetVersion (this))
					throw new ProductException (74, true, Errors.MX0074, Constants.Version, DeploymentTarget, Xamarin.SdkVersions.GetVersion (this), PlatformName, ProductName);
			}

			if (Platform == ApplePlatform.WatchOS && EnableCoopGC.HasValue && !EnableCoopGC.Value)
				throw ErrorHelper.CreateError (88, Errors.MT0088);

			if (!EnableCoopGC.HasValue)
				EnableCoopGC = Platform == ApplePlatform.WatchOS;

			SetObjectiveCExceptionMode ();
			SetManagedExceptionMode ();

			if (SymbolMode == SymbolMode.Default) {
#if MONOTOUCH
				SymbolMode = EnableBitCode ? SymbolMode.Code : SymbolMode.Linker;
#else
				SymbolMode = SymbolMode.Linker;
#endif
			}

#if MONOTOUCH
			if (EnableBitCode && SymbolMode != SymbolMode.Code) {
				// This is a warning because:
				// * The user will get a linker error anyway if they do this.
				// * I see it as quite unlikely that anybody will in fact try this (it must be manually set in the additional mtouch arguments).
				// * I find it more probable that Apple will remove the -u restriction, in which case someone might actually want to try this, and if it's a warning, we won't prevent it.
				ErrorHelper.Warning (115, Errors.MT0115);
			}
#endif

			if (!DebugTrack.HasValue) {
				DebugTrack = false;
			} else if (DebugTrack.Value && !EnableDebug) {
				ErrorHelper.Warning (32, Errors.MT0032);
			}

			if (!package_managed_debug_symbols.HasValue) {
				package_managed_debug_symbols = EnableDebug;
			} else if (package_managed_debug_symbols.Value && IsLLVM) {
				ErrorHelper.Warning (3007, Errors.MX3007);
			}

			Optimizations.Initialize (this, out var messages);
			ErrorHelper.Show (messages);
			if (Driver.Verbosity > 3)
				Driver.Log (4, $"Enabled optimizations: {Optimizations}");
		}

		void SelectMonoNative ()
		{
			switch (Platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				MonoNativeMode = DeploymentTarget.Major >= 10 ? MonoNativeMode.Unified : MonoNativeMode.Compat;
				break;
			case ApplePlatform.WatchOS:
				if (IsArchEnabled (Abis, Abi.ARM64_32)) {
					MonoNativeMode = MonoNativeMode.Unified;
				} else {
					MonoNativeMode = DeploymentTarget.Major >= 3 ? MonoNativeMode.Unified : MonoNativeMode.Compat;
				}
				break;
			case ApplePlatform.MacOSX:
				if (DeploymentTarget >= new Version (10, 12))
					MonoNativeMode = MonoNativeMode.Unified;
				else
					MonoNativeMode = MonoNativeMode.Compat;
				break;
			default:
				throw ErrorHelper.CreateError (71, Errors.MX0071, Platform, ProductName);
			}
		}

		public string GetLibNativeName ()
		{
			switch (MonoNativeMode) {
			case MonoNativeMode.Unified:
				return "libmono-native-unified";
			case MonoNativeMode.Compat:
				return "libmono-native-compat";
			default:
				throw ErrorHelper.CreateError (99, Errors.MX0099, $"Invalid mono native type: '{MonoNativeMode}'");
			}
		}

		public void RunRegistrar ()
		{
			// The static registrar.
			if (Registrar != RegistrarMode.Static)
				throw new ProductException (67, Errors.MT0067, Registrar); // this is only called during our own build

			if (RootAssemblies.Count < 1)
				throw ErrorHelper.CreateError (130, Errors.MX0130);

			var registrar_m = RegistrarOutputLibrary;
			var RootAssembly = RootAssemblies [0];
			var resolvedAssemblies = new Dictionary<string, AssemblyDefinition> ();
			var resolver = new PlatformResolver () {
				RootDirectory = Path.GetDirectoryName (RootAssembly),
#if MMP
				CommandLineAssemblies = RootAssemblies,
#endif
			};

			if (Platform == ApplePlatform.iOS && !Driver.IsDotNet) {
				if (Is32Build) {
					resolver.ArchDirectory = Driver.GetArch32Directory (this);
				} else {
					resolver.ArchDirectory = Driver.GetArch64Directory (this);
				}
			}

			var ps = new ReaderParameters ();
			ps.AssemblyResolver = resolver;
			foreach (var reference in References) {
				var r = resolver.Load (reference);
				if (r == null)
					throw ErrorHelper.CreateError (2002, Errors.MT2002, reference);
			}

			var productAssembly = Driver.GetProductAssembly (this);
			bool foundProductAssembly = false;
			foreach (var asm in RootAssemblies) {
				var rootName = Path.GetFileNameWithoutExtension (asm);
				if (rootName == productAssembly)
					foundProductAssembly = true;
				
				try {
					AssemblyDefinition lastAssembly = ps.AssemblyResolver.Resolve (AssemblyNameReference.Parse (rootName), new ReaderParameters ());
					if (lastAssembly == null) {
						ErrorHelper.CreateWarning (7, Errors.MX0007, rootName);
						continue;
					}
					
					if (resolvedAssemblies.TryGetValue (rootName, out var previousAssembly)) {
						if (lastAssembly.MainModule.RuntimeVersion != previousAssembly.MainModule.RuntimeVersion) {
							Driver.Log (2, "Attemping to load an assembly another time {0} (previous {1})", lastAssembly.FullName, previousAssembly.FullName);
						}
						continue;
					}
					
					resolvedAssemblies.Add (rootName, lastAssembly);
					Driver.Log (3, "Loaded {0}", lastAssembly.MainModule.FileName);
				} catch (Exception ex) {
					ErrorHelper.Warning (9, ex, Errors.MX0009, $"{rootName}: {ex.Message}");
					continue;
				}
			}

			if (!foundProductAssembly)
				throw ErrorHelper.CreateError (131, Errors.MX0131, productAssembly, string.Join ("', '", RootAssemblies.ToArray ()));

#if MONOTOUCH
			if (SelectAbis (Abis, Abi.SimulatorArchMask).Count > 0) {
				BuildTarget = BuildTarget.Simulator;
			} else if (SelectAbis (Abis, Abi.DeviceArchMask).Count > 0) {
				BuildTarget = BuildTarget.Device;
			} else {
				throw ErrorHelper.CreateError (99, Errors.MX0099, "No valid ABI");
			}
#endif
			var registrar = new Registrar.StaticRegistrar (this);
			if (RootAssemblies.Count == 1)
				registrar.GenerateSingleAssembly (resolver, resolvedAssemblies.Values, Path.ChangeExtension (registrar_m, "h"), registrar_m, Path.GetFileNameWithoutExtension (RootAssembly));
			else
				registrar.Generate (resolvedAssemblies.Values, Path.ChangeExtension (registrar_m, "h"), registrar_m);
		}

		public IEnumerable<Abi> Abis {
			get { return abis; }
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
					throw ErrorHelper.CreateError (76, Errors.MT0076, "Xamarin.WatchOS");
				break;
			case ApplePlatform.TVOS:
				if (abis.Count == 0)
					throw ErrorHelper.CreateError (76, Errors.MT0076, "Xamarin.TVOS");
				break;
			case ApplePlatform.MacOSX:
				if (abis.Count == 0)
					abis.Add (Abi.x86_64);
				break;
			default:
				throw ErrorHelper.CreateError (71, Errors.MX0071, Platform, ProductName);
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
					validAbis.Add (Abi.ARM64_32);
					validAbis.Add (Abi.ARM64_32 | Abi.LLVM);
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
			case ApplePlatform.MacOSX:
				validAbis.Add (Abi.x86_64);
				break;
			default:
				throw ErrorHelper.CreateError (71, Errors.MX0071, Platform, ProductName);
			}

#if MMP
			// This is technically not needed, because we'll fail the validation just below, but this handles
			// a common case (existing 32-bit projects) and shows a better error message.
			if (abis.Count == 1 && abis [0] == Abi.i386)
				throw ErrorHelper.CreateError (144, Errors.MM0144);
#endif

			foreach (var abi in abis) {
				if (!validAbis.Contains (abi))
					throw ErrorHelper.CreateError (75, Errors.MT0075, abi, Platform, string.Join (", ", validAbis.Select ((v) => v.AsString ()).ToArray ()));
			}
		}

		public void ClearAbi ()
		{
			abis = null;
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
				case "arm64_32":
					value = Abi.ARM64_32;
					break;
				case "arm64_32+llvm":
					value = Abi.ARM64_32 | Abi.LLVM;
					break;
				case "armv7k":
					value = Abi.ARMv7k;
					break;
				case "armv7k+llvm":
					value = Abi.ARMv7k | Abi.LLVM;
					break;
				default:
					throw ErrorHelper.CreateError (15, Errors.MT0015, str);
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

		public string MonoGCParams {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
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
				case ApplePlatform.MacOSX:
					return EnableSGenConc ? "major=marksweep-conc" : "major=marksweep";
				default:
					throw ErrorHelper.CreateError (71, Errors.MX0071, Platform, ProductName);
				}
			}
		}

		// This is to load the symbols for all assemblies, so that we can give better error messages
		// (with file name / line number information).
		public void LoadSymbols ()
		{
			foreach (var t in Targets)
				t.LoadSymbols ();
		}

		public bool IsFrameworkAvailableInSimulator (string framework)
		{
			if (!Driver.GetFrameworks (this).TryGetValue (framework, out var fw))
				return true; // Unknown framework, assume it's valid for the simulator

			return fw.IsFrameworkAvailableInSimulator (this);
		}

		public static bool TryParseManagedExceptionMode (string value, out MarshalManagedExceptionMode mode)
		{
			mode = MarshalManagedExceptionMode.Default;

			switch (value) {
			case "default":
				mode = MarshalManagedExceptionMode.Default;
				break;
			case "unwindnative":
			case "unwindnativecode":
				mode = MarshalManagedExceptionMode.UnwindNativeCode;
				break;
			case "throwobjectivec":
			case "throwobjectivecexception":
				mode = MarshalManagedExceptionMode.ThrowObjectiveCException;
				break;
			case "abort":
				mode = MarshalManagedExceptionMode.Abort;
				break;
			case "disable":
				mode = MarshalManagedExceptionMode.Disable;
				break;
			default:
				return false;
			}

			return true;
		}

		public static bool TryParseObjectiveCExceptionMode (string value, out MarshalObjectiveCExceptionMode mode)
		{
			mode = MarshalObjectiveCExceptionMode.Default;
			switch (value) {
			case "default":
				mode = MarshalObjectiveCExceptionMode.Default;
				break;
			case "unwindmanaged":
			case "unwindmanagedcode":
				mode = MarshalObjectiveCExceptionMode.UnwindManagedCode;
				break;
			case "throwmanaged":
			case "throwmanagedexception":
				mode = MarshalObjectiveCExceptionMode.ThrowManagedException;
				break;
			case "abort":
				mode = MarshalObjectiveCExceptionMode.Abort;
				break;
			case "disable":
				mode = MarshalObjectiveCExceptionMode.Disable;
				break;
			default:
				return false;
			}
			return true;
		}

		public void SetManagedExceptionMode ()
		{
			switch (MarshalManagedExceptions) {
			case MarshalManagedExceptionMode.Default:
				if (EnableCoopGC.Value) {
					MarshalManagedExceptions = MarshalManagedExceptionMode.ThrowObjectiveCException;
				} else {
					switch (Platform) {
					case ApplePlatform.iOS:
					case ApplePlatform.TVOS:
					case ApplePlatform.WatchOS:
						MarshalManagedExceptions = EnableDebug && IsSimulatorBuild ? MarshalManagedExceptionMode.UnwindNativeCode : MarshalManagedExceptionMode.Disable;
						break;
					case ApplePlatform.MacOSX:
						MarshalManagedExceptions = EnableDebug ? MarshalManagedExceptionMode.UnwindNativeCode : MarshalManagedExceptionMode.Disable;
						break;
					default:
						throw ErrorHelper.CreateError (71, Errors.MX0071 /* Unknown platform: {0}. This usually indicates a bug in {1}; please file a bug report at https://github.com/xamarin/xamarin-macios/issues/new with a test case. */, Platform, ProductName);
					}
				}
				IsDefaultMarshalManagedExceptionMode = true;
				break;
			case MarshalManagedExceptionMode.UnwindNativeCode:
			case MarshalManagedExceptionMode.Disable:
				if (EnableCoopGC.Value)
					throw ErrorHelper.CreateError (89, Errors.MT0089, "--marshal-managed-exceptions", MarshalManagedExceptions.ToString ().ToLowerInvariant ());
				break;
			}
		}

		public void SetObjectiveCExceptionMode ()
		{
			switch (MarshalObjectiveCExceptions) {
			case MarshalObjectiveCExceptionMode.Default:
				if (EnableCoopGC.Value) {
					MarshalObjectiveCExceptions = MarshalObjectiveCExceptionMode.ThrowManagedException;
				} else {
					switch (Platform) {
					case ApplePlatform.iOS:
					case ApplePlatform.TVOS:
					case ApplePlatform.WatchOS:
						MarshalObjectiveCExceptions = EnableDebug && IsSimulatorBuild ? MarshalObjectiveCExceptionMode.UnwindManagedCode : MarshalObjectiveCExceptionMode.Disable;
						break;
					case ApplePlatform.MacOSX:
						MarshalObjectiveCExceptions = EnableDebug ? MarshalObjectiveCExceptionMode.ThrowManagedException : MarshalObjectiveCExceptionMode.Disable;
						break;
					default:
						throw ErrorHelper.CreateError (71, Errors.MX0071 /* Unknown platform: {0}. This usually indicates a bug in {1}; please file a bug report at https://github.com/xamarin/xamarin-macios/issues/new with a test case. */, Platform, ProductName);
					}
				}
				break;
			case MarshalObjectiveCExceptionMode.UnwindManagedCode:
			case MarshalObjectiveCExceptionMode.Disable:
				if (EnableCoopGC.Value)
					throw ErrorHelper.CreateError (89, Errors.MT0089, "--marshal-objectivec-exceptions", MarshalObjectiveCExceptions.ToString ().ToLowerInvariant ());
				break;
			}
		}
	}
}
