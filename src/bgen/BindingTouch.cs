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

#nullable enable

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Mono.Options;

using ObjCRuntime;
using Foundation;

using Xamarin.Bundler;
using Xamarin.Utils;

public class BindingTouch : IDisposable {
	TargetFramework? target_framework;
#if NET
	public static ApplePlatform [] AllPlatforms = new ApplePlatform [] { ApplePlatform.iOS, ApplePlatform.MacOSX, ApplePlatform.TVOS, ApplePlatform.MacCatalyst };
	public static PlatformName [] AllPlatformNames = new PlatformName [] { PlatformName.iOS, PlatformName.MacOSX, PlatformName.TvOS, PlatformName.MacCatalyst };
#else
	public static ApplePlatform [] AllPlatforms = new ApplePlatform [] { ApplePlatform.iOS, ApplePlatform.MacOSX, ApplePlatform.TVOS, ApplePlatform.WatchOS };
	public static PlatformName [] AllPlatformNames = new PlatformName [] { PlatformName.iOS, PlatformName.MacOSX, PlatformName.TvOS, PlatformName.WatchOS };
#endif
	public PlatformName CurrentPlatform;
	public bool BindThirdPartyLibrary = true;
	public bool skipSystemDrawing;
	public string? outfile;

#if !NET
	const string DefaultCompiler = "/Library/Frameworks/Mono.framework/Versions/Current/bin/csc";
#endif
	string compiler = string.Empty;
	string []? compile_command = null;
	string? baselibdll;
	string? attributedll;
	string compiled_api_definition_assembly = string.Empty;
	bool noNFloatUsing;

	List<string> libs = new List<string> ();
	List<string> references = new List<string> ();

	public MetadataLoadContext? universe;
	public Frameworks? Frameworks;

	AttributeManager? attributeManager;
	public AttributeManager AttributeManager => attributeManager!;

	TypeManager? typeManager;
	public TypeManager TypeManager => typeManager!;

	NamespaceManager? namespaceManager;
	public NamespaceManager NamespaceManager => namespaceManager!;
	bool disposedValue;
	readonly Dictionary<System.Type, Type> ikvm_type_lookup = new Dictionary<System.Type, Type> ();
	internal Dictionary<System.Type, Type> IKVMTypeLookup {
		get { return ikvm_type_lookup; }
	}

	public TargetFramework TargetFramework {
		get { return target_framework!.Value; }
	}

	public static string ToolName {
		get { return "bgen"; }
	}

	internal bool IsDotNet {
		get { return TargetFramework.IsDotNet; }
	}

	static void ShowHelp (OptionSet os)
	{
		Console.WriteLine ("{0} - Mono Objective-C API binder", ToolName);
		Console.WriteLine ("Usage is:\n {0} [options] apifile1.cs [--api=apifile2.cs [--api=apifile3.cs]] [-s=core1.cs [-s=core2.cs]] [core1.cs [core2.cs]] [-x=extra1.cs [-x=extra2.cs]]", ToolName);

		os.WriteOptionDescriptions (Console.Out);
	}

	public static int Main (string [] args)
	{
		try {
			return Main2 (args);
		} catch (Exception ex) {
			ErrorHelper.Show (ex, false);
			return 1;
		}
	}

	string GetAttributeLibraryPath ()
	{
		if (!string.IsNullOrEmpty (attributedll))
			return attributedll!;

		if (IsDotNet)
			return Path.Combine (GetSDKRoot (), "lib", "Xamarin.Apple.BindingAttributes.dll");

		switch (CurrentPlatform) {
		case PlatformName.iOS:
			return Path.Combine (GetSDKRoot (), "lib", "bgen", "Xamarin.iOS.BindingAttributes.dll");
		case PlatformName.WatchOS:
			return Path.Combine (GetSDKRoot (), "lib", "bgen", "Xamarin.WatchOS.BindingAttributes.dll");
		case PlatformName.TvOS:
			return Path.Combine (GetSDKRoot (), "lib", "bgen", "Xamarin.TVOS.BindingAttributes.dll");
		case PlatformName.MacCatalyst:
			return Path.Combine (GetSDKRoot (), "lib", "bgen", "Xamarin.MacCatalyst.BindingAttributes.dll");
		case PlatformName.MacOSX:
			if (target_framework == TargetFramework.Xamarin_Mac_4_5_Full) {
				return Path.Combine (GetSDKRoot (), "lib", "bgen", "Xamarin.Mac-full.BindingAttributes.dll");
			} else if (target_framework == TargetFramework.Xamarin_Mac_4_5_System) {
				return Path.Combine (GetSDKRoot (), "lib", "bgen", "Xamarin.Mac-full.BindingAttributes.dll");
			} else if (target_framework == TargetFramework.Xamarin_Mac_2_0_Mobile) {
				return Path.Combine (GetSDKRoot (), "lib", "bgen", "Xamarin.Mac-mobile.BindingAttributes.dll");
			} else {
				throw ErrorHelper.CreateError (1053, target_framework);
			}
		default:
			throw new BindingException (1047, CurrentPlatform);
		}
	}

	IEnumerable<string> GetLibraryDirectories ()
	{
		if (!IsDotNet) {
			switch (CurrentPlatform) {
			case PlatformName.iOS:
				yield return Path.Combine (GetSDKRoot (), "lib", "mono", "Xamarin.iOS");
				break;
			case PlatformName.WatchOS:
				yield return Path.Combine (GetSDKRoot (), "lib", "mono", "Xamarin.WatchOS");
				break;
			case PlatformName.TvOS:
				yield return Path.Combine (GetSDKRoot (), "lib", "mono", "Xamarin.TVOS");
				break;
			case PlatformName.MacCatalyst:
				yield return Path.Combine (GetSDKRoot (), "lib", "mono", "Xamarin.MacCatalyst");
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
				} else {
					throw ErrorHelper.CreateError (1053, target_framework);
				}
				break;
			default:
				throw new BindingException (1047, CurrentPlatform);
			}
		}
		foreach (var lib in libs)
			yield return lib;
	}

	string GetSDKRoot ()
	{
		switch (CurrentPlatform) {
		case PlatformName.iOS:
		case PlatformName.WatchOS:
		case PlatformName.TvOS:
		case PlatformName.MacCatalyst:
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
			throw new BindingException (1047, CurrentPlatform);
		}
	}

	void SetTargetFramework (string fx)
	{
		TargetFramework tf;
		if (!TargetFramework.TryParse (fx, out tf))
			throw ErrorHelper.CreateError (68, fx);
		target_framework = tf;

		if (!TargetFramework.IsValidFramework (target_framework.Value))
			throw ErrorHelper.CreateError (70, target_framework.Value, string.Join (" ", TargetFramework.ValidFrameworks.Select ((v) => v.ToString ()).ToArray ()));
	}

	static int Main2 (string [] args)
	{
		using var touch = new BindingTouch ();
		return touch.Main3 (args);
	}

	int Main3 (string [] args)
	{
		bool show_help = false;
		bool zero_copy = false;
		string? basedir = null;
		string? tmpdir = null;
		string? ns = null;
		bool delete_temp = true, debug = false;
		bool unsafef = true;
		bool external = false;
		bool public_mode = true;
		bool nostdlib = false;
		bool? inline_selectors = null;
		List<string> sources;
		var resources = new List<string> ();
		var linkwith = new List<string> ();
		var api_sources = new List<string> ();
		var core_sources = new List<string> ();
		var extra_sources = new List<string> ();
		var defines = new List<string> ();
		string? generate_file_list = null;
		bool process_enums = false;

		ErrorHelper.ClearWarningLevels ();

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
			{ "core", "Use this to build product assemblies", v => BindThirdPartyLibrary = false },
			{ "r|reference=", "Adds a reference", v => references.Add (v) },
			{ "lib=", "Adds the directory to the search path for the compiler", v => libs.Add (v) },
			{ "compiler=", "Sets the compiler to use (Obsolete) ", v => compiler = v, true },
			{ "compile-command=", "Sets the command to execute the C# compiler (this be an executable + arguments).", v =>
				{
					if (!StringUtils.TryParseArguments (v, out compile_command, out var ex))
						throw ErrorHelper.CreateError (27, "--compile-command", ex);
				}
			},
			{ "sdk=", "Sets the .NET SDK to use (Obsolete)", v => {}, true },
			{ "new-style", "Build for Unified (Obsolete).", v => { Console.WriteLine ("The --new-style option is obsolete and ignored."); }, true},
			{ "d=", "Defines a symbol", v => defines.Add (v) },
			{ "api=", "Adds a API definition source file", v => api_sources.Add (v) },
			{ "s=", "Adds a source file required to build the API", v => core_sources.Add (v) },
			{ "q", "Quiet", v => ErrorHelper.Verbosity-- },
			{ "v", "Sets verbose mode", v => ErrorHelper.Verbosity++ },
			{ "x=", "Adds the specified file to the build, used after the core files are compiled", v => extra_sources.Add (v) },
			{ "e", "Generates smaller classes that can not be subclassed (previously called 'external mode')", v => external = true },
			{ "p", "Sets private mode", v => public_mode = false },
			{ "baselib=", "Sets the base library", v => baselibdll = v },
			{ "attributelib=", "Sets the attribute library", v => attributedll = v },
			{ "use-zero-copy", v=> zero_copy = true },
			{ "nostdlib", "Does not reference mscorlib.dll library", l => nostdlib = true },
			{ "no-mono-path", "Launches compiler with empty MONO_PATH", l => { }, true },
			{ "native-exception-marshalling", "Enable the marshalling support for Objective-C exceptions", (v) => { /* no-op */} },
			{ "inline-selectors:", "If Selector.GetHandle is inlined and does not need to be cached (enabled by default in Xamarin.iOS, disabled in Xamarin.Mac)",
				v => inline_selectors = string.Equals ("true", v, StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty (v)
			},
			{ "process-enums", "Process enums as bindings, not external, types.", v => process_enums = true },
			{ "link-with=,", "Link with a native library {0:FILE} to the binding, embedded as a resource named {1:ID}",
				(path, id) => {
					if (path is null || path.Length == 0)
						throw new Exception ("-link-with=FILE,ID requires a filename.");

					if (id is null || id.Length == 0)
						id = Path.GetFileName (path);

					if (linkwith.Contains (id))
						throw new Exception ("-link-with=FILE,ID cannot assign the same resource id to multiple libraries.");

					resources.Add (string.Format ("-res:{0},{1}", path, id));
					linkwith.Add (id);
				}
			},
			{ "unified-full-profile", "Launches compiler pointing to XM Full Profile", l => { /* no-op*/ }, true },
			{ "unified-mobile-profile", "Launches compiler pointing to XM Mobile Profile", l => { /* no-op*/ }, true },
			{ "target-framework=", "Specify target framework to use. Always required, and the currently supported values are: 'Xamarin.iOS,v1.0', 'Xamarin.TVOS,v1.0', 'Xamarin.WatchOS,v1.0', 'XamMac,v1.0', 'Xamarin.Mac,Version=v2.0,Profile=Mobile', 'Xamarin.Mac,Version=v4.5,Profile=Full' and 'Xamarin.Mac,Version=v4.5,Profile=System')", v => SetTargetFramework (v) },
			{ "warnaserror:", "An optional comma-separated list of warning codes that should be reported as errors (if no warnings are specified all warnings are reported as errors).", v => {
					try {
						if (!string.IsNullOrEmpty (v)) {
							foreach (var code in v.Split (new char [] { ',' }, StringSplitOptions.RemoveEmptyEntries))
								ErrorHelper.SetWarningLevel (ErrorHelper.WarningLevel.Error, int.Parse (code));
						} else {
							ErrorHelper.SetWarningLevel (ErrorHelper.WarningLevel.Error);
						}
					} catch (Exception ex) {
						throw ErrorHelper.CreateError (26, ex.Message);
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
						throw ErrorHelper.CreateError (26, ex.Message);
					}
				}
			},
			{ "no-nfloat-using:", "If a global using alias directive for 'nfloat = System.Runtime.InteropServices.NFloat' should automatically be created.", (v) => {
					noNFloatUsing = string.Equals ("true", v, StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty (v);
				}
			},
			{ "compiled-api-definition-assembly=", "An assembly with the compiled api definitions.", (v) => compiled_api_definition_assembly = v },
			new Mono.Options.ResponseFileSource (),
		};

		try {
			sources = os.Parse (args);
		} catch (Exception e) {
			Console.Error.WriteLine ("{0}: {1}", ToolName, e.Message);
			Console.Error.WriteLine ("see {0} --help for more information", ToolName);
			return 1;
		}

		if (show_help) {
			ShowHelp (os);
			return 0;
		}

		if (!target_framework.HasValue)
			throw ErrorHelper.CreateError (86);

		switch (target_framework.Value.Platform) {
		case ApplePlatform.iOS:
			CurrentPlatform = PlatformName.iOS;
			nostdlib = true;
			if (string.IsNullOrEmpty (baselibdll))
				baselibdll = Path.Combine (GetSDKRoot (), "lib/mono/Xamarin.iOS/Xamarin.iOS.dll");
			if (!IsDotNet) {
				references.Add ("Facades/System.Drawing.Common");
				ReferenceFixer.FixSDKReferences (GetSDKRoot (), "lib/mono/Xamarin.iOS", references);
			}
			break;
		case ApplePlatform.TVOS:
			CurrentPlatform = PlatformName.TvOS;
			nostdlib = true;
			if (string.IsNullOrEmpty (baselibdll))
				baselibdll = Path.Combine (GetSDKRoot (), "lib/mono/Xamarin.TVOS/Xamarin.TVOS.dll");
			if (!IsDotNet) {
				references.Add ("Facades/System.Drawing.Common");
				ReferenceFixer.FixSDKReferences (GetSDKRoot (), "lib/mono/Xamarin.TVOS", references);
			}
			break;
		case ApplePlatform.WatchOS:
			CurrentPlatform = PlatformName.WatchOS;
			nostdlib = true;
			if (string.IsNullOrEmpty (baselibdll))
				baselibdll = Path.Combine (GetSDKRoot (), "lib/mono/Xamarin.WatchOS/Xamarin.WatchOS.dll");
			if (!IsDotNet) {
				references.Add ("Facades/System.Drawing.Common");
				ReferenceFixer.FixSDKReferences (GetSDKRoot (), "lib/mono/Xamarin.WatchOS", references);
			}
			break;
		case ApplePlatform.MacCatalyst:
			CurrentPlatform = PlatformName.MacCatalyst;
			nostdlib = true;
			if (string.IsNullOrEmpty (baselibdll))
				baselibdll = Path.Combine (GetSDKRoot (), "lib/mono/Xamarin.MacCatalyst/Xamarin.MacCatalyst.dll");
			if (!IsDotNet) {
				// references.Add ("Facades/System.Drawing.Common");
				ReferenceFixer.FixSDKReferences (GetSDKRoot (), "lib/mono/Xamarin.MacCatalyst", references);
			}
			break;
		case ApplePlatform.MacOSX:
			CurrentPlatform = PlatformName.MacOSX;
			nostdlib = true;
			if (string.IsNullOrEmpty (baselibdll)) {
				if (target_framework == TargetFramework.Xamarin_Mac_2_0_Mobile)
					baselibdll = Path.Combine (GetSDKRoot (), "lib", "reference", "mobile", "Xamarin.Mac.dll");
				else if (target_framework == TargetFramework.Xamarin_Mac_4_5_Full || target_framework == TargetFramework.Xamarin_Mac_4_5_System)
					baselibdll = Path.Combine (GetSDKRoot (), "lib", "reference", "full", "Xamarin.Mac.dll");
				else if (target_framework == TargetFramework.DotNet_macOS)
					baselibdll = Path.Combine (GetSDKRoot (), "lib", "mono", "Xamarin.Mac", "Xamarin.Mac.dll");
				else
					throw ErrorHelper.CreateError (1053, target_framework);
			}
			if (target_framework == TargetFramework.Xamarin_Mac_2_0_Mobile) {
				skipSystemDrawing = true;
				references.Add ("Facades/System.Drawing.Common");
				ReferenceFixer.FixSDKReferences (GetSDKRoot (), "lib/mono/Xamarin.Mac", references);
			} else if (target_framework == TargetFramework.Xamarin_Mac_4_5_Full) {
				skipSystemDrawing = true;
				references.Add ("Facades/System.Drawing.Common");
				ReferenceFixer.FixSDKReferences (GetSDKRoot (), "lib/mono/4.5", references);
			} else if (target_framework == TargetFramework.Xamarin_Mac_4_5_System) {
				skipSystemDrawing = false;
				ReferenceFixer.FixSDKReferences ("/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.5", references, forceSystemDrawing: true);
			} else if (target_framework == TargetFramework.DotNet_macOS) {
				skipSystemDrawing = false;
			} else {
				throw ErrorHelper.CreateError (1053, target_framework);
			}

			break;
		default:
			throw ErrorHelper.CreateError (1053, target_framework);
		}

		if (sources.Count > 0) {
			api_sources.Insert (0, sources [0]);
			for (int i = 1; i < sources.Count; i++)
				core_sources.Insert (i - 1, sources [i]);
		}

		if (api_sources.Count == 0) {
			Console.WriteLine ("Error: no api file provided");
			ShowHelp (os);
			return 1;
		}

		if (tmpdir is null)
			tmpdir = GetWorkDir ();

		string firstApiDefinitionName = Path.GetFileNameWithoutExtension (api_sources [0]);
		firstApiDefinitionName = firstApiDefinitionName.Replace ('-', '_'); // This is not exhaustive, but common.
		if (outfile is null)
			outfile = firstApiDefinitionName + ".dll";

		var refs = references.Select ((v) => "-r:" + v);
		var paths = libs.Select ((v) => "-lib:" + v);

		try {
			var tmpass = GetCompiledApiBindingsAssembly (tmpdir, refs, nostdlib, api_sources, core_sources, defines, paths);
			universe = new MetadataLoadContext (
				new SearchPathsAssemblyResolver (
					GetLibraryDirectories ().ToArray (),
					references.ToArray ()),
				"mscorlib"
			);

			Assembly api;
			try {
				api = universe.LoadFromAssemblyPath (tmpass);
			} catch (Exception e) {
				if (Driver.Verbosity > 0)
					Console.WriteLine (e);

				Console.Error.WriteLine ("Error loading API definition from {0}", tmpass);
				return 1;
			}

			Assembly baselib;
			try {
				baselib = universe.LoadFromAssemblyPath (baselibdll);
			} catch (Exception e) {
				if (Driver.Verbosity > 0)
					Console.WriteLine (e);

				Console.Error.WriteLine ("Error loading base library {0}", baselibdll);
				return 1;
			}

			attributeManager ??= new AttributeManager (this);
			Frameworks = new Frameworks (CurrentPlatform);

			// Explicitly load our attribute library so that IKVM doesn't try (and fail) to find it.
			universe.LoadFromAssemblyPath (GetAttributeLibraryPath ());

			typeManager ??= new (this, api, universe.CoreAssembly, baselib);

			foreach (var linkWith in AttributeManager.GetCustomAttributes<LinkWithAttribute> (api)) {
#if NET
				if (string.IsNullOrEmpty (linkWith.LibraryName))
#else
				if (linkWith.LibraryName is null || string.IsNullOrEmpty (linkWith.LibraryName))
#endif
					continue;

				if (!linkwith.Contains (linkWith.LibraryName)) {
					Console.Error.WriteLine ("Missing native library {0}, please use `--link-with' to specify the path to this library.", linkWith.LibraryName);
					return 1;
				}
			}

			foreach (var r in references) {
				// IKVM has a bug where it doesn't correctly compare assemblies, which means it
				// can end up loading the same assembly (in particular any System.Runtime whose
				// version > 4.0, but likely others as well) more than once. This is bad, because
				// we compare types based on reference equality, which breaks down when there are
				// multiple instances of the same type.
				// 
				// So just don't ask IKVM to load assemblies that have already been loaded.
				var fn = Path.GetFileNameWithoutExtension (r);
				var assemblies = universe.GetAssemblies ();
				if (assemblies.Any ((v) => v.GetName ().Name == fn))
					continue;

				if (File.Exists (r)) {
					try {
						universe.LoadFromAssemblyPath (r);
					} catch (Exception ex) {
						ErrorHelper.Warning (1104, r, ex.Message);
					}
				}
			}

			var types = new List<Type> ();
			var strong_dictionaries = new List<Type> ();
			foreach (var t in api.GetTypes ()) {
				if ((process_enums && t.IsEnum) ||
					AttributeManager.HasAttribute<BaseTypeAttribute> (t) ||
					AttributeManager.HasAttribute<ProtocolAttribute> (t) ||
					AttributeManager.HasAttribute<StaticAttribute> (t) ||
					AttributeManager.HasAttribute<PartialAttribute> (t))
					types.Add (t);
				if (AttributeManager.HasAttribute<StrongDictionaryAttribute> (t))
					strong_dictionaries.Add (t);
			}

			namespaceManager ??= new NamespaceManager (
				this,
				ns ?? firstApiDefinitionName,
				skipSystemDrawing
			);

			var g = new Generator (this, public_mode, external, debug, types.ToArray (), strong_dictionaries.ToArray ()) {
				BaseDir = basedir ?? tmpdir,
				ZeroCopyStrings = zero_copy,
				InlineSelectors = inline_selectors ?? (CurrentPlatform != PlatformName.MacOSX),
			};

			g.Go ();

			if (generate_file_list is not null) {
				using (var f = File.CreateText (generate_file_list)) {
					foreach (var x in g.GeneratedFiles.OrderBy ((v) => v))
						f.WriteLine (x);
				}
				return 0;
			}

			var cargs = new List<string> ();
			if (unsafef)
				cargs.Add ("-unsafe");
			cargs.Add ("-target:library");
			cargs.Add ("-out:" + outfile);
			foreach (var def in defines)
				cargs.Add ("-define:" + def);
#if NET
			cargs.Add ("-define:NET");
#endif
			cargs.AddRange (g.GeneratedFiles);
			cargs.AddRange (core_sources);
			cargs.AddRange (extra_sources);
			cargs.AddRange (refs);
			cargs.Add ("-r:" + baselibdll);
			cargs.AddRange (resources);
			if (nostdlib) {
				cargs.Add ("-nostdlib");
				cargs.Add ("-noconfig");
			}
			if (!string.IsNullOrEmpty (Path.GetDirectoryName (baselibdll)))
				cargs.Add ("-lib:" + Path.GetDirectoryName (baselibdll));

			AddNFloatUsing (cargs, tmpdir);

			Compile (cargs, 1000, tmpdir);
		} finally {
			if (delete_temp)
				Directory.Delete (tmpdir, true);
		}
		return 0;
	}

	// If anything is modified in this function, check if the _CompileApiDefinitions MSBuild target needs to be updated as well.
	string GetCompiledApiBindingsAssembly (string tmpdir, IEnumerable<string> refs, bool nostdlib, List<string> api_sources, List<string> core_sources, List<string> defines, IEnumerable<string> paths)
	{
		if (!string.IsNullOrEmpty (compiled_api_definition_assembly))
			return compiled_api_definition_assembly;

		var tmpass = Path.Combine (tmpdir, "temp.dll");

		// -nowarn:436 is to avoid conflicts in definitions between core.dll and the sources
		// Keep source files at the end of the command line - csc will create TWO assemblies if any sources preceed the -out parameter
		var cargs = new List<string> ();

		cargs.Add ("-debug");
		cargs.Add ("-unsafe");
		cargs.Add ("-target:library");
		cargs.Add ("-nowarn:436");
		cargs.Add ("-out:" + tmpass);
		cargs.Add ("-r:" + GetAttributeLibraryPath ());
		cargs.AddRange (refs);
		cargs.Add ("-r:" + baselibdll);
		foreach (var def in defines)
			cargs.Add ("-define:" + def);
#if NET
		cargs.Add ("-define:NET");
#endif
		cargs.AddRange (paths);
		if (nostdlib) {
			cargs.Add ("-nostdlib");
			cargs.Add ("-noconfig");
		}
		cargs.AddRange (api_sources);
		cargs.AddRange (core_sources);
		if (!string.IsNullOrEmpty (Path.GetDirectoryName (baselibdll)))
			cargs.Add ("-lib:" + Path.GetDirectoryName (baselibdll));

		AddNFloatUsing (cargs, tmpdir);

		Compile (cargs, 2, tmpdir);

		return tmpass;
	}

	void AddNFloatUsing (List<string> cargs, string tmpdir)
	{
#if NET
		if (noNFloatUsing)
			return;
		var tmpusing = Path.Combine (tmpdir, "GlobalUsings.g.cs");
		File.WriteAllText (tmpusing, "global using nfloat = global::System.Runtime.InteropServices.NFloat;\n");
		cargs.Add (tmpusing);
#endif
	}

	void Compile (List<string> arguments, int errorCode, string tmpdir)
	{
		var responseFile = Path.Combine (tmpdir, $"compile-{errorCode}.rsp");
		// The /noconfig argument is not allowed in a response file, so don't put it there.
		var responseFileArguments = arguments
			.Where (arg => !string.Equals (arg, "/noconfig", StringComparison.OrdinalIgnoreCase) && !string.Equals (arg, "-noconfig", StringComparison.OrdinalIgnoreCase))
			.ToArray (); // StringUtils.QuoteForProcess only accepts IList, not IEnumerable
		File.WriteAllLines (responseFile, StringUtils.QuoteForProcess (responseFileArguments));
		// We create a new list here on purpose to not modify the input argument.
		arguments = arguments.Where (arg => !responseFileArguments.Contains (arg)).ToList ();
		arguments.Add ($"@{responseFile}");

		if (compile_command is null || compile_command.Length == 0) {
#if !NET
			if (string.IsNullOrEmpty (compiler))
				compiler = DefaultCompiler;
#endif
			if (string.IsNullOrEmpty (compiler))
				throw ErrorHelper.CreateError (28);
			compile_command = new string [] { compiler };
		}

		for (var i = 1; i < compile_command.Length; i++) {
			arguments.Insert (i - 1, compile_command [i]);
		}

		if (Driver.RunCommand (compile_command [0], arguments, null, out var compile_output, true, Driver.Verbosity) != 0)
			throw ErrorHelper.CreateError (errorCode, $"{compiler} {StringUtils.FormatArguments (arguments)}\n{compile_output}".Replace ("\n", "\n\t"));
		var output = string.Join (Environment.NewLine, compile_output.ToString ().Split (new char [] { '\n' }, StringSplitOptions.RemoveEmptyEntries));
		if (!string.IsNullOrEmpty (output))
			Console.WriteLine (output);
	}

	static string GetWorkDir ()
	{
		while (true) {
			string p = Path.Combine (Path.GetTempPath (), Path.GetRandomFileName ());
			if (Directory.Exists (p))
				continue;

			var di = Directory.CreateDirectory (p);
			return di.FullName;
		}
	}

	protected virtual void Dispose (bool disposing)
	{
		if (!disposedValue) {
			if (disposing) {
				universe?.Dispose ();
				universe = null;
			}

			disposedValue = true;
		}
	}

	public void Dispose ()
	{
		Dispose (disposing: true);
		GC.SuppressFinalize (this);
	}
}

static class ReferenceFixer {
	public static void FixSDKReferences (string sdkRoot, string sdk_offset, List<string> references) => FixSDKReferences (Path.Combine (sdkRoot, sdk_offset), references);

	public static void FixSDKReferences (string sdk_path, List<string> references, bool forceSystemDrawing = false)
	{
		FixRelativeReferences (sdk_path, references);
		AddMissingRequiredReferences (sdk_path, references, forceSystemDrawing);
	}

	static bool ContainsReference (List<string> references, string name) => references.Any (v => Path.GetFileNameWithoutExtension (v) == name);
	static void AddSDKReference (List<string> references, string sdk_path, string name) => references.Add (Path.Combine (sdk_path, name));

	static void AddMissingRequiredReferences (string sdk_path, List<string> references, bool forceSystemDrawing = false)
	{
		foreach (var requiredLibrary in new string [] { "System", "mscorlib", "System.Core" }) {
			if (!ContainsReference (references, requiredLibrary))
				AddSDKReference (references, sdk_path, requiredLibrary + ".dll");
		}
		if (forceSystemDrawing && !ContainsReference (references, "System.Drawing"))
			AddSDKReference (references, sdk_path, "System.Drawing.dll");
	}

	static bool ExistsInSDK (string sdk_path, string name) => File.Exists (Path.Combine (sdk_path, name));

	static void FixRelativeReferences (string sdk_path, List<string> references)
	{
		foreach (var r in references.Where (x => ExistsInSDK (sdk_path, x + ".dll")).ToList ()) {
			references.Remove (r);
			AddSDKReference (references, sdk_path, r + ".dll");
		}
	}
}

class SearchPathsAssemblyResolver : MetadataAssemblyResolver {
	readonly string [] libraryPaths;
	readonly string [] references;

	public SearchPathsAssemblyResolver (string [] libraryPaths, string [] references)
	{
		this.libraryPaths = libraryPaths;
		this.references = references;
	}

	public override Assembly? Resolve (MetadataLoadContext context, AssemblyName assemblyName)
	{
		string? name = assemblyName.Name;
		if (name is not null) {
			foreach (var asm in context.GetAssemblies ()) {
				if (asm.GetName ().Name == name)
					return asm;
			}

			string dllName = name + ".dll";
			foreach (var libraryPath in libraryPaths) {
				string path = Path.Combine (libraryPath, dllName);
				if (File.Exists (path)) {
					return context.LoadFromAssemblyPath (path);
				}
			}
			foreach (var reference in references) {
				if (Path.GetFileName (reference).Equals (dllName, StringComparison.OrdinalIgnoreCase)) {
					return context.LoadFromAssemblyPath (reference);
				}
			}
		}
		return null;
	}
}
