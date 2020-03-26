
/*
 * Copyright 2011-2014 Xamarin Inc. All rights reserved.
 * Copyright 2010 Novell Inc.
 *
 * Authors:
 *   Sebastien Pouliot <sebastien@xamarin.com>
 *   Aaron Bockover <abock@xamarin.com>
 *   Rolf Bjarne Kvinge <rolf@xamarin.com>
 *   Geoff Norton <gnorton@novell.com>
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml;
using System.Threading.Tasks;

using Mono.Cecil;
using Mono.Linker;
using Mono.Options;
using Mono.Tuner;
using MonoMac.Tuner;
using Xamarin.Utils;
using Xamarin.Linker;
using Registrar;
using ObjCRuntime;

namespace Xamarin.Bundler {
	public enum RegistrarMode {
		Default,
		Dynamic,
		PartialStatic,
		Static,
	}

	enum Action {
		None,
		Help,
		Version,
		RunRegistrar,
	}

	public static partial class Driver {
		internal const string NAME = "mmp";
		const string PRODUCT = "Xamarin.Mac";
		const string LOCAL_BUILD_DIR = "_mac-build";
		const string FRAMEWORK_LOCATION_VARIABLE = "XAMMAC_FRAMEWORK_PATH";
		internal static Application App = new Application (Environment.GetCommandLineArgs ());
		static Target BuildTarget;
		static List<string> references = new List<string> ();
		static List<string> resources = new List<string> ();
		static List<string> resolved_assemblies = new List<string> ();
		static List<string> ignored_assemblies = new List<string> ();
		public static List<string> native_references = new List<string> ();
		static List<string> native_libraries_copied_in = new List<string> ();

		static Action action;
		static string output_dir;
		static string app_name;
		static bool generate_plist;
		public static RegistrarMode Registrar { get { return App.Registrar; } private set { App.Registrar = value; } }
		static bool no_executable;
		static bool embed_mono = true;
		static bool? profiling = false;
		static List<string> link_flags;
		static LinkerOptions linker_options;
		static bool? disable_lldb_attach = null;
		static bool? disable_omit_fp = null;
		static string machine_config_path = null;
		static bool bypass_linking_checks = false;

		static string arch = "x86_64";
		static string contents_dir;
		static string frameworks_dir;
		static string macos_dir;
		static string resources_dir;
		static string mmp_dir;
		
		static string mono_dir;
		static string custom_bundle_name;

		static string tls_provider;
		static string http_message_provider;

		static string BundleName { get { return custom_bundle_name != null ? custom_bundle_name : "MonoBundle"; } }
		static string AppPath { get { return Path.Combine (macos_dir, app_name); } }
		public static string Arch => arch;

		static string icon;
		static string certificate_name;
		public static bool Force;

		static bool is_extension, is_xpc_service;
		static bool frameworks_copied_to_bundle_dir;    // Have we copied any frameworks to Foo.app/Contents/Frameworks?
		static bool dylibs_copied_to_bundle_dir => native_libraries_copied_in.Count > 0;

		const string pkg_config = "/Library/Frameworks/Mono.framework/Commands/pkg-config";

		static Version min_xcode_version = new Version (6, 0);

		static void ShowHelp (OptionSet os) {
			Console.WriteLine ("mmp - Xamarin.Mac Packer");
			Console.WriteLine ("Copyright 2010 Novell Inc.");
			Console.WriteLine ("Copyright 2011-2016 Xamarin Inc.");
			Console.WriteLine ("Usage: mmp [options] application-exe");
			os.WriteOptionDescriptions (Console.Out);
		}

		public static bool IsUnifiedFullXamMacFramework { get { return TargetFramework == TargetFramework.Xamarin_Mac_4_5_Full; } }
		public static bool IsUnifiedFullSystemFramework { get { return TargetFramework == TargetFramework.Xamarin_Mac_4_5_System; } }
		public static bool IsUnifiedMobile { get { return TargetFramework == TargetFramework.Xamarin_Mac_2_0_Mobile; } }
		public static bool LinkProhibitedFrameworks { get; private set; }

		public static bool Is64Bit { 
			get {
				return arch == "x86_64";
			}
		}

		public static string GetProductAssembly (Application app)
		{
			return "Xamarin.Mac";
		}

		public static string GetPlatformFrameworkDirectory (Application app)
		{
			if (IsUnifiedMobile)
				return Path.Combine (FrameworkLibDirectory, "mono", "Xamarin.Mac");
			else if (IsUnifiedFullXamMacFramework)
				return Path.Combine (FrameworkLibDirectory, "mono", "4.5");
			throw new InvalidOperationException ("PlatformFrameworkDirectory when not Mobile or Full?");
		}

		public static string GetArch32Directory (Application app)
		{
			throw new InvalidOperationException ("Arch32Directory when not Mobile or Full?");
		}
		
		public static string GetArch64Directory (Application app)
		{
			if (IsUnifiedMobile)
				return Path.Combine (FrameworkLibDirectory, "x86_64", "mobile");
			else if (IsUnifiedFullXamMacFramework)
				return Path.Combine (FrameworkLibDirectory, "x86_64", "full");
			throw new InvalidOperationException ("Arch64Directory when not Mobile or Full?");
		}
					

		static AOTOptions aotOptions = null;

		public static bool EnableDebug {
			get { return App.EnableDebug; }
		}

		static int Main2 (string [] args)
		{
			var os = new OptionSet () {
				{ "f|force", "Forces the recompilation of code, regardless of timestamps", v=> Force = true },
				{ "cache=", "Specify the directory where temporary build files will be cached", v => App.Cache.Location = v },
				{ "a|assembly=", "Add an assembly to be processed", v => references.Add (v) },
				{ "r|resource=", "Add a resource to be included", v => resources.Add (v) },
				{ "o|output=", "Specify the output path", v => output_dir = v },
				{ "n|name=", "Specify the application name", v => app_name = v },
				{ "d|debug", "Build a debug bundle", v => App.EnableDebug = true },
				{ "s|sgen:", "Use the SGen Garbage Collector",
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
					true // do not show this option anymore
				},
				{ "nolink", "Do not link the assemblies", v => App.LinkMode = LinkMode.None },
				{ "mapinject", "Inject a fast method map [deprecated]", v => { ErrorHelper.Show (new MonoMacException (16, false, Errors.MX0016,  "--mapinject")); } },
				{ "minos=", "Minimum supported version of Mac OS X", 
					v => {
						try {
							App.DeploymentTarget = StringUtils.ParseVersion (v);
						} catch (Exception ex) {
							ErrorHelper.Error (26, ex, Errors.MX0026, $"minos:{v}", ex.Message);
						}
					}
				},
				{ "linkplatform", "Link only the Xamarin.Mac.dll platform assembly", v => App.LinkMode = LinkMode.Platform },
				{ "linksdkonly", "Link only the SDK assemblies", v => App.LinkMode = LinkMode.SDKOnly },
				{ "linkskip=", "Skip linking of the specified assembly", v => App.LinkSkipped.Add (v) },
				{ "i18n=", "List of i18n assemblies to copy to the output directory, separated by commas (none,all,cjk,mideast,other,rare,west)", v => App.I18n = LinkerOptions.ParseI18nAssemblies (v) },
				{ "c|certificate=", "The Code Signing certificate for the application", v => { certificate_name = v; }},
				{ "p", "Generate a plist for the application", v => { generate_plist = true; }},
				{ "i|icon=", "Use the specified file as the bundle icon", v => { icon = v; }},
				{ "xml=", "Provide an extra XML definition file to the linker", v => App.Definitions.Add (v) },
				{ "time", v => WatchLevel++ },
				{ "arch=", "Specify the architecture ('x86_64') of the native runtime (default to 'x86_64', which is the only valid value)", v => { arch = v; } },
				{ "profile=", "(Obsoleted in favor of --target-framework) Specify the .NET profile to use", v => SetTargetFramework (v), true },
				{ "force-thread-check", "Keep UI thread checks inside (even release) builds [DEPRECATED, use --optimize=-remove-uithread-checks instead]", v => { App.Optimizations.RemoveUIThreadChecks = false; }, true},
				{ "disable-thread-check", "Remove UI thread checks inside (even debug) builds [DEPRECATED, use --optimize=remove-uithread-checks instead]", v => { App.Optimizations.RemoveUIThreadChecks = true; }, true},
				{ "registrar:", "Specify the registrar to use (dynamic [default], static, partial)", v => {
						switch (v) {
						case "static":
							Registrar = RegistrarMode.Static;
							break;
						case "dynamic":
							Registrar = RegistrarMode.Dynamic;
							break;
						case "partial":
						case "partial-static":
							Registrar = RegistrarMode.PartialStatic;
							break;
						case "il":
							Registrar = RegistrarMode.Dynamic;
							break;
						case "default":
							Registrar = RegistrarMode.Default;
							break;
						default:
							throw new MonoMacException (20, true, Errors.MX0020, "--registrar", "dynamic, static, partial, or default");
						}
					}
				},
				{ "sdk=", "Specifies the SDK version to compile against (version, for example \"10.9\")",
					v => {
						try {
							App.SdkVersion = StringUtils.ParseVersion (v);
						} catch (Exception ex) {
							ErrorHelper.Error (26, ex, Errors.MX0026, $"sdk:{v}", ex.Message);
						}
					}
				},
				{ "no-root-assembly", "Specifies that mmp will not process a root assembly. This is if the app needs to be packaged with a different directory structure than what mmp supports.", v => no_executable = true },
				{ "embed-mono:", "Specifies whether the app will embed the Mono runtime, or if it will use the system Mono found at runtime (default: true).", v => {
						embed_mono = ParseBool (v, "embed-mono");
					}
				},
				{ "link_flags=", "Specifies additional arguments to the native linker.",
					v => {
						if (!StringUtils.TryParseArguments (v, out var lf, out var ex))
							throw ErrorHelper.CreateError (26, ex, Errors.MX0026, $"-link_flags={v}", ex.Message);
						if (link_flags == null)
							link_flags = new List<string> ();
						link_flags.AddRange (lf);
					}
				},
				{ "ignore-native-library=", "Add a native library to be ignored during assembly scanning and packaging",
					v => ignored_assemblies.Add (v)
				},
				{ "native-reference=", "Add a native (static, dynamic, or framework) library to be included in the bundle. Can be specified multiple times.",
					v => {
						native_references.Add (v);
						if (v.EndsWith (".framework", true, CultureInfo.InvariantCulture))
							App.Frameworks.Add (v);
					}
				},
				{ "profiling:", "Enable profiling", v => profiling = ParseBool (v, "profiling") },
				{ "custom_bundle_name=", "Specify a custom name for the MonoBundle folder.", v => custom_bundle_name = v, true }, // Hidden hack for "universal binaries"
				{ "tls-provider=", "Specify the default TLS provider", v => { tls_provider = v; }},
				{ "http-message-handler=", "Specify the default HTTP Message Handler", v => { http_message_provider = v; }},
				{ "extension", "Specifies an app extension", v => is_extension = true },
				{ "xpc", "Specifies an XPC service", v => { is_extension = true; is_xpc_service = true; }},
				{ "allow-unsafe-gac-resolution", "Allow MSBuild to resolve from the System GAC", v => {} , true }, // Used in Xamarin.Mac.XM45.targets and must be ignored here. Hidden since it is a total hack. If you can use it, you don't need support
				{ "force-unsupported-linker", "Bypass safety checkes preventing unsupported linking options.", v => bypass_linking_checks = true , true }, // Undocumented option for a reason, You get to keep the pieces when it breaks
				{ "disable-lldb-attach=", "Disable automatic lldb attach on crash", v => disable_lldb_attach = ParseBool (v, "disable-lldb-attach")},
				{ "disable-omit-fp=", "Disable a JIT optimization where the frame pointer is omitted from the stack. This is optimization is disabled by default for debug builds.", v => disable_omit_fp = ParseBool (v, "disable-omit-fp") },
				{ "machine-config=", "Custom machine.config file to copy into MonoBundle/mono/4.5/machine.config. Pass \"\" to copy in a valid \"empty\" config file.", v => machine_config_path = v },
				{ "runregistrar:", "Runs the registrar on the input assembly and outputs a corresponding native library.",
					v => {
						action = Action.RunRegistrar;
						App.RegistrarOutputLibrary = v;
					},
					true /* this is an internal option */
				},
				{ "xamarin-framework-directory=", "The framework directory", v => { framework_dir = v; }, true },
				{ "xamarin-full-framework", "Used with --target-framework=4.5 to select XM Full Target Framework. Deprecated, use --target-framework=Xamarin.Mac,Version=v4.0,Profile=Full instead.", v => { TargetFramework = TargetFramework.Xamarin_Mac_4_5_Full; }, true },
				{ "xamarin-system-framework", "Used with --target-framework=4.5 to select XM Full Target Framework. Deprecated, use --target-framework=Xamarin.Mac,Version=v4.0,Profile=System instead.", v => { TargetFramework = TargetFramework.Xamarin_Mac_4_5_System; }, true },
				{ "aot:", "Specify assemblies that should be AOT compiled\n- none - No AOT (default)\n- all - Every assembly in MonoBundle\n- core - Xamarin.Mac, System, mscorlib\n- sdk - Xamarin.Mac.dll and BCL assemblies\n- |hybrid after option enables hybrid AOT which allows IL stripping but is slower (only valid for 'all')\n - Individual files can be included for AOT via +FileName.dll and excluded via -FileName.dll\n\nExamples:\n  --aot:all,-MyAssembly.dll\n  --aot:core,+MyOtherAssembly.dll,-mscorlib.dll",
					v => {
						aotOptions = new AOTOptions (v);
					}
				},
				{ "link-prohibited-frameworks", "Natively link against prohibited (rejected by AppStore) frameworks", v => { LinkProhibitedFrameworks = true; } },
				{ "warn-on-type-ref=", "Warn if any of the comma-separated types is referenced by assemblies - both before and after linking", v => {
						App.WarnOnTypeRef.AddRange (v.Split (new char [] { ',' }, StringSplitOptions.RemoveEmptyEntries));
					}
				},
			};

			var extra_args = Environment.GetEnvironmentVariable ("MMP_ENV_OPTIONS");
			if (!string.IsNullOrEmpty (extra_args)) {
				var l = new List<string> (args);
				l.AddRange (extra_args.Split (new char [] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
				args = l.ToArray ();
			}

			if (ParseOptions (App, os, args, ref action))
				return 0;

			if (aotOptions == null) {
				string forceAotVariable = Environment.GetEnvironmentVariable ("XM_FORCE_AOT");
				if (forceAotVariable != null)
					aotOptions = new AOTOptions (forceAotVariable);
			}

			App.RuntimeOptions = RuntimeOptions.Create (App, http_message_provider, tls_provider);


			if (IsUnifiedFullSystemFramework) {
				// With newer Mono builds, the system assemblies passed to us by msbuild are
				// no longer safe to copy into the bundle. They are stripped "fake" BCL
				// copies. So we redirect to the "real" ones. Thanks TargetFrameworkDirectories :(
				Regex monoAPIRegex = new Regex("lib/mono/.*-api/", RegexOptions.IgnoreCase);
				Regex monoAPIFacadesRegex = new Regex("lib/mono/.*-api/Facades/", RegexOptions.IgnoreCase);
				FixReferences (x => monoAPIRegex.IsMatch (x) && !monoAPIFacadesRegex.IsMatch (x), x => x.Replace(monoAPIRegex.Match(x).Value, "lib/mono/4.5/"));
			}

			if (Registrar == RegistrarMode.PartialStatic && App.LinkMode != LinkMode.None)
				throw new MonoMacException (2110, true, Errors.MM2110);

			// sanity check as this should never happen: we start out by not setting any
			// Unified/Classic properties, and only IsUnifiedMobile if we are are on the
			// XM framework. If we are not, we set IsUnifiedFull to true iff we detect
			// an explicit reference to the full unified Xamarin.Mac assembly; that is
			// only one of IsUnifiedMobile or IsUnifiedFull should ever be true. IsUnified
			// is true if one of IsUnifiedMobile or IsUnifiedFull is true; IsClassic is
			// implied if IsUnified is not true;
			int IsUnifiedCount = IsUnifiedMobile ? 1 : 0;
			if (IsUnifiedFullSystemFramework)
				IsUnifiedCount++;
			if (IsUnifiedFullXamMacFramework)
				IsUnifiedCount++;
			if (IsUnifiedCount != 1)
				throw ErrorHelper.CreateError (99, Errors.MX0099 ,"IsClassic/IsUnified/IsUnifiedMobile/IsUnifiedFullSystemFramework/IsUnifiedFullXamMacFramework logic regression");

			ValidateXamarinMacReference ();
			if (!bypass_linking_checks && (IsUnifiedFullSystemFramework || IsUnifiedFullXamMacFramework)) {
				switch (App.LinkMode) {
				case LinkMode.None:
				case LinkMode.Platform:
					break;
				default:
					throw new MonoMacException (2007, true, Errors.MM2007);
				}
			}

			ValidateXcode (false, true);

			App.Initialize ();

			// InitializeCommon needs SdkVersion set to something valid
			ValidateSDKVersion ();

			if (!Is64Bit)
				throw ErrorHelper.CreateError (144, Errors.MM0144);

			// InitializeCommon needs the current profile
			if (IsUnifiedFullXamMacFramework || IsUnifiedFullSystemFramework)
				Profile.Current = new XamarinMacProfile ();
			else
				Profile.Current = new MacMobileProfile ();

			BuildTarget = new Target (App);
			App.Targets.Add (BuildTarget);
			App.InitializeCommon ();

			Log ("Xamarin.Mac {0}.{1}", Constants.Version, Constants.Revision);
			Log (1, "Selected target framework: {0}; API: Unified", targetFramework);
			Log (1, $"Selected Linking: '{App.LinkMode}'");

			if (action == Action.RunRegistrar) {
				App.Registrar = RegistrarMode.Static;
				App.RunRegistrar ();
				return 0;
			}
			try {
				Pack (App.RootAssemblies);
			} finally {
				if (App.Cache.IsCacheTemporary) {
					// If we used a temporary directory we created ourselves for the cache
					// (in which case it's more a temporary location where we store the 
					// temporary build products than a cache), it will not be used again,
					// so just delete it.
					try {
						Directory.Delete (App.Cache.Location, true);
					} catch {
						// Don't care.
					}
				} else {
					// Write the cache data as the last step, so there is no half-done/incomplete (but yet detected as valid) cache.
					App.Cache.ValidateCache ();
				}
			}

			Log ("bundling complete");
			return 0;
		}

		static void ValidateXamarinMacReference ()
		{
			// Many Xamarin.Mac references are technically valid, so whitelisting risks breaking working project
			// However, passing in Mobile / Xamarin.Mac folders and resolving full/4.5 or vice versa is 
			// far from expected. So catch the common cases if we can
			string reference = references.FirstOrDefault (x => x.EndsWith ("Xamarin.Mac.dll", StringComparison.Ordinal));
			if (reference != null) {
				bool valid = true;
				if (IsUnifiedMobile)
					valid = !reference.Contains ("full/") && !reference.Contains ("4.5/");
				else if (IsUnifiedFullXamMacFramework || IsUnifiedFullSystemFramework)
					valid = !reference.Contains ("mobile/") && !reference.Contains ("Xamarin.Mac/");
				if (!valid)
					throw ErrorHelper.CreateError (1407, Errors.MM1407, reference, TargetFramework);
			}
		}

		static void FixReferences (Func<string, bool> match, Func<string, string> fix)
		{
			var assembliesToFix = references.Where (x => match(x)).ToList ();
			references = references.Except (assembliesToFix).ToList ();
			var fixedAssemblies = assembliesToFix.Select (x => fix(x));
			references.AddRange (fixedAssemblies);
		}

		// SDK versions are only passed in as X.Y but some frameworks/APIs require X.Y.Z
		// Mutate them if we have a new enough Xcode
		static Version MutateSDKVersionToPointRelease (Version rv)
		{
			if (rv.Major == 10 && (rv.Revision == 0 || rv.Revision == -1)) {
				if (rv.Minor == 15 && XcodeVersion >= new Version (11, 4))
					return new Version (rv.Major, rv.Minor, 4);
				if (rv.Minor == 13 && XcodeVersion >= new Version (9, 3))
					return new Version (rv.Major, rv.Minor, 4);
				if (rv.Minor == 13 && XcodeVersion >= new Version (9, 2))
					return new Version (rv.Major, rv.Minor, 2);
				if (rv.Minor == 13 && XcodeVersion >= new Version (9, 1))
					return new Version (rv.Major, rv.Minor, 1);
				if (rv.Minor == 12 && XcodeVersion >= new Version (8, 3))
					return new Version (rv.Major, rv.Minor, 4);
				if (rv.Minor == 12 && XcodeVersion >= new Version (8, 2))
					return new Version (rv.Major, rv.Minor, 2);
				if (rv.Minor == 12 && XcodeVersion >= new Version (8, 1))
					return new Version (rv.Major, rv.Minor, 1);
				if (rv.Minor == 11 && XcodeVersion >= new Version (7, 3))
					return new Version (rv.Major, rv.Minor, 4);
			}
			// Since Version has wrong behavior:
			// new Version (10, 14) < new Version (10, 14, 0) => true
			// Force any unset revision to 0 instead of -1
			if (rv.Revision == -1)
				return new Version (rv.Major, rv.Minor, 0);
			return rv;
		}

		// Validates that sdk_version is set to a reasonable value before compile
		static void ValidateSDKVersion ()
		{
			if (App.SdkVersion != null) {
				// We can't do mutation while parsing command line args as XcodeVersion isn't set yet
				App.SdkVersion = MutateSDKVersionToPointRelease (App.SdkVersion);
				return;
			}

			if (string.IsNullOrEmpty (DeveloperDirectory))
				return;

			var sdks = new List<Version> ();
			var sdkdir = Path.Combine (DeveloperDirectory, "Platforms", "MacOSX.platform", "Developer", "SDKs");
			foreach (var sdkpath in Directory.GetDirectories (sdkdir)) {
				var sdk = Path.GetFileName (sdkpath);
				if (sdk.StartsWith ("MacOSX", StringComparison.Ordinal) && sdk.EndsWith (".sdk", StringComparison.Ordinal)) {
					Version sdkVersion;
					if (Version.TryParse (sdk.Substring (6, sdk.Length - 10), out sdkVersion))
						sdks.Add (sdkVersion);
				}
			}
			if (sdks.Count > 0) {
				sdks.Sort ();
				// select the highest.
				App.SdkVersion = MutateSDKVersionToPointRelease (sdks [sdks.Count - 1]);
			}
		}

		public static Frameworks GetFrameworks (Application app)
		{
			return Frameworks.MacFrameworks;
		}

		static void CheckForUnknownCommandLineArguments (IList<Exception> exceptions, IList<string> arguments)
		{
			for (int i = arguments.Count - 1; i >= 0; i--) {
				if (arguments [i].StartsWith ("-", StringComparison.Ordinal)) {
					exceptions.Add (ErrorHelper.CreateError (18, Errors.MX0018, arguments [i]));
					arguments.RemoveAt (i);
				}
			}
		}

		public static void SelectRegistrar ()
		{
			if (Registrar == RegistrarMode.Default) {
				if (!App.EnableDebug)
					Registrar = RegistrarMode.Static;
				else if (App.LinkMode == LinkMode.None && embed_mono && App.IsDefaultMarshalManagedExceptionMode && File.Exists (PartialStaticLibrary))
					Registrar = RegistrarMode.PartialStatic;
				else
					Registrar = RegistrarMode.Dynamic;
				Log (1, $"Defaulting registrar to '{Registrar}'");
			}
		}

		static void Pack (IList<string> unprocessed)
		{
			string fx_dir = null;
			string root_assembly = null;
			var native_libs = new Dictionary<string, List<MethodDefinition>> ();

			if (no_executable) {
				if (unprocessed.Count != 0) {
					var exceptions = new List<Exception> ();

					CheckForUnknownCommandLineArguments (exceptions, unprocessed);

					exceptions.Add (new MonoMacException (50, true, Errors.MM0050, unprocessed.Count, string.Join ("', '", unprocessed.ToArray ())));

					throw new AggregateException (exceptions);
				}

				if (string.IsNullOrEmpty (output_dir))
					throw new MonoMacException (51, true, Errors.MM0051);

				if (string.IsNullOrEmpty (app_name))
					app_name = Path.GetFileNameWithoutExtension (output_dir);
			} else {
				if (unprocessed.Count != 1) {
					var exceptions = new List<Exception> ();

					CheckForUnknownCommandLineArguments (exceptions, unprocessed);

					if (unprocessed.Count > 1) {
						exceptions.Add (ErrorHelper.CreateError (8, Errors.MM0008, unprocessed.Count, string.Join ("', '", unprocessed.ToArray ())));
					} else if (unprocessed.Count == 0) {
						exceptions.Add (ErrorHelper.CreateError (17, Errors.MX0017));
					}

					throw new AggregateException (exceptions);
				}

				root_assembly = unprocessed [0];
				if (!File.Exists (root_assembly))
					throw new MonoMacException (7, true, Errors.MX0007, root_assembly);
				
				string root_wo_ext = Path.GetFileNameWithoutExtension (root_assembly);
				if (Profile.IsSdkAssembly (root_wo_ext) || Profile.IsProductAssembly (root_wo_ext))
					throw new MonoMacException (3, true, Errors.MX0003, root_wo_ext);

				if (references.Exists (a => Path.GetFileNameWithoutExtension (a).Equals (root_wo_ext)))
					throw new MonoMacException (23, true, Errors.MM0023, root_wo_ext);

				string monoFrameworkDirectory;

				if (TargetFramework == TargetFramework.Xamarin_Mac_2_0_Mobile) {
					monoFrameworkDirectory = TargetFramework.Identifier;
				} else {
					monoFrameworkDirectory = "4.5";
				}

				fx_dir = Path.Combine (MonoDirectory, "lib", "mono", monoFrameworkDirectory);

				if (!Directory.Exists (fx_dir))
					throw new MonoMacException (1403, true, Errors.MM1403, "Directory", fx_dir, TargetFramework);

				references.Add (root_assembly);
				BuildTarget.Resolver.CommandLineAssemblies = references;

				if (string.IsNullOrEmpty (app_name))
					app_name = root_wo_ext;
			
				if (string.IsNullOrEmpty (output_dir))
					output_dir = Environment.CurrentDirectory;
			}

			CreateDirectoriesIfNeeded ();

			switch (arch) {
			case "x86_64":
				BuildTarget.Abis = new List<Abi> { Abi.x86_64 };
				break;
			default:
				throw new MonoMacException (5205, true, Errors.MM5205, arch);
			}

			Watch ("Setup", 1);

			if (!no_executable) {
				BuildTarget.Resolver.FrameworkDirectory = fx_dir;
				BuildTarget.Resolver.RootDirectory = Path.GetDirectoryName (Path.GetFullPath (root_assembly));
				GatherAssemblies ();
				CheckReferences ();

				if (!is_extension && !resolved_assemblies.Exists (f => Path.GetExtension (f).ToLower () == ".exe") && !App.Embeddinator)
					throw new MonoMacException (79, true, Errors.MM0079, "");

				// i18n must be dealed outside linking too (e.g. bug 11448)
				if (App.LinkMode == LinkMode.None)
					CopyI18nAssemblies (App.I18n);

				CopyAssemblies ();
				Watch ("Copy Assemblies", 1);
			}

			CopyResources ();
			Watch ("Copy Resources", 1);

			CopyConfiguration ();
			Watch ("Copy Configuration", 1);

			ExtractNativeLinkInfo ();

			BuildTarget.ValidateAssembliesBeforeLink ();

			if (!no_executable) {
				foreach (var nr in native_references) {
					if (!native_libs.ContainsKey (nr))
						native_libs.Add (nr, null);
				}

				var linked_native_libs = Link ();
				foreach (var kvp in linked_native_libs) {
					List<MethodDefinition> methods;
					if (native_libs.TryGetValue (kvp.Key, out methods)) {
						if (methods == null) {
							methods = new List<MethodDefinition> (); 
							native_libs [kvp.Key] = methods;
						}
						methods.AddRange (kvp.Value);
					}
					else {
						native_libs.Add (kvp.Key, kvp.Value);
					}
				}
				Watch (string.Format ("Linking (mode: '{0}')", App.LinkMode), 1);
			}

			// These must occur _after_ Linking
			BuildTarget.CollectAllSymbols ();
			BuildTarget.ComputeLinkerFlags ();
			BuildTarget.GatherFrameworks ();

			CopyMonoNative ();

			CopyDependencies (native_libs);
			Watch ("Copy Dependencies", 1);

			// MDK check
			Compile ();
			Watch ("Compile", 1);
			
			if (generate_plist)
				GeneratePList ();

			if (App.LinkMode != LinkMode.All && App.RuntimeOptions != null)
				App.RuntimeOptions.Write (resources_dir);

			if (aotOptions != null && aotOptions.IsAOT) {
				AOTCompilerType compilerType;
				if (IsUnifiedMobile || IsUnifiedFullXamMacFramework)
					compilerType = AOTCompilerType.Bundled64;
				else if (IsUnifiedFullSystemFramework)
					compilerType = AOTCompilerType.System64;
				else
					throw ErrorHelper.CreateError (0099, Errors.MX0099, "\"AOT with unexpected profile.\"");

				AOTCompiler compiler = new AOTCompiler (aotOptions, compilerType, IsUnifiedMobile, !EnableDebug);
				compiler.Compile (mmp_dir);
				Watch ("AOT Compile", 1);
			}

			if (!string.IsNullOrEmpty (certificate_name)) {
				CodeSign ();
				Watch ("Code Sign", 1);
			}
		}

		static void CopyMonoNative ()
		{
			string name;
			if (File.Exists (Path.Combine (MonoDirectory, "lib", "libmono-system-native.dylib"))) {
				// legacy libmono-system-native.a needs to be included if it exists in the mono in question
				name = "libmono-system-native";
			} else {
				// use modern libmono-native
				switch (BuildTarget.MonoNativeMode) {
				case MonoNativeMode.Unified:
					name = "libmono-native-unified";
					break;
				case MonoNativeMode.Compat:
					name = "libmono-native-compat";
					break;
				default:
					throw ErrorHelper.CreateError (99, Errors.MX0099, $"Invalid mono native type: '{BuildTarget.MonoNativeMode}'");
				}
			}

			var src = Path.Combine (MonoDirectory, "lib", name + ".dylib");
			var dest = Path.Combine (mmp_dir, "libmono-native.dylib");
			Watch ($"Adding mono-native library {name} for {BuildTarget.MonoNativeMode}.", 1);

			if (App.Optimizations.TrimArchitectures == true) {
				// copy to temp directory and lipo there to avoid touching the final dest file if it's up to date
				var temp_dest = Path.Combine (App.Cache.Location, "libmono-native.dylib");

				if (Application.UpdateFile (src, temp_dest))
					LipoLibrary (name, temp_dest);
				Application.UpdateFile (temp_dest, dest);
			}
			else {
				// we can directly update the dest
				Application.UpdateFile (src, dest);
			}
		}

		static void ExtractNativeLinkInfo ()
		{
			var exceptions = new List<Exception> ();

			BuildTarget.ExtractNativeLinkInfo (exceptions);

			if (exceptions.Count > 0)
				throw new AggregateException (exceptions);

			Watch ("Extracted native link info", 1);
		}

		static string MonoDirectory {
			get {
				if (mono_dir == null) {
					if (IsUnifiedFullXamMacFramework || IsUnifiedMobile) {
						mono_dir = FrameworkDirectory;
					} else {
						var dir = new StringBuilder ();
						RunCommand (pkg_config, new [] { "--variable=prefix", "mono-2" }, dir);
						mono_dir = Path.GetFullPath (dir.ToString ().Replace (Environment.NewLine, String.Empty));
					}
				}
				return mono_dir;
			}
		}

		static void GeneratePList () {
			var sr = new StreamReader (typeof (Driver).Assembly.GetManifestResourceStream (App.Embeddinator ? "Info-framework.plist.tmpl" : "Info.plist.tmpl"));
			var all = sr.ReadToEnd ();
			var icon_str = (icon != null) ? "\t<key>CFBundleIconFile</key>\n\t<string>" + icon + "</string>\n\t" : "";
			var path = Path.Combine (App.Embeddinator ? resources_dir : contents_dir, "Info.plist");
			using (var sw = new StreamWriter (path)){
				sw.WriteLine (
					all.Replace ("@BUNDLEDISPLAYNAME@", app_name).
					Replace ("@EXECUTABLE@", app_name).
					Replace ("@BUNDLEID@", string.Format ("org.mono.bundler.{0}", app_name)).  
					Replace ("@BUNDLEICON@", icon_str).
					Replace ("@BUNDLENAME@", app_name).
					Replace ("@ASSEMBLY@", references.Where (e => Path.GetExtension (e) == ".exe").FirstOrDefault ()));
			}
		}	


		// the 'codesign' is provided with OSX, not with Xcode (no need to use xcrun)
		// note: by default the monodevelop addin does the signing (not mmp)
		static void CodeSign () {
			RunCommand ("codesign", String.Format ("-v -s \"{0}\" \"{1}\"", certificate_name, App.AppDirectory));
		}

		[DllImport (Constants.libSystemLibrary)]
		static extern int unlink (string pathname);

		[DllImport (Constants.libSystemLibrary)]
		static extern int symlink (string path1, string path2);

		[DllImport (Constants.libSystemLibrary)]
		static extern IntPtr realpath (string path, IntPtr buffer);

		static string GetRealPath (string path)
		{
			if (path.StartsWith ("@executable_path/", StringComparison.Ordinal))
				path = Path.Combine (mmp_dir, path.Substring (17));
			if (path.StartsWith ("@rpath/", StringComparison.Ordinal))
				path = Path.Combine (mmp_dir, path.Substring (7));

			const int PATHMAX = 1024 + 1;
			IntPtr buffer = IntPtr.Zero;
			try {
				buffer = Marshal.AllocHGlobal (PATHMAX);

				var result = realpath (path, buffer);

				if (result == IntPtr.Zero)
					return path;
				else
					return Marshal.PtrToStringAuto (buffer);
			}
			finally {
				if (buffer != IntPtr.Zero)
					Marshal.FreeHGlobal (buffer);
			}
		}

		static string PartialStaticLibrary {
			get {
				return Path.Combine (FrameworkLibDirectory, string.Format ("mmp/Xamarin.Mac.registrar.{0}.a", IsUnifiedMobile ? "mobile" : "full"));
			}
		}

		static string GenerateMain ()
		{
			var sb = new StringBuilder ();
			using (var sw = new StringWriter (sb)) {
				sw.WriteLine ("#define MONOMAC 1");
				sw.WriteLine ("#include <xamarin/xamarin.h>");
				sw.WriteLine ("#import <AppKit/NSAlert.h>");
				sw.WriteLine ("#import <Foundation/NSDate.h>"); // 10.7 wants this even if not needed on 10.9
				if (Driver.Registrar == RegistrarMode.PartialStatic)
					sw.WriteLine ("extern \"C\" void xamarin_create_classes_Xamarin_Mac ();");
				sw.WriteLine ();
				sw.WriteLine ();
				sw.WriteLine ();
				sw.WriteLine ("extern \"C\" int xammac_setup ()");

				sw.WriteLine ("{");
				if (custom_bundle_name != null) {
					sw.WriteLine ("extern NSString* xamarin_custom_bundle_name;");
					sw.WriteLine ("\txamarin_custom_bundle_name = @\"" + custom_bundle_name + "\";");
				}
				if (!App.IsDefaultMarshalManagedExceptionMode)
					sw.WriteLine ("\txamarin_marshal_managed_exception_mode = MarshalManagedExceptionMode{0};", App.MarshalManagedExceptions);
				sw.WriteLine ("\txamarin_marshal_objectivec_exception_mode = MarshalObjectiveCExceptionMode{0};", App.MarshalObjectiveCExceptions);
				if (disable_lldb_attach.HasValue ? disable_lldb_attach.Value : !App.EnableDebug)
					sw.WriteLine ("\txamarin_disable_lldb_attach = true;");
				if (disable_omit_fp ?? App.EnableDebug)
					sw.WriteLine ("\txamarin_disable_omit_fp = true;");
				sw.WriteLine ();


				if (Driver.Registrar == RegistrarMode.Static)
					sw.WriteLine ("\txamarin_create_classes ();");
				else if (Driver.Registrar == RegistrarMode.PartialStatic)
					sw.WriteLine ("\txamarin_create_classes_Xamarin_Mac ();");

				if (App.EnableDebug)
					sw.WriteLine ("\txamarin_debug_mode = TRUE;");

				if (App.EnableSGenConc)
					sw.WriteLine ("\tsetenv (\"MONO_GC_PARAMS\", \"major=marksweep-conc\", 1);");
				else
					sw.WriteLine ("\tsetenv (\"MONO_GC_PARAMS\", \"major=marksweep\", 1);");

				sw.WriteLine ("\txamarin_supports_dynamic_registration = {0};", App.DynamicRegistrationSupported ? "TRUE" : "FALSE");

				if (aotOptions != null && aotOptions.IsHybridAOT)
					sw.WriteLine ("\txamarin_mac_hybrid_aot = TRUE;");

				if (IsUnifiedMobile)
					sw.WriteLine ("\txamarin_mac_modern = TRUE;");

				sw.WriteLine ("\treturn 0;");
				sw.WriteLine ("}");
				sw.WriteLine ();
			}

			return sb.ToString ();
		}

		static void HandleFramework (IList<string> args, string framework, bool weak)
		{
			string name = Path.GetFileName (framework);
			if (name.Contains ('.'))
				name = name.Remove (name.IndexOf (".", StringComparison.Ordinal));
			string path = Path.GetDirectoryName (framework);
			if (!string.IsNullOrEmpty (path)) {
				args.Add ("-F");
				args.Add (path);
			}
		
			if (weak)
				BuildTarget.WeakFrameworks.Add (name);
			else
				BuildTarget.Frameworks.Add (name);

			if (!framework.EndsWith (".framework", StringComparison.Ordinal))
				return;

			// TODO - There is a chunk of code in mtouch that calls Xamarin.MachO.IsDynamicFramework and doesn't copy if framework of static libs
			// But IsDynamicFramework is not on XM yet

			CreateDirectoryIfNeeded (frameworks_dir);
			Application.UpdateDirectory (framework, frameworks_dir);
			frameworks_copied_to_bundle_dir = true;

			if (App.Optimizations.TrimArchitectures == true)
				LipoLibrary (framework, Path.Combine (name, Path.Combine (frameworks_dir, name + ".framework", name)));
		}

		static void CheckSystemMonoVersion ()
		{
			string mono_version;

			var versionFile = "/Library/Frameworks/Mono.framework/Versions/Current/VERSION";
			if (File.Exists (versionFile)) {
				mono_version = File.ReadAllText (versionFile);
			} else {
				mono_version = RunPkgConfig ("--modversion", force_system_mono: true);
			}

			mono_version = mono_version.Trim ();

			if (!Version.TryParse (mono_version, out var mono_ver))
				return;

			if (mono_ver < MonoVersions.MinimumMonoVersion)
				throw ErrorHelper.CreateError (1, Errors.MM0001, MonoVersions.MinimumMonoVersion, mono_version);
		}

		static void Compile ()
		{
			string [] cflags = Array.Empty<string> ();

			string mainSource = GenerateMain ();
			string registrarPath = null;

			CheckSystemMonoVersion ();

			if (Registrar == RegistrarMode.Static) {
				registrarPath = Path.Combine (App.Cache.Location, "registrar.m");
				var registrarH = Path.Combine (App.Cache.Location, "registrar.h");
				BuildTarget.StaticRegistrar.Generate (BuildTarget.Resolver.ResolverCache.Values, registrarH, registrarPath);

				var platform_assembly = BuildTarget.Resolver.ResolverCache.First ((v) => v.Value.Name.Name == BuildTarget.StaticRegistrar.PlatformAssembly).Value;
				Frameworks.Gather (App, platform_assembly, BuildTarget.Frameworks, BuildTarget.WeakFrameworks);
			}

			var str_cflags = RunPkgConfig ("--cflags");
			var libdir = RunPkgConfig ("--variable=libdir");

			if (!StringUtils.TryParseArguments (str_cflags, out cflags, out var ex))
				throw ErrorHelper.CreateError (147, ex, Errors.MM0147, str_cflags, ex.Message);

			var libmain = embed_mono ? "libxammac" : "libxammac-system";
			var libxammac = Path.Combine (FrameworkLibDirectory, libmain + (App.EnableDebug ? "-debug" : "") + ".a");

			if (!File.Exists (libxammac))
				throw new MonoMacException (5203, true, Errors.MM5203, libxammac);

			try {
				var args = new List<string> ();
				if (App.EnableDebug)
					args.Add ("-g");
				if (App.Embeddinator) {
					args.Add ("-shared");
					args.Add ("-install_name");
					args.Add ($"@loader_path/../Frameworks/{App.Name}.framework/{App.Name}");
				}
				args.Add ($"-mmacosx-version-min={App.DeploymentTarget.ToString ()}");
				args.Add ("-arch");
				args.Add (arch);
				if (arch == "x86_64")
					args.Add ($"-fobjc-runtime=macosx-{App.DeploymentTarget.ToString ()}");
				if (!embed_mono)
					args.Add ("-DDYNAMIC_MONO_RUNTIME");

				if (XcodeVersion >= new Version (9, 0))
					args.Add ("-Wno-unguarded-availability-new");

				if (XcodeVersion.Major >= 11)
					args.Add ("-std=c++14");

				bool appendedObjc = false;
				var sourceFiles = new List<string> ();
				foreach (var assembly in BuildTarget.Assemblies) {
					if (assembly.LinkWith != null) {
						foreach (var linkWith in assembly.LinkWith) {
							Log (2, "Found LinkWith on {0} for {1}", assembly.FileName, linkWith);
							if (linkWith.EndsWith (".dylib", StringComparison.Ordinal)) {
								// Link against the version copied into MonoBudle, since we install_name_tool'd it already
								string libName = Path.GetFileName (linkWith);
								string finalLibPath = Path.Combine (mmp_dir, libName);
								args.Add (finalLibPath);
							}
							else {
								args.Add (linkWith);
							}
						}
						if (!appendedObjc) {
							appendedObjc = true;
							args.Add ("-ObjC");
						}
					}
					if (assembly.LinkerFlags != null)
						foreach (var linkFlag in assembly.LinkerFlags)
							args.Add (linkFlag);
					if (assembly.Frameworks != null) {
						foreach (var f in assembly.Frameworks) {
							Log (2, $"Adding Framework {f} for {assembly.FileName}");
							HandleFramework (args, f, false);
						}
					}
					if (assembly.WeakFrameworks != null) { 
						foreach (var f in assembly.WeakFrameworks) {
							Log (2, $"Adding Weak Framework {f} for {assembly.FileName}");
							HandleFramework (args, f, true);
						}
					}
				}

				foreach (var framework in App.Frameworks)
					HandleFramework (args, framework, false);

				foreach (var lib in native_libraries_copied_in)
				{
					// Link against the version copied into MonoBudle, since we install_name_tool'd it already
					string libName = Path.GetFileName (lib);
					string finalLibPath = Path.Combine (mmp_dir, libName);
					args.Add (finalLibPath);
				}

				if (frameworks_copied_to_bundle_dir) {
					args.Add ("-rpath");
					args.Add ("@loader_path/../Frameworks");
				}
				if (dylibs_copied_to_bundle_dir) {
					args.Add ("-rpath");
					args.Add ($"@loader_path/../{BundleName}");
				}

				if (is_extension && !is_xpc_service) {
					args.Add ("-e");
					args.Add ("_xamarin_mac_extension_main");
					args.Add ("-framework");
					args.Add ("NotificationCenter");
				}

				foreach (var f in BuildTarget.Frameworks) {
					args.Add ("-framework");
					args.Add (f);
				}
				foreach (var f in BuildTarget.WeakFrameworks) {
					args.Add ("-weak_framework");
					args.Add (f);
				}

				var requiredSymbols = BuildTarget.GetRequiredSymbols ();
				Driver.WriteIfDifferent (Path.Combine (App.Cache.Location, "exported-symbols-list"), string.Join ("\n", requiredSymbols.Select ((symbol) => symbol.Prefix + symbol.Name).ToArray ()));
				switch (App.SymbolMode) {
				case SymbolMode.Ignore:
					break;
				case SymbolMode.Code:
					string reference_m = Path.Combine (App.Cache.Location, "reference.m");
					reference_m = BuildTarget.GenerateReferencingSource (reference_m, requiredSymbols);
					if (!string.IsNullOrEmpty (reference_m))
						sourceFiles.Add (reference_m);
					break;
				case SymbolMode.Linker:
				case SymbolMode.Default:
					foreach (var symbol in requiredSymbols) {
						args.Add ("-u");
						args.Add (symbol.Prefix + symbol.Name);
					}
					break;
				default:
					throw ErrorHelper.CreateError (99, Errors.MX0099, $"invalid symbol mode: {App.SymbolMode}");
				}

				bool linkWithRequiresForceLoad = BuildTarget.Assemblies.Any (x => x.ForceLoad);
				if (no_executable || linkWithRequiresForceLoad)
					args.Add ("-force_load"); // make sure nothing is stripped away if we don't have a root assembly, since anything can end up being used.
				args.Add (libxammac);
				args.Add ("-o");
				args.Add (AppPath);
				args.AddRange (cflags);
				if (embed_mono) {
					string libmono = Path.Combine (libdir, "libmonosgen-2.0.a");

					if (!File.Exists (libmono))
						throw new MonoMacException (5202, true, Errors.MM5202);

					args.Add (libmono);

					string libmonoSystemNative = Path.Combine (libdir, "libmono-system-native.a");
					if (File.Exists (libmonoSystemNative)) {
						// legacy libmono-system-native.a needs to be included if it exists in the mono in question
						args.Add (libmonoSystemNative);
						args.Add ("-u");
						args.Add ("_SystemNative_RealPath"); // This keeps libmono_system_native_la-pal_io.o symbols
					} else {
						// add modern libmono-native
						string libmono_native_name;
						switch (BuildTarget.MonoNativeMode) {
						case MonoNativeMode.Unified:
							libmono_native_name = "libmono-native-unified";
							break;
						case MonoNativeMode.Compat:
							libmono_native_name = "libmono-native-compat";
							break;
						default:
							throw ErrorHelper.CreateError (99, Errors.MX0099, $"Invalid mono native type: '{BuildTarget.MonoNativeMode}'");
						}

						args.Add (Path.Combine (libdir, libmono_native_name + ".a"));
						args.Add ("-framework");
						args.Add ("GSS");
					}

					if (profiling.HasValue && profiling.Value) {
						args.Add (Path.Combine (libdir, "libmono-profiler-log.a"));
						args.Add ("-u");
						args.Add ("_mono_profiler_init_log");
					}
					args.Add ("-lz");
				}

				if (Registrar == RegistrarMode.PartialStatic) {
					args.Add (PartialStaticLibrary);
					args.Add ("-framework");
					args.Add ("Quartz");
				}

				args.Add ("-liconv");
				args.Add ("-lc++");
				args.Add ("-x");
				args.Add ("objective-c++");
				if (XcodeVersion.Major >= 10) {
					// Xcode 10 doesn't ship with libstdc++
					args.Add ("-stdlib=libc++");
				}
				args.Add ($"-I{Path.Combine (FrameworkDirectory, "include")}");
				if (registrarPath != null)
					args.Add (registrarPath);
				args.Add ("-fno-caret-diagnostics");
				args.Add ("-fno-diagnostics-fixit-info");
				if (link_flags != null)
					args.AddRange (link_flags);
				if (!string.IsNullOrEmpty (DeveloperDirectory))
				{
					var sysRootSDKVersion = new Version (App.SdkVersion.Major, App.SdkVersion.Minor); // Sys Root SDKs do not have X.Y.Z, just X.Y 
					args.Add ("-isysroot");
					args.Add (Path.Combine (DeveloperDirectory, "Platforms", "MacOSX.platform", "Developer", "SDKs", "MacOSX" + sysRootSDKVersion + ".sdk"));
				}

				if (App.RequiresPInvokeWrappers) {
					var state = linker_options.MarshalNativeExceptionsState;
					state.End ();
					args.Add (state.SourcePath);
				}

				var main = Path.Combine (App.Cache.Location, "main.m");
				File.WriteAllText (main, mainSource);
				sourceFiles.Add (main);
				args.AddRange (sourceFiles);

				RunClang (args);
			} catch (Win32Exception e) {
				throw new MonoMacException (5103, true, e, Errors.MM5103, "driver");
			}
		}

		static string RunPkgConfig (string option, bool force_system_mono = false)
		{
			string [] env = null;

			if (!File.Exists (pkg_config))
				throw ErrorHelper.CreateError (5313, Errors.MX5313);

			if (!IsUnifiedFullSystemFramework && !force_system_mono)
				env = new [] { "PKG_CONFIG_PATH", Path.Combine (FrameworkLibDirectory, "pkgconfig") };

			var sb = new StringBuilder ();
			int rv;
			try {
				rv = RunCommand (pkg_config, new [] { option, "mono-2" }, env, sb);
			} catch (Exception e) {
				throw ErrorHelper.CreateError (5314, e, Errors.MX5314, e.Message);
			}
			if (rv != 0)
				throw ErrorHelper.CreateError (5312, Errors.MX5312, rv);

			return sb.ToString ().Trim ();
		}

		// check that we have a reference to Xamarin.Mac.dll and not to MonoMac.dll.
		static void CheckReferences ()
		{
			List<Exception> exceptions = new List<Exception> ();
			var cache = BuildTarget.Resolver.ToResolverCache ();
			var incompatibleReferences = new List<string> ();
			var haveValidReference = false;

			foreach (string entry in cache.Keys) {
				switch (entry) {
				case "Xamarin.Mac":
					haveValidReference = true;
					break;
				case "XamMac":
					incompatibleReferences.Add (entry);
					break;
				case "MonoMac":
					incompatibleReferences.Add (entry);
					break;
				}
			}

			if (!haveValidReference)
				exceptions.Add (new MonoMacException (1401, true, Errors.MM1401));

			foreach (var refName in incompatibleReferences)
				exceptions.Add (new MonoMacException (1402, true, Errors.MM1402, refName + ".dll"));

			if (exceptions.Count > 0)
				throw new AggregateException (exceptions);
		}

		static IDictionary<string,List<MethodDefinition>> Link ()
		{
			var cache = (Dictionary<string, AssemblyDefinition>) BuildTarget.Resolver.ResolverCache;
			var resolver = cache != null
				? new Mono.Linker.AssemblyResolver (cache)
				: new Mono.Linker.AssemblyResolver ();

			resolver.AddSearchDirectory (BuildTarget.Resolver.RootDirectory);
			resolver.AddSearchDirectory (BuildTarget.Resolver.FrameworkDirectory);

			var options = new LinkerOptions {
				MainAssembly = BuildTarget.Resolver.GetAssembly (references [references.Count - 1]),
				OutputDirectory = mmp_dir,
				LinkSymbols = App.EnableDebug,
				LinkMode = App.LinkMode,
				Resolver = resolver,
				SkippedAssemblies = App.LinkSkipped,
				I18nAssemblies = App.I18n,
				ExtraDefinitions = App.Definitions,
				Architecture = arch,
				RuntimeOptions = App.RuntimeOptions,
				MarshalNativeExceptionsState = !App.RequiresPInvokeWrappers ? null : new PInvokeWrapperGenerator ()
				{
					App = App,
					SourcePath = Path.Combine (App.Cache.Location, "pinvokes.m"),
					HeaderPath = Path.Combine (App.Cache.Location, "pinvokes.h"),
					Registrar = (StaticRegistrar) BuildTarget.StaticRegistrar,
				},
				SkipExportedSymbolsInSdkAssemblies = !embed_mono,
				Target = BuildTarget,
				WarnOnTypeRef = App.WarnOnTypeRef,
			};

			linker_options = options;

			MonoMac.Tuner.Linker.Process (options, out var context, out resolved_assemblies);

			ErrorHelper.Show (context.Exceptions);

			// Idealy, this would be handled by Linker.Process above. However in the non-linking case
			// we do not run MobileMarkStep which generates the pinvoke list. Hack around this for now
			// https://bugzilla.xamarin.com/show_bug.cgi?id=43419
			if (App.LinkMode == LinkMode.None)
				return ProcessDllImports ();
			else
				return BuildTarget.LinkContext.PInvokeModules;
		}

		static Dictionary<string, List<MethodDefinition>> ProcessDllImports ()
		{
			var pinvoke_modules = new Dictionary<string, List<MethodDefinition>> ();

			foreach (string assembly_name in resolved_assemblies) {
				AssemblyDefinition assembly = BuildTarget.Resolver.GetAssembly (assembly_name);
				if (assembly != null) {
					foreach (ModuleDefinition md in assembly.Modules) {
						if (md.HasTypes) {
							foreach (TypeDefinition type in md.Types) {
								if (type.HasMethods) {
									foreach (MethodDefinition method in type.Methods) {
										if ((method != null) && !method.HasBody && method.IsPInvokeImpl) {
											// this happens for c++ assemblies (ref #11448)
											if (method.PInvokeInfo == null)
												continue;
											string module = method.PInvokeInfo.Module.Name;

											if (!String.IsNullOrEmpty (module)) {
												List<MethodDefinition> methods;
												if (!pinvoke_modules.TryGetValue (module, out methods))
													pinvoke_modules.Add (module, methods = new List<MethodDefinition> ());
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return pinvoke_modules;
		}

		static void CopyDependencies (IDictionary<string, List<MethodDefinition>> libraries)
		{
			// Process LinkWith first so we don't have unnecessary warnings
			foreach (var assembly in BuildTarget.Assemblies.Where (a => a.LinkWith != null)) {
				foreach (var linkWith in assembly.LinkWith.Where (l => l.EndsWith (".dylib", StringComparison.Ordinal))) {
					string libName = Path.GetFileName (linkWith);
					string finalLibPath = Path.Combine (mmp_dir, libName);
					Application.UpdateFile (linkWith, finalLibPath);
					RunInstallNameTool (new [] { "-id", "@executable_path/../" + BundleName + "/" + libName, finalLibPath });
					native_libraries_copied_in.Add (libName);
				}
			}

			var processed = new HashSet<string> ();
			foreach (var kvp in libraries.Where (l => !native_libraries_copied_in.Contains (Path.GetFileName (l.Key))))
				ProcessNativeLibrary (processed, kvp.Key, kvp.Value);

			// .dylibs might refers to specific paths (e.g. inside Mono framework directory) and/or
			// refer to a symlink (pointing to a more specific version of the library)
			var sb = new List<string> ();
			foreach (string library in Directory.GetFiles (mmp_dir, "*.dylib")) {
				foreach (string lib in Xamarin.MachO.GetNativeDependencies (library)) {
					if (lib.StartsWith ("/Library/Frameworks/Mono.framework/Versions/", StringComparison.Ordinal)) {
						string libname = Path.GetFileName (lib);
						string real_lib = GetRealPath (lib);
						string real_libname	= Path.GetFileName (real_lib);
						// if a symlink was specified then re-create it inside the .app
						if (libname != real_libname)
							CreateSymLink (mmp_dir, real_libname, libname);
						sb.Add ("-change");
						sb.Add (lib);
						sb.Add ("@executable_path/../" + BundleName + "/" + libname);
					}
				}
				// if required update the paths inside the .dylib that was copied
				if (sb.Count > 0) {
					sb.Add (library);
					RunInstallNameTool (sb);
					sb.Clear ();
				}
			}
		}

		static string GetLibraryShortenedName (string name)
		{
			// GetFileNameWithoutExtension only removes one extension, e.g. libx.so.2 won't work
			int start = name.StartsWith ("lib", StringComparison.Ordinal) ? 3 : 0;
			int end = name.Length;
			if (name.EndsWith (".dylib", StringComparison.Ordinal))
				end -= 6;
			else if (name.EndsWith (".so", StringComparison.Ordinal))
				end -= 3;
			else if (name.EndsWith (".dll", StringComparison.OrdinalIgnoreCase))
				end -= 4;
			return name.Substring (start, end - start);
		}

		static bool ShouldSkipNativeLibrary (string fileName)
		{
			string shortendName = GetLibraryShortenedName (fileName);

			// well known libraries we do not bundle or warn about
			switch (shortendName.ToLowerInvariant ()) {
			case "xammac":	// we have a p/invoke to this library in Runtime.mac.cs, for users that don't bundle with mmp.
			case "__internal":	// mono runtime
			case "kernel32":	// windows specific
			case "gdi32":		// windows specific
			case "ole32":		// windows specific
			case "user32":		// windows specific
			case "advapi32":	// windows specific
			case "crypt32":		// windows specific
			case "msvcrt":		// windows specific
			case "iphlpapi":	// windows specific
			case "winmm":		// windows specific
			case "winspool":	// windows specific
			case "c":		// system provided
			case "objc":            // system provided
			case "objc.a":		// found in swift core libraries
			case "system.b":	// found in swift core libraries
			case "system":		// system provided, libSystem.dylib -> CommonCrypto
			case "x11":		// msvcrt pulled in
			case "winspool.drv":	// msvcrt pulled in
			case "cups":		// msvcrt pulled in
			case "fam.so.0":	// msvcrt pulled in
			case "gamin-1.so.0":	// msvcrt pulled in
			case "asound.so.2":	// msvcrt pulled in
			case "oleaut32": // referenced by System.Runtime.InteropServices.Marshal._[S|G]etErrorInfo
			case "system.native":	// handled by CopyMonoNative()
			case "system.security.cryptography.native.apple": // same
			case "system.net.security.native": // same
				return true;
			}
			// Shutup the warning until we decide on bug: 36478
			if (shortendName.ToLowerInvariant () == "intl" && !native_references.Any (x => x.Contains ("libintl.dylib")) && IsUnifiedFullXamMacFramework)
				return true;
			return false;
		}

		static void ProcessNativeLibrary (HashSet<string> processed, string library, List<MethodDefinition> used_by_methods)
		{
			// We do not bundle system libraries, ever
			if (library.StartsWith ("/usr/lib/", StringComparison.Ordinal) || library.StartsWith ("/System/Library/", StringComparison.Ordinal))
				return;

			// If we've been required to ignore this library, skip it
			if (ignored_assemblies.Contains (library))
				return;

			// If we're passed in a framework, ignore
			if (App.Frameworks.Contains (library))
				return;

			// We need to check both the name and the shortened name, since we might get passed:
			// full path - /foo/bar/libFoo.dylib
			// just name - libFoo
			string name = Path.GetFileName (library);
			string libName = "lib" + GetLibraryShortenedName (name) + ".dylib";

			// There is a list of libraries we always skip, honor that
			if (ShouldSkipNativeLibrary (name))
				return;

			string src = null;
			// If we've been passed in a full path and it is there, let's just go with that
			if (File.Exists (library))
				src = library;

			// Now let's check inside mono/lib
			string monoDirPath = Path.Combine (MonoDirectory, "lib", libName);
			if (src == null && File.Exists (monoDirPath))
				src = monoDirPath;

			// Now let's check in path with our libName
			string path = Path.GetDirectoryName (library);
			if (src == null && !String.IsNullOrEmpty (path)) {
				string pathWithLibName = Path.Combine (path, name);
				if (File.Exists (pathWithLibName))
					src = pathWithLibName;
			}

			// If we can't find it at this point, scream
			if (src == null) {
				ErrorHelper.Show (new MonoMacException (2006, false, Errors.MM2006, name));
				if (used_by_methods != null && used_by_methods.Count > 0) {
					const int referencedByLimit = 25;
					bool limitReferencedByWarnings = used_by_methods.Count > referencedByLimit && Verbosity < 4;
					foreach (var m in limitReferencedByWarnings ? used_by_methods.Take (referencedByLimit) : used_by_methods) {
						ErrorHelper.Warning (2009, Errors.MM2009, m.DeclaringType.FullName, m.Name);
					}
					if (limitReferencedByWarnings)
						ErrorHelper.Warning (2012, Errors.MM2012, referencedByLimit, used_by_methods.Count);
				}
				return;
			}
			string real_src = GetRealPath (src);

			string dest = Path.Combine (mmp_dir, Path.GetFileName (real_src));
			Log (2, "Native library '{0}' copied to application bundle.", Path.GetFileName (real_src));

			if (GetRealPath (dest) == real_src) {
				Console.WriteLine ("Dependency {0} was already at destination, skipping.", Path.GetFileName (real_src));
			}
			else {
				// install_name_tool gets angry if you copy in a read only native library
				CopyFileAndRemoveReadOnly (real_src, dest);
			}

			bool isStaticLib = real_src.EndsWith (".a", StringComparison.Ordinal);
			bool isDynamicLib = real_src.EndsWith (".dylib", StringComparison.Ordinal);

			if (isDynamicLib && App.Optimizations.TrimArchitectures == true)
				LipoLibrary (name, dest);

			if (native_references.Contains (real_src)) {
				if (!isStaticLib)
					RunInstallNameTool (new [] { "-id", "@executable_path/../" + BundleName + "/" + name, dest });
				native_libraries_copied_in.Add (name);
			}

			// if a symlink was used then it might still be needed at runtime
			if (src != real_src)
				CreateSymLink (mmp_dir, real_src, src);

			// We add both the resolved location and the initial request.
			// @executable_path subtitution and other resolving can have these be different
			// and we'll loop forever otherwise
			processed.Add (real_src);
			processed.Add (library);

			// process native dependencies
			if (!isStaticLib) {
				foreach (string dependency in Xamarin.MachO.GetNativeDependencies (real_src)) {
					string lib = GetRealPath (dependency);
					if (!processed.Contains (lib))
						ProcessNativeLibrary (processed, lib, null);
				}
			}
		}

		static void LipoLibrary (string name, string dest)
		{
			var existingArchs = Xamarin.MachO.GetArchitectures (dest);
			// If we have less than two, it will either match by default or 
			// we'll fail a link/launch time with a better error so bail
			if (existingArchs.Count () < 2)
				return;

			RunLipo (new [] { dest, "-thin", arch, "-output", dest });
			if (name != "MonoPosixHelper" && name != "libmono-native-unified" && name != "libmono-native-compat")
				ErrorHelper.Warning (2108, Errors.MM2108, name, arch);
		}

		static void CreateSymLink (string directory, string real, string link)
		{
			string cd = Environment.CurrentDirectory;
			Environment.CurrentDirectory = directory;
			symlink (Path.GetFileName (real), "./" + Path.GetFileName (link));
			Environment.CurrentDirectory = cd;
		}

		/* Currently we clobber any existing files, perhaps we should error and have a -force flag */
		static void CreateDirectoriesIfNeeded () {
			if (App.Embeddinator) {
				App.AppDirectory = Path.Combine (output_dir, app_name + ".framework");
				contents_dir = Path.Combine (App.AppDirectory, "Versions", "A");
				macos_dir = contents_dir;
			} else {
				string bundle_ext;
				if (is_extension && is_xpc_service)
					bundle_ext = "xpc";
				else if (is_extension)
					bundle_ext = "appex";
				else
					bundle_ext = "app";

				App.AppDirectory = Path.Combine (output_dir, string.Format ("{0}.{1}", app_name, bundle_ext));
				contents_dir = Path.Combine (App.AppDirectory, "Contents");
				macos_dir = Path.Combine (contents_dir, "MacOS");
			}

			frameworks_dir = Path.Combine (contents_dir, "Frameworks");
			resources_dir = Path.Combine (contents_dir, "Resources");
			mmp_dir = Path.Combine (contents_dir, BundleName);

			CreateDirectoryIfNeeded (App.AppDirectory);
			CreateDirectoryIfNeeded (contents_dir);
			CreateDirectoryIfNeeded (macos_dir);
			CreateDirectoryIfNeeded (resources_dir);
			CreateDirectoryIfNeeded (mmp_dir);

			if (App.Embeddinator) {
				CreateSymlink (Path.Combine (App.AppDirectory, "Versions", "Current"), "A");
				CreateSymlink (Path.Combine (App.AppDirectory, app_name), $"Versions/Current/{app_name}");
				CreateSymlink (Path.Combine (App.AppDirectory, "Resources"), "Versions/Current/Resources");
			}
		}

		// Mono.Unix can't create symlinks to relative paths, it insists on the target to a full path before creating the symlink.
		static void CreateSymlink (string file, string target)
		{
			unlink (file); // Delete any existing symlinks.
			var rv = symlink (target, file);
			if (rv != 0)
				throw ErrorHelper.CreateError (1034, Errors.MM1034, file, target, Marshal.GetLastWin32Error());
		}

		static void CreateDirectoryIfNeeded (string dir) {
			if (!Directory.Exists (dir))
				Directory.CreateDirectory (dir);
		}

		static void CopyConfiguration () {
			if (IsUnifiedMobile) {
				CopyResourceFile ("config_mobile", "config");
			}
			else {
				if (IsUnifiedFullXamMacFramework) {
					CopyResourceFile ("machine.4_5.config", "machine.config");
				}
				else {
					string machine_config = Path.Combine (MonoDirectory, "etc", "mono", "4.5", "machine.config");

					if (!File.Exists (machine_config))
						throw new MonoMacException (1403, true, Errors.MM1403, "File", machine_config, TargetFramework);

					File.Copy (machine_config, Path.Combine (mmp_dir, "machine.config"), true);
				}

				CopyResourceFile ("config", "config");
			}

			if (machine_config_path != null) {
				string machineConfigDestDir = Path.Combine (mmp_dir, "mono/4.5/");
				string machineConfigDestFile = Path.Combine (machineConfigDestDir, "machine.config");

				CreateDirectoryIfNeeded (machineConfigDestDir);
				if (machine_config_path == String.Empty) {
					File.WriteAllLines (machineConfigDestFile, new string [] { "<?xml version=\"1.0\" encoding=\"utf-8\"?>", "<configuration>", "</configuration>" });
				}
				else {
					if (!File.Exists (machine_config_path))
						throw new MonoMacException (97, true, Errors.MM0097, machine_config_path);
					File.Copy (machine_config_path, machineConfigDestFile);
				}
			}
		}

		static void CopyResourceFile (string streamName, string outputFileName) {
			var sr = new StreamReader (typeof (Driver).Assembly.GetManifestResourceStream (streamName));
			var all = sr.ReadToEnd ();
			var config = Path.Combine (mmp_dir, outputFileName);
			using (var sw = new StreamWriter (config)) {
				sw.WriteLine (all);
			}
		}

		static void CopyI18nAssemblies (I18nAssemblies i18n)
		{
			if (i18n == I18nAssemblies.None)
				return;

			string fx_dir = BuildTarget.Resolver.FrameworkDirectory;
			// always needed (if any is specified)
			resolved_assemblies.Add (Path.Combine (fx_dir, "I18N.dll"));
			// optionally needed
			if ((i18n & I18nAssemblies.CJK) != 0)
				resolved_assemblies.Add (Path.Combine (fx_dir, "I18N.CJK.dll"));
			if ((i18n & I18nAssemblies.MidEast) != 0)
				resolved_assemblies.Add (Path.Combine (fx_dir, "I18N.MidEast.dll"));
			if ((i18n & I18nAssemblies.Other) != 0)
				resolved_assemblies.Add (Path.Combine (fx_dir, "I18N.Other.dll"));
			if ((i18n & I18nAssemblies.Rare) != 0)
				resolved_assemblies.Add (Path.Combine (fx_dir, "I18N.Rare.dll"));
			if ((i18n & I18nAssemblies.West) != 0)
				resolved_assemblies.Add (Path.Combine (fx_dir, "I18N.West.dll"));
		}

		static void CopyFileAndRemoveReadOnly (string src, string dest) {
			File.Copy (src, dest, true);

			FileAttributes attrs = File.GetAttributes (dest);
			if ((attrs & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
				File.SetAttributes (dest, attrs & ~FileAttributes.ReadOnly);
		}

		static void CopyAssemblies () {
			foreach (string asm in resolved_assemblies) {
				var configfile = string.Format ("{0}.config", asm);
				string filename = Path.GetFileName (asm);

				// The linker later gets angry if you copy in a read only assembly
				CopyFileAndRemoveReadOnly (asm, Path.Combine (mmp_dir, filename));

				Log (1, "Added assembly {0}", asm);

				if (App.EnableDebug) {
					var mdbfile = asm + ".mdb";
					if (File.Exists (mdbfile))
						CopyFileAndRemoveReadOnly (mdbfile, Path.Combine (mmp_dir, Path.GetFileName (mdbfile)));
					var pdbfile = Path.ChangeExtension (asm, ".pdb");
					if (File.Exists (pdbfile))
						CopyFileAndRemoveReadOnly (pdbfile, Path.Combine (mmp_dir, Path.GetFileName (pdbfile)));
				}
				if (File.Exists (configfile))
					File.Copy (configfile, Path.Combine (mmp_dir, Path.GetFileName (configfile)), true);
			}

			foreach (var assembly in BuildTarget.Assemblies)
				assembly.CopySatellitesToDirectory (mmp_dir);
		}

		static void CopyResources () {
			foreach (string res in resources) {
				File.Copy (res, Path.Combine (resources_dir, Path.GetFileName (res)), true);
			}
		}

		static void GatherAssemblies () {
			foreach (string asm in references) {
				AssemblyDefinition assembly = AddAssemblyPathToResolver (asm);
				ProcessAssemblyReferences (assembly);
			}
			if (BuildTarget.Resolver.Exceptions.Count > 0)
				throw new AggregateException (BuildTarget.Resolver.Exceptions);
		}

		static void ProcessAssemblyReferences (AssemblyDefinition assembly) {
			if (assembly == null)
				return;

			var fqname = GetRealPath (assembly.MainModule.FileName);

			if (resolved_assemblies.Contains (fqname))
				return;
			
			Target.PrintAssemblyReferences (assembly);

			var asm = new Assembly (BuildTarget, assembly);
			asm.ComputeSatellites ();
			BuildTarget.Assemblies.Add (asm);

			resolved_assemblies.Add (fqname);

			foreach (AssemblyNameReference reference in assembly.MainModule.AssemblyReferences) {
				AssemblyDefinition reference_assembly = AddAssemblyReferenceToResolver (reference.Name);
				ProcessAssemblyReferences (reference_assembly);
			}
		}

		static AssemblyDefinition AddAssemblyPathToResolver (string path)
		{
			if (AssemblySwapInfo.AssemblyNeedsSwappedOut (path))
				path = AssemblySwapInfo.GetSwappedAssemblyPath (path);

			var assembly = BuildTarget.Resolver.Load (path);
			if (assembly == null)
				ErrorHelper.Warning (1501, Errors.MM1501, path);
			return assembly;
		}

		static AssemblyDefinition AddAssemblyReferenceToResolver (string reference)
		{
			if (AssemblySwapInfo.ReferencedNeedsSwappedOut (reference))
				return BuildTarget.Resolver.Load (AssemblySwapInfo.GetSwappedReference (reference));

			return BuildTarget.Resolver.Resolve (reference);
		}
	}

	public static class AssemblySwapInfo {
		static HashSet<string> xammac_reference_assemblies_names = new HashSet<string> {
			"Xamarin.Mac",
			"Xamarin.Mac.CFNetwork",
			"OpenTK"
		};

		public static bool AssemblyNeedsSwappedOut (string path) => NeedsSwappedCore (Path.GetFileNameWithoutExtension (path));
		public static bool ReferencedNeedsSwappedOut (string reference) => NeedsSwappedCore (reference);

		static bool NeedsSwappedCore (string name)
		{
			if (name.Contains ("OpenTK") && Driver.IsUnifiedFullXamMacFramework)
				return false;

			return xammac_reference_assemblies_names.Contains (name);
		}

		public static string GetSwappedAssemblyPath (string path) => GetSwappedPathCore (Path.GetFileNameWithoutExtension (path));
		public static string GetSwappedReference (string reference) => GetSwappedPathCore (reference);

		static string GetSwappedPathCore (string name)
		{
			string flavor = (Driver.IsUnifiedFullSystemFramework || Driver.IsUnifiedFullXamMacFramework) ? "full" : "mobile";
			switch (Driver.Arch) {
				case "x86_64":
					return Path.Combine (Driver.FrameworkLibDirectory, Driver.Arch, flavor, name + ".dll");
				default:
					throw new MonoMacException (5205, true, Errors.MM5205, Driver.Arch);
			}
		}
	}
}
