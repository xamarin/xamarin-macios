using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.MacDev;
using Xamarin.Utils;
using ClassRedirector;

namespace Xamarin.Bundler {
	public abstract class ProcessTask : BuildTask {
		public string FileName;
		public IList<string> Arguments;
		public Dictionary<string, string> Environment = new Dictionary<string, string> ();
		protected StringBuilder Output;

		protected string Command {
			get {
				var result = new StringBuilder ();
				if (Environment.TryGetValue ("MONO_PATH", out var mono_path)) {
					result.Append ("MONO_PATH=");
					result.Append (StringUtils.Quote (mono_path));
					result.Append (' ');
				}
				result.Append (FileName);
				result.Append (' ');
				result.Append (StringUtils.FormatArguments (Arguments));
				return result.ToString ();
			}
		}

		protected Task<int> StartAsync ()
		{
			return Task.Run (() => Start ());
		}

		// both stdout and stderr will be sent to this function.
		// null will be sent when there's no more output
		// calls to this function will be synchronized (no need to lock in here).
		protected virtual void OutputReceived (string line)
		{
			if (line is not null)
				Output.AppendLine (line);
		}

		protected int Start ()
		{
			if (Driver.Verbosity > 0)
				Console.WriteLine (Command);

			Output = new StringBuilder ();

			var rv = Execution.RunWithCallbacksAsync (FileName, Arguments, environment: Environment, standardOutput: OutputReceived, standardError: OutputReceived).Result;

			OutputReceived (null);

			return rv.ExitCode;
		}
	}

	class GenerateMainTask : BuildTask {
		public Target Target;
		public Abi Abi;
		public string MainM;
		public IList<string> RegistrationMethods;

		public override IEnumerable<string> Inputs {
			get {
				foreach (var asm in Target.Assemblies)
					yield return asm.FullPath;
			}
		}

		public override IEnumerable<string> Outputs {
			get {
				yield return MainM;
			}
		}

		protected override void Execute ()
		{
			Target.GenerateMain (Target.App.Platform, Abi, MainM, RegistrationMethods);
		}
	}

	class CompileMainTask : CompileTask {
		protected override void CompilationFailed (int exitCode)
		{
			throw ErrorHelper.CreateError (5103, Errors.MT5103_A, string.Join ("', '", CompilerFlags.SourceFiles.ToArray ()));
		}
	}

	class PinvokesTask : CompileTask {
		protected override void CompilationFailed (int exitCode)
		{
			throw ErrorHelper.CreateError (4002, Errors.MT4002);
		}
	}

	class RunRegistrarTask : BuildTask {
		public Target Target;
		public string RegistrarCodePath;
		public string RegistrarHeaderPath;
		public List<string> RegistrationMethods;

		public override IEnumerable<string> Inputs {
			get {
				foreach (var asm in Target.Assemblies)
					yield return asm.FullPath;
			}
		}

		public override IEnumerable<string> Outputs {
			get {
				yield return RegistrarHeaderPath;
				yield return RegistrarCodePath;
			}
		}

		protected override void Execute ()
		{
			var assemblies = Target.Assemblies.Select ((a) => a.AssemblyDefinition);
			Target.StaticRegistrar.Generate (assemblies, RegistrarHeaderPath, RegistrarCodePath, out var initialization_name);
			RegistrationMethods.Add (initialization_name);
		}
	}

	class CompileRegistrarTask : CompileTask {
		public string RegistrarCodePath;
		public string RegistrarHeaderPath;

		public override IEnumerable<string> Inputs {
			get {
				yield return RegistrarHeaderPath;
				yield return RegistrarCodePath;
			}
		}

		protected override void CompilationFailed (int exitCode)
		{
			throw ErrorHelper.CreateError (4109, "Failed to compile the generated registrar code. Please file a bug report at https://github.com/xamarin/xamarin-macios/issues/new");
		}
	}

	public class AOTTask : ProcessTask {
		public Assembly Assembly;
		public string AssemblyName;
		public bool AddBitcodeMarkerSection;
		public string AssemblyPath; // path to the .s file.
		List<string> inputs;
		public AotInfo AotInfo;
		List<Exception> exceptions = new List<Exception> ();
		List<string> output_lines = new List<string> ();

		public override IEnumerable<string> Outputs {
			get {
				return AotInfo.AotDataFiles
							  .Union (AotInfo.AsmFiles)
							  .Union (AotInfo.BitcodeFiles)
							  .Union (AotInfo.ObjectFiles);
			}
		}

		public override IEnumerable<string> Inputs {
			get {
				yield return Assembly.FullPath;
			}
		}

		public override IEnumerable<string> FileDependencies {
			get {
				if (inputs is null) {
					inputs = new List<string> ();
					if (Assembly.HasDependencyMap)
						inputs.AddRange (Assembly.DependencyMap);
					inputs.Add (AssemblyName);
					foreach (var abi in Assembly.Target.Abis)
						inputs.Add (Driver.GetAotCompiler (Assembly.App, abi, Assembly.Target.Is64Build));
					var mdb = Assembly.FullPath + ".mdb";
					if (File.Exists (mdb))
						inputs.Add (mdb);
					var pdb = Path.ChangeExtension (Assembly.FullPath, ".pdb");
					if (File.Exists (pdb))
						inputs.Add (pdb);
					var config = Assembly.FullPath + ".config";
					if (File.Exists (config))
						inputs.Add (config);
				}
				return inputs;
			}
		}

		public override bool IsUptodate {
			get {
				// We can only check dependencies if we know the assemblies this assembly depend on (otherwise always rebuild).
				return Assembly.HasDependencyMap && base.IsUptodate;
			}
		}

		protected override void OutputReceived (string line)
		{
			if (line is null)
				return;

			if (line.StartsWith ("AOT restriction: Method '", StringComparison.Ordinal) && line.Contains ("must be static since it is decorated with [MonoPInvokeCallback]")) {
				exceptions.Add (ErrorHelper.CreateError (3002, line));
			} else {
				CheckFor5107 (AssemblyName, line, exceptions);
			}
			output_lines.Add (line);
		}

		protected async override Task ExecuteAsync ()
		{
			var exit_code = await StartAsync ();

			if (exit_code == 0) {
				if (AddBitcodeMarkerSection)
					File.AppendAllText (AssemblyPath, @"
.section __LLVM, __bitcode
.byte 0
.section __LLVM, __cmdline
.byte 0
");
				return;
			}

			WriteLimitedOutput ($"AOT Compilation exited with code {exit_code}, command:\n{Command}", output_lines, exceptions);

			exceptions.Add (ErrorHelper.CreateError (3001, Errors.MX3001, "AOT", AssemblyName));

			throw new AggregateException (exceptions);
		}

		public override string ToString ()
		{
			return Path.GetFileName (AssemblyName);
		}
	}

	public class NativeLinkTask : BuildTask {
		public Target Target;
		public string OutputFile;
		public CompilerFlags CompilerFlags;

		public override IEnumerable<string> Inputs {
			get {
				CompilerFlags.PopulateInputs ();
				return CompilerFlags.Inputs;
			}
		}

		public override IEnumerable<string> Outputs {
			get {
				yield return OutputFile;
			}
		}

		protected override async Task ExecuteAsync ()
		{
			// always show the native linker warnings since many of them turn out to be very important
			// and very hard to diagnose otherwise when hidden from the build output. Ref: bug #2430
			var linker_errors = new List<Exception> ();
			var output = new StringBuilder ();
			var cmd_length = Target.App.CompilerPath.Length + 1 + CompilerFlags.ToString ().Length;

			try {
				var code = await Driver.RunCommandAsync (Target.App.CompilerPath, CompilerFlags.ToArray (), output: output, suppressPrintOnErrors: true);

				Application.ProcessNativeLinkerOutput (Target, output.ToString (), CompilerFlags.AllLibraries, linker_errors, code != 0);

				if (code != 0) {
					Console.WriteLine ($"Process exited with code {code}, command:\n{Target.App.CompilerPath} {CompilerFlags.ToString ()}\n{output} ");
					// if the build failed - it could be because of missing frameworks / libraries we identified earlier
					foreach (var assembly in Target.Assemblies) {
						if (assembly.UnresolvedModuleReferences is null)
							continue;

						foreach (var mr in assembly.UnresolvedModuleReferences) {
							// TODO: add more diagnose information on the warnings
							var name = Path.GetFileNameWithoutExtension (mr.Name);
							linker_errors.Add (new ProductException (5215, false, Errors.MT5215, name));
						}
					}
					// mtouch does not validate extra parameters given to GCC when linking (--gcc_flags)
					if (Target.App.CustomLinkFlags?.Count > 0)
						linker_errors.Add (new ProductException (5201, true, Errors.MT5201, StringUtils.FormatArguments (Target.App.CustomLinkFlags)));
					else
						linker_errors.Add (new ProductException (5202, true, Errors.MT5202));

					if (code == 255) {
						// check command length
						// getconf ARG_MAX
						StringBuilder getconf_output = new StringBuilder ();
						if (Driver.RunCommand ("getconf", new [] { "ARG_MAX" }, output: getconf_output, suppressPrintOnErrors: true) == 0) {
							int arg_max;
							if (int.TryParse (getconf_output.ToString ().Trim (' ', '\t', '\n', '\r'), out arg_max)) {
								if (cmd_length > arg_max) {
									linker_errors.Add (ErrorHelper.CreateError (5217, Errors.MT5217, cmd_length));
								} else {
									Driver.Log (3, $"Linker failure is probably not due to command-line length (actual: {cmd_length} limit: {arg_max}");
								}
							} else {
								Driver.Log (3, "Failed to parse 'getconf ARG_MAX' output: {0}", getconf_output);
							}
						} else {
							Driver.Log (3, "Failed to execute 'getconf ARG_MAX'\n{0}", getconf_output);
						}
					}
				}

				if (code == 0 && Driver.XcodeVersion.Major >= 14 && Target.App.BitCodeMode != BitCodeMode.None)
					Target.App.StripBitcode (OutputFile);
			} catch (System.ComponentModel.Win32Exception wex) {
				/* This means we failed to execute the linker, not that the linker itself returned with a failure */
				if (wex.NativeErrorCode == 7 /* E2BIG = Too many arguments */ ) {
					linker_errors.Add (ErrorHelper.CreateError (5217, wex, Errors.MT5217, cmd_length));
				} else {
					linker_errors.Add (ErrorHelper.CreateError (5222, wex, Errors.MX5222, wex.Message));
				}
			}

			ErrorHelper.Show (linker_errors);

			// the native linker can prefer private (and existing) over public (but non-existing) framework when weak_framework are used
			// on an iOS target version where the framework does not exists, e.g. targeting iOS6 for JavaScriptCore added in iOS7 results in
			// /System/Library/PrivateFrameworks/JavaScriptCore.framework/JavaScriptCore instead of
			// /System/Library/Frameworks/JavaScriptCore.framework/JavaScriptCore
			// more details in https://bugzilla.xamarin.com/show_bug.cgi?id=31036
			if (CompilerFlags.WeakFrameworks.Count > 0)
				Target.AdjustDylibs (OutputFile);
			Driver.Watch ("Native Link", 1);
		}

		public override string ToString ()
		{
			return Path.GetFileName (OutputFile);
		}
	}

	public class LinkTask : CompileTask {
		protected override async Task ExecuteAsync ()
		{
			await base.ExecuteAsync ();
			// we can't trust the native linker to pick the right (non private) framework when an older TargetVersion is used
			if (CompilerFlags.WeakFrameworks.Count > 0)
				Target.AdjustDylibs (OutputFile);
			// Remove bitcode from the binary
			if (Driver.XcodeVersion.Major >= 14 && App.BitCodeMode != BitCodeMode.None)
				App.StripBitcode (OutputFile);
		}

		protected override void CompilationFailed (int exitCode)
		{
			throw ErrorHelper.CreateError (5216, Errors.MT5216, OutputFile);
		}
	}

	public class CompileTask : BuildTask {
		public Target Target;
		public Application App { get { return Target.App; } }
		public bool SharedLibrary;
		public string OutputFile;
		public Abi Abi;
		public string InstallName;
		public string Language;

		public override IEnumerable<string> Inputs {
			get {
				CompilerFlags.PopulateInputs ();
				return CompilerFlags.Inputs;
			}
		}

		public override IEnumerable<string> Outputs {
			get {
				yield return OutputFile;
			}
		}

		public bool IsAssembler {
			get {
				return Language == "assembler";
			}
		}

		public string InputFile {
			set {
				// This is an accumulative setter-only property,
				// to make it possible add dependencies using object initializers.
				CompilerFlags.AddSourceFile (value);
			}
		}

		CompilerFlags compiler_flags;
		public CompilerFlags CompilerFlags {
			get { return compiler_flags ?? (compiler_flags = new CompilerFlags (Target)); }
			set { compiler_flags = value; }
		}

		public static void GetArchFlags (CompilerFlags flags, params Abi [] abis)
		{
			GetArchFlags (flags, (IEnumerable<Abi>) abis);
		}

		public static void GetArchFlags (CompilerFlags flags, IEnumerable<Abi> abis)
		{
			bool enable_thumb = false;

			foreach (var abi in abis) {
				var arch = abi.AsArchString ();
				flags.AddOtherFlag ($"-arch", arch);

				enable_thumb |= (abi & Abi.Thumb) != 0;
			}

			if (enable_thumb)
				flags.AddOtherFlag ("-mthumb");
		}

		public static void GetCompilerFlags (Application app, CompilerFlags flags, bool is_assembler, string language = null)
		{
			if (!is_assembler)
				flags.AddOtherFlag ("-gdwarf-2");

			if (!is_assembler) {
				if (language != "objective-c") {
					// error: invalid argument '-std=c++14' not allowed with 'Objective-C'
					flags.AddOtherFlag ("-std=c++14");
				}

				flags.AddOtherFlag ($"-I{Driver.GetProductSdkIncludeDirectory (app)}");
			}
			flags.AddOtherFlag ($"-isysroot", Driver.GetFrameworkDirectory (app));
			flags.AddOtherFlag ("-Qunused-arguments"); // don't complain about unused arguments (clang reports -std=c99 and -Isomething as unused).
		}

		public static void GetCatalystCompilerFlags (CompilerFlags flags, Abi abi, Application app)
		{
			GetCompilerFlags (app, flags, false);
			flags.AddOtherFlag ($"-target", $"{abi.AsArchString ()}-apple-ios{app.DeploymentTarget}-macabi");
			var isysroot = Driver.GetFrameworkDirectory (app);
			flags.AddOtherFlag ($"-isystem", Path.Combine (isysroot, "System", "iOSSupport", "usr", "include"));
			flags.AddOtherFlag ($"-iframework", Path.Combine (isysroot, "System", "iOSSupport", "System", "Library", "Frameworks"));
			flags.AddOtherFlag ($"-L{Path.Combine (isysroot, "System", "iOSSupport", "usr", "lib")}");
		}

		public static void GetSimulatorCompilerFlags (CompilerFlags flags, bool is_assembler, Application app, string language = null)
		{
			GetCompilerFlags (app, flags, is_assembler, language);

			string sim_platform = Driver.GetPlatformDirectory (app);
			string plist = Path.Combine (sim_platform, "Info.plist");

			var dict = Driver.FromPList (plist);
			var dp = dict.Get<PDictionary> ("DefaultProperties");
			if (dp.GetString ("GCC_OBJC_LEGACY_DISPATCH") == "YES")
				flags.AddOtherFlag ("-fobjc-legacy-dispatch");
			string objc_abi = dp.GetString ("OBJC_ABI_VERSION");
			if (!String.IsNullOrWhiteSpace (objc_abi))
				flags.AddOtherFlag ($"-fobjc-abi-version={objc_abi}");

			plist = Path.Combine (Driver.GetFrameworkDirectory (app), "SDKSettings.plist");
			string min_prefix = app.CompilerPath.Contains ("clang") ? Driver.GetTargetMinSdkName (app) : "iphoneos";
			dict = Driver.FromPList (plist);
			dp = dict.Get<PDictionary> ("DefaultProperties");
			if (app.DeploymentTarget == new Version ()) {
				string target = dp.GetString ("IPHONEOS_DEPLOYMENT_TARGET");
				if (!String.IsNullOrWhiteSpace (target))
					flags.AddOtherFlag ($"-m{min_prefix}-version-min={target}");
			} else {
				flags.AddOtherFlag ($"-m{min_prefix}-version-min={app.DeploymentTarget}");
			}
			string defines = dp.GetString ("GCC_PRODUCT_TYPE_PREPROCESSOR_DEFINITIONS");
			if (!String.IsNullOrWhiteSpace (defines))
				flags.AddDefine (defines.Replace (" ", String.Empty));
		}

		void GetDeviceCompilerFlags (CompilerFlags flags, bool is_assembler)
		{
			GetCompilerFlags (App, flags, is_assembler, Language);

			flags.AddOtherFlag ($"-m{Driver.GetTargetMinSdkName (App)}-version-min={App.DeploymentTarget.ToString ()}");
		}

		void GetSharedCompilerFlags (CompilerFlags flags, string install_name)
		{
			if (string.IsNullOrEmpty (install_name))
				throw new ArgumentNullException (nameof (install_name));

			flags.AddOtherFlag ("-shared");
			if (!App.EnableBitCode && !Target.Is64Build)
				flags.AddOtherFlag ("-read_only_relocs", "suppress");
			if (App.EnableBitCode)
				flags.AddOtherFlag ("-lc++");
			flags.LinkWithMono ();
			flags.AddOtherFlag ("-install_name", install_name);
			flags.AddOtherFlag ("-fapplication-extension"); // fixes this: warning MT5203: Native linking warning: warning: linking against dylib not safe for use in application extensions: [..]/actionextension.dll.arm64.dylib
		}

		void GetStaticCompilerFlags (CompilerFlags flags)
		{
			flags.AddOtherFlag ("-c");
		}

		void GetBitcodeCompilerFlags (CompilerFlags flags)
		{
			flags.AddOtherFlag (App.EnableMarkerOnlyBitCode ? "-fembed-bitcode-marker" : "-fembed-bitcode");
		}

		protected override async Task ExecuteAsync ()
		{
			int exitCode = await CompileAsync ();
			if (exitCode != 0)
				CompilationFailed (exitCode);
		}

		protected virtual void CompilationFailed (int exitCode)
		{
			throw ErrorHelper.CreateError (5106, Errors.MT5106, string.Join ("', '", CompilerFlags.SourceFiles.ToArray ()));
		}

		protected async Task<int> CompileAsync ()
		{
			if (App.IsDeviceBuild) {
				GetDeviceCompilerFlags (CompilerFlags, IsAssembler);
			} else if (App.Platform == ApplePlatform.MacCatalyst) {
				GetCatalystCompilerFlags (CompilerFlags, Abi, App);
			} else {
				GetSimulatorCompilerFlags (CompilerFlags, IsAssembler, App, Language);
			}

			if (App.EnableBitCode)
				GetBitcodeCompilerFlags (CompilerFlags);
			GetArchFlags (CompilerFlags, Abi);

			if (SharedLibrary) {
				GetSharedCompilerFlags (CompilerFlags, InstallName);
			} else {
				GetStaticCompilerFlags (CompilerFlags);
			}

			if (App.EnableDebug)
				CompilerFlags.AddDefine ("DEBUG");

			CompilerFlags.AddOtherFlag ("-o", OutputFile);

			if (!string.IsNullOrEmpty (Language))
				CompilerFlags.AddOtherFlag ("-x", Language);

			Directory.CreateDirectory (Path.GetDirectoryName (OutputFile));

			var exceptions = new List<Exception> ();
			var output = new List<string> ();
			var assembly_name = Path.GetFileNameWithoutExtension (OutputFile);
			var output_received = new Action<string> ((string line) => {
				if (line is null)
					return;
				output.Add (line);
				CheckFor5107 (assembly_name, line, exceptions);
			});

			var rv = await Driver.RunCommandAsync (App.CompilerPath, CompilerFlags.ToArray (), output_received: output_received, suppressPrintOnErrors: true);

			WriteLimitedOutput (rv != 0 ? $"Compilation failed with code {rv}, command:\n{App.CompilerPath} {CompilerFlags.ToString ()}" : null, output, exceptions);

			ErrorHelper.Show (exceptions);

			return rv;
		}

		public override string ToString ()
		{
			if (compiler_flags is null || compiler_flags.SourceFiles is null)
				return Path.GetFileName (OutputFile);
			return string.Join (", ", compiler_flags.SourceFiles.Select ((arg) => Path.GetFileName (arg)).ToArray ());
		}
	}

	public class BitCodeifyTask : BuildTask {
		public string Input { get; set; }
		public string OutputFile { get; set; }
		public ApplePlatform Platform { get; set; }
		public Abi Abi { get; set; }
		public Version DeploymentTarget { get; set; }

		public override IEnumerable<string> Inputs {
			get {
				yield return Input;
			}
		}

		public override IEnumerable<string> Outputs {
			get {
				yield return OutputFile;
			}
		}

		protected override void Execute ()
		{
			new BitcodeConverter (Input, OutputFile, Platform, Abi, DeploymentTarget).Convert ();
		}

		public override string ToString ()
		{
			return Path.GetFileName (Input);
		}
	}

	public class LipoTask : BuildTask {
		public Application App;
		public IEnumerable<string> InputFiles { get; set; }
		public string OutputFile { get; set; }

		public override IEnumerable<string> Inputs {
			get {
				return InputFiles;
			}
		}

		public override IEnumerable<string> Outputs {
			get {
				yield return OutputFile;
			}
		}

		protected override void Execute ()
		{
			Application.Lipo (App, OutputFile, InputFiles.ToArray ());
		}

		public override string ToString ()
		{
			return Path.GetFileName (string.Join (",", InputFiles));
		}
	}


	public class FileCopyTask : BuildTask {
		public string InputFile { get; set; }
		public string OutputFile { get; set; }

		public override IEnumerable<string> Inputs {
			get {
				yield return InputFile;
			}
		}

		public override IEnumerable<string> Outputs {
			get {
				yield return OutputFile;
			}
		}

		protected override void Execute ()
		{
			Application.UpdateFile (InputFile, OutputFile);
		}

		public override string ToString ()
		{
			return $"cp {InputFile} {OutputFile}";
		}
	}
}
