//
// mtouch.cs: A tool to generate the necessary code to boot a Mono
// application on the iPhone
//
// It has a couple of modes of operation:
//
// COMPILATION:
//
//   Default
//
//        Compiles the assemblies specified and produces Assembly
//        language code and C files that can be incorporated into an
//        existing project.
//
//   -sim Compile to Simulator image
//
//        This compiles the given assemblies into the specified
//        directory.   The target directory can then be used as
//        the contents of the .app
//
//   -dev Compile the Device image
//
//        This compiles the given assemblies into the specified
//        directory.   The target directory can then be used as
//        the contents of the .app
//
// LAUNCHING:
//   -launchsim=APP
//
//        This launches the specified File.App in the simulator
//
//   -debugsim=AP
//
//        Launches the application in debug mode on the simulator
//
//   -launchdev=ID
//
//        Launches the specified app bundle ID on the connected device
//
// Copyright 2009, Novell, Inc.
// Copyright 2011-2013 Xamarin Inc. All rights reserved.
//
// Authors:
//   Miguel de Icaza
//   Geoff Norton
//   Jb Evain
//   Sebastien Pouliot
//

using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using Mono.Options;
using Mono.Cecil;
using Mono.Cecil.Mdb;
using Mono.Tuner;

using MonoTouch.Tuner;
using XamCore.Registrar;
using XamCore.ObjCRuntime;

using Xamarin.Linker;
using Xamarin.Utils;

using Xamarin.MacDev;

public enum OutputFormat {
	Default,
	Xml,
}

namespace Xamarin.Bundler
{
	partial class Driver {
		public static void ShowHelp (OptionSet os)
		{
			Console.WriteLine ("mtouch - Mono Compiler for iOS");
			Console.WriteLine ("Copyright 2009-2011, Novell, Inc.");
			Console.WriteLine ("Copyright 2011-2016, Xamarin Inc.");
			Console.WriteLine ("Usage is: mtouch [options]");

			os.WriteOptionDescriptions (Console.Out);
		}

		enum Action {
			None,
			Help,
			Version,
			Build,
			InstallSim,
			LaunchSim,
			DebugSim,
			InstallDevice,
			LaunchDevice,
			DebugDevice,
			KillApp,
			KillAndLaunch,
			ListDevices,
			ListSimulators,
			IsAppInstalled,
			ListApps,
			LogDev,
			RunRegistrar,
			ListCrashReports,
			DownloadCrashReport,
			KillWatchApp,
			LaunchWatchApp,
		}

		static Application app = new Application ();
		static Action action;

		// The list of assemblies that we do generate debugging info for.
		static bool debug_all = false;
		static List<string> debug_assemblies = new List<string> ();

		//
		// iPhone Developer platform
		static string framework_dir;
		static Version sdk_version = new Version ();
		static string compiler = string.Empty;
		static string compiler_path;
		public static bool enable_generic_nsobject = false;
		static bool xcode_version_check = true;

		//
		// Output generation
		static bool fast_sim = true;
		static bool force = false;
		static bool? llvm_asmwriter;
		static string cross_prefix = Environment.GetEnvironmentVariable ("MONO_CROSS_PREFIX");
		static string extra_args = Environment.GetEnvironmentVariable ("MTOUCH_ENV_OPTIONS");

		//
		// Where we output the generated code (source or compiled, depending on mode)
		//
		static string output_dir = ".";
		static string aot_args = "static,asmonly,direct-icalls,";
		static string aot_other_args = "";
		static int verbose = GetDefaultVerbosity ();
		static bool dry_run = false;
		static bool? debug_track;
		static Dictionary<string, string> environment_variables = new Dictionary<string, string> ();

		public static List<string> classic_only_arguments = new List<string> (); // list of deprecated arguments, which (if used) will cause errors in Unified profile)

		//
		// We need to put a hard dep on Mono.Cecil.Mdb.dll so that it get's mkbundled
		//
#pragma warning disable 169
		static MdbReader mdb_reader;
#pragma warning restore 169

		static int GetDefaultVerbosity ()
		{
			var v = 0;
			var fn = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), ".mtouch-verbosity");
			if (File.Exists (fn)) {
				v = (int) new FileInfo (fn).Length;
				if (v == 0)
					v = 4; // this is the magic verbosity level we give everybody.
			}
			return v;
		}

		static string mtouch_dir;

		public static void Log (string format, params object [] args)
		{
			Log (0, format, args);
		}

		public static void Log (int min_verbosity, string format, params object [] args)
		{
			if (min_verbosity > verbose)
				return;

			Console.WriteLine (format, args);
		}

		public static bool EnableDebug {
			get {
				return app.EnableDebug;
			}
		}

		public static bool IsUnified {
			get { return app.IsUnified; }
		}

		public static int Verbosity {
			get { return verbose; }
		}

		public static bool DryRun {
			get { return dry_run; }
		}

		public static bool Force {
			get { return force; }
			set { force = value; }
		}

		static string Platform {
			get {
				switch (app.Platform) {
				case ApplePlatform.iOS:
					return app.IsDeviceBuild ? "iPhoneOS" : "iPhoneSimulator";
				case ApplePlatform.WatchOS:
					return app.IsDeviceBuild ? "WatchOS" : "WatchSimulator";
				case ApplePlatform.TVOS:
					return app.IsDeviceBuild ? "AppleTVOS" : "AppleTVSimulator";
				default:
					throw ErrorHelper.CreateError (71, "Unknown platform: {0}. This usually indicates a bug in Xamarin.iOS; please file a bug report at http://bugzilla.xamarin.com with a test case.", app.Platform);
				}
			}
		}

		public static Application App {
			get { return app; }
		}

		public static string MonoTouchLibDirectory {
			get {
				return Path.Combine (ProductSdkDirectory, "usr", "lib");
			}
		}

		public static string DriverBinDirectory {
			get {
				return MonoTouchBinDirectory;
			}
		}

		public static string MonoTouchBinDirectory {
			get {
				return Path.Combine (MonoTouchDirectory, "bin");
			}
		}

		public static string MonoTouchDirectory {
			get {
				if (mtouch_dir == null) {
					mtouch_dir = Path.GetFullPath (GetFullPath () + "/../../..");
#if DEV
					// when launched from Xamarin Studio, mtouch is not in the final install location,
					// so walk the directory hierarchy to find the root source directory.
					while (!File.Exists (Path.Combine (mtouch_dir, "Make.config")))
						mtouch_dir = Path.GetDirectoryName (mtouch_dir);
					mtouch_dir = Path.Combine (mtouch_dir, "_ios-build", "Library", "Frameworks", "Xamarin.iOS.framework", "Versions", "Current");
#endif
					mtouch_dir = Target.GetRealPath (mtouch_dir);
				}
				return mtouch_dir;
			}
		}

		public static string PlatformFrameworkDirectory {
			get {
				switch (app.Platform) {
				case ApplePlatform.iOS:
					return Path.Combine (MonoTouchDirectory, "lib", "mono", app.IsUnified ? "Xamarin.iOS" : "2.1");
				case ApplePlatform.WatchOS:
					return Path.Combine (MonoTouchDirectory, "lib", "mono", "Xamarin.WatchOS");
				case ApplePlatform.TVOS:
					return Path.Combine (MonoTouchDirectory, "lib", "mono", "Xamarin.TVOS");
				default:
					throw ErrorHelper.CreateError (71, "Unknown platform: {0}. This usually indicates a bug in Xamarin.iOS; please file a bug report at http://bugzilla.xamarin.com with a test case.", app.Platform);
				}
			}
		}

		public static string ProductSdkDirectory {
			get {
				switch (app.Platform) {
				case ApplePlatform.iOS:
					return Path.Combine (MonoTouchDirectory, "SDKs", app.IsDeviceBuild ? "MonoTouch.iphoneos.sdk" : "MonoTouch.iphonesimulator.sdk");
				case ApplePlatform.WatchOS:
					return Path.Combine (MonoTouchDirectory, "SDKs", app.IsDeviceBuild ? "Xamarin.WatchOS.sdk" : "Xamarin.WatchSimulator.sdk");
				case ApplePlatform.TVOS:
					return Path.Combine (MonoTouchDirectory, "SDKs", app.IsDeviceBuild ? "Xamarin.AppleTVOS.sdk" : "Xamarin.AppleTVSimulator.sdk");
				default:
					throw ErrorHelper.CreateError (71, "Unknown platform: {0}. This usually indicates a bug in Xamarin.iOS; please file a bug report at http://bugzilla.xamarin.com with a test case.", app.Platform);
				}
			}
		}

		public static string ProductFrameworksDirectory {
			get {
				return Path.Combine (Driver.ProductSdkDirectory, "Frameworks");
			}
		}

		// This is for the -mX-version-min=A.B compiler flag
		public static string TargetMinSdkName {
			get {
				switch (app.Platform) {
				case ApplePlatform.iOS:
					return app.IsDeviceBuild ? "iphoneos" : "ios-simulator";
				case ApplePlatform.WatchOS:
					return app.IsDeviceBuild ? "watchos" : "watchos-simulator";
				case ApplePlatform.TVOS:
					return app.IsDeviceBuild ? "tvos" : "tvos-simulator";
				default:
					throw ErrorHelper.CreateError (71, "Unknown platform: {0}. This usually indicates a bug in Xamarin.iOS; please file a bug report at http://bugzilla.xamarin.com with a test case.", app.Platform);
				}
			}
		}

		public static string ProductAssembly {
			get {
				switch (app.Platform) {
				case ApplePlatform.iOS:
					return app.IsUnified ? "Xamarin.iOS" : "monotouch";
				case ApplePlatform.WatchOS:
					return "Xamarin.WatchOS";
				case ApplePlatform.TVOS:
					return "Xamarin.TVOS";
				default:
					throw ErrorHelper.CreateError (71, "Unknown platform: {0}. This usually indicates a bug in Xamarin.iOS; please file a bug report at http://bugzilla.xamarin.com with a test case.", app.Platform);
				}
			}
		}

		public static bool LLVMAsmWriter {
			get {
				if (llvm_asmwriter.HasValue)
					return llvm_asmwriter.Value;
				switch (app.Platform) {
				case ApplePlatform.iOS:
					return false;
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
					return true;
				default:
					throw ErrorHelper.CreateError (71, "Unknown platform: {0}. This usually indicates a bug in Xamarin.iOS; please file a bug report at http://bugzilla.xamarin.com with a test case.", app.Platform);
				}
			}
		}

		static string sdk_root;
		static string developer_directory;

		const string XcodeDefault = "/Applications/Xcode.app";

		static string FindSystemXcode ()
		{
			var output = new StringBuilder ();
			if (Driver.RunCommand ("xcode-select", "-p", output: output) != 0) {
				ErrorHelper.Warning (59, "Could not find the currently selected Xcode on the system: {0}", output.ToString ());
				return null;
			}
			return output.ToString ().Trim ();
		}

		static void ValidateXcode ()
		{
			// Allow a few actions, since these seem to always work no matter the Xcode version.
			var accept_any_xcode_version = action == Action.ListDevices || action == Action.ListCrashReports || action == Action.ListApps || action == Action.LogDev;

			if (sdk_root == null) {
				sdk_root = FindSystemXcode ();
				if (sdk_root == null) {
					// FindSystemXcode showed a warning in this case. In particular do not use 'string.IsNullOrEmpty' here,
					// because FindSystemXcode may return an empty string (with no warning printed) if the xcode-select command
					// succeeds, but returns nothing.
					sdk_root = null;
				} else if (!Directory.Exists (sdk_root)) {
					ErrorHelper.Warning (60, "Could not find the currently selected Xcode on the system. 'xcode-select --print-path' returned '{0}', but that directory does not exist.", sdk_root);
					sdk_root = null;
				} else {
					if (!accept_any_xcode_version)
						ErrorHelper.Warning (61, "No Xcode.app specified (using --sdkroot), using the system Xcode as reported by 'xcode-select --print-path': {0}", sdk_root);
				}
				if (sdk_root == null) {
					sdk_root = XcodeDefault;
					if (!Directory.Exists (sdk_root))
						throw ErrorHelper.CreateError (56, "Cannot find Xcode in the default location (/Applications/Xcode.app). Please install Xcode, or pass a custom path using --sdkroot <path>.");
					ErrorHelper.Warning (62, "No Xcode.app specified (using --sdkroot or 'xcode-select --print-path'), using the default Xcode instead: {0}", sdk_root);
				}
			} else if (!Directory.Exists (sdk_root)) {
				throw ErrorHelper.CreateError (55, "The Xcode path '{0}' does not exist.", sdk_root);
			}

			// Check what kind of path we got
			if (File.Exists (Path.Combine (sdk_root, "Contents", "MacOS", "Xcode"))) {
				// path to the Xcode.app
				developer_directory = Path.Combine (sdk_root, "Contents", "Developer");
			} else if (File.Exists (Path.Combine (sdk_root, "..", "MacOS", "Xcode"))) {
				// path to Contents/Developer
				developer_directory = Path.GetFullPath (Path.Combine (sdk_root, "..", "..", "Contents", "Developer"));
			} else {
				throw ErrorHelper.CreateError (57, "Cannot determine the path to Xcode.app from the sdk root '{0}'. Please specify the full path to the Xcode.app bundle.", sdk_root);
			}

			var plist_path = Path.Combine (Path.GetDirectoryName (DeveloperDirectory), "version.plist");

			if (File.Exists (plist_path)) {
				var plist = FromPList (plist_path);
				var version = plist.GetString ("CFBundleShortVersionString");
				xcode_version = new Version (version);
			} else {
				throw ErrorHelper.CreateError (58, "The Xcode.app '{0}' is invalid (the file '{1}' does not exist).", Path.GetDirectoryName (Path.GetDirectoryName (developer_directory)), plist_path);
			}

			if (xcode_version_check && XcodeVersion < new Version (6, 0))
				ErrorHelper.Error (51, "Xamarin.iOS {0} requires Xcode 6.0 or later. The current Xcode version (found in {2}) is {1}.", Constants.Version, XcodeVersion.ToString (), sdk_root);

			if (XcodeVersion < new Version (7, 0) && !accept_any_xcode_version)
				ErrorHelper.Warning (79, "The recommended Xcode version for Xamarin.iOS {0} is Xcode 7.0 or later. The current Xcode version (found in {2}) is {1}.", Constants.Version, XcodeVersion.ToString (), sdk_root);

			Driver.Log (1, "Using Xcode {0} found in {1}", XcodeVersion, sdk_root);
		}

		public static string DeveloperDirectory {
			get {
				return developer_directory;
			}
		}

		public static string FrameworkDirectory {
			get { return framework_dir; }
		}

		public static bool IsUsingClang {
			get { return compiler_path.EndsWith ("clang") || compiler_path.EndsWith ("clang++"); }
		}

		public static string CompilerPath {
			get { return compiler_path; }
		}

		public static Version SDKVersion { get { return sdk_version; } }

		public static string PlatformsDirectory {
			get {
				return Path.Combine (DeveloperDirectory, "Platforms");
			}
		}

		public static string PlatformDirectory {
			get {
				return Path.Combine (PlatformsDirectory, Platform + ".platform");
			}
		}

		public static int XcodeRun (string command, string args, StringBuilder output = null)
		{
			string [] env = DeveloperDirectory != String.Empty ? new string [] { "DEVELOPER_DIR", DeveloperDirectory } : null;
			int ret = RunCommand ("xcrun", String.Concat ("-sdk macosx ", command, " ", args), env, output);
			if (ret != 0 && verbose > 1) {
				StringBuilder debug = new StringBuilder ();
				RunCommand ("xcrun", String.Concat ("--find ", command), env, debug);
				Console.WriteLine ("failed using `{0}` from: {1}", command, debug);
			}
			return ret;
		}

		public static string GetAotCompiler (bool is64bits)
		{
			switch (app.Platform) {
			case ApplePlatform.iOS:
				if (is64bits) {
					return Path.Combine (cross_prefix, "bin", "arm64-darwin-mono-sgen");
				} else {
					return Path.Combine (cross_prefix, "bin", "arm-darwin-mono-sgen");
				}
			case ApplePlatform.WatchOS:
				return Path.Combine (cross_prefix, "bin", "armv7k-unknown-darwin-mono-sgen");
			case ApplePlatform.TVOS:
				return Path.Combine (cross_prefix, "bin", "aarch64-unknown-darwin-mono-sgen");
			default:
				throw ErrorHelper.CreateError (71, "Unknown platform: {0}. This usually indicates a bug in Xamarin.iOS; please file a bug report at http://bugzilla.xamarin.com with a test case.", app.Platform);
			}
		}

		public static string GetAotArguments (string filename, Abi abi, string outputDir, string outputFile, string llvmOutputFile, string dataFile)
		{
			string fname = Path.GetFileName (filename);
			StringBuilder args = new StringBuilder ();
			bool enable_llvm = (abi & Abi.LLVM) != 0;
			bool enable_thumb = (abi & Abi.Thumb) != 0;
			bool enable_debug = app.EnableDebug;
			bool enable_mdb = app.PackageMdb;
			bool llvm_only = app.EnableLLVMOnlyBitCode;
			string arch = abi.AsArchString ();

			args.Append ("--debug ");

			if (enable_llvm)
				args.Append ("--llvm ");

			if (!llvm_only)
				args.Append ("-O=gsharedvt ");
			args.Append (aot_other_args).Append (" ");
			args.Append ("--aot=mtriple=");
			args.Append (enable_thumb ? arch.Replace ("arm", "thumb") : arch);
			args.Append ("-ios,");
			args.Append ("data-outfile=").Append (Quote (dataFile)).Append (",");
			args.Append (aot_args);
			if (llvm_only)
				args.Append ("llvmonly,");
			else
				args.Append ("full,");

			var aname = Path.GetFileNameWithoutExtension (fname);
			var sdk_or_product = Profile.IsSdkAssembly (aname) || Profile.IsProductAssembly (aname);

			if (enable_llvm)
				args.Append ("nodebug,");
			else if (!(enable_debug || enable_mdb))
				args.Append ("nodebug,");
			else if (debug_all || debug_assemblies.Contains (fname) || !sdk_or_product)
				args.Append ("soft-debug,");

			args.Append ("dwarfdebug,");

			/* Needed for #4587 */
			if (enable_debug && !enable_llvm)
				args.Append ("no-direct-calls,");

			if (!app.UseDlsym (filename))
				args.Append ("direct-pinvoke,");

			if (app.EnableMSym) {
				var msymdir = Quote (Path.Combine (outputDir, $"{fname}.aotid.msym"));
				args.Append ($"msym-dir={msymdir},");
			}

			if (enable_llvm)
				args.Append ("llvm-path=").Append (MonoTouchDirectory).Append ("/LLVM/bin/,");

			if (!llvm_only)
				args.Append ("outfile=").Append (Quote (outputFile));
			if (!llvm_only && enable_llvm)
				args.Append (",");
			if (enable_llvm)
				args.Append ("llvm-outfile=").Append (Quote (llvmOutputFile));
			args.Append (" \"").Append (filename).Append ("\"");
			return args.ToString ();
		}

		public static ProcessStartInfo CreateStartInfo (string file_name, string arguments, string mono_path, string mono_debug = null)
		{
			var info = new ProcessStartInfo (file_name, arguments);
			info.UseShellExecute = false;
			info.RedirectStandardOutput = true;
			info.RedirectStandardError = true;

			info.EnvironmentVariables ["MONO_PATH"] = mono_path;
			info.EnvironmentVariables ["MONO_GC_PARAMS"] = app.MonoGCParams;
			if (mono_debug != null)
				info.EnvironmentVariables ["MONO_DEBUG"] = mono_debug;

			return info;
		}

		static string EncodeAotSymbol (string symbol)
		{
			var sb = new StringBuilder ();
			/* This mimics what the aot-compiler does */
			foreach (var b in System.Text.Encoding.UTF8.GetBytes (symbol)) {
				char c = (char) b;
				if ((c >= '0' && c <= '9') ||
					(c >= 'a' && c <= 'z') ||
					(c >= 'A' && c <= 'Z')) {
					sb.Append (c);
					continue;
				}
				sb.Append ('_');
			}
			return sb.ToString ();
		}

		// note: this is executed under Parallel.ForEach
		public static string GenerateMain (IEnumerable<Assembly> assemblies, string assembly_name, Abi abi, string main_source, IList<string> registration_methods)
		{
			var assembly_externs = new StringBuilder ();
			var assembly_aot_modules = new StringBuilder ();
			var register_assemblies = new StringBuilder ();
			var enable_llvm = (abi & Abi.LLVM) != 0;

			register_assemblies.AppendLine ("\tguint32 exception_gchandle = 0;");
			foreach (var s in assemblies) {
				if ((abi & Abi.SimulatorArchMask) == 0) {
					var info = s.AssemblyDefinition.Name.Name;
					info = EncodeAotSymbol (info);
					assembly_externs.Append ("extern void *mono_aot_module_").Append (info).AppendLine ("_info;");
					assembly_aot_modules.Append ("\tmono_aot_register_module (mono_aot_module_").Append (info).AppendLine ("_info);");
				}
				string sname = s.FileName;
				if (assembly_name != sname && IsBoundAssembly (s)) {
					register_assemblies.Append ("\txamarin_open_and_register (\"").Append (sname).Append ("\", &exception_gchandle);").AppendLine ();
					register_assemblies.AppendLine ("\txamarin_process_managed_exception_gchandle (exception_gchandle);");
				}
			}

			try {
				StringBuilder sb = new StringBuilder ();
				using (var sw = new StringWriter (sb)) {
					sw.WriteLine ("#include \"xamarin/xamarin.h\"");
					// Trial builds are only executable in the next 24 hours
					
					sw.WriteLine ();
					sw.WriteLine (assembly_externs);

					sw.WriteLine ("void xamarin_register_modules_impl ()");
					sw.WriteLine ("{");
					sw.WriteLine (assembly_aot_modules);
					sw.WriteLine ("}");
					sw.WriteLine ();

					sw.WriteLine ("void xamarin_register_assemblies_impl ()");
					sw.WriteLine ("{");
					sw.WriteLine (register_assemblies);
					sw.WriteLine ("}");
					sw.WriteLine ();

					if (registration_methods != null) {
						foreach (var method in registration_methods) {
							sw.Write ("void ");
							sw.Write (method);
							sw.WriteLine ("();");
						}
					}

					// Burn in a reference to the profiling symbol so that the native linker doesn't remove it
					// On iOS we can pass -u to the native linker, but that doesn't work on tvOS, where
					// we're building with bitcode (even when bitcode is disabled, we still build with the
					// bitcode marker, which makes the linker reject -u).
					if (App.EnableProfiling) {
						sw.WriteLine ("extern \"C\" { void mono_profiler_startup_log (); }");
						sw.WriteLine ("typedef void (*xamarin_profiler_symbol_def)();");
						sw.WriteLine ("extern xamarin_profiler_symbol_def xamarin_profiler_symbol;");
						sw.WriteLine ("xamarin_profiler_symbol_def xamarin_profiler_symbol = NULL;");
					}

					sw.WriteLine ("void xamarin_setup_impl ()");
					sw.WriteLine ("{");

					if (App.EnableProfiling)
						sw.WriteLine ("\txamarin_profiler_symbol = mono_profiler_startup_log;");

					if (App.EnableLLVMOnlyBitCode)
						sw.WriteLine ("\tmono_jit_set_aot_mode (MONO_AOT_MODE_LLVMONLY);");

					if (app.Registrar == RegistrarMode.LegacyDynamic || app.Registrar == RegistrarMode.LegacyStatic)
						sw.WriteLine ("\txamarin_use_old_dynamic_registrar = TRUE;");
					else
						sw.WriteLine ("\txamarin_use_old_dynamic_registrar = FALSE;");

					if (registration_methods != null) {
						for (int i = 0; i < registration_methods.Count; i++) {
							sw.Write ("\t");
							sw.Write (registration_methods [i]);
							sw.WriteLine ("();");
						}
					}

					if (app.EnableDebug)
						sw.WriteLine ("\txamarin_gc_pump = {0};", debug_track.Value ? "TRUE" : "FALSE");
					sw.WriteLine ("\txamarin_init_mono_debug = {0};", app.PackageMdb ? "TRUE" : "FALSE");
					sw.WriteLine ("\txamarin_compact_seq_points = {0};", app.EnableMSym ? "TRUE" : "FALSE");
					sw.WriteLine ("\txamarin_executable_name = \"{0}\";", assembly_name);
					sw.WriteLine ("\txamarin_use_new_assemblies = {0};", app.IsUnified ? "1" : "0");
					sw.WriteLine ("\tmono_use_llvm = {0};", enable_llvm ? "TRUE" : "FALSE");
					sw.WriteLine ("\txamarin_log_level = {0};", verbose);
					sw.WriteLine ("\txamarin_arch_name = \"{0}\";", abi.AsArchString ());
					sw.WriteLine ("\txamarin_marshal_managed_exception_mode = MarshalManagedExceptionMode{0};", app.MarshalManagedExceptions);
					sw.WriteLine ("\txamarin_marshal_objectivec_exception_mode = MarshalObjectiveCExceptionMode{0};", app.MarshalObjectiveCExceptions);
					if (app.EnableDebug)
						sw.WriteLine ("\txamarin_debug_mode = TRUE;");
					if (!string.IsNullOrEmpty (app.MonoGCParams))
						sw.WriteLine ("\tsetenv (\"MONO_GC_PARAMS\", \"{0}\", 1);", app.MonoGCParams);
					foreach (var kvp in environment_variables)
						sw.WriteLine ("\tsetenv (\"{0}\", \"{1}\", 1);", kvp.Key.Replace ("\"", "\\\""), kvp.Value.Replace ("\"", "\\\""));
					sw.WriteLine ("}");
					sw.WriteLine ();
					sw.Write ("int ");
					sw.Write (app.IsWatchExtension ? "xamarin_watchextension_main" : "main");
					sw.WriteLine (" (int argc, char **argv)");
					sw.WriteLine ("{");
					sw.WriteLine ("\tNSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];");
					if (app.IsExtension) {
						// the name of the executable must be the bundle id (reverse dns notation)
						// but we do not want to impose that (ugly) restriction to the managed .exe / project name / ...
						sw.WriteLine ("\targv [0] = \"{0}\";", Path.GetFileNameWithoutExtension (app.RootAssembly));
						sw.WriteLine ("\tint rv = xamarin_main (argc, argv, true);");
					} else {
						sw.WriteLine ("\tint rv = xamarin_main (argc, argv, false);");
					}
					sw.WriteLine ("\t[pool drain];");
					sw.WriteLine ("\treturn rv;");
					sw.WriteLine ("}");

					sw.WriteLine ("void xamarin_initialize_callbacks () __attribute__ ((constructor));");
					sw.WriteLine ("void xamarin_initialize_callbacks ()");
					sw.WriteLine ("{");
					sw.WriteLine ("\txamarin_setup = xamarin_setup_impl;");
					sw.WriteLine ("\txamarin_register_assemblies = xamarin_register_assemblies_impl;");
					sw.WriteLine ("\txamarin_register_modules = xamarin_register_modules_impl;");
					sw.WriteLine ("}");

				}
				WriteIfDifferent (main_source, sb.ToString ());
			} catch (MonoTouchException) {
				throw;
			} catch (Exception e) {
				throw new MonoTouchException (4001, true, e, "The main template could not be expanded to `{0}`.", main_source);
			}

			return main_source;
		}

		[DllImport (Constants.libSystemLibrary)]
		static extern int symlink (string path1, string path2);

		public static bool Symlink (string path1, string path2)
		{
			return symlink (path1, path2) == 0;
		}

		[DllImport (Constants.libSystemLibrary)]
		static extern int unlink (string pathname);

		public static void FileDelete (string file)
		{
			// File.Delete can't always delete symlinks (in particular if the symlink points to a file that doesn't exist).
			unlink (file);
			// ignore any errors.
		}

		public static void CopyAssembly (string source, string target, string target_dir = null)
		{
			if (File.Exists (target))
				File.Delete (target);
			if (target_dir == null)
				target_dir = Path.GetDirectoryName (target);
			if (!Directory.Exists (target_dir))
				Directory.CreateDirectory (target_dir);
			File.Copy (source, target);

			string sconfig = source + ".config";
			string tconfig = target + ".config";
			if (File.Exists (tconfig))
				File.Delete (tconfig);
			if (File.Exists (sconfig))
				File.Copy (sconfig, tconfig);

			string tdebug = target + ".mdb";
			if (File.Exists (tdebug))
				File.Delete (tdebug);
			string sdebug = source + ".mdb";
			if (File.Exists (sdebug))
				File.Copy (sdebug, tdebug);
		}

		public static bool SymlinkAssembly (string source, string target, string target_dir)
		{
			if (app.IsSimulatorBuild && Driver.XcodeVersion >= new Version (6, 0)) {
				// Don't symlink with Xcode 6, it has a broken simulator (at least in
				// that doesn't work properly when installing/upgrading apps with symlinks.
				return false;
			}

			if (File.Exists (target))
				File.Delete (target);
			if (!Directory.Exists (target_dir))
				Directory.CreateDirectory (target_dir);
			if (!Symlink (source, target))
				return false;

			string sconfig = source + ".config";
			string tconfig = target + ".config";
			if (File.Exists (tconfig))
				File.Delete (tconfig);
			if (File.Exists (sconfig))
				Symlink (sconfig, tconfig);

			string tdebug = target + ".mdb";
			if (File.Exists (tdebug))
				File.Delete (tdebug);
			string sdebug = source + ".mdb";
			if (File.Exists (sdebug))
				Symlink (sdebug, tdebug);
			return true;
		}

		static List<string> mdbs = new List<string> ();

		public static void DeleteDebugInfos ()
		{
			foreach (var mdb in mdbs) {
				if (File.Exists (mdb)) {
					try {
						File.Delete (mdb);
					} catch {
						// Ignore any exceptions
					}
				}
			}
		}


		public static string Quote (string f)
		{
			if (f.IndexOf (' ') == -1 && f.IndexOf ('\'') == -1 && f.IndexOf (',') == -1)
				return f;

			var s = new StringBuilder ();

			s.Append ('"');
			foreach (var c in f) {
				if (c == '"' || c == '\\')
					s.Append ('\\');

				s.Append (c);
			}
			s.Append ('"');

			return s.ToString ();
		}

		public static void GatherFrameworks (Target target, HashSet<string> frameworks, HashSet<string> weak_frameworks)
		{
			AssemblyDefinition monotouch = null;

			foreach (var assembly in target.Assemblies) {
				if (assembly.AssemblyDefinition.FullName == target.ProductAssembly.FullName) {
					monotouch = assembly.AssemblyDefinition;
					break;
				}
			}

			// *** make sure any change in the above lists (or new list) are also reflected in 
			// *** Makefile so simlauncher-sgen does not miss any framework

			HashSet<string> processed = new HashSet<string> ();
			Version v80 = new Version (8, 0);

			foreach (ModuleDefinition md in monotouch.Modules) {
				foreach (TypeDefinition td in md.Types) {
					// process only once each namespace (as we keep adding logic below)
					string nspace = td.Namespace;
					if (processed.Contains (nspace))
						continue;
					processed.Add (nspace);

					Framework framework;
					if (Driver.Frameworks.TryGetValue (nspace, out framework)) {
						// framework specific processing
						switch (framework.Name) {
						case "CoreAudioKit":
							// CoreAudioKit seems to be functional in the iOS 9 simulator.
							if (app.IsSimulatorBuild && SDKVersion.Major < 9)
								continue;
							break;
						case "Metal":
						case "MetalKit":
						case "MetalPerformanceShaders":
							// some frameworks do not exists on simulators and will result in linker errors if we include them
							if (app.IsSimulatorBuild)
								continue;
							break;
						case "PushKit":
							// in Xcode 6 beta 7 this became an (ld) error - it was a warning earlier :(
							// ld: embedded dylibs/frameworks are only supported on iOS 8.0 and later (@rpath/PushKit.framework/PushKit) for architecture armv7
							// this was fixed in Xcode 6.2 (6.1 was still buggy) see #29786
							if ((app.DeploymentTarget < v80) && (XcodeVersion < new Version (6, 2))) {
								ErrorHelper.Warning (49, "{0}.framework is supported only if deployment target is 8.0 or later. {0} features might not work correctly.", framework.Name);
								continue;
							}
							break;
						}

						if (sdk_version >= framework.Version) {
							var add_to = app.DeploymentTarget >= framework.Version ? frameworks : weak_frameworks;
							add_to.Add (framework.Name);
							continue;
						}
					}
				}
			}
		}

		public static bool CanWeSymlinkTheApplication ()
		{
			if (app.Platform != ApplePlatform.iOS)
				return false;

			// We can not symlink when building an extension.
			if (app.IsExtension)
				return false;

			// We can not symlink if we have to link with user frameworks.
			if (app.Frameworks.Count > 0 || app.UseMonoFramework.Value)
				return false;
			foreach (var target in app.Targets) {
				foreach (var assembly in target.Assemblies)
					if (assembly.Frameworks != null && assembly.Frameworks.Count > 0)
						return false;
			}

			//We can only symlink when running in the simulation
			if (!app.IsSimulatorBuild)
				return false;

			//mtouch was invoked with --nofastsim, eg symlinking was explicit disabled
			if (!fast_sim)
				return false;

			//Can't symlink if we are running the linker since the assemblies content will change
			if (app.LinkMode != LinkMode.None)
				return false;

			//Custom gcc flags requires us to build template.m
			if (!string.IsNullOrEmpty (app.UserGccFlags))
				return false;

			// Setting environment variables is done in the generated main.m, so we can't symlink in this case.
			if (environment_variables.Count > 0)
				return false;

			if (app.Registrar == RegistrarMode.Static || app.Registrar == RegistrarMode.LegacyStatic || app.Registrar == RegistrarMode.LegacyDynamic)
				return false;

			// The default exception marshalling differs between release and debug mode, but we
			// only have one simlauncher, so to use the simlauncher we'd have to chose either
			// debug or release mode. Debug is more frequent, so make that the fast path.
			if (!app.EnableDebug)
				return false;

			if (app.MarshalObjectiveCExceptions != MarshalObjectiveCExceptionMode.UnwindManagedCode)
				return false; // UnwindManagedCode is the default for debug builds.

			return true;
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static IntPtr /* CFPropertyListRef */ CFPropertyListCreateWithData (
			IntPtr /* CFAllocatorRef */ allocator,
			IntPtr /* CFDataRef */ data,
			int /* CFOptionFlags */ options,
			IntPtr /* CFPropertyListFormat */ format,
			IntPtr /* CFErrorRef */ error);

		internal static PDictionary FromPList (string name)
		{
			if (!File.Exists (name))
				throw new MonoTouchException (24, true, "Could not find required file '{0}'.", name);
			return PDictionary.FromFile (name);
		}

		static int watch_level;
		static Stopwatch watch;

		public static void Watch (string msg, int level)
		{
			if (watch != null && (watch_level > level))
				Console.WriteLine ("{0}: {1} ms", msg, watch.ElapsedMilliseconds);
		}

		internal static bool TryParseBool (string value, out bool result)
		{
			if (string.IsNullOrEmpty (value)) {
				result = true;
				return true;
			}

			switch (value.ToLowerInvariant ()) {
			case "1":
			case "yes":
			case "true":
			case "enable":
				result = true;
				return true;
			case "0":
			case "no":
			case "false":
			case "disable":
				result = false;
				return true;
			default:
				return bool.TryParse (value, out result);
			}
		}

		internal static bool ParseBool (string value, string name, bool show_error = true)
		{
			bool result;
			if (!TryParseBool (value, out result))
				throw ErrorHelper.CreateError (26, "Could not parse the command line argument '-{0}:{1}'", name, value);
			return result;
		}

		public static int Main (string [] args)
		{
			try {
				Console.OutputEncoding = new UTF8Encoding (false, false);
				return Main2 (args);
			}
			catch (Exception e) {
				ErrorHelper.ExitCallback = (int exitCode) => Exit (exitCode);
				ErrorHelper.Show (e);
			}
			finally {
				Watch ("Total time", 0);
			}
			return 0;
		}

		static void SetAction (Action value)
		{
			switch (action) {
			case Action.None:
				action = value;
				break;
			case Action.KillApp:
				if (value == Action.LaunchDevice) {
					action = Action.KillAndLaunch;
					break;
				}
				goto default;
			case Action.LaunchDevice:
				if (value == Action.KillApp) {
					action = Action.KillAndLaunch;
					break;
				}
				goto default;
			default:
				throw new MonoTouchException (19, true, "Only one --[log|install|kill|launch]dev or --[launch|debug|list]sim option can be used.");
			}
		}


		[DllImport ("/usr/lib/libSystem.dylib")]
		static extern void exit (int exitcode);

		public static void Exit (int exitCode = 0)
		{
			// This is ugly. *Very* ugly. The problem is that:
			//
			// * The Apple API we use will create background threads to process messages.
			// * Those messages are delivered to managed code, which means the mono runtime
			//   starts tracking those threads.
			// * The mono runtime will not terminate those threads (in fact the mono runtime
			//   will do nothing at all to those threads, since they're not running managed
			//   code) upon shutdown. But the mono runtime will wait for those threads to
			//   exit before exiting the process. This means mtouch will never exit.
			// 
			// So just go the nuclear route.
			exit (exitCode);
		}

		static int Main2 (string [] args)
		{
			var assemblies = new List<string> ();

			if (extra_args != null) {
				var l = new List<string> (args);
				foreach (var s in extra_args.Split (new char [] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
					l.Add (s);
				args = l.ToArray ();
			}

			string tls_provider = null;
			string http_message_handler = null;

			var os = new OptionSet () {
			{ "h|?|help", "Displays the help", v => SetAction (Action.Help) },
			{ "version", "Output version information and exit.", v => SetAction (Action.Version) },
			{ "d|dir=", "Output directory for the results (Used for partial builds) [Deprecated]", v => { output_dir = v; classic_only_arguments.Add ("--dir"); }, true },
			{ "cp=|crossprefix=", "Specifies the Mono cross compiler prefix [Deprecated]", v => { cross_prefix = v; classic_only_arguments.Add ("--crossprefix"); }, true },
			{ "f|force", "Forces the recompilation of code, regardless of timestamps", v=>force = true },
			{ "cache=", "Specify the directory where object files will be cached", v => Cache.Location = v },
			{ "aot=", "Arguments to the static compiler",
				v => aot_args = v + (v.EndsWith (",") ? String.Empty : ",") + aot_args
			},
			{ "aot-options=", "Non AOT arguments to the static compiler",
				v => {
					if (v.Contains ("--profile") || v.Contains ("--attach"))
						throw new Exception ("Unsupported flag to -aot-options");
					aot_other_args = v + " " + aot_other_args;
				}
			},
			{ "gsharedvt:", "Generic sharing for value-types - always enabled [Deprecated]", v => {} },
			{ "libdir", "Directories that contains the assemblies to use for compiling [Deprecated]", v => { classic_only_arguments.Add ("--libdir"); }, true },
			{ "v", "Verbose", v => verbose++ },
			{ "q", "Quiet", v => verbose-- },
			{ "n", "Dry run [Deprecated]", v => { dry_run = true; classic_only_arguments.Add ("-n"); }, true },
			{ "keeptemp", "[Deprecated]", v => { classic_only_arguments.Add ("--keeptemp"); }, true /* deprecated, hide it for the future, but don't show errors if it's present */ },
			{ "time", v => watch_level++ },
			{ "executable=", "Specifies the native executable name to output", v => app.ExecutableName = v },
			{ "m|main=", "Specifies the name of the main startup file, defaults to main.m [Deprecated]", v => { classic_only_arguments.Add ("--main"); }, true },
			{ "nomanifest", "Do not generate PkgInfo and Info.plist [Deprecated in the Unified API]", v => { app.GenerateManifests = false; classic_only_arguments.Add ("--nomanifest"); } },
			{ "nofastsim", "Do not run the simulator fast-path build", v => fast_sim = false },
			{ "nolink", "Do not link the assemblies", v => app.LinkMode = LinkMode.None },
			{ "mapinject", "[Deprecated]", v => { classic_only_arguments.Add ("--mapinject"); }, true },
			{ "nodebugtrack", "Disable debug tracking of object resurrection bugs", v => debug_track = false },
			{ "debugtrack:", "Enable debug tracking of object resurrection bugs (enabled by default for the simulator)", v => { debug_track = ParseBool (v, "--debugtrack"); } },
			{ "linkerdumpdependencies", "Dump linker dependencies for linker-analyzer tool", v => app.LinkerDumpDependencies = true },
			{ "linksdkonly", "Link only the SDK assemblies", v => app.LinkMode = LinkMode.SDKOnly },
			{ "linkskip=", "Skip linking of the specified assembly", v => app.LinkSkipped.Add (v) },
			{ "nolinkaway", "Disable the linker step which replace code with a 'Linked Away' exception.", v => app.LinkAway = false },
			{ "xml=", "Provide an extra XML definition file to the linker", v => app.Definitions.Add (v) },
			{ "i18n=", "List of i18n assemblies to copy to the output directory, separated by commas (none,all,cjk,mideast,other,rare,west)", v => app.I18n = LinkerOptions.ParseI18nAssemblies (v) },
			{ "symbollist=", "Asks mtouch to create a file with all the symbols that should not be stripped instead of stripping.", v => app.SymbolList = v, true /* hidden, this is only used between mtouch and the MSBuild tasks, not for public consumption */ },
			{ "nostrip", "Do not strip the AOT compiled assemblies", v => app.ManagedStrip = false },
			{ "nosymbolstrip:", "A comma-separated list of symbols to not strip from the app (if the list is empty, stripping is disabled completely)", v =>
				{
					if (string.IsNullOrEmpty (v)) {
						app.NativeStrip = false;
					} else {
						app.NoSymbolStrip.AddRange (v.Split (','));
					}
				} },
			{ "dsym:", "Turn on (default for device) or off (default for simulator) .dSYM symbols.", v => app.BuildDSym = ParseBool (v, "dsym") },
			{ "nosign", "Do not sign the application [Deprecated in the Unified API]", v => { app.Sign = false; classic_only_arguments.Add ("--nosign"); } },
			{ "dlsym:", "Use dlsym to resolve pinvokes in AOT compiled assemblies", v => app.ParseDlsymOptions (v) },
			{ "r|ref=", "Add an assembly to the resolver", v => app.References.Add (v) },
			{ "gcc_flags=", "Set flags to be passed along to gcc at link time", v => app.UserGccFlags = v },
			{ "framework=", "Link with the specified framework. This can either be a system framework (like 'UIKit'), or it can be a path to a custom framework ('/path/to/My.framework'). In the latter case the entire 'My.framework' directory is copied into the app as well.", (v) => app.Frameworks.Add (v) },
			{ "weak-framework=", "Weak link with the specified framework. This can either be a system framework (like 'UIKit'), or it can be a path to a custom framework ('/path/to/My.framework'). In the latter case the entire 'My.framework' directory is copied into the app as well.", (v) => app.Frameworks.Add (v) },	

			//
			// Bundle configuration
			//
			{ "displayname=", "Specifies the display name [Deprecated]", v => { app.BundleDisplayName = v; classic_only_arguments.Add ("--displayname"); }, true },
			{ "bundleid=", "Specifies the bundle identifier (com.foo.exe) [Deprecated]", v => { app.BundleId = v; classic_only_arguments.Add ("--bundleid"); }, true },
			{ "mainnib=", "Specifies the name of the main Nib file to load [Deprecated]", v => { app.MainNib = v; classic_only_arguments.Add ("--mainnib"); }, true },
			{ "icon=", "Specifies the name of the icon to use [Deprecated]", v => { app.Icon = v; classic_only_arguments.Add ("--icon"); }, true },
				
			// What we do
			{ "sim=", "Compile for the Simulator, specify the output directory for code", v =>
				{
					SetAction (Action.Build);
					output_dir = v;
					app.BuildTarget = BuildTarget.Simulator;
				}
			},
			{ "dev=", "Compile for the Device, specify the output directory for the code", v => {
					SetAction (Action.Build);
					output_dir = v;
					app.BuildTarget = BuildTarget.Device;
				}
			},
			{ "c|certificate=", "The Code Signing certificate for the application [Deprecated]", v => { app.CertificateName = v; classic_only_arguments.Add ("--certificate"); }, true },
			// Configures the tooling used to build code.
			{ "sdk=", "Specifies the name of the SDK to compile against (version, for example \"3.2\")",
				v => {
					try {
						sdk_version = Version.Parse (v);
					} catch (Exception ex) {
						ErrorHelper.Error (26, ex, "Could not parse the command line argument '{0}': {1}", "-sdk", ex.Message);
					}
				}
			},
			{ "targetver=", "Specifies the name of the minimum deployment target (version, for example \"" + Xamarin.SdkVersions.iOS.ToString () + "\")",
				v => {
					try {
						app.DeploymentTarget = Version.Parse (v);
					} catch (Exception ex) {
						throw new MonoTouchException (26, true, ex, "Could not parse the command line argument '{0}': {1}", "-targetver", ex.Message);
					}
				}
			},
			{ "device=", "Specifies the device type to launch the simulator as [DEPRECATED]", v => { }, true },
			
			// Launch/debug options
			{ "timeout:", "Specify a timeout (in seconds) for the commands that doesn't have fixed/known duration. Default: 0.5 [DEPRECATED]", v => { }, true },
			{ "listapps", "List the apps installed on the device to stderr. [DEPRECATED]", v => { SetAction (Action.ListApps); }, true },
			{ "listsim=", "List the available simulators. The output is xml, and written to the specified file. [DEPRECATED]", v => { SetAction (Action.ListSimulators); }, true},
			{ "listdev:", "List the currently connected devices and their UDIDs [DEPRECATED]", v => { SetAction (Action.ListDevices); }, true },
			{ "installdev=", "Install the specified MonoTouch.app in the device [DEPRECATED]", v => { SetAction (Action.InstallDevice); }, true },
			{ "launchdev=", "Launch an app that is installed on device, specified by bundle identifier [DEPRECATED]", v => { SetAction (Action.LaunchDevice); }, true },
			{ "debugdev=", "Launch app app that is installed on device, specified by bundle identifer, and wait for a native debugger to attach. [DEPRECATED]", v => { SetAction (Action.DebugDevice); }, true },
			{ "provide-assets=", "If the app contains on-demand resources, then mtouch should provide those from this .app directory. [DEPRECATED]", v => { }, true },
			{ "wait-for-exit:", "If mtouch should wait until the launched app exits before existing. [DEPRECATED]", v => { }, true },
			{ "wait-for-unlock:", "If mtouch should wait until the device is unlocked when launching an app (or exit with an error code). [DEPRECATED]", v => { }, true },
			{ "killdev=", "Kill an app that is running on device, specified by bundle identifier [DEPRECATED]", v => { SetAction (Action.KillApp); }, true },
			{ "logdev", "Write the syslog from the device to the console [DEPRECATED]", v => { SetAction (Action.LogDev); }, true },
			{ "list-crash-reports:", "Lists crash reports on the specified device [DEPRECATED]", v => { SetAction (Action.ListCrashReports); }, true },
			{ "download-crash-report=", "Download a crash report from the specified device [DEPRECATED]", v => { SetAction (Action.DownloadCrashReport); }, true},
			{ "download-crash-report-to=", "Specifies the file to save the downloaded crash report. [DEPRECATED]", v => { }, true },
			{ "devname=", "Specify which device (when many are present) the [install|lauch|kill|log]dev command applies [DEPRECATED]", v => { }, true},
			{ "isappinstalled=", "Check if the specified app id is installed. Returns 0 when installed, 1 if not. [DEPRECATED]", v => { SetAction (Action.IsAppInstalled); }, true },
			{ "launchsim=", "Launch the specified MonoTouch.app in the simulator [DEPRECATED]", v => { SetAction (Action.LaunchSim); }, true },
			{ "enable-native-debugging:", "Don't do anything that may interfere with native debugging (such as avoiding the iOS simulator launch timeout). [DEPRECATED]", v => { }, true },
			{ "installsim=", "Install the specified MonoTouch.app in the simulator [DEPRECATED]", v => { SetAction (Action.InstallSim); }, true},
			{ "launchsimwatch=", "Specify the watch app to launch [DEPRECATED]", v => { SetAction (Action.LaunchWatchApp); }, true },
			{ "killsimwatch=", "Specify the watch app to kill [DEPRECATED]", v => { SetAction (Action.KillWatchApp); }, true },
			{ "watchnotificationpayload=", "Specify the jSON notification payload file [DEPRECATED]", v => { }, true },
			{ "watchlaunchmode=", "Specify the watch launch mode (Default|Glance|Notification) [DEPRECATED]", v => { }, true },
			{ "enable-background-fetch", "Enable mode to send background fetch requests [Deprecated]", v => { }, true},
			{ "launch-for-background-fetch", "Launch due to a background fetch [DEPRECATED]", v => { }, true},
			{ "debugsim=", "Debug the specified MonoTouch.app in the simulator [DEPRECATED]", v => { SetAction (Action.DebugSim); }, true },
			{ "argument=", "Launch the app with this command line argument. This must be specified multiple times for multiple arguments [DEPRECATED]", v => { }, true },
			{ "setenv=", "Set the environment variable in the application on startup", v =>
				{
					int eq = v.IndexOf ('=');
					if (eq <= 0)
						throw new MonoTouchException (2, true, "Could not parse the environment variable '{0}'", v);
					string name = v.Substring (0, eq);
					string value = v.Substring (eq + 1);
					environment_variables.Add (name, value);
				}
			},
			{ "sgen:", "Enable the SGen garbage collector",
					v => {
						if (!ParseBool (v, "sgen")) 
							ErrorHelper.Warning (43, "The Boehm garbage collector is not supported. The SGen garbage collector has been selected instead.");
					},
					true // do not show the option anymore
				},
			{ "boehm:", "Enable the Boehm garbage collector",
					v => {
						if (ParseBool (v, "boehm"))
							ErrorHelper.Warning (43, "The Boehm garbage collector is not supported. The SGen garbage collector has been selected instead."); }, 
					true // do not show the option anymore
				},
			{ "new-refcount:", "Enable new refcounting logic",
				v => {
					if (!ParseBool (v, "new-refcount"))
						ErrorHelper.Warning (80, "Disabling the new refcount logic is deprecated.");
				},
				true // do not show the option anymore
			},
			{ "llvm", "Enable the LLVM compiler [Deprecated, use --abi instead]", v => { app.ParseAbi ("armv7+llvm"); classic_only_arguments.Add ("--llvm"); }, true },
			{ "thumb", "Enable LLVM-Thumb support [Deprecated, use --abi instead]", v => { app.ParseAbi ("armv7+llvm+thumb2"); classic_only_arguments.Add ("--thumb"); }, true },
			{ "armv7", "Enable ARMv7 support [Deprecated, use --abi instead]", v => { app.ParseAbi ("armv7"); classic_only_arguments.Add ("--armv7"); }, true },
			{ "abi=", "Comma-separated list of ABIs to target. Currently supported: armv7, armv7+llvm, armv7+llvm+thumb2, armv7s, armv7s+llvm, armv7s+llvm+thumb2, arm64, arm64+llvm, i386, x86_64", v => app.ParseAbi (v) },
			{ "override-abi=", "Override any previous abi. Only used for testing.", v => { app.ClearAbi (); app.ParseAbi (v); }, true }, // Temporary command line arg until XS has better support for 64bit architectures.
			{ "cxx", "Enable C++ support", v => { app.EnableCxx = true; }},
			{ "enable-repl:", "Enable REPL support (simulator and not linking only)", v => { app.EnableRepl = ParseBool (v, "enable-repl"); }, true /* this is a hidden option until we've actually used it and made sure it works as expected */ },
			{ "pie:", "Enable (default) or disable PIE (Position Independent Executable).", v => { app.EnablePie = ParseBool (v, "pie"); }},
			{ "compiler=", "Specify the Objective-C compiler to use (valid values are gcc, g++, clang, clang++ or the full path to a GCC-compatible compiler).", v => { compiler = v; }},
			{ "fastdev", "Build an app that supports fastdev (this app will only work when launched using Xamarin Studio)", v => { app.FastDev = true; }},
			{ "force-thread-check", "Keep UI thread checks inside (even release) builds", v => { app.ThreadCheck = true; }},
			{ "disable-thread-check", "Remove UI thread checks inside (even debug) builds", v => { app.ThreadCheck = false; }},
			{ "debug:", "Generate debug code in Mono for the specified assembly (set to 'all' to generate debug code for all assemblies, the default is to generate debug code for user assemblies only)",
				v => {
					app.EnableDebug = true;
					if (v != null){
						if (v == "all"){
							debug_all = true;
							return;
						}
						debug_assemblies.Add (Path.GetFileName (v));
					}
				}
			},
			{ "package-mdb:", "Specify whether debug info files (*.mdb) should be packaged in the app. Default is 'true' for debug builds and 'false' for release builds.", v => app.PackageMdb = ParseBool (v, "package-mdb") },
			{ "msym:", "Specify whether managed symbolication files (*.msym) should be created. Default is 'false' for debug builds and 'true' for release builds.", v => app.EnableMSym = ParseBool (v, "msym") },
			{ "extension", v => app.IsExtension = true },
			{ "app-extension=", "The path of app extensions that are included in the app. This must be specified once for each app extension.", v => app.Extensions.Add (v), true /* MSBuild-internal for now */ },
			{ "profiling:", "Enable profiling", v => app.EnableProfiling = ParseBool (v, "profiling") },
			{ "noregistrar", "Disable the optimized class registrar [Deprecated, use --registrar:dynamic instead]", v => { app.Registrar = RegistrarMode.Dynamic; classic_only_arguments.Add ("--noregistrar"); }, true},
			{ "registrar:", "Specify the registrar to use (dynamic, static or default (dynamic in the simulator, static on device))", v =>
				{
					var split = v.Split ('=');
					var name = split [0];
					var value = split.Length > 1 ? split [1] : string.Empty;

					switch (name) {
					case "static":
						app.Registrar = RegistrarMode.Static;
						break;
					case "dynamic":
						app.Registrar = RegistrarMode.Dynamic;
						break;
					case "default":
						app.Registrar = RegistrarMode.Default;
						break;
					case "legacy":
						app.Registrar = RegistrarMode.Legacy;
						break;
					case "legacystatic":
					case "oldstatic":
						app.Registrar = RegistrarMode.LegacyStatic;
						break;
					case "legacydynamic":
					case "olddynamic":
						app.Registrar = RegistrarMode.LegacyDynamic;
						break;
					default:
						throw new MonoTouchException (20, true, "The valid options for '{0}' are '{1}'.", "--registrar", "static, dynamic or default");
					}

					switch (value) {
					case "trace":
						app.RegistrarOptions = RegistrarOptions.Trace;
						break;
					case "default":
					case "":
						app.RegistrarOptions = RegistrarOptions.Default;
						break;
					default:
						throw new MonoTouchException (20, true, "The valid options for '{0}' are '{1}'.", "--registrar", "static, dynamic or default");
					}
				}
			},
			{ "runregistrar:", "Runs the registrar on the input assembly and outputs a corresponding native library.",
				v => {
					SetAction (Action.RunRegistrar);
					app.RegistrarOutputLibrary = v;
				},
				true /* this is an internal option */
			},
			{ "unsupported--enable-generics-in-registrar", "[Deprecated]", v => { enable_generic_nsobject = true; classic_only_arguments.Add ("--unsupported--enable-generics-in-registrar"); }, true },
			{ "stderr=", "Redirect the standard error for the simulated application to the specified file [DEPRECATED]", v => { }, true },
			{ "stdout=", "Redirect the standard output for the simulated application to the specified file [DEPRECATED]", v => { }, true },
			{ "sdkroot=", "Specify the location of Apple SDKs, default to 'xcode-select' value.", v => sdk_root = v },
			{ "crashreporting-api-key=", "Specify the Crashlytics API key to use (which will also enable Crashlytics support). [Deprecated].", v =>
				{
					throw new MonoTouchException (16, true, "The option '{0}' has been deprecated.", "--crashreporting-api-key");
				}, true
			},
			{ "crashreporting-delay=", "Specify the delay before Crashlytics should process crash reports in the backgorund. [Deprecated].", v =>
				{
					throw new MonoTouchException (16, true, "The option '{0}' has been deprecated.", "--crashreporting-delay");
				}, true
			},
			{ "crashreporting-console-poll-interval=", "Specify how often (in milliseconds) to poll the Device Console for new messages to be copied to crash reports (set to 0 to disable). Disabled by default. [Deprecated].", v =>
				{
					throw new MonoTouchException (16, true, "The option '{0}' has been deprecated.", "--crashreporting-console-poll-interval");
				}, true
			},
			{ "no-xcode-version-check", "Ignores the Xcode version check.", v => { xcode_version_check = false; }, true /* This is a non-documented option. Please discuss any customers running into the xcode version check on the maciosdev@ list before giving this option out to customers. */ },
			{ "mono:", "Comma-separated list of options for how the Mono runtime should be included. Possible values: 'static' (link statically), 'framework' (linked as a user framework), '[no-]package-framework' (if the Mono.framework should be copied to the app bundle or not. The default value is 'framework' for extensions, and main apps if the app targets iOS 8.0 or later and contains extensions, otherwise 'static'. The Mono.framework will be copied to the app bundle if mtouch detects it's needed, but this may be overridden if the default values for 'framework' vs 'static' is overwridden.", v =>
				{
					foreach (var opt in v.Split (new char [] { ',' })) {
						switch (v) {
						case "static":
							app.UseMonoFramework = false;
							break;
						case "framework":
							app.UseMonoFramework = true;
							break;
						case "package-framework":
							app.PackageMonoFramework = true;
							break;
						case "no-package-framework":
							app.PackageMonoFramework = false;
							break;
						default:
							throw new MonoTouchException (20, true, "The valid options for '{0}' are '{1}'.", "--mono", "static, framework or [no-]package-framework");
						}
					}
				}
			},
			{"target-framework=", "Specify target framework to use. Currently supported: 'MonoTouch,v1.0', 'Xamarin.iOS,v1.0', 'Xamarin.WatchOS,v1.0' and 'Xamarin.TVOS,v1.0' (defaults to '" + TargetFramework.Default + "')", v => SetTargetFramework (v) },
			{ "bitcode:", "Enable generation of bitcode (asmonly, full, marker)", v =>
				{
					switch (v) {
					case "asmonly":
						app.BitCodeMode = BitCodeMode.ASMOnly;
						break;
					case "full":
						app.BitCodeMode = BitCodeMode.LLVMOnly;
						break;
					case "marker":
						app.BitCodeMode = BitCodeMode.MarkerOnly;
						break;
					default:
						throw new MonoTouchException (20, true, "The valid options for '{0}' are '{1}'.", "--bitcode", "asmonly, full or marker");
					}
				}
			},
			{ "llvm-asm", "Make the LLVM compiler emit assembly files instead of object files. [Deprecated]", v => { llvm_asmwriter = true; }, true},
			{ "http-message-handler=", "Specify the default HTTP message handler for HttpClient", v => { http_message_handler = v; }},
			{ "output-format=", "Specify the output format for some commands. Possible values: Default, XML", v =>
				{
				}
			},
			{ "tls-provider=", "Specify the default TLS provider", v => { tls_provider = v; }},
			{ "xamarin-framework-directory=", "The framework directory", v => { mtouch_dir = v; }, true },
		};

			AddSharedOptions (os);

			try {
				assemblies = os.Parse (args);
			}
			catch (MonoTouchException) {
				throw;
			}
			catch (Exception e) {
				throw new MonoTouchException (10, true, e, "Could not parse the command line arguments: {0}", e);
			}

			if (watch_level > 0) {
				watch = new Stopwatch ();
				watch.Start ();
			}

			if (action == Action.Help) {
				ShowHelp (os);
				return 0;
			} else if (action == Action.Version) {
				Console.Write ("mtouch {0}.{1}", Constants.Version, Constants.Revision);
				Console.WriteLine ();
				return 0;
			}

			app.SetDefaultFramework ();
			app.SetDefaultAbi ();

			if (app.EnableDebug && app.IsLLVM)
				ErrorHelper.Warning (3003, "Debugging is not supported when building with LLVM. Debugging has been disabled.");

			if (!app.IsLLVM && (app.EnableAsmOnlyBitCode || app.EnableLLVMOnlyBitCode))
				ErrorHelper.Error (3008, "Bitcode support requires the use of LLVM (--abi=arm64+llvm etc.)");

			if (EnableDebug) {
				if (!debug_track.HasValue) {
					debug_track = app.IsSimulatorBuild;
				}
			} else {
				if (debug_track.HasValue) {
					ErrorHelper.Warning (32, "The option '--debugtrack' is ignored unless '--debug' is also specified.");
				}
				debug_track = false;
			}

			if (app.EnableAsmOnlyBitCode)
				llvm_asmwriter = true;

			ErrorHelper.Verbosity = verbose;

			app.RuntimeOptions = RuntimeOptions.Create (http_message_handler, tls_provider);

			ValidateXcode ();

			switch (action) {
			/* Device actions */
			case Action.LogDev:
			case Action.InstallDevice:
			case Action.ListDevices:
			case Action.IsAppInstalled:
			case Action.ListCrashReports:
			case Action.DownloadCrashReport:
			case Action.KillApp:
			case Action.KillAndLaunch:
			case Action.LaunchDevice:
			case Action.DebugDevice:
			case Action.ListApps:
			/* Simulator actions */
			case Action.DebugSim:
			case Action.LaunchSim:
			case Action.InstallSim:
			case Action.LaunchWatchApp:
			case Action.KillWatchApp:
			case Action.ListSimulators:
				return CallMlaunch ();
			}

			framework_dir = GetFrameworkDir (Platform, sdk_version);
			Driver.Log ("Xamarin.iOS {0}{1} using framework: {2}", Constants.Version, verbose > 1 ? "." + Constants.Revision : string.Empty, framework_dir);

			if (action == Action.None)
				throw new MonoTouchException (52, true, "No command specified.");

			// warn if we ask to remove thread checks but the linker is not enabled
			if (app.LinkMode == LinkMode.None && app.ThreadCheck.HasValue && !app.ThreadCheck.Value) {
				ErrorHelper.Show (new MonoTouchException (2003, false, "Option '{0}' will be ignored since linking is disabled", "-disable-thread-check"));
			}

			if (sdk_version < new Version (6, 0) && app.IsArchEnabled (Abi.ARMv7s))
				throw new MonoTouchException (14, true, "The iOS {0} SDK does not support building applications targeting {1}", sdk_version, "ARMv7s");

			if (sdk_version < new Version (7, 0) && app.IsArchEnabled (Abi.ARM64))
				throw new MonoTouchException (14, true, "The iOS {0} SDK does not support building applications targeting {1}", sdk_version, "ARM64");

			if (sdk_version < new Version (7, 0) && app.IsArchEnabled (Abi.x86_64))
				throw new MonoTouchException (14, true, "The iOS {0} SDK does not support building applications targeting {1}", sdk_version, "x86_64");

			if (!Directory.Exists (framework_dir)) {
				Console.WriteLine ("Framework does not exist {0}", framework_dir);
				Console.WriteLine ("   Platform = {0}", Platform);
				Console.WriteLine ("   SDK = {0}", sdk_version);
				Console.WriteLine ("   Deployment Version: {0}", app.DeploymentTarget);
			}

			if (!Directory.Exists (PlatformDirectory))
				throw new MonoTouchException (6, true, "There is no devel platform at {0}, use --platform=PLAT to specify the SDK", PlatformDirectory);

			if (!Directory.Exists (output_dir))
				throw new MonoTouchException (5, true, "The output directory '{0}' does not exist", output_dir);

			if (assemblies.Count != 1) {
				var exceptions = new List<Exception> ();
				for (int i = assemblies.Count - 1; i >= 0; i--) {
					if (assemblies [i].StartsWith ("-")) {
						exceptions.Add (new MonoTouchException (18, true, "Unknown command line argument: '{0}'", assemblies [i]));
						assemblies.RemoveAt (i);
					}
				}
				if (assemblies.Count > 1) {
					exceptions.Add (new MonoTouchException (8, true, "You should provide one root assembly only, found {0} assemblies: '{1}'", assemblies.Count, string.Join ("', '", assemblies.ToArray ())));
				} else if (assemblies.Count == 0) {
					exceptions.Add (new MonoTouchException (17, true, "You should provide a root assembly."));
				}

				throw new AggregateException (exceptions);
			}

			if (app.EnableRepl && app.BuildTarget != BuildTarget.Simulator)
				throw new MonoTouchException (29, true, "REPL (--enable-repl) is only supported in the simulator (--sim)");

			if (app.EnableRepl && app.LinkMode != LinkMode.None)
				throw new MonoTouchException (82, true, "REPL (--enable-repl) is only supported when linking is not used (--nolink).");

			if (cross_prefix == null)
				cross_prefix = MonoTouchDirectory;

			Watch ("Setup", 1);

			app.RootAssembly = assemblies [0];
			app.AppDirectory = output_dir;
			if (action == Action.RunRegistrar) {
				app.RunRegistrar ();
			} else {
				app.Build ();
			}

			return 0;
		}

		static void RedirectStream (StreamReader @in, StreamWriter @out)
		{
			new Thread (() =>
			{
				string line;
				while ((line = @in.ReadLine ()) != null) {
					@out.WriteLine (line);
					@out.Flush ();
				}
			})
			{ IsBackground = true }.Start ();
		}

		static string MlaunchPath {
			get {
				// check next to mtouch first
				var path = Path.Combine (MonoTouchDirectory, "bin", "mlaunch");
				if (File.Exists (path))
					return path;

				// check inside XS
				path = "/Applications/Xamarin Studio.app/Contents/Resources/lib/monodevelop/AddIns/MonoDevelop.IPhone/mlaunch.app/Contents/MacOS/mlaunch";
				if (File.Exists (path))
					return path;

				// check an environment variable
				path = Environment.GetEnvironmentVariable ("MLAUNCH_PATH");
				if (File.Exists (path))
					return path;

				throw ErrorHelper.CreateError (92, "Could not find 'mlaunch'.");
			}
		}

		static int CallMlaunch ()
		{
			Log (1, "Forwarding to mlaunch");
			using (var p = new Process ()) {
				p.StartInfo.UseShellExecute = false;
				p.StartInfo.RedirectStandardError = true;
				p.StartInfo.RedirectStandardInput = true;
				p.StartInfo.RedirectStandardOutput = true;
				p.StartInfo.FileName = MlaunchPath;

				var sb = new StringBuilder ();
				foreach (var str in Environment.GetCommandLineArgs ().Skip (1)) {
					if (sb.Length > 0)
						sb.Append (' ');
					sb.Append (Quote (str));
				}
				p.StartInfo.Arguments = sb.ToString ();
				p.Start ();

				RedirectStream (new StreamReader (Console.OpenStandardInput ()), p.StandardInput);
				RedirectStream (p.StandardOutput, new StreamWriter (Console.OpenStandardOutput ()));
				RedirectStream (p.StandardError, new StreamWriter (Console.OpenStandardError ()));

				p.WaitForExit ();

				GC.Collect (); // Workaround for: https://bugzilla.xamarin.com/show_bug.cgi?id=43462#c14

				return p.ExitCode;
			}
		}

		static string FindGcc (bool gpp)
		{
			var usr_bin = Path.Combine (PlatformsDirectory, Platform + ".platform", "Developer", "usr", "bin");
			var gcc = (gpp ? "g++" : "gcc");
			var compiler_path = Path.Combine (usr_bin, gcc + "-4.2");

			if (!File.Exists (compiler_path)) {
				// latest iOS5 beta (6+) do not ship with a gcc-4.2 symlink - see bug #346
				compiler_path = Path.Combine (usr_bin, gcc);
			}

			return compiler_path;
		}

		public static void CalculateCompilerPath ()
		{
			var fallback_to_clang = false;
			var original_compiler = string.Empty;

			//
			// The default is gcc, but if we can't find gcc,
			// we try again with clang.
			//

			if (string.IsNullOrEmpty (compiler)) {
				// by default we use `gcc` before iOS7 SDK, falling back to `clang`. Otherwise we go directly to `clang`
				// so we don't get bite by the fact that Xcode5 has a gcc compiler (which calls `clang`, even if not 100% 
				// compitable wrt options) for the simulator but not for devices!
				// ref: https://bugzilla.xamarin.com/show_bug.cgi?id=13838
				if (app.Platform == ApplePlatform.iOS) {
					fallback_to_clang = sdk_version < new Version (7, 0);
				} else {
					fallback_to_clang = false;
				}
				if (fallback_to_clang)
					compiler = app.EnableCxx ? "g++" : "gcc";
				else
					compiler = app.EnableCxx ? "clang++" : "clang";
			}

		tryagain:
			switch (compiler) {
			case "clang++":
				compiler_path = Path.Combine (DeveloperDirectory, "Toolchains", "XcodeDefault.xctoolchain", "usr", "bin", "clang++");
				break;
			case "clang":
				compiler_path = Path.Combine (DeveloperDirectory, "Toolchains", "XcodeDefault.xctoolchain", "usr", "bin", "clang");
				break;
			case "gcc":
				compiler_path = FindGcc (false);
				break;
			case "g++":
				compiler_path = FindGcc (true);
				break;
			default: // This is the full path to a compiler.
				compiler_path = compiler;
				break;
			}

			if (!File.Exists (compiler_path)) {
				if (fallback_to_clang) {
					// Couldn't find gcc, try to find clang.
					original_compiler = compiler;
					compiler = app.EnableCxx ? "clang++" : "clang";
					fallback_to_clang = false;
					goto tryagain;
				}
				if (string.IsNullOrEmpty (original_compiler)) {
					throw new MonoTouchException (5101, true, "Missing '{0}' compiler. Please install Xcode 'Command-Line Tools' component", compiler);
				} else {
					throw new MonoTouchException (5103, true, "Could not find neither the '{0}' nor the '{1}' compiler. Please install Xcode 'Command-Line Tools' component", compiler, original_compiler);
				}
			}
		}

		// workaround issues like:
		// * Xcode 4.x versus 4.3 (location of /Developer); and 
		// * the (optional) installation of "Command-Line Tools" by Xcode
		public static void RunStrip (string options)
		{
			// either /Developer (Xcode 4.2 and earlier), /Applications/Xcode.app/Contents/Developer (Xcode 4.3) or user override
			string strip = FindTool ("strip");
			if (strip == null)
				throw new MonoTouchException (5301, "Missing 'strip' tool. Please install Xcode 'Command-Line Tools' component");

			if (RunCommand (strip, options) != 0)
				throw new MonoTouchException (5304, true, "Failed to strip the final binary. Please review the build log.");
		}

		static string FindTool (string tool)
		{
			// either /Developer (Xcode 4.2 and earlier), /Applications/Xcode.app/Contents/Developer (Xcode 4.3) or user override
			var path = Path.Combine (DeveloperDirectory, "usr", "bin", tool);
			if (File.Exists (path))
				return path;

			// Xcode "Command-Line Tools" install a copy in /usr/bin (and it can be there afterward)
			path = Path.Combine ("/usr", "bin", tool);
			if (File.Exists (path))
				return path;

			// Xcode 4.3 (without command-line tools) also has a copy of 'strip'
			path = Path.Combine (DeveloperDirectory, "Toolchains", "XcodeDefault.xctoolchain", "usr", "bin", tool);
			if (File.Exists (path))
				return path;

			return null;
		}

		public static void CreateDsym (string output_dir, string appname, string dsym_dir)
		{
			string quoted_app_path = Quote (Path.Combine (output_dir, appname));
			string quoted_dsym_dir = Quote (dsym_dir);
			RunDsymUtil (string.Format ("{0} -t 4 -z -o {1}", quoted_app_path, quoted_dsym_dir));
			RunCommand ("/usr/bin/mdimport", quoted_dsym_dir);
		}

		public static void RunLipo (string options)
		{
			string lipo = FindTool ("lipo");
			if (lipo == null)
				throw new MonoTouchException (5305, true, "Missing 'lipo' tool. Please install Xcode 'Command-Line Tools' component");
			if (RunCommand (lipo, options) != 0)
				throw new MonoTouchException (5306, true, "Failed to create the a fat library. Please review the build log.");
		}

		static void RunDsymUtil (string options)
		{
			// either /Developer (Xcode 4.2 and earlier), /Applications/Xcode.app/Contents/Developer (Xcode 4.3) or user override
			string dsymutil = FindTool ("dsymutil");
			if (dsymutil == null) {
				ErrorHelper.Warning (5302, "Missing 'dsymutil' tool. Please install Xcode 'Command-Line Tools' component");
				return;
			}
			if (RunCommand (dsymutil, options) != 0)
				throw new MonoTouchException (5303, true, "Failed to generate the debug symbols (dSYM directory). Please review the build log.");
		}

		static string GetFrameworkDir (string platform, Version iphone_sdk)
		{
			return Path.Combine (PlatformsDirectory, platform + ".platform", "Developer", "SDKs", platform + sdk_version.ToString () + ".sdk");
		}

		static bool IsBoundAssembly (Assembly s)
		{
			if (s.IsFrameworkAssembly)
				return false;

			AssemblyDefinition ad = s.AssemblyDefinition;

			foreach (ModuleDefinition md in ad.Modules)
				foreach (TypeDefinition td in md.Types)
					if (td.IsNSObject ())
						return true;

			return false;
		}

		struct timespec {
			public IntPtr tv_sec;
			public IntPtr tv_nsec;
		}

		struct stat { /* when _DARWIN_FEATURE_64_BIT_INODE is defined */
			public uint st_dev;
			public ushort st_mode;
			public ushort st_nlink;
			public ulong st_ino;
			public uint st_uid;
			public uint st_gid;
			public uint st_rdev;
			public timespec st_atimespec;
			public timespec st_mtimespec;
			public timespec st_ctimespec;
			public timespec st_birthtimespec;
			public ulong st_size;
			public ulong st_blocks;
			public uint st_blksize;
			public uint st_flags;
			public uint st_gen;
			public uint st_lspare;
			public ulong st_qspare_1;
			public ulong st_qspare_2;
		}

		[DllImport (Constants.libSystemLibrary, EntryPoint = "lstat$INODE64", SetLastError = true)]
		static extern int lstat (string path, out stat buf);

		public static bool IsSymlink (string file)
		{
			stat buf;
			var rv = lstat (file, out buf);
			if (rv != 0)
				throw new Exception (string.Format ("Could not lstat '{0}': {1}", file, Marshal.GetLastWin32Error ()));
			const int S_IFLNK = 40960;
			return (buf.st_mode & S_IFLNK) == S_IFLNK;
		}

		public static Frameworks Frameworks {
			get {
				switch (app.Platform) {
				case ApplePlatform.iOS:
					return Frameworks.iOSFrameworks;
				case ApplePlatform.WatchOS:
					return Frameworks.WatchFrameworks;
				case ApplePlatform.TVOS:
					return Frameworks.TVOSFrameworks;
				default:
					throw ErrorHelper.CreateError (71, "Unknown platform: {0}. This usually indicates a bug in Xamarin.iOS; please file a bug report at http://bugzilla.xamarin.com with a test case.", app.Platform);
				}
			}
		}

		public static Version MinOSVersion {
			get {
				return app.DeploymentTarget;
			}
		}
	}
}
