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
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using Mono.Options;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Tuner;

using MonoTouch.Tuner;
using Registrar;
using ObjCRuntime;

using Xamarin.Linker;
using Xamarin.Utils;

using Xamarin.MacDev;

public enum OutputFormat {
	Default,
	Xml,
}

namespace Xamarin.Bundler
{
	public partial class Driver {
		internal const string NAME = "mtouch";

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
			Embeddinator,
		}

		static bool xcode_version_check = true;

		//
		// Output generation
		static bool force = false;
		static bool dot;
		static string cross_prefix = Environment.GetEnvironmentVariable ("MONO_CROSS_PREFIX");
		static string extra_args = Environment.GetEnvironmentVariable ("MTOUCH_ENV_OPTIONS");

		static int verbose = GetDefaultVerbosity ();

		public static bool Dot {
			get {
				return dot;
			}
		}

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

		public static bool IsUnified {
			get { return true; }
		}

		public static bool Force {
			get { return force; }
			set { force = value; }
		}

		public static string GetPlatform (Application app)
		{
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

		public static string GetMonoTouchLibDirectory (Application app)
		{
			return Path.Combine (GetProductSdkDirectory (app), "usr", "lib");
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

		public static string GetPlatformFrameworkDirectory (Application app)
		{
			switch (app.Platform) {
			case ApplePlatform.iOS:
				return Path.Combine (MonoTouchDirectory, "lib", "mono", "Xamarin.iOS");
			case ApplePlatform.WatchOS:
				return Path.Combine (MonoTouchDirectory, "lib", "mono", "Xamarin.WatchOS");
			case ApplePlatform.TVOS:
				return Path.Combine (MonoTouchDirectory, "lib", "mono", "Xamarin.TVOS");
			default:
				throw ErrorHelper.CreateError (71, "Unknown platform: {0}. This usually indicates a bug in Xamarin.iOS; please file a bug report at http://bugzilla.xamarin.com with a test case.", app.Platform);
			}
		}

		public static string GetArch32Directory (Application app)
		{
			switch (app.Platform) {
			case ApplePlatform.iOS:
				return Path.Combine (GetPlatformFrameworkDirectory (app), "..", "..", "32bits");
			default:
				throw ErrorHelper.CreateError (71, "Unknown platform: {0}. This usually indicates a bug in Xamarin.iOS; please file a bug report at http://bugzilla.xamarin.com with a test case.", app.Platform);
			}
		}

		public static string GetArch64Directory (Application app)
		{
			switch (app.Platform) {
			case ApplePlatform.iOS:
				return Path.Combine (GetPlatformFrameworkDirectory (app), "..", "..", "64bits");
			default:
				throw ErrorHelper.CreateError (71, "Unknown platform: {0}. This usually indicates a bug in Xamarin.iOS; please file a bug report at http://bugzilla.xamarin.com with a test case.", app.Platform);
			}
		}

		public static string GetProductSdkDirectory (Application app)
		{
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

		public static string GetProductFrameworksDirectory (Application app)
		{
			return Path.Combine (GetProductSdkDirectory (app), "Frameworks");
		}

		// This is for the -mX-version-min=A.B compiler flag
		public static string GetTargetMinSdkName (Application app)
		{
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

		public static string GetProductAssembly (Application app)
		{
			switch (app.Platform) {
			case ApplePlatform.iOS:
				return "Xamarin.iOS";
			case ApplePlatform.WatchOS:
				return "Xamarin.WatchOS";
			case ApplePlatform.TVOS:
				return "Xamarin.TVOS";
			default:
				throw ErrorHelper.CreateError (71, "Unknown platform: {0}. This usually indicates a bug in Xamarin.iOS; please file a bug report at http://bugzilla.xamarin.com with a test case.", app.Platform);
			}
		}

		public static bool GetLLVMAsmWriter (Application app)
		{
			if (app.LLVMAsmWriter.HasValue)
				return app.LLVMAsmWriter.Value;
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

		static void ValidateXcode (Action action)
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

		public static string GetFrameworkDirectory (Application app)
		{
			return GetFrameworkDir (GetPlatform (app), app.SdkVersion);
		}

		public static bool IsUsingClang (Application app)
		{
			return app.CompilerPath.EndsWith ("clang", StringComparison.Ordinal) || app.CompilerPath.EndsWith ("clang++", StringComparison.Ordinal);
		}

		public static string PlatformsDirectory {
			get {
				return Path.Combine (DeveloperDirectory, "Platforms");
			}
		}

		public static string GetPlatformDirectory (Application app)
		{
			return Path.Combine (PlatformsDirectory, GetPlatform (app) + ".platform");
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

		public static string GetAotCompiler (Application app, bool is64bits)
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
				return Path.Combine (cross_prefix, "bin", "arm64-darwin-mono-sgen");
			default:
				throw ErrorHelper.CreateError (71, "Unknown platform: {0}. This usually indicates a bug in Xamarin.iOS; please file a bug report at http://bugzilla.xamarin.com with a test case.", app.Platform);
			}
		}

		public static string GetAotArguments (Application app, string filename, Abi abi, string outputDir, string outputFile, string llvmOutputFile, string dataFile)
		{
			string fname = Path.GetFileName (filename);
			StringBuilder args = new StringBuilder ();
			bool enable_llvm = (abi & Abi.LLVM) != 0;
			bool enable_thumb = (abi & Abi.Thumb) != 0;
			bool enable_debug = app.EnableDebug;
			bool enable_debug_symbols = app.PackageManagedDebugSymbols;
			bool llvm_only = app.EnableLLVMOnlyBitCode;
			bool interp = app.IsInterpreted (Assembly.GetIdentity (filename));
			bool interp_full = !interp && app.UseInterpreter && fname == "mscorlib.dll";
			bool is32bit = (abi & Abi.Arch32Mask) > 0;
			string arch = abi.AsArchString ();

			args.Append ("--debug ");

			if (enable_llvm)
				args.Append ("--llvm ");

			if (!llvm_only && !interp)
				args.Append ("-O=gsharedvt ");
			args.Append (app.AotOtherArguments).Append (" ");
			args.Append ("--aot=mtriple=");
			args.Append (enable_thumb ? arch.Replace ("arm", "thumb") : arch);
			args.Append ("-ios,");
			args.Append ("data-outfile=").Append (StringUtils.Quote (dataFile)).Append (",");
			args.Append (app.AotArguments);
			if (llvm_only)
				args.Append ("llvmonly,");
			else if (interp) {
				if (fname != "mscorlib.dll")
					throw ErrorHelper.CreateError (99, $"Internal error: can only enable the interpreter for mscorlib.dll when AOT-compiling assemblies (tried to interpret {fname}). Please file an issue at https://github.com/xamarin/xamarin-macios/issues/new.");
				args.Append ("interp,");
			} else if (interp_full) {
				if (fname != "mscorlib.dll")
					throw ErrorHelper.CreateError (99, $"Internal error: can only enable the interpreter for mscorlib.dll when AOT-compiling assemblies (tried to interpret {fname}). Please file an issue at https://github.com/xamarin/xamarin-macios/issues/new."); 
				args.Append ("interp,full,");
			} else
				args.Append ("full,");

			var aname = Path.GetFileNameWithoutExtension (fname);
			var sdk_or_product = Profile.IsSdkAssembly (aname) || Profile.IsProductAssembly (aname);

			if (enable_llvm)
				args.Append ("nodebug,");
			else if (!(enable_debug || enable_debug_symbols))
				args.Append ("nodebug,");
			else if (app.DebugAll || app.DebugAssemblies.Contains (fname) || !sdk_or_product)
				args.Append ("soft-debug,");

			args.Append ("dwarfdebug,");

			/* Needed for #4587 */
			if (enable_debug && !enable_llvm)
				args.Append ("no-direct-calls,");

			if (!app.UseDlsym (filename))
				args.Append ("direct-pinvoke,");

			if (app.EnableMSym) {
				var msymdir = StringUtils.Quote (Path.Combine (outputDir, "Msym"));
				args.Append ($"msym-dir={msymdir},");
			}

			if (enable_llvm)
				args.Append ("llvm-path=").Append (MonoTouchDirectory).Append (is32bit ? "/LLVM36/bin/," : "/LLVM/bin/,");

			if (!llvm_only)
				args.Append ("outfile=").Append (StringUtils.Quote (outputFile));
			if (!llvm_only && enable_llvm)
				args.Append (",");
			if (enable_llvm)
				args.Append ("llvm-outfile=").Append (StringUtils.Quote (llvmOutputFile));
			args.Append (" \"").Append (filename).Append ("\"");
			return args.ToString ();
		}

		public static ProcessStartInfo CreateStartInfo (Application app, string file_name, string arguments, string mono_path, string mono_debug = null)
		{
			var info = new ProcessStartInfo (file_name, arguments);
			info.UseShellExecute = false;
			info.RedirectStandardOutput = true;
			info.RedirectStandardError = true;

			info.EnvironmentVariables ["MONO_PATH"] = mono_path;
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
		public static string GenerateMain (Application app, IEnumerable<Assembly> assemblies, string assembly_name, Abi abi, string main_source, IList<string> registration_methods)
		{
			var assembly_externs = new StringBuilder ();
			var assembly_aot_modules = new StringBuilder ();
			var register_assemblies = new StringBuilder ();
			var assembly_location = new StringBuilder ();
			var assembly_location_count = 0;
			var enable_llvm = (abi & Abi.LLVM) != 0;

			register_assemblies.AppendLine ("\tguint32 exception_gchandle = 0;");
			foreach (var s in assemblies) {
				if (!s.IsAOTCompiled)
					continue;
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

			if ((abi & Abi.SimulatorArchMask) == 0 || app.Embeddinator) {
				var frameworks = assemblies.Where ((a) => a.BuildTarget == AssemblyBuildTarget.Framework)
				                           .OrderBy ((a) => a.Identity, StringComparer.Ordinal);
				foreach (var asm_fw in frameworks) {
					var asm_name = asm_fw.Identity;
					if (asm_fw.BuildTargetName == asm_name)
						continue; // this is deduceable
					var prefix = string.Empty;
					if (!app.HasFrameworksDirectory && asm_fw.IsCodeShared)
						prefix = "../../";
					var suffix = string.Empty;
					if (app.IsSimulatorBuild)
						suffix = "/simulator";
					assembly_location.AppendFormat ("\t{{ \"{0}\", \"{2}Frameworks/{1}.framework/MonoBundle{3}\" }},\n", asm_name, asm_fw.BuildTargetName, prefix, suffix);
					assembly_location_count++;
				}
			}

			try {
				StringBuilder sb = new StringBuilder ();
				using (var sw = new StringWriter (sb)) {
					sw.WriteLine ("#include \"xamarin/xamarin.h\"");

					if (assembly_location.Length > 0) {
						sw.WriteLine ();
						sw.WriteLine ("struct AssemblyLocation assembly_location_entries [] = {");
						sw.WriteLine (assembly_location);
						sw.WriteLine ("};");

						sw.WriteLine ();
						sw.WriteLine ("struct AssemblyLocations assembly_locations = {{ {0}, assembly_location_entries }};", assembly_location_count);
					}
					
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
							sw.Write ("extern \"C\" void ");
							sw.Write (method);
							sw.WriteLine ("();");
						}
					}

					// Burn in a reference to the profiling symbol so that the native linker doesn't remove it
					// On iOS we can pass -u to the native linker, but that doesn't work on tvOS, where
					// we're building with bitcode (even when bitcode is disabled, we still build with the
					// bitcode marker, which makes the linker reject -u).
					if (app.EnableProfiling) {
						sw.WriteLine ("extern \"C\" { void mono_profiler_init_log (); }");
						sw.WriteLine ("typedef void (*xamarin_profiler_symbol_def)();");
						sw.WriteLine ("extern xamarin_profiler_symbol_def xamarin_profiler_symbol;");
						sw.WriteLine ("xamarin_profiler_symbol_def xamarin_profiler_symbol = NULL;");
					}

					if (app.UseInterpreter) {
						sw.WriteLine ("extern \"C\" { void mono_ee_interp_init (const char *); }");
						sw.WriteLine ("extern \"C\" { void mono_icall_table_init (void); }");
						sw.WriteLine ("extern \"C\" { void mono_marshal_ilgen_init (void); }");
						sw.WriteLine ("extern \"C\" { void mono_method_builder_ilgen_init (void); }");
						sw.WriteLine ("extern \"C\" { void mono_sgen_mono_ilgen_init (void); }");
					}

					sw.WriteLine ("void xamarin_setup_impl ()");
					sw.WriteLine ("{");

					if (app.EnableProfiling)
						sw.WriteLine ("\txamarin_profiler_symbol = mono_profiler_init_log;");

					if (app.EnableLLVMOnlyBitCode)
						sw.WriteLine ("\tmono_jit_set_aot_mode (MONO_AOT_MODE_LLVMONLY);");
					else if (app.UseInterpreter) {
						sw.WriteLine ("\tmono_icall_table_init ();");
						sw.WriteLine ("\tmono_marshal_ilgen_init ();");
						sw.WriteLine ("\tmono_method_builder_ilgen_init ();");
						sw.WriteLine ("\tmono_sgen_mono_ilgen_init ();");
						sw.WriteLine ("\tmono_ee_interp_init (NULL);");
						sw.WriteLine ("\tmono_jit_set_aot_mode (MONO_AOT_MODE_INTERP);");
					} else if (app.IsDeviceBuild)
						sw.WriteLine ("\tmono_jit_set_aot_mode (MONO_AOT_MODE_FULL);");

					if (assembly_location.Length > 0)
						sw.WriteLine ("\txamarin_set_assembly_directories (&assembly_locations);");

					if (registration_methods != null) {
						for (int i = 0; i < registration_methods.Count; i++) {
							sw.Write ("\t");
							sw.Write (registration_methods [i]);
							sw.WriteLine ("();");
						}
					}

					if (app.EnableDebug)
						sw.WriteLine ("\txamarin_gc_pump = {0};", app.DebugTrack.Value ? "TRUE" : "FALSE");
					sw.WriteLine ("\txamarin_init_mono_debug = {0};", app.PackageManagedDebugSymbols ? "TRUE" : "FALSE");
					sw.WriteLine ("\txamarin_executable_name = \"{0}\";", assembly_name);
					sw.WriteLine ("\tmono_use_llvm = {0};", enable_llvm ? "TRUE" : "FALSE");
					sw.WriteLine ("\txamarin_log_level = {0};", verbose.ToString (CultureInfo.InvariantCulture));
					sw.WriteLine ("\txamarin_arch_name = \"{0}\";", abi.AsArchString ());
					if (!app.IsDefaultMarshalManagedExceptionMode)
						sw.WriteLine ("\txamarin_marshal_managed_exception_mode = MarshalManagedExceptionMode{0};", app.MarshalManagedExceptions);
					sw.WriteLine ("\txamarin_marshal_objectivec_exception_mode = MarshalObjectiveCExceptionMode{0};", app.MarshalObjectiveCExceptions);
					if (app.EnableDebug)
						sw.WriteLine ("\txamarin_debug_mode = TRUE;");
					if (!string.IsNullOrEmpty (app.MonoGCParams))
						sw.WriteLine ("\tsetenv (\"MONO_GC_PARAMS\", \"{0}\", 1);", app.MonoGCParams);
					foreach (var kvp in app.EnvironmentVariables)
						sw.WriteLine ("\tsetenv (\"{0}\", \"{1}\", 1);", kvp.Key.Replace ("\"", "\\\""), kvp.Value.Replace ("\"", "\\\""));
					sw.WriteLine ("\txamarin_supports_dynamic_registration = {0};", app.DynamicRegistrationSupported ? "TRUE" : "FALSE");
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
						sw.WriteLine ("\targv [0] = (char *) \"{0}\";", Path.GetFileNameWithoutExtension (app.RootAssemblies [0]));
						sw.WriteLine ("\tint rv = xamarin_main (argc, argv, XamarinLaunchModeExtension);");
					} else {
						sw.WriteLine ("\tint rv = xamarin_main (argc, argv, XamarinLaunchModeApp);");
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
				WriteIfDifferent (main_source, sb.ToString (), true);
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

			string tpdb = Path.ChangeExtension (target, "pdb");
			if (File.Exists (tpdb))
				File.Delete (tpdb);

			string spdb = Path.ChangeExtension (source, "pdb");
			if (File.Exists (spdb))
				File.Copy (spdb, tpdb);
		}

		public static bool SymlinkAssembly (Application app, string source, string target, string target_dir)
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

			string tpdb = Path.ChangeExtension (target, "pdb");
			if (File.Exists (tpdb))
				File.Delete (tpdb);

			string spdb = Path.ChangeExtension (source, "pdb");
			if (File.Exists (spdb))
				Symlink (spdb, tpdb);

			return true;
		}

		public static bool CanWeSymlinkTheApplication (Application app)
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
			if (app.NoFastSim)
				return false;

			//Can't symlink if we are running the linker since the assemblies content will change
			if (app.LinkMode != LinkMode.None)
				return false;

			//Custom gcc flags requires us to build template.m
			if (!string.IsNullOrEmpty (app.UserGccFlags))
				return false;

			// Setting environment variables is done in the generated main.m, so we can't symlink in this case.
			if (app.EnvironmentVariables.Count > 0)
				return false;

			// If we are asked to run with concurrent sgen we also need to pass environment variables
			if (app.EnableSGenConc)
				return false;

			if (app.UseInterpreter)
				return false;

			if (app.Registrar == RegistrarMode.Static)
				return false;

			// The default exception marshalling differs between release and debug mode, but we
			// only have one simlauncher, so to use the simlauncher we'd have to chose either
			// debug or release mode. Debug is more frequent, so make that the fast path.
			if (!app.EnableDebug)
				return false;

			if (app.MarshalObjectiveCExceptions != MarshalObjectiveCExceptionMode.UnwindManagedCode)
				return false; // UnwindManagedCode is the default for debug builds.

			if (app.Embeddinator)
				return false;

			return true;
		}

		internal static PDictionary FromPList (string name)
		{
			if (!File.Exists (name))
				throw new MonoTouchException (24, true, "Could not find required file '{0}'.", name);
			return PDictionary.FromFile (name);
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
				SetCurrentLanguage ();
				return Main2 (args);
			}
			catch (Exception e) {
				ErrorHelper.Show (e);
			}
			finally {
				Watch ("Total time", 0);
			}
			return 0;
		}

		static Application ParseArguments (string [] args, out Action a)
		{
			var action = Action.None;
			var app = new Application (args);

			if (extra_args != null) {
				var l = new List<string> (args);
				foreach (var s in extra_args.Split (new char [] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
					l.Add (s);
				args = l.ToArray ();
			}

			string tls_provider = null;
			string http_message_handler = null;

			Action<Action> SetAction = (Action value) =>
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
			};

			var os = new OptionSet () {
			{ "h|?|help", "Displays the help", v => SetAction (Action.Help) },
			{ "version", "Output version information and exit.", v => SetAction (Action.Version) },
			{ "f|force", "Forces the recompilation of code, regardless of timestamps", v=>force = true },
			{ "dot:", "Generate a dot file to visualize the build tree.", v => dot = true },
			{ "cache=", "Specify the directory where object files will be cached", v => app.Cache.Location = v },
			{ "aot=", "Arguments to the static compiler",
				v => app.AotArguments = v + (v.EndsWith (",", StringComparison.Ordinal) ? String.Empty : ",") + app.AotArguments
			},
			{ "aot-options=", "Non AOT arguments to the static compiler",
				v => {
					if (v.Contains ("--profile") || v.Contains ("--attach"))
						throw new Exception ("Unsupported flag to -aot-options");
					app.AotOtherArguments = v + " " + app.AotOtherArguments;
				}
			},
			{ "gsharedvt:", "Generic sharing for value-types - always enabled [Deprecated]", v => {} },
			{ "v", "Verbose", v => verbose++ },
			{ "q", "Quiet", v => verbose-- },
			{ "time", v => WatchLevel++ },
			{ "executable=", "Specifies the native executable name to output", v => app.ExecutableName = v },
			{ "nofastsim", "Do not run the simulator fast-path build", v => app.NoFastSim = true },
			{ "nodevcodeshare", "Do not share native code between extensions and main app.", v => app.NoDevCodeShare = true },
			{ "nolink", "Do not link the assemblies", v => app.LinkMode = LinkMode.None },
			{ "nodebugtrack", "Disable debug tracking of object resurrection bugs", v => app.DebugTrack = false },
			{ "debugtrack:", "Enable debug tracking of object resurrection bugs (enabled by default for the simulator)", v => { app.DebugTrack = ParseBool (v, "--debugtrack"); } },
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
			{ "dlsym:", "Use dlsym to resolve pinvokes in AOT compiled assemblies", v => app.ParseDlsymOptions (v) },
			{ "r|ref=", "Add an assembly to the resolver", v => app.References.Add (v) },
			{ "gcc_flags=", "Set flags to be passed along to gcc at link time", v => app.UserGccFlags = v },
			{ "framework=", "Link with the specified framework. This can either be a system framework (like 'UIKit'), or it can be a path to a custom framework ('/path/to/My.framework'). In the latter case the entire 'My.framework' directory is copied into the app as well.", (v) => app.Frameworks.Add (v) },
			{ "weak-framework=", "Weak link with the specified framework. This can either be a system framework (like 'UIKit'), or it can be a path to a custom framework ('/path/to/My.framework'). In the latter case the entire 'My.framework' directory is copied into the app as well.", (v) => app.Frameworks.Add (v) },	

			// What we do
			{ "sim=", "Compile for the Simulator, specify the output directory for code", v =>
				{
					SetAction (Action.Build);
					app.AppDirectory = v;
					app.BuildTarget = BuildTarget.Simulator;
				}
			},
			{ "dev=", "Compile for the Device, specify the output directory for the code", v => {
					SetAction (Action.Build);
					app.AppDirectory = v;
					app.BuildTarget = BuildTarget.Device;
				}
			},
			// Configures the tooling used to build code.
			{ "sdk=", "Specifies the name of the SDK to compile against (version, for example \"3.2\")",
				v => {
					try {
						app.SdkVersion = StringUtils.ParseVersion (v);
					} catch (Exception ex) {
						ErrorHelper.Error (26, ex, $"Could not parse the command line argument '-sdk:{v}': {ex.Message}");
					}
				}
			},
			{ "targetver=", "Specifies the name of the minimum deployment target (version, for example \"" + Xamarin.SdkVersions.iOS.ToString () + "\")",
				v => {
					try {
						app.DeploymentTarget = StringUtils.ParseVersion (v);
					} catch (Exception ex) {
						throw new MonoTouchException (26, true, ex,  $"Could not parse the command line argument '-targetver:{v}': {ex.Message}");
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
			{ "wait-for-exit:", "If mtouch should wait until the launched app exits. [DEPRECATED]", v => { }, true },
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
					app.EnvironmentVariables.Add (name, value);
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
			{ "abi=", "Comma-separated list of ABIs to target. Currently supported: armv7, armv7+llvm, armv7+llvm+thumb2, armv7s, armv7s+llvm, armv7s+llvm+thumb2, arm64, arm64+llvm, i386, x86_64", v => app.ParseAbi (v) },
			{ "override-abi=", "Override any previous abi. Only used for testing.", v => { app.ClearAbi (); app.ParseAbi (v); }, true }, // Temporary command line arg until XS has better support for 64bit architectures.
			{ "cxx", "Enable C++ support", v => { app.EnableCxx = true; }},
			{ "enable-repl:", "Enable REPL support (simulator and not linking only)", v => { app.EnableRepl = ParseBool (v, "enable-repl"); }, true /* this is a hidden option until we've actually used it and made sure it works as expected */ },
			{ "pie:", "Enable (default) or disable PIE (Position Independent Executable).", v => { app.EnablePie = ParseBool (v, "pie"); }},
			{ "compiler=", "Specify the Objective-C compiler to use (valid values are gcc, g++, clang, clang++ or the full path to a GCC-compatible compiler).", v => { app.Compiler = v; }},
			{ "fastdev", "Build an app that supports fastdev (this app will only work when launched using Xamarin Studio)", v => { app.AddAssemblyBuildTarget ("@all=dynamiclibrary"); }},
			{ "force-thread-check", "Keep UI thread checks inside (even release) builds [DEPRECATED, use --optimize=-remove-uithread-checks instead]", v => { app.Optimizations.RemoveUIThreadChecks = false; }, true},
			{ "disable-thread-check", "Remove UI thread checks inside (even debug) builds [DEPRECATED, use --optimize=remove-uithread-checks instead]", v => { app.Optimizations.RemoveUIThreadChecks = true; }, true},
			{ "debug:", "Generate debug code in Mono for the specified assembly (set to 'all' to generate debug code for all assemblies, the default is to generate debug code for user assemblies only)",
				v => {
					app.EnableDebug = true;
					if (v != null){
						if (v == "all"){
							app.DebugAll = true;
							return;
						}
						app.DebugAssemblies.Add (Path.GetFileName (v));
					}
				}
			},
			{ "package-mdb:", "Specify whether debug info files (*.mdb) should be packaged in the app. Default is 'true' for debug builds and 'false' for release builds.", v => app.PackageManagedDebugSymbols = ParseBool (v, "package-mdb"), true },
			{ "package-debug-symbols:", "Specify whether debug info files (*.mdb / *.pdb) should be packaged in the app. Default is 'true' for debug builds and 'false' for release builds.", v => app.PackageManagedDebugSymbols = ParseBool (v, "package-debug-symbols") },
			{ "msym:", "Specify whether managed symbolication files (*.msym) should be created. Default is 'false' for debug builds and 'true' for release builds.", v => app.EnableMSym = ParseBool (v, "msym") },
			{ "extension", v => app.IsExtension = true },
			{ "app-extension=", "The path of app extensions that are included in the app. This must be specified once for each app extension.", v => app.Extensions.Add (v), true /* MSBuild-internal for now */ },
			{ "profiling:", "Enable profiling", v => app.EnableProfiling = ParseBool (v, "profiling") },
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
			{ "stderr=", "Redirect the standard error for the simulated application to the specified file [DEPRECATED]", v => { }, true },
			{ "stdout=", "Redirect the standard output for the simulated application to the specified file [DEPRECATED]", v => { }, true },
			{ "sdkroot=", "Specify the location of Apple SDKs, default to 'xcode-select' value.", v => sdk_root = v },

			{ "no-xcode-version-check", "Ignores the Xcode version check.", v => { xcode_version_check = false; }, true /* This is a non-documented option. Please discuss any customers running into the xcode version check on the maciosdev@ list before giving this option out to customers. */ },
			{ "mono:", "Comma-separated list of options for how the Mono runtime should be included. Possible values: 'static' (link statically), 'framework' (linked as a user framework), '[no-]package-framework' (if the Mono.framework should be copied to the app bundle or not. The default value is 'framework' for extensions, and main apps if the app targets iOS 8.0 or later and contains extensions, otherwise 'static'. The Mono.framework will be copied to the app bundle if mtouch detects it's needed, but this may be overridden if the default values for 'framework' vs 'static' is overwridden.", v =>
				{
					foreach (var opt in v.Split (new char [] { ',' })) {
						switch (opt) {
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
			{ "llvm-asm", "Make the LLVM compiler emit assembly files instead of object files. [Deprecated]", v => { app.LLVMAsmWriter = true; }, true},
			{ "llvm-opt=", "Specify how to optimize the LLVM output (only applicable when using LLVM to compile to bitcode), per assembly: 'assembly'='optimizations', where 'assembly is the filename (including extension) of the assembly (the special value 'all' can be passed to set the same optimization for all assemblies), and 'optimizations' are optimization arguments. Valid optimization flags are Clang optimization flags.", v =>
				{
						var equals = v.IndexOf ('=');
						if (equals == -1)
							throw ErrorHelper.CreateError (26, "Could not parse the command line argument '{0}': {1}", "--llvm-opt=" + v, "Both assembly and optimization must be specified (assembly=optimization)");
						var asm = v.Substring (0, equals);
						var opt = v.Substring (equals + 1); // An empty string is valid here, meaning 'no optimizations'
						app.LLVMOptimizations [asm] = opt;
				}
			},
			{ "interpreter:", "Enable the *experimental* interpreter. Optionally takes a comma-separated list of assemblies to interpret (if prefixed with a minus sign, the assembly will be AOT-compiled instead). 'all' can be used to specify all assemblies. This argument can be specified multiple times.", v =>
				{ 
					app.UseInterpreter = true;
					if (!string.IsNullOrEmpty (v)) {
						app.InterpretedAssemblies.AddRange (v.Split (new char [] { ',' }, StringSplitOptions.RemoveEmptyEntries));
					}
				}
			},
			{ "http-message-handler=", "Specify the default HTTP message handler for HttpClient", v => { http_message_handler = v; }},
			{ "output-format=", "Specify the output format for some commands. Possible values: Default, XML", v =>
				{
				}
			},
			{ "tls-provider=", "Specify the default TLS provider", v => { tls_provider = v; }},
			{ "xamarin-framework-directory=", "The framework directory", v => { mtouch_dir = v; }, true },
			{ "assembly-build-target=", "Specifies how to compile assemblies to native code. Possible values: 'staticobject' (default), 'dynamiclibrary' and 'framework'. " +
					"Each option also takes an assembly and a potential name (defaults to the name of the assembly). Example: --assembly-build-target=mscorlib.dll=framework[=name]." +
					"There also two special names: '@all' and '@sdk': --assembly-build-target=@all|@sdk=framework[=name].", v =>
					{
						app.AddAssemblyBuildTarget (v);
					}
			},
		};

			AddSharedOptions (app, os);

			try {
				app.RootAssemblies.AddRange (os.Parse (args));
			}
			catch (MonoTouchException) {
				throw;
			}
			catch (Exception e) {
				throw new MonoTouchException (10, true, e, "Could not parse the command line arguments: {0}", e);
			}

			a = action;

			if (action == Action.Help) {
				ShowHelp (os);
				return null;
			} else if (action == Action.Version) {
				Console.Write ("mtouch {0}.{1}", Constants.Version, Constants.Revision);
				Console.WriteLine ();
				return null;
			}

			app.SetDefaultFramework ();
			app.SetDefaultAbi ();

			app.RuntimeOptions = RuntimeOptions.Create (app, http_message_handler, tls_provider);

			if (action == Action.Build || action == Action.RunRegistrar) {
				if (app.RootAssemblies.Count == 0)
					throw new MonoTouchException (17, true, "You should provide a root assembly.");
			}

			return app;
		}

		static int Main2 (string[] args)
		{
			Action action;
			var app = ParseArguments (args, out action);

			if (app == null)
				return 0;
			
			LogArguments (args);

			ErrorHelper.Verbosity = verbose;

			ValidateXcode (action);

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

			if (app.SdkVersion == null)
				throw new MonoTouchException (25, true, "No SDK version was provided. Please add --sdk=X.Y to specify which {0} SDK should be used to build your application.", app.PlatformName);

			var framework_dir = GetFrameworkDirectory (app);
			Driver.Log ("Xamarin.iOS {0}.{1} using framework: {2}", Constants.Version, Constants.Revision, framework_dir);

			if (action == Action.None)
				throw new MonoTouchException (52, true, "No command specified.");

			if (app.SdkVersion < new Version (6, 0) && app.IsArchEnabled (Abi.ARMv7s))
				throw new MonoTouchException (14, true, "The iOS {0} SDK does not support building applications targeting {1}", app.SdkVersion, "ARMv7s");

			if (app.SdkVersion < new Version (7, 0) && app.IsArchEnabled (Abi.ARM64))
				throw new MonoTouchException (14, true, "The iOS {0} SDK does not support building applications targeting {1}", app.SdkVersion, "ARM64");

			if (app.SdkVersion < new Version (7, 0) && app.IsArchEnabled (Abi.x86_64))
				throw new MonoTouchException (14, true, "The iOS {0} SDK does not support building applications targeting {1}", app.SdkVersion, "x86_64");

			if (!Directory.Exists (framework_dir)) {
				Console.WriteLine ("Framework does not exist {0}", framework_dir);
				Console.WriteLine ("   Platform = {0}", GetPlatform (app));
				Console.WriteLine ("   SDK = {0}", app.SdkVersion);
				Console.WriteLine ("   Deployment Version: {0}", app.DeploymentTarget);
			}

			if (!Directory.Exists (GetPlatformDirectory (app)))
				throw new MonoTouchException (6, true, "There is no devel platform at {0}, use --platform=PLAT to specify the SDK", GetPlatformDirectory (app));

			if (!Directory.Exists (app.AppDirectory))
				Directory.CreateDirectory (app.AppDirectory);

			if (app.EnableRepl && app.BuildTarget != BuildTarget.Simulator)
				throw new MonoTouchException (29, true, "REPL (--enable-repl) is only supported in the simulator (--sim)");

			if (app.EnableRepl && app.LinkMode != LinkMode.None)
				throw new MonoTouchException (82, true, "REPL (--enable-repl) is only supported when linking is not used (--nolink).");

			if (cross_prefix == null)
				cross_prefix = MonoTouchDirectory;

			Watch ("Setup", 1);

			if (action == Action.RunRegistrar) {
				app.RunRegistrar ();
			} else if (action == Action.Build) {
				if (app.IsExtension && !app.IsWatchExtension) {
					var sb = new StringBuilder ();
					foreach (var arg in args)
						sb.AppendLine (arg);
					File.WriteAllText (Path.Combine (Path.GetDirectoryName (app.AppDirectory), "build-arguments.txt"), sb.ToString ());
				} else {
					foreach (var appex in app.Extensions) {
						var f_path = Path.Combine (appex, "..", "build-arguments.txt");
						if (!File.Exists (f_path))
							continue;
						Action app_action;
						var ext = ParseArguments (File.ReadAllLines (f_path), out app_action);
						ext.ContainerApp = app;
						app.AppExtensions.Add (ext);
						if (app_action != Action.Build)
							throw ErrorHelper.CreateError (99, "Internal error: Extension build action is '{0}' when it should be 'Build'. Please file a bug report with a test case (http://bugzilla.xamarin.com).", app_action);
					}

					app.BuildAll ();
				}
			} else {
				throw ErrorHelper.CreateError (99, "Internal error: Invalid action: {0}. Please file a bug report with a test case (http://bugzilla.xamarin.com).", action);
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

				// check an environment variable
				path = Environment.GetEnvironmentVariable ("MLAUNCH_PATH");
				if (File.Exists (path))
					return path;

				throw ErrorHelper.CreateError (93, "Could not find 'mlaunch'.");
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
					sb.Append (StringUtils.Quote (str));
				}
				p.StartInfo.Arguments = sb.ToString ();
				p.Start ();

				RedirectStream (new StreamReader (Console.OpenStandardInput ()), p.StandardInput);
				RedirectStream (p.StandardOutput, new StreamWriter (Console.OpenStandardOutput ()));
				RedirectStream (p.StandardError, new StreamWriter (Console.OpenStandardError ()));

				p.WaitForExit ();

				return p.ExitCode;
			}
		}

		static string FindGcc (Application app, bool gpp)
		{
			var usr_bin = Path.Combine (GetPlatformDirectory (app), GetPlatform (app) + ".platform", "Developer", "usr", "bin");
			var gcc = (gpp ? "g++" : "gcc");
			var compiler_path = Path.Combine (usr_bin, gcc + "-4.2");

			if (!File.Exists (compiler_path)) {
				// latest iOS5 beta (6+) do not ship with a gcc-4.2 symlink - see bug #346
				compiler_path = Path.Combine (usr_bin, gcc);
			}

			return compiler_path;
		}

		public static void CalculateCompilerPath (Application app)
		{
			var fallback_to_clang = false;
			var original_compiler = string.Empty;

			//
			// The default is gcc, but if we can't find gcc,
			// we try again with clang.
			//

			if (string.IsNullOrEmpty (app.Compiler)) {
				// by default we use `gcc` before iOS7 SDK, falling back to `clang`. Otherwise we go directly to `clang`
				// so we don't get bite by the fact that Xcode5 has a gcc compiler (which calls `clang`, even if not 100% 
				// compitable wrt options) for the simulator but not for devices!
				// ref: https://bugzilla.xamarin.com/show_bug.cgi?id=13838
				if (app.Platform == ApplePlatform.iOS) {
					fallback_to_clang = app.SdkVersion < new Version (7, 0);
				} else {
					fallback_to_clang = false;
				}
				if (fallback_to_clang)
					app.Compiler = app.EnableCxx ? "g++" : "gcc";
				else
					app.Compiler = app.EnableCxx ? "clang++" : "clang";
			}

		tryagain:
			switch (app.Compiler) {
			case "clang++":
				app.CompilerPath = Path.Combine (DeveloperDirectory, "Toolchains", "XcodeDefault.xctoolchain", "usr", "bin", "clang++");
				break;
			case "clang":
				app.CompilerPath = Path.Combine (DeveloperDirectory, "Toolchains", "XcodeDefault.xctoolchain", "usr", "bin", "clang");
				break;
			case "gcc":
				app.CompilerPath = FindGcc (app, false);
				break;
			case "g++":
				app.CompilerPath = FindGcc (app, true);
				break;
			default: // This is the full path to a compiler.
				app.CompilerPath = app.Compiler;
				break;
			}

			if (!File.Exists (app.CompilerPath)) {
				if (fallback_to_clang) {
					// Couldn't find gcc, try to find clang.
					original_compiler = app.Compiler;
					app.Compiler = app.EnableCxx ? "clang++" : "clang";
					fallback_to_clang = false;
					goto tryagain;
				}
				if (string.IsNullOrEmpty (original_compiler)) {
					throw new MonoTouchException (5101, true, "Missing '{0}' compiler. Please install Xcode 'Command-Line Tools' component", app.Compiler);
				} else {
					throw new MonoTouchException (5103, true, "Could not find neither the '{0}' nor the '{1}' compiler. Please install Xcode 'Command-Line Tools' component", app.Compiler, original_compiler);
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
			string quoted_app_path = StringUtils.Quote (Path.Combine (output_dir, appname));
			string quoted_dsym_dir = StringUtils.Quote (dsym_dir);
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
			return Path.Combine (PlatformsDirectory, platform + ".platform", "Developer", "SDKs", platform + iphone_sdk.ToString () + ".sdk");
		}

		static bool IsBoundAssembly (Assembly s)
		{
			if (s.IsFrameworkAssembly)
				return false;

			AssemblyDefinition ad = s.AssemblyDefinition;

			foreach (ModuleDefinition md in ad.Modules)
				foreach (TypeDefinition td in md.Types)
					if (td.IsNSObject (s.Target.LinkContext))
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

		public static Frameworks GetFrameworks (Application app)
		{
			switch (app.Platform) {
			case ApplePlatform.iOS:
				return Frameworks.GetiOSFrameworks (app);
			case ApplePlatform.WatchOS:
				return Frameworks.GetwatchOSFrameworks (app);
			case ApplePlatform.TVOS:
				return Frameworks.TVOSFrameworks;
			default:
				throw ErrorHelper.CreateError (71, "Unknown platform: {0}. This usually indicates a bug in Xamarin.iOS; please file a bug report at http://bugzilla.xamarin.com with a test case.", app.Platform);
			}
		}

		public static bool IsFrameworkAvailableInSimulator (Application app, string framework)
		{
			if (!GetFrameworks (app).TryGetValue (framework, out var fw))
				return true; // Unknown framework, assume it's valid for the simulator

			if (fw.VersionAvailableInSimulator == null)
				return false;

			if (fw.VersionAvailableInSimulator > app.SdkVersion)
				return false;

			return true;
		}
	}
}
