using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Linker;
using Mono.Tuner;

using Xamarin;
using Xamarin.Linker;
using Xamarin.MacDev;
using Xamarin.Utils;

using ObjCRuntime;

using ClassRedirector;

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
		ManagedStatic,
	}

	public partial class Application {
		public Cache Cache;
		public string AppDirectory = ".";
		public bool DeadStrip = true;
		public bool EnableDebug;
		// The list of assemblies that we do generate debugging info for.
		public bool DebugAll;
		public bool UseInterpreter; // Only applicable to mobile platforms.
		public List<string> DebugAssemblies = new List<string> ();
		internal RuntimeOptions RuntimeOptions;
		public Optimizations Optimizations = new Optimizations ();
		public RegistrarMode Registrar = RegistrarMode.Default;
		public RegistrarOptions RegistrarOptions = RegistrarOptions.Default;
		public SymbolMode SymbolMode;
		public HashSet<string> IgnoredSymbols = new HashSet<string> ();

		// The AOT arguments are currently not used for macOS, but they could eventually be used there as well (there's no mmp option to set these yet).
		public List<string> AotArguments = new List<string> ();
		public List<string> AotOtherArguments = null;
		public bool? AotFloat32 = null;

		public DlsymOptions DlsymOptions;
		public List<Tuple<string, bool>> DlsymAssemblies;
		public List<string> CustomLinkFlags;

		public string CompilerPath;

		public Application ContainerApp; // For extensions, this is the containing app
		public bool IsCodeShared { get; private set; }

		public HashSet<string> Frameworks = new HashSet<string> ();
		public HashSet<string> WeakFrameworks = new HashSet<string> ();

		public bool IsExtension;
		public ApplePlatform Platform { get { return Driver.TargetFramework.Platform; } }

		public List<string> MonoLibraries = new List<string> ();
		public List<string> InterpretedAssemblies = new List<string> ();

		// EnableMSym: only implemented for Xamarin.iOS
		bool? enable_msym;
		public bool EnableMSym {
			get { return enable_msym.Value; }
			set { enable_msym = value; }
		}

		// Linker config
#if !NET
		public LinkMode LinkMode = LinkMode.Full;
#endif
		bool? are_any_assemblies_trimmed;
		public bool AreAnyAssembliesTrimmed {
			get {
				if (are_any_assemblies_trimmed.HasValue)
					return are_any_assemblies_trimmed.Value;
#if NET
				// This shouldn't happen, we should always set AreAnyAssembliesTrimmed to some value for .NET.
				throw ErrorHelper.CreateError (99, "A custom LinkMode value is not supported for .NET");
#else
				return LinkMode != LinkMode.None;
#endif
			}
			set {
				are_any_assemblies_trimmed = value;
			}
		}
		public List<string> LinkSkipped = new List<string> ();
		public List<string> Definitions = new List<string> ();
#if !NET
		public I18nAssemblies I18n;
#endif
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

		public bool? DisableLldbAttach = null; // Only applicable to Xamarin.Mac
		public bool? DisableOmitFramePointer = null; // Only applicable to Xamarin.Mac
		public string CustomBundleName = "MonoBundle"; // Only applicable to Xamarin.Mac and Mac Catalyst

		public XamarinRuntime XamarinRuntime;
		public bool? UseMonoFramework;
		public string RuntimeIdentifier; // Only used for build-time --run-registrar support

		// The bitcode mode to compile to.
		// This variable does not apply to macOS, because there's no bitcode on macOS.
		public BitCodeMode BitCodeMode { get; set; }

		public bool EnableAsmOnlyBitCode { get { return BitCodeMode == BitCodeMode.ASMOnly; } }
		public bool EnableLLVMOnlyBitCode { get { return BitCodeMode == BitCodeMode.LLVMOnly; } }
		public bool EnableMarkerOnlyBitCode { get { return BitCodeMode == BitCodeMode.MarkerOnly; } }
		public bool EnableBitCode { get { return BitCodeMode != BitCodeMode.None; } }

		public bool SkipMarkingNSObjectsInUserAssemblies { get; set; }

		// check if needs to be removed: https://github.com/xamarin/xamarin-macios/issues/18693
		public bool DisableAutomaticLinkerSelection { get; set; }

		// assembly_build_targets describes what kind of native code each assembly should be compiled into for mobile targets (iOS, tvOS, watchOS).
		// An assembly can be compiled into: static object (.o), dynamic library (.dylib) or a framework (.framework).
		// In the case of a framework, each framework may contain the native code for multiple assemblies.
		// This variable does not apply to macOS (if assemblies are AOT-compiled, the AOT compiler will output a .dylib next to the assembly and there's nothing extra for us)
		Dictionary<string, Tuple<AssemblyBuildTarget, string>> assembly_build_targets = new Dictionary<string, Tuple<AssemblyBuildTarget, string>> ();

		public string ContentDirectory {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
					return AppDirectory;
				case ApplePlatform.MacOSX:
				case ApplePlatform.MacCatalyst:
					return Path.Combine (AppDirectory, "Contents", CustomBundleName);
				default:
					throw ErrorHelper.CreateError (71, Errors.MX0071, Platform, ProductName);
				}
			}
		}

		public string FrameworksDirectory {
			get {
				return Path.Combine (AppDirectory, RelativeFrameworksPath);
			}
		}

		public string RelativeFrameworksPath {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
					return "Frameworks";
				case ApplePlatform.MacOSX:
				case ApplePlatform.MacCatalyst:
					return Path.Combine ("Contents", "Frameworks");
				default:
					throw ErrorHelper.CreateError (71, Errors.MX0071, Platform, ProductName);
				}
			}
		}

		public string RelativeDylibPublishPath {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
					return string.Empty;
				case ApplePlatform.MacOSX:
				case ApplePlatform.MacCatalyst:
					return Path.Combine ("Contents", CustomBundleName);
				default:
					throw ErrorHelper.CreateError (71, Errors.MX0071, Platform, ProductName);
				}
			}
		}

		// How Mono should be embedded into the app.
		AssemblyBuildTarget? libmono_link_mode;
		public AssemblyBuildTarget LibMonoLinkMode {
			get {
				if (libmono_link_mode.HasValue)
					return libmono_link_mode.Value;

				if (Platform == ApplePlatform.MacOSX) {
					// This property was implemented for iOS, but might be re-used for macOS if desired after testing to verify it works as expected.
					throw ErrorHelper.CreateError (99, Errors.MX0099, "LibMonoLinkMode isn't a valid operation for macOS apps.");
				}

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
			set {
				libmono_link_mode = value;
			}
		}

		// How libxamarin should be embedded into the app.
		AssemblyBuildTarget? libxamarin_link_mode;
		public AssemblyBuildTarget LibXamarinLinkMode {
			get {
				if (libxamarin_link_mode.HasValue)
					return libxamarin_link_mode.Value;

				if (Platform == ApplePlatform.MacOSX) {
					// This property was implemented for iOS, but might be re-used for macOS if desired after testing to verify it works as expected.
					throw ErrorHelper.CreateError (99, Errors.MX0099, "LibXamarinLinkMode isn't a valid operation for macOS apps.");
				}

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
			set {
				libxamarin_link_mode = value;
			}
		}

		// How the generated libpinvoke library should be linked into the app.
		public AssemblyBuildTarget LibPInvokesLinkMode => LibXamarinLinkMode;
		// How the profiler library should be linked into the app.
		public AssemblyBuildTarget LibProfilerLinkMode => OnlyStaticLibraries ? AssemblyBuildTarget.StaticObject : AssemblyBuildTarget.DynamicLibrary;

		// How the libmononative library should be linked into the app.
		public AssemblyBuildTarget LibMonoNativeLinkMode {
			get {
				// if there's a specific way libmono is being linked, use the same way.
				if (libmono_link_mode.HasValue)
					return libmono_link_mode.Value;
				return HasDynamicLibraries ? AssemblyBuildTarget.DynamicLibrary : AssemblyBuildTarget.StaticObject;
			}
		}

		// If all assemblies are compiled into static libraries.
		public bool OnlyStaticLibraries {
			get {
				if (Platform == ApplePlatform.MacOSX)
					throw ErrorHelper.CreateError (99, Errors.MX0099, "Using assembly_build_targets isn't a valid operation for macOS apps.");

				return assembly_build_targets.All ((abt) => abt.Value.Item1 == AssemblyBuildTarget.StaticObject);
			}
		}

		// If any assembly in the app is compiled into a dynamic library.
		public bool HasDynamicLibraries {
			get {
				if (Platform == ApplePlatform.MacOSX)
					throw ErrorHelper.CreateError (99, Errors.MX0099, "Using assembly_build_targets isn't a valid operation for macOS apps.");

				return assembly_build_targets.Any ((abt) => abt.Value.Item1 == AssemblyBuildTarget.DynamicLibrary);
			}
		}

		// If any assembly in the app is compiled into a framework.
		public bool HasFrameworks {
			get {
				if (Platform == ApplePlatform.MacOSX)
					throw ErrorHelper.CreateError (99, Errors.MX0099, "Using assembly_build_targets isn't a valid operation for macOS apps.");

				return assembly_build_targets.Any ((abt) => abt.Value.Item1 == AssemblyBuildTarget.Framework);
			}
		}

		// If this application has a Frameworks directory (or if any frameworks should be put in a containing app's Framework directory).
		// This is used to know where to place embedded .frameworks (for app extensions they should go into the containing app's Frameworks directory).
		// This logic works on all platforms.
		public bool HasFrameworksDirectory {
			get {
				if (!IsExtension)
					return true;

				if (IsWatchExtension && Platform == ApplePlatform.WatchOS)
					return true;

				return false;
			}
		}

		bool RequiresXcodeHeaders {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
				case ApplePlatform.MacCatalyst:
					return !AreAnyAssembliesTrimmed;
				case ApplePlatform.MacOSX:
					return (Registrar == RegistrarMode.Static || Registrar == RegistrarMode.ManagedStatic) && !AreAnyAssembliesTrimmed;
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
				case ApplePlatform.MacCatalyst:
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
				case ApplePlatform.MacCatalyst:
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
				if (!string.IsNullOrEmpty (RuntimeIdentifier))
					return !IsSimulatorBuild;

				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
					return BuildTarget == BuildTarget.Device;
				case ApplePlatform.MacOSX:
				case ApplePlatform.MacCatalyst:
					return false;
				default:
					throw ErrorHelper.CreateError (71, Errors.MX0071, Platform, ProductName);
				}
			}
		}

		public bool IsSimulatorBuild {
			get {
				if (!string.IsNullOrEmpty (RuntimeIdentifier))
					return RuntimeIdentifier.IndexOf ("simulator", StringComparison.OrdinalIgnoreCase) >= 0;

				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
					return BuildTarget == BuildTarget.Simulator;
				case ApplePlatform.MacOSX:
				case ApplePlatform.MacCatalyst:
					return false;
				default:
					throw ErrorHelper.CreateError (71, Errors.MX0071, Platform, ProductName);
				}
			}
		}

		// It seems the watch simulator is able to correctly select which architecture to use
		// for a fat executable, so limit ourselves to arch-specific executables anymore.
		public bool ArchSpecificExecutable {
			get {
				return !IsWatchExtension;
			}
		}

#if !NET
		public static int Concurrency => Driver.Concurrency;
#endif
		public Version DeploymentTarget;
		public Version SdkVersion; // for Mac Catalyst this is the iOS version
		public Version NativeSdkVersion; // this is the same as SdkVersion, except that for Mac Catalyst it's the macOS SDK version.

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

		public Version GetMacCatalystmacOSVersion (Version iOSVersion)
		{
			if (!MacCatalystSupport.TryGetMacOSVersion (Driver.GetFrameworkDirectory (this), iOSVersion, out var value, out var knowniOSVersions))
				throw ErrorHelper.CreateError (183, Errors.MX0183 /* Could not map the Mac Catalyst version {0} to a corresponding macOS version. Valid Mac Catalyst versions are: {1} */, iOSVersion.ToString (), string.Join (", ", knowniOSVersions));

			return value;
		}

		public Version GetMacCatalystiOSVersion (Version macOSVersion)
		{
			if (!MacCatalystSupport.TryGetiOSVersion (Driver.GetFrameworkDirectory (this), macOSVersion, out var value, out var knownMacOSVersions))
				throw ErrorHelper.CreateError (184, Errors.MX0184 /* Could not map the macOS version {0} to a corresponding Mac Catalyst version. Valid macOS versions are: {1} */, macOSVersion.ToString (), string.Join (", ", knownMacOSVersions));

			return value;
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

		public void ParseCustomLinkFlags (string value, string value_name)
		{
			if (!StringUtils.TryParseArguments (value, out var lf, out var ex))
				throw ErrorHelper.CreateError (26, ex, Errors.MX0026, $"-{value_name}={value}", ex.Message);
			if (CustomLinkFlags is null)
				CustomLinkFlags = new List<string> ();
			CustomLinkFlags.AddRange (lf);
		}

		public void ParseInterpreter (string value)
		{
			UseInterpreter = true;
			if (!string.IsNullOrEmpty (value))
				InterpretedAssemblies.AddRange (value.Split (new char [] { ',' }, StringSplitOptions.RemoveEmptyEntries));
		}

#if !NET
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
#endif

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

				var plist = Driver.FromPList (InfoPListPath);
				var dict = plist.Get<PDictionary> ("NSExtension");
				if (dict is null)
					return null;
				return dict.GetString ("NSExtensionPointIdentifier");
			}
		}

		string info_plistpath;
		public string InfoPListPath {
			get {
				if (info_plistpath is not null)
					return info_plistpath;

				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
					return Path.Combine (AppDirectory, "Info.plist");
				case ApplePlatform.MacCatalyst:
				case ApplePlatform.MacOSX:
					return Path.Combine (AppDirectory, "Contents", "Info.plist");
				default:
					throw ErrorHelper.CreateError (71, Errors.MX0071, Platform, ProductName);
				}
			}
			set {
				info_plistpath = value;
			}
		}

		// This is just a name for this app to show in log/error messages, etc.
		public string Name {
			get { return Path.GetFileNameWithoutExtension (AppDirectory); }
		}

		bool? requires_pinvoke_wrappers;
		public bool RequiresPInvokeWrappers {
			get {
				if (requires_pinvoke_wrappers.HasValue)
					return requires_pinvoke_wrappers.Value;

				// By default this is disabled for .NET
				if (Driver.IsDotNet)
					return false;

				if (Platform == ApplePlatform.MacOSX)
					return false;

				if (IsSimulatorBuild)
					return false;

				if (Platform == ApplePlatform.MacCatalyst)
					return false;

				return MarshalObjectiveCExceptions == MarshalObjectiveCExceptionMode.ThrowManagedException || MarshalObjectiveCExceptions == MarshalObjectiveCExceptionMode.Abort;
			}
			set {
				requires_pinvoke_wrappers = value;
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
				case ApplePlatform.MacCatalyst:
					return "MacCatalyst";
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
				EmbeddedResource embedded = module.Resources [i] as EmbeddedResource;

				if (embedded is null || embedded.Name != name)
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

		public static bool ExtractResource (ModuleDefinition module, string name, string path, bool remove)
		{
			for (int i = 0; i < module.Resources.Count; i++) {
				EmbeddedResource embedded = module.Resources [i] as EmbeddedResource;

				if (embedded is null || embedded.Name != name)
					continue;

				string dirname = Path.GetDirectoryName (path);
				if (!Directory.Exists (dirname))
					Directory.CreateDirectory (dirname);

				using (Stream ostream = File.OpenWrite (path)) {
					embedded.GetResourceStream ().CopyTo (ostream);
				}

				if (remove)
					module.Resources.RemoveAt (i);

				return true;
			}

			return false;
		}

		// Returns true if the source file was copied to the target or false if it was already up to date.
		public static bool UpdateFile (string source, string target, bool check_contents = false)
		{
			if (!Application.IsUptodate (source, target, check_contents)) {
				CopyFile (source, target);
				return true;
			} else {
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
			return FileCopier.IsUptodate (sources, targets, check_stamp);
		}

		public static void UpdateDirectory (string source, string target)
		{
			FileCopier.UpdateDirectory (source, target);
		}

		static string [] NonEssentialDirectoriesInsideFrameworks = { "CVS", ".svn", ".git", ".hg", "Headers", "PrivateHeaders", "Modules" };

		// Duplicate xcode's `builtin-copy` exclusions
		public static void ExcludeNonEssentialFrameworkFiles (string framework)
		{
			// builtin-copy -exclude .DS_Store -exclude CVS -exclude .svn -exclude .git -exclude .hg -exclude Headers -exclude PrivateHeaders -exclude Modules -exclude \*.tbd
			File.Delete (Path.Combine (framework, ".DS_Store"));
			File.Delete (Path.Combine (framework, "*.tbd"));
			foreach (var dir in NonEssentialDirectoriesInsideFrameworks)
				DeleteDir (Path.Combine (framework, dir));
		}

		static void DeleteDir (string dir)
		{
			// Xcode generates symlinks inside macOS frameworks
			var realdir = Target.GetRealPath (dir, warnIfNoSuchPathExists: false);
			// unlike File.Delete this would throw if the directory does not exists
			if (Directory.Exists (realdir)) {
				Directory.Delete (realdir, true);
				if (realdir != dir)
					File.Delete (dir); // because a symlink is a file :)
			}
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

		public void InitializeCommon ()
		{
			InitializeDeploymentTarget ();
			SelectRegistrar ();
			SelectMonoNative ();

			RuntimeOptions = RuntimeOptions.Create (this, HttpMessageHandler, TlsProvider);

			if (Platform == ApplePlatform.MacCatalyst) {
				// Our input SdkVersion is the macOS SDK version, but the rest of our code expects the supporting iOS version, so convert here.
				// The macOS SDK version is still stored in NativeSdkVersion for when we need it.
				SdkVersion = GetMacCatalystiOSVersion (NativeSdkVersion);
			}

			if (RequiresXcodeHeaders && SdkVersion < SdkVersions.GetVersion (this)) {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
				case ApplePlatform.MacCatalyst:
					throw ErrorHelper.CreateError (180, Errors.MX0180, ProductName, PlatformName, SdkVersions.GetVersion (this), SdkVersions.Xcode);
				case ApplePlatform.MacOSX:
					throw ErrorHelper.CreateError (179, Errors.MX0179, ProductName, PlatformName, SdkVersions.GetVersion (this), SdkVersions.Xcode);
				default:
					// Default to the iOS error message, it's better than showing MX0071 (unknown platform), which would be completely unrelated
					goto case ApplePlatform.iOS;
				}
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

			if (Driver.XcodeVersion.Major >= 14 && BitCodeMode != BitCodeMode.None && Platform == ApplePlatform.TVOS) {
				// We currently have to leave watchOS alone, because the process is to first build bitcode, then compile bitcode into native code, and finally remove the bitcode from the executable (this is likely fixable, but looks like it's a larger effort involving the runtime team).
				ErrorHelper.Warning (186, Errors.MX0186 /* Bitcode is enabled, but bitcode is not supported in Xcode 14+ and has been disabled. Please disable bitcode by removing the 'MtouchEnableBitcode' property from the project file. */);
				BitCodeMode = BitCodeMode.None;
			}

			Optimizations.Initialize (this, out var messages);
			ErrorHelper.Show (messages);
			if (Driver.Verbosity > 3)
				Driver.Log (4, $"Enabled optimizations: {Optimizations}");
		}

		void InitializeDeploymentTarget ()
		{
			if (DeploymentTarget is null)
				DeploymentTarget = SdkVersions.GetVersion (this);

			if (Platform == ApplePlatform.iOS && (HasDynamicLibraries || HasFrameworks) && DeploymentTarget.Major < 8) {
				ErrorHelper.Warning (78, Errors.MT0078, DeploymentTarget);
				DeploymentTarget = new Version (8, 0);
			}

			if (DeploymentTarget is not null) {
				if (DeploymentTarget < SdkVersions.GetMinVersion (this))
					throw new ProductException (73, true, Errors.MT0073, ProductConstants.Version, DeploymentTarget, Xamarin.SdkVersions.GetMinVersion (this), PlatformName, ProductName);
				if (DeploymentTarget > SdkVersions.GetVersion (this))
					throw new ProductException (74, true, Errors.MX0074, ProductConstants.Version, DeploymentTarget, Xamarin.SdkVersions.GetVersion (this), PlatformName, ProductName);
			}
		}

		void SelectMonoNative ()
		{
			switch (Platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
			case ApplePlatform.WatchOS:
			case ApplePlatform.MacOSX:
			case ApplePlatform.MacCatalyst:
				MonoNativeMode = MonoNativeMode.Unified;
				break;
			default:
				throw ErrorHelper.CreateError (71, Errors.MX0071, Platform, ProductName);
			}
		}

		public string GetLibNativeName ()
		{
			switch (MonoNativeMode) {
			case MonoNativeMode.Unified:
				if (Platform == ApplePlatform.MacCatalyst)
					return "libmono-native";

				return "libmono-native-unified";
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
			resolver.Configure ();

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
				if (r is null)
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
					if (lastAssembly is null) {
						ErrorHelper.Warning (7, Errors.MX0007, rootName);
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
			if (RootAssemblies.Count == 1) {
				registrar.GenerateSingleAssembly (resolver, resolvedAssemblies.Values, Path.ChangeExtension (registrar_m, "h"), registrar_m, Path.GetFileNameWithoutExtension (RootAssembly), out var _);
			} else {
				registrar.Generate (resolver, resolvedAssemblies.Values, Path.ChangeExtension (registrar_m, "h"), registrar_m, out var _);
			}
		}

		public IEnumerable<Abi> Abis {
			get { return abis; }
			set { abis = new List<Abi> (value); }
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
			if (abis is null)
				abis = new List<Abi> ();

			switch (Platform) {
			case ApplePlatform.iOS:
				if (abis.Count == 0) {
					if (DeploymentTarget is null || DeploymentTarget.Major >= 11) {
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
			case ApplePlatform.MacCatalyst:
				if (abis.Count == 0)
					throw ErrorHelper.CreateError (76, Errors.MT0076, "Xamarin.MacCatalyst");
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
					validAbis.Add (Abi.x86_64);
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
			case ApplePlatform.MacCatalyst:
				validAbis.Add (Abi.x86_64);
				validAbis.Add (Abi.ARM64);
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
				if (abis is not null) {
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

		public void ParseRegistrar (string v)
		{
			var split = v.Split ('=');
			var name = split [0];
			var value = split.Length > 1 ? split [1] : string.Empty;
			switch (name) {
			case "static":
				Registrar = RegistrarMode.Static;
				break;
			case "dynamic":
				Registrar = RegistrarMode.Dynamic;
				break;
			case "default":
				Registrar = RegistrarMode.Default;
				break;
#if !MTOUCH
			case "partial":
			case "partial-static":
				Registrar = RegistrarMode.PartialStatic;
				break;
#endif
#if NET
			case "managed-static":
				Registrar = RegistrarMode.ManagedStatic;
				break;
#endif
			default:
#if NET
				throw ErrorHelper.CreateError (20, Errors.MX0020, "--registrar", "managed-static, static, dynamic or default");
#else
				throw ErrorHelper.CreateError (20, Errors.MX0020, "--registrar", "static, dynamic or default");
#endif
			}

			switch (value) {
			case "trace":
				RegistrarOptions = RegistrarOptions.Trace;
				break;
			case "default":
			case "":
				RegistrarOptions = RegistrarOptions.Default;
				break;
			default:
				throw ErrorHelper.CreateError (20, Errors.MX0020, "--registrar", "static, dynamic or default");
			}
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
				case ApplePlatform.MacCatalyst:
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
				if (Driver.IsDotNet) {
					MarshalManagedExceptions = MarshalManagedExceptionMode.ThrowObjectiveCException;
				} else if (EnableCoopGC.Value) {
					MarshalManagedExceptions = MarshalManagedExceptionMode.ThrowObjectiveCException;
				} else {
					switch (Platform) {
					case ApplePlatform.iOS:
					case ApplePlatform.TVOS:
					case ApplePlatform.WatchOS:
						MarshalManagedExceptions = EnableDebug && IsSimulatorBuild ? MarshalManagedExceptionMode.UnwindNativeCode : MarshalManagedExceptionMode.Disable;
						break;
					case ApplePlatform.MacOSX:
					case ApplePlatform.MacCatalyst:
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
				if (XamarinRuntime == XamarinRuntime.CoreCLR)
					throw ErrorHelper.CreateError (185, Errors.MX0185 /* The option '{0}' cannot take the value '{1}' when using CoreCLR. */, "--marshal-managed-exceptions", MarshalManagedExceptions.ToString ().ToLowerInvariant ());
				break;
			}
		}

		public void SetObjectiveCExceptionMode ()
		{
			switch (MarshalObjectiveCExceptions) {
			case MarshalObjectiveCExceptionMode.Default:
				if (Driver.IsDotNet) {
					MarshalObjectiveCExceptions = MarshalObjectiveCExceptionMode.ThrowManagedException;
				} else if (EnableCoopGC.Value) {
					MarshalObjectiveCExceptions = MarshalObjectiveCExceptionMode.ThrowManagedException;
				} else {
					switch (Platform) {
					case ApplePlatform.iOS:
					case ApplePlatform.TVOS:
					case ApplePlatform.WatchOS:
						MarshalObjectiveCExceptions = EnableDebug && IsSimulatorBuild ? MarshalObjectiveCExceptionMode.UnwindManagedCode : MarshalObjectiveCExceptionMode.Disable;
						break;
					case ApplePlatform.MacOSX:
					case ApplePlatform.MacCatalyst:
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
				if (XamarinRuntime == XamarinRuntime.CoreCLR)
					throw ErrorHelper.CreateError (185, Errors.MX0185 /* The option '{0}' cannot take the value '{1}' when using CoreCLR. */, "--marshal-objectivec-exceptions", MarshalObjectiveCExceptions.ToString ().ToLowerInvariant ());
				break;
			}
		}

		// For mobile device builds: returns whether an assembly is interpreted.
		// For macOS: N/A
		public bool IsInterpreted (string assembly)
		{
			if (Platform == ApplePlatform.MacOSX)
				throw ErrorHelper.CreateError (99, Errors.MX0099, "IsInterpreted isn't a valid operation for macOS apps.");

#if !NET
			if (IsSimulatorBuild)
				return false;
#endif

			// IsAOTCompiled and IsInterpreted are not opposites: mscorlib.dll can be both.
			if (!UseInterpreter)
				return false;

			// Go through the list of assemblies to interpret in reverse order,
			// so that the last option passed to mtouch takes precedence.
			for (int i = InterpretedAssemblies.Count - 1; i >= 0; i--) {
				var opt = InterpretedAssemblies [i];
				if (opt == "all")
					return true;
				else if (opt == "-all")
					return false;
				else if (opt == assembly)
					return true;
				else if (opt [0] == '-' && opt.Substring (1) == assembly)
					return false;
			}

			// There's an implicit 'all' at the start of the list.
			return true;
		}

		// For mobile device builds: returns whether an assembly is AOT-compiled.
		// For macOS: while AOT is supported for macOS, this particular method was not written for macOS, and would need
		// revision/testing to be used so desired.
		public bool IsAOTCompiled (string assembly)
		{
#if NET
			if (Platform == ApplePlatform.MacOSX)
				return false; // AOT on .NET for macOS hasn't been implemented yet.
#else
			if (Platform == ApplePlatform.MacOSX)
				throw ErrorHelper.CreateError (99, Errors.MX0099, "IsAOTCompiled isn't a valid operation for macOS apps.");
#endif
			if (!UseInterpreter) {
				if (Platform == ApplePlatform.MacCatalyst)
					return IsArchEnabled (Abi.ARM64);

				if (IsSimulatorBuild && IsArchEnabled (Abi.ARM64))
					return true;

				return IsDeviceBuild;
			}

			// IsAOTCompiled and IsInterpreted are not opposites: mscorlib.dll can be both:
			// - mscorlib will always be processed by the AOT compiler to generate required wrapper functions for the interpreter to work
			// - mscorlib might also be fully AOT-compiled (both when the interpreter is enabled and when it's not)
			if (assembly == Driver.CorlibName)
				return true;

			return !IsInterpreted (assembly);
		}

		public IList<string> GetAotArguments (string filename, Abi abi, string outputDir, string outputFile, string llvmOutputFile, string dataFile)
		{
			GetAotArguments (filename, abi, outputDir, outputFile, llvmOutputFile, dataFile, null, out var processArguments, out var aotArguments);
			processArguments.Add (string.Join (",", aotArguments));
			processArguments.Add (filename);
			return processArguments;
		}

		public void GetAotArguments (string filename, Abi abi, string outputDir, string outputFile, string llvmOutputFile, string dataFile, bool? isDedupAssembly, out List<string> processArguments, out List<string> aotArguments, string llvm_path = null)
		{
			string fname = Path.GetFileName (filename);
			processArguments = new List<string> ();
			var app = this;
			bool enable_llvm = (abi & Abi.LLVM) != 0;
			bool enable_thumb = (abi & Abi.Thumb) != 0;
			bool enable_debug = app.EnableDebug;
			bool enable_debug_symbols = app.PackageManagedDebugSymbols;
			bool llvm_only = app.EnableLLVMOnlyBitCode;
			bool interp = app.IsInterpreted (Assembly.GetIdentity (filename));
			bool interp_full = !interp && app.UseInterpreter;
			bool is32bit = (abi & Abi.Arch32Mask) > 0;
			string arch = abi.AsArchString ();

			processArguments.Add ("--debug");

			if (enable_llvm)
				processArguments.Add ("--llvm");

			if (!llvm_only && !interp)
				processArguments.Add ("-O=gsharedvt");
			if (app.AotOtherArguments is not null)
				processArguments.AddRange (app.AotOtherArguments);
			if (app.AotFloat32.HasValue)
				processArguments.Add (app.AotFloat32.Value ? "-O=float32" : "-O=-float32");
			aotArguments = new List<string> ();
			if (Platform == ApplePlatform.MacCatalyst) {
				aotArguments.Add ($"--aot=mtriple={arch}-apple-ios{DeploymentTarget}-macabi");
			} else {
				aotArguments.Add ($"--aot=mtriple={(enable_thumb ? arch.Replace ("arm", "thumb") : arch)}-ios");
			}
			aotArguments.Add ($"data-outfile={dataFile}");
			aotArguments.Add ("static");
			aotArguments.Add ("asmonly");
			// This method is used in legacy build as well, where dedup is not supported. 
			// Variable isDedupAssembly could have the following values:
			// - NULL means that dedup is not enabled
			// - FALSE means that dedup-skip flag should be passed for all assemblies except a container assemblt
			// - TRUE means that dedup-include flag should be passed for the container assembly
			if (isDedupAssembly.HasValue) {
				if (isDedupAssembly.Value) {
					aotArguments.Add ($"dedup-include={fname}");
				} else {
					aotArguments.Add ($"dedup-skip");
				}
			}
			if (app.LibMonoLinkMode == AssemblyBuildTarget.StaticObject || !Driver.IsDotNet)
				aotArguments.Add ("direct-icalls");
			aotArguments.AddRange (app.AotArguments);
			if (llvm_only)
				aotArguments.Add ("llvmonly");
			else if (interp) {
				if (fname != Driver.CorlibName + ".dll")
					throw ErrorHelper.CreateError (99, Errors.MX0099, fname);
				aotArguments.Add ("interp");
			} else if (interp_full) {
				aotArguments.Add ("interp");
				aotArguments.Add ("full");
			} else
				aotArguments.Add ("full");

			if (IsDeviceBuild) {
				aotArguments.Add ("readonly-value=ObjCRuntime.Runtime.Arch=i4/0");
			} else if (IsSimulatorBuild) {
				aotArguments.Add ("readonly-value=ObjCRuntime.Runtime.Arch=i4/1");
			}

			var aname = Path.GetFileNameWithoutExtension (fname);
			var sdk_or_product = Profile.IsSdkAssembly (aname) || Profile.IsProductAssembly (aname);

			if (enable_llvm)
				aotArguments.Add ("nodebug");
			else if (!(enable_debug || enable_debug_symbols))
				aotArguments.Add ("nodebug");
			else if (app.DebugAll || app.DebugAssemblies.Contains (fname) || !sdk_or_product)
				aotArguments.Add ("soft-debug");

			aotArguments.Add ("dwarfdebug");

			/* Needed for #4587 */
			if (enable_debug && !enable_llvm)
				aotArguments.Add ("no-direct-calls");

			if (!app.UseDlsym (filename))
				aotArguments.Add ("direct-pinvoke");

			if (app.EnableMSym) {
				var msymdir = Path.Combine (outputDir, "Msym");
				aotArguments.Add ($"msym-dir={msymdir}");
			}

			if (enable_llvm) {
				if (!string.IsNullOrEmpty (llvm_path)) {
					aotArguments.Add ($"llvm-path={llvm_path}");
				} else {
					aotArguments.Add ($"llvm-path={Driver.GetFrameworkCurrentDirectory (app)}/LLVM/bin/");
				}
			}

			aotArguments.Add ($"outfile={outputFile}");
			if (enable_llvm)
				aotArguments.Add ($"llvm-outfile={llvmOutputFile}");

#if NET
			// If the interpreter is enabled, and we're building for x86_64, we're AOT-compiling but we
			// don't have access to infinite trampolines. So we're bumping the trampoline count (unless
			// the developer has already set a value) to something higher than the default.
			//
			// Ref:
			// * https://github.com/xamarin/xamarin-macios/issues/14887
			// * https://github.com/dotnet/runtime/issues/68808
			if (interp && (abi & Abi.x86_64) == Abi.x86_64) {
				// The default values are here: https://github.com/dotnet/runtime/blob/main/src/mono/mono/mini/aot-compiler.c#L13945-L13953
				// Let's try 4x the default values.
				var trampolines = new []
				{
					(Name: "ntrampolines", Default: 4096),
					(Name: "nrgctx-trampolines", Default: 4096),
					(Name: "nimt-trampolines", Default: 512),
					(Name: "nrgctx-fetch-trampolines", Default: 128),
					(Name: "ngsharedvt-trampolines", Default: 512),
					(Name: "nftnptr-arg-trampolines", Default: 128),
					(Name: "nunbox-arbitrary-trampolines", Default: 256),
				};
				foreach (var tramp in trampolines) {
					var nameWithEq = tramp.Name + "=";
					if (!aotArguments.Any (v => v.StartsWith (nameWithEq, StringComparison.Ordinal)))
						aotArguments.Add (nameWithEq + (tramp.Default * 4).ToString (CultureInfo.InvariantCulture));
				}
			}
#endif
		}

		public string AssemblyName {
			get {
				return Path.GetFileName (RootAssemblies [0]);
			}
		}

		internal ProductConstants ProductConstants {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.MacCatalyst:
					return ProductConstants.iOS;
				case ApplePlatform.TVOS:
					return ProductConstants.tvOS;
				case ApplePlatform.WatchOS:
					return ProductConstants.watchOS;
				case ApplePlatform.MacOSX:
					return ProductConstants.macOS;
				default:
					throw ErrorHelper.CreateError (71, Errors.MX0071, Platform, ProductName);
				}
			}
		}

		public void SetDlsymOption (string asm, bool dlsym)
		{
			if (DlsymAssemblies is null)
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
				if (DlsymAssemblies is null)
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

			if (DlsymAssemblies is not null) {
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

			// Even if this assembly is aot'ed, if we are using the interpreter we can't yet
			// guarantee that code in this assembly won't be executed in interpreted mode,
			// which can happen for virtual calls between assemblies, during exception handling
			// etc. We make sure we don't strip away symbols needed for pinvoke calls.
			// https://github.com/mono/mono/issues/14206
			if (UseInterpreter)
				return true;

			// There are native frameworks which aren't available in the simulator, and we have
			// bound P/Invokes to those native frameworks. This means that AOT-compiling for
			// the simulator will fail because the corresponding native functions don't exist.
			// So default to dlsym for the simulator.
			if (IsSimulatorBuild && Profile.IsProductAssembly (Path.GetFileNameWithoutExtension (assembly)))
				return true;

			switch (Platform) {
			case ApplePlatform.iOS:
				return !Profile.IsSdkAssembly (Path.GetFileNameWithoutExtension (assembly));
			case ApplePlatform.TVOS:
			case ApplePlatform.WatchOS:
				return false;
			case ApplePlatform.MacCatalyst:
				// https://github.com/xamarin/xamarin-macios/issues/14437
				return true;
			default:
				throw ErrorHelper.CreateError (71, Errors.MX0071, Platform, ProductName);
			}
		}

		public bool VerifyDynamicFramework (string framework_path)
		{
			var framework_filename = Path.Combine (framework_path, Path.GetFileNameWithoutExtension (framework_path));
			var dynamic = false;

			try {
				dynamic = MachO.IsDynamicFramework (framework_filename);
			} catch (Exception e) {
				throw ErrorHelper.CreateError (140, e, Errors.MT0140, framework_filename);
			}

			if (!dynamic)
				Driver.Log (1, "The framework {0} is a framework of static libraries, and will not be copied to the app.", framework_path);

			return dynamic;
		}
	}
}
