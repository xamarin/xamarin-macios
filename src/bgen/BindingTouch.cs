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

using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Mono.Options;

using Xamarin.Utils;

#if XAMMACIOS_DEBUGGER
using System.Diagnostics;
#endif

public class BindingTouch : IDisposable, IToolLog {
	readonly IToolLog log;

	public BindingTouch (IToolLog log)
	{
		this.log = log;
		Verbosity = log.Verbosity;
	}

	public static ApplePlatform [] AllPlatforms = new ApplePlatform [] { ApplePlatform.iOS, ApplePlatform.MacOSX, ApplePlatform.TVOS, ApplePlatform.MacCatalyst };
	public static PlatformName [] AllPlatformNames = new PlatformName [] { PlatformName.iOS, PlatformName.MacOSX, PlatformName.TvOS, PlatformName.MacCatalyst };
	public PlatformName CurrentPlatform;
	public ApplePlatform Platform { get => CurrentPlatform.AsApplePlatform (); }
	public bool BindThirdPartyLibrary = true;
	public string? outfile;

	string compiled_api_definition_assembly = string.Empty;
	bool supportsXmlDocumentation = true;
	List<string> references = new List<string> ();

	public bool SupportsXmlDocumentation { get => supportsXmlDocumentation; }

	public MetadataLoadContext? universe;
	public Frameworks? Frameworks;

	DocumentationManager? documentationManager;
	public DocumentationManager DocumentationManager => documentationManager!;

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

	static void ShowHelp (IToolLog log, OptionSet os)
	{
		log.Log ("{0} - Mono Objective-C API binder", ToolName);
		log.Log ("Usage is:\n {0} [options] --compiled-api-definition-assembly=api.dll --sourceonly=generated-sources.txt --tmpdir=generated-sources", ToolName);

		using var writer = new StringWriter ();
		os.WriteOptionDescriptions (writer);
		log.Log (writer.ToString ().TrimEnd ());
	}

	public static int Main (string [] args)
	{
		return Run (args, ConsoleLog.Instance);
	}

	public static int Run (string [] args, IToolLog log)
	{
		try {
#if XAMMACIOS_DEBUGGER
			// the following code will only be available for the macios
			// developers to be able to debug the generator. This will
			// block the generator until a debugger has attached to it
			// our customers won't find any use for this.
			var process = Process.GetCurrentProcess();
			log.Log ($"Waiting for debugger to attach: ({ process.Id}) {process.ProcessName} { string.Join (" ", args)}");
			while (!Debugger.IsAttached) {
				Thread.Sleep (100);
			}

			log.Log ("Debugger attached");
#endif
			return Main2 (args, log);
		} catch (Exception ex) {
			ErrorHelper.Show (log, ex, false);
			return 1;
		}
	}

	static int Main2 (string [] args, IToolLog log)
	{
		using var touch = new BindingTouch (log);
		return touch.Main3 (args);
	}

	public bool TryCreateOptionSet (BindingTouchConfig config, string [] args)
	{
		try {
			config.OptionSet = new OptionSet () {
				{ "h|?|help", "Displays the help", v => config.ShowHelp = true },
				{ "a", "Include alpha bindings (Obsolete).", v => {}, true },
				{ "outdir=", "Sets the output directory for the generated binding source files", v => { config.BindingFilesOutputDirectory = v; }},
				{ "o|out=", "Sets the name of the generated binding assembly", v => outfile = v },
				{ "tmpdir=", "Sets the working directory for temp files", v => { config.TemporaryFileDirectory = v; config.DeleteTemporaryFiles = false; }},
				{ "debug", "Generates a debugging build of the binding", v => config.IsDebug = true },
				{ "sourceonly=", "Writes the generated source file list", v => config.GeneratedFileList = v },
				{ "ns=", "Sets the namespace for storing helper classes", v => config.HelperClassNamespace = v },
				{ "core", "Use this to build product assemblies", v => BindThirdPartyLibrary = false },
				{ "r|reference=", "Adds a reference", v => references.Add (v) },
				{ "lib=", "Adds a directory to the assembly search path", v => LibraryManager.Libraries.Add (v) },
				{ "sdk=", "Sets the .NET SDK to use (Obsolete)", v => {}, true },
				{ "new-style", "Build for Unified (Obsolete).", v => { Log ("The --new-style option is obsolete and ignored."); }, true},
				{ "q", "Quiet", v => Verbosity-- },
				{ "v", "Sets verbose mode", v => Verbosity++ },
				{ "e", "Generates smaller classes that can not be subclassed (previously called 'external mode')", v => config.IsExternal = true },
				{ "p", "Sets private mode", v => config.IsPublicMode = false },
				{ "baselib=", "Sets the base library", v => config.Baselibdll = v },
				{ "attributelib=", "Sets the attribute library", v => config.Attributedll = v },
#if !XAMCORE_5_0
				{ "use-zero-copy", v => ErrorHelper.Warning (this, 1027) },
#endif
				{ "nostdlib", "Does not reference mscorlib.dll library", l => config.OmitStandardLibrary = true },
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

						config.LinkWith.Add (id);
					}
				},
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
								foreach (var code in v.Split (new char [] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
									if (int.TryParse (code, out var nowarnCode))
										ErrorHelper.SetWarningLevel (ErrorHelper.WarningLevel.Disable, nowarnCode);
								}
							} else {
								ErrorHelper.SetWarningLevel (ErrorHelper.WarningLevel.Disable);
							}
						} catch (Exception ex) {
							throw ErrorHelper.CreateError (27, "--nowarn", ex.Message);
						}
					}
				},
				{ "compiled-api-definition-assembly=", "An assembly with the compiled api definitions.", (v) => compiled_api_definition_assembly = v },
				{ "xmldoc:", "If the generator supports xml documentation in the API definition (default: true)", (v) => {
						supportsXmlDocumentation = string.Equals ("true", v, StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty (v);
					}
				},
				new Mono.Options.ResponseFileSource (),
			};
			var extra = config.OptionSet.Parse (args);
			if (extra.Count > 0) {
				var message = extra [0].StartsWith ("-", StringComparison.Ordinal) ?
					$"Unknown option: '{extra [0]}'." :
					"API definition source files are no longer supported; use --compiled-api-definition-assembly.";
				throw new OptionException (message, extra [0]);
			}
		} catch (Exception e) {
			LogError ($"{ToolName}: {e.Message}");
			LogError ($"see {ToolName} --help for more information");
			return false;
		}

		return true;
	}

	public bool TryInitializeApi (BindingTouchConfig config, [NotNullWhen (true)] out Api? api)
	{
		api = null;
		if (string.IsNullOrEmpty (compiled_api_definition_assembly)) {
			Log ("Error: no compiled api definition assembly provided");
			ShowHelp (this, config.OptionSet);
			return false;
		}
		if (config.TemporaryFileDirectory is null)
			config.TemporaryFileDirectory = GetWorkDir ();

		var firstApiDefinitionName = Path.GetFileNameWithoutExtension (compiled_api_definition_assembly);
		firstApiDefinitionName = firstApiDefinitionName.Replace ('-', '_'); // This is not exhaustive, but common.
		if (outfile is null)
			outfile = firstApiDefinitionName + ".dll";

		try {
			var tmpass = compiled_api_definition_assembly;
			universe = new MetadataLoadContext (
				new SearchPathsAssemblyResolver (
					LibraryManager.GetLibraryDirectories (LibraryInfo, CurrentPlatform).ToArray (),
					references.ToArray ()),
				"mscorlib"
			);

			if (!TryLoadApi (tmpass, out Assembly? apiAssembly) ||
				!TryLoadApi (LibraryInfo.BaseLibDll, out Assembly? baselib))
				return false;

			documentationManager = new DocumentationManager (supportsXmlDocumentation ? tmpass : string.Empty);

			Frameworks = new Frameworks (CurrentPlatform);

			// Explicitly load our attribute library so that IKVM doesn't try (and fail) to find it.
			universe.LoadFromAssemblyPath (LibraryManager.GetAttributeLibraryPath (LibraryInfo, CurrentPlatform));

			typeCache ??= new (universe, Frameworks, CurrentPlatform, apiAssembly, universe.CoreAssembly, baselib,
				BindThirdPartyLibrary);
			attributeManager ??= new (this, typeCache);
			typeManager ??= new (this);

			if (!TestLinkWith (apiAssembly, config))
				return false;

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
						ErrorHelper.Warning (this, 1104, r, ex.Message);
					}
				}
			}

			api = TypeManager.ParseApi (apiAssembly, config.ProcessEnums);
			namespaceCache ??= new NamespaceCache (
				CurrentPlatform,
				config.HelperClassNamespace ?? firstApiDefinitionName
			);


		} catch (Exception ex) {
			ErrorHelper.Show (this, ex);
			return false;
		}

		return true;
	}

	bool ValidateGeneratedSourceOutput (BindingTouchConfig config)
	{
		if (string.IsNullOrEmpty (config.GeneratedFileList)) {
			Log ("Error: no generated source file list provided");
			ShowHelp (this, config.OptionSet);
			return false;
		}
		if (config.BindingFilesOutputDirectory is null && config.DeleteTemporaryFiles) {
			Log ("Error: no persistent generated source output directory provided");
			ShowHelp (this, config.OptionSet);
			return false;
		}

		return true;
	}

	int Main3 (string [] args)
	{
		ErrorHelper.ClearWarningLevels ();
		BindingTouchConfig config = new ();

		if (!TryCreateOptionSet (config, args))
			return 1;

		if (config.ShowHelp) {
			ShowHelp (this, config.OptionSet);
			return 0;
		}

		if (!ValidateGeneratedSourceOutput (config))
			return 1;

		libraryInfo = LibraryInfo.LibraryInfoBuilder.Build (references, config);
		CurrentPlatform = LibraryManager.DetermineCurrentPlatform (TargetFramework.Platform);

		if (!TryInitializeApi (config, out Api? api) || !TryGenerate (config, api))
			return 1;

		return 0;
	}

	bool TryGenerate (BindingTouchConfig config, Api api)
	{
		try {
			var g = new Generator (this, api, config.IsPublicMode, config.IsExternal, config.IsDebug) {
				BaseDir = config.BindingFilesOutputDirectory ?? config.TemporaryFileDirectory!,
				InlineSelectors = config.InlineSelectors ?? (CurrentPlatform != PlatformName.MacOSX),
			};

			g.Go ();
			if (config.GeneratedFileList is not null) {
				using (var f = File.CreateText (config.GeneratedFileList)) {
					foreach (var x in g.GeneratedFiles.OrderBy ((v) => v))
						f.WriteLine (x);
				}
			}
		} finally {
			if (config.DeleteTemporaryFiles && config.TemporaryFileDirectory is not null)
				Directory.Delete (config.TemporaryFileDirectory, true);
		}

		return true;
	}

	bool TestLinkWith (Assembly apiAssembly, BindingTouchConfig config)
	{
		foreach (var linkWith in AttributeManager.GetCustomAttributes<LinkWithAttribute> (apiAssembly)) {
			if (string.IsNullOrEmpty (linkWith.LibraryName))
				continue;

			if (!config.LinkWith.Contains (linkWith.LibraryName)) {
				LogError ($"Missing native library {linkWith.LibraryName}, please use `--link-with' to specify the path to this library.");
				return false; // return 1;
			}
		}

		return true;
	}

	bool TryLoadApi (string? name, [NotNullWhen (true)] out Assembly? assembly)
	{
		assembly = null;
		if (string.IsNullOrEmpty (name))
			return false;
		try {
			assembly = universe?.LoadFromAssemblyPath (name);
		} catch (Exception e) {
			if (Verbosity > 0)
				Log (e.ToString ());

			LogError ($"Error loading {name}");
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

	public void Log (string message)
	{
		log.Log (message);
	}

	public void LogError (string message)
	{
		log.LogError (message);
	}

	public void LogError (BindingException exception)
	{
		log.LogError (exception);
	}

	public void LogWarning (BindingException exception)
	{
		log.LogWarning (exception);
	}

	public void LogException (Exception exception)
	{
		log.LogException (exception);
	}

	int verbosity = 0;
	public int Verbosity {
		get => verbosity;
		set => verbosity = value;
	}
}

namespace Xamarin.Bundler {
	public partial class Driver {
		public static int GetDefaultVerbosity () => 0;
	}
}
