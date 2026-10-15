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

using Registrar;

#if !LEGACY_TOOLS && !ASSEMBLY_PREPARER
using ClassRedirector;
#endif

#if LEGACY_TOOLS
using PlatformResolver = MonoTouch.Tuner.MonoTouchResolver;
#else
using PlatformResolver = Xamarin.Linker.DotNetResolver;
#endif

#nullable enable

#if LEGACY_TOOLS
namespace Mono.Linker {
	public class LinkContext {
		public IEnumerable<AssemblyDefinition> GetAssemblies ()
		{
			throw new System.NotImplementedException ();
		}
	}
}
#endif // LEGACY_TOOLS

namespace Xamarin.Bundler {

	public enum BuildTarget {
		None,
		Simulator,
		Device,
	}

	[Flags]
	public enum RegistrarOptions {
		Default = 0,
		Trace = 1,
	}

	public enum ExportAttributeRemovalBlocker {
		DynamicRegistrationSupported,
		BlockLiteralSetupBlockOptimizationDisabled,
		StaticBlockToDelegateLookupOptimizationDisabled,
		RuntimeGetBlockWrapperCreatorRequired,
		RegistrarHelperGetBlockForDelegateRequired,
		NSXpcInterfaceMethodInfoOverloadUsed,
	}

	public partial class Application : IToolLog {
		public Cache? Cache;
		public string AppDirectory = ".";
		public bool DeadStrip = true;
		public bool EnableDebug;
		// The list of assemblies that we do generate debugging info for.
		public bool DebugAll;
		public bool UseInterpreter; // Only applicable to mobile platforms.
		public List<string> DebugAssemblies = new List<string> ();
		public Optimizations Optimizations = new Optimizations ();
#if LEGACY_TOOLS
		public readonly RegistrarMode Registrar = RegistrarMode.Static;
#else
		public RegistrarMode Registrar = RegistrarMode.Default;
#endif
		public RegistrarOptions RegistrarOptions = RegistrarOptions.Default;
		public SymbolMode SymbolMode;
		public HashSet<string> IgnoredSymbols = new HashSet<string> ();
		public bool? PublishReadyToRun;
		public string PublishReadyToRunContainerFormat = "";

		// The AOT arguments are currently not used for macOS, but they could eventually be used there as well (there's no mmp option to set these yet).
		public List<string> AotArguments = new List<string> ();
		public List<string>? AotOtherArguments = null;
		public bool? AotFloat32 = null;
		public bool? TrimExportAttributes;
		public HashSet<ExportAttributeRemovalBlocker> TrimExportAttributesBlockers = new HashSet<ExportAttributeRemovalBlocker> ();

		// The set of UnmanagedCallersOnly trampoline symbols (without the leading Mach-O underscore)
		// that survived the NativeAOT compiler (ILC). This is only set when the native registrar code
		// is generated after ILC has run, so that we can avoid emitting direct native references to
		// trampolines ILC trimmed away (we route those through the dlsym fallback instead). A null value
		// means the information isn't available (e.g. we're not compiling for NativeAOT, or the registrar
		// runs before ILC), in which case every trampoline is assumed to have survived.
		public HashSet<string>? SurvivingTrampolineSymbols;

		// Returns true if the native registrar can emit a direct reference to the given UnmanagedCallersOnly
		// trampoline. When we know which trampolines survived ILC, a trampoline that didn't survive must not
		// be referenced directly (it would be an undefined symbol at native link time).
		public bool DidTrampolineSurviveIlc (string ucoEntryPoint)
		{
			if (SurvivingTrampolineSymbols is null)
				return true;
			return SurvivingTrampolineSymbols.Contains (ucoEntryPoint);
		}

		// The set of Objective-C class names whose inlined Class.GetHandle native function is still
		// referenced by the NativeAOT compiler's (ILC) output. These classes must be registered in the
		// native registrar code even if all their trampolines were trimmed away, because managed code
		// still looks up their class handle (and the generated native code references the Objective-C
		// class, which wouldn't exist otherwise). This is set alongside SurvivingTrampolineSymbols.
		// See docs/code/class-handles.md.
		public HashSet<string>? ClassesReferencedByInlinedClassGetHandle;

		// Returns true if managed code that survived ILC looks up the class handle for the given
		// Objective-C class name using the inlined Class.GetHandle optimization.
		public bool IsClassReferencedByInlinedClassGetHandle (string exportedName)
		{
			if (ClassesReferencedByInlinedClassGetHandle is null)
				return false;
			return ClassesReferencedByInlinedClassGetHandle.Contains (exportedName);
		}

#if ASSEMBLY_PREPARER
		public bool InCustomTrimmerStep = false;
		public bool IsPostProcessingAssemblies;
		// During post-processing, static registrars read metadata removed during trimming
		// from the original assemblies.
		// This resolver provides access to the original assemblies (a separate metadata universe from the
		// post-trim assemblies), and is null when not applicable.
		public Mono.Cecil.IAssemblyResolver? PreTrimAssemblyResolver;
#else
		public bool InCustomTrimmerStep = true;
		public bool IsPostProcessingAssemblies => InCustomTrimmerStep;
#endif

#if !LEGACY_TOOLS
		public DlsymOptions DlsymOptions;
		public List<Tuple<string, bool>>? DlsymAssemblies;
#endif // !LEGACY_TOOLS
		public List<string>? CustomLinkFlags;

		public HashSet<string> Frameworks = new HashSet<string> ();
		public HashSet<string> WeakFrameworks = new HashSet<string> ();

		public bool IsExtension;
		public TargetFramework TargetFramework { get; set; }
		public ApplePlatform Platform { get { return TargetFramework.Platform; } }

		public List<string> DylibsToConvertToFrameworks = new List<string> ();
		public List<string> MonoLibraries = new List<string> ();
		public List<string> InterpretedAssemblies = new List<string> ();

		public string TypeMapAssemblyName = "";
		public string TypeMapOutputDirectory = "";

		// Linker config
#if LEGACY_TOOLS
		public LinkMode LinkMode = LinkMode.Full;
#endif
		bool? are_any_assemblies_trimmed;
		public bool AreAnyAssembliesTrimmed {
			get {
				if (are_any_assemblies_trimmed.HasValue)
					return are_any_assemblies_trimmed.Value;
#if !LEGACY_TOOLS
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
		public bool EnableSGenConc;

		public bool IsAnyStaticRegistrar {
			get => Registrar == RegistrarMode.Static || Registrar == RegistrarMode.ManagedStatic || Registrar == RegistrarMode.TrimmableStatic;
		}

		public Dictionary<string, (string Value, bool Overwrite)> EnvironmentVariables = new Dictionary<string, (string Value, bool Overwrite)> ();

		public MarshalObjectiveCExceptionMode MarshalObjectiveCExceptions;
		public MarshalManagedExceptionMode MarshalManagedExceptions;

		bool is_default_marshal_managed_exception_mode;
		public bool IsDefaultMarshalManagedExceptionMode {
			get { return is_default_marshal_managed_exception_mode || MarshalManagedExceptions == MarshalManagedExceptionMode.Default; }
			set { is_default_marshal_managed_exception_mode = value; }
		}
		public List<string> RootAssemblies = new List<string> ();
		public List<string> References = new List<string> ();
		public string? RegistrarOutputLibrary;

		public BuildTarget BuildTarget;

		public XamarinRuntime XamarinRuntime;
		public string? RuntimeIdentifier; // Only used for build-time --run-registrar support

		public bool SkipMarkingNSObjectsInUserAssemblies { get; set; }

		// How Mono should be embedded into the app.
#if !LEGACY_TOOLS
		AssemblyBuildTarget? libmono_link_mode;
		public bool HasLibMonoLinkMode => libmono_link_mode.HasValue;
		public AssemblyBuildTarget LibMonoLinkMode {
			get {
				if (!libmono_link_mode.HasValue)
					throw new InvalidOperationException ("LibMonoLinkMode has not been set.");
				return libmono_link_mode.Value;
			}
			set {
				libmono_link_mode = value;
			}
		}

		// How libxamarin should be embedded into the app.
		AssemblyBuildTarget? libxamarin_link_mode;
		public bool HasLibXamarinLinkMode => libxamarin_link_mode.HasValue;
		public AssemblyBuildTarget LibXamarinLinkMode {
			get {
				if (!libxamarin_link_mode.HasValue)
					throw new InvalidOperationException ("LibXamarinLinkMode has not been set.");
				return libxamarin_link_mode.Value;
			}
			set {
				libxamarin_link_mode = value;
			}
		}

		// How the libmononative library should be linked into the app.
		public AssemblyBuildTarget LibMonoNativeLinkMode {
			get {
				// if there's a specific way libmono is being linked, use the same way.
				if (!libmono_link_mode.HasValue)
					throw new InvalidOperationException ("LibMonoNativeLinkMode has not been set.");
				return libmono_link_mode.Value;
			}
		}
#endif // !LEGACY_TOOLS

		bool RequiresXcodeHeaders {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.MacCatalyst:
					return !AreAnyAssembliesTrimmed;
				case ApplePlatform.MacOSX:
					return (Registrar == RegistrarMode.Static || Registrar == RegistrarMode.ManagedStatic || Registrar == RegistrarMode.TrimmableStatic) && !AreAnyAssembliesTrimmed;
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
				if (!StringUtils.IsNullOrEmpty (RuntimeIdentifier))
					return RuntimeIdentifier.IndexOf ("simulator", StringComparison.OrdinalIgnoreCase) >= 0;

				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
					return BuildTarget == BuildTarget.Simulator;
				case ApplePlatform.MacOSX:
				case ApplePlatform.MacCatalyst:
					return false;
				default:
					throw ErrorHelper.CreateError (71, Errors.MX0071, Platform, ProductName);
				}
			}
		}

		public Version? DeploymentTarget;
		public Version? SdkVersion; // for Mac Catalyst this is the iOS version
		public Version? NativeSdkVersion; // this is the same as SdkVersion, except that for Mac Catalyst it's the macOS SDK version.

		Abi abi;
		public bool IsLLVM { get { return IsArchEnabled (Abi.LLVM); } }

		bool? package_managed_debug_symbols;
		public bool PackageManagedDebugSymbols {
			get {
				if (!package_managed_debug_symbols.HasValue)
					throw new InvalidOperationException ("PackageManagedDebugSymbols has not been set.");
				return package_managed_debug_symbols.Value;
			}
			set { package_managed_debug_symbols = value; }
		}
		public bool GenerateTrustedPlatformAssemblies;
		public bool IsMultiRidBuild;
		public List<string> TrustedPlatformAssemblies = new List<string> ();

		public Version GetMacCatalystiOSVersion (Version macOSVersion)
		{
#if LEGACY_TOOLS
			if (macOSVersion.Major >= 26 && SdkRoot is null) {
				// this shouldn't happen for normal builds, nor for customers, so just show an internal 99 warning.
				ErrorHelper.Warning (this, 99, Errors.MX0099, $"No Xcode configured, assuming the macOS version {macOSVersion} is identical to the Mac Catalyst/iOS version.");
				return macOSVersion;
			}
#endif

			if (!MacCatalystSupport.TryGetiOSVersion (Driver.GetFrameworkDirectory (this), macOSVersion, out var value, out var knownMacOSVersions))
				throw ErrorHelper.CreateError (184, Errors.MX0184 /* Could not map the macOS version {0} to a corresponding Mac Catalyst version. Valid macOS versions are: {1} */, macOSVersion.ToString (), string.Join (", ", knownMacOSVersions.OrderBy (v => v)));

			return value;
		}

#if !LEGACY_TOOLS
		public Application (LinkerConfiguration configuration)
#else
		public Application ()
#endif
		{
#if !LEGACY_TOOLS
			this.configuration = configuration;
			this.LinkContext = new Tuner.DerivedLinkContext (configuration, this);
#endif
			this.StaticRegistrar = new StaticRegistrar (this);
			this.Resolver = new PlatformResolver (this);
			SetDefaultHiddenWarnings ();
		}

#if !LEGACY_TOOLS
		public void CreateCache (string [] arguments)
		{
			Cache = new Cache (arguments);
		}
#endif // !LEGACY_TOOLS

		public bool DynamicRegistrationSupported {
			get {
				return Optimizations.RemoveDynamicRegistrar != true;
			}
		}

#if !LEGACY_TOOLS
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

		public void UnsetInterpreter ()
		{
			UseInterpreter = false;
			InterpretedAssemblies.Clear ();
		}
#endif // !LEGACY_TOOLS

#if !LEGACY_TOOLS
		public bool IsTodayExtension {
			get {
				return ExtensionIdentifier == "com.apple.widget-extension";
			}
		}

		public bool IsTVExtension {
			get {
				return ExtensionIdentifier == "com.apple.tv-services";
			}
		}

		public string? ExtensionIdentifier {
			get {
				if (!IsExtension)
					return null;

				var plist = Driver.FromPList (InfoPListPath);
				if (plist is null)
					return null;
				var dict = plist.Get<PDictionary> ("NSExtension");
				if (dict is null)
					return null;
				return dict.GetString ("NSExtensionPointIdentifier");
			}
		}

		string? info_plistpath;
		public string InfoPListPath {
			get {
				if (info_plistpath is not null)
					return info_plistpath;

				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
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
#endif // !LEGACY_TOOLS

		// This is just a name for this app to show in log/error messages, etc.
		public string Name {
			get { return Path.GetFileNameWithoutExtension (AppDirectory); }
		}

#if !LEGACY_TOOLS
		bool? requires_pinvoke_wrappers;
		public bool RequiresPInvokeWrappers {
			get {
				if (requires_pinvoke_wrappers.HasValue)
					return requires_pinvoke_wrappers.Value;

				return false;
			}
			set {
				requires_pinvoke_wrappers = value;
			}
		}
#endif // !LEGACY_TOOLS

#if !LEGACY_TOOLS
		public bool RequireLinkWithAttributeForObjectiveCClassSearch;
#else
		public bool RequireLinkWithAttributeForObjectiveCClassSearch = true;
#endif

		public string PlatformName {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
					return "iOS";
				case ApplePlatform.TVOS:
					return "tvOS";
				case ApplePlatform.MacOSX:
					return "macOS";
				case ApplePlatform.MacCatalyst:
					return "MacCatalyst";
				default:
					throw new NotImplementedException ();
				}
			}
		}

		public static bool IsUptodate (IToolLog log, string source, string target, bool check_contents = false, bool check_stamp = true)
		{
			return FileCopier.IsUptodate (log, source, target, check_contents, check_stamp);
		}

		public static void RemoveResource (ModuleDefinition module, string name)
		{
			for (int i = 0; i < module.Resources.Count; i++) {
				var embedded = module.Resources [i] as EmbeddedResource;

				if (embedded is null || embedded.Name != name)
					continue;

				module.Resources.RemoveAt (i);
				break;
			}
		}

#if !LEGACY_TOOLS
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
				string dest_pdb = Path.ChangeExtension (destination, ".pdb");
				if (File.Exists (dest_pdb))
					File.Delete (dest_pdb);
			}
		}
#endif // !LEGACY_TOOLS

		public static bool ExtractResource (ModuleDefinition module, string name, string path, bool remove)
		{
			for (int i = 0; i < module.Resources.Count; i++) {
				var embedded = module.Resources [i] as EmbeddedResource;

				if (embedded is null || embedded.Name != name)
					continue;

				var dirname = Path.GetDirectoryName (path);
				if (!string.IsNullOrEmpty (dirname) && !Directory.Exists (dirname))
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

		// Checks if any of the source files have a time stamp later than any of the target files.
		//
		// If check_stamp is true, the function will use the timestamp of a "target".stamp file
		// if it's later than the timestamp of the "target" file itself.
		public static bool IsUptodate (IToolLog log, IEnumerable<string> sources, IEnumerable<string> targets, bool check_stamp = true)
		{
			return FileCopier.IsUptodate (log, sources, targets, check_stamp);
		}

		public static void UpdateDirectory (IToolLog log, string source, string target)
		{
			FileCopier.UpdateDirectory (log, source, target);
		}

		public void InitializeCommon ()
		{
			InitializeDeploymentTarget ();

			if (Platform == ApplePlatform.MacCatalyst) {
				// Our input SdkVersion is the macOS SDK version, but the rest of our code expects the supporting iOS version, so convert here.
				// The macOS SDK version is still stored in NativeSdkVersion for when we need it.
				if (NativeSdkVersion is null)
					throw ErrorHelper.CreateError (183, Errors.MX0183 /* NativeSdkVersion is required for Mac Catalyst builds. */);
				SdkVersion = GetMacCatalystiOSVersion (NativeSdkVersion);
			}

			if (RequiresXcodeHeaders && SdkVersion < SdkVersions.GetVersion (this)) {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.MacCatalyst:
					throw ErrorHelper.CreateError (180, Errors.MX0180, ProductName, PlatformName, SdkVersions.GetVersion (this), SdkVersions.Xcode);
				case ApplePlatform.MacOSX:
					throw ErrorHelper.CreateError (179, Errors.MX0179, ProductName, PlatformName, SdkVersions.GetVersion (this), SdkVersions.Xcode);
				default:
					// Default to the iOS error message, it's better than showing MX0071 (unknown platform), which would be completely unrelated
					goto case ApplePlatform.iOS;
				}
			}

			SetObjectiveCExceptionMode ();
			SetManagedExceptionMode ();

			if (SymbolMode == SymbolMode.Default) {
				SymbolMode = SymbolMode.Linker;
			}

			if (!package_managed_debug_symbols.HasValue) {
				package_managed_debug_symbols = EnableDebug;
			} else if (package_managed_debug_symbols.Value && IsLLVM) {
				ErrorHelper.Warning (this, 3007, Errors.MX3007);
			}

			Optimizations.Initialize (this, out var messages);
			ErrorHelper.Show (this, messages);
			if (this.Verbosity > 3)
				this.Log (4, $"Enabled optimizations: {Optimizations}");
		}

		void InitializeDeploymentTarget ()
		{
			if (DeploymentTarget is null)
				DeploymentTarget = SdkVersions.GetVersion (this);

			if (DeploymentTarget is not null) {
				if (DeploymentTarget < SdkVersions.GetMinVersion (this))
					throw new ProductException (73, true, Errors.MT0073, ProductConstants.Version, DeploymentTarget, Xamarin.SdkVersions.GetMinVersion (this), PlatformName, ProductName);
				if (DeploymentTarget > SdkVersions.GetVersion (this))
					throw new ProductException (74, true, Errors.MX0074, ProductConstants.Version, DeploymentTarget, Xamarin.SdkVersions.GetVersion (this), PlatformName, ProductName);
			}
		}

#if !ASSEMBLY_PREPARER
		public void RunRegistrar ()
		{
			// The static registrar.
			if (Registrar != RegistrarMode.Static)
				throw new ProductException (67, Errors.MT0067, Registrar); // this is only called during our own build

			if (RootAssemblies.Count < 1)
				throw ErrorHelper.CreateError (130, Errors.MX0130);

			var registrar_m = RegistrarOutputLibrary;
			if (registrar_m is null)
				throw ErrorHelper.CreateError (99, "RegistrarOutputLibrary must be specified.");
			var RootAssembly = RootAssemblies [0];
			var resolvedAssemblies = new Dictionary<string, AssemblyDefinition> ();
			var resolver = new PlatformResolver (this) {
				RootDirectory = Path.GetDirectoryName (RootAssembly),
			};
			resolver.Configure ();

			var ps = new ReaderParameters ();
			ps.AssemblyResolver = resolver;
			foreach (var reference in References) {
				var r = resolver.Load (this, reference);
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
						ErrorHelper.Warning (this, 7, Errors.MX0007, rootName);
						continue;
					}

					if (resolvedAssemblies.TryGetValue (rootName, out var previousAssembly)) {
						if (lastAssembly.MainModule.RuntimeVersion != previousAssembly.MainModule.RuntimeVersion) {
							this.Log (2, "Attemping to load an assembly another time {0} (previous {1})", lastAssembly.FullName, previousAssembly.FullName);
						}
						continue;
					}

					resolvedAssemblies.Add (rootName, lastAssembly);
					this.Log (3, "Loaded {0}", lastAssembly.MainModule.FileName);
				} catch (Exception ex) {
					ErrorHelper.Warning (this, 9, ex, Errors.MX0009, $"{rootName}: {ex.Message}");
					continue;
				}
			}

			if (!foundProductAssembly)
				throw ErrorHelper.CreateError (131, Errors.MX0131, productAssembly, string.Join ("', '", RootAssemblies.ToArray ()));

			var registrar = new Registrar.StaticRegistrar (this);
			if (RootAssemblies.Count == 1) {
				registrar.GenerateSingleAssembly (resolver, resolvedAssemblies.Values, Path.ChangeExtension (registrar_m, "h"), registrar_m, Path.GetFileNameWithoutExtension (RootAssembly), out var _);
			} else {
				registrar.Generate (resolver, resolvedAssemblies.Values, Path.ChangeExtension (registrar_m, "h"), registrar_m, out var _);
			}
		}
#endif // !ASSEMBLY_PREPARER

		public Abi Abi {
			get { return abi; }
			set { abi = value; }
		}

		public bool IsArchEnabled (Abi arch)
		{
			return IsArchEnabled (abi, arch);
		}

		public static bool IsArchEnabled (Abi abi, Abi arch)
		{
			return (abi & arch) != 0;
		}

#if !LEGACY_TOOLS
		public void ValidateAbi ()
		{
			var validAbis = new List<Abi> ();
			switch (Platform) {
			case ApplePlatform.iOS:
				if (IsDeviceBuild) {
					validAbis.Add (Abi.ARM64);
					validAbis.Add (Abi.ARM64 | Abi.LLVM);
				} else {
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

			if (!validAbis.Contains (abi))
				throw ErrorHelper.CreateError (75, Errors.MT0075, abi, Platform, string.Join (", ", validAbis.Select ((v) => v.AsString ()).ToArray ()));
		}

		public void ClearAbi ()
		{
			abi = default;
		}
#endif // !LEGACY_TOOLS

		public void ParseAbi (string abi)
		{
			Abi value;
			switch (abi) {
			case "x86_64":
				value = Abi.x86_64;
				break;
			case "arm64":
				value = Abi.ARM64;
				break;
			case "arm64+llvm":
				value = Abi.ARM64 | Abi.LLVM;
				break;
			default:
				throw ErrorHelper.CreateError (15, Errors.MT0015, abi);
			}

			this.abi = value;
		}

#if !LEGACY_TOOLS
		public void ParseRegistrar (string? v)
		{
			if (StringUtils.IsNullOrEmpty (v))
				return;

			var split = v.Split ('=');
			var name = split [0];
			var value = split.Length > 1 ? split [1] : string.Empty;
			switch (name.ToLowerInvariant ()) {
			case "static":
				Registrar = RegistrarMode.Static;
				break;
			case "dynamic":
				Registrar = RegistrarMode.Dynamic;
				break;
			case "default":
				Registrar = RegistrarMode.Default;
				break;
			case "partial":
			case "partial-static":
			case "partialstatic":
				Registrar = RegistrarMode.PartialStatic;
				break;
			case "managed-static":
			case "managedstatic":
				Registrar = RegistrarMode.ManagedStatic;
				break;
			case "trimmable-static":
			case "trimmablestatic":
				Registrar = RegistrarMode.TrimmableStatic;
				break;
			default:
				throw ErrorHelper.CreateError (20, Errors.MX0020, "--registrar", "managed-static, trimmable-static, static, dynamic or default");
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
#endif // !LEGACY_TOOLS

#if !LEGACY_TOOLS
		public string MonoGCParams {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
					// Configure sgen to use a small nursery
					string ret = "nursery-size=512k";
					if (IsTodayExtension) {
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
#endif // !LEGACY_TOOLS

		public bool IsFrameworkUnavailable (string @namespace)
		{
			if (!Driver.GetFrameworks (this).TryGetValue (@namespace, out var fw))
				return false; // Unknown framework, assume it's valid

			return fw.IsFrameworkUnavailable (this);
		}

#if !LEGACY_TOOLS
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
#endif // !LEGACY_TOOLS

		public void SetManagedExceptionMode ()
		{
			switch (MarshalManagedExceptions) {
			case MarshalManagedExceptionMode.Default:
				MarshalManagedExceptions = MarshalManagedExceptionMode.ThrowObjectiveCException;
				IsDefaultMarshalManagedExceptionMode = true;
				break;
			case MarshalManagedExceptionMode.UnwindNativeCode:
			case MarshalManagedExceptionMode.Disable:
				if (XamarinRuntime == XamarinRuntime.CoreCLR)
					throw ErrorHelper.CreateError (185, Errors.MX0185 /* The option '{0}' cannot take the value '{1}' when using CoreCLR. */, "--marshal-managed-exceptions", MarshalManagedExceptions.ToString ().ToLowerInvariant ());
				break;
			}
		}

		public void SetObjectiveCExceptionMode ()
		{
			switch (MarshalObjectiveCExceptions) {
			case MarshalObjectiveCExceptionMode.Default:
				MarshalObjectiveCExceptions = MarshalObjectiveCExceptionMode.ThrowManagedException;
				break;
			case MarshalObjectiveCExceptionMode.UnwindManagedCode:
			case MarshalObjectiveCExceptionMode.Disable:
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

#if LEGACY_TOOLS
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

		// Returns whether an assembly is Mono AOT-compiled.
		public bool IsAOTCompiled (string assembly)
		{
			// We're never AOT compiled if we're using CoreCLR
			if (XamarinRuntime == XamarinRuntime.CoreCLR)
				return false;

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

#if !LEGACY_TOOLS
		public IList<string> GetAotArguments (string filename, Abi abi, string outputDir, string outputFile, string llvmOutputFile, string dataFile)
		{
			GetAotArguments (filename, abi, outputDir, outputFile, llvmOutputFile, dataFile, null, out var processArguments, out var aotArguments);
			processArguments.Add (string.Join (",", aotArguments));
			processArguments.Add (filename);
			return processArguments;
		}

		public void GetAotArguments (string filename, Abi abi, string outputDir, string outputFile, string llvmOutputFile, string dataFile, bool? isDedupAssembly, out List<string> processArguments, out List<string> aotArguments, string? llvm_path = null)
		{
			string fname = Path.GetFileName (filename);
			processArguments = new List<string> ();
			var app = this;
			bool enable_llvm = (abi & Abi.LLVM) != 0;
			bool enable_thumb = (abi & Abi.Thumb) != 0;
			bool enable_debug = app.EnableDebug;
			bool enable_debug_symbols = app.PackageManagedDebugSymbols;
			bool interp = app.IsInterpreted (Assembly.GetIdentity (filename)) && !(isDedupAssembly.HasValue && isDedupAssembly.Value);
			bool interp_full = !interp && app.UseInterpreter;
			string arch = abi.AsArchString ();

			processArguments.Add ("--debug");

			if (enable_llvm)
				processArguments.Add ("--llvm");

			if (!interp)
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
			if (app.LibMonoLinkMode == AssemblyBuildTarget.StaticObject)
				aotArguments.Add ("direct-icalls");
			aotArguments.AddRange (app.AotArguments);
			if (interp) {
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

			if (enable_llvm) {
				if (!string.IsNullOrEmpty (llvm_path)) {
					aotArguments.Add ($"llvm-path={llvm_path}");
				} else {
					aotArguments.Add ($"llvm-path={app.FrameworkCurrentDirectory}/LLVM/bin/");
				}
			}

			aotArguments.Add ($"outfile={outputFile}");
			if (enable_llvm)
				aotArguments.Add ($"llvm-outfile={llvmOutputFile}");

			// If the interpreter is enabled, and we're building for x86_64, we're AOT-compiling but we
			// don't have access to infinite trampolines. So we're bumping the trampoline count (unless
			// the developer has already set a value) to something higher than the default.
			//
			// Ref:
			// * https://github.com/dotnet/macios/issues/14887
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
		}
#endif // !LEGACY_TOOLS

		public string AssemblyName {
			get {
				return Path.GetFileName (RootAssemblies [0]);
			}
		}

		internal ProductConstants ProductConstants {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
					return ProductConstants.iOS;
				case ApplePlatform.MacCatalyst:
					return ProductConstants.MacCatalyst;
				case ApplePlatform.TVOS:
					return ProductConstants.tvOS;
				case ApplePlatform.MacOSX:
					return ProductConstants.macOS;
				default:
					throw ErrorHelper.CreateError (71, Errors.MX0071, Platform, ProductName);
				}
			}
		}

#if !LEGACY_TOOLS
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
				return false;
			case ApplePlatform.MacCatalyst:
				// https://github.com/dotnet/macios/issues/14437
				return true;
			default:
				throw ErrorHelper.CreateError (71, Errors.MX0071, Platform, ProductName);
			}
		}
#endif // !LEGACY_TOOLS

#if !LEGACY_TOOLS
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
				this.Log (1, "The framework {0} is a framework of static libraries, and will not be copied to the app.", framework_path);

			return dynamic;
		}
#endif // !LEGACY_TOOLS

		public void SetDefaultHiddenWarnings ()
		{
			// People don't like these warnings (#20670), and they also complicate our tests, so ignore them.
			ErrorHelper.ParseWarningLevel (this, ErrorHelper.WarningLevel.Disable, "4178"); // The class '{0}' will not be registered because the {1} framework has been removed from the {2} SDK.
			ErrorHelper.ParseWarningLevel (this, ErrorHelper.WarningLevel.Disable, "4189"); // The class '{0}' will not be registered because it has been removed from the {1} SDK.
			ErrorHelper.ParseWarningLevel (this, ErrorHelper.WarningLevel.Disable, "4190"); // The class '{0}' will not be registered because the {1} framework has been deprecated from the {2} SDK.
		}

		IToolLog GetLog ()
		{
#if LEGACY_TOOLS
			return ConsoleLog.Instance;
#else
			return Configuration.Logger ?? ConsoleLog.Instance;
#endif
		}

		public void Log (string message)
		{
			GetLog ().Log (message);
		}

		public void LogError (string message)
		{
			GetLog ().LogError (message);
		}

		public void LogError (ProductException exception)
		{
			GetLog ().LogError (exception);
		}

		public void LogWarning (ProductException exception)
		{
			GetLog ().LogWarning (exception);
		}

		public void LogException (Exception exception)
		{
			GetLog ().LogException (exception);
		}

		int verbosity = Driver.GetDefaultVerbosity (Driver.NAME);
		public int Verbosity {
			get => verbosity;
			set => verbosity = value;
		}

		public string? SdkRoot { get; set; }
		public string? DeveloperDirectory { get; set; }

		string? framework_dir;
		public string FrameworkCurrentDirectory {
			get {
				if (framework_dir is null)
					throw new InvalidOperationException ($"Teh current framework directory hasn't been set.");
				return framework_dir;
			}
			set {
				framework_dir = value;
			}
		}

		/// <summary>
		/// This returns the /Applications/Xcode*.app/Contents/Developer/Platforms directory
		/// </summary>
		public string PlatformsDirectory {
			get {
				if (DeveloperDirectory is null)
					throw new InvalidOperationException ("DeveloperDirectory is not set");
				return Path.Combine (DeveloperDirectory, "Platforms");
			}
		}

		Version? xcode_version;
		public Version XcodeVersion {
			get {
				if (xcode_version is null)
					throw ErrorHelper.CreateError (99, Errors.MX0099, "The Xcode version has not been configured. Pass --xcode-version or configure an Xcode installation.");
				return xcode_version;
			}
			set {
				xcode_version = value;
			}
		}

		public string? XcodeProductVersion { get; set; }
	}
}
