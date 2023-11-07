/*
 * Copyright 2014 Xamarin Inc. All rights reserved.
 *
 * Authors:
 *   Rolf Bjarne Kvinge <rolf@xamarin.com>
 *
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.MacDev;
using Xamarin.Utils;
using ObjCRuntime;

using Mono.Linker;

namespace Xamarin.Bundler {
	public partial class Driver {

		public static bool Force { get; set; }

		public static bool IsUnifiedFullXamMacFramework { get { return TargetFramework == TargetFramework.Xamarin_Mac_4_5_Full; } }
		public static bool IsUnifiedFullSystemFramework { get { return TargetFramework == TargetFramework.Xamarin_Mac_4_5_System; } }
		public static bool IsUnifiedMobile { get { return TargetFramework == TargetFramework.Xamarin_Mac_2_0_Mobile; } }

#if MMP
		// We know that Xamarin.Mac apps won't compile unless the developer is using Xcode 12+: https://github.com/xamarin/xamarin-macios/issues/11937, so just set that as the min Xcode version.
		static Version min_xcode_version = new Version (12, 0);
#else
		static Version min_xcode_version = new Version (6, 0);
#endif

#if !NET
		public static int Main (string [] args)
		{
			try {
#if MMP
				ErrorHelper.Platform = ApplePlatform.MacOSX;
#else
				ErrorHelper.Platform = ApplePlatform.iOS;
#endif
				Console.OutputEncoding = new UTF8Encoding (false, false);
				SetCurrentLanguage ();
				return Main2 (args);
			} catch (Exception e) {
				ErrorHelper.Show (e);
			} finally {
				Watch ("Total time", 0);
			}
			return 0;
		}

		// Returns true if the process should exit (with a 0 exit code; failures are propagated using exceptions)
		static bool ParseOptions (Application app, Mono.Options.OptionSet options, string [] args, ref Action action)
		{
			Action a = Action.None; // Need a temporary local variable, since anonymous functions can't write directly to ref/out arguments.

			List<string> optimize = null;

			options.Add ("h|?|help", "Displays the help.", v => a = Action.Help);
			options.Add ("f|force", "Forces the recompilation of code, regardless of timestamps.", v => Force = true);
			options.Add ("cache=", "Specify the directory where temporary build files will be cached.", v => app.Cache.Location = v);
			options.Add ("version", "Output version information and exit.", v => a = Action.Version);
			options.Add ("v|verbose", "Specify how verbose the output should be. This can be passed multiple times to increase the verbosity.", v => Verbosity++);
			options.Add ("q|quiet", "Specify how quiet the output should be. This can be passed multiple times to increase the silence.", v => Verbosity--);
			options.Add ("debug:", "Build a debug app. If AOT-compiling, will also generate native debug code for the specified assembly (set to 'all' to generate debug code for all assemblies, the default is to generate debug code for user assemblies only).", v => {
				app.EnableDebug = true;
				if (v is not null) {
					if (v == "all") {
						app.DebugAll = true;
						return;
					}
					app.DebugAssemblies.Add (Path.GetFileName (v));
				}
			});
			options.Add ("reference=", "Add an assembly to be processed.", v => app.References.Add (v));
			// Unfortunately -r is used in mmp for something else (--resource), which means we can't use the same arguments for both mtouch and mmp.
			// So add --reference, which is now used by both (and accepted by bgen as well), and deprecate -r|--ref for mtouch and -a|--assembly for mmp.
			options.Add ("targetver=", "Minimum supported version of the target OS. For Mac Catalyst, this is the corresponding iOS version", v => {
				try {
					app.DeploymentTarget = StringUtils.ParseVersion (v);
				} catch (Exception ex) {
					throw ErrorHelper.CreateError (26, ex, Errors.MX0026, "targetver:" + v, ex.Message);
				}
			});
			options.Add ("sdkroot=", "Specify the location of Apple SDKs, default to 'xcode-select' value.", v => sdk_root = v);
			options.Add ("sdk=", "Specifies the SDK version to compile against (version, for example \"10.9\"). For Mac Catalyst, this is the macOS version of the SDK.", v => {
				try {
					app.SdkVersion = StringUtils.ParseVersion (v);
					app.NativeSdkVersion = app.SdkVersion;
				} catch (Exception ex) {
					throw ErrorHelper.CreateError (26, ex, Errors.MX0026, $"sdk:{v}", ex.Message);
				}
			});
			options.Add ("target-framework=", "Specify target framework to use. Currently supported: '" + string.Join ("', '", TargetFramework.ValidFrameworks.Select ((v) => v.ToString ())) + "'.", v => SetTargetFramework (v));
#if MMP
			options.Add ("abi=", "Comma-separated list of ABIs to target. x86_64, arm64", v => app.ParseAbi (v));
#else
			options.Add ("abi=", "Comma-separated list of ABIs to target. Currently supported: armv7, armv7+llvm, armv7+llvm+thumb2, armv7s, armv7s+llvm, armv7s+llvm+thumb2, arm64, arm64+llvm, arm64_32, arm64_32+llvm, i386, x86_64.", v => app.ParseAbi (v));
#endif
			options.Add ("no-xcode-version-check", "Ignores the Xcode version check.", v => { min_xcode_version = null; }, true /* This is a non-documented option. Please discuss any customers running into the xcode version check on the maciosdev@ list before giving this option out to customers. */);
			options.Add ("nolink", "Do not link the assemblies.", v => app.LinkMode = LinkMode.None);
#if MMP
			options.Add ("linkplatform", "Link only the Xamarin.Mac.dll platform assembly.", v => app.LinkMode = LinkMode.Platform);
#endif
			options.Add ("linksdkonly", "Link only the SDK assemblies.", v => app.LinkMode = LinkMode.SDKOnly);
			options.Add ("linkskip=", "Skip linking of the specified assembly.", v => app.LinkSkipped.Add (v));
			options.Add ("i18n=", "List of i18n assemblies to copy to the output directory, separated by commas (none, all, cjk, mideast, other, rare and/or west).", v => app.ParseI18nAssemblies (v));
			options.Add ("xml=", "Provide an extra XML definition file to the linker.", v => app.Definitions.Add (v));
			options.Add ("warnaserror:", "An optional comma-separated list of warning codes that should be reported as errors (if no warnings are specified all warnings are reported as errors).", v => {
				try {
					if (!string.IsNullOrEmpty (v)) {
						foreach (var code in v.Split (new char [] { ',' }, StringSplitOptions.RemoveEmptyEntries))
							ErrorHelper.SetWarningLevel (ErrorHelper.WarningLevel.Error, int.Parse (code));
					} else {
						ErrorHelper.SetWarningLevel (ErrorHelper.WarningLevel.Error);
					}
				} catch (Exception ex) {
					throw ErrorHelper.CreateError (26, ex, Errors.MX0026, "--warnaserror", ex.Message);
				}
			});
			options.Add ("nowarn:", "An optional comma-separated list of warning codes to ignore (if no warnings are specified all warnings are ignored).", v => {
				try {
					if (!string.IsNullOrEmpty (v)) {
						foreach (var code in v.Split (new char [] { ',' }, StringSplitOptions.RemoveEmptyEntries))
							ErrorHelper.SetWarningLevel (ErrorHelper.WarningLevel.Disable, int.Parse (code));
					} else {
						ErrorHelper.SetWarningLevel (ErrorHelper.WarningLevel.Disable);
					}
				} catch (Exception ex) {
					throw ErrorHelper.CreateError (26, ex, Errors.MX0026, "--nowarn", ex.Message);
				}
			});
			options.Add ("coop:", "If the GC should run in cooperative mode.", v => { app.EnableCoopGC = ParseBool (v, "coop"); }, hidden: true);
			options.Add ("sgen-conc", "Enable the *experimental* concurrent garbage collector.", v => { app.EnableSGenConc = true; });
			options.Add ("marshal-objectivec-exceptions:", "Specify how Objective-C exceptions should be marshalled. Valid values: default, unwindmanagedcode, throwmanagedexception, abort and disable. The default depends on the target platform (on watchOS the default is 'throwmanagedexception', while on all other platforms it's 'disable').", v => {
				if (Application.TryParseObjectiveCExceptionMode (v, out var value)) {
					app.MarshalObjectiveCExceptions = value;
				} else {
					throw ErrorHelper.CreateError (26, Errors.MX0026, "--marshal-objective-exceptions", $"Invalid value: {v}. Valid values are: default, unwindmanagedcode, throwmanagedexception, abort and disable.");
				}
			});
			options.Add ("marshal-managed-exceptions:", "Specify how managed exceptions should be marshalled. Valid values: default, unwindnativecode, throwobjectivecexception, abort and disable. The default depends on the target platform (on watchOS the default is 'throwobjectivecexception', while on all other platform it's 'disable').", v => {
				if (Application.TryParseManagedExceptionMode (v, out var value)) {
					app.MarshalManagedExceptions = value;
				} else {
					throw ErrorHelper.CreateError (26, Errors.MX0026, "--marshal-managed-exceptions", $"Invalid value: {v}. Valid values are: default, unwindnativecode, throwobjectivecexception, abort and disable.");
				}
			});
			options.Add ("j|jobs=", "The level of concurrency. Default is the number of processors.", v => {
				Jobs = int.Parse (v);
			});
			options.Add ("embeddinator", "Enables Embeddinator targetting mode.", v => {
				app.Embeddinator = true;
			}, true);
			options.Add ("dynamic-symbol-mode:", "Specify how dynamic symbols are treated so that they're not linked away by the native linker. Valid values: linker (pass \"-u symbol\" to the native linker), code (generate native code that uses the dynamic symbol), ignore (do nothing and hope for the best). The default is 'code' when using bitcode, and 'linker' otherwise.", (v) => {
				switch (v.ToLowerInvariant ()) {
				case "default":
					app.SymbolMode = SymbolMode.Default;
					break;
				case "linker":
					app.SymbolMode = SymbolMode.Linker;
					break;
				case "code":
					app.SymbolMode = SymbolMode.Code;
					break;
				case "ignore":
					app.SymbolMode = SymbolMode.Ignore;
					break;
				default:
					throw ErrorHelper.CreateError (26, Errors.MX0026, "--dynamic-symbol-mode", $"Invalid value: {v}. Valid values are: default, linker, code and ignore.");
				}
			});
			options.Add ("ignore-dynamic-symbol:", "Specify that Xamarin.iOS/Xamarin.Mac should not try to prevent the linker from removing the specified symbol.", (v) => {
				app.IgnoredSymbols.Add (v);
			});
			options.Add ("root-assembly=", "Specifies any root assemblies. There must be at least one root assembly, usually the main executable.", (v) => {
				app.RootAssemblies.Add (v);
			});
			options.Add ("optimize=", "A comma-delimited list of optimizations to enable/disable. To enable an optimization, use --optimize=[+]remove-uithread-checks. To disable an optimizations: --optimize=-remove-uithread-checks. Use '+all' to enable or '-all' disable all optimizations. Only compiler-generated code or code otherwise marked as safe to optimize will be optimized.\n" +
					"Available optimizations:\n" +
					"    dead-code-elimination: By default always enabled (requires the linker). Removes IL instructions the linker can determine will never be executed. This is most useful in combination with the inline-* optimizations, since inlined conditions almost always also results in blocks of code that will never be executed.\n" +
					"    remove-uithread-checks: By default enabled for release builds (requires the linker). Remove all UI Thread checks (makes the app smaller, and slightly faster at runtime).\n" +
#if MONOTOUCH
					"    inline-isdirectbinding: By default enabled unless the interpreter is enabled (requires the linker). Tries to inline calls to NSObject.IsDirectBinding to load a constant value. Makes the app smaller, and slightly faster at runtime.\n" +
#else
					"    inline-isdirectbinding: By default disabled, because it may require the linker. Tries to inline calls to NSObject.IsDirectBinding to load a constant value. Makes the app smaller, and slightly faster at runtime.\n" +
#endif
#if MONOTOUCH
					"    remove-dynamic-registrar: By default enabled when the static registrar is enabled and the interpreter is not used. Removes the dynamic registrar (makes the app smaller).\n" +
					"    inline-runtime-arch: By default always enabled (requires the linker). Inlines calls to ObjCRuntime.Runtime.Arch to load a constant value. Makes the app smaller, and slightly faster at runtime.\n" +
#endif
					"    blockliteral-setupblock: By default enabled when using the static registrar. Optimizes calls to BlockLiteral.SetupBlock and certain BlockLiteral constructors to avoid having to calculate the block signature at runtime.\n" +
					"    inline-intptr-size: By default enabled for builds that target a single architecture (requires the linker). Inlines calls to IntPtr.Size to load a constant value. Makes the app smaller, and slightly faster at runtime.\n" +
					"    inline-dynamic-registration-supported: By default always enabled (requires the linker). Optimizes calls to Runtime.DynamicRegistrationSupported to be a constant value. Makes the app smaller, and slightly faster at runtime.\n" +
#if !MONOTOUCH
					"    register-protocols: Remove unneeded metadata for protocol support. Makes the app smaller and reduces memory requirements. Disabled when the interpreter is used or when the static registrar is not enabled.\n" +
					"    trim-architectures: Remove unneeded architectures from bundled native libraries. Makes the app smaller and is required for macOS App Store submissions.\n" +
#else
					"    register-protocols: Remove unneeded metadata for protocol support. Makes the app smaller and reduces memory requirements. Disabled, by default, to allow dynamic code loading.\n" +
					"    remove-unsupported-il-for-bitcode: Remove IL that is not supported when compiling to bitcode, and replace with a NotSupportedException.\n" +
					"    force-rejected-types-removal: Forcefully remove types that are known to cause rejections when applications are submitted to Apple. This includes: `UIWebView` and related types.\n" +
#endif
					"",
					(v) => {
						if (optimize is null)
							optimize = new List<string> ();
						optimize.Add (v);
					});
			options.Add ("package-debug-symbols:", "Specify whether debug info files (*.mdb / *.pdb) should be packaged in the app. Default is 'true' for debug builds and 'false' for release builds.", v => app.PackageManagedDebugSymbols = ParseBool (v, "package-debug-symbols"));
			options.Add ("profiling:", "Enable profiling.", v => app.EnableProfiling = ParseBool (v, "profiling"));
			options.Add ("debugtrack:", "Enable debug tracking of object resurrection bugs.", v => { app.DebugTrack = ParseBool (v, "--debugtrack"); });
			options.Add ("http-message-handler=", "Specify the default HTTP message handler for HttpClient.", v => { app.HttpMessageHandler = v; });
			options.Add ("tls-provider=", "Specify the default TLS provider.", v => { app.TlsProvider = v; });
			options.Add ("setenv=", "Set the environment variable in the application on startup.", v => {
				int eq = v.IndexOf ('=');
				if (eq <= 0)
					throw ErrorHelper.CreateError (2, Errors.MT0002, v);
				var name = v.Substring (0, eq);
				var value = v.Substring (eq + 1);
				app.EnvironmentVariables.Add (name, value);
			}
			);
			options.Add ("registrar:", "Specify the registrar to use (dynamic, static or default (dynamic in the simulator, static on device)).", v => {
				app.ParseRegistrar (v);
			});
			options.Add ("runregistrar:", "Runs the registrar on the input assembly and outputs a corresponding native library.",
				v => {
					a = Action.RunRegistrar;
					app.RegistrarOutputLibrary = v;
				},
				true /* this is an internal option */
			);
			options.Add ("warn-on-type-ref=", "Warn if any of the comma-separated types is referenced by assemblies - both before and after linking.", v => {
				app.WarnOnTypeRef.AddRange (v.Split (new char [] { ',' }, StringSplitOptions.RemoveEmptyEntries));
			});
			options.Add ("xamarin-runtime=", "Which runtime to use (MonoVM or CoreCLR).", v => {
				if (!Enum.TryParse<XamarinRuntime> (v, out var rv))
					throw new InvalidOperationException ($"Invalid XamarinRuntime '{v}'");
				app.XamarinRuntime = rv;
			}, true /* hidden - this is only for build-time --runregistrar support */);

			options.Add ("rid=", "The runtime identifier we're building for", v => {
				app.RuntimeIdentifier = v;
			}, true /* hidden - this is only for build-time --runregistrar support */);
			options.Add ("require-pinvoke-wrappers:", v => {
				app.RequiresPInvokeWrappers = ParseBool (v, "--require-pinvoke-wrappers");
			});
			options.Add ("skip-marking-nsobjects-in-user-assemblies:", "Don't mark NSObject (and any subclass of NSObject) in user assemblies in the linker. This may break your app, use at own risk.", v => {
				app.SkipMarkingNSObjectsInUserAssemblies = ParseBool (v, "--skip-marking-nsobjects-in-user-assemblies");
			});

			// check if needs to be removed: https://github.com/xamarin/xamarin-macios/issues/18693
			options.Add ("disable-automatic-linker-selection:", "Don't force the classic linker (ld64).", v => {
				app.DisableAutomaticLinkerSelection = ParseBool (v, "--disable-automatic-linker-selection");
			});

			// Keep the ResponseFileSource option at the end.
			options.Add (new Mono.Options.ResponseFileSource ());

			try {
				app.RootAssemblies.AddRange (options.Parse (args));
			} catch (ProductException) {
				throw;
			} catch (Exception e) {
				throw ErrorHelper.CreateError (10, e, Errors.MX0010, e);
			}

			if (a != Action.None)
				action = a;

			if (action == Action.Help || args.Length == 0) {
				ShowHelp (options);
				return true;
			} else if (action == Action.Version) {
				Console.WriteLine (NAME + " {0}.{1}", Constants.Version, Constants.Revision);
				return true;
			}

			LogArguments (args);

			var validateFramework = true;
#if MTOUCH
			validateFramework = !IsMlaunchAction (action);
#endif
			if (validateFramework)
				ValidateTargetFramework ();

			if (optimize is not null) {
				// This must happen after the call to ValidateTargetFramework, so that app.Platform is correct.
				var messages = new List<ProductException> ();
				foreach (var opt in optimize)
					app.Optimizations.Parse (app.Platform, opt, messages);
				ErrorHelper.Show (messages);
			}

			return false;
		}
#endif // !NET

#if !NET
		static int Jobs;
		public static int Concurrency {
			get {
				return Jobs == 0 ? Environment.ProcessorCount : Jobs;
			}
		}
#endif

		public static int Verbosity {
			get { return ErrorHelper.Verbosity; }
			set { ErrorHelper.Verbosity = value; }
		}

		static Driver ()
		{
			Verbosity = GetDefaultVerbosity ();
		}

		static int GetDefaultVerbosity ()
		{
			var v = 0;
			var fn = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.UserProfile), $".{NAME}-verbosity");
			if (File.Exists (fn)) {
				v = (int) new FileInfo (fn).Length;
				if (v == 0)
					v = 4; // this is the magic verbosity level we give everybody.
			}
			return v;
		}

		public static void Log (string value)
		{
			Log (0, value);
		}

		public static void Log (string format, params object [] args)
		{
			Log (0, format, args);
		}

		public static void Log (int min_verbosity, string value)
		{
			if (min_verbosity > Verbosity)
				return;

			Console.WriteLine (value);
		}

		public static void Log (int min_verbosity, string format, params object [] args)
		{
			if (min_verbosity > Verbosity)
				return;

			if (args.Length > 0)
				Console.WriteLine (format, args);
			else
				Console.WriteLine (format);
		}

		public static bool IsDotNet {
			get { return TargetFramework.IsDotNet; }
		}

		static TargetFramework targetFramework;

		public static TargetFramework TargetFramework {
			get { return targetFramework; }
			set { targetFramework = value; }
		}

		// We need to delay validating the target framework until we've parsed all the command line arguments,
		// so first store it here, and then we call ValidateTargetFramework when we're done parsing the command
		// line arguments.
		static string target_framework;
		static void SetTargetFramework (string value)
		{
			target_framework = value;
		}

		static void ValidateTargetFramework ()
		{
			if (string.IsNullOrEmpty (target_framework))
				throw ErrorHelper.CreateError (86, Errors.MX0086 /* A target framework (--target-framework) must be specified */);

			var fx = target_framework;
			switch (fx.Trim ().ToLowerInvariant ()) {
			case "xammac":
			case "mobile":
			case "xamarin.mac":
				targetFramework = TargetFramework.Xamarin_Mac_2_0_Mobile;
				ErrorHelper.Warning (90, Errors.MX0090, /* The target framework '{0}' is deprecated. Use '{1}' instead. */ fx, targetFramework);
				return;
			default:
				TargetFramework parsedFramework;
				if (!TargetFramework.TryParse (fx, out parsedFramework))
					throw ErrorHelper.CreateError (68, Errors.MX0068, fx);

				targetFramework = parsedFramework;

				break;
			}

			bool show_0090 = false;
#if MONOMAC
			if (!TargetFramework.IsValidFramework (targetFramework)) {
				// For historic reasons this is messy.
				// If the TargetFramework we got isn't any of the one we accept, we have to do some fudging.
				bool force45From40UnifiedSystemFull = false;

				// Detect Classic usage, and show an error.
				if (App.References.Any ((v) => Path.GetFileName (v) == "XamMac.dll"))
					throw ErrorHelper.CreateError (143, Errors.MM0143 /* Projects using the Classic API are not supported anymore. Please migrate the project to the Unified API. */);

				if (targetFramework == TargetFramework.Net_2_0
					|| targetFramework == TargetFramework.Net_3_0
					|| targetFramework == TargetFramework.Net_3_5
					|| targetFramework == TargetFramework.Net_4_0
					|| targetFramework == TargetFramework.Net_4_5) {
					// .NETFramework,v2.0 => Xamarin.Mac,Version=v4.5,Profile=Full
					// .NETFramework,v3.0 => Xamarin.Mac,Version=v4.5,Profile=Full
					// .NETFramework,v3.5 => Xamarin.Mac,Version=v4.5,Profile=Full
					// .NETFramework,v4.0 => Xamarin.Mac,Version=v4.5,Profile=Full
					// .NETFramework,v4.5 => Xamarin.Mac,Version=v4.5,Profile=Full
					TargetFramework = TargetFramework.Xamarin_Mac_4_5_Full;
				} else if (TargetFramework.Identifier == TargetFramework.Xamarin_Mac_2_0_Mobile.Identifier
					&& TargetFramework.Version == TargetFramework.Xamarin_Mac_2_0_Mobile.Version) {
					// At least once instance of a TargetFramework of Xamarin.Mac,v2.0,(null) was found already. Assume any v2.0 implies a desire for Modern.
					TargetFramework = TargetFramework.Xamarin_Mac_2_0_Mobile;
				} else if (TargetFramework.Identifier == TargetFramework.Xamarin_Mac_4_5_Full.Identifier
						 && TargetFramework.Profile == TargetFramework.Xamarin_Mac_4_5_Full.Profile) {
					// Xamarin.Mac,Version=vX.Y,Profile=Full => Xamarin.Mac,Version=v4.5,Profile=Full
					TargetFramework = TargetFramework.Xamarin_Mac_4_5_Full;
				} else if (TargetFramework.Identifier == TargetFramework.Xamarin_Mac_4_5_System.Identifier
						 && TargetFramework.Profile == TargetFramework.Xamarin_Mac_4_5_System.Profile) {
					// Xamarin.Mac,Version=vX.Y,Profile=System => Xamarin.Mac,Version=v4.5,Profile=System
					TargetFramework = TargetFramework.Xamarin_Mac_4_5_System;
				} else {
					// This is a total hack. Instead of passing in an argument, we walk the references looking for
					// the "right" Xamarin.Mac and assume you are doing something
					foreach (var asm in App.References) {
						if (asm.EndsWith ("reference/full/Xamarin.Mac.dll", StringComparison.Ordinal)) {
							force45From40UnifiedSystemFull = TargetFramework == TargetFramework.Net_4_0;
							TargetFramework = TargetFramework.Xamarin_Mac_4_5_System;
							break;
						} else if (asm.EndsWith ("mono/4.5/Xamarin.Mac.dll", StringComparison.Ordinal)) {
							TargetFramework = TargetFramework.Xamarin_Mac_4_5_Full;
							break;
						}
					}
				}

				if (force45From40UnifiedSystemFull) {
					// Xamarin.Mac Unified Full System profile requires .NET 4.5, not .NET 4.0.
					FixReferences (x => x.Contains ("lib/mono/4.0"), x => x.Replace ("lib/mono/4.0", "lib/mono/4.5"));
				}

				show_0090 = true;
			}
#endif

			// Verify that our TargetFramework is our limited list of valid target frameworks.
			if (!TargetFramework.IsValidFramework (TargetFramework))
				throw ErrorHelper.CreateError (70, Errors.MX0070, fx, "'" + string.Join ("', '", TargetFramework.ValidFrameworks.Select ((v) => v.ToString ()).ToArray ()) + "'");

			// Only show the warning if no errors were shown.
			if (show_0090)
				ErrorHelper.Warning (90, Errors.MX0090, /* The target framework '{0}' is deprecated. Use '{1}' instead. */ fx, TargetFramework);
		}

#if !MMP_TEST
		static void FileMove (string source, string target)
		{
			File.Delete (target);
			File.Move (source, target);
		}

		static void MoveIfDifferent (string path, string tmp, bool use_stamp = false)
		{
			// Don't read the entire file into memory, it can be quite big in certain cases.

			bool move = false;

			using (var fs1 = new FileStream (path, FileMode.Open, FileAccess.Read)) {
				using (var fs2 = new FileStream (tmp, FileMode.Open, FileAccess.Read)) {
					if (fs1.Length != fs2.Length) {
						Log (3, "New file '{0}' has different length, writing new file.", path);
						move = true;
					} else {
						move = !Cache.CompareStreams (fs1, fs2);
					}
				}
			}

			if (move) {
				FileMove (tmp, path);
			} else {
				Log (3, "Target {0} is up-to-date.", path);
				if (use_stamp)
					Driver.Touch (path + ".stamp");
			}
		}

		public static void WriteIfDifferent (string path, string contents, bool use_stamp = false)
		{
			var tmp = path + ".tmp";

			try {
				if (!File.Exists (path)) {
					Directory.CreateDirectory (Path.GetDirectoryName (path));
					File.WriteAllText (path, contents);
					Log (3, "File '{0}' does not exist, creating it.", path);
					return;
				}

				File.WriteAllText (tmp, contents);
				MoveIfDifferent (path, tmp, use_stamp);
			} catch (Exception e) {
				File.WriteAllText (path, contents);
				ErrorHelper.Warning (1014, e, Errors.MT1014, path, e.Message);
			} finally {
				File.Delete (tmp);
			}
		}

		public static void WriteIfDifferent (string path, byte [] contents, bool use_stamp = false)
		{
			var tmp = path + ".tmp";

			try {
				if (!File.Exists (path)) {
					File.WriteAllBytes (path, contents);
					Log (3, "File '{0}' does not exist, creating it.", path);
					return;
				}

				File.WriteAllBytes (tmp, contents);
				MoveIfDifferent (path, tmp, use_stamp);
			} catch (Exception e) {
				File.WriteAllBytes (path, contents);
				ErrorHelper.Warning (1014, e, Errors.MT1014, path, e.Message);
			} finally {
				File.Delete (tmp);
			}
		}
#endif


		internal static string GetFullPath ()
		{
			return System.Reflection.Assembly.GetExecutingAssembly ().Location;
		}

		static string xcode_product_version;
		public static string XcodeProductVersion {
			get {
				return xcode_product_version;
			}
		}

		static Version xcode_version;
		public static Version XcodeVersion {
			get {
				return xcode_version;
			}
		}

		static void SetCurrentLanguage ()
		{
			// There's no way to change the current culture from the command-line
			// without changing the system settings, so honor LANG if set.
			// This eases testing mtouch/mmp with different locales significantly,
			// and won't run into issues where changing the system language leaves
			// the tester with an incomprehensible system.
			var lang_variable = Environment.GetEnvironmentVariable ("LANG");
			if (string.IsNullOrEmpty (lang_variable))
				return;

			// Mimic how mono transforms LANG into a culture name:
			// https://github.com/mono/mono/blob/fc6e8a27fc55319141ceb29fbb7b5c63a9030b5e/mono/metadata/locales.c#L568-L576
			var lang = lang_variable;
			var idx = lang.IndexOf ('.');
			if (idx >= 0)
				lang = lang.Substring (0, idx);
			idx = lang.IndexOf ('@');
			if (idx >= 0)
				lang = lang.Substring (0, idx);
			lang = lang.Replace ('_', '-');
			try {
				var culture = CultureInfo.GetCultureInfo (lang);
				if (culture is not null) {
					CultureInfo.DefaultThreadCurrentCulture = culture;
					Log (2, $"The current language was set to '{culture.DisplayName}' according to the LANG environment variable (LANG={lang_variable}).");
				}
			} catch (Exception e) {
				ErrorHelper.Warning (124, e, Errors.MT0124, lang, lang_variable, e.Message);
			}
		}

		static void LogArguments (string [] arguments)
		{
			if (Verbosity < 1)
				return;
			if (!arguments.Any ((v) => v.Length > 0 && v [0] == '@'))
				return; // no need to print arguments unless we get response files
			LogArguments (arguments, 1);
		}

		static void LogArguments (string [] arguments, int indentation)
		{
			Log ("Provided arguments:");
			var indent = new string (' ', indentation * 4);
			foreach (var arg in arguments) {
				Log (indent + StringUtils.Quote (arg));
				if (arg.Length > 0 && arg [0] == '@') {
					var fn = arg.Substring (1);
					LogArguments (File.ReadAllLines (fn), indentation + 1);
				}
			}
		}

		public static void Touch (IEnumerable<string> filenames, DateTime? timestamp = null)
		{
			if (timestamp is null)
				timestamp = DateTime.Now;
			foreach (var filename in filenames) {
				try {
					var fi = new FileInfo (filename);
					if (!fi.Exists) {
						using (var fo = fi.OpenWrite ()) {
							// Create an empty file.
						}
					}
					fi.LastWriteTime = timestamp.Value;
				} catch (Exception e) {
					ErrorHelper.Warning (128, Errors.MT0128, filename, e.Message);
				}
			}
		}

		public static void Touch (params string [] filenames)
		{
			Touch ((IEnumerable<string>) filenames);
		}

		static int watch_level;
		static Stopwatch watch;

		public static int WatchLevel {
			get { return watch_level; }
			set {
				watch_level = value;
				if ((watch_level > 0) && (watch is null)) {
					watch = new Stopwatch ();
					watch.Start ();
				}
			}
		}

		public static void Watch (string msg, int level)
		{
			if ((watch is null) || (level > WatchLevel))
				return;
			for (int i = 0; i < level; i++)
				Console.Write ("!");
			Console.WriteLine ("Timestamp {0}: {1} ms", msg, watch.ElapsedMilliseconds);
		}

		internal static PDictionary FromPList (string name)
		{
			if (!File.Exists (name))
				throw ErrorHelper.CreateError (24, Errors.MT0024, name);
			return PDictionary.FromFile (name);
		}

		const string XcodeDefault = "/Applications/Xcode.app";

		static string FindSystemXcode ()
		{
			var output = new StringBuilder ();
			if (Driver.RunCommand ("xcode-select", new [] { "-p" }, output: output) != 0) {
				ErrorHelper.Warning (59, Errors.MX0059, output.ToString ());
				return null;
			}
			return output.ToString ().Trim ();
		}

		static string sdk_root;
		static string developer_directory;

		public static string SdkRoot {
			get => sdk_root;
			set => sdk_root = value;
		}

		public static string DeveloperDirectory {
			get {
				return developer_directory;
			}
		}

		// This returns the /Applications/Xcode*.app/Contents/Developer/Platforms directory
		public static string PlatformsDirectory {
			get {
				return Path.Combine (DeveloperDirectory, "Platforms");
			}
		}

		// This returns the /Applications/Xcode*.app/Contents/Developer/Platforms/*.platform directory
		public static string GetPlatformDirectory (Application app)
		{
			return Path.Combine (PlatformsDirectory, GetPlatform (app) + ".platform");
		}

		static string local_build;
		public static string WalkUpDirHierarchyLookingForLocalBuild (Application app)
		{
			if (local_build is null) {
				var localPath = Path.GetDirectoryName (GetFullPath ());
				while (localPath.Length > 1) {
					if (File.Exists (Path.Combine (localPath, "Make.config"))) {
						local_build = Path.Combine (localPath, app.LocalBuildDir, "Library", "Frameworks", app.ProductName + ".framework", "Versions", "Current");
						return local_build;
					}

					localPath = Path.GetDirectoryName (localPath);
				}
			}
			return local_build;
		}

		// This is the 'Current' directory of the installed framework
		// For XI/XM installed from package it's /Library/Frameworks/Xamarin.iOS.framework/Versions/Current or /Library/Frameworks/Xamarin.Mac.framework/Versions/Current
		static string framework_dir;
		public static string GetFrameworkCurrentDirectory (Application app)
		{
			if (framework_dir is null) {
				var env_framework_dir = Environment.GetEnvironmentVariable (app.FrameworkLocationVariable);
				if (!string.IsNullOrEmpty (env_framework_dir)) {
					framework_dir = env_framework_dir;
				} else {
#if DEBUG
					// when launched from Visual Studio, the executable is not in the final install location,
					// so walk the directory hierarchy to find the root source directory.
					framework_dir = WalkUpDirHierarchyLookingForLocalBuild (app);
#else
					framework_dir = Path.GetDirectoryName (Path.GetDirectoryName (Path.GetDirectoryName (GetFullPath ())));
#endif
				}
				framework_dir = Target.GetRealPath (framework_dir);
			}
			return framework_dir;
		}

		public static void SetFrameworkCurrentDirectory (string value)
		{
			framework_dir = value;
		}

		// This is the 'Current/bin' directory of the installed framework
		// For XI/XM installed from package it's one of these two:
		//    /Library/Frameworks/Xamarin.iOS.framework/Versions/Current/bin
		//    /Library/Frameworks/Xamarin.Mac.framework/Versions/Current/bin
		public static string GetFrameworkBinDirectory (Application app)
		{
			return Path.Combine (GetFrameworkCurrentDirectory (app), "bin");
		}

		// This is the 'Current/lib' directory of the installed framework
		// For XI/XM installed from package it's one of these two:
		//    /Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib
		//    /Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib
		public static string GetFrameworkLibDirectory (Application app)
		{
			return Path.Combine (GetFrameworkCurrentDirectory (app), "lib");
		}

		// This is the directory where the libxamarin*.[a|dylib] and libxammac*.[a|dylib] libraries are
		public static string GetXamarinLibraryDirectory (Application app)
		{
			return GetProductSdkLibDirectory (app);
		}

		// This is the directory where the Xamarin[-debug].framework frameworks are
		public static string GetXamarinFrameworkDirectory (Application app)
		{
			return GetProductFrameworksDirectory (app);
		}

		public static string GetProductFrameworksDirectory (Application app)
		{
			return Path.Combine (GetProductSdkDirectory (app), "Frameworks");
		}

		// This is the directory where the platform assembly (Xamarin.*.dll) can be found
		public static string GetPlatformFrameworkDirectory (Application app)
		{
			switch (app.Platform) {
			case ApplePlatform.iOS:
				return Path.Combine (GetFrameworkLibDirectory (app), "mono", "Xamarin.iOS");
			case ApplePlatform.WatchOS:
				return Path.Combine (GetFrameworkLibDirectory (app), "mono", "Xamarin.WatchOS");
			case ApplePlatform.TVOS:
				return Path.Combine (GetFrameworkLibDirectory (app), "mono", "Xamarin.TVOS");
			case ApplePlatform.MacCatalyst:
				return Path.Combine (GetFrameworkLibDirectory (app), "mono", "Xamarin.MacCatalyst");
			case ApplePlatform.MacOSX:
#if MMP
				if (IsUnifiedMobile)
					return Path.Combine (GetFrameworkLibDirectory (app), "mono", "Xamarin.Mac");
				return Path.Combine (GetFrameworkLibDirectory (app), "mono", "4.5");
#endif
			default:
				throw ErrorHelper.CreateError (71, Errors.MX0071, app.Platform, app.ProductName);
			}
		}

		// This is the directory that contains the native libraries (libmono*.[a|dylib]) that come from mono.
		// For Xamarin.Mac it can be:
		// * /Library/Frameworks/Mono.framework/Versions/Current/lib/ (when using system mono)
		// * /Library/Frameworks/Xamarin.Mac.framework/Versions/Current/SDKs/*.sdk/lib
		// For Xamarin.iOS it can be:
		// * /Library/Frameworks/Xamarin.iOS.framework/Versions/Current/SDKs/*.sdk/lib
		static string mono_lib_directory;
		public static string GetMonoLibraryDirectory (Application app)
		{
			if (mono_lib_directory is null) {
#if MMP
				if (IsUnifiedFullSystemFramework) {
					mono_lib_directory = RunPkgConfig ("--variable=libdir");
				} else {
					mono_lib_directory = GetProductSdkLibDirectory (app);
				}
#else
				mono_lib_directory = GetProductSdkLibDirectory (app);
#endif
			}
			return mono_lib_directory;
		}

		// /Library/Frameworks/Xamarin.*.framework/Versions/Current/SDKs/*.sdk/Frameworks
		public static string GetMonoFrameworksDirectory (Application app)
		{
#if MMP
			if (IsUnifiedFullSystemFramework)
				throw ErrorHelper.CreateError (99, Errors.MX0099, "Calling 'GetMonoFrameworksDirectory' is not allowed when targetting the full system framework.");
#endif
			return Path.Combine (GetProductSdkDirectory (app), "Frameworks");
		}

		// /Library/Frameworks/Xamarin.*.framework/Versions/Current/SDKs/*.sdk/lib
		public static string GetProductSdkLibDirectory (Application app)
		{
			return Path.Combine (GetProductSdkDirectory (app), "lib");
		}

		// /Library/Frameworks/Xamarin.*.framework/Versions/Current/SDKs/*.sdk/include
		public static string GetProductSdkIncludeDirectory (Application app)
		{
			return Path.Combine (GetProductSdkDirectory (app), "include");
		}

		// /Library/Frameworks/Xamarin.*.framework/Versions/Current/SDKs/*.sdk/Frameworks
		public static string GetProductSdkFrameworksDirectory (Application app)
		{
			return Path.Combine (GetProductSdkDirectory (app), "Frameworks");
		}

		// /Library/Frameworks/Xamarin.*.framework/Versions/Current/SDKs/*.sdk
		public static string GetProductSdkDirectory (Application app)
		{
			var sdksDir = Path.Combine (GetFrameworkCurrentDirectory (app), "SDKs");
			string sdkName;
			switch (app.Platform) {
			case ApplePlatform.iOS:
				sdkName = app.IsDeviceBuild ? "MonoTouch.iphoneos.sdk" : "MonoTouch.iphonesimulator.sdk";
				break;
			case ApplePlatform.WatchOS:
				sdkName = app.IsDeviceBuild ? "Xamarin.WatchOS.sdk" : "Xamarin.WatchSimulator.sdk";
				break;
			case ApplePlatform.TVOS:
				sdkName = app.IsDeviceBuild ? "Xamarin.AppleTVOS.sdk" : "Xamarin.AppleTVSimulator.sdk";
				break;
			case ApplePlatform.MacOSX:
				sdkName = "Xamarin.macOS.sdk";
				break;
			case ApplePlatform.MacCatalyst:
				sdkName = "Xamarin.MacCatalyst.sdk";
				break;
			default:
				throw ErrorHelper.CreateError (71, Errors.MX0071, app.Platform, app.ProductName);
			}
			return Path.Combine (sdksDir, sdkName);
		}

		// This returns the platform to use in /Applications/Xcode*.app/Contents/Developer/Platforms/*.platform
		public static string GetPlatform (Application app)
		{
			switch (app.Platform) {
			case ApplePlatform.iOS:
				return app.IsDeviceBuild ? "iPhoneOS" : "iPhoneSimulator";
			case ApplePlatform.WatchOS:
				return app.IsDeviceBuild ? "WatchOS" : "WatchSimulator";
			case ApplePlatform.TVOS:
				return app.IsDeviceBuild ? "AppleTVOS" : "AppleTVSimulator";
			case ApplePlatform.MacOSX:
			case ApplePlatform.MacCatalyst:
				return "MacOSX";
			default:
				throw ErrorHelper.CreateError (71, Errors.MX0071, app.Platform, app.ProductName);
			}
		}

		// This returns the correct /Applications/Xcode*.app/Contents/Developer/Platforms/*.platform/Developer/SDKs/*X.Y.sdk directory
		public static string GetFrameworkDirectory (Application app)
		{
			var platform = GetPlatform (app);

			return Path.Combine (PlatformsDirectory, platform + ".platform", "Developer", "SDKs", platform + app.NativeSdkVersion.ToString () + ".sdk");
		}

		public static string GetProductAssembly (Application app)
		{
			switch (app.Platform) {
			case ApplePlatform.iOS:
				return IsDotNet ? "Microsoft.iOS" : "Xamarin.iOS";
			case ApplePlatform.WatchOS:
				return IsDotNet ? "Microsoft.watchOS" : "Xamarin.WatchOS";
			case ApplePlatform.TVOS:
				return IsDotNet ? "Microsoft.tvOS" : "Xamarin.TVOS";
			case ApplePlatform.MacOSX:
				return IsDotNet ? "Microsoft.macOS" : "Xamarin.Mac";
			case ApplePlatform.MacCatalyst:
				return IsDotNet ? "Microsoft.MacCatalyst" : "Xamarin.MacCatalyst";
			default:
				throw ErrorHelper.CreateError (71, Errors.MX0071, app.Platform, app.ProductName);
			}
		}

		public static void ValidateXcode (Application app, bool accept_any_xcode_version, bool warn_if_not_found)
		{
			if (sdk_root is null) {
				sdk_root = FindSystemXcode ();
				if (sdk_root is null) {
					// FindSystemXcode showed a warning in this case. In particular do not use 'string.IsNullOrEmpty' here,
					// because FindSystemXcode may return an empty string (with no warning printed) if the xcode-select command
					// succeeds, but returns nothing.
					sdk_root = null;
				} else if (!Directory.Exists (sdk_root)) {
					ErrorHelper.Warning (60, Errors.MX0060, sdk_root);
					sdk_root = null;
				} else {
					if (!accept_any_xcode_version)
						ErrorHelper.Warning (61, Errors.MT0061, sdk_root);
				}
				if (sdk_root is null) {
					sdk_root = XcodeDefault;
					if (!Directory.Exists (sdk_root)) {
						if (warn_if_not_found) {
							// mmp: and now we give up, but don't throw like mtouch, because we don't want to change behavior (this sometimes worked it appears)
							ErrorHelper.Warning (56, Errors.MX0056);
							return; // Can't validate the version below if we can't even find Xcode...
						}

						throw ErrorHelper.CreateError (56, Errors.MX0056);
					}
					ErrorHelper.Warning (62, Errors.MT0062, sdk_root);
				}
			} else if (!Directory.Exists (sdk_root)) {
				throw ErrorHelper.CreateError (55, Errors.MT0055, sdk_root);
			}

			// Check what kind of path we got
			if (File.Exists (Path.Combine (sdk_root, "Contents", "MacOS", "Xcode"))) {
				// path to the Xcode.app
				developer_directory = Path.Combine (sdk_root, "Contents", "Developer");
			} else if (File.Exists (Path.Combine (sdk_root, "..", "MacOS", "Xcode"))) {
				// path to Contents/Developer
				developer_directory = Path.GetFullPath (Path.Combine (sdk_root, "..", "..", "Contents", "Developer"));
			} else {
				throw ErrorHelper.CreateError (57, Errors.MT0057, sdk_root);
			}

			var plist_path = Path.Combine (Path.GetDirectoryName (DeveloperDirectory), "version.plist");

			if (File.Exists (plist_path)) {
				var plist = FromPList (plist_path);
				var version = plist.GetString ("CFBundleShortVersionString");
				xcode_version = new Version (version);
				xcode_product_version = plist.GetString ("ProductBuildVersion");
			} else {
				throw ErrorHelper.CreateError (58, Errors.MT0058, Path.GetDirectoryName (Path.GetDirectoryName (DeveloperDirectory)), plist_path);
			}

			if (!accept_any_xcode_version) {
				if (min_xcode_version is not null && XcodeVersion < min_xcode_version)
					throw ErrorHelper.CreateError (51, Errors.MT0051, app.ProductConstants.Version, XcodeVersion.ToString (), sdk_root, app.ProductName, min_xcode_version);

				if (XcodeVersion < SdkVersions.XcodeVersion)
					ErrorHelper.Warning (79, Errors.MT0079, app.ProductConstants.Version, XcodeVersion.ToString (), sdk_root, SdkVersions.Xcode, app.ProductName);
			}

			Driver.Log (1, "Using Xcode {0} ({2}) found in {1}", XcodeVersion, sdk_root, XcodeProductVersion);
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
				throw ErrorHelper.CreateError (26, Errors.MX0026, name, value);
			return result;
		}

		static readonly Dictionary<string, string> tools = new Dictionary<string, string> ();
		static string FindTool (Application app, string tool)
		{
			string path;

			lock (tools) {
				if (tools.TryGetValue (tool, out path))
					return path;
			}

			path = LocateTool (app, tool);
			static string LocateTool (Application app, string tool)
			{
				if (XcrunFind (app, tool, out var path))
					return path;

				// either /Developer (Xcode 4.2 and earlier), /Applications/Xcode.app/Contents/Developer (Xcode 4.3) or user override
				path = Path.Combine (DeveloperDirectory, "usr", "bin", tool);
				if (File.Exists (path))
					return path;

				// Xcode 4.3 (without command-line tools) also has a copy of 'strip'
				path = Path.Combine (DeveloperDirectory, "Toolchains", "XcodeDefault.xctoolchain", "usr", "bin", tool);
				if (File.Exists (path))
					return path;

				// Xcode "Command-Line Tools" install a copy in /usr/bin (and it can be there afterward)
				path = Path.Combine ("/usr", "bin", tool);
				if (File.Exists (path))
					return path;

				return null;
			}

			// We can end up finding the same tool multiple times.
			// That's not a problem.
			lock (tools)
				tools [tool] = path;

			if (path is null)
				throw ErrorHelper.CreateError (5307, Errors.MX5307 /* Missing '{0}' tool. Please install Xcode 'Command-Line Tools' component */, tool);

			return path;
		}

		static bool XcrunFind (Application app, string tool, out string path)
		{
			return XcrunFind (app, ApplePlatform.None, false, tool, out path);
		}

		static bool XcrunFind (Application app, ApplePlatform platform, bool is_simulator, string tool, out string path)
		{
			var env = new Dictionary<string, string> ();
			// Unset XCODE_DEVELOPER_DIR_PATH. See https://github.com/xamarin/xamarin-macios/issues/3931.
			env.Add ("XCODE_DEVELOPER_DIR_PATH", null);
			// Set DEVELOPER_DIR if we have it
			if (!string.IsNullOrEmpty (DeveloperDirectory))
				env.Add ("DEVELOPER_DIR", DeveloperDirectory);

			path = null;

			var args = new List<string> ();
			if (platform != ApplePlatform.None) {
				args.Add ("-sdk");
				switch (platform) {
				case ApplePlatform.iOS:
					args.Add (is_simulator ? "iphonesimulator" : "iphoneos");
					break;
				case ApplePlatform.MacOSX:
					args.Add ("macosx");
					break;
				case ApplePlatform.TVOS:
					args.Add (is_simulator ? "appletvsimulator" : "appletvos");
					break;
				case ApplePlatform.WatchOS:
					args.Add (is_simulator ? "watchsimulator" : "watchos");
					break;
				default:
					throw ErrorHelper.CreateError (71, Errors.MX0071 /* Unknown platform: {0}. This usually indicates a bug in {1}; please file a bug report at https://github.com/xamarin/xamarin-macios/issues/new with a test case. */, platform.ToString (), app.ProductName);
				}
			}
			args.Add ("-f");
			args.Add (tool);

			var stdout = new StringBuilder ();
			var stderr = new StringBuilder ();
			var both = new StringBuilder ();
			// xcrun can write unrelated stuff to stderr even if it succeeds, so we need to separate stdout and stderr.
			// We also want to print out what happened if something went wrong, and in that case we don't want stdout
			// and stderr captured separately, because related lines could end up printed far from eachother in time,
			// and that's confusing. So capture stdout and stderr by themselves, and also capture both together.
			int ret = RunCommand ("xcrun", args, env,
				(v) => {
					lock (both) {
						both.AppendLine (v);
						stdout.AppendLine (v);
					}
				},
				(v) => {
					lock (both) {
						both.AppendLine (v);
						stderr.AppendLine (v);
					}
				});

			if (ret == 0) {
				path = stdout.ToString ().Trim ();
				if (!File.Exists (path)) {
					ErrorHelper.Warning (5315, Errors.MX5315 /* The tool xcrun failed to return a valid result (the file {0} does not exist). Check build log for details. */, tool, path);
					return false;
				}
			} else {
				Log (1, "Failed to locate the developer tool '{0}', 'xcrun {1}' returned with the exit code {2}:\n{3}", tool, string.Join (" ", args), ret, both.ToString ());
			}

			return ret == 0;
		}

		public static void RunXcodeTool (Application app, string tool, params string [] arguments)
		{
			RunXcodeTool (app, tool, (IList<string>) arguments);
		}

		public static void RunXcodeTool (Application app, string tool, IList<string> arguments)
		{
			var executable = FindTool (app, tool);
			var rv = RunCommand (executable, arguments);
			if (rv != 0)
				throw ErrorHelper.CreateError (5309, Errors.MX5309 /* Failed to execute the tool '{0}', it failed with an error code '{1}'. Please check the build log for details. */, tool, rv);
		}

		public static void RunClang (Application app, IList<string> arguments)
		{
			RunXcodeTool (app, "clang", arguments);
		}

		public static void RunInstallNameTool (Application app, IList<string> arguments)
		{
			RunXcodeTool (app, "install_name_tool", arguments);
		}

		public static void RunBitcodeStrip (Application app, IList<string> arguments)
		{
			RunXcodeTool (app, "bitcode_strip", arguments);
		}

		public static void RunLipo (Application app, string output, IEnumerable<string> inputs)
		{
			var sb = new List<string> ();
			sb.AddRange (inputs);
			sb.Add ("-create");
			sb.Add ("-output");
			sb.Add (output);
			RunLipo (app, sb);
		}

		public static void RunLipoAndCreateDsym (Application app, string output, IEnumerable<string> inputs)
		{
			RunLipo (app, output, inputs);

			var dsymFolders = inputs.Select (input => input + ".dSYM").Where (Directory.Exists).ToArray ();
			if (dsymFolders.Length > 1) {
				// Lipo the dSYMs into one big happy dSYM
				var dsymLibsDir = dsymFolders.Select (dsym => Path.Combine (dsym, "Contents", "Resources", "DWARF")).ToArray ();
				var allLibs = dsymLibsDir.Where (Directory.Exists).SelectMany (dir => Directory.EnumerateFiles (dir)).Select (dir => Path.GetFileName (dir)).Distinct ().ToArray ();

				foreach (var lib in allLibs) {
					var outputLib = Path.Combine (dsymLibsDir [0], lib);
					var allDsymInputs = dsymLibsDir.Select (libDir => Path.Combine (libDir, lib)).Where (File.Exists).ToArray ();
					Driver.RunLipo (app, outputLib, allDsymInputs);
				}
			}

			// Move the dSYM next to its executable
			if (dsymFolders.Length > 0) {
				var outputDsymDir = output + ".dSYM";
				if (Directory.Exists (outputDsymDir))
					Directory.Delete (outputDsymDir, true);
				Directory.Move (dsymFolders [0], outputDsymDir);
				RunCommand ("/usr/bin/mdimport", outputDsymDir);
			}
		}

		public static void RunLipo (Application app, IList<string> options)
		{
			RunXcodeTool (app, "lipo", options);
		}

		public static void CreateDsym (Application app, string output_dir, string appname, string dsym_dir)
		{
			RunDsymUtil (app, Path.Combine (output_dir, appname), "-num-threads", "4", "-z", "-o", dsym_dir);
			RunCommand ("/usr/bin/mdimport", dsym_dir);
		}

		public static void RunDsymUtil (Application app, params string [] options)
		{
			RunXcodeTool (app, "dsymutil", options);
		}

		public static void RunStrip (Application app, IList<string> options)
		{
			RunXcodeTool (app, "strip", options);
		}

		public static string CorlibName {
			get {
				if (IsDotNet)
					return "System.Private.CoreLib";
				return "mscorlib";
			}
		}

		public static Frameworks GetFrameworks (Application app)
		{
			var rv = Frameworks.GetFrameworks (app.Platform, app.IsSimulatorBuild);
			if (rv is null)
				throw ErrorHelper.CreateError (71, Errors.MX0071, app.Platform, app.ProductName);
			return rv;
		}
	}
}
