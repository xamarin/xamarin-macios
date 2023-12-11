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
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Mono.Options;

using ObjCRuntime;
using Foundation;

using Xamarin.Bundler;
using Xamarin.Utils;

public class BindingTouch : IDisposable {
#if NET
	public static ApplePlatform [] AllPlatforms = new ApplePlatform [] { ApplePlatform.iOS, ApplePlatform.MacOSX, ApplePlatform.TVOS, ApplePlatform.MacCatalyst };
	public static PlatformName [] AllPlatformNames = new PlatformName [] { PlatformName.iOS, PlatformName.MacOSX, PlatformName.TvOS, PlatformName.MacCatalyst };
#else
	public static ApplePlatform [] AllPlatforms = new ApplePlatform [] { ApplePlatform.iOS, ApplePlatform.MacOSX, ApplePlatform.TVOS, ApplePlatform.WatchOS };
	public static PlatformName [] AllPlatformNames = new PlatformName [] { PlatformName.iOS, PlatformName.MacOSX, PlatformName.TvOS, PlatformName.WatchOS };
#endif
	public PlatformName CurrentPlatform;
	public bool BindThirdPartyLibrary = true;
	public string? outfile;

#if !NET
	const string DefaultCompiler = "/Library/Frameworks/Mono.framework/Versions/Current/bin/csc";
#endif
	string compiler = string.Empty;
	string []? compile_command = null;
	string compiled_api_definition_assembly = string.Empty;
	bool noNFloatUsing;
	List<string> references = new List<string> ();

	public MetadataLoadContext? universe;
	public Frameworks? Frameworks;

	Assembly? apiAssembly;
	AttributeManager? attributeManager;
	public AttributeManager AttributeManager => attributeManager!;

	TypeManager? typeManager;
	public TypeManager TypeManager => typeManager!;

	NamespaceCache? namespaceCache;
	public NamespaceCache NamespaceCache => namespaceCache!;

	TypeCache? typeCache;
	public TypeCache TypeCache => typeCache!;
	public LibraryManager LibraryManager = new ();

	bool disposedValue;

	LibraryInfo? libraryInfo;
	public LibraryInfo LibraryInfo => libraryInfo!;

	public TargetFramework TargetFramework {
		get { return LibraryInfo.TargetFramework; }
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

	static int Main2 (string [] args)
	{
		using var touch = new BindingTouch ();
		return touch.Main3 (args);
	}

	public OptionSet CreateOptionSet (BindingTouchConfig config)
	{
		return new OptionSet () {
			{ "h|?|help", "Displays the help", v => config.ShowHelp = true },
			{ "a", "Include alpha bindings (Obsolete).", v => {}, true },
			{ "outdir=", "Sets the output directory for the temporary binding files", v => { config.BindingFilesOutputDirectory = v; }},
			{ "o|out=", "Sets the name of the output library", v => outfile = v },
			{ "tmpdir=", "Sets the working directory for temp files", v => { config.TemporaryFileDirectory = v; config.DeleteTemporaryFiles = false; }},
			{ "debug", "Generates a debugging build of the binding", v => config.IsDebug = true },
			{ "sourceonly=", "Only generates the source", v => config.GeneratedFileList = v },
			{ "ns=", "Sets the namespace for storing helper classes", v => config.HelperClassNamespace = v },
			{ "unsafe", "Sets the unsafe flag for the build", v=> config.IsUnsafe = true },
			{ "core", "Use this to build product assemblies", v => BindThirdPartyLibrary = false },
			{ "r|reference=", "Adds a reference", v => references.Add (v) },
			{ "lib=", "Adds the directory to the search path for the compiler", v => LibraryManager.Libraries.Add (v) },
			{ "compiler=", "Sets the compiler to use (Obsolete) ", v => compiler = v, true },
			{ "compile-command=", "Sets the command to execute the C# compiler (this be an executable + arguments).", v =>
				{
					if (!StringUtils.TryParseArguments (v, out compile_command, out var ex))
						throw ErrorHelper.CreateError (27, "--compile-command", ex);
				}
			},
			{ "sdk=", "Sets the .NET SDK to use (Obsolete)", v => {}, true },
			{ "new-style", "Build for Unified (Obsolete).", v => { Console.WriteLine ("The --new-style option is obsolete and ignored."); }, true},
			{ "d=", "Defines a symbol", v => config.Defines.Add (v) },
			{ "api=", "Adds a API definition source file", v => config.ApiSources.Add (v) },
			{ "s=", "Adds a source file required to build the API", v => config.CoreSources.Add (v) },
			{ "q", "Quiet", v => ErrorHelper.Verbosity-- },
			{ "v", "Sets verbose mode", v => ErrorHelper.Verbosity++ },
			{ "x=", "Adds the specified file to the build, used after the core files are compiled", v => config.ExtraSources.Add (v) },
			{ "e", "Generates smaller classes that can not be subclassed (previously called 'external mode')", v => config.IsExternal = true },
			{ "p", "Sets private mode", v => config.IsPublicMode = false },
			{ "baselib=", "Sets the base library", v => config.Baselibdll = v },
			{ "attributelib=", "Sets the attribute library", v => config.Attributedll = v },
			{ "use-zero-copy", v=> config.UseZeroCopy = true },
			{ "nostdlib", "Does not reference mscorlib.dll library", l => config.OmitStandardLibrary = true },
			{ "no-mono-path", "Launches compiler with empty MONO_PATH", l => { }, true },
			{ "native-exception-marshalling", "Enable the marshalling support for Objective-C exceptions", (v) => { /* no-op */} },
			{ "inline-selectors:", "If Selector.GetHandle is inlined and does not need to be cached (enabled by default in Xamarin.iOS, disabled in Xamarin.Mac)",
				v => config.InlineSelectors = string.Equals ("true", v, StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty (v)
			},
			{ "process-enums", "Process enums as bindings, not external, types.", v => config.ProcessEnums = true },
			{ "link-with=,", "Link with a native library {0:FILE} to the binding, embedded as a resource named {1:ID}",
				(path, id) => {
					if (path is null || path.Length == 0)
						throw new Exception ("-link-with=FILE,ID requires a filename.");

					if (id is null || id.Length == 0)
						id = Path.GetFileName (path);

					if (config.LinkWith.Contains (id))
						throw new Exception ("-link-with=FILE,ID cannot assign the same resource id to multiple libraries.");

					config.Resources.Add (string.Format ("-res:{0},{1}", path, id));
					config.LinkWith.Add (id);
				}
			},
			{ "unified-full-profile", "Launches compiler pointing to XM Full Profile", l => { /* no-op*/ }, true },
			{ "unified-mobile-profile", "Launches compiler pointing to XM Mobile Profile", l => { /* no-op*/ }, true },
			{ "target-framework=", "Specify target framework to use. Always required, and the currently supported values are: 'Xamarin.iOS,v1.0', 'Xamarin.TVOS,v1.0', 'Xamarin.WatchOS,v1.0', 'XamMac,v1.0', 'Xamarin.Mac,Version=v2.0,Profile=Mobile', 'Xamarin.Mac,Version=v4.5,Profile=Full' and 'Xamarin.Mac,Version=v4.5,Profile=System')", v => config.TargetFramework = v },
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
	}

	int Main3 (string [] args)
	{
		ErrorHelper.ClearWarningLevels ();
		BindingTouchConfig config = new ();
		OptionSet os = CreateOptionSet (config);

		try {
			config.Sources = os.Parse (args);
		} catch (Exception e) {
			Console.Error.WriteLine ("{0}: {1}", ToolName, e.Message);
			Console.Error.WriteLine ("see {0} --help for more information", ToolName);
			return 1;
		}

		if (config.ShowHelp) {
			ShowHelp (os);
			return 0;
		}

		libraryInfo = LibraryInfo.LibraryInfoBuilder.Build (references, config);
		CurrentPlatform = LibraryManager.DetermineCurrentPlatform (TargetFramework.Platform);

		if (config.Sources.Count > 0) {
			config.ApiSources.Insert (0, config.Sources [0]);
			for (int i = 1; i < config.Sources.Count; i++)
				config.CoreSources.Insert (i - 1, config.Sources [i]);
		}

		if (config.ApiSources.Count == 0) {
			Console.WriteLine ("Error: no api file provided");
			ShowHelp (os);
			return 1;
		}

		if (config.TemporaryFileDirectory is null)
			config.TemporaryFileDirectory = GetWorkDir ();

		string firstApiDefinitionName = Path.GetFileNameWithoutExtension (config.ApiSources [0]);
		firstApiDefinitionName = firstApiDefinitionName.Replace ('-', '_'); // This is not exhaustive, but common.
		if (outfile is null)
			outfile = firstApiDefinitionName + ".dll";

		var refs = references.Select ((v) => "-r:" + v);
		var paths = LibraryManager.Libraries.Select ((v) => "-lib:" + v);

		try {
			var tmpass = GetCompiledApiBindingsAssembly (LibraryInfo, config.TemporaryFileDirectory, refs, LibraryInfo.OmitStandardLibrary, config.ApiSources, config.CoreSources, config.Defines, paths);
			universe = new MetadataLoadContext (
				new SearchPathsAssemblyResolver (
					LibraryManager.GetLibraryDirectories (LibraryInfo, CurrentPlatform).ToArray (),
					references.ToArray ()),
				"mscorlib"
			);

			if (!TryLoadApi (tmpass, out apiAssembly) || !TryLoadApi (LibraryInfo.BaseLibDll, out Assembly? baselib))
				return 1;

			Frameworks = new Frameworks (CurrentPlatform);

			// Explicitly load our attribute library so that IKVM doesn't try (and fail) to find it.
			universe.LoadFromAssemblyPath (LibraryManager.GetAttributeLibraryPath (LibraryInfo, CurrentPlatform));

			typeCache ??= new (universe, Frameworks, CurrentPlatform, apiAssembly, universe.CoreAssembly, baselib, BindThirdPartyLibrary);
			attributeManager ??= new (typeCache);
			typeManager ??= new (this);

			foreach (var linkWith in AttributeManager.GetCustomAttributes<LinkWithAttribute> (apiAssembly)) {
#if NET
				if (string.IsNullOrEmpty (linkWith.LibraryName))
#else
				if (linkWith.LibraryName is null || string.IsNullOrEmpty (linkWith.LibraryName))
#endif
					continue;

				if (!config.LinkWith.Contains (linkWith.LibraryName)) {
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

			var api = TypeManager.ParseApi (apiAssembly, config.ProcessEnums);
			namespaceCache ??= new NamespaceCache (
				CurrentPlatform,
				config.HelperClassNamespace ?? firstApiDefinitionName,
				LibraryManager.DetermineSkipSystemDrawing (LibraryInfo.TargetFramework)
			);

			var g = new Generator (this, api, config.IsPublicMode, config.IsExternal, config.IsDebug) {
				BaseDir = config.BindingFilesOutputDirectory ?? config.TemporaryFileDirectory,
				ZeroCopyStrings = config.UseZeroCopy,
				InlineSelectors = config.InlineSelectors ?? (CurrentPlatform != PlatformName.MacOSX),
			};

			g.Go ();

			if (config.GeneratedFileList is not null) {
				using (var f = File.CreateText (config.GeneratedFileList)) {
					foreach (var x in g.GeneratedFiles.OrderBy ((v) => v))
						f.WriteLine (x);
				}
				return 0;
			}

			var cargs = new List<string> ();
			if (config.IsUnsafe)
				cargs.Add ("-unsafe");
			cargs.Add ("-target:library");
			cargs.Add ("-out:" + outfile);
			foreach (var def in config.Defines)
				cargs.Add ("-define:" + def);
#if NET
			cargs.Add ("-define:NET");
#endif
			cargs.AddRange (g.GeneratedFiles);
			cargs.AddRange (config.CoreSources);
			cargs.AddRange (config.ExtraSources);
			cargs.AddRange (refs);
			cargs.Add ("-r:" + LibraryInfo.BaseLibDll);
			cargs.AddRange (config.Resources);
			if (LibraryInfo.OmitStandardLibrary) {
				cargs.Add ("-nostdlib");
				cargs.Add ("-noconfig");
			}
			if (!string.IsNullOrEmpty (Path.GetDirectoryName (LibraryInfo.BaseLibDll)))
				cargs.Add ("-lib:" + Path.GetDirectoryName (LibraryInfo.BaseLibDll));

			AddNFloatUsing (cargs, config.TemporaryFileDirectory);

			Compile (cargs, 1000, config.TemporaryFileDirectory);
		} finally {
			if (config.DeleteTemporaryFiles)
				Directory.Delete (config.TemporaryFileDirectory, true);
		}
		return 0;
	}

	public bool IsApiAssembly (Assembly assembly)
	{
		return assembly == apiAssembly;
	}

	// If anything is modified in this function, check if the _CompileApiDefinitions MSBuild target needs to be updated as well.
	string GetCompiledApiBindingsAssembly (LibraryInfo libraryInfo, string tmpdir, IEnumerable<string> refs, bool nostdlib, List<string> api_sources, List<string> core_sources, List<string> defines, IEnumerable<string> paths)
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
		cargs.Add ("-r:" + LibraryManager.GetAttributeLibraryPath (libraryInfo, CurrentPlatform));
		cargs.AddRange (refs);
		cargs.Add ("-r:" + libraryInfo.BaseLibDll);
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
		if (!string.IsNullOrEmpty (Path.GetDirectoryName (libraryInfo.BaseLibDll)))
			cargs.Add ("-lib:" + Path.GetDirectoryName (libraryInfo.BaseLibDll));

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

	bool TryLoadApi (string? name, [NotNullWhen (true)] out Assembly? assembly)
	{
		assembly = null;
		if (string.IsNullOrEmpty (name))
			return false;
		try {
			assembly = universe?.LoadFromAssemblyPath (name);
		} catch (Exception e) {
			if (Driver.Verbosity > 0)
				Console.WriteLine (e);

			Console.Error.WriteLine ("Error loading {0}", name);
		}

		return assembly is not null;
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
