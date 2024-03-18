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
using System.Text;
using System.Threading;

using Mono.Options;

using ObjCRuntime;

using Xamarin.Utils;

public enum OutputFormat {
	Default,
	Xml,
}

namespace Xamarin.Bundler {
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

		//
		// Output generation
		static string dotfile;
		static string cross_prefix = Environment.GetEnvironmentVariable ("MONO_CROSS_PREFIX");
		static string extra_args = Environment.GetEnvironmentVariable ("MTOUCH_ENV_OPTIONS");

		public static string DotFile {
			get {
				return dotfile;
			}
		}

		public static bool IsUnified {
			get { return true; }
		}

		public static string GetArchDirectory (Application app, bool is64bit)
		{
			if (is64bit)
				return GetArch64Directory (app);
			return GetArch32Directory (app);
		}

		public static string GetArch32Directory (Application app)
		{
			switch (app.Platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.WatchOS:
				return Path.Combine (GetPlatformFrameworkDirectory (app), "..", "..", "32bits", app.PlatformName);
			default:
				throw ErrorHelper.CreateError (71, Errors.MX0071, app.Platform, app.ProductName);
			}
		}

		public static string GetArch64Directory (Application app)
		{
			switch (app.Platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
			case ApplePlatform.WatchOS:
			case ApplePlatform.MacCatalyst:
				return Path.Combine (GetPlatformFrameworkDirectory (app), "..", "..", "64bits", app.PlatformName);
			default:
				throw ErrorHelper.CreateError (71, Errors.MX0071, app.Platform, app.ProductName);
			}
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
				throw ErrorHelper.CreateError (71, Errors.MX0071, app.Platform, app.ProductName);
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
				throw ErrorHelper.CreateError (71, Errors.MX0071, app.Platform, app.ProductName);
			}
		}

		public static bool IsUsingClang (Application app)
		{
			return app.CompilerPath.EndsWith ("clang", StringComparison.Ordinal) || app.CompilerPath.EndsWith ("clang++", StringComparison.Ordinal);
		}

		public static string GetAotCompiler (Application app, Abi abi, bool is64bits)
		{
			switch (app.Platform) {
			case ApplePlatform.iOS:
				if (is64bits) {
					return Path.Combine (cross_prefix, "bin", "arm64-darwin-mono-sgen");
				} else {
					return Path.Combine (cross_prefix, "bin", "arm-darwin-mono-sgen");
				}
			case ApplePlatform.WatchOS:
				/* Use arm64_32 cross only for Debug mode */
				if (abi == Abi.ARM64_32 && !app.EnableLLVMOnlyBitCode) {
					return Path.Combine (cross_prefix, "bin", "arm64_32-darwin-mono-sgen");
				} else {
					return Path.Combine (cross_prefix, "bin", "armv7k-unknown-darwin-mono-sgen");
				}
			case ApplePlatform.TVOS:
				return Path.Combine (cross_prefix, "bin", "arm64-darwin-mono-sgen");
			default:
				throw ErrorHelper.CreateError (71, Errors.MX0071, app.Platform, app.ProductName);
			}
		}

		public static void CopyAssembly (string source, string target, string target_dir = null)
		{
			if (File.Exists (target))
				File.Delete (target);
			if (target_dir is null)
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
			if (!PathUtils.Symlink (source, target))
				return false;

			string sconfig = source + ".config";
			string tconfig = target + ".config";
			if (File.Exists (tconfig))
				File.Delete (tconfig);
			if (File.Exists (sconfig))
				PathUtils.Symlink (sconfig, tconfig);

			string tdebug = target + ".mdb";
			if (File.Exists (tdebug))
				File.Delete (tdebug);
			string sdebug = source + ".mdb";
			if (File.Exists (sdebug))
				PathUtils.Symlink (sdebug, tdebug);

			string tpdb = Path.ChangeExtension (target, "pdb");
			if (File.Exists (tpdb))
				File.Delete (tpdb);

			string spdb = Path.ChangeExtension (source, "pdb");
			if (File.Exists (spdb))
				PathUtils.Symlink (spdb, tpdb);

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
					if (assembly.Frameworks is not null && assembly.Frameworks.Count > 0)
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
			if (app.CustomLinkFlags?.Count > 0)
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

		static Application ParseArguments (string [] args, out Action a)
		{
			var action = Action.None;
			var app = new Application (args);

			a = Action.None;

			if (extra_args is not null) {
				var l = new List<string> (args);
				foreach (var s in extra_args.Split (new char [] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
					l.Add (s);
				args = l.ToArray ();
			}

			Action<Action> SetAction = (Action value) => {
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
					throw new ProductException (19, true, Errors.MT0019);
				}
			};

			var os = new OptionSet () {
			{ "dot:", "Generate a dot file to visualize the build tree.", v => dotfile = v ?? string.Empty },
			{ "aot=", "Arguments to the static compiler",
				v => app.AotArguments.AddRange (v.Split (new char [] { ',' }, StringSplitOptions.RemoveEmptyEntries))
			},
			{ "aot-options=", "Non AOT arguments to the static compiler",
				v => {
					if (v.Contains ("--profile") || v.Contains ("--attach"))
						throw new Exception ("Unsupported flag to -aot-options");
					if (!StringUtils.TryParseArguments (v, out var aot_options, out var ex))
						throw ErrorHelper.CreateError (26, ex, Errors.MX0026, "-aot-options="+v, ex.Message);
					if (app.AotOtherArguments is null)
						app.AotOtherArguments = new List<string> ();
					app.AotOtherArguments.AddRange (aot_options);
				}
			},
			{ "gsharedvt:", "Generic sharing for value-types - always enabled [Deprecated]", v => {} },
			{ "time", v => WatchLevel++ },
			{ "executable=", "Specifies the native executable name to output", v => app.ExecutableName = v },
			{ "nofastsim", "Do not run the simulator fast-path build", v => app.NoFastSim = true },
			{ "nodevcodeshare", "Do not share native code between extensions and main app.", v => app.NoDevCodeShare = true },
			{ "nodebugtrack", "Disable debug tracking of object resurrection bugs [DEPRECATED - already disabled by default]", v => app.DebugTrack = false, true },
			{ "linkerdumpdependencies", "Dump linker dependencies for linker-analyzer tool", v => app.LinkerDumpDependencies = true },
			{ "nolinkaway", "Disable the linker step which replace code with a 'Linked Away' exception.", v => app.LinkAway = false },
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
			{ "r|ref=", "Add an assembly to the resolver [DEPRECATED, use --reference instead]", v => app.References.Add (v), true /* Hide: this option is deprecated in favor of the shared --reference option instead */ },
			{ "gcc_flags=", "Set flags to be passed along to gcc at link time", v =>
				{
					app.ParseCustomLinkFlags (v, "gcc_flags");
				}
			},
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
			{ "sgen:", "Enable the SGen garbage collector",
					v => {
						if (!ParseBool (v, "sgen"))
							ErrorHelper.Warning (43, Errors.MX0043);
					},
					true // do not show the option anymore
				},
			{ "boehm:", "Enable the Boehm garbage collector",
					v => {
						if (ParseBool (v, "boehm"))
							ErrorHelper.Warning (43, Errors.MX0043); },
					true // do not show the option anymore
				},
			{ "new-refcount:", "Enable new refcounting logic",
				v => {
					if (!ParseBool (v, "new-refcount"))
						ErrorHelper.Warning (80, Errors.MX0080);
				},
				true // do not show the option anymore
			},
			{ "override-abi=", "Override any previous abi. Only used for testing.", v => { app.ClearAbi (); app.ParseAbi (v); }, true }, // Temporary command line arg until XS has better support for 64bit architectures.
			{ "cxx", "Enable C++ support", v => { app.EnableCxx = true; }},
			{ "enable-repl:", "Enable REPL support. For simulator only and disabling linking is recommended.", v => { app.EnableRepl = ParseBool (v, "enable-repl"); } },
			{ "pie:", "Enable (default) or disable PIE (Position Independent Executable).", v => { app.EnablePie = ParseBool (v, "pie"); }},
			{ "compiler=", "Specify the Objective-C compiler to use (valid values are gcc, g++, clang, clang++ or the full path to a GCC-compatible compiler).", v => { app.Compiler = v; }},
			{ "fastdev", "Build an app that supports fastdev (this app will only work when launched using Xamarin Studio)", v => { app.AddAssemblyBuildTarget ("@all=dynamiclibrary"); }},
			{ "force-thread-check", "Keep UI thread checks inside (even release) builds [DEPRECATED, use --optimize=-remove-uithread-checks instead]", v => { app.Optimizations.RemoveUIThreadChecks = false; }, true},
			{ "disable-thread-check", "Remove UI thread checks inside (even debug) builds [DEPRECATED, use --optimize=remove-uithread-checks instead]", v => { app.Optimizations.RemoveUIThreadChecks = true; }, true},
			{ "package-mdb:", "Specify whether debug info files (*.mdb) should be packaged in the app. Default is 'true' for debug builds and 'false' for release builds. [DEPRECATED]", v => app.PackageManagedDebugSymbols = ParseBool (v, "package-mdb"), true },
			{ "msym:", "Specify whether managed symbolication files (*.msym) should be created. Default is 'false' for debug builds and 'true' for release builds.", v => app.EnableMSym = ParseBool (v, "msym") },
			{ "extension", v => app.IsExtension = true },
			{ "app-extension=", "The path of app extensions that are included in the app. This must be specified once for each app extension.", v => app.Extensions.Add (v), true /* MSBuild-internal for now */ },
			{ "stderr=", "Redirect the standard error for the simulated application to the specified file [DEPRECATED]", v => { }, true },
			{ "stdout=", "Redirect the standard output for the simulated application to the specified file [DEPRECATED]", v => { }, true },

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
							throw new ProductException (20, true, Errors.MX0020, "--mono", "static, framework or [no-]package-framework");
						}
					}
				}
			},
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
						throw new ProductException (20, true, Errors.MX0020, "--bitcode", "asmonly, full or marker");
					}
				}
			},
			{ "llvm-asm", "Make the LLVM compiler emit assembly files instead of object files. [Deprecated]", v => { app.LLVMAsmWriter = true; }, true},
			{ "llvm-opt=", "Specify how to optimize the LLVM output (only applicable when using LLVM to compile to bitcode), per assembly: 'assembly'='optimizations', where 'assembly is the filename (including extension) of the assembly (the special value 'all' can be passed to set the same optimization for all assemblies), and 'optimizations' are optimization arguments. Valid optimization flags are Clang optimization flags.", v =>
				{
						var equals = v.IndexOf ('=');
						if (equals == -1)
							throw ErrorHelper.CreateError (26, Errors.MX0026, "-llvm-opt=" + v, "Both assembly and optimization must be specified (assembly=optimization)");
						var asm = v.Substring (0, equals);
						var opt = v.Substring (equals + 1); // An empty string is valid here, meaning 'no optimizations'
						app.LLVMOptimizations [asm] = opt;
				}
			},
			{ "interpreter:", "Enable the *experimental* interpreter. Optionally takes a comma-separated list of assemblies to interpret (if prefixed with a minus sign, the assembly will be AOT-compiled instead). 'all' can be used to specify all assemblies. This argument can be specified multiple times.", v =>
				{
					app.ParseInterpreter (v);
				}
			},
			{ "output-format=", "Specify the output format for some commands. Possible values: Default, XML", v =>
				{
				}
			},
			{ "xamarin-framework-directory=", "The framework directory", v => { framework_dir = v; }, true },
			{ "assembly-build-target=", "Specifies how to compile assemblies to native code. Possible values: 'staticobject' (default), 'dynamiclibrary' and 'framework'. " +
					"Each option also takes an assembly and a potential name (defaults to the name of the assembly). Example: --assembly-build-target=mscorlib.dll=framework[=name]." +
					"There also two special names: '@all' and '@sdk': --assembly-build-target=@all|@sdk=framework[=name].", v =>
					{
						app.AddAssemblyBuildTarget (v);
					}
			},
			/* Any new options that are identical between mtouch and mmp should be added to common/Driver.cs */
		};

			if (ParseOptions (app, os, args, ref action))
				return null;

			a = action;

			app.SetDefaultAbi ();

			if (action == Action.Build || action == Action.RunRegistrar) {
				if (app.RootAssemblies.Count == 0)
					throw new ProductException (17, true, Errors.MX0017);
			}

			return app;
		}

		static int Main2 (string [] args)
		{
			Action action;
			var app = ParseArguments (args, out action);

			if (app is null)
				return 0;

			// Allow a few actions, since these seem to always work no matter the Xcode version.
			var accept_any_xcode_version = action == Action.ListDevices || action == Action.ListCrashReports || action == Action.ListApps || action == Action.LogDev;
			ValidateXcode (app, accept_any_xcode_version, false);

			if (IsMlaunchAction (action))
				return CallMlaunch (app);

			if (app.SdkVersion is null)
				throw new ProductException (25, true, Errors.MT0025, app.PlatformName);

			var framework_dir = GetFrameworkDirectory (app);
			Driver.Log ("Xamarin.iOS {0}.{1} using framework: {2}", Constants.Version, Constants.Revision, framework_dir);

			if (action == Action.None)
				throw new ProductException (52, true, Errors.MT0052);

			if (app.SdkVersion < new Version (6, 0) && app.IsArchEnabled (Abi.ARMv7s))
				throw new ProductException (14, true, Errors.MT0014, app.SdkVersion, "ARMv7s");

			if (app.SdkVersion < new Version (7, 0) && app.IsArchEnabled (Abi.ARM64))
				throw new ProductException (14, true, Errors.MT0014, app.SdkVersion, "ARM64");

			if (app.SdkVersion < new Version (7, 0) && app.IsArchEnabled (Abi.x86_64))
				throw new ProductException (14, true, Errors.MT0014, app.SdkVersion, "x86_64");

			if (!Directory.Exists (framework_dir)) {
				Console.WriteLine ("Framework does not exist {0}", framework_dir);
				Console.WriteLine ("   Platform = {0}", GetPlatform (app));
				Console.WriteLine ("   SDK = {0}", app.SdkVersion);
				Console.WriteLine ("   Deployment Version: {0}", app.DeploymentTarget);
			}

			if (!Directory.Exists (GetPlatformDirectory (app)))
				throw new ProductException (6, true, Errors.MT0006, GetPlatformDirectory (app));

			if (!Directory.Exists (app.AppDirectory))
				Directory.CreateDirectory (app.AppDirectory);

			if (app.EnableRepl && app.BuildTarget != BuildTarget.Simulator)
				throw new ProductException (29, true, Errors.MT0029);

			if (app.EnableRepl && app.LinkMode != LinkMode.None)
				throw new ProductException (82, true, Errors.MT0082);

			if (cross_prefix is null)
				cross_prefix = GetFrameworkCurrentDirectory (app);

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
							throw ErrorHelper.CreateError (99, Errors.MX0099, $"Extension build action is '{app_action}' when it should be 'Build'");
					}

					app.BuildAll ();
				}
			} else {
				throw ErrorHelper.CreateError (99, Errors.MX0099, action);
			}

			return 0;
		}

		static bool IsMlaunchAction (Action action)
		{
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
				return true;
			}

			return false;
		}

		static void RedirectStream (StreamReader @in, StreamWriter @out)
		{
			new Thread (() => {
				string line;
				while ((line = @in.ReadLine ()) is not null) {
					@out.WriteLine (line);
					@out.Flush ();
				}
			}) { IsBackground = true }.Start ();
		}

		static string GetMlaunchPath (Application app)
		{
			// check next to mtouch first
			var path = Path.Combine (GetFrameworkBinDirectory (app), "mlaunch");
			if (File.Exists (path))
				return path;

			// check an environment variable
			path = Environment.GetEnvironmentVariable ("MLAUNCH_PATH");
			if (File.Exists (path))
				return path;

			throw ErrorHelper.CreateError (93, Errors.MT0093);
		}

		static int CallMlaunch (Application app)
		{
			Log (1, "Forwarding to mlaunch");
			using (var p = new Process ()) {
				p.StartInfo.UseShellExecute = false;
				p.StartInfo.RedirectStandardError = true;
				p.StartInfo.RedirectStandardInput = true;
				p.StartInfo.RedirectStandardOutput = true;
				p.StartInfo.FileName = GetMlaunchPath (app);

				var sb = Environment.GetCommandLineArgs ().Skip (1).ToList ();
				p.StartInfo.Arguments = StringUtils.FormatArguments (sb);
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

			switch (app.Platform) {
			case ApplePlatform.TVOS:
			case ApplePlatform.WatchOS:
				if (Driver.XcodeVersion.Major >= 14)
					app.EnableCxx = true;
				break;
			}

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
					throw new ProductException (5101, true, Errors.MT5101, app.Compiler);
				} else {
					throw new ProductException (5103, true, Errors.MT5103, app.Compiler, original_compiler);
				}
			}
		}
	}
}
