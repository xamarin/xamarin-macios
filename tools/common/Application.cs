using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Xamarin.Linker;
using Xamarin.Utils;

using ObjCRuntime;

#if MONOTOUCH
using PlatformException = Xamarin.Bundler.MonoTouchException;
using PlatformResolver = MonoTouch.Tuner.MonoTouchResolver;
#else
using PlatformException = Xamarin.Bundler.MonoMacException;
using PlatformResolver = Xamarin.Bundler.MonoMacResolver;
#endif

namespace Xamarin.Bundler {

	[Flags]
	public enum RegistrarOptions {
		Default = 0,
		Trace = 1,
	}

	public enum LinkMode {
		None,
		SDKOnly,
		All,
#if !MONOTOUCH
		Platform,
#endif
	}

	public partial class Application
	{
		public Cache Cache;
		public string AppDirectory = ".";
		public bool DeadStrip = true;
		public bool EnableDebug;
		internal RuntimeOptions RuntimeOptions;
		public Optimizations Optimizations = new Optimizations ();
		public RegistrarMode Registrar = RegistrarMode.Default;
		public RegistrarOptions RegistrarOptions = RegistrarOptions.Default;
		public SymbolMode SymbolMode;
		public HashSet<string> IgnoredSymbols = new HashSet<string> ();

		public HashSet<string> Frameworks = new HashSet<string> ();
		public HashSet<string> WeakFrameworks = new HashSet<string> ();

		public ApplePlatform Platform { get { return Driver.TargetFramework.Platform; } }

		// Linker config
		public LinkMode LinkMode = LinkMode.All;
		public List<string> LinkSkipped = new List<string> ();
		public List<string> Definitions = new List<string> ();
		public Mono.Linker.I18nAssemblies I18n;

		public bool? EnableCoopGC;
		public bool EnableSGenConc;
		public MarshalObjectiveCExceptionMode MarshalObjectiveCExceptions;
		public MarshalManagedExceptionMode MarshalManagedExceptions;

		bool is_default_marshal_managed_exception_mode;
		public bool IsDefaultMarshalManagedExceptionMode {
			get { return is_default_marshal_managed_exception_mode || MarshalManagedExceptions == MarshalManagedExceptionMode.Default; }
			set { is_default_marshal_managed_exception_mode = value; }
		}
		public List<string> RootAssemblies = new List<string> ();
		public List<Application> SharedCodeApps = new List<Application> (); // List of appexes we're sharing code with.
		public string RegistrarOutputLibrary;

		public static int Concurrency => Driver.Concurrency;
		public Version DeploymentTarget;
		public Version SdkVersion;
	
		public bool Embeddinator { get; set; }

		public Application (string[] arguments)
		{
			Cache = new Cache (arguments);
		}

		public bool DynamicRegistrationSupported {
			get {
				return Optimizations.RemoveDynamicRegistrar != true;
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
#else
				if (Driver.Is64Bit)
					return false;
#endif
				return MarshalObjectiveCExceptions == MarshalObjectiveCExceptionMode.ThrowManagedException || MarshalObjectiveCExceptions == MarshalObjectiveCExceptionMode.Abort;
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

			var wp = new WriterParameters () { WriteSymbols = symbols };
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
				ErrorHelper.Error (1013, "Dependency tracking error: no files to compare. Please file a bug report at https://github.com/xamarin/xamarin-macios/issues/new with a test case.");

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
			Namespaces.Initialize ();
			SelectRegistrar ();

			if (RequiresXcodeHeaders && SdkVersion < SdkVersions.GetVersion (Platform)) {
				throw ErrorHelper.CreateError (91, "This version of {0} requires the {1} {2} SDK (shipped with Xcode {3}). Either upgrade Xcode to get the required header files or {4} (to try to avoid the new APIs).", ProductName, PlatformName, SdkVersions.GetVersion (Platform), SdkVersions.Xcode, Error91LinkerSuggestion);
			}

			if (DeploymentTarget != null) {
				if (DeploymentTarget < Xamarin.SdkVersions.GetMinVersion (Platform))
					throw new PlatformException (73, true, "{4} {0} does not support a deployment target of {1} for {3} (the minimum is {2}). Please select a newer deployment target in your project's Info.plist.", Constants.Version, DeploymentTarget, Xamarin.SdkVersions.GetMinVersion (Platform), PlatformName, ProductName);
				if (DeploymentTarget > Xamarin.SdkVersions.GetVersion (Platform))
					throw new PlatformException (74, true, "{4} {0} does not support a deployment target of {1} for {3} (the maximum is {2}). Please select an older deployment target in your project's Info.plist or upgrade to a newer version of {4}.", Constants.Version, DeploymentTarget, Xamarin.SdkVersions.GetVersion (Platform), PlatformName, ProductName);
			}

			if (Platform == ApplePlatform.WatchOS && EnableCoopGC.HasValue && !EnableCoopGC.Value)
				throw ErrorHelper.CreateError (88, "The GC must be in cooperative mode for watchOS apps. Please remove the --coop:false argument to mtouch.");

			if (!EnableCoopGC.HasValue)
				EnableCoopGC = Platform == ApplePlatform.WatchOS;

			if (EnableCoopGC.Value) {
				switch (MarshalObjectiveCExceptions) {
				case MarshalObjectiveCExceptionMode.UnwindManagedCode:
				case MarshalObjectiveCExceptionMode.Disable:
					throw ErrorHelper.CreateError (89, "The option '{0}' cannot take the value '{1}' when cooperative mode is enabled for the GC.", "--marshal-objectivec-exceptions", MarshalObjectiveCExceptions.ToString ().ToLowerInvariant ());
				}
				switch (MarshalManagedExceptions) {
				case MarshalManagedExceptionMode.UnwindNativeCode:
				case MarshalManagedExceptionMode.Disable:
					throw ErrorHelper.CreateError (89, "The option '{0}' cannot take the value '{1}' when cooperative mode is enabled for the GC.", "--marshal-managed-exceptions", MarshalManagedExceptions.ToString ().ToLowerInvariant ());
				}
			}


			bool isSimulatorOrDesktopDebug = EnableDebug;
#if MTOUCH
			isSimulatorOrDesktopDebug &= IsSimulatorBuild;
#endif

			if (MarshalObjectiveCExceptions == MarshalObjectiveCExceptionMode.Default) {
				if (EnableCoopGC.Value) {
					MarshalObjectiveCExceptions = MarshalObjectiveCExceptionMode.ThrowManagedException;
				} else {
					MarshalObjectiveCExceptions = isSimulatorOrDesktopDebug ? MarshalObjectiveCExceptionMode.UnwindManagedCode : MarshalObjectiveCExceptionMode.Disable;
				}
			}

			if (MarshalManagedExceptions == MarshalManagedExceptionMode.Default) {
				if (EnableCoopGC.Value) {
					MarshalManagedExceptions = MarshalManagedExceptionMode.ThrowObjectiveCException;
				} else {
					MarshalManagedExceptions = isSimulatorOrDesktopDebug ? MarshalManagedExceptionMode.UnwindNativeCode : MarshalManagedExceptionMode.Disable;
				}
				IsDefaultMarshalManagedExceptionMode = true;
			}

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
				ErrorHelper.Warning (115, "It is recommended to reference dynamic symbols using code (--dynamic-symbol-mode=code) when bitcode is enabled.");
			}
#endif

			Optimizations.Initialize (this);
		}

		public void RunRegistrar ()
		{
			// The static registrar.
			if (Registrar != RegistrarMode.Static)
				throw new PlatformException (67, "Invalid registrar: {0}", Registrar); // this is only called during our own build

			if (RootAssemblies.Count < 1)
				throw ErrorHelper.CreateError (130, "No root assemblies found. You should provide at least one root assembly.");

			var registrar_m = RegistrarOutputLibrary;
			var RootAssembly = RootAssemblies [0];
			var resolvedAssemblies = new Dictionary<string, AssemblyDefinition> ();
			var resolver = new PlatformResolver () {
				FrameworkDirectory = Driver.GetPlatformFrameworkDirectory (this),
				RootDirectory = Path.GetDirectoryName (RootAssembly),
#if MMP
				CommandLineAssemblies = RootAssemblies,
#endif
			};

			if (Platform == ApplePlatform.iOS || Platform == ApplePlatform.MacOSX) {
				if (Is32Build) {
					resolver.ArchDirectory = Driver.GetArch32Directory (this);
				} else {
					resolver.ArchDirectory = Driver.GetArch64Directory (this);
				}
			}

			var ps = new ReaderParameters ();
			ps.AssemblyResolver = resolver;
			resolvedAssemblies.Add ("mscorlib", ps.AssemblyResolver.Resolve (AssemblyNameReference.Parse ("mscorlib"), new ReaderParameters ()));

			var productAssembly = Driver.GetProductAssembly (this);
			bool foundProductAssembly = false;
			foreach (var asm in RootAssemblies) {
				var rootName = Path.GetFileNameWithoutExtension (asm);
				if (rootName == productAssembly)
					foundProductAssembly = true;
				
				try {
					AssemblyDefinition lastAssembly = ps.AssemblyResolver.Resolve (AssemblyNameReference.Parse (rootName), new ReaderParameters ());
					if (lastAssembly == null) {
						ErrorHelper.CreateWarning (7, "The root assembly '{0}' does not exist", rootName);
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
					ErrorHelper.Warning (9, ex, "Error while loading assemblies: {0}: {1}", rootName, ex.Message);
					continue;
				}
			}

			if (!foundProductAssembly)
				throw ErrorHelper.CreateError (131, "Product assembly '{0}' not found in assembly list: '{1}'", productAssembly, string.Join ("', '", RootAssemblies.ToArray ()));

#if MONOTOUCH
			BuildTarget = BuildTarget.Simulator;
#endif
			var registrar = new Registrar.StaticRegistrar (this);
			if (RootAssemblies.Count == 1)
				registrar.GenerateSingleAssembly (resolvedAssemblies.Values, Path.ChangeExtension (registrar_m, "h"), registrar_m, Path.GetFileNameWithoutExtension (RootAssembly));
			else
				registrar.Generate (resolvedAssemblies.Values, Path.ChangeExtension (registrar_m, "h"), registrar_m);
		}
	}
}
