//
// Authors:
//   Miguel de Icaza
//
// Copyright 2011-2014 Xamarin Inc.
// Copyright 2009-2010 Novell, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using System.IO;
using System.Linq;
using IKVM.Reflection;
using Type = IKVM.Reflection.Type;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Mono.Options;

using ObjCRuntime;
using Foundation;
using Xamarin.Utils;

class BindingTouch {
	static TargetFramework? target_framework;
	public static PlatformName CurrentPlatform;
	public static bool Unified;
	public static bool skipSystemDrawing;
	public static string outfile;

	static string baselibdll;
	static string attributedll;
	static string compiler = "/Library/Frameworks/Mono.framework/Versions/Current/bin/csc";

	static List<string> libs = new List<string> ();

	public static Universe universe;

	public static TargetFramework TargetFramework {
		get { return target_framework.Value; }
	}

	public static string ToolName {
		get { return Path.GetFileNameWithoutExtension (System.Reflection.Assembly.GetEntryAssembly ().Location); }
	}

	static void ShowHelp (OptionSet os)
	{
		Console.WriteLine ("{0} - Mono Objective-C API binder", ToolName);
		Console.WriteLine ("Usage is:\n {0} [options] apifile1.cs [--api=apifile2.cs [--api=apifile3.cs]] [-s=core1.cs [-s=core2.cs]] [core1.cs [core2.cs]] [-x=extra1.cs [-x=extra2.cs]]", ToolName);
		
		os.WriteOptionDescriptions (Console.Out);
	}
	
	static int Main (string [] args)
	{
		try {
			return Main2 (args);
		} catch (Exception ex) {
			ErrorHelper.Show (ex);
			return 1;
		}
	}

	static string GetAttributeLibraryPath ()
	{
		if (!string.IsNullOrEmpty (attributedll))
			return attributedll;

		switch (CurrentPlatform) {
		case PlatformName.iOS:
			if (Unified) {
				return Path.Combine (GetSDKRoot (), "lib", "bgen", "Xamarin.iOS.BindingAttributes.dll");
			} else {
				return Path.Combine (GetSDKRoot (), "lib", "bgen", "monotouch.BindingAttributes.dll");
			}
		case PlatformName.WatchOS:
			return Path.Combine (GetSDKRoot (), "lib", "bgen", "Xamarin.WatchOS.BindingAttributes.dll");
		case PlatformName.TvOS:
			return Path.Combine (GetSDKRoot (), "lib", "bgen", "Xamarin.TVOS.BindingAttributes.dll");
		case PlatformName.MacOSX:
			if (target_framework == TargetFramework.Xamarin_Mac_4_5_Full) {
				return Path.Combine (GetSDKRoot (), "lib", "bgen", "Xamarin.Mac-full.BindingAttributes.dll");
			} else if (target_framework == TargetFramework.Xamarin_Mac_4_5_System) {
				return Path.Combine (GetSDKRoot (), "lib", "bgen", "Xamarin.Mac-full.BindingAttributes.dll");
			} else if (target_framework == TargetFramework.Xamarin_Mac_2_0_Mobile) {
				return Path.Combine (GetSDKRoot (), "lib", "bgen", "Xamarin.Mac-mobile.BindingAttributes.dll");
			} else if (target_framework == TargetFramework.XamMac_1_0) {
				return Path.Combine (GetSDKRoot (), "lib", "bgen", "XamMac.BindingAttributes.dll");
			} else {
				throw ErrorHelper.CreateError (1043, "Internal error: unknown target framework '{0}'.", target_framework);
			}
		default:
			throw new BindingException (1047, "Unsupported platform: {0}. Please file a bug report (http://bugzilla.xamarin.com) with a test case.", CurrentPlatform);
		}
	}

	static IEnumerable<string> GetLibraryDirectories ()
	{
		switch (CurrentPlatform) {
		case PlatformName.iOS:
			yield return Path.Combine (GetSDKRoot (), "lib", "mono", Unified ? "Xamarin.iOS" : "2.1");
			break;
		case PlatformName.WatchOS:
			yield return Path.Combine (GetSDKRoot (), "lib", "mono", "Xamarin.WatchOS");
			break;
		case PlatformName.TvOS:
			yield return Path.Combine (GetSDKRoot (), "lib", "mono", "Xamarin.TVOS");
			break;
		case PlatformName.MacOSX:
			if (target_framework == TargetFramework.Xamarin_Mac_4_5_Full) {
				yield return Path.Combine (GetSDKRoot (), "lib", "reference", "full");
				yield return Path.Combine (GetSDKRoot (), "lib", "mono", "4.5");
			} else if (target_framework == TargetFramework.Xamarin_Mac_4_5_System) {
				yield return "/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.5";
				yield return Path.Combine (GetSDKRoot (), "lib", "mono", "4.5");
			} else if (target_framework == TargetFramework.Xamarin_Mac_2_0_Mobile) {
				yield return Path.Combine (GetSDKRoot (), "lib", "mono", "Xamarin.Mac");
			} else if (target_framework == TargetFramework.XamMac_1_0) {
				yield return Path.Combine (GetSDKRoot (), "lib", "mono");
				yield return "/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.5";
			} else {
				throw ErrorHelper.CreateError (1043, "Internal error: unknown target framework '{0}'.", target_framework);
			}
			break;
		default:
			throw new BindingException (1047, "Unsupported platform: {0}. Please file a bug report (http://bugzilla.xamarin.com) with a test case.", CurrentPlatform);
		}
		foreach (var lib in libs)
			yield return lib;
	}

	static string LocateAssembly (string name)
	{
		foreach (var asm in universe.GetAssemblies ()) {
			if (asm.GetName ().Name == name)
				return asm.Location;
		}

		foreach (var lib in GetLibraryDirectories ()) {
			var path = Path.Combine (lib, name);
			if (File.Exists (path))
				return path;
			path += ".dll";
			if (File.Exists (path))
				return path;
		}

		throw new FileNotFoundException ($"Could not find the assembly '{name}' in any of the directories: {string.Join (", ", GetLibraryDirectories ())}");
	}

	static string GetSDKRoot ()
	{
		switch (CurrentPlatform) {
		case PlatformName.iOS:
		case PlatformName.WatchOS:
		case PlatformName.TvOS:
			var sdkRoot = Environment.GetEnvironmentVariable ("MD_MTOUCH_SDK_ROOT");
			if (string.IsNullOrEmpty (sdkRoot))
				sdkRoot = "/Library/Frameworks/Xamarin.iOS.framework/Versions/Current";
			return sdkRoot;
		case PlatformName.MacOSX:
			var macSdkRoot = Environment.GetEnvironmentVariable ("XamarinMacFrameworkRoot");
			if (string.IsNullOrEmpty (macSdkRoot))
				macSdkRoot = "/Library/Frameworks/Xamarin.Mac.framework/Versions/Current";
			return macSdkRoot;
		default:
			throw new BindingException (1047, "Unsupported platform: {0}. Please file a bug report (http://bugzilla.xamarin.com) with a test case.", CurrentPlatform);
		}
	}

	static void SetTargetFramework (string fx)
	{
		TargetFramework tf;
		if (!TargetFramework.TryParse (fx, out tf))
			throw ErrorHelper.CreateError (68, "Invalid value for target framework: {0}.", fx);
		target_framework = tf;

		if (Array.IndexOf (TargetFramework.ValidFrameworks, target_framework.Value) == -1)
			throw ErrorHelper.CreateError (70, "Invalid target framework: {0}. Valid target frameworks are: {1}.", target_framework.Value, string.Join (" ", TargetFramework.ValidFrameworks.Select ((v) => v.ToString ()).ToArray ()));
	}

	public static string NamespacePlatformPrefix {
		get {
			switch (CurrentPlatform) {
			case PlatformName.MacOSX:
				return Unified ? null : "MonoMac";
			case PlatformName.iOS:
				return Unified ? null : "MonoTouch";
			default:
				return null;
			}

		}
	}

	static int Main2 (string [] args)
	{
		bool show_help = false;
		bool zero_copy = false;
		string basedir = null;
		string tmpdir = null;
		string ns = null;
		bool delete_temp = true, debug = false;
		bool verbose = false;
		bool unsafef = true;
		bool external = false;
		bool public_mode = true;
		bool nostdlib = false;
		bool? inline_selectors = null;
		List<string> sources;
		var resources = new List<string> ();
		var linkwith = new List<string> ();
		var references = new List<string> ();
		var api_sources = new List<string> ();
		var core_sources = new List<string> ();
		var extra_sources = new List<string> ();
		var defines = new List<string> ();
		string generate_file_list = null;
		bool process_enums = false;

		var os = new OptionSet () {
			{ "h|?|help", "Displays the help", v => show_help = true },
			{ "a", "Include alpha bindings (Obsolete).", v => {}, true },
			{ "outdir=", "Sets the output directory for the temporary binding files", v => { basedir = v; }},
			{ "o|out=", "Sets the name of the output library", v => outfile = v },
			{ "tmpdir=", "Sets the working directory for temp files", v => { tmpdir = v; delete_temp = false; }},
			{ "debug", "Generates a debugging build of the binding", v => debug = true },
			{ "sourceonly=", "Only generates the source", v => generate_file_list = v },
			{ "ns=", "Sets the namespace for storing helper classes", v => ns = v },
			{ "unsafe", "Sets the unsafe flag for the build", v=> unsafef = true },
			{ "core", "Use this to build product assemblies", v => Generator.BindThirdPartyLibrary = false },
			{ "r=", "Adds a reference", v => references.Add (v) },
			{ "lib=", "Adds the directory to the search path for the compiler", v => libs.Add (StringUtils.Quote (v)) },
			{ "compiler=", "Sets the compiler to use (Obsolete) ", v => compiler = v, true },
			{ "sdk=", "Sets the .NET SDK to use (Obsolete)", v => {}, true },
			{ "new-style", "Build for Unified (Obsolete).", v => { Console.WriteLine ("The --new-style option is obsolete and ignored."); }, true},
			{ "d=", "Defines a symbol", v => defines.Add (v) },
			{ "api=", "Adds a API definition source file", v => api_sources.Add (StringUtils.Quote (v)) },
			{ "s=", "Adds a source file required to build the API", v => core_sources.Add (StringUtils.Quote (v)) },
			{ "v", "Sets verbose mode", v => verbose = true },
			{ "x=", "Adds the specified file to the build, used after the core files are compiled", v => extra_sources.Add (StringUtils.Quote (v)) },
			{ "e", "Generates smaller classes that can not be subclassed (previously called 'external mode')", v => external = true },
			{ "p", "Sets private mode", v => public_mode = false },
			{ "baselib=", "Sets the base library", v => baselibdll = v },
			{ "attributelib=", "Sets the attribute library", v => attributedll = v },
			{ "use-zero-copy", v=> zero_copy = true },
			{ "nostdlib", "Does not reference mscorlib.dll library", l => nostdlib = true },
			{ "no-mono-path", "Launches compiler with empty MONO_PATH", l => { } },
			{ "native-exception-marshalling", "Enable the marshalling support for Objective-C exceptions", (v) => { /* no-op */} },
			{ "inline-selectors:", "If Selector.GetHandle is inlined and does not need to be cached (enabled by default in Xamarin.iOS, disabled in Xamarin.Mac)",
				v => inline_selectors = string.Equals ("true", v, StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty (v)
			},
			{ "process-enums", "Process enums as bindings, not external, types.", v => process_enums = true },
			{ "link-with=,", "Link with a native library {0:FILE} to the binding, embedded as a resource named {1:ID}",
				(path, id) => {
					if (path == null || path.Length == 0)
						throw new Exception ("-link-with=FILE,ID requires a filename.");
					
					if (id == null || id.Length == 0)
						id = Path.GetFileName (path);
					
					if (linkwith.Contains (id))
						throw new Exception ("-link-with=FILE,ID cannot assign the same resource id to multiple libraries.");
					
					resources.Add (string.Format ("-res:{0},{1}", path, id));
					linkwith.Add (id);
				}
			},
			{ "unified-full-profile", "Launches compiler pointing to XM Full Profile", l => { /* no-op*/ }, true },
			{ "unified-mobile-profile", "Launches compiler pointing to XM Mobile Profile", l => { /* no-op*/ }, true },
			{ "target-framework=", "Specify target framework to use. Always required, and the currently supported values are: 'MonoTouch,v1.0', 'Xamarin.iOS,v1.0', 'Xamarin.TVOS,v1.0', 'Xamarin.WatchOS,v1.0', 'XamMac,v1.0', 'Xamarin.Mac,Version=v2.0,Profile=Mobile', 'Xamarin.Mac,Version=v4.5,Profile=Full' and 'Xamarin.Mac,Version=v4.5,Profile=System')", v => SetTargetFramework (v) },
			{ "warnaserror:", "An optional comma-separated list of warning codes that should be reported as errors (if no warnings are specified all warnings are reported as errors).", v => {
					try {
						if (!string.IsNullOrEmpty (v)) {
							foreach (var code in v.Split (new char [] { ',' }, StringSplitOptions.RemoveEmptyEntries))
								ErrorHelper.SetWarningLevel (ErrorHelper.WarningLevel.Error, int.Parse (code));
						} else {
							ErrorHelper.SetWarningLevel (ErrorHelper.WarningLevel.Error);
						}
					} catch (Exception ex) {
						throw ErrorHelper.CreateError (26, $"Could not parse the command line argument '--warnaserror': {ex.Message}");
					}
				}
			},
			{ "nowarn:", "An optional comma-separated list of warning codes to ignore (if no warnings are specified all warnings are ignored).", v => {
					try {
						if (!string.IsNullOrEmpty (v)) {
							foreach (var code in v.Split (new char [] { ',' }, StringSplitOptions.RemoveEmptyEntries))
								ErrorHelper.SetWarningLevel (ErrorHelper.WarningLevel.Disable, int.Parse (code));
						} else {
							ErrorHelper.SetWarningLevel (ErrorHelper.WarningLevel.Disable);
						}
					} catch (Exception ex) {
						throw ErrorHelper.CreateError (26, $"Could not parse the command line argument '--nowarn': {ex.Message}");
					}
				}
			},
			new Mono.Options.ResponseFileSource (),
		};

		try {
			sources = os.Parse (args);
		} catch (Exception e){
			Console.Error.WriteLine ("{0}: {1}", ToolName, e.Message);
			Console.Error.WriteLine ("see {0} --help for more information", ToolName);
			return 1;
		}

		if (show_help) {
			ShowHelp (os);
			return 0;
		}

		if (!target_framework.HasValue)
			throw ErrorHelper.CreateError (86, "A target framework (--target-framework) must be specified.");

		switch (target_framework.Value.Identifier.ToLowerInvariant ()) {
		case "monotouch":
			CurrentPlatform = PlatformName.iOS;
			Unified = false;
			if (string.IsNullOrEmpty (baselibdll))
				baselibdll = Path.Combine (GetSDKRoot (), "lib/mono/2.1/monotouch.dll");
			Path.Combine (GetSDKRoot (), "bin/smcs");
			AddAnyMissingSystemReferencesFromSDK ("lib/mono/2.1/", references);
			break;
		case "xamarin.ios":
			CurrentPlatform = PlatformName.iOS;
			Unified = true;
			nostdlib = true;
			if (string.IsNullOrEmpty (baselibdll))
				baselibdll = Path.Combine (GetSDKRoot (), "lib/mono/Xamarin.iOS/Xamarin.iOS.dll");
			AddAnyMissingSystemReferencesFromSDK ("lib/mono/Xamarin.iOS", references);
			break;
		case "xamarin.tvos":
			CurrentPlatform = PlatformName.TvOS;
			Unified = true;
			nostdlib = true;
			if (string.IsNullOrEmpty (baselibdll))
				baselibdll = Path.Combine (GetSDKRoot (), "lib/mono/Xamarin.TVOS/Xamarin.TVOS.dll");
			AddAnyMissingSystemReferencesFromSDK ("lib/mono/Xamarin.TVOS", references);
			break;
		case "xamarin.watchos":
			CurrentPlatform = PlatformName.WatchOS;
			Unified = true;
			nostdlib = true;
			if (string.IsNullOrEmpty (baselibdll))
				baselibdll = Path.Combine (GetSDKRoot (), "lib/mono/Xamarin.WatchOS/Xamarin.WatchOS.dll");
			AddAnyMissingSystemReferencesFromSDK ("lib/mono/Xamarin.WatchOS", references);
			break;
		case "xammac":
			CurrentPlatform = PlatformName.MacOSX;
			Unified = false;
			if (string.IsNullOrEmpty (baselibdll))
				baselibdll = Path.Combine (GetSDKRoot (), "lib", "mono", "XamMac.dll");
			AddAnyMissingSystemReferences ("/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.5", references);
			break;
		case "xamarin.mac":
			CurrentPlatform = PlatformName.MacOSX;
			Unified = true;
			nostdlib = true;
			if (string.IsNullOrEmpty (baselibdll)) {
				if (target_framework == TargetFramework.Xamarin_Mac_2_0_Mobile)
					baselibdll = Path.Combine (GetSDKRoot (), "lib", "reference", "mobile", "Xamarin.Mac.dll");
				else if (target_framework == TargetFramework.Xamarin_Mac_4_5_Full || target_framework == TargetFramework.Xamarin_Mac_4_5_System)
					baselibdll = Path.Combine (GetSDKRoot (), "lib", "reference", "full", "Xamarin.Mac.dll");
				else
					throw ErrorHelper.CreateError (1043, "Internal error: unknown target framework '{0}'.", target_framework); 
			}
			if (target_framework == TargetFramework.Xamarin_Mac_2_0_Mobile) {
				skipSystemDrawing = true;
				AddAnyMissingSystemReferencesFromSDK ("lib/mono/Xamarin.Mac", references);
			} else if (target_framework == TargetFramework.Xamarin_Mac_4_5_Full) {
				skipSystemDrawing = true;
				AddAnyMissingSystemReferencesFromSDK ("lib/mono/4.5", references);
			} else if (target_framework == TargetFramework.Xamarin_Mac_4_5_System) {
				skipSystemDrawing = false;
				AddAnyMissingSystemReferences ("/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.5", references);
			} else {
				throw ErrorHelper.CreateError (1043, "Internal error: unknown target framework '{0}'.", target_framework); 
			}

			break;
		default:
			throw ErrorHelper.CreateError (1043, "Internal error: unknown target framework '{0}'.", target_framework);
		}

		if (target_framework == TargetFramework.XamMac_1_0 && !references.Any ((v) => Path.GetFileNameWithoutExtension (v) == "System.Drawing")) {
			// If we're targeting XM/Classic ensure we have a reference to System.Drawing.dll.
			references.Add ("/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.5/System.Drawing.dll");
		}

		if (sources.Count > 0) {
			api_sources.Insert (0, StringUtils.Quote (sources [0]));
			for (int i = 1; i < sources.Count; i++)
				core_sources.Insert (i - 1, StringUtils.Quote (sources [i]));
		}

		if (api_sources.Count == 0) {
			Console.WriteLine ("Error: no api file provided");
			ShowHelp (os);
			return 1;
		}

		if (tmpdir == null)
			tmpdir = GetWorkDir ();

		string firstApiDefinitionName = Path.GetFileNameWithoutExtension (StringUtils.Unquote (api_sources [0]));
		firstApiDefinitionName = firstApiDefinitionName.Replace ('-', '_'); // This is not exhaustive, but common.
		if (outfile == null)
			outfile = firstApiDefinitionName + ".dll";

		string refs = string.Empty;
		foreach (var r in references) {
			if (refs != string.Empty)
				refs += " ";
			refs += "-r:" + StringUtils.Quote (r);
		}
		string paths = (libs.Count > 0 ? "-lib:" + String.Join (" -lib:", libs.ToArray ()) : "");

		try {
			var tmpass = Path.Combine (tmpdir, "temp.dll");

			// -nowarn:436 is to avoid conflicts in definitions between core.dll and the sources
			// Keep source files at the end of the command line - csc will create TWO assemblies if any sources preceed the -out parameter
			var cargs = new StringBuilder ();

			cargs.Append ("-debug -unsafe -target:library -nowarn:436").Append (' ');
			cargs.Append ("-out:").Append (StringUtils.Quote (tmpass)).Append (' ');
			cargs.Append ("-r:").Append (StringUtils.Quote (GetAttributeLibraryPath ())).Append (' ');
			cargs.Append (refs).Append (' ');
			if (unsafef)
				cargs.Append ("-unsafe ");
			cargs.Append ("-r:").Append (StringUtils.Quote (baselibdll)).Append (' ');
			foreach (var def in defines)
				cargs.Append ("-define:").Append (def).Append (' ');
			cargs.Append (paths).Append (' ');
			if (nostdlib) {
				cargs.Append ("-nostdlib ");
				cargs.Append ("-noconfig ");
			}
			foreach (var qs in api_sources)
				cargs.Append (qs).Append (' ');
			foreach (var cs in core_sources)
				cargs.Append (cs).Append (' ');
			if (!string.IsNullOrEmpty (Path.GetDirectoryName (baselibdll)))
				cargs.Append ("-lib:").Append (Path.GetDirectoryName (baselibdll)).Append (' ');
			

			var si = new ProcessStartInfo (compiler, cargs.ToString ()) {
				UseShellExecute = false,
			};
				
			// HACK: We are calling btouch with forced 2.1 path but we need working mono for compiler
			si.EnvironmentVariables.Remove ("MONO_PATH");

			if (verbose)
				Console.WriteLine ("{0} {1}", si.FileName, si.Arguments);
			
			var p = Process.Start (si);
			p.WaitForExit ();
			if (p.ExitCode != 0){
				Console.WriteLine ("{0}: API binding contains errors.", ToolName);
				return 1;
			}

			universe = new Universe (UniverseOptions.EnableFunctionPointers | UniverseOptions.ResolveMissingMembers | UniverseOptions.MetadataOnly);

			Assembly api;
			try {
				api = universe.LoadFile (tmpass);
			} catch (Exception e) {
				if (verbose)
					Console.WriteLine (e);
				
				Console.Error.WriteLine ("Error loading API definition from {0}", tmpass);
				return 1;
			}

			Assembly baselib;
			try {
				baselib = universe.LoadFile (baselibdll);
			} catch (Exception e){
				if (verbose)
					Console.WriteLine (e);

				Console.Error.WriteLine ("Error loading base library {0}", baselibdll);
				return 1;
			}

			Assembly corlib_assembly = universe.LoadFile (LocateAssembly ("mscorlib"));
			Assembly platform_assembly = baselib;
			Assembly system_assembly = universe.LoadFile (LocateAssembly ("System"));
			Assembly binding_assembly = universe.LoadFile (GetAttributeLibraryPath ());
			TypeManager.Initialize (api, corlib_assembly, platform_assembly, system_assembly, binding_assembly);

			foreach (var linkWith in AttributeManager.GetCustomAttributes<LinkWithAttribute> (api)) {
				if (!linkwith.Contains (linkWith.LibraryName)) {
					Console.Error.WriteLine ("Missing native library {0}, please use `--link-with' to specify the path to this library.", linkWith.LibraryName);
					return 1;
				}
			}

			foreach (var r in references) {
				if (File.Exists (r)) {
					try {
						universe.LoadFile (r);
					} catch (Exception ex) {
						ErrorHelper.Show (new BindingException (1104, false, "Could not load the referenced library '{0}': {1}.", r, ex.Message));
					}
				}
			}

			var types = new List<Type> ();
			var  strong_dictionaries = new List<Type> ();
			foreach (var t in api.GetTypes ()){
				if ((process_enums && t.IsEnum) ||
				    AttributeManager.HasAttribute<BaseTypeAttribute> (t) ||
				    AttributeManager.HasAttribute<ProtocolAttribute> (t) ||
				    AttributeManager.HasAttribute<StaticAttribute> (t) ||
				    AttributeManager.HasAttribute<PartialAttribute> (t))
					types.Add (t);
				if (AttributeManager.HasAttribute<StrongDictionaryAttribute> (t))
					strong_dictionaries.Add (t);
			}

			var nsManager = new NamespaceManager (
				NamespacePlatformPrefix,
				ns == null ? firstApiDefinitionName : ns,
				skipSystemDrawing
			);

			var g = new Generator (nsManager, public_mode, external, debug, types.ToArray (), strong_dictionaries.ToArray ()){
				BaseDir = basedir != null ? basedir : tmpdir,
				ZeroCopyStrings = zero_copy,
				InlineSelectors = inline_selectors ?? (Unified && CurrentPlatform != PlatformName.MacOSX),
			};

			if (!Unified && !Generator.BindThirdPartyLibrary) {
				foreach (var mi in baselib.GetType (nsManager.CoreObjCRuntime + ".Messaging").GetMethods ()){
					if (mi.Name.IndexOf ("_objc_msgSend", StringComparison.Ordinal) != -1)
						g.RegisterMethodName (mi.Name);
				}
			}

			g.Go ();

			if (generate_file_list != null){
				using (var f = File.CreateText (generate_file_list)){
					foreach (var x in g.GeneratedFiles.OrderBy ((v) => v))
						f.WriteLine (x);
				}
				return 0;
			}

			cargs.Clear ();
			if (unsafef)
				cargs.Append ("-unsafe ");
			cargs.Append ("-target:library ");
			cargs.Append ("-out:").Append (StringUtils.Quote (outfile)).Append (' ');
			foreach (var def in defines)
				cargs.Append ("-define:").Append (def).Append (' ');
			foreach (var gf in g.GeneratedFiles)
				cargs.Append (gf).Append (' ');
			foreach (var cs in core_sources)
				cargs.Append (cs).Append (' ');
			foreach (var es in extra_sources)
				cargs.Append (es).Append (' ');
			cargs.Append (refs).Append (' ');
			cargs.Append ("-r:").Append (StringUtils.Quote (baselibdll)).Append (' ');
			foreach (var res in resources)
				cargs.Append (res).Append (' ');
			if (nostdlib) {
				cargs.Append ("-nostdlib ");
				cargs.Append ("-noconfig ");
			}
			if (!string.IsNullOrEmpty (Path.GetDirectoryName (baselibdll)))
				cargs.Append ("-lib:").Append (Path.GetDirectoryName (baselibdll)).Append (' ');
				
			si = new ProcessStartInfo (compiler, cargs.ToString ()) {
				UseShellExecute = false,
			};

			// HACK: We are calling btouch with forced 2.1 path but we need working mono for compiler
			si.EnvironmentVariables.Remove ("MONO_PATH");

			if (verbose)
				Console.WriteLine ("{0} {1}", si.FileName, si.Arguments);

			p = Process.Start (si);
			p.WaitForExit ();
			if (p.ExitCode != 0){
				Console.WriteLine ("{0}: API binding contains errors.", ToolName);
				return 1;
			}
		} finally {
			if (delete_temp)
				Directory.Delete (tmpdir, true);
		}
		return 0;
	}
	
	static void AddAnyMissingSystemReferences (string sdk_path, List<string> references)
	{
		if (!references.Any ((v) => Path.GetFileNameWithoutExtension (v) == "System"))
			references.Add (Path.Combine (sdk_path, "System.dll"));
		if (!references.Any ((v) => Path.GetFileNameWithoutExtension (v) == "mscorlib"))
			references.Add (Path.Combine (sdk_path, "mscorlib.dll"));

		// In theory I believe this should be skipSystemDrawing however multiple XI targets do not set it
		// so I do not believe it would be safe. 
		if (target_framework == TargetFramework.Xamarin_Mac_4_5_System &&
		    !references.Any ((v) => Path.GetFileNameWithoutExtension (v) == "System.Drawing"))
			references.Add (Path.Combine (sdk_path, "System.Drawing.dll"));
	}

	static void AddAnyMissingSystemReferencesFromSDK (string sdk_offset, List<string> references)
	{
		AddAnyMissingSystemReferences (Path.Combine (GetSDKRoot (), sdk_offset), references);
	}

	static string GetWorkDir ()
	{
		while (true){
			string p = Path.Combine (Path.GetTempPath(), Path.GetRandomFileName());
			if (Directory.Exists (p))
				continue;
			
			var di = Directory.CreateDirectory (p);
			return di.FullName;
		}
	}
}
